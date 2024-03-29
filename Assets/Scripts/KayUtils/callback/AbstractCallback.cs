﻿/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 8/18/2017 4:46:46 PM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KayUtils
{
    public abstract class AbstractCallback
    {
        public abstract Delegate Handler
        {
            get;
            set;
        }

        public abstract void Run();
    }

    public class Callback : AbstractCallback
    {
        private Action mAction;

        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action; }
        }

        public override void Run()
        {
            mAction();
        }
    }

    public class Callback<T> : AbstractCallback
    {
        private Action<T> mAction;
        private T mArg1;

        public T Arg1
        {
            get { return mArg1; }
            set { mArg1 = value; }
        }
        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action<T>; }
        }

        public override void Run()
        {
            mAction(mArg1);
        }
    }

    public class Callback<T, U> : AbstractCallback
    {
        private Action<T, U> mAction;
        private T mArg1;
        private U mArg2;

        public T Arg1
        {
            get { return mArg1; }
            set { mArg1 = value; }
        }

        public U Arg2
        {
            get { return mArg2; }
            set { mArg2 = value; }
        }
        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action<T, U>; }
        }

        public override void Run()
        {
            mAction(mArg1, mArg2);
        }
    }

    public class Callback<T, U, V> : AbstractCallback
    {
        private Action<T, U, V> mAction;
        private T mArg1;
        private U mArg2;
        private V mArg3;

        public T Arg1
        {
            get { return mArg1; }
            set { mArg1 = value; }
        }

        public U Arg2
        {
            get { return mArg2; }
            set { mArg2 = value; }
        }

        public V Arg3
        {
            get { return mArg3; }
            set { mArg3 = value; }
        }

        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action<T, U, V>; }
        }

        public override void Run()
        {
            mAction(mArg1, mArg2, mArg3);
        }
    }
    public class Callback<T, U, V, W> : AbstractCallback
    {
        private Action<T, U, V, W> mAction;
        private T mArg1;
        private U mArg2;
        private V mArg3;
        private W mArg4;

        public T Arg1
        {
            get { return mArg1; }
            set { mArg1 = value; }
        }

        public U Arg2
        {
            get { return mArg2; }
            set { mArg2 = value; }
        }

        public V Arg3
        {
            get { return mArg3; }
            set { mArg3 = value; }
        }
        public W Arg4
        {
            get { return mArg4; }
            set { mArg4 = value; }
        }
        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action<T, U, V, W>; }
        }

        public override void Run()
        {
            mAction(mArg1, mArg2, mArg3, mArg4);
        }
    }
}
