using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class Availability : System.Web.UI.Page
{
    const DayOfWeek FirstDay = DayOfWeek.Monday;
    const string cnAvailability = "availability";
    const string cnLeave = "leave";

    protected async void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime dtBeg = DateTime.Today;
            dtBeg = dtBeg.AddDays(1 - (int)dtBeg.DayOfWeek);
            txtDate.Text = dtBeg.ToString("M/d/yyyy");

            try
            {
                await GetAvailability();
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex);
                Response.Write(Err.Html());
            }
        }
    }

    protected async void txtDate_TextChanged(object sender, EventArgs e)
    {
        await GetAvailability();
    }

    protected async Task<bool> GetAvailability()
    {
        try
        {
            DateTime dtBeg = FindBegDate(DateTime.Parse(txtDate.Text));
            DateTime dtEnd = dtBeg.AddDays(6);

            Sling Sling = new Sling();

            if (await Sling.Login(Sling.Email, Sling.Password))
            {
                await Sling.LoadUsers();
                await ClearAndLoad(Sling, dtBeg, dtEnd);
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex);
            Response.Write(Err.Html());
        }

        return true;
    }

    protected DateTime FindBegDate(DateTime dt)
    {
        return dt.AddDays(FirstDay - dt.DayOfWeek);
    }

    protected async Task<bool> ClearAndLoad(Sling Sling, DateTime dtBeg, DateTime dtEnd)
    {
        try
        {
            Sling.ClearCalendar();
            await Sling.LoadCalendar(dtBeg, dtEnd);
            //await Sling.LoadAvailability(dtBeg, dtEnd);
            Render(Sling, dtBeg, dtEnd);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex);
            Response.Write(Err.Html());
        }

        return true;
    }

    protected void Render(Sling Sling, DateTime dtBeg, DateTime dtEnd)
    {
        string[] GroupNames = new string[]
        {
            "Female Service Crew",
            "Male Service Crew"
            //"Hot Kitchen",
            //"Cold Kitchen",
            //"Molly's"
        };
        string[] DayNames = new string[]
        {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"
        };

        try
        {
            foreach (string GroupName in GroupNames)
            {
                Sling.Group Group = Sling.FindGroupByName(GroupName);

                if (Group != null)
                {
                    HtmlGenericControl h = new HtmlGenericControl("h1") { InnerText = string.Format("{0} - Times Unavailable", GroupName) };
                    phSchedule.Controls.Add(h);

                    HtmlTable tbl = new HtmlTable() { };
                    phSchedule.Controls.Add(tbl);

                    HtmlTableRow tr = new HtmlTableRow();
                    tbl.Controls.Add(tr);

                    // headers
                    HtmlTableCell th = new HtmlTableCell("th") { InnerText = "Employee" };
                    tr.Controls.Add(th);

                    DateTime dt = dtBeg;
                    for (int iDay = 0; iDay < 7; iDay++)
                    {
                        tr.Controls.Add(new HtmlTableCell("th") { InnerHtml = string.Format("{0}<br/> {1}/{2}", DayNames[(int)dt.DayOfWeek], dt.Month, dt.Day) });
                        dt = dt.AddDays(1);
                    }

                    // employees
                    Sling.SortUsers(Group.Users);
                    foreach (Sling.User User in Group.Users)
                    {
                        tr = new HtmlTableRow();
                        tbl.Controls.Add(tr);

                        HtmlTableCell tc = new HtmlTableCell() { InnerText = string.Format("{0} {1}", User.FirstName, (User.LastName != null && User.LastName.Length > 0 ? User.LastName[0].ToString() : string.Empty)) };
                        tr.Controls.Add(tc);

                        // events for the employee
                        dt = dtBeg;
                        for (int iDay = 0; iDay < 7; iDay++)
                        {
                            // one day at a time
                            DateTime dtEndOfDay = dt.AddDays(1);

                            // look for events on the day
                            tc = new HtmlTableCell();
                            tr.Controls.Add(tc);

                            string Desc = "";
                            bool fAllDay = false;

                            foreach (Sling.CalendarEvent Event in User.CalendarEvents)
                            {
                                if (Event.Type == cnAvailability && dt.Day == Event.Beg.Day ||
                                    Event.Type == cnLeave &&
                                    (dt <= Event.Beg && Event.Beg <= dtEndOfDay ||
                                     dt <= Event.End && Event.End <= dtEndOfDay ||
                                     Event.Beg < dt && dtEndOfDay < Event.End))
                                {
                                    if (Event.FullDay || Event.Beg < dt && dtEndOfDay < Event.End)
                                    {
                                        fAllDay = true;
                                        Desc = "All Day";
                                    }
                                    else
                                    {
                                        if (!fAllDay)
                                        {
                                            Desc += string.Format("{0} - {1}<br/>",
                                                        (dt <= Event.Beg ? TimeFormat(Event.Beg) : "Midnight"),
                                                        (Event.End <= dtEndOfDay ? TimeFormat(Event.End) : "Midnight"));
                                        }
                                    }
                                }
                            }
                            tc.InnerHtml = Desc;
                            dt = dt.AddDays(1);
                        }
                    }
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex);
            Response.Write(Err.Html());
        }
    }

    protected string TimeFormat(DateTime tm)
    {
        int Hour = tm.Hour;
        int Min = tm.Minute;
        string sMin;
        string sAP;

        if (Hour < 12)
        {
            sAP = "am";
        }
        else
        {
            sAP = "pm";
            Hour -= 12;
        }
        if (Hour == 0)
        {
            Hour = 12;
        }
        sMin = "0" + Min.ToString();
        sMin = sMin.Substring(sMin.Length - 2, 2);

        string Fmt;
        if (Hour == 12 && Min == 0)
        {
            Fmt = (sAP == "am" ? "Midnight" : "Noon");
        }
        else
        {
            Fmt = string.Format("{0}:{1}{2}", Hour, sMin, sAP);
        }

        return Fmt;
    }
}