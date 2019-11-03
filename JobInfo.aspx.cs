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

public partial class JobInfo : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected Utils.Calendar calDayDate;
	protected Utils.Calendar calWeekDate;
	protected Utils.Calendar calMonthDate;
	protected Utils.Calendar calBegDateRange;
	protected Utils.Calendar calEndDateRange;
	protected bool fReport = false;
	protected DateTime dtBeg;
	protected DateTime dtEnd;
	protected bool fNewPage;
	protected int iDtl = 0;

	private void Page_Init(object sender, System.EventArgs e)
	{
		InitCalendars();
	}

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Setup();
		}
	}

	private void InitCalendars()
	{
		calDayDate = new Utils.Calendar("DayDate", ref txtDayDate);
		calDayDate.ImageButton(imgDayDate);

		calWeekDate = new Utils.Calendar("WeekDate", ref txtWeekDate);
		calWeekDate.ImageButton(imgWeekDate);

		calMonthDate = new Utils.Calendar("MonthDate", ref txtMonthDate);
		calMonthDate.ImageButton(imgMonthDate);

		calBegDateRange = new Utils.Calendar("BegDateRange", ref txtBegDateRange);
		calBegDateRange.ImageButton(imgBegDateRange);

		calEndDateRange = new Utils.Calendar("EndDateRange", ref txtEndDateRange);
		calEndDateRange.ImageButton(imgEndDateRange);
	}

	private void Setup()
	{
		DateTime Tomorrow = DateTime.Today.AddDays(1);

		txtDayDate.Text = Fmt.Dt(Tomorrow);
		txtWeekDate.Text = Fmt.Dt(Tomorrow);
		txtMonthDate.Text = Fmt.Dt(Tomorrow.AddDays(1 - Tomorrow.Day).AddMonths(1));
		txtBegDateRange.Text =
		txtEndDateRange.Text = txtWeekDate.Text;

		txtDayDate.Attributes.Add("onChange", "iSetChk('rdoDay', true);");
		txtWeekDate.Attributes.Add("onChange", "iSetChk('rdoWeek', true);");
		txtMonthDate.Attributes.Add("onChange", "iSetChk('rdoMonth', true);");
		txtBegDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
		txtEndDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
	}

	protected void btnReport_Click(object sender, System.EventArgs e)
	{
		if (rdoDay.Checked)
		{
			dtBeg = Convert.ToDateTime(txtDayDate.Text);
			dtEnd = dtBeg;
		}

		if (rdoWeek.Checked)
		{
			dtBeg = Convert.ToDateTime(txtWeekDate.Text);
			dtEnd = dtBeg.AddDays(6);
		}
		if (rdoMonth.Checked)
		{
			dtBeg = Convert.ToDateTime(txtMonthDate.Text);
			dtBeg = dtBeg.AddDays(1 - dtBeg.Day);
			dtEnd = dtBeg.AddMonths(1).AddDays(-1);
		}
		if (rdoRange.Checked)
		{
			dtBeg = Convert.ToDateTime(txtBegDateRange.Text);
			dtEnd = Convert.ToDateTime(txtEndDateRange.Text);
		}

		fReport = true;
	}

	protected void Report()
	{
        string Html =
            "</head>\n<body class='RptBody'>\n" +
            WebPage.Table("align='center' width='100%'") +
            WebPage.Tabs(1) + "<tr>\n" +
            WebPage.Tabs(2) + "<td align='center'>\n" +
            WebPage.Tabs(3) + WebPage.Div((rdoPrivate.Checked ? "Private" : "Corporate") + " Jobs", "center", "RptTitle") +
            WebPage.Tabs(5) + WebPage.Div(Fmt.Dt(dtBeg) + " - " + Fmt.Dt(dtEnd), "center", "RptSubTitle") +
            WebPage.Tabs(2) + "</td>\n" +
            WebPage.Tabs(1) + "</tr>\n" +
            WebPage.Tabs(1) + "<tr>\n" +
            WebPage.Tabs(2) + WebPage.Space(1, 10) +
            WebPage.Tabs(1) + "</tr>\n" +
            WebPage.TableEnd();

        Html +=
            WebPage.Table("align='center'") +
            WebPage.Tabs(1) + "<tr>\n" +
            WebPage.Tabs(2) + "<td></td>\n" +           // JobRno
            WebPage.Tabs(2) + WebPage.Space(15, 1) +
            WebPage.Tabs(2) + "<td></td>\n" +           // JobDate
            WebPage.Tabs(2) + WebPage.Space(15, 1) +
            WebPage.Tabs(2) + "<td></td>\n" +           // JobDesc
            WebPage.Tabs(2) + WebPage.Space(15, 1) +
            WebPage.Tabs(2) + "<td></td>\n" +           // total servings
            WebPage.Tabs(1) + "</tr>\n" +
            WebPage.Tabs(1) + "<tr>\n" +
            WebPage.Tabs(2) + "<th>Job #</th>\n" +          // JobRno
            WebPage.Tabs(2) + "<th></th>\n" +
            WebPage.Tabs(2) + "<th>Date</th>\n" +           // JobDate
            WebPage.Tabs(2) + "<th></th>\n" +
            WebPage.Tabs(2) + "<th>Description</th>\n" +    // JobDesc
            WebPage.Tabs(2) + "<th></th>\n" +
            WebPage.Tabs(2) + "<th>Servings</th>\n" +       // total servings
            WebPage.Tabs(1) + "</tr>\n";

        Response.Write(Html);
        string JobDesc = "(Case When Len(j.JobDesc) <> 0 Then j.JobDesc Else Coalesce(u.Name, c.Name, '') + ' - ' + j.EventType End)";
        string Sql = String.Format(
            "Select j.JobRno, JobDate, " + JobDesc + " as JobDesc, " +
            "(IsNull(j.NumMenServing, 0) + IsNull(j.NumWomenServing, 0) + IsNull(j.NumChildServing, 0)) As NumServings " +
            "From mcJobs j " +
            "Left Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers u on c.CustomerRno = u.CustomerRno " +
            "Where j.JobType = {0} And " +
            "j.JobDate Between {1} And {2} And " +
            "IsNull(j.ProposalFlg, 0) = 0 And " +
			"j.CancelledDtTm Is Null " +
            "Order By j.JobDate, j.JobRno, " + JobDesc,
            DB.Put(rdoPrivate.Checked ? "Private" : "Corporate"),
			DB.PutDtTm(dtBeg),
			DB.PutDtTm(dtEnd));

		try
		{
			DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                int JobRno = DB.Int32(dr["JobRno"]);
                string JobDate = Fmt.Dt(DB.DtTm(dr["JobDate"]));
                string Desc = DB.Str(dr["JobDesc"]);
                string Servings = Fmt.Num(DB.Int32(dr["NumServings"]));

                Html =
                    WebPage.Tabs(1) + "<tr>\n" +
                    WebPage.Tabs(2) + "<td align='right'>" + JobRno + "</td>\n" +
                    WebPage.Tabs(2) + "<td></td>\n" +
                    WebPage.Tabs(2) + "<td>" + JobDate + "</td>\n" +
                    WebPage.Tabs(2) + "<td></td>\n" +
                    WebPage.Tabs(2) + "<td>" + Desc + "</td>\n" +
                    WebPage.Tabs(2) + "<td></td>\n" +
                    WebPage.Tabs(2) + "<td align='right'>" + Servings + "</td>\n" +
                    WebPage.Tabs(1) + "</tr>\n";

                Response.Write(Html);
            }
        }
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

        Html = WebPage.TableEnd();
        Response.Write(Html);
        Response.End();
    }
}