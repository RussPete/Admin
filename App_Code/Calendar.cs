using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Collections.Specialized;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

namespace Utils
{
	/// <summary>
	/// Summary description for SelectList.
	/// </summary>
	public class Calendar
	{
		private DateTime dtDate;
		private string sName;
		private TextBox txtBox;
		private Image imgButton;
		private string sHideControls;

		public Calendar(
			string Name, 
			ref TextBox txtBox)
		{
			this.dtDate		= DateTime.Today;
			this.sName		= Name;
			this.txtBox		= txtBox;
		}

		public Calendar(
			DateTime Date,
			string Name, 
			ref TextBox txtBox)
		{
			this.dtDate = Date;
			this.sName  = Name;
			this.txtBox = txtBox;
		}

		public DateTime Date
		{
			get
			{
				return dtDate;
			}
			set
			{
				dtDate = value;
			}
		}

		public string Name
		{
			get
			{
				return sName;
			}
			set
			{
				sName = value;
			}
		}

		public string HideControlsList
		{
			get
			{
				return sHideControls;
			}
			set
			{
				sHideControls = value;
			}
		}

		public void ImageButton(Image imgButton)
		{
			this.imgButton = imgButton;
			imgButton.Attributes.Add("onClick", "cal" + sName + ".ButtonClicked();");
		}

		private void AddEvents()
		{
			txtBox.Attributes.Add("onFocus", "cal" + sName + ".OnFocus();");
			txtBox.Attributes.Add("onBlur", "cal" + sName + ".OnBlur();");	//"cal" + sName + ".Hide();");	//"setTimeout('cal" + sName + ".Hide();', 200);");
		}

		public string JavaScript()
		{
			AddEvents();

			string OnChange = txtBox.Attributes["onChange"];

			String Html = 
				"<script language='javascript'>\n" +
				"\n" +
				"var cal" + this.sName + " = new Calendar(" + 
				"'" + dtDate + "', " + 
				"'" + sName + "', " +
				"'" + txtBox.ID + "', " +
				(OnChange != null ? "\"" + OnChange + "\"" : "null") + ", " +
				(sHideControls != null ? "'" + sHideControls + "'" : "null") + " " + 
				");\n" +
				"\n" + 
				"</script>\n";

			return Html;
		}

		public string DefCalendar()
		{
			DateTime FirstDayOfMonth = dtDate.AddDays(1 - dtDate.Day);
				DateTime FirstDay = FirstDayOfMonth.AddDays(-Convert.ToInt16(FirstDayOfMonth.DayOfWeek));
			DateTime Dt = FirstDay;

			string Html = 
				"<div id='div" + sName + "' class='Cal' style='cursor: default; display: none; position: absolute;'>\n" + 
				"	<table id='tbl" + sName + "' border='0' cellpadding='0' cellspacing='0'>\n" +
				"		<tr>\n" +
				"			<td colspan='7' style='width: 100%;'>\n" +
				"				<table border='0' cellpadding='0' cellspacing='0' class='CalHdr'>\n" +
				"					<tr>\n" +
				"						<td " +
				"onClick='cal" + sName + ".PrevMonth(12);' " +
				"onMouseOver='calMsgOn(this, \"Previous Year\");' " +
				"onMouseOut='calMsgOff();'" +
				"><img src='Images/CalPrvYr.gif' border='0'></td>\n" +
				"						<td " +
				"onClick='cal" + sName + ".PrevMonth(1);' " +
				"onMouseOver='calMsgOn(this, \"Previous Month\");' " +
				"onMouseOut='calMsgOff();'" +
				"><img src='Images/CalPrvMth.gif' border='0'></td>\n" +
				"						<td id='td" + sName + "MonthYear' align='center'>" + dtDate.ToString("MMMM yyyy") + "</td>\n" +
				"						<td align='right' " +
				"onClick='cal" + sName + ".NextMonth(1);' " +
				"onMouseOver='calMsgOn(this, \"Next Month\");' " +
				"onMouseOut='calMsgOff();'" +
				"><img src='Images/CalNxtMth.gif' border='0'></td>\n" +
				"						<td align='right' " +
				"onClick='cal" + sName + ".NextMonth(12);' " +
				"onMouseOver='calMsgOn(this, \"Next Year\");' " +
				"onMouseOut='calMsgOff();'" +
				"><img src='Images/CalNxtYr.gif' border='0'></td>\n" +
				"					</tr>\n" +
				"				</table>\n" +
				"			</td>\n" +
				"		</tr>\n" +
				"		<tr class='CalDaysHdr'>\n" +
				"			<td>S</td>\n" +
				"			<td>M</td>\n" +
				"			<td>T</td>\n" +
				"			<td>W</td>\n" +
				"			<td>T</td>\n" +
				"			<td>F</td>\n" +
				"			<td>S</td>\n" +
				"		</tr>\n";

			for (int iWeek = 0; iWeek < 6; iWeek++)
			{
				Html +=
					"		<tr>\n";
				for (int iDay = 0; iDay < 7; iDay++)
				{
					string Class = "";
					if (Dt.Month != dtDate.Month)	{ Class = "class='CalOtherMonth' "; }
					if (Dt == dtDate)				{ Class = "class='CalSelected' "; }

					Html +=
						"			<td " +
						"date='" + Dt.ToString("M/d/yyyy") + "' " + 
						Class + 
						(Dt == DateTime.Today ? "style='font-weight: bold;' " : "") + 
						"onClick='cal" + sName + ".SetDate(this);' " +
						"onMouseOver='cal" + sName + ".MouseOver(this);' " +
						"onMouseOut='cal" + sName + ".MouseOut(this);'>" + 
						Dt.Day + "</td>\n";
					Dt = Dt.AddDays(1);
				}
				Html +=
					"		</tr>\n";
			}

			Html +=
				"	</table>\n" + 
				"</div>\n";

			Html += 
				"<div id='divMsg' style='display: none; position: absolute; border: black 1px solid; padding: 1px; background-color: #FFFBD9; font-size: 9pt;'></div>\n";

			return Html;
		}
	}
}
