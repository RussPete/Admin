using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class Service_Ingredient : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		// insure the user is logged in
		g.User = (string)Session["User"];
		if (!(g.User == null || g.User == ""))
		{
			switch (Parm.Str("Action"))
			{
				case "SetNonPurchase":
					SetNonPurchase(Parm.Int("Rno"), Parm.Bool("NonPurchase"));
					break;
			}
		}
    }

	private void SetNonPurchase(int IngredientRno, bool NonPurchase)
	{
		if (IngredientRno > 0)
		{
			DB db = new DB();

			string Sql = string.Format(
				"Update Ingredients Set NonPurchaseFlg = {1} " + 
				"Where IngredRno = {0}", 
				IngredientRno, 
				DB.PutBool(NonPurchase));
			db.Exec(Sql);
		}
	}
}