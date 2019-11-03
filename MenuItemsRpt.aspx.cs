using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class MenuItemsRpt : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	
	protected void Page_Load(object sender, EventArgs e)
    {
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.MenuSetup.Title;
			Setup();
		}
	}

	private void Setup()
	{
		string Sql = 
			"Select m.Category, m.MenuItem, l.Name " +
			"From mcJobMenuItems m " +
			"Inner Join mcJobMenuCategories c On m.Category = c.Category " +
			"Left Join KitchenLocations l On m.KitchenLocRno = l.KitchenLocRno " +
			"Where (m.HideFlg Is Null Or m.HideFlg = 0) " +
			"And (c.HideFlg Is Null Or c.HideFlg = 0) " +
			"Order By m.Category, m.MenuItem ";
		try 
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				HtmlGenericControl li = new HtmlGenericControl("li");

				HtmlGenericControl Category = new HtmlGenericControl("div");
				Category.InnerText = DB.Str(dr["Category"]);
				li.Controls.Add(Category);

				HtmlGenericControl MenuItem = new HtmlGenericControl("div");
				MenuItem.InnerText = DB.Str(dr["MenuItem"]);
				li.Controls.Add(MenuItem);

				HtmlGenericControl Loction = new HtmlGenericControl("div");
				Loction.InnerText = DB.Str(dr["Name"]);
				li.Controls.Add(Loction);

				ulMenuItems.Controls.Add(li);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}
