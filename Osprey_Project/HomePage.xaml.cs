using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Osprey_Project
{
    public partial class HomePage : Page
    {
        private int _currentImageIndex = 0;
        private readonly string[] _imageSources =
        {
"8.jpg", // Replace with actual image paths
"9.jpg",
"1.jpg",
"2.jpg",
"3.jpg",
"4.jpg"
};

        private readonly DispatcherTimer _imageChangeTimer;

        // Reference to the MainFrame for navigation
        private Frame _mainFrame;

        public HomePage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame; // Initialize the MainFrame reference

            _imageChangeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _imageChangeTimer.Tick += (sender, e) => ChangeImage();
            _imageChangeTimer.Start();

            StartImageSlider();
        }

        public HomePage()
        {
        }

        private void StartImageSlider()
        {
            UpdateImage();
        }

        private void UpdateImage()
        {
            try
            {
                ImageSlider.Source = new BitmapImage(new Uri(_imageSources[_currentImageIndex], UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }

        private void ChangeImage()
        {
            _currentImageIndex = (_currentImageIndex + 1) % _imageSources.Length;
            SmoothImageTransition();
        }

        private void SmoothImageTransition()
        {
            var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            fadeOut.Completed += (s, e) =>
            {
                UpdateImage();
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                ImageSlider.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };
            ImageSlider.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void OnLeftArrowClick(object sender, RoutedEventArgs e)
        {
            _currentImageIndex = (_currentImageIndex - 1 + _imageSources.Length) % _imageSources.Length;
            SmoothImageTransition();
        }

        private void OnRightArrowClick(object sender, RoutedEventArgs e)
        {
            ChangeImage();
        }

        private void OnGetStartedClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Welcome to Osprey! Let's get started.", "Get Started", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LearnMoreButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                // Find the parent Border (card) and the additional details panel
                var card = FindParent<Border>(toggleButton);
                var additionalDetails = FindChild<StackPanel>(card, "AdditionalDetails" + toggleButton.Name.Substring(toggleButton.Name.Length - 1));

                if (card != null && additionalDetails != null)
                {
                    // Expand the card
                    var expandAnimation = (Storyboard)FindResource("ExpandCard");
                    expandAnimation.Begin(card);

                    // Show additional details
                    additionalDetails.Visibility = Visibility.Visible;
                }
            }
        }

        private void LearnMoreButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                // Find the parent Border (card) and the additional details panel
                var card = FindParent<Border>(toggleButton);
                var additionalDetails = FindChild<StackPanel>(card, "AdditionalDetails" + toggleButton.Name.Substring(toggleButton.Name.Length - 1));

                if (card != null && additionalDetails != null)
                {
                    // Collapse the card
                    var collapseAnimation = (Storyboard)FindResource("CollapseCard");
                    collapseAnimation.Begin(card);

                    // Hide additional details
                    additionalDetails.Visibility = Visibility.Collapsed;

                    // Reset the ScrollViewer's vertical offset to 0
                    var scrollViewer = FindChild<ScrollViewer>(card, "Event" + toggleButton.Name.Substring(toggleButton.Name.Length - 1) + "ScrollViewer");
                    if (scrollViewer != null)
                    {
                        scrollViewer.ScrollToVerticalOffset(0);
                    }
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T && (child as FrameworkElement)?.Name == childName)
                {
                    return child as T;
                }
                var result = FindChild<T>(child, childName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private void BookEventButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the BookEventPage
            if (_mainFrame != null)
            {
                _mainFrame.Navigate(new BookEventPage());
            }
            else
            {
                MessageBox.Show("Navigation frame is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToSocialFeedButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the SocialFeedPage
            if (_mainFrame != null)
            {
                _mainFrame.Navigate(new SocialFeedPage());
            }
            else
            {
                MessageBox.Show("Navigation frame is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Chatbot
        private void InitializeChatbot()
        {
            // Add a welcome message from the chatbot
            AddChatMessage("AI Assistant", "Hello! How can I assist you today?", false);
        }

        private void ChatbotToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ChatbotPanel.Visibility = Visibility.Visible;
        }

        private void ChatbotToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ChatbotPanel.Visibility = Visibility.Collapsed;
        }

        private void CloseChatbotButton_Click(object sender, RoutedEventArgs e)
        {
            ChatbotToggleButton.IsChecked = false;
            ChatbotPanel.Visibility = Visibility.Collapsed;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = ChatInputBox.Text.Trim();
            if (!string.IsNullOrEmpty(userMessage))
            {
                // Add the user's message to the chat
                AddChatMessage("You", userMessage, true);

                // Clear the input box
                ChatInputBox.Clear();

                // Simulate a response from the chatbot
                string botResponse = GetBotResponse(userMessage);
                AddChatMessage("AI Assistant", botResponse, false);
            }
        }

        private void AddChatMessage(string sender, string message, bool isUser)
        {
            // Create a message bubble
            Border messageBubble = new Border
            {
                Background = isUser ? Brushes.LightBlue : Brushes.LightGray,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 5, 0, 5),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                MaxWidth = 300
            };

            TextBlock messageText = new TextBlock
            {
                Text = $"{sender}: {message}",
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap
            };

            messageBubble.Child = messageText;

            // Add the message bubble to the chat panel
            ChatMessagesPanel.Children.Add(messageBubble);

            // Scroll to the bottom of the chat
            ChatScrollViewer.ScrollToEnd();
        }

        private string GetBotResponse(string userMessage)
        {
            // Simulate a simple chatbot response
            if (userMessage.ToLower().Contains("hello") || userMessage.ToLower().Contains("hi"))
            {
                return "Hello! How can I help you?";
            }
            else if (userMessage.ToLower().Contains("event"))
            {
                return "We have several events coming up! Check out the 'Explore Our Events' section for more details.";
            }
            else if (userMessage.ToLower().Contains("contact"))
            {
                return "You can contact us at info@osprey.com or call +1 (123) 456-7890.";
            }
            else
            {
                return "I'm sorry, I didn't understand that. Can you please rephrase your question?";
            }
        }

        private void ChatInputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(ChatInputBox.Text))
            {
                PlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                PlaceholderText.Visibility = Visibility.Collapsed;
            }
        }

        private void Sopnsor_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the BookEventPage
            if (_mainFrame != null)
            {
                _mainFrame.Navigate(new SponsorPage());
            }
            else
            {
                MessageBox.Show("Navigation frame is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}