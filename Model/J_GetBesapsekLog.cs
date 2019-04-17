using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetBesapsekLog
    {
        private string _Id;

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        private string _BookTime;

        public string BookTime
        {
            get { return _BookTime; }
            set { _BookTime = value; }
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
        private string _SubmitDateTime;

        public string SubmitDateTime
        {
            get { return _SubmitDateTime; }
            set { _SubmitDateTime = value; }
        }
        private bool _IsValid;

        public bool IsValid
        {
            get { return _IsValid; }
            set { _IsValid = value; }
        }
        private string _Remark;

        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        private string _Operator;

        public string Operator
        {
            get { return _Operator; }
            set { _Operator = value; }
        }
        private string _SeatShortNo;

        public string SeatShortNo
        {
            get { return _SeatShortNo; }
            set { _SeatShortNo = value; }
        }
    }
}
