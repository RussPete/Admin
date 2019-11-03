using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using System.Diagnostics;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class StockedItems : System.Web.UI.Page
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

	protected void btnQuantities_Click(object sender, System.EventArgs e)
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
        Debug.WriteLine("> Report");
        lstIngred = new List<Ingred>();

        hfBegDate.Value = dtBeg.ToString();
        hfEndDate.Value = dtEnd.ToString();

        ltlBegDate.Text = Fmt.Dt(dtBeg);
		ltlEndDate.Text = Fmt.Dt(dtEnd);

		string Sql = string.Format(
			"Select\n" +
			"j.JobRno, Coalesce(cu.Name, c.Name) as Customer, j.NumMenServing, NumWomenServing, NumChildServing, f.Qty, f.MenuItem,\n" +
			"r.RecipeRno, r.Name As Recipe,\n" +
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
            string Html = string.Empty;
            int ID = 0;
            //Response.Write(Sql + "<br/>");
            DataTable dtRecipeIngredXref = db.DataTable(Sql);
			if (dtRecipeIngredXref.Rows.Count > 0)
			{
				Ingred.LoadVendors();
				Ingred.LoadIngredConversions();
				Ingred.LoadIngredPurchases();

                // find the ingredients for the recipes
                Sql = string.Format(
                    "Select IngredRno, Name, StockedFlg, PrefVendors\n" +
                    "From Ingredients i\n" +
                    "Where IsNull(NonPurchaseFlg, 0) = 0\n" +
                    "and IsNull(HideFlg, 0) = 0");
                    //IngredRno In ({ 0})\n" +
                    //"And 
                    //Str.Join(DB.StrArray(dtRecipeIngredXref, "IngredRno"), ","));
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

                // sort by vendor, ingredient
                lstIngred.Sort(CompareIngredByName);

				string PrevVendor = string.Empty;

                //Html += ReportHeader();

				foreach (Ingred CurrIngred in lstIngred)
				{
                    ID++;
                    string sID = ID.ToString();

					CurrIngred.FindPurchaseInfo();
                    CurrIngred.Note = string.Empty;     // remove any "No prior receipt found" note

                    // replace stocked items quantity
                    if (CurrIngred.StockedFlg)
                    {
                        CurrIngred.AdjustStockedItemsQuantity(db);
                    }

                    /*
                    if (CurrIngred.fFoundConversion)
                    {
                        Html += string.Format(
                            "\t\t<tr>\n" +
                            "\t\t\t<td><input id='hfIngredRno{0}' type='hidden' value='{1}'/><img src='Images/Box.gif'/></td>\n" +
                            "\t\t\t<td class='Qty'>{2}</td>\n" +
                            "\t\t\t<td>{3}{4}</td>\n" +
                            "\t\t\t<td title='{11}' class='qtp Ingred'><a href='Ingredients.aspx?Rno={1}' target='_blank' tabindex='-1'>{5}</a></td>\n" +
                            "\t\t\t<td><input id='txtQty{0}' type=text class='Qty' value='{6}'/></td>\n" +
                            "\t\t\t<td><input id='txtUnitQty{0}' type=text class='Qty UnitQty' value='{7}'/></td>\n" +
                            "\t\t\t<td><input id='hfUnitRno{0}' type=hidden value='{8}' class='UnitRno'/><input id='txtUnit{0}' type=text class='Unit' value='{9}'/></td>\n" +
                            "\t\t\t<td>{10}</td>\n" +
                            "\t\t</tr>\n",
                            ID,
                            CurrIngred.IngredRno,
                            Str.ShowFract(CurrIngred.PurchaseQty * CurrIngred.PurchaseUnitQty, 2),
                            string.Empty,
                            (CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural),
                            5 CurrIngred.Name.Replace("'", "&apos;"),
                            Math.Ceiling(CurrIngred.PurchaseQty),
                            (CurrIngred.PurchaseUnitQty != 1 ? CurrIngred.PurchaseUnitQty.ToString() : string.Empty),
                            CurrIngred.PurchaseUnitRno,
                            (CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural),
                            10 CurrIngred.Vendor,
                            string.Empty,
                            CurrIngred.AddlInfo);
                    }
                    else
                    {
                        Html += string.Format(
                            "\t\t<tr>\n" +
                            "\t\t\t<td><input id='hfIngredRno{0}' type='hidden' value='{1}'/><img src='Images/Box.gif'/></td>\n" +
                            "\t\t\t<td class='Qty'>{2}</td>\n" +
                            "\t\t\t<td>{3}{4}</td>\n" +
                            "\t\t\t<td title='{11}' class='qtp'>{5}</td>\n" +
                            "\t\t\t<td><input id='txtQty{0}' type=text class='Qty' value='{6}'/></td>\n" +
                            "\t\t\t<td><input id='txtUnitQty{0}' type=text class='Qty UnitQty' value='{7}'/></td>\n" +
                            "\t\t\t<td><input id='hfUnitRno{0}' type=hidden value='{8}' class='UnitRno'/><input id='txtUnit{0}' type=text class='Unit' value='{9}'/></td>\n" +
                            "\t\t\t<td>{10}</td>\n" +
                            "\t\t</tr>\n",
                            ID,
                            CurrIngred.IngredRno,
                            Str.ShowFract(CurrIngred.Qty, 2),
                            string.Empty,
                            (CurrIngred.Qty <= 1 ? CurrIngred.UnitSingle : CurrIngred.UnitPlural),
                            CurrIngred.Name.Replace("'", "&apos;"),
                            "?",
                            (CurrIngred.PurchaseUnitQty != 1 ? CurrIngred.PurchaseUnitQty + " " : string.Empty),
                            CurrIngred.PurchaseUnitRno,
                            (CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural),
                            CurrIngred.Vendor,
                            string.Empty,
                            CurrIngred.AddlInfo);

                        if (CurrIngred.PurchaseUnitRno != 0)
                        {
                            CurrIngred.AddNote(
                                string.Format(
                                    "<a href='Ingredients.aspx?Rno={2}' target='_blank' tabindex='-1'><i class=\"icon-warning-sign\"></i> Need conversion from {0} to {1}.</a>", 
                                    CurrIngred.UnitPlural, 
                                    CurrIngred.PurchaseUnitPlural, 
                                    CurrIngred.IngredRno));
                        }
                     }

                    if (CurrIngred.Note.Length > 0)
					{
						Html += string.Format(
						    "<tr><td colspan='9' class='Notes'><ul>{0}</ul></td></tr>\n",
						    CurrIngred.Note);
                    }
                    */

                    TableRow tr = new TableRow();
                    tblReport.Rows.Add(tr);

                    TableCell tc = new TableCell();
                    tr.Cells.Add(tc);

                    tc.Controls.Add(new HiddenField() { ID = "hfIngredRno" + sID, Value = CurrIngred.IngredRno.ToString() });
                    tc.Controls.Add(new Image() { ImageUrl = "Images/Box.gif" });

                    tr.Cells.Add(new TableCell() { CssClass = "Qty", Text = Str.ShowFract(CurrIngred.PurchaseQty * CurrIngred.PurchaseUnitQty, 2) });
                    tr.Cells.Add(new TableCell() { Text = (CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural) });

                    string Name = CurrIngred.Name.Replace("'", "&apos;");
                    if (CurrIngred.fFoundConversion)
                    {
                        tc = new TableCell() { ToolTip = CurrIngred.AddlInfo, CssClass = "qtp Ingred" };
                        tr.Cells.Add(tc);
                        tc.Controls.Add(new HyperLink() { NavigateUrl = "Ingredients.aspx?Rno=" + CurrIngred.IngredRno, Target = "_blank", TabIndex = -1, Text = Name });
                    }
                    else
                    {
                        tr.Cells.Add(new TableCell() { ToolTip = CurrIngred.AddlInfo, CssClass = "qtp", Text = Name });
                    }

                    tr.Cells.Add(new TableCell() { CssClass="Qty", Text = Math.Ceiling(CurrIngred.PurchaseQty).ToString() });
                    tr.Cells.Add(new TableCell()
                    {
                        CssClass = "UnitQty",
                        Text = string.Format("{0} {1}",
                            (CurrIngred.PurchaseUnitQty != 1 ? Math.Ceiling(CurrIngred.PurchaseUnitQty).ToString() : string.Empty),
                            (CurrIngred.PurchaseQty <= 1 ? CurrIngred.PurchaseUnitSingle : CurrIngred.PurchaseUnitPlural))
                    });

                    tc = new TableCell();
                    tr.Cells.Add(tc);
                    tc.Controls.Add(new TextBox() { ID = "txtQty" + sID, CssClass = "Qty", Text = (CurrIngred.StockedPurchaseQty != 0 ? Math.Ceiling(CurrIngred.StockedPurchaseQty).ToString() : string.Empty) });

                    if (!CurrIngred.fFoundConversion && CurrIngred.PurchaseUnitRno != 0 && CurrIngred.UnitPlural.Length > 0 && CurrIngred.PurchaseUnitPlural.Length > 0)
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
                        //Html += string.Format(
                        //	"<tr><td colspan='9' class='Notes'><ul>{0}</ul></td></tr>\n",
                        //	CurrIngred.Note);
                        tr = new TableRow();
                        tblReport.Rows.Add(tr);
                        tr.Cells.Add(new TableCell() { ColumnSpan = 8, CssClass = "Notes", Text = string.Format("<ul>{0}</ul>", CurrIngred.Note) });
                    }
				}
			}
			//Html += "</table>";
            hfParmCount.Value = ID.ToString();
			//ltlReport.Text = Html;
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
        Debug.WriteLine("< Report");
    }

    private static int CompareIngredByName(Ingred A, Ingred B)
    {
        return A.Name.CompareTo(B.Name);
    }

    private string NewVendor(string Vendor, bool First)
	{
        Debug.WriteLine("> NewVendor");
        string Html = string.Empty;

		if (First)
		{
		}
		else
		{
			Html += "</table>";
		}

		Html += string.Format(
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

        Debug.WriteLine("< NewVendor");
        return Html;
	}

	private string FinishVendor()
	{
		return "</table>\n";
	}

	private string ReportHeader()
	{
        Debug.WriteLine("> ReportHeader");
        string Html =
			"<table class='Report'>\n" +
			"\t<thead>\n" +
			"\t\t<tr>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th colspan='2' class='Center'>Jobs</th>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th colspan='3' class='Center'>Purchase</th>\n" +
			"\t\t</tr>\n" +
			"\t\t<tr class='Underline'>\n" +
			"\t\t\t<th></th>\n" +
			"\t\t\t<th class='Qty'>Qty</th>\n" +
			"\t\t\t<th>Unit</th>\n" +
			"\t\t\t<th>Ingredient</th>\n" +
			"\t\t\t<th class='Qty'>Qty</th>\n" +
            "\t\t\t<th class='Qty'>Unit Qty</th>\n" +
            "\t\t\t<th>Unit</th>\n" +
			"\t\t\t<th>Vendor</th>\n" +
			"\t\t</tr>\n" +
			"\t</thead>\n";

        Debug.WriteLine("< ReportHeader");
        return Html;
	}

	private void AddIngredient(DataRow dr)
	{
        Debug.WriteLine("> AddIngredient");
        int VendorRno = 0;
		int IngredRno = DB.Int32(dr["IngredRno"]);
		bool fFound = false;

        // check for stocked flag
        bool fStockedFlag = DB.Bool(dr["StockedFlg"]);
        if (fStockedFlag)
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
        Debug.WriteLine("< AddIngredient");
    }

    private void AddServings(int IngredRno, decimal Scaler, int JobRno, string Customer, int JobServings, string MenuItem, int RecipeRno, string Recipe, decimal RecipeServings, decimal Qty, int UnitRno, string UnitSingle, string UnitPlural, string Subrecipe, List<string> Notes)
	{
        Debug.WriteLine("> AddServings");
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
        Debug.WriteLine("< AddServings");
    }

    private void AddSubrecipe(int ParentRecipeRno, int SubrecipeRno, decimal Scaler, int JobRno, string Customer, int JobServings, string MenuItem, int RecipeRno, string Recipe, decimal Qty, int UnitRno, string UnitSingle, string UnitPlural, string Subrecipe, List<String> Notes)
	{
        Debug.WriteLine("> AddSubrecipe");
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
					string Note = string.Format("<a href='Recipes.aspx?Rno={0}' target='Fix' tabindex='-1'><i class=\"icon-warning-sign\"></i> <b>{2}'s</b> unit needs to use subrecipe's yield unit ({3}) or a known unit.</a>", ParentRecipeRno, DB.Str(dr["ParentUnit"]), DB.Str(dr["Subrecipe"]), DB.Str(dr["YieldUnit"]));
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
						string Note = string.Format("<a href='Recipes.aspx?Rno={0}' target='_blank' tabindex='-1'><i class=\"icon-warning-sign\"></i> Recipe for <b>{1}</b> is missing the yield quantity.</a>", SubrecipeRno, DB.Str(dr["Subrecipe"]));
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
        Debug.WriteLine("< AddSubrecipe");
    }

    protected void btnSave_Click(object sender, System.EventArgs e)
    {
        int Count = Parm.Int("hfParmCount");
        for (int i = 1; i <= Count; i++)
        {
            int IngredRno = Parm.Int("hfIngredRno" + i);
            decimal Qty = Parm.Dec("txtQty" + i);

            if (IngredRno != 0)
            {
                string Sql = string.Format(
                    "Update Ingredients Set StockedPurchaseQty = {1} Where IngredRno = {0}",
                    IngredRno,
                    Qty);
                try
                {
                    db.Exec(Sql);
                }
                catch (Exception Ex)
                {
                    Err Err = new Err(Ex, Sql);
                    Response.Write(Err.Html());
                }
            }
        }

        fReport = true;

        dtBeg = Str.DtTm(hfBegDate.Value);
        dtEnd = Str.DtTm(hfEndDate.Value);
    }

    protected void btnReset_Click(object sender, System.EventArgs e)
    {
        string Sql = "Update Ingredients Set StockedPurchaseQty = null Where StockedFlg = 1";
        try
        {
            db.Exec(Sql);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        fReport = true;

        dtBeg = Str.DtTm(hfBegDate.Value);
        dtEnd = Str.DtTm(hfEndDate.Value);
    }

    protected string UnitData()
    {
        return Misc.UnitData(db);
    }

    #endregion Report
}