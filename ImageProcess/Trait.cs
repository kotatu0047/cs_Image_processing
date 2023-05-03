using System;
using System.Drawing;
using System.Drawing.Imaging;
//using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcess
{
    public static class Trait
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// レッド => ブルーと、ブルー => レッドのチャネル変換を行います
        /// </summary>
        /// <param name="srcBitmap">変換元画像</param>
        /// <returns>変換後画像</returns>
        public unsafe static Bitmap ChannelSwap(in Bitmap srcBitmap)
        {
            if (srcBitmap == null) return null;
            // Format24bppRgbの24bitピクセルフォーマットにのみ対応する
            // todo 実行不能の原因メッセージをユーザに通知
            if (srcBitmap.PixelFormat != PixelFormat.Format24bppRgb) return null;
            //if (Bitmap.GetPixelFormatSize(srcBitmap.PixelFormat) != 24) return null;

            var distBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height, PixelFormat.Format24bppRgb);
            var rect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);

            // todo try catch and using?
            var srcBitCount = Bitmap.GetPixelFormatSize(srcBitmap.PixelFormat);
            var srcAlignment = srcBitCount / 8;
            var distBitCount = Bitmap.GetPixelFormatSize(distBitmap.PixelFormat);
            var distAlignment = distBitCount / 8;
            BitmapData srcLooked = null;
            BitmapData distLooked = null;

            try
            {
                // 画像をメモリロックし、先頭ポインタを取得
                srcLooked = srcBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                byte* srcPtr = (byte*)srcLooked.Scan0;
                distLooked = distBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte* distPtr = (byte*)distLooked.Scan0;
                for (int y = 0; y < srcLooked.Height; ++y)
                {
                    //行の先頭のポインタ
                    byte* srcPtrHead = srcPtr + y * srcLooked.Stride;
                    byte* distPtrHead = distPtr + y * distLooked.Stride;

                    for (int x = 0; x < srcLooked.Width; ++x)
                    {
                        distPtrHead[0] = srcPtrHead[2]; //ブルー => レッド
                        distPtrHead[1] = srcPtrHead[1]; //グリーン => グリーン
                        distPtrHead[2] = srcPtrHead[0]; //レッド => ブルー

                        srcPtrHead += srcAlignment;
                        distPtrHead += distAlignment;
                    }
                }
            }
            finally
            {
                if (srcLooked != null) srcBitmap.UnlockBits(srcLooked);
                if (distLooked != null) distBitmap.UnlockBits(distLooked);
            }

            return distBitmap;
        }

        public static BitmapSource CreateBitmapSourceByBitmap(in Bitmap bitmap)
        {
            if (bitmap == null) return null;
            BitmapSource bitmapSource = null;
            var hBitmap = bitmap.GetHbitmap();

            try
            {
                bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
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

            return bitmapSource;
        }

        public static (BitmapSource src, Bitmap bitmap) CreateBitmapAndBitmapSourceByFilePath(string filePath)
        {
            Bitmap bitmap = null;
            BitmapSource srcImage = null;

            if (System.IO.File.Exists(filePath) == false) return (null, null);
            bitmap = new Bitmap(filePath);
            srcImage = CreateBitmapSourceByBitmap(bitmap);

            return (srcImage, bitmap);
        }
    }
}