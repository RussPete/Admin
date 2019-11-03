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

public partial class MergeContacts : System.Web.UI.Page
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
        lblRno1.Text =
        lblRno2.Text =
        hfRno1.Value =
        hfRno2.Value =
        rdoName1.Text =
        rdoName2.Text =
        rdoPhone1.Text =
        rdoPhone2.Text =
        rdoCell1.Text =
        rdoCell2.Text =
        rdoFax1.Text =
        rdoFax2.Text =
        rdoEmail1.Text =
        rdoEmail2.Text =
        rdoTitle1.Text =
        rdoTitle2.Text =
        rdoCustomer1.Text =
        rdoCustomer2.Text =
        txtCreatedDt1.Text =
        txtCreatedDt2.Text =
        txtUpdatedDt1.Text =
        txtCreatedDt2.Text = string.Empty;

        rdoName1.Checked =
        rdoName2.Checked =
        rdoPhone1.Checked =
        rdoPhone2.Checked =
        rdoCell1.Checked =
        rdoCell2.Checked =
        rdoFax1.Checked =
        rdoFax2.Checked =
        rdoEmail1.Checked =
        rdoEmail2.Checked =
        rdoTitle1.Checked =
        rdoTitle2.Checked =
        rdoCustomer1.Checked =
        rdoCustomer2.Checked = false;
    }

	protected void GetData(int Rno, int iCol)
	{
		string Sql = string.Format(
			"Select c.*, cu.Name as CustomerName " +
            "From Contacts c " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where ContactRno = {0}",
			Rno);

		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr != null)
			{
                switch (iCol)
                {
                    case 1:
                        hfRno1.Value = Rno.ToString();
                        lblRno1.Text = Rno.ToString();
                        rdoName1.Text = DB.Str(dr["Name"]);
                        rdoPhone1.Text = DB.Str(dr["Phone"]);
                        rdoCell1.Text = DB.Str(dr["Cell"]);
                        rdoFax1.Text = DB.Str(dr["Fax"]);
                        rdoEmail1.Text = DB.Str(dr["Email"]);
                        rdoTitle1.Text = DB.Str(dr["Title"]);
                        rdoCustomer1.Text = DB.Str(dr["CustomerName"]);
                        txtCreatedDt1.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
                        txtCreatedUser1.Text = DB.Str(dr["CreatedUser"]);
                        txtUpdatedDt1.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
                        txtUpdatedUser1.Text = DB.Str(dr["UpdatedUser"]);

                        rdoName1.Checked =
                        rdoPhone1.Checked =
                        rdoCell1.Checked =
                        rdoFax1.Checked =
                        rdoEmail1.Checked =
                        rdoTitle1.Checked =
                        rdoCustomer1.Checked = true;

                        FocusField = "rdoName1";
                        break;

                    case 2:
                        if (Rno.ToString() != lblRno1.Text)
                        {
                            hfRno2.Value = Rno.ToString();
                            lblRno2.Text = Rno.ToString();
                            rdoName2.Text = DB.Str(dr["Name"]);
                            rdoPhone2.Text = DB.Str(dr["Phone"]);
                            rdoCell2.Text = DB.Str(dr["Cell"]);
                            rdoFax2.Text = DB.Str(dr["Fax"]);
                            rdoEmail2.Text = DB.Str(dr["Email"]);
                            rdoTitle2.Text = DB.Str(dr["Title"]);
                            rdoCustomer2.Text = DB.Str(dr["CustomerName"]);
                            txtCreatedDt2.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
                            txtCreatedUser2.Text = DB.Str(dr["CreatedUser"]);
                            txtUpdatedDt2.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
                            txtUpdatedUser2.Text = DB.Str(dr["UpdatedUser"]);

                            if (rdoName1.Text.Length == 0 && rdoName2.Text.Length > 0)
                            {
                                rdoName2.Checked = true;
                            }
                            if (rdoPhone1.Text.Length == 0 && rdoPhone2.Text.Length > 0)
                            {
                                rdoPhone2.Checked = true;
                            }
                            if (rdoCell1.Text.Length == 0 && rdoCell2.Text.Length > 0)
                            {
                                rdoCell2.Checked = true;
                            }
                            if (rdoFax1.Text.Length == 0 && rdoFax2.Text.Length > 0)
                            {
                                rdoFax1.Checked = true;
                            }
                            if (rdoEmail1.Text.Length == 0 && rdoEmail2.Text.Length > 0)
                            {
                                rdoEmail2.Checked = true;
                            }
                            if (rdoTitle1.Text.Length == 0 && rdoTitle2.Text.Length > 0)
                            {
                                rdoTitle2.Checked = true;
                            }
                            if (rdoCustomer1.Text.Length == 0 && rdoCustomer2.Text.Length > 0)
                            {
                                rdoCustomer2.Checked = true;
                            }

                            FocusField = "rdoName2";
                        }
                        break;
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
		int Rno1 = Str.Num(hfRno1.Value);
        int Rno2 = Str.Num(hfRno2.Value);
        string Sql = string.Empty;
		try
		{
			if (Rno1 != 0 && Rno2 != 0)
			{
                StringBuilder sbSql = new StringBuilder();
                bool fComma = false;

                if (rdoName2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.Name = c2.Name");
                    fComma = true;
                }

                if (rdoPhone2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.Phone = c2.Phone");
                    fComma = true;
                }

                if (rdoCell2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.Cell = c2.Cell");
                    fComma = true;
                }

                if (rdoFax2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.Fax = c2.Fax");
                    fComma = true;
                }

                if (rdoEmail2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.Email = c2.Email");
                    fComma = true;
                }

                if (rdoTitle2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.Title = c2.Title");
                    fComma = true;
                }

                if (rdoCustomer2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.CustomerRno = c2.CustomerRno");
                    fComma = true;
                }

                Sql = "Begin Tran;\n";
                if (sbSql.Length > 0)
                {
                    Sql += string.Format(
                        "Update c Set {2} From Contacts c Inner Join Contacts c2 on c2.ContactRno = {1} Where c.ContactRno = {0};\n",
                        Rno1,
                        Rno2,
                        sbSql.ToString());
                }
                Sql += string.Format("Delete From Contacts Where ContactRno = {0};\n", Rno2);
                Sql += string.Format("Update mcJobs Set ContactRno = {0} Where ContactRno = {1};\n", Rno1, Rno2);
                Sql += "Commit Tran;\n";

                db.Exec(Sql);
            }
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
		Sql += " Order by Name, ContactRno";

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
				ListItem Item = new ListItem(string.Format("{0} - {1}", Name, Rno), Rno.ToString());
				if (fHidden) Item.Attributes.Add("class", "Hidden");

				if (Item.Value == NextCurrItem)
				{
					Item.Selected = true;

					Rno = Str.Num(Item.Value);
                    if (lblRno1.Text.Length == 0)
                    {
                        GetData(Rno, 1);
                    }
                    else
                    {
                        GetData(Rno, 2);
                    }
                    btnUpdate.Enabled = true;
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

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		int Rno = Str.Num(lstList.SelectedItem.Value);
		fStartHere = true;
		StartHere = lstList.SelectedItem.Value;
		LoadList();
		btnUpdate.Enabled = true;
	}

	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		SaveData();
		ClearData();
		LoadList();
	}

    protected void btnClear_Click(object sender, System.EventArgs e)
    {
        ClearData();
    }
}
