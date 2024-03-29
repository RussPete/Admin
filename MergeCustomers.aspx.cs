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

public partial class MergeCustomers : System.Web.UI.Page
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
        rdoTaxExempt1.Text =
        rdoTaxExempt2.Text =
        rdoCompany1.Text =
        rdoCompany2.Text =
        rdoOnlineOrder1.Text =
        rdoOnlineOrder2.Text =
        rdoEmailConf1.Text =
        rdoEmailConf2.Text =
        rdoSignedBy1.Text =
        rdoSignedBy2.Text =
        rdoSigned1.Text =
        rdoSigned2.Text =
        txtCreatedDt1.Text =
        txtCreatedDt2.Text =
        txtUpdatedDt1.Text =
        txtCreatedDt2.Text = string.Empty;

        rdoName1.Checked =
        rdoName2.Checked =
        rdoTaxExempt1.Checked =
        rdoTaxExempt2.Checked =
        rdoCompany1.Checked =
        rdoCompany2.Checked =
        rdoOnlineOrder1.Checked =
        rdoOnlineOrder2.Checked =
        rdoEmailConf1.Checked =
        rdoEmailConf2.Checked =
        rdoSignedBy1.Checked =
        rdoSignedBy2.Checked =
        rdoSigned1.Checked =
        rdoSigned2.Checked = false;
    }

	protected void GetData(int Rno, int iCol)
	{
		string Sql = string.Format(
			"Select c.* " +
            "From Customers c " +
            "Where CustomerRno = {0}",
			Rno);

		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr != null)
			{
                switch (iCol)
                {
                    case 1:
                        hfRno1.Value         = Rno.ToString();
                        lblRno1.Text         = Rno.ToString();
                        rdoName1.Text        = DB.Str(dr["Name"]);
                        rdoTaxExempt1.Text   = Fmt.YesNo(DB.Bool(dr["TaxExemptFlg"]));
                        rdoCompany1.Text     = Fmt.YesNo(DB.Bool(dr["CompanyFlg"]));
                        rdoOnlineOrder1.Text = Fmt.YesNo(DB.Bool(dr["OnlineOrderFlg"]));
                        rdoEmailConf1.Text   = Fmt.YesNo(DB.Bool(dr["EmailConfirmationsFlg"]));
                        rdoSignedBy1.Text    = Fmt.YesNo(DB.Bool(dr["AnnualContractSignedName"]));
                        rdoSigned1.Text      = Fmt.YesNo(DB.Bool(dr["AnnualContractSignedDtTm"]));
                        txtCreatedDt1.Text   = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
                        txtCreatedUser1.Text = DB.Str(dr["CreatedUser"]);
                        txtUpdatedDt1.Text   = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
                        txtUpdatedUser1.Text = DB.Str(dr["UpdatedUser"]);

                        rdoName1.Checked =
                        rdoTaxExempt1.Checked =
                        rdoCompany1.Checked =
                        rdoOnlineOrder1.Checked =
                        rdoEmailConf1.Checked = 
                        rdoSignedBy1.Checked = 
                        rdoSigned1.Checked = true;

                        FocusField = "rdoName1";
                        break;

                    case 2:
                        if (Rno.ToString() != lblRno1.Text)
                        {
                            hfRno2.Value         = Rno.ToString();
                            lblRno2.Text         = Rno.ToString();
                            rdoName2.Text        = DB.Str(dr["Name"]);
                            rdoTaxExempt1.Text   = Fmt.YesNo(DB.Bool(dr["TaxExemptFlg"]));
                            rdoCompany1.Text     = Fmt.YesNo(DB.Bool(dr["CompanyFlg"]));
                            rdoOnlineOrder1.Text = Fmt.YesNo(DB.Bool(dr["OnlineOrderFlg"]));
                            rdoEmailConf1.Text   = Fmt.YesNo(DB.Bool(dr["EmailConfirmationsFlg"]));
                            rdoSignedBy1.Text    = Fmt.YesNo(DB.Bool(dr["AnnualContractSignedName"]));
                            rdoSigned1.Text      = Fmt.YesNo(DB.Bool(dr["AnnualContractSignedDtTm"]));
                            txtCreatedDt2.Text   = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
                            txtCreatedUser2.Text = DB.Str(dr["CreatedUser"]);
                            txtUpdatedDt2.Text   = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
                            txtUpdatedUser2.Text = DB.Str(dr["UpdatedUser"]);

                            if (rdoName1.Text.Length == 0 && rdoName2.Text.Length > 0)
                            {
                                rdoName2.Checked = true;
                            }
                            if (rdoTaxExempt1.Text.Length == 0 && rdoTaxExempt2.Text.Length > 0)
                            {
                                rdoTaxExempt2.Checked = true;
                            }
                            if (rdoCompany1.Text.Length == 0 && rdoCompany2.Text.Length > 0)
                            {
                                rdoCompany2.Checked = true;
                            }
                            if (rdoOnlineOrder1.Text.Length == 0 && rdoOnlineOrder2.Text.Length > 0)
                            {
                                rdoOnlineOrder1.Checked = true;
                            }
                            if (rdoEmailConf1.Text.Length == 0 && rdoEmailConf2.Text.Length > 0)
                            {
                                rdoEmailConf2.Checked = true;
                            }
                            if (rdoSignedBy1.Text.Length == 0 && rdoSignedBy2.Text.Length > 0)
                            {
                                rdoSignedBy2.Checked = true;
                            }
                            if (rdoSigned1.Text.Length == 0 && rdoSigned2.Text.Length > 0)
                            {
                                rdoSigned2.Checked = true;
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

                if (rdoTaxExempt2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.TaxExemptFlg = c2.TaxExemptFlg");
                    fComma = true;
                }

                if (rdoCompany2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.CompanyFlg = c2.CompanyFlg");
                    fComma = true;
                }

                if (rdoOnlineOrder2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.OnlineOrderFlg = c2.OnlineOrderFlg");
                    fComma = true;
                }

                if (rdoEmailConf2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.EmailConfirmationsFlg = c2.EmailConfirmationsFlg");
                    fComma = true;
                }

                if (rdoSignedBy2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.AnnualContractSignedName = c2.AnnualContractSignedName");
                    fComma = true;
                }

                if (rdoSigned2.Checked)
                {
                    sbSql.Append((fComma ? ", " : string.Empty) + "c.AnnualContractSignedDtTm = c2.AnnualContractSignedDtTm");
                    fComma = true;
                }

                Sql = "Begin Tran;\n";
                if (sbSql.Length > 0)
                {
                    Sql += string.Format(
                        "Update c Set {2} From Customers c Inner Join Customers c2 on c2.CustomerRno = {1} Where c.CustomerRno = {0};\n",
                        Rno1,
                        Rno2,
                        sbSql.ToString());
                }
                Sql += string.Format("Delete From Customers Where CustomerRno = {0};\n", Rno2);
                Sql += string.Format("Update Contacts Set CustomerRno = {0} Where CustomerRno = {1};\n", Rno1, Rno2);
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
            "Select CustomerRno, Name, InactiveDtTm " +
            "From Customers c";
		if (!chkShowHidden.Checked) Sql += " Where (InactiveDtTm Is Null)";
		Sql += " Order by Name, CustomerRno";

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

		lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Customers";
	}

	private void LoadListSql(string Sql, string NextCurrItem)
	{
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				Int32 Rno = DB.Int32(dr["CustomerRno"]);
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
