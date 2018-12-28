///********************************************************************************
//** All rights reserved
//** Auth： kay.yang
//** E-mail: 1025115216@qq.com
//** Date： 9/28/2017 3:03:58 PM
//** Version:  v1.0.0
//*********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using KayMath;
//using KayUtils;
//using UnityEngine;

//namespace KayAlgorithm
//{
//    public class EarPoint3
//    {
//        public int mIndex;
//        public Vector3 mPoint;
//        public Vector2 mPoint2;
//        public EarPoint3(int index, float pX, float pY, float pZ)
//        {
//            mIndex = index;
//            mPoint = new Vector3(pX, pY, pZ);
//            mPoint2 = new Vector2(pX, pZ);
//        }

//        public EarPoint3(int index, Vector3 v)
//        {
//            mIndex = index;
//            mPoint = v;
//            mPoint2 = new Vector2(v[0], v[2]);
//        }

//        public EarPoint3()
//        {
//            mIndex = -1;
//            mPoint = Vector3.zero;
//            mPoint2 = Vector2.zero;
//        }

//        public float this[int idx]
//        {
//            get
//            {
//                return mPoint[idx];
//            }
//        }

//        public override bool Equals(object o)
//        {
//            return this == (EarPoint3)o;
//        }

//        public override int GetHashCode()
//        {
//            return mPoint.GetHashCode();
//        }

//        public static bool operator ==(EarPoint3 e1, EarPoint3 e2)
//        {
//            return e1.mIndex == e2.mIndex;
//        }
//        public static bool operator !=(EarPoint3 e1, EarPoint3 e2)
//        {
//            return e1.mIndex != e2.mIndex;
//        }
//    }

//    public class EarPolygon3
//    {
//        private LinkedList<EarPoint3> mHead;
//        private EarPolygon3 mParent;
//        private int mNumberOfPoints;
//        private List<EarPolygon3> mChildren;
//        public List<List<Vector3>> mResults;

//        private float mArea; // 带符号
//        public EarPolygon3()
//        {
//            mChildren = new List<EarPolygon3>();
//            mResults = new List<List<Vector3>>();
//            mHead = new LinkedList<EarPoint3>();
//            mParent = null;
//            mNumberOfPoints = 0;
//        }
//        public EarPolygon3(EarPolygon3 parent)
//        {
//            mChildren = new List<EarPolygon3>();
//            mResults = new List<List<Vector3>>();
//            mHead = new LinkedList<EarPoint3>();
//            mParent = parent;
//            parent.AddChild(this);
//            mNumberOfPoints = 0;
//        }

//        public void AddPolygon(List<Vector3> poly)
//        {
//            for (int i = 0; i < poly.Count; ++i)
//            {
//                AddPoint(poly[i]);
//            }
//        }

//        public bool AddPoint(Vector3 v)
//        {
//            mHead.AddLast(new EarPoint3(mNumberOfPoints++, v));
//            return true;
//        }

//        public bool AddPoint(float x, float y, float z)
//        {
//            mHead.AddLast(new EarPoint3(mNumberOfPoints++, x, y, z));
//            return true;
//        }
//        public LinkedListNode<EarPoint3> Get() { return mHead.First; }
//        public int NumPoints() { return mHead.Count; }
//        public int NumChildren() { return mChildren.Count; }
//        public void AddChild(EarPolygon3 child) { mChildren.Add(child); }
//        public void Reverse(int pos)
//        {
//            if (pos < 0)
//            {
//                LinkedList<EarPoint3> head = new LinkedList<EarPoint3>();
//                LinkedListNode<EarPoint3> last = mHead.Last;
//                do
//                {
//                    head.AddLast(last.Value);
//                    last = last.Previous;
//                } while (last != null);
//                mHead = head;
//            }
//            else
//            {
//                mChildren[pos].Reverse(-1);
//            }
//        }
//        public List<EarPolygon3> GetChildren() { return mChildren; }
//        public EarPolygon3 this[int idx]
//        {
//            get
//            {
//                return mChildren[idx];
//            }
//        }
//        public LinkedListNode<EarPoint3> InsertPoint(float x, float y, float z, LinkedListNode<EarPoint3> cur)
//        {
//            EarPoint3 newPoint = new EarPoint3(mNumberOfPoints++, x, y, z);
//            return mHead.AddAfter(cur, newPoint);
//        }
//        public void AddResult(Vector3 v1, Vector3 v2, Vector3 v3)
//        {
//            mResults.Add(new List<Vector3>());
//            mResults[mResults.Count - 1].Add(v1);
//            mResults[mResults.Count - 1].Add(v2);
//            mResults[mResults.Count - 1].Add(v3);
//        }

//        public void CalculateArea()
//        {
//            mArea = 0.0f;
//            LinkedListNode<EarPoint3> active = Get();
//            LinkedListNode<EarPoint3> next;
//            for (int i = 0; i < NumPoints(); ++i)
//            {
//                next = Next(active);
//                mArea += (active.Value[0] * next.Value[2] - next.Value[0] * active.Value[2]);
//                active = next;
//            }
//            mArea *= 0.5f;
//        }
//        public LinkedListNode<EarPoint3> Previous(LinkedListNode<EarPoint3> cur)
//        {
//            if (cur.Previous == null)
//            {
//                return mHead.Last;
//            }
//            return cur.Previous;
//        }
//        public LinkedListNode<EarPoint3> Next(LinkedListNode<EarPoint3> cur)
//        {
//            if (cur.Next == null)
//            {
//                return mHead.First;
//            }
//            return cur.Next;
//        }
//        public bool IsCCW()
//        {
//            return mArea > 0;
//        }
//        public bool Remove(LinkedListNode<EarPoint3> point)
//        {
//            mHead.Remove(point);
//            return true;
//        }
//    }

//    public class EarClipping3
//    {
//        public static void Clip(EarPolygon3 poly)
//        {
//            Merge(poly);
//            RecordEars(poly);
//        }
//        public static List<Vector4> Merge(EarPolygon3 poly)
//        {
//            OrientatePolygon(poly);
//            return MergePolygon(poly);
//        }
//        private static void OrientatePolygon(EarPolygon3 poly)
//        {
//            poly.CalculateArea();
//            if (!poly.IsCCW())
//            {
//                poly.Reverse(-1);
//            }
//            for (int i = 0; i < poly.NumChildren(); i++)
//            {
//                poly[i].CalculateArea();
//                if (poly[i].IsCCW())
//                {
//                    poly[i].Reverse(-1);
//                }
//            }
//        }
//        private static List<Vector4> MergePolygon(EarPolygon3 poly)
//        {
//            List<Vector4> connects = new List<Vector4>();
//            if (poly.NumChildren() > 0)
//            {
//                List<EarPolygon3> children = poly.GetChildren();
//                List<KeyValuePair<int, float>> order = ChildOrder(children);
//                KeyValuePair<LinkedListNode<EarPoint3>, LinkedListNode<EarPoint3>> connection;
//                LinkedListNode<EarPoint3> temp;
//                for (int i = 0; i < order.Count; i++)
//                {
//                    connection = GetSplit(poly, children[order[i].Key], order[i].Value);
//                    connects.Add(new Vector4(connection.Key.Value[0], connection.Key.Value[1], connection.Value.Value[0], connection.Value.Value[1]));
//                    LinkedListNode<EarPoint3> newP = poly.InsertPoint(connection.Key.Value[0], connection.Key.Value[1], connection.Key.Value[2], connection.Value);
//                    temp = connection.Key;
//                    do
//                    {
//                        temp = children[order[i].Key].Next(temp);
//                        newP = poly.InsertPoint(temp.Value[0], temp.Value[1], temp.Value[2], newP);
//                    } while (temp.Value != connection.Key.Value);
//                    newP = poly.InsertPoint(connection.Value.Value[0], connection.Value.Value[1], connection.Value.Value[2], newP);
//                }
//            }
//            return connects;
//        }
//        private static bool RecordEars(EarPolygon3 poly)
//        {
//            LinkedListNode<EarPoint3> active = poly.Get();
//            int NumPoints = poly.NumPoints() - 2;
//            while (poly.NumPoints() >= 3)
//            {
//                int num = poly.NumPoints();
//                int idx = active.Value.mIndex;
//                do
//                {
//                    if (IsConvex(active, poly))
//                    {
//                        if (IsEar(active, poly))
//                        {
//                            break;
//                        }
//                    }
//                    active = poly.Next(active);
//                } while (idx != active.Value.mIndex);

//                poly.AddResult(poly.Previous(active).Value.mPoint, active.Value.mPoint, poly.Next(active).Value.mPoint);
//                active = poly.Next(active);
//                poly.Remove(poly.Previous(active));
//                continue;
//            }
//            return true;
//        }
//        private static bool IsEar(LinkedListNode<EarPoint3> ele, EarPolygon3 poly)
//        {
//            LinkedListNode<EarPoint3> checkerN1 = poly.Next(ele);
//            LinkedListNode<EarPoint3> checker = poly.Next(checkerN1);
//            while (checker.Value.mIndex != poly.Previous(ele).Value.mIndex)
//            {
//                if (InTriangle(checker.Value, ele.Value, poly.Next(ele).Value, poly.Previous(ele).Value))
//                {
//                    return false;
//                }
//                checker = poly.Next(checker);
//            }
//            return true;
//        }
//        private static bool InTriangle(EarPoint3 pointToCheck, EarPoint3 earTip, EarPoint3 earTipPlusOne, EarPoint3 earTipMinusOne)
//        {
//            bool isIntriangle = GeoTriangleUtils.IsPointInTriangle2(earTip.mPoint2, earTipPlusOne.mPoint2, earTipMinusOne.mPoint2, ref pointToCheck.mPoint2);
//            if (isIntriangle)
//            {
//                if (pointToCheck.mPoint2 == earTip.mPoint2 || pointToCheck.mPoint2 == earTipPlusOne.mPoint2 || pointToCheck.mPoint2 == earTipMinusOne.mPoint2) // 端点
//                {
//                    return false;
//                }
//            }
//            return isIntriangle;
//        }
//        private static bool IsInSegment(LinkedListNode<EarPoint3> ele, EarPolygon3 poly)
//        {
//            LinkedListNode<EarPoint3> a = poly.Previous(ele);
//            LinkedListNode<EarPoint3> b = ele;
//            LinkedListNode<EarPoint3> c = poly.Next(ele);
//            return GeoSegmentUtils.IsPointInSegment3(a.Value.mPoint, c.Value.mPoint, ref b.Value.mPoint);
//        }
//        private static bool IsConvex(LinkedListNode<EarPoint3> ele, EarPolygon3 poly)
//        {
//            LinkedListNode<EarPoint3> a = poly.Previous(ele);
//            LinkedListNode<EarPoint3> b = ele;
//            LinkedListNode<EarPoint3> c = poly.Next(ele);
//            return GeoPolygonUtils.IsConvexAngle(a.Value.mPoint, b.Value.mPoint, c.Value.mPoint);
//        }
//        private static KeyValuePair<LinkedListNode<EarPoint3>, LinkedListNode<EarPoint3>> GetSplit(EarPolygon3 outer, EarPolygon3 inner, float smallestX)
//        {
//            LinkedListNode<EarPoint3> smallest = inner.Get();
//            do
//            {
//                if (smallest.Value[0] == smallestX)
//                    break;
//                smallest = smallest.Next;
//            } while (smallest != null);

//            LinkedListNode<EarPoint3> closest = GetClosest(OrderPoints(outer, smallest), 0, outer, smallest, inner);
//            KeyValuePair<LinkedListNode<EarPoint3>, LinkedListNode<EarPoint3>> split = new KeyValuePair<LinkedListNode<EarPoint3>, LinkedListNode<EarPoint3>>(smallest, closest);
//            return split;
//        }
//        class EarNodeValueCompare3 : IComparer<LinkedListNode<EarPoint3>>
//        {
//            LinkedListNode<EarPoint3> mActivePoint;
//            public EarNodeValueCompare3(LinkedListNode<EarPoint3> activePoint)
//            {
//                mActivePoint = activePoint;
//            }
//            public int Compare(LinkedListNode<EarPoint3> n1, LinkedListNode<EarPoint3> n2)
//            {
//                Vector2 t1 = n1.Value.mPoint - mActivePoint.Value.mPoint;
//                Vector2 t2 = n2.Value.mPoint - mActivePoint.Value.mPoint;
//                return t1.sqrMagnitude.CompareTo(t2.sqrMagnitude);
//            }
//        }
//        private static List<LinkedListNode<EarPoint3>> OrderPoints(EarPolygon3 poly, LinkedListNode<EarPoint3> point)
//        {
//            LinkedListNode<EarPoint3> head = poly.Get();
//            List<LinkedListNode<EarPoint3>> pointContainer = new List<LinkedListNode<EarPoint3>>();
//            LinkedListNode<EarPoint3> activePoint = point;
//            while (head != null)
//            {
//                pointContainer.Add(head);
//                head = head.Next;
//            }
//            pointContainer.Sort(new EarNodeValueCompare3(activePoint));
//            return pointContainer;
//        }
//        private static List<KeyValuePair<int, float>> ChildOrder(List<EarPolygon3> children)
//        {
//            List<KeyValuePair<int, float>> toSort = new List<KeyValuePair<int, float>>();
//            LinkedListNode<EarPoint3> head;
//            int size = children.Count;
//            for (int i = 0; i < size; i++)
//            {
//                head = children[i].Get();
//                float smallestX = head.Value[0];
//                do
//                {
//                    smallestX = head.Next.Value[0] > smallestX ? smallestX : head.Next.Value[0];
//                    head = head.Next;
//                } while (head.Next != null);
//                toSort.Add(new KeyValuePair<int, float>(i, smallestX));
//            }
//            toSort.Sort((x, y) => x.Value.CompareTo(y.Value));
//            return toSort;
//        }
//        private static LinkedListNode<EarPoint3> GetClosest(List<LinkedListNode<EarPoint3>> pointsOrdered, int index, EarPolygon3 poly, LinkedListNode<EarPoint3> innerPoint, EarPolygon3 polychild)
//        {
//            LinkedListNode<EarPoint3> a = innerPoint;
//            LinkedListNode<EarPoint3> b = pointsOrdered[index];
//            LinkedListNode<EarPoint3> c = poly.Get();
//            bool intersection = false;
//            do
//            {
//                intersection = DoIntersect(a.Value, b.Value, c.Value, poly.Next(c).Value);
//                c = c.Next;
//            } while ((!intersection) && (c != null));
//            if (!intersection)
//            {
//                c = a;
//                do
//                {
//                    intersection = DoIntersect(a.Value, b.Value, c.Value, polychild.Next(c).Value);
//                    c = polychild.Next(c);
//                } while ((!intersection) && (c.Value != a.Value));
//                if (!intersection)
//                {
//                    return b;
//                }
//            }
//            return GetClosest(pointsOrdered, index + 1, poly, innerPoint, polychild);
//        }
//        private static bool DoIntersect(EarPoint3 a, EarPoint3 b, EarPoint3 c, EarPoint3 d)
//        {
//            GeoInsectPointInfo insect = new GeoInsectPointInfo();
//            bool isInsect = GeoSegmentUtils.IsSegmentInsectSegment2(a.mPoint2, b.mPoint2, c.mPoint2, d.mPoint2, ref insect);
//            if (isInsect)
//            {
//                float x = insect.mHitGlobalPoint[0];
//                float y = insect.mHitGlobalPoint[1];
//                if ((x == c[0] && y == c[1]) || (x == d[0] && y == d[1])) // 端点检测
//                {
//                    return false;
//                }
//            }
//            return isInsect;
//        }
//    }
//}
