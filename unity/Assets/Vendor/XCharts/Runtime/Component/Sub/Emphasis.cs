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
    /// 高亮的图形样式和文本标签样式。
    /// </summary>
    [System.Serializable]
    public class Emphasis : SubComponent
    {
        [SerializeField] private bool m_Show;
        [SerializeField] private SerieLabel m_Label = new SerieLabel();
        [SerializeField] private ItemStyle m_ItemStyle = new ItemStyle();

        public void Reset()
        {
            m_Show = false;
            m_Label.Reset();
            m_ItemStyle.Reset();
        }

        /// <summary>
        /// 是否启用高亮样式。
        /// </summary>
        public bool show
        {
            get { return m_Show; }
            set { m_Show = value; }
        }
        /// <summary>
        /// 图形文本标签。
        /// </summary>
        public SerieLabel label
        {
            get { return m_Label; }
            set { if (PropertyUtil.SetClass(ref m_Label, value, true)) SetAllDirty(); }
        }
        /// <summary>
        /// 图形样式。
        /// </summary>
        public ItemStyle itemStyle
        {
            get { return m_ItemStyle; }
            set { if (PropertyUtil.SetClass(ref m_ItemStyle, value, true)) SetVerticesDirty(); }
        }

        public override bool vertsDirty { get { return m_VertsDirty || label.vertsDirty || itemStyle.vertsDirty; } }

        public override bool componentDirty { get { return m_ComponentDirty || label.componentDirty; } }

        internal override void ClearVerticesDirty()
        {
            base.ClearVerticesDirty();
            label.ClearVerticesDirty();
            itemStyle.ClearVerticesDirty();
        }

        internal override void ClearComponentDirty()
        {
            base.ClearComponentDirty();
            label.ClearComponentDirty();
        }
    }
}