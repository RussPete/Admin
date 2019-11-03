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

public partial class Dishes : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected Int32 JobRno;
	protected int cDishes;
	protected int cShowDishes;
	protected int cShowDishes1;
	protected int cShowDishes2;
	protected const int cnMinRowsFor2Cols = 10;
	protected const int cnMaxNoteLength = 20;

	private const string cnPaperGlass = "Paper/Glass";
	private const string cnPaper = "Paper";
	private const string cnGlass = "Glass";

    private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

		cDishes = Str.Num(txtcDishes.Text);
		cShowDishes = Str.Num(txtcShowDishes.Text);
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
		LoadDishes();

		txtcDishes.Text = Convert.ToString(cDishes);
		txtcShowDishes.Text = Convert.ToString(cShowDishes);

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
                //if (cDishes > 1)
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
			FocusField = "chkInclude1";
		}
	}

	private void Setup()
	{
		LoadList();

		lstList.Attributes.Add("onchange", "ClearDirty();");
		btnUpdate.Attributes.Add("onClick", "ClearDirty();");
		btnShowUpdate.Attributes.Add("onClick", "ClearDirty();");
	}

	private void RemoveDishes()
	{
		if (pnlEdit.Visible && cDishes > 0) { RemoveEditDishes(); }
		if (pnlShow.Visible && cShowDishes > 0) { RemoveShowDishes(); }
	}

	private void RemoveEditDishes()
	{
		for (int iDish = 1; iDish <= cDishes; iDish++)
		{
			TableRow tr = tblDishes.Rows[iDish];

			CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iDish);
			if (chkRemove != null && chkRemove.Checked)
			{
				String Sql = "";
				try
				{
					Int32 DishSeq = Str.Num(WebPage.FindTextBox(ref tr, "txtDishSeq" + iDish));
					Sql = "Delete From mcJobDishes Where JobRno = " + JobRno + " And DishSeq = " + DishSeq;
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

	private void RemoveShowDishes()
	{
		int iDish = 0;
		RemoveShowDishesTable(tblShowDishes1, cShowDishes1, ref iDish);
		RemoveShowDishesTable(tblShowDishes2, cShowDishes2, ref iDish);
	}

	private void RemoveShowDishesTable(Table tblShowDishes, int cShowDishes, ref int iDish)
	{
		for (int iRow = 1; iRow <= cShowDishes; iRow++)
		{
			TableRow tr = tblShowDishes.Rows[iRow];
			iDish++;

			CheckBox chkInclude = (CheckBox)tr.FindControl("chkInclude" + iDish);
			if (chkInclude != null && !chkInclude.Checked)
			{
				String Sql = "";
				try
				{
					Int32 DishSeq = Str.Num(FindLabel(ref tr, "lblDishSeq" + iDish));
					if (DishSeq > 0)
					{
						Sql = "Delete From mcJobDishes Where JobRno = " + JobRno + " And DishSeq = " + DishSeq;
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

	private void SaveDishes()
	{
		if (pnlEdit.Visible && cDishes > 0) { SaveEditDishes(); }
		if (pnlShow.Visible && cShowDishes > 0) { SaveShowDishes(); }
	}

	private void SaveEditDishes()
	{
		String Sql = "";

		for (int iDish = 1; iDish <= cDishes; iDish++)
		{
			TableRow tr = tblDishes.Rows[iDish];

			CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iDish);
			if (chkRemove != null && !chkRemove.Checked)
			{
				Int32 DishSeq = Str.Num(WebPage.FindTextBox(ref tr, "txtDishSeq" + iDish));

				String DishItem = WebPage.FindTextBox(ref tr, "txtDishItem" + iDish);
				String OrigDishItem = WebPage.FindTextBox(ref tr, "txtOrigDishItem" + iDish);

				int Qty = Str.Num(WebPage.FindTextBox(ref tr, "txtQty" + iDish));
				int OrigQty = Str.Num(WebPage.FindTextBox(ref tr, "txtOrigQty" + iDish));

				String Note = WebPage.FindTextBox(ref tr, "txtNote" + iDish);
				String OrigNote = WebPage.FindTextBox(ref tr, "txtOrigNote" + iDish);

				//if Paper/Glass
				if (!chkRemove.Visible)
				{
					//this is the Paper/Glass
					bool fPaper = (Request.Form["rbEditPaperGlass"] == "rbEditPaper");

					DishItem = cnPaperGlass;
					Qty = 0;
					Note = (fPaper ? cnPaper : cnGlass);
				}

				if (DishItem != OrigDishItem ||
					Qty != OrigQty ||
					Note != OrigNote)
				{
					DateTime Tm = DateTime.Now;

					try
					{
						if (DishSeq == 0)
						{
							DishSeq = db.NextSeq("mcJobDishes", "JobRno", JobRno, "DishSeq");

							Sql =
								"Insert Into mcJobDishes (JobRno, DishSeq, CreatedDtTm, CreatedUser) Values (" +
								JobRno + ", " +
								DishSeq + ", " +
								DB.PutDtTm(Tm) + ", " +
								DB.PutStr(g.User) + ")";
							db.Exec(Sql);
						}

						Sql =
							"Update mcJobDishes Set " +
							"DishItem = " + DB.PutStr(DishItem, 50) + ", " +
							"Qty = " + Qty + ", " +
							"Note = " + DB.PutStr(Note, 128) + ", " +
							"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
							"UpdatedUser = " + DB.PutStr(g.User) + " " +
							"Where JobRno = " + JobRno + " " +
							"And DishSeq = " + DishSeq;
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

	private void SaveShowDishes()
	{
		int iDish = 0;
		SaveShowDishesTable(tblShowDishes1, cShowDishes1, ref iDish);
		SaveShowDishesTable(tblShowDishes2, cShowDishes2, ref iDish);
	}

	private void SaveShowDishesTable(Table tblShowDishes, int cShowDishes, ref int iDish)
	{
		String Sql = "";

		for (int iRow = 1; iRow <= cShowDishes; iRow++)
		{
			TableRow tr = tblShowDishes.Rows[iRow];
			iDish++;

			CheckBox chkInclude = (CheckBox)tr.FindControl("chkInclude" + iDish);
			if (chkInclude != null)
			{
				if (!chkInclude.Visible)
				{
					//this is the Paper/Glass
					bool fPaper = (Request.Form["rbShowPaperGlass"] == "rbShowPaper");

					Int32 DishSeq = Str.Num(FindLabel(ref tr, "lblDishSeq" + iDish));
					DateTime Tm = DateTime.Now;

					try
					{
						if (DishSeq == 0)
						{
							DishSeq = db.NextSeq("mcJobDishes", "JobRno", JobRno, "DishSeq");

							Sql =
								"Insert Into mcJobDishes (JobRno, DishSeq, DishItem, CreatedDtTm, CreatedUser) Values (" +
								JobRno + ", " +
								DishSeq + ", " +
								DB.PutStr(cnPaperGlass, 50) + ", " +
								DB.PutDtTm(Tm) + ", " +
								DB.PutStr(g.User) + ")";
							db.Exec(Sql);
						}

						Sql =
							"Update mcJobDishes Set " +
							"Note = " + DB.PutStr(fPaper ? cnPaper : cnGlass) + ", " +
							"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
							"UpdatedUser = " + DB.PutStr(g.User) + " " +
							"Where JobRno = " + JobRno + " " + 
							"And DishSeq = " + DishSeq;
						db.Exec(Sql);
					}
					catch (Exception Ex)
					{
						Err Err = new Err(Ex, Sql);
						Response.Write(Err.Html());
					}
				}
				else
				{
					//regular Dish
					if (chkInclude.Checked)
					{
						Int32 DishSeq = Str.Num(FindLabel(ref tr, "lblDishSeq" + iDish));

						if (DishSeq == 0)
						{
							String DishItem = FindLabel(ref tr, "lblDishItem" + iDish);
							DateTime Tm = DateTime.Now;

							try
							{
								DishSeq = db.NextSeq("mcJobDishes", "JobRno", JobRno, "DishSeq");

								Sql =
									"Insert Into mcJobDishes (JobRno, DishSeq, DishItem, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) Values (" +
									JobRno + ", " +
									DishSeq + ", " +
									DB.PutStr(DishItem, 50) + ", " +
									DB.PutDtTm(Tm) + ", " +
									DB.PutStr(g.User) + "," +
									DB.PutDtTm(Tm) + ", " +
									DB.PutStr(g.User) + ")";
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

	private void LoadDishes()
	{
		LoadEditDishes();
		LoadShowDishes();
	}

	private void LoadEditDishes()
	{
		if (JobRno > 0)
		{
			String Sql = "";

			try
			{
				Sql =
					"Select f1.DishSeq " +
					"From mcJobDishes f1 Inner Join mcJobDishes f2 " +
					"On f1.JobRno = f2.JobRno And f1.DishSeq <> f2.DishSeq " +
					"And f1.DishItem = f2.DishItem " +
					"Where f1.JobRno = " + JobRno;
				DupSeq Dups = new DupSeq(db, Sql);

				Sql = "Select * From mcJobDishes Where JobRno = " + JobRno + " Order By DishSeq";
				DataTable dt = db.DataTable(Sql);
				cDishes = dt.Rows.Count + 1;
				AddEditLines();

				int iRow = 0;
				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					TableRow tr = tblDishes.Rows[iRow];

					string Item = DB.Str(dr["DishItem"]);
					int Qty = DB.Int32(dr["DishSeq"]);
					string Note = DB.Str(dr["Note"]);
					bool fPaperGlass = (Item == cnPaperGlass);

					HtmlInputHidden txtDishSeq = (HtmlInputHidden)tr.FindControl("txtDishSeq" + iRow);
					txtDishSeq.Value = DB.Str(dr["DishSeq"]);

					TextBox txtDishItem = (TextBox)tr.FindControl("txtDishItem" + iRow);
					TextBox txtOrigDishItem = (TextBox)tr.FindControl("txtOrigDishItem" + iRow);
					txtDishItem.Text =
					txtOrigDishItem.Text = DB.Str(dr["DishItem"]);
					txtDishItem.CssClass = (Dups.In(DB.Int32(dr["DishSeq"])) ? "Dup" : "") + "DishItem";

					TextBox txtQty = (TextBox)tr.FindControl("txtQty" + iRow);
					TextBox txtOrigQty = (TextBox)tr.FindControl("txtOrigQty" + iRow);
					txtQty.Text =
					txtOrigQty.Text = Fmt.Num(DB.Int32(dr["Qty"]), false);

					TextBox txtNote = (TextBox)tr.FindControl("txtNote" + iRow);
					TextBox txtOrigNote = (TextBox)tr.FindControl("txtOrigNote" + iRow);
					txtNote.Text =
					txtOrigNote.Text = DB.Str(dr["Note"]);

					if (fPaperGlass)
					{
						CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iRow);
						System.Web.UI.WebControls.Image imgDishItem = (System.Web.UI.WebControls.Image)tr.FindControl("ddDishItem" + iRow);

						bool fPaper = (Note == cnPaper || Note == "");

						chkRemove.Visible = 
						imgDishItem.Visible =
						txtDishItem.Visible =
						txtQty.Visible =
						txtNote.Visible = false;

						RadioButton rbPaper = new RadioButton();
						rbPaper.ID = "rbEditPaper";
						rbPaper.GroupName = "rbEditPaperGlass";
						rbPaper.Text = "Paper&nbsp;&nbsp;";
						rbPaper.Checked = fPaper;

						RadioButton rbGlass = new RadioButton();
						rbGlass.ID = "rbEditGlass";
						rbGlass.GroupName = "rbEditPaperGlass";
						rbGlass.Text = cnGlass;
						rbGlass.Checked = !fPaper;

						TableCell tc = tr.Cells[4];
						tc.Controls.Add(rbPaper);
						tc.Controls.Add(rbGlass);
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
			cDishes = 1;
			AddEditLines();
		}

		FocusField = "txtDishItem" + cDishes;
	}

	private void LoadShowDishes()
	{
		if (JobRno > 0)
		{
			String Sql = string.Format(
				"Select s.DishItem, s.DishSeq As Seq, js.DishSeq, js.Qty, js.Note, Case When js.DishSeq Is Null Then 0 Else 1 End As InJob  " +
				"From mcJobDishes s " +
				"Left Outer Join mcJobDishes js On s.DishItem = js.DishItem " +
				"And js.JobRno = {0} " +
				"Where s.JobRno = 0 " +
				"Union " +
				"Select DishItem, (1000 + DishSeq) As Seq, DishSeq, Qty, Note, 1 As InJob " +
				"From mcJobDishes s " +
				"Where JobRno = {0} " +
				"And DishItem Not In (Select DishItem From mcJobDishes Where JobRno = 0) " +
				"Order By Seq",
				JobRno);
			try
			{
				DataTable dt = db.DataTable(Sql);
				cShowDishes = dt.Rows.Count;
				AddShowLines();

				int iRow = 0;
				int iDish = 0;
				Table tblShowDishes = tblShowDishes1;

				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					iDish++;
					LoadShowDish(tblShowDishes, iRow, iDish, dr);

					if (iDish == cShowDishes1)
					{
						tblShowDishes = tblShowDishes2;
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
			cShowDishes = 1;
			AddShowLines();
		}
	}

	private void LoadShowDish(Table tblShowDishes, int iRow, int iDish, DataRow dr)
	{
		TableRow tr = tblShowDishes.Rows[iRow];

		string Item = DB.Str(dr["DishItem"]);
		string Note = DB.Str(dr["Note"]);
		bool fPaperGlass = (Item == cnPaperGlass);

		CheckBox chkInclude = (CheckBox)tr.FindControl("chkInclude" + iDish);
		chkInclude.Checked = DB.Bool(dr["InJob"]);

		Label lblDishSeq = (Label)tr.FindControl("lblDishSeq" + iDish);
		lblDishSeq.Text = Convert.ToString(DB.Int32(dr["DishSeq"]));

		Label lblDishItem = (Label)tr.FindControl("lblDishItem" + iDish);
		lblDishItem.Text = Item;

        //Label lblQty = (Label)tr.FindControl("lblQty" + iDish);
        //lblQty.Text = Fmt.Num(DB.Int32(dr["Qty"]), false);

        //Label lblNote = (Label)tr.FindControl("lblNote" + iDish);
        //if (Note.Length > cnMaxNoteLength)
        //{
        //    lblNote.Text = Note.Substring(0, cnMaxNoteLength);
        //    lblNote.ToolTip = Note;
        //}
        //else
        //{
        //    lblNote.Text = Note;
        //}

        //if (fPaperGlass)
        //{
        //    bool fPaper = (Note == cnPaper || Note == "");

        //    chkInclude.Visible =
        //    lblDishItem.Visible =
        //    lblNote.Visible = false;

        //    RadioButton rbPaper = new RadioButton();
        //    rbPaper.ID = "rbShowPaper";
        //    rbPaper.GroupName = "rbShowPaperGlass";
        //    rbPaper.Text = "Paper&nbsp;&nbsp;";
        //    rbPaper.Checked = fPaper;

        //    RadioButton rbGlass = new RadioButton();
        //    rbGlass.ID = "rbShowGlass";
        //    rbGlass.GroupName = "rbShowPaperGlass";
        //    rbGlass.Text = cnGlass;
        //    rbGlass.Checked = !fPaper;

        //    TableCell tc = tr.Cells[2];
        //    tc.Controls.Add(rbPaper);
        //    tc.Controls.Add(rbGlass);
        //}
	}

	private void LoadList()
	{
		string Sql = "Select DishItem From mcJobDishes Where JobRno = 0 Order By DishSeq";
		string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");
		lstList.Items.Clear();

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				ListItem Item = new ListItem(DB.Str(r["DishItem"]));
				Item.Selected = (Item.Value == CurrItem);
				lstList.Items.Add(Item);
			}

			lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Dishes";
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
		if (cDishes > 0) { AddEditLines(); }
		if (cShowDishes > 0) { AddShowLines(); }
	}

	private void AddEditLines()
	{
		String Sql;
		SelectList.Clear();

		try
		{
			for (int iDish = 1; iDish <= cDishes; iDish++)
			{
				TableRow r = new TableRow();

				for (int iCell = 0; iCell < 9; iCell++)
				{
					r.Cells.Add(new TableCell());
				}

				r.Cells[0].Text = "<b>" + iDish + "</b>";

				HtmlInputHidden txtDishSeq = new HtmlInputHidden();
				txtDishSeq.ID = "txtDishSeq" + iDish;
				r.Cells[1].Controls.Add(txtDishSeq);

				// Remove
				CheckBox chkRemove = new CheckBox();
				chkRemove.ID = "chkRemove" + iDish;
				chkRemove.Attributes.Add("onClick", "SetDirty();");
				r.Cells[2].Controls.Add(chkRemove);
				r.Cells[2].HorizontalAlign = HorizontalAlign.Center;

				// DishItem
				TextBox txtDishItem = new TextBox();
				txtDishItem.ID = "txtDishItem" + iDish;
				txtDishItem.MaxLength = 50;
				txtDishItem.CssClass = "DishItem";
				txtDishItem.AutoPostBack = true;
				txtDishItem.Attributes.Add("onChange", "HideDirty();");
				txtDishItem.TextChanged += new System.EventHandler(this.btnUpdate_Click);
				r.Cells[4].Controls.Add(txtDishItem);

				System.Web.UI.WebControls.Image imgDishItem = new System.Web.UI.WebControls.Image();
				imgDishItem.ID = "ddDishItem" + iDish;
				imgDishItem.ImageUrl = "Images/DropDown.gif";
				imgDishItem.BorderWidth = 0;
				imgDishItem.ImageAlign = ImageAlign.AbsMiddle;
				r.Cells[4].Controls.Add(imgDishItem);

				Sql = "Select Distinct DishItem From mcJobDishes Order By DishItem";
				SelectList slDishItem = new SelectList("DishItem" + iDish, ref txtDishItem);
				slDishItem.ImageButton(imgDishItem);
				slDishItem.AutoPostBack = true;
				slDishItem.Height = 400;
				slDishItem.ClearValues();
				slDishItem.AddDBValues(db, Sql);

				txtDishItem = new TextBox();
				txtDishItem.ID = "txtOrigDishItem" + iDish;
				txtDishItem.Visible = false;
				r.Cells[4].Controls.Add(txtDishItem);

				// Qty
				TextBox txtQty = new TextBox();
				txtQty.ID = "txtQty" + iDish;
				txtQty.Style.Add("width", "50px");
				txtQty.Attributes.Add("onChange", "SetDirty();");
				r.Cells[6].Controls.Add(txtQty);

				txtQty = new TextBox();
				txtQty.ID = "txtOrigQty" + iDish;
				txtQty.Visible = false;
				r.Cells[6].Controls.Add(txtQty);

				// Note
				TextBox txtNote = new TextBox();
				txtNote.ID = "txtNote" + iDish;
				txtNote.MaxLength = 128;
				txtNote.Style.Add("width", "150px");
				if (iDish == cDishes)
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
				r.Cells[8].Controls.Add(txtNote);

				txtNote = new TextBox();
				txtNote.ID = "txtOrigNote" + iDish;
				txtNote.Visible = false;
				r.Cells[8].Controls.Add(txtNote);

				tblDishes.Rows.Add(r);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}
	}

	private void AddShowLines()
	{
		int iDish = 0;

		if (cShowDishes < 2 * cnMinRowsFor2Cols)
		{
			cShowDishes1 = cShowDishes;
			cShowDishes2 = 0;

			AddShowTable(tblShowDishes1, tdShowDishes1, cShowDishes1, ref iDish);
		}
		else
		{
			cShowDishes1 = (cShowDishes + 1) / 2;
			cShowDishes2 = cShowDishes - cShowDishes1;

			AddShowTable(tblShowDishes1, tdShowDishes1, cShowDishes1, ref iDish);
			AddShowTable(tblShowDishes2, tdShowDishes2, cShowDishes2, ref iDish);
		}
	}

	private void AddShowTable(Table tblShowDishes, TableCell tdShowDishes, int cShowDishes, ref int iDish)
	{
		AddShowHeader(tblShowDishes, tdShowDishes);

		for (int iRow = 1; iRow <= cShowDishes; iRow++)
		{
			TableRow r = new TableRow();
			iDish++;

			//for (int iCell = 0; iCell < 7; iCell++)
            for (int iCell = 0; iCell < 3; iCell++)
            {
				r.Cells.Add(new TableCell());
			}

			// Include
			CheckBox chkInclude = new CheckBox();
			chkInclude.ID = "chkInclude" + iDish;
			chkInclude.Attributes.Add("onClick", "SetDirty();");
			r.Cells[0].Controls.Add(chkInclude);
			r.Cells[0].HorizontalAlign = HorizontalAlign.Center;

			Label lblDishSeq = new Label();
			lblDishSeq.ID = "lblDishSeq" + iDish;
			lblDishSeq.Visible = false;
			r.Cells[1].Controls.Add(lblDishSeq);

			// DishItem
			Label lblDishItem = new Label();
			lblDishItem.ID = "lblDishItem" + iDish;
			r.Cells[2].Controls.Add(lblDishItem);

            //// Qty
            //Label lblQty = new Label();
            //lblQty.ID = "lblQty" + iDish;
            //r.Cells[4].Controls.Add(lblQty);
            //r.Cells[4].HorizontalAlign = HorizontalAlign.Right;

            //// Note
            //Label lblNote = new Label();
            //lblNote.ID = "lblNote" + iDish;
            //r.Cells[6].Controls.Add(lblNote);
            //r.Cells[6].Wrap = false;

			tblShowDishes.Rows.Add(r);
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
		c.Text = "Dish Item";
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

		if (tbl == tblShowDishes2)
		{
			imgShowDishes.Width = 20;
		}
	}

	private void RemoveLines()
	{
		while (tblDishes.Rows.Count > 1)
		{
			tblDishes.Rows.Remove(tblDishes.Rows[1]);
		}
		while (tblShowDishes1.Rows.Count > 0)
		{
			tblShowDishes1.Rows.Remove(tblShowDishes1.Rows[0]);
		}
		while (tblShowDishes2.Rows.Count > 0)
		{
			tblShowDishes2.Rows.Remove(tblShowDishes2.Rows[0]);
		}
	}

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		RemoveDishes();
		SaveDishes();

		String Sql = "";

		try
		{
			String DishItem = lstList.SelectedItem.Value;

			Int32 DishSeq = db.NextSeq("mcJobDishes", "JobRno", JobRno, "DishSeq");

			Sql =
				"Insert Into mcJobDishes (JobRno, DishSeq, DishItem, CreatedDtTm, CreatedUser) Values (" +
				JobRno + ", " +
				DishSeq + ", " +
				DB.PutStr(DishItem, 50) + ", " +
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
				Sql = "Select Count(*) From mcJobDishes Where JobRno = " + JobRno;
				cDishes = db.SqlNum(Sql);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		else
		{
			cDishes = 0;
		}
	}

	private void Update()
	{
		RemoveDishes();
		SaveDishes();
		LoadList();
	}
}