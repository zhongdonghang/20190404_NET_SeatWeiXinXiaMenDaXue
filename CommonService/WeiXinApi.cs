using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using Common;

namespace CommonService
{
    public static class WeiXinApi
    {
        public static string GetToken()
        {

            if (!AccessTokenContainer.CheckRegistered(GetAppSettings.AppID))//检查是否已经注册
            {
                AccessTokenContainer.Register(GetAppSettings.AppID, GetAppSettings.AppSecret);//如果没有注册则进行注册
            }
            var result = AccessTokenContainer.GetAccessTokenResult(GetAppSettings.AppID); //获取AccessToken结果

            return result.access_token;
        }
    }
}
