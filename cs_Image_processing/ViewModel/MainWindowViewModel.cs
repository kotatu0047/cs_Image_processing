using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using cs_Image_processing.Model;
using cs_Image_processing.lib;
using ImageProcess;

namespace cs_Image_processing.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 内部処理用Bitmap
        /// 画面では使いません
        /// </summary>
        private Bitmap _bitmap;

        private ConvertMode _currentConvertMode;
        /// <summary>
        /// 現在選択中の画像変換モード
        /// </summary>
        public ConvertMode CurrentConvertMode
        {
            get { return this._currentConvertMode; }
            set
            {
                if (this._currentConvertMode != value)
                {
                    this._currentConvertMode = value;
                    NotifyPropertyChanged("CurrentConvertMode");
                }
            }
        }

        private Dictionary<ConvertMode, string> _convertModeItemSource;
        /// <summary>
        /// セレクトボックスに表示する、変換モードの選択肢
        /// </summary>
        public Dictionary<ConvertMode, string> ConvertModeItemSource
        {
            get { return this._convertModeItemSource; }
            set
            {
                this._convertModeItemSource = value;
                NotifyPropertyChanged("SelectConvertModeItemSource");
            }
        }

        private int _sliderValue;
        /// <summary>
        /// スライダーの値
        /// </summary>
        public int SliderValue
        {
            get { return this._sliderValue; }
            set
            {
                this._sliderValue = value;
                NotifyPropertyChanged("SliderValue");
            }
        }

        private bool _sliderEnabled;
        /// <summary>
        /// スライダーUiの有効/無効
        /// </summary>
        public bool SliderEnabled
        {
            get { return this._sliderEnabled; }
            set
            {
                this._sliderEnabled = value;
                NotifyPropertyChanged("SliderEnabled");
            }
        }

        /// <summary>
        /// 変換元画像のBitmapSource
        /// </summary>
        private BitmapSource _srcImage;
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
        private BitmapSource _convertedSrcImage;
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
                var result = Img.CreateBitmapAndBitmapSourceByFilePath(dialog.FileName);
                if (result.src != null) { this.SrcImage = result.src; }
                if (result.bitmap != null) { this._bitmap = result.bitmap; }
            }
        }

        public ICommand ExcuteButtonPushed { get; set; }
        /// <summary>
        /// 現在チェックボックスで選択中の選択肢に応じて、画像処理を実行し、実行結果をConvertedSrcImageにセットする
        /// </summary>
        private void ExcuteButtonPushedCommand()
        {
            if (this._bitmap == null) return;
            Bitmap convertResult = null;

            switch (this.CurrentConvertMode)
            {
                case ConvertMode.ChannelSwap:
                    convertResult = Img.ChannelSwap(in this._bitmap);
                    break;
                case ConvertMode.Grayscale:
                    convertResult = Img.Grayscale(in this._bitmap);
                    break;
                case ConvertMode.Rainbow:
                    convertResult = Img.Rainbow(in this._bitmap, this.SliderValue);
                    break;
                default:
                    break;
            }
            if (convertResult == null) return;
            var result = Img.CreateBitmapSourceByBitmap(in convertResult);
            if (result == null) return;
            this.ConvertedSrcImage = result;
        }

        /// <summary>
        /// 変数の更新通知用
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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

        /// <summary>
        /// コンストラクタ 
        /// </summary>
        public MainWindowViewModel()
        {
            ReadImageButtonPushed = new RelayCommand(ReadImageButtonPushedCommand);
            ExcuteButtonPushed = new RelayCommand(ExcuteButtonPushedCommand);
            this._bitmap = null;
            this.CurrentConvertMode = ConvertMode.ChannelSwap;
            this.ConvertModeItemSource = MainWindowModel.ConvertModeItemSourceFactory();
            this.SliderValue = 0;
            this.SrcImage = null;
            this.ConvertedSrcImage = null;
            this.SliderEnabled = true;
        }

        /// <summary>
        /// 変換モードが変更された時のイベントを実行する
        /// </summary>
        /// <param name="value"></param>
        public void OnConvertModeSelectionChange(ConvertMode value)
        {
            this.SliderEnabled = MainWindowModel.IsEnableSlider(this._currentConvertMode);
            NotifyPropertyChanged("SliderEnabled");
        }

        /// <summary>
        /// スライドの値が変更された時のイベントを実行する
        /// </summary>
        /// <param name="value"></param>
        public void OnSliderValueChange(int value)
        {
            if (this.CurrentConvertMode != ConvertMode.Rainbow) return;
            if (this._bitmap == null) return;

            Bitmap convertResult = null;
            convertResult = Img.Rainbow(in this._bitmap, this.SliderValue);
            if (convertResult == null) return;
            var result = Img.CreateBitmapSourceByBitmap(in convertResult);
            if (result == null) return;
            this.ConvertedSrcImage = result;
        }
    }
}
