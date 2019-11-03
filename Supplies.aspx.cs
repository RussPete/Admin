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

public partial class Supplies : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected Int32 JobRno;
	protected int cSupplies;
	protected int cShowSupplies;
	protected int cShowSupplies1;
	protected int cShowSupplies2;
	protected const int cnMinRowsFor2Cols = 10;
	protected const int cnMaxNoteLength = 20;

    private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

		//cSupplies = Str.Num(txtcSupplies.Text);
		cShowSupplies = Str.Num(txtcShowSupplies.Text);
		AddLines();

		// Put user code to initialize the page here
		if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Jobs.Title;
            Setup();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
		RemoveLines();
		LoadSupplies();

		//txtcSupplies.Text = Convert.ToString(cSupplies);
		txtcShowSupplies.Text = Convert.ToString(cShowSupplies);

        //pnlEdit.Visible =
        //    pnlShow.Visible = false;

        //switch (Request["View"])
        //{
        //    case "Show":
        //        pnlShow.Visible = true;
        //        break;

        //    case "Edit":
        //        pnlEdit.Visible = true;
        //        break;

        //    default:
        //        //if (cSupplies > 1)
        //        //{
        //        //    pnlEdit.Visible = true;
        //        //}
        //        //else
        //        //{
        //            pnlShow.Visible = true;
        //        //}
        //        break;
        //}

        pnlShow.Visible = true;
		if (pnlShow.Visible)
		{
			FocusField = "txtQty1";
		}
	}

	private void Setup()
	{
		//LoadList();

		lstList.Attributes.Add("onchange", "ClearDirty();");
		btnUpdate.Attributes.Add("onClick", "ClearDirty();");
		btnShowUpdate.Attributes.Add("onClick", "ClearDirty();");
	}

    //private void RemoveSupplies()
    //{
    //    //if (pnlEdit.Visible && cSupplies > 0) { RemoveEditSupplies(); }
    //    if (pnlShow.Visible && cShowSupplies > 0) { RemoveShowSupplies(); }
    //}

    //private void RemoveEditSupplies()
    //{
    //    for (int iSupply = 1; iSupply <= cSupplies; iSupply++)
    //    {
    //        TableRow tr = tblSupplies.Rows[iSupply];

    //        CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iSupply);
    //        if (chkRemove != null && chkRemove.Checked)
    //        {
    //            String Sql = "";
    //            try
    //            {
    //                Int32 SupplySeq = Str.Num(WebPage.FindTextBox(ref tr, "txtSupplySeq" + iSupply));
    //                Sql = "Delete From mcJobSupplies Where JobRno = " + JobRno + " And SupplySeq = " + SupplySeq;
    //                db.Exec(Sql);
    //            }
    //            catch (Exception Ex)
    //            {
    //                Err Err = new Err(Ex, Sql);
    //                Response.Write(Err.Html());
    //            }
    //        }
    //    }
    //}

    //private void RemoveShowSupplies()
    //{
    //    int iSupply = 0;
    //    RemoveShowSuppliesTable(tblShowSupplies1, cShowSupplies1, ref iSupply);
    //    RemoveShowSuppliesTable(tblShowSupplies2, cShowSupplies2, ref iSupply);
    //}

    //private void RemoveShowSuppliesTable(Table tblShowSupplies, int cShowSupplies, ref int iSupply)
    //{
    //    for (int iRow = 1; iRow <= cShowSupplies; iRow++)
    //    {
    //        TableRow tr = tblShowSupplies.Rows[iRow];
    //        iSupply++;

    //        CheckBox chkInclude = (CheckBox)tr.FindControl("chkInclude" + iSupply);
    //        if (chkInclude != null && !chkInclude.Checked)
    //        {
    //            String Sql = "";
    //            try
    //            {
    //                Int32 SupplySeq = Str.Num(FindLabel(ref tr, "lblSupplySeq" + iSupply));
    //                if (SupplySeq > 0)
    //                {
    //                    Sql = "Delete From mcJobSupplies Where JobRno = " + JobRno + " And SupplySeq = " + SupplySeq;
    //                    db.Exec(Sql);
    //                }
    //            }
    //            catch (Exception Ex)
    //            {
    //                Err Err = new Err(Ex, Sql);
    //                Response.Write(Err.Html());
    //            }
    //        }
    //    }
    //}

	private void SaveSupplies()
	{
		//if (pnlEdit.Visible && cSupplies > 0) { SaveEditSupplies(); }
		if (pnlShow.Visible && cShowSupplies > 0) { SaveShowSupplies(); }
	}

    //private void SaveEditSupplies()
    //{
    //    String Sql = "";

    //    for (int iSupply = 1; iSupply <= cSupplies; iSupply++)
    //    {
    //        TableRow tr = tblSupplies.Rows[iSupply];

    //        CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iSupply);
    //        if (chkRemove != null && !chkRemove.Checked)
    //        {
    //            Int32 SupplySeq = Str.Num(WebPage.FindTextBox(ref tr, "txtSupplySeq" + iSupply));

    //            String SupplyItem = WebPage.FindTextBox(ref tr, "txtSupplyItem" + iSupply);
    //            String OrigSupplyItem = WebPage.FindTextBox(ref tr, "txtOrigSupplyItem" + iSupply);

    //            int Qty = Str.Num(WebPage.FindTextBox(ref tr, "txtQty" + iSupply));
    //            int OrigQty = Str.Num(WebPage.FindTextBox(ref tr, "txtOrigQty" + iSupply));

    //            String Note = WebPage.FindTextBox(ref tr, "txtNote" + iSupply);
    //            String OrigNote = WebPage.FindTextBox(ref tr, "txtOrigNote" + iSupply);

    //            if (SupplyItem != OrigSupplyItem ||
    //                Qty != OrigQty ||
    //                Note != OrigNote)
    //            {
    //                DateTime Tm = DateTime.Now;

    //                try
    //                {
    //                    if (SupplySeq == 0)
    //                    {
    //                        SupplySeq = db.NextSeq("mcJobSupplies", "JobRno", JobRno, "SupplySeq");

    //                        Sql =
    //                            "Insert Into mcJobSupplies (JobRno, SupplySeq, CreatedDtTm, CreatedUser) Values (" +
    //                            JobRno + ", " +
    //                            SupplySeq + ", " +
    //                            DB.PutDtTm(Tm) + ", " +
    //                            DB.PutStr(g.User) + ")";
    //                        db.Exec(Sql);
    //                    }

    //                    Sql =
    //                        "Update mcJobSupplies Set " +
    //                        "SupplyItem = " + DB.PutStr(SupplyItem, 50) + ", " +
    //                        "Qty = " + Qty + ", " +
    //                        "Note = " + DB.PutStr(Note, 128) + ", " +
    //                        "UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
    //                        "UpdatedUser = " + DB.PutStr(g.User) + " " +
    //                        "Where JobRno = " + JobRno + " " +
    //                        "And SupplySeq = " + SupplySeq;
    //                    db.Exec(Sql);
    //                }
    //                catch (Exception Ex)
    //                {
    //                    Err Err = new Err(Ex, Sql);
    //                    Response.Write(Err.Html());
    //                }
    //            }
    //        }
    //    }
    //}

	private void SaveShowSupplies()
	{
        String Sql = "";
        bool fInsert = false;
        try
        {
            Sql = string.Format("Select Count(*) From mcJobSupplies Where JobRno = {0}", JobRno);
            fInsert = (db.SqlNum(Sql) == 0);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

		int iSupply = 0;
        SaveShowSuppliesTable(tblShowSupplies1, cShowSupplies1, ref iSupply, fInsert);
        SaveShowSuppliesTable(tblShowSupplies2, cShowSupplies2, ref iSupply, fInsert);
	}

	private void SaveShowSuppliesTable(Table tblShowSupplies, int cShowSupplies, ref int iSupply, bool fInsert)
	{
        String Sql = "";

		for (int iRow = 1; iRow <= cShowSupplies; iRow++)
		{
			TableRow tr = tblShowSupplies.Rows[iRow];
			iSupply++;

			CheckBox chkInclude = (CheckBox)tr.FindControl("chkInclude" + iSupply);
			Int32 SupplySeq = Str.Num(FindLabel(ref tr, "lblSupplySeq" + iSupply));
			string SupplyItem = FindLabel(ref tr, "lblSupplyItem" + iSupply);
            TextBox txtQty = (TextBox)tr.FindControl("txtQty" + iSupply);
            TextBox txtNote = (TextBox)tr.FindControl("txtNote" + iSupply);
            int? Qty = Str.NullNum(txtQty.Text);
            string Note = Str.NullStr(txtNote.Text);
			DateTime Tm = DateTime.Now;

            if (chkInclude.Checked && !Qty.HasValue)
            {
                Qty = 0;
            }

			try
			{
                if (fInsert)
                {
					Sql = string.Format(
						"Insert Into mcJobSupplies (JobRno, SupplySeq, SupplyItem, Qty, Note, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) Values (" +
                        "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {5}, {6})",
						JobRno, 
						SupplySeq, 
						DB.PutStr(SupplyItem, 50),
                        DB.Put(Qty),
                        DB.Put(Note, 128),
						DB.PutDtTm(Tm),
						DB.PutStr(g.User));
                }
                else
                {
                    Sql = string.Format(
                        "Update mcJobSupplies Set " +
                        "Qty = {2}, " + 
                        "Note = {3}, " +
                        "UpdatedDtTm = {4}, " +
                        "UpdatedUser = {5} " +
                        "Where JobRno = {0} and SupplySeq = {1}",
                        JobRno,
                        SupplySeq,
                        DB.Put(Qty),
                        DB.Put(Note, 128),
						DB.PutDtTm(Tm),
						DB.PutStr(g.User));
                }

				db.Exec(Sql);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
	}

	//private String FindTextBox(ref TableRow tr, String ID)
	//{
	//	String Text = "";

	//	try
	//	{
	//		Control Ctrl = tr.FindControl(ID);
	//		if (Ctrl != null)
	//		{
	//			TextBox txtBox = new TextBox();
	//			HtmlInputHidden txtHidden = new HtmlInputHidden();

	//			if (Ctrl.GetType() == txtBox.GetType())
	//			{
	//				txtBox = (TextBox)Ctrl;
	//				Text = txtBox.Text.Trim();
	//			}
	//			else
	//				if (Ctrl.GetType() == txtHidden.GetType())
	//				{
	//					txtHidden = (HtmlInputHidden)Ctrl;
	//					Text = txtHidden.Value.Trim();
	//				}
	//		}
	//	}
	//	catch (Exception Ex)
	//	{
	//		Err Err = new Err(Ex);
	//		Response.Write(Err.Html());
	//	}

	//	return Text;
	//}

	private String FindLabel(ref TableRow tr, String ID)
	{
		String Label = "";

		try
		{
			Control Ctrl = tr.FindControl(ID);
			if (Ctrl != null)
			{
				Label lblText = new Label();

				if (Ctrl.GetType() == lblText.GetType())
				{
					lblText = (Label)Ctrl;
					Label = lblText.Text.Trim();
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		return Label;
	}

	private void LoadSupplies()
	{
		//LoadEditSupplies();
		LoadShowSupplies();
	}

    //private void LoadEditSupplies()
    //{
    //    if (JobRno > 0)
    //    {
    //        String Sql = "";

    //        try
    //        {
    //            Sql =
    //                "Select f1.SupplySeq " +
    //                "From mcJobSupplies f1 Inner Join mcJobSupplies f2 " +
    //                "On f1.JobRno = f2.JobRno And f1.SupplySeq <> f2.SupplySeq " +
    //                "And f1.SupplyItem = f2.SupplyItem " +
    //                "Where f1.JobRno = " + JobRno;
    //            DupSeq Dups = new DupSeq(db, Sql);

    //            Sql = "Select * From mcJobSupplies Where JobRno = " + JobRno + " Order By SupplySeq";
    //            DataTable dt = db.DataTable(Sql);
    //            cSupplies = dt.Rows.Count + 1;
    //            AddEditLines();

    //            int iRow = 0;
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                iRow++;
    //                TableRow tr = tblSupplies.Rows[iRow];

    //                HtmlInputHidden txtSupplySeq = (HtmlInputHidden)tr.FindControl("txtSupplySeq" + iRow);
    //                txtSupplySeq.Value = DB.Str(dr["SupplySeq"]);

    //                TextBox txtSupplyItem = (TextBox)tr.FindControl("txtSupplyItem" + iRow);
    //                TextBox txtOrigSupplyItem = (TextBox)tr.FindControl("txtOrigSupplyItem" + iRow);
    //                txtSupplyItem.Text =
    //                    txtOrigSupplyItem.Text = DB.Str(dr["SupplyItem"]);
    //                txtSupplyItem.CssClass = (Dups.In(DB.Int32(dr["SupplySeq"])) ? "Dup" : "") + "SupplyItem";

    //                TextBox txtQty = (TextBox)tr.FindControl("txtQty" + iRow);
    //                TextBox txtOrigQty = (TextBox)tr.FindControl("txtOrigQty" + iRow);
    //                txtQty.Text =
    //                    txtOrigQty.Text = Fmt.Num(DB.Int32(dr["Qty"]), false);

    //                TextBox txtNote = (TextBox)tr.FindControl("txtNote" + iRow);
    //                TextBox txtOrigNote = (TextBox)tr.FindControl("txtOrigNote" + iRow);
    //                txtNote.Text =
    //                    txtOrigNote.Text = DB.Str(dr["Note"]);
    //            }
    //        }
    //        catch (Exception Ex)
    //        {
    //            Err Err = new Err(Ex, Sql);
    //            Response.Write(Err.Html());
    //        }
    //    }
    //    else
    //    {
    //        cSupplies = 1;
    //        AddEditLines();
    //    }

    //    FocusField = "txtSupplyItem" + cSupplies;
    //}

	private void LoadShowSupplies()
	{
		if (JobRno > 0)
		{
			String Sql = string.Format(
                "Select SupplySeq, SupplyItem, Qty, Note, (Case When Qty Is Null Then 0 Else 1 End) InJob \n" +
	            "From mcJobSupplies \n" +
	            "Where JobRno = (Select Coalesce((Select Top 1 JobRno From mcJobSupplies Where JobRno = {0}), 0)) \n" +
	            "Order By SupplySeq", 
				JobRno);
			try
			{
				DataTable dt = db.DataTable(Sql);
				cShowSupplies = dt.Rows.Count;
				AddShowLines();

				int iRow = 0;
				int iSupply = 0;
				Table tblShowSupplies = tblShowSupplies1;

				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					iSupply++;
					LoadShowSupply(tblShowSupplies, iRow, iSupply, dr);

					if (iSupply == cShowSupplies1)
					{
						tblShowSupplies = tblShowSupplies2;
						iRow = 0;
					}
				}
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		else
		{
			cShowSupplies = 1;
			AddShowLines();
		}
	}

	private void LoadShowSupply(Table tblShowSupplies, int iRow, int iSupply, DataRow dr)
	{
		TableRow tr = tblShowSupplies.Rows[iRow];

		CheckBox chkInclude = (CheckBox)tr.FindControl("chkInclude" + iSupply);
		chkInclude.Checked = DB.Bool(dr["InJob"]);

		Label lblSupplySeq = (Label)tr.FindControl("lblSupplySeq" + iSupply);
		lblSupplySeq.Text = Convert.ToString(DB.Int32(dr["SupplySeq"]));

		Label lblSupplyItem = (Label)tr.FindControl("lblSupplyItem" + iSupply);
		lblSupplyItem.Text = DB.Str(dr["SupplyItem"]);

        TextBox txtQty = (TextBox)tr.FindControl("txtQty" + iSupply);
        txtQty.Text = Fmt.Num(DB.Int32(dr["Qty"]), false);

        TextBox txtNote = (TextBox)tr.FindControl("txtNote" + iSupply);
        txtNote.Text = DB.Str(dr["Note"]);
	}

    //private void LoadList()
    //{
    //    //string Sql = "Select Distinct SupplyItem From mcJobSupplies Where SupplyItem Not In (Select SupplyItem From mcJobSupplies Where JobRno = -1) Order By SupplyItem";
    //    string Sql = "Select SupplyItem From mcJobSupplies Where JobRno = 0 And SupplyItem Not In (Select SupplyItem From mcJobSupplies Where JobRno = -1) Order By SupplySeq";
    //    string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");
    //    lstList.Items.Clear();

    //    try
    //    {
    //        DataTable dt = db.DataTable(Sql);
    //        foreach (DataRow r in dt.Rows)
    //        {
    //            ListItem Item = new ListItem(DB.Str(r["SupplyItem"]));
    //            Item.Selected = (Item.Value == CurrItem);
    //            lstList.Items.Add(Item);
    //        }

    //        lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Supplies";
    //    }
    //    catch (Exception Ex)
    //    {
    //        Err Err = new Err(Ex);
    //        Response.Write(Err.Html());
    //    }
    //}

    protected void UpdateList(object sender, System.EventArgs e)
    {
        //LoadList();
        Update();
    }

	private void AddLines()
	{
		//if (cSupplies > 0) { AddEditLines(); }
		if (cShowSupplies > 0) { AddShowLines(); }
	}

    //private void AddEditLines()
    //{
    //    String Sql;
    //    SelectList.Clear();

    //    for (int iSupply = 1; iSupply <= cSupplies; iSupply++)
    //    {
    //        TableRow r = new TableRow();

    //        for (int iCell = 0; iCell < 9; iCell++)
    //        {
    //            r.Cells.Add(new TableCell());
    //        }

    //        r.Cells[0].Text = "<b>" + iSupply + "</b>";

    //        HtmlInputHidden txtSupplySeq = new HtmlInputHidden();
    //        txtSupplySeq.ID = "txtSupplySeq" + iSupply;
    //        r.Cells[1].Controls.Add(txtSupplySeq);

    //        // Remove
    //        CheckBox chkRemove = new CheckBox();
    //        chkRemove.ID = "chkRemove" + iSupply;
    //        chkRemove.Attributes.Add("onClick", "SetDirty();");
    //        r.Cells[2].Controls.Add(chkRemove);
    //        r.Cells[2].HorizontalAlign = HorizontalAlign.Center;

    //        // SupplyItem
    //        TextBox txtSupplyItem = new TextBox();
    //        txtSupplyItem.ID = "txtSupplyItem" + iSupply;
    //        txtSupplyItem.MaxLength = 50;
    //        txtSupplyItem.CssClass = "SupplyItem";
    //        txtSupplyItem.AutoPostBack = true;
    //        txtSupplyItem.Attributes.Add("onChange", "HideDirty();");
    //        txtSupplyItem.TextChanged += new System.EventHandler(this.btnUpdate_Click);
    //        r.Cells[4].Controls.Add(txtSupplyItem);

    //        System.Web.UI.WebControls.Image imgSupplyItem = new System.Web.UI.WebControls.Image();
    //        imgSupplyItem.ID = "ddSupplyItem" + iSupply;
    //        imgSupplyItem.ImageUrl = "Images/DropDown.gif";
    //        imgSupplyItem.BorderWidth = 0;
    //        imgSupplyItem.ImageAlign = ImageAlign.AbsMiddle;
    //        r.Cells[4].Controls.Add(imgSupplyItem);

    //        Sql = "Select Distinct SupplyItem From mcJobSupplies Where SupplyItem Not In (Select SupplyItem From mcJobSupplies Where JobRno = -1) Order By SupplyItem";
    //        SelectList slSupplyItem = new SelectList("SupplyItem" + iSupply, ref txtSupplyItem);
    //        slSupplyItem.ImageButton(imgSupplyItem);
    //        slSupplyItem.AutoPostBack = true;
    //        slSupplyItem.Height = 400;
    //        slSupplyItem.ClearValues();
    //        slSupplyItem.AddDBValues(db, Sql);

    //        txtSupplyItem = new TextBox();
    //        txtSupplyItem.ID = "txtOrigSupplyItem" + iSupply;
    //        txtSupplyItem.Visible = false;
    //        r.Cells[4].Controls.Add(txtSupplyItem);

    //        // Qty
    //        TextBox txtQty = new TextBox();
    //        txtQty.ID = "txtQty" + iSupply;
    //        txtQty.Style.Add("width", "50px");
    //        txtQty.Attributes.Add("onChange", "SetDirty();");
    //        r.Cells[6].Controls.Add(txtQty);

    //        txtQty = new TextBox();
    //        txtQty.ID = "txtOrigQty" + iSupply;
    //        txtQty.Visible = false;
    //        r.Cells[6].Controls.Add(txtQty);

    //        // Note
    //        TextBox txtNote = new TextBox();
    //        txtNote.ID = "txtNote" + iSupply;
    //        txtNote.MaxLength = 128;
    //        txtNote.Style.Add("width", "150px");
    //        if (iSupply == cSupplies)
    //        {
    //            //txtNote.AutoPostBack = true;
    //            txtNote.Attributes.Add("onBlur", "this.value+=' ';ClearDirty();__doPostBack('" + txtNote.ID + "','');");
    //            txtNote.Attributes.Add("onChange", "ClearDirty();");
    //            txtNote.TextChanged += new System.EventHandler(this.btnUpdate_Click);
    //        }
    //        else
    //        {
    //            txtNote.Attributes.Add("onChange", "SetDirty();");
    //        }
    //        r.Cells[8].Controls.Add(txtNote);

    //        txtNote = new TextBox();
    //        txtNote.ID = "txtOrigNote" + iSupply;
    //        txtNote.Visible = false;
    //        r.Cells[8].Controls.Add(txtNote);

    //        tblSupplies.Rows.Add(r);
    //    }
    //}

	private void AddShowLines()
	{
		int iSupply = 0;

		if (cShowSupplies < 2 * cnMinRowsFor2Cols)
		{
			cShowSupplies1 = cShowSupplies;
			cShowSupplies2 = 0;

			AddShowTable(tblShowSupplies1, tdShowSupplies1, cShowSupplies1, ref iSupply);
		}
		else
		{
			cShowSupplies1 = (cShowSupplies + 1) / 2;
			cShowSupplies2 = cShowSupplies - cShowSupplies1;

			AddShowTable(tblShowSupplies1, tdShowSupplies1, cShowSupplies1, ref iSupply);
			AddShowTable(tblShowSupplies2, tdShowSupplies2, cShowSupplies2, ref iSupply);
		}
	}

	private void AddShowTable(Table tblShowSupplies, TableCell tdShowSupplies, int cShowSupplies, ref int iSupply)
	{
		AddShowHeader(tblShowSupplies, tdShowSupplies);

		for (int iRow = 1; iRow <= cShowSupplies; iRow++)
		{
			TableRow r = new TableRow();
			iSupply++;

            for (int iCell = 0; iCell < 7; iCell++)
            //for (int iCell = 0; iCell < 3; iCell++)
            {
				r.Cells.Add(new TableCell());
			}

			// Include
			CheckBox chkInclude = new CheckBox();
			chkInclude.ID = "chkInclude" + iSupply;
            chkInclude.TabIndex = -1;
			chkInclude.Attributes.Add("onClick", "SetDirty();");
			r.Cells[0].Controls.Add(chkInclude);
			r.Cells[0].HorizontalAlign = HorizontalAlign.Center;

			Label lblSupplySeq = new Label();
			lblSupplySeq.ID = "lblSupplySeq" + iSupply;
			lblSupplySeq.Visible = false;
			r.Cells[1].Controls.Add(lblSupplySeq);

			// SupplyItem
			Label lblSupplyItem = new Label();
			lblSupplyItem.ID = "lblSupplyItem" + iSupply;
			r.Cells[2].Controls.Add(lblSupplyItem);

            // Qty
            TextBox txtQty = new TextBox();
            txtQty.ID = "txtQty" + iSupply;
            txtQty.CssClass = "Qty";
            r.Cells[4].Controls.Add(txtQty);
            r.Cells[4].HorizontalAlign = HorizontalAlign.Right;

            // Note
            TextBox txtNote = new TextBox();
            txtNote.ID = "txtNote" + iSupply;
            txtNote.CssClass = "Note";
            txtNote.TabIndex = -1;
            r.Cells[6].Controls.Add(txtNote);
            r.Cells[6].Wrap = false;

			tblShowSupplies.Rows.Add(r);
		}
	}

	private void AddShowHeader(Table tbl, TableCell td)
	{
		TableRow r;
		TableCell c;

		td.Visible = true;
		tbl.Rows.Add(r = new TableRow());

		r.Cells.Add(c = new TableCell());
		c.Text = "Include";
		c.Font.Bold = true;

		r.Cells.Add(c = new TableCell());
		c.Width = 10;

		r.Cells.Add(c = new TableCell());
		c.Text = "Supply Item";
		c.Font.Bold = true;

        //r.Cells.Add(c = new TableCell());
        //c.Width = 10;

        //r.Cells.Add(c = new TableCell());
        //c.Text = "Quantity";
        //c.Font.Bold = true;

        //r.Cells.Add(c = new TableCell());
        //c.Width = 10;

        //r.Cells.Add(c = new TableCell());
        //c.Text = "Note";
        //c.Font.Bold = true;

		if (tbl == tblShowSupplies2)
		{
			imgShowSupplies.Width = 20;
		}
	}

	private void RemoveLines()
	{
        //while (tblSupplies.Rows.Count > 1)
        //{
        //    tblSupplies.Rows.Remove(tblSupplies.Rows[1]);
        //}
		while (tblShowSupplies1.Rows.Count > 0)
		{
			tblShowSupplies1.Rows.Remove(tblShowSupplies1.Rows[0]);
		}
		while (tblShowSupplies2.Rows.Count > 0)
		{
			tblShowSupplies2.Rows.Remove(tblShowSupplies2.Rows[0]);
		}
	}

    protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
    {
    //    RemoveSupplies();
    //    SaveSupplies();

    //    String Sql = "";

    //    try
    //    {
    //        String SupplyItem = lstList.SelectedItem.Value;

    //        Int32 SupplySeq = db.NextSeq("mcJobSupplies", "JobRno", JobRno, "SupplySeq");

    //        Sql =
    //            "Insert Into mcJobSupplies (JobRno, SupplySeq, SupplyItem, CreatedDtTm, CreatedUser) Values (" +
    //            JobRno + ", " +
    //            SupplySeq + ", " +
    //            DB.PutStr(SupplyItem, 50) + ", " +
    //            DB.PutDtTm(DateTime.Now) + ", " +
    //            DB.PutStr(g.User) + ")";
    //        db.Exec(Sql);
    //    }
    //    catch (Exception Ex)
    //    {
    //        Err Err = new Err(Ex, Sql);
    //        Response.Write(Err.Html());
    //    }

        //LoadList();
    }

	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		Update();

		if (JobRno > 0)
		{
			String Sql = "";
			try
			{
				Sql = "Select Count(*) From mcJobSupplies Where JobRno = " + JobRno;
				cSupplies = db.SqlNum(Sql);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		else
		{
			cSupplies = 0;
		}
	}

	private void Update()
	{
		//RemoveSupplies();
		SaveSupplies();
		//LoadList();
	}
}