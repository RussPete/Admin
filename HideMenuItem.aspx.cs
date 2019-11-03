using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class HideMenuItem : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
    {
		int MenuItemRno = Parm.Int("Rno");
		if (MenuItemRno > 0)
		{
			string Sql = string.Empty;
			try
			{
				Sql = string.Format("Update mcJobMenuItems Set HideFlg = 1 Where MenuItemRno = {0}", MenuItemRno);
				DB db = new DB();

				db.Exec(Sql);
				db.Close();
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
		}
	}
}
