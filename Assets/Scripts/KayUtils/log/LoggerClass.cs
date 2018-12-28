/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 8/19/2017 11:52:53 AM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace KayUtils
{
    /// <summary>
    /// 日志等级声明。
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// 缺省
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 调试
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// 信息
        /// </summary>
        INFO = 2,
        /// <summary>
        /// 警告
        /// </summary>
        WARNING = 4,
        /// <summary>
        /// 错误
        /// </summary>
        ERROR = 8,
        /// <summary>
        /// 异常
        /// </summary>
        EXCEPT = 16,
        /// <summary>
        /// 关键错误
        /// </summary>
        CRITICAL = 32,
    }

    public class LoggerConfig
    {
        public int mFileMaxSize; // kb
        public LogLevel mLogLevel;
        public string mFilePath;
        public string mDirectory;
        public string mFileName;
        public string mFileExtention;
        public int mFlushInterval;
        public void ParseFilePath()
        {
            mFilePath = FileUtils.PathNormalize(mFilePath);
            mDirectory = FileUtils.GetDirectoryName(mFilePath);
            mFileName = FileUtils.GetFileNameWithoutExtention(mFilePath);
            mFileExtention = FileUtils.GetFileExtention(mFilePath);
        }
        static LoggerConfig mDefault = null;
        public static LoggerConfig Default
        {
            get
            {
                if (mDefault == null)
                {
                    mDefault = new LoggerConfig();
                    mDefault.mFileMaxSize = 1024 * 2; // 2m
                    mDefault.mFilePath = "logs/system.log";
                    mDefault.mFlushInterval = 2000;
                    mDefault.mLogLevel = LogLevel.DEBUG | LogLevel.ERROR | LogLevel.EXCEPT | LogLevel.INFO | LogLevel.WARNING;
                }
                return mDefault;
            }
        }
    }

    public class LogFileWriter
    {
        List<string> mCacheMessages = new List<string>();
        LoggerConfig mConfig;
        string mDay;
        string mFilePath;
        int mIndex = 0;
        StreamWriter mFS;
        Thread mThread = null;
        bool isStopped = false;
        public LogFileWriter(LoggerConfig config)
        {
            mConfig = config;
            mDay = DateTimeUtils.FormatTime(DateTime.Now);
            mFilePath = mConfig.mFilePath;
            mFS = new StreamWriter(mFilePath, true, UTF8Encoding.UTF8);
            mFS.AutoFlush = false;
            StartLoging();
        }
        public void Log(string message)
        {
            lock (this)
            {
                mCacheMessages.Add(message);
            }
        }

        public void Stop()
        {
            isStopped = true;
        }

        private void StartLoging()
        {
            if (mThread == null)
            {
                mThread = new Thread(Check);
                mThread.Start();
            }
        }

        void Check()
        {
            while (true)
            {
                List<string> back = null;  
                if (mCacheMessages.Count > 0)
                {
                    lock (this)
                    {
                        back = mCacheMessages;
                        mCacheMessages = new List<string>();
                    }
                }
                if (back != null)
                {
                    foreach (string msg in back)
                    {
                        if (msg.Substring(0, 10) == mDay)
                        {
                            mFS.WriteLine(msg);
                        }
                        else
                        {
                            mFS.Flush();
                            mFS.Close();
                            RenameNextSeq();
                            mIndex = 0;
                            mDay = msg.Substring(0, 10);
                        }
                    }
                    mFS.Flush();
                    FileInfo info = new FileInfo(mFilePath);
                    if ((info.Length >> 10) > mConfig.mFileMaxSize) // kb
                    {
                        mFS.Close();
                        RenameNextSeq();
                    }
                }
                if (isStopped)
                {
                    mFS.Flush();
                    mFS.Close();
                    break;
                }
                else
                {
                    Thread.Sleep(mConfig.mFlushInterval);
                }  
            }

        }

        void RenameNextSeq()
        {
            string rename = null;
            for (int i = mIndex + 1; ; ++i)
            {
                rename = string.Format("{0}/{1}_{2}_{3}.{4}", mConfig.mDirectory, mConfig.mFileName, mDay, i, mConfig.mFileExtention);
                if (!File.Exists(rename))
                {
                    mIndex = i;
                    break;
                }
            }
            File.Move(mFilePath, rename);
            mFS = new StreamWriter(mFilePath, true, UTF8Encoding.Unicode);
            mFS.AutoFlush = false;
        }
    }

    public class LoggerClass
    {
        static LogFileWriter mFileWriter;
        static LoggerConfig Config;
        static Action<string> mLog;
        static bool isInitialize = false;

        public static void Stop()
        {
            if (mFileWriter != null)
            {
                mFileWriter.Stop();
            }
        }

        public static void Initialize(LoggerConfig config)
        {
            if (!isInitialize)
            {
                Config = config;
                Config.ParseFilePath();
                if (!Directory.Exists(Config.mDirectory))
                {
                    Directory.CreateDirectory(Config.mDirectory);
                }
                mFileWriter = new LogFileWriter(Config);
                mLog = mFileWriter.Log;
                isInitialize = true;
            }
        }

        public static void AddLogOut(Action<string> logOut)
        {
            mLog += logOut;
        }

        public static LoggerClass GetLogger(Type classType)
        {
            return new LoggerClass(classType.FullName);
        }

        string mClassFullName;
        private LoggerClass(string classFullName)
        {
            mClassFullName = classFullName;
        }

        public void LogDebug(string message)
        {
            if ((Config.mLogLevel & LogLevel.DEBUG) == LogLevel.DEBUG)
            {
                string msg = GeneratorMessage(LogLevel.DEBUG, message);
                mLog(msg);
            }
        }

        public void LogInfo(string message)
        {
            if ((Config.mLogLevel & LogLevel.INFO) == LogLevel.INFO)
            {
                string msg = GeneratorMessage(LogLevel.INFO, message);
                mLog(msg);
            }
        }

        public void LogWarning(string message)
        {
            if ((Config.mLogLevel & LogLevel.WARNING) == LogLevel.WARNING)
            {
                string msg = GeneratorMessage(LogLevel.WARNING, message);
                mLog(msg);
            }
        }

        public void LogError(string message)
        {
            if ((Config.mLogLevel & LogLevel.ERROR) == LogLevel.ERROR)
            {
                string msg = GeneratorMessage(LogLevel.ERROR, message);
                mLog(msg);
            }
        }

        public void LogExcept(string message)
        {
            if ((Config.mLogLevel & LogLevel.EXCEPT) == LogLevel.EXCEPT)
            {
                string msg = GeneratorMessage(LogLevel.EXCEPT, message);
                mLog(msg);
            }
        }

        public void LogCritical(string message)
        {
            if ((Config.mLogLevel & LogLevel.CRITICAL) == LogLevel.CRITICAL)
            {
                string msg = GeneratorMessage(LogLevel.CRITICAL, message);
                mLog(msg);
            }
        }

        private string GeneratorMessage(LogLevel loglevel, string message)
        {
            return string.Format("{0} {1} {2} {3}", DateTimeUtils.FormatTimeHMS(DateTime.Now), mClassFullName, loglevel, message);
        }
    }
}
