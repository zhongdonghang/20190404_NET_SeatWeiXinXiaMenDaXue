using System.Collections.Generic;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.GoogleMap;
using Senparc.Weixin.MP.Helpers;


namespace CommonService
{
    public class LocationService
    {
        public ResponseMessageText GetResponseMessage(RequestMessageLocation requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            //处理发送位置请求
            responseMessage.Content = requestMessage.Label;
            return responseMessage;
        }
    }
}