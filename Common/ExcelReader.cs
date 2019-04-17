using System;
using System.Data;
using System.Data.OleDb;

namespace Common
{
	/// <summary>
	/// ��ȡExcel�ļ�����ת����DataTable����
	/// </summary>
	public class ExcelReader : IDisposable
	{
		protected OleDbConnection _conn;
		protected OleDbDataAdapter _da;
		protected DataTable _dt;
		protected string _connString;
		protected string _sql;

		/// <summary>
		/// ���캯������ʼ�����ݿ����Ӷ���
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
		/// ����Excel�ļ�������·����
		/// </summary>
		public string Path
		{
			set
			{
				_connString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + value + "; Extended Properties=Excel 8.0;";
			}
		}

		/// <summary>
		/// ����Excel�ļ��еı�����ƣ�һ��Ϊ[sheet1$]
		/// </summary>
		public string TableName
		{
			set{ _sql = "select * from [" + value + "$]"; }
		}

		/// <summary>
		/// ��Excel�ļ���
		/// </summary>
		public void OpenExcel()
		{
			if( _connString == "" ) throw new Exception("δ�����ļ�·����");
			else if( _sql == "" ) throw new Exception("δ����Excel������ơ�");
			else
			{
				_conn.ConnectionString = _connString;
				_conn.Open();
			}
		}

		/// <summary>
		/// ��ȡExcel���ת�����DataTable����
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
		/// ���ٶ���
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
