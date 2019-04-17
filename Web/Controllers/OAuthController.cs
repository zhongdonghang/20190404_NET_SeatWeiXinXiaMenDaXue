using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Dos.ORM;
using Model;

namespace Web.Controllers
{
    public class OAuthController : Controller
    {
        //
        // GET: /OAuth/
        public ActionResult BaseCallback(string code, string state)
        {
            //通过，用code换取access_token
            var result = OAuthApi.GetAccessToken(GetAppSettings.AppID, GetAppSettings.AppSecret, code);
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的

            if (DbSession.Default.Count<tb_User>(tb_User._.OpenId == result.openid) > 0)
            {
                tb_User user = DbSession.Default.From<tb_User>().Where(tb_User._.OpenId == result.openid).ToFirst();

                if (user.State == 2)
                {
                    Session["OpenID"] = result.openid;
                    Session["User"] = user;
                    return Redirect(state);
                }
                else
                {
                    Session["ID"] = result.openid;
                    return RedirectToAction("Bind", "NewUser");
                }
            }
            else
            {
                var userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);

                tb_User userEntity = new tb_User()
                {
                    OpenId = userInfo.openid,
                    NickName = userInfo.nickname,
                    HeadImgUrl = userInfo.headimgurl,
                    Name = "",
                    Sex = userInfo.sex,
                    Moblie = "",
                    State = 1,
                    Integral = 0,
                    IsDealer = false,
                    CreateTime = DateTime.Now
                };
                DbSession.Default.Insert<tb_User>(userEntity);
                Session["ID"] = result.openid;
                return RedirectToAction("Bind", "NewUser");
            }
        }


        public ActionResult BaseCallbackGetSeatNow(string code, string state)
        {
            //通过，用code换取access_token
            var result = OAuthApi.GetAccessToken(GetAppSettings.AppID, GetAppSettings.AppSecret, code);
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的

            if (DbSession.Default.Count<tb_User>(tb_User._.OpenId == result.openid) > 0)
            {
                tb_User user = DbSession.Default.From<tb_User>().Where(tb_User._.OpenId == result.openid).ToFirst();

                if (user.State == 2)
                {
                    Session["OpenID"] = result.openid;
                    Session["User"] = user;
                    return Redirect("/User/GetSeatNowStatus?param=" + state);
                }
                else
                {
                    Session["ID"] = result.openid;
                    return RedirectToAction("Bind", "NewUser");
                }
            }
            else
            {
                var userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);

                tb_User userEntity = new tb_User()
                {
                    OpenId = userInfo.openid,
                    NickName = userInfo.nickname,
                    HeadImgUrl = userInfo.headimgurl,
                    Name = "",
                    Sex = userInfo.sex,
                    Moblie = "",
                    State = 1,
                    Integral = 0,
                    IsDealer = false,
                    CreateTime = DateTime.Now
                };
                DbSession.Default.Insert<tb_User>(userEntity);
                Session["ID"] = result.openid;
                return RedirectToAction("Bind", "NewUser");
            }
        }
    }
}
