using IWshRuntimeLibrary;
using System.IO;
using System.Windows.Forms;
using File = System.IO.File;

namespace SpotlightPaper
{
    public class Environment
    {
        public static void createAppShortcut(string location)
        {
            if (File.Exists(location + "\\SpotlightPaper.lnk"))
            {
                File.Delete(location + "\\SpotlightPaper.lnk");
            }

            WshShell shell = new WshShell();
            string shortcutAddress = location + "\\SpotlightPaper.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Shortcut for SpotlightPaper";
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.Save();
        }
    }
}
