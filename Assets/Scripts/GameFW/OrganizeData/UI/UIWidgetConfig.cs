using GameFW.Core.Base;
using GameFW.OrganizeData.Entity;
using UnityEngine;

namespace GameFW.OrganizeData.UI
{
    /// <summary>
    /// UI控件参数记录
    /// </summary>
    public class UIWidgetConfig : EntityConfig
    {
        /// <summary>
        /// 根据读取的参数初始化
        /// </summary>
        /// <param name="info"></param>
        public override void InitSelf(EntityInfo info)
        {
            entityInfo = info;
            if (entityInfo != null)
            {
                transform.SetParent(MgrCenter.UIMgr.GetItem(entityInfo.parentId).transform);
                base.InitSelf(info);

                UIWidgetInfo uiInfo = info as UIWidgetInfo;
                RectTransform rec = transform.GetComponent<RectTransform>();
                rec.anchoredPosition = uiInfo.anchoredPosition;
                rec.anchoredPosition3D = uiInfo.anchoredPosition3D;
                rec.anchorMax = uiInfo.anchorMax;
                rec.anchorMin = uiInfo.anchorMin;
                rec.offsetMax = uiInfo.offsetMax;
                rec.offsetMin = uiInfo.offsetMin;
                rec.pivot = uiInfo.pivot;
                rec.sizeDelta = uiInfo.sizeDelta;
            }
        }

        /// <summary>
        /// 设置子类的属性
        /// </summary>
        protected override void SubPropertiesSet()
        {
            RectTransform rec = transform.GetComponent<RectTransform>();
            UIWidgetInfo uiInfo = new UIWidgetInfo(entityInfo);

            uiInfo.anchoredPosition = rec.anchoredPosition;
            uiInfo.anchoredPosition3D = rec.anchoredPosition3D;
            uiInfo.anchorMax = rec.anchorMax;
            uiInfo.anchorMin = rec.anchorMin;
            uiInfo.offsetMax = rec.offsetMax;
            uiInfo.offsetMin = rec.offsetMin;
            uiInfo.pivot = rec.pivot;
            uiInfo.sizeDelta = rec.sizeDelta;
            entityInfo = uiInfo;

            this.entitySaveOption = new EntitySaveOption(0, 0, 0, true, true, false, "UI", null, null);
        }
    }
}
