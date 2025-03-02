using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Osprey_Project
{
    public partial class SettingsPage : Page
    {
        private static readonly string ConnectionString = "mongodb://localhost:27017"; // MongoDB local connection
        private static readonly string DatabaseName = "OspreyDB"; //  MongoDB database name
        private static readonly string CollectionName = "users"; // Collection name for storing user profiles

        private readonly IMongoCollection<UserProfile> _collection;

        public SettingsPage()
        {
            InitializeComponent();

            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase(DatabaseName);
            _collection = database.GetCollection<UserProfile>(CollectionName);

            // Debugging: Check if currentUser is set correctly
            MessageBox.Show($"Current logged-in user: {UserManager.CurrentUser}", "Settings Page", MessageBoxButton.OK, MessageBoxImage.Information);

            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                // Debugging: Ensure the currentUser value is correct
                if (string.IsNullOrEmpty(UserManager.CurrentUser))
                {
                    MessageBox.Show("No logged-in user found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Find the user profile for the current logged-in user
                var userProfile = _collection.Find(u => u.Username == UserManager.CurrentUser).FirstOrDefault();

                if (userProfile != null)
                {
                    UsernameBox.Text = userProfile.Username;
                    PasswordBox.Password = userProfile.Password;
                    NameBox.Text = userProfile.Name;
                    DOB.Text= userProfile.DOB;
                    AddressBox.Text = userProfile.Address;
                }
                else
                {
                    MessageBox.Show("User profile not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hash the new password before saving it
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password);

                // Create an updated profile with the new data
                var updatedProfile = new UserProfile
                {
                    Username = UsernameBox.Text,
                    Password = hashedPassword, // Use the hashed password
                    Name = NameBox.Text,
                    DOB = DOB.Text,
                    Address = AddressBox.Text
                };

                var existingProfile = _collection.Find(u => u.Username == UserManager.CurrentUser).FirstOrDefault();

                if (existingProfile != null)
                {
                    // Update existing profile in MongoDB
                    existingProfile.Username = UsernameBox.Text;
                    existingProfile.Password = hashedPassword; // Store the hashed password
                    existingProfile.Name = NameBox.Text;
                    existingProfile.DOB = DOB.Text;
                    existingProfile.Address = AddressBox.Text;

                    _collection.ReplaceOne(u => u.Username == UserManager.CurrentUser, existingProfile); // Save updated profile
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // If the profile doesn't exist, create a new one
                    _collection.InsertOne(updatedProfile);
                    MessageBox.Show("Profile created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        
        }
    }
}
