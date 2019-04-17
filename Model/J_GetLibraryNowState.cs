using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetLibraryNowState
    {
        private string _LibraryNo;

        public string LibraryNo
        {
            get { return _LibraryNo; }
            set { _LibraryNo = value; }
        }
        private string _LibraryName;

        public string LibraryName
        {
            get { return _LibraryName; }
            set { _LibraryName = value; }
        }
        private int _AllSeats;

        public int AllSeats
        {
            get { return _AllSeats; }
            set { _AllSeats = value; }
        }
        private int _AllUsed;

        public int AllUsed
        {
            get { return _AllUsed; }
            set { _AllUsed = value; }
        }
        private int _AllBooked;

        public int AllBooked
        {
            get { return _AllBooked; }
            set { _AllBooked = value; }
        }
        private int _UsedPercentage;

        public int UsedPercentage
        {
            get { return _UsedPercentage; }
            set { _UsedPercentage = value; }
        }
        private List<J_RoomStatus> _RoomStatus;

        public List<J_RoomStatus> RoomStatus
        {
            get { return _RoomStatus; }
            set { _RoomStatus = value; }
        }

        public string Percent
        {
            get
            {
                double db = (((double)AllUsed / (double)AllSeats) * 100);
                if (db > 0 && db < 1)
                {
                    return "1";
                }
                return ((int)db).ToString();
            }
        }
    }

    public class J_RoomStatus
    {

        public string SeatAmount_Used1
        {
            get {
                int result = SeatAmount_All - SeatAmount_Last;
                return result.ToString();
            }
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
        private int _SeatAmount_All;

        public int SeatAmount_All
        {
            get { return _SeatAmount_All; }
            set { _SeatAmount_All = value; }
        }
        private int _SeatAmount_Used;

        public int SeatAmount_Used
        {
            get { return _SeatAmount_Used; }
            set { _SeatAmount_Used = value; }
        }
        private int _SeatAmount_Bespeak;

        public int SeatAmount_Bespeak
        {
            get { return _SeatAmount_Bespeak; }
            set { _SeatAmount_Bespeak = value; }
        }
        private string _OpenCloseState;

        public string OpenCloseState
        {
            get { return _OpenCloseState; }
            set { _OpenCloseState = value; }
        }
        private int _SeatAmount_Last;

        public int SeatAmount_Last
        {
            get { return _SeatAmount_Last; }
            set { _SeatAmount_Last = value; }
        }

        private bool _IsCanBookNowSeat;

        public bool IsCanBookNowSeat
        {
            get { return _IsCanBookNowSeat; }
            set { _IsCanBookNowSeat = value; }
        }

        public string Percent
        {
            get
            {
                double db = (((double)SeatAmount_Used / (double)SeatAmount_All) * 100);
                if (db > 0 && db < 1)
                {
                    return "1";
                }
                return ((int)db).ToString();
            }
        }
    }
}
