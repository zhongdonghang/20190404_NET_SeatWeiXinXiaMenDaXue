using Common;
using Dos.ORM;
using Model;
using MvcExtension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeatManage.SeatManageComm;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class NewUserController : Controller
    {
        //
        // GET: /NewUser/

        #region 主页
        /// <summary>
        /// 主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 我的
        /// <summary>
        /// 我的
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult Profile()
        {
            var u = (Session["User"] as tb_User);
            SeatManage.SeatManageComm.WriteLog.Write(u.NickName + "&hello,NewLayout");
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

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetUserInformation()
        {
            var u = (Session["User"] as tb_User);
            SeatManage.SeatManageComm.WriteLog.Write(u.NickName + "&hello");
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

        /// <summary>
        /// 我的学习记录
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetStudyLog()
        {
            return View();
        }

        /// <summary>
        /// 我的预约
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult MyReserve(string param)
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

        /// <summary>
        /// 图书馆信息
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetLibraryInformation()
        {
            var u = (Session["User"] as tb_User);
            SeatManage.SeatManageComm.WriteLog.Write(u.NickName + "&hello");
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

        #region 当前座位
        /// <summary>
        /// 当前座位
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult MySeat(string param)
        {
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            if (param != null && param != "")
                param = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(param.Replace(" ", "+"));
            
            string besappsekLog;
            List<J_GetBesapsekLog> list = new List<J_GetBesapsekLog>();
            AppWebService.BasicAPI.GetBesapsekLog(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 100, out besappsekLog);
            //SeatManage.SeatManageComm.WriteLog.Write("33");
            //SeatManage.SeatManageComm.WriteLog.Write("ViewBag.User.SchoolNo："+ ViewBag.User.SchoolNo+ "ViewBag.User.StudentNo:"+ ViewBag.User.StudentNo+ "besappsekLog:"+besappsekLog);
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
                SeatManage.SeatManageComm.WriteLog.Write("44");
                NameValueCollection paramlist = UrlCommon.GetQueryString(param);
                //schoolNo=2014101603&clientNo=201410160302&codeTime=2016-02-23 11:20:15
                DateTime CodeTime = Convert.ToDateTime(paramlist["codeTime"].ToString());
                string SchoolNo = paramlist["schoolNo"].ToString();
                if (CodeTime > DateTime.Now.AddMinutes(-5))
                {
                    SeatManage.SeatManageComm.WriteLog.Write("1");
                    AppWebService.BasicAPI.GetUserNowState(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, true, out msg);
                    SeatManage.SeatManageComm.WriteLog.Write(msg);
                    ViewBag.UserNowState = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                    //return Content("1   " + CodeTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"));
                    return View();
                }
                else
                {
                    SeatManage.SeatManageComm.WriteLog.Write("2");
                    AppWebService.BasicAPI.GetUserNowState(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, false, out msg);
                    SeatManage.SeatManageComm.WriteLog.Write(msg);
                    ViewBag.UserNowState = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                    //return Content("2   " + CodeTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"));
                    return View();
                }
            }
            else
            {

                AppWebService.BasicAPI.GetUserNowState(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, false, out msg);
               // SeatManage.SeatManageComm.WriteLog.Write("msg:" + msg);
                ViewBag.UserNowState = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                return View();
            }
        }

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

        #region 座位码
        /// <summary>
        /// 座位码
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult ShowQRCode(string param)
        {
            tb_User user = Session["User"] as tb_User;
            ViewBag.User = Session["User"] as tb_User;
            string msg;
            if (param != null && param != "")
                param = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(param.Replace(" ", "+"));
            try
            {
                string path = Server.MapPath("~/QRCodeImages/" + user.SchoolNo + "_" + user.CardNo + ".jpg");
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

        #region 座位预约
        /// <summary>
        /// 座位预约页面
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult ReserveSeat()
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
                str += "<a class=\"weui-cell weui-cell_access\" href=\"/NewUser/GetRoomBesapeakState?date=" + date + "&roomNo=" + item.RoomNo + "&roomName=" + item.RoomName + "\">" +
                            "<div class=\"weui-cell__hd\"><img src=\"/dist/img/Seat1.png\" alt=\"\" style=\"width:20px;margin-right:5px;display:block\"></div>" +
                                  "<div class=\"weui-cell__bd\"><p>图书馆：" + item.LibraryName + "</p><p>阅览室：" + item.RoomName + "</p>" +
                                  "</div>" +
                            "<div class=\"weui - cell__ft\">去选座</div>" +
                        "</a>";

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

        #region 进出记录
        /// <summary>
        /// 进出记录
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetEnterOutLog()
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

        #region 预约记录
        /// <summary>
        /// 预约记录
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult GetReserveLog()
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

        #region 违规/黑名单
        /// <summary>
        /// 违规/黑名单
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
            AppWebService.BasicAPI.GetBlacklist(ViewBag.User.SchoolNo, ViewBag.User.StudentNo, 0, 20, out msg);
            List<J_GetBlacklist> listBlack = JSONSerializer.JSONStringToList<J_GetBlacklist>(msg);
            ViewBag.ListBlack = listBlack;
            return View();
        }
        #endregion

        #region 排行榜
        /// <summary>
        /// 总榜
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult RankAll()
        {
            return View();
        }

        /// <summary>
        /// 月榜
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult RankMonth()
        {
            return View();
        }

        /// <summary>
        /// 周榜
        /// </summary>
        /// <returns></returns>
        [OAuthBaseFilter]
        public ActionResult RankWeek()
        {
            return View();
        }
        #endregion

        #region 绑定/解绑
        /// <summary>
        /// 绑定
        /// </summary>
        /// <returns></returns>
        public ActionResult Bind()
        {
            if (Session["ID"] == null || Session["User"] != null)
            {
                return RedirectToAction("Profile", "NewUser");
            }
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(GetAppSettings.AppID, GetAppSettings.AppSecret, "http://wechat.gxchuwei.com/NewUser/Bind");
            return View(jssdkUiPackage);
        }

        /// <summary>
        /// 解绑
        /// </summary>
        /// <returns></returns>
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

        #region 使用手册
            /// <summary>
            /// 使用手册
            /// </summary>
            /// <returns></returns>
        public ActionResult Manual()
        {
            return View();
        }
        #endregion

        #region 自述
        /// <summary>
        /// ReadMe
        /// </summary>
        /// <returns></returns>
        public ActionResult ReadMe()
        {
            return View();
        }
        #endregion
    }
}
