using System;
using System.Drawing;

namespace ImageProcess
{
    /// <summary>
    /// Class HsvColor
    /// </summary>
    public static class HsvColor
    {
        /// <summary>
        /// 指定したHsvColorからColorを作成する
        /// </summary>
        /// <param name="hsv">HsvColor</param>
        /// <returns>Color</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Color ToRgb(float v, float s, float arg_h)
        {
            float r, g, b;
            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                float h = arg_h / 60f;
                int i = (int)Math.Floor(h);
                float f = h - i;
                float p = v * (1f - s);
                float q;
                if (i % 2 == 0)
                {
                    //t
                    q = v * (1f - (1f - f) * s);
                }
                else
                {
                    q = v * (1f - f * s);
                }

                switch (i)
                {
                    case 0:
                        r = v;
                        g = q;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = v;
                        b = q;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;
                    case 4:
                        r = q;
                        g = p;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = p;
                        b = q;
                        break;
                    default:
                        throw new ArgumentException(
                            "色相の値が不正です。", "hsv");
                }
            }

            return Color.FromArgb(
                (int)Math.Round(r * 255f),
                (int)Math.Round(g * 255f),
                (int)Math.Round(b * 255f));
        }
    }


}
