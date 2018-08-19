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
        String image;
        App parent;

        Timer timer;

        DirectoryInfo info;

        Settings settings;

        string _tempImageclicked;

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

            // Make sure sample image is set
            setPapers(false, settings.lastloaded);

            // Setup UI
            chEnable.IsChecked = settings.changepaper;
            chAutostart.IsChecked = settings.autostart;

            //Correct current spotlight imageview to right dimensions
            imgBackground.Height = (this.Width / Screen.PrimaryScreen.WorkingArea.Width) * Screen.PrimaryScreen.WorkingArea.Height;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update desktop wallpaper
            setPapers(true);
        }

        private void chEnable_Checked(object sender, RoutedEventArgs e)
        {
            Uri iconUri = new Uri(System.Windows.Forms.Application.StartupPath + "\\tray-on.ico", UriKind.RelativeOrAbsolute);

            // Start or stop timer
            if (chEnable.IsChecked == true)
            {
                setPapers(true);
                timer.Start();

                settings.lastloaded = "";
            }
            else
            {
                timer.Stop();
                iconUri = new Uri(System.Windows.Forms.Application.StartupPath + "\\tray.ico", UriKind.RelativeOrAbsolute);
            }

            // Set window icon
            this.Icon = BitmapFrame.Create(iconUri);

            // Update app
            parent.runningChanged(chEnable.IsChecked.Value);

            // Update settings
            settings.changepaper = chEnable.IsChecked == true;
            settings.saveSettings();
        }

        private void setPapers(Boolean wallpaperset, string customimage = "")
        {
            string image = "";

            if (customimage == "" || customimage == null)
            {
                // Get source datafolder
                info = new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
                    + "\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");

                // Get latest image from source
                List<FileInfo> files = info.GetFiles()
                 .OrderByDescending(f => f.LastWriteTime).ToList();

                // Get screen rotation
                bool landscape = Screen.PrimaryScreen.WorkingArea.Height <= Screen.PrimaryScreen.WorkingArea.Width;

                // Get image from source datafolder
                int count = 0;
                bool skip = false;
                while ((image == "" || !skip) && count < files.Count)
                {
                    if (Imaging.IsValidImage(files[count].FullName))
                    {
                        // Get image size
                        System.Drawing.Image img = System.Drawing.Image.FromFile(files[count].FullName);
                        bool landscapeimage = img.Height <= img.Width;

                        // Set source if valid
                        if (landscape && landscapeimage || !landscape && !landscapeimage)
                        {
                            image = files[count].FullName;
                            skip = !skip;
                        }
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
            info = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + "\\images\\");
            
            if(info.GetFiles().Length > 0)
            {
                spPreviousSpots_strip.Visibility = Visibility.Visible;
                spSources.Children.Clear();

                // Get latest image from source
                List<FileInfo> locals = info.GetFiles()
                 .OrderByDescending(f => f.LastWriteTime).ToList();
                foreach (FileInfo info in locals)
                {
                    System.Windows.Controls.Button btnimagecontainer = new System.Windows.Controls.Button();

                    // Get image
                    System.Drawing.Image i = System.Drawing.Image.FromFile(info.FullName);

                    // Set image control
                    Image img = new Image();
                    img.Width = 150;
                    img.Height = (150.00 / i.Width) * i.Height;
                    img.Source = new BitmapImage(new Uri(info.FullName, UriKind.Absolute));

                    btnimagecontainer.Content = img;

                    btnimagecontainer.Tag = info.Name;

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

        private void MiSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Spotlight image as";
            saveFileDialog.Filter = "JPG|*.jpg";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.Copy(System.Windows.Forms.Application.StartupPath + "\\images\\" + this._tempImageclicked, saveFileDialog.FileName);
            }
        }

        private void MiSet_Click(object sender, RoutedEventArgs e)
        {
            // Unable autochange
            chEnable.IsChecked = false;

            setPapers(true, this._tempImageclicked);

            // Update settings
            settings.lastloaded = this._tempImageclicked;
            settings.saveSettings();
        }

        private void Btnimagecontainer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button origin = (System.Windows.Controls.Button)sender;

            // Get image clicked
            this._tempImageclicked = origin.Tag.ToString();

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
    }
}
