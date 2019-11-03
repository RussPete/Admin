using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Globals;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

/// <summary>
/// Summary description for Food
/// </summary>
public class FoodCategories
{
	public enum Type
	{
		Hot,
		Cold,
		Other
	}

	protected Type eType;
	protected int iCategory;
	protected string[] aCategory;
	protected string[] aHot = new string[] { "Meats", "Sides", "Pasta", "Soup", "Sandwich", "Appetizers" };
	protected string[] aCold = new string[] { "Salads", "Condiment", "Bread", "Drink", "Desserts" };
	private DB db;

	public FoodCategories()
	{
		db = new DB();
//		aHot  =
//		aCold = null;
	}

	public string FirstCategory(Type eType)
	{
		string First = null;
		this.eType = eType;
		iCategory = 0;

		SetCategory(eType);

		if (aCategory != null && iCategory < aCategory.Length)
		{
			First = aCategory[iCategory];
		}

		return First;
	}

	public string NextCategory()
	{
		string Next = null;
		if (aCategory != null && iCategory + 1 < aCategory.Length)
		{
			iCategory++;
			Next = aCategory[iCategory];
		}

		return Next;
	}

	public string SqlList(Type eType)
	{
		string List = "";

		SetCategory(eType);

		if (aCategory != null)
		{
			foreach (string Category in aCategory)
			{
				List += (List.Length == 0 ? "'" : ", '") + Category + "'";
			}
		}

		return List;
	}

	private void SetCategory(Type eType)
	{
		aCategory = null;

		switch (eType)
		{
			case Type.Hot:
			case Type.Cold:
				aCategory = GetCategories(eType);
				break;

			default:
				aCategory = null;
				break;
		}
	}

	private string[] GetCategories(Type eType)
	{
		string[] aCategory = (eType == Type.Hot ? aHot : aCold);

		if (aCategory == null)
		{
			string Sql = string.Format(
				"Select Distinct Category From mcJobMenuItems Where {0} = 1",
				(eType == Type.Hot ? "HotFlg" : "ColdFlg"));

			try
			{
				DataTable dt = db.DataTable(Sql);
				int Cnt = dt.Rows.Count;
				int iCategory = 0;
				if (Cnt > 0)
				{
					aCategory = new string[Cnt];
					foreach (DataRow dr in dt.Rows)
					{
						aCategory[iCategory++] = DB.Str(dr["Category"]);
					}
				}
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				HttpContext.Current.Response.Write(Err.Html());
			}

			switch (eType)
			{
				case Type.Hot:
//					aHot = aCategory;
					break;

				case Type.Cold:
//					aCold = aCategory;
					break;
			}
		}

		return aCategory;
	}
}
