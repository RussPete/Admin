using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Utils;
using Globals;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class ProposalMenu : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected Int32 JobRno;
	private const string NewTitle = "Menu";
    private const string NewItem = "Item";
    protected string EmailType;
	protected string EmailAttachment;
	protected string ErrMsg = string.Empty;

    protected string NewTitleHtml = string.Empty;
    protected string NewItemHtml = string.Empty;

    private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

		//pnlPreview.Visible = false;

		// Put user code to initialize the page here
		if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Jobs.Title;
        }
        else
		{
			if (hfEmail.Value == "true")
			{
				SaveAttachment();
				SendEmail();
			}
		}
	}

	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		SaveUpdates();
	}

	protected void btnSavePreview_Click(object sender, EventArgs e)
	{
		SaveUpdates();
        //pnlPreview.Visible = true;
        pnlPreview.CssClass = "PdfPreview ShowPreview";
		frmPreview.Attributes.Add("src", Page.ResolveUrl(string.Format("{0}?Rno={1}", @"~\ProposalPreview.aspx", JobRno)));        
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
		Misc.DefaultProposalMenu(JobRno);
		string Sql = string.Format(
			"Select * " +
			"From mcJobFood " +
			//"Where JobRno = {0} And (MenuItem <> '' Or MenuItem Is Null) " +
            "Where JobRno = {0} " +
            "Order By IsNull(ProposalSeq, 0), IsNull(FoodSeq, 0)", 
			JobRno);
		try
		{
			DB db = new DB();
			Misc.DefaultInvoicePrice(db, JobRno);

			DataTable dt = db.DataTable(Sql, 300);
			int cLine = 0;

			if (FirstTime(dt))
			{
				//ulMenu.Controls.Add(EditLine(cLine++, 0, true, NewTitle));
			}

			foreach (DataRow dr in dt.Rows)
			{
                string MenuItem = DB.Str(dr["MenuItem"]);        
                string ProposalMenuItem = DB.Str(dr["ProposalMenuItem"]);
				bool fTitleLine = DB.Bool(dr["ProposalTitleFlg"]);
                ulMenu.Controls.Add(EditLine(cLine++, DB.Int32(dr["JobFoodRno"]), fTitleLine, (ProposalMenuItem.Length > 0 ? ProposalMenuItem : MenuItem), DB.Str(dr["Category"]), DB.Str(dr["MenuItem"]), DB.Bool(dr["ProposalHideFlg"])));
			}
			hfNumLines.Value = cLine.ToString();

			NewTitleHtml = Misc.ToString(EditLine("xxxx", 0, true, NewTitle));
            NewItemHtml = Misc.ToString(EditLine("xxxx", 0, false, NewItem));

			{
				Sql = "Select j.ProposalFlg, j.PropCreatedDtTm, j.PropEmailedDtTm, j.ConfirmEmailedDtTm, j.JobType, c.Email, " +
                    "j.PropCreatedUser, j.PropUpdatedDtTm, j.PropUpdatedUser, j.PropGeneratedDtTm, j.PropGeneratedUser, " +
                    "j.PropEmailedUser, j.ConfirmEmailedUser " +
                    "From mcJobs j " +
                    "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                    "Where JobRno = " + JobRno;
				DataRow dr = db.DataRow(Sql);
				if (dr != null)
				{
					bool fProposal = DB.Bool(dr["ProposalFlg"]);
					DateTime dtPropCreated = DB.DtTm(dr["PropCreatedDtTm"]);
					DateTime dtPropEmailed = DB.DtTm(dr["PropEmailedDtTm"]);
					DateTime dtConfirmEmailed = DB.DtTm(dr["ConfirmEmailedDtTm"]);
                    string JobType = DB.Str(dr["JobType"]);

                    hfJobRno.Value = JobRno.ToString();
                    EmailType = (fProposal ? "Proposal" : (JobType == "Private" ? "Private Confirmation" : "Corporate Confirmation"));
					txtEmail.Text = DB.Str(dr["Email"]);
					txtSubject.Text = (fProposal ? (dtPropEmailed == DateTime.MinValue ? string.Empty : "Revised ") + "Proposal" : (dtConfirmEmailed == DateTime.MinValue ? string.Empty : "Revised ") + "Confirmation") + " from " + Globals.g.Company.Name;
					txtMessage.Text = (fProposal ? "Attached is your " + (dtPropEmailed == DateTime.MinValue ? string.Empty : "revised ") + "proposal from " + Globals.g.Company.Name + ".\n\nWe appreciate your business." : (dtConfirmEmailed == DateTime.MinValue ? string.Empty : "Attached is your revised confirmation from " + Globals.g.Company.Name + ".\n\nWe appreciate your business."));
                    chkTermsOfService.Checked = (JobType == "Private");

                    btnEmail.Visible = fProposal;
					btnEmailConfirm.Visible = !fProposal;

					if (dtPropCreated == DateTime.MinValue)
					{
						btnEmail.Attributes.Add("disabled", "disabled");
						btnEmail.Attributes.Add("title", "Save Client Pricing and then email.");
						btnEmailConfirm.Attributes.Add("disabled", "disabled");
						btnEmailConfirm.Attributes.Add("title", "Save Client Pricing and then email.");
					}
					if (dtPropEmailed != DateTime.MinValue)
					{
						btnEmail.Value = "Re-Email Proposal";
					}
					if (dtConfirmEmailed != DateTime.MinValue)
					{
						btnEmailConfirm.Value = "Re-Email Confirm";
					}

					txtCreatedDt.Text = Fmt.DtTm(dtPropCreated);
					txtCreatedUser.Text = DB.Str(dr["PropCreatedUser"]);
					txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["PropUpdatedDtTm"]));
					txtUpdatedUser.Text = DB.Str(dr["PropUpdatedUser"]);
					txtGeneratedDt.Text = Fmt.DtTm(DB.DtTm(dr["PropGeneratedDtTm"]));
					txtGeneratedUser.Text = DB.Str(dr["PropGeneratedUser"]);
					txtPropEmailedDt.Text = Fmt.DtTm(dtPropEmailed);
					txtPropEmailedUser.Text = DB.Str(dr["PropEmailedUser"]);
					txtConfirmEmailedDt.Text = Fmt.DtTm(dtConfirmEmailed);
					txtConfirmEmailedUser.Text = DB.Str(dr["ConfirmEmailedUser"]);
					pnlPreview.ToolTip = string.Format("{0} Preview", (fProposal ? "Proposal" : "Confirmation"));
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private HtmlGenericControl EditLine(int ID, int JobFoodRno, bool fGroupTitle, string ProposalMenuItem)
	{
		return EditLine(ID.ToString(), JobFoodRno, fGroupTitle, ProposalMenuItem, string.Empty, string.Empty, false);
	}

	private HtmlGenericControl EditLine(int ID, int JobFoodRno, bool fGroupTitle, string ProposalMenuItem, string Category, string MenuItem, bool fHide)
	{
		return EditLine(ID.ToString(), JobFoodRno, fGroupTitle, ProposalMenuItem,Category, MenuItem, fHide);
	}

	private HtmlGenericControl EditLine(string ID, int JobFoodRno, bool fGroupTitle, string ProposalMenuItem)
	{
		return EditLine(ID, JobFoodRno, fGroupTitle, ProposalMenuItem, string.Empty, string.Empty, false);
	}

	private HtmlGenericControl EditLine(string ID, int JobFoodRno, bool fGroupTitle, string ProposalMenuItem, string Category, string MenuItem, bool fHide)
	{
		HtmlGenericControl li = new HtmlGenericControl("li");
		if (fHide) li.Attributes.Add("class", "NotShow");

		HtmlInputHidden hf = new HtmlInputHidden();
		hf.ID = string.Format("hfJobFoodRno{0}", ID);
		hf.Value = JobFoodRno.ToString();
		li.Controls.Add(hf);

		hf = new HtmlInputHidden();
		hf.ID = string.Format("hfSortOrder{0}", ID);
		hf.Value = ID;
		hf.Attributes.Add("class", "SortOrder");
		li.Controls.Add(hf);

		hf = new HtmlInputHidden();
		hf.ID = string.Format("hfTitle{0}", ID);
		hf.Value = fGroupTitle.ToString();
		li.Controls.Add(hf);

		HtmlGenericControl sp = new HtmlGenericControl("span");
		sp.Attributes.Add("class", "ui-icon ui-icon-arrowthick-2-n-s Inline");
		li.Controls.Add(sp);

		if (fGroupTitle)
		{
			// group title
			HtmlGenericControl lbl = new HtmlGenericControl("label");
			lbl.InnerText = ProposalMenuItem;
			lbl.Attributes.Add("class", "Title Name");
			li.Controls.Add(lbl);

			HtmlInputText txt = new HtmlInputText();
			txt.ID = string.Format("txtProposal{0}", ID);
			txt.Value = ProposalMenuItem;
			txt.Attributes.Add("class", "Title Name");
			txt.Style.Add("display", "none");
			li.Controls.Add(txt);

			//lbl = new HtmlGenericControl("label");
			//string DeleteID = string.Format("chkDelete{0}", ID);
			//lbl.InnerText = "Delete ";
			//lbl.Attributes.Add("for", DeleteID);
			//lbl.Attributes.Add("class", "Delete");
			//li.Controls.Add(lbl);

			//HtmlInputCheckBox chk = new HtmlInputCheckBox();
			//chk.ID = DeleteID;
			//chk.Attributes.Add("class", "Delete");
			//chk.Value = "1";
			//li.Controls.Add(chk);
		}
		else
		{
			// menu item
			//sp = new HtmlGenericControl("span");
			//sp.Attributes.Add("class", "Space");
			//li.Controls.Add(sp);

			HtmlGenericControl lbl = new HtmlGenericControl("label");
			lbl.InnerText = ProposalMenuItem;
			lbl.Attributes.Add("title", string.Format("{0} - {1}", Category, MenuItem));
			lbl.Attributes.Add("class", "Name");
			li.Controls.Add(lbl);

			HtmlInputText txt = new HtmlInputText();
			txt.ID = string.Format("txtProposal{0}", ID);
			txt.Value = ProposalMenuItem;
			txt.Attributes.Add("class", "Name");
			txt.Style.Add("display", "none");
			li.Controls.Add(txt);
		}

		hf = new HtmlInputHidden();
		hf.ID = string.Format("hfHide{0}", ID);
		hf.Value = fHide.ToString();
		li.Controls.Add(hf);

		HtmlGenericControl img = new HtmlGenericControl("img");
		img.Attributes.Add("src", "Images\\Add16.png");
		img.Attributes.Add("class", "Show");
		img.Attributes.Add("title", (fGroupTitle ? "Keep this group title on the proposal" : "Show this menu item on the proposal"));
		li.Controls.Add(img);

		img = new HtmlGenericControl("img");
		img.Attributes.Add("src", "Images\\Delete16.png");
		img.Attributes.Add("class", "NotShow");
		img.Attributes.Add("title", (fGroupTitle ? "Remove this group title from the proposal" : "Hide this menu item from the proposal"));
		li.Controls.Add(img);

		return li;
	}

	protected bool FirstTime(DataTable dt)
	{
		bool fFirstTime = true;

		foreach (DataRow dr in dt.Rows)
		{
			if (DB.Int32(dr["ProposalSeq"]) > 0)
			{
				fFirstTime = false;
				break;
			}
		}

		return fFirstTime;
	}

	protected void SaveUpdates()
	{
		string Sql = string.Empty;

		try
		{
			DB db = new DB();

			int cLines = Str.Num(hfNumLines.Value);
			for (int i = 0; i < cLines; i++)
			{
				int JobFoodRno = Parm.Int(string.Format("hfJobFoodRno{0}", i));
				int SortOrder = Parm.Int(string.Format("hfSortOrder{0}", i));
				bool fProposalTitle = Parm.Bool(string.Format("hfTitle{0}", i));
				string Proposal = Parm.Str(string.Format("txtProposal{0}", i));
				//bool fDelete = Parm.Bool(string.Format("chkDelete{0}", i));
				bool fHide = Parm.Bool(string.Format("hfHide{0}", i));
				DateTime Tm = DateTime.Now;

				// if deleting a title
				if (fHide && Proposal.Length == 0)
				{
					Sql = string.Format("Delete From mcJobFood Where JobFoodRno = {0}", JobFoodRno);
					db.Exec(Sql);
				}
				else
				{
					// if a new title or item
					if (JobFoodRno == 0)
					{
						// add to the menu
						Sql = string.Format(
							"Insert Into mcJobFood (JobRno, CreatedDtTm, CreatedUser) Values ({0}, {1}, {2}); Select @@Identity",
							JobRno,
							DB.PutDtTm(Tm),
							DB.PutStr(g.User));
						JobFoodRno = db.SqlNum(Sql);
					}

                    Sql = string.Format("Select MenuItemRno From mcJobFood Where JobFoodRno = {0}", JobFoodRno);
                    int MenuItemRno = db.SqlNum(Sql);

					// update group title or menu item
					Sql = string.Format(
                        "Update mcJobFood Set ProposalSeq = {1}, ProposalMenuItem = {2}, ProposalTitleFlg = {3}, ProposalHideFlg = {4} Where JobFoodRno = {0}",
						JobFoodRno,
						SortOrder,
						DB.PutStr(Proposal, 1000),
                        DB.PutBool(fProposalTitle),
						DB.PutBool(fHide));
					db.Exec(Sql);
				}
			}
			db.Close();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void SendEmail()
	{
		Misc.SendProposalEmail(Page, db, JobRno, txtEmail.Text, txtSubject.Text, txtMessage.Text, chkTermsOfService.Checked, EmailAttachment);
	}

	protected void SaveAttachment()
	{
		EmailAttachment = string.Empty;

		if (fuAttachment.HasFile)
		{
			WebConfig Cfg = new WebConfig();
			string AttachmentDir = Cfg.Str("Attachment Dir");

			if (AttachmentDir.Length == 0)
			{
				ErrMsg += "The 'Attachment Dir' value '{0}' is missing in the Web.config file.<br />";
			}
			else if (!Directory.Exists(AttachmentDir))
			{
				ErrMsg += string.Format("The 'Attachment Dir' value '{0}' in the Web.config file points to an invalid directory.<br />", AttachmentDir);
			}
			else
			{
				try
				{
					string Filename = string.Format(@"{0}\{1} - {2}", AttachmentDir, JobRno, fuAttachment.FileName);
					if (File.Exists(Filename))
					{
						File.Delete(Filename);
					}
					fuAttachment.SaveAs(Filename);

					EmailAttachment = Filename;
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex);
					Response.Write(Err.Html());
				}
			}
		}
	}
}
