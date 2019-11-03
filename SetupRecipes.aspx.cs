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

public partial class SetupRecipes : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.Recipes.Title;
			ClearData();
			Setup();
		}
	}

    private void Setup()
    {
		string Sql = "Select Top 1 SettingRno From Settings Order By SettingRno";
		try 
		{
			int Rno = db.SqlNum(Sql);
			if (Rno != 0)
			{
				GetData(Rno);
			}
		}
		catch (Exception Ex)
		{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
		}
    }
    
	protected void ClearData()
	{
		txtBaseCostPct.Text = string.Empty;
	}

    protected void GetData(int Rno)
    {
        string Sql = string.Format("Select * From Settings Where SettingRno = {0}", Rno);

		ClearData();

        try
        {
            DataTable dt = db.DataTable(Sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

				hfRno.Value = Rno.ToString();
				txtBaseCostPct.Text = Fmt.Num(DB.Dec(dr["BaseCostPct"]), 4);
				txtAsIsBaseCostPct.Text = Fmt.Num(DB.Dec(dr["AsIsBaseCostPct"]), 4);
			}
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

	protected void SaveData()
	{
		int Rno = Str.Num(hfRno.Value);
		string Sql = string.Empty;

		try
		{
			Sql = string.Format(
				"Update Settings Set " +
				"BaseCostPct = {1}, " +
				"AsIsBaseCostPct = {2} " +
				"Where SettingRno = {0}",
				Rno,
				Str.Dec(txtBaseCostPct.Text),
				Str.Dec(txtAsIsBaseCostPct.Text));
			db.Exec(Sql);

			// recalculate the menu item price after changing the default cost percentages
			Sql = 
				"Update mcJobMenuItems Set ServingPrice = r.PortionPrice / (IsNull(r.BaseCostPct, (Select Top 1 Case When IsNull(r.MenuItemAsIsFlg, 0) = 1 Then AsIsBaseCostPct Else BaseCostPct End From Settings)) / 100) " + 
				"From mcJobMenuItems m Inner Join Recipes r On m.RecipeRno = r.RecipeRno " +
				"Where r.BaseCostPct Is Null";
			db.Exec(Sql);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		SaveData();
	}
}