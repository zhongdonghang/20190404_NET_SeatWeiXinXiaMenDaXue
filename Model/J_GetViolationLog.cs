using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetViolationLog
    {
        private string _StudentNo;

        public string StudentNo
        {
            get { return _StudentNo; }
            set { _StudentNo = value; }
        }
        private string _SeatNo;

        public string SeatNo
        {
            get { return _SeatNo; }
            set { _SeatNo = value; }
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
        private string _EnterOutTime;

        public string EnterOutTime
        {
            get { return _EnterOutTime; }
            set { _EnterOutTime = value; }
        }
        private string _Remark;

        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
    }
}
