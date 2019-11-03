using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class Service_MenuItem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		// insure the user is logged in
		g.User = (string)Session["User"];
		if (!(g.User == null || g.User == ""))
		{
			switch (Parm.Str("Action"))
			{
				case "SetAsHidden":
					SetAsHidden(Parm.Int("Rno"), Parm.Bool("Hide"));
					break;

				case "MarkAsIs":
					MarkAsIs(Parm.Int("Rno"), Parm.Bool("AsIs"));
					break;

				case "Recipe":
					Recipe(Parm.Int("Rno"), Parm.Int("RecipeRno"));
					break;
			}
		}
    }

	private void SetAsHidden(int MenuItemRno, bool Hide)
	{
		if (MenuItemRno > 0)
		{
			DB db = new DB();

			string Sql = string.Format(
				"Update mcJobMenuItems Set HideFlg = {1} " + 
				"Where MenuItemRno In " +
				"(Select MenuItemRno From mcJobMenuItems where MenuItem = " +
				"(Select MenuItem From mcJobMenuItems Where MenuItemRno = {0}))", 
				MenuItemRno, 
				DB.PutBool(Hide));
			db.Exec(Sql);
		}
	}

	private void MarkAsIs(int MenuItemRno, bool AsIs)
	{
		if (MenuItemRno > 0)
		{
            //DB db = new DB();

            //string Sql = string.Format(
            //    "Update mcJobMenuItems Set AsIsFlg = {1} " + 
            //    "Where MenuItemRno In " +
            //    "(Select MenuItemRno From mcJobMenuItems where MenuItem = " +
            //    "(Select MenuItem From mcJobMenuItems Where MenuItemRno = {0}))", 
            //    MenuItemRno, 
            //    DB.PutBool(AsIs));
            //db.Exec(Sql);

            Misc.SetAsIsData(MenuItemRno, AsIs);
		}
	}

	private void Recipe(int MenuItemRno, int RecipeRno)
	{
		if (MenuItemRno > 0)
		{
			DB db = new DB();

			string Sql = string.Format(
				"Update mcJobMenuItems Set RecipeRno = {1} " +
				"Where MenuItemRno In " +
				"(Select MenuItemRno From mcJobMenuItems where MenuItem = " +
				"(Select MenuItem From mcJobMenuItems Where MenuItemRno = {0}))",
				MenuItemRno,
				RecipeRno);
			db.Exec(Sql);
		}
	}
}