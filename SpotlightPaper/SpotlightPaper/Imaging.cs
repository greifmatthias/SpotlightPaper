using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SpotlightPaper
{
    public class Imaging
    {
        public bool IsValidImage(string filename)
        {
            try
            {
                BitmapImage newImage = new BitmapImage(new Uri(filename));
            }
            catch (NotSupportedException)
            {
                // System.NotSupportedException:
                // No imaging component suitable to complete this operation was found.
                return false;
            }
            return true;
        }
    }
}
