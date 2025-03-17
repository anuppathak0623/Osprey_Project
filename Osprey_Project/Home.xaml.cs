using System.Windows;
using System.Windows.Controls;

namespace Osprey_Project
{
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();

            // Automatically load HomePage.xaml into MainFrame when Home.xaml opens
            MainFrame.Navigate(new HomePage(MainFrame));
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage(MainFrame)); // Navigates to HomePage.xaml
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AboutPage()); // Navigates to AboutPage.xaml
        }

        private void Membership_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MembershipPage()); // Navigates to MembershipPage.xaml
        }

        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ContactPage()); // Navigates to ContactPage.xaml
        }

        // Profile Button Click Event - Opens Context Menu
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button profileButton && profileButton.ContextMenu != null)
            {
                profileButton.ContextMenu.IsOpen = true;
            }
        }

        // Settings Click Event - Navigates to Settings Page
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage()); // Navigates to the SettingsPage.xaml
        }

        // Logout Click Event - Shows Logout Message
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?",
                                              "Confirm Logout",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("You have been logged out!", "Logout", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow obj = new MainWindow();
                this.Visibility = Visibility.Hidden;
                obj.Show();
            }
        }
    }
}