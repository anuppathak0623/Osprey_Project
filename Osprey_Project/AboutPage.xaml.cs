using System;
using System.Windows;
using System.Windows.Controls;
using MongoDB.Driver;

namespace Osprey_Project
{
    public partial class AboutPage : Page
    {
        private static readonly string ConnectionString = "mongodb://localhost:27017";
        private static readonly string DatabaseName = "OspreyDB";
        private static readonly string CollectionName = "Comments";

        private readonly IMongoCollection<Comments> _collection;
        private string loggedInUser; // Store username internally

        public AboutPage()
        {
            InitializeComponent();

            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase(DatabaseName);
            _collection = database.GetCollection<Comments>(CollectionName);

            // Retrieve the logged-in username internally
            if (!string.IsNullOrEmpty(UserManager.CurrentUser))
            {
                loggedInUser = UserManager.CurrentUser;
            }
            else
            {
                loggedInUser = "Anonymous"; // Default if no user is logged in
            }
        }

        private void SubmitComment_Click(object sender, RoutedEventArgs e)
        {
            string comment = CommentBox.Text.Trim();

            if (string.IsNullOrEmpty(comment))
            {
                MessageBox.Show("Please enter a comment.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string combined = loggedInUser + " : " + comment;

            StackPanel commentPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 0, 0, 20)
            };

            Label newLabel = new Label { Content = combined };
            commentPanel.Children.Add(newLabel);
            CommentsSection.Children.Add(commentPanel);

            CommentBox.Clear();

            try
            {
                var newComment = new Comments
                {
                    FullName = loggedInUser,
                    CommentText = comment
                };

                _collection.InsertOne(newComment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving comment to MongoDB: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            MessageBox.Show("Your comment has been submitted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CommentBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
