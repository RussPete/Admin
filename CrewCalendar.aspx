<%@ Page AutoEventWireup="true" CodeFile="CrewCalendar.aspx.cs" Inherits="CrewCalendar"	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Crew Calendar</title> 
		<!-- CrewCalendar.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<link href="ReportStyles.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="JavaScript">
			//----------------------------------------------------------------------------
			$(document).ready(function ()
			{
				$(".JobDate").datepicker(
				{
					showOn: "both",
					buttonImage: "Images/CalendarIcon.gif",
					buttonImageOnly: true
				});
			});

			//-----------------------------------------------------------------------------
			function Init()
			{
				<%=SetFocus()%>
			}

			//-----------------------------------------------------------------------------
			function ShowJob(JobNum)
			{
				var Style = GetStyle('tblJob' + JobNum);
				var fDisplay = (Style.display == "none");
	
				SetDisplay('tblJob' + JobNum, fDisplay);
			}

			//-----------------------------------------------------------------------------
			function ShowAll()
			{
				var i;
				for (i = 0; i <= EndJobNum; i++)
				{
					SetDisplay('tblJob' + i, true);
				}
			}

			//-----------------------------------------------------------------------------
			function HideAll()
			{
				var i;
				for (i = 0; i <= EndJobNum; i++)
				{
					SetDisplay('tblJob' + i, false);
				}
			}

			//-----------------------------------------------------------------------------
			function Print()
			{
				var fOk = true;
	
				if (iGetChk("chkCrewMember") ||
					iGetChk("chkAllCrewMembers") ||
					iGetChk("chkMaster"))
				{
					fOk = true;
				}
				else
				{
					fOk = false;
					alert("Please choose at least one option.");
				}

				return fOk;
			}

		</script>
		<%
if (!fReport)
{		
%>
	</head>
	<body onload="Init();">
		<form id="form" method="post" target="_blank" autocomplete="off" runat="server">
			<% Pg.Top(); %>
			<div class="FeatureMain">Crew Calendar</div>
			<table cellspacing="5" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
					<td></td>
				</tr>
				<tr>
					<td colspan="3"><b>Choose:</b></td>
				</tr>
				<tr>
					<td><asp:checkbox id="chkCrewMember" runat="server" text="Individual"></asp:checkbox></td>
					<td></td>
					<td colspan="3">
						<asp:dropdownlist id="ddlCrewMember" runat="server"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td colspan="3"><asp:checkbox id="chkAllCrewMembers" runat="server" text="All Individuals' Schedules" checked="True"></asp:checkbox></td>
				</tr>
				<tr>
					<td colspan="3"><asp:checkbox id="chkMaster" runat="server" text="Master Schedule" checked="True"></asp:checkbox></td>
				</tr>
				<tr>
					<td colspan="5">
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><img height="1" src="Images/Space.gif" alt="" width="40"></td>
								<td><hr width="125" /></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colspan="3"><b>Batch Print:</b></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoWeek" runat="server" text="Week" groupname="rdoPeriod" checked="True"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtWeekDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgWeekDate" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoMonth" runat="server" text="Month" groupname="rdoPeriod" checked="False"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtMonthDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgMonthDate" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoRange" runat="server" text="Range" groupname="rdoPeriod" checked="False"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtBegDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgBegDateRange" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
					<td align="center">thru</td>
					<td><asp:textbox id="txtEndDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgEndDateRange" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
				<tr>
					<td align="center" colspan="5"><asp:button id="btnReport" runat="server" text="Report" OnClick="btnReport_Click"></asp:button></td>
				</tr>
				<tr>
					<td><img height="80" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
			</table>
			<% Pg.Bottom(); %>
		</form>
		<%
}
else
{
	Report();
}		
%>
	</body>
</html>
