using System.Collections.Generic;
using UnityEngine;

namespace GameFW.GameMgr.AttackTree
{
    /// <summary>
    /// 进攻树
    /// </summary>
    public class AtkTree
    {
        #region 单例
        public static AtkTree Instance
        {
            get
            {
                return Nested.m_pInstance;
            }
        }

        private class Nested
        {
            internal static readonly AtkTree m_pInstance = new AtkTree();
            Nested() { }
        }

        private AtkTree() { }
        #endregion

        private AtkNode root;//根节点
        private Dictionary<int, AtkNode> atkNodeDic = new Dictionary<int, AtkNode>();

        #region 增、改进攻点

        /// <summary>
        /// 添加一个进攻点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="pos"></param>
        /// <param name="cost"></param>
        /// <param name="value"></param>
        /// <param name="couldAtkOnAllPass"></param>
        public void AddAtkNode(int id, int parentId, Vector3 pos, float cost, float value, bool couldAtkOnAllPass)
        {
            if (!atkNodeDic.ContainsKey(id))
            {
                AtkNode node = new AtkNode(id, pos, cost, value, couldAtkOnAllPass);
                if (atkNodeDic.ContainsKey(parentId))
                {
                    atkNodeDic[parentId].subNodes.Add(node);
                }
                else
                {
                    if (root == null)
                        root = node;
                }
                atkNodeDic.Add(id, node);
            }
        }

        /// <summary>
        /// 攻占一个进攻点
        /// </summary>
        /// <param name="id"></param>
        public void TakenAtkNode(int id)
        {
            if (atkNodeDic.ContainsKey(id))
            {
                atkNodeDic[id].IsTaken = true;
            }
        }

        #endregion

        #region 根据价值、效用、消耗得到不同的可用进攻点

        /// <summary>
        /// 得到所有可用的进攻点
        /// </summary>
        /// <returns></returns>
        public List<AtkNode> GetAttackableNodes()
        {
            List<AtkNode> nodes = new List<AtkNode>();
            if (root != null)
            {
                Queue<AtkNode> queue = new Queue<AtkNode>();
                queue.Enqueue(root);
                while (queue.Count > 0)
                {
                    AtkNode node = queue.Dequeue();
                    if (!node.IsTaken && node.CouldAtk)
                        nodes.Add(node);
                    foreach (AtkNode n in node.subNodes)
                    {
                        queue.Enqueue(n);
                    }
                }
            }

            return nodes;
        }

        /// <summary>
        /// 得到一个可以攻击的点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurFirstAtkPos()
        {
            Vector3 ret = Vector3.zero;
            if (root != null)
            {
                Queue<AtkNode> queue = new Queue<AtkNode>();
                queue.Enqueue(root);
                while (queue.Count > 0)
                {
                    AtkNode node = queue.Dequeue();
                    if (node.CouldAtk)
                    {
                        if (!node.IsTaken)
                            return node.AtkPos;
                    }
                    else
                    {
                        foreach (AtkNode n in node.subNodes)
                        {
                            if (!n.IsTaken)
                                queue.Enqueue(n);
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 得到最容易的进攻点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurEasiestAtkPos()
        {
            List<AtkNode> nodes = GetAttackableNodes();
            Vector3 ret = Vector3.zero;
            float minCost = float.MaxValue;
            foreach (AtkNode node in nodes)
            {
                if (node.Cost < minCost)
                {
                    ret = node.AtkPos;
                    minCost = node.Cost;
                }
            }

            return ret;
        }

        /// <summary>
        /// 得到最难的进攻点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurHardestAtkPos()
        {
            List<AtkNode> nodes = GetAttackableNodes();
            Vector3 ret = Vector3.zero;
            float cost = float.MinValue;
            foreach (AtkNode node in nodes)
            {
                if (node.Cost > cost)
                {
                    ret = node.AtkPos;
                    cost = node.Cost;
                }
            }

            return ret;
        }

        /// <summary>
        /// 得到当前最具性价比的进攻点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurMostValueableAtkPos()
        {
            List<AtkNode> nodes = GetAttackableNodes();
            Vector3 ret = Vector3.zero;
            float utility = float.MinValue;
            foreach (AtkNode node in nodes)
            {
                if (node.Value / node.Cost > utility)
                {
                    ret = node.AtkPos;
                    utility = node.Value / node.Cost;
                }
            }

            return ret;
        }

        /// <summary>
        /// 得到当前价值最大的进攻点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurMaxValueAtkPos()
        {
            List<AtkNode> nodes = GetAttackableNodes();
            Vector3 ret = Vector3.zero;
            float value = float.MinValue;
            foreach (AtkNode node in nodes)
            {
                if (node.Value > value)
                {
                    ret = node.AtkPos;
                    value = node.Value;
                }
            }

            return ret;
        }

        /// <summary>
        /// 得到当前价值最小的进攻点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurMinValueAtkPos()
        {
            List<AtkNode> nodes = GetAttackableNodes();
            Vector3 ret = Vector3.zero;
            float value = float.MaxValue;
            foreach (AtkNode node in nodes)
            {
                if (node.Value < value)
                {
                    ret = node.AtkPos;
                    value = node.Value;
                }
            }

            return ret;
        }

        #endregion
    }
}
