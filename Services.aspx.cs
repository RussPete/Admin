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

public partial class Services : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected Int32 JobRno;
	protected int cServices;
	protected int cShowServices;
	protected int cShowServices1;
	protected int cShowServices2;
	protected const int cnMinRowsFor2Cols = 6;
	protected const int cnMaxNoteLength = 20;

    private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

		cServices = Str.Num(txtcServices.Text);
		cShowServices = Str.Num(txtcShowServices.Text);
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
		LoadServices();

		txtcServices.Text = Convert.ToString(cServices);
		txtcShowServices.Text = Convert.ToString(cShowServices);

		pnlEdit.Visible =
		pnlShow.Visible = false;

		switch (Request["View"])
		{
			case "Show":
				pnlShow.Visible = true;
				break;

			case "Edit":
				pnlEdit.Visible = true;
				break;

			default:
                //if (cServices > 1)
                //{
                //    pnlEdit.Visible = true;
                //}
                //else
                //{
					pnlShow.Visible = true;
				//}
				break;
		}

		if (pnlShow.Visible)
		{
			FocusField = "chkShowMCCResp1";
		}
	}

	private void Setup()
	{
		LoadList();

		lstList.Attributes.Add("onchange", "ClearDirty();");
		btnUpdate.Attributes.Add("onClick", "ClearDirty();");
		btnShowUpdate.Attributes.Add("onClick", "ClearDirty();");
	}

	private void RemoveServices()
	{
		if (pnlEdit.Visible && cServices > 0) { RemoveEditServices(); }
		if (pnlShow.Visible && cShowServices > 0) { RemoveShowServices(); }
	}

	private void RemoveEditServices()
	{
		for (int iService = 1; iService <= cServices; iService++)
		{
			TableRow tr = tblServices.Rows[iService + 1];

			CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iService);
			if (chkRemove != null && chkRemove.Checked)
			{
				String Sql = "";
				try
				{
					Int32 ServiceSeq = Str.Num(WebPage.FindTextBox(ref tr, "txtServiceSeq" + iService));
					Sql = "Delete From mcJobServices Where JobRno = " + JobRno + " And ServiceSeq = " + ServiceSeq;
					db.Exec(Sql);
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
			}
		}
	}

	private void RemoveShowServices()
	{
		int iService = 0;
		RemoveShowServicesTable(tblShowServices1, cShowServices1, ref iService);
		RemoveShowServicesTable(tblShowServices2, cShowServices2, ref iService);
	}

	private void RemoveShowServicesTable(Table tblShowServices, int cShowServices, ref int iService)
	{
		for (int iRow = 1; iRow <= cShowServices; iRow++)
		{
			TableRow tr = tblShowServices.Rows[iRow + 1];
			iService++;

			CheckBox chkShowMCCResp = (CheckBox)tr.FindControl("chkShowMCCResp" + iService);
			CheckBox chkShowCustomerResp = (CheckBox)tr.FindControl("chkShowCustomerResp" + iService);
			if (chkShowMCCResp != null && !chkShowMCCResp.Checked &&
				chkShowCustomerResp != null && !chkShowCustomerResp.Checked)
			{
				String Sql = "";
				try
				{
					Int32 ServiceSeq = Str.Num(FindLabel(ref tr, "lblServiceSeq" + iService));
					if (ServiceSeq > 0)
					{
						Sql = "Delete From mcJobServices Where JobRno = " + JobRno + " And ServiceSeq = " + ServiceSeq;
						db.Exec(Sql);
					}
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
			}
		}
	}

	private void SaveServices()
	{
		if (pnlEdit.Visible && cServices > 0) { SaveEditServices(); }
		if (pnlShow.Visible && cShowServices > 0) { SaveShowServices(); }
	}

	private void SaveEditServices()
	{
		String Sql = "";

		for (int iService = 1; iService <= cServices; iService++)
		{
			TableRow tr = tblServices.Rows[iService + 1];

			CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iService);
			if (chkRemove != null && !chkRemove.Checked)
			{
				Int32 ServiceSeq = Str.Num(WebPage.FindTextBox(ref tr, "txtServiceSeq" + iService));

				String ServiceItem = WebPage.FindTextBox(ref tr, "txtServiceItem" + iService);
				String OrigServiceItem = WebPage.FindTextBox(ref tr, "txtOrigServiceItem" + iService);

				bool MCCResp = FindCheckBox(ref tr, "chkMCCResp" + iService);
				bool OrigMCCResp = FindCheckBox(ref tr, "chkOrigMCCResp" + iService);

				bool CustomerResp = FindCheckBox(ref tr, "chkCustomerResp" + iService);
				bool OrigCustomerResp = FindCheckBox(ref tr, "chkOrigCustomerResp" + iService);

				String Note = WebPage.FindTextBox(ref tr, "txtNote" + iService);
				String OrigNote = WebPage.FindTextBox(ref tr, "txtOrigNote" + iService);

				if (ServiceItem != OrigServiceItem ||
					MCCResp != OrigMCCResp ||
					CustomerResp != OrigCustomerResp ||
					Note != OrigNote)
				{
					DateTime Tm = DateTime.Now;

					try
					{
						if (ServiceSeq == 0)
						{
							ServiceSeq = db.NextSeq("mcJobServices", "JobRno", JobRno, "ServiceSeq");

							Sql =
								"Insert Into mcJobServices (JobRno, ServiceSeq, CreatedDtTm, CreatedUser) Values (" +
								JobRno + ", " +
								ServiceSeq + ", " +
								DB.PutDtTm(Tm) + ", " +
								DB.PutStr(g.User) + ")";
							db.Exec(Sql);

							MCCResp = true;	// set on by default for new selections
						}

						Sql =
							"Update mcJobServices Set " +
							"ServiceItem = " + DB.PutStr(ServiceItem, 50) + ", " +
							"MCCRespFlg = " + DB.PutBool(MCCResp) + ", " +
							"CustomerRespFlg = " + DB.PutBool(CustomerResp) + ", " +
							"Note = " + DB.PutStr(Note, 128) + ", " +
							"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
							"UpdatedUser = " + DB.PutStr(g.User) + " " +
							"Where JobRno = " + JobRno + " " +
							"And ServiceSeq = " + ServiceSeq;
						db.Exec(Sql);
					}
					catch (Exception Ex)
					{
						Err Err = new Err(Ex, Sql);
						Response.Write(Err.Html());
					}
				}
			}
		}
	}

	private void SaveShowServices()
	{
		int iService = 0;
		SaveShowServicesTable(tblShowServices1, cShowServices1, ref iService);
		SaveShowServicesTable(tblShowServices2, cShowServices2, ref iService);
	}

	private void SaveShowServicesTable(Table tblShowServices, int cShowServices, ref int iService)
	{
		String Sql = "";

		for (int iRow = 1; iRow <= cShowServices; iRow++)
		{
			TableRow tr = tblShowServices.Rows[iRow + 1];
			iService++;

			CheckBox chkShowMCCResp = (CheckBox)tr.FindControl("chkShowMCCResp" + iService);
			CheckBox chkShowCustomerResp = (CheckBox)tr.FindControl("chkShowCustomerResp" + iService);

			if (chkShowMCCResp != null && chkShowCustomerResp != null)
			{
				Int32 ServiceSeq = Str.Num(FindLabel(ref tr, "lblServiceSeq" + iService));
				String ServiceItem = FindLabel(ref tr, "lblServiceItem" + iService);
				DateTime Tm = DateTime.Now;

				try
				{
					if (chkShowMCCResp.Checked ||
						chkShowCustomerResp.Checked)
					{
						if (ServiceSeq == 0)
						{
							ServiceSeq = db.NextSeq("mcJobServices", "JobRno", JobRno, "ServiceSeq");

							Sql =
								"Insert Into mcJobServices (JobRno, ServiceSeq, CreatedDtTm, CreatedUser) Values (" +
								JobRno + ", " +
								ServiceSeq + ", " +
								DB.PutDtTm(Tm) + ", " +
								DB.PutStr(g.User) + ")";
							db.Exec(Sql);
						}

						Sql =
							"Update mcJobServices Set " +
							"ServiceItem = " + DB.PutStr(ServiceItem, 50) + ", " +
							"MCCRespFlg = " + DB.PutBool(chkShowMCCResp.Checked) + ", " +
							"CustomerRespFlg = " + DB.PutBool(chkShowCustomerResp.Checked) + ", " +
							"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
							"UpdatedUser = " + DB.PutStr(g.User) + " " +
							"Where JobRno = " + JobRno + " " +
							"And ServiceSeq = " + ServiceSeq;
						db.Exec(Sql);
					}
					else
					{
						if (ServiceSeq > 0)
						{
							Sql =
								"Delete From mcJobServices " +
								"Where JobRno = " + JobRno + " " +
								"And ServiceSeq = " + ServiceSeq;
							db.Exec(Sql);
						}
					}
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
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

	private bool FindCheckBox(ref TableRow tr, String ID)
	{
		bool fChecked = false;

		try
		{
			Control Ctrl = tr.FindControl(ID);
			if (Ctrl != null)
			{
				CheckBox chkBox = new CheckBox();

				if (Ctrl.GetType() == chkBox.GetType())
				{
					chkBox = (CheckBox)Ctrl;
					fChecked = chkBox.Checked;
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		return fChecked;
	}

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

	private void LoadServices()
	{
		LoadEditServices();
		LoadShowServices();
	}

	private void LoadEditServices()
	{
		if (JobRno > 0)
		{
			String Sql = "";

			try
			{
				Sql =
					"Select f1.ServiceSeq " +
					"From mcJobServices f1 Inner Join mcJobServices f2 " +
					"On f1.JobRno = f2.JobRno And f1.ServiceSeq <> f2.ServiceSeq " +
					"And f1.ServiceItem = f2.ServiceItem " +
					"Where f1.JobRno = " + JobRno;
				DupSeq Dups = new DupSeq(db, Sql);

				Sql = "Select * From mcJobServices Where JobRno = " + JobRno + " Order By ServiceSeq";
				DataTable dt = db.DataTable(Sql);
				cServices = dt.Rows.Count + 1;
				AddEditLines();

				int iRow = 0;
				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					TableRow tr = tblServices.Rows[iRow];

					HtmlInputHidden txtServiceSeq = (HtmlInputHidden)tr.FindControl("txtServiceSeq" + iRow);
					txtServiceSeq.Value = DB.Str(dr["ServiceSeq"]);

					TextBox txtServiceItem = (TextBox)tr.FindControl("txtServiceItem" + iRow);
					TextBox txtOrigServiceItem = (TextBox)tr.FindControl("txtOrigServiceItem" + iRow);
					txtServiceItem.Text =
					txtOrigServiceItem.Text = DB.Str(dr["ServiceItem"]);
					txtServiceItem.CssClass = (Dups.In(DB.Int32(dr["ServiceSeq"])) ? "Dup" : "") + "ServiceItem";

					bool fMCC = DB.Bool(dr["MCCRespFlg"]);
					bool fCustomer = DB.Bool(dr["CustomerRespFlg"]);
					string Class = (!fMCC && !fCustomer ? "NotChecked" : "");

					CheckBox chkMCCResp = (CheckBox)tr.FindControl("chkMCCResp" + iRow);
					CheckBox chkOrigMCCResp = (CheckBox)tr.FindControl("chkOrigMCCResp" + iRow);
					chkMCCResp.Checked =
					chkOrigMCCResp.Checked = fMCC;
					chkMCCResp.CssClass = Class;

					CheckBox chkCustomerResp = (CheckBox)tr.FindControl("chkCustomerResp" + iRow);
					CheckBox chkOrigCustomerResp = (CheckBox)tr.FindControl("chkOrigCustomerResp" + iRow);
					chkCustomerResp.Checked =
					chkOrigCustomerResp.Checked = fCustomer;
					chkCustomerResp.CssClass = Class;

					TextBox txtNote = (TextBox)tr.FindControl("txtNote" + iRow);
					TextBox txtOrigNote = (TextBox)tr.FindControl("txtOrigNote" + iRow);
					txtNote.Text =
					txtOrigNote.Text = DB.Str(dr["Note"]);
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
			cServices = 1;
			AddLines();
		}

		FocusField = "txtServiceItem" + cServices;
	}

	private void LoadShowServices()
	{
		if (JobRno > 0)
		{
			String Sql =
				"Select s.ServiceItem, s.ServiceSeq As Seq, js.ServiceSeq, js.MCCRespFlg, js.CustomerRespFlg, js.Note " +
				"From mcJobServices s Left Outer Join mcJobServices js " +
				"On s.ServiceItem = js.ServiceItem " +
				"And js.JobRno = " + JobRno + " " +
				"Where s.JobRno = 0 " +
				"Union " +
				"Select ServiceItem, (1000 + ServiceSeq) As Seq, ServiceSeq, MCCRespFlg, CustomerRespFlg, Note " +
				"From mcJobServices " +
				"Where JobRno = " + JobRno + " " +
				"And ServiceItem Not In (Select ServiceItem From mcJobServices Where JobRno = 0) " +
				"Order By Seq";

			try
			{
				DataTable dt = db.DataTable(Sql);
				cShowServices = dt.Rows.Count;
				AddShowLines();

				int iRow = 1;
				int iService = 0;
				Table tblShowServices = tblShowServices1;

				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					iService++;
					LoadShowService(tblShowServices, iRow, iService, dr);

					if (iService == cShowServices1)
					{
						tblShowServices = tblShowServices2;
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
			cShowServices = 1;
			AddShowLines();
		}
	}

	private void LoadShowService(Table tblShowServices, int iRow, int iService, DataRow dr)
	{
		TableRow tr = tblShowServices.Rows[iRow];

		Int32 ServiceSeq = DB.Int32(dr["ServiceSeq"]);

		Label lblServiceItem = (Label)tr.FindControl("lblServiceItem" + iService);
		lblServiceItem.Text = DB.Str(dr["ServiceItem"]);

		Label lblServiceSeq = (Label)tr.FindControl("lblServiceSeq" + iService);
		lblServiceSeq.Text = Convert.ToString(ServiceSeq);

		CheckBox chkShowMCCResp = (CheckBox)tr.FindControl("chkShowMCCResp" + iService);
		chkShowMCCResp.Checked = DB.Bool(dr["MCCRespFlg"]);

		CheckBox chkShowCustomerResp = (CheckBox)tr.FindControl("chkShowCustomerResp" + iService);
		chkShowCustomerResp.Checked = DB.Bool(dr["CustomerRespFlg"]);

        //Label lblNote = (Label)tr.FindControl("lblNote" + iService);
        //string Note = DB.Str(dr["Note"]);
        //if (Note.Length > cnMaxNoteLength)
        //{
        //    lblNote.Text = Note.Substring(0, cnMaxNoteLength);
        //    lblNote.ToolTip = Note;
        //}
        //else
        //{
        //    lblNote.Text = Note;
        //}
	}

	private void LoadList()
	{
		string Sql = 
			"Select ServiceItem From mcJobServices Where JobRno = 0 Order By ServiceSeq";
		string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");
		lstList.Items.Clear();

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				ListItem Item = new ListItem(DB.Str(r["ServiceItem"]));
                if (Item.Value.Length > 0)
                {
                    Item.Selected = (Item.Value == CurrItem);
                    lstList.Items.Add(Item);
                }
			}

			lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Services";
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}
	}

	protected void UpdateList(object sender, System.EventArgs e)
	{
		//LoadList();
		Update();
	}

	private void AddLines()
	{
		if (cServices > 0) { AddEditLines(); }
		if (cShowServices > 0) { AddShowLines(); }
	}

	private void AddEditLines()
	{
		String Sql;
		SelectList.Clear();

		for (int iService = 1; iService <= cServices; iService++)
		{
			TableRow r = new TableRow();

			for (int iCell = 0; iCell < 10; iCell++)
			{
				r.Cells.Add(new TableCell());
			}

			r.Cells[0].Text = "<b>" + iService + "</b>";

			HtmlInputHidden txtServiceSeq = new HtmlInputHidden();
			txtServiceSeq.ID = "txtServiceSeq" + iService;
			r.Cells[1].Controls.Add(txtServiceSeq);

			// Remove
			CheckBox chkRemove = new CheckBox();
			chkRemove.ID = "chkRemove" + iService;
			chkRemove.Attributes.Add("onClick", "SetDirty();");
			r.Cells[2].Controls.Add(chkRemove);
			r.Cells[2].HorizontalAlign = HorizontalAlign.Center;

			// ServiceItem
			TextBox txtServiceItem = new TextBox();
			txtServiceItem.ID = "txtServiceItem" + iService;
			txtServiceItem.MaxLength = 50;
			txtServiceItem.CssClass = "ServiceItem";
			txtServiceItem.AutoPostBack = true;
			txtServiceItem.Attributes.Add("onChange", "HideDirty();");
			txtServiceItem.TextChanged += new System.EventHandler(this.btnUpdate_Click);
			r.Cells[4].Controls.Add(txtServiceItem);

			System.Web.UI.WebControls.Image imgServiceItem = new System.Web.UI.WebControls.Image();
			imgServiceItem.ID = "ddServiceItem" + iService;
			imgServiceItem.ImageUrl = "Images/DropDown.gif";
			imgServiceItem.BorderWidth = 0;
			imgServiceItem.ImageAlign = ImageAlign.AbsMiddle;
			r.Cells[4].Controls.Add(imgServiceItem);

			Sql = "Select Distinct ServiceItem From mcJobServices Order By ServiceItem";
			SelectList slServiceItem = new SelectList("ServiceItem" + iService, ref txtServiceItem);
			slServiceItem.ImageButton(imgServiceItem);
			slServiceItem.AutoPostBack = true;
			slServiceItem.ClearValues();
			slServiceItem.AddDBValues(db, Sql);

			txtServiceItem = new TextBox();
			txtServiceItem.ID = "txtOrigServiceItem" + iService;
			txtServiceItem.Visible = false;
			r.Cells[4].Controls.Add(txtServiceItem);

			// MCC Resp
			CheckBox chkMCCResp = new CheckBox();
			chkMCCResp.ID = "chkMCCResp" + iService;
			chkMCCResp.Attributes.Add("onClick", "SetDirty();");
			r.Cells[6].Controls.Add(chkMCCResp);
			r.Cells[6].HorizontalAlign = HorizontalAlign.Center;

			chkMCCResp = new CheckBox();
			chkMCCResp.ID = "chkOrigMCCResp" + iService;
			chkMCCResp.Visible = false;
			r.Cells[6].Controls.Add(chkMCCResp);

			// Customer Resp
			CheckBox chkCustomerResp = new CheckBox();
			chkCustomerResp.ID = "chkCustomerResp" + iService;
			chkCustomerResp.Attributes.Add("onClick", "SetDirty();");
			r.Cells[7].Controls.Add(chkCustomerResp);
			r.Cells[7].HorizontalAlign = HorizontalAlign.Center;

			chkCustomerResp = new CheckBox();
			chkCustomerResp.ID = "chkOrigCustomerResp" + iService;
			chkCustomerResp.Visible = false;
			r.Cells[7].Controls.Add(chkCustomerResp);

			// Note
			TextBox txtNote = new TextBox();
			txtNote.ID = "txtNote" + iService;
			txtNote.MaxLength = 128;
			txtNote.Style.Add("width", "150px");
			if (iService == cServices)
			{
				//txtNote.AutoPostBack = true;
				txtNote.Attributes.Add("onBlur", "this.value+=' ';ClearDirty();__doPostBack('" + txtNote.ID + "','');");
				txtNote.Attributes.Add("onChange", "ClearDirty();");
				txtNote.TextChanged += new System.EventHandler(this.btnUpdate_Click);
			}
			else
			{
				txtNote.Attributes.Add("onChange", "SetDirty();");
			}
			r.Cells[9].Controls.Add(txtNote);

			txtNote = new TextBox();
			txtNote.ID = "txtOrigNote" + iService;
			txtNote.Visible = false;
			r.Cells[9].Controls.Add(txtNote);

			tblServices.Rows.Add(r);
		}
	}

	private void AddShowLines()
	{
		int iService = 0;

		if (cShowServices < 2 * cnMinRowsFor2Cols)
		{
			cShowServices1 = cShowServices;
			cShowServices2 = 0;

			AddShowTable(tblShowServices1, tdShowServices1, cShowServices1, ref iService);
		}
		else
		{
			cShowServices1 = (cShowServices + 1) / 2;
			cShowServices2 = cShowServices - cShowServices1;

			AddShowTable(tblShowServices1, tdShowServices1, cShowServices1, ref iService);
			AddShowTable(tblShowServices2, tdShowServices2, cShowServices2, ref iService);
		}
	}

	private void AddShowTable(Table tblShowServices, TableCell tdShowServices, int cShowServices, ref int iService)
	{
		AddShowHeader(tblShowServices, tdShowServices);

		for (int iRow = 1; iRow <= cShowServices; iRow++)
		{
			TableRow r = new TableRow();
			iService++;

			//for (int iCell = 0; iCell < 6; iCell++)
            for (int iCell = 0; iCell < 4; iCell++)
            {
				r.Cells.Add(new TableCell());
			}

			// ServiceItem
			Label lblServiceItem = new Label();
			lblServiceItem.ID = "lblServiceItem" + iService;
			r.Cells[0].Controls.Add(lblServiceItem);

			Label lblServiceSeq = new Label();
			lblServiceSeq.ID = "lblServiceSeq" + iService;
			lblServiceSeq.Visible = false;
			r.Cells[1].Controls.Add(lblServiceSeq);

			// MCCResp
			CheckBox chkShowMCCResp = new CheckBox();
			chkShowMCCResp.ID = "chkShowMCCResp" + iService;
			chkShowMCCResp.Attributes.Add("onClick", "SetDirty();");
			r.Cells[2].Controls.Add(chkShowMCCResp);
			r.Cells[2].HorizontalAlign = HorizontalAlign.Center;

			// CustomerResp
			CheckBox chkShowCustomerResp = new CheckBox();
			chkShowCustomerResp.ID = "chkShowCustomerResp" + iService;
			chkShowCustomerResp.Attributes.Add("onClick", "SetDirty();");
			r.Cells[3].Controls.Add(chkShowCustomerResp);
			r.Cells[3].HorizontalAlign = HorizontalAlign.Center;

            //// Note
            //Label lblNote = new Label();
            //lblNote.ID = "lblNote" + iService;
            //r.Cells[5].Controls.Add(lblNote);
            //r.Cells[5].Wrap = false;

			tblShowServices.Rows.Add(r);
		}
	}

	private void AddShowHeader(Table tbl, TableCell td)
	{
		TableRow r;
		TableCell c;

		td.Visible = true;
		tbl.Rows.Add(r = new TableRow());

		r.Cells.Add(c = new TableCell());
		r.Cells.Add(c = new TableCell());

		r.Cells.Add(c = new TableCell());
		c.Text = "Responsible Party";
		c.Font.Bold = true;
		c.ColumnSpan = 2;
		c.HorizontalAlign = HorizontalAlign.Center;

		r.Cells.Add(c = new TableCell());
		r.Cells.Add(c = new TableCell());

		tbl.Rows.Add(r = new TableRow());

		r.Cells.Add(c = new TableCell());
		c.Text = "Service Item";
		c.Font.Bold = true;

		r.Cells.Add(c = new TableCell());
		c.Width = 10;

		r.Cells.Add(c = new TableCell());
		c.Text = g.Company.Initials;
		c.Font.Bold = true;
		c.HorizontalAlign = HorizontalAlign.Center;

		r.Cells.Add(c = new TableCell());
		c.Text = "Cust";
		c.Font.Bold = true;
		c.HorizontalAlign = HorizontalAlign.Center;

        //r.Cells.Add(c = new TableCell());
        //c.Width = 10;

        //r.Cells.Add(c = new TableCell());
        //c.Text = "Note";
        //c.Font.Bold = true;

		if (tbl == tblShowServices2)
		{
			imgShowServices.Width = 20;
		}
	}

	private void RemoveLines()
	{
		while (tblServices.Rows.Count > 2)
		{
			tblServices.Rows.Remove(tblServices.Rows[2]);
		}
		while (tblShowServices1.Rows.Count > 0)
		{
			tblShowServices1.Rows.Remove(tblShowServices1.Rows[0]);
		}
		while (tblShowServices2.Rows.Count > 0)
		{
			tblShowServices2.Rows.Remove(tblShowServices2.Rows[0]);
		}
	}

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		RemoveServices();
		SaveServices();

		String Sql = "";

		try
		{
			String ServiceItem = lstList.SelectedItem.Value;

			Int32 ServiceSeq = db.NextSeq("mcJobServices", "JobRno", JobRno, "ServiceSeq");

			Sql =
				"Insert Into mcJobServices (JobRno, ServiceSeq, ServiceItem, MCCRespFlg, " +
				"CreatedDtTm, CreatedUser) Values (" +
				JobRno + ", " +
				ServiceSeq + ", " +
				DB.PutStr(ServiceItem, 50) + ", " +
				"1, " +
				DB.PutDtTm(DateTime.Now) + ", " +
				DB.PutStr(g.User) + ")";
			db.Exec(Sql);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		LoadList();
	}

	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		Update();

		if (JobRno > 0)
		{
			String Sql = "";
			try
			{
				Sql = "Select Count(*) From mcJobServices Where JobRno = " + JobRno;
				cServices = db.SqlNum(Sql);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		else
		{
			cServices = 0;
		}
	}

	private void Update()
	{
		RemoveServices();
		SaveServices();
		LoadList();
	}
}