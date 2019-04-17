using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostHandlerService
{
    public class J_GetUserNowState
    {
        private string _StudentNum;
        private string _InRoom;
        private string _SeatNum;
        private string _Status;
        private string _CanOperation;
        private string _NowStatusRemark;
        private string _Time;
        private string _Name;



        public string StudentNum
        {
            get { return _StudentNum; }
            set { _StudentNum = value; }
        }

     

        public string InRoom
        {
            get { return _InRoom; }
            set { _InRoom = value; }
        }
        

        public string SeatNum
        {
            get { return _SeatNum; }
            set { _SeatNum = value; }
        }
       

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public string StatusStr
        {
            get
            {
                return Status == "Seating" ? "在座" :
                          Status == "Leave" ? "没有座位" :
                          Status == "Booking" ? "等待签到" :
                          Status == "Waiting" ? "正在等待座位" :
                          Status == "ShortLeave" ? "暂离中" : "";
            }
        }

       

        public string CanOperation
        {
            get { return _CanOperation; }
            set { _CanOperation = value; }
        }
       

        public string NowStatusRemark
        {
            get { return _NowStatusRemark; }
            set { _NowStatusRemark = value; }
        }

       

        public string Time
        {
            get { return _Time; }
            set { _Time = value; }
        }

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
            }
        }
    }
}
