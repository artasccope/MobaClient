using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nav
{
    public class NavAOI
    {

        private float left, bottom;
        private int width, height;
        private List<int>[,] navPolyAOI;
        private float tileSize;

        public float Left { get { return left; } }
        public float Bottom { get { return bottom; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public float TileSize { get { return tileSize; } }
        public List<int>[,] NavPolyAOI { get { return navPolyAOI; } }

        public NavAOI(float left, float bottom, float width, float height, float tileSize) {
            this.left = left;
            this.bottom = bottom;
            this.width = Mathf.CeilToInt(width / tileSize);
            this.height = Mathf.CeilToInt(height / tileSize);
            this.tileSize = tileSize;

            navPolyAOI = new List<int>[this.width, this.height];
        }

        public NavAOI(float left, float bottom, float width, float height, float tileSize, List<int>[,] aoiList)
        {
            this.left = left;
            this.bottom = bottom;
            this.width = Mathf.CeilToInt(width / tileSize);
            this.height = Mathf.CeilToInt(height / tileSize);
            this.tileSize = tileSize;

            navPolyAOI = new List<int>[this.width, this.height];
            if (aoiList != null) {
                for (int i = 0; i < this.width; i++) {
                    for (int j = 0; j < this.height; j++) {
                        if (aoiList[i, j] != null) {
                            foreach (int k in aoiList[i, j]) {
                                this.AddPolyToAOI(i,j, k);
                            }
                        }
                    }
                }
            }
        }

        public List<int> GetPolyListInAOI(float x, float z) {
            int i = Mathf.FloorToInt((x - left) / tileSize);
            int j = Mathf.FloorToInt((z - bottom) / tileSize);

            if (navPolyAOI[i, j] != null && navPolyAOI[i, j].Count > 0)
                return navPolyAOI[i, j];
            else
                return null;
        }

        public void AddPolyToAOI(int x, int z, int poly) {
            if (navPolyAOI[x, z] == null)
                navPolyAOI[x, z] = new List<int>();

            if (!navPolyAOI[x, z].Contains(poly))
                navPolyAOI[x, z].Add(poly);
        }
    }
}
