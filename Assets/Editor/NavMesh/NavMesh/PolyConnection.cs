using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nav
{
    public class PolyConnection:IComparable<PolyConnection>
    {
        public int A { get; set; }
        public int B { get; set; }

        public float Weight { get; set; }

        public GeoEdge ConnectionEdge { get; set; }

        public PolyConnection(int a, int b, GeoEdge connectionEdge) {
            A = a;
            B = b;
            this.ConnectionEdge = connectionEdge;
        }

        public int Other(int p) {
            if (p == A)
                return B;
            else
                return A;
        }

        public int CompareTo(PolyConnection other)
        {
            if ((this.A == other.A && this.B == other.B) || (this.A == other.B && this.B == other.A))
                return 0;
            else if (this.A > other.A && this.B > other.B)
                return 1;
            else
                return -1;
        }
    }
}
