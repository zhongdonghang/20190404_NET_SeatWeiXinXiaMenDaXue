using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppWebService.AppWebServiceSoap;
using Common;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Model;
using SeatManage.SeatManageComm;

namespace AppWebService
{
    public static class BasicAPI
    {

        static MobileAppWebServiceSoapClient service = new MobileAppWebServiceSoapClient();

        public static MySoapHeader InitHeader(string SchoolNum)
        {
            service.Endpoint.Address = new System.ServiceModel.EndpointAddress(GetAppSettings.WebServiceURL);
            MySoapHeader soapHeader = new MySoapHeader();
            soapHeader.UserName = GetAppSettings.SoapUser;
            soapHeader.PassWord = GetAppSettings.SoapPwd;
            soapHeader.SchoolNum = SchoolNum;
            return soapHeader;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="studentNo">学员名称</param>
        /// <returns>是否调用成功</returns>
        public static bool GetUserInfo(string SchoolNum, string studentNo ,out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetUserInfo(soapHeader, studentNo));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 获取当前用户状态
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="studentNo">学员编号</param>
        /// <param name="isCheckCode">是否签到扫码</param>
        /// <param name="Msg">返回消息,如果成功返回用户状态</param>
        /// <returns>是否调用成功</returns>
        public static bool GetUserNowState(string SchoolNum, string studentNo, bool isCheckCode, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetUserNowState(soapHeader, studentNo, isCheckCode));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 获取阅览室使用状态
        /// </summary>
        /// <param name="SchoolNum"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool GetLibraryNowState(string SchoolNum,out string Msg)
        {
            MySoapHeader sopaHeadder = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetLibraryNowState(sopaHeadder));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 获取进出记录
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="studentNo">读者学号</param>
        /// <param name="pageIndex">页码（从0开始为第一页）</param>
        /// <param name="pageSize">数据数目，每页显示多少数据</param>
        /// <param name="Msg">返回消息，如果成功返回进出记录</param>
        /// <returns>是否调用成功</returns>
        public static bool GetEnterOutLog(string SchoolNum, string studentNo, int pageIndex, int pageSize, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetEnterOutLog(soapHeader, studentNo, pageIndex, pageSize));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }
        
        /// <summary>
        /// 获取预约记录
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="studentNo">读者学号</param>
        /// <param name="pageIndex">页码（从0开始为第一页）</param>
        /// <param name="pageSize">数据数目，每页显示多少数据</param>
        /// <param name="Msg">返回消息，如果成功返回进出记录</param>
        /// <returns>是否调用成功</returns>
        public static bool GetBesapsekLog(string SchoolNum, string studentNo, int pageIndex, int pageSize, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetBesapsekLog(soapHeader, studentNo, pageIndex, pageSize));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 获取违规记录
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="studentNo">读者学号</param>
        /// <param name="pageIndex">页码（从0开始为第一页）</param>
        /// <param name="pageSize">数据数目，每页显示多少数据</param>
        /// <param name="Msg">返回消息，如果成功返回进出记录</param>
        /// <returns>是否调用成功</returns>
        public static bool GetViolationLog(string SchoolNum, string studentNo, int pageIndex, int pageSize, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetViolationLog(soapHeader, studentNo, pageIndex, pageSize));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }


        /// <summary>
        /// 获取黑名单记录
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="studentNo">读者学号</param>
        /// <param name="pageIndex">页码（从0开始为第一页）</param>
        /// <param name="pageSize">数据数目，每页显示多少数据</param>
        /// <param name="Msg">返回消息，如果成功返回进出记录</param>
        /// <returns>是否调用成功</returns>
        public static bool GetBlacklist(string SchoolNum, string studentNo, int pageIndex, int pageSize, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetBlacklist(soapHeader, studentNo, pageIndex, pageSize));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }


        /// <summary>
        /// 释放座位
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="studentNo">学员编号</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool ReleaseSeat(string SchoolNum, string studentNo, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.ReleaseSeat(soapHeader, studentNo));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 座位签到
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="studentNo">学员编号</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool CheckSeat(string SchoolNum, string studentNo, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.CheckSeat(soapHeader, studentNo));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 暂离回来
        /// </summary>
        /// <param name="SchoolNum"></param>
        /// <param name="studentNo"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool ComeBack(string SchoolNum, string studentNo, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.ComeBack(soapHeader, studentNo));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }


        /// <summary>
        /// 取消当日预约记录
        /// </summary>
        /// <param name="SchoolNum"></param>
        /// <param name="studentNo"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool CancelBesapeakByCardNo(string SchoolNum, string studentNo, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.CancelBesapeakByCardNo(soapHeader, studentNo,DateTime.Now.ToString("yyyy-MM-dd")));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

       /// <summary>
       /// 座位暂离
       /// </summary>
       /// <param name="SchoolNum"></param>
       /// <param name="studentNo"></param>
       /// <param name="Msg"></param>
       /// <returns></returns>
        public static bool ShortLeave(string SchoolNum, string studentNo, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.ShortLeave(soapHeader, studentNo));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 取消等待
        /// </summary>
        /// <param name="SchoolNum"></param>
        /// <param name="studentNo"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool CancelWait(string SchoolNum, string studentNo, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.CancelWait(soapHeader, studentNo));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 根据预约编号取消订单
        /// </summary>
        /// <param name="SchoolNum"></param>
        /// <param name="bespeakId"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool CancelBesapeak(string SchoolNum, int bespeakId, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.CancelBesapeak(soapHeader, bespeakId));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }


        /// <summary>
        /// 获取可预约阅览室
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="date">日期</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool GetCanBespeakRoomInfo(string SchoolNum, string date, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetCanBespeakRoomInfo(soapHeader, date));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }


        /// <summary>
        /// 获取阅览室可预约座位
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="roomNo">阅览室编号</param>
        /// <param name="bespeakTime">预约时间</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool GetRoomBesapeakState(string SchoolNum, string roomNo,string bespeakTime, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetRoomBesapeakState(soapHeader, roomNo, bespeakTime));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 获取座位信息
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="seatNo">阅览室编号</param>
        /// <param name="roomNo">座位编号</param>
        /// <param name="studentNo">学员学号</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool GetSeatNowStatus(string SchoolNum, string seatNo, string roomNo, string studentNo, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetSeatNowStatus(soapHeader,seatNo, roomNo, studentNo));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 更换座位
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="seatNo">座位号</param>
        /// <param name="roomNo">阅览室</param>
        /// <param name="studentNo">学员编号</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool ChangeSeat(string SchoolNum, string seatNo, string roomNo, string studentNo, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.ChangeSeat(soapHeader, seatNo, roomNo, studentNo));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        /// <summary>
        /// 预约提交
        /// </summary>
        /// <param name="SchoolNum">学校编号</param>
        /// <param name="seatNo">完整座位编号</param>
        /// <param name="roomNo">阅览室编号</param>
        /// <param name="studentNo">学员编号</param>
        /// <param name="besapeakTime">预约日期</param>
        /// <param name="isNowBesapeak">是否及时预约</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool SubmitBesapeskSeat(string SchoolNum, string seatNo, string roomNo, string studentNo, string besapeakTime, bool isNowBesapeak, out string Msg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.SubmitBesapeskSeat(soapHeader, seatNo, roomNo, studentNo, besapeakTime, isNowBesapeak));
            Msg = ja["Msg"].ToString();
            return Convert.ToBoolean(ja["Result"].ToString());
        }

        #region 20180312 add
        /// <summary>
        /// 选择位置
        /// </summary>
        /// <param name="SchoolNum"></param>
        /// <param name="studentNo"></param>
        /// <param name="seatNo"></param>
        /// <param name="roomNo"></param>
        /// <returns></returns>
        public static string SelectSeat(string SchoolNum,string studentNo, string seatNo, string roomNo)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.SelectSeat(soapHeader, studentNo, seatNo, roomNo));
           // Msg = ja["Msg"].ToString();
            return ja["Result"].ToString();
        }

        /// <summary>
        /// 续时
        /// </summary>
        /// <param name="SchoolNum"></param>
        /// <param name="studentNo"></param>
        /// <returns></returns>
        public static string DelayTime(string SchoolNum, string studentNo)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.DelayTime(soapHeader,studentNo));
            // Msg = ja["Msg"].ToString();
            return ja["Result"].ToString();
        }


        /// <summary>
        /// 获取座位预约信息
        /// </summary>
        /// <param name="SchoolNum"></param>
        /// <param name="seatNo"></param>
        /// <param name="roomNo"></param>
        /// <param name="bespeakTime"></param>
        /// <returns></returns>
        public static string GetSeatBespeakInfo(string SchoolNum, string seatNo, string roomNo, string bespeakTime,out string outMsg)
        {
            MySoapHeader soapHeader = InitHeader(SchoolNum);
            JObject ja = (JObject)JsonConvert.DeserializeObject(service.GetSeatBespeakInfo(soapHeader,seatNo,roomNo,bespeakTime));
            outMsg = ja["Msg"].ToString();
            return ja["Result"].ToString();
        }

        #endregion

    }
}
