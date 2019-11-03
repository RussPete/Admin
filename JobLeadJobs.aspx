<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JobLeadJobs.aspx.cs" Inherits="JobLeadJobs" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Job Lead Jobs</title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="ReportStyles.css" type="text/css" rel="stylesheet">
    <style>
        body
        {
            font-family: arial, sans-serif;
        }

        .RptJobBorder
        {
            width: 90%;
        }

        ul.IngredSel 
        {
            margin: 0px;
            padding-left: 25px;
        }

        .Crew
        {
            border: solid 1px #888;
            padding: 5px;
            margin: 10px auto;
            max-width: 350px;
        }

    </style>
	<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script language="javascript" src="Sling.js" type="text/javascript"></script>
	<script language="javascript" type="text/javascript">
<%--        var slingEmail = "<%=SlingEmail%>";
		var slingPassword = "<%=SlingPassword%>";
		var slingOrgId = "<%=SlingOrgId%>";--%>

	    var sling;

	    var Days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

		//----------------------------------------------------------------------------
	    $(document).ready(function ()
	    {
	        //setupSlingShift();

	        $(".Job").each((index, element) =>
	        {
	            let jobRno = $(element).data("jobrno");
	            $(element).load(`/DailyJobs.aspx?JobRno=${jobRno} .RptJobBorder`);
	        });
	    });

	    //----------------------------------------------------------------------------
	    function setupSlingShift()
	    {
	        if (slingEmail && slingPassword)
	        {
	            sling = new Sling();
	            sling.login(slingEmail, slingPassword, slingOrgId, () =>
	            {
	                sling.loadUsers(() =>
	                {
	                    var begDate = new Date(Date.now());
	                    begDate = new Date(begDate.getFullYear(), begDate.getMonth(), begDate.getDate());
	                    var endDate = new Date(begDate);
	                    endDate.setDate(endDate.getDate() + 14);

	                    sling.getCalendar(begDate, endDate, (data) =>
	                    {
	                        processShifts(data);
	                    });
	                });
	            });
	        }
	    }

	    //----------------------------------------------------------------------------
	    function processShifts(data)
	    {
	        let events = [];
	        let jobs = {};
            // raw event data from sling
	        for (let eventData of data)
	        {
	            if (eventData.type == "shift")
	            {
                    // user for event
	                let user = (eventData.user ? sling.userCollection.findUser(eventData.user.id) : null);
	                if (user)
	                {	                
                        // group events by job #
	                    let jobRno = parseJobRno(eventData.summary);
	                    let job = jobs[jobRno] = jobs[jobRno] || { events: [] };
	                    job.jobRno = jobRno;
	                    job.events.push(new CalendarEvent(eventData.id, eventData.type, eventData.fullDay, eventData.dtstart, eventData.dtend, eventData.summary, eventData.status, user));
	                }
	            }
	        }

	        let userId = parseUserId(location.search);
	        let user = sling.userCollection.findUser(userId);
	        $("form").append(`<h1>${user.firstName} ${user.lastName}</h1>`);

            // sort crew on each job
	        for (let iJob in jobs)
	        {
	            let job = jobs[iJob];
	            job.events.sort((a, b) =>    // sort by beg time, firstname, lastname
	            {
	                let rc = a.begTime.valueOf() - b.begTime.valueOf();
	                if (rc == 0)
	                {
	                    rc = a.user.firstName.toLocaleLowerCase().localeCompare(b.user.firstName.toLocaleLowerCase());
	                    if (rc == 0)
	                    {
	                        rc = a.user.lastName.toLocaleLowerCase().localeCompare(b.user.lastName.toLocaleLowerCase());
	                    }
	                }

	                return rc;
	            });

                // prepare to show the jobs where are user id is the job lead
	            if (job.events[0].user.id == userId)
	            {
	                let jobRno = job.jobRno;
	                let dt = job.events[0].begTime;
	                $("form")
                        .append(`<h3>Job #${jobRno}&nbsp;&nbsp;${Days[dt.getDay()]} ${dt.getMonth() + 1}/${dt.getDate()}/${dt.getFullYear()}</h3>`)
                        .append(`<div id="${jobRno}"/>`)
                        .append(`<div id="crew${jobRno}" class="Crew"/>`);
	                $(`#${jobRno}`).load(`/DailyJobs.aspx?JobRno=${jobRno} .RptJobBorder`);

	                let html = "";
	                for (let event of job.events)
	                {
	                    html += formatCrew(event);
	                }
                    $(`#crew${jobRno}`).html(html);
	            }
	        }
	    }

	    //----------------------------------------------------------------------------
	    function parseJobRno(notes)
	    {
	        let jobRno = 0;
	        let i = notes.indexOf("Job #");
	        if (i >= 0)
	        {
	            jobRno = parseInt(notes.substr(i + 5));
	        }

	        return jobRno;
	    }

	    //----------------------------------------------------------------------------
	    function parseUserId(str)
	    {
	        let id = 0;
	        let i = str.indexOf("userid=");
	        if (i >= 0)
	        {
	            id = parseInt(str.substr(i + 7));
	        }

	        return id;
	    }

	    //----------------------------------------------------------------------------
	    function formatCrew(event)
	    {
	        let hr = event.begTime.getHours();
	        let ap = "am";
	        if (hr >= 12)
	        {
	            ap = "pm";
	            if (hr > 12)
	                hr -= 12;
	        }
	        let min = "0" + event.begTime.getMinutes();
	        min = min.substr(min.length - 2, 2);
	        return `${hr}:${min}${ap} ${event.user.firstName} ${event.user.lastName && event.user.lastName.length > 0 ? event.user.lastName[0] : ""}<br/>\n`;
	    }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <h1><asp:Label ID="lblJobLead" runat="server" /></h1>
        <asp:PlaceHolder ID="phJobs" runat="server" />
    </form>
</body>
</html>
