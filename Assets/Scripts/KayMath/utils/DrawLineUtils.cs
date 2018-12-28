/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 6/30/2017 11:13:04 AM
** Version:  v1.0.0
*********************************************************************************/
#define DRAW_IMAGE

#if DRAW_IMAGE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Drawing;

namespace KayMath
{
    public class DrawLineUtils
    {
        public static List<KeyValuePair<int, System.Drawing.Color>> BackPair2 = new List<KeyValuePair<int, System.Drawing.Color>>();
        public static List<KeyValuePair<int, System.Drawing.Color>> BackPair3 = new List<KeyValuePair<int, System.Drawing.Color>>();
        public static List<KeyValuePair<int, System.Drawing.Color>> BackPair4 = new List<KeyValuePair<int, System.Drawing.Color>>();
        static DrawLineUtils()
        {
            BackPair2.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.Red));
            BackPair2.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.Green));

            BackPair3.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.Red));
            BackPair3.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.Green));
            BackPair3.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.White));

            BackPair4.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.Red));
            BackPair4.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.Green));      
            BackPair4.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.Blue));
            BackPair4.Add(new KeyValuePair<int, System.Drawing.Color>(0, System.Drawing.Color.White));
        }

        static void CalculateBoundary(List<Vector2> datas, out float minX, out float minY, out float lengthX, out float lengthY)
        {
	        int size = datas.Count;
	        Vector2 temp;
	        Vector2 minB = new Vector2(1e5f, 1e5f);
            Vector2 maxB = new Vector2(-1e5f, -1e5f);

	        for (int i = 0; i < size; ++i)
	        {
                temp = datas[i];
                if (temp[0] < minB[0])
		        {
                    minB[0] = temp[0];
		        }
                if (temp[1] < minB[1])
		        {
                    minB[1] = temp[1];
		        }
                if (temp[0] > maxB[0])
		        {
                    maxB[0] = temp[0];
		        }
                if (temp[1] > maxB[1])
		        {
                    maxB[1] = temp[1];
		        }
	        }
	        minX = minB[0];
	        minY = minB[1];
	        lengthX = maxB[0] - minB[0];
	        lengthY = maxB[1] - minB[1];
        }

        static System.Drawing.Point Convert(Vector2 v)
        {
            System.Drawing.Point value = new System.Drawing.Point();
            value.X = (int)(v[0]);
            value.Y = (int)(v[1]);
            return value;
        }

        static System.Drawing.Point[] ResizeData(List<Vector2> datas, float minX, float minY, float lengthX, float lengthY, int bmp_size)
        {
	        int size = datas.Count;
            Vector2 temp;
            List<System.Drawing.Point> polygon = new List<Point>();
            double x = bmp_size * 1.0f / lengthX;
            double y = bmp_size * 1.0f / lengthY;
	        for (int i = 0; i < size; ++i)
	        {
                temp = datas[i];
                System.Drawing.Point value = new System.Drawing.Point();
                value.X = (int)((temp[0] - minX) * x);
                value.Y = (int)((temp[1] - minY) * y);
                if (value.X >= bmp_size)
                {
                    value.X = bmp_size - 1;
                }
                if (value.X <= 0)
                {
                    value.X = 1;
                }
                value.Y = bmp_size - value.Y;
                if (value.Y >= bmp_size)
                {
                    value.Y = bmp_size - 1;
                }
                if (value.Y <= 0)
                {
                    value.Y = 1;
                }
                polygon.Add(value);
	        }
            return polygon.ToArray();
        }



        public static Bitmap CreateBMP(int bmp_size, out System.Drawing.Graphics g)
        {
            Bitmap bmp = new Bitmap(bmp_size, bmp_size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = System.Drawing.Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.Black);
            return bmp;
        }

        public static void Calculate(List<List<Vector2>> polygons, out float x, out float y, out float lx, out float ly, out float max)
        {
            int size = polygons.Count;
            List<Vector2> tempData = new List<Vector2>();
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < polygons[i].Count; ++j)
                {
                    tempData.Add(polygons[i][j]);
                }
            }
            CalculateBoundary(tempData, out x, out y, out lx, out ly);
            max = lx > ly ? lx : ly;
            tempData.Clear();
        }

        public static void Calculate(List<Vector2> polygons, out float x, out float y, out float lx, out float ly, out float max)
        {
            CalculateBoundary(polygons, out x, out y, out lx, out ly);
            max = lx > ly ? lx : ly;
        }

        public static void DrawPolygon(System.Drawing.Graphics g, List<List<Vector2>> polygons, System.Drawing.Color color, float x, float y, float max, int bmp_size)
        {
            Pen pen = new Pen(color);
            int size = polygons.Count;
            for (int i = 0; i < size; ++i)
            {
                Point[] dat = ResizeData(polygons[i], x, y, max, max, bmp_size);
                g.DrawPolygon(pen, dat);
            }
            pen.Dispose();
        }

        public static void DrawPolygon(System.Drawing.Graphics g, List<Vector2> polygon, System.Drawing.Color color)
        {
            Pen pen = new Pen(color);
            Point[] points = new Point[polygon.Count];
            for (int i = 0; i < polygon.Count; ++i)
            {
                points[i] = Convert(polygon[i]);
                
            }
            g.DrawPolygon(pen, points);
            pen.Dispose();
        }

        public static void DrawLine(System.Drawing.Graphics g, List<List<Vector2>> polygons, System.Drawing.Color color, float x, float y, float max, int bmp_size)
        {
            Pen pen = new Pen(color);
            int size = polygons.Count;
            for (int i = 0; i < size; ++i)
            {
                Point[] dat = ResizeData(polygons[i], x, y, max, max, bmp_size);
                g.DrawLine(pen, dat[0], dat[1]);
            }
            pen.Dispose();
        }

        public static void Save(string fileName, Bitmap bmp, System.Drawing.Graphics g)
        {
            bmp.Save(fileName);
            g.Dispose();
            bmp.Dispose();
        }

        public static void SavePolygonToFile(string fileName, List<List<Vector2>> polygons, List<KeyValuePair<int,  System.Drawing.Color>> countPair = null)
        {
            int bmp_size = 1024;
            Bitmap bmp = new Bitmap(bmp_size, bmp_size, System.Drawing.Imaging.PixelFormat.Format32bppArgb); 
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.Black);
            int size = polygons.Count;
            List<Vector2> tempData = new List<Vector2>();
	        for (int i = 0; i < size; ++i)
	        {
                for (int j = 0; j < polygons[i].Count; ++j)
		        {
                    tempData.Add(polygons[i][j]);
		        }
	        }
            float x, y, lx, ly;
            CalculateBoundary(tempData, out x, out y, out lx, out ly);
            float max = lx > ly ? lx : ly;
            tempData.Clear();
            Pen pen;
            int count = -1;
            if (countPair != null)
            {
                pen = new Pen(countPair[0].Value);
                count = countPair[0].Key;
                countPair.RemoveAt(0);
            }
            else
            {
                pen = new Pen(System.Drawing.Color.Red);
            }       
            for (int i = 0; i < size; ++i)
	        {
                Point[] dat = ResizeData(polygons[i], x, y, max, max, bmp_size);
                if (dat.Length > 2)
                {
                    g.DrawPolygon(pen, dat);
                }
                else
                {
                    g.DrawLine(pen, dat[0], dat[1]);
                }
                if (i == count)
		        {
                    if (countPair != null && countPair.Count > 0)
                    {
                        count = countPair[0].Key;
                        pen.Dispose();
                        pen = new Pen(countPair[0].Value);
                        countPair.RemoveAt(0);
                    }
		        }
	        }
            bmp.Save(fileName);
            g.Dispose();
            pen.Dispose();
            bmp.Dispose();
        }
    }
}
#endif