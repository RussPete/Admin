using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for DB.
	/// </summary>
	public class DB : IDisposable
	{
		public string sConn;
		public SqlConnection Conn;

		public DB()
		{
			//Debug.WriteLine("DB");
			WebConfig Cfg = new WebConfig();

			sConn
				= "server="				+ Cfg.Str("mcDBHost") 
				+ ";initial catalog="	+ Cfg.Str("mcDBName") 
				+ ";uid="				+ Cfg.Str("mcDBUser") 
				+ ";pwd="				+ Cfg.Str("mcDBPass");

			Conn = new SqlConnection(sConn);
			//Conn.Open();
			Open();
		}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DB() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public void Open()
		{
			//Debug.WriteLine("DB.Open " + sConn);
			if (Conn.State == ConnectionState.Closed)
			{
				//Debug.WriteLine("DB.Open Open Connection");
				Conn.Open();
			}
		}

		public void Close()
		{
			//Debug.WriteLine("DB.Close");
			if (Conn.State == ConnectionState.Open)
			{
				Conn.Close();
			}
		}

		public SqlDataReader Reader(string Sql)
		{
			Open();
			return new SqlCommand(Sql, Conn).ExecuteReader();
		}

		public static Int32 DBExec(string Sql)
		{
			DB db = new DB();
			db.Open();
			Int32 rc = new SqlCommand(Sql, db.Conn).ExecuteNonQuery();
			db.Close();

			return rc;
		}

        public static Int32 DBExec(string Sql, string Field, string Value)
        {
            DB db = new DB();
            Int32 rc = db.Exec(Sql, Field, Value);
            db.Close();

            return rc;
        }

        public Int32 Exec(string Sql)
		{
			Open();
			return new SqlCommand(Sql, Conn).ExecuteNonQuery();
		}

        public Int32 Exec(string Sql, string Field, string Value)
        {
            int Count = 0;

            Open();
            using (SqlCommand SqlCmd = new SqlCommand(Sql, Conn))
            {
                SqlCmd.Parameters.AddWithValue(Field, Value);
                Count = SqlCmd.ExecuteNonQuery();
            }

            return Count;
        }

        public static string DBSqlStr(string Sql)
		{
			DB db = new DB();
			db.Open();
			string Str = db.SqlStr(Sql);
			db.Close();

			return Str;
		}

		public string SqlStr(string Sql)
		{
			Open();
			return Str(new SqlCommand(Sql, Conn).ExecuteScalar());
		}

		public Int32 SqlNum(string Sql)
		{
			Open();
			return Int32(new SqlCommand(Sql, Conn).ExecuteScalar());
		}

        public Int32 SqlNum(string Sql, string Field, string Value)
        {
            Int32 Num = 0;

            using (SqlCommand SqlCmd = new SqlCommand(Sql, Conn))
            {
                SqlCmd.Parameters.AddWithValue(Field, Value);
                Num = Int32(SqlCmd.ExecuteScalar());
            }

            return Num;
        }

        public decimal SqlDec(string Sql)
		{
			Open();
			return Dec(new SqlCommand(Sql, Conn).ExecuteScalar());
		}

		public DateTime SqlDtTm(string Sql)
		{
			Open();
			return DB.DtTm(new SqlCommand(Sql, Conn).ExecuteScalar());
		}

		public bool SqlBool(string Sql)
		{
			Open();
			return Bool(new SqlCommand(Sql, Conn).ExecuteScalar());
		}

		public string[] StrArray(string Sql)
		{	
			string[] aStr = null;
			DataTable tbl = DataTable(Sql);
			aStr = new string[tbl.Rows.Count];
			int iStr = 0;
			foreach (DataRow dr in tbl.Rows)
			{
				aStr[iStr++] = Str(dr[0]);
			}

			return aStr;
		}

		public static string[] StrArray(DataTable dt, string Field)
		{
			string[] aStr = null;
			aStr = new string[dt.Rows.Count];
			int iStr = 0;
			foreach (DataRow dr in dt.Rows)
			{
				aStr[iStr++] = Str(dr[Field]);
			}

			return aStr;
		}

		public Int32 NextRno(string Table, string Field)
		{
			string Sql = 
				"Select Max(" + Field + ") From " + Table;

			return SqlNum(Sql) + 1;
		}

		public Int32 NextSeq(string Table, string RnoField, Int32 Rno, string Field)
		{
			string Sql =
				"Select Max(" + Field + ") From " + Table + " " +
				"Where " + RnoField + " = " + Rno;

			return SqlNum(Sql) + 1;
		}

		public static string Str(Object Obj)
		{
			string Value = "";

			if (Obj != DBNull.Value)
			{
				Value = Convert.ToString(Obj);
			}

			return Value;
		}

		public static Int32 Int32(Object Obj)
		{
			Int32 Value = 0;

			if (Obj != DBNull.Value)
			{
				Value = Convert.ToInt32(Obj);
			}

			return Value;
		}

		public static Decimal Dec(Object Obj)
		{
			Decimal Value = 0;

			if (Obj != DBNull.Value)
			{
				Value = Convert.ToDecimal(Obj);
			}

			return Value;
		}

		public static Boolean Bool(Object Obj)
		{
			Boolean Value = false;

			if (Obj != DBNull.Value)
			{
				Value = Convert.ToBoolean(Obj);
			}

			return Value;
		}

		public static DateTime DtTm(Object Obj)
		{
			DateTime Value = DateTime.MinValue;

			if (Obj != DBNull.Value)
			{
				Value = Convert.ToDateTime(Obj);
			}

			return Value;
		}

        public static string Put(string Str)
        {
            return PutStr(Str);
        }

		public static string PutStr(string Str)
		{
			if (Str == null)
			{
				Str = "Null";
			}
			else
			{
				Str = "'" + Str.Replace("'", "''") + "'";
			}

			return Str;
		}

        public static string Put(string Str, int MaxLen)
        {
            return PutStr(Str, MaxLen);
        }

        public static string PutStr(string Str, int MaxLen)
		{
			if (Str == null)
			{
				Str = "Null";
			}
			else
			{
				if (Str.Length > MaxLen)
				{
					Str = Str.Substring(0, MaxLen);
				}
				Str = "'" + Str.Replace("'", "''") + "'";
			}

			return Str;
		}

        public static string Put(DateTime DtTm)
        {
            return PutDtTm(DtTm);
        }

		public static string PutDtTm(DateTime DtTm)
		{
			string Str = "Null";

			if (DtTm != DateTime.MinValue)
			{
				Str = "'" + DtTm + "'";
			}

			return Str;
		}

		public static string PutDtTm(string sDate, string sTime)
		{
			string DtTm = "Null";

			if (sTime.Trim().Length > 0)
			{
				DateTime Tm = Utils.Str.DtTm(sDate + " " + sTime);
				DtTm = PutDtTm(Tm);
			}

			return DtTm;
		}

        public static string Put(Boolean fSet)
        {
            return PutBool(fSet);
        }

		public static string PutBool(Boolean fSet)
		{
			return (fSet ? "1" : "0");
		}

        public static string Put(Int32? Num)
        {
            return (Num.HasValue ? Num.Value.ToString() : "Null");
        }

		public static string Where(string Where)
		{
			return (Where == "" ? "" : "Where " + Where);
		}

		public static string And(string Curr)
		{
			return (Curr == "" ? "" : Curr + "And ");
		}

		public static string And(string Curr, string More)
		{
			return (More == "" ? Curr : And(Curr) + More + " ");
		}

        public static DataSet DBDataSet(string Sql)
        {
            DataSet ds = new DataSet();
            DB db = new DB();
            ds = db.DataSet(Sql);
            db.Close();

            return ds;
        }

        public DataSet DataSet(string Sql)
        {
            Open();
            DataSet ds = new DataSet();
            SqlDataAdapter Ad = new SqlDataAdapter(Sql, Conn);
            Ad.Fill(ds);
            Close();

            return ds;
        }

        public static DataTable DBDataTable(string Sql)
		{
			DataTable dt;
			DB db = new DB();
			dt = db.DataTable(Sql);
			db.Close();

			return dt;
		}

		public DataTable DataTable(string Sql)
		{
			Open();
			DataTable dt = new DataTable();
			SqlDataAdapter Ad = new SqlDataAdapter(Sql, Conn);
			Ad.Fill(dt);
			Close();

			return dt;
		}

		public DataTable DataTable(string Sql, int Timeout)
		{
			Open();
			DataTable dt = new DataTable();
			SqlCommand cmd = new SqlCommand(Sql, Conn);
			cmd.CommandTimeout = Timeout;
			SqlDataAdapter Ad = new SqlDataAdapter(cmd);
			Ad.Fill(dt);
			Close();

			return dt;
		}

        public DataTable DataTable(string Sql, string Field, string Value)
        {
            DataTable dt = null;

            Open();
            dt = new DataTable();
            using (SqlCommand SqlCmd = new SqlCommand(Sql, Conn))
            {
                SqlCmd.Parameters.AddWithValue(Field, Value);
                SqlDataAdapter Ad = new SqlDataAdapter(SqlCmd);
                Ad.Fill(dt);
            }
            Close();

            return dt;
        }
    

        public static DataRow DBDataRow(string Sql)
		{
			DataRow dr;
			DB db = new DB();
			dr = db.DataRow(Sql);
			db.Close();

			return dr;
		}

		public DataRow DataRow(string Sql)
		{
			DataRow dr = null;

			DataTable dt = DataTable(Sql);
			if (dt.Rows.Count > 0)
			{
				dr = dt.Rows[0];
			}

			return dr;
		}

        public DataRow DataRow(string Sql, string Field, string Value)
        {
            DataRow dr = null;

            DataTable dt = DataTable(Sql, Field, Value);
            if (dt.Rows.Count > 0)
            {
                dr = dt.Rows[0];
            }

            return dr;
        }
    }
}
