using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetEnterOutLog
    {
        private string _Id;

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        private string _EnterOutTime;

        public string EnterOutTime
        {
            get { return _EnterOutTime; }
            set { _EnterOutTime = value; }
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
        private string _EnterOutState;

        public string EnterOutState
        {
            get { return _EnterOutState; }
            set { _EnterOutState = value; }
        }

        public string EnterOutStateStr
        {
            get
            {
                return EnterOutState == "Leave" ? "读者离开" :
                          EnterOutState == "SelectSeat" ? "读者选座" :
                          EnterOutState == "BookingCancel" ? "取消预约" :
                          EnterOutState == "BookingConfirmation" ? "确认预约" :
                          EnterOutState == "ComeBack" ? "暂离回来" :
                          EnterOutState == "ContinuedTime" ? "续时" :
                          EnterOutState == "ReselectSeat" ? "重新选座" :
                          EnterOutState == "ShortLeave" ? "读者暂离中" :
                          EnterOutState == "Waiting" ? "等待座位" :
                          EnterOutState == "WaitingSuccess" ? "等待成功" :
                          EnterOutState == "WaitingCancel" ? "取消等待" :
                          EnterOutState == "BespeakWaiting" ? "预约等待" :
                          EnterOutState == "Timing" ? "正在计时" :
                          EnterOutState == "CancelTime" ? "取消计时" : "";
            }
        }

        private string _Remark;

        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
    }
}
