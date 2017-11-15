using GameFW.OrganizeData.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameFW.OrganizeData.UI
{
    /// <summary>
    /// UI控件信息
    /// </summary>
    public class UIWidgetInfo:EntityInfo
    {
        public UIWidgetInfo(EntityInfo entityInfo):base(entityInfo){

        }
        public UIWidgetInfo() { }
        /// <summary>
        /// 层级中的父RectTransform的id
        /// </summary>
        public Vector2 anchoredPosition;
        public Vector3 anchoredPosition3D;
        public Vector2 anchorMax;
        public Vector2 anchorMin;
        public Vector2 offsetMax;
        public Vector2 offsetMin;
        public Vector2 pivot;
        public Vector2 sizeDelta;
    }
}
