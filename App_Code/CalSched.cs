using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Create HTML for a weekly or monthly calendar view of a schedule of events on a web page.
	/// </summary>
	public class CalSched
	{
		#region Class fields

		protected DateTime dtBeg;
		protected DateTime dtEnd;
		protected DateTime tmBegDay;
		protected DateTime tmEndDay;
		protected int BlockMinutes;
		protected bool fShowTimeColumn = true;
		protected DataFormater fnFormatData;
		protected CalEvent fnBegCal;
		protected CalEvent fnEndCal;
		protected DataTable tblData;
		protected SqlConnection cnSql;
		protected string sSqlFormat;
		protected string sBegField;
		protected string sEndField;
		protected string sSql;
		protected bool fNewPage;

		#endregion

		#region Class definitions

		public enum ViewPeriod
		{
			Week,
			Month
		}

		public delegate string DataFormater(DataRow dr);
		public delegate string CalEvent();

		#endregion

		/// <summary>
		/// Create a new Calendar Schedule using today as the beginning and 
		/// ending date of the period.
		/// </summary>
		public CalSched()
		{
			dtBeg = 
			dtEnd = DateTime.Today;
		}

		/// <summary>
		/// Create a new Calendar Schedule using the given date as the beginning and 
		/// ending date of the period.
		/// </summary>
		public CalSched(DateTime dt)
		{
			dtBeg = 
			dtEnd = dt;
		}

		/// <summary>
		/// Create a new Calendar Schedule using the given Beg and End date as the 
		/// beginning and ending date of the period.
		/// </summary>
		public CalSched(DateTime dtBeg, DateTime dtEnd)
		{
			this.dtBeg = dtBeg;
			this.dtEnd = dtEnd;
		}

		/// <summary>
		/// Time of day the weekly calendar begins showing detailed items.
		/// </summary>
		public void BegTimeStr(string Time)
		{
			tmBegDay = Convert.ToDateTime(Time);
		}

		/// <summary>
		/// Time of day the weekly calendar ends showing detailed items.
		/// </summary>
		public void EndTimeStr(string Time)
		{
			tmEndDay = Convert.ToDateTime(Time);
		}

		/// <summary>
		/// Time of day the weekly calendar begins showing detailed items.
		/// </summary>
		public DateTime BegTime
		{
			get
			{
				return tmBegDay;
			}
			set
			{
				tmBegDay = value;
			}
		}

		/// <summary>
		/// Time of day the weekly calendar ends showing detailed items.
		/// </summary>
		public DateTime EndTime
		{
			get
			{
				return tmEndDay;
			}
			set
			{
				tmEndDay = value;
			}
		}

		/// <summary>
		/// Length of time in minutes for each time block during a day in the weekly calendar.
		/// </summary>
		public int TimeBlock
		{
			get
			{
				return BlockMinutes;
			}
			set
			{
				BlockMinutes = value;
			}
		}

		/// <summary>
		/// Show the Time column on the weekly calendar.
		/// </summary>
		public bool ShowTimeColumn
		{
			get
			{
				return fShowTimeColumn;
			}
			set
			{
				fShowTimeColumn = value;
			}
		}

		/// <summary>
		/// SQL DataTable populated with calendar events, sorted by BegField, EndField.
		/// </summary>
		public DataTable Table
		{
			get
			{
				return tblData;
			}
			set
			{
				tblData = value;
			}
		}

		/// <summary>
		/// Connection to the SQL database from which calendar events will be extracted 
		/// using the SqlQuery.
		/// </summary>
		public SqlConnection DBConnection
		{
			get
			{
				return cnSql;
			}
			set
			{
				cnSql = value;
			}
		}

		/// <summary>
		/// A call back function used to format the data for each event on the calendar.
		/// "string DataFormater(DataRow dr)"
		/// </summary>
		public DataFormater FormatData
		{
			get
			{
				return fnFormatData;
			}
			set
			{
				fnFormatData = value;
			}
		}

		/// <summary>
		/// A call back function - called at the beginning of each calendar.
		/// "string CalEvent()"
		/// </summary>
		public CalEvent BegCal
		{
			get
			{
				return fnBegCal;
			}
			set
			{
				fnBegCal = value;
			}
		}

		/// <summary>
		/// A call back function - called at the ending of each calendar.
		/// "string CalEvent()"
		/// </summary>
		public CalEvent EndCal
		{
			get
			{
				return fnEndCal;
			}
			set
			{
				fnEndCal = value;
			}
		}

		/// <summary>
		/// Format string for building a SQL query. 
		/// SqlFormat has two parameters for the start and end date of a period displayed on the calendar.
		/// </summary>
		public void SqlDataQuery(string SqlFormat, string BegField, string EndField)
		{
			sSqlFormat	= SqlFormat;
			sBegField	= BegField;
			sEndField	= EndField;
		}
			
		/// <summary>
		/// Format string for building a SQL query. 
		/// Has two parameters for the start and end date of a period displayed on the calendar.
		/// </summary>
		public string SqlFormat
		{
			get
			{
				return sSqlFormat;
			}
			set
			{
				sSqlFormat = value;
			}
		}

		/// <summary>
		/// Name of the beginning datetime field in the SQL query.
		/// </summary>
		public string BegField
		{
			get
			{
				return sBegField;
			}
			set
			{
				sBegField = value;
			}
		}

		/// <summary>
		/// Name of the ending datetime field in the SQL query.
		/// </summary>
		public string EndField
		{
			get
			{
				return sBegField;
			}
			set
			{
				sBegField = value;
			}
		}

		/// <summary>
		/// SQL query used to extract data from the database. Usefull for reporting errors.
		/// </summary>
		public string SqlQuery
		{
			get
			{
				return sSql;
			}
		}

		/// <summary>
		/// Create the HTML to view the schedule on a calendar in a weekly or monthly format.
		/// </summary>
		public void View(HttpResponse Response, ViewPeriod Period)
		{
			switch (Period)
			{
				case ViewPeriod.Week:
					ViewWeek(Response);
					break;

				case ViewPeriod.Month:
					ViewMonths(Response);
					break;
			}
		}

		protected void ViewWeek(HttpResponse Response)
		{
			GetData();

			string Html = "";

			if (fnBegCal != null)
			{
				Html += fnBegCal();
			}

			DateTime dtBegOfWeek = dtBeg;
			Html =
				WebPage.Tabs(0) + WebPage.Table("align='center' class='CalSched'") +
				WebPage.Tabs(1) + "<tr>\n";

			if (fShowTimeColumn)
			{
				Html += WebPage.Tabs(2) + "<td align='center'>Time</td>\n";
			}

			// days of the week
			DateTime dtDay = dtBeg;
			while (dtDay <= dtEnd)
			{
				Html += 
					WebPage.Tabs(2) + "<td align='center'><b>" + dtDay.ToString("MM/dd<br>ddd") + "</b></td>\n";
				dtDay = dtDay.AddDays(1);
			}
			Html +=
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.Tabs(1) + "<tr>\n";

			// min width for column
			dtDay = dtBeg;
			while (dtDay <= dtEnd)
			{
				Html += WebPage.Tabs(2) + "<td class='NoBorder'>" + WebPage.NoOp(80, 1) + "</td>\n";
				dtDay = dtDay.AddDays(1);
			}
			Html +=
				WebPage.Tabs(1) + "</tr>\n";

			DateTime tmBegBlockWeek = new DateTime(dtBegOfWeek.Year, dtBegOfWeek.Month, dtBegOfWeek.Day);
			DateTime tmEndBlockWeek	= new DateTime(dtBegOfWeek.Year, dtBegOfWeek.Month, dtBegOfWeek.Day, tmBegDay.Hour, tmBegDay.Minute, 0);
			DateTime tmEndDayWeek	= new DateTime(dtBegOfWeek.Year, dtBegOfWeek.Month, dtBegOfWeek.Day, tmEndDay.Hour, tmEndDay.Minute, 0);
			TimeSpan tsBlock		= new TimeSpan(0, this.TimeBlock, 0);
			string sTime = "Before<br>" + tmEndBlockWeek.ToString("h:mmt").ToLower();
			string sClass = "CalSched";

			bool fLastBlock = false;
			while (!fLastBlock)
			{
				fLastBlock = (tmEndBlockWeek.Hour == 0);		// last block of time will end at 00:00 of the next day

				Html += WebPage.Tabs(1) + "<tr>\n";
				if (fShowTimeColumn)
				{
					Html += WebPage.Tabs(2) + "<td align='center' valign='top' class='" + sClass + "'>" + sTime + "</td>\n";
				}

				dtDay = dtBeg;
				while (dtDay <= dtEnd)
				{
					dtDay	= new DateTime(dtDay.Year, dtDay.Month, dtDay.Day, 0, 0, 0);
					DateTime tmBegBlockDay	= dtDay; 
					DateTime tmEndBlockDay	= (tmBegBlockWeek.Day == tmEndBlockWeek.Day ? dtDay : dtDay.AddDays(1));
					tmBegBlockDay	= new DateTime(tmBegBlockDay.Year, tmBegBlockDay.Month, tmBegBlockDay.Day, tmBegBlockWeek.Hour, tmBegBlockWeek.Minute, 0);
					tmEndBlockDay	= new DateTime(tmEndBlockDay.Year, tmEndBlockDay.Month, tmEndBlockDay.Day, tmEndBlockWeek.Hour, tmEndBlockWeek.Minute, 0);

					string sEvent = "";
					int iEvent = FirstEvent(tmBegBlockDay, tmEndBlockDay);
					if (iEvent > -1)
					{
						while (iEvent > -1)
						{
							sEvent += 
								(sEvent.Length > 0 ? WebPage.SpaceTable(1, 2) : "") +
								FormatEvent(iEvent);

							iEvent = NextEvent(iEvent, tmBegBlockDay, tmEndBlockDay);
						}
					}
					else
					{
						sEvent = WebPage.Tabs(3) + "&nbsp;\n";
					}

					Html += 
						WebPage.Tabs(2) + "<td class='" + sClass + "' valign='top'>\n" + 
						sEvent + 
						WebPage.Tabs(2) + "</td>\n";

					dtDay = dtDay.AddDays(1);
				}
				Html +=
					WebPage.Tabs(1) + "</tr>\n";

				sClass = "CalSchedMid";

				// 2 cases exist here, 
				// the last block of time at the end of the normal day (tmEndDay)
				// or anytime before that

				if (tmEndBlockWeek >= tmEndDayWeek)
				{
					// last block of time for the day (after the End of Day time)
					tmBegBlockWeek = tmEndDayWeek;
					tmEndBlockWeek = new DateTime(dtBegOfWeek.Year, dtBegOfWeek.Month, dtBegOfWeek.Day).AddDays(1);
					sTime = tmBegBlockWeek.ToString("h:mmt").ToLower() + "<br>&amp; After";
				}
				else
				{
					tmBegBlockWeek = tmEndBlockWeek;
					tmEndBlockWeek += tsBlock;
					sTime = tmBegBlockWeek.ToString("h:mmt").ToLower();
				}
				Debug.WriteLine("BegBlockWeek " + tmBegBlockWeek.ToString());
				Debug.WriteLine("EndBlockWeek " + tmEndBlockWeek.ToString());
			}

			Html +=
				WebPage.Tabs(0) + WebPage.TableEnd();

			if (fnEndCal != null)
			{
				Html += fnEndCal();
			}

			Response.Write(Html);
		}

		protected void ViewMonths(HttpResponse Response)
		{
			fNewPage = false;

			DateTime dtBegMonth = dtBeg;
			while (dtBegMonth < dtEnd)
			{
				DateTime dtEndMonth = dtBegMonth.AddMonths(1);
				dtEndMonth = dtEndMonth.AddDays(-(int)dtEndMonth.Day);

				ViewMonth(Response, dtBegMonth, dtEndMonth);

				dtBegMonth = dtEndMonth.AddDays(1);

				fNewPage = true;
			}
		}

		protected void ViewMonth(HttpResponse Response, DateTime dtBegMonth, DateTime dtEndMonth)
		{
			GetData();

			DateTime dtBegOfWeek	= dtBegMonth.AddDays(1 - (int)dtBegMonth.Day);
			dtBegOfWeek				= dtBegOfWeek.AddDays(-(int)dtBegOfWeek.DayOfWeek);
			DateTime dtEndOfWeek	= dtBegOfWeek.AddDays(6);

			string Html = "";

			if (fnBegCal != null)
			{
				Html += fnBegCal();
			}

			// Month top
			Html +=
				(fNewPage ? "<div style='page-break-before: always;'>&nbsp;</div>\n" : "") +
				WebPage.Tabs(0) + WebPage.Table("align='center' valign='top' class='CalSched'") +
				WebPage.Tabs(1) + "<tr><td colspan='" +(fShowTimeColumn ? 8 : 7) + "' align='center' class='RptNotice'>" + 
				dtBegMonth.ToString("MMMM yyyy") + "</td></tr>\n" +
				WebPage.Tabs(1) + "<tr>\n" +
				(fShowTimeColumn ? WebPage.Tabs(2) + "<td align='center'>Time</td>\n" : "");

			// days of the week
			DateTime dtDay = dtBegOfWeek;
			while (dtDay <= dtEndOfWeek)
			{
				Html += 
					WebPage.Tabs(2) + "<td align='center'><b>" + dtDay.ToString("ddd") + "</b></td>\n";
				dtDay = dtDay.AddDays(1);
			}
			Html +=
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.Tabs(1) + "<tr>\n";

			// min width for column
			dtDay = dtBegOfWeek;
			while (dtDay <= dtEndOfWeek)
			{
				Html += WebPage.Tabs(2) + "<td class='NoBorder'>" + WebPage.NoOp(80, 1) + "</td>\n";
				dtDay = dtDay.AddDays(1);
			}
			Html +=
				WebPage.Tabs(1) + "</tr>\n";

			while (dtBegOfWeek < dtEndMonth)
			{
				DateTime tmBegBlockWeek = new DateTime(dtBegOfWeek.Year, dtBegOfWeek.Month, dtBegOfWeek.Day);
				DateTime tmEndBlockWeek	= new DateTime(dtBegOfWeek.Year, dtBegOfWeek.Month, dtBegOfWeek.Day, tmBegDay.Hour, tmBegDay.Minute, 0);
				DateTime tmEndDayWeek	= new DateTime(dtBegOfWeek.Year, dtBegOfWeek.Month, dtBegOfWeek.Day, tmEndDay.Hour, tmEndDay.Minute, 0);
				TimeSpan tsBlock		= new TimeSpan(0, this.TimeBlock, 0);
				string sTime = "Before<br>" + tmEndBlockWeek.ToString("h:mmt").ToLower();
				string sClass = "CalSched";

				bool fFirstBlock = true;
				bool fLastBlock = false;
				while (!fLastBlock)
				{
					fLastBlock = (tmEndBlockWeek.Hour == 0);		// last block of time will end at 00:00 of the next day

					Html += WebPage.Tabs(1) + "<tr>\n";
					if (fShowTimeColumn)
					{
						Html += WebPage.Tabs(2) + "<td align='center' valign='top' class='" + sClass + "'>" + sTime + "</td>\n";
					}

					dtDay = dtBegOfWeek;
					while (dtDay <= dtEndOfWeek)
					{
						dtDay	= new DateTime(dtDay.Year, dtDay.Month, dtDay.Day, 0, 0, 0);
						DateTime tmBegBlockDay	= dtDay; 
						DateTime tmEndBlockDay	= (tmBegBlockWeek.Day == tmEndBlockWeek.Day ? dtDay : dtDay.AddDays(1));
						tmBegBlockDay	= new DateTime(tmBegBlockDay.Year, tmBegBlockDay.Month, tmBegBlockDay.Day, tmBegBlockWeek.Hour, tmBegBlockWeek.Minute, 0);
						tmEndBlockDay	= new DateTime(tmEndBlockDay.Year, tmEndBlockDay.Month, tmEndBlockDay.Day, tmEndBlockWeek.Hour, tmEndBlockWeek.Minute, 0);

						string sInRange = "";
						string sEvent =
							(fFirstBlock ? WebPage.Tabs(3) + "<div align='right'>" + dtDay.Day + "</div>\n" : "");

						if (dtDay.Month == dtBegMonth.Month && dtDay >= dtBeg && dtDay <= dtEnd)
						{
							int iEvent = FirstEvent(tmBegBlockDay, tmEndBlockDay);
							if (iEvent > -1)
							{
								while (iEvent > -1)
								{
									sEvent += 
										(sEvent.Length > 0 ? WebPage.SpaceTable(1, 2) : "") +
										FormatEvent(iEvent);

									iEvent = NextEvent(iEvent, tmBegBlockDay, tmEndBlockDay);
								}
							}
							else
							{
								sEvent += WebPage.Tabs(3) + "&nbsp;\n";
							}
						}
						else
						{
							sInRange = "Not";
							sEvent += WebPage.Tabs(3) + "&nbsp;\n";
						}

						Html += 
							WebPage.Tabs(2) + "<td class='" + sClass + sInRange + "' valign='top'>\n" + 
							sEvent + 
							WebPage.Tabs(2) + "</td>\n";

						dtDay = dtDay.AddDays(1);
					}
					Html +=
						WebPage.Tabs(1) + "</tr>\n";

					sClass = "CalSchedMid";
					fFirstBlock = false;

					// 2 cases exist here, 
					// the last block of time at the end of the normal day (tmEndDay)
					// or anytime before that

					if (tmEndBlockWeek >= tmEndDayWeek)
					{
						// last block of time for the day (after the End of Day time)
						tmBegBlockWeek = tmEndDayWeek;
						tmEndBlockWeek = new DateTime(dtBegOfWeek.Year, dtBegOfWeek.Month, dtBegOfWeek.Day).AddDays(1);
						sTime = tmBegBlockWeek.ToString("h:mmt").ToLower() + "<br>&amp; After";
					}
					else
					{
						tmBegBlockWeek = tmEndBlockWeek;
						tmEndBlockWeek += tsBlock;
						sTime = tmBegBlockWeek.ToString("h:mmt").ToLower();
					}
					Debug.WriteLine("BegBlockWeek " + tmBegBlockWeek.ToString());
					Debug.WriteLine("EndBlockWeek " + tmEndBlockWeek.ToString());
				}
				dtBegOfWeek	= dtEndOfWeek.AddDays(1);
				dtEndOfWeek	= dtBegOfWeek.AddDays(6);
			}

			Html +=
				WebPage.Tabs(0) + WebPage.TableEnd();

			if (fnEndCal != null)
			{
				Html += fnEndCal();
			}

			Response.Write(Html);
		}


		protected void GetData()
		{
			if (sSqlFormat != null)
			{
				bool fOpenned = false;

				if (cnSql.State == ConnectionState.Closed)
				{
					fOpenned = true;

					cnSql.Open();
				}

				sSql = String.Format(sSqlFormat, dtBeg, dtEnd);

				tblData = new DataTable();
				SqlDataAdapter Ad = new SqlDataAdapter(sSql, cnSql);
				Ad.Fill(tblData);

				if (fOpenned)
				{
					cnSql.Close();
				}
			}
		}

		protected int FirstEvent(DateTime dtBegBlock, DateTime dtEndBlock)
		{
			return NextEvent(-1, dtBegBlock, dtEndBlock);
		}

		protected int NextEvent(int iEvent, DateTime dtBegBlock, DateTime dtEndBlock)
		{
			Debug.WriteLine("NextEvent Beg " + dtBegBlock.ToString());
			Debug.WriteLine("NextEvent End " + dtEndBlock.ToString());
			int iNext = -1;

			for (int i = iEvent + 1; i < tblData.Rows.Count; i++)
			{
				DateTime dtBegTime = BegEventTime(i);
				Debug.WriteLine(i + " " + dtBegTime.ToString());
				if (dtBegTime >= dtBegBlock &&
					dtBegTime <  dtEndBlock)
				{
					iNext = i;
					break;
				}
			}

			return iNext;
		}

		protected DateTime BegEventTime(int iRow)
		{
			DataRow dr = tblData.Rows[iRow];
			return dbDtTm(sBegField == null ? dr[0] : dr[sBegField]);
		}

		protected string dbStr(Object Obj)
		{
			string Value = "";

			if (Obj != DBNull.Value)
			{
				Value = Convert.ToString(Obj);
			}

			return Value;
		}

		protected DateTime dbDtTm(Object Obj)
		{
			DateTime Value = DateTime.MinValue;

			if (Obj != DBNull.Value)
			{
				Value = Convert.ToDateTime(Obj);
			}

			return Value;
		}

		protected string FormatEvent(int iRow)
		{
			string Html;
			DataRow dr = tblData.Rows[iRow];

			if (fnFormatData != null)
			{
				Html = fnFormatData(tblData.Rows[iRow]);
			}
			else
			{
				Html = "<b>" + dbStr(dr[0]) + "</b><br>";
				for (int i = 1; i < tblData.Columns.Count; i++)
				{
					Html += "&nbsp;&nbsp;" + dbStr(dr[i]) + "<br>";
				}
			}

			return Html;
		}
	}
}
