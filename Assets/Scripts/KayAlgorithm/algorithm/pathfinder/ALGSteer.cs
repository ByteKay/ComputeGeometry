/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 7/6/2017 3:53:15 PM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KayAlgorithm
{
    // 障碍物
    public class ALGObstacle
    {
        // 求 切点
        public virtual void Tangent()
        {

        }

        // 判断射线相交
        public virtual bool Intersect()
        {
            return false;
        }
    }

    public class AlgCircleObstacle : ALGObstacle
    {

    }

    public class AlgEllipseObstacle : ALGObstacle
    {

    }

    public class AlgRectangleObstacle : ALGObstacle
    {

    }

    public class AlgPolygonObstacle : ALGObstacle
    {

    }
    /// <summary>
    /// 计算切线方向， 改变方向，实时走向目标点。不需要实现刷路径
    /// 需要控制路点
    /// </summary>
    public class ALGSteer
    {

    }
}
