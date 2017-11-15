using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class Node3D
    {
        private Node3D[] subNodes = new Node3D[8] { null, null, null, null, null, null, null, null };

        public Vector3 Value { get; set; }
        public int Index { get; set; }

        public Node3D SubNodeByIndex(int index) {
            if (index > 7)
                return null;

            return subNodes[index];
        }

        public void SetSubNodeByIndex(int index, Node3D value) {
            if (index > 7)
                return;

            subNodes[index] = value;
        }

        public Node3D(Vector3 point, int index) {
            this.Value = point;
            this.Index = index;
        }
    }
}
