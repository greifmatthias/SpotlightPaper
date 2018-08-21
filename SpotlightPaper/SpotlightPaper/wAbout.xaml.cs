using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotlightPaper
{
    /// <summary>
    /// Interaction logic for wAbout.xaml
    /// </summary>
    public partial class wAbout : Window
    {
        public wAbout()
        {
            InitializeComponent();

            tbVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void btnGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/greifmatthias/SpotlightPaper");
        }

        private void btnDeveloper_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.greifmatthias.be");
        }
    }
}
