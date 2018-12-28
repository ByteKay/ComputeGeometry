using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using UnityEngine;

namespace KayUtils
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlDataAttribute : Attribute
    {
        string mFileName;
        string mDescription;
        public XmlDataAttribute(string fileName, string desc)
        {
            mFileName = fileName;
            mDescription = desc;
        }

        public string FileName
        {
            get
            {
                return mFileName;
            }
            set
            {
                mFileName = value;
            }
        }

        public string Description
        {
            get
            {
                return mDescription;
            }
            set
            {
                mDescription = value;
            }
        }
    }
    public abstract class XmlData
    {
        public const string mKeyFieldName = "mId";
        public const string mKeyValueFieldName = "DataMap";
        public const string mXmlSuffix = ".xml";
        public int mId
        {
            get;
            set;
        }
    }
    public abstract class XmlData<T> : XmlData where T : XmlData<T>
    {
        private static Dictionary<int, T> mDataMap = null;
        public static Dictionary<int, T> DataMap
        {
            get
            {
                if (mDataMap == null)
                {
                    GetDataMap();
                }
                return mDataMap;
            }
            set 
            {
                mDataMap = value; 
            }
        }
        private static void GetDataMap()
        {
            var type = typeof(T);
            XmlDataAttribute attr = XmlDataAttribute.GetCustomAttribute(type, typeof(XmlDataAttribute), false) as XmlDataAttribute;
            if (attr != null)
            {
                string fileName = attr.FileName;
                var result = XmlDataLoader.Instance.FormatXMLData(fileName, typeof(Dictionary<int, T>), type);
                mDataMap = result as Dictionary<int, T>;
            }
            else
            {
                mDataMap = new Dictionary<int, T>();
            }
        }
    }
    public class XmlDataLoader
    {
        public static XmlDataLoader Instance = new XmlDataLoader();
        protected Action<int, int, string> mProgress = null;
        protected Action mFinished = null;
        protected string mNameSpace;
        protected Type mType;
        protected string mBaseDirectory;
        public void Init(Type type, string baseDirectory, Action<int, int, string> progress = null, Action finished = null)
        {
            mProgress = progress;
            mFinished = finished;
            if (mProgress == null)
            {
                mProgress = DefaultProgress;
            }
            if (mFinished == null)
            {
                mFinished = DefaultCompleted;
            }
            mNameSpace = type.Namespace;
            mType = type;
            mBaseDirectory = baseDirectory;
        }
        public object FormatXMLData(string fileName, Type dicType, Type type)
        {
            object result = null;
            try
            {
                result = dicType.GetConstructor(Type.EmptyTypes).Invoke(null);
                Dictionary<Int32, Dictionary<String, String>> map;//Int32 为 mId, string 为 属性名, string 为 属性值
                string tempFile = fileName;
                if (!tempFile.EndsWith(XmlData.mXmlSuffix))
                {
                    tempFile = fileName + XmlData.mXmlSuffix;
                }
                tempFile = string.Format("{0}/{1}", mBaseDirectory, tempFile);
                if (XMLParser.LoadIntMap(tempFile, out map))
                {
                    var props = type.GetProperties();//获取实体属性
                    foreach (var item in map)
                    {
                        var t = type.GetConstructor(Type.EmptyTypes).Invoke(null);//构造实体实例
                        foreach (var prop in props)
                        {
                            if (prop.Name == XmlData.mKeyFieldName)
                            {
                                prop.SetValue(t, item.Key, null);
                            }
                            else
                            {
                                if (item.Value.ContainsKey(prop.Name))
                                {
                                    var value = XmlFileUtils.GetValue(item.Value[prop.Name], prop.PropertyType);
                                    prop.SetValue(t, value, null);
                                }
                            }
                        }
                        dicType.GetMethod("Add").Invoke(result, new object[] { item.Key, t });
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.Error(ex.Message);
            }
            return result;
        }
        public void LoadData()
        {
            try
            {
                List<Type> gameDataType = new List<Type>();
                Assembly ass = mType.Assembly;
                var types = ass.GetTypes();
                foreach (var item in types)
                {
                    if (item.Namespace == mNameSpace)
                    {
                        var type = item.BaseType;
                        while (type != null)
                        {
                            if (type == typeof(XmlData) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(XmlData<>)))
                            {
                                gameDataType.Add(item);
                                break;
                            }
                            else
                            {
                                type = type.BaseType;
                            }
                        }
                    }
                }
                LoadData(gameDataType);
                GC.Collect();
                if (mFinished != null)
                {
                    mFinished();
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.Error(ex.Message);
            }
        }
        private void LoadData(List<Type> gameDataType)
        {
            var count = gameDataType.Count;
            var i = 1;
            foreach (var item in gameDataType)
            {
                var p = item.GetProperty(XmlData.mKeyValueFieldName, ~BindingFlags.DeclaredOnly);
                XmlDataAttribute attr = XmlDataAttribute.GetCustomAttribute(item, typeof(XmlDataAttribute), false) as XmlDataAttribute;
                if (p != null && attr != null)
                {
                    var result = FormatXMLData(attr.FileName, p.PropertyType, item);
                    p.GetSetMethod().Invoke(null, new object[] { result });
                }
                string name = attr != null ? attr.FileName : null;
                if (mProgress != null)
                    mProgress(i, count, name);
                i++;
            }
        }
        private static void DefaultCompleted()
        {
            SimpleLogger.Log("successfully completed!");
        }
        private static void DefaultProgress(int index, int count, string name)
        {
            SimpleLogger.Log(string.Format("{0} {1} of {2} completed", name, index, count));
        }
    }
}