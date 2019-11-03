using Globals;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;

public partial class ShoppingList : System.Web.UI.Page
{
	#region Parms

	protected WebPage Pg;
	protected DB db;
	protected bool fReport = false;
	protected DateTime dtBeg;
	protected DateTime dtEnd;
	protected bool fNewPage;
	protected int iDtl = 0;
	protected List<Ingred> lstIngred;
    protected string VendorInfo = string.Empty;

	private void Page_Init(object sender, System.EventArgs e)
	{
		//InitCalendars();
	}

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.Shopping.Title;
			Setup();
		}
	}

	//private void InitCalendars()
	//{
	//	calDayDate = new Utils.Calendar("DayDate", ref txtDayDate);
	//	calDayDate.ImageButton(imgDayDate);

	//	calWeekDate = new Utils.Calendar("WeekDate", ref txtWeekDate);
	//	calWeekDate.ImageButton(imgWeekDate);

	//	calMonthDate = new Utils.Calendar("MonthDate", ref txtMonthDate);
	//	calMonthDate.ImageButton(imgMonthDate);

	//	calBegDateRange = new Utils.Calendar("BegDateRange", ref txtBegDateRange);
	//	calBegDateRange.ImageButton(imgBegDateRange);

	//	calEndDateRange = new Utils.Calendar("EndDateRange", ref txtEndDateRange);
	//	calEndDateRange.ImageButton(imgEndDateRange);
	//}

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

	private void Page_PreRender(object sender, System.EventArgs e)
	{
		if (fReport)
		{
			Report();
		}
	}
	
	#endregion Parms

	#region Report

	private void Report()
	{
		lstIngred = new List<Ingred>();

        hfBegDate.Value = dtBeg.ToString();
        hfEndDate.Value = dtEnd.ToString();

        ltlBegDate.Text = Fmt.Dt(dtBeg);
        ltlEndDate.Text = Fmt.Dt(dtEnd);

        string Sql = string.Format(
			"Select\n" +
            "j.JobRno, Coalesce(cu.Name, c.Name) as Customer, j.NumMenServing, NumWomenServing, NumChildServing, " +
            "f.Qty, f.MenuItem, r.RecipeRno, r.Name As Recipe,\n" +
			"r.NumServings, r.MenServingRatio, r.WomenServingRatio, r.ChildServingRatio,\n" +
			"r.YieldQty, r.YieldUnitRno, (Select UnitSingle From Units Where UnitRno = r.YieldUnitRno) As YieldUnit,\n" +
			"r.PortionQty, r.PortionUnitRno, (Select UnitSingle From Units Where UnitRno = r.PortionUnitRno) As PortionUnit,\n" +
			"x.IngredRno, x.SubrecipeRno, x.UnitQty, x.UnitRno,\n" +
			"(Select UnitSingle From Units Where UnitRno = x.UnitRno) As UnitSingle,\n" +
			"(Select UnitPlural From Units Where UnitRno = x.UnitRno) As UnitPlural\n" +
			"From mcJobs j\n" +
            "Inner Join Contacts c On j.ContactRno = c.ContactRno\n" +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno\n" + 
            "Inner Join mcJobFood f On j.JobRno = f.JobRno\n" +
			"Inner Join mcJobMenuItems m On f.MenuItemRno = m.MenuItemRno\n" +
			"Inner Join Recipes r On m.RecipeRno = r.RecipeRno\n" +
			"Inner join RecipeIngredXref x On r.RecipeRno = x.RecipeRno\n" +
			"Where JobDate Between {0} And {1}\n" +
			"And j.ProposalFlg = 0\n" +
			"And j.CancelledDtTm Is Null\n" +
			"Order By j.JobRno, f.MenuItem\n",
			DB.PutDtTm(dtBeg),
			DB.PutDtTm(dtEnd));

		try
		{
			//Response.Write(Sql + "<br/>");
			DataTable dtRecipeIngredXref = db.DataTable(Sql);
			//Response.Write(dtRecipeIngredXref.Rows.Count + "<br/>");
			if (dtRecipeIngredXref.Rows.Count > 0)
			{
				Ingred.LoadVendors();
				Ingred.LoadIngredConversions();
				Ingred.LoadIngredPurchases();

                // find the ingredients for the recipes
                Sql = string.Format(
                    "With IngredData As (\n" +
                    "\tSelect IngredRno, Name, IsNull(StockedFlg, 0) As StockedFlg, PrefVendors\n" +
                    //"\t(Select Top 1 v.VendorRno From VendorIngredXref x Inner Join Vendors v On x.VendorRno = v.VendorRno Where x.IngredRno = i.IngredRno) As VendorRno, " +
                    //"\t(Select Top 1 v.Name From VendorIngredXref x Inner Join Vendors v On x.VendorRno = v.VendorRno Where x.IngredRno = i.IngredRno) As Vendor " +
                    "\tFrom Ingredients i\n" +
                    "\tWhere (IngredRno In ({0}) Or IsNull(StockedFlg, 0) = 1 And IsNull(StockedPurchaseQty, 0) <> 0 And IsNull(HideFlg, 0) = 0)\n" +
                    "\tAnd IsNull(NonPurchaseFlg, 0) = 0\n" +
                    ")\n" +
                    "Select * From IngredData {1}\n",
                    Str.Join(DB.StrArray(dtRecipeIngredXref, "IngredRno"), ","),
                    //(chkIncludeStocked.Checked ? string.Empty : "Where StockedFlg = 0\n"));
                    string.Empty);
				DataTable dt = db.DataTable(Sql);
				//Response.Write(Sql + "<br/>");

				// build a collection of ingredients
				foreach (DataRow dr in dt.Rows)
				{
					AddIngredient(dr);
				}

				// add the number of servings and recipe quantities
				foreach (DataRow dr in dtRecipeIngredXref.Rows)
				{
					int JobServings = DB.Int32(dr["Qty"]);
					if (JobServings == 0)
					{
						JobServings = DB.Int32(dr["NumMenServing"]) + DB.Int32(dr["NumWomenServing"]) + DB.Int32(dr["NumChildServing"]);
					}

					int IngredRno = DB.Int32(dr["IngredRno"]);
					int SubrecipeRno = DB.Int32(dr["SubrecipeRno"]);
					decimal NumServings = DB.Dec(dr["NumServings"]);

					// if an ingredient
					if (IngredRno != 0)
					{
						AddServings(IngredRno, JobServings, DB.Int32(dr["JobRno"]), DB.Str(dr["Customer"]), JobServings, DB.Str(dr["MenuItem"]), DB.Int32(dr["RecipeRno"]), DB.Str(dr["Recipe"]), NumServings, DB.Dec(dr["UnitQty"]), DB.Int32(dr["UnitRno"]), DB.Str(dr["UnitSingle"]), DB.Str(dr["UnitPlural"]), string.Empty, null);
					}

					// if a subrecipe
					if (SubrecipeRno != 0)
					{
						if (NumServings != 0)
						{
							int RecipeRno = DB.Int32(dr["RecipeRno"]);
							AddSubrecipe(RecipeRno, SubrecipeRno, JobServings / NumServings, DB.Int32(dr["JobRno"]), DB.Str(dr["Customer"]), JobServings, DB.Str(dr["MenuItem"]), RecipeRno, DB.Str(dr["Recipe"]), DB.Dec(dr["UnitQty"]), DB.Int32(dr["UnitRno"]), DB.Str(dr["UnitSingle"]), DB.Str(dr["UnitPlural"]), string.Empty, new List<string>());
						}
					}
				}

				// add any menu items that do not have a recipe
				Sql = string.Format(
                    "Select j.JobRno, Coalesce(cu.Name, c.Name) as Customer, j.NumMenServing, NumWomenServing, NumChildServing, f.Qty, f.Category, f.MenuItem, f.MenuItemRno,\n" +
					"u.UnitRno, u.UnitSingle, u.UnitPlural, m.AsIsFlg, m.RecipeRno\n" +
					"From mcJobs j\n" +
                    "Inner Join Contacts c On j.ContactRno = c.ContactRno\n" +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno\n" +
                    "Inner Join mcJobFood f On j.JobRno = f.JobRno\n" +
					"Inner Join mcJobMenuItems m On f.MenuItemRno = m.MenuItemRno\n" +
					"Inner Join Units u on u.UnitSingle = 'ea'\n" +
					"Where JobDate Between {0} And {1}\n" +
					"And j.ProposalFlg = 0\n" +
					"And j.CancelledDtTm Is Null\n" +
					"And m.RecipeRno Is Null\n" +
					"And m.MenuItem <> ''\n" +
					"Order By j.JobRno, f.MenuItem",
					DB.PutDtTm(dtBeg),
					DB.PutDtTm(dtEnd));
				dt = db.DataTable(Sql);
				foreach (DataRow dr in dt.Rows)
				{
					int JobServings = DB.Int32(dr["Qty"]);
					if (JobServings == 0)
					{
						JobServings = DB.Int32(dr["NumMenServing"]) + DB.Int32(dr["NumWomenServing"]) + DB.Int32(dr["NumChildServing"]);
					}

					string MenuItem = string.Format("{0} - {1}", DB.Str(dr["Category"]) ,DB.Str(dr["MenuItem"]));

					Ingred NotIngred = null;
					foreach (Ingred CurrIngred in lstIngred)
					{
						if (CurrIngred.IngredRno == 0 && CurrIngred.Name == MenuItem)
						{
							NotIngred = CurrIngred;
							break;
						}
					}

					if (NotIngred == null)
					{
						NotIngred = new Ingred(0);
						NotIngred.Name = MenuItem;
                        if (!DB.Bool(dr["AsIsFlg"]) && DB.Int32(dr["RecipeRno"]) == 0)
                        {
                            NotIngred.AddNote(string.Format("Menu item needs a <a href=\"Recipes.aspx\" target=\"_blank\">recipe</a> or <a href=\"SetupMenuItems.aspx?Rno={0}\" target=\"_blank\">checked As Is.</a>", DB.Int32(dr["MenuItemRno"])));
                        }
						lstIngred.Add(NotIngred);
					}

					NotIngred.AddQty(JobServings, DB.Int32(dr["JobRno"]), DB.Str(dr["Customer"]), JobServings, MenuItem, 0, MenuItem, 1, 1, DB.Int32(dr["UnitRno"]), DB.Str(dr["UnitSingle"]), DB.Str(dr["UnitPlural"]), string.Empty);
				}

				// sort by vendor, ingredient
				lstIngred.Sort();

				string PrevVendor = string.Empty;
                int iCount = 0;

				//Html += ReportHeader();

				foreach (Ingred CurrIngred in lstIngred)
				{
					//if (CurrIngred.Vendor != PrevVendor)
					//{
					//	Html += NewVendor(CurrIngred.Vendor, fFirst);
					//	fFirst = false;
					//	fPrevStockedFlg = false;
					//} 

					//if (CurrIngred.StockedFlg != fPrevStockedFlg)
					//{
					//	Html += "<tr><td colspan='8' class='Stocked'>Stock Items</td></tr>\n";
					//}

					CurrIngred.FindPurchaseInfo();

                    // replace stocked items quantity
                    if (CurrIngred.StockedFlg)
                    {
                        CurrIngred.AdjustStockedItemsQuantity(db);
                    }

                    decimal Qty = (CurrIngred.StockedFlg ? CurrIngred.StockedPurchaseQty : CurrIngred.PurchaseQty);     // && CurrIngred.StockedPurchaseQty != 0
                    if (Qty == 0)
                    {
                        continue;
                    }

                    /*
                    if (CurrIngred.fFoundConversion)
					{
						Html += string.Format(
							"\t\t<tr>\n" +
							"\t\t\t<td><img src='Images/Box.gif'/></td>\n" +
							"\t\t\t<td class='Qty'>{0}</td>\n" +
							"\t\t\t<td>{1}</td>\n" +
							"\t\t\t<td title='{7}' class='qtp Ingred'><a href='Ingredients.aspx?Rno={8}' target='_blank'>{2}</a></td>\n" +
							"\t\t\t<td class='Qty'>{3}</td>\n" +
							"\t\t\t<td>{4}{5}</td>\n" +
							"\t\t\t<td>{6}</td>\n" +
							"\t\t</tr>\n",
							Str.ShowFract(CurrIngred.PurchaseQty * CurrIngred.PurchaseUnitQty, 2),
							(CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural),
							CurrIngred.Name.Replace("'", "&apos;"),
							Math.Ceiling(CurrIngred.PurchaseQty),
							(CurrIngred.PurchaseUnitQty != 1 ? Math.Ceiling(CurrIngred.PurchaseUnitQty) + " " : string.Empty),
							(CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural),
							CurrIngred.Vendor,
							CurrIngred.AddlInfo,
							CurrIngred.IngredRno);
					}
					else
					{
						Html += string.Format(
							"\t\t<tr>\n" +
							"\t\t\t<td><img src='Images/Box.gif'/></td>\n" +
							"\t\t\t<td class='Qty'>{0}</td>\n" +
							"\t\t\t<td>{1}</td>\n" +
							"\t\t\t<td title='{7}' class='qtp'>{3}</td>\n" +
							"\t\t\t<td class='Qty'>{3}</td>\n" +
							"\t\t\t<td>{4}{5}</td>\n" +
							"\t\t\t<td>{6}</td>\n" +
							"\t\t</tr>\n",
							Str.ShowFract(CurrIngred.Qty, 2),
							(CurrIngred.Qty <= 1 ? CurrIngred.UnitSingle : CurrIngred.UnitPlural),
							CurrIngred.Name.Replace("'", "&apos;"),
							"?",
							(CurrIngred.PurchaseUnitQty != 1 ? Math.Ceiling(CurrIngred.PurchaseUnitQty) + " " : string.Empty),
							(CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural),
							CurrIngred.Vendor,
							CurrIngred.AddlInfo);

						if (CurrIngred.PurchaseUnitRno != 0)
						{
							CurrIngred.AddNote(string.Format("<a href='Ingredients.aspx?Rno={2}' target='_blank'><i class=\"icon-warning-sign\"></i> Need conversion from {0} to {1}.</a>", CurrIngred.UnitPlural, CurrIngred.PurchaseUnitPlural, CurrIngred.IngredRno));
						}
					}

					if (CurrIngred.Note.Length > 0)
					{
						Html += string.Format(
							"<tr><td colspan='8' class='Notes'><ul>{0}</ul></td></tr>\n",
							CurrIngred.Note);
					}
                    */

                    TableRow tr = new TableRow();
                    tblReport.Rows.Add(tr);

                    TableCell tc = new TableCell();
                    tr.Cells.Add(tc);

                    tc.Controls.Add(new Image() { ImageUrl = "Images/Box.gif" });
                    tc.Controls.Add(new HiddenField() { ID = "hfIngredRno" + iCount, Value = CurrIngred.IngredRno.ToString() });
                    tc.Controls.Add(new HiddenField() { ID = "hfStocked" + iCount, Value = CurrIngred.StockedFlg.ToString() });
                    tc.Controls.Add(new HiddenField() { ID = "hfStockedQty" + iCount, Value = CurrIngred.StockedPurchaseQty.ToString() });
                    tc.Controls.Add(new HiddenField() { ID = "hfQty" + iCount, Value = CurrIngred.PurchaseQty.ToString() });
                    tc.Controls.Add(new HiddenField() { ID = "hfUnitQty" + iCount, Value = CurrIngred.PurchaseUnitQty.ToString() });
                    tc.Controls.Add(new HiddenField() { ID = "hfUnitRno" + iCount, Value = CurrIngred.PurchaseUnitRno.ToString() });
                    tc.Controls.Add(new HiddenField() { ID = "hfVendorRno" + iCount, Value = CurrIngred.VendorRno.ToString() });                    

                    tr.Cells.Add(new TableCell() { CssClass = "Qty", Text = Str.ShowFract(CurrIngred.PurchaseQty * CurrIngred.PurchaseUnitQty, 2) });
                    tr.Cells.Add(new TableCell() { Text = (CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural) });

                    string Name = CurrIngred.Name.Replace("'", "&apos;");
                    if (CurrIngred.fFoundConversion)
                    {
                        tc = new TableCell() { ToolTip = CurrIngred.AddlInfo, CssClass = "qtp Ingred" };
                        tr.Cells.Add(tc);
                        tc.Controls.Add(new HyperLink() { NavigateUrl = "Ingredients.aspx?Rno=" + CurrIngred.IngredRno, Target = "_blank", Text = Name });
                    }
                    else
                    {
                        tr.Cells.Add(new TableCell() { ToolTip = CurrIngred.AddlInfo, CssClass = "qtp Ingred", Text = Name });
                    }

                    tr.Cells.Add(new TableCell() { CssClass = "Qty", Text = Math.Ceiling(Qty).ToString() });
                    tr.Cells.Add(new TableCell() { Text = string.Format("{0}{1}", (CurrIngred.PurchaseUnitQty != 1 ? Math.Ceiling(CurrIngred.PurchaseUnitQty).ToString() : string.Empty), (CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural)) });
                    tr.Cells.Add(new TableCell() { CssClass="Vendor", Text = CurrIngred.Vendor });

                    if (!CurrIngred.fFoundConversion && CurrIngred.PurchaseUnitRno != 0)
                    {
                        CurrIngred.AddNote(
                            string.Format(
                                "<a href='Ingredients.aspx?Rno={2}' target='_blank' tabindex='-1'><i class=\"icon-warning-sign\"></i> Need conversion from {0} to {1}.</a>",
                                CurrIngred.UnitPlural,
                                CurrIngred.PurchaseUnitPlural,
                                CurrIngred.IngredRno));
                    }

                    if (CurrIngred.Note.Length > 0)
                    {
                        tr = new TableRow();
                        tblReport.Rows.Add(tr);
                        tr.Cells.Add(new TableCell() { ColumnSpan = 7, CssClass = "Notes", Text = string.Format("<ul>{0}</ul>", CurrIngred.Note) });
                    }

                    iCount++;
                    //PrevVendor = CurrIngred.Vendor;
                    //fPrevStockedFlg = CurrIngred.StockedFlg;
                }
                hfParmCount.Value = iCount.ToString();
            }
            //Html += FinishVendor() + "<br /><br />";
            //Html += "</table><br /><br />";

            //ltlReport.Text = Html;

            FindVendorInfo();
        }
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private string NewVendor(string Vendor, bool First)
	{
		string Html = string.Empty;

		if (First)
		{
			//Html += "<table class='Report'>\n";
		}
		else
		{
			Html += "</table>";
		}

		Html += string.Format(
			//"<div class='RptSubTitle'>{0}</div>\n" +
			"<table class='Report'>\n" +
			"\t<thead>\n" +
			"\t\t<tr class='NewVendor'><td colspan='8'><div class='RptSubTitle'>{0}</div></td></tr>\n" +
			"\t\t<tr>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th colspan='2' class='Center'>Jobs</th>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th colspan='2' class='Center'>Purchase</th>\n" +
			"\t\t</tr>\n" +
			"\t\t<tr class='Underline'>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th class='Qty'>Qty</th>\n" +
			"\t\t\t<th>Unit</th>\n" +
			"\t\t\t<th>Ingredient</th>\n" +
			"\t\t\t<th class='Qty'>Qty</th>\n" +
			"\t\t\t<th>Unit</th>\n" +
			"\t\t\t<th class='Price'>Last $</th>\n" +
			"\t\t\t<th class='Price'>Ext $</th>\n" +
			"\t\t</tr>\n" +
			"\t</thead>\n",
			Vendor);

		return Html;
	}

	private string FinishVendor()
	{
		return "</table>\n";
	}

	private string ReportHeader()
	{
		string Html =
			"<table class='Report'>\n" +
			"\t<thead>\n" +
			"\t\t<tr>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th colspan='2' class='Center'>Jobs</th>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th colspan='2' class='Center'>Purchase</th>\n" +
			"\t\t</tr>\n" +
			"\t\t<tr class='Underline'>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th class='Qty'>Qty</th>\n" +
			"\t\t\t<th>Unit</th>\n" +
			"\t\t\t<th>Ingredient</th>\n" +
			"\t\t\t<th class='Qty'>Qty</th>\n" +
			"\t\t\t<th>Unit</th>\n" +
			"\t\t\t<th>Vendor</th>\n" +
			"\t\t</tr>\n" +
			"\t</thead>\n";

		return Html;
	}

	private void AddIngredient(DataRow dr)
	{
		int VendorRno = 0;
		int IngredRno = DB.Int32(dr["IngredRno"]);
		bool fFound = false;

		// check for stocked flag
		bool fStockedFlag = DB.Bool(dr["StockedFlg"]);
		//if (chkIncludeStocked.Checked || !chkIncludeStocked.Checked && !fStockedFlag)
		{
			// see if the ingredient is already in the list
			foreach (Ingred Ingred in lstIngred)
			{
				if (Ingred.IngredRno == IngredRno)
				{
					fFound = true;
				}
			}

			if (!fFound)
			{
				// not in the ingredit list, so add it

				string PrefVendors = DB.Str(dr["PrefVendors"]);
				string[] aPrefVendors = PrefVendors.Split(new char[] { ',' });
				if (aPrefVendors.Length > 1)
				{
					VendorRno = Str.Num(aPrefVendors[0]);
				}
				else
				{
					VendorRno = Ingred.LastPurchaseVendor(IngredRno);
				}

				lstIngred.Add(new Ingred(IngredRno, DB.Str(dr["Name"]), fStockedFlag, VendorRno, Ingred.VendorName(VendorRno)));
			}
		}
	}

	private void AddServings(int IngredRno, decimal Scaler, int JobRno, string Customer, int JobServings, string MenuItem, int RecipeRno, string Recipe, decimal RecipeServings, decimal Qty, int UnitRno, string UnitSingle, string UnitPlural, string Subrecipe, List<string> Notes)
	{
		foreach (Ingred Ingred in lstIngred)
		{
			if (Ingred.IngredRno == IngredRno && (Ingred.UnitRno == 0 || Ingred.UnitRno == UnitRno))
			{
				Ingred.AddQty(Scaler, JobRno, Customer, JobServings, MenuItem, RecipeRno, Recipe, RecipeServings, Qty, UnitRno, UnitSingle, UnitPlural, Subrecipe);

				if (Notes != null)
				{
					foreach (string Note in Notes)
					{
						if (Note.Length > 0)
						{
							Ingred.AddNote(Note);
						}
					}
				}
			}
		}
	}

	private void AddSubrecipe(int ParentRecipeRno, int SubrecipeRno, decimal Scaler, int JobRno, string Customer, int JobServings, string MenuItem, int RecipeRno, string Recipe, decimal Qty, int UnitRno, string UnitSingle, string UnitPlural, string Subrecipe, List<String> Notes)
	{
		string Sql = string.Format(
			"Select\n" +
			"r.RecipeRno, r.Name As Subrecipe,\n" +
			"r.NumServings, r.MenServingRatio, r.WomenServingRatio, r.ChildServingRatio,\n" +
			"r.YieldQty, r.YieldUnitRno, (Select UnitPlural From Units Where UnitRno = r.YieldUnitRno) As YieldUnit,\n" +
			"r.PortionQty, r.PortionUnitRno, (Select UnitPlural From Units Where UnitRno = r.PortionUnitRno) As PortionUnit,\n" +
			"x.IngredRno, x.SubrecipeRno, x.UnitQty, x.UnitRno,\n" +
			"(Select UnitSingle From Units Where UnitRno = x.UnitRno) As UnitSingle,\n" +
			"(Select UnitPlural From Units Where UnitRno = x.UnitRno) As UnitPlural,\n" +
			"i.Name, i.PrefVendors, i.StockedFlg,\n" + 
			"(Select Name From Recipes Where RecipeRno = {1}) As ParentRecipe,\n" +
			"(Select UnitPlural From Units Where UnitRno = {2}) As ParentUnit\n" +
			"From Recipes r\n" +
			"Inner join RecipeIngredXref x On r.RecipeRno = x.RecipeRno\n" +
			"Left Join Ingredients i On x.IngredRno = i.IngredRno\n" +
			"Where r.RecipeRno = {0}\n" +
            "And IsNull(i.NonPurchaseFlg, 0) = 0",
            SubrecipeRno,
			ParentRecipeRno,
			UnitRno);

		try 
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				int IngredRno = DB.Int32(dr["IngredRno"]);
				int xSubrecipeRno = DB.Int32(dr["SubrecipeRno"]);
				decimal YieldQty = DB.Dec(dr["YieldQty"]);
				int YieldUnitRno = DB.Int32(dr["YieldUnitRno"]);

				Ingred Ingred = new Ingred(0);
				decimal ConversionScaler = Ingred.ConversionScaler(UnitRno, YieldUnitRno);
				decimal NextScaler = Scaler * Qty * ConversionScaler;

				if (ConversionScaler == 0)
				{
					string Note = string.Format("<a href='Recipes.aspx?Rno={0}' target='Fix'><i class=\"icon-warning-sign\"></i> <b>{2}'s</b> unit needs to use subrecipe's yield unit ({3}) or a known unit.</a>", ParentRecipeRno, DB.Str(dr["ParentUnit"]), DB.Str(dr["Subrecipe"]), DB.Str(dr["YieldUnit"]));
					Notes.Add(Note);
				}

				// if an ingredient
				if (IngredRno != 0)
				{
					AddIngredient(dr);
					AddServings(IngredRno, NextScaler, JobRno, Customer, JobServings, MenuItem, RecipeRno, Recipe, YieldQty, DB.Dec(dr["UnitQty"]), DB.Int32(dr["UnitRno"]), DB.Str(dr["UnitSingle"]), DB.Str(dr["UnitPlural"]), Subrecipe + " &lt; " + DB.Str(dr["Subrecipe"]), Notes);
				}

				// if a subrecipe
				if (xSubrecipeRno != 0)
				{
					if (YieldQty == 0)
					{
						string Note = string.Format("<a href='Recipes.aspx?Rno={0}' target='_blank'><i class=\"icon-warning-sign\"></i> Recipe for <b>{1}</b> is missing the yield quantity.</a>", SubrecipeRno, DB.Str(dr["Subrecipe"]));
						Notes.Add(Note);
						YieldQty = 1;
					}
					AddSubrecipe(SubrecipeRno, xSubrecipeRno, NextScaler / YieldQty, JobRno, Customer, JobServings, MenuItem, RecipeRno, Recipe, DB.Dec(dr["UnitQty"]), DB.Int32(dr["UnitRno"]), DB.Str(dr["UnitSingle"]), DB.Str(dr["UnitPlural"]), Subrecipe + " &lt; " + DB.Str(dr["Subrecipe"]), Notes);
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
    }

    protected void btnSave_Click(object sender, System.EventArgs e)
    {
        DateTime Tm = DateTime.Now;

        fReport = true;
        dtBeg = Str.DtTm(hfBegDate.Value);
        dtEnd = Str.DtTm(hfEndDate.Value);

        string Sql = string.Format(
            "Delete From ShoppingListDetails Where ShoppingListRno in (Select ShoppingListRno From ShoppingLists Where BegDt = {0} And EndDt = {1});\n" +
            "Delete From ShoppingLists Where BegDt = {0} And EndDt = {1}",
            DB.PutDtTm(dtBeg),
            DB.PutDtTm(dtEnd));
        try
        {
            db.Exec(Sql);

            int Count = Parm.Int("hfParmCount");
            for (int i = 0; i < Count; i++)
            {
                Int32 IngredRno = Parm.Int("hfIngredRno" + i);
                bool fStocked = Parm.Bool("hfStocked" + i);
                decimal StockedPurchaseQty = Parm.Dec("hfStockedQty" + i);
                decimal PurchaseQty = Parm.Dec("hfQty" + i);
                decimal PurchaseUnitQty = Parm.Dec("hfUnitQty" + i);
                Int32 PurchaseUnitRno = Parm.Int("hfUnitRno" + i);
                Int32 VendorRno = Parm.Int("hfVendorRno" + i);
                Int32 ShoppingListRno;

                Sql = string.Format(
                    "Select ShoppingListRno From ShoppingLists Where VendorRno = {0} And BegDt = {1} And EndDt = {2}", 
                    VendorRno, 
                    DB.PutDtTm(dtBeg),
                    DB.PutDtTm(dtEnd));
                ShoppingListRno = db.SqlNum(Sql);
                if (ShoppingListRno == 0)
                {
                    Sql = string.Format(
                        "Insert Into ShoppingLists (VendorRno, BegDt, EndDt, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) " +
                        "Values ({0}, {1}, {2}, {3}, {4}, {5}, {6});" +
                        "Select @@Identity",
                        VendorRno,
                        DB.PutDtTm(dtBeg),
                        DB.PutDtTm(dtEnd),
                        DB.PutDtTm(Tm),
                        DB.PutStr(g.User),
                        DB.PutDtTm(Tm),
                        DB.PutStr(g.User));
                    ShoppingListRno = db.SqlNum(Sql);
                }

                Sql = string.Format(
                    "Insert Into ShoppingListDetails (ShoppingListRno, IngredRno, PurchaseQty, PurchaseUnitQty, PurchaseUnitRno, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) " +
                    "Values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                    ShoppingListRno,
                    IngredRno,
                    (fStocked ? StockedPurchaseQty : PurchaseQty),
                    PurchaseUnitQty,
                    PurchaseUnitRno,
                    DB.PutDtTm(Tm),
                    DB.PutStr(g.User),
                    DB.PutDtTm(Tm),
                    DB.PutStr(g.User));
                db.Exec(Sql);
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    private void FindVendorInfo()
    {
        VendorInfo = string.Empty;
        string Sql = "Select VendorRno, Name From Vendors Where IsNull(HideFlg, 0) = 0 Order By Name";
        try
        {
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                VendorInfo += string.Format("{{rno:{0},label:\"{1}\"}},", DB.Int32(dr["VendorRno"]), DB.Str(dr["Name"]));
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    #endregion Report
}