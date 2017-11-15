using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class BinaryTree3D
    {
        private Node3D rootNode;
        /// <summary>
        /// 插入节点，假设已经检查过是否包含了
        /// Node的子节点排序方式为：
        /// 4 5 6 7
        /// 0 1 2 3
        /// </summary>
        /// <param name="node"></param>
        /// <param name="root"></param>
        private Node3D _Insert(Node3D node, Node3D root) {
            if (root == null)
                return node;

            if (Vector3.Distance(node.Value, root.Value) < 1e-09) {
                if (node.Index < root.Index) {
                    root.Index = node.Index;
                    return root;
                }
            }
                

            if (node.Value.x >= root.Value.x)
            {
                if (node.Value.y >= root.Value.y)
                {
                    if (node.Value.z >= root.Value.z)
                    {
                        root.SetSubNodeByIndex(5, _Insert(node, root.SubNodeByIndex(5)));
                    }
                    else
                    {
                        root.SetSubNodeByIndex(4, _Insert(node, root.SubNodeByIndex(4)));
                    }
                }
                else
                {
                    if (node.Value.z >= root.Value.z)
                    {
                        root.SetSubNodeByIndex(1, _Insert(node, root.SubNodeByIndex(1)));
                    }
                    else
                    {
                        root.SetSubNodeByIndex(0, _Insert(node, root.SubNodeByIndex(0)));
                    }
                }
            }
            else
            {
                if (node.Value.y >= root.Value.y)
                {
                    if (node.Value.z >= root.Value.z)
                    {
                        root.SetSubNodeByIndex(6, _Insert(node, root.SubNodeByIndex(6)));
                    }
                    else
                    {
                        root.SetSubNodeByIndex(7, _Insert(node, root.SubNodeByIndex(7)));
                    }
                }
                else
                {
                    if (node.Value.z >= root.Value.z)
                    {
                        root.SetSubNodeByIndex(2, _Insert(node, root.SubNodeByIndex(2)));
                    }
                    else
                    {
                        root.SetSubNodeByIndex(3, _Insert(node, root.SubNodeByIndex(3)));
                    }
                }
            }
            return root;
        }

        private bool _Contains(Vector3 point, Node3D root) {
            if (root == null)
                return false;

            if (Vector3.Distance(point, root.Value) < 1e-06)
                return true;

            if (point.x >= root.Value.x)
            {
                if (point.y >= root.Value.y)
                {
                    if (point.z >= root.Value.z)
                    {
                        root = root.SubNodeByIndex(5);
                    }
                    else {
                        root = root.SubNodeByIndex(4);
                    }
                }
                else {
                    if (point.z >= root.Value.z)
                    {
                        root = root.SubNodeByIndex(1);
                    }
                    else
                    {
                        root = root.SubNodeByIndex(0);
                    }
                }
            }
            else {
                if (point.y >= root.Value.y)
                {
                    if (point.z >= root.Value.z)
                    {
                        root = root.SubNodeByIndex(6);
                    }
                    else
                    {
                        root = root.SubNodeByIndex(7);
                    }
                }
                else
                {
                    if (point.z >= root.Value.z)
                    {
                        root = root.SubNodeByIndex(2);
                    }
                    else
                    {
                        root = root.SubNodeByIndex(3);
                    }
                }
            }

            return _Contains(point, root);
        }

        private Node3D _GetNodeByPoint(Vector3 point, Node3D root)
        {
            if (root == null)
                return null;

            if (Vector3.Distance(point, root.Value) < 1e-06)
                return root;

            if (point.x >= root.Value.x)
            {
                if (point.y >= root.Value.y)
                {
                    if (point.z >= root.Value.z)
                    {
                        root = root.SubNodeByIndex(5);
                    }
                    else
                    {
                        root = root.SubNodeByIndex(4);
                    }
                }
                else
                {
                    if (point.z >= root.Value.z)
                    {
                        root = root.SubNodeByIndex(1);
                    }
                    else
                    {
                        root = root.SubNodeByIndex(0);
                    }
                }
            }
            else
            {
                if (point.y >= root.Value.y)
                {
                    if (point.z >= root.Value.z)
                    {
                        root = root.SubNodeByIndex(6);
                    }
                    else
                    {
                        root = root.SubNodeByIndex(7);
                    }
                }
                else
                {
                    if (point.z >= root.Value.z)
                    {
                        root = root.SubNodeByIndex(2);
                    }
                    else
                    {
                        root = root.SubNodeByIndex(3);
                    }
                }
            }

            return _GetNodeByPoint(point, root);
        }

        public int this[Vector3 point] {
            get {
                Node3D node = _GetNodeByPoint(point, rootNode);
                if (node != null)
                    return node.Index;
                else
                    return -1;
            }
        }

        private int count = 0;

        public int Count {
            get {
                return count;
            }
        }

        public void  Add(Vector3 point, int index) {
            if (!_Contains(point, rootNode))
                count++;

            Node3D node = new Node3D(point, index);
            rootNode = _Insert(node, rootNode);
        }



        public bool Contains(Vector3 point) {
            return _Contains(point, rootNode);
        }

        private void Clear(ref Node3D root) {
            if (root == null)
                return;

            for (int i = 0; i < 8; i++) {
                Node3D subNode = root.SubNodeByIndex(i);
                Clear(ref subNode);
            }

            root = null;
        }

        public void Clear() {
            Clear(ref rootNode);
            count = 0;
        }
    }
}
