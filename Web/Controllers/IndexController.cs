using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeatManage.SeatManageComm;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.AdvancedAPIs;
using MvcExtension;
using AppWebService;
using Model;



namespace Web.Controllers
{
    [ErrorLog]
    public class IndexController : Controller
    {
        //
        // GET: /Index/
        public ActionResult Index()
        {
            //用于测试返回json使用
            //try
            //{
                //http://zhanzuo.jeeshu.com/UserHome/GetSeatNowStatus?param=Xgxsdicj/yZUDBzS4IAw6ELz1UmvnE5vAnv2JEdZLJaLg25m/220E95F1RXLzYx6VPWoG1v1R6rXa3eI0+0t5A==
                //string str = "JKH28fL2ZxwANO3bor9W/rP4xeEdV7 dSm2cMY1i28u0am/dKkN4e0XqYFjfJTpoCwQpxBOq6qIKPwbEA4eUNe/2Sx93Kd7MPnfv3bkIVc1IbkJc2rJ6nv3XVx/eNgBwBSalNLpKKIt1 4FZxyIWQHWOBTrubiGl4/HyJcN5hAGWfETEWb/UrJox9trrGORusl9hlKbO3uwt68Z9WSACM 2Ph5mc 5st88SiQlH3KTLs8xBhdErOJwBDZw5WImNWHW G7 d0etpO9kCsIS0RGk34k4qxbhxloZO190/Q6TlCl gcTBPkD 4APr80iudNNORXOMRX6jFHLvR01CRbaCtd9z/PmEeCNIzHqUR6SeE=";
                //string param = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(str.Replace(" ", "+"));
                //NameValueCollection paramlist = UrlCommon.GetQueryString(param);
                //str = str.Replace('_', '+').Replace('-', '/').Replace('~', '=');

                //return Content(SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(str));
                string msg;//"Xgxsdicj/yZUDBzS4IAw6MnhIKiJTtjYUBodKmeYlrqVEtW4xthvjYoMKMRZOUeZluBkR1t0/mOhjQwSz6tgpw==";
              
               // AppWebService.BasicAPI.GetLibraryNowState("2014101603", out msg);
                //J_GetSeatNowStatus seatStatus =  JSONSerializer.Deserialize<J_GetSeatNowStatus>(msg);
                //AppWebService.BasicAPI.CancelBesapeakByCardNo("2014101603", "hhk", out msg);
                //AppWebService.BasicAPI.GetRoomBesapeakState("2014101603", "101001", "2016-02-29", out msg);
                //AppWebService.BasicAPI.GetSeatNowStatus("2014101603", "101001001", "101001", "hhk", out msg);
                //J_GetSeatNowStatus entity = JSONSerializer.Deserialize<J_GetSeatNowStatus>(msg);
                return Content(Convert.ToInt32("B").ToString());
                //return Content(SeatManage.SeatManageComm.AESAlgorithm.AESEncrypt(str));
            //}
            //catch (Exception ex)
            //{
            //    return Content(ex.Message);
            //    //throw;
            //}
            


        }

        public ActionResult Demo()
        {
            return View();
        }

    }
}
