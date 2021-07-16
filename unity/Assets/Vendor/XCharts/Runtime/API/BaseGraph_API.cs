﻿/************************************************/
/*                                              */
/*     Copyright (c) 2018 - 2021 monitor1394    */
/*     https://github.com/monitor1394           */
/*                                              */
/************************************************/

using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace XCharts
{
    /// <summary>
    /// The base class of all graphs or components.
    /// 所有图形的基类。
    /// </summary>
    public partial class BaseGraph
    {
        /// <summary>
        /// The background component.
        /// 背景组件。
        /// </summary>
        /// <value></value>
        public Background background { get { return m_Background; } }
        /// <summary>
        /// The x of graph. 
        /// 图形的X
        /// </summary>
        public float graphX { get { return m_GraphX; } }
        /// <summary>
        /// The y of graph. 
        /// 图形的Y
        /// </summary>
        public float graphY { get { return m_GraphY; } }
        /// <summary>
        /// The width of graph. 
        /// 图形的宽
        /// </summary>
        public float graphWidth { get { return m_GraphWidth; } }
        /// <summary>
        /// The height of graph. 
        /// 图形的高
        /// </summary>
        public float graphHeight { get { return m_GraphHeight; } }
        /// <summary>
        /// The position of graph.
        /// 图形的左下角起始坐标。
        /// </summary>
        public Vector3 graphPosition { get { return m_GraphPosition; } }
        public Rect graphRect { get { return m_GraphRect; } }
        /// <summary>
        /// The postion of pointer.
        /// 鼠标位置。
        /// </summary>
        public Vector2 pointerPos { get; protected set; }
        /// <summary>
        /// Whether the mouse pointer is in the chart.
        /// 鼠标是否在图表内。
        /// </summary>
        public bool isPointerInChart { get; protected set; }
        /// <summary>
        /// 警告信息。
        /// </summary>
        public string warningInfo { get; protected set; }
        public bool isControlledByLayout { get { return m_IsControlledByLayout; } }
        /// <summary>
        /// 强制开启鼠标事件检测。
        /// </summary>
        public bool forceOpenRaycastTarget { get { return m_ForceOpenRaycastTarget; } set { m_ForceOpenRaycastTarget = value; } }
        /// <summary>
        /// 鼠标点击回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onPointerClick { set { m_OnPointerClick = value; m_ForceOpenRaycastTarget = true; } }
        /// <summary>
        /// 鼠标按下回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onPointerDown { set { m_OnPointerDown = value; m_ForceOpenRaycastTarget = true; } }
        /// <summary>
        /// 鼠标弹起回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onPointerUp { set { m_OnPointerUp = value; m_ForceOpenRaycastTarget = true; } }
        /// <summary>
        /// 鼠标进入回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onPointerEnter { set { m_OnPointerEnter = value; m_ForceOpenRaycastTarget = true; } }
        /// <summary>
        /// 鼠标退出回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onPointerExit { set { m_OnPointerExit = value; m_ForceOpenRaycastTarget = true; } }
        /// <summary>
        /// 鼠标开始拖拽回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onBeginDrag { set { m_OnBeginDrag = value; m_ForceOpenRaycastTarget = true; } }
        /// <summary>
        /// 鼠标拖拽回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onDrag { set { m_OnDrag = value; m_ForceOpenRaycastTarget = true; } }
        /// <summary>
        /// 鼠标结束拖拽回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onEndDrag { set { m_OnEndDrag = value; m_ForceOpenRaycastTarget = true; } }
        /// <summary>
        /// 鼠标滚动回调。
        /// </summary>
        public Action<PointerEventData, BaseGraph> onScroll { set { m_OnScroll = value; m_ForceOpenRaycastTarget = true; } }

        /// <summary>
        /// 设置图形的宽高（在非stretch pivot下才有效，其他情况需要自己调整RectTransform）
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void SetSize(float width, float height)
        {
            if (LayerHelper.IsFixedWidthHeight(rectTransform))
            {
                rectTransform.sizeDelta = new Vector2(width, height);
            }
            else
            {
                Debug.LogError("Can't set size on stretch pivot,you need to modify rectTransform by yourself.");
            }
        }

        /// <summary>
        /// Redraw graph in next frame.
        /// 在下一帧刷新图形。
        /// </summary>
        public void RefreshGraph()
        {
            m_RefreshChart = true;
        }

        public void RefreshAllComponent()
        {
            SetAllComponentDirty();
            RefreshGraph();
        }

        /// <summary>
        /// 检测警告信息。
        /// </summary>
        /// <returns></returns>
        public string CheckWarning()
        {
            warningInfo = CheckHelper.CheckChart(this);
            return warningInfo;
        }

        /// <summary>
        /// 移除所有图表子节点，会自动重现初始化。
        /// </summary>
        public void RemoveChartObject()
        {
            ChartHelper.DestroyAllChildren(transform);
            //SetAllComponentDirty();
        }
    }
}
