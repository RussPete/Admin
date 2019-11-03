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

public partial class PrintJobSheet : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected Int32 JobRno;

	private void PrintJobSheet_Load(object sender, System.EventArgs e)
	{
		db = new DB();
		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

		// Put user code to initialize the page here
		if (!Page.IsPostBack)
		{
			Setup();
		}
	}

	private void Setup()
	{
		JobInfo();
	}

	private void JobInfo()
	{
		string Sql =
            "Select j.NumMenServing, j.NumWomenServing, j.NumChildServing, j.Status, j.EventType, j.ServiceType, j.Location, " +
            "j.PrintDirectionsFlg, j.LocationDirections, j.JobDate, j.MealTime, j.ArrivalTime, j.DepartureTime, j.PricePerPerson, " +
            "j.JobNotes, j." +
            "cu.Name as Customer, c.Name, c.Phone, c.Cell, c.Fax, c.Email, " +
            "From mcJobs j " +
            "Left Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where JobRno = " + JobRno;
		try
		{
			DataTable dt = db.DataTable(Sql);
			DataRow r = dt.Rows[0];

			Int32 cMen = DB.Int32(r["NumMenServing"]);
			Int32 cWomen = DB.Int32(r["NumWomenServing"]);
			Int32 cChild = DB.Int32(r["NumChildServing"]);

			lblPrinted.Text = String.Format("{0}", DateTime.Now);
			lblJobRno.Text = JobRno.ToString();
			lblStatus.Text = DB.Str(r["Status"]);
			lblCustomer.Text = DB.Str(r["Customer"]);
			lblName.Text = DB.Str(r["Name"]);
			lblPhone.Text = DB.Str(r["Phone"]);
			lblCell.Text = DB.Str(r["Cell"]);
			lblFax.Text = DB.Str(r["Fax"]);
			lblEmail.Text = DB.Str(r["Email"]);
			lblEventType.Text = DB.Str(r["EventType"]);
			lblServType.Text = DB.Str(r["ServiceType"]);
			lblLocation.Text = DB.Str(r["Location"]);
			if (DB.Bool(r["PrintDirectionsFlg"]))
			{
				lblDirections.Text = DB.Str(r["LocationDirections"]).Replace("\n", "<br>");
			}
			lblJobDate.Text = Fmt.Dt(DB.DtTm(r["JobDate"]));
			lblMealTime.Text = Fmt.Tm12Hr(DB.DtTm(r["MealTime"]));
			lblArrivalTime.Text = Fmt.Tm12Hr(DB.DtTm(r["ArrivalTime"]));
			lblDepartureTime.Text = Fmt.Tm12Hr(DB.DtTm(r["DepartureTime"]));
			lblNumServing.Text = Fmt.Num(cMen + cWomen + cChild);
			lblNumMenServing.Text = Fmt.Num(cMen);
			lblNumWomenServing.Text = Fmt.Num(cWomen);
			lblNumChildServing.Text = Fmt.Num(cChild);
			lblPricePerPerson.Text = Fmt.Dollar(DB.Dec(r["PricePerPerson"]), false);
			lblJobNotes.Text = DB.Str(r["JobNotes"]);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void FoodCategory(string Category)
	{
		bool fFirst = true;
		string Sql =
			"Select * From mcJobFood " +
			"Where JobRno = " + JobRno + " " +
			"And Category = " + DB.PutStr(Category) + " " +
			"Order By FoodSeq";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				FoodItem(Category, fFirst, r);
				fFirst = false;
			}

			/*
							if (fFirst)
							{
								Response.Write(
									"<tr>\n" +
									"\t<td align=\"right\"><b>" + Category + "</b></td>\n" +
									"</tr>\n");
							}
			*/
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void FoodOther()
	{
		string Sql =
			"Select * From mcJobFood " +
			"Where JobRno = " + JobRno + " " +
			"And Category Not In ('Meats', 'Sides', 'Salads', 'Bread', 'Desserts', 'Drink') " +
			"Order By Category, MenuItem";
		string Category;
		string PrevCategory = "";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				Category = DB.Str(r["Category"]);
				FoodItem(Category, Category != PrevCategory, r);
				PrevCategory = Category;
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void FoodItem(string Category, bool fFirst, DataRow r)
	{
		string Html;
		string MenuItem = DB.Str(r["MenuItem"]);
		string QtyNote = DB.Str(r["QtyNote"]);
		string ServiceNote = DB.Str(r["ServiceNote"]);
		string Class = "class=\"JobItem\"";

		if (MenuItem.Length > 0 ||
			QtyNote.Length > 0 ||
			ServiceNote.Length > 0)
		{
			if (QtyNote.Length > 0)
			{
				MenuItem =
					"\n<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\n" +
					"\t<tr>\n" +
					"\t\t<td valign=\"top\" class=\"JobItem\">" + MenuItem + "</td>\n" +
					"\t\t<td><img width=\"5\" height=\"1\" src=\"Images/Space.gif\"></td>\n" +
					"\t\t<td align=\"right\">" + QtyNote + "</td>\n" +
					"\t</tr>\n" +
					"</table>\n";
				Class = "";
			}

			Html =
				(fFirst ? "<tr><td><img width=\"1\" height=\"10\" src=\"Images/Space.gif\"></td></tr>\n" : "") +
				"<tr>\n" +
				"\t<td valign=\"top\" align=\"right\" >" + (fFirst ? "<b>" + Category + "</b>" : "") + "</td>\n" +
				"\t<td valign=\"top\" align=\"right\">" + (MenuItem.Length > 0 ? "<img src=\"Images/Diamond.gif\" class=\"JobBullet\">" : "") + "</td>\n" +
				"\t<td valign=\"top\" colspan=\"2\" " + Class + ">" + MenuItem + "</d>\n" +
				"</tr>\n";

			if (ServiceNote.Length > 0)
			{
				Html +=
					"<tr>\n" +
					"\t<td colspan=\"3\"></td>\n" +
					"\t<td><i>" + ServiceNote + "</i></td>\n" +
					"</tr>\n";
			}

			Response.Write(Html);
		}
	}

	protected void Services()
	{
		string Sql =
			"Select * From mcJobServices " +
			"Where JobRno = " + JobRno + " " +
			"Order By ServiceSeq";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				string sMCC = "<img src=\"Images/" + (DB.Bool(r["MCCRespFlg"]) ? "BoxChecked.gif" : "Box.gif") + "\" border=\"0\">";
				string sCustomer = "<img src=\"Images/" + (DB.Bool(r["CustomerRespFlg"]) ? "BoxChecked.gif" : "Box.gif") + "\" border=\"0\">";
				string Html =
					"<tr>\n" +
					"\t<td valign=\"top\" align=\"right\" nowrap class=\"JobItem\">" + DB.Str(r["ServiceItem"]) + "</td>\n" +
					"\t<td></td>\n" +
					"\t<td valign=\"top\" align=\"center\">" + sMCC + "</td>\n" +
					"\t<td></td>\n" +
					"\t<td valign=\"top\" align=\"center\">" + sCustomer + "</td>\n" +
					"\t<td align=\"right\">" + DB.Str(r["Note"]) + "</td>\n" +
					"</tr>\n";
				Response.Write(Html);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void Supplies()
	{
		string Sql =
			"Select * From mcJobSupplies " +
			"Where JobRno = " + JobRno + " " +
			"Order By SupplySeq";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				string Html =
					"<tr>\n" +
					"\t<td valign=\"top\" class=\"JobItem\">" + DB.Str(r["SupplyItem"]) + "</d>\n" +
					"\t<td></td>\n" +
					"\t<td valign=\"top\" align=\"right\">" + Fmt.Num(DB.Int32(r["Qty"]), false) + "</d>\n" +
					"</tr>\n";

				string Note = DB.Str(r["Note"]);
				if (Note.Length > 0)
				{
					Html +=
						"<tr>\n" +
						"\t<td colspan=\"3\" align=\"right\"><i>" + Note + "</i></td>\n" +
						"</tr>\n";
				}
				Response.Write(Html);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void Crew()
	{
		string Sql =
			"Select * From mcJobCrew " +
			"Where JobRno = " + JobRno + " " +
			"Order By CrewSeq";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				string Html =
					"<tr>\n" +
					"\t<td valign=\"top\" class=\"JobItem\">" + DB.Str(r["CrewMember"]) + "</d>\n" +
					"\t<td></td>\n" +
					"\t<td valign=\"top\">" + DB.Str(r["CrewAssignment"]) + "</d>\n" +
					"\t<td></td>\n" +
					"\t<td valign=\"top\" align=\"right\">" + Fmt.Tm12Hr(DB.DtTm(r["ReportTime"])) + "</d>\n" +
					"</tr>\n";

				string Note = DB.Str(r["Note"]);
				if (Note.Length > 0)
				{
					Html +=
						"<tr>\n" +
						"\t<td colspan=\"5\" align=\"left\"><img width=\"20\" height=\"1\" src=\"Images/Space.gif\"><i>" + Note + "</i></td>\n" +
						"</tr>\n";
				}
				Response.Write(Html);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}