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

                runningChanged(this.settings.changepaper);

                trayicon.DoubleClick += Trayicon_DoubleClick;

                // Init control window
                window = new MainWindow(this.settings, this);

                // On application quit
                this.Exit += App_Exit;
            }
            else
            {
                // just one instance is required to run
                this.Shutdown();
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            trayicon.Visible = false;
        }

        private void Trayicon_DoubleClick(object sender, EventArgs e)
        {
            window.Show();
        }

        public void runningChanged(Boolean isrunning)
        {
            this.settings.saveSettings();

            // Update tray icon
            if (isrunning)
            {
                trayicon.Icon = new Icon("tray-on.ico");
                trayicon.Text = "SpotlightPaper is doing great.";
            }
            else
            {
                trayicon.Icon = new Icon("tray.ico");
                trayicon.Text = "SpotlightPaper is not doing much.";
            }
        }
    }
}
