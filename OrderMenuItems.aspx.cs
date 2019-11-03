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

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class OrderMenuItems : System.Web.UI.Page
{
	private DB db;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		SaveData();

		db.Close();
	}


	protected void SaveData()
	{
		string Sql = string.Empty;

		try
		{
			if (Parm.Bool("hfMenuItemsSaved"))
			{
				int NumMenuItems = Parm.Int("hfNumMenuItems");
				for (int i = 0; i < NumMenuItems; i++)
				{
					Sql = string.Format("Update mcJobMenuItems Set CategorySortOrder = {1} Where MenuItemRno = {0}", Parm.Int(string.Format("hfMenuItem{0}", i)), Parm.Int(string.Format("hfSortOrder{0}", i)));
					db.Exec(Sql);
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}
