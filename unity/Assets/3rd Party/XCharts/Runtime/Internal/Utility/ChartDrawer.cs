/************************************************/
/*                                              */
/*     Copyright (c) 2018 - 2021 monitor1394    */
/*     https://github.com/monitor1394           */
/*                                              */
/************************************************/


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XUGL;

namespace XCharts
{
    public static class ChartDrawer
    {
        public static void DrawSymbol(VertexHelper vh, SerieSymbolType type, float symbolSize,
           float tickness, Vector3 pos, Color32 color, Color32 toColor, float gap, float[] cornerRadius,
           Color32 backgroundColor, float smoothness)
        {
            switch (type)
            {
                case SerieSymbolType.None:
                    break;
                case SerieSymbolType.Circle:
                    if (gap > 0)
                    {
                        UGL.DrawDoughnut(vh, pos, symbolSize, symbolSize + gap, backgroundColor, backgroundColor, color, smoothness);
                    }
                    else
                    {
                        UGL.DrawCricle(vh, pos, symbolSize, color, toColor, smoothness);
                    }
                    break;
                case SerieSymbolType.EmptyCircle:
                    if (gap > 0)
                    {
                        UGL.DrawCricle(vh, pos, symbolSize + gap, backgroundColor, smoothness);
                        UGL.DrawEmptyCricle(vh, pos, symbolSize, tickness, color, color, backgroundColor, smoothness);
                    }
                    else
                    {
                        UGL.DrawEmptyCricle(vh, pos, symbolSize, tickness, color, color, backgroundColor, smoothness);
                    }
                    break;
                case SerieSymbolType.Rect:
                    if (gap > 0)
                    {
                        UGL.DrawSquare(vh, pos, symbolSize + gap, backgroundColor);
                        UGL.DrawSquare(vh, pos, symbolSize, color, toColor);
                    }
                    else
                    {
                        UGL.DrawRoundRectangle(vh, pos, symbolSize, symbolSize, color, color, 0, cornerRadius, true);
                    }
                    break;
                case SerieSymbolType.Triangle:
                    if (gap > 0)
                    {
                        UGL.DrawTriangle(vh, pos, symbolSize + gap, backgroundColor);
                        UGL.DrawTriangle(vh, pos, symbolSize, color, toColor);
                    }
                    else
                    {
                        UGL.DrawTriangle(vh, pos, symbolSize, color, toColor);
                    }
                    break;
                case SerieSymbolType.Diamond:
                    if (gap > 0)
                    {
                        UGL.DrawDiamond(vh, pos, symbolSize + gap, backgroundColor);
                        UGL.DrawDiamond(vh, pos, symbolSize, color, toColor);
                    }
                    else
                    {
                        UGL.DrawDiamond(vh, pos, symbolSize, color, toColor);
                    }
                    break;
            }
        }

        // public static void DrawLineStyle(VertexHelper vh, LineStyle lineStyle,
        //     Vector3 startPos, Vector3 endPos, Color32 color, float themeWidth)
        // {
        //     var type = lineStyle.type;
        //     var width = lineStyle.GetWidth(themeWidth);
        //     DrawLineStyle(vh, type, width, startPos, endPos, color);
        // }

        public static void DrawLineStyle(VertexHelper vh, LineStyle lineStyle,
        Vector3 startPos, Vector3 endPos, Color32 color, float themeWidth, LineStyle.Type themeType)
        {
            var type = lineStyle.GetType(themeType);
            var width = lineStyle.GetWidth(themeWidth);
            DrawLineStyle(vh, type, width, startPos, endPos, color);
        }

        public static void DrawLineStyle(VertexHelper vh, LineStyle.Type lineType, float lineWidth,
            Vector3 startPos, Vector3 endPos, Color32 color)
        {
            switch (lineType)
            {
                case LineStyle.Type.Dashed:
                    UGL.DrawDashLine(vh, startPos, endPos, lineWidth, color);
                    break;
                case LineStyle.Type.Dotted:
                    UGL.DrawDotLine(vh, startPos, endPos, lineWidth, color);
                    break;
                case LineStyle.Type.Solid:
                    UGL.DrawLine(vh, startPos, endPos, lineWidth, color);
                    break;
                case LineStyle.Type.DashDot:
                    UGL.DrawDashDotLine(vh, startPos, endPos, lineWidth, color);
                    break;
                case LineStyle.Type.DashDotDot:
                    UGL.DrawDashDotDotLine(vh, startPos, endPos, lineWidth, color);
                    break;
            }
        }
    }
}