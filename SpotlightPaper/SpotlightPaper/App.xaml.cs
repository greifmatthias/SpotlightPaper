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

        public App()
        {
            Boolean running = false;

            // Try to read config
            if (File.Exists("Config"))
            {
                TextReader tr = new StreamReader("Config");
                running = Convert.ToBoolean(tr.ReadLine());
                tr.Close();
            }

            // Setup tray icon
            trayicon = new NotifyIcon();
            trayicon.Visible = true;

            runningChanged(running);

            trayicon.DoubleClick += Trayicon_DoubleClick;

            // Init control window
            window = new MainWindow(running, this);

            this.Exit += App_Exit;
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
            // Save
            TextWriter tw = new StreamWriter("Config");
            tw.WriteLine(isrunning.ToString());
            tw.Close();

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
