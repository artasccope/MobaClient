using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFW.ID
{
    /// <summary>
    /// Id计算
    /// </summary>
    public class IDCaculater
    {

        /// <summary>
        /// 根据层次结构得到一个transform的名字(从根到叶拼接)
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string TransformNameInHierachy(Transform trans, string prefix) {
            StringBuilder name = new StringBuilder(prefix);

            Stack<Transform> transStack = new Stack<Transform>();
            while (trans != null)
            {
                transStack.Push(trans);
                trans = trans.parent;
            }

            while (transStack.Count > 0) {
                Transform t = transStack.Pop();
                name.Append(t.name);
            }
            return name.ToString();
        }

        /// <summary>
        /// 得到拼接的字符串的Hash值
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int TransformIdInSceneHierachy(Transform trans)
        {
            return TransformNameInHierachy(trans, SceneManager.GetActiveScene().name).GetHashCode();
        }
    }
}
