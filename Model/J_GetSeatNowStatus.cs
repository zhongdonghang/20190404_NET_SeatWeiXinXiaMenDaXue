using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetSeatNowStatus
    {
        private string _SeatNo;

        public string SeatNo
        {
            get { return _SeatNo; }
            set { _SeatNo = value; }
        }
        private string _SeatShortNo;

        public string SeatShortNo
        {
            get { return _SeatShortNo; }
            set { _SeatShortNo = value; }
        }
        private string _RoomNo;

        public string RoomNo
        {
            get { return _RoomNo; }
            set { _RoomNo = value; }
        }
        private string _RoomName;

        public string RoomName
        {
            get { return _RoomName; }
            set { _RoomName = value; }
        }
        private string _Status;

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        private string _CanOperation;

        public string CanOperation
        {
            get { return _CanOperation; }
            set { _CanOperation = value; }
        }

        private List<String> _CanBookingDate;

        public List<String> CanBookingDate
        {
            get { return _CanBookingDate; }
            set { _CanBookingDate = value; }
        }

        public string StatusStr
        {
            get
            {
                return Status == "Seating" ? "在座" :
                          Status == "Leave" ? "空闲" :
                          Status == "Booking" ? "有预约" :
                          Status == "Waiting" ? "正在等待座位" :
                          Status == "ShortLeave" ? "暂离中" :
                          Status == "StopUsed" ? "停用" : "";
            }
        }

    }
}
