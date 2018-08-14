﻿using Microsoft.Win32;
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

        Settings settings;

        public MainWindow(Settings settings, App parent)
        {
            InitializeComponent();

            // Set
            this.settings = settings;

            // Check if need to run
            this.parent = parent;

            //Setup timer
            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1000 * 60 * 60; // 1 hour
            timer.Enabled = true;

            // Setup UI
            chEnable.IsChecked = settings.changepaper;
            chAutostart.IsChecked = settings.autostart;

            // Make sure sample image is set
            setPapers(false);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update desktop wallpaper
            setPapers(true);
        }

        private void chEnable_Checked(object sender, RoutedEventArgs e)
        {
            Uri iconUri = new Uri("tray-on.ico", UriKind.RelativeOrAbsolute);

            // Start or stop timer
            if (chEnable.IsChecked == true)
            {
                setPapers(true);
                timer.Start();
            }
            else
            {
                timer.Stop();
                iconUri = new Uri("tray.ico", UriKind.RelativeOrAbsolute);
            }

            // Set window icon
            this.Icon = BitmapFrame.Create(iconUri);

            // Update app
            parent.runningChanged(chEnable.IsChecked.Value);

            // Update settings
            this.settings.changepaper = chEnable.IsChecked == true;
            this.settings.saveSettings();
        }

        private void setPapers(Boolean wallpaperset)
        {
            // Get datafolder
            info = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + "\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");

            // Get latest image
            List<FileInfo> files = info.GetFiles()
             .OrderByDescending(f => f.LastWriteTime).ThenBy(f => f.Name).ToList();

            // Get screen rotation
            bool landscape = Screen.PrimaryScreen.WorkingArea.Height <= Screen.PrimaryScreen.WorkingArea.Width;

            string image = "";
            int count = 0;

            while (image == "")
            {
                // Get image size
                System.Drawing.Image img = System.Drawing.Image.FromFile(files[count].FullName);
                bool landscapeimage = img.Height <= img.Width;

                // Set source if valid
                if (landscape && landscapeimage || !landscape && !landscapeimage)
                {
                    image = files[count].FullName;
                }

                count++;
            }

            // Set desktop wallpaper if needed
            if (wallpaperset)
            {
                Wallpaper.Set(new Uri(image), Wallpaper.Style.Centered);
                tbUpdateStamp.Text = DateTime.Now.ToLocalTime().ToString();
            }

            // Set sample
            imgBackground.Source = new BitmapImage(new Uri(image, UriKind.Absolute));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void chAutostart_Checked(object sender, RoutedEventArgs e)
        {
            // Get regis entry
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            // Set/Delete key if needed
            if (chAutostart.IsChecked == true)
            {
                // Set key to run at startup
                rk.SetValue(System.Windows.Forms.Application.ProductName, System.Windows.Forms.Application.ExecutablePath);
            }
            else
            {
                // Delete key
                if (rk.GetValueNames().Contains(System.Windows.Forms.Application.ProductName))
                {
                    rk.DeleteValue(System.Windows.Forms.Application.ProductName);
                }
            }

            // Set/Save settings
            settings.autostart = chAutostart.IsChecked == true;
            settings.saveSettings();
        }
    }
}
