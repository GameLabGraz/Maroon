/************************************************/
/*                                              */
/*     Copyright (c) 2018 - 2021 monitor1394    */
/*     https://github.com/monitor1394           */
/*                                              */
/************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XUGL;

namespace XCharts
{
    public partial class CoordinateChart
    {
        protected Action<PointerEventData, int> m_OnPointerClickBar;

        protected void DrawYBarSerie(VertexHelper vh, Serie serie, int colorIndex)
        {
            if (!IsActive(serie.name)) return;
            if (serie.animation.HasFadeOut()) return;
            var xAxis = m_XAxes[serie.xAxisIndex];
            var yAxis = m_YAxes[serie.yAxisIndex];
            var grid = GetSerieGridOrDefault(serie);
            var showData = serie.GetDataList(dataZoom);
            float categoryWidth = AxisHelper.GetDataWidth(yAxis, grid.runtimeHeight, showData.Count, dataZoom);
            float barGap = GetBarGap();
            float totalBarWidth = GetBarTotalWidth(categoryWidth, barGap);
            float barWidth = serie.GetBarWidth(categoryWidth);
            float offset = (categoryWidth - totalBarWidth) / 2;
            float barGapWidth = barWidth + barWidth * barGap;
            float space = serie.barGap == -1 ? offset : offset + GetBarIndex(serie) * barGapWidth;
            var isStack = SeriesHelper.IsStack(m_Series, serie.stack, SerieType.Bar);
            m_StackSerieData.Clear();
            if (isStack) SeriesHelper.UpdateStackDataList(m_Series, serie, dataZoom, m_StackSerieData);

            int maxCount = serie.maxShow > 0 ?
                (serie.maxShow > showData.Count ? showData.Count : serie.maxShow)
                : showData.Count;
            var isPercentStack = SeriesHelper.IsPercentStack(m_Series, serie.stack, SerieType.Bar);
            bool dataChanging = false;
            float dataChangeDuration = serie.animation.GetUpdateAnimationDuration();
            float xMinValue = xAxis.GetCurrMinValue(dataChangeDuration);
            float xMaxValue = xAxis.GetCurrMaxValue(dataChangeDuration);
            var isAllBarEnd = true;
            for (int i = serie.minShow; i < maxCount; i++)
            {
                var serieData = showData[i];
                if (serie.IsIgnoreValue(serieData.GetData(1)))
                {
                    serie.dataPoints.Add(Vector3.zero);
                    continue;
                }
                var highlight = (tooltip.show && tooltip.IsSelected(i))
                    || serie.data[i].highlighted
                    || serie.highlighted;
                var itemStyle = SerieHelper.GetItemStyle(serie, serieData, highlight);

                serieData.canShowLabel = true;
                float value = showData[i].GetCurrData(1, dataChangeDuration, xAxis.inverse, xMinValue, xMaxValue);
                float borderWidth = value == 0 ? 0 : itemStyle.runtimeBorderWidth;
                if (showData[i].IsDataChanged()) dataChanging = true;
                float axisLineWidth = value == 0 ? 0
                    : ((value < 0 ? -1 : 1) * yAxis.axisLine.GetWidth(m_Theme.axis.lineWidth));

                float pY = grid.runtimeY + i * categoryWidth;
                if (!yAxis.boundaryGap) pY -= categoryWidth / 2;
                float pX = grid.runtimeX + xAxis.runtimeZeroXOffset + axisLineWidth;
                if (isStack)
                {
                    for (int n = 0; n < m_StackSerieData.Count - 1; n++)
                    {
                        pX += m_StackSerieData[n][i].runtimeStackHig;
                    }
                }

                var barHig = 0f;
                var valueTotal = 0f;
                if (isPercentStack)
                {
                    valueTotal = GetSameStackTotalValue(serie.stack, i);
                    barHig = valueTotal != 0 ? (value / valueTotal * grid.runtimeWidth) : 0;
                }
                else
                {
                    valueTotal = xMaxValue - xMinValue;
                    if (valueTotal != 0)
                        barHig = (xMinValue > 0 ? value - xMinValue : value)
                            / valueTotal * grid.runtimeWidth;
                }
                serieData.runtimeStackHig = barHig;
                var isBarEnd = false;
                float currHig = CheckAnimation(serie, i, barHig, out isBarEnd);
                if (!isBarEnd) isAllBarEnd = false;
                Vector3 plt, prt, prb, plb, top;
                if (value < 0)
                {
                    plt = new Vector3(pX - borderWidth, pY + space + barWidth - borderWidth);
                    prt = new Vector3(pX + currHig + borderWidth, pY + space + barWidth - borderWidth);
                    prb = new Vector3(pX + currHig + borderWidth, pY + space + borderWidth);
                    plb = new Vector3(pX - borderWidth, pY + space + borderWidth);
                }
                else
                {
                    plt = new Vector3(pX + borderWidth, pY + space + barWidth - borderWidth);
                    prt = new Vector3(pX + currHig - borderWidth , pY + space + barWidth - borderWidth);
                    prb = new Vector3(pX + currHig - borderWidth , pY + space + borderWidth);
                    plb = new Vector3(pX + borderWidth, pY + space + borderWidth);
                }
                top = new Vector3(pX + currHig - borderWidth, pY + space + barWidth / 2);
                plt = ClampInGrid(grid, plt);
                prt = ClampInGrid(grid, prt);
                prb = ClampInGrid(grid, prb);
                plb = ClampInGrid(grid, plb);
                top = ClampInGrid(grid, top);
                serie.dataPoints.Add(top);
                if (serie.show)
                {
                    switch (serie.barType)
                    {
                        case BarType.Normal:
                            DrawNormalBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, true, grid);
                            break;
                        case BarType.Zebra:
                            DrawZebraBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, true, grid);
                            break;
                        case BarType.Capsule:
                            DrawCapsuleBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, true, grid);
                            break;
                    }
                }
            }
            if (isAllBarEnd) serie.animation.AllBarEnd();
            if (dataChanging)
            {
                RefreshPainter(serie);
            }
        }

        private float CheckAnimation(Serie serie, int dataIndex, float barHig, out bool isBarEnd)
        {
            float currHig = serie.animation.CheckBarProgress(dataIndex, barHig, serie.dataCount, out isBarEnd);
            if (!serie.animation.IsFinish())
            {
                RefreshPainter(serie);
                m_IsPlayingAnimation = true;
            }
            return currHig;
        }

        protected void DrawXBarSerie(VertexHelper vh, Serie serie, int colorIndex)
        {
            if (!IsActive(serie.index)) return;
            if (serie.animation.HasFadeOut()) return;
            var showData = serie.GetDataList(dataZoom);
            var yAxis = m_YAxes[serie.yAxisIndex];
            var xAxis = m_XAxes[serie.xAxisIndex];
            var grid = GetSerieGridOrDefault(serie);
            var isStack = SeriesHelper.IsStack(m_Series, serie.stack, SerieType.Bar);
            m_StackSerieData.Clear();
            if (isStack) SeriesHelper.UpdateStackDataList(m_Series, serie, dataZoom, m_StackSerieData);
            float categoryWidth = AxisHelper.GetDataWidth(xAxis, grid.runtimeWidth, showData.Count, dataZoom);
            float barGap = GetBarGap();
            float totalBarWidth = GetBarTotalWidth(categoryWidth, barGap);
            float barWidth = serie.GetBarWidth(categoryWidth);
            float offset = (categoryWidth - totalBarWidth) / 2;
            float barGapWidth = barWidth + barWidth * barGap;
            float space = serie.barGap == -1 ? offset : offset + GetBarIndex(serie) * barGapWidth;
            int maxCount = serie.maxShow > 0
                ? (serie.maxShow > showData.Count ? showData.Count : serie.maxShow)
                : showData.Count;

            var isPercentStack = SeriesHelper.IsPercentStack(m_Series, serie.stack, SerieType.Bar);
            bool dataChanging = false;
            float dataChangeDuration = serie.animation.GetUpdateAnimationDuration();
            float yMinValue = yAxis.GetCurrMinValue(dataChangeDuration);
            float yMaxValue = yAxis.GetCurrMaxValue(dataChangeDuration);
            var isAllBarEnd = true;
            for (int i = serie.minShow; i < maxCount; i++)
            {
                var serieData = showData[i];
                if (serie.IsIgnoreValue(serieData.GetData(1)))
                {
                    serie.dataPoints.Add(Vector3.zero);
                    continue;
                }
                var highlight = (tooltip.show && tooltip.IsSelected(i))
                    || serie.data[i].highlighted
                    || serie.highlighted;
                var itemStyle = SerieHelper.GetItemStyle(serie, serieData, highlight);
                float value = serieData.GetCurrData(1, dataChangeDuration, yAxis.inverse, yMinValue, yMaxValue);
                float borderWidth = value == 0 ? 0 : itemStyle.runtimeBorderWidth;
                if (serieData.IsDataChanged()) dataChanging = true;
                float pX = grid.runtimeX + i * categoryWidth;
                float zeroY = grid.runtimeY + yAxis.runtimeZeroYOffset;
                if (!xAxis.boundaryGap) pX -= categoryWidth / 2;
                float axisLineWidth = value == 0 ? 0 :
                     ((value < 0 ? -1 : 1) * xAxis.axisLine.GetWidth(m_Theme.axis.lineWidth));
                float pY = zeroY + axisLineWidth;
                if (isStack)
                {
                    for (int n = 0; n < m_StackSerieData.Count - 1; n++)
                    {
                        pY += m_StackSerieData[n][i].runtimeStackHig;
                    }
                }

                var barHig = 0f;
                var valueTotal = 0f;
                if (isPercentStack)
                {
                    valueTotal = GetSameStackTotalValue(serie.stack, i);
                    barHig = valueTotal != 0 ? (value / valueTotal * grid.runtimeHeight) : 0;
                }
                else
                {
                    valueTotal = yMaxValue - yMinValue;
                    if (valueTotal != 0)
                    {
                        barHig = (yMinValue > 0 ? value - yMinValue : value) / valueTotal * grid.runtimeHeight;
                    }
                }
                serieData.runtimeStackHig = barHig;
                var isBarEnd = false;
                float currHig = CheckAnimation(serie, i, barHig, out isBarEnd);
                if (!isBarEnd) isAllBarEnd = false;
                Vector3 plb, plt, prt, prb, top;
                if (value < 0)
                {
                    plb = new Vector3(pX + space + borderWidth, pY - borderWidth);
                    plt = new Vector3(pX + space + borderWidth, pY + currHig + borderWidth);
                    prt = new Vector3(pX + space + barWidth - borderWidth, pY + currHig + borderWidth);
                    prb = new Vector3(pX + space + barWidth - borderWidth, pY - borderWidth);
                }
                else
                {
                    plb = new Vector3(pX + space + borderWidth, pY + borderWidth);
                    plt = new Vector3(pX + space + borderWidth, pY + currHig - borderWidth);
                    prt = new Vector3(pX + space + barWidth - borderWidth, pY + currHig - borderWidth);
                    prb = new Vector3(pX + space + barWidth - borderWidth, pY + borderWidth);
                }
                top = new Vector3(pX + space + barWidth / 2, pY + currHig - borderWidth);
                if (serie.clip)
                {
                    plb = ClampInGrid(grid, plb);
                    plt = ClampInGrid(grid, plt);
                    prt = ClampInGrid(grid, prt);
                    prb = ClampInGrid(grid, prb);
                    top = ClampInGrid(grid, top);
                }
                serie.dataPoints.Add(top);
                if (serie.show && currHig != 0)
                {
                    switch (serie.barType)
                    {
                        case BarType.Normal:
                            DrawNormalBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, false, grid);
                            break;
                        case BarType.Zebra:
                            DrawZebraBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, false, grid);
                            break;
                        case BarType.Capsule:
                            DrawCapsuleBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                               pX, pY, plb, plt, prt, prb, false, grid);
                            break;
                    }
                }
            }
            if (isAllBarEnd)
            {
                serie.animation.AllBarEnd();
            }
            if (dataChanging)
            {
                RefreshPainter(serie);
            }
        }

        private void DrawNormalBar(VertexHelper vh, Serie serie, SerieData serieData, ItemStyle itemStyle, int colorIndex,
            bool highlight, float space, float barWidth, float pX, float pY, Vector3 plb, Vector3 plt, Vector3 prt,
            Vector3 prb, bool isYAxis, Grid grid)
        {
            var areaColor = SerieHelper.GetItemColor(serie, serieData, m_Theme, colorIndex, highlight);
            var areaToColor = SerieHelper.GetItemToColor(serie, serieData, m_Theme, colorIndex, highlight);
            DrawBarBackground(vh, serie, serieData, itemStyle, colorIndex, highlight, pX, pY, space, barWidth, isYAxis, grid);
            var borderWidth = itemStyle.runtimeBorderWidth;
            if (isYAxis)
            {
                if (serie.clip)
                {
                    prb = ClampInGrid(grid, prb);
                    plb = ClampInGrid(grid, plb);
                    plt = ClampInGrid(grid, plt);
                    prt = ClampInGrid(grid, prt);
                }
                var borderColor = itemStyle.borderColor;
                var itemWidth = Mathf.Abs(prb.x - plt.x);
                var itemHeight = Mathf.Abs(prt.y - plb.y);
                var center = new Vector3((plt.x + prb.x) / 2, (prt.y + plb.y) / 2);
                if (itemWidth > 0 && itemHeight > 0)
                {
                    if (ItemStyleHelper.IsNeedCorner(itemStyle))
                    {
                        UGL.DrawRoundRectangle(vh, center, itemWidth, itemHeight, areaColor, areaToColor, 0,
                            itemStyle.cornerRadius, isYAxis);
                    }
                    else
                    {
                        CheckClipAndDrawPolygon(vh, plb, plt, prt, prb, areaColor, areaToColor, serie.clip, grid);
                    }
                    UGL.DrawBorder(vh, center, itemWidth, itemHeight, borderWidth, borderColor, 0,
                        itemStyle.cornerRadius, isYAxis);
                }
            }
            else
            {
                if (serie.clip)
                {
                    prb = ClampInGrid(grid, prb);
                    plb = ClampInGrid(grid, plb);
                    plt = ClampInGrid(grid, plt);
                    prt = ClampInGrid(grid, prt);
                }
                var borderColor = itemStyle.borderColor;
                var itemWidth = Mathf.Abs(prt.x - plb.x);
                var itemHeight = Mathf.Abs(plt.y - prb.y);
                var center = new Vector3((plb.x + prt.x) / 2, (plt.y + prb.y) / 2);
                if (itemWidth > 0 && itemHeight > 0)
                {
                    if (ItemStyleHelper.IsNeedCorner(itemStyle))
                    {
                        UGL.DrawRoundRectangle(vh, center, itemWidth, itemHeight, areaColor, areaToColor, 0,
                            itemStyle.cornerRadius, isYAxis);
                    }
                    else
                    {
                        CheckClipAndDrawPolygon(vh, ref prb, ref plb, ref plt, ref prt, areaColor, areaToColor,
                            serie.clip, grid);
                    }
                    UGL.DrawBorder(vh, center, itemWidth, itemHeight, borderWidth, borderColor, 0,
                        itemStyle.cornerRadius, isYAxis);
                }
            }
        }

        private void DrawZebraBar(VertexHelper vh, Serie serie, SerieData serieData, ItemStyle itemStyle, int colorIndex,
            bool highlight, float space, float barWidth, float pX, float pY, Vector3 plb, Vector3 plt, Vector3 prt,
            Vector3 prb, bool isYAxis, Grid grid)
        {
            var areaColor = SerieHelper.GetItemColor(serie, serieData, m_Theme, colorIndex, highlight);
            DrawBarBackground(vh, serie, serieData, itemStyle, colorIndex, highlight, pX, pY, space, barWidth, isYAxis, grid);
            if (isYAxis)
            {
                plt = (plb + plt) / 2;
                prt = (prt + prb) / 2;
                CheckClipAndDrawZebraLine(vh, plt, prt, barWidth / 2, serie.barZebraWidth, serie.barZebraGap,
                    areaColor, serie.clip, grid);
            }
            else
            {
                plb = (prb + plb) / 2;
                plt = (plt + prt) / 2;
                CheckClipAndDrawZebraLine(vh, plb, plt, barWidth / 2, serie.barZebraWidth, serie.barZebraGap,
                    areaColor, serie.clip, grid);
            }
        }

        private void DrawCapsuleBar(VertexHelper vh, Serie serie, SerieData serieData, ItemStyle itemStyle, int colorIndex,
            bool highlight, float space, float barWidth, float pX, float pY, Vector3 plb, Vector3 plt, Vector3 prt,
            Vector3 prb, bool isYAxis, Grid grid)
        {
            var areaColor = SerieHelper.GetItemColor(serie, serieData, m_Theme, colorIndex, highlight);
            var areaToColor = SerieHelper.GetItemToColor(serie, serieData, m_Theme, colorIndex, highlight);
            DrawBarBackground(vh, serie, serieData, itemStyle, colorIndex, highlight, pX, pY, space, barWidth, isYAxis, grid);
            var borderWidth = itemStyle.runtimeBorderWidth;
            var radius = barWidth / 2 - borderWidth;
            var isGradient = !ChartHelper.IsValueEqualsColor(areaColor, areaToColor);
            if (isYAxis)
            {
                var diff = Vector3.right * radius;
                if (plt.x < prt.x)
                {
                    var pcl = (plt + plb) / 2 + diff;
                    var pcr = (prt + prb) / 2 - diff;
                    if (pcr.x > pcl.x)
                    {
                        if (isGradient)
                        {
                            var barLen = prt.x - plt.x;
                            var rectStartColor = Color32.Lerp(areaColor, areaToColor, radius / barLen);
                            var rectEndColor = Color32.Lerp(areaColor, areaToColor, (barLen - radius) / barLen);
                            CheckClipAndDrawPolygon(vh, plb + diff, plt + diff, prt - diff, prb - diff, rectStartColor,
                                rectEndColor, serie.clip, grid);
                            UGL.DrawSector(vh, pcl, radius, areaColor, rectStartColor, 180, 360, 1, isYAxis);
                            UGL.DrawSector(vh, pcr, radius, rectEndColor, areaToColor, 0, 180, 1, isYAxis);
                        }
                        else
                        {
                            CheckClipAndDrawPolygon(vh, plb + diff, plt + diff, prt - diff, prb - diff, areaColor,
                                areaToColor, serie.clip, grid);
                            UGL.DrawSector(vh, pcl, radius, areaColor, 180, 360);
                            UGL.DrawSector(vh, pcr, radius, areaToColor, 0, 180);
                        }
                    }
                }
                else if (plt.x > prt.x)
                {
                    var pcl = (plt + plb) / 2 - diff;
                    var pcr = (prt + prb) / 2 + diff;
                    if (pcr.x < pcl.x)
                    {
                        if (isGradient)
                        {
                            var barLen = plt.x - prt.x;
                            var rectStartColor = Color32.Lerp(areaColor, areaToColor, radius / barLen);
                            var rectEndColor = Color32.Lerp(areaColor, areaToColor, (barLen - radius) / barLen);
                            CheckClipAndDrawPolygon(vh, plb - diff, plt - diff, prt + diff, prb + diff, rectStartColor,
                                rectEndColor, serie.clip, grid);
                            UGL.DrawSector(vh, pcl, radius, rectStartColor, areaColor, 0, 180, 1, isYAxis);
                            UGL.DrawSector(vh, pcr, radius, areaToColor, rectEndColor, 180, 360, 1, isYAxis);
                        }
                        else
                        {
                            CheckClipAndDrawPolygon(vh, plb - diff, plt - diff, prt + diff, prb + diff, areaColor,
                                areaToColor, serie.clip, grid);
                            UGL.DrawSector(vh, pcl, radius, areaColor, 0, 180);
                            UGL.DrawSector(vh, pcr, radius, areaToColor, 180, 360);
                        }
                    }
                }
            }
            else
            {
                var diff = Vector3.up * radius;
                if (plt.y > plb.y)
                {
                    var pct = (plt + prt) / 2 - diff;
                    var pcb = (plb + prb) / 2 + diff;
                    if (pct.y > pcb.y)
                    {
                        if (isGradient)
                        {
                            var barLen = plt.y - plb.y;
                            var rectStartColor = Color32.Lerp(areaColor, areaToColor, radius / barLen);
                            var rectEndColor = Color32.Lerp(areaColor, areaToColor, (barLen - radius) / barLen);
                            CheckClipAndDrawPolygon(vh, prb + diff, plb + diff, plt - diff, prt - diff, rectStartColor,
                                rectEndColor, serie.clip, grid);
                            UGL.DrawSector(vh, pct, radius, rectEndColor, areaToColor, 270, 450, 1, isYAxis);
                            UGL.DrawSector(vh, pcb, radius, rectStartColor, areaColor, 90, 270, 1, isYAxis);
                        }
                        else
                        {
                            CheckClipAndDrawPolygon(vh, prb + diff, plb + diff, plt - diff, prt - diff, areaColor,
                                areaToColor, serie.clip, grid);
                            UGL.DrawSector(vh, pct, radius, areaToColor, 270, 450);
                            UGL.DrawSector(vh, pcb, radius, areaColor, 90, 270);
                        }
                    }
                }
                else if (plt.y < plb.y)
                {
                    var pct = (plt + prt) / 2 + diff;
                    var pcb = (plb + prb) / 2 - diff;
                    if (pct.y < pcb.y)
                    {
                        if (isGradient)
                        {
                            var barLen = plb.y - plt.y;
                            var rectStartColor = Color32.Lerp(areaColor, areaToColor, radius / barLen);
                            var rectEndColor = Color32.Lerp(areaColor, areaToColor, (barLen - radius) / barLen);
                            CheckClipAndDrawPolygon(vh, prb - diff, plb - diff, plt + diff, prt + diff, rectStartColor,
                                rectEndColor, serie.clip, grid);
                            UGL.DrawSector(vh, pct, radius, rectEndColor, areaToColor, 90, 270, 1, isYAxis);
                            UGL.DrawSector(vh, pcb, radius, rectStartColor, areaColor, 270, 450, 1, isYAxis);
                        }
                        else
                        {
                            CheckClipAndDrawPolygon(vh, prb - diff, plb - diff, plt + diff, prt + diff, areaColor,
                                areaToColor, serie.clip, grid);
                            UGL.DrawSector(vh, pct, radius, areaToColor, 90, 270);
                            UGL.DrawSector(vh, pcb, radius, areaColor, 270, 450);
                        }
                    }
                }
            }
        }

        private void DrawBarBackground(VertexHelper vh, Serie serie, SerieData serieData, ItemStyle itemStyle,
            int colorIndex, bool highlight, float pX, float pY, float space, float barWidth, bool isYAxis, Grid grid)
        {
            var color = SerieHelper.GetItemBackgroundColor(serie, serieData, m_Theme, colorIndex, highlight, false);
            if (ChartHelper.IsClearColor(color)) return;
            if (isYAxis)
            {
                var axis = m_YAxes[serie.yAxisIndex];
                var axisWidth = axis.axisLine.GetWidth(m_Theme.axis.lineWidth);
                Vector3 plt = new Vector3(grid.runtimeX + axisWidth, pY + space + barWidth);
                Vector3 prt = new Vector3(grid.runtimeX + axisWidth + grid.runtimeWidth, pY + space + barWidth);
                Vector3 prb = new Vector3(grid.runtimeX + axisWidth + grid.runtimeWidth, pY + space);
                Vector3 plb = new Vector3(grid.runtimeX + axisWidth, pY + space);
                if (serie.barType == BarType.Capsule)
                {
                    var radius = barWidth / 2;
                    var diff = Vector3.right * radius;
                    var pcl = (plt + plb) / 2 + diff;
                    var pcr = (prt + prb) / 2 - diff;
                    CheckClipAndDrawPolygon(vh, plb + diff, plt + diff, prt - diff, prb - diff, color, color, serie.clip, grid);
                    UGL.DrawSector(vh, pcl, radius, color, 180, 360);
                    UGL.DrawSector(vh, pcr, radius, color, 0, 180);
                    if (itemStyle.NeedShowBorder())
                    {
                        var borderWidth = itemStyle.borderWidth;
                        var borderColor = itemStyle.borderColor;
                        var smoothness = settings.cicleSmoothness;
                        var inRadius = radius - borderWidth;
                        var outRadius = radius;
                        var p1 = plb + diff + Vector3.up * borderWidth / 2;
                        var p2 = prb - diff + Vector3.up * borderWidth / 2;
                        var p3 = plt + diff - Vector3.up * borderWidth / 2;
                        var p4 = prt - diff - Vector3.up * borderWidth / 2;
                        UGL.DrawLine(vh, p1, p2, borderWidth / 2, borderColor);
                        UGL.DrawLine(vh, p3, p4, borderWidth / 2, borderColor);
                        UGL.DrawDoughnut(vh, pcl, inRadius, outRadius, borderColor, ChartConst.clearColor32,
                            180, 360, smoothness);
                        UGL.DrawDoughnut(vh, pcr, inRadius, outRadius, borderColor, ChartConst.clearColor32,
                            0, 180, smoothness);
                    }
                }
                else
                {
                    CheckClipAndDrawPolygon(vh, ref plb, ref plt, ref prt, ref prb, color, color, serie.clip, grid);
                }
            }
            else
            {
                var axis = m_XAxes[serie.xAxisIndex];
                var axisWidth = axis.axisLine.GetWidth(m_Theme.axis.lineWidth);
                Vector3 plb = new Vector3(pX + space, grid.runtimeY + axisWidth);
                Vector3 plt = new Vector3(pX + space, grid.runtimeY + grid.runtimeHeight + axisWidth);
                Vector3 prt = new Vector3(pX + space + barWidth, grid.runtimeY + grid.runtimeHeight + axisWidth);
                Vector3 prb = new Vector3(pX + space + barWidth, grid.runtimeY + axisWidth);
                if (serie.barType == BarType.Capsule)
                {
                    var radius = barWidth / 2;
                    var diff = Vector3.up * radius;
                    var pct = (plt + prt) / 2 - diff;
                    var pcb = (plb + prb) / 2 + diff;
                    CheckClipAndDrawPolygon(vh, prb + diff, plb + diff, plt - diff, prt - diff, color, color,
                        serie.clip, grid);
                    UGL.DrawSector(vh, pct, radius, color, 270, 450);
                    UGL.DrawSector(vh, pcb, radius, color, 90, 270);
                    if (itemStyle.NeedShowBorder())
                    {
                        var borderWidth = itemStyle.borderWidth;
                        var borderColor = itemStyle.borderColor;
                        var smoothness = settings.cicleSmoothness;
                        var inRadius = radius - borderWidth;
                        var outRadius = radius;
                        var p1 = plb + diff + Vector3.right * borderWidth / 2;
                        var p2 = plt - diff + Vector3.right * borderWidth / 2;
                        var p3 = prb + diff - Vector3.right * borderWidth / 2;
                        var p4 = prt - diff - Vector3.right * borderWidth / 2;
                        UGL.DrawLine(vh, p1, p2, borderWidth / 2, borderColor);
                        UGL.DrawLine(vh, p3, p4, borderWidth / 2, borderColor);
                        UGL.DrawDoughnut(vh, pct, inRadius, outRadius, borderColor, ChartConst.clearColor32,
                            270, 450, smoothness);
                        UGL.DrawDoughnut(vh, pcb, inRadius, outRadius, borderColor, ChartConst.clearColor32,
                            90, 270, smoothness);
                    }
                }
                else
                {
                    CheckClipAndDrawPolygon(vh, ref prb, ref plb, ref plt, ref prt, color, color, serie.clip, grid);
                }
            }
        }

        private float GetBarGap()
        {
            float gap = 0.3f;
            for (int i = 0; i < m_Series.Count; i++)
            {
                var serie = m_Series.list[i];
                if (serie.type == SerieType.Bar || serie.type == SerieType.Candlestick)
                {
                    if (serie.barGap != 0)
                    {
                        gap = serie.barGap;
                    }
                }
            }
            return gap;
        }

        private float GetSameStackTotalValue(string stack, int dataIndex)
        {
            if (string.IsNullOrEmpty(stack)) return 0;
            float total = 0;
            foreach (var serie in m_Series.list)
            {
                if (serie.type == SerieType.Bar)
                {
                    if (stack.Equals(serie.stack))
                    {
                        total += serie.data[dataIndex].data[1];
                    }
                }
            }
            return total;
        }


        private HashSet<string> barStackSet = new HashSet<string>();
        private float GetBarTotalWidth(float categoryWidth, float gap)
        {
            float total = 0;
            float lastGap = 0;
            barStackSet.Clear();
            for (int i = 0; i < m_Series.Count; i++)
            {
                var serie = m_Series.list[i];
                if (!serie.show) continue;
                if (serie.type == SerieType.Bar || serie.type == SerieType.Candlestick)
                {
                    if (!string.IsNullOrEmpty(serie.stack))
                    {
                        if (barStackSet.Contains(serie.stack)) continue;
                        barStackSet.Add(serie.stack);
                    }
                    var width = GetStackBarWidth(categoryWidth, serie);
                    if (gap == -1)
                    {
                        if (width > total) total = width;
                    }
                    else
                    {
                        lastGap = width * gap;
                        total += width;
                        total += lastGap;
                    }
                }
            }
            if (total > 0 && gap != -1) total -= lastGap;
            return total;
        }

        private float GetStackBarWidth(float categoryWidth, Serie now)
        {
            if (string.IsNullOrEmpty(now.stack)) return now.GetBarWidth(categoryWidth);
            float barWidth = 0;
            for (int i = 0; i < m_Series.Count; i++)
            {
                var serie = m_Series.list[i];
                if ((serie.type == SerieType.Bar && serie.type == SerieType.Candlestick)
                    && serie.show && now.stack.Equals(serie.stack))
                {
                    if (serie.barWidth > barWidth) barWidth = serie.barWidth;
                }
            }
            if (barWidth > 1) return barWidth;
            else return barWidth * categoryWidth;
        }

        private List<string> tempList = new List<string>();
        private int GetBarIndex(Serie currSerie)
        {
            tempList.Clear();
            int index = 0;
            for (int i = 0; i < m_Series.Count; i++)
            {
                var serie = m_Series.GetSerie(i);
                if (serie.type != SerieType.Bar) continue;
                if (string.IsNullOrEmpty(serie.stack))
                {
                    if (serie.index == currSerie.index) return index;
                    tempList.Add(string.Empty);
                    index++;
                }
                else
                {
                    if (!tempList.Contains(serie.stack))
                    {
                        if (serie.index == currSerie.index) return index;
                        tempList.Add(serie.stack);
                        index++;
                    }
                    else
                    {
                        if (serie.index == currSerie.index) return tempList.IndexOf(serie.stack);
                    }
                }
            }
            return 0;
        }
    }
}