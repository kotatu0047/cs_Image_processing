using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using Microsoft.WindowsAPICodePack.Dialogs;
using cs_Image_processing.lib;
using ImageProcess;

namespace cs_Image_processing.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        //[System.Runtime.InteropServices.DllImport("gdi32.dll")]
        //private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// 内部処理用Bitmap
        /// 画面では使いません
        /// </summary>
        private Bitmap _bitmap;

        /// <summary>
        /// 変換元画像のBitmapSource
        /// </summary>
        private BitmapSource _srcImage { get; set; }
        public BitmapSource SrcImage
        {
            get
            {
                return _srcImage;
            }
            set
            {
                this._srcImage = value;
                NotifyPropertyChanged("SrcImage");
            }
        }

        /// <summary>
        /// 変換後画像のBitmapSource
        /// </summary>
        private BitmapSource _convertedSrcImage { get; set; }
        public BitmapSource ConvertedSrcImage
        {
            get
            {
                return _convertedSrcImage;
            }
            set
            {
                this._convertedSrcImage = value;
                NotifyPropertyChanged("ConvertedSrcImage");
            }
        }

        public ICommand ReadImageButtonPushed { get; set; }
        /// <summary>
        /// 画像ファイルを読み込み、System.Windows.Media.Imaging.BitmapSource型へ変換し、SrcImageにセットする
        /// 内部処理に使うために、_bitmapにもSystem.Drawing.Bitmap型でセットする
        /// </summary>
        private void ReadImageButtonPushedCommand()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("画像ファイル選択", "*.jpg"));
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var result = Trait.CreateBitmapAndBitmapSourceByFilePath(dialog.FileName);
                if (result.src != null) { this.SrcImage = result.src; }
                if (result.bitmap != null) { this._bitmap = result.bitmap; }
            }
        }

        public ICommand ExcuteButtonPushed { get; set; }
        /// <summary>
        /// 画像処理を実行し、実行結果をConvertedSrcImageにセットする
        /// </summary>
        private void ExcuteButtonPushedCommand()
        {
            if (this._bitmap == null) return;

            var channelSwapResult = Trait.ChannelSwap(in this._bitmap);
            if (channelSwapResult == null) return;
            var result = Trait.CreateBitmapSourceByBitmap(in channelSwapResult);
            if (result == null) return;
            this.ConvertedSrcImage = result;
        }

        /// <summary>
        /// 変数の更新通知用
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// コンストラクタ 
        /// </summary>
        public MainWindowViewModel()
        {
            ReadImageButtonPushed = new RelayCommand(ReadImageButtonPushedCommand);
            ExcuteButtonPushed = new RelayCommand(ExcuteButtonPushedCommand);
            _bitmap = null;
            _srcImage = null;
            _convertedSrcImage = null;
        }

        /// <summary>
        /// 変数の更新通知用
        /// </summary>
        /// <param name="info">変更されたプロパティ名</param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
