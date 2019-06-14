using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinMsgService.OutMsg
{
    public class TempModel
    {
        public string touser { get; set; }
        public string template_id { get; set; }
        public string url { get; set; }

        public string topcolor { get; set; }

        /// <summary>
        /// 图书馆超期催还通知
        /// </summary>
        public OverdueNoticeTempData objOverdueNoticeTempData { get; set; }
        /// <summary>
        /// 还书通知
        /// </summary>
        public GiveBackBookNoticeTempData objGiveBackBookNoticeTempData { get; set; }
        /// <summary>
        /// 委托图书到馆通知
        /// </summary>
        public BooksToLibraryNoticeTempData objBooksToLibraryNoticeTempData { get; set; }
        /// <summary>
        /// 成功还书通知
        /// </summary>
        public GiveBackBookSucceedNoticeTempData objGiveBackBookSucceedNoticeTempData { get; set; }
        /// <summary>
        /// 活动即将开始提醒
        /// </summary>
        public ActivityToBeginningNoticeTempData objActivityToBeginningNoticeTempData { get; set; }
        /// <summary>
        /// 图书馆借书成功通知
        /// </summary>
        public BorrowBooksSucceedNoticeTempData objBorrowBooksSucceedNoticeTempData { get; set; }

        //public TemplateModel(string first, string keyword1, string keyword2, string keyword3, string remark)
        //{
        //    data = new TemplateData()
        //    {
        //        first = new TempItem(first),
        //        keyword1 = new TempItem(keyword1),
        //        keyword2 = new TempItem(keyword2),
        //        keyword3 = new TempItem(keyword3),
        //        remark = new TempItem(remark)
        //    };

        //}
    }
}
