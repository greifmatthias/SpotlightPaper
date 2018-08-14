using Microsoft.Win32;
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
            setPapers(false, settings.lastloaded);
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
        }

        private void setPapers(Boolean wallpaperset, string customimage = "")
        {
            string image = "";

            if (customimage == "")
            {
                // Get source datafolder
                info = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    + "\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");

                // Get latest image from source
                List<FileInfo> files = info.GetFiles()
                 .OrderByDescending(f => f.LastWriteTime).ThenBy(f => f.Name).ToList();

                // Get screen rotation
                bool landscape = Screen.PrimaryScreen.WorkingArea.Height <= Screen.PrimaryScreen.WorkingArea.Width;

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

                // Copy file if new image to local folder
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\images\\"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\images\\");
                }
                if (!File.Exists(System.Windows.Forms.Application.StartupPath + "\\images\\" + files[count].Name))
                {
                    File.Copy(image, System.Windows.Forms.Application.StartupPath + "\\images\\" + files[count].Name);
                }

                // Set image source of local folder
                image = System.Windows.Forms.Application.StartupPath + "\\images\\" + files[count].Name;
            }
            else
            {
                image = System.Windows.Forms.Application.StartupPath + "\\images\\" + customimage;
            }

            // Set desktop wallpaper if needed
            if (wallpaperset)
            {
                Wallpaper.Set(new Uri(image), Wallpaper.Style.Centered);
                tbUpdateStamp.Text = DateTime.Now.ToLocalTime().ToString();
            }

            // Set sample
            imgBackground.Source = new BitmapImage(new Uri(image, UriKind.Absolute));

            // Update strip
            // Get local datafolder
            spSources.Children.Clear();
            info = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + "\\images\\");
            // Get latest image from source
            List<FileInfo> locals = info.GetFiles()
             .OrderBy(f => f.LastWriteTime).ThenBy(f => f.Name).ToList();
            foreach (FileInfo info in locals)
            {
                Image img = new Image();
                img.Width = 150;
                img.Height = 80;
                img.Source = new BitmapImage(new Uri(info.FullName, UriKind.Absolute));
                spSources.Children.Add(img);
            }
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
