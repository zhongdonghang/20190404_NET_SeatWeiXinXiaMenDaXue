using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiXinMsgService;
using WeiXinMsgService.Template;

namespace WeChatMsg
{
    /// <summary>
    /// Summary description for MsgHandler
    /// </summary>
    public class MsgHandler : IHttpHandler
    {
        Com service = new Com();
        private string signature = "";//微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。 
        private string timestamp = "";//时间戳 
        private string nonce = "";//随机数 
        private string echostr = "";//随机字符串 
        private HttpContext context = null;

        public void ProcessRequest(HttpContext context)
        {
            SeatManage.SeatManageComm.WriteLog.Write("1:ProcessRequest" );

            this.context = context;
            context.Response.ContentType = "text/plain";
            if (this.context.Request.HttpMethod == "POST")
            {
                SeatManage.SeatManageComm.WriteLog.Write("POST");

                //如果是POST请求，则响应请求内容
                // ResponseMsg(string OPENID,string first,string keyword1,string keyword2,string keyword3,string remark)
                try
                {
                   string msg = AESAlgorithm.AESDecrypt(context.Request.Params["msg"]);
                    //SchoolNum=201812221&StudentNo=30320182200067&MsgType=UserOperation&Room=玉堂&SeatNo=151&AddTime=2019-04-22 16:17:12&EndTime=&Days=VRType=&Msg=在终端20181222111手动选择，玉堂，151号座位

                    string[] kv =  msg.Split('&');
                    
                    // SeatManage.SeatManageComm.WriteLog.Write(msg);
                    /* 广西楚惟服务器的算法
                    string OPENID = context.Request.Params["OPENID"].ToString();
                    string first = context.Request.Params["first"].ToString();
                    string keyword1 = context.Request.Params["keyword1"].ToString();
                    string keyword2 = context.Request.Params["keyword2"].ToString();
                    string keyword3 = context.Request.Params["keyword3"].ToString();
                    string remark = context.Request.Params["remark"].ToString();
                    SeatManage.SeatManageComm.WriteLog.Write(OPENID + "$$$$" + first + "$$$$" + keyword1 + "$$$$" + keyword2 + "$$$$" + keyword3 + "$$$$" + remark);
                    ResponseMsg(OPENID, first, keyword1, keyword2, keyword3, remark);
                    */
                }
                catch (Exception ex)
                {
                    SeatManage.SeatManageComm.WriteLog.Write("5:Exception ex"+ex);
                   // throw;
                }
            }
            else if (this.context.Request.HttpMethod == "GET")//如果是Get请求，则是接入验证，返回随机字符串。
            {
                SeatManage.SeatManageComm.WriteLog.Write("3:GET");
                signature = context.Request.Params["signature"];
                timestamp = context.Request.Params["timestamp"];
                nonce = context.Request.Params["nonce"];
                echostr = context.Request.Params["echostr"];
                
                if (!service.CheckSignature(signature, timestamp, nonce))//验证请求是否微信发过来的。
                {//不是则结束响应
                    SeatManage.SeatManageComm.WriteLog.Write("signature:"+ signature);
                    SeatManage.SeatManageComm.WriteLog.Write("timestamp:"+ timestamp);
                    SeatManage.SeatManageComm.WriteLog.Write("nonce:"+ nonce);
                    SeatManage.SeatManageComm.WriteLog.Write("no CheckSignature");
                    this.context.Response.End();
                }
                SeatManage.SeatManageComm.WriteLog.Write("CheckSignature%%%%CheckSignature");
                context.Response.Write(echostr);
            }
        }


        private void ResponseMsg(string OPENID,string first,string keyword1,string keyword2,string keyword3,string remark)
        {
            try
            {
                SeatManage.SeatManageComm.WriteLog.Write("4:ResponseMsg");
                string tempID = "oLdL1fEDKZsPdc9njn7V2yioHhFWpQQy1JyfxKV_NsM";//座位状态变更通知模板ID
                string access_tocken = Com.IsExistAccess_Token("wx5c27898c83a612dc", "51fed2e73dddd6e6eabbb528c973074d");

                SendTools tools = new SendTools();
                TemplateModel m = new TemplateModel(first, keyword1, keyword2, keyword3, remark);
                m.touser = OPENID;
                m.template_id = tempID;
                m.url = "https://lib.xmu.edu.cn/seatwx/NewUser/MySeat";
                m.topcolor = "#FF0000";
                OpenApiResult result = tools.SendTemplateMessage(access_tocken, m);
                SeatManage.SeatManageComm.WriteLog.Write("msg_id" + result.msg_id + "error_code" + result.error_code + "error_msg" + result.error_msg);

            }
            catch (Exception ex)
            {
                SeatManage.SeatManageComm.WriteLog.Write(ex.ToString());
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}