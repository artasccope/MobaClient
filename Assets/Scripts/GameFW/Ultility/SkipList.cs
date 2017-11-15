using System;
using System.Collections.Generic;

namespace GameFW.Ultility
{
    /// <summary>
    /// 跳表,先用byte来实现
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SkipList<T>
    {
        /// <summary>
        /// 由于TKey不是唯一的，要找到确切的元素仍然需要对一个数据集进行比较，
        /// 对于一个优先队列，索引(即优先级)相同的仍可能是一个大数据集。
        /// </summary>
        private Dictionary<T, SkipListNode<T>> dicCache = new Dictionary<T, SkipListNode<T>>();

        private SkipListNode<T> head;
        private SkipListNode<T> tail;

        private SkipListNode<T> Find(T value) {
            if (dicCache.ContainsKey(value))
                return dicCache[value];
            else
                return null;
        }

        public SkipList(int maxLevel) {
            this.maxLevel = maxLevel;
            this.level = 1;
            this.count = 0;
            tail = new SkipListNode<T>(0, Byte.MaxValue, default(T));
            head = new SkipListNode<T>(maxLevel, Byte.MinValue, default(T));
            for (int i = 0; i < maxLevel; i++) {
                head.forward.Add(tail);
            }
            head.prev = null;
            tail.prev = head;
        }

        private int level;
        private int maxLevel;
        private int count;
        public int Count {
            get {
                return count;
            }
        }
        public int MaxLevel {
            get {
                return maxLevel;            
            }
        }

        /// <summary>
        /// 增加一个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(byte key, T value) {
            if (this.dicCache.ContainsKey(value))
                return;

            SkipListNode<T> cur = head;
            SkipListNode<T>[] downCache = new SkipListNode<T>[maxLevel];
            int k = this.level - 1;
            while (k >= 0) {
                while (cur.forward != null && cur.forward.Count > k && cur.forward[k].key <= key) {
                    cur = cur.forward[k];
                }
                downCache[k] = cur;
                k--;
            }

            k = UnityEngine.Random.Range(0, maxLevel);
            if (k > this.level) {
                this.level++;
                k = this.level - 1;
                downCache[k] = cur;
            }
            SkipListNode<T> newNode = new SkipListNode<T>(k, key, value);
            for (int i = 0; i <= k; i++) {
                cur = downCache[i];
                newNode.forward.Add(cur.forward[i]);
                cur.forward[i] = newNode;
            }
            newNode.forward[0].prev = newNode;

            this.count++;
        }
    }
}
