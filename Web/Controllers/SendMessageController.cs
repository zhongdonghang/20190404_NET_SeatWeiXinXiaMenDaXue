using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonService;
using SeatManage.SeatManageComm;
using System.Collections.Specialized;
using Common;
using Model;
using Dos.ORM;
using System.IO;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace Web.Controllers
{
    public class SendMessageController : Controller
    {
        

        public ActionResult Index(string msg)
        {
            msg = AESAlgorithm.AESDecrypt(msg.Replace(" ", "+"));//解密参数

            //string path = Server.MapPath("/App_Data/" + Guid.NewGuid().ToString() + ".txt");//将参数写入文件
            //StreamWriter sw = new StreamWriter(path);
            //sw.Write(msg.Replace("&", "\r\n"));//TextBox2中的文本是可以编辑后的。
            //sw.Close();
            //sw.Dispose();

            NameValueCollection param = UrlCommon.GetQueryString(msg);//获取参数
            string SchoolNum = param["SchoolNum"].ToString();
            string StudentNo = param["StudentNo"].ToString();
            tb_User user = DbSession.Default.From<tb_User>().Where(tb_User._.SchoolNo == SchoolNum && tb_User._.StudentNo == StudentNo).ToFirst();
            string MsgType = param["MsgType"].ToString();
            switch (MsgType)
            {
                case "UserOperation":
                    var UserOperation = new UserOperation()
                    {
                        first = new TemplateDataItem(user.Name+" 您好"),
                        keyword1 = new TemplateDataItem(param["Room"].ToString()),
                        keyword2 = new TemplateDataItem(param["SeatNo"].ToString()),
                        keyword3 = new TemplateDataItem(param["AddTime"].ToString()),
                        remark = new TemplateDataItem(param["Msg"].ToString())
                    };
                    TemplateApi.SendTemplateMessage(WeiXinApi.GetToken(), user.OpenId, "At7HOxsJ5CW81OV81hipLglDV21O46UVU9Gm_nToXGQ", "#7B68EE", GetAppSettings.SysURL + "/User/SeatState", UserOperation);
                break;
                default:
                    var UserOperation1 = new UserOperation()
                    {
                        first = new TemplateDataItem(user.Name+" 您好"),
                        keyword1 = new TemplateDataItem(param["Room"].ToString()),
                        keyword2 = new TemplateDataItem(param["SeatNo"].ToString()),
                        keyword3 = new TemplateDataItem(param["AddTime"].ToString()),
                        remark = new TemplateDataItem(param["Msg"].ToString())
                    };
                    TemplateApi.SendTemplateMessage(WeiXinApi.GetToken(), user.OpenId, "At7HOxsJ5CW81OV81hipLglDV21O46UVU9Gm_nToXGQ", "#7B68EE", GetAppSettings.SysURL + "/User/SeatState", UserOperation1);
                break;
            }

            return Content("0");
        }

        #region 定义模板模型
        public class UserOperation
        {
            public TemplateDataItem first { get; set; }
            public TemplateDataItem keyword1 { get; set; }
            public TemplateDataItem keyword2 { get; set; }
            public TemplateDataItem keyword3 { get; set; }
            public TemplateDataItem remark { get; set; }
        }

        public class AdminOperation
        {
            public TemplateDataItem first { get; set; }
            public TemplateDataItem keyword1 { get; set; }
            public TemplateDataItem keyword2 { get; set; }
            public TemplateDataItem keyword3 { get; set; }
            public TemplateDataItem remark { get; set; }
        }

        #endregion

    }
}
