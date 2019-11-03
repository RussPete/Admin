using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utils;

public partial class JobLeadJobs : System.Web.UI.Page
{
    //protected String SlingEmail;
    //protected String SlingPassword;
    //protected String SlingOrgId;

    Sling Sling;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //WebConfig wc = new WebConfig();
            //SlingEmail = wc.Str("SlingEmail");
            //SlingPassword = wc.Str("SlingPassword");
            //SlingOrgId = wc.Str("SlingOrgId");
        }
    }

    protected async void Page_PreRender(object sender, EventArgs e)
    {
        await SetupSling();
        await ProcessShifts();
    }

    protected async Task SetupSling()
    {
        Sling = new Sling();
        if (await Sling.Login(Sling.Email, Sling.Password))
        {
            await Sling.LoadUsers();

            DateTime dtBeg = DateTime.Now;
            DateTime dtEnd = dtBeg.AddDays(14);

            await Sling.LoadCalendar(dtBeg, dtEnd);
        }
    }

    protected async Task ProcessShifts()
    {
        int UserID = Str.Num(Request.QueryString["userid"]);
        Sling.User JobLead = await Sling.FindUser(UserID);

        lblJobLead.Text = (JobLead != null ? string.Format("{0} {1}", JobLead.FirstName, JobLead.LastName) : "User Not Found");

        Collection<Job> Jobs = new Collection<Job>();
        
        // look at all the shifts this job lead is assigned to
        foreach (Sling.CalendarEvent JobLeadShift in JobLead.CalendarEvents)
        {
            if (JobLeadShift.Type != "shift")
            {
                continue;
            }

            // prepare a job
            Job Job = new Job() { JobRno = JobLeadShift.JobRno };
            foreach (Sling.CalendarEvent Shift in Sling.Shifts)
            {
                // find every assigned to this job
                if (Shift.JobRno == JobLeadShift.JobRno)
                {
                    Job.cShifts.Add(Shift);
                }
            }

            if (Job.cShifts.Count > 0)
            {
                // sort the shift
                Sling.SortShiftCrew(Job.cShifts);
                if (Job.cShifts[0].User != null && Job.cShifts[0].User.ID != 0)
                {
                    Job.JobLeadUserID = Job.cShifts[0].User.ID;
                }

                // if the job lead is assigned to this job as a job lead
                if (Job.JobLeadUserID == UserID)
                {
                    // add to the list of jobs
                    Jobs.Add(Job);
                }
            }
        }

        // show the jobs
        foreach (Job Job in Jobs)
        {
            DateTime dtBeg = Job.cShifts[0].Beg;
            phJobs.Controls.Add(new HtmlGenericControl("h3") { InnerHtml = string.Format("Job #{0}&nbsp;&nbsp;{1}", Job.JobRno, dtBeg.ToString("ddd M/d/yyyy")) });

            HtmlGenericControl Div;
            phJobs.Controls.Add(Div = new HtmlGenericControl("div"));
            Div.Attributes.Add("class", "Job");
            Div.Attributes.Add("data-jobrno", Job.JobRno.ToString());

            string Html = string.Empty;
            foreach (Sling.CalendarEvent Shift in Job.cShifts)
            {
                Html += FormatCrew(Shift);
            }
            phJobs.Controls.Add(Div = new HtmlGenericControl("div") { InnerHtml = Html });
            Div.Attributes.Add("class", "Crew");
        }
    }

    protected string FormatCrew(Sling.CalendarEvent Shift)
    {
        string htmlCrew = string.Empty;
        if (Shift.User != null)
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


            htmlCrew = string.Format("{0}:{1}{2} {3} {4}<br/>\n", hr, min, ap, Shift.User.FirstName, (Shift.User.LastName != null && Shift.User.LastName.Length > 0 ? Shift.User.LastName.Substring(0, 1) : string.Empty));
        }

        return htmlCrew;
    }


    public class Job
    {
        public int JobRno;
        public int JobLeadUserID;
        public Collection<Sling.CalendarEvent> cShifts;

        public Job()
        {
            cShifts = new Collection<Sling.CalendarEvent>();
        }
    }
}