using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SpotlightPaper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private MainWindow window;
        private NotifyIcon trayicon;

        private Settings settings;

        public App()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() == 1)
            {
                // Load settings
                this.settings = new Settings();
                this.settings.loadSettings();

                // Setup tray icon
                trayicon = new NotifyIcon();
                trayicon.Visible = true;

                // Init control window
                window = new MainWindow(this.settings, this);
                
                // Update trayicon
                runningChanged(this.settings.changepaper);

                // On trayicon doubleclick
                trayicon.DoubleClick += Trayicon_DoubleClick;

                // On application quit
                this.Exit += App_Exit;

                // Ensure app shortcut is available in program start menu; Because standalone
                Environment.createAppShortcut(System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu));
            }
            else
            {
                // just one instance is required to run; older one is running, shutdown this one
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            // Hide icon when exit
            trayicon.Visible = false;
        }

        private void Trayicon_DoubleClick(object sender, EventArgs e)
        {
            window.Show();
        }

        public void runningChanged(Boolean isrunning)
        {
            // Update tray icon
            if (isrunning)
            {                
                trayicon.Icon = new Icon(System.Windows.Forms.Application.StartupPath + "\\tray-on.ico");
                trayicon.Text = "SpotlightPaper is doing great.";
            }
            else
            {
                trayicon.Icon = new Icon(System.Windows.Forms.Application.StartupPath + "\\tray.ico");
                trayicon.Text = "SpotlightPaper is not doing much.";
            }
        }
    }
}
