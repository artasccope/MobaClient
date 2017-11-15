using GameFW.Core.Base;
using GameFW.ID;
using UnityEngine;

namespace GameFW.Entity
{
    /// <summary>
    /// Entity注册器
    /// </summary>
    public class EntityRegister:MonoBehaviour
    {
        private int curId;

        /// <summary>
        /// 自动注册
        /// </summary>
        private void Awake()
        {
            curId = IDCaculater.TransformIdInSceneHierachy(transform);
            MgrCenter.EntityMgr.RegistItem(curId, gameObject);
        }

        /// <summary>
        /// 以新的Id注册
        /// </summary>
        /// <param name="id"></param>
        public void Regist(int id)
        {
            MgrCenter.EntityMgr.UnRegistItem(curId);
            curId = id;
            MgrCenter.EntityMgr.RegistItem(curId, gameObject);
        }

        /// <summary>
        /// 注册AOI
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        public void RegistAOI(int id, GameObject obj) {
            if (obj != null) {
                MgrCenter.EntityMgr.RegistItemToAOI(id, obj);
            }
        }
    }
}
