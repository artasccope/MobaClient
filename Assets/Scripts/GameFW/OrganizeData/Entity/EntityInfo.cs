using UnityEngine;

namespace GameFW.OrganizeData.Entity
{
    /// <summary>
    /// 实体的实例信息
    /// </summary>
    public class EntityInfo : ScriptableObject
    {
        #region 构造器
        public EntityInfo() { }

        public EntityInfo(EntityInfo info)
        {
            this.parentId = info.parentId;
            this.id = info.id;
            this.siblingIndex = info.siblingIndex;
            this.localEulerAngles = info.localEulerAngles;
            this.localPosition = info.localPosition;
            this.localScale = info.localScale;
        }

        #endregion

        [SerializeField]
        public int parentId;
        [SerializeField]
        public int id;
        [SerializeField]
        public int siblingIndex;//在父transform的child中的index
        [SerializeField]
        public Vector3 localEulerAngles;
        [SerializeField]
        public Vector3 localPosition;
        [SerializeField]
        public Vector3 localScale;
    }
}
