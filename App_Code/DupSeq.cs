using System;
using System.Data;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for DupSeq.
	/// </summary>
	public class DupSeq
	{
		private Int32[] aDups;

		public DupSeq(DB db, string Sql)
		{
			DataTable dt = db.DataTable(Sql);

			aDups = new Int32[dt.Rows.Count];
			int iDups = 0;

			foreach (DataRow dr in dt.Rows)
			{
				aDups[iDups++] = DB.Int32(dr[0]);
			}
		}

		public bool In(Int32 Value)
		{
			bool fFound = false;

			for (int i = 0; i < aDups.Length; i++)
			{
				if (aDups[i] == Value)
				{
					fFound = true;
					break;
				}
			}

			return fFound;
		}
	}
}
