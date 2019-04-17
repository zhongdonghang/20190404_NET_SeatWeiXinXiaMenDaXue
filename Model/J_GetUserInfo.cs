using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class J_GetUserInfo
    {
        private string _CardId;

        public string CardId
        {
            get { return _CardId; }
            set { _CardId = value; }
        }
        private string _StudentNo;

        public string StudentNo
        {
            get { return _StudentNo; }
            set { _StudentNo = value; }
        }
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private string _Sex;

        public string Sex
        {
            get { return _Sex; }
            set { _Sex = value; }
        }
        private string _Department;

        public string Department
        {
            get { return _Department; }
            set { _Department = value; }
        }
        private string _ReaderType;

        public string ReaderType
        {
            get { return _ReaderType; }
            set { _ReaderType = value; }
        }
    }
}
