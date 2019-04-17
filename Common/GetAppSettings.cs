using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Common
{
    public static class GetAppSettings
    {
        public static string AppID
        {
            get
            {
                return ConfigurationManager.AppSettings["AppID"].ToString();
            }
        }

        public static string AppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["AppSecret"].ToString();
            }
        }

        public static string SoapUser
        {
            get
            {
                return ConfigurationManager.AppSettings["SoapUser"].ToString();
            }
        }

        public static string SoapPwd
        {
            get
            {
                return ConfigurationManager.AppSettings["SoapPwd"].ToString();
            }
        }
        public static string Token
        {
            get
            {
                return ConfigurationManager.AppSettings["Token"].ToString();
            }
        }

        public static string SysURL
        {
            get
            {
                return ConfigurationManager.AppSettings["SysURL"].ToString();
            }
        }
        public static string WebServiceURL
        {
            get
            {
                return ConfigurationManager.AppSettings["WebServiceURL"].ToString();
            }
        }
    }
}
