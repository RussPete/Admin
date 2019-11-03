<%@ Page AutoEventWireup="true" CodeFile="DailyJobs.aspx.cs" Inherits="DailyJobs" Language="C#" Async="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Production Sheets</title> 
		<!-- DailyJobs.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<link href="ReportStyles.css" type="text/css" rel="stylesheet">
        <style>
            .Filter
            {
                display: inline-block;
            }
            .Filter.Job input
            {
                width: 60px;
            }
            .Filter.Customer input
            {
                width: 250px;
            }
            .Crew
            {
                float: right;
                border: solid 1px #CCC;
                padding: 0px 2px;
                width: 100px;
                font-size: 7pt;
                position: relative;
            }
            .Crew .Planning
            {
                font-weight: bold;
                text-align: center;
            }
            .Crew .Time
            {
                float: right;
            }
            .Crew .Name
            {
                float: left;
                padding-right: 5px;
            }
            .Reset
            {
                clear: both;
            }
            .Crew hr
            {
                border-color: #ccc;
                border-style: solid;
                border-width: 1px 0px 0px 0px;
                margin-left: -3px;
                margin-right: -3px;
            }

            @page 
            {
                <% if (fLetterSize) { %> 
                size: 8.5in 11in;
                <% } else { %>
                size: 8.5in 14in;
                <% } %>
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

		    var fDoSling = false;

			//----------------------------------------------------------------------------
			$(document).ready(function()
			{
				$(".JobDate").datepicker(
				{
					showOn: "both",
					buttonImage: "Images/CalendarIcon.gif",
					buttonImageOnly: true
				});

				$("#chkAM").change(function ()
				{
				    if (!this.checked)
				    {
				        $("#chkPM").prop("checked", true);
				    }
				});
				$("#chkPM").change(function ()
				{
				    if (!this.checked)
				    {
				        $("#chkAM").prop("checked", true);
				    }
                });
                $("#txtFilterDate, #txtFilterJob, #txtFilterCustomer").keyup(function ()
                {
                    FilterList();
                });
                $("#txtFilterDate").change(function ()
                {
                    FilterList();
                });


				//var crew = $(".Crew");
				//if (crew.length > 0 && fDoSling)
				//{
				//    setupSlingShift();
				//}
			});

			//----------------------------------------------------------------------------
			function FilterList()
            {
                var FilterDate     = $("#txtFilterDate").val();
                var FilterJob      = $("#txtFilterJob").val();
                var FilterCustomer = $("#txtFilterCustomer").val();

                if (FilterDate || FilterJob || FilterCustomer)
                {
                    // hide all options
                    $("#lbJobs option").css("display", "none");

                    if (FilterDate)
                    {
                        $("#lbJobs option[Date*='" + FilterDate + "']").css("display", "");
                    }
                    if (FilterJob)
                    {
                        $("#lbJobs option[Job*='" + FilterJob + "']").css("display", "");
                    }
                    if (FilterCustomer)
                    {
                        $("#lbJobs option[Customer*='" + FilterCustomer + "' i]").css("display", "");
                    }
                }
                else
                {
                    // show all options
                    $("#lbJobs option").css("display", "");
                }
			}
		
			//----------------------------------------------------------------------------
			function SameWindow()
			{
				SetTarget("form", "");
			}

			//----------------------------------------------------------------------------
			function NewWindow()
			{
				SetTarget("form", "_blank");
			}

			//----------------------------------------------------------------------------
			function OptionsSet()
			{
				SetClass("tdJobDate", (iGetChk("rdoJobDate") ? "OptionSet" : ""));
				SetClass("tdEnteredDate", (iGetChk("rdoEnteredDate") ? "OptionSet" : ""));
			}

			//----------------------------------------------------------------------------
			function OptionSet(fChecked, Element)
			{
				SetClass(Element, (fChecked ? "OptionSet" : ""));
			}

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
                    // published
			        for (let event of crewEvents)
			        {
			            if (event.user.id == prevUserId || event.status == "planning")
			            {
			                continue;
			            }
			            htmlCrew += formatCrew(event);
			            prevUserId = event.user.id;
			        }
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
			        if (htmlCrew.length > 0)
			        {
			            $(this).html(htmlCrew).show();
			        }
			    });
			}

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
		<%
if (!fReport)
{		
%>
	</head>
	<body>
		<form id="form" method="post" autocomplete="off" runat="server">
			<% Pg.Top(); %>
			<div class="FeatureMain">Production Sheets</div>
			<table cellspacing="10" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
					<td></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoDay" runat="server" text="Day" groupname="rdoPeriod" checked="True" /></td>
					<td></td>
					<td><asp:textbox id="txtDayDate" runat="server" cssclass="JobDate" /></td>
					<!--
					<td></td>
					<td></td>
					<td><asp:Button ID="btnSelectDay" Text="Select Jobs" runat="server" 
							onclick="btnSelectDay_Click" OnClientClick="$('form').attr('target', '');" /></td>
					-->							
				</tr>
				<!--
				<tr>
					<td><asp:radiobutton id="rdoWeek" runat="server" text="Week" groupname="rdoPeriod" checked="False"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtWeekDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgWeekDate" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoMonth" runat="server" text="Month" groupname="rdoPeriod" checked="False"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtMonthDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgMonthDate" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle"></asp:image></td>
				</tr>
				-->
				<tr>
					<td><asp:radiobutton id="rdoRange" runat="server" text="Range" groupname="rdoPeriod" checked="False" /></td>
					<td></td>	
					<td><asp:textbox id="txtBegDateRange" runat="server" cssclass="JobDate" /></td>
					<td align="center">thru</td>
					<td><asp:textbox id="txtEndDateRange" runat="server" cssclass="JobDate" /></td>
					<!--
					<td><asp:Button ID="btnSelectRange" Text="Select Jobs" runat="server" 
							onclick="btnSelectRange_Click" OnClientClick="$('form').attr('target', '');" /></td>
					-->
				</tr>
                <tr>
                    <td>Search By</td>
                    <td></td>
                    <td colspan="3">
                        <div>
                            <div class="Filter">
                                <div>Job Date</div>
                                <asp:TextBox ID="txtFilterDate" cssclass="JobDate" runat="server" /></div>
                            <div class="Filter Job">
                                <div>Job #</div>
                                <asp:TextBox ID="txtFilterJob" runat="server" /></div>
                            <div class="Filter Customer">
                                <div>Customer</div>
                                <asp:TextBox ID="txtFilterCustomer" runat="server" /></div>
                        </div>
                    </td>
                </tr>
				<tr>
					<td><asp:radiobutton id="rdoCustomerDate" runat="server" checked="False" groupname="rdoPeriod" text="Job" /></td>
					<td></td>
					<td colspan="3">
                        <asp:ListBox ID="lbJobs" SelectionMode="Multiple" Rows="10" Width="500" runat="server" /><asp:dropdownlist id="ddlCustomerDate" Visible="false" runat="server" /><br />
                        <asp:checkbox id="chkIncludeAll" runat="server" text="Include All" autopostback="true" OnCheckedChanged="chkIncludeAll_CheckedChanged" />
					</td>
				</tr>
				<tr>
					<td><img height="1" src="Images/Space.gif" alt="" width="1"/></td>
				</tr>
				<tr>
					<td colspan="2"></td>
					<td><asp:checkbox id="chkSummaryOnly" runat="server" text="Summary Only" /></td>
				</tr>
				<tr>
					<td>Job Time</td>
                    <td></td>
					<td><asp:checkbox id="chkAM" runat="server" text="AM" checked="true" />
                        <asp:checkbox id="chkPM" runat="server" text="PM" checked="true" />
                    </td>
				</tr>
				<tr>
					<td>Paper Size</td>
                    <td></td>
					<td>
                        <asp:RadioButton ID="rdoLetter" GroupName="PaperSize" Text="Letter" Checked="true" runat="server" />
                        <asp:RadioButton ID="rdoLegal" GroupName="PaperSize" Text="Legal" runat="server" />
                    </td>
				</tr>
<%--				
				<tr>
					<td><img height="1" src="Images/Space.gif" alt="" width="1"/></td>
				</tr>
				<tr>
					<td colspan="2"></td>
					<td><asp:checkbox id="chkHot" runat="server" text="Hot Food" checked="false" /></td>
				</tr>
				<tr>
					<td colspan="2"></td>
					<td><asp:checkbox id="chkCold" runat="server" text="Cold Food" checked="false" /></td>
				</tr>
				<tr>
					<td colspan="2"></td>
					<td><asp:checkbox id="chkAllFood" runat="server" text="All Food" checked="true" /></td>
				</tr>
--%>				
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
				<tr>
					<td align="center" colspan="5"><asp:button id="btnReport" runat="server" text="Report" OnClick="btnReport_Click" OnClientClick="$('form').attr('target', '_blank');" /></td>
				</tr>
				<tr>
					<td><img height="60" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
			</table>
			<% Pg.Bottom(); %>
		</form>
		<%
}
else
{    
	//Report();
}		
%>
	</body>
</html>
