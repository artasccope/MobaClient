using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameFW.GameMgr.AttackTree
{
    /// <summary>
    /// 进攻节点，从根到叶搜索
    /// </summary>
    public class AtkNode
    {
        public AtkNode(int id, Vector3 atkPos, float cost, float value, bool couldAtkOnAllPass) {
            this.Id = id;
            this.AtkPos = atkPos;
            this.couldAtkOnAllPass = couldAtkOnAllPass;
            this.Cost = cost;
            this.Value = value;
        }
        /// <summary>
        /// 进攻点的Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 进攻点的位置
        /// </summary>
        public Vector3 AtkPos { get; set; }
        /// <summary>
        /// 子进攻点
        /// </summary>
        public List<AtkNode> subNodes = new List<AtkNode>();
        /// <summary>
        /// 此进攻点是否已经被攻下
        /// </summary>
        public bool IsTaken {
            get; set;
        }
        /// <summary>
        /// 攻下此进攻点的价值
        /// </summary>
        public float Value { get; set; }
        /// <summary>
        /// 攻下此进攻点的预估花费
        /// </summary>
        public float Cost { get; set; }
        /// <summary>
        /// true:当所有子节点都为IsTaken时此节点才为CouldAtk
        /// false：只要有一个子节点为IsTaken时此节点就为CouldAtk
        /// </summary>
        private bool couldAtkOnAllPass;
        /// <summary>
        /// 当前节点是否可进攻
        /// </summary>
        public bool CouldAtk
        {
            get
            {
                if (couldAtkOnAllPass)
                {
                    foreach (AtkNode node in subNodes)
                    {
                        if (node.IsTaken == false)
                            return false;
                    }
                    return true;
                }
                else {
                    foreach (AtkNode node in subNodes) {
                        if (node.IsTaken == true)
                            return true;
                    }
                    return false;
                }
            }
        }
    }
}
