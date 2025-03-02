using System.Windows;
using System.Windows.Controls;
using MongoDB.Driver;

namespace Osprey_Project
{
    public partial class ContactPage : Page
    {
        private static readonly string ConnectionString = "mongodb://localhost:27017";
        private static readonly string DatabaseName = "OspreyDB";
        private static readonly string CollectionName = "ContactMessages";

        private readonly IMongoCollection<Contact> _collection;

        public ContactPage()
        {
            InitializeComponent();

            // Initialize MongoDB client and collection
            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase(DatabaseName);
            _collection = database.GetCollection<Contact>(CollectionName);

            // Auto-fill Full Name field with the logged-in user's full name
            FullNameBox.Text = UserManager.FullName;
            FullNameBox.IsReadOnly = true; // Make it read-only
            FullNameBox.Foreground = System.Windows.Media.Brushes.Black; // Ensure visibility
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string message = MessageInput.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Please fill out all fields before submitting.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show($"Thank you, {fullName}!\nYour message has been received.\nWe will get back to you at {email}.",
            "Submitted", MessageBoxButton.OK, MessageBoxImage.Information);

            try
            {
                var newMessage = new Contact
                {
                    FullName = fullName,
                    Email = email,
                    Message = message
                };

                _collection.InsertOne(newMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving message to MongoDB: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            EmailBox.Clear();
            MessageInput.Clear();
        }
    }
}
