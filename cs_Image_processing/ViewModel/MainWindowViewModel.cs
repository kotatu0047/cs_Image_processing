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
   public  class MainWindowViewModel : INotifyPropertyChanged
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private int _count;
        public int count
        {
            get { return _count; }
            set
            {
                _count = value;
                NotifyPropertyChanged("count");
            }
        }

        //public MainWindowModel _model;
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

                //if (_model.Src != value)
                //{
                //    _model.Src = value;
                //    OnPropertyChanged("SrcImage");
                //}
            }
        }

        public ICommand Button1_Pushed { get; set; }
        private void Button1_Command()
        {
            //カウント数を増やす
            count++;
        }

        public  ICommand ReadImageButtonPushed { get; set; }
        private void ReadImageButtonPushedCommand ()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("画像ファイル選択", "*.jpg"));
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var filePath = dialog.FileName;
                if (System.IO.File.Exists(filePath) == false) return;
                var bitmap = new Bitmap(filePath);
                var hBitmap =  bitmap.GetHbitmap();
                try
                {
                    this.SrcImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
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
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            Button1_Pushed = new RelayCommand(Button1_Command);
            ReadImageButtonPushed = new RelayCommand(ReadImageButtonPushedCommand);
            count = 1;
            _src = null;  //new MainWindowModel();
        }


        //変数の更新通知用
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
