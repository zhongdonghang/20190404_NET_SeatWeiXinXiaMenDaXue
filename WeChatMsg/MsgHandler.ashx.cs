﻿using System;
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
            this.context = context;
            context.Response.ContentType = "text/plain";
            if (this.context.Request.HttpMethod == "POST")
            {
                //如果是POST请求，则响应请求内容
                // ResponseMsg(string OPENID,string first,string keyword1,string keyword2,string keyword3,string remark)
                string OPENID = context.Request.Params["OPENID"].ToString();
                string first = context.Request.Params["first"].ToString();
                string keyword1 = context.Request.Params["keyword1"].ToString();
                string keyword2 = context.Request.Params["keyword2"].ToString();
                string keyword3 = context.Request.Params["keyword3"].ToString();
                string remark = context.Request.Params["remark"].ToString();
              //  SeatManage.SeatManageComm.WriteLog.Write(OPENID+"$$$$"+ first + "$$$$" + keyword1 + "$$$$" + keyword2 + "$$$$" + keyword3 + "$$$$" + remark);
                ResponseMsg( OPENID,  first,  keyword1,  keyword2,  keyword3,  remark);
            }
            else if (this.context.Request.HttpMethod == "GET")//如果是Get请求，则是接入验证，返回随机字符串。
            {
                signature = context.Request.Params["signature"];
                timestamp = context.Request.Params["timestamp"];
                nonce = context.Request.Params["nonce"];
                echostr = context.Request.Params["echostr"];
                
                if (!service.CheckSignature(signature, timestamp, nonce))//验证请求是否微信发过来的。
                {//不是则结束响应
                    SeatManage.SeatManageComm.WriteLog.Write("no CheckSignature");
                    this.context.Response.End();
                }
                SeatManage.SeatManageComm.WriteLog.Write("CheckSignature");
                context.Response.Write(echostr);
            }
        }


        private void ResponseMsg(string OPENID,string first,string keyword1,string keyword2,string keyword3,string remark)
        {
            try
            {
                string tempID = "WY4Z5Z0rigwhaSBl3onTma-VZC2sdTlHNviIXHV1n2s";//座位状态变更通知模板ID
                string access_tocken = Com.IsExistAccess_Token("wx2b5a801cd0aa12c7", "42ac52963fadb13b2abff3a4c374888e");

                SendTools tools = new SendTools();
                TemplateModel m = new TemplateModel(first, keyword1, keyword2, keyword3, remark);
                m.touser = OPENID;
                m.template_id = tempID;
                m.url = "http://wechat.gxchuwei.com/NewUser/MySeat";
                m.topcolor = "#FF0000";
                OpenApiResult result = tools.SendTemplateMessage(access_tocken, m);
              //  SeatManage.SeatManageComm.WriteLog.Write("msg_id" + result.msg_id + "error_code" + result.error_code + "error_msg" + result.error_msg);

            }
            catch (Exception ex)
            {
                SeatManage.SeatManageComm.WriteLog.Write(ex.ToString());
            }
        }

        #region MyRegion
        //string json = "{"+
        //                    " \"touser\": \""+ OPENID + "\","+
        //                    " \"template_id\": \""+ tempID + "\","+
        //                    " \"url\": \"http://wechat.gxchuwei.com/User/SeatState\"," +
        //                    " \"topcolor\": \"#FF0000\","+
        //                    " \"data\": { "+
        //                        " \"first\": {" +
        //                                    " \"value\":\""+ first + "\","+
        //                                    " \"color\":\"#173177\""+
        //                                " }," +
        //                         " \"keyword1\": {" +
        //                                    " \"value\":\""+ keyword1 + "\"," +
        //                                    " \"color\":\"#173177\"" +
        //                                " }," +
        //                          " \"keyword2\": {" +
        //                                    " \"value\":\""+ keyword2 + "\"," +
        //                                    " \"color\":\"#173177\"" +
        //                                " }," +
        //                          " \"keyword3\": {" +
        //                                    " \"value\":\""+ keyword3 + "\"," +
        //                                    " \"color\":\"#173177\"" +
        //                                " }," +
        //                          " \"remark\": {" +
        //                                    " \"value\":\""+ remark + "\"," +
        //                                    " \"color\":\"#173177\"" +
        //                                " }" +
        //                          " } " +
        //                    " }";
        //SeatManage.SeatManageComm.WriteLog.Write(json);
        //string ret = new Com().HttpPost("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token="+ access_tocken + "", json);
        //SeatManage.SeatManageComm.WriteLog.Write(ret);
        #endregion


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}