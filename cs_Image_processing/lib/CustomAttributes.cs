using System;
using System.Reflection;

namespace cs_Image_processing.lib.CustomAttributes
{
    /// <summary>
    /// getDisplayNameメソッドの実装を誓約させるインターフェース
    /// </summary>
    public interface IDisplayName
    {
        string getDisplayName();
    }


    /// <summary>
    /// Ui表示用名を持つカスタムEnum属性
    /// </summary>
    public class UiDisplayNameAttribute : Attribute, IDisplayName
    {
        /// <summary>
        /// Ui表示用
        /// </summary>
        private string _displayname;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="displayname">Ui表示用名</param>
        public UiDisplayNameAttribute(string displayname) => this._displayname = displayname;

        /// <summary>
        /// IDisplayNameインターフェースの実装
        /// </summary>
        /// <returns>DisplayName</returns>
        public string getDisplayName()
        {
            return this._displayname;
        }
    }

    /// <summary>
    ///  Enum型の拡張メソッドを定義 
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Attribute,IDisplayName を継承した属性を持つEnumから、DisplayNameを取得する、
        /// 見つからない場合空文字を返す 
        /// </summary>
        /// <typeparam name="T">Attribute,IDisplayName を継承したカスタム属性</typeparam>
        /// <param name="value">拡張メソッドの呼び出し自身のEnum</param>
        /// <returns>DisplayName</returns>
        public static string GetDisplayName<T>(this Enum value) where T : Attribute, IDisplayName
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field.GetCustomAttribute<T>() is T att)
            {
                return att.getDisplayName();
            }

            return "";
        }
    }
}
