/********************************************************************************
** All rights reserved
** Auth�� kay.yang
** E-mail: 1025115216@qq.com
** Date�� 7/28/2017 2:50:22 PM
** Version:  v1.0.0
*********************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace KayUtils
{
    public class XmlFileUtils
    {
        /// <summary>
        /// ��ֵ�ָ����� ��:��
        /// </summary>
        private const Char KEY_VALUE_SPRITER = ':';
        /// <summary>
        /// �ֵ���ָ����� ��,��
        /// </summary>
        private const Char MAP_SPRITER = ',';
        /// <summary>
        /// ����ָ����� ','
        /// </summary>
        private const Char LIST_SPRITER = ';';

        public static T DeserializeFromXml<T>(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                     T defal = default(T);
                     return defal;
                }
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    T ret = (T)xs.Deserialize(reader);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                string error = ex.StackTrace;
                return default(T);
            }
        }
        public static void SerializeToXml<T>(string filePath, T obj)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    xs.Serialize(writer, obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#if UNITY_IPHONE
	    public static void SaveXML(object config, string path)
	    {
		    var root = new System.Security.SecurityElement("root");
		    root.AddChild(new System.Security.SecurityElement("record"));
		    var xml = root.Children[0] as System.Security.SecurityElement;
		    var props = config.GetType().GetProperties();
		    foreach (var item in props)
		    {
			    if (item.Name.Contains("GuideTimes"))
			    {
				    //dictonary
				    var temp = item.GetGetMethod().Invoke(config, null);
				    string value="";
				    foreach(var v in temp as Dictionary<ulong,string>)
				    {
					    value=value+v.Key.ToString()+":"+v.Value+",";
				    }
				    xml.AddChild(new System.Security.SecurityElement(item.Name, value));
			    }
			    else
			    {
				    var value = item.GetGetMethod().Invoke(config, null);
				    xml.AddChild(new System.Security.SecurityElement(item.Name, value.ToString()));
			    }
		    }
		    XMLParser.SaveText(path, root.ToString());
	    }
#endif
        // ������չ�ɵݹ�ģʽ���������Ƕ�׵�
        public static void SaveXML<T>(string path, List<T> data)
        {
            var root = new System.Security.SecurityElement("root");
            string attrName = typeof(T).Name;
            var props = typeof(T).GetProperties();
            var keyProp = props[0];         
            foreach (var prop in props)
            {
                if (XmlData.mKeyFieldName == prop.Name)
                {
                    keyProp = prop;
                    break;
                }
            }
            foreach (var item in data)
            {
                // ������LoadXMLʱ��key id�����ȼ��أ����������һ������ id
                var xml = new System.Security.SecurityElement(attrName);
                object obj = keyProp.GetGetMethod().Invoke(item, null);
                xml.AddChild(new System.Security.SecurityElement(keyProp.Name, obj.ToString()));
                foreach (var prop in props)
                {
                    if (prop == keyProp)
                    {
                        continue;
                    }
                    var type = prop.PropertyType;
                    String result = String.Empty;
                    obj = prop.GetGetMethod().Invoke(item, null);
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        result = typeof(XmlFileUtils).GetMethod("PackMap")
                        .MakeGenericMethod(type.GetGenericArguments())
                        .Invoke(null, new object[] { obj, KEY_VALUE_SPRITER, MAP_SPRITER }).ToString();
                    }
                    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        result = typeof(XmlFileUtils).GetMethod("PackList")
                        .MakeGenericMethod(type.GetGenericArguments())
                        .Invoke(null, new object[] { obj, LIST_SPRITER }).ToString();
                    }
                    else if (type.BaseType == typeof(Enum))
                    {
                        Type underType = Enum.GetUnderlyingType(type);
                        if (underType == typeof(Int32))
                        {
                            obj = EnumConvertUtils.EnumToInt(obj) + "";
                        }
                        result = obj.ToString();
                    }
                    else
                    {
                        result = obj.ToString();
                    }
                    xml.AddChild(new System.Security.SecurityElement(prop.Name, result));
                }
                root.AddChild(xml);
            }
            XMLParser.SaveText(path, root.ToString());
        }

        public static List<T> LoadXML<T>(string path)
        {
            var text = FileUtils.LoadTextFile(path);
            List<T> list = new List<T>();
            try
            {
                if (String.IsNullOrEmpty(text))
                {
                    return list;
                }
                Type type = typeof(T);
                var xml = XMLParser.LoadXML(text);
                Dictionary<Int32, Dictionary<String, String>> map = XMLParser.LoadIntMap(xml, text);
                var props = type.GetProperties();
                foreach (var item in map)
                {
                    var obj = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    foreach (var prop in props)
                    {
                        if (prop.Name == XmlData.mKeyFieldName)
                            prop.SetValue(obj, item.Key, null);
                        else
                            try
                            {
                                if (item.Value.ContainsKey(prop.Name))
                                {
                                    var value = GetValue(item.Value[prop.Name], prop.PropertyType);
                                    prop.SetValue(obj, value, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                SimpleLogger.Log("LoadXML error: " + item.Value[prop.Name] + " " + prop.PropertyType);
                                SimpleLogger.Except(ex);
                            }
                    }
                    list.Add((T)obj);
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.Except(ex);
                SimpleLogger.Error("error text: \n" + text);
            }
            return list;
        }
        /// <summary>
        /// ���ֵ��ַ���ת��Ϊ��������ֵ���Ͷ�Ϊ���͵��ֵ����
        /// </summary>
        /// <param name="strMap">�ֵ��ַ���</param>
        /// <param name="keyValueSpriter">��ֵ�ָ���</param>
        /// <param name="mapSpriter">�ֵ���ָ���</param>
        /// <returns>�ֵ����</returns>
        public static Dictionary<Int32, Int32> ParseMapIntInt(String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<Int32, Int32> result = new Dictionary<Int32, Int32>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int key;
                int value;
                if (int.TryParse(item.Key, out key) && int.TryParse(item.Value, out value))
                {
                    result.Add(key, value);
                }
                else
                {
                    SimpleLogger.Log(String.Format("Parse failure: {0}, {1}", item.Key, item.Value));
                }
            }
            return result;
        }
        /// <summary>
        /// ���ֵ��ַ���ת��Ϊ������Ϊ���ͣ�ֵ����Ϊ�����ȸ��������ֵ����
        /// </summary>
        /// <param name="strMap">�ֵ��ַ���</param>
        /// <param name="keyValueSpriter">��ֵ�ָ���</param>
        /// <param name="mapSpriter">�ֵ���ָ���</param>
        /// <returns>�ֵ����</returns>
        public static Dictionary<Int32, float> ParseMapIntFloat(String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            var result = new Dictionary<Int32, float>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int key;
                float value;
                if (int.TryParse(item.Key, out key) && float.TryParse(item.Value, out value))
                    result.Add(key, value);
                else
                    SimpleLogger.Log(String.Format("Parse failure: {0}, {1}", item.Key, item.Value));
            }
            return result;
        }
        /// <summary>
        /// ���ֵ��ַ���ת��Ϊ������Ϊ���ͣ�ֵ����Ϊ�ַ������ֵ����
        /// </summary>
        /// <param name="strMap">�ֵ��ַ���</param>
        /// <param name="keyValueSpriter">��ֵ�ָ���</param>
        /// <param name="mapSpriter">�ֵ���ָ���</param>
        /// <returns>�ֵ����</returns>
        public static Dictionary<Int32, String> ParseMapIntString(String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<Int32, String> result = new Dictionary<Int32, String>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int key;
                if (int.TryParse(item.Key, out key))
                    result.Add(key, item.Value);
                else
                    SimpleLogger.Log(String.Format("Parse failure: {0}", item.Key));
            }
            return result;
        }
        /// <summary>
        /// ���ֵ��ַ���ת��Ϊ������Ϊ�ַ�����ֵ����Ϊ�����ȸ��������ֵ����
        /// </summary>
        /// <param name="strMap">�ֵ��ַ���</param>
        /// <param name="keyValueSpriter">��ֵ�ָ���</param>
        /// <param name="mapSpriter">�ֵ���ָ���</param>
        /// <returns>�ֵ����</returns>
        public static Dictionary<String, float> ParseMapStringFloat(String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<String, float> result = new Dictionary<String, float>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                float value;
                if (float.TryParse(item.Value, out value))
                    result.Add(item.Key, value);
                else
                    SimpleLogger.Log(String.Format("Parse failure: {0}", item.Value));
            }
            return result;
        }
        /// <summary>
        /// ���ֵ��ַ���ת��Ϊ������Ϊ�ַ�����ֵ����Ϊ���͵��ֵ����
        /// </summary>
        /// <param name="strMap">�ֵ��ַ���</param>
        /// <param name="keyValueSpriter">��ֵ�ָ���</param>
        /// <param name="mapSpriter">�ֵ���ָ���</param>
        /// <returns>�ֵ����</returns>
        public static Dictionary<String, Int32> ParseMapStringInt(String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<String, Int32> result = new Dictionary<String, Int32>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int value;
                if (int.TryParse(item.Value, out value))
                    result.Add(item.Key, value);
                else
                    SimpleLogger.Log(String.Format("Parse failure: {0}", item.Value));
            }
            return result;
        }
        /// <summary>
        /// ���ֵ��ַ���ת��Ϊ������Ϊ T��ֵ����Ϊ U ���ֵ����
        /// </summary>
        /// <typeparam name="T">�ֵ�Key����</typeparam>
        /// <typeparam name="U">�ֵ�Value����</typeparam>
        /// <param name="strMap">�ֵ��ַ���</param>
        /// <param name="keyValueSpriter">��ֵ�ָ���</param>
        /// <param name="mapSpriter">�ֵ���ָ���</param>
        /// <returns>�ֵ����</returns>
        public static Dictionary<T, U> ParseMapAny<T, U>(String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            var typeT = typeof(T);
            var typeU = typeof(U);
            var result = new Dictionary<T, U>();
            //��תΪ�ֵ�
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                try
                {
                    T key = (T)GetValue(item.Key, typeT);
                    U value = (U)GetValue(item.Value, typeU);

                    result.Add(key, value);
                }
                catch (Exception)
                {
                    SimpleLogger.Log(String.Format("Parse failure: {0}, {1}", item.Key, item.Value));
                }
            }
            return result;
        }
        /// <summary>
        /// ���ֵ��ַ���ת��Ϊ��������ֵ���Ͷ�Ϊ�ַ������ֵ����
        /// </summary>
        /// <param name="strMap">�ֵ��ַ���</param>
        /// <param name="keyValueSpriter">��ֵ�ָ���</param>
        /// <param name="mapSpriter">�ֵ���ָ���</param>
        /// <returns>�ֵ����</returns>
        public static Dictionary<String, String> ParseMap(String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            if (String.IsNullOrEmpty(strMap))
            {
                return result;
            }

            var map = strMap.Split(mapSpriter);//�����ֵ���ָ����ָ��ַ�������ȡ��ֵ���ַ���
            for (int i = 0; i < map.Length; i++)
            {
                if (String.IsNullOrEmpty(map[i]))
                {
                    continue;
                }

                var keyValuePair = map[i].Split(keyValueSpriter);//���ݼ�ֵ�ָ����ָ��ֵ���ַ���
                if (keyValuePair.Length == 2)
                {
                    if (!result.ContainsKey(keyValuePair[0]))
                        result.Add(keyValuePair[0], keyValuePair[1]);
                    else
                        SimpleLogger.Log(String.Format("Key {0} already exist, index {1} of {2}.", keyValuePair[0], i, strMap));
                }
                else
                {
                    SimpleLogger.Log(String.Format("KeyValuePair are not match: {0}, index {1} of {2}.", map[i], i, strMap));
                }
            }
            return result;
        }
        /// <summary>
        /// ���ֵ����ת��Ϊ�ֵ��ַ�����
        /// </summary>
        /// <typeparam name="T">�ֵ�Key����</typeparam>
        /// <typeparam name="U">�ֵ�Value����</typeparam>
        /// <param name="map">�ֵ����</param>
        /// <returns>�ֵ��ַ���</returns>
        public static String PackMap<T, U>(IEnumerable<KeyValuePair<T, U>> map, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            if (map.Count() == 0)
                return "";
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in map)
                {
                    sb.AppendFormat("{0}{1}{2}{3}", item.Key, keyValueSpriter, item.Value, mapSpriter);
                }
                return sb.ToString().Remove(sb.Length - 1, 1);
            }
        }
        /// <summary>
        /// ���б��ַ���ת��Ϊ����Ϊ T ���б����
        /// </summary>
        /// <typeparam name="T">�б�ֵ��������</typeparam>
        /// <param name="strList">�б��ַ���</param>
        /// <param name="listSpriter">����ָ���</param>
        /// <returns>�б����</returns>
        public static List<T> ParseListAny<T>(String strList, Char listSpriter = LIST_SPRITER)
        {
            var type = typeof(T);
            var list = ParseList(strList, listSpriter);
            var result = new List<T>();
            foreach (var item in list)
            {
                result.Add((T)GetValue(item, type));
            }
            return result;
        }
        /// <summary>
        /// ���б��ַ���ת��Ϊ�ַ������б����
        /// </summary>
        /// <param name="strList">�б��ַ���</param>
        /// <param name="listSpriter">����ָ���</param>
        /// <returns>�б����</returns>
        public static List<String> ParseList(String strList, Char listSpriter = LIST_SPRITER)
        {
            var result = new List<String>();
            if (String.IsNullOrEmpty(strList))
                return result;

            var trimString = strList.Trim();
            if (String.IsNullOrEmpty(strList))
            {
                return result;
            }
            var detials = trimString.Split(listSpriter);//.Substring(1, trimString.Length - 2)
            foreach (var item in detials)
            {
                if (!String.IsNullOrEmpty(item))
                    result.Add(item.Trim());
            }

            return result;
        }
        /// <summary>
        /// ���б����ת��Ϊ�б��ַ�����
        /// </summary>
        /// <typeparam name="T">�б�ֵ��������</typeparam>
        /// <param name="list">�б����</param>
        /// <param name="listSpriter">�б�ָ���</param>
        /// <returns>�б��ַ���</returns>
        public static String PackList<T>(List<T> list, Char listSpriter = LIST_SPRITER)
        {
            if (list.Count == 0)
                return "";
            else
            {
                StringBuilder sb = new StringBuilder();
                //sb.Append("[");
                foreach (var item in list)
                {
                    sb.AppendFormat("{0}{1}", item, listSpriter);
                }
                sb.Remove(sb.Length - 1, 1);
                //sb.Append("]");

                return sb.ToString();
            }

        }
        public static String PackArray<T>(T[] array, Char listSpriter = LIST_SPRITER)
        {
            var list = new List<T>();
            list.AddRange(array);
            return PackList(list, listSpriter);
        }
        /// <summary>
        /// ���ַ���ת��Ϊ��Ӧ���͵�ֵ��
        /// </summary>
        /// <param name="value">�ַ���ֵ����</param>
        /// <param name="type">ֵ������</param>
        /// <returns>��Ӧ���͵�ֵ</returns>
        public static object GetValue(String value, Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type == typeof(string))
            {
                return value;
            }
            if (type == typeof(Int32))
            {
                return Convert.ToInt32(Convert.ToDouble(value == "" ? "-1" : value));
            }
            if (type == typeof(float))
            {
                return float.Parse(value == "" ? "0.0" : value);
            }
            if (type == typeof(byte))
            {
                return Convert.ToByte(Convert.ToDouble(value == "" ? "0" : value));
            }
            if (type == typeof(sbyte))
            {
                return Convert.ToSByte(Convert.ToDouble(value == "" ? "0" : value));
            }
            if (type == typeof(UInt32))
            {
                return Convert.ToUInt32(Convert.ToDouble(value == "" ? "0" : value));
            }
            if (type == typeof(Int16))
            {
                return Convert.ToInt16(Convert.ToDouble(value == "" ? "0" : value));
            }
            if (type == typeof(Int64))
            {
                return Convert.ToInt64(Convert.ToDouble(value == "" ? "0" : value));
            }
            if (type == typeof(UInt16))
            {
                return Convert.ToUInt16(Convert.ToDouble(value == "" ? "0" : value));
            }
            if (type == typeof(UInt64))
            {
                return Convert.ToUInt64(Convert.ToDouble(value == "" ? "0" : value));
            }
            if (type == typeof(double))
            {
                return double.Parse(value == "" ? "0.0" : value);
            }
            if (type == typeof(bool))
            {
                if (value == "0")
                {
                    return false;
                }
                else if (value == "1")
                {
                    return true;
                }
                else
                {
                    return bool.Parse(value == "" ? "0" : value);
                }
            }
            if (type.BaseType == typeof(Enum))
            {
                return GetValue(value == "" ? "0" : value, Enum.GetUnderlyingType(type));
            }
            if (type == typeof(Vector4))
            {
                Vector4 result;
                ParseVector4(value == "" ? "0,0,0,0" : value, out result);
                return result;
            }
            if (type == typeof(Vector3))
            {
                Vector3 result;
                ParseVector3(value == "" ? "0,0,0" : value, out result);
                return result;
            }
            if (type == typeof(Vector2))
            {
                Vector2 result;
                ParseVector2(value == "" ? "0,0" : value, out result);
                return result;
            }
            if (type == typeof(Quaternion))
            {
                Quaternion result;
                ParseQuaternion(value == "" ? "0,0,0,1" : value, out result);
                return result;
            }
            if (type == typeof(Color))
            {
                Color result;
                ParseColor(value == "" ? "0,0,0,0" : value, out result);
                return result;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type[] types = type.GetGenericArguments();
                var map = ParseMap(value);
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in map)
                {
                    var key = GetValue(item.Key, types[0]);
                    var v = GetValue(item.Value, types[1]);
                    type.GetMethod("Add").Invoke(result, new object[] { key, v });
                }
                return result;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type t = type.GetGenericArguments()[0];
                var list = ParseList(value);
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in list)
                {
                    var v = GetValue(item, t);
                    type.GetMethod("Add").Invoke(result, new object[] { v });
                }
                return result;
            }
            return null;
        }
        /// <summary>
        /// ��ָ����ʽ(255, 255, 255, 255) ת��Ϊ Color 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="result"></param>
        /// <returns>���� true/false ��ʾ�Ƿ�ɹ�</returns>
        public static bool ParseColor(string inputString, out Color result)
        {
            string trimString = inputString.Trim();
            trimString = trimString.Replace("(", "");
            trimString = trimString.Replace(")", "");
            result = new Color();
            try
            {
                string[] detail = trimString.Split(',');
                for (int i = 0; i < detail.Length; ++i)
                {
                    result[i] = float.Parse(detail[i].Trim()) / 255;
                }
                return true;
            }
            catch (Exception e)
            {
                SimpleLogger.Error("Parse Color error: " + trimString + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// ��ָ����ʽ(1.0, 2) ת��Ϊ Vector2
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="result"></param>
        /// <returns>���� true/false ��ʾ�Ƿ�ɹ�</returns>
        public static bool ParseVector2(string inputString, out Vector2 result)
        {
            string trimString = inputString.Trim();
            trimString = trimString.Replace("(", "");
            trimString = trimString.Replace(")", "");
            result = new Vector2();
            try
            {
                string[] detail = trimString.Split(',');
                for (int i = 0; i < detail.Length; ++i)
                {
                    result[i] = float.Parse(detail[i].Trim());
                }
                return true;
            }
            catch (Exception e)
            {
                SimpleLogger.Error("Parse Vector3 error: " + trimString + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// ��ָ����ʽ(1.0, 2, 3.4) ת��Ϊ Vector3 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="result"></param>
        /// <returns>���� true/false ��ʾ�Ƿ�ɹ�</returns>
        public static bool ParseVector3(string inputString, out Vector3 result)
        {
            string trimString = inputString.Trim();
            trimString = trimString.Replace("(", "");
            trimString = trimString.Replace(")", "");
            result = new Vector3();
            try
            {
                string[] detail = trimString.Split(',');
                for (int i = 0; i < detail.Length; ++i)
                {
                    result[i] = float.Parse(detail[i].Trim());
                }
                return true;
            }
            catch (Exception e)
            {
                SimpleLogger.Error("Parse Vector3 error: " + trimString + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// ��ָ����ʽ(1.0, 2, 3.4, 1.0) ת��Ϊ Vector4
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="result"></param>
        /// <returns>���� true/false ��ʾ�Ƿ�ɹ�</returns>
        public static bool ParseVector4(string inputString, out Vector4 result)
        {
            string trimString = inputString.Trim();
            trimString = trimString.Replace("(", "");
            trimString = trimString.Replace(")", "");
            result = new Vector4();
            try
            {
                string[] detail = trimString.Split(',');
                for (int i = 0; i < detail.Length; ++i)
                {
                    result[i] = float.Parse(detail[i].Trim());
                }
                return true;
            }
            catch (Exception e)
            {
                SimpleLogger.Error("Parse Vector3 error: " + trimString + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// ��ָ����ʽ(1.0, 2, 3.4) ת��Ϊ Vector3 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="result"></param>
        /// <returns>���� true/false ��ʾ�Ƿ�ɹ�</returns>
        public static bool ParseQuaternion(string inputString, out Quaternion result)
        {
            string trimString = inputString.Trim();
            trimString = trimString.Replace("(", "");
            trimString = trimString.Replace(")", "");
            result = new Quaternion();
            try
            {
                string[] detail = trimString.Split(',');
                for (int i = 0; i < detail.Length; ++i)
                {
                    result[i] = float.Parse(detail[i].Trim());
                }
                return true;
            }
            catch (Exception e)
            {
                SimpleLogger.Error("Parse Quaternion error: " + trimString + e.Message);
                return false;
            }
        }
    }
}