using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcess
{
    /// <summary>
    /// TODO 重複している処理が多いのでリファクタリングが必用
    /// </summary>
    public static class Img
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// BitmapからBitmapSourceを作成
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
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

        /// <summary>
        /// filePathで指定された画像ファイルを読み込み、
        /// BitmapとBitmapSource両方に変換して返す
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static (BitmapSource src, Bitmap bitmap) CreateBitmapAndBitmapSourceByFilePath(string filePath)
        {
            Bitmap bitmap = null;
            BitmapSource srcImage = null;

            if (System.IO.File.Exists(filePath) == false) return (null, null);
            bitmap = new Bitmap(filePath);
            srcImage = CreateBitmapSourceByBitmap(bitmap);

            return (srcImage, bitmap);
        }


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

            var distBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height, PixelFormat.Format24bppRgb);
            var rect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);

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

        /// <summary>
        /// 画像ファイルをグレースケールに変換します
        /// </summary>
        /// <param name="srcBitmap">変換元画像</param>
        /// <returns>変換後画像</returns>
        public unsafe static Bitmap Grayscale(in Bitmap srcBitmap)
        {
            if (srcBitmap == null) return null;
            // Format24bppRgbの24bitピクセルフォーマットにのみ対応する
            // todo 実行不能の原因メッセージをユーザに通知
            if (srcBitmap.PixelFormat != PixelFormat.Format24bppRgb) return null;

            // モノクロ画像フォーマットを指定して、ビットマップを作成
            var distBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height, PixelFormat.Format8bppIndexed);
            var palette = distBitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            distBitmap.Palette = palette;

            var rect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);

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
                distLooked = distBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                byte* distPtr = (byte*)distLooked.Scan0;
                for (int y = 0; y < srcLooked.Height; ++y)
                {
                    //行の先頭のポインタ
                    byte* srcPtrHead = srcPtr + y * srcLooked.Stride;
                    byte* distPtrHead = distPtr + y * distLooked.Stride;

                    for (int x = 0; x < srcLooked.Width; ++x)
                    {
                        var blue = (float)srcPtrHead[0] * 0.0722;
                        var green = (float)srcPtrHead[1] * 0.7152;
                        var red = (float)srcPtrHead[2] * 0.2126;

                        distPtrHead[0] = (byte)Math.Floor(blue + green + red);

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


        /// <summary>
        /// 画像ファイルをレインボーカラーに変換させます
        /// </summary>
        /// <param name="srcBitmap">変換元画像</param>
        /// <param name="thresholdArg">レインボーカラーパレットの開始位置を決めるしきい値
        /// この値を変更することで、異なるレインボーカラーのパターンで画像を生成することができます
        /// 指定可能な値は0-360です
        /// </param>
        /// <returns>変換後画像</returns>
        /// <exception cref="ArgumentOutOfRangeException">thresholdArgの値が不正</exception>
        public unsafe static Bitmap Rainbow(in Bitmap srcBitmap, int thresholdArg = 0)
        {
            if (thresholdArg < 0 || thresholdArg > 360) throw new ArgumentOutOfRangeException("引数:thresholdArgの値が不正です");

            if (srcBitmap == null) return null;
            // Format24bppRgbの24bitピクセルフォーマットにのみ対応する
            // todo 実行不能の原因メッセージをユーザに通知
            if (srcBitmap.PixelFormat != PixelFormat.Format24bppRgb) return null;

            // モノクロ画像フォーマットを指定して、ビットマップを作成
            var distBitmap = new Bitmap(srcBitmap.Width, srcBitmap.Height, PixelFormat.Format8bppIndexed);

            var palette = distBitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                var _i = i < 128 ? i : i - 128;

                float h = (float)(_i * 2.8) + thresholdArg;  // thresholdArg = 0~360
                if (h >= 360)
                {
                    h -= 360;
                }

                palette.Entries[i] = HsvColor.ToRgb(1f, 1f, h);
            }
            distBitmap.Palette = palette;

            var rect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);

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
                distLooked = distBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                byte* distPtr = (byte*)distLooked.Scan0;
                for (int y = 0; y < srcLooked.Height; ++y)
                {
                    //行の先頭のポインタ
                    byte* srcPtrHead = srcPtr + y * srcLooked.Stride;
                    byte* distPtrHead = distPtr + y * distLooked.Stride;

                    for (int x = 0; x < srcLooked.Width; ++x)
                    {
                        var blue = (float)srcPtrHead[0] * 0.0722;
                        var green = (float)srcPtrHead[1] * 0.7152;
                        var red = (float)srcPtrHead[2] * 0.2126;

                        distPtrHead[0] = (byte)Math.Floor(blue + green + red);
                        //(byte)((srcPtrHead[0] * 0.0722) + (srcPtrHead[1] * 0.7152) + (srcPtrHead[2] * 0.2126)); 

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
    }
}