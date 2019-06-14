using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeiXinMsgService.OutMsg;
using SendHelp = Senparc.Weixin.CommonAPIs.CommonJsonSend;

namespace WeiXinMsgService.Template
{
 public   class SendTools
    {
        public OpenApiResult SendTemplateMessage(string token, TemplateModel model)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}", token);
            try
            {
                var res = SendHelp.Send<OpenApiResult>(token, url, model);
                return res;
            }
            catch (Exception e)
            {
                return new OpenApiResult() { error_code = -1, error_msg = e.Message };
            }

        }

        /// <summary>
        /// 发送外部消息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public OpenApiResult SendTempMessage(string token, TempModel model)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}", token);
            try
            {
                var res = SendHelp.Send<OpenApiResult>(token, url, model);
                return res;
            }
            catch (Exception e)
            {
                return new OpenApiResult() { error_code = -1, error_msg = e.Message };
            }

        }
    }
}
