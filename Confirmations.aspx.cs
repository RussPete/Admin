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
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class Confirmations : System.Web.UI.Page
{
    protected WebPage Pg;
    protected DB db;

    private void Page_Init(object sender, System.EventArgs e)
    {
    }

    private void Page_Load(object sender, System.EventArgs e)
    {
        db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
        {
            Session["Menu"] = WebPage.Reports.Title;
        }
    }

    private void Page_PreRender(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Setup();
        }
    }

    private void Setup()
    {
        DateTime dtBeg = DateTime.Today;
        //DateTime dtEnd = dtBeg.AddDays(14 - (int)dtBeg.DayOfWeek);
        DateTime dtEnd = dtBeg.AddDays(21 - (int)dtBeg.DayOfWeek);      // SaReena wants to try having more time to confirm
        //DateTime dtEnd = dtBeg.AddDays(21 - (int)dtBeg.DayOfWeek);    // for easier testing

        string Sql = string.Format(
            "Select j.JobRno, Coalesce(cu.Name, c.Name) as Customer, j.JobDate, j.FinalConfirmEmailedDtTm, c.Name, c.Email, c.Phone, c.Cell, j.ServiceType " +
            "From mcJobs j " +
            "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where (JobType = 'Corporate' Or ServiceType Like '%Pick Up%') And " +
            "    JobDate Between {0} And {1} And " +
            "    ConfirmedDtTm Is Null And " +
            "    CancelledDtTm Is Null And " +
            "    IsNull(ProposalFlg , 0) = 0 And " +
            "    IsNull(c.Email, '') <> '' " +
            "Order By JobDate, Customer",
            DB.Put(dtBeg),
            DB.Put(dtEnd));
        string Html = string.Empty;

        try
        {
            int iRow = 0;
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                int JobRno = DB.Int32(dr["JobRno"]);
                DateTime dtJob = DB.DtTm(dr["JobDate"]);
                Html += string.Format(
                    "<tr>\n" +
                        "<td class=\"Send\"><input type=\"checkbox\" name=\"chkSend{0}\" value=\"true\"{9}><input type=\"hidden\" name=\"hfJobRno{0}\" value={1}></td>\n" +
                        "<td><a href=\"Job.aspx?JobRno={1}\" target=\"Job\">{1}</a></td>\n" +
                        "<td class=\"Name\"><a href=\"Job.aspx?JobRno={1}\" target=\"Job\">{2}</a></td>\n" +
                        "<td>{3}</td>\n" +
                        "<td class=\"Sent\">{4}</td>\n" +
                        "<td class=\"Contact\">{5}</td>\n" +
                        "<td>{6}</td>\n" +
                        "<td>{7}</td>\n" +
                        //"<td>{8}</td>\n" +
                        "<td class=\"Confirmed\"><input type=\"checkbox\" name=\"chkConfirmed{0}\" value=\"true\"></td>\n" +
                    "</tr>\n",
                    iRow,
                    JobRno,
                    DB.Str(dr["Customer"]),
                    Fmt.Dt(dtJob),
                    Fmt.DtTm12Hr(DB.DtTm(dr["FinalConfirmEmailedDtTm"])),
                    DB.Str(dr["Name"]),
                    DB.Str(dr["Phone"]),
                    DB.Str(dr["Cell"]),
                    DB.Str(dr["ServiceType"]),
                    (DateTime.Now < Misc.ConfirmationDeadline(dtJob) && DB.Str(dr["Email"]).Length > 0 ? string.Empty : " disabled"));
                iRow++;
            }
            hfRow.Value = iRow.ToString();
            lblReport.Text = Html;
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        MarkConfirm();
        SendConfirmEmail();

        Response.Redirect(Request.RawUrl);
    }

    protected void MarkConfirm()
    { 
        int cRows = Str.Num(hfRow.Value);
        for (int iRow = 0; iRow < cRows; iRow++)
        {
            bool fConfirmed = Str.Bool(Request.Params["chkConfirmed" + iRow.ToString()]);
            int JobRno = Str.Num(Request.Params["hfJobRno" + iRow]);

            if (fConfirmed && JobRno != 0)
            {
                DateTime Tm = DateTime.Now;
                string Sql = "Update mcJobs Set " +
                    "ConfirmedDtTm = " + DB.Put(Tm) + ", " +
                    "ConfirmedBy = " + DB.Put(g.User) + ", " +
                    "UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
                    "UpdatedUser = " + DB.PutStr(g.User) + " " +
                    "Where JobRno = " + JobRno;
                try
                {
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

    protected void SendConfirmEmail()
    {
        int cRows = Str.Num(hfRow.Value);
        for (int iRow = 0; iRow < cRows; iRow++)
        {
            bool fSend = Str.Bool(Request.Params["chkSend" + iRow.ToString()]);
            int JobRno = Str.Num(Request.Params["hfJobRno" + iRow]);

            if (fSend && JobRno != 0)
            {
                string Sql = string.Format(
                    "Select c.Email, JobDate, Guid, Coalesce(cu.Name, c.Name) as Customer, Location, GuestArrivalTime, MealTime, EndTime, NumMenServing, NumWomenServing, NumChildServing " +
                    "From mcJobs j " +
                    "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " + 
                    "Where JobRno = {0}", JobRno);
                try
                {
                    DataRow dr = db.DataRow(Sql);
                    if (dr != null)
                    {
                        string Email = DB.Str(dr["Email"]);
                        if (Email != null && Email.Length > 0)
                        {
                            string Subject = string.Format("Catering Confirmation for Event #{0}", JobRno);
                            string Guid = DB.Str(dr["Guid"]);
                            if (Guid == string.Empty)
                            {
                                Sql = string.Format("Update mcJobs Set Guid = NewID() Where JobRno = {0}; Select Guid From mcJobs Where JobRno = {0}", JobRno);
                                Guid = db.SqlStr(Sql);
                            }
                            DateTime dtJob = DB.DtTm(dr["JobDate"]);
                            DateTime dtDeadline = Misc.ConfirmationDeadline(dtJob);
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

                            string eMailBody = string.Format(
                                "Thank you for booking with us! We need to do a final confirmation for your upcoming event on {2}.\n\n" +
                                "Please review the details on this <a href=\"{0}/CustomerConfirmation.aspx?id={1}\">confirmation page</a> " +
                                "and confirm your event by {3}.\n\n" +
                                "<table border=\"0\">" +
                                "<tr><td>" +
                                "<u>Job Information</u>" +
                                "</td></tr>" +
                                "<tr><td style=\"margin-right: 20px;\">" +
                                "Event: #{4}\n" + 
                                "Customer: {5}\n" +
                                "Date: {2}\n" +
                                //"</td><td>" +
                                "Location: {6}\n" +
                                "Event Time: {7}\n" +
                                "Guest Count: {8}</div>" +
                                "</td></tr>" +
                                "</table>",
                                Request.Url.GetLeftPart(UriPartial.Authority),
                                Guid,
                                Fmt.DtNth(dtJob),
                                Fmt.DtTm12Hr(dtDeadline),
                                JobRno,
                                Customer,
                                Location,
                                EventTime,
                                NumServings);

                            Misc.SendCustomerConfirmationEmail(Page, db, JobRno, Email, Subject, eMailBody);
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
}