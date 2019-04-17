using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetUserNowState
    {
        private string _StudentNum;

        public string StudentNum
        {
            get { return _StudentNum; }
            set { _StudentNum = value; }
        }

        private string _InRoom;

        public string InRoom
        {
            get { return _InRoom; }
            set { _InRoom = value; }
        }
        private string _SeatNum;

        public string SeatNum
        {
            get { return _SeatNum; }
            set { _SeatNum = value; }
        }
        private string _Status;

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public string StatusStr
        {
            get { return Status == "Seating" ? "在座" : 
                            Status == "Leave" ? "没有座位" :
                            Status == "Booking" ? "等待签到" :
                            Status == "Waiting" ? "正在等待座位" :
                            Status == "ShortLeave" ? "暂离中" : "";
            }
        }

        private string _CanOperation;

        public string CanOperation
        {
            get { return _CanOperation; }
            set { _CanOperation = value; }
        }
        private string _NowStatusRemark;

        public string NowStatusRemark
        {
            get { return _NowStatusRemark; }
            set { _NowStatusRemark = value; }
        }

        private string _Time;

        public string Time
        {
            get { return _Time; }
            set { _Time = value; }
        }
    }
}
