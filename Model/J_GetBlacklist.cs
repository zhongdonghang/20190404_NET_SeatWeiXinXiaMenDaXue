using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetBlacklist
    {
        private int _ID;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        private string _StudentNo;

        public string StudentNo
        {
            get { return _StudentNo; }
            set { _StudentNo = value; }
        }
        private string _AddTime;

        public string AddTime
        {
            get { return _AddTime; }
            set { _AddTime = value; }
        }
        private string _OutTime;

        public string OutTime
        {
            get { return _OutTime; }
            set { _OutTime = value; }
        }
        private string _ReMark;

        public string ReMark
        {
            get { return _ReMark; }
            set { _ReMark = value; }
        }
        private bool _IsValid;

        public bool IsValid
        {
            get { return _IsValid; }
            set { _IsValid = value; }
        }
        private string _OutBlacklistMode;

        public string OutBlacklistMode
        {
            get { return _OutBlacklistMode; }
            set { _OutBlacklistMode = value; }
        }

        public string OutBlacklistModeStr
        {
            get
            {
                return OutBlacklistMode == "AutomaticMode" ? "自动离开" :
                          OutBlacklistMode == "ManuallyMode" ? "手动操作" : "";
            }
        }
    }
}
