using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotlightPaper
{
    public partial class MainWindow : Window
    {
        // Vars
        String image;

        public MainWindow()
        {
            InitializeComponent();

            // Get datafolder
            DirectoryInfo folder = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + "\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");

            image = folder.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .First().FullName;

            imgBackground.Source = new BitmapImage(new Uri(image, UriKind.Absolute));
        }

        private void chEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (chEnable.IsChecked == true)
            {
                Wallpaper.Set(new Uri(image), Wallpaper.Style.Centered);
            }
        }
    }
}
