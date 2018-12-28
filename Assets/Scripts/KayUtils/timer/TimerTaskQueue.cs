using System;
using System.Diagnostics;

using KayAlgorithm;
using KayDatastructure;
namespace KayUtils
{
    public class TimerTask
    {
        private uint mTimerId;
        private ulong mNextTick;
        private int mInterval;
        public AbstractCallback mCallback;

        public TimerTask(AbstractCallback callback)
        {
            mCallback = callback;
        }

        public uint TimerId
        {
            get { return mTimerId; }
            set { mTimerId = value; }
        }

        public int Interval
        {
            get { return mInterval; }
            set { mInterval = value; }
        }

        public ulong NextTick
        {
            get { return mNextTick; }
            set { mNextTick = value; }
        }

        public AbstractCallback Callback
        {
            get { return mCallback; }
            set { mCallback = value; }
        }
        public void DoAction()
        {
            mCallback.Run();
        }
    }

    public class TimerTaskQueue
    {
        private static uint mNextTimerId;
        private static uint mCurrentTick;
        private static KeyedPriorityQueue<uint, TimerTask, ulong> mPriorityQueue;
        private static Stopwatch mStopWatch;
        private static readonly object mQueueLock = new object();

        private TimerTaskQueue() { }

        static TimerTaskQueue()
        {
            mPriorityQueue = new KeyedPriorityQueue<uint, TimerTask, ulong>();
            mStopWatch = new Stopwatch();
        }

        public static uint AddTimer(uint start, int interval, Action handler)
        {
            Callback callback = new Callback();
            callback.Handler = handler;
            var p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }

        public static uint AddTimer<T>(uint start, int interval, Action<T> handler, T arg1)
        {
            Callback<T> callback = new Callback<T>();
            callback.Arg1 = arg1;
            callback.Handler = handler;
            var p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }

        public static uint AddTimer<T, U>(uint start, int interval, Action<T, U> handler, T arg1, U arg2)
        {
            Callback<T, U> callback = new Callback<T, U>();
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            callback.Handler = handler;
            var p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }

        public static uint AddTimer<T, U, V>(uint start, int interval, Action<T, U, V> handler, T arg1, U arg2, V arg3)
        {
            Callback<T, U, V> callback = new Callback<T, U, V>();
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            callback.Arg3 = arg3;
            callback.Handler = handler;
            var p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }
        public static uint AddTimer<T, U, V, W>(uint start, int interval, Action<T, U, V> handler, T arg1, U arg2, V arg3, W arg4)
        {
            Callback<T, U, V, W> callback = new Callback<T, U, V, W>();
            callback.Arg1 = arg1;
            callback.Arg2 = arg2;
            callback.Arg3 = arg3;
            callback.Arg4 = arg4;
            callback.Handler = handler;
            var p = GetTimerData(callback, start, interval);
            return AddTimer(p);
        }
        public static void DelTimer(uint timerId)
        {
            lock (mQueueLock)
            {
                mPriorityQueue.Remove(timerId);
            }
        }

        public static void Tick()
        {
            mCurrentTick += (uint)mStopWatch.ElapsedMilliseconds;
            mStopWatch.Reset();
            mStopWatch.Start();
            while (mPriorityQueue.Count != 0)
            {
                TimerTask p;
                lock (mQueueLock)
                {
                    p = mPriorityQueue.Peek();
                }
                if (mCurrentTick < p.NextTick)
                {
                    break;
                }
                lock (mQueueLock)
                {
                    mPriorityQueue.Dequeue();
                }
                if (p.Interval > 0)
                {
                    p.NextTick += (ulong)p.Interval;
                    lock (mQueueLock)
                    {
                        mPriorityQueue.Enqueue(p.TimerId, p, p.NextTick);
                    }
                    p.DoAction();
                }
                else
                {
                    p.DoAction();
                }
            }
        }

        public static void Reset()
        {
            mCurrentTick = 0;
            mNextTimerId = 0;
            mStopWatch.Stop();
            lock (mQueueLock)
            {
                while (mPriorityQueue.Count != 0)
                {
                    mPriorityQueue.Dequeue();
                }
            }
        }

        private static uint AddTimer(TimerTask p)
        {
            lock (mQueueLock)
            {
                mPriorityQueue.Enqueue(p.TimerId, p, p.NextTick);
            }
            return p.TimerId;
        }

        private static TimerTask GetTimerData(AbstractCallback callback, uint start, int interval)
        {
            TimerTask task = new TimerTask(callback);
            task.Interval = interval;
            task.TimerId = ++mNextTimerId;
            task.NextTick = mCurrentTick + 1 + start;
            return task;
        }
    }

}

