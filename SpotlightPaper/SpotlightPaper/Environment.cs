using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotlightPaper
{
    public class Environment
    {
        public static void createAppShortcut(string location)
        {
            WshShell shell = new WshShell();
            string shortcutAddress = location + "\\SpotlightPaper.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Shortcut for SpotlightPaper";
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.Save();
        }
    }
}
