using System;
using Globals;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for Err.
	/// </summary>
	public class Err
	{
		private Exception Ex;
		private String SrcFile;
		private String SrcFunct;
		private Int32 SrcLine;
		private String Sql;

		public Err(Exception Ex)
		{
			this.Ex = Ex;
			this.Sql = null;
			ParseInfo();
			Save();
		}

		public Err(Exception Ex, String Sql)
		{
			this.Ex = Ex;
			this.Sql = Sql;
			ParseInfo();
			Save();
		}

		private void ParseInfo()
		{
			SrcFile = "";
			SrcFunct = "";
			SrcLine = 0;

			int iLine = Ex.StackTrace.LastIndexOf(":line ");
			if (iLine > -1)
			{
				String sLine = Ex.StackTrace.Substring(iLine + 6);
				SrcLine = Convert.ToInt32(Ex.StackTrace.Substring(iLine + 6));

				int iFile = Ex.StackTrace.LastIndexOf('\\', iLine);
				if (iFile > -1)
				{
					SrcFile = Ex.StackTrace.Substring(iFile + 1, iLine - iFile - 1);

					int iIn = Ex.StackTrace.LastIndexOf(" in ", iFile);
					if (iIn > -1)
					{
						int iPeriod = Ex.StackTrace.LastIndexOf('.', iIn);
						SrcFunct = Ex.StackTrace.Substring(iPeriod + 1, iIn - iPeriod - 1);
					}
				}
			}
		}

		private void Save()
		{
			DB db = new DB();

			String Sql = 
				"Insert Into mcErrors (DtTm, Message, SrcFile, SrcFunct, SrcLine, Sql, Stack) Values (" + 
				DB.PutDtTm(DateTime.Now) + ", " + 
				DB.PutStr(Ex.Message, 1024) + ", " + 
				DB.PutStr(SrcFile, 32) + ", " + 
				DB.PutStr(SrcFunct, 64) + ", " + 
				SrcLine + ", " +
				DB.PutStr(this.Sql, 4096) + ", " + 
				DB.PutStr(Ex.StackTrace, 4096) + ")";
			try
			{
				db.Exec(Sql);
			}
			catch (Exception Ex1)
			{
				Ex1.GetType();
			}
		}

		public String Html()
		{
			String Html = 
				String.Format(
					"<table border='0' cellspacing='0' cellpadding='0'>\n" + 
					"<tr><th valign='top' align='right'>{0}</th><td width='10'></td><td>{1}</td></tr>\n" + 
					"<tr><th valign='top' align='right'>{2}</th><td></td><td>{3}</td></tr>\n" + 
					"<tr><th valign='top' align='right'>{4}</th><td></td><td>{5}</td></tr>\n" + 
					"<tr><th valign='top' align='right'>{6}</th><td></td><td>{7}</td></tr>\n" + 
					"<tr><th valign='top' align='right'>{8}</th><td></td><td>{9}</td></tr>\n" + 
					"<tr><th valign='top' align='right'>{10}</th><td></td><td>{11}</td></tr>\n" + 
					"</table>\n",
					"Error",     Ex.ToString(), //.Message,
					"File",      SrcFile,
					"Line",      SrcLine,
					"Function",  SrcFunct,
					"Sql",       Sql,
					"Stack",     Ex.StackTrace.Replace("\r\n", "<br>"));

			return Html;
		}
	}
}
