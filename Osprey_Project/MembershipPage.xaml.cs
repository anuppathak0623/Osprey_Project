using System;
using System.Windows;
using System.Windows.Controls;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Osprey_Project
{
    public partial class MembershipPage : Page
    {
        private static readonly string ConnectionString = "mongodb://localhost:27017";
        private static readonly string DatabaseName = "OspreyDB";
        private static readonly string CollectionName = "Members";

        private readonly IMongoCollection<Membership> _collection;
        private string selectedOption_Category = "";
        private string selectedOption_Plan = "";

        public MembershipPage()
        {
            InitializeComponent();

            try
            {
                var client = new MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                _collection = database.GetCollection<Membership>(CollectionName);

                // Auto-fill Full Name
                NameTextBox.Text = UserManager.FullName;
                NameTextBox.Foreground = System.Windows.Media.Brushes.Black; // Ensure the text is visible
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to MongoDB: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Athletes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AthletesPage());
        }

        private void Entertainers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new EntertainersPage());
        }

        private void Executives_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ExecutivesPage());
        }

        private void College_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CollegePage());
        }

        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Category.SelectedItem is ComboBoxItem selectedItem)
            {
                selectedOption_Category = selectedItem.Content.ToString();
            }
        }

        private void Plan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Plan.SelectedItem is ComboBoxItem selectedItem)
            {
                selectedOption_Plan = selectedItem.Content.ToString();
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newUser = new Membership
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = NameTextBox.Text,
                    Email = EmailTextBox.Text,
                    Category = selectedOption_Category,
                    Plan = selectedOption_Plan
                };

                _collection.InsertOne(newUser);
                MessageBox.Show("You will get an Email for Payment after 15 days of Trial", "Applied successfully!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving application: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
