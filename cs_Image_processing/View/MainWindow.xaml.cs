using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using cs_Image_processing.ViewModel;
using cs_Image_processing.Model;

namespace cs_Image_processing.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        /// <summary>
        /// スライダー操作時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            var viewModel = (MainWindowViewModel)(this.DataContext);
            viewModel.OnSliderValueChange((int)slider.Value);
        }

        /// <summary>
        /// 変換モード選択時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var viewModel = (MainWindowViewModel)(this.DataContext);
            viewModel.OnConvertModeSelectionChange(((KeyValuePair<ConvertMode , string>)comboBox.SelectedItem).Key);
        }
    }
}
