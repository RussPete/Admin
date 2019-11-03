using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class CheckSling : System.Web.UI.Page
{
    protected WebPage Pg;
    protected DB db;
    protected Sling Sling;
    protected Dictionary<Int32, string> MarvellJobs = new Dictionary<Int32, string>();

    private void Page_Init(object sender, System.EventArgs e)
    {
    }

    private async void Page_Load(object sender, System.EventArgs e)
    {
        db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
        {
            Session["Menu"] = WebPage.Sling.Title;

            try
            {
                await LoadData();
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex);
                Response.Write(Err.Html());
            }
        }
    }

    private async Task LoadData()
    {
        const int NumDays = 28;

        await StartSling();
        await Sling.LoadCalendar(DateTime.Today, DateTime.Today.AddDays(NumDays));

        //Debug.WriteLine("# shifts " + Sling.Shifts.Count.ToString());
        WebsiteJobs(Sling, DateTime.Today, DateTime.Today.AddDays(NumDays));
        SlingJobs(Sling, DateTime.Today);
    }

    private async Task<bool> StartSling()
    {
        WebConfig wc = new WebConfig();
        string SlingEmail = wc.Str("SlingEmail");
        string SlingPassword = wc.Str("SlingPassword");
        string SlingOrg = wc.Str("SlingOrgId");

        Sling = new Sling();
        await Sling.Login(SlingEmail, SlingPassword);

        return true;
    }

    private void WebsiteJobs(Sling Sling, DateTime dtBeg, DateTime dtEnd)
    {
        string Sql = string.Format(
            "Select JobRno, SlingShiftId, JobDate, Crew " +
            "From mcJobs " +
            "Where JobDate Between {0} and {1} " +
            "And IsNull(Crew, '') <> '' " +
            "And IsNull(ProposalFlg, 0) = 0 " +
            "Order By JobDate",
            DB.Put(dtBeg),
            DB.Put(dtEnd.AddDays(1).AddSeconds(-1)));
        try
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<tr><td colspan=\"4\"><h2 class=\"section\">Jobs Missing Sling Shift</h2></td></tr>");
            sbHtml.AppendFormat("<tr><th>Date</th><th>Job #</th><th>Sling Shift</th><th>Add Shift</th><td><label class=\"TextLink All Jobs\">All</label>&nbsp;&nbsp;<label class=\"TextLink None Jobs\">None</label></td></tr>\n");

            int i = 0;
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                Int32 JobRno = DB.Int32(dr["JobRno"]);
                string SlingShiftId = DB.Str(dr["SlingShiftId"]);
                DateTime JobDt = DB.DtTm(dr["JobDate"]);

                MarvellJobs.Add(JobRno, SlingShiftId);

                Sling.CalendarEvent Shift = null;
                if (SlingShiftId.Length > 0)
                {
                    Shift = Sling.FindShift(SlingShiftId);
                }
                if (Shift == null)
                {
                    sbHtml.AppendFormat("<tr><td>{0}</td><td><a href='Job.aspx?JobRno={1}' target='Job'>{1}</a></td><td>{2}</td><td class=\"checkbox\"><input type=checkbox name=\"JobId_{3}\" value=\"{1}\" checked=\"checked\" class=\"Job\"></td></tr>\n", JobDt.ToShortDateString(), JobRno, SlingShiftId, ++i);
                }
            }
            ltlData.Text = sbHtml.ToString();

            hfNumJobsIDs.Value = i.ToString();
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    private void SlingJobs(Sling Sling, DateTime dtBeg)
    {
        StringBuilder sbHtml = new StringBuilder();
        sbHtml.AppendLine("<tr><td colspan=\"4\"><h2 class=\"section\">Sling Shifts Missing Job</h2></td></tr>");
        sbHtml.AppendFormat("<tr><th>Date/Time</th><th>Job #</th><th>Sling Shift</th><th>Delete Shift</th><td><label class=\"TextLink All Shifts\">All</label>&nbsp;&nbsp;<label class=\"TextLink None Shifts\">None</label></td></tr>\n");
        Sling.SortShifts();

        int i = 0;
        foreach (Sling.CalendarEvent Shift in Sling.Shifts)
        {
            if (Shift.Beg > dtBeg)
            {
                string ShiftID;
                bool fFound = MarvellJobs.TryGetValue(Shift.JobRno, out ShiftID);
                if (fFound && Shift.ID != ShiftID && Shift.User == null)
                {
                    fFound = false;
                }
                if (!fFound)
                {
                    sbHtml.AppendFormat("<tr><td>{0}</td><td><a href='Job.aspx?JobRno={1}' target='Job'>{1}</a></td><td>{2}</td><td class=\"checkbox\"><input type=checkbox name=\"SlingId_{3}\" value=\"{2}\" checked=\"checked\" class=\"Shift\"></td></tr>\n", Shift.Beg.ToString(), Shift.JobRno, Shift.ID, ++i);
                }
            }
        }

        ltlData.Text += sbHtml.ToString();

        hfNumSlingIDs.Value = i.ToString();
    }

    protected async void btnProcess_Click(object sender, EventArgs e)
    {
        await StartSling();

        int NumJobsIDs = Str.Num(hfNumJobsIDs.Value);
        for (int i = 1; i <= NumJobsIDs; i++)
        {
            Int32 JobRno = Parm.Int("JobId_" + i.ToString());
            if (JobRno > 0)
            {
                // add the shift
                await AddShift(JobRno);
            }
        }

        int NumSlingIDs = Str.Num(hfNumSlingIDs.Value);
        for (int i = 1; i <= NumSlingIDs; i++)
        {
            string SlingId = Parm.Str("SlingId_" + i.ToString());
            if (SlingId != string.Empty)
            {
                // delete the shift
                await Sling.DeleteShift(Str.Num(SlingId));
            }
        }

        await LoadData();
    }

    protected async Task AddShift(int JobRno)
    {
        string Sql = string.Format(
            "Select JobDate, LoadTime, DepartureTime, ArrivalTime, MealTime, EndTime, " +
            "Customer, NumMenServing, Location, Crew, ProductionNotes " +
            "From mcJobs Where JobRno = {0}", 
            JobRno);
        try
        {
            DataRow dr = db.DataRow(Sql);

            DateTime dtJob     = DB.DtTm(dr["JobDate"]);
            DateTime tmLoad    = DB.DtTm(dr["LoadTime"]);
            DateTime tmDepart  = DB.DtTm(dr["DepartureTime"]);
            DateTime tmArrival = DB.DtTm(dr["ArrivalTime"]);
            DateTime tmMeal    = DB.DtTm(dr["MealTime"]);

            DateTime tmBeg = 
                tmLoad    != DateTime.MinValue ? tmLoad :
                tmDepart  != DateTime.MinValue ? tmDepart :
                tmArrival != DateTime.MinValue ? tmArrival :
                tmMeal    != DateTime.MinValue ? tmMeal :
                DateTime.MinValue;

            tmBeg = new DateTime(dtJob.Year, dtJob.Month, dtJob.Day, tmBeg.Hour, tmBeg.Minute, tmBeg.Second);

            DateTime tmEnd = DB.DtTm(dr["EndTime"]);
            if (tmEnd > DateTime.MinValue)
            {
                tmEnd = new DateTime(dtJob.Year, dtJob.Month, dtJob.Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second);
            }
            else
            {
                tmEnd = tmBeg.AddHours(1);
            }

            if (dtJob > DateTime.MinValue && tmBeg > DateTime.MinValue)
            {
                string Customer  = DB.Str(dr["Customer"]);
                int Servings     = DB.Int32(dr["NumMenServing"]);
                string Location  = DB.Str(dr["Location"]);
                string Crew      = DB.Str(dr["Crew"]);
                string ProdNotes = DB.Str(dr["ProductionNotes"]);

                ArrayList aSummary = new ArrayList();
                foreach (string SummaryPart in new string[]
                {
                    Customer,
                    string.Format("Job #{0}", JobRno),
                    string.Format("{0} Servings", Servings),
                    Location,
                    Crew,
                    ProdNotes
                })
                {
                    if (SummaryPart.Length > 0)
                    {
                        aSummary.Add(SummaryPart);
                    }
                }
                string Summary = string.Join(" - \\n", aSummary.ToArray());

                int SlingShiftId = await Sling.CreateShift(tmBeg, tmEnd, Sling.Location, Sling.Position, Summary);
                await Sling.PublishShift(SlingShiftId);

                Sql = string.Format("Update mcJobs Set SlingShiftId = {1} Where JobRno = {0}", JobRno, SlingShiftId);
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