using System;
using Senparc.Weixin.MP.Entities;
using System.Collections.Specialized;
using Model;
using Dos.ORM;
using SeatManage.SeatManageComm;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.AdvancedAPIs;


namespace CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        private string GetWelcomeInfo()
        {
            string message = "欢迎关注";
            return message;
        }

        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            // 预处理文字或事件类型请求。

            if (requestMessage.Content == "OneClick")
            {
                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                strongResponseMessage.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
                return strongResponseMessage;
                
            }
            return null;//返回null，则继续执行OnTextRequest或OnEventRequest
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase reponseMessage = null;
            try
            {
                
                tb_User user = DbSession.Default.From<tb_User>().Where(tb_User._.OpenId == requestMessage.FromUserName).ToFirst();
                string message = "操作失败";
                //菜单点击，需要跟创建菜单时的Key匹配
                switch (requestMessage.EventKey)
                {
                    case "我的座位":
                        {
                            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                            string msg;

                            AppWebService.BasicAPI.GetUserNowState(user.SchoolNo, user.StudentNo, false, out msg);
                            J_GetUserNowState entity = JSONSerializer.Deserialize<J_GetUserNowState>(msg);
                            string str = "阅览室:" + (entity.InRoom == "" ? "无" : entity.InRoom) + "\r\n"
                                            + "座位号:" + (entity.SeatNum == "" ? "无" : entity.SeatNum) + "\r\n"
                                            + "在座状态:" + (entity.StatusStr == "" ? "无" : entity.StatusStr) + "\r\n"
                                            + "备注:" + (entity.NowStatusRemark == "" ? "无" : entity.NowStatusRemark);



                            responseMessage.Content = str;
                            reponseMessage = responseMessage;
                        }
                        break;
                    case "暂离座位":
                        {
                            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();

                            AppWebService.BasicAPI.ShortLeave(user.SchoolNo, user.CardNo, out message);
                            responseMessage.Content = message;
                            reponseMessage = responseMessage;
                        }
                        break;
                    case "释放座位":
                        {
                            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                            AppWebService.BasicAPI.ReleaseSeat(user.SchoolNo, user.CardNo, out message);
                            responseMessage.Content = message;
                            reponseMessage = responseMessage;
                        }
                        break;
                    case "取消预约":
                        {
                            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                            AppWebService.BasicAPI.CancelBesapeakByCardNo(user.SchoolNo, user.CardNo, out message);
                            responseMessage.Content = message;
                            reponseMessage = responseMessage;
                        }
                        break;
                    case "解除绑定":
                        {
                            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                            if (user.State != 2)
                            {
                                responseMessage.Content = "对不起，您尚未绑定帐号！";
                            }
                            else
                            {
                                user.Attach();
                                user.State = 1;
                                user.StudentNo = null;
                                user.SchoolNo = null;
                                user.CardNo = null;
                                DbSession.Default.Update<tb_User>(user);
                                responseMessage.Content = "帐号已解绑，如需使用请重新绑定账户！";
                            }
                            reponseMessage = responseMessage;
                        }



                        break;
                    default:
                        {
                            //var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                            //strongResponseMessage.Content = "您点击了按钮：" + requestMessage.EventKey;
                            reponseMessage = null;
                        }
                        break;
                }

                return reponseMessage;
            }
            catch (Exception ex)
            {
                var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content  = "帐号未绑定或学校未连接！";
                reponseMessage = responseMessage;
                return reponseMessage;
            }
            
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "您刚才发送了ENTER事件请求。";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            return responseMessage;
        }

        

        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_MassSendJobFinishRequest(RequestMessageEvent_MassSendJobFinish requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "接收到了群发完成的信息。";
            return responseMessage;
        }


        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            //通过扫描关注
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();

            return responseMessage;
        }


        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = GetWelcomeInfo();

            CreateUserInfo(requestMessage.FromUserName);
            //responseMessage.Content = userinfo.nickname;

            return responseMessage;

        }

        public static void CreateUserInfo(string openid)
        {
            var result = WeiXinApi.GetToken();
            UserInfoJson userinfo = UserApi.Info(result, openid);
            if (DbSession.Default.Count<tb_User>(tb_User._.OpenId == userinfo.openid) == 0)
            {
                tb_User userEntity = new tb_User()
                {
                    OpenId = userinfo.openid,
                    NickName = userinfo.nickname,
                    HeadImgUrl = userinfo.headimgurl,
                    Name = "",
                    Sex = userinfo.sex,
                    Moblie = "",
                    State = 1,
                    Integral = 0,
                    IsDealer = false,
                    CreateTime = DateTime.Now,
                    ExpDate = null
                };
                DbSession.Default.Insert<tb_User>(userEntity);
            }
            else
            {
                tb_User userEntity = DbSession.Default.From<tb_User>().Where(tb_User._.OpenId == userinfo.openid).ToFirst();
                userEntity.Attach();
                userEntity.NickName = userinfo.nickname;
                userEntity.HeadImgUrl = userinfo.headimgurl;
                DbSession.Default.Update<tb_User>(userEntity);
            }
        }

        



        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            //tb_User user = Bll_User.GetUserInfoForOpenId(requestMessage.FromUserName);
            //user.Attach();
            //user.State = 2;
            //Bll_User.UpdateUser(user);
            responseMessage.Content = "有空再来";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            try
            {
                string str = SeatManage.SeatManageComm.AESAlgorithm.AESDecrypt(requestMessage.ScanCodeInfo.ScanResult);
                if (str == "")
                {
                    responseMessage.Content = "二维码无效，请选【微信绑定】功能后刷卡或输入编号获取您的绑定二维码！";
                }
                else
                {
                    NameValueCollection col = Common.UrlCommon.GetQueryString(str);
                    tb_User user = DbSession.Default.From<tb_User>().Where(tb_User._.OpenId == requestMessage.FromUserName).ToFirst();
                    if (user.State == 2)
                    {
                        responseMessage.Content = "您的微信已经绑定帐号:"+user.CardNo+",请勿重复绑定，如需更换请先解绑！";
                    }
                    else if (DbSession.Default.Count<tb_User>(tb_User._.SchoolNo == col["schoolNo"] && tb_User._.CardNo == col["cardNo"]) > 0)
                    {
                        var olduser = DbSession.Default.From<tb_User>().Where(tb_User._.SchoolNo == col["schoolNo"] && tb_User._.CardNo == col["cardNo"]).ToFirst();
                        olduser.Attach();
                        olduser.SchoolNo = null;
                        olduser.CardNo = null;
                        olduser.ClientNo = null;
                        olduser.State = 1;
                        DbSession.Default.Update<tb_User>(olduser);

                        user.Attach();
                        user.SchoolNo = col["schoolNo"];
                        user.CardNo = col["cardNo"];
                        user.ClientNo = col["clientNo"];
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
                            responseMessage.Content = "恭喜你【" + user.Name + "】帐号绑定成功，当前使用帐号:" + user.StudentNo;
                        }
                        else
                        {
                            responseMessage.Content = "帐号绑定失败：没有查询到用户信息，请联系管理员！";
                        }
                    }
                    else
                    {                        
                        user.Attach();
                        user.SchoolNo = col["schoolNo"];
                        user.CardNo = col["cardNo"];
                        user.ClientNo = col["clientNo"];
                        user.State = 2;
                        string msg;
                        if (AppWebService.BasicAPI.GetUserInfo(user.SchoolNo, user.CardNo, out msg))
                        {
                            J_GetUserInfo entity = JSONSerializer.Deserialize<J_GetUserInfo>(msg);
                            user.Name = entity.Name;
                            user.StudentNo = entity.StudentNo;
                            user.Department = entity.Department;
                            user.ReaderType = entity.ReaderType;
                            user.ExpDate = DateTime.Now.AddMonths(3);
                            DbSession.Default.Update<tb_User>(user);
                            responseMessage.Content = "恭喜你【" + user.Name + "】帐号绑定成功，当前使用帐号:" + user.StudentNo;
                        }
                        else
                        {
                            responseMessage.Content = "帐号绑定失败：没有查询到用户信息，请联系管理员！";
                        }
                    }
                    
                }
               
                return responseMessage;
            }
            catch (Exception ex)
            {
                responseMessage.Content = "绑定帐号出错：" + ex.Message;
                
            }
            return responseMessage;
            
        }

        /// <summary>
        /// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出拍照或者相册发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出系统拍照发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出微信相册发图器";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出地理位置选择器";
            return responseMessage;
        }
    }
}