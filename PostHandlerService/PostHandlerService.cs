using ScanQRCodeHandle.Model;
using SeatManage.SeatManageComm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PostHandlerService
{
    public class PostHandlerService : IService.IService
    {

        string ReqURL = "http://www.gxchuwei.com:90/MsgHandler.ashx";

        /// <summary>
        /// 构造方法
        /// </summary>
        public PostHandlerService()
        {
            this.PostProgress += PostHandlerService_PostProgress; ;

            timer = new System.Timers.Timer(2000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        /// <summary>
        /// 取出未处理列表
        /// </summary>
        /// <returns></returns>
        private List<QRScanLog> GetLogs()
        {
            string sql = "select * from QRScanLog where Flag='0'";
            List<QRScanLog> logs = new List<QRScanLog>();
            DataSet ds = DBUtility.DbHelperSQL.Query(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QRScanLog log = new QRScanLog();
                log.ID = int.Parse(dr["ID"].ToString());
                log.SchoolNo = dr["SchoolNo"].ToString();
                log.CardNo = dr["CardNo"].ToString();
                log.ScanTime = DateTime.Parse(dr["ScanTime"].ToString());
                log.DeviceNo = dr["DeviceNo"].ToString();
                log.Flag = dr["Flag"].ToString();
                logs.Add(log);
            }
            return logs;
        }

        /// <summary>
        /// 模拟Http Post请求
        /// </summary>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "POST";
            request.ContentLength = 0;
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }


        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer.Stop();
               // PostProgress("ok");
                List<QRScanLog> logs = GetLogs();
                foreach (QRScanLog item in logs)
                {
                    #region 获取用户信息
                    string sqlSelectUser = "select * from tb_User where CardNo = '" + item.CardNo + "'";
                    DataSet ds = DBUtility.DbHelperSQL.Query(sqlSelectUser); 
                    #endregion

                    string msg = "";
                    bool isTrue = AppWebService.BasicAPI.GetUserNowState(item.SchoolNo, item.CardNo, false, out msg);
                    SeatManage.SeatManageComm.WriteLog.Write("GetUserNowState msg:" + msg);
                    J_GetUserNowState userNowState = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                    string message = "";
                    string retMsg = "";

                    switch (userNowState.Status)
                    {
                        case "Seating":
                            string OPENID = ds.Tables[0].Rows[0]["OpenId"].ToString();
                            string first = ds.Tables[0].Rows[0]["NickName"].ToString() + "/" + ds.Tables[0].Rows[0]["Name"].ToString()+ ":\n 你正在使用的座位状态已经变更为【无座】状态";
                            string keyword1 = userNowState.InRoom;
                            string keyword2 = userNowState.SeatNum;
                            string keyword3 = DateTime.Now.ToString();
                            string remark = "感谢您的使用,欢迎下次光临，祝你学习进步";
                            bool b = AppWebService.BasicAPI.ReleaseSeat(item.SchoolNo, item.CardNo, out message);
                            string  postDataStr = "OPENID="+ OPENID + "&first="+ first + "&keyword1="+ keyword1 + "&keyword2="+ keyword2 + "&keyword3="+ keyword3 + "&remark="+ remark + "";
                            HttpPost(ReqURL, postDataStr);
                            retMsg = "释放座位";
                            break;
                        case "Leave": //没有座位，提醒去预约
                            string OPENID1 = ds.Tables[0].Rows[0]["OpenId"].ToString();
                            string first1 = ds.Tables[0].Rows[0]["NickName"].ToString() + "/" + ds.Tables[0].Rows[0]["Name"].ToString() + ":\n 您还没有座位，请先预约";
                            string keyword11 = "无"; ;
                            string keyword21 = "无";
                            string keyword31 = DateTime.Now.ToString();
                            string remark1 = "您还没有预约座位，请先预约座位";
                           // bool b = AppWebService.BasicAPI.ReleaseSeat(item.SchoolNo, item.CardNo, out message);
                            string postDataStr1 = "OPENID=" + OPENID1 + "&first=" + first1 + "&keyword1=" + keyword11 + "&keyword2=" + keyword21 + "&keyword3=" + keyword31 + "&remark=" + remark1 + "";
                            HttpPost(ReqURL, postDataStr1);
                            retMsg = "离开状态，请先预约座位";
                            break;
                        case "Booking":

                            string OPENID2 = ds.Tables[0].Rows[0]["OpenId"].ToString();
                            string first2 = ds.Tables[0].Rows[0]["NickName"].ToString() + "/" + ds.Tables[0].Rows[0]["Name"].ToString() + ":\n 你正在使用的座位状态已经变更为【在座】状态";
                            string keyword12 = userNowState.InRoom;
                            string keyword22 = userNowState.SeatNum;
                            string keyword32 = DateTime.Now.ToString();
                            string remark2 = "签到成功，请遵守相关规定，祝你学习愉快";
                            AppWebService.BasicAPI.CheckSeat(item.SchoolNo, item.CardNo, out message);
                            string postDataStr2 = "OPENID=" + OPENID2 + "&first=" + first2 + "&keyword1=" + keyword12 + "&keyword2=" + keyword22 + "&keyword3=" + keyword32 + "&remark=" + remark2 + "";
                            HttpPost(ReqURL, postDataStr2);

                           // AppWebService.BasicAPI.CheckSeat(item.SchoolNo, item.CardNo, out message);
                            retMsg = "预约签到";
                            break;
                        case "Waiting":
                            break;
                        case "ShortLeave":

                            string OPENID3 = ds.Tables[0].Rows[0]["OpenId"].ToString();
                            string first3 = ds.Tables[0].Rows[0]["NickName"].ToString() + "/" + ds.Tables[0].Rows[0]["Name"].ToString() + ":\n 你正在使用的座位状态已经变更为【在座】状态";
                            string keyword13 = userNowState.InRoom;
                            string keyword23 = userNowState.SeatNum;
                            string keyword33 = DateTime.Now.ToString();
                            string remark3 = "暂离回来，请遵守相关规定，祝你学习愉快";
                            AppWebService.BasicAPI.ComeBack(item.SchoolNo, item.CardNo, out message);
                            string postDataStr3 = "OPENID=" + OPENID3 + "&first=" + first3 + "&keyword1=" + keyword13 + "&keyword2=" + keyword23 + "&keyword3=" + keyword33 + "&remark=" + remark3 + "";
                            HttpPost(ReqURL, postDataStr3);
                            retMsg = "暂离回来";
                            break;
                        default:
                            break;
                    }
                    //更新处理标志
                    string sql = "update  QRScanLog set Flag = '1' where ID = "+item.ID+"";
                    DBUtility.DbHelperSQL.ExecuteSql(sql);
                    PostProgress(DateTime.Now.ToString()+":处理了学校编号【"+item.SchoolNo+"】读者学号【"+item.CardNo+"】姓名【"+userNowState.Name+"】的【"+retMsg+"】操作");
                }
            }
            catch (Exception ex)
            {
                SeatManage.SeatManageComm.WriteLog.Write(string.Format("执行遇到异常：{0}", ex.Message));
                PostProgress(string.Format("执行遇到异常：{0}", ex.Message));
            }
            finally
            {
                timer.Start();
            }
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <param name="message"></param>
        private void PostHandlerService_PostProgress(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <param name="message"></param>
        public delegate void EventHandleSync(string message);

        /// <summary>
        /// 处理事件
        /// </summary>
        public event EventHandleSync PostProgress;

        private System.Timers.Timer timer = null;

        /// <summary>
        /// 启动时更新原来未处理记录
        /// </summary>
        /// <returns></returns>
        public int  DeleteOldData()
        {
            string sql = "update  QRScanLog set Flag = '1' where Flag = '0'";
            int count = DBUtility.DbHelperSQL.ExecuteSql(sql);
            return count;
        }

        #region 接口方法
        public void Dispose()
        {
            timer.Stop();
        }

        public void Start()
        {
            //启动之前先处理之前遗留记录
            int count = DeleteOldData();
            PostProgress("启动，遗留记录已经处理"+count+"条");
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        } 
        #endregion
    }
}
