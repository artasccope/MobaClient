using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public struct PointIndexPair
    {
        public PointIndexPair(int index, Vector3 point) {
            this.Index = index;
            this.Point = point;
        }

        public Vector3 Point { get; set; }
        public int Index { get; set; }
    }
}
