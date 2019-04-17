using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Web.Script.Serialization;
namespace SeatManage.SeatManageComm
{
    /// <summary>
    /// Json序列化
    /// </summary>
    public class JSONSerializer
    {

        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        /// <summary>
        /// 对象序列化成字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(Object obj)
        {
            try
            {
                string result = serializer.Serialize(obj);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }
        /// <summary>
        /// 字符串序列化成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>(string input)
        {
            try
            {
                T obj = serializer.Deserialize<T>(input);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<T> JSONStringToList<T>(string JsonStr)
        {
            try
            {
                List<T> objs = serializer.Deserialize<List<T>>(JsonStr);
                return objs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}

