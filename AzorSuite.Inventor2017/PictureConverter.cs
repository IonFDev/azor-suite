using System.Drawing;
using System.Windows.Forms;
using stdole;

namespace AzorSuite.Inventor2017
{
    internal class PictureConverter : AxHost
    {
        private PictureConverter()
            : base(string.Empty)
        {
        }

        public static IPictureDisp ImageToPictureDisp(
            System.Drawing.Image image)
        {
            return (IPictureDisp)
                GetIPictureDispFromPicture(image);
        }
    }
}