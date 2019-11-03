using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Globals;
using Utils;
using System.Net.Mail;
using System.Threading;

/// <summary>
/// Summary description for Misc
/// </summary>
public class Misc
{
    public const string cnPerPerson = "Per Person";
    public const string cnPerItem = "Per Item";
    public const string cnAllIncl = "All Inclusive";
    public const string Password = "IM5rxl8bsRchuyposVafWyMyH8/cLHIslwmAfE6twAo=";
    public const string cnSlingUserEmail = "Sling User Email";
    public const string cnSlingUserPassword = "Sling User Password";

    public Misc()
    {
    }

    public static string Parms(Page Page)
    {
        StringBuilder sbParms = new StringBuilder();
        string[] AllKeys = Page.Request.Params.AllKeys;
        foreach (string Key in AllKeys)
        {
            sbParms.AppendFormat("[{0}] : {1}<br />\n", Key, Page.Request.Params[Key]);
        }

        return sbParms.ToString();
    }

    public static string AbsoluteUrl(Page Page, string RelativeUrl)
    {
        HttpRequest Request = HttpContext.Current.Request;
        string Url = string.Format("http{0}://{1}{2}", (Request.IsSecureConnection ? "s" : ""), Request.Url.Authority, Page.ResolveUrl(RelativeUrl));

        return Url;
    }

    public static string jsArray(string[] aStr)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string str in aStr)
        {
            sb.AppendFormat("{0}\"{1}\"", (sb.Length == 0 ? string.Empty : ", "), Str.js(str));
        }

        return string.Format("[{0}]", sb.ToString());
    }

    public static string ToString(HtmlGenericControl ctl)
    {
        string Str = string.Empty;
        using (StringWriter swriter = new StringWriter())
        {
            HtmlTextWriter writer = new HtmlTextWriter(swriter);
            ctl.RenderControl(writer);
            Str = swriter.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        return Str;
    }

    public static void DefaultProposalMenu(int JobRno)
    {
        string Sql = string.Format("Select Count(*) From mcJobFood Where JobRno = {0} And FoodSeq Is Null", JobRno);
        DB db = new DB();
        if (db.SqlNum(Sql) == 0)
        {
            Sql = string.Format("Insert Into mcJobFood (JobRno, ProposalSeq, ProposalMenuItem, ProposalTitleFlg, CreatedDtTm, CreatedUser) Values ({0}, 0, 'Menu:', 1, GetDate(), '{1}')", JobRno, g.User);
            db.Exec(Sql);
        }
        db.Close();
    }

    public static void SetAsIsData(int MenuItemRno, bool fAsIs)
    {
        string Sql = string.Format("Select AsIsFlg From mcJobMenuItems Where MenuItemRno = {0}", MenuItemRno);
        try
        {
            DB db = new DB();
            bool fPrevAsIs = db.SqlBool(Sql);

            // if not currently flagged as an As Is menu item, but will be now
            if (!fPrevAsIs && fAsIs)
            {
                bool fUpdateWithLastPrice = false;

                Sql = string.Format("Select MenuItem From mcJobMenuItems Where MenuItemRno = {0}", MenuItemRno);
                string MenuItem = db.SqlStr(Sql);

                // we need a recipe, ingredient and recipe ingredient xref for this as is menu item

                // find or create a recipe for this as item
                Sql = string.Format(
                    "Select RecipeRno From Recipes Where Name = {0} And MenuItemAsIsFlg = 1",
                    DB.PutStr(MenuItem));
                int RecipeRno = db.SqlNum(Sql);
                bool fNewRecipe = false;
                if (RecipeRno == 0)
                {
                    Sql = string.Format(
                        "Insert Into Recipes (Name, SubRecipeFlg, NumServings, YieldQty, YieldUnitRno, PortionQty, PortionUnitRno, MenuItemAsIsFlg, CreatedDtTm, CreatedUser) " +
                        "Values ({0}, 0, 1, 1, (Select UnitRno From Units Where UnitSingle = 'ea'), 1, (Select UnitRno From Units Where UnitSingle = 'ea'), 1, GetDate(), {1}); " +
                        "Select Scope_Identity()",
                        DB.PutStr(MenuItem),
                        DB.PutStr(g.User));
                    RecipeRno = db.SqlNum(Sql);
                    fNewRecipe = true;
                }

                // find similar menu items in other categories, and set their As Is flag and set their recipe pointer
                Sql = string.Format(
                    "Update mcJobMenuItems Set AsIsFlg = 1, RecipeRno = {1} Where MenuItem In (Select MenuItem From mcJobMenuItems Where MenuItemRno = {0}) And RecipeRno Is Null",
                    MenuItemRno,
                    RecipeRno);
                db.Exec(Sql);

                // find or create an ingredient
                Sql = string.Format(
                    "Select Top 1 IngredRno From Ingredients Where Name In (Select MenuItem From mcJobMenuItems Where MenuItemRno = {0})",
                    MenuItemRno);
                int IngredRno = db.SqlNum(Sql);
                if (IngredRno == 0)  // create ingredient
                {
                    Sql = string.Format(
                        "Insert Into Ingredients (Name, MenuItemAsIsFlg, HideFlg, CreatedDtTm, CreatedUser) " +
                        "Values ((Select MenuItem From mcJobMenuItems Where MenuItemRno = {0}), 1, 1, GetDate(), {1}); " +
                        "Select Scope_Identity()",
                        MenuItemRno,
                        DB.PutStr(g.User));
                    IngredRno = db.SqlNum(Sql);
                }
                else // found ingredient
                {
                    Sql = string.Format(
                        "Update Ingredients Set MenuItemAsIsFlg = 1 Where IngredRno = {0}",
                        IngredRno);
                    db.Exec(Sql);

                    fUpdateWithLastPrice = true;
                }

                // if a new recipe, add the xref
                if (fNewRecipe)
                {
                    Sql = string.Format(
                        "Insert Into RecipeIngredXref (RecipeRno, RecipeSeq, IngredRno, UnitQty, UnitRno, CreatedDtTm, CreatedUser) " +
                        "Values ({0}, 1, {1}, 1, (Select UnitRno From Units Where UnitSingle = 'ea'), GetDate(), {2}); " +
                        "Select Scope_Identity()",
                        RecipeRno,
                        IngredRno,
                        DB.PutStr(g.User));

                    int RecipeIngredRno = db.SqlNum(Sql);
                }

                if (fUpdateWithLastPrice)
                {
                    Ingred.UpdateWithLastPrice(IngredRno);
                }
            }
            // if currently flagged as an As Is menu item, but will not be now
            else if (fPrevAsIs && !fAsIs)
            {
                Sql = string.Format(
                    "Select RecipeRno From mcJobMenuItems Where MenuItemRno = {0}", MenuItemRno);
                int RecipeRno = db.SqlNum(Sql);
                if (RecipeRno > 0)
                {
                    Sql = string.Format(
                        "Delete From Ingredients Where IngredRno in (Select IngredRno From RecipeIngredXref Where RecipeRno = {0}) And MenuItemAsIsFlg = 1 And (Select Count(*) From PurchaseDetails Where IngredRno = Ingredients.IngredRno) = 0; " +
                        "Update Ingredients Set MenuItemAsIsFlg = Null Where IngredRno in (Select IngredRno From RecipeIngredXref Where RecipeRno = {0}); " +
                        "Delete From RecipeIngredXref Where RecipeRno In (Select RecipeRno From Recipes Where RecipeRno = {0} And MenuItemAsIsFlg = 1); " +
                        "Delete From Recipes Where RecipeRno = {0} And MenuItemAsIsFlg = 1; ",
                        RecipeRno);
                    db.Exec(Sql);

                    Sql = string.Format(
                        "Update mcJobMenuItems Set AsIsFlg = 0, RecipeRno = Null, ServingPrice = Null Where RecipeRno = {0} And AsIsFlg = 1",
                        RecipeRno);
                    db.Exec(Sql);
                }
            }
            db.Close();
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            HttpContext.Current.Response.Write(Err.Html());
        }
    }

    public static void DefaultInvoicePrice(DB db, int JobRno)
    {
        string Sql = string.Format("Select Count(*) From JobInvoicePrices Where JobRno = {0}", JobRno);
        try
        {
            int Num = db.SqlNum(Sql);
            if (Num == 0)
            {
                Sql = string.Format("Select * From mcJobs Where JobRno = {0}", JobRno);
                DataRow dr = db.DataRow(Sql);
                if (dr != null)
                {
                    int cMen = DB.Int32(dr["NumMenServing"]);
                    int cWomen = DB.Int32(dr["NumWomenServing"]);
                    int cChild = DB.Int32(dr["NumChildServing"]);
                    int Count = cMen + cWomen + cChild;
                    decimal PricePerPerson = DB.Dec(dr["PricePerPerson"]);
                    decimal ExtPricePerPerson = PricePerPerson * Count;

                    InsertPrice(db, JobRno, Misc.cnPerPerson, Count, "servings", PricePerPerson, ExtPricePerPerson);
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            HttpContext.Current.Response.Write(Err.Html());
        }
    }

    public static void InsertPrice(DB db, int JobRno, string PriceType, int Count, string Desc, decimal Price, decimal ExtPrice)
    {
        string Sql = "";
        DateTime Tm = DateTime.Now;

        try
        {
            int nRno = db.NextRno("JobInvoicePrices", "Rno");
            Sql =
                "Insert Into JobInvoicePrices (Rno, JobRno, Seq, PriceType, " +
                "InvPerPersonCount, InvPerPersonDesc, InvPerPersonPrice, InvPerPersonExtPrice, " +
                "InvPerItemCount, InvPerItemDesc, InvPerItemPrice, InvPerItemExtPrice, " +
                "InvAllInclDesc, InvAllInclPrice, InvAllInclExtPrice, " +
                "CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) Values (" +
                nRno + ", " +
                JobRno + ", " +
                db.NextSeq("JobInvoicePrices", "JobRno", JobRno, "Seq") + ", " +
                DB.PutStr(PriceType, 20) + ", " +
                Count + ", " +
                DB.PutStr(Desc, 50) + ", " +
                Price + ", " +
                ExtPrice + ", " +
                Count + ", " +
                DB.PutStr(Desc, 50) + ", " +
                Price + ", " +
                ExtPrice + ", " +
                DB.PutStr(Desc, 50) + ", " +
                Price + ", " +
                ExtPrice + ", " +
                DB.PutDtTm(Tm) + ", " +
                DB.PutStr(g.User) + ", " +
                DB.PutDtTm(Tm) + ", " +
                DB.PutStr(g.User) + ")";
            db.Exec(Sql);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            HttpContext.Current.Response.Write(Err.Html());
        }
    }

    public static string UnitData(DB db)
    {
        string Sql = "Select UnitRno, UnitSingle, UnitPlural From Units Where IsNull(HideFlg, 0) = 0 Order By UnitPlural";
        StringBuilder sb = new StringBuilder();

        try
        {
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                int Rno = DB.Int32(dr["UnitRno"]);
                string Single = DB.Str(dr["UnitSingle"]).Replace(@"\", @"\\").Replace("'", @"\'");
                string Plural = DB.Str(dr["UnitPlural"]).Replace(@"\", @"\\").Replace("'", @"\'");
                sb.AppendFormat("{{label:'{0}',single:'{0}',plural:'{1}',id:{2}}},", Single, Plural, Rno);
                if (Single != Plural)
                {
                    sb.AppendFormat("{{label:'{1}',single:'{0}',plural:'{1}',id:{2}}},", Single, Plural, Rno);
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex);
            HttpContext.Current.Response.Write(Err.Html());
        }

        return string.Format("[{0}]", sb.ToString().Remove(sb.Length - 1, 1));
    }

    public static string CreateFromEmailAddress(string User)
    {
        WebConfig wc = new WebConfig();
        string[] Names = wc.Str("Email Users").Split('|');
        string AdjustedName = null;

        foreach (string Name in Names)
        {
            string[] Parts = Name.Split('>');
            if (AdjustedName == null)
            {
                AdjustedName = Parts[1];
            }
            if (User.ToLower() == Parts[0].ToLower())
            {
                AdjustedName = Parts[1];
                break;
            }
        }

        AdjustedName += wc.Str("Email From");
        return AdjustedName;
    }

    public static string PrimaryEmailAddress()
    {
        WebConfig wc = new WebConfig();
        string[] Names = wc.Str("Email Users").Split('|');
        string[] Parts = Names[0].Split('>');
        string Email = Parts[0] + wc.Str("Email From");
        return Email;
    }

    private static string SignTOS =
        "Please sign the attached Terms of Service agreement and email it back to us at " + g.Company.InfoEmail + ".\n" +
        "There are a couple of ways to do this:\n" +
        "1. You can use the Fill &Sign feature in Adobe Acrobat Reader.\n" +
        "OR\n" +
        "2. You can print it, sign it, take a picture or scan it, then email the image.";

    public static void SendCustomerConfirmationEmail(Page Page, DB db, int JobRno, string Email, string Subject, string Message)
    {
        SendProposalEmail(Page, db, JobRno, Email, Subject, Message, false, null, true);
    }

    public static void SendProposalEmail(Page Page, DB db, int JobRno, string Email, string Subject, string Message, bool AttachContract)
    {
        SendProposalEmail(Page, db, JobRno, Email, Subject, Message, AttachContract, string.Empty, false);
    }

    public static void SendProposalEmail(Page Page, DB db, int JobRno, string Email, string Subject, string Message, bool AttachContract, string AttachmentFilename)
    {
        SendProposalEmail(Page, db, JobRno, Email, Subject, Message, AttachContract, AttachmentFilename, false);
    }

    public static void SendProposalEmail(Page Page, DB db, int JobRno, string Email, string Subject, string Message, bool AttachContract, string AttachmentFilename, bool fCustomerConfirmation)
	{
		bool fProposal = false;
		string JobType = string.Empty;
		string PaymentTerms = string.Empty;
		DateTime dtProposalEmailed = DateTime.MinValue;
		DateTime dtConfirmEmailed = DateTime.MinValue;
        DateTime dtFinalConfirmEmailed = DateTime.MinValue;

        string Sql = "Select JobType, PmtTerms, ProposalFlg, PropEmailedDtTm, ConfirmEmailedDtTm, FinalConfirmEmailedDtTm From mcJobs Where JobRno = " + JobRno;
		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr != null)
			{
				fProposal = DB.Bool(dr["ProposalFlg"]);
				JobType = DB.Str(dr["JobType"]);
				PaymentTerms = (JobType == "Corporate" ? DB.Str(dr["PmtTerms"]) : string.Empty).ToUpper();
				dtProposalEmailed = DB.DtTm(dr["PropEmailedDtTm"]);
				dtConfirmEmailed = DB.DtTm(dr["ConfirmEmailedDtTm"]);
                dtFinalConfirmEmailed = DB.DtTm(dr["FinalConfirmEmailedDtTm"]);
            }
        }
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Page.Response.Write(Err.Html());
			goto AllDone;
		}

		int nTrys = 0;

	TryAgain:

		nTrys++;

		try
		{
			WebConfig wc = new WebConfig();
            MailMessage Msg = new MailMessage(Misc.CreateFromEmailAddress(g.User), Email);
			Msg.CC.Add(wc.Str("Email CC"));
            Msg.CC.Add(Misc.CreateFromEmailAddress(g.User));
            Msg.Subject = Misc.EnvSubject() + Subject;

			if (AttachmentFilename != null && AttachmentFilename.Length > 0 && File.Exists(AttachmentFilename))
			{
				Msg.Attachments.Add(new Attachment(AttachmentFilename));
			}

            string Body = string.Empty;

            // use the message for the email body if a proposal or re-emailing a confirmation
            if (fProposal || dtConfirmEmailed != DateTime.MinValue || fCustomerConfirmation)
			{
                Msg.IsBodyHtml = true;


                Body += Message;
                if (AttachContract)
                {
                    Body += "\n\n" + SignTOS;
                }
                Body = string.Format(
                    "<p style='text-align: center;'><img src='http://{0}{1}' alt='" + g.Company.Name +" Logo' width='160' /></p>",
                    HttpContext.Current.Request.Url.Host,
                    VirtualPathUtility.ToAbsolute("~/Images/MarvellLogoTrans.png")) +
                    "<p>" + Body.Replace("\r\n\r\n", "</p><p>").Replace("\n\n", "</p><p>").Replace("\r\n", "<br />").Replace("\n", "<br />") + "</p>";
            }
            else
			{
				Msg.IsBodyHtml = true;

                string SignTheTOS = string.Empty;
                if (AttachContract)
                {
                    SignTheTOS = "<p>" + SignTOS.Replace("\n", "<br />") + "</p>";
                }

                string Html = (JobType == "Private" ? Misc.PrivateConfirmEmail(SignTheTOS) : Misc.CorporateConfirmEmail(SignTheTOS, PaymentTerms));
                Html = Html.Replace("<Custom Message>", (Message.Length > 0 ? "<p>" + Message.Replace("\r\n\r\n", "</p><p>").Replace("\r\n", "<br />") + "</p>" : string.Empty));
                Body += Html;
            }
            Msg.Body = Body;

            MemoryStream ms = Utils.PdfFiles.Proposal(Page, JobRno);

            bool fTest = false;
            if (fTest)
            {
                FileStream fs = new FileStream(Page.Server.MapPath(@"~/App_Data/Invoice.pdf"), FileMode.Create, FileAccess.Write);
                ms.WriteTo(fs);
                fs.Close();
            }
            else
            {
                Msg.Attachments.Add(new Attachment(ms, (fProposal ? "Proposal" : "Confirmation") + ".pdf", "application/pdf"));

                FileStream fs = null;
                if (AttachContract)
                {
                    fs = new FileStream(Page.Server.MapPath(@"~/Contracts/" + (JobType == "Private" ? "PrivateEvents.pdf" : "CorporateEvents.pdf")), FileMode.Open, FileAccess.Read);
                    Msg.Attachments.Add(new Attachment(fs, "Terms of Service.pdf", "application/pdf"));
                }

                SmtpClient Smtp = new SmtpClient();
                Smtp.Send(Msg);

                if (AttachContract)
                {
                    fs.Close();
                }
            }

            ms.Close();
        }
		catch (Exception Ex)
		{
            if (nTrys <= 3)
            {
                Thread.Sleep(1500);     // 1 1/2 seconds
                goto TryAgain;
            }
			Err Err = new Err(Ex);
			Page.Response.Write(Err.Html());
		}

		try
		{
			if (fProposal)
			{
				if (dtProposalEmailed == DateTime.MinValue)
				{
					Sql = string.Format("Update mcJobs Set PropEmailedDtTm = GetDate(), PropEmailedUser = '{1}' Where JobRno = {0}", JobRno, g.User);
					db.Exec(Sql);
				}
			}
			else 
            if (fCustomerConfirmation)
            {
                if (dtFinalConfirmEmailed == DateTime.MinValue)
                {
                    Sql = string.Format("Update mcJobs Set FinalConfirmEmailedDtTm = GetDate(), FinalConfirmEmailedUser = '{1}' Where JobRno = {0}", JobRno, g.User);
                    db.Exec(Sql);
                }
            }
            else
			{
				if (dtConfirmEmailed == DateTime.MinValue)
				{
					Sql = string.Format("Update mcJobs Set ConfirmEmailedDtTm = GetDate(), ConfirmEmailedUser = '{1}' Where JobRno = {0}", JobRno, g.User);
					db.Exec(Sql);
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Page.Response.Write(Err.Html());
		}

	AllDone:
		return;
	}

	private static string CCFee()
	{
		decimal Pct = 0;
		string Sql = "Select top 1 CCFeePct from Settings";
		DataRow dr = DB.DBDataRow(Sql);
		
		if (dr != null)
		{
			Pct = DB.Dec(dr["CCFeePct"]);
		}

		return string.Format("{0:##0.##}%", Pct);
	}

	public static string[] PrivatePaymentOptions = 
	{
		"PRIVATE EVENT PAYMENT OPTIONS:",
		"Credit Card - Visa, Master Card and Discover. We do not accept American Express.",
		"Check - Payment must be received at minimum 24 hours before event.",
		"Deposit - All private events require a $300 - $500 non-refundable payment to secure date."
	};

	public static string[] CorporatePaymentOptions = 
	{
		"EVENT PAYMENT OPTIONS:",
		"Credit Card - We accept Visa, Master Card and Discover. We do not accept American Express.",
		"Check - Payment terms are {0}. If different terms are needed please contact our finance team.",
		"Deposit - All large events require a 25% non-refundable deposit."
	};

	public static string PrivateConfirmEmail(string SignTOS)
	{
		StringBuilder sbMsg = new StringBuilder();
		foreach (string Msg in PrivatePaymentOptions)
		{
			sbMsg.AppendFormat("<p><strong>{0}</strong></p>", Msg);
		}
		return ConfirmEmail(SignTOS, sbMsg.ToString());
	}

	public static string CorporateConfirmEmail(string SignTOS, string PaymentTerms)
	{
		StringBuilder sbMsg = new StringBuilder();
		foreach (string Msg in CorporatePaymentOptions)
		{
			sbMsg.AppendFormat("<p><strong>{0}</strong></p>", string.Format(Msg, PaymentTerms));
		}
		return ConfirmEmail(SignTOS, sbMsg.ToString());
	}

	private static string ConfirmEmail(string SignTOS, string Msg)
	{
		string s = string.Format(
            "<p style='text-align: center;'><img src='http://{0}{1}' alt='" + g.Company.Name + " Logo' width='160' /></p>" +
			"<Custom Message>" +
            "<p>Thank you for booking your event with " + g.Company.Name + "! For your convenience, we have attached your customized menu along with the pricing. We will call you 7-10 business days prior to the date of your event to confirm details.</p>" +
			"{2}{3}" +
			"<p style='text-align: center;'><strong>We are thrilled to be a part of your event and confident that you will be pleased with our food and service!</strong></p>",
			HttpContext.Current.Request.Url.Host,
			VirtualPathUtility.ToAbsolute("~/Images/MarvellLogoTrans.png"),
            SignTOS,
			Msg);

        return s;
	}

    public static string MangleNumber(int Number)
    {
        // convert the decimal to hex
        string Hex = Number.ToString("X");

        // shift the ascii value by 5
        char[] aHex = Hex.ToCharArray();
        for (int i = 0; i < aHex.Length; i++)
        {
            aHex[i] = (char)((int)aHex[i] + 5);
        }

        string Value = new string(aHex);
        return Value;
    }

    public static int UnMangleNumber(string Value)
    {
        // shift characters by 5;
        char[]aValue = Value.ToCharArray();
        for (int i = 0; i < aValue.Length; i++)
        {
            aValue[i] = (char)((int)aValue[i] - 5);
        }
        Value = new string(aValue);

        // now convert the hexnumber to decimal
        int Number = 0;
        if (!int.TryParse(Value, System.Globalization.NumberStyles.HexNumber, null, out Number))
        {
            Number = 0;
        }

        return Number;
    }

    public static DateTime Max(DateTime dtA, DateTime dtB)
    {
        DateTime Max;

        if (dtA > dtB)
        {
            Max = dtA;
        }
        else
        {
            Max = dtB;
        }

        return Max;
    }

    public static DateTime ConfirmationDeadline(DateTime JobDate)
    {
        DateTime dtDeadline = JobDate.AddDays((int)DayOfWeek.Wednesday - 7 - (int)JobDate.DayOfWeek);
        //DateTime dtDeadline = JobDate.AddDays((int)DayOfWeek.Saturday - 7 - (int)JobDate.DayOfWeek);  // for easier testing
        dtDeadline = new DateTime(dtDeadline.Year, dtDeadline.Month, dtDeadline.Day, 17, 0, 0);

        return dtDeadline;
    }

    public static string EnvSubject()
    {
        string Subject = string.Empty;

        using (WebConfig wc = new WebConfig())
        {
            Subject = wc.Str("Env");
            if (Subject.Length > 0)
            {
                Subject += " - ";
            }
        }

        return Subject;
    }
}
