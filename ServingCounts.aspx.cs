using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Utils;
using Globals;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class ServingCounts : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected Utils.Calendar calDayDate;
	protected Utils.Calendar calWeekDate;
	protected Utils.Calendar calMonthDate;
	protected Utils.Calendar calBegDateRange;
	protected Utils.Calendar calEndDateRange;
	protected bool fReport = false;
	protected DateTime dtBeg;
	protected DateTime dtEnd;
	protected bool fNewPage;
	protected int iDtl = 0;

	private void Page_Init(object sender, System.EventArgs e)
	{
		InitCalendars();
	}

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Setup();
		}
	}

	private void InitCalendars()
	{
		calDayDate = new Utils.Calendar("DayDate", ref txtDayDate);
		calDayDate.ImageButton(imgDayDate);

		calWeekDate = new Utils.Calendar("WeekDate", ref txtWeekDate);
		calWeekDate.ImageButton(imgWeekDate);

		calMonthDate = new Utils.Calendar("MonthDate", ref txtMonthDate);
		calMonthDate.ImageButton(imgMonthDate);

		calBegDateRange = new Utils.Calendar("BegDateRange", ref txtBegDateRange);
		calBegDateRange.ImageButton(imgBegDateRange);

		calEndDateRange = new Utils.Calendar("EndDateRange", ref txtEndDateRange);
		calEndDateRange.ImageButton(imgEndDateRange);
	}

	private void Setup()
	{
		DateTime Tomorrow = DateTime.Today.AddDays(1);

		txtDayDate.Text = Fmt.Dt(Tomorrow);
		txtWeekDate.Text = Fmt.Dt(Tomorrow);
		txtMonthDate.Text = Fmt.Dt(Tomorrow.AddDays(1 - Tomorrow.Day).AddMonths(1));
		txtBegDateRange.Text =
		txtEndDateRange.Text = txtWeekDate.Text;

		txtDayDate.Attributes.Add("onChange", "iSetChk('rdoDay', true);");
		txtWeekDate.Attributes.Add("onChange", "iSetChk('rdoWeek', true);");
		txtMonthDate.Attributes.Add("onChange", "iSetChk('rdoMonth', true);");
		txtBegDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
		txtEndDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
	}

	protected void btnReport_Click(object sender, System.EventArgs e)
	{
		if (rdoDay.Checked)
		{
			dtBeg = Convert.ToDateTime(txtDayDate.Text);
			dtEnd = dtBeg;
		}

		if (rdoWeek.Checked)
		{
			dtBeg = Convert.ToDateTime(txtWeekDate.Text);
			dtEnd = dtBeg.AddDays(6);
		}
		if (rdoMonth.Checked)
		{
			dtBeg = Convert.ToDateTime(txtMonthDate.Text);
			dtBeg = dtBeg.AddDays(1 - dtBeg.Day);
			dtEnd = dtBeg.AddMonths(1).AddDays(-1);
		}
		if (rdoRange.Checked)
		{
			dtBeg = Convert.ToDateTime(txtBegDateRange.Text);
			dtEnd = Convert.ToDateTime(txtEndDateRange.Text);
		}

		fReport = true;
	}

	protected void Report()
	{
		Response.Write("</head>\n<body class='RptBody'>\n");

		int cJobs = 0;
		int cServings = 0;

		string Sql = String.Format(
			"Select Count(*) As Cnt, " +
            "(Sum(IsNull(NumMenServing, 0)) + Sum(IsNull(NumWomenServing, 0)) + Sum(IsNull(NumChildServing, 0))) As NumServings " +
			"From mcJobs Where JobDate Between {0} And {1} And IsNull(ProposalFlg, 0) = 0 " +
			"And CancelledDtTm Is Null",
			DB.PutDtTm(dtBeg),
			DB.PutDtTm(dtEnd));

		try
		{
			DataTable dt = db.DataTable(Sql);
			DataRow dr = dt.Rows[0];
			cJobs = DB.Int32(dr["Cnt"]);
			cServings = DB.Int32(dr["NumServings"]);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		string Html =
			WebPage.Table("align='center' width='100%'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td width='30%' valign='bottom'>\n" +
			//				WebPage.Tabs(2) + "<a href='javascript:ShowAll();'>Show Details</a>&nbsp;&nbsp;<a href='javascript:HideAll();'>Hide Details</a>\n" +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td align='center'>\n" +
			WebPage.Tabs(3) + WebPage.Div("Menu Servings", "center", "RptTitle") +
			WebPage.Tabs(5) + WebPage.Div(Fmt.Dt(dtBeg) + " - " + Fmt.Dt(dtEnd), "center", "RptSubTitle") +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td width='30%' valign='bottom' align='right'>" +
			WebPage.Span(Fmt.Num(cJobs), "RptNotice") + " Jobs, " +
			WebPage.Span(Fmt.Num(cServings), "RptNotice") + " Servings</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.TableEnd();

		Html +=
			WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +			// Category
			WebPage.Tabs(2) + WebPage.Space(15, 15) +	// bullet
			WebPage.Tabs(2) + WebPage.Space(20, 1) +	// space before JobDate
			WebPage.Tabs(2) + "<td></td>\n" +			// JobDate
			WebPage.Tabs(2) + WebPage.Space(5, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +			// JobTime
			WebPage.Tabs(2) + WebPage.Space(5, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +			// Customer
			WebPage.Tabs(2) + WebPage.Space(20, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +			// total servings
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			//WebPage.Tabs(2) + "<td></td>\n" +			// men servings
			//WebPage.Tabs(2) + WebPage.Space(5, 1) +
			//WebPage.Tabs(2) + "<td></td>\n" +			// women servings
			//WebPage.Tabs(2) + WebPage.Space(5, 1) +
			//WebPage.Tabs(2) + "<td></td>\n" +			// child servings
			//WebPage.Tabs(2) + WebPage.Space(20, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +			// total jobs
			WebPage.Tabs(1) + "</tr>\n";

		Html +=
			WebPage.Tabs(1) + "<tr><td colspan='12'><span class='RptInfo'>Click an item name to show or hide job details for that item.</span></td></tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='7'>\n" +
			//				WebPage.Tabs(3) + "<a href='javascript:ShowAll();'>Show</a>&nbsp;(Job Details)&nbsp;<a href='javascript:HideAll();'>Hide</a>\n" +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td align='right' class='RptHdrBold' colspan='3'><b># Servings</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			//WebPage.Tabs(2) + "<td align='right' class='RptHdr'>M</td>\n" +
			//WebPage.Tabs(2) + "<td></td>\n" +
			//WebPage.Tabs(2) + "<td align='right' class='RptHdr'>W</td>\n" +
			//WebPage.Tabs(2) + "<td></td>\n" +
			//WebPage.Tabs(2) + "<td align='right' class='RptHdr'>C</td>\n" +
			//WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right' class='RptHdrBold'><b># Jobs</b></td>\n" +
			WebPage.Tabs(1) + "</tr>\n";

		Response.Write(Html);

		//FoodCategories FoodCat = new FoodCategories();
		//for (
		//    string Category = FoodCat.FirstCategory(FoodCategories.Type.Hot);
		//    Category != null;
		//    Category = FoodCat.NextCategory())
		//{
		//    MenuCategory(Category);
		//}
		//for (
		//    string Category = FoodCat.FirstCategory(FoodCategories.Type.Cold);
		//    Category != null;
		//    Category = FoodCat.NextCategory())
		//{
		//    MenuCategory(Category);
		//}

		//MenuCategory("Meats");
		//MenuCategory("Soup");
		//MenuCategory("Pasta");
		//MenuCategory("Sandwich");
		//MenuCategory("Appetizers");
		//MenuCategory("Sides");
		//MenuCategory("Salads");
		//MenuCategory("Bread");
		//MenuCategory("Desserts");
		//MenuCategory("Drink");
		MenuCategory("");

		Response.Write(
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 30) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
			WebPage.Tabs(2) + "<td colspan='12'>\n" +
			WebPage.Tabs(3) + "<input type=button value='Show Details' title='Click to show job details for all items.' onClick='ShowAll();' class='RptSmallBtn'>\n" +
			WebPage.Tabs(3) + "<input type=button value='Hide Details' title='Click to hide job details for all items.' onClick='HideAll();' class='RptSmallBtn'>\n" +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.TableEnd() +
			"<script lang='javascript'>var iEndDtl = " + --iDtl + ";</script>\n"
			);
	}

	protected void MenuCategory(string Category)
	{
		//FoodCategories FoodCat = new FoodCategories();
		//string Sql = String.Format(
		//    "Select Category, MenuItem, JobDate, Customer, " +
		//    "Coalesce(MealTime, ArrivalTime, DepartureTime) As JobTime, " +
		//    "NumMenServing, NumWomenServing, NumChildServing, Qty " +
		//    "From mcJobs j Inner Join mcJobFood f On j.JobRno = f.JobRno " +
		//    "Where JobDate Between {0} And {1} " +
		//    "And Category {2} " +
		//    "And CancelledDtTm Is Null " +
		//    "Order By Category, MenuItem, JobDate, JobTime, Customer",
		//    DB.PutDtTm(dtBeg),
		//    DB.PutDtTm(dtEnd),
		//    (Category.Length > 0 ?
		//    "= " + DB.PutStr(Category) :
		//    //"Not In ('Meats', 'Soup', 'Pasta', 'Sandwich', 'Appetizers', 'Sides', 'Salads', 'Bread', 'Desserts', 'Drink')"));
		//    "Not In (" + FoodCat.SqlList(FoodCategories.Type.Hot) + ", " + FoodCat.SqlList(FoodCategories.Type.Cold) + ")"));
		string Sql = String.Format(
            "Select Category, MenuItem, JobDate, Coalesce(cu.Name, c.Name) as Customer, " +
			"Coalesce(MealTime, ArrivalTime, DepartureTime) As JobTime, " +
			"NumMenServing, NumWomenServing, NumChildServing, Qty " +
			"From mcJobs j " +
            "Inner Join Contacts c On j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Inner Join mcJobFood f On j.JobRno = f.JobRno " +
			"Where JobDate Between {0} And {1} " +
            "And IsNull(ProposalFlg, 0) = 0 " +
            "And CancelledDtTm Is Null " +
            "And Trim(MenuItem) <> '' " +
			"Order By Category, MenuItem, JobDate, JobTime, Customer",
			DB.PutDtTm(dtBeg),
			DB.PutDtTm(dtEnd));

		try
		{
			string PrevCategory = null;
			string PrevMenuItem = null;
			string FirstCategory = "First";

			DataTable dt = db.DataTable(Sql);
			for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
			{
				DataRow dr = dt.Rows[iRow];
				Category = DB.Str(dr["Category"]);
				string MenuItem = GetMenuItem(ref dr);

				int Men;
				int Women;
				int Child;
				int Servings;
				int Qty;

				if (MenuItem != PrevMenuItem)
				{
					int iBeg = iRow;
					//int cQty = 0;
					int cServing = 0;
					int cMen = 0;
					int cWomen = 0;
					int cChild = 0;
					int cJobs = 0;

					PrevMenuItem = MenuItem;

					while (MenuItem == PrevMenuItem && iRow < dt.Rows.Count)
					{
						Qty = DB.Int32(dr["Qty"]);
						Men = DB.Int32(dr["NumMenServing"]);
						Women = DB.Int32(dr["NumWomenServing"]);
						Child = DB.Int32(dr["NumChildServing"]);

						cMen += Men;
						cWomen += Women;
						cChild += Child;
						cServing += (Qty != 0 ? Qty : Men + Women + Child);
						//cQty += Qty;
						cJobs++;

						if (++iRow < dt.Rows.Count)
						{
							dr = dt.Rows[iRow];
							MenuItem = GetMenuItem(ref dr);
						}
					}

					string sClass = (Category == PrevCategory ? "" : " class=\"" + FirstCategory + "RptCategory\"");
					string sCategory = (Category == PrevCategory ? "" : "<b>" + Category + "</b>");

					Response.Write(
						WebPage.Tabs(1) + "<tr" + sClass + ">\n" +
						WebPage.Tabs(2) + "<td>" + sCategory + "</td>\n" +
						WebPage.Tabs(2) + "<td valign='top' align='right'><img src='Images/Diamond.gif' class='RptBullet'></td>\n" +
						WebPage.Tabs(2) + "<td colspan='6' class='JobItem'><a href='javascript:ShowDtl(" + iDtl + ", " + (iDtl + cJobs - 1) + ");'>" + PrevMenuItem + "</a></td>\n" +
						WebPage.Tabs(2) + "<td></td>\n" +
						WebPage.Tabs(2) + "<td align='right' class='RptNotice'>" + Fmt.Num(cServing, false) + "</td>\n" +
						WebPage.Tabs(2) + "<td></td>\n" +
						//WebPage.Tabs(2) + "<td align='right'>" + Fmt.Num(cMen, false) + "</td>\n" +
						//WebPage.Tabs(2) + "<td></td>\n" +
						//WebPage.Tabs(2) + "<td align='right'>" + Fmt.Num(cWomen, false) + "</td>\n" +
						//WebPage.Tabs(2) + "<td></td>\n" +
						//WebPage.Tabs(2) + "<td align='right'>" + Fmt.Num(cChild, false) + "</td>\n" +
						//WebPage.Tabs(2) + "<td></td>\n" +
						WebPage.Tabs(2) + "<td align='right' class='RptNotice'>" + Fmt.Num(cJobs, false) + "</td>\n" +
						WebPage.Tabs(1) + "</tr>\n"
					);

					if (sClass != "")
					{
						FirstCategory = "";
					}

					iRow = iBeg;
				}

				dr = dt.Rows[iRow];
				MenuItem = GetMenuItem(ref dr);
				Men = DB.Int32(dr["NumMenServing"]);
				Women = DB.Int32(dr["NumWomenServing"]);
				Child = DB.Int32(dr["NumChildServing"]);
				Servings = Men + Women + Child;
				Qty = DB.Int32(dr["Qty"]);

				Response.Write(
					WebPage.Tabs(1) + "<tr id='Dtl" + iDtl++ + "' style='display: none;'>\n" +
					WebPage.Tabs(2) + "<td colspan='3'></td>\n" +
					WebPage.Tabs(2) + "<td>" + Fmt.Dt(DB.DtTm(dr["JobDate"])) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Tm12Hr(DB.DtTm(dr["JobTime"])) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td>" + DB.Str(dr["Customer"]) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Num((Qty != 0 ? Qty : Servings), false) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					//WebPage.Tabs(2) + "<td align='right' class='RptSmall'>" + Fmt.Num(Men, false) + "</td>\n" +
					//WebPage.Tabs(2) + "<td></td>\n" +
					//WebPage.Tabs(2) + "<td align='right' class='RptSmall'>" + Fmt.Num(Women, false) + "</td>\n" +
					//WebPage.Tabs(2) + "<td></td>\n" +
					//WebPage.Tabs(2) + "<td align='right' class='RptSmall'>" + Fmt.Num(Child, false) + "</td>\n" +
					//WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'></td>\n" +
					WebPage.Tabs(1) + "</tr>\n"
				);

				PrevMenuItem = MenuItem;
				PrevCategory = Category;
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private string GetMenuItem(ref DataRow dr)
	{
		const string cnBlank = "&nbsp;&lt;blank&gt;";

		string MenuItem = DB.Str(dr["MenuItem"]);

		MenuItem = (MenuItem.Length > 0 ? MenuItem : cnBlank);

		return MenuItem;
	}
}