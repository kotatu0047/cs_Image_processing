using cs_Image_processing.lib.CustomAttributes;
using System;
using System.Collections.Generic;

namespace cs_Image_processing.Model
{
    /// <summary>
    /// 選択可能な変換モード一覧
    /// </summary>
    public enum ConvertMode
    {
        [UiDisplayNameAttribute("チャネル変換")]
        ChannelSwap = 1,
        [UiDisplayNameAttribute("グレースケール")]
        Grayscale,
        [UiDisplayNameAttribute("レインボー")]
        Rainbow,
    }

    /// <summary>
    /// MainWindowのModel
    /// </summary>
    public static class MainWindowModel
    {
        /// <summary>
        /// セレクトボックスに表示する、変換モードの選択肢を生成
        /// </summary>
        /// <returns></returns>
        public static Dictionary<ConvertMode, string> ConvertModeItemSourceFactory()
        {
            Dictionary<ConvertMode, string> itemSource = new Dictionary<ConvertMode, string>();
            foreach (ConvertMode convertMode in (ConvertMode[])Enum.GetValues(typeof(ConvertMode)))
            {
                itemSource.Add(convertMode, convertMode.GetDisplayName<UiDisplayNameAttribute>());
            }

            return itemSource;
        }

        /// <summary>
        ///  スライダーUiを有効/無効化するか決定する
        /// </summary>
        /// <param name="currentMode">現在選択中の変換モード</param>
        /// <returns></returns>
        public static bool IsEnableSlider(ConvertMode currentMode)
        {
            switch (currentMode)
            {
                case ConvertMode.ChannelSwap:
                case ConvertMode.Grayscale:
                    return false;
                case ConvertMode.Rainbow:
                    return true;
                default: return false;
            }
        }
    }
}
