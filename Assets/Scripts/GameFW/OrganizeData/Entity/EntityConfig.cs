using GameFW.Core.Base;
using GameFW.ID;
using UnityEngine;

namespace GameFW.OrganizeData.Entity
{
    /// <summary>
    /// 实体参数
    /// </summary>
    public class EntityConfig : MonoBehaviour
    {
        public EntityInfo entityInfo = null;//实体信息

        private void Reset()
        {
            //TODO
        }
        
        /// <summary>
        /// 初始化自身
        /// </summary>
        /// <param name="info"></param>
        public virtual void InitSelf(EntityInfo info)
        {
            entityInfo = info;
            if (entityInfo != null)
            {
                switch (LayerMask.LayerToName(gameObject.layer)) {
                    case "Building":
                        if (MgrCenter.EntityMgr.GetItem(entityInfo.parentId) != null)
                        {
                            transform.SetParent(MgrCenter.EntityMgr.GetItem(entityInfo.parentId).transform);
                            if (entityInfo.siblingIndex < transform.parent.childCount)
                            {
                                transform.SetSiblingIndex(entityInfo.siblingIndex);
                                Debug.Log("sibling id:" + transform.GetSiblingIndex());
                            }
                            else
                                Debug.Log("set sibling index failed. " + gameObject.name);
                            transform.localEulerAngles = entityInfo.localEulerAngles;
                            transform.localPosition = entityInfo.localPosition;
                            transform.localScale = entityInfo.localScale;
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 记录实例信息
        /// </summary>
        public virtual void RecordInfo()
        {
            if (entityInfo == null)
                entityInfo = new EntityInfo();

            entityInfo.id = IDCaculater.TransformIdInSceneHierachy(transform);
            if (transform.parent == null)
                entityInfo.parentId = -1;
            else
            {
                entityInfo.parentId = IDCaculater.TransformIdInSceneHierachy(transform.parent);
            }

            entityInfo.siblingIndex = transform.GetSiblingIndex();
            entityInfo.localEulerAngles = transform.localEulerAngles;
            entityInfo.localPosition = transform.localPosition;
            entityInfo.localScale = transform.localScale;

            SubPropertiesSet();
        }
        
        /// <summary>
        /// 子类特有的信息设置
        /// </summary>
        protected virtual void SubPropertiesSet() {
        }

        /// <summary>
        /// 实体存储信息选项
        /// </summary>
        public EntitySaveOption entitySaveOption;
    }
}
