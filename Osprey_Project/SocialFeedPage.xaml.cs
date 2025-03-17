using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MongoDB.Driver;
using static Osprey_Project.SocialFeedPage;

namespace Osprey_Project
{
    public static class MongoDbHelper
    {
        private static readonly string ConnectionString = "mongodb://localhost:27017"; // Update with your connection string
        private static readonly string DatabaseName = "OspreyDB";
        private static readonly string CollectionName = "Posts";

        private static IMongoDatabase Database => new MongoClient(ConnectionString).GetDatabase(DatabaseName);
        private static IMongoCollection<Post> Collection => Database.GetCollection<Post>(CollectionName);

        public static void InsertPost(Post post)
        {
            Collection.InsertOne(post);
        }

        public static List<Post> GetAllPosts()
        {
            return Collection.Find(_ => true).ToList();
        }

        public static void UpdateComments(string postId, Comment newComment)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var update = Builders<Post>.Update.Push(p => p.Comments, newComment);
            Collection.UpdateOne(filter, update);
        }
     
    }
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return string.IsNullOrEmpty(stringValue) ? Visibility.Collapsed : Visibility.Visible;
            }
            else if (value is BitmapImage bitmapImage)
            {
                return bitmapImage == null ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class SocialFeedPage : Page
    {
        // In-memory data storage
        private readonly List<Post> _posts = new List<Post>();
        private readonly List<User> _users = new List<User>();
        private readonly List<User> _following = new List<User>(); // Users the current user is following
        private readonly List<User> _followers = new List<User>(); // Users following the current user
        private readonly HashSet<string> _likedPosts = new HashSet<string>(); // Track liked posts
        private readonly Dictionary<string, List<ChatMessage>> _chats = new Dictionary<string, List<ChatMessage>>(); // Store chat messages
        private readonly List<string> _notifications = new List<string>(); // List of notifications

        public SocialFeedPage()
        {
            InitializeComponent();

            // Load posts from MongoDB
            _posts.AddRange(MongoDbHelper.GetAllPosts());

            // Bind posts to the ListView
            FeedListView.ItemsSource = _posts;

            // Add default users
            _users.Add(new User { UserId = "1", UserName = "Alice" });
            _users.Add(new User { UserId = "2", UserName = "Bob" });
            _users.Add(new User { UserId = "3", UserName = "Charlie" });

            // Add default posts
            _posts.Add(new Post
            {
                Id = "1",
                UserId = "1",
                UserName = "Alice",
                Content = "Welcome to Osprey! Let's build an amazing community together."
            });

            _posts.Add(new Post
            {
                Id = "2",
                UserId = "2",
                UserName = "Bob",
                Content = "Excited to be part of this project! 🚀"
            });

            _posts.Add(new Post
            {
                Id = "3",
                UserId = "3",
                UserName = "Charlie",
                Content = "Let's connect and share ideas! 💡"
            });

            // Bind posts to the ListView
            FeedListView.ItemsSource = _posts;

            // Bind users to the UserListView
            UserListView.ItemsSource = _users;

            // Bind following and followers to their ListViews
            FollowingListView.ItemsSource = _following;
            FollowersListView.ItemsSource = _followers;

            // Attach the TextChanged event handler for the PostTextBox
            PostTextBox.TextChanged += PostTextBox_TextChanged;

        }



        // Event handler for PostTextBox text changes
        private void PostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Show/hide the placeholder text based on whether the TextBox is empty
            PlaceholderTextBlock.Visibility = string.IsNullOrEmpty(PostTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Event handler for Add Image button click
        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select an image
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                // Set the image URL for the post
                PostTextBox.Text += $" [Image: {dialog.FileName}]";
            }
        }

        // Event handler for Follow button click
        // Event handler for Follow button click
        // Event handler for Follow button click
        private void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string userId)
            {
                var userToFollow = _users.FirstOrDefault(u => u.UserId == userId);
                if (userToFollow != null)
                {
                    if (!userToFollow.IsFollowing)
                    {
                        // Add the user to the current user's "Following" list
                        _following.Add(userToFollow);
                        userToFollow.IsFollowing = true; // Mark as followed

                        // Update the button text to "Following"
                        button.Content = "Following";
                        button.IsEnabled = false; // Disable the button

                        // Refresh the Following ListView
                        FollowingListView.ItemsSource = null;
                        FollowingListView.ItemsSource = _following;

                        // Show a confirmation message
                        MessageBox.Show($"You are now following {userToFollow.UserName}!", "Follow", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }



        // Event handler for Post button click
        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PostTextBox.Text))
            {
                MessageBox.Show("Please enter some content for your post.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string imagePath = string.Empty;
            if (PostTextBox.Text.Contains("[Image:"))
            {
                int startIndex = PostTextBox.Text.IndexOf("[Image:") + 7;
                int endIndex = PostTextBox.Text.IndexOf("]");
                imagePath = PostTextBox.Text.Substring(startIndex, endIndex - startIndex).Trim();
            }

            var post = new Post
            {
                UserId = UserManager.CurrentUser,
                UserName = UserManager.FullName,
                Content = PostTextBox.Text.Replace($"[Image: {imagePath}]", "").Trim(),
                ImageUrl = ! string.IsNullOrEmpty(imagePath) ? LoadImage(imagePath): null,
                LikeCount = 0,
                Comments = new List<Comment>() // Initialize the comment list
            };

            // Add to local list and MongoDB
            _posts.Add(post);
            MongoDbHelper.InsertPost(post);

            // Refresh the feed
            FeedListView.ItemsSource = null;
            FeedListView.ItemsSource = _posts;

            // Clear the input
            PostTextBox.Clear();
        }




        // Helper method to load an image from a file path or URL
        private BitmapImage LoadImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            // Check if the imagePath is a URL or a local file path
            if (Uri.TryCreate(imagePath, UriKind.Absolute, out Uri uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                // Load image from URL
                bitmapImage.UriSource = uri;
            }
            else if (System.IO.File.Exists(imagePath))
            {
                // Load image from local file path
                bitmapImage.UriSource = new Uri(imagePath, UriKind.Absolute);
            }
            else
            {
                return null; // Invalid path or URL
            }

            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        // Event handler for Like button click
        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string postId)
            {
                var post = _posts.FirstOrDefault(p => p.Id == postId);
                if (post != null && !_likedPosts.Contains(postId))
                {
                    post.LikeCount++;
                    _likedPosts.Add(postId); // Track the liked post
                    FeedListView.ItemsSource = null;
                    FeedListView.ItemsSource = _posts;
                }
            }
        }

        // Event handler for Comment button click
        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string postId)
            {
                var post = _posts.FirstOrDefault(p => p.Id == postId);
                if (post != null)
                {
                    var commentInput = new InputDialog("Add a comment:");
                    if (commentInput.ShowDialog() == true)
                    {
                        var newComment = new Comment
                        {
                            Commenter = UserManager.FullName, // Use the logged-in user's full name
                            Text = commentInput.Answer
                        };

                        post.Comments.Add(newComment);

                        // ✅ Update the comment in the database
                        MongoDbHelper.UpdateComments(postId, newComment);

                        // ✅ Refresh the feed
                        FeedListView.ItemsSource = null;
                        FeedListView.ItemsSource = _posts;
                    }
                }
            }
        }



        // Event handler for Share button click
        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string postId)
            {
                var post = _posts.FirstOrDefault(p => p.Id == postId);
                if (post != null)
                {
                    MessageBox.Show($"You shared the post by {post.UserName}!", "Share", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Event handler for ChatTextBox text changes
        private void ChatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle chat text changes (if needed)
        }

        // Navbar Button Click Handlers
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            HomeSection.Visibility = Visibility.Visible;
            FeedListView.Visibility = Visibility.Visible;
            MessageSection.Visibility = Visibility.Collapsed;
            FriendsSection.Visibility = Visibility.Collapsed;
            NotificationSection.Visibility = Visibility.Collapsed;
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            HomeSection.Visibility = Visibility.Collapsed;
            FeedListView.Visibility = Visibility.Collapsed;
            MessageSection.Visibility = Visibility.Visible;
            FriendsSection.Visibility = Visibility.Collapsed;
            NotificationSection.Visibility = Visibility.Collapsed;
        }

        // Event handler for FriendsButton click
        private void FriendsButton_Click(object sender, RoutedEventArgs e)
        {
            HomeSection.Visibility = Visibility.Collapsed;
            FeedListView.Visibility = Visibility.Collapsed;
            MessageSection.Visibility = Visibility.Collapsed;
            FriendsSection.Visibility = Visibility.Visible;
            NotificationSection.Visibility = Visibility.Collapsed;

            // Refresh the following and followers lists
            FollowingListView.ItemsSource = null;
            FollowingListView.ItemsSource = _following;

            FollowersListView.ItemsSource = null;
            FollowersListView.ItemsSource = _followers;
        }

        

        // Chat Button Click Handlers
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string userId)
            {
                ChatPanel.Visibility = Visibility.Visible;
                ChatListView.ItemsSource = _chats.ContainsKey(userId) ? _chats[userId] : new List<ChatMessage>();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ChatTextBox.Text))
            {
                var userId = "DefaultUser"; // Replace with the default user ID
                if (!_chats.ContainsKey(userId))
                {
                    _chats[userId] = new List<ChatMessage>();
                }

                _chats[userId].Add(new ChatMessage
                {
                    Sender = "CurrentUser",
                    Message = ChatTextBox.Text
                });

                ChatListView.ItemsSource = null;
                ChatListView.ItemsSource = _chats[userId];
                ChatTextBox.Clear();

                // Show confirmation message
                MessageBox.Show("Your message has been sent!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Models
        public class Post
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string UserId { get; set; } = "DefaultUser";
            public string UserName { get; set; } = "DefaultUser";
            public string Content { get; set; } = string.Empty;
            public BitmapImage ImageUrl { get; set; }
            public string VideoUrl { get; set; } = string.Empty;
            public int LikeCount { get; set; } = 0;
            public List<Comment> Comments { get; set; } = new List<Comment>();
        }

        public class User
        {
            public string UserId { get; set; } = Guid.NewGuid().ToString();
            public string UserName { get; set; } = "DefaultUser";
            public bool IsFollowing { get; set; } = false; // Track follow state
        }

        public class FriendRequest
        {
            public string FromUserId { get; set; } = "DefaultUser";
            public string FromUserName { get; set; } = "DefaultUser";
            public string ToUserId { get; set; } = "DefaultUser";
        }

        public class Comment
        {
            public string Commenter { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
        }

        public class ChatMessage
        {
            public string Sender { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }
    }

    // InputDialog for adding comments
    public class InputDialog : Window
    {
        public string Answer { get; private set; }

        public InputDialog(string prompt)
        {
            Title = "Comment";
            Width = 300;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var promptTextBlock = new TextBlock { Text = prompt, Margin = new Thickness(10) };
            var inputTextBox = new TextBox { Margin = new Thickness(10) };
            var okButton = new Button { Content = "OK", IsDefault = true, Margin = new Thickness(10) };

            okButton.Click += (sender, e) =>
            {
                Answer = inputTextBox.Text;
                DialogResult = true;
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(promptTextBlock);
            stackPanel.Children.Add(inputTextBox);
            stackPanel.Children.Add(okButton);

            Content = stackPanel;
        }
    }
}