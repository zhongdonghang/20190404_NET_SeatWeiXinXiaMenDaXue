using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace WeiXinMsgService
{
    public  class SqlTools
    {
        public static string GetOpenId(string studentNo,string schoolNo )
        {
            string result = string.Empty;
            using (SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DosConn"].ToString()))
            {
                cn.Open();
                string sql = "select * from  [dbo].[tb_User] where SchoolNo = '" + schoolNo + "' and StudentNo ='" + studentNo + "'";
                SqlCommand cmd = new SqlCommand(sql, cn);
                Object obj = cmd.ExecuteScalar();
                result = obj == null ? "" : obj.ToString();
                cn.Close();
                cn.Dispose();
            }
            return result;
        }
    }
}
