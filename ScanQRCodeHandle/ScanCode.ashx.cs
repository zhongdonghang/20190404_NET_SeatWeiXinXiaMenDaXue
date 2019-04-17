
using ScanQRCodeHandle.Model;
using SeatManage.SeatManageComm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace ScanQRCodeHandle
{
    /// <summary>
    /// Summary description for ScanCode
    /// </summary>
    public class ScanCode : IHttpHandler
    {
        public string url { get; set; }
        public ScanCode()
        {
            url = ConfigurationManager.AppSettings["WeCharPushUrl"];
        }

        private QRScanLog GetQRScanLogByRequest(string vgdecoderesult)
        {
            //schoolNo=20180423&cardNo=200001
            string decryptStr = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(vgdecoderesult);
            SeatManage.SeatManageComm.WriteLog.Write(decryptStr);
            string[] reqParms = decryptStr.Split('&');
           
            string[] schoolNokv = reqParms[0].Split('=');
            string schoolNo = schoolNokv[1].ToString();

            string[] reqCardNoParmKv = reqParms[1].Split('=');
            string cardno = reqCardNoParmKv[1];
            QRScanLog log = new QRScanLog();
            log.SchoolNo = schoolNo;
            log.CardNo = cardno;
            log.ScanTime = DateTime.Now;
            log.DeviceNo = "0";
            log.Flag = "0";

            return log;
        }

        /// <summary>
        /// 处理扫码请求（wifi版本）
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                if (HttpContext.Current.Request.Params["vgdecoderesult"] == null)
                {
                    throw new Exception("vgdecoderesult为空");
                }
                else
                {
                    string vgdecoderesult = HttpContext.Current.Request.Params["vgdecoderesult"].ToString();
                    SeatManage.SeatManageComm.WriteLog.Write("接收到:"+ vgdecoderesult);
                    vgdecoderesult = vgdecoderesult.Replace("\0", "");
                    vgdecoderesult = vgdecoderesult.Replace(" ", "");
                    vgdecoderesult = vgdecoderesult.Replace("\t", "");

                    QRScanLog log = GetQRScanLogByRequest(vgdecoderesult);

                    string msg = "";
                    bool isTrue = AppWebService.BasicAPI.GetUserNowState(log.SchoolNo, log.CardNo, false, out msg);
                    J_GetUserNowState userNowState = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                    string message = "";
                    // string retMsg = "";
                    /*
                     *                     预约成功消息
                     SchoolNum=20180605&StudentNo=20152102166&MsgType=UserOperation&Room=二楼C区（普通阅览区）&SeatNo=033&AddTime=2018-07-15 16:57:56&EndTime=&Days=VRType=&Msg=您在移动客户端预约2018/7/22 8:00:00 在二楼C区（普通阅览区） 033号座位，请在7:40至8:40之间到图书馆刷卡确认。
                     */
                    string postDataStr = "";
                    string aesStr = "";
                    string contenttype = "application/x-www-form-urlencoded";//更网站该方法支持的类型要一致
                    string para = "";
                    switch (userNowState.Status)
                    {
                        case "Seating":
                            bool b = AppWebService.BasicAPI.ReleaseSeat(log.SchoolNo, log.CardNo, out message);
                            if (b)
                            {
                                 postDataStr = "SchoolNum=" + log.SchoolNo + "&StudentNo=" + log.CardNo + "&MsgType=UserOperation&Room=" + userNowState.InRoom + "&SeatNo=" + userNowState.SeatNum + "&AddTime=" + DateTime.Now.ToString() + "&EndTime=&Days=VRType=&Msg=在移动端释放" + userNowState.InRoom + " [" + userNowState.SeatNum + "]号座位";
                                 aesStr = SeatManage.SeatManageComm.AESAlgorithm.AESEncrypt(postDataStr, "SeatManage_WeiCharCode");
                                 aesStr = aesStr.Replace("+", "%2B");
                                 para = "msg=" + aesStr;
                                 PostMsg(contenttype, url, para);
                            }
                            break;
                        case "Leave": //没有座位，提醒去预约
                            postDataStr = "SchoolNum=" + log.SchoolNo + "&StudentNo=" + log.CardNo + "&MsgType=UserOperation&Room=" + userNowState.InRoom + "&SeatNo=" + userNowState.SeatNum + "&AddTime=" + DateTime.Now.ToString() + "&EndTime=&Days=VRType=&Msg=您还没有座位，请先预约";
                            aesStr = SeatManage.SeatManageComm.AESAlgorithm.AESEncrypt(postDataStr, "SeatManage_WeiCharCode");
                            aesStr = aesStr.Replace("+", "%2B");
                            para = "msg=" + aesStr;
                            PostMsg(contenttype, url, para);
                            break;
                        case "Booking":

                            if (AppWebService.BasicAPI.CheckSeat(log.SchoolNo, log.CardNo, out message))
                            {
                                postDataStr = "SchoolNum=" + log.SchoolNo + "&StudentNo=" + log.CardNo + "&MsgType=UserOperation&Room=" + userNowState.InRoom + "&SeatNo=" + userNowState.SeatNum + "&AddTime=" + DateTime.Now.ToString() + "&EndTime=&Days=VRType=&Msg=在移动终端扫码，入座预约的"+userNowState.InRoom+" ["+userNowState.SeatNum+"]座位";
                                aesStr = SeatManage.SeatManageComm.AESAlgorithm.AESEncrypt(postDataStr, "SeatManage_WeiCharCode");
                                aesStr = aesStr.Replace("+", "%2B");
                                para = "msg=" + aesStr;
                                PostMsg(contenttype, url, para);
                            }
                            break;
                        case "Waiting":
                            break;
                        case "ShortLeave":
                            if (AppWebService.BasicAPI.ComeBack(log.SchoolNo, log.CardNo, out message))
                            {
                                postDataStr = "SchoolNum=" + log.SchoolNo + "&StudentNo=" + log.CardNo + "&MsgType=UserOperation&Room=" + userNowState.InRoom + "&SeatNo=" + userNowState.SeatNum + "&AddTime=" + DateTime.Now.ToString() + "&EndTime=&Days=VRType=&Msg=在移动终端扫码，暂离回来" + userNowState.InRoom + " [" + userNowState.SeatNum + "]座位";
                                aesStr = SeatManage.SeatManageComm.AESAlgorithm.AESEncrypt(postDataStr, "SeatManage_WeiCharCode");
                                aesStr = aesStr.Replace("+", "%2B");
                                para = "msg=" + aesStr;
                                PostMsg(contenttype, url, para);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                SeatManage.SeatManageComm.WriteLog.Write(ex.ToString());
            }
        }

        private string PostMsg(string contenttype,string service,string para)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(service);
            myRequest.Method = "POST";
            myRequest.ContentType = contenttype;
            myRequest.ContentLength = para.Length;
            Stream newStream = myRequest.GetRequestStream();
            // Send the data.
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] postdata = encoding.GetBytes(para);
            newStream.Write(postdata, 0, para.Length);
            newStream.Close();
            // Get response
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd(); //得到结果
            return content;
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