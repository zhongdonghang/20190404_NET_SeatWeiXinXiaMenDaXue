using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
   public class J_UserRecord
    {
        private List<J_GetEnterOutLog> enterOutLogList;

        public List<J_GetEnterOutLog> EnterOutLogList
        {
            get
            {
                return enterOutLogList;
            }

            set
            {
                enterOutLogList = value;
            }
        }

        private List<J_GetViolationLog> violationLogList;
        public List<J_GetViolationLog> ViolationLogList
        {
            get
            {
                return violationLogList;
            }

            set
            {
                violationLogList = value;
            }
        }

        private List<J_GetBlacklist> blackList;
        public List<J_GetBlacklist> BlackList
        {
            get
            {
                return blackList;
            }

            set
            {
                blackList = value;
            }
        }

        public List<J_GetBesapsekLog> BesapsekLogList
        {
            get
            {
                return besapsekLogList;
            }

            set
            {
                besapsekLogList = value;
            }
        }

        private List<J_GetBesapsekLog> besapsekLogList;

    }
}
