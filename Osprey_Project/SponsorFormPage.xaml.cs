using System;
using System.Windows;
using System.Windows.Controls;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Osprey_Project
{
    public partial class SponsorFormPage : Page
    {
        private readonly IMongoCollection<BsonDocument> _sponsorCollection;

        public SponsorFormPage()
        {
            InitializeComponent();

            // Initialize MongoDB connection
            var client = new MongoClient("mongodb://localhost:27017"); // Change if needed
            var database = client.GetDatabase("OspreyDB"); // Your MongoDB database
            _sponsorCollection = database.GetCollection<BsonDocument>("Sponsors");
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            // Read inputs from the form
            string companyName = CompanyNameTextBox.Text.Trim();
            string contactPerson = ContactPersonTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string reason = ReasonTextBox.Text.Trim();
            string marketingMethod = (MarketingMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Validation
            if (string.IsNullOrEmpty(companyName) ||
                string.IsNullOrEmpty(contactPerson) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(reason) ||
                string.IsNullOrEmpty(marketingMethod))
            {
                MessageBox.Show("All fields are required. Please fill out the entire form.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create MongoDB document
            var sponsor = new BsonDocument
            {
                { "CompanyName", companyName },
                { "ContactPerson", contactPerson },
                { "Email", email },
                { "Phone", phone },
                { "Reason", reason },
                { "MarketingMethod", marketingMethod },
                { "CreatedAt", DateTime.UtcNow }
            };

            try
            {
                // Insert into MongoDB
                await _sponsorCollection.InsertOneAsync(sponsor);

                // Show success message
                MessageBox.Show("Your Request has been processed, We'll be in touch soon. ", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear fields after submission
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Back_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SponsorPage());
        }

        private void ClearForm()
        {
            CompanyNameTextBox.Text = string.Empty;
            ContactPersonTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            ReasonTextBox.Text = string.Empty;
            MarketingMethodComboBox.SelectedIndex = -1;
        }
    }
}
