<%@ Page AutoEventWireup="true" CodeFile="JobLeads.aspx.cs" Inherits="JobLeads" Async="true" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Production Sheets</title> 
		<!-- JobLeads.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<link href="ReportStyles.css" type="text/css" rel="stylesheet">
        <style>
            table.Jobs td
            {
                padding: 0px 10px;
            }

            .Send
            {
                text-align: center;
            }
        </style>
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
        <script language="javascript" src="Sling.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
<%--            var slingEmail = "<%=SlingEmail%>";
		    var slingPassword = "<%=SlingPassword%>";
		    var slingOrgId = "<%=SlingOrgId%>";--%>

			//----------------------------------------------------------------------------
			$(document).ready(function()
			{
				$(".JobDate").datepicker(
				{
					showOn: "both",
					buttonImage: "Images/CalendarIcon.gif",
					buttonImageOnly: true
				});

				//var crew = $(".Crew");
				//if (crew.length > 0)
				//{
				//    setupSlingShift();
				//}

				$(".Clear").click(() =>
				{
				    $("input:checkbox").prop("checked", false);
				});

				$(".All").click(() =>
				{
				    $("input:checkbox").prop("checked", true);
				});

				$("#btnSend").click(() =>
				{
				    sendClicked();
			    });

				$(".Send input").click(() =>
				{
				    //sendClicked();
				});
			});
		
			var sling = null;

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
			                var begDate = new Date($("#hfBegDate").val());
			                var endDate = new Date($("#hfEndDate").val());
			                endDate.setDate(endDate.getDate() + 1);

			                sling.getCalendar(begDate, endDate, (data) =>
			                {
			                    processShifts(data);
			                });
			            });
			        });
			    }
			}

			var jobLeads = {};

		    //----------------------------------------------------------------------------
			function processShifts(data)
			{
			    let events = [];
			    for (let eventData of data)
			    {
			        let user = (eventData.user ? sling._userCollection.findUser(eventData.user.id) : null);
			        events.push(new CalendarEvent(eventData.id, eventData.type, eventData.fullDay, eventData.dtstart, eventData.dtend, eventData.summary, eventData.status, user));
			    }

			    $(".Crew").each(function(index) 
			    {
			        let jobRno = $(this).data("jobrno");
			        let crewEvents = [];
			        for (let event of events)
			        {
			            if (event.summary.indexOf("Job #" + jobRno) >= 0)
			            {
			                if (event.user)
			                {
			                    crewEvents.push(event);
			                }
			            }
			        }
			        crewEvents.sort((a, b) =>    // sort by beg time, firstname, lastname
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
			        let htmlCrew = "";
			        let prevUserId = 0;
			        let userId = 0;
                    // published
			        for (let event of crewEvents)
			        {
                        /*
			            if (event.user.id == prevUserId || event.status == "planning")
			            {
			                continue;
			            }
                        */
			            userId = event.user.id;
			            let jobLead = jobLeads[event.user.id] = jobLeads[event.user.id] || { user: event.user, events: [] };
			            jobLead.events.push({ event: event, crew: $(this) });
			            htmlCrew += formatCrew(event);
			            break;
			            //prevUserId = event.user.id;
			        }
                    /*
			        let htmlPlan = "";
			        prevUserId = 0;
			        // planning
			        for (let event of crewEvents)
			        {
			            if (event.user.id == prevUserId || event.status != "planning")
			            {
			                continue;
			            }
			            htmlPlan += formatCrew(event);
			            prevUserId = event.user.id;
			        }
			        if (htmlPlan.length > 0)
			        {
			            if (htmlCrew.length > 0)
			            {
			                htmlCrew += "<hr>\n";
			            }
			            htmlCrew += "<b>Planning</b><br/>\n" + htmlPlan;
			        }
                    */
			        if (htmlCrew.length > 0)
			        {
			            $(this).html(htmlCrew).show();
			            $(this).parent().find("td.Link").html(`<a href="/JobLeadJobs.aspx?userid=${userId}" target="JobLead">Jobs</a>`);
			        }
			    });
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

		    //----------------------------------------------------------------------------
			function sendNotices()
			{
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

			//----------------------------------------------------------------------------
			function sendClicked()
			{
			    let Jobs = "";
			    $("td.Send input:checked").each((index, element) =>
			    {
			        let tr = $(element).parent().parent();
			        Jobs += "," + tr.find("td.JobRno").text();
			    });
			    Jobs = Jobs.substr(1);
			    $("#hfSendNoticeJobs").val(Jobs);
			}
		</script>
	</head>
	<body>
		<form id="form" method="post" autocomplete="off" runat="server">
			<% Pg.Top(); %>
			<div class="FeatureMain">Job Leads</div>
			<table cellspacing="10" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
					<td></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoDay" runat="server" text="Day" groupname="rdoPeriod" checked="True"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtDayDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgDayDate" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoRange" runat="server" text="Range" groupname="rdoPeriod" checked="False"></asp:radiobutton></td>
					<td></td>	
					<td><asp:textbox id="txtBegDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgBegDateRange" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
					<td align="center">thru</td>
					<td><asp:textbox id="txtEndDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgEndDateRange" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1" /></td>
				</tr>
                <tr>
                    <td align="center" colspan="5"><asp:button id="btnFindJobLeads" runat="server" text="Find Job Leads" OnClick="btnFindJobLeads_Click" ></asp:button></td>
                </tr>
                <tr>
                    <td><img height="20" src="Images/Space.gif" alt="" width="1" /></td>
                </tr>
<%
    if (fJobs)
    {
%>
                <tr>
                    <td colspan="5">
                        <asp:Table ID="tblJobs" CssClass="Jobs" runat="server">
                            <asp:TableHeaderRow><asp:TableCell ColumnSpan="5"/><asp:TableCell><span class="TextLink Small Clear">Clear</span>&nbsp;&nbsp;<span class="TextLink Small All">All</span></asp:TableCell></asp:TableHeaderRow>
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell>Job #</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Date</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Time</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Customer</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Job Lead</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Send</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Link</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Notice</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                        </asp:Table>
                    </td>
                </tr>
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1" /></td>
				</tr>
                <tr>
                    <td align="center" colspan="5"><asp:Button ID="btnSend" Text="Send Notices" runat="server" OnClick="btnSend_Click" /></td>
                </tr>
                <tr>
                    <td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
                </tr>
<%
    }
%>

            </table>
            <asp:HiddenField ID="hfBegDate" runat="server" />
            <asp:HiddenField ID="hfEndDate" runat="server" />
            <asp:HiddenField ID="hfSendNoticeJobs" runat="server" />

            <% Pg.Bottom(); %>
		</form>
	</body>
</html>
