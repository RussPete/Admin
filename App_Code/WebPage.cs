using System;
using Globals;
using System.Data;
//using System.Web;
//using System.Web.SessionState;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.HtmlControls;
using Utils;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Diagnostics;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public class WebPage
{
	private String Images;
	private System.Web.HttpResponse Response;
	private System.Web.HttpRequest Request;
	private System.Web.SessionState.HttpSessionState Session;
	public int NumServings = 0;

    public class PageInfo
    {
        public string Title;
        public string Name;
        public bool fNewPage;
        public PageAccess.AccessLevels AccessLevel;

        protected static PageAccess PageAccess = new PageAccess();

        public PageInfo(string Title, string Name, bool fNewPage = false)
        {
            this.Title = Title;
            this.Name  = Name;
            this.fNewPage = fNewPage;
            this.AccessLevel = PageAccess.AccessLevel(Name);
        }
    }

    public class MenuInfo
    {
        public string Title;
        public PageInfo[] Menu;

        public MenuInfo(string Title, PageInfo[] Menu)
        {
            this.Title = Title;
            this.Menu = Menu;
        }
    }

    public static MenuInfo Jobs = new MenuInfo(
        "Jobs",
        new PageInfo[]
        {
            new PageInfo("Job",            "Job"),
            new PageInfo("Menu",           "Food"),
            new PageInfo("Services",       "Services"),
            new PageInfo("Tables",         "Tables"),
            new PageInfo("Linens",         "Linens"),
            new PageInfo("Dishes",         "Dishes"),
            new PageInfo("Supplies",       "Supplies"),
            new PageInfo("Invoice",        "Invoice"),
            new PageInfo("Client Menu",    "ProposalMenu"),
            new PageInfo("Client Pricing", "ProposalPricing")
        }
    );

    public static MenuInfo Reports = new MenuInfo(
        "Reports",
        new PageInfo[]
        {
            new PageInfo("Production",     "DailyJobs"),
            new PageInfo("Job Leads",      "JobLeads"),
            new PageInfo("Menu Servings",  "ServingCounts"),
            new PageInfo("Job Calendar",   "JobCalendar"),
            new PageInfo("Job Sheets",     "JobSheets"),
            new PageInfo("Invoices",       "Invoices"),
            new PageInfo("Confirmations",  "Confirmations"),
            new PageInfo("Job Info",       "JobInfo")
        }
    );

    public static MenuInfo Shopping = new MenuInfo(
        "Shopping",
        new PageInfo[]
        {
            new PageInfo("Stocked Items",  "StockedItems"),
            new PageInfo("Shopping List",  "ShoppingList"),
            new PageInfo("Receipts",       "Purchases"),
            new PageInfo("Vendors",        "Vendors"),
            new PageInfo("Cleanup",        "CleanupShopping")
        }
    );

    public static MenuInfo Contacts = new MenuInfo(
        "Contacts",
        new PageInfo[]
        {
            new PageInfo("Contacts",        "Contacts"),
            new PageInfo("Customers",       "Customers"),
            new PageInfo("Merge Contacts",  "MergeContacts"),
            new PageInfo("Merge Customers", "MergeCustomers")
        }
    );

    public static MenuInfo MenuSetup = new MenuInfo(
        "Menu Setup",
        new PageInfo[]
        {
            new PageInfo("Categories",     "SetupMenuCategories"),
            new PageInfo("Menu Items",     "SetupMenuItems"),
            new PageInfo("Quote Prices",   "MenuItemQuotes"),
            new PageInfo("Invoice",        "SetupInvoice")
        }
    );

    public static MenuInfo Recipes = new MenuInfo(
        "Recipes",
        new PageInfo[]
        {
            new PageInfo("Recipes",        "Recipes"),
            new PageInfo("Recipe Book",    "RecipeBook"),
            new PageInfo("Ingredients",    "Ingredients"),
            new PageInfo("Units",          "Units"),
            new PageInfo("Cleanup",        "CleanupRecipes"),
            new PageInfo("Setup",          "SetupRecipes")
        }
    );

    public static MenuInfo JobSetup = new MenuInfo(
        "Job Setup",
        new PageInfo[]
        {
            new PageInfo("Services",       "OrderServices"),
            new PageInfo("Tables",         "OrderTables"),
            new PageInfo("Linens",         "OrderLinens"),
            new PageInfo("Dishes",         "OrderDishes"),
            new PageInfo("Supplies",       "OrderSupplies")
        }
    );

    public static MenuInfo Accounting = new MenuInfo(
        "Accounting",
        new PageInfo[]
        {
            new PageInfo("Accounting",     "Accounting"),
            new PageInfo("Check Search",   "CheckSearch"),
            new PageInfo("Pmt Search",     "PaymentSearch"),
            new PageInfo("Unpaid Jobs",    "UnpaidJobs"),
            new PageInfo("CC Payments",    "DailyCCPmts")
        }
    );

    public static MenuInfo EditProfile = new MenuInfo(
        "Edit Profile",
        new PageInfo[]
        {
            new PageInfo("Edit Profile",    "EditProfile")
        }
    );

    public static MenuInfo Users = new MenuInfo(
        "Users",
        new PageInfo[]
        {
            new PageInfo("Users",          "Users"),
            new PageInfo("Sling User",     "SlingUser")
        }
    );

    public static MenuInfo Sling = new MenuInfo(
        "Sling",
        new PageInfo[]
        {
            new PageInfo("Check Sling",    "CheckSling"),
            new PageInfo("Availability",   "Availability", true),
            new PageInfo("Reset Sling",    "ResetSling")
        }
    );

    public static MenuInfo Logout = new MenuInfo(
        "Logout",
        new PageInfo[]
        {
           new PageInfo("Default",        "Default")
        }
    );

    protected static MenuInfo[] Menus =
    {
        Jobs,
        Reports,
        Shopping,
        Contacts,
        MenuSetup,
        Recipes,
        JobSetup,
        null,
        Accounting,
        null,
        EditProfile,
        Users,
        Sling,
        null,
        Logout
    };

    public WebPage(String Images)
	{
		this.Images   = Images;
		this.Response = System.Web.HttpContext.Current.Response;
		this.Request = System.Web.HttpContext.Current.Request;
		this.Session = System.Web.HttpContext.Current.Session;
	}

	/// <summary>
	/// Top of the Web Page - banner, logo, menus
	/// </summary>
	public void Top()
	{
		Top(false);
	}

	/// <summary>
	/// Top of the Web Page - banner, logo, menus
	/// </summary>
	public void Top(bool fBanner)
	{
		//Session.Timeout = 30;  set web.config file to 10 minutes

		Response.Write(
			//"<IFRAME ID=\"KeepAliveFrame\" src=\"KeepSessionAlive.aspx\" frameBorder=\"0\" width=\"0\" height=\"0\" runat=\"server\"></IFRAME>\n" +
			Tabs(2) + Table("width='100%'") + "\n" +
			Tabs(3) + "<tr>\n" +
			Tabs(4) + "<td colspan='2' class='Header'>\n" +
			//Tabs(5) + "<img src='" + Images + "/AHeader.gif' width='741' height='135' alt=''/>" +
			Tabs(5) + "<div style='height: 11px;' />" +
			Tabs(4) + "</td>\n" +
			Tabs(3) + "</tr>\n" +
			Tabs(3) + "<tr>\n" +
			Tabs(4) + "<td class='LeftSideMenu' rowspan='2'>\n" +
			Tabs(5) + "<div style='width: 90px;' />\n");
		LeftMenu();
		Response.Write(
			Tabs(4) + "</td>\n" +
			Tabs(4) + "<td align='center' class='TopMenu'>\n");
		TopMenu();
		Response.Write(
			Tabs(4) + "</td>\n" +
			Tabs(3) + "</tr>\n" +
			Tabs(3) + "<tr>\n" +
			Tabs(4) + "<td>\n" +
			Tabs(5) + Table("width='1000'") + "\n" +
			Tabs(6) + "<tr>\n" +
			Tabs(7) + "<td class='Page'>\n");
	}

	public void Bottom()
	{
        string User = (string)Session["User"];
        string displayName = UserHelper.getDisplayNameForUser(User);
		string Html =
			Tabs(7) + "</td>\n" +
			Tabs(6) + "<tr>\n" +
			Tabs(5) + TableEnd() + "\n" +
			Tabs(5) + HorizLine(3, "100%", 0) + "\n";

		if (User != null && User.Length > 0)
		{
			Html +=
				Tabs(5) + string.Format("<div class='User' align='center'>User - {0} ({1})</div>\n", new string[] { displayName, User });
		}
		Html +=
			Tabs(4) + "</td>\n" +
			Tabs(3) + "</tr>\n" +
			Tabs(2) + TableEnd();

		Response.Write(Html);
		/*
						"						</td></tr>\n" +
						"					</table>\n" +
						"				</td>\n" +
						"			</tr>\n" +
						"			<tr><td colspan=\"2\"><hr /></td></tr>\n" +
						"			<tr><td colspan=\"2\">\n" +
						"				<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\n" +
						"					<tr>\n" +
						"						<td align=\"center\" width=\"12%\"><a href=\"AboutUs.asp\">About Us</a></td>\n" +
						"						<td align=\"center\" width=\"12%\"><a href=\"Contacts.asp\">Contact Us</a></td>\n" +
						"						<td align=\"center\" width=\"12%\"><a href=\"SiteMap.asp\">Site Map</a></td>\n" +
						"					</tr>\n" +
						"				</table>\n" +
						"			</td></tr>\n" +
						"			<tr><td colspan=\"2\"><hr /></td></tr>\n" +
						"			<tr><td colspan=\"2\" align=\"center\" class=\"Copyright\">" + Copyright + "</td></tr>\n" +
						"		</table>\n");
		*/
	}

	private void LeftMenu()
	{
		Response.Write(
			Tabs(5) + Table() + "\n");

        using (WebConfig wc = new WebConfig())
        {
            Response.Write("<div class=\"Env\">" + wc.Str("Env") + "</div>");
        }

        bool fPrevWasNull = false;

        foreach (MenuInfo Menu in Menus)
        {
            PageInfo Found = null;
            if (Menu != null)
            {
                foreach (PageInfo MenuItem in Menu.Menu)
                {
                    if (MenuItem.AccessLevel <= g.UserAccessLevel)
                    {
                        Found = MenuItem;
                        break;
                    }
                }
                if (Found != null)
                {
                    LeftMenuItem(Menu.Title, Found.Name + ".aspx", Found.AccessLevel, Found.fNewPage);
                    fPrevWasNull = false;
                }
            }
            else
            {
                if (!fPrevWasNull)
                {
                    Response.Write(WebPage.SpaceTr(1, 20));
                }
                fPrevWasNull = true;
            }
        }

  //      Response.Write(WebPage.SpaceTr(1, 20));

  //      LeftMenuItem("Jobs", "Job.aspx");
		//LeftMenuItem("Menu Setup", "SetupMenuCategories.aspx", PageAccess.AccessLevels.OfficeStaff);
  //      LeftMenuItem("Recipes", "Recipes.aspx", PageAccess.AccessLevels.OfficeStaff);
  //      LeftMenuItem("Shopping", "StockedItems.aspx", PageAccess.AccessLevels.OfficeStaff);
		//LeftMenuItem("Reports", "DailyJobs.aspx");
		//Response.Write(WebPage.SpaceTr(1, 20));
		//LeftMenuItem("Accounting", "Accounting.aspx", PageAccess.AccessLevels.Everything);
		//try
		//{
		//	if (Session["User"].ToString().Length > 0)
		//	{
  //              Response.Write(WebPage.SpaceTr(1, 20));
  //              LeftMenuItem("Edit Profile", "EditProfile.aspx");

  //              if (UserHelper.IsAdminUser(Session["User"].ToString()))
  //              {                    
  //                  LeftMenuItem("Users", "Users.aspx", PageAccess.AccessLevels.Everything);
  //                  LeftMenuItem("Availability", "Availability.aspx", PageAccess.AccessLevels.OfficeStaff, true);
  //              }

  //              Response.Write(WebPage.SpaceTr(1, 20));
		//		LeftMenuItem("Logout", "Default.aspx");
		//	}
		//}
		//catch (Exception Ex)
		//{
		//	Ex.GetType();
		//}

		Response.Write(
			Tabs(5) + TableEnd() + "\n");
	}

    private void LeftMenuItem(String ItemName, String WebPage, PageAccess.AccessLevels AccessLevel = PageAccess.AccessLevels.General, bool fNewPage = false)
	{
        if (g.UserAccessLevel >= AccessLevel)
        {
            string ClassName = "MenuButton";
            string Target = (fNewPage ? " target=availability" : "");
            Response.Write(
                Tabs(6) + "<tr><td class=\"" + ClassName + "\" onMouseOver=\"ClassOver(this);\" " +
                "onMouseOut=\"ClassOut(this);\" onClick=\"OpenPage('" + WebPage + "');\" nowrap>" +
                "<a href=\"" + WebPage + "\" onclick=\"fSkipOpen = true;\"" + Target + ">" + ItemName + "</a></td></tr>\n");
        }
	}

	private void TopMenu()
	{
		//bool fReports	= ((string)Session["Menu"] == "Reports");
		//bool fMaintance = ((string)Session["Menu"] == "Maintance");

		Response.Write(
			Tabs(5) + Table() + "\n" +
			Tabs(6) + "<tr>\n");

        string MenuTitle = (string)Session["Menu"];

        MenuInfo Found = null;
        foreach (MenuInfo Menu in Menus)
        {
            if (Menu != null)
            {
                if (Menu.Title == MenuTitle)
                {
                    Found = Menu;
                    break;
                }
            }
        }
        if (Found != null)
        {
            foreach (PageInfo Page in Found.Menu)
            {
                if (Page.AccessLevel <= g.UserAccessLevel)
                {
                    TopMenuItem(Page.Title, Page.Name + ".aspx", Page.fNewPage);
                }
            }
        }
/*
        switch ((string)Session["Menu"])
		{
			default:
				TopMenuItem("Job", "Job.aspx");
				TopMenuItem("Menu", "Food.aspx");
				TopMenuItem("Services", "Services.aspx");
				TopMenuItem("Linens", "Linens.aspx");
				TopMenuItem("Dishes", "Dishes.aspx");
				TopMenuItem("Supplies", "Supplies.aspx");
				//TopMenuItem("Crew", "Crew.aspx");
				TopMenuItem("Invoice", "Invoice.aspx");
				TopMenuItem("Client Menu", "ProposalMenu.aspx");
				TopMenuItem("Client Pricing", "ProposalPricing.aspx");
				break;

			case "Maintance":
				TopMenuItem("Categories", "SetupMenuCategories.aspx");
				TopMenuItem("Menu Items", "SetupMenuItems.aspx");
				TopMenuItem("Quote Prices", "MenuItemQuotes.aspx");
				TopMenuItem("Invoice", "SetupInvoice.aspx");
				break;

            case "Recipes":
                TopMenuItem("Recipes", "Recipes.aspx");
                TopMenuItem("Recipe Book", "RecipeBook.aspx");
                TopMenuItem("Ingredients", "Ingredients.aspx");
                TopMenuItem("Units", "Units.aspx");
                TopMenuItem("Cleanup", "CleanupRecipes.aspx");
                TopMenuItem("Setup", "SetupRecipes.aspx");
                break;

            case "Shopping":
                TopMenuItem("Stocked Items", "StockedItems.aspx");
                TopMenuItem("Shopping List", "ShoppingList.aspx");
				TopMenuItem("Receipts", "Purchases.aspx");
				TopMenuItem("Vendors", "Vendors.aspx");
                TopMenuItem("Cleanup", "CleanupShopping.aspx");
                break;

			case "Reports":
				TopMenuItem("Production", "DailyJobs.aspx");
                TopMenuItem("Job Leads", "JobLeads.aspx");
                TopMenuItem("Menu Servings", "ServingCounts.aspx");
				TopMenuItem("Job Calendar", "JobCalendar.aspx");
				//TopMenuItem("Crew Calendar", "CrewCalendar.aspx");
				TopMenuItem("Job Sheets", "JobSheets.aspx");
				//TopMenuItem("Jobs Entered", "JobsEntered.aspx");
				TopMenuItem("Invoices", "Invoices.aspx");
				break;

			case "Accounting":
				TopMenuItem("Accounting", "Accounting.aspx");
				TopMenuItem("Check Search", "CheckSearch.aspx");
				TopMenuItem("Pmt Search", "PaymentSearch.aspx");
				break;

            case "Users":
                TopMenuItem("Users", "Users.aspx");
                TopMenuItem("Sling", "SlingUser.aspx");
                break;

            case "EditProfile":
                break;
		}
*/
		Response.Write(
			Tabs(6) + "</tr>\n" +
			Tabs(5) + TableEnd() + "\n");
	}

	private void TopMenuItem(string ItemName, string WebPage, bool fNewPage = false)
	{
		string ClassName = "TopMenuButton";
        string Target = (fNewPage ? " target=availability" : "");
        Response.Write(
			Tabs(6) + "<td class=\"" + ClassName + "\" onMouseOver=\"ClassOver(this);\" " +
			"onMouseOut=\"ClassOut(this);\" onClick=\"OpenPage('" + WebPage + "');\" nowrap>" +
			"<a href=\"" + WebPage + "\" onclick=\"fSkipOpen = true;\"" + Target + ">" + ItemName + "</a></td>\n");
    }

    public void JobSubPage(string PageName)
	{
		Int32 JobRno = Str.Num((String)Session["JobRno"]);
		string Sql = "";
		string Customer = "";
		string JobDate = "";
		string EventType = "";
		string ServiceType = "";
		string NumServings = "";

		try
		{
			Sql = 
				"Select Coalesce(cu.Name, c.Name) as Customer, JobDate, EventType, ServiceType, " +
				"IsNull(NumMenServing, 0) + IsNull(NumWomenServing, 0) + IsNull(NumChildServing, 0) As NumServings " +
				"From mcJobs j " +
                "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                "Where JobRno = " + JobRno;
			DB db = new DB();
			DataTable dt = db.DataTable(Sql);
			if (dt.Rows.Count > 0)
			{
				DataRow r			= dt.Rows[0];
				Customer			= DB.Str(r["Customer"]);
				JobDate				= Fmt.Dt(DB.DtTm(r["JobDate"]));
				EventType			= DB.Str(r["EventType"]);
				ServiceType			= DB.Str(r["ServiceType"]);
				this.NumServings	= DB.Int32(r["NumServings"]);
				NumServings			= Fmt.Num(this.NumServings);
			}
			db.Close();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		string Html = 
			"<table cellspacing=\"0\" cellpadding=\"0\" align=\"center\" border=\"0\" width=\"100%\" class=\"JobInfo\">\n" +
			"	<tr>\n" +
			"		<td colspan=\"2\" align=\"center\"><span class=\"FeatureMain\">" + PageName + "</span></td>\n" +
			"	</tr>\n" +
			"	<tr>\n" +
            "		<td>" + JobDate + "&nbsp;&nbsp;" + Customer + "&nbsp;&nbsp;&nbsp;<span class=\"JobInfoTitle\">Job #</span>" + JobRno + "</td>\n" +
			"		<td align=\"right\">" + "<span class=\"JobInfoTitle\">Event:</span> " + EventType + "&nbsp;&nbsp;&nbsp;<span class=\"JobInfoTitle\">Service:</span> " + ServiceType + "&nbsp;&nbsp;&nbsp;<span class=\"JobInfoTitle\">Servings:</span> " + NumServings + "</td>\n" +
			"	</tr>\n" +
			"	<tr><td><img height=\"10\" src=\"" + Images + "/Space.gif\" width=\"1\" alt=\"\" /></td></tr>\n" +
			"</table>\n";

		Response.Write(Html);
	}

	public void CheckLogin(string PageName)
	{
		g.User = (string)Session["User"];
        g.UserAccessLevel = (PageAccess.AccessLevels)(Session["AccessLevel"] != null ? Session["AccessLevel"] : PageAccess.AccessLevels.General);
		//g.User = CookieStr("User");
		if (g.User == null || g.User == "")
		{
			//Response.Redirect("Default.aspx");
			Response.Redirect("~/Default.aspx");
			//Response.End();
			//Response.Write("Redirect to " + VirtualPathUtility.ToAppRelative("~/Default.aspx"));
		}
		else
		{
			// refresh to keep session from timing out
			Session["User"] = g.User;
		}

        //if (Page != null && Page.AccessLevel > g.UserAccessLevel)
        //{
        //    Response.Redirect("~/Job.aspx");
        //}
    }

    public Int32 JobRno()
	{
		Int32 JobRno = Str.Num((String)Session["JobRno"]);
		if (JobRno == 0)
		{
			Session["Warn"] = "SelectJob";
			//Response.Redirect("Job.aspx");
			Response.Redirect("~/Job.aspx");
			Response.End();
		}

		return JobRno;
	}

	public bool fProposal
	{
		get
		{
			bool fProposal = false;
			Int32 JobRno = Str.Num((String)Session["JobRno"]);

			if (JobRno > 0)
			{
				string Sql = string.Format("Select ProposalFlg From mcJobs Where JobRno = {0}", JobRno);
				try
				{
					DB db = new DB();
					fProposal = (db.SqlNum(Sql) == 1);
					db.Close();
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
			}

			return fProposal;
		}
	}

	public static string Tabs(int cTabs)
	{
		string Html = "";
		for (int i = 0; i < cTabs; i++)
		{
			Html += "\t";
		}

		return Html;
	}

	public static string Table()
	{
		return Table(false, "");
	}

	public static string Table(bool fBorder)
	{
		return Table(fBorder, "");
	}

	public static string Table(string Attrib)
	{
		return Table(false, Attrib);
	}

	public static string Table(bool fBorder, string Attrib)
	{
		return 
			"<table border='" + 
			(fBorder ? "1" : "0") + 
			"' cellpadding='0' cellspacing='0'" + 
			(Attrib.Length > 0 ? " " + Attrib : "") +
			">\n";
	}

	public static string TableEnd() { return "</table>\n"; }

	public static string Border(int BorderSize, int Padding)
	{
		return Border(BorderSize, "black", Padding);
	}
	public static string Border(int BorderSize, string Color, int Padding)
	{
		return String.Format(
			"<table style='border: solid {0}px {1};' border='0' cellpadding='{2}' cellspacing='0'><tr><td>\n",
			BorderSize,
			Color,
			Padding);
	}

	public static string BorderEnd() { return "</td></tr></table>\n"; }

	public static string Div(string Html, string sAlign, string Class)
	{
		return String.Format("<div align='{0}' class='{1}'>{2}</div>\n", sAlign, Class, Html);
	}

	public static string Span(int Html, string Class)
	{
		return Span(Convert.ToString(Html), Class);
	}

	public static string Span(string Html, string Class)
	{
		return String.Format("<span class='{0}'>{1}</span>", Class, Html);
	}

	public static string NoOp()
	{
		return NoOp(1, 1, null);
	}

	public static string NoOp(int Width, int Height)
	{
		return NoOp(Width, Height, null);
	}

	public static string NoOp(int Width, int Height, string Class)
	{
		return String.Format("<img width='{0}' height='{1}' src='Images/Space.gif' alt=''{2} />", Width, Height, (Class == null ? "" : " class='" + Class + "'"));
	}

    public static string NoSpace(int Width, int Height)
    {
        return String.Format("<td style='width: {0}px; height: {1}px;'/>\n", Width, Height);
    }

    public static string Space(int Width, int Height)
	{
		return String.Format("<td><img width='{0}' height='{1}' src='Images/Space.gif' alt='' /></td>\n", Width, Height);
	}

	public static string Space(int Width, int Height, string Class)
	{
		return String.Format("<td><img width='{0}' height='{1}' src='Images/Space.gif' alt='' class='{2}' /></td>\n", Width, Height, Class);
	}

	public static string SpaceTr(int Width, int Height)
	{
		return String.Format("<tr><td><img width='{0}' height='{1}' src='Images/Space.gif' alt='' /></td></tr>\n", Width, Height);
	}

	public static string SpaceTable(int Width, int Height)
	{
		return String.Format("<table border='0' cellpadding='0' cellspacing='0' style='border: none;'><tr><td style='border: none;'><img width='{0}' height='{1}' src='Images/Space.gif' alt='' border='0' /></td></tr></table>\n", Width, Height);
	}

	public static string HorizLine(int cTabs, string Width, int WhiteSpace)
	{
		return 
			Tabs(cTabs)	    + Table("align='center' width='" + Width + "' class='HorizLine'") +
			Tabs(cTabs + 1)	+ SpaceTr(1, WhiteSpace) +
			Tabs(cTabs + 1)	+ "<tr><td style='border-bottom: solid 1px black;'><img width='1' height='1' src='Images/Space.gif' alt='' /></td></tr>\n" +
			Tabs(cTabs + 1)	+ SpaceTr(1, WhiteSpace) +
			Tabs(cTabs)     + TableEnd();
	}

	public static string HorizRule(int cColSpan, string Width)
	{
		return 
			String.Format("<tr><td colspan='{0}'><hr width='{1}' /></td></tr>\n", cColSpan, Width);
	}

	public static string CookieStr(string Index)
	{
		string Value = "";
		if (System.Web.HttpContext.Current.Request.Cookies[Index] != null)
		{
			Value = System.Web.HttpContext.Current.Request.Cookies[Index].Value;
		}

		return Value;
	}


	public static string FindTextBox(ref TableRow tr, String ID)
	{
		String Text = "";

		try
		{
			Control Ctrl = tr.FindControl(ID);
			if (Ctrl != null)
			{
				TextBox txtBox = new TextBox();
				HtmlInputHidden txtHidden = new HtmlInputHidden();

				if (Ctrl.GetType() == txtBox.GetType())
				{
					txtBox = (TextBox)Ctrl;
					Text = txtBox.Text.Trim();
				}
				else if (Ctrl.GetType() == txtHidden.GetType())
				{
					txtHidden = (HtmlInputHidden)Ctrl;
					Text = txtHidden.Value.Trim();
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}

		return Text;
	}
}
