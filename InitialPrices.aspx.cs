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
using System.Text;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class InitialPrices : System.Web.UI.Page
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
			Setup();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
	}

	private void Setup()
	{		
	}

	protected void btnInitialize_Click(object sender, EventArgs e)
	{
		string Sql = "Select IngredRno From Ingredients Order By IngredRno";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				int IngredRno = DB.Int32(dr["IngredRno"]);

				Ingred.UpdateWithLastPrice(IngredRno);
				Response.Write(IngredRno.ToString() + "<br/>");
			}

			Sql = "Update mcJobMenuItems Set ServingQuote = ServingPrice";
			db.Exec(Sql);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

}

