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

public partial class HideMenuItems : System.Web.UI.Page
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
		string Sql = "Select MenuItemRno, Category, MenuItem, HideFlg From mcJobMenuItems Where HideFlg Is Null Or HideFlg = 0 Order By Category, MenuItem ";
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

				HtmlGenericControl Hide = new HtmlGenericControl("div");
				HtmlInputCheckBox chkHide = new HtmlInputCheckBox();
				chkHide.Attributes.Add("data-Rno", DB.Int32(dr["MenuItemRno"]).ToString());
				Hide.Controls.Add(chkHide);
				li.Controls.Add(Hide);

				ulMenuItems.Controls.Add(li);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	[WebMethod]
	public static int HideMenuItem(int MenuItemRno)
	{
		Debug.WriteLine("HideMenuItem");
		string Sql = string.Empty;
		int rc = 0;

		try
		{
			rc = 1;
			Sql = string.Format("Update mcJobMenuItems Set HideFlg = 1 Where MenuItemRno = {0}", MenuItemRno);
			DB db = new DB();

			db.Exec(Sql);
			db.Close();
		}
		catch (Exception Ex)
		{
			rc = 3;
			throw Ex;
		}

		return rc;
	}
}
