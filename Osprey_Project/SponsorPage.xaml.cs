using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Osprey_Project
{
    public partial class SponsorPage : Page
    {
        public SponsorPage()
        {
            InitializeComponent();
            LoadSponsors();
        }

        private void LoadSponsors()
        {
            try
            {
                // Sample sponsor data (replace with real data from database if needed)
                var sponsors = new List<string>
                {
                    "Images/sponsor1.png",
                    "Images/sponsor2.png",
                    "Images/sponsor3.png"
                };

               

                foreach (var sponsor in sponsors)
                {
                    var border = new Border
                    {
                        Width = 150,
                        Height = 100,
                        Margin = new Thickness(10),
                        Background = System.Windows.Media.Brushes.White,
                        CornerRadius = new CornerRadius(10),
                        BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray),
                        BorderThickness = new Thickness(1)
                    };

                    var image = new Image
                    {
                        Source = new BitmapImage(new Uri(sponsor, UriKind.Relative)),
                        Stretch = System.Windows.Media.Stretch.Uniform,
                        Margin = new Thickness(10)
                    };

                    border.Child = image;
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sponsors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BecomeSponsor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 NavigationService?.Navigate(new SponsorFormPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
    }
}
