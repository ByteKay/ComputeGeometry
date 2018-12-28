using KayMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace KayAlgorithm
{
    public class IncrementalConvex
    {
        public static List<Vector2> BuildConvex(List<Vector2> src)
        {
            IncrementalConvex convex = new IncrementalConvex(src);
            return convex.Build();
        }

        private List<Vector2> mPoints = new List<Vector2>();
        private int mCount;

        public IncrementalConvex(List<Vector2> src)
        {
            mPoints.AddRange(src);
            mCount = mPoints.Count;
        }

        public List<Vector2> Build()
        {
            GeoPointsArray2 polygon2 = FirstTriangle();
            for (int i = 0; i < mCount; ++i)
            {
                Vector2 tmp = mPoints[i];
                if (GeoPolygonUtils.IsPointInConvexPolygon2(polygon2, tmp) == -1)
                {
                    AddPoint(polygon2, tmp);
                }
            }
            return polygon2.mPointArray;
        }

        private void AddPoint(GeoPointsArray2 polygon2, Vector2 p)
        {
            int cnt = polygon2.Count;

            List<bool> convexMask = new List<bool>();

            for (int i = 0; i < cnt; ++i)
            {
                int j = i + 1;
                if (j == cnt)
                {
                    j = 0;
                }
                convexMask.Add(GeoPolygonUtils.IsConvexAngle(polygon2[i], polygon2[j], p));
            }

            int start = -1;
            int end = -1;
            for (int i = 0; i < cnt; i++)
            {
                int j = i + 1;
                if (j == cnt)
                {
                    j = 0;
                } 
                if (start == -1 && convexMask[i] == false && convexMask[j] == true)
                {
                    start = j;
                }
                if (end == -1 && convexMask[i] == true && convexMask[j] == false)
                {
                    end = j;
                }
                if (start != -1 && end != -1)
                {
                    break;
                }
            }
            if (start != -1 && end != -1)
            {
                GeoPointsArray2 other = new GeoPointsArray2();
                if (start <= end)
                {
                    for (int i = start; i <= end; ++i)
                    {
                        other.Add(polygon2[i]);
                    }
                }
                else
                {
                    for (int i = start; i < cnt; ++i)
                    {
                        other.Add(polygon2[i]);
                    }
                    for (int i = 0; i <= end; ++i)
                    {
                        other.Add(polygon2[i]);
                    }
                }
                other.Add(p);
                polygon2.Clear();
                polygon2.mPointArray.AddRange(other.mPointArray);
            }
            
        }

        // CCW
        private GeoPointsArray2 FirstTriangle()
        {
            GeoPointsArray2 polygon2 = new GeoPointsArray2();

            int minIndex = 0;
            int maxIndex = 0;
            float minX = mPoints[0].x;
            float maxX = mPoints[0].x;
            for (int i = 1; i < mCount; ++i)
            {
                if (mPoints[i].x < minX)
                {
                    minX = mPoints[i].x;
                    minIndex = i;
                }
                if (mPoints[i].x > maxX)
                {
                    maxX = mPoints[i].x;
                    maxIndex = i;
                }
            }

            Vector2 max = mPoints[maxIndex];
            Vector2 min = mPoints[minIndex];

            if (maxIndex > minIndex)
            {
                mPoints.RemoveAt(maxIndex);
                mPoints.RemoveAt(minIndex);
            }
            else
            {
                mPoints.RemoveAt(minIndex);
                mPoints.RemoveAt(maxIndex);
            }
            mCount = mPoints.Count;

            Vector2 dir = max - min;
            // 逆时针
            Vector2 left = new Vector2(-dir.y, dir.x);
            maxX = -1e10f;
            int sign = 0;
            for (int i = 0; i < mCount; ++i)
            {
                dir = mPoints[i] - min;
                float d = Vector2.Dot(dir, left);
                bool s = d < 0;
                if (s)
                {
                    d = -d;
                }
                if (d > maxX)
                {
                    maxX = d;
                    sign = s ? -1 : 1;
                    maxIndex = i;
                }
            }
            Vector2 threePoint = mPoints[maxIndex];
            mPoints.RemoveAt(maxIndex);
            mCount = mPoints.Count;

            Assert.IsTrue(maxX > -1.0f, "wrong");
            polygon2.Add(min);
            if (sign == 1)
            {
                polygon2.Add(max);
                polygon2.Add(threePoint);
            }
            else
            {
                polygon2.Add(threePoint);
                polygon2.Add(max);
            }
            return polygon2;
        }
    }
}
