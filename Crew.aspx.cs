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

public partial class Crew : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected Int32 JobRno;
	protected int cCrew;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

		cCrew = Str.Num(txtcCrew.Text);
		AddLines();

		// Put user code to initialize the page here
		if (!Page.IsPostBack)
		{
			Setup();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
		RemoveLines();
		LoadCrew();

		txtcCrew.Text = Convert.ToString(cCrew);
	}

	private void Setup()
	{
		LoadList();

		lstList.Attributes.Add("onchange", "ClearDirty();");
		btnUpdate.Attributes.Add("onClick", "ClearDirty();");
	}

	private void RemoveCrew()
	{
		for (int iCrew = 1; iCrew <= cCrew; iCrew++)
		{
			TableRow tr = tblCrew.Rows[iCrew];

			CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iCrew);
			if (chkRemove != null && chkRemove.Checked)
			{
				String Sql = "";
				try
				{
					Int32 CrewSeq = Str.Num(WebPage.FindTextBox(ref tr, "txtCrewSeq" + iCrew));
					Sql = "Delete From mcJobCrew Where JobRno = " + JobRno + " And CrewSeq = " + CrewSeq;
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

	private void SaveCrew()
	{
		String Sql = "";
		String JobDate = "";

		try
		{
			Sql = "Select JobDate From mcJobs Where JobRno = " + JobRno;
			JobDate = Fmt.Dt(db.SqlDtTm(Sql));
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		for (int iCrew = 1; iCrew <= cCrew; iCrew++)
		{
			TableRow tr = tblCrew.Rows[iCrew];

			CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iCrew);
			if (chkRemove != null && !chkRemove.Checked)
			{
				Int32 CrewSeq = Str.Num(WebPage.FindTextBox(ref tr, "txtCrewSeq" + iCrew));

				String CrewMember = WebPage.FindTextBox(ref tr, "txtCrewMember" + iCrew);
				String OrigCrewMember = WebPage.FindTextBox(ref tr, "txtOrigCrewMember" + iCrew);

				String Assignment = WebPage.FindTextBox(ref tr, "txtAssignment" + iCrew);
				String OrigAssignment = WebPage.FindTextBox(ref tr, "txtOrigAssignment" + iCrew);

				String ReportTime = WebPage.FindTextBox(ref tr, "txtReportTime" + iCrew);
				String OrigReportTime = WebPage.FindTextBox(ref tr, "txtOrigReportTime" + iCrew);

				String Note = WebPage.FindTextBox(ref tr, "txtNote" + iCrew);
				String OrigNote = WebPage.FindTextBox(ref tr, "txtOrigNote" + iCrew);

				if (CrewMember != OrigCrewMember ||
					Assignment != OrigAssignment ||
					ReportTime != OrigReportTime ||
					Note != OrigNote)
				{
					DateTime Tm = DateTime.Now;

					try
					{
						if (CrewSeq == 0)
						{
							CrewSeq = db.NextSeq("mcJobCrew", "JobRno", JobRno, "CrewSeq");

							Sql =
								"Insert Into mcJobCrew (JobRno, CrewSeq, CreatedDtTm, CreatedUser) Values (" +
								JobRno + ", " +
								CrewSeq + ", " +
								DB.PutDtTm(Tm) + ", " +
								DB.PutStr(g.User) + ")";
							db.Exec(Sql);
						}

						Sql =
							"Update mcJobCrew Set " +
							"CrewMember = " + DB.PutStr(CrewMember, 50) + ", " +
							"CrewAssignment = " + DB.PutStr(Assignment, 50) + ", " +
							"ReportTime = " + DB.PutDtTm(JobDate, ReportTime) + ", " +
							"Note = " + DB.PutStr(Note, 128) + ", " +
							"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
							"UpdatedUser = " + DB.PutStr(g.User) + " " +
							"Where JobRno = " + JobRno + " " +
							"And CrewSeq = " + CrewSeq;
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

	private void LoadCrew()
	{
		if (JobRno > 0)
		{
			String Sql = "";

			try
			{
				Sql =
					"Select f1.CrewSeq " +
					"From mcJobCrew f1 Inner Join mcJobCrew f2 " +
					"On f1.JobRno = f2.JobRno And f1.CrewSeq <> f2.CrewSeq " +
					"And f1.CrewMember = f2.CrewMember " +
					"Where f1.JobRno = " + JobRno;
				DupSeq Dups = new DupSeq(db, Sql);

				Sql = "Select * From mcJobCrew Where JobRno = " + JobRno + " Order By CrewSeq";
				DataTable dt = db.DataTable(Sql);
				cCrew = dt.Rows.Count + 1;
				AddLines();

				int iRow = 0;
				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					TableRow tr = tblCrew.Rows[iRow];

					HtmlInputHidden txtCrewSeq = (HtmlInputHidden)tr.FindControl("txtCrewSeq" + iRow);
					txtCrewSeq.Value = DB.Str(dr["CrewSeq"]);

					TextBox txtCrewMember = (TextBox)tr.FindControl("txtCrewMember" + iRow);
					TextBox txtOrigCrewMember = (TextBox)tr.FindControl("txtOrigCrewMember" + iRow);
					txtCrewMember.Text =
					txtOrigCrewMember.Text = DB.Str(dr["CrewMember"]);
					txtCrewMember.CssClass = (Dups.In(DB.Int32(dr["CrewSeq"])) ? "Dup" : "") + "CrewMember";

					TextBox txtAssignment = (TextBox)tr.FindControl("txtAssignment" + iRow);
					TextBox txtOrigAssignment = (TextBox)tr.FindControl("txtOrigAssignment" + iRow);
					txtAssignment.Text =
					txtOrigAssignment.Text = DB.Str(dr["CrewAssignment"]);

					TextBox txtReportTime = (TextBox)tr.FindControl("txtReportTime" + iRow);
					TextBox txtOrigReportTime = (TextBox)tr.FindControl("txtOrigReportTime" + iRow);
					txtReportTime.Text =
					txtOrigReportTime.Text = Fmt.Tm12Hr(DB.DtTm(dr["ReportTime"]));

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
			cCrew = 1;
			AddLines();
		}

		if (FocusField.Length == 0)
		{
			FocusField = "txtCrewMember" + cCrew;
		}
		else
		{
			FocusField += cCrew - 1;
		}
	}

	private void LoadList()
	{
		string Sql =
			"Select Distinct CrewMember From mcJobCrew " +
			"Where CrewMember Is Not Null Order By CrewMember";
		string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");
		lstList.Items.Clear();

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				ListItem Item = new ListItem(DB.Str(r["CrewMember"]));
				Item.Selected = (Item.Value == CurrItem);
				lstList.Items.Add(Item);
			}

			lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Crew Members";
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}
	}

	protected void UpdateList(object sender, System.EventArgs e)
	{
		LoadList();
	}

	private void AddLines()
	{
		String Sql;
		SelectList.Clear();

		for (int iCrew = 1; iCrew <= cCrew; iCrew++)
		{
			TableRow r = new TableRow();

			for (int iCell = 0; iCell < 11; iCell++)
			{
				r.Cells.Add(new TableCell());
			}

			r.Cells[0].Text = "<b>" + iCrew + "</b>";

			HtmlInputHidden txtCrewSeq = new HtmlInputHidden();
			txtCrewSeq.ID = "txtCrewSeq" + iCrew;
			r.Cells[1].Controls.Add(txtCrewSeq);

			// Remove
			CheckBox chkRemove = new CheckBox();
			chkRemove.ID = "chkRemove" + iCrew;
			chkRemove.Attributes.Add("onClick", "SetDirty();");
			r.Cells[2].Controls.Add(chkRemove);
			r.Cells[2].HorizontalAlign = HorizontalAlign.Center;

			// CrewMember
			TextBox txtCrewMember = new TextBox();
			txtCrewMember.ID = "txtCrewMember" + iCrew;
			txtCrewMember.MaxLength = 50;
			txtCrewMember.CssClass = "CrewMember";
			txtCrewMember.Attributes.Add("onChange", "SetDirty();");
			r.Cells[4].Controls.Add(txtCrewMember);

			System.Web.UI.WebControls.Image imgCrewMember = new System.Web.UI.WebControls.Image();
			imgCrewMember.ID = "ddCrewMember" + iCrew;
			imgCrewMember.ImageUrl = "Images/DropDown.gif";
			imgCrewMember.BorderWidth = 0;
			imgCrewMember.ImageAlign = ImageAlign.AbsMiddle;
			r.Cells[4].Controls.Add(imgCrewMember);

			Sql = "Select Distinct CrewMember From mcJobCrew Order By CrewMember";
			SelectList slCrewMember = new SelectList("CrewMember" + iCrew, ref txtCrewMember);
			slCrewMember.ImageButton(imgCrewMember);
			slCrewMember.ClearValues();
			slCrewMember.AddDBValues(db, Sql);

			txtCrewMember = new TextBox();
			txtCrewMember.ID = "txtOrigCrewMember" + iCrew;
			txtCrewMember.Visible = false;
			r.Cells[4].Controls.Add(txtCrewMember);

			// Assignment
			TextBox txtAssignment = new TextBox();
			txtAssignment.ID = "txtAssignment" + iCrew;
			txtAssignment.MaxLength = 50;
			txtAssignment.CssClass = "Assignment";
			txtAssignment.Attributes.Add("onChange", "SetDirty();");
			r.Cells[6].Controls.Add(txtAssignment);

			System.Web.UI.WebControls.Image imgAssignment = new System.Web.UI.WebControls.Image();
			imgAssignment.ID = "ddAssignment" + iCrew;
			imgAssignment.ImageUrl = "Images/DropDown.gif";
			imgAssignment.BorderWidth = 0;
			imgAssignment.ImageAlign = ImageAlign.AbsMiddle;
			r.Cells[6].Controls.Add(imgAssignment);

			Sql = "Select Distinct CrewAssignment From mcJobCrew Order By CrewAssignment";
			SelectList slAssignment = new SelectList("Assignment" + iCrew, ref txtAssignment);
			slAssignment.ImageButton(imgAssignment);
			slAssignment.ClearValues();
			slAssignment.AddDBValues(db, Sql);

			txtAssignment = new TextBox();
			txtAssignment.ID = "txtOrigAssignment" + iCrew;
			txtAssignment.Visible = false;
			r.Cells[6].Controls.Add(txtAssignment);

			// Report Time
			TextBox txtReportTime = new TextBox();
			txtReportTime.ID = "txtReportTime" + iCrew;
			txtReportTime.CssClass = "JobTime";
			txtReportTime.Attributes.Add("onChange", "ValidateTime(this);SetDirty();");
			r.Cells[8].Controls.Add(txtReportTime);

			txtReportTime = new TextBox();
			txtReportTime.ID = "txtOrigReportTime" + iCrew;
			txtReportTime.Visible = false;
			r.Cells[8].Controls.Add(txtReportTime);

			// Note
			TextBox txtNote = new TextBox();
			txtNote.ID = "txtNote" + iCrew;
			txtNote.MaxLength = 128;
			txtNote.Style.Add("width", "150px");
			if (iCrew == cCrew)
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
			r.Cells[10].Controls.Add(txtNote);

			txtNote = new TextBox();
			txtNote.ID = "txtOrigNote" + iCrew;
			txtNote.Visible = false;
			r.Cells[10].Controls.Add(txtNote);

			tblCrew.Rows.Add(r);
		}
	}

	private void RemoveLines()
	{
		while (tblCrew.Rows.Count > 1)
		{
			tblCrew.Rows.Remove(tblCrew.Rows[1]);
		}
	}

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		RemoveCrew();
		SaveCrew();

		String Sql = "";

		try
		{
			String CrewMember = lstList.SelectedItem.Value;

			Int32 CrewSeq = db.NextSeq("mcJobCrew", "JobRno", JobRno, "CrewSeq");

			Sql =
				"Insert Into mcJobCrew (JobRno, CrewSeq, CrewMember, CreatedDtTm, CreatedUser) Values (" +
				JobRno + ", " +
				CrewSeq + ", " +
				DB.PutStr(CrewMember, 50) + ", " +
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

		FocusField = "txtAssignment";
	}

	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		Update();

		if (JobRno > 0)
		{
			String Sql = "";
			try
			{
				Sql = "Select Count(*) From mcJobCrew Where JobRno = " + JobRno;
				cCrew = db.SqlNum(Sql);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		else
		{
			cCrew = 0;
		}
	}

	private void Update()
	{
		RemoveCrew();
		SaveCrew();
		LoadList();
	}
}