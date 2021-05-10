/************************************************/
/*                                              */
/*     Copyright (c) 2018 - 2021 monitor1394    */
/*     https://github.com/monitor1394           */
/*                                              */
/************************************************/


using UnityEngine;

namespace XCharts
{
    /// <summary>
    /// The style of line.
    /// 线条样式。
    /// 注： 修改 lineStyle 中的颜色不会影响图例颜色，如果需要图例颜色和折线图颜色一致，需修改 itemStyle.color，线条颜色默认也会取该颜色。
    /// toColor，toColor2可设置水平方向的渐变，如需要设置垂直方向的渐变，可使用VisualMap。
    /// </summary>
    [System.Serializable]
    public class LineStyle : SubComponent
    {
        /// <summary>
        /// 线的类型。
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 实线
            /// </summary>
            Solid,
            /// <summary>
            /// 虚线
            /// </summary>
            Dashed,
            /// <summary>
            /// 点线
            /// </summary>
            Dotted,
            /// <summary>
            /// 点划线
            /// </summary>
            DashDot,
            /// <summary>
            /// 双点划线
            /// </summary>
            DashDotDot,
            None,
        }
        [SerializeField] private bool m_Show = true;
        [SerializeField] private Type m_Type = Type.Solid;
        [SerializeField] private Color32 m_Color;
        [SerializeField] private Color32 m_ToColor;
        [SerializeField] private Color32 m_ToColor2;
        [SerializeField] private float m_Width = 0;
        [SerializeField] private float m_Length = 0;
        [SerializeField] [Range(0, 1)] private float m_Opacity = 1;

        /// <summary>
        /// Whether show line.
        /// 是否显示线条。当作为子组件，它的父组件有参数控制是否显示时，改参数无效。
        /// </summary>
        public bool show
        {
            get { return m_Show; }
            set { if (PropertyUtil.SetStruct(ref m_Show, value)) SetVerticesDirty(); }
        }
        /// <summary>
        /// the type of line.
        /// 线的类型。
        /// </summary>
        public Type type
        {
            get { return m_Type; }
            set { if (PropertyUtil.SetStruct(ref m_Type, value)) SetVerticesDirty(); }
        }
        /// <summary>
        /// the color of line, default use serie color.
        /// 线的颜色。
        /// </summary>
        public Color32 color
        {
            get { return m_Color; }
            set { if (PropertyUtil.SetColor(ref m_Color, value)) SetVerticesDirty(); }
        }
        /// <summary>
        /// the middle color of line, default use serie color.
        /// 线的渐变颜色（需要水平方向渐变时）。
        /// </summary>
        public Color32 toColor
        {
            get { return m_ToColor; }
            set { if (PropertyUtil.SetColor(ref m_ToColor, value)) SetVerticesDirty(); }
        }
        /// <summary>
        /// the end color of line, default use serie color.
        /// 线的渐变颜色2（需要水平方向三个渐变色的渐变时）。
        /// </summary>
        public Color32 toColor2
        {
            get { return m_ToColor2; }
            set { if (PropertyUtil.SetColor(ref m_ToColor2, value)) SetVerticesDirty(); }
        }
        /// <summary>
        /// the width of line.
        /// 线宽。
        /// /// </summary>
        public float width
        {
            get { return m_Width; }
            set { if (PropertyUtil.SetStruct(ref m_Width, value)) SetVerticesDirty(); }
        }
        /// <summary>
        /// the length of line.
        /// 线长。
        /// /// </summary>
        public float length
        {
            get { return m_Length; }
            set { if (PropertyUtil.SetStruct(ref m_Length, value)) SetVerticesDirty(); }
        }
        /// <summary>
        /// Opacity of the line. Supports value from 0 to 1, and the line will not be drawn when set to 0.
        /// 线的透明度。支持从 0 到 1 的数字，为 0 时不绘制该图形。
        /// </summary>
        public float opacity
        {
            get { return m_Opacity; }
            set { if (PropertyUtil.SetStruct(ref m_Opacity, value)) SetVerticesDirty(); }
        }

        public LineStyle()
        {
        }

        public LineStyle(float width)
        {
            this.width = width;
        }

        public LineStyle(LineStyle.Type type)
        {
            this.type = type;
        }

        public LineStyle(LineStyle.Type type, float width)
        {
            this.type = type;
            this.width = width;
        }

        public LineStyle Clone()
        {
            var lineStyle = new LineStyle();
            lineStyle.show = show;
            lineStyle.type = type;
            lineStyle.color = color;
            lineStyle.toColor = toColor;
            lineStyle.toColor2 = toColor2;
            lineStyle.width = width;
            lineStyle.opacity = opacity;
            return lineStyle;
        }

        public void Copy(LineStyle lineStyle)
        {
            show = lineStyle.show;
            type = lineStyle.type;
            color = lineStyle.color;
            toColor = lineStyle.toColor;
            toColor2 = lineStyle.toColor2;
            width = lineStyle.width;
            opacity = lineStyle.opacity;
        }

        public Color32 GetColor()
        {
            if (m_Opacity == 1) return m_Color;
            var color = m_Color;
            color.a = (byte)(color.a * m_Opacity);
            return color;
        }

        public bool IsNeedGradient()
        {
            return !ChartHelper.IsClearColor(m_ToColor) || !ChartHelper.IsClearColor(m_ToColor2);
        }

        public Color32 GetGradientColor(float value, Color32 defaultColor)
        {
            var color = ChartConst.clearColor32;
            if (!IsNeedGradient()) return color;
            value = Mathf.Clamp01(value);
            var startColor = ChartHelper.IsClearColor(m_Color) ? defaultColor : m_Color;
            if (!ChartHelper.IsClearColor(m_ToColor2))
            {
                if (value <= 0.5f) color = Color32.Lerp(startColor, m_ToColor, 2 * value);
                else color = Color32.Lerp(m_ToColor, m_ToColor2, 2 * (value - 0.5f));
            }
            else
            {
                color = Color32.Lerp(startColor, m_ToColor, value);
            }
            if (m_Opacity != 1)
            {
                color.a = (byte)(color.a * m_Opacity);
            }
            return color;
        }

        public Type GetType(Type themeType)
        {
            return type == Type.None ? themeType : type;
        }
        
        public float GetWidth(float themeWidth)
        {
            return width == 0 ? themeWidth : width;
        }

        public float GetLength(float themeLength)
        {
            return length == 0 ? themeLength : length;
        }

        public Color32 GetColor(Color32 themeColor)
        {
            if (!ChartHelper.IsClearColor(color))
            {
                return GetColor();
            }
            else
            {
                var color = themeColor;
                color.a = (byte)(color.a * opacity);
                return color;
            }
        }
    }
}