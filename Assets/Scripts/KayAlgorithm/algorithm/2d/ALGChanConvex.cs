using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KayAlgorithm
{
    public class ChanConvex
    {
        public static List<Vector2> BuildConvex(List<Vector2> src)
        {
            ChanConvex convex = new ChanConvex(src);
            return convex.Build();
        }

        public ChanConvex(List<Vector2> src)
        {

        }

        public List<Vector2> Build()
        {
            return null;
        }

        
    }
}
