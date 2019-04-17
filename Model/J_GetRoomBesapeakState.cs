using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetRoomBesapeakState
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
    }
}
