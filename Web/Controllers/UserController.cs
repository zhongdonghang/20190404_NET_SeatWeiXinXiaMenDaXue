using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcExtension;
using Model;
using SeatManage.SeatManageComm;
using Common;
using System.Collections.Specialized;
using Dos.ORM;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.CommonAPIs;
using WeChatJsSdk.SdkCore;
using AppWebService;
using System.Drawing;
using System.IO;

namespace Web.Controllers
{
    public class UserController : Controller
    {

        #region 扫码选座
        public ActionResult Demo()
        {
            return View();
        }


        public ActionResult ScanMyQRCodeResult()
        {
            try
            {
                if (Request.Params["vgdecoderesult"] == null)
                {
                    SeatManage.SeatManageComm.WriteLog.Write("vgdecoderesult为空");
                }
                else
                {
                    //schoolNo=20180423&cardNo=200001
                    string vgdecoderesult = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(Request.Params["vgdecoderesult"].ToString());
                    string[] ar1 = vgdecoderesult.Split('&');
                    string[] ar2 = ar1[0].Split('=');
                    string schoolNo = ar2[1].ToString();

                    string[] ar3 = ar1[1].Split('=');
                    string cardno = ar3[1];
                    QRScanLog log = new QRScanLog();
                    log.SchoolNo = schoolNo;
                    log.CardNo = cardno;
                    log.ScanTime = DateTime.Now;
                    log.DeviceNo = "0";
                    log.Flag = "0";
                    SeatManage.SeatManageComm.WriteLog.Write(log.SchoolNo);
                    DbSession.Default.Update<QRScanLog>(log);
                }
            }
            catch (Exception ex)
            {
                SeatManage.SeatManageComm.WriteLog.Write(ex.ToString());
            }

            //扫码成功，在这里推送消息
            return Index();
        }

        [OAuthBaseFilter]
        public ActionResult ShowMyQRCode()
        {
            tb_User user = Session["User"] as tb_User;
            try
            {
                string path = Server.MapPath("~/QRCodeImages/" +user.SchoolNo+"_"+ user.CardNo + ".jpg");
                if (!System.IO.File.Exists(path))
                {
                    string schoolNo = user.SchoolNo;
                    string cardno = user.CardNo;
                    string AESCode = string.Format("schoolNo={0}&cardNo={1}", schoolNo, cardno);
                    
                    AESCode = AESAlgorithm.AESEncrypt(AESCode, "SeatManage_WeiCharCode");
                    AESCode = AESCode.Replace("+", "%2B");
                    Bitmap bitmap = QRCode.GetDimensionalCode(AESCode, 6, 8);
                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmap.Dispose();
                }
                ViewBag.QRCodeImage = "/QRCodeImages/" + user.SchoolNo + "_" + user.CardNo + ".jpg";
            }
            catch (Exception ex)
            {
                SeatManage.SeatManageComm.WriteLog.Write(ex.ToString());
            }
            return View();
        }
        


        //扫码
        [OAuthBaseFilter]
        public ActionResult SelectSeat()
        {
            //获取当前用户的座位状态  GetUserNowState
            tb_User u = (Session["User"] as tb_User);
            string outMsg = "";
            bool isTrue = AppWebService.BasicAPI.GetUserNowState(u.SchoolNo, u.CardNo, false, out outMsg);
            J_GetUserNowState objJ_GetUserNowState = new J_GetUserNowState();
            JObject obj = (JObject)JsonConvert.DeserializeObject(outMsg);
            objJ_GetUserNowState.StudentNum = obj["StudentNum"].ToString();
            // objJ_GetUserNowState.SeatNo = obj["Name"].ToString();
            objJ_GetUserNowState.InRoom = obj["InRoom"].ToString();
            objJ_GetUserNowState.SeatNum = obj["SeatNum"].ToString();
            objJ_GetUserNowState.Status = obj["Status"].ToString();
            objJ_GetUserNowState.CanOperation = obj["CanOperation"].ToString();
            objJ_GetUserNowState.NowStatusRemark = obj["NowStatusRemark"].ToString();
            objJ_GetUserNowState.Time = obj["Time"].ToString();
            ViewBag.UserNowState = objJ_GetUserNowState.Status;
            ViewBag.UserInRoom = objJ_GetUserNowState.InRoom;
            ViewBag.SeatNum = objJ_GetUserNowState.SeatNum;
            ViewBag.OpTime = objJ_GetUserNowState.Time;
            ViewBag.NowStatusRemark = objJ_GetUserNowState.NowStatusRemark;
            ViewBag.StatusStr = objJ_GetUserNowState.StatusStr;
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(GetAppSettings.AppID, GetAppSettings.AppSecret, "http://wechat.gxchuwei.com/User/SelectSeat");
            return View(jssdkUiPackage);
        }

        [HttpPost]
        public JsonResult ChangeSeat(string seatNo,string roomNo)
        {
            string message = "";
            string result = "";
            try
            {
                tb_User user = Session["User"] as tb_User;
                bool isTrue = AppWebService.BasicAPI.ChangeSeat(user.SchoolNo, seatNo, roomNo, user.StudentNo, out message);
                if (isTrue)
                {
                    result = "yes";
                   // message = "暂离回来成功";
                }
                else
                {
                    result = "no";
                    //message = "暂离回来失败，请重试";
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                result = "no";
                throw;
            }
            return Json(new { status = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ComeBack(string SchoolNo, string StudentNo)
        {
            string message = "";
            string result = "";
            try
            {
                bool isTrue = AppWebService.BasicAPI.ComeBack(SchoolNo, StudentNo,out message);
                if (isTrue)
                {
                    result = "yes";
                    message = "暂离回来成功";
                }
                else
                {
                    result = "no";
                    message = "暂离回来失败，请重试";
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                result = "no";
                throw;
            }
            return Json(new { status = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult XuShiForSeatingAndSeating(string SchoolNo, string StudentNo)
        {
            string message = "";
            string result = "";
            try
            {
                result = AppWebService.BasicAPI.DelayTime(SchoolNo, StudentNo);
                if (message.ToLower() == "true")
                {
                    result = "yes";
                    message = "续时成功";
                }
                else
                {
                    result = "no";
                    message = "续时失败";
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                result = "no";
                throw;
            }
            return Json(new { status = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 选择位置，人是离开状态的，座位也是离开状态的
        /// </summary>
        /// <param name="SchoolNo">学校编号</param>
        /// <param name="StudentNo">学号</param>
        /// <param name="SeatNo">座位号</param>
        /// <param name="ReadingRoomNo">阅览室编号</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SelectSeatForLeaveAndLeave(string SchoolNo,string  StudentNo, string SeatNo,string ReadingRoomNo)
        {
            string message = "";
            string result = "";
            try
            {
                WriteLog.Write(""+ SchoolNo+","+ StudentNo + "," + SeatNo + "," + ReadingRoomNo + "");
                message = AppWebService.BasicAPI.SelectSeat(SchoolNo, StudentNo, SeatNo, ReadingRoomNo);
                WriteLog.Write(message);
                if (message.ToLower() == "true")
                {
                    result = "yes";
                    message = "你成功选择了阅览室" + ReadingRoomNo + "的" + SeatNo + "号位置";
                }
                else
                {
                    result = "no";
                    message = "该位置不可用，或者你的状态不是【离开】状态";
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                result = "no";
                throw;
            }
            return Json(new {status = result, message = message}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 扫码结果响应
        /// </summary>
        /// <param name="ScanResult"></param>
        /// <param name="UserNowState"></param>
        /// <param name="UserInRoom"></param>
        /// <param name="SeatNum"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ScanSelectSeat(string ScanResult, string UserNowState, string UserInRoom, string SeatNum)
        {
            string message = "";
            string OperateCode = "-1";
            string bindResult = "no";
            string tmp = "";
            string returnHtml = "";
            try
            {
                string str = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(ScanResult);
                if (str == "")
                {
                    message = "二维码无效，请选【微信绑定】功能后刷卡或输入编号获取您的绑定二维码！";
                }
                else
                {
                    //扫码结果 schoolNo=20180301&readingRoomNo=101002&seatNo=101002060
                    NameValueCollection col = Common.UrlCommon.GetQueryString(str);
                    tb_User user = Session["User"] as tb_User;


                    //取出位置状态
                    string outMsgGetSeatNowStatus = "";
                    bool isTrueGetSeatNowStatus = BasicAPI.GetSeatNowStatus(user.SchoolNo, col["seatNo"], col["readingRoomNo"], user.CardNo, out outMsgGetSeatNowStatus);
                    if (isTrueGetSeatNowStatus)
                    {
                        J_GetSeatNowStatus objJ_GetSeatNowStatus = new J_GetSeatNowStatus();
                        JObject obj = (JObject)JsonConvert.DeserializeObject(outMsgGetSeatNowStatus);
                        objJ_GetSeatNowStatus.SeatNo = obj["SeatNo"].ToString();
                        objJ_GetSeatNowStatus.SeatShortNo = obj["SeatShortNo"].ToString();
                        objJ_GetSeatNowStatus.RoomNo = obj["RoomNo"].ToString();
                        objJ_GetSeatNowStatus.RoomName = obj["RoomName"].ToString();
                        objJ_GetSeatNowStatus.Status = obj["Status"].ToString();
                        objJ_GetSeatNowStatus.CanOperation = obj["CanOperation"].ToString();
                        objJ_GetSeatNowStatus.CanBookingDate = new List<string>();
                        // objJ_GetSeatNowStatus.CanBookingDate.Add(obj["CanBookingDate"].ToString());数组

                        string ReadingRoomNo = col["readingRoomNo"];
                        string tmpSeatNum = col["seatNo"];

                      //  WriteLog.Write(UserNowState+"$$$"+ objJ_GetSeatNowStatus.Status);

                        if (UserNowState == "Seating") //重新选座,暂离，续时，释放座位
                        {
                            if (objJ_GetSeatNowStatus.Status == "Seating")
                            {
                                if (UserInRoom == objJ_GetSeatNowStatus.RoomName && SeatNum == objJ_GetSeatNowStatus.SeatShortNo)
                                {
                                    //返回暂离，续时，释放座位操作按钮
                                    OperateCode = "1";
                                    message = "返回暂离，续时，释放座位操作按钮";
                                    tmp = "Seating/Seating";
                                    returnHtml = "<div class=\"ui-dialog show\">" +
                                               "<div class=\"ui-dialog-cnt\">" +
                                                " <div class=\"ui-dialog-bd\">" +
                                                        " <h3>操作确认</h3>" +
                                                         "<p>您的位置是【" + ReadingRoomNo + "】的【" + tmpSeatNum + "】号座位，维持良好学习环境，请勿恶意操作</p>" +
                                                  "</div>" +
                                                    "<div class=\"ui-dialog-ft\">" +
                                                        "<button id=\"btnResult1XuShi\" onclick=\"btnResult1XuShiClick('"+user.SchoolNo+"','"+user.StudentNo+"');\"  type = \"button\" data-role=\"button\" class=\"btn-recommand\">续时</button>" +
                                                        "<button id=\"btnResult1ZanLi\" onclick=\"btnResult1ZanLiClick()\"  type = \"button\" data-role=\"button\" class=\"btn-recommand\">暂离</button>" +
                                                        "<button id=\"btnResult1ShiFang\" onclick=\"btnResult1ShiFang()\"  type = \"button\" data-role=\"button\" class=\"btn-recommand\">释放</button>" +
                                                    "</div>" +
                                                 "</div>" +
                                                "</div>";
                                }
                                else
                                {
                                    OperateCode = "-9";
                                    message = "您是【在座】状态，你的座位是【" + UserInRoom + "】的【" + SeatNum + "】位置，您现在扫码的位置是【" + objJ_GetSeatNowStatus.RoomName + "】的【" + objJ_GetSeatNowStatus.SeatShortNo + "】位置，并且这个位置已经有人在座，请回到原来位置，或者重新选择.";
                                }
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Leave")
                            {
                                //返回重新选座操作
                                OperateCode = "2";
                                message = "重新选座操作";
                                tmp = "Seating/Leave";
                                returnHtml = "<div class=\"ui-dialog show\">" +
                                           "<div class=\"ui-dialog-cnt\">" +
                                            " <div class=\"ui-dialog-bd\">" +
                                                    " <h3>操作确认</h3>" +
                                                     "<p>您重新选择位置是【" + ReadingRoomNo + "】的【" + tmpSeatNum + "】号座位，维持良好学习环境，请勿恶意操作</p>" +
                                              "</div>" +
                                                "<div class=\"ui-dialog-ft\">" +
                                                    "<button id=\"btnResult2CancelReSelect\" onclick=\"btnResult2CancelReSelectClick()\"  type = \"button\" data-role=\"button\" >取消</button>" +
                                                    "<button id=\"btnResult2ReSelect\" onclick=\"btnResult2ReSelectClick('"+ tmpSeatNum + "','"+ ReadingRoomNo + "')\"  type = \"button\" data-role=\"button\" class=\"btn-recommand\">重新选择</button>" +
                                                "</div>" +
                                             "</div>" +
                                            "</div>";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Booking")
                            {
                                //该位置已经有预约，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有预约，不能操作";
                                tmp = "Seating/Booking";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Waiting")
                            {
                                //该位置已经有等待，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有等待，不能操作";
                                tmp = "Seating/Waiting";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "ShortLeave")
                            {
                                //该位置已经有暂离，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有暂离，不能操作";
                                tmp = "Seating/ShortLeave";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "StopUsed")
                            {
                                //该位置已经有停用，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有停用，不能操作";
                                tmp = "Seating/StopUsed";
                            }
                            else
                            {
                                //座位异常，不能操作
                                OperateCode = "-9";
                                message = "座位异常，不能操作";
                                tmp = "Seating/ex";
                            }
                        }
                        else if (UserNowState == "Leave")//选座
                        {
                            if (objJ_GetSeatNowStatus.Status == "Seating")
                            {
                                //该位置已经有人，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有人，不能操作";
                                tmp = "Leave/Seating";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Leave")
                            {
                                //获取此位置的预约信息
                                string outMsg = "";
                                string BespeakInfo =   BasicAPI.GetSeatBespeakInfo(user.SchoolNo, tmpSeatNum, ReadingRoomNo, DateTime.Now.ToString(),out outMsg);
                                //WriteLog.Write(user.SchoolNo + "--" + tmpSeatNum + "--" + ReadingRoomNo + "" + DateTime.Now.ToString());
                                //WriteLog.Write("BespeakInfo--"+ BespeakInfo+"outMsg--"+outMsg);
                                if (BespeakInfo == "False")
                                {
                                    OperateCode = "-9";
                                    message = outMsg;
                                    tmp = "Leave/Seating";
                                }
                                else
                                {
                                    //位置是空闲，可以入座
                                    OperateCode = "3";
                                    message = "位置是空闲，可以入座";
                                    tmp = "Leave/Leave";
                                    returnHtml = "<div class=\"ui-dialog show\">" +
                                                   "<div class=\"ui-dialog-cnt\">" +
                                                    " <div class=\"ui-dialog-bd\">" +
                                                            " <h3>入座确认</h3>" +
                                                             "<p>您确定选择【" + ReadingRoomNo + "】的【" + tmpSeatNum + "】号座位？维持良好学习环境，请勿恶意操作</p>" +
                                                      "</div>" +
                                                        "<div class=\"ui-dialog-ft\">" +
                                                            "<button id=\"btnResult3Cancel\" onclick=\"btnResult3CancelClick();\" type = \"button\" data-role=\"button\">取消</button>" +
                                                            "<button id=\"btnResult3Ok\" onclick=\"btnResult3OkClick('" + user.SchoolNo + "','" + user.StudentNo + "','" + tmpSeatNum + "','" + ReadingRoomNo + "');\" type = \"button\" data-role=\"button\" class=\"btn-recommand\">确认</button>" +
                                                        "</div>" +
                                                     "</div>" +
                                                    "</div>";
                                }

                            }
                            else if (objJ_GetSeatNowStatus.Status == "Booking")
                            {
                                //该位置已经有预约，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有预约，不能操作";
                                tmp = "Leave/Booking";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Waiting")
                            {
                                //该位置已经有等待，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有等待，不能操作";
                                tmp = "Leave/Waiting";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "ShortLeave")
                            {
                                //该位置已经有暂离，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有暂离，不能操作";
                                tmp = "Leave/ShortLeave";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "StopUsed")
                            {
                                //该位置已经有停用，不能操作
                                OperateCode = "-9";
                                message = "该位置已经有停用，不能操作";
                                tmp = "Leave/StopUsed";
                            }
                            else
                            {
                                //座位异常，不能操作
                                OperateCode = "-9";
                                message = "座位异常，不能操作";
                                tmp = "Leave/ex";
                            }
                        }
                        else if (UserNowState == "Booking")//预约确认
                        {
                            //OperateCode = "4";
                            //message = "判断是否是可以预约签到";
                            //tmp = "Booking/Booking";

                            if (UserInRoom == objJ_GetSeatNowStatus.RoomName && SeatNum == objJ_GetSeatNowStatus.SeatShortNo)
                            {
                                OperateCode = "6";
                                message = "签到成功";
                                tmp = "Leave/Leave";
                                returnHtml = "<div class=\"ui-dialog show\">" +
                                               "<div class=\"ui-dialog-cnt\">" +
                                                " <div class=\"ui-dialog-bd\">" +
                                                        " <h3>预约签到</h3>" +
                                                         "<p>您确定签到【" + ReadingRoomNo + "】的【" + tmpSeatNum + "】号座位？维持良好学习环境，请勿恶意操作</p>" +
                                                  "</div>" +
                                                    "<div class=\"ui-dialog-ft\">" +
                                                        "<button id=\"btnResult6Cancel\" onclick=\"btnResult6CancelClick();\" type = \"button\" data-role=\"button\">取消</button>" +
                                                        "<button id=\"btnResult6Ok\" onclick=\"btnResult6OkClick('" + user.SchoolNo + "','" + user.StudentNo + "');\" type = \"button\" data-role=\"button\" class=\"btn-recommand\">确认签到</button>" +
                                                    "</div>" +
                                                 "</div>" +
                                                "</div>";
                            }
                            else
                            {
                                OperateCode = "-9";
                                message = "签到位置错误，请确认阅览室和座位编号";
                                tmp = "Leave/ex";
                            }
                        }
                        else if (UserNowState == "Waiting")//不操作
                        {
                            if (objJ_GetSeatNowStatus.Status == "Seating")
                            {
                                //用户处于等待状态暂时不处理
                                OperateCode = "-9";
                                message = "用户处于等待状态暂时不处理";
                                tmp = "Waiting/Seating";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Leave")
                            {
                                //用户处于等待状态暂时不处理
                                OperateCode = "-9";
                                message = "用户处于等待状态暂时不处理";
                                tmp = "Waiting/Leave";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Booking")
                            {
                                //该位置已经有预约，不能操作
                                message = "用户处于等待状态暂时不处理";
                                tmp = "Waiting/Booking";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Waiting")
                            {
                                //用户处于等待状态暂时不处理
                                OperateCode = "-9";
                                message = "用户处于等待状态暂时不处理";
                                tmp = "Waiting/Waiting";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "ShortLeave")
                            {
                                //用户处于等待状态暂时不处理
                                OperateCode = "-9";
                                message = "用户处于等待状态暂时不处理";
                                tmp = "Waiting/ShortLeave";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "StopUsed")
                            {
                                //用户处于等待状态暂时不处理
                                OperateCode = "-9";
                                message = "用户处于等待状态暂时不处理";
                                tmp = "Waiting/StopUsed";
                            }
                            else
                            {
                                //用户处于等待状态暂时不处理
                                OperateCode = "-9";
                                message = "用户处于等待状态暂时不处理";
                                tmp = "Waiting/ex";
                            }
                        }
                        else if (UserNowState == "ShortLeave")//暂离回来
                        {
                            if (objJ_GetSeatNowStatus.Status == "Seating" ||  objJ_GetSeatNowStatus.Status == "ShortLeave")
                            {
                                //判断位置是不是自己的，是否符合暂离回来操作
                                //message = "判断位置是不是自己的，是否符合暂离回来操作";
                                //tmp = "ShortLeave/Seating";
                                if (UserInRoom == objJ_GetSeatNowStatus.RoomName && SeatNum == objJ_GetSeatNowStatus.SeatShortNo)
                                {
                                    OperateCode = "5";
                                    //  public static bool ComeBack(string SchoolNum, string studentNo, out string Msg)
                                    returnHtml = "<div class=\"ui-dialog show\">" +
                                                   "<div class=\"ui-dialog-cnt\">" +
                                                    " <div class=\"ui-dialog-bd\">" +
                                                            " <h3>操作确认</h3>" +
                                                             "<p>您的操作是【暂离回来】，座位是阅览室【" + ReadingRoomNo + "】的【" + tmpSeatNum + "】号？维持良好学习环境，请勿恶意操作</p>" +
                                                      "</div>" +
                                                        "<div class=\"ui-dialog-ft\">" +
                                                            "<button id=\"btnResult5Cancel\" onclick=\"btnResult5CancelClick();\" type = \"button\" data-role=\"button\">取消</button>" +
                                                            "<button id=\"btnResult5Ok\" onclick=\"btnResult5OkClick('" + user.SchoolNo + "','" + user.StudentNo + "');\" type = \"button\" data-role=\"button\" class=\"btn-recommand\">暂离回来</button>" +
                                                        "</div>" +
                                                     "</div>" +
                                                    "</div>";
                                    //  WriteLog.Write(returnHtml);
                                }
                                else
                                {
                                    message = "您的操作是【暂离回来】，但这个位置不是您刚才的位置，请确认";
                                }
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Leave" )
                            {
                                //判断位置是不是自己的，是否符合暂离回来操作
                                //message = "判断位置是不是自己的，是否符合暂离回来操作";
                                //tmp = "ShortLeave/Leave";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Booking")
                            {
                                //判断位置是不是自己的，是否符合暂离回来操作
                                message = "判断位置是不是自己的，是否符合暂离回来操作";
                                tmp = "ShortLeave/Booking";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "Waiting")
                            {
                                //判断位置是不是自己的，是否符合暂离回来操作
                                tmp = "ShortLeave/Waiting";
                            }
                            else if (objJ_GetSeatNowStatus.Status == "ShortLeave")
                            {
                                //判断位置是不是自己的，是否符合暂离回来操作
                                //message = "判断位置是不是自己的，是否符合暂离回来操作";
                              //  tmp = "ShortLeave/ShortLeave";


                            }
                            else if (objJ_GetSeatNowStatus.Status == "StopUsed")
                            {
                                //该位置已经有停用，不能操作
                                message = "判断位置是不是自己的，是否符合暂离回来操作";
                                tmp = "ShortLeave/StopUsed";
                            }
                            else
                            {
                                //座位异常，不能操作
                                message = "座位异常，不能操作";
                                tmp = "ShortLeave/ex";
                            }
                        }
                        else //异常
                        {
                            message = "座位异常，不能操作";
                            tmp = "ex";
                        }
                    }
                }
                bindResult = "yes";
                return Json(new { status = bindResult, message = message, opCode = OperateCode, tmp = tmp, retHtml= returnHtml }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteLog.Write(ex.ToString());
                return Json(new { status = bindResult, message = ex.Message, opCode = "-9", tmp = "ex" }, JsonRequestBehavior.AllowGet);
            }


        }


        #endregion

        #region 扫码绑定用户
        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <returns></returns>
        public ActionResult BindUserInfo()
        {
            if (Session["ID"] == null || Session["User"] != null)
            {
                return RedirectToAction("Index", "User");
            }
            //JSSDK sdk = new JSSDK(GetAppSettings.AppID, GetAppSettings.AppSecret, true);
            //SignPackage config = sdk.GetSignPackage(JsApiEnum.scanQRCode | JsApiEnum.onMenuShareQQ | JsApiEnum.closeWindow);
            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //ViewBag.config = serializer.Serialize(config);

            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(GetAppSettings.AppID, GetAppSettings.AppSecret, "http://wechat.gxchuwei.com/User/BindUserInfo");
            //return Content(Request.Url.AbsoluteUri);
            return View(jssdkUiPackage);
        }

        [OAuthBaseFilter]
        public ActionResult CancelBind()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CancelBindAction()
        {
            SeatManage.SeatManageComm.WriteLog.Write("CancelBindAction");
            try
            {
                tb_User user = Session["User"] as tb_User;
                // SeatManage.SeatManageComm.WriteLog.Write("CancelBindAction"+user.OpenId);
                user.Attach();
                user.State = 1;
                user.StudentNo = null;
                user.SchoolNo = null;
                user.CardNo = null;
                DbSession.Default.Update<tb_User>(user);
                Session.Clear();
                SeatManage.SeatManageComm.WriteLog.Write("CancelBindAction2" + "帐号已解绑");
                return Json(new { status = "yes", message = "帐号已解绑，如需使用请重新绑定账户！" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                SeatManage.SeatManageComm.WriteLog.Write("ex:" + ex);
                return Json(new { status = "no", message = ex.Message }, JsonRequestBehavior.AllowGet);
                //throw;
            }

        }

        [HttpPost]
        public JsonResult ScanBindUserInfo(string ScanResult)
        {
            string message = "";
            string bindResult = "no";
            try
            {
                string str = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(ScanResult);
                if (str == "")
                {
                    message = "二维码无效，请选【微信绑定】功能后刷卡或输入编号获取您的绑定二维码！";
                }
                else
                {
                    // SeatManage.SeatManageComm.WriteLog.Write("Session[ID].ToString()" + Session["ID"].ToString());
                    NameValueCollection col = Common.UrlCommon.GetQueryString(str);
                    tb_User user = DbSession.Default.From<tb_User>().Where(tb_User._.OpenId == Session["ID"].ToString()).ToFirst();
                    if (user.State == 2)
                    {
                        message = "您的微信已经绑定帐号:" + user.CardNo + ",请勿重复绑定，如需更换请先解绑！";
                    }
                    else if (DbSession.Default.Count<tb_User>(tb_User._.SchoolNo == col["schoolNo"] && tb_User._.CardNo == col["cardNo"]) > 0)
                    {
                        //message = "对不起，此帐号已经被绑定！请联系管理员核实身份信息！";


                        tb_User odluser = DbSession.Default.From<tb_User>().Where(tb_User._.SchoolNo == col["schoolNo"] && tb_User._.CardNo == col["cardNo"]).ToFirst();
                        odluser.Attach();
                        odluser.State = 1;
                        odluser.StudentNo = null;
                        odluser.SchoolNo = null;
                        odluser.CardNo = null;
                        DbSession.Default.Update<tb_User>(odluser);


                        user.Attach();
                        user.SchoolNo = col["schoolNo"];
                        user.CardNo = col["cardNo"];
                        user.ClientNo = col["clientNo"];
                        if (user.SchoolNo != null)
                        {
                            if (user.SchoolNo != string.Empty)
                            {
                                user.State = 2;
                                string msg;
                                if (AppWebService.BasicAPI.GetUserInfo(user.SchoolNo, user.CardNo, out msg))
                                {
                                    J_GetUserInfo entity = JSONSerializer.Deserialize<J_GetUserInfo>(msg);
                                    user.Name = entity.Name;
                                    user.StudentNo = entity.StudentNo;
                                    user.Department = entity.Department;
                                    user.ReaderType = entity.ReaderType;
                                    if (user.ExpDate == null)
                                    {
                                        user.ExpDate = DateTime.Now.AddMonths(3);
                                    }
                                    DbSession.Default.Update<tb_User>(user);
                                    message = "恭喜你【" + user.Name + "】帐号绑定成功，当前使用帐号:" + user.StudentNo;
                                    bindResult = "yes";
                                }
                                else
                                {
                                    message = "帐号绑定失败：没有查询到用户信息，请联系管理员！";
                                }
                            }
                            else
                            {
                                message = "帐号绑定失败：没有查询到用户信息，请联系管理员！";
                            }
                        }
                        else
                        {
                            message = "帐号绑定失败：没有查询到用户信息，请联系管理员！";
                        }


                    }
                    else
                    {
                        user.Attach();
                        user.SchoolNo = col["schoolNo"];
                        user.CardNo = col["cardNo"];
                        user.ClientNo = col["clientNo"];
                        user.State = 2;
                        if (user.SchoolNo != null)
                        {
                            if (user.SchoolNo != string.Empty)
                            {
                                string msg;
                                if (AppWebService.BasicAPI.GetUserInfo(user.SchoolNo, user.CardNo, out msg))
                                {
                                    J_GetUserInfo entity = JSONSerializer.Deserialize<J_GetUserInfo>(msg);
                                    user.Name = entity.Name;
                                    user.StudentNo = entity.StudentNo;
                                    user.Department = entity.Department;
                                    user.ReaderType = entity.ReaderType;
                                    if (user.ExpDate == null)
                                    {
                                        user.ExpDate = DateTime.Now.AddMonths(3);
                                    }
                                    DbSession.Default.Update<tb_User>(user);
                                    message = "恭喜你【" + user.Name + "】帐号绑定成功，当前使用帐号:" + user.StudentNo;
                                    bindResult = "yes";
                                }
                                else
                                {
                                    message = "帐号绑定失败：没有查询到用户信息，请联系管理员！";
                                }
                            }
                            else
                            {
                                message = "帐号绑定失败：没有查询到用户信息，请联系管理员！";
                            }
                        }
                        else
                        {
                            message = "帐号绑定失败：没有查询到用户信息，请联系管理员！";
                        }

                    }

                }

                return Json(new { status = bindResult, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = bindResult, message = ex.Message }, JsonRequestBehavior.AllowGet);

            }


        }
        #endregion

        #region 个人中心首页
        /// <summary>
        /// 个人中心首页
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult Index()
        {
            var u = (Session["User"] as tb_User);
            SeatManage.SeatManageComm.WriteLog.Write(u.NickName+"&hello");
          //  SeatManage.SeatManageComm.WriteLog.Write("用户：" + u.CardNo);
            ViewBag.User = DbSession.Default.From<tb_User>().Where(tb_User._.ID == u.ID).ToFirst();
            string msg;
            if (AppWebService.BasicAPI.GetLibraryNowState(ViewBag.User.SchoolNo, out msg))
            {
                List<J_GetLibraryNowState> LibraryList = JSONSerializer.JSONStringToList<J_GetLibraryNowState>(msg);
                ViewBag.List = LibraryList;
                return View();
            }
            return Content(msg);
        }
        #endregion

        #region 预约签到



        #endregion

        #region 座位状态
        /// <summary>
        /// 座位状态
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult SeatState(string param)
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            if (param != null && param != "")
                param = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(param.Replace(" ", "+"));

            string besappsekLog;
            List<J_GetBesapsekLog> list = new List<J_GetBesapsekLog>();
            AppWebService.BasicAPI.GetBesapsekLog(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 100, out besappsekLog);
            foreach (J_GetBesapsekLog item in JSONSerializer.JSONStringToList<J_GetBesapsekLog>(besappsekLog))
            {
                if (item.IsValid)
                {
                    list.Add(item);
                }
            }
            ViewBag.List = list;
            ViewBag.Count = list.Count;

            if (param != null && param != "")
            {
                NameValueCollection paramlist = UrlCommon.GetQueryString(param);
                //schoolNo=2014101603&clientNo=201410160302&codeTime=2016-02-23 11:20:15
                DateTime CodeTime = Convert.ToDateTime(paramlist["codeTime"].ToString());
                string SchoolNo = paramlist["schoolNo"].ToString();
                if (CodeTime > DateTime.Now.AddMinutes(-5))
                {
                    AppWebService.BasicAPI.GetUserNowState(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, true, out msg);
                    ViewBag.UserNowState = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                    //return Content("1   " + CodeTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"));
                    return View();
                }
                else
                {
                    AppWebService.BasicAPI.GetUserNowState(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, false, out msg);
                    ViewBag.UserNowState = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                    //return Content("2   " + CodeTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"));
                    return View();
                }
            }
            else
            {
                AppWebService.BasicAPI.GetUserNowState(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, false, out msg);
                ViewBag.UserNowState = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                return View();
            }


        }
        #endregion

        #region 座位状态操作
        /// <summary>
        /// 释放座位
        /// </summary>
        /// <param name="ScanResult"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SeatOperation(string action)
        {
            //SeatManage.SeatManageComm.WriteLog.Write("SeatOperation");
            //SeatManage.SeatManageComm.WriteLog.Write("SeatOperation action:"+ action);
            string message = "";
            tb_User user = Session["User"] as tb_User;
            switch (action)
            {
                case "Leave":
                    AppWebService.BasicAPI.ReleaseSeat(user.SchoolNo, user.CardNo, out message);
                    break;
                case "ShortLeave":
                    AppWebService.BasicAPI.ShortLeave(user.SchoolNo, user.CardNo, out message);
                    break;
                case "CancelWait":
                    AppWebService.BasicAPI.CancelWait(user.SchoolNo, user.CardNo, out message);
                    break;
                case "CancelBook":
                    AppWebService.BasicAPI.CancelBesapeakByCardNo(user.SchoolNo, user.CardNo, out message);
                    break;
                case "ComeBack":
                    AppWebService.BasicAPI.ComeBack(user.SchoolNo, user.CardNo, out message);
                    break;
                case "CheckBook":
                    AppWebService.BasicAPI.CheckSeat(user.SchoolNo, user.CardNo, out message);
                    break;
                default:
                    break;
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据编号取消预约
        /// </summary>
        /// <param name="bespeakId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CancelBesapeak(string bespeakId)
        {
            string message = "";
            tb_User user = Session["User"] as tb_User;
            AppWebService.BasicAPI.CancelBesapeak(user.SchoolNo, Convert.ToInt32(bespeakId), out message);
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 剩余座位
        /// <summary>
        /// 剩余座位
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult ResidueSeat()
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            if (AppWebService.BasicAPI.GetCanBespeakRoomInfo(ViewBag.User.SchoolNo, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), out msg))
            {
                //List<J_GetCanBespeakRoomInfo> list = JSONSerializer.JSONStringToList<J_GetCanBespeakRoomInfo>(msg);
                //ViewBag.List = list;
                List<J_GetCanBespeakRoomInfo> list = new List<J_GetCanBespeakRoomInfo>();
                foreach (J_GetCanBespeakRoomInfo item in JSONSerializer.JSONStringToList<J_GetCanBespeakRoomInfo>(msg))
                {
                    string message;
                    AppWebService.BasicAPI.GetRoomBesapeakState(ViewBag.User.SchoolNo, item.RoomNo, DateTime.Now.ToString("yyyy-MM-dd"), out message);
                    JObject ja = (JObject)JsonConvert.DeserializeObject(message);
                    List<J_GetRoomBesapeakState> RoomList = JSONSerializer.JSONStringToList<J_GetRoomBesapeakState>(ja["SeatList"].ToString());
                    item.ResidueSeat = RoomList.Count().ToString();
                    list.Add(item);
                }
                ViewBag.List = list;
                return View();
            }
            else
            {
                return Content(msg);
            }
        }
        #endregion

        #region 记录查询

        [OAuthBaseFilter]
        public ActionResult RecordQuery()
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            J_UserRecord objJ_UserRecord = new J_UserRecord();

            AppWebService.BasicAPI.GetEnterOutLog(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg);
            List<J_GetEnterOutLog> EnterOutLoglist = JSONSerializer.JSONStringToList<J_GetEnterOutLog>(msg);
            objJ_UserRecord.EnterOutLogList = EnterOutLoglist;

            AppWebService.BasicAPI.GetViolationLog(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg);
            List<J_GetViolationLog> ViolationLogList = JSONSerializer.JSONStringToList<J_GetViolationLog>(msg);
            objJ_UserRecord.ViolationLogList = ViolationLogList;

            AppWebService.BasicAPI.GetBlacklist(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg);
            List<J_GetBlacklist> BlacklistList = JSONSerializer.JSONStringToList<J_GetBlacklist>(msg);
            objJ_UserRecord.BlackList = BlacklistList;

            AppWebService.BasicAPI.GetBesapsekLog(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg);
            List<J_GetBesapsekLog> besapsekLogList = JSONSerializer.JSONStringToList<J_GetBesapsekLog>(msg);
            objJ_UserRecord.BesapsekLogList = besapsekLogList;
            ViewBag.Data = objJ_UserRecord;



            return View();
        }

        #endregion

        #region 进出记录

        /// <summary>
        /// 获取进出记录
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult EnterOutLog()
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            if (AppWebService.BasicAPI.GetEnterOutLog(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg))
            {
                List<J_GetEnterOutLog> list = JSONSerializer.JSONStringToList<J_GetEnterOutLog>(msg);
                ViewBag.List = list;
                return View();
            }
            else
            {
                ViewBag.List = null;
                return View();
            }
        }
        #endregion

        #region 违规记录

        /// <summary>
        /// 获取违规记录
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetViolationLog()
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            AppWebService.BasicAPI.GetViolationLog(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg);
            List<J_GetViolationLog> list = JSONSerializer.JSONStringToList<J_GetViolationLog>(msg);
            ViewBag.List = list;
            return View();

        }
        #endregion

        #region 黑名单记录
        /// <summary>
        /// 黑名单记录
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetBlacklist()
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            AppWebService.BasicAPI.GetBlacklist(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg);
            List<J_GetBlacklist> list = JSONSerializer.JSONStringToList<J_GetBlacklist>(msg);
            ViewBag.List = list;
            return View();

        }
        #endregion

        #region 预约记录
        /// <summary>
        /// 获取预约记录
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetBesapsekLog()
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            if (AppWebService.BasicAPI.GetBesapsekLog(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg))
            {
                List<J_GetBesapsekLog> list = JSONSerializer.JSONStringToList<J_GetBesapsekLog>(msg);
                ViewBag.List = list;
                return View();
            }
            else
            {
                return Content(msg);
            }
        }
        #endregion

        #region 座位预约
        /// <summary>
        /// 显示当日可预约阅览室
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult SubscribeSeat()
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            AppWebService.BasicAPI.GetCanBespeakRoomInfo(ViewBag.User.SchoolNo, DateTime.Now.ToString("yyyy-MM-dd"), out msg);
            List<J_GetCanBespeakRoomInfo> list = JSONSerializer.JSONStringToList<J_GetCanBespeakRoomInfo>(msg);
            ViewBag.List = list;
            return View();
        }

        /// <summary>
        /// 获取指定日期阅览室
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetReadingRoom(string date)
        {
            string str = "";
            string msg;
            tb_User user = Session["User"] as tb_User;
            AppWebService.BasicAPI.GetCanBespeakRoomInfo(user.SchoolNo, date, out msg);
            List<J_GetCanBespeakRoomInfo> list = JSONSerializer.JSONStringToList<J_GetCanBespeakRoomInfo>(msg);
            foreach (J_GetCanBespeakRoomInfo item in list)
            {
                //  SeatManage.SeatManageComm.WriteLog.Write(" <a href=\"/User/GetRoomBesapeakState?date=" + date + "&roomNo =" + item.RoomNo + "&roomName =" + item.RoomName + "\"> <div class=\"ui-btn\"> 选位置</div></a>");
                str += "<li class=\"ui-border-t\">" +
                            " <div class=\"ui-avatar-s\">" +
                                  "<span style = \"background-image:url(/Img/readingroom.jpg)\" ></span> " +
                             " </div > " +
                            " <div class=\"ui-list-info\">" +
                     "    <h4 class=\"ui-nowrap\">阅览室：" + item.RoomName + "</h4>" +
                       " <h5 class=\"ui-nowrap\">所属图书馆:" + item.LibraryName + "</h5>" +
                    " </div>" +
                    " <a href=\"/User/GetRoomBesapeakState?date=" + date + "&roomNo=" + item.RoomNo + "&roomName=" + item.RoomName + "\"> <div class=\"ui-btn\"> 选位置</div></a>" +
                " </li>";
            }
            return Json(new { message = str }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取阅览室座位信息
        /// </summary>
        /// <param name="date"></param>
        /// <param name="roomNo"></param>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetRoomBesapeakState(string date, string roomNo, string roomName)
        {

            // SeatManage.SeatManageComm.WriteLog.Write(date+"--"+ roomNo+"--"+roomName);


            ViewBag.User = Session["User"] as tb_User;
            string msg;
            if (AppWebService.BasicAPI.GetRoomBesapeakState(ViewBag.User.SchoolNo, roomNo, date, out msg))
            {
                JObject ja = (JObject)JsonConvert.DeserializeObject(msg);
                List<J_GetRoomBesapeakState> list = JSONSerializer.JSONStringToList<J_GetRoomBesapeakState>(ja["SeatList"].ToString());
                ViewBag.List = list;
                ViewBag.TimeList = ja["TimeList"].ToString();
                ViewBag.Date = date;
                ViewBag.RoomNo = roomNo;
                ViewBag.RoomName = roomName;
                return View();
            }
            else
            {
                return Content(msg);
            }
        }

        [OAuthBaseFilter]
        public ActionResult ConfirmPage(string date, string roomNo, string roomName, string seatNo, string seatShortNo)
        {
            //return Content(date + "  " + roomNo + "  " + roomName + "  " + seatNo + "  " + seatShortNo);
            try
            {
                ViewBag.User = Session["User"] as tb_User;
                string msg;
                ViewBag.Date = date;
                ViewBag.RoomNo = roomNo;
                ViewBag.RoomName = roomName;
                ViewBag.SeatNo = seatNo;
                ViewBag.SeatShortNo = seatShortNo;
                AppWebService.BasicAPI.GetRoomBesapeakState(ViewBag.User.SchoolNo, roomNo, date, out msg);
                JObject ja = (JObject)JsonConvert.DeserializeObject(msg);
                ViewBag.TimeList = ja["TimeList"].ToString();
                ViewBag.CheckBeforeTime = Convert.ToInt32(ja["CheckBeforeTime"].ToString());
                ViewBag.CheckLastTime = Convert.ToInt32(ja["CheckLastTime"].ToString());
                ViewBag.CheckKeepTime = Convert.ToInt32(ja["CheckKeepTime"].ToString());
                ViewBag.IsCanSelectTime = Convert.ToBoolean(ja["IsCanSelectTime"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
                //throw;
            }

        }

        /// <summary>
        /// 选择座位预约
        /// </summary>
        /// <param name="seatNo"></param>
        /// <param name="roomNo"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitBesapeskSeat(string seatNo, string roomNo, string date, string time)
        {
            tb_User user = Session["User"] as tb_User;
            string datetime = date + (time != "none" ? (" " + time) : "");
            string msg;
            AppWebService.BasicAPI.SubmitBesapeskSeat(user.SchoolNo, seatNo, roomNo, user.StudentNo, datetime, (time != "none" ? false : true), out msg);
            return Json(new { message = msg }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 查看座位信息
        /// <summary>
        /// 查询座位信息
        /// </summary>
        /// <returns></returns>
        [OAuthGetSeatNowStatusFilter]
        public ActionResult GetSeatNowStatus(string param)
        {
            try
            {
                ViewBag.User = Session["User"] as tb_User;
                string msg;
                if (param != null && param != "")
                    param = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(param.Replace(" ", "+"));
                if (param != null && param != "")
                {

                    NameValueCollection paramlist = UrlCommon.GetQueryString(param);

                    if (paramlist["schoolNo"].ToString() != ViewBag.User.SchoolNo)
                    {
                        return Content("查询失败，只能查询自己所在学校！");
                    }
                    AppWebService.BasicAPI.GetSeatNowStatus(ViewBag.User.SchoolNo, paramlist["seatNo"].ToString(), paramlist["roomNo"].ToString(), ViewBag.User.StudentNo, out msg);
                    WriteLog.Write("GetSeatNowStatus msg:" + msg);
                    J_GetSeatNowStatus SeatNowStatus = JSONSerializer.Deserialize<J_GetSeatNowStatus>(msg);
                    ViewBag.GetSeatNowStatus = SeatNowStatus;

                    ViewBag.seatNo = paramlist["seatNo"].ToString();
                    ViewBag.roomNo = paramlist["roomNo"].ToString();
                    ViewBag.DateCount = SeatNowStatus.CanBookingDate.Count();

                    return View();
                }
                else
                {
                    return Content("参数无效，请勿非法操作!");
                }


            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        /// <summary>
        /// 更换座位
        /// </summary>
        /// <param name="seatNo">座位编号</param>
        /// <param name="roomNo">阅览室</param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult ChangeSeat(string seatNo, string roomNo)
        //{
        //    string str = "";
        //    string msg;
        //    tb_User user = Session["User"] as tb_User;
        //    AppWebService.BasicAPI.ChangeSeat(user.SchoolNo, seatNo, roomNo, user.StudentNo, out msg);
        //    return Json(new { message = msg }, JsonRequestBehavior.AllowGet);
        //}
        #endregion

    }
}
