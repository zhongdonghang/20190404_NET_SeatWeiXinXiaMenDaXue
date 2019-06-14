﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WeiXinMsgService;
using WeiXinMsgService.OutMsg;
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
                try
                {
                    if (context.Request.Params["msg"] != null)//座位系统自身消息
                    {
                        string msg = AESAlgorithm.AESDecrypt(context.Request.Params["msg"]);
                        string[] kv = msg.Split('&');
                        string SchoolNum = kv[0].Split('=')[1];
                        string StudentNo = kv[1].Split('=')[1];
                        string MsgType = kv[2].Split('=')[1];
                        string Room = kv[3].Split('=')[1];
                        string SeatNo = kv[4].Split('=')[1];
                        string AddTime = kv[5].Split('=')[1];
                        string EndTime = kv[6].Split('=')[1];
                        string Msg = kv[8].Split('=')[1];
                        string OPENID = SqlTools.GetOpenId(StudentNo);
                        string first = "您的座位状态发生改变";//context.Request.Params["first"].ToString();
                        string keyword1 = Room;//context.Request.Params["keyword1"].ToString();
                        string keyword2 = SeatNo;//context.Request.Params["keyword2"].ToString();
                        string keyword3 = AddTime;//context.Request.Params["keyword3"].ToString();
                        string remark = Msg;//context.Request.Params["remark"].ToString();
                        SeatManage.SeatManageComm.WriteLog.Write("OPENID:" + OPENID);
                        ResponseMsg(OPENID, first, keyword1, keyword2, keyword3, remark);
                    }
                    else if (context.Request.Params["IsOutMsg"] != null)//座位系统系统外部消息
                    {
                        string msgType = context.Request.Params["MsgType"].ToString();//消息类型
                        string msgReturnURL = context.Request.Params["MsgReturnURL"].ToString();//点击消息跳转url
                        string toReader = context.Request.Params["ToReader"].ToString();//发送到的读者学工号，多个读者用逗号隔开
                        switch (msgType)
                        {
                            case "OverdueNotice"://图书馆超期催还通知
                                string first = context.Request.Params["Content"].Trim();
                                string keyword1 = context.Request.Params["StudentName"].Trim();
                                string keyword2 = context.Request.Params["BookName"].Trim();
                                string keyword3 = context.Request.Params["BarCode"].Trim();
                                string keyword4 = context.Request.Params["GiveBackDate"].Trim();
                                string keyword5 = context.Request.Params["OverdueDates"].Trim();
                                string remark = context.Request.Params["Remark"].Trim();

                                ResponseOverdueNoticeMsg(toReader, first, keyword1, keyword2, keyword3, keyword4, keyword5, remark);
                                break;
                            case "GiveBackBookNotice"://还书通知
                                break;
                            case "BooksToLibraryNotice"://委托图书到馆通知
                                break;
                            case "GiveBackBookSucceedNotice"://成功还书通知
                                break;
                            case "ActivityToBeginningNotice"://活动即将开始提醒
                                break;
                            case "BorrowBooksSucceedNotice"://图书馆借书成功通知
                                break;
                            default:
                                SeatManage.SeatManageComm.WriteLog.Write("未知消息类型:"+ msgType + "，请检查请求参数");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    SeatManage.SeatManageComm.WriteLog.Write("5:Exception ex"+ex);
                   // throw;
                }
            }
            else if (this.context.Request.HttpMethod == "GET")//如果是Get请求，则是接入验证，返回随机字符串。
            {
                signature = context.Request.Params["signature"];
                timestamp = context.Request.Params["timestamp"];
                nonce = context.Request.Params["nonce"];
                echostr = context.Request.Params["echostr"];
                
                if (!service.CheckSignature(signature, timestamp, nonce))//验证请求是否微信发过来的。
                {
                    this.context.Response.End();
                }
                context.Response.Write(echostr);
            }
        }


        #region 发送外部消息
        /// <summary>
        /// 图书馆超期催还通知
        /// </summary>
        private void ResponseOverdueNoticeMsg(string toReaders,string first,string keyword1,string keyword2,string keyword3,string keyword4,string keyword5,string remark)
        {
            string tempID = ConfigurationManager.AppSettings["OverdueNoticeID"];//模板ID
            string access_tocken = Com.IsExistAccess_Token(ConfigurationManager.AppSettings["AppID"], ConfigurationManager.AppSettings["AppSecret"]);
            SendTools tools = new SendTools();
            OverdueNoticeTempData data = new OverdueNoticeTempData();
            data.first = new TempItem(first);
            data.keyword1 = new TempItem(keyword1);
            data.keyword2 = new TempItem(keyword2);
            data.keyword3 = new TempItem(keyword3);
            data.keyword4 = new TempItem(keyword4);
            data.keyword5 = new TempItem(keyword5);
            data.remark = new TempItem(remark);
            TempModel model = new TempModel();
            model.objOverdueNoticeTempData = data;
            model.template_id = tempID;
            model.url = ConfigurationManager.AppSettings["OverdueNotice_URL"];
            model.topcolor = "#FF0000";

            //处理学号toReaders，逗号隔开
            string[] readers = toReaders.Split(',');
            foreach (var item in readers)
            {
                string OPENID = SqlTools.GetOpenId(item);
                if (OPENID != "")
                {
                    model.touser = OPENID;
                    OpenApiResult result = tools.SendTempMessage(access_tocken, model);
                }
            }
        }
        /// <summary>
        /// 还书通知
        /// </summary>
        private void ResponseGiveBackBookNoticeMsg(string toReaders)
        {

        }
        /// <summary>
        /// 委托图书到馆通知
        /// </summary>
        private void ResponseBooksToLibraryNoticeMsg(string toReaders)
        {

        }
        /// <summary>
        /// 成功还书通知
        /// </summary>
        private void ResponseGiveBackBookSucceedNoticeMsg(string toReaders)
        {

        }
        /// <summary>
        /// 活动即将开始提醒
        /// </summary>
        private void ResponseActivityToBeginningNoticeMsg(string toReaders)
        {

        }
        /// <summary>
        /// 图书馆借书成功通知
        /// </summary>
        private void ResponseBorrowBooksSucceedNoticeMsg(string toReaders)
        {

        }
        #endregion

        //发送内部消息
        private void ResponseMsg(string OPENID,string first,string keyword1,string keyword2,string keyword3,string remark)
        {
            try
            {
              //  SeatManage.SeatManageComm.WriteLog.Write("4:ResponseMsg");
                string tempID = "oLdL1fEDKZsPdc9njn7V2yioHhFWpQQy1JyfxKV_NsM";//座位状态变更通知模板ID
                string access_tocken = Com.IsExistAccess_Token("wx5c27898c83a612dc", "51fed2e73dddd6e6eabbb528c973074d");

                SendTools tools = new SendTools();
                TemplateModel m = new TemplateModel(first, keyword1, keyword2, keyword3, remark);
                m.touser = OPENID;
                m.template_id = tempID;
                m.url = "https://lib.xmu.edu.cn/seatwx/NewUser/MySeat";
                m.topcolor = "#FF0000";
                OpenApiResult result = tools.SendTemplateMessage(access_tocken, m);
               // SeatManage.SeatManageComm.WriteLog.Write("msg_id" + result.msg_id + "error_code" + result.error_code + "error_msg" + result.error_msg);

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