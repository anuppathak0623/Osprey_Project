using System;
using System.Windows;
using MongoDB.Driver;
using BCrypt.Net;

namespace Osprey_Project
{
    public partial class MainWindow : Window
    {
        private static readonly string ConnectionString = "mongodb://localhost:27017";
        private static readonly string DatabaseName = "OspreyDB";
        private static readonly string CollectionName = "users";

        private readonly IMongoCollection<UserProfile> _collection;

        public MainWindow()
        {
            InitializeComponent();
            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase(DatabaseName);
            _collection = database.GetCollection<UserProfile>(CollectionName);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _collection.Find(u => u.Username == txtUsername.Text).FirstOrDefault();

                if (user != null && !string.IsNullOrEmpty(user.Password))
                {
                    // Verify the hashed password using BCrypt
                    if (BCrypt.Net.BCrypt.Verify(txtPassword.Password, user.Password))
                    {
                        // Store the current logged-in user details
                        UserManager.CurrentUser = user.Username;
                        UserManager.FullName = user.Name; // Store full name

                        MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Open the Home page
                        Home obj = new Home();
                        this.Visibility = Visibility.Hidden;
                        obj.Show();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during login: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenProfileCreationWindow(object sender, RoutedEventArgs e)
        {
            ProfileCreationWindow obj = new ProfileCreationWindow();
            this.Visibility = Visibility.Hidden;
            obj.Show();
        }
    }
}
