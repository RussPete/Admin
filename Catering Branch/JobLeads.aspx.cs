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

public partial class JobLeads : System.Web.UI.Page
{
    protected WebPage Pg;
    protected DB db;
    protected bool fJobs = false;
    protected bool fSendNotices = false;
    protected DateTime dtBeg;
    protected DateTime dtEnd;
    protected Int32 JobRno;
    protected Int32[] JobRnos;
    //protected String SlingEmail;
    //protected String SlingPassword;
    //protected String SlingOrgId;
    protected Sling Sling;

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
            Setup();
        }
        //WebConfig wc = new WebConfig();
        //SlingEmail = wc.Str("SlingEmail");
        //SlingPassword = wc.Str("SlingPassword");
        //SlingOrgId = wc.Str("SlingOrgId");
    }

    private async void Page_PreRender(object sender, System.EventArgs e)
    {
        if (fJobs)
        {
            await Jobs();
        }
    }

    private void Setup()
    {
        if (Request.QueryString["JobRno"] != null)
        {
            JobRno = Str.Num(Request.QueryString["JobRno"]);
        }

        DateTime Today = DateTime.Today;
        DateTime FirstDay = Today.AddDays((Today.DayOfWeek < DayOfWeek.Saturday ? 0 : 7) - (int)Today.DayOfWeek);

        rdoRange.Checked = true;

        txtDayDate.Text = Fmt.Dt(Today);
        txtBegDateRange.Text = Fmt.Dt(FirstDay);
        txtEndDateRange.Text = Fmt.Dt(FirstDay.AddDays(6));

        txtDayDate.Attributes.Add("onChange", "iSetChk('rdoDay', true);");
        txtBegDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
        txtEndDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
    }

    protected void btnFindJobLeads_Click(object sender, System.EventArgs e)
    {
        ReportDates();
    }

    protected void ReportDates()
    {
        if (rdoDay.Checked)
        {
            dtBeg = Convert.ToDateTime(txtDayDate.Text);
            dtEnd = dtBeg;
        }

        if (rdoRange.Checked)
        {
            dtBeg = Convert.ToDateTime(txtBegDateRange.Text);
            dtEnd = Convert.ToDateTime(txtEndDateRange.Text);
        }

        hfBegDate.Value = dtBeg.ToString("MM/dd/yyyy");
        hfEndDate.Value = dtEnd.ToString("MM/dd/yyyy");

        fJobs = true;
    }

    protected async Task Jobs()
    {
        // connect to sling, retrieve all the shifts and orgainize shifts into jobs
        await SetupSlingShifts();

        string JobDate = (rdoDay.Checked
            ? string.Format("= {0}", DB.PutDtTm(dtBeg))
            : string.Format("Between {0} And {1}", DB.PutDtTm(dtBeg), DB.PutDtTm(dtEnd)));
        string Sql = string.Format(
            "Select JobRno, JobDate, MealTime, Coalesce(cu.Name, c.Name) as Customer " +
            "From mcJobs j " +
            "Inner Join Contacts c On j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " + 
            "Where JobDate {0} And IsNull(Crew, '') > '' Order By JobDate, MealTime",
            JobDate);

        Collection<Job> cJobs = new Collection<Job>();

        try
        {
            // gather information on the jobs;
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                int JobRno = DB.Int32(dr["JobRno"]);
                DateTime Date = DB.DtTm(dr["JobDate"]);
                DateTime Time = DB.DtTm(dr["MealTime"]);
                string Customer = DB.Str(dr["Customer"]);

                Job Job;
                cJobs.Add(Job = new Job() { JobRno = JobRno, Date = Date, Time = Time, Customer = Customer });

                // find the shifts connected with this job
                foreach (Sling.CalendarEvent Shift in Sling.Shifts)
                {
                    if (Job.JobRno == Shift.JobRno)
                    {
                        Job.cShifts.Add(Shift);
                    }
                }

                // sort shifts within this job, job lead in first shift
                Sling.SortShiftCrew(Job.cShifts);
                if (Job.cShifts.Count > 0 && Job.cShifts[0].User != null && Job.cShifts[0].User.ID != 0)
                {
                    Job.JobLeadUserID = Job.cShifts[0].User.ID;
                }
            }

            // track who the job leads are
            Collection<int> JobLeads = new Collection<int>();

            int[] JobsToSend = SendNoticeJobs();

            // show the jobs and leads
            int Count = 0;
            foreach (Job Job in cJobs)
            {
                if (Job.JobLeadUserID == 0)
                {
                    continue;
                }

                Sling.User JobLead = Job.cShifts[0].User;
                string JobLeadName = FormatCrew(Job.cShifts[0]);

                string Link = string.Empty;
                if (JobLeadName.Length > 0)
                {
                    Link = string.Format("<a href='JobLeadJobs.aspx?userid={0}' target='JobLead'>Jobs</a>", JobLead.ID);
                }

                TableCell td;            
                TableRow tr = new TableRow();
                tblJobs.Rows.Add(tr);

                tr.Cells.Add(new TableCell() { CssClass = "JobRno", Text = Job.JobRno.ToString() });
                tr.Cells.Add(new TableCell() { CssClass = "Date", Text = Fmt.Dt(Job.Date) });
                tr.Cells.Add(new TableCell() { CssClass = "Time", Text = Fmt.Tm12Hr(Job.Time) });
                tr.Cells.Add(new TableCell() { CssClass = "Customer", Text = Job.Customer });
                tr.Cells.Add(new TableCell() { CssClass = "Crew", Text = JobLeadName });

                tr.Cells.Add(td = new TableCell() { CssClass = "Send" });
                td.Controls.Add(Job.chkSend = new CheckBox() { ID = "chkSend" + ++Count });
                Job.chkSend.Checked = (!fSendNotices || SendToJob(JobsToSend, Job.JobRno));

                tr.Cells.Add(new TableCell() { CssClass = "Link", Text = Link });
                tr.Cells.Add(td = new TableCell() { CssClass = "Notice" });
                td.Controls.Add(Job.lblNotice = new Label());

                if (fSendNotices && Job.chkSend.Checked)
                {
                    AddJobLead(JobLeads, Job.JobLeadUserID);
                }
            }

            // send the actual notices
            if (fSendNotices)
            {
                // for each job lead
                foreach (int JobLeadUserID in JobLeads)
                {
                    bool fFirst = true;
                    string Note = string.Empty;
                    string FirstName = string.Empty;
                    string LastName = string.Empty;

                    // look through the jobs for this job lead
                    // put all the jobs for this job lead
                    foreach (Job Job in cJobs)
                    {
                        if (Job.JobLeadUserID == JobLeadUserID && Job.chkSend.Checked)
                        {
                            if (!fFirst)
                            {
                                Note += "----------------------------------------\n";
                            }
                            fFirst = false;
                            Note += string.Format("Job #: {0}\\nDate: {1}\\nTime: {2}\\nCustomer: {3}\\n{4}://{5}{6}{7}?userid={8}", 
                                Job.JobRno, 
                                Fmt.Dt(Job.Date), 
                                Fmt.Tm12Hr(Job.Time), 
                                Job.Customer, 
                                Request.Url.Scheme,
                                Request.Url.Host,
                                (Request.Url.Port == 80 || Request.Url.Port == 403 ? "" : ":" + Request.Url.Port),
                                ResolveUrl("~/JobLeadJobs.aspx"),
                                JobLeadUserID);
                            Job.lblNotice.Text = "Sent";

                            FirstName = Job.cShifts[0].User.FirstName;
                            LastName = Job.cShifts[0].User.LastName;
                        }
                    }

                    string Msg = string.Format(
                        "{{" +
                            "\"content\": \"{0}\"," +
                            "\"name\": \"{1} {2}\"," +
                            "\"personas\": [{{" +
                                "\"id\": {3}," +
                                "\"type\": \"user\"" +
                            "}}]," +
                            "\"private\": true" +
                        "}}",
                        Note,
                        FirstName,
                        LastName,
                        JobLeadUserID);

                    // send the message
                    await Sling.AddToPrivateConversation(Msg);
                }
            } 
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    //protected string FindJobLead(int JobRno, out int UserID)
    //{
    //    string htmlCrew = string.Empty;
    //    UserID = 0;

    //    Job Job = FindJob(JobRno);
    //    if (Job != null)
    //    {
    //        htmlCrew = FormatCrew(Job.cShifts[0]);
    //        UserID = Job.cShifts[0].User.ID;
    //    }

    //    return htmlCrew;
    //}

    protected bool SendToJob(int[] JobsToSend, int JobRno)
    {
        bool fSend = false;

        foreach (int JobToSend in JobsToSend)
        {
            if (JobToSend == JobRno)
            {
                fSend = true;
                break;
            }
        }

        return fSend;
    }

    protected void AddJobLead(Collection<int> JobLeads, int JobLeadUserID)
    {
        bool fFound = false;
        foreach (int UserID in JobLeads)
        {
            if (UserID == JobLeadUserID)
            {
                fFound = true;
                break;
            }
        }
        if (!fFound)
        {
            JobLeads.Add(JobLeadUserID);
        }
    }

    protected void btnSelectDay_Click(object sender, EventArgs e)
    {
        rdoDay.Checked = true;
    }

    protected void btnSelectRange_Click(object sender, EventArgs e)
    {
        rdoRange.Checked = true;
    }

    protected async Task<bool> SetupSlingShifts()
    {
        Sling = new Sling();
        if (await Sling.Login(Sling.Email, Sling.Password))
        {
            await Sling.LoadUsers();
            dtEnd = dtEnd.AddDays(1);
            await Sling.LoadCalendar(dtBeg, dtEnd);

        //    // organize shifts into Jobs
        //    foreach (Sling.CalendarEvent Shift in Sling.Shifts)
        //    {
        //        Job Job = FindJob(Shift.JobRno);
        //        if (Job == null)
        //        {
        //            Job = new Job() { JobRno = Shift.JobRno };
        //            cJobs.Add(Job);
        //        }
        //        Job.cShifts.Add(Shift);
        //    }

        //    // sort shifts within each job, job lead in first shift
        //    foreach (Job Job in cJobs)
        //    {
        //        Sling.SortShiftCrew(Job.cShifts);
        //    }
        }

        return true;
    }

    protected string FormatCrew(Sling.CalendarEvent Shift)
    {
        int hr = Shift.Beg.Hour;
        string ap = "am";
        if (hr >= 12)
        {
            ap = "pm";
            if (hr > 12)
            {
                hr -= 12;
            }
        }
        string min = "0" + Shift.Beg.Minute.ToString();
        min = min.Substring(min.Length - 2, 2);

        string htmlCrew = string.Format("{0}:{1}{2} {3} {4}", hr, min, ap, Shift.User.FirstName, (Shift.User.LastName != null && Shift.User.LastName.Length > 0 ? Shift.User.LastName.Substring(0, 1) : string.Empty));

        return htmlCrew;
    }

    protected int[] SendNoticeJobs()
    {
        string[] sJobRnos = hfSendNoticeJobs.Value.Split(new char[] { ',' });
        int[] SendNoticeJobRnos = new int[sJobRnos.Length];
        int iJobRno = 0;

        foreach (string sJobRno in sJobRnos)
        {
            SendNoticeJobRnos[iJobRno++] = Str.Num(sJobRno);
        }



        return SendNoticeJobRnos;
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        ReportDates();
        fJobs = true;
        fSendNotices = true;
    }

    protected void SendNotices(int JobLeadUserID)
    {
        /*
                // clear notices
			    $(".Jobs .Notice").html("");

			    for (let iJobLead in jobLeads)
        {
            let jobLead = jobLeads[iJobLead];
            let note = "";
            let fFirst = true;
            let fSend = false;
            let aTr = [];
            for (let event of jobLead.events)
			        {
			            let tr = event.crew.parent();
			            if (!fFirst)

                        {
        note += "----------------------------------------\n";
    }
    fFirst = false;
    //note += `Job #${event.crew.data("jobrno")}\n`;
    note += `Job #: ${tr.find("td.JobRno").text()}\nDate: ${tr.find("td.Date").text()}\nTime: ${tr.find("td.Time").text()}\nCustomer: ${tr.find("td.Customer").text()}\n${location.protocol}//${location.host}/JobLeadJobs.aspx?userid=${jobLead.user.id}\n`;

			            if (tr.find("td.Send input:checked").length > 0)

                        {
        fSend = true;
        aTr.push(tr);
    }
    }

    let msg =
    {
                        //"content": `New Job\n${start}\n${$("#txtCustomer").val()}\nJob #${rno}\n${location.protocol}//${location.host}/Job.aspx?JobRno=${rno}`,
                        "content": note,
                        "name": `${jobLead.user.firstName} ${jobLead.user.lastName}`,
                        "personas": [{
                            "id": jobLead.user.id,
                            "type": "user"
                        }],
                        private: true
                    };
			        if (fSend)
			        {
			            sling.addToPrivateConversation(msg, (data) =>
			            {
			                for (let tr of aTr)
			                {
			                    tr.find(".Notice").html("Sent");
			                }
			            });
			        }
			    }
			}
            */
    }

    //protected Job FindJob(int JobRno)
    //{
    //    Job FoundJob = null;

    //    foreach (Job Job in cJobs)
    //    {
    //        if (Job.JobRno == JobRno)
    //        {
    //            FoundJob = Job;
    //            break;
    //        }
    //    }

    //    return FoundJob;
    //}

    public class Job
    {
        public int JobRno;
        public DateTime Date;
        public DateTime Time;
        public string Customer;
        public int JobLeadUserID;
        public Collection<Sling.CalendarEvent> cShifts;
        public CheckBox chkSend;
        public Label lblNotice;

        public Job()
        {
            cShifts = new Collection<Sling.CalendarEvent>();
        }
    }

    public class JobLead
    {
        public int UserID;
        public Collection<Job> cJobs;

        public JobLead()
        {
             cJobs = new Collection<Job>();
        }
    }
}