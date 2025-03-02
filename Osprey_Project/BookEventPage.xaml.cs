using System;
using System.Windows;
using System.Windows.Controls;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Osprey_Project
{
    public partial class BookEventPage : Page
    {
        private static readonly string ConnectionString = "mongodb://localhost:27017";
        private static readonly string DatabaseName = "OspreyDB";
        private static readonly string CollectionName = "EventsBooking";

        private readonly IMongoCollection<BookEvent> _collection;
        private string selectedEvent; // Stores the selected event

        public BookEventPage()
        {
            InitializeComponent();

            try
            {
                var client = new MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                _collection = database.GetCollection<BookEvent>(CollectionName);

                // Auto-fill Full Name (if UserManager is available)
                if (!string.IsNullOrWhiteSpace(UserManager.FullName))
                {
                    NameTextBox.Text = UserManager.FullName;
                    NameTextBox.Foreground = System.Windows.Media.Brushes.Black;
                }

                // Ensure default selection
                if (EventComboBox.Items.Count > 0)
                {
                    EventComboBox.SelectedIndex = 0; // Set first event as default
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to MongoDB: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EventComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                selectedEvent = selectedItem.Content.ToString(); // Store selected event
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = NameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            DateTime? selectedDate = EventDatePicker.SelectedDate; // Nullable DateTime

            // Ensure all fields are filled
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(selectedEvent) ||
                selectedDate == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newEvent = new BookEvent
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    FullName = fullName,
                    Email = email,
                    Event = selectedEvent,
                    Date = selectedDate.Value // Store selected date
                };

                _collection.InsertOne(newEvent);
                MessageBox.Show("Event booked successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear fields after booking
                NameTextBox.Clear();
                EmailTextBox.Clear();
                EventComboBox.SelectedIndex = 0;
                EventDatePicker.SelectedDate = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving event: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
