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

public partial class Vendors : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";

	private bool fStartHere = false;
	private string StartHere;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.Shopping.Title;
			Setup();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
	}

	private void Setup()
	{
		LoadList();
	}

	protected void ClearData()
	{
		hfRno.Value =
		txtName.Text =
		txtPhone.Text =
		txtContact.Text =
		txtEmail.Text =
		txtWebsite.Text =
		txtNote.Text =
		txtCreatedDt.Text =
		txtCreatedUser.Text =
		txtUpdatedDt.Text =
		txtUpdatedUser.Text = string.Empty;
		chkHide.Checked = false;

		btnDelete.Enabled = false;
		btnDelete.ToolTip = "New vendor item record";
	}

	protected void GetData(int Rno)
	{
		string Sql = string.Format(
			"Select * " +
			"From Vendors " +
			"Where VendorRno = {0}",
			Rno);

		ClearData();

		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr != null)
			{
				hfRno.Value = Rno.ToString();
				txtName.Text = DB.Str(dr["Name"]);
				chkHide.Checked = DB.Bool(dr["HideFlg"]);
				txtPhone.Text = DB.Str(dr["Phone"]);
				txtContact.Text = DB.Str(dr["Contact"]);
				txtEmail.Text = DB.Str(dr["Email"]);
				txtWebsite.Text = DB.Str(dr["Website"]);
				txtNote.Text = DB.Str(dr["Note"]);
				txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
				txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);

				Sql = string.Format("Select Count(*) From Purchases Where VendorRno = {0}", Rno);
				int Count = db.SqlNum(Sql);

				if (Count > 0)
				{
					btnDelete.Enabled = false;
					btnDelete.ToolTip = string.Format("Vendor for {0} receipts", Count);
				}
				else
				{
					btnDelete.Enabled = true;
					btnDelete.ToolTip = "Not a vendor for any receipts";
				}

				FocusField = "txtName";
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void SaveData()
	{
		int Rno = Str.Num(hfRno.Value);
		string Sql = string.Empty;
		try
		{
			if (Rno == 0)
			{
				//Response.Write("New<br/>");
				Sql = string.Format(
					"Insert Into Vendors (Name, CreatedDtTm, CreatedUser) Values ({0}, GetDate(), {1}); " +
					"Select Scope_Identity()",
					DB.PutStr(txtName.Text, 50),
					DB.PutStr(g.User));
				Rno = db.SqlNum(Sql);
			}

			Sql = string.Format(
				"Update Vendors Set " +
				"Name = {1}, " +
				"Phone = {2}, " +
				"Contact = {3}, " +
				"Email = {4}, " +
				"Website = {5}, " +
				"HideFlg = {6}, " +
				"Note = {7}, " +
				"UpdatedDtTm = GetDate(), " +
				"UpdatedUser = {8} " +
				"Where VendorRno = {0}",
				Rno,
				DB.PutStr(txtName.Text, 50),
				DB.PutStr(txtPhone.Text, 50),
				DB.PutStr(txtContact.Text, 50),
				DB.PutStr(txtEmail.Text, 100),
				DB.PutStr(txtWebsite.Text, 100),
				DB.PutBool(chkHide.Checked),
				DB.PutStr(txtNote.Text, 2000),
				DB.PutStr(g.User));
			db.Exec(Sql);

			fStartHere = true;
			StartHere = Rno.ToString();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}
	}

	private void LoadList()
	{
		LoadList(false);
	}

	private void LoadList(bool fNext)
	{
		string Sql =
			"Select VendorRno, Name, HideFlg " +
			"From Vendors v";
		if (!chkShowHidden.Checked) Sql += " Where (HideFlg = 0 Or HideFlg Is Null)";
		Sql += " Order by Name";

		string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");
		string NextCurrItem = (lstList.SelectedIndex >= 0 && lstList.SelectedIndex + 1 < lstList.Items.Count ? lstList.Items[lstList.SelectedIndex + 1].Value : string.Empty);
		if (!fNext && CurrItem.Length > 0)
		{
			NextCurrItem = CurrItem;
		}

		//Response.Write("fStartHere " + fStartHere + "<br/>");
		// if a place to start
		if (!fNext && fStartHere)
		{
			NextCurrItem = StartHere;
			//Response.Write("Start Here [" + StartHere + "]<br/>");
		}

		//if (lstList.SelectedIndex >= 0) Response.Write("Curr " + lstList.SelectedItem.Value + "<br/>");
		lstList.Items.Clear();

		//Response.Write(Sql + "<br/>");
		LoadListSql(Sql, NextCurrItem);

		lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Vendors";

		//btnUpdate.Enabled = (CurrItem.Length > 0);
	}

	private void LoadListSql(string Sql, string NextCurrItem)
	{
		//Response.Write("NextCurr [" + NextCurrItem + "]<br/>");
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				Int32 Rno = DB.Int32(dr["VendorRno"]);
				String Name = DB.Str(dr["Name"]);
				bool fHidden = DB.Bool(dr["HideFlg"]);
				ListItem Item = new ListItem(Name, Rno.ToString());
				if (fHidden) Item.Attributes.Add("class", "Hidden");

				if (Item.Value == NextCurrItem)
				{
					Item.Selected = true;

					Rno = Str.Num(Item.Value);
					GetData(Rno);
					btnUpdate.Enabled =
					btnNext.Enabled = true;
				}

				lstList.Items.Add(Item);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void UpdateList(object sender, System.EventArgs e)
	{
		LoadList();
	}

	protected void Update()
	{
		Update(false);
	}

	protected void Update(bool fNext)
	{
		SaveData();

		//ClearData();
		LoadList(fNext);
	}

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		int Rno = Str.Num(lstList.SelectedItem.Value);
		//GetData(Rno);
		fStartHere = true;
		StartHere = lstList.SelectedItem.Value;
		LoadList();
		btnUpdate.Enabled =
		btnNext.Enabled = true;
	}

	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		SaveData();
		ClearData();
		LoadList();

		//Update(false);
	}

	protected void btnUpdateNext_Click(object sender, EventArgs e)
	{
		SaveData();
		ClearData();
		LoadList(true);
	}

	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		ClearData();
	}

	protected void btnDelete_Click(object sender, EventArgs e)
	{
		int Rno = Str.Num(hfRno.Value);
		string Sql = String.Format(
			"Delete From Vendors Where VendorRno = {0}",
			Rno);

		try
		{
			db.Exec(Sql);
			LoadList(true);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}

