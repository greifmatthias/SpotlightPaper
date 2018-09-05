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
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace SpotlightPaper
{
    public partial class MainWindow : Window
    {
        // Vars
        App parent;
        Settings settings;

        // Temp
        string _imageclicked;

        public MainWindow(Settings settings, App parent)
        {
            InitializeComponent();

            // Set
            this.settings = settings;

            // Check if need to run
            this.parent = parent;

            // Setup timer
            Timer timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1000 * 60 * 60; // 1 hour
            timer.Enabled = true;

            timer.Start();

            // Make sure images are set
            setPapers(settings.lastloaded);

            // Setup UI
            chEnable.IsChecked = settings.changepaper;
            chAutostart.IsChecked = settings.autostart;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update desktop wallpaper
            checkDir();
            setPapers(settings.lastloaded);
        }

        private void chEnable_Checked(object sender, RoutedEventArgs e)
        {
            Uri iconUri = new Uri(System.Windows.Forms.Application.StartupPath + "\\tray-on.ico", UriKind.RelativeOrAbsolute);
            
            if (chEnable.IsChecked == true)
            {
                // Set wallpaper
                setPapers(null);
                // Reset setting
                settings.lastloaded = "";

                // Update UI
                spLastUpdate.Visibility = Visibility.Visible;
            }
            else
            {
                iconUri = new Uri(System.Windows.Forms.Application.StartupPath + "\\tray.ico", UriKind.RelativeOrAbsolute);

                // Update UI
                spLastUpdate.Visibility = Visibility.Collapsed;
            }

            // Set window icon
            this.Icon = BitmapFrame.Create(iconUri);

            // Update tray
            parent.runningChanged(chEnable.IsChecked.Value);

            // Update settings
            settings.changepaper = chEnable.IsChecked == true;
            settings.saveSettings();
        }

        private void setPapers(string customimage = "")
        {
            // Check source folder
            checkDir();

            String image = "";

            // Get file
            DirectoryInfo info = new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
                + "\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");

            // Get latest image from source
            List<FileInfo> files = info.GetFiles()
             .OrderByDescending(f => f.LastWriteTime).ToList();

            // Set image
            if (customimage == "" || customimage == null)
            {
                // Set image source of local folder
                image = System.Windows.Forms.Application.StartupPath + "\\images\\" + files[0].Name;
            }
            else
            {
                image = System.Windows.Forms.Application.StartupPath + "\\images\\" + customimage;
            }

            // Set image if valid
            if (image != "")
            {
                // Set desktop wallpaper if needed
                Wallpaper.Set(new Uri(image), Wallpaper.Style.Centered);
                tbUpdateStamp.Text = DateTime.Now.ToLocalTime().ToString();

                // Set sample
                imgBackground.Source = new BitmapImage(new Uri(image, UriKind.Absolute));
            }
        }

        private void MiSave_Click(object sender, RoutedEventArgs e)
        {
            // Open up a dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Spotlight image as";
            saveFileDialog.Filter = "JPG|*.jpg";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Copy from temp folder
                File.Copy(System.Windows.Forms.Application.StartupPath + "\\images\\" + this._imageclicked, saveFileDialog.FileName);
            }
        }

        private void MiSet_Click(object sender, RoutedEventArgs e)
        {
            // Unable autochange
            chEnable.IsChecked = false;

            // Set wallpaper
            setPapers(this._imageclicked);

            // Update settings
            settings.lastloaded = this._imageclicked;
            settings.saveSettings();
        }

        private void Btnimagecontainer_Click(object sender, RoutedEventArgs e)
        {
            // Get strip image clicked
            System.Windows.Controls.Button origin = (System.Windows.Controls.Button)sender;

            // Get image clicked
            this._imageclicked = origin.Tag.ToString();

            // Open contextmenu
            origin.ContextMenu.IsOpen = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void chAutostart_Checked(object sender, RoutedEventArgs e)
        {
            // Get machine autostartup folder
            string startupdir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup);

            // Set/Delete shortcut if needed
            if (chAutostart.IsChecked == true)
            {
                // Create shortcut to startup folder
                Environment.createAppShortcut(startupdir);
            }
            else
            {
                // Delete shortcut
                if (File.Exists(startupdir + "\\SpotlightPaper.lnk"))
                {
                    File.Delete(startupdir + "\\SpotlightPaper.lnk");
                }
            }

            // Set/Save settings
            settings.autostart = chAutostart.IsChecked == true;
            settings.saveSettings();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            new wAbout().ShowDialog();
        }

        private void checkDir()
        {
            // Check target folder
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\images\\"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\images\\");
            }
            
            // Get source datafolder
            DirectoryInfo info = new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
                + "\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");

            // Get latest image from source
            List<FileInfo> files = info.GetFiles()
             .OrderByDescending(f => f.LastWriteTime).ToList();

            // Get image from source datafolder
            for(int i = 0; i < files.Count; i++)
            {
                if (Imaging.IsValidImage(files[i].FullName))
                {
                    // Get image size
                    System.Drawing.Image img = System.Drawing.Image.FromFile(files[i].FullName);

                    // Copy file if valid
                    if (Screen.PrimaryScreen.Bounds.Height == img.Height && Screen.PrimaryScreen.Bounds.Width == img.Width)
                    {
                        // Copy if new
                        if (!File.Exists(System.Windows.Forms.Application.StartupPath + "\\images\\" + files[i].Name))
                        {
                            files[i].CopyTo(System.Windows.Forms.Application.StartupPath + "\\images\\" + files[i].Name);
                        }
                    }
                }
            }

            // Update strip
            // Get local datafolder
            info = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + "\\images\\");

            if (info.GetFiles().Length > 0)
            {
                spPreviousSpots_strip.Visibility = Visibility.Visible;
                spSources.Children.Clear();

                // Get latest image from source
                List<FileInfo> locals = info.GetFiles()
                 .OrderByDescending(f => f.LastWriteTime).ToList();

                foreach (FileInfo finfo in locals)
                {
                    System.Windows.Controls.Button btnimagecontainer = new System.Windows.Controls.Button();

                    // Get image
                    System.Drawing.Image i = System.Drawing.Image.FromFile(finfo.FullName);

                    // Set image control
                    Image img = new Image();
                    img.Width = 150;
                    img.Height = (150.00 / i.Width) * i.Height;
                    img.Source = new BitmapImage(new Uri(finfo.FullName, UriKind.Absolute));

                    btnimagecontainer.Content = img;

                    btnimagecontainer.Tag = finfo.Name;

                    // Setup contextmenu
                    System.Windows.Controls.ContextMenu menu = new System.Windows.Controls.ContextMenu();

                    System.Windows.Controls.MenuItem miSet = new System.Windows.Controls.MenuItem();
                    miSet.Header = "Set";
                    miSet.Click += MiSet_Click;
                    menu.Items.Add(miSet);
                    System.Windows.Controls.MenuItem miSave = new System.Windows.Controls.MenuItem();
                    miSave.Header = "Save";
                    miSave.Click += MiSave_Click;
                    menu.Items.Add(miSave);

                    btnimagecontainer.ContextMenu = menu;

                    // On image click
                    btnimagecontainer.Click += Btnimagecontainer_Click;

                    // Add to strip
                    spSources.Children.Add(btnimagecontainer);
                }
            }
            else
            {
                spPreviousSpots_strip.Visibility = Visibility.Collapsed;
            }
        }
    }
}
