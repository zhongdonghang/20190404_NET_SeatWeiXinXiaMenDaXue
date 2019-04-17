using Dos.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Table("QRScanLog")]
    [Serializable]
    public partial class QRScanLog: Entity
    {

        public override string ToString()
        {

            return "ID:"+ID+"  SchoolNo:"+SchoolNo+"   Cardno:"+cardNo+"    ScanTime:"+scanTime+"     DeviceNo:"+deviceNo+"   Flag:"+flag+"";
        }

        private int iD;
        private string schoolNo;
        private string cardNo;
        private DateTime scanTime;
        private string deviceNo;
        private string flag;

        public int ID
        {
            get
            {
                return iD;
            }

            set
            {
               // this.OnPropertyValueChange(_AppDo, _ID, value);
                iD = value;
            }
        }

        public string SchoolNo
        {
            get
            {
                return schoolNo;
            }

            set
            {
                schoolNo = value;
            }
        }

        public string CardNo
        {
            get
            {
                return cardNo;
            }

            set
            {
                cardNo = value;
            }
        }

        public DateTime ScanTime
        {
            get
            {
                return scanTime;
            }

            set
            {
                scanTime = value;
            }
        }

        public string DeviceNo
        {
            get
            {
                return deviceNo;
            }

            set
            {
                deviceNo = value;
            }
        }

        public string Flag
        {
            get
            {
                return flag;
            }

            set
            {
                flag = value;
            }
        }
    }
}
