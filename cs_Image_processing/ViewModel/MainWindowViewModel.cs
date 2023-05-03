using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using Microsoft.WindowsAPICodePack.Dialogs;
using cs_Image_processing.lib;
using cs_Image_processing.Model;

namespace cs_Image_processing.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private BitmapSource _src { get; set; }
        public BitmapSource SrcImage
        {
            get
            {
                return _src;
            }
            set
            {
                this._src = value;
                NotifyPropertyChanged("SrcImage");
            }
        }

        public ICommand ReadImageButtonPushed { get; set; }
        /// <summary>
        /// 画像ファイルを読み込み、System.Windows.Media.Imaging.BitmapSource型へ変換し、SrcImageにセットする
        /// </summary>
        private void ReadImageButtonPushedCommand()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("画像ファイル選択", "*.jpg"));
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var bitmapSource = ImageOperator.createBitmapSourceByFilePath(dialog.FileName);
                if (bitmapSource != null) { this.SrcImage = bitmapSource; }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            ReadImageButtonPushed = new RelayCommand(ReadImageButtonPushedCommand);
            _src = null;
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
