using Globals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class CustomerConfirmation : System.Web.UI.Page
{
    protected DB db;

    protected void Page_Load(object sender, EventArgs e)
    {
        db = new DB();

        if (!Page.IsPostBack)
        {
        }
        else
        {
            ProcessPostBack();
        }
    }

    private void Page_PreRender(object sender, System.EventArgs e)
    {
        if (Page.IsPostBack)
        {
            Response.Redirect(g.Company.WebsiteUrl);
            Response.End();
        }
        else
        {
            string id = Request.Params["id"];

            if (id == null || id == string.Empty)
            {
                lblInstructions.Text =
                    "<p>The catering event parameter is missing.</p>" +
                    "<p>Please call or email our office to discuss your catering event.</p>" +
                    "<p>Visit our website <a href=\"" + g.Company.WebsiteUrl + ">" + g.Company.Website +"</a>.</p>";
            }
            else
            {
                FormatJob(id);
            }
        }
    }

    private bool FormatJob(string id)
    {
        bool fOK = true;
        string Sql = string.Format(
            "Select JobRno, JobDate, CancelledDtTm, Coalesce(cu.Name, c.Name) as Customer, Location, " +
            "GuestArrivalTime, MealTime, EndTime, " +
            "NumMenServing, NumWomenServing, NumChildServing, ServicePropDesc, DeliveryPropDesc, " +
            "ChinaPropDesc, AddServicePropDesc, FuelTravelPropDesc, FacilityPropDesc, RentalsPropDesc, " +
            "Adj1PropDesc, Adj2PropDesc, cu.TaxExemptFlg, SubTotTaxPct, EstTotPropDesc, DepositPropDesc, " +
            "FinishConfirmDtTm, ConfirmedDtTm " +
            "From mcJobs  j " +
            "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where Guid = {0}", DB.Put(id));
        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
                int JobRno = DB.Int32(dr["JobRno"]);
                DateTime dtFinishedConfirm = DB.DtTm(dr["FinishConfirmDtTm"]);
                DateTime dtConfirmed = DB.DtTm(dr["ConfirmedDtTm"]);
                if (dtFinishedConfirm == DateTime.MinValue && dtConfirmed == DateTime.MinValue)
                {
                    DateTime dtJob = DB.DtTm(dr["JobDate"]);
                    DateTime dtCancelled = DB.DtTm(dr["CancelledDtTm"]);
                    if (DateTime.Now <= dtJob && dtCancelled == DateTime.MinValue)
                    {
                        DateTime dtDeadline = Misc.ConfirmationDeadline(dtJob);

                        if (DateTime.Now < dtDeadline)
                        {
                            lblInstructions.Text =
                                "<p>You will see and confirm your event deails in several sections.</p>" +
                                "<p>Please carefully review the information then click <span class=\"success\">Yes</span> " +
                                "if it's all correct or <span class=\"danger\">No</span> to submit changes in the box that will appear. " +
                                "You can click <span class=\"danger\">No</span> on any previous section.</p>" +
                                "<p>" + Globals.g.Company.Name + " will contact you soon if you do not confirm every section.</p>" +
                                "<p>Visit our website <a href=\"" + g.Company.WebsiteUrl + "\">" + g.Company.Website + " </a>.</p>";

                            // info
                            //------------------------------------------------------------
                            string Customer = DB.Str(dr["Customer"]);
                            string Location = DB.Str(dr["Location"]);
                            DateTime GuestTime = DB.DtTm(dr["GuestArrivalTime"]);
                            DateTime BegTime = DB.DtTm(dr["MealTime"]);
                            DateTime EndTime = DB.DtTm(dr["EndTime"]);

                            if (GuestTime != DateTime.MinValue && GuestTime < BegTime)
                            {
                                BegTime = GuestTime;
                            }

                            string EventTime;
                            if (EndTime == DateTime.MinValue)
                            {
                                EventTime = BegTime.ToString("h:mm tt").ToLower();
                            }
                            else
                            {
                                EventTime = string.Format("{0} - {1}", BegTime.ToString("h:mm tt"), EndTime.ToString("h:mm tt")).ToLower();
                            }

                            int NumServings = DB.Int32(dr["NumMenServing"]) + DB.Int32(dr["NumWomenServing"]) + DB.Int32(dr["NumChildServing"]);

                            string Html = string.Format(
                                "<div class=\"Info\">" +
                                    "<div class=\"card\">" +
                                        "<h3 class=\"card-header\">Job Information</h3>" +
                                        "<div class=\"card-body container\">" +
                                            "<div class=\"row\">" +
                                                "<div class=\"col\">" +
                                                    "Event: #{0}<br />" +
                                                    "Customer: {1}<br />" +
                                                    "Date: {2}" +
                                                "</div>" +
                                                "<div class=\"col\">" +
                                                    "Location: {3}<br />" +
                                                    "Event Time: {4}<br />" +
                                                    "Guest Count: {5}" +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                    "</div>",
                                    JobRno,
                                    Customer,
                                    Fmt.DtNth(dtJob),
                                    Location,
                                    EventTime,
                                    NumServings) +
                                ConfirmButtons("Job") +
                                "</div>";


                            // menu 
                            //------------------------------------------------------------
                            Sql = string.Format(
                                "Select * " +
                                "From mcJobFood " +
                                "Where JobRno = {0} And ProposalHideFlg = 0 " +
                                "Order By ProposalSeq, FoodSeq",
                                JobRno);
                            DataTable dt = db.DataTable(Sql);
                            if (dt.Rows.Count > 0)
                            {
                                Html +=
                                    "<div class=\"Menu\">" +
                                        "<div class=\"card\">" +
                                            "<h3 class=\"card-header\">Food</h3>" +
                                            "<div class=\"card-body\">";
                                foreach (DataRow drMenu in dt.Rows)
                                {
                                    string ProposalMenuItem = DB.Str(drMenu["ProposalMenuItem"]);
                                    bool fProposalTitle = DB.Bool(drMenu["ProposalTitleFlg"]);

                                    if (fProposalTitle)
                                    {
                                        Html += string.Format("<h4 class=\"card-title\">{0}</h4>", ProposalMenuItem);
                                    }
                                    else
                                    {
                                        Html += string.Format("<div class=\"card-text\">{0}</div>", (ProposalMenuItem.Length > 0 ? ProposalMenuItem : "&nbspc;"));
                                    }
                                }
                                Html +=
                                            "</div>" +
                                        "</div>" +
                                        ConfirmButtons("Menu") +
                                    "</div>";
                            }

                            // prices
                            //------------------------------------------------------------
                            Sql = string.Format("Select * From JobInvoicePrices Where JobRno = {0} Order By Seq", JobRno);
                            dt = db.DataTable(Sql);
                            if (dt.Rows.Count > 0)
                            {
                                Html +=
                                    "<div class=\"Prices\">" +
                                        "<div class=\"card\">" +
                                            "<h3 class=\"card-header\">Pricing</h3>" +
                                            "<div class=\"card-body\">";
                                foreach (DataRow drPrice in dt.Rows)
                                {
                                    string PriceType = DB.Str(drPrice["PriceType"]);
                                    string Desc = string.Empty;

                                    switch (PriceType)
                                    {
                                        case Misc.cnPerPerson:
                                            Desc = DB.Str(drPrice["PropPerPersonDesc"]);
                                            break;

                                        case Misc.cnPerItem:
                                            Desc = DB.Str(drPrice["PropPerItemDesc"]);
                                            break;

                                        case Misc.cnAllIncl:
                                            Desc = DB.Str(drPrice["PropAllInclDesc"]);
                                            break;
                                    }

                                    if (Desc.Length > 0)
                                    {
                                        Html += string.Format("<p class=\"card-text\">{0}</p>", Desc);
                                    }
                                }
                                Html +=
                                            "</div>" +
                                        "</div>" +
                                        ConfirmButtons("Prices") +
                                    "</div>";
                            }

                            // fees
                            //------------------------------------------------------------

                            Html +=
                                "<div class=\"Fees\">" +
                                    "<div class=\"card\">" +
                                        "<h3 class=\"card-header\">Additional Services</h3>" +
                                        "<div class=\"card-body\">";
                            // services
                            string Fee = DB.Str(dr["ServicePropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }

                            // delivery
                            Fee = DB.Str(dr["DeliveryPropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }

                            // china
                            Fee = DB.Str(dr["ChinaPropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }

                            // additional services
                            Fee = DB.Str(dr["AddServicePropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }

                            // fuel & travel
                            Fee = DB.Str(dr["FuelTravelPropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }

                            // facility
                            Fee = DB.Str(dr["FacilityPropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }

                            // rentals
                            Fee = DB.Str(dr["RentalsPropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }

                            // adjustment 1
                            Fee = DB.Str(dr["Adj1PropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }

                            // adjustment 2
                            Fee = DB.Str(dr["Adj2PropDesc"]);
                            if (Fee.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                            }
                            Html +=
                                        "</div>" +
                                    "</div>" +
                                    ConfirmButtons("Fees") +
                                "</div>";


                            Html +=
                                "<div class=\"Total\">" +
                                    "<div class=\"card\">" +
                                        "<h3 class=\"card-header\">Total</h3>" +
                                        "<div class=\"card-body\">";
                            // sales tax
                            //------------------------------------------------------------
                            bool fTaxExempt = DB.Bool(dr["TaxExemptFlg"]);
                            if (!fTaxExempt)
                            {
                                string Tax = string.Format("Utah Food Sales Tax {0:0.00}%", DB.Dec(dr["SubTotTaxPct"]));
                                Html += string.Format("<p class=\"card-text\">{0}</p>", Tax);
                            }

                            // total
                            //------------------------------------------------------------
                            string TotalDesc = string.Format("\n{0}", DB.Str(dr["EstTotPropDesc"]));
                            Html += string.Format("<p class=\"card-text\">{0}</p>", TotalDesc);

                            // deposit
                            //------------------------------------------------------------
                            string DepositDesc = DB.Str(dr["DepositPropDesc"]);
                            if (DepositDesc.Length > 0)
                            {
                                Html += string.Format("<p class=\"card-text\">{0}</p>", DepositDesc);
                            }
                            Html +=
                                        "</div>" +
                                    "</div>" +
                                    ConfirmButtons("Total") +
                                "</div>";

                            // tables & Linens
                            //------------------------------------------------------------
                            Html +=
                                "<div class=\"Tables\">" +
                                    "<div class=\"card\">" +
                                        "<h3 class=\"card-header\">Tables &amp; Linens</h3>" +
                                        "<div class=\"card-body container\">" +
                                            "<div class=\"row\">" +
                                                "<div class=\"col-11 col-sm-10 col-md-7 col-lg-6 col-xl-5 Buffet\">" +
                                                    "<div>" +
                                                        "Buffet tables provided by " +
                                                        "<select name=ddlTables>" +
                                                            "<option value=\"" + Globals.g.Company.Name + "\">" + Globals.g.Company.Name + "</option>" +
                                                            "<option value=\"Venue\">Venue</option>" +
                                                        "</select>" +
                                                    "</div>" +
                                                    "<div>" +
                                                        "Buffet linen color " +
                                                        "<select name=\"ddlLinenColor\">" +
                                                            "<option value=\"White\">White</option>" +
                                                            "<option value=\"Black\">Black</option>" +
                                                            "<option value=\"Ivory\">Ivory</option>" +
                                                            (dtJob.Month == 12 ? "<option value=\"Red\">Red</option>" : string.Empty) +
                                                        "</select>" +
                                                    "</div>" +
                                                    "<div>" +
                                                        "Linen color for guest tables " +
                                                        "<input type=text id=\"txtGuestTableColor\" name=\"txtGuestTableColor\" />" +
                                                        "<div class=\"ColorRequired\">*color is required</div>" +
                                                    "</div>" +
                                                    "<div>" +
                                                        "Guest linens provided by " +
                                                        "<select id=\"ddlLinensProvidedBy\" name=\"ddlLinensProvidedBy\">" +
                                                            "<option value=\"" + Globals.g.Company.Name + "\">" + Globals.g.Company.Name + "</option>" +
                                                            "<option value=\"Customer\">Customer</option>" +
                                                        "</select>" +
                                                    "</div>" +
                                                    "<div class=\"GuestTables\">" +
                                                        "<div>" +
                                                            "<div class=\"TableSizeShape\"><u>Table Size &amp; Shape</u></div>" +
                                                            "<div class=\"TableCount\"><u>Count</u></div>" +
                                                        "</div>" +
                                                        "<div>" +
                                                            "<div class=\"TableSizeShape\"><input type=text id=\"txtTableSizeShape1\" name=\"txtTableSizeShape1\"></div>" +
                                                            "<div class=\"TableCount\"><input type=text id=\"txtTableCount1\" name=\"txtTableCount1\"></div>" +
                                                        "</div>" +
                                                        "<div>" +
                                                            "<div class=\"TableSizeShape\"><input type=text id=\"txtTableSizeShape2\" name=\"txtTableSizeShape2\"></div>" +
                                                            "<div class=\"TableCount\"><input type=text id=\"txtTableCount2\" name=\"txtTableCount2\"></div>" +
                                                        "</div>" +
                                                        "<div>" +
                                                            "<div class=\"TableSizeShape\"><input type=text id=\"txtTableSizeShape3\" name=\"txtTableSizeShape3\"></div>" +
                                                            "<div class=\"TableCount\"><input type=text id=\"txtTableCount3\" name=\"txtTableCount3\"></div>" +
                                                        "</div>" +
                                                        "<div>" +
                                                            "<div class=\"TableSizeShape\"><input type=text id=\"txtTableSizeShape4\" name=\"txtTableSizeShape4\"></div>" +
                                                            "<div class=\"TableCount\"><input type=text id=\"txtTableCount4\" name=\"txtTableCount4\"></div>" +
                                                        "</div>" +
                                                    "</div>" +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                    "</div>" +
                                    ConfirmButtons("Tables", true) +
                                "</div>";

                            lblJob.Text = Html;
                        }
                        else
                        {
                            fOK = false;
                            lblInstructions.Text =
                                "<p>Sorry, its is too late to confirm your event online.</p>" +
                                "<p>Please call or <a href=\"mailto: Info(" + g.Company.InfoEmail + ")\">email</a> " + Globals.g.Company.Name + " to confirm or discuss your event.</p>" +
                                "<p>Visit our website <a href=\"" + g.Company.WebsiteUrl + "\">" + g.Company.Website + "</a>.</p>";
                        }
                    }
                    else
                    {
                        fOK = false;
                        lblInstructions.Text = string.Format(
                            "<p>Sorry, that page link is has expired, usually because the event date has past or the event was canceled.</p>" +
                            "<p>If you believe there is a problem, please call or <a href=\"mailto: Info(" + g.Company.InfoEmail + ")\">email</a> " + Globals.g.Company.Name + " to discuss event #{0}.</p>" +
                            "<p>Visit our website <a href=\"" + g.Company.WebsiteUrl + "\">" + g.Company.Website + "</a>.</p>",
                            JobRno);
                    }
                }
                else
                {
                    fOK = false;
                    lblInstructions.Text = string.Format(
                        "<p>Your event has already been confirmed.</p>" +
                        "<p>Please call or <a href=\"mailto: Info(" + g.Company.InfoEmail + ")\">email</a> " + Globals.g.Company.Name + " to discuss event #{0}.</p>" +
                        "<p>Visit our website <a href=\"" + Globals.g.Company.WebsiteUrl + "\">" + Globals.g.Company.Website + "</a>.</p>",
                        JobRno);
                }
            }
            else
            {
                fOK = false;
                lblInstructions.Text =
                    "<p>Sorry, this page link is not valid.</p>" +
                    "<p>Please call or <a href=\"mailto: Info(" + Globals.g.Company.InfoEmail + ")\">email</a> " + Globals.g.Company.Name + " to confirm or discuss your event.</p>" +
                    "<p>Visit our website <a href=\"" + Globals.g.Company.WebsiteUrl + "\">" + Globals.g.Company.Website + "</a>.</p>";
            }
        }
        catch (SqlException)
        {
            fOK = false;
            lblInstructions.Text =
                "<p>Sorry, this page link is not valid.</p>" +
                "<p>Please call or <a href=\"mailto: Info(" + Globals.g.Company.InfoEmail + ")\">email</a> " + Globals.g.Company.Name + " to confirm or discuss your event.</p>" +
                "<p>Visit our website <a href=\"" + Globals.g.Company.WebsiteUrl + "\">" + Globals.g.Company.Website + "</a>.</p>";
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return fOK;
    }

    protected string ConfirmButtons(string SectionName, bool fFinished = false)
    {
        return string.Format(
            "<div class=\"container Confirm\">" +
            (!fFinished ?
                "<div class=\"row align-items-center btn-group-toggle\" data-toggle=\"buttons\">" +
                    "<div class=\"col-4 col-sm-3 col-md-2 prompt\">Confirm</div>" +
                    "<label class=\"col-4 col-sm-3 col-md-2 btn btn-outline-success Yes\">" +
                        "<input type=\"radio\" name=\"rdo{0}Confirm\" value=\"Yes\">Yes" +
                    "</label>" +
                    "<label class=\"col-4 col-sm-3 col-md-2 btn btn-outline-danger No\">" +
                        "<input type=\"radio\" name=\"rdo{0}Confirm\" value=\"No\">No" +
                    "</label>" +
                "</div>" +
                "<div class=\"Notes Prompt\">Describe any changes to the above section:</div>"
            :
                "<div class=\"Notes Prompt\">Describe any additional details:</div>") +
                "<div class=\"Notes Text row\">" +
                    "<textarea name=\"txt{0}\" class=\"col notes\"></textarea>" +
                "</div>" +
            (!fFinished ?
                "<div class=\"Notes Next row\">" +
                    "<button type=\"button\" class=\"col-4 col-sm-3 col-md-2 btn btn-outline-primary Next\">Next</button>" +
                "</div>"
            :
                "<div class=\"Notes Finish row\">" +
                    "<button type=\"submit\" class=\"col-4 col-sm-3 col-md-2 btn btn-outline-primary Finish\">Finish</button>" +
                    "<label id=\"lblFinish\" class=\"col-8 col-sm-9 col-md-10 danger\"></label>" +
                "</div>"
            ) +
            "</div>",
            SectionName);
    }

    private void ProcessPostBack()
    {
        string id = Request.Params["id"];

        if (id.Length > 0)
        {
            int JobRno = 0;
            string FromEmail = string.Empty;

            string Sql = "Select JobRno, Email From mcJobs j Inner Join Contacts c on j.ContactRno = c.ContactRno Where Guid = @Guid";
            try
            {
                DataRow dr = db.DataRow(Sql, "@Guid", id);
                if (dr != null)
                {
                    JobRno = DB.Int32(dr["JobRno"]);
                    FromEmail = DB.Str(dr["Email"]);
                }
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                Response.Write(Err.Html());
                goto AllDone;
            }

            SectionInfo[] Sections =
            {
                new SectionInfo() { id = "Job", Desc = "Job" },
                new SectionInfo() { id = "Menu", Desc = "Menu" },
                new SectionInfo() { id = "Prices", Desc = "Prices" },
                new SectionInfo() { id = "Fees", Desc = "Fees" },
                new SectionInfo() { id = "Total", Desc = "Total" }
            };

            bool fConfirmed = true;
            foreach (SectionInfo section in Sections)
            {
                fConfirmed = fConfirmed && Request.Params["rdo" + section.id + "Confirm"] == "Yes";
            }

            DateTime Tm = DateTime.Now;
            Sql =
                "Update mcJobs Set " +
                "FinishConfirmDtTm = " + DB.Put(Tm) + ", " +
                (fConfirmed ? "ConfirmedDtTm = " + DB.Put(Tm) + ", " : string.Empty) +
                (fConfirmed ? "ConfirmedBy = 'Customer', " : string.Empty) +
                "UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
                "UpdatedUser = 'Customer' " +
                "Where Guid = @Guid";
            try
            {
                db.Exec(Sql, "@Guid", id);
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                Response.Write(Err.Html());
                goto AllDone;
            }

            string Subject = string.Format("Event #{0} {1}Confirmed", JobRno, (fConfirmed ? string.Empty : "NOT "));
            string Body = string.Empty;

            Sql = string.Format(
                "Select JobRno, JobDate, Coalesce(cu.Name, c.Name) as Customer, Location, GuestArrivalTime, MealTime, EndTime, " +
                "NumMenServing, NumWomenServing, NumChildServing, ServicePropDesc, DeliveryPropDesc, " +
                "ChinaPropDesc, AddServicePropDesc, FuelTravelPropDesc, FacilityPropDesc, RentalsPropDesc, " +
                "Adj1PropDesc, Adj2PropDesc, cu.TaxExemptFlg, SubTotTaxPct, EstTotPropDesc, DepositPropDesc, " +
                "ConfirmedDtTm " +
                "From mcJobs j " +
                "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " + 
                "Where JobRno = {0}", JobRno);
            try
            {
                DataRow dr = db.DataRow(Sql);
                if (dr != null)
                {

                    if (fConfirmed)
                    {
                        Body = string.Format("<h1>Event #{0} Confirmed</h1>", JobRno);
                    }
                    else
                    {
                        Body += string.Format("<h1>Event #{0} Not Confimed</h1>", JobRno);
                    }

                    // info
                    //------------------------------------------------------------
                    DateTime dtJob = DB.DtTm(dr["JobDate"]);
                    string Customer = DB.Str(dr["Customer"]);
                    string Location = DB.Str(dr["Location"]);
                    DateTime GuestTime = DB.DtTm(dr["GuestArrivalTime"]);
                    DateTime BegTime = DB.DtTm(dr["MealTime"]);
                    DateTime EndTime = DB.DtTm(dr["EndTime"]);

                    if (GuestTime != DateTime.MinValue && GuestTime < BegTime)
                    {
                        BegTime = GuestTime;
                    }

                    string EventTime;
                    if (EndTime == DateTime.MinValue)
                    {
                        EventTime = BegTime.ToString("h:mm tt").ToLower();
                    }
                    else
                    {
                        EventTime = string.Format("{0} - {1}", BegTime.ToString("h:mm tt"), EndTime.ToString("h:mm tt")).ToLower();
                    }

                    int NumServings = DB.Int32(dr["NumMenServing"]) + DB.Int32(dr["NumWomenServing"]) + DB.Int32(dr["NumChildServing"]);

                    string SectionConfirmed = Request.Params["rdoJobConfirm"];
                    Body += string.Format(
                        "<table border=\"0\">" +
                        "<tr><td colspan=\"2\"><h2>Job Information</h2></td></tr>" +
                        "<tr>" +
                            "<td>" +
                                "Event: #{0}<br />" +
                                "Customer: {1}<br />" +
                                "Date: {2}" +
                            "</td>" +
                            "<td>" +
                                "Location: {3}<br />" +
                                "Event Time: {4}<br />" +
                                "Guest Count: {5}" +
                            "</td>" +
                        "</tr></table>" +
                        "<p>Confirm: <b>{6}</b></p>",
                        JobRno,
                        Customer,
                        Fmt.DtNth(dtJob),
                        Location,
                        EventTime,
                        NumServings,
                        SectionConfirmed);
                    if (SectionConfirmed != "Yes")
                    {
                        Body += "<p><b>Notes:</b><br />" + Request.Params["txtJob"].Replace("\r\n\r\n", "</p><p>").Replace("\n\n", "</p><p>") + "</p>";
                    }

                    // menu 
                    //------------------------------------------------------------
                    Sql = string.Format(
                        "Select * " +
                        "From mcJobFood " +
                        "Where JobRno = {0} And ProposalHideFlg = 0 " +
                        "Order By ProposalSeq, FoodSeq",
                        JobRno);
                    DataTable dt = db.DataTable(Sql);
                    if (dt.Rows.Count > 0)
                    {
                        Body += 
                            "<table border=\"0\">" +
                            "<tr><td colspan=\"2\"><h2>Food</h2></td></tr>";

                        foreach (DataRow drMenu in dt.Rows)
                        {
                            string ProposalMenuItem = DB.Str(drMenu["ProposalMenuItem"]);
                            bool fProposalTitle = DB.Bool(drMenu["ProposalTitleFlg"]);

                            if (fProposalTitle)
                            {
                                Body += string.Format("<tr><td colspan=\"2\"><b>{0}</b></td></tr>", ProposalMenuItem);
                            }
                            else
                            {
                                Body += string.Format("<tr><td width=\"20\">&nbsp;</td><td>{0}</td></tr>", (ProposalMenuItem.Length > 0 ? ProposalMenuItem : "&nbspc;"));
                            }
                        }

                        SectionConfirmed = Request.Params["rdoMenuConfirm"];
                        Body += string.Format(
                            "</table>" +
                            "<p>Confirm: <b>{0}</b></p>",
                            SectionConfirmed);
                        if (SectionConfirmed != "Yes")
                        {
                            Body += "<p><b>Notes:</b><br />" + Request.Params["txtMenu"].Replace("\r\n\r\n", "</p><p>").Replace("\n\n", "</p><p>") + "</p>";
                        }
                    }

                    // prices
                    //------------------------------------------------------------
                    Sql = string.Format("Select * From JobInvoicePrices Where JobRno = {0} Order By Seq", JobRno);
                    dt = db.DataTable(Sql);
                    if (dt.Rows.Count > 0)
                    {
                        Body +=
                            "<table border=\"0\">" +
                            "<tr><td><h2>Pricing</h2></td></tr>";
                        foreach (DataRow drPrice in dt.Rows)
                        {
                            string PriceType = DB.Str(drPrice["PriceType"]);
                            string Desc = string.Empty;

                            switch (PriceType)
                            {
                                case Misc.cnPerPerson:
                                    Desc = DB.Str(drPrice["PropPerPersonDesc"]);
                                    break;

                                case Misc.cnPerItem:
                                    Desc = DB.Str(drPrice["PropPerItemDesc"]);
                                    break;

                                case Misc.cnAllIncl:
                                    Desc = DB.Str(drPrice["PropAllInclDesc"]);
                                    break;
                            }

                            if (Desc.Length > 0)
                            {
                                Body += string.Format("<tr><td>{0}</td></tr>", Desc);
                            }
                        }

                        SectionConfirmed = Request.Params["rdoPricesConfirm"];
                        Body += string.Format(
                            "</table>" +
                            "<p>Confirm: <b>{0}</b></p>",
                            SectionConfirmed);
                        if (SectionConfirmed != "Yes")
                        {
                            Body += "<p><b>Notes:</b><br />" + Request.Params["txtPrices"].Replace("\r\n\r\n", "</p><p>").Replace("\n\n", "</p><p>") + "</p>";
                        }
                    }

                    // fees
                    //------------------------------------------------------------

                    Body +=
                        "<table border=\"0\">" +
                        "<tr><td><h2>Additional Servics</h2></td></tr>";

                    // services
                    string Fee = DB.Str(dr["ServicePropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    // delivery
                    Fee = DB.Str(dr["DeliveryPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    // china
                    Fee = DB.Str(dr["ChinaPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    // additional services
                    Fee = DB.Str(dr["AddServicePropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    // fuel & travel
                    Fee = DB.Str(dr["FuelTravelPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    // facility
                    Fee = DB.Str(dr["FacilityPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    // rentals
                    Fee = DB.Str(dr["RentalsPropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    // adjustment 1
                    Fee = DB.Str(dr["Adj1PropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    // adjustment 2
                    Fee = DB.Str(dr["Adj2PropDesc"]);
                    if (Fee.Length > 0)
                    {
                        Body += string.Format("<p class=\"card-text\">{0}</p>", Fee);
                    }

                    SectionConfirmed = Request.Params["rdoFeesConfirm"];
                    Body += string.Format(
                        "</table>" +
                        "<p>Confirm: <b>{0}</b></p>",
                        SectionConfirmed);
                    if (SectionConfirmed != "Yes")
                    {
                        Body += "<p><b>Notes:</b><br />" + Request.Params["txtFees"].Replace("\r\n\r\n", "</p><p>").Replace("\n\n", "</p><p>") + "</p>";
                    }


                    Body +=
                        "<table border=\"0\">" +
                        "<tr><td><h2>Total</h2></td></tr>";

                    // sales tax
                    //------------------------------------------------------------
                    bool fTaxExempt = DB.Bool(dr["TaxExemptFlg"]);
                    if (!fTaxExempt)
                    {
                        string Tax = string.Format("Utah Food Sales Tax {0:0.00}%", DB.Dec(dr["SubTotTaxPct"]));
                        Body += string.Format("<tr><td>{0}</td></tr>", Tax);
                    }

                    // total
                    //------------------------------------------------------------
                    string TotalDesc = string.Format("\n{0}", DB.Str(dr["EstTotPropDesc"]));
                    Body += string.Format("<tr><td>{0}</td></tr>", TotalDesc);

                    // deposit
                    //------------------------------------------------------------
                    string DepositDesc = DB.Str(dr["DepositPropDesc"]);
                    if (DepositDesc.Length > 0)
                    {
                        Body += string.Format("<tr><td>{0}</td></tr>", DepositDesc);
                    }

                    SectionConfirmed = Request.Params["rdoTotalConfirm"];
                    Body += string.Format(
                        "</table>" +
                        "<p>Confirm: <b>{0}</b></p>",
                        SectionConfirmed);
                    if (SectionConfirmed != "Yes")
                    {
                        Body += "<p><b>Notes:</b><br />" + Request.Params["txtTotal"].Replace("\r\n\r\n", "</p><p>").Replace("\n\n", "</p><p>") + "</p>";
                    }

                    // tables & linens
                    //------------------------------------------------------------
                    string LinensProvidedBy = Request.Params["ddlLinensProvidedBy"];
                    Body +=
                        "<table border=\"0\">" +
                            "<tr><td colspan=\"2\"><h2>Tables &amp; Linens</h2></td></tr>" +
                            "<tr><td colspan=\"2\">Buffet tables provided by <b>" + Request.Params["ddlTables"] + "</b></td></tr>" +
                            "<tr><td colspan=\"2\">Buffet linen color <b>" + Request.Params["ddlLinenColor"] + "</b></td></tr>" +
                            "<tr><td colspan=\"2\">Linen color for guest tables <b>" + Request.Params["txtGuestTableColor"] + "</b></td></tr>" +
                            "<tr><td colspan=\"2\">Guest linens provided by <b>" + LinensProvidedBy + "</b></td></tr>" +
                        (LinensProvidedBy == Globals.g.Company.Name ?
                            "<tr><td colspan=\"2\">&nbsp;</td></tr>" +
                            "<tr><td align=\"center\"><u>Table Size &amp; Shape</u></td><td align=\"center\"><u>Count</u></td></tr>" +
                            "<tr><td align=\"center\">" + Request.Params["txtTableSizeShape1"] + "</td><td align=\"center\">" + Request.Params["txtTableCount1"] +"</td></tr>" +
                            "<tr><td align=\"center\">" + Request.Params["txtTableSizeShape2"] + "</td><td align=\"center\">" + Request.Params["txtTableCount2"] + "</td></tr>" +
                            "<tr><td align=\"center\">" + Request.Params["txtTableSizeShape3"] + "</td><td align=\"center\">" + Request.Params["txtTableCount3"] + "</td></tr>" +
                            "<tr><td align=\"center\">" + Request.Params["txtTableSizeShape4"] + "</td><td align=\"center\">" + Request.Params["txtTableCount4"] + "</td></tr>"
                        : 
                            string.Empty) +
                        "</table>" +
                        "<p><b>Notes:</b><br />" + Request.Params["txtTables"].Replace("\r\n\r\n", "</p><p>").Replace("\n\n", "</p><p>") + "</p>";
                }

                int nTrys = 0;

                TryAgain:

                nTrys++;

                try
                {
                    WebConfig wc = new WebConfig();
                    MailMessage Msg = new MailMessage(FromEmail, Misc.PrimaryEmailAddress());
                    Msg.CC.Add(wc.Str("Email CC"));
                    Msg.Subject = Misc.EnvSubject() + Subject;
                    Msg.IsBodyHtml = true;
                    Msg.Body = Body;

                    SmtpClient Smtp = new SmtpClient();
                    Smtp.Send(Msg);
                }
                catch (Exception Ex)
                {
                    if (nTrys <= 3)
                    {
                        Thread.Sleep(1500);     // 1 1/2 seconds
                        goto TryAgain;
                    }
                    Err Err = new Err(Ex, Sql);
                    Response.Write(Err.Html());
                    goto AllDone;
                }
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                Response.Write(Err.Html());
            }
        }

    AllDone:

        return;
    }

    public class SectionInfo
    {
        public string id;
        public string Desc;
    }
}
