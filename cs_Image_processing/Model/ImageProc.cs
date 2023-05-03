using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;

namespace cs_Image_processing.Model
{
    public static class ImageProc
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public static (BitmapSource src , Bitmap bitmap ) CreateBitmapAndBitmapSourceByFilePath(string filePath)
        {
            Bitmap bitmap = null;
            BitmapSource srcImage = null;

            if (System.IO.File.Exists(filePath) == false) return (null , null);
            bitmap = new System.Drawing.Bitmap(filePath);
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

            return (srcImage , bitmap);
        }

        public static void ChannelSwap(Bitmap bitmap) {
            if ( Bitmap.GetPixelFormatSize(bitmap.PixelFormat) <= 8)
            {


            }
            

        //if (bitmap.Get == null) return;
        
        }
    }
}
