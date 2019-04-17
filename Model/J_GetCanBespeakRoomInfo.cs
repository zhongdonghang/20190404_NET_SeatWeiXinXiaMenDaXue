using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetCanBespeakRoomInfo
    {
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
        private string _LibraryName;

        public string LibraryName
        {
            get { return _LibraryName; }
            set { _LibraryName = value; }
        }

        private string _ResidueSeat;

        public string ResidueSeat
        {
            get { return _ResidueSeat; }
            set { _ResidueSeat = value; }
        }


    }
}
