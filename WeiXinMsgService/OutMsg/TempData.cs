using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeiXinMsgService.Template;

namespace WeiXinMsgService.OutMsg
{
    /// <summary>
    /// 图书馆超期催还通知
    /// </summary>
    public class OverdueNoticeTempData
    {
        public TempItem first { get; set; }
        public TempItem keyword1 { get; set; }
        public TempItem keyword2 { get; set; }
        public TempItem keyword3 { get; set; }
        public TempItem keyword4 { get; set; }
        public TempItem keyword5 { get; set; }
        public TempItem remark { get; set; }

    }

    /// <summary>
    /// 还书通知
    /// </summary>
    public class GiveBackBookNoticeTempData
    {
        public TempItem first { get; set; }
        public TempItem name { get; set; }
        public TempItem date { get; set; }
        public TempItem remark { get; set; }
    }

    /// <summary>
    /// 委托图书到馆通知
    /// </summary>
    public class BooksToLibraryNoticeTempData
    {
        public TempItem first { get; set; }
        public TempItem keyword1 { get; set; }
        public TempItem keyword2 { get; set; }
        public TempItem remark { get; set; }
    }

    /// <summary>
    /// 成功还书通知
    /// </summary>
    public class GiveBackBookSucceedNoticeTempData
    {
        public TempItem first { get; set; }
        public TempItem keyword1 { get; set; }
        public TempItem keyword2 { get; set; }
        public TempItem keyword3 { get; set; }
        public TempItem keyword4 { get; set; }
        public TempItem remark { get; set; }
    }

    /// <summary>
    /// 活动即将开始提醒
    /// </summary>
    public class ActivityToBeginningNoticeTempData
    {
        public TempItem first { get; set; }
        public TempItem keyword1 { get; set; }
        public TempItem keyword2 { get; set; }
        public TempItem keyword3 { get; set; }
        public TempItem keyword4 { get; set; }
        public TempItem remark { get; set; }
    }

    /// <summary>
    /// 图书馆借书成功通知
    /// </summary>
    public class BorrowBooksSucceedNoticeTempData
    {
        public TempItem first { get; set; }
        public TempItem keyword1 { get; set; }
        public TempItem keyword2 { get; set; }
        public TempItem keyword3 { get; set; }
        public TempItem keyword4 { get; set; }
        public TempItem remark { get; set; }
    }
}
