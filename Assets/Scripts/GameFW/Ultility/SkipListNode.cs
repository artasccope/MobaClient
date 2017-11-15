using System.Collections.Generic;

namespace GameFW.Ultility
{
    /// <summary>
    /// 跳表节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SkipListNode<T>
    {
        public byte key;
        public T item;
        public List<SkipListNode<T>> forward;
        public SkipListNode<T> prev;

        public SkipListNode() {
            key = 0;
            item = default(T);
        }

        public SkipListNode(int level, byte key, T item)
        {
            this.key = key;
            this.item = item;
            if (level == 0)
                forward = null;
            else
            {
                forward = new List<SkipListNode<T>>(level);
            }
        }
    }
}
