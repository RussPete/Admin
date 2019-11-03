using Globals;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Linq;
using System.Web;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

/// <summary>
/// Summary description for Ingred
/// </summary>
public class Ingred : IComparable<Ingred>
{
	#region Ingred

	public int IngredRno;
	public string Name;
	public bool StockedFlg;
	public int VendorRno;
	public string Vendor;
	public decimal Qty;
	public int UnitRno;
	public string UnitSingle;
	public string UnitPlural;
	public decimal PurchaseQty;
	public decimal PurchaseUnitQty;
	public int PurchaseUnitRno;
	public string PurchaseUnitSingle;
	public string PurchaseUnitPlural;
    public decimal StockedPurchaseQty;
	public decimal LastPriceQty;
	public decimal LastPriceUnitQty;
	public decimal LastPrice;
	public decimal ExtPrice;
	public bool fFoundConversion;
	public string Note;
	private IList<IngredConv> lstIngredConversions;
	private IList<AddlInfoData> lstAddlInfo;

	protected static VendorData[] Vendors;
	protected static IngredConv[] IngredConvs;
	protected static IngredPurchase[] IngredPurchases;

	private bool fShowConv = false;

	public Ingred(int IngredRno)
	{
		Init(IngredRno, string.Empty, false, 0, string.Empty);
	}

	public Ingred(int IngredRno, string Name, bool StockedFlg, int VendorRno, string Vendor)
	{
		Init(IngredRno, Name, StockedFlg, VendorRno, Vendor);
	}

	private void Init(int IngredRno, string Name, bool StockedFlg, int VendorRno, string Vendor)
	{
		if (Vendor == null || Vendor.Length == 0)
		{
			//Vendor = "&lt;Undefined Vendor&gt;";
			Vendor = "* No Vendor *";
		}

		this.IngredRno = IngredRno;
		this.Name = Name;
		this.StockedFlg = StockedFlg;
		this.VendorRno = VendorRno;
		this.Vendor = Vendor;
		this.Qty = 0;
		this.UnitRno = 0;
		this.UnitSingle =
		this.UnitPlural = String.Empty;
		this.PurchaseQty =
		this.PurchaseUnitQty = 0;
		this.PurchaseUnitRno = 0;
		this.PurchaseUnitSingle =
		this.PurchaseUnitPlural = string.Empty;
		this.LastPriceQty =
		this.LastPriceUnitQty =
		this.LastPrice =
		this.ExtPrice = 0;
		this.fFoundConversion = false;
		this.Note = string.Empty;
		this.lstAddlInfo = new List<AddlInfoData>();
	}

	public void AddQty(decimal Scaler, int JobRno, string Customer, int JobServings, string MenuItem, int RecipeRno, string Recipe, decimal RecipeServings, decimal Qty, int UnitRno, string UnitSingle, string UnitPlural, string Subrecipe)
	{
		if (RecipeServings != 0)
		{
			decimal ServingQty = Scaler / RecipeServings * Qty;
			this.Qty += ServingQty;
			this.lstAddlInfo.Add(new AddlInfoData(JobRno, Customer, JobServings, MenuItem, RecipeRno, Recipe, ServingQty, UnitSingle, UnitPlural, Subrecipe));
		}
		else
		{
			AddNote(string.Format("<a href='Recipes.aspx?Rno={1}' target='_blank'>Recipe for <b>{0}</b> is missing number of servings.</a>", Recipe, RecipeRno));
			this.lstAddlInfo.Add(new AddlInfoData(JobRno, Customer, JobServings, MenuItem, RecipeRno, Recipe, Qty, UnitSingle, UnitPlural, Subrecipe));
		}
		this.UnitRno = UnitRno;
		this.UnitSingle = UnitSingle;
		this.UnitPlural = UnitPlural;
	}

	public void FindPurchaseInfo()
	{
		bool fFound = false;

		for (int i = 0; i < IngredPurchases.Length; i++)
		{
			if (IngredPurchases[i].VendorRno == VendorRno && IngredPurchases[i].IngredRno == IngredRno)
			{
				LastPrice			= IngredPurchases[i].Price;
				LastPriceQty		=
				PurchaseQty			= IngredPurchases[i].Qty;
				LastPriceUnitQty	=
				PurchaseUnitQty		= IngredPurchases[i].UnitQty;
				PurchaseUnitRno		= IngredPurchases[i].UnitRno;
				PurchaseUnitSingle	= IngredPurchases[i].UnitSingle;
				PurchaseUnitPlural	= IngredPurchases[i].UnitPlural;

				fFound				= true;
				fFoundConversion	= FindConversion();
				break;
			}
		}

		if (!fFound)
		{
			AddNote(IngredRno != 0 ? string.Format("No <a href=\"Purchases.aspx?IngredRno={0}\" target=\"Fix\" tabindex='-1'>prior receipt</a> found.", IngredRno) : "No prior receipt found.");
		}
	}

	private const string NextLevel = "&gt;";
	private string ShowConversion;
	private int StepsRemoved;

	public decimal ConversionScaler(int FromUnitRno, int ToUnitRno)
	{
		decimal Scaler = 0;
		string FromUnit = string.Empty;
		string ToUnit = string.Empty;

		if (IngredConvs == null)
		{
			LoadIngredConversions();
		}

		lstIngredConversions = FindIngredConversions(IngredRno);

		bool fFound = false;
		string Sql = string.Empty;

		try
		{
			ShowConversion = string.Empty;
			StepsRemoved = 0;

			HttpResponse Resp = System.Web.HttpContext.Current.Response;
			if (fShowConv)
			{
				FromUnit = Unit(FromUnitRno);
				ToUnit = Unit(ToUnitRno);
				Resp.Write(string.Format("------------<br/>{0} -> {1}<br/>", FromUnit, ToUnit));
			}

			if (FromUnitRno == ToUnitRno)
			{
				if (fShowConv) Resp.Write("^^^^^^^^^<br/>"); Resp.Flush();
				fFound = true;
				Scaler = 1;
				ShowConversion = string.Format(" = {0}", ToUnit);
				StepsRemoved = 0;
			}
			else
			{
				lstIngredConversions = FindIngredConversions(IngredRno);

				fFound = false;

				if (fShowConv)
				{
					// show the possible conversions
					Resp.Write("<table>");
					foreach (IngredConv IngredConv in lstIngredConversions)
					{
						Resp.Write(string.Format("<tr><td>{4}{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", IngredConv.RecipeQty, IngredConv.RecipeUnit, IngredConv.PurchaseQty, IngredConv.PurchaseUnit, (IngredConv.IngredRno == 0 ? string.Empty : "*")));
					}
					Resp.Write("</table>"); Resp.Flush();
				}

				// clear visited flags
				// see if the purchase unit is found in the list of conversions
				// see if there is a simple match at the first level
				bool fFoundPurchaseUnit = false;
				foreach (IngredConv IngredConv in lstIngredConversions)
				{
					IngredConv.fVisited = false;

					if (IngredConv.PurchaseUnitRno == ToUnitRno)
					{
						fFoundPurchaseUnit = true;
					}

					if (IngredConv.RecipeUnitRno == FromUnitRno && IngredConv.PurchaseUnitRno == ToUnitRno)
					{
						Scaler = (IngredConv.PurchaseQty / IngredConv.RecipeQty);
						ShowConversion = string.Format(" * {0} {1} / {2} {3} = {4} {5}", Str.ShowFract(IngredConv.PurchaseQty), IngredConv.PurchaseUnit, Str.ShowFract(IngredConv.RecipeQty), IngredConv.RecipeUnit, Str.ShowFract(Scaler), ToUnit);
						StepsRemoved = 1;
						fFound = true;
					}
				}

				if (!fFound && fFoundPurchaseUnit)
				{
					foreach (IngredConv IngredConv in lstIngredConversions)
					{
						
						if (IngredConv.RecipeUnitRno == FromUnitRno)
						{
							IngredConv.fVisited = true;
							if ((Scaler = NextConversionScaler(1, ToUnitRno, ToUnit, IngredConv, NextLevel)) != 0)
							{
								StepsRemoved++;
								break;
							}
						}
					}
				}
			}
			if (fShowConv) Resp.Write(string.Format("Found in {0} steps {1} -> {2} => 1 {3}{4}<br/>", StepsRemoved, FromUnit, ToUnit, FromUnit, ShowConversion)); Resp.Flush();

			// add a new ingredient conversion if too many steps, next time it will be much shorter
			if (StepsRemoved > 1 && IngredRno != 0)
			{
				Sql = string.Format(
					"Insert Into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) " +
					"Values ({0}, {1}, {2}, {3}, {4}, GetDate(), {5});" + 
					"Select Scope_Identity()",
					IngredRno,
					1,
					ToUnitRno,
					1 / Scaler,
					FromUnitRno,
					DB.PutStr(g.User));
				int IngredConvRno = DB.DBExec(Sql);

				string PurchaseUnit = DB.DBSqlStr("Select UnitSingle From Units Where UnitRno = " + ToUnitRno);
				string RecipeUnit = DB.DBSqlStr("Select UnitSingle From Units Where UnitRno = " + FromUnitRno);

				Ingred.AddIngredConv(IngredConvRno, IngredRno, 1, ToUnitRno, PurchaseUnit, 1 / Scaler, FromUnitRno, RecipeUnit);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}

		return Scaler;
	}

	protected decimal NextConversionScaler(decimal Scaler, int ToUnitRno, string ToUnit, IngredConv IngredConv, string Level)
	{
		decimal NextScaler = 0;

		HttpResponse Resp = System.Web.HttpContext.Current.Response;
		if (fShowConv) Resp.Write(string.Format("{0}{1} {2} -> {3} {4}<br/>", Level, IngredConv.RecipeQty, IngredConv.RecipeUnit, IngredConv.PurchaseQty, IngredConv.PurchaseUnit)); Resp.Flush();

		if (IngredConv.PurchaseUnitRno == ToUnitRno)
		{
			if (fShowConv) Resp.Write("^-----------^<br/>"); Resp.Flush();
			NextScaler = Scaler * IngredConv.PurchaseQty / IngredConv.RecipeQty;
			ShowConversion = string.Format(" * {0} {1} / {2} {3} = {4} {5}", Str.ShowFract(IngredConv.PurchaseQty), IngredConv.PurchaseUnit, Str.ShowFract(IngredConv.RecipeQty), IngredConv.RecipeUnit, Str.ShowFract(NextScaler), ToUnit);
			StepsRemoved = 1;
		}
		else
		{
			IngredConv.fVisited = true;
			foreach (IngredConv SearchIngredConv in lstIngredConversions)
			{				
				if (!SearchIngredConv.fVisited)
				{
					// find a matching unit, but not the reverse copy of the same record
					if (SearchIngredConv.RecipeUnitRno == IngredConv.PurchaseUnitRno && SearchIngredConv.IngredConvRno != IngredConv.IngredConvRno)
					{
						if ((NextScaler = NextConversionScaler(Scaler * IngredConv.PurchaseQty / IngredConv.RecipeQty, ToUnitRno, ToUnit, SearchIngredConv, Level + NextLevel)) != 0)
						{
							ShowConversion = string.Format(" * {0} {1} / {2} {3}{4}", Str.ShowFract(IngredConv.PurchaseQty), IngredConv.PurchaseUnit, Str.ShowFract(IngredConv.RecipeQty), IngredConv.RecipeUnit, ShowConversion);
							StepsRemoved++;
							break;
						}
					}
				}
			}
		}

		return NextScaler;
	}

	public static string Unit(int UnitRno)
	{
		string Unit = "Not Found";
		string Sql = string.Format("Select UnitSingle From Units Where UnitRno = {0}", UnitRno);

		try
		{
			Unit = DB.DBSqlStr(Sql);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}

		return Unit;
	}

	protected bool FindConversion()
	{
		bool fFound = false;
		string Sql = string.Empty;

		try
		{
			ShowConversion = string.Empty;
			StepsRemoved = 0;

			//AddNote(string.Format("Recipe {0} {1} -> Purchase {2} {3} {4}<br/>", Str.ShowFract(Qty), UnitSingle, PurchaseQty, PurchaseUnitQty, PurchaseUnitSingle));
			HttpResponse Resp = System.Web.HttpContext.Current.Response;

			if (fShowConv) Resp.Write(string.Format("------------<br/>{0} {1} {2} -> {3} {4}<br/>", Name, Str.ShowFract(Qty), UnitSingle, Str.ShowFract(PurchaseQty), PurchaseUnitSingle));

			decimal Scaler = ConversionScaler(UnitRno, PurchaseUnitRno);
			fFound = (Scaler != 0);
			if (PurchaseUnitQty != 0)
			{
				PurchaseQty = Qty * Scaler / PurchaseUnitQty;
			}
			else
			{
				PurchaseQty = Qty * Scaler;
			}
			ShowConversion = string.Format(" = {0} {1} {2}", Str.ShowFract(PurchaseQty), Str.ShowFract(PurchaseUnitQty), PurchaseUnitSingle);


		//	if (UnitRno == PurchaseUnitRno)
		//	{
		//		if (fShowConv) Resp.Write("^^^^^^^^^<br/>"); Resp.Flush();
		//		fFound = true;
		//		PurchaseQty = Qty / PurchaseUnitQty;
		//		ShowConversion = string.Format(" = {0} {1} {2}", Str.ShowFract(PurchaseQty), Str.ShowFract(PurchaseUnitQty), PurchaseUnitSingle);
		//		StepsRemoved = 0;
		//	}
		//	else
		//	{
		//		lstIngredConversions = FindIngredConversions(IngredRno);

		//		fFound = false;

		//		if (fShowConv)
		//		{
		//			// show the possible conversions
		//			Resp.Write("<table>");
		//			foreach (IngredConv IngredConv in lstIngredConversions)
		//			{
		//				Resp.Write(string.Format("<tr><td>{4}{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", IngredConv.RecipeQty, IngredConv.RecipeUnit, IngredConv.PurchaseQty, IngredConv.PurchaseUnit, (IngredConv.IngredRno == 0 ? string.Empty : "*")));
		//			}
		//			Resp.Write("</table>"); Resp.Flush();
		//		}

		//		// clear visited flags
		//		// see if the purchase unit is found in the list of conversions
		//		// see if there is a simple match at the first level
		//		bool fFoundPurchaseUnit = false;
		//		foreach (IngredConv IngredConv in lstIngredConversions)
		//		{
		//			IngredConv.fVisited = false;

		//			if (IngredConv.PurchaseUnitRno == PurchaseUnitRno)
		//			{
		//				fFoundPurchaseUnit = true;
		//			}

		//			if (IngredConv.RecipeUnitRno == UnitRno && IngredConv.PurchaseUnitRno == PurchaseUnitRno)
		//			{
		//				PurchaseQty = (Qty * IngredConv.PurchaseQty / IngredConv.RecipeQty);
		//				PurchaseQty = PurchaseQty / PurchaseUnitQty;
		//				ShowConversion = string.Format(" * {0} {1} / {2} {3} = {4} {5} {6}", Str.ShowFract(IngredConv.PurchaseQty), IngredConv.PurchaseUnit, Str.ShowFract(IngredConv.RecipeQty), IngredConv.RecipeUnit, Str.ShowFract(PurchaseQty), Str.ShowFract(PurchaseUnitQty), PurchaseUnitSingle);
		//				StepsRemoved = 1;
		//				fFound = true;
		//			}
		//		}

		//		if (!fFound && fFoundPurchaseUnit)
		//		{
		//			for (int i = 0; i < lstIngredConversions.Count; i++)
		//			{
		//				IngredConv IngredConv = lstIngredConversions[i];
		//				if (IngredConv.RecipeUnitRno == UnitRno)
		//				{
		//					IngredConv.fVisited = true;
		//					if (fFound = FindNextConversion(Qty, IngredConv, NextLevel))
		//					{
		//						StepsRemoved++;
		//						break;
		//					}
		//				}
		//			}
		//		}
		//	}
		//	if (fShowConv) Resp.Write(string.Format("Found in {9} steps {0} {1} {2} -> {3} {4} {5} => {6} {7}{8}<br/>", fFound, Str.ShowFract(Qty), UnitSingle, Str.ShowFract(PurchaseQty), Str.ShowFract(PurchaseUnitQty), PurchaseUnitSingle, Str.ShowFract(Qty), UnitSingle, ShowConversion, StepsRemoved)); Resp.Flush();

		//	// add a new ingredient conversion if too many steps, next time it will be much shorter
		//	if (StepsRemoved > 1)
		//	{
		//		Sql = string.Format(
		//			"Insert Into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeQty, RecipeUnitRno, CreatedDtTm, CreatedUser) " +
		//			"Values ({0}, {1}, {2}, {3}, {4}, GetDate(), {5})",
		//			IngredRno,
		//			PurchaseQty * PurchaseUnitQty,
		//			PurchaseUnitRno,
		//			Qty,
		//			UnitRno,
		//			DB.PutStr(g.User));
		//		DB.DBExec(Sql);
		//	}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}

		return fFound;
	}

	protected bool FindNextConversion(decimal ConvQty, IngredConv IngredConv, string Level)
	{
		bool fFound = false;

		HttpResponse Resp = System.Web.HttpContext.Current.Response;
		if (fShowConv) Resp.Write(string.Format("{0}{1} {2} -> {3} {4}<br/>", Level, IngredConv.RecipeQty, IngredConv.RecipeUnit, IngredConv.PurchaseQty, IngredConv.PurchaseUnit)); Resp.Flush();

		if (IngredConv.PurchaseUnitRno == PurchaseUnitRno)
		{
			if (fShowConv) Resp.Write("^-----------^<br/>"); Resp.Flush();
			PurchaseQty = (ConvQty * IngredConv.PurchaseQty / IngredConv.RecipeQty);
			PurchaseQty = PurchaseQty / PurchaseUnitQty;
			ShowConversion = string.Format(" * {0} {1} / {2} {3} = {4} {5} {6}", Str.ShowFract(IngredConv.PurchaseQty), IngredConv.PurchaseUnit, Str.ShowFract(IngredConv.RecipeQty), IngredConv.RecipeUnit, Str.ShowFract(PurchaseQty), Str.ShowFract(PurchaseUnitQty), PurchaseUnitSingle);
			StepsRemoved = 1;
			fFound = true;
		}
		else
		{
			IngredConv.fVisited = true;
			for (int i = 0; i < lstIngredConversions.Count; i++)
			{
				IngredConv SearchIngredConv = lstIngredConversions[i];

				if (!SearchIngredConv.fVisited)
				{
					// find a matching unit, but not the reverse copy of the same record
					if (SearchIngredConv.RecipeUnitRno == IngredConv.PurchaseUnitRno && SearchIngredConv.IngredConvRno != IngredConv.IngredConvRno)
					{
						if (fFound = FindNextConversion(ConvQty * IngredConv.PurchaseQty / IngredConv.RecipeQty, SearchIngredConv, Level + NextLevel))
						{
							ShowConversion = string.Format(" * {0} {1} / {2} {3}{4}", Str.ShowFract(IngredConv.PurchaseQty), IngredConv.PurchaseUnit, Str.ShowFract(IngredConv.RecipeQty), IngredConv.RecipeUnit, ShowConversion);
							StepsRemoved++;
							break;
						}
					}
				}
			}
		}

		return fFound;
	}


	// sort by vendor, ingredient
	public int CompareTo(Ingred Other)
	{
		int rc = 1;

		if (Other != null)
		{
			rc = Vendor.CompareTo(Other.Vendor);

			if (rc == 0)
			{
				//rc = StockedFlg.CompareTo(Other.StockedFlg);

				//if (rc == 0)
				//{
					rc = Name.CompareTo(Other.Name);
				//}
			}
		}

		return rc;
	}

	public void AddNote(string Note)
	{
		if (!this.Note.Contains(Note))
		{
			this.Note += string.Format("<li>{0}</li>\n", Note);
		}
	}

	public string AddlInfo
	{
		get
		{
			string Info = "<table>\n";
			foreach (AddlInfoData Addl in lstAddlInfo)
			{
				Info += string.Format(
					"<tr><td>Job: {0} {1}</td><td>Qty:{2}</td><td>{3}</td><td class=\"Qty\">{4}</td><td>{5}</td>", 
					Addl.JobRno,
                    Addl.Customer.Replace("'", "&apos;"),
					Addl.JobServings,
					Addl.MenuItem.Replace("'", "&apos;") + 
					(Addl.MenuItem.ToLower() != Addl.Recipe.ToLower() ? " <span class=\"Recipe\">(" + Addl.Recipe.Replace("'", "&apos;") + ")</span>" : string.Empty) +
					(Addl.Subrecipe.Length > 0 ? Addl.Subrecipe : string.Empty),
					Str.ShowFract(Addl.Qty), 
					(Addl.Qty <= 1 ? UnitSingle : UnitPlural));
			}
			Info += "</table>";

			return Info;
		}
	}

	#endregion Ingred

	#region AddlInfoData

	public class AddlInfoData
	{
		public int JobRno;
        public string Customer;
		public int JobServings;
		public string MenuItem;
		public int RecipeRno;
		public string Recipe;
		public decimal Qty;
		public string UnitSingle;
		public string UnitPlural;
		public string Subrecipe;

        public AddlInfoData(int JobRno, string Customer, int JobServings, string MenuItem, int RecipeRno, string Recipe, decimal Qty, string UnitSingle, string UnitPlural)
		{
			Init(JobRno, Customer, JobServings, MenuItem, RecipeRno, Recipe, Qty, UnitSingle, UnitPlural, string.Empty);
		}

        public AddlInfoData(int JobRno, string Customer, int JobServings, string MenuItem, int RecipeRno, string Recipe, decimal Qty, string UnitSingle, string UnitPlural, string Subrecipe)
		{
			Init(JobRno, Customer, JobServings, MenuItem, RecipeRno, Recipe, Qty, UnitSingle, UnitPlural, Subrecipe);
		}

		private void Init(int JobRno, string Customer, int JobServings, string MenuItem, int RecipeRno, string Recipe, decimal Qty, string UnitSingle, string UnitPlural, string Subrecipe)
		{
			this.JobRno			= JobRno;
            this.Customer       = Customer;
			this.JobServings	= JobServings;
			this.MenuItem		= MenuItem;
			this.RecipeRno		= RecipeRno;
			this.Recipe			= Recipe;
			this.Qty			= Qty;
			this.UnitSingle		= UnitSingle;
			this.UnitPlural		= UnitPlural;
			this.Subrecipe		= Subrecipe;
		}
	}

	#endregion AddlInfoData

	#region Vendors

	protected struct VendorData
	{
		public int VendorRno;
		public string Name;
	}

	public static void LoadVendors()
	{
		string Sql = "Select * From Vendors Order By VendorRno";
		try
		{
			DataTable dt = DB.DBDataTable(Sql);
			Vendors = new VendorData[dt.Rows.Count];
			int i = 0;
			foreach (DataRow dr in dt.Rows)
			{
				Vendors[i].VendorRno = DB.Int32(dr["VendorRno"]);
				Vendors[i].Name = DB.Str(dr["Name"]);
				i++;
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	public static string VendorName(int VendorRno)
	{
		string Name = string.Empty;

		for (int i = 0; i < Vendors.Length; i++)
		{
			if (Vendors[i].VendorRno == VendorRno)
			{
				Name = Vendors[i].Name;
				break;
			}
		}

		return Name;
	}

	#endregion Vendors

	#region IngredConvs

	public class IngredConv
	{
		public int IngredConvRno;
		public int IngredRno;
		public decimal PurchaseQty;
		public int PurchaseUnitRno;
		public string PurchaseUnit;
		public decimal RecipeQty;
		public int RecipeUnitRno;
		public string RecipeUnit;
		public bool fVisited;
	}

	public static void LoadIngredConversions()
	{
		string Sql =
			"Select *, " +
			"(Select UnitSingle From Units Where UnitRno = c.PurchaseUnitRno) As PurchaseUnit,\n" +
			"(Select UnitSingle From Units Where UnitRno = c.RecipeUnitRno) As RecipeUnit\n" +
			"From IngredConv c Where PurchaseQty <> 0 And PurchaseQty Is Not Null And RecipeQty <> 0 And RecipeQty Is Not Null Order By IngredRno";
		try
		{
			DataTable dt = DB.DBDataTable(Sql);
			IngredConvs = new IngredConv[dt.Rows.Count];
			int i = 0;
			foreach (DataRow dr in dt.Rows)
			{
				IngredConvs[i]					= new IngredConv();
				IngredConvs[i].IngredConvRno	= DB.Int32(dr["IngredConvRno"]);
				IngredConvs[i].IngredRno		= DB.Int32(dr["IngredRno"]);
				IngredConvs[i].PurchaseQty		= DB.Dec(dr["PurchaseQty"]);
				IngredConvs[i].PurchaseUnitRno	= DB.Int32(dr["PurchaseUnitRno"]);
				IngredConvs[i].PurchaseUnit		= DB.Str(dr["PurchaseUnit"]);
				IngredConvs[i].RecipeQty		= DB.Dec(dr["RecipeQty"]);
				IngredConvs[i].RecipeUnitRno	= DB.Int32(dr["RecipeUnitRno"]);
				IngredConvs[i].RecipeUnit		= DB.Str(dr["RecipeUnit"]);
				IngredConvs[i].fVisited			= false;
				i++;
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	private static IList<IngredConv> FindIngredConversions(int IngredRno)
	{
		IList<IngredConv> lstIngredConversions = new List<IngredConv>();

		// first find all the ingredient conversions
		foreach (IngredConv IngredConv in IngredConvs)
		{
			if (IngredConv.IngredRno == IngredRno)
			{
				lstIngredConversions.Add(IngredConv);
			}
		}

		// next find all the standard conversions
		foreach (IngredConv IngredConv in IngredConvs)
		{
			if (IngredConv.IngredRno == 0)
			{
				lstIngredConversions.Add(IngredConv);
			}
		}

		// now copy the found conversions but in reverse order
		int Len = lstIngredConversions.Count;
		for (int i = 0; i < Len; i++)
		{
			IngredConv IngredConv	= lstIngredConversions[i];
			IngredConv Reverse		= new IngredConv();
			Reverse.IngredConvRno	= IngredConv.IngredConvRno;
			Reverse.IngredRno		= IngredConv.IngredRno;
			Reverse.RecipeQty		= IngredConv.PurchaseQty;
			Reverse.RecipeUnitRno	= IngredConv.PurchaseUnitRno;
			Reverse.RecipeUnit		= IngredConv.PurchaseUnit;
			Reverse.PurchaseQty		= IngredConv.RecipeQty;
			Reverse.PurchaseUnitRno = IngredConv.RecipeUnitRno;
			Reverse.PurchaseUnit	= IngredConv.RecipeUnit;
			Reverse.fVisited		= IngredConv.fVisited;

			lstIngredConversions.Add(Reverse);
		}

		return lstIngredConversions;
	}

	public static void AddIngredConv(int IngredConvRno, int IngredRno, decimal PurchaseQty, int PurchaseUnitRno, string PurchaseUnit, decimal RecipeQty, int RecipeUnitRno, string RecipeUnit)
	{
		int i = IngredConvs.Length;
		IngredConv[] temp = new IngredConv[i + 1];
		IngredConvs.CopyTo(temp, 0);
		IngredConvs = temp;
		
		IngredConvs[i]					= new IngredConv();
		IngredConvs[i].IngredConvRno	= IngredConvRno;
		IngredConvs[i].IngredRno		= IngredRno;
		IngredConvs[i].PurchaseQty		= PurchaseQty;
		IngredConvs[i].PurchaseUnitRno	= PurchaseUnitRno;
		IngredConvs[i].PurchaseUnit		= PurchaseUnit;
		IngredConvs[i].RecipeQty		= RecipeQty;
		IngredConvs[i].RecipeUnitRno	= RecipeUnitRno;
		IngredConvs[i].RecipeUnit		= RecipeUnit;
		IngredConvs[i].fVisited			= false;
	}

	#endregion IngredConvs

	#region IngredPurchases

	public class IngredPurchase
	{
		public int VendorRno;
		public int IngredRno;
		public decimal Qty;
		public decimal UnitQty;
		public int UnitRno;
		public string UnitSingle;
		public string UnitPlural;
		public decimal Price;
		public DateTime PurchaseDt;
	}

	public static void LoadIngredPurchases()
	{
		string Sql =
			"With Ingreds As\n" +
			"(\n" +
			"	Select Distinct VendorRno, IngredRno\n" +
			"	From Purchases p Inner Join PurchaseDetails d On p.PurchaseRno = d.PurchaseRno\n" +
			"	Group By VendorRno, IngredRno\n" +
			"),\n" +
			"IngredDetails As\n" +
			"(\n" +
			"	Select *,\n" +
			"	(\n" +
			"		Select Top 1 d.PurchaseDetailRno\n" +
			"		From Purchases p Inner Join PurchaseDetails d On p.PurchaseRno = d.PurchaseRno\n" +
			"		Where p.VendorRno = i.VendorRno And d.IngredRno = i.IngredRno\n" +
			"		Order By p.PurchaseDt Desc\n" +
			"	) As PurchaseDetailRno\n" +
			"	From Ingreds i\n" +
			")\n" +
			"Select i.VendorRno, i.IngredRno, d.PurchaseQty, d.PurchaseUnitQty, d.PurchaseUnitRno, d.Price, p.PurchaseDt,\n" +
			"(Select UnitSingle From Units Where UnitRno = d.PurchaseUnitRno) As UnitSingle,\n" +
			"(Select UnitPlural From Units Where UnitRno = d.PurchaseUnitRno) As UnitPlural\n" +
			"From IngredDetails i Inner Join PurchaseDetails d On i.PurchaseDetailRno = d.PurchaseDetailRno\n" +
			"Inner Join Purchases p On d.PurchaseRno = p.PurchaseRno\n" +
			"Order By VendorRno, IngredRno\n";
		try
		{
			DataTable dt = DB.DBDataTable(Sql);
			IngredPurchases = new IngredPurchase[dt.Rows.Count];
			int i = 0;
			foreach (DataRow dr in dt.Rows)
			{
				IngredPurchases[i]				= new IngredPurchase();
				IngredPurchases[i].VendorRno	= DB.Int32(dr["VendorRno"]);
				IngredPurchases[i].IngredRno	= DB.Int32(dr["IngredRno"]);
				IngredPurchases[i].Qty			= DB.Int32(dr["PurchaseQty"]);
				IngredPurchases[i].UnitQty		= DB.Int32(dr["PurchaseUnitQty"]);
				IngredPurchases[i].UnitRno		= DB.Int32(dr["PurchaseUnitRno"]);
				IngredPurchases[i].UnitSingle	= DB.Str(dr["UnitSingle"]);
				IngredPurchases[i].UnitPlural	= DB.Str(dr["UnitPlural"]);
				IngredPurchases[i].Price		= DB.Dec(dr["Price"]);
				IngredPurchases[i].PurchaseDt	= DB.DtTm(dr["PurchaseDt"]);
				i++;
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	public static int LastPurchaseVendor(int IngredRno)
	{
		int VendorRno = 0;
		DateTime BestPurchaseDt = DateTime.MinValue;

		for (int i = 0; i < IngredPurchases.Length; i++)
		{
			if (IngredPurchases[i].IngredRno == IngredRno && IngredPurchases[i].PurchaseDt > BestPurchaseDt)
			{
				VendorRno = IngredPurchases[i].VendorRno;
				BestPurchaseDt = IngredPurchases[i].PurchaseDt;
			}
		}

		return VendorRno;
	}

	public static IngredPurchase LastPurchase(int IngredRno)
	{
		IngredPurchase Purchase = null;
		//DateTime BestPurchaseDt = DateTime.MinValue;

		////if (IngredPurchases == null)
		//{
		//	LoadIngredPurchases();
		//}

		//for (int i = 0; i < IngredPurchases.Length; i++)
		//{
		//	if (IngredPurchases[i].IngredRno == IngredRno && IngredPurchases[i].PurchaseDt > BestPurchaseDt)
		//	{
		//		Purchase = IngredPurchases[i];
		//		BestPurchaseDt = IngredPurchases[i].PurchaseDt;
		//	}
		//}

		string Sql = string.Format(
			"Select Top 1 p.VendorRno, d.IngredRno, d.PurchaseQty, d.PurchaseUnitQty, d.PurchaseUnitRno, d.Price, p.PurchaseDt,\n" +
			"(Select UnitSingle From Units Where UnitRno = d.PurchaseUnitRno) As UnitSingle,\n" +
			"(Select UnitPlural From Units Where UnitRno = d.PurchaseUnitRno) As UnitPlural\n" +
			"from PurchaseDetails d Inner Join Purchases p On d.PurchaseRno = p.PurchaseRno\n" +
			"Where IngredRno = {0}\n" +
			"Order By p.PurchaseDt Desc, d.PurchaseRno Desc",
			IngredRno);
		try
		{
			DataRow dr = DB.DBDataRow(Sql);
			if (dr != null) 
			{
				Purchase			= new IngredPurchase();
				Purchase.VendorRno	= DB.Int32(dr["VendorRno"]);
				Purchase.IngredRno	= DB.Int32(dr["IngredRno"]);
				Purchase.Qty		= DB.Dec(dr["PurchaseQty"]);
				Purchase.UnitQty	= DB.Dec(dr["PurchaseUnitQty"]);
				Purchase.UnitRno	= DB.Int32(dr["PurchaseUnitRno"]);
				Purchase.UnitSingle	= DB.Str(dr["UnitSingle"]);
				Purchase.UnitPlural	= DB.Str(dr["UnitPlural"]);
				Purchase.Price		= DB.Dec(dr["Price"]);
				Purchase.PurchaseDt	= DB.DtTm(dr["PurchaseDt"]);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}

		return Purchase;
	}

    public void AdjustStockedItemsQuantity(DB db)
    {
        string Sql = string.Format(
            //"Select StockedPurchaseQty, StockedPurchaseUnitQty, StockedPurchaseUnitRno, UnitSingle, UnitPlural " +
            //"From Ingredients i Left Join Units u on i.StockedPurchaseUnitRno = u.UnitRno " +
            "Select StockedPurchaseQty " +
            "From Ingredients " +
            "Where IngredRno = {0}",
            this.IngredRno);
        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
                decimal Qty = DB.Dec(dr["StockedPurchaseQty"]);
                if (Qty != 0)
                {
                //    decimal UnitQty = DB.Dec(dr["StockedPurchaseUnitQty"]);
                //    this.Qty = Qty * (UnitQty != 0 ? UnitQty : 1);
                //    this.UnitRno = DB.Int32(dr["StockedPurchaseUnitRno"]);
                //    this.UnitSingle = DB.Str(dr["UnitSingle"]);
                //    this.UnitPlural = DB.Str(dr["UnitPlural"]);

                //    this.PurchaseQty = Qty;
                //    this.PurchaseUnitQty = DB.Dec(dr["StockedPurchaseUnitQty"]);
                //    this.PurchaseUnitRno = DB.Int32(dr["StockedPurchaseUnitRno"]);
                //    this.PurchaseUnitSingle = DB.Str(dr["UnitSingle"]);
                //    this.PurchaseUnitPlural = DB.Str(dr["UnitPlural"]);

                    this.StockedPurchaseQty = Qty;
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            System.Web.HttpContext.Current.Response.Write(Err.Html());
        }
    }

    #endregion IngredPurchases

    #region UpdatePrices

    // update the ingredient's prices with no price history (basically, the one and only receipt was deleted)
    public void UpdatePrice()
	{
		try
		{
			DB db = new DB();

			UpdatePrices(db, IngredRno, 0, null, null, null);

			db.Close();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	// update the ingredient's prices in recipies
	public void UpdatePrice(decimal? Qty, int? UnitRno, decimal? Price)
	{
		string Sql = string.Empty;

		try
		{
			DB db = new DB();

			UpdatePrices(db, IngredRno, 0, Qty, UnitRno, Price);

			db.Close();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	// update the subrecipe's prices in recipies
	public static void UpdateSubrecipePrice(int SubrecipeRno)
	{
		string Sql = string.Empty;

		try
		{
			DB db = new DB();

			// now check to see if this recipe is a sub recipe for other recipes and then update the parents
			decimal YieldQty = 0;
			int YieldUnitRno = 0;
			decimal RecipePrice = 0;

			// get the recipe info
			Sql = string.Format("Select YieldQty, YieldUnitRno, BaseCostPrice From Recipes Where RecipeRno = {0}", SubrecipeRno);
			DataRow drRecipe = db.DataRow(Sql);
			if (drRecipe != null)
			{
				YieldQty = DB.Dec(drRecipe["YieldQty"]);
				YieldUnitRno = DB.Int32(drRecipe["YieldUnitRno"]);
				RecipePrice = DB.Dec(drRecipe["BaseCostPrice"]);
			}

			UpdatePrices(db, 0, SubrecipeRno, YieldQty, YieldUnitRno, RecipePrice);

			db.Close();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	// update the ingredients prices in recipies
	private static void UpdatePrices(DB db, int IngredRno, int SubrecipeRno, decimal? Qty, int? UnitRno, decimal? Price)
	{
		string Sql = string.Empty;

		try
		{
			// update price in all recipie ingred xref records
			Sql = string.Format("Select RecipeIngredRno, RecipeRno, UnitQty, UnitRno, BaseCostPrice From RecipeIngredXref Where {0} = {1}", (IngredRno != 0 ? "IngredRno" : "SubrecipeRno"), (IngredRno != 0 ? IngredRno : SubrecipeRno));
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				int RecipeIngredRno		= DB.Int32(dr["RecipeIngredRno"]);
				int RecipeRno			= DB.Int32(dr["RecipeRno"]);
				decimal RecipeUnitQty	= DB.Dec(dr["UnitQty"]);
				int RecipeUnitRno		= DB.Int32(dr["UnitRno"]);
				decimal CurrIngredPrice = DB.Dec(dr["BaseCostPrice"]);

				bool fRecalcRecipe = false;

				// if there is a price
				if (Price.HasValue)
				{
					// calculate the new price
					Ingred Ingred = new Ingred(IngredRno);
					decimal ConvScaler = Ingred.ConversionScaler(UnitRno.Value, RecipeUnitRno);
					decimal IngredPrice = (Qty * ConvScaler == 0 ? 0 : Math.Round(Price.Value / Qty.Value / ConvScaler * RecipeUnitQty, 4));

					// if price has changed
					if (IngredPrice != CurrIngredPrice)
					{
						// update recipe ingred xref price and recipie cost
						Sql = string.Format(
							"Update RecipeIngredXref Set " +
							"BaseCostPrice = {1}, " +
							"UpdatedDtTm = GetDate(), " +
							"UpdatedUser = {2} " +
							"Where RecipeIngredRno = {0}",
							RecipeIngredRno,
							IngredPrice,
							DB.PutStr(g.User));
						db.Exec(Sql);

						fRecalcRecipe = true;
					}
				}
				else
				{
					// if there is not a price, remove the price from the recipe ingredient
					Sql = string.Format(
						"Update RecipeIngredXref Set " +
						"BaseCostPrice = Null, " +
						"UpdatedDtTm = GetDate(), " +
						"UpdatedUser = {1} " +
						"Where RecipeIngredRno = {0}",
						RecipeIngredRno,
						DB.PutStr(g.User));
					db.Exec(Sql);

					fRecalcRecipe = true;
				}

				if (fRecalcRecipe)
				{
					RecalcRecipePrice(db, RecipeRno);
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	public static void RecalcRecipePrice(DB db, int RecipeRno)
	{
		string Sql = string.Empty;

		try
		{
			Sql = string.Format(
				"Update Recipes Set " +
				"BaseCostPrice = (Select Sum(IsNull(BaseCostPrice, 0)) From RecipeIngredXref Where RecipeRno = {0}), " +
				"PortionPrice = Case When NumServings > 0 Then (Select Sum(IsNull(BaseCostPrice, 0)) From RecipeIngredXref Where RecipeRno = {0}) / NumServings Else 0 End, " +
				"InaccuratePriceFlg = Case When (Select Count(*) From RecipeIngredXref x Inner Join Ingredients i On x.IngredRno = i.IngredRno Where RecipeRno = {0} And BaseCostPrice Is Null And IsNull(i.NonPurchaseFlg, 0) = 0) > 0 Then 1 Else 0 End, " + 
				"UpdatedDtTm = GetDate(), " +
				"UpdatedUser = {1} " +
				"Where RecipeRno = {0}",
				RecipeRno,
				DB.PutStr(g.User));
			db.Exec(Sql);

			// set the cost for any menu items
			Sql = string.Format(
				"Update mcJobMenuItems Set " +
				"ServingPrice = r.PortionPrice / (IsNull(r.BaseCostPct, (Select Top 1 Case When IsNull(r.MenuItemAsIsFlg, 0) = 1 Then AsIsBaseCostPct Else BaseCostPct End From Settings)) / 100), " +
				"InaccuratePriceFlg = r.InaccuratePriceFlg " + 
				"From mcJobMenuItems m Inner Join Recipes r On m.RecipeRno = r.RecipeRno " +
				"Where r.RecipeRno = {0}", RecipeRno);
			db.Exec(Sql);

			// now check to see if this recipe is a sub recipe for other recipes and then update the parents
			decimal YieldQty = 0;
			int YieldUnitRno = 0;
			decimal RecipePrice = 0;

			// get the recipe info
			Sql = string.Format("Select YieldQty, YieldUnitRno, BaseCostPrice From Recipes Where RecipeRno = {0}", RecipeRno);
			DataRow drRecipe = db.DataRow(Sql);
			if (drRecipe != null)
			{
				YieldQty = DB.Dec(drRecipe["YieldQty"]);
				YieldUnitRno = DB.Int32(drRecipe["YieldUnitRno"]);
				RecipePrice = DB.Dec(drRecipe["BaseCostPrice"]);
			}

			UpdatePrices(db, 0, RecipeRno, YieldQty, YieldUnitRno, RecipePrice);

		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	public static void UpdateWithLastPrice(int IngredRno)
	{
		try
		{
			Ingred.IngredPurchase Last = Ingred.LastPurchase(IngredRno);
			Ingred Ing = new Ingred(IngredRno);
			if (Last != null)
			{
				Ing.UpdatePrice(Last.Qty * Last.UnitQty, Last.UnitRno, Last.Price);
			}
			else
			{
				// not last price, remove the currently set price
				Ing.UpdatePrice();
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			System.Web.HttpContext.Current.Response.Write(Err.Html());
		}
	}

	#endregion UpdatePrices
}