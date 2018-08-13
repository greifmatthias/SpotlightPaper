using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        App parent;

        Timer timer;

        DirectoryInfo info;

        public MainWindow(Boolean running, App parent)
        {
            InitializeComponent();

            // Check if need to run
            this.parent = parent;

            //Setup timer
            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1000 * 60 * 60; // 1 hour
            timer.Enabled = true;

            if (running)
            {
                timer.Start();
            }

            // Setup wallpapers
            setPapers(running);

            // Setup UI
            chEnable.IsChecked = running;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            setPapers(true);
        }

        private void chEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (chEnable.IsChecked == true)
            {
                setPapers(true);
                timer.Start();
            }
            else
            {
                timer.Stop();
            }

            parent.runningChanged(chEnable.IsChecked.Value);
        }

        private void setPapers(Boolean wallpaperset)
        {
            // Get datafolder
            info = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + "\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");

            // Get latest image
            image = info.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .First().FullName;

            if (wallpaperset)
            {
                Wallpaper.Set(new Uri(image), Wallpaper.Style.Centered);
            }

            imgBackground.Source = new BitmapImage(new Uri(image, UriKind.Absolute));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
