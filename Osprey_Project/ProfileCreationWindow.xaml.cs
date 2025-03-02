using System;
using System.Windows;
using MongoDB.Bson;
using MongoDB.Driver;
using BCrypt.Net;

namespace Osprey_Project
{
    public partial class ProfileCreationWindow : Window
    {
        private static readonly string ConnectionString = "mongodb://localhost:27017"; // MongoDB Local Connection
        private static readonly string DatabaseName = "OspreyDB"; // Your database name
        private static readonly string CollectionName = "users"; // Collection name

        private readonly IMongoCollection<UserProfile> _collection;

        public ProfileCreationWindow()
        {
            InitializeComponent();

            try
            {
                var client = new MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                _collection = database.GetCollection<UserProfile>(CollectionName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to MongoDB: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Check if the username already exists
                var existingUser = _collection.Find(u => u.Username == txtUsername.Text).FirstOrDefault();

                if (existingUser != null)
                {
                    MessageBox.Show("Username already exists. Please choose a different one.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Hash the password before storing it
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(txtPassword.Password);

                var newUser = new UserProfile
                {
                    Id = ObjectId.GenerateNewId().ToString(), // Generate a new MongoDB ID
                    Name = txtName.Text,
                    Username = txtUsername.Text,
                    Password = hashedPassword, // Store the hashed password
                    DOB = txtDOB.Text,
                    Address = txtAddress.Text
                };

                // Insert the new user profile into MongoDB
                _collection.InsertOne(newUser);

                MessageBox.Show("Profile created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();

                // Optionally navigate back to the main window
                MainWindow obj = new MainWindow();
                obj.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving profile: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow obj = new MainWindow();
            this.Close();
            obj.Show();
        }
    }
}
