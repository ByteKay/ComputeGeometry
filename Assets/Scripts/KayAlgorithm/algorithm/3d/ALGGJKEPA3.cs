/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 6/30/2017 11:13:04 AM
** Version:  v1.0.0
*********************************************************************************/
using System;
using System.Collections.Generic;

using UnityEngine;
namespace KayAlgorithm
{
    public class Epision
    {
        public static double E;
        static Epision()
        {
            double e = 0.5;
            while (1.0 + e > 1.0)
            {
                e *= 0.5;
            }
            E = e;
        }
    }

    public class GJK2
    {

        // 根据方向获取最远点
        private static Vector2 Support(List<Vector2> poly1, Vector2 direction)
        {
            double max = Double.MinValue;
            int index = -1;
            for (int i = 0; i != poly1.Count; ++i)
            {
                double dot = Vector2.Dot(poly1[i], direction);
                if (dot > max)
                {
                    max = dot;
                    index = i;
                }
            }
            return poly1[index];
        }

        // 计算一个 Minkowski 差
        private static Vector2 Minkowski(List<Vector2> poly1, List<Vector2> poly2, Vector2 dir)
        {
            Vector2 v1 = Support(poly1, dir);
            Vector2 v2 = Support(poly2, -dir);
            return v1 - v2;
        }

        // 计算主体部分，迭代
        public static bool Loop(List<Vector2> poly1, List<Vector2> poly2)
        {
            // 初始化
            List<Vector2> simplex = new List<Vector2>();
            Vector2 direction = new Vector2(1.0f, 0.0f);
            Vector2 point = Minkowski(poly1, poly2, direction);
            // 找不到正向的点，不相交
            if (Vector2.Dot(point, direction) <= 0)
            {
                return false;
            }
            simplex.Add(point);
            direction *= -1.0f;
            while (true)
            {
                point = Minkowski(poly1, poly2, direction);
                if (Vector2.Dot(point, direction) <= Epision.E)
                {
                    return false;
                }
                simplex.Add(point);
                if (Evaluate(ref simplex, ref direction))
                {
                    return true;
                }
            }
        }

        private static bool Evaluate(ref List<Vector2> simplex, ref Vector2 direction)
        {
            switch (simplex.Count)
            {
                case 2:
                    {
                        // 现在还是一条线段
                        // 最后添加的点在1位置
                        // 接下来选择一个方向，且原点在正方向
                        Vector2 ab = simplex[0] - simplex[1];
                        Vector2 ao = -simplex[1];
                        // 做垂直于ba的一个向量
                        direction = Cross(ab, ao, ab);
                        // 原点在直线上 
                        if (direction.sqrMagnitude <= Epision.E)
                        {
                            float tmp = ab[0];
                            direction[0] = -ab[1];
                            direction[1] = tmp;
                            direction.Normalize();
                        }
                    }
                    return false;
                case 3:
                    {
                        // 此时已经是一个三角形
                        // 索引2为最后添加的点
                        // 原点和最后添加的点在在0和1线段的同一侧  
                        Vector2 ac = simplex[0] - simplex[2];
                        Vector2 ab = simplex[1] - simplex[2];
                        Vector2 ao = -simplex[2];

                        // 计算ab和ac的垂直方向，且方向是原理边对应的顶点的  
                        // 每个垂向量，将平面分成两个区域  
                        Vector2 abDir = Cross(ac, ab, ab);
                        Vector2 acDir = Cross(ab, ac, ac);
                        // 然后判断原点在哪一个区域
                        if (Vector2.Dot(abDir, ao) >= 0)
                        {
                            // 原点在abDir侧
                            direction = abDir;
                            simplex.RemoveAt(0);
                            return false;
                        }
                        else
                        {
                            if (Vector2.Dot(acDir, ao) >= 0)
                            {
                                // 原点在acDir侧
                                direction = acDir;
                                simplex.RemoveAt(1);
                                return false;
                            }
                            return true;
                        }

                    }
                default:
                    throw new Exception("wrong");
            }
        }


        private static Vector2 Cross(Vector2 a, Vector2 b, Vector2 c)
        {
            // (a×b)×c =b(a·c) - a(b·c)
            // a×(b×c) =b(a·c) - c(a·b),
            /*
                Vector2 r = new Vector2();
                // perform a.dot(c)
                double ac = a.x * c.x + a.y * c.y;
                // perform b.dot(c)
                double bc = b.x * c.x + b.y * c.y;
                // perform b * a.dot(c) - a * b.dot(c)
                r.x = b.x * ac - a.x * bc;
                r.y = b.y * ac - a.y * bc;
             */
            return Vector2.Dot(a, c) * b - Vector2.Dot(b, c) * a;
        }

    }

}
