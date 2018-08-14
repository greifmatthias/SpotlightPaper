using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotlightPaper
{
    public class Settings
    {
        public Boolean autostart { get; set; }
        public Boolean changepaper { get; set; }

        public string lastloaded { get; set; }

        public void loadSettings()
        {
            // Try to read config
            if (File.Exists("Config"))
            {
                TextReader tr = new StreamReader("Config");
                this.changepaper = Convert.ToBoolean(tr.ReadLine());
                this.autostart = Convert.ToBoolean(tr.ReadLine());
                this.lastloaded = Convert.ToString(tr.ReadLine());
                tr.Close();
            }
            else
            {
                changepaper = false;
                autostart = false;
            }
        }

        public void saveSettings()
        {
            // Save
            TextWriter tw = new StreamWriter("Config");
            tw.WriteLine(this.changepaper.ToString());
            tw.WriteLine(this.autostart.ToString());
            tw.WriteLine(this.lastloaded);
            tw.Close();
        }
    }
}
