using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace cs_Image_processing.Model
{
    public static class ImageOperator
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource createBitmapSourceByFilePath(string filePath)
        {
            BitmapSource srcImage = null;

            if (System.IO.File.Exists(filePath) == false) return null;
            var bitmap = new Bitmap(filePath);
            var hBitmap = bitmap.GetHbitmap();

            try
            {
                srcImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return srcImage;
        }
    }
}
