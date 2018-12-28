/********************************************************************************
** Copyright 深圳市全名点游科技有限公司
** All rights reserved
** Auth： kay.yang
** Date： 12/21/2016 5:57:11 PM
** Desc： JoinGame 全名交游
** Version:  v1.0.0
*********************************************************************************/
#undef DEBUG

using System;
using UnityEngine;

namespace KayUtils
{
    public class SimpleLogger
    {
        const string PreSuffix = "SM: ";

        public static void Log(string format, params object[] message)
        {
#if DEBUG
            Console.WriteLine(format, message);
#else
            Debug.Log(string.Format(format, message));
#endif
        }

        public static void Log(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#else
            Debug.Log(PreSuffix + message);
#endif
        }

        public static void Error(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#else
            Debug.LogError(PreSuffix + message);
#endif
        }

        public static void Except(Exception e)
        {
#if DEBUG
            Console.WriteLine(e.Message);
#else
            Error(e.Message);
#endif
        }


    }
}

