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

public partial class Contacts : System.Web.UI.Page
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
			Session["Menu"] = WebPage.Contacts.Title;

            int Rno = Str.Num((string)Request.QueryString["ContactRno"]);
            if (Rno != 0)
            {
                fStartHere = true;
                StartHere = Rno.ToString();
            }

            FillCustomers();
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
        ddlCustomer.ClearSelection();
        hfRno.Value =
        ddlCustomer.Text =
		txtName.Text =
		txtPhone.Text =
        txtCell.Text =
        txtFax.Text =
		txtEmail.Text =
		txtTitle.Text =
		txtCreatedDt.Text =
		txtCreatedUser.Text =
		txtUpdatedDt.Text =
		txtUpdatedUser.Text = string.Empty;
		chkHide.Checked = false;

		btnDelete.Enabled = false;
		btnDelete.ToolTip = "New contact item record";
	}

	protected void GetData(int Rno)
	{
		string Sql = string.Format(
			"Select * " +
            "From Contacts " +
            "Where ContactRno = {0}",
			Rno);

		ClearData();

		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr != null)
			{
				hfRno.Value = Rno.ToString();
                int CustomerRno = DB.Int32(dr["CustomerRno"]);
                ddlCustomer.ClearSelection();
                ListItem Item = ddlCustomer.Items.FindByValue(CustomerRno == 0 ? "" : CustomerRno.ToString());
                if (Item != null)
                {
                    Item.Selected = true;
                }
                
				txtName.Text = DB.Str(dr["Name"]);
                chkHide.Checked = (DB.DtTm(dr["InactiveDtTm"]) != DateTime.MinValue);
                txtPhone.Text = DB.Str(dr["Phone"]);
				txtCell.Text = DB.Str(dr["Cell"]);
                txtFax.Text = DB.Str(dr["Fax"]);
                txtEmail.Text = DB.Str(dr["Email"]);
				txtTitle.Text = DB.Str(dr["Title"]);
				txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
				txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);

				Sql = string.Format("Select Count(*) From mcJobs Where ContactRno = {0}", Rno);
				int Count = db.SqlNum(Sql);

				if (Count > 0)
				{
					btnDelete.Enabled = false;
					btnDelete.ToolTip = string.Format("Contact for {0} jobs", Count);
				}
				else
				{
					btnDelete.Enabled = true;
					btnDelete.ToolTip = "Not a contact for any jobs";
				}

				FocusField = "txtName";

                // jobs
                Sql = string.Format("Select JobRno, JobDesc, JobDate From mcJobs Where ContactRno = {0} Order By JobDate Desc", Rno);
                DataTable dt = db.DataTable(Sql);
                foreach (DataRow drJobs in dt.Rows)
                {
                    int JobRno = DB.Int32(drJobs["JobRno"]);
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    HtmlAnchor a = new HtmlAnchor()
                    {
                        InnerText = string.Format("{0} {1}", JobRno, DB.Str(drJobs["JobDesc"])),
                        HRef = Page.ResolveUrl(string.Format("{0}?JobRno={1}", @"~\Job.aspx", JobRno)), Target = "Job"
                    };
                    HtmlGenericControl div = new HtmlGenericControl("div") { InnerText = Fmt.Dt(DB.DtTm(drJobs["JobDate"])) };
                    div.Attributes.Add("class", "Date");
                    li.Controls.Add(div);
                    li.Controls.Add(a);
                    phJobs.Controls.Add(li);
                }
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
				Sql = string.Format(
					"Insert Into Contacts (Name, CreatedDtTm, CreatedUser) Values ({0}, GetDate(), {1}); " +
					"Select Scope_Identity()",
					DB.PutStr(txtName.Text, 50),
					DB.PutStr(g.User));
				Rno = db.SqlNum(Sql);
			}

			Sql = string.Format(
				"Update Contacts Set " +
                "CustomerRno = {1}, " +
				"Name = {2}, " +
				"Phone = {3}, " +
				"Cell = {4}, " +
                "Fax = {5}, " +
                "Email = {6}, " +
				"Title = {7}, " +
				"UpdatedDtTm = GetDate(), " +
				"UpdatedUser = {8} " +
				"Where ContactRno = {0}",
				Rno,
                (ddlCustomer.SelectedValue.Length > 0 ? ddlCustomer.SelectedValue : "Null"), 
				DB.Put(txtName.Text, 50),
				DB.Put(txtPhone.Text, 20),
				DB.Put(txtCell.Text, 20),
                DB.Put(txtFax.Text, 20),
                DB.Put(txtEmail.Text, 80),
				DB.Put(txtTitle.Text, 30),
				DB.PutStr(g.User));
			db.Exec(Sql);

            Sql = string.Format("Select Case When InactiveDtTm Is Null Then 0 else 1 End as Hidden From Contacts Where ContactRno = {0}", Rno);
            if (chkHide.Checked != db.SqlBool(Sql))
            {
                Sql = string.Format(
                    "Update Contacts Set " +
                    "InactiveDtTm = {1}, " +
                    "InactiveUser = {2} " +
                    "Where ContactRno = {0}",
                    Rno,
                    (chkHide.Checked ? "GetDate()" : "Null"),
                    (chkHide.Checked ? DB.Put(g.User) : "Null"));
                db.Exec(Sql);
            }

            fStartHere = true;
			StartHere = Rno.ToString();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
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
            "Select ContactRno, Name, InactiveDtTm " +
            "From Contacts c";
		if (!chkShowHidden.Checked) Sql += " Where (InactiveDtTm Is Null)";
		Sql += " Order by Name";

		string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");
		string NextCurrItem = (lstList.SelectedIndex >= 0 && lstList.SelectedIndex + 1 < lstList.Items.Count ? lstList.Items[lstList.SelectedIndex + 1].Value : string.Empty);
		if (!fNext && CurrItem.Length > 0)
		{
			NextCurrItem = CurrItem;
		}

		// if a place to start
		if (!fNext && fStartHere)
		{
			NextCurrItem = StartHere;
		}

		lstList.Items.Clear();

		LoadListSql(Sql, NextCurrItem);

		lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Contacts";
	}

	private void LoadListSql(string Sql, string NextCurrItem)
	{
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				Int32 Rno = DB.Int32(dr["ContactRno"]);
				String Name = DB.Str(dr["Name"]);
				bool fHidden = (DB.DtTm(dr["InactiveDtTm"]) != DateTime.MinValue);
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

		LoadList(fNext);
	}

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		int Rno = Str.Num(lstList.SelectedItem.Value);
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
            "Delete From Contacts Where ContactRno = {0}",
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

    protected void FillCustomers()
    {
        string Sql = 
            "Select Null as CustomerRno, '' as Name Union " +
            "Select CustomerRno, Name From Customers Where InactiveDtTm Is Null Order By Name";

        try
        {
            DataTable dt = db.DataTable(Sql);
            ddlCustomer.DataSource = dt;
            ddlCustomer.DataTextField = "Name";
            ddlCustomer.DataValueField = "CustomerRno";
            ddlCustomer.DataBind();
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

    }
}
