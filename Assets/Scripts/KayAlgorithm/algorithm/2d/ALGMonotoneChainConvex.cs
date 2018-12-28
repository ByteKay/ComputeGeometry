using KayMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KayAlgorithm
{
    public class MonotoneChainConvex
    {
        public static List<Vector2> BuildConvex(List<Vector2> src)
        {
            MonotoneChainConvex convex = new MonotoneChainConvex(src);
            return convex.Build();
        }

        private List<Vector2> mPoints = new List<Vector2>();
        private int mCount;

        public MonotoneChainConvex(List<Vector2> src)
        {
            mPoints.AddRange(src);
            mCount = mPoints.Count;
        }

        class SortElementCompare : IComparer<Vector2>
        {
            public int Compare(Vector2 x, Vector2 y)
            {
                int res = x.x.CompareTo(y.x);
                if (res == 0)
                {
                    return x.y.CompareTo(y.y);
                }
                return res;
            }
        }

        private List<Vector2> Build()
        {
            Sort();
            return Calculate();
        }

        private void Sort()
        {
            mPoints.Sort(new SortElementCompare());
        }

        private List<Vector2> Calculate()
        {
            int l = 0;
            int u = 0;
            int[] lIndices = new int[mCount];
            int[] uIndices = new int[mCount];
            for (int i = 0; i < mCount; ++i)
            {
                while (l >= 2 && IsConvcave(lIndices[l - 2], lIndices[l - 1], i))
                    l--;
                lIndices[l++] = i;

                while (u >= 2 && IsConvex(uIndices[u - 2], uIndices[u - 1], i))
                    u--;
                uIndices[u++] = i;
            }
            List<Vector2> res = new List<Vector2>();
            HashSet<int> temp = new HashSet<int>();

            for (int i = 0; i < l; ++i)
            {
                if (temp.Add(lIndices[i]))
                {
                    res.Add(mPoints[lIndices[i]]);
                }
            }
            for (int i = u - 1; i >= 0; --i)
            {
                if (temp.Add(uIndices[i]))
                {
                    res.Add(mPoints[uIndices[i]]);
                }
            }
            temp.Clear();
            return res;
        }
        private bool IsConvcave(int i, int j, int k)
        {
            return GeoPolygonUtils.IsConcaveAngle(mPoints[i], mPoints[j], mPoints[k]);
        }

        private bool IsConvex(int i, int j, int k)
        {
            return GeoPolygonUtils.IsConvexAngle(mPoints[i], mPoints[j], mPoints[k]);
        }
    }
}

