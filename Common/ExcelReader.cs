using System;
using System.Data;
using System.Data.OleDb;

namespace Common
{
	/// <summary>
	/// 读取Excel文件，并转换成DataTable对象。
	/// </summary>
	public class ExcelReader : IDisposable
	{
		protected OleDbConnection _conn;
		protected OleDbDataAdapter _da;
		protected DataTable _dt;
		protected string _connString;
		protected string _sql;

		/// <summary>
		/// 构造函数，初始化数据库连接对象；
		/// </summary>
		public ExcelReader()
		{
			_conn = new OleDbConnection();
			_connString = "";
			_sql = "";
		}

		public ExcelReader( string connString )
		{
			_conn = new OleDbConnection( );
			_connString = connString;
			_sql = "";
		}

		/// <summary>
		/// 设置Excel文件的所在路径。
		/// </summary>
		public string Path
		{
			set
			{
				_connString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + value + "; Extended Properties=Excel 8.0;";
			}
		}

		/// <summary>
		/// 设置Excel文件中的表的名称，一般为[sheet1$]
		/// </summary>
		public string TableName
		{
			set{ _sql = "select * from [" + value + "$]"; }
		}

		/// <summary>
		/// 打开Excel文件。
		/// </summary>
		public void OpenExcel()
		{
			if( _connString == "" ) throw new Exception("未设置文件路径。");
			else if( _sql == "" ) throw new Exception("未设置Excel表格名称。");
			else
			{
				_conn.ConnectionString = _connString;
				_conn.Open();
			}
		}

		/// <summary>
		/// 获取Excel表格转换后的DataTable对象。
		/// </summary>
		public DataTable GetTable()
		{
			_da = new OleDbDataAdapter( _sql, _conn );
			_dt = new DataTable( "Excel" );
			_da.Fill( _dt );
            _conn.Close();
			return _dt;
		}

		/// <summary>
		/// 销毁对象。
		/// </summary>
		public void Dispose()
		{
			if( _dt != null ) _dt.Dispose();
			if( _da != null ) _da.Dispose();
			if( _conn != null ) _conn.Dispose();
			GC.SuppressFinalize( this );
		}
	}
}
