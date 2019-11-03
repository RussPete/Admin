<%@ Page AutoEventWireup="true" CodeFile="JobsEntered.aspx.cs" Inherits="JobsEntered"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Job Entered</title> 
		<!-- JobSheets.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="ReportStyles.css" type="text/css" rel="stylesheet">
		<style>
			.CustTitle { WIDTH: 80px }
			.ul { BORDER-BOTTOM: #999999 1px solid }
			.BlankBody { PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px }
			.BlankBody { FONT-SIZE: 12pt }
			.BlankBody TD { FONT-SIZE: 12pt }
			TD.BlankSmall { FONT-SIZE: 10pt }
			TD.ulBlankSmall { FONT-SIZE: 8pt; BORDER-BOTTOM: #999999 1px solid }
			.BlankSec { FONT-WEIGHT: bold; FONT-SIZE: 14pt }
			.BlankSrv { BORDER-RIGHT: #999999 1px solid; BORDER-TOP: #999999 1px solid; BORDER-LEFT: #999999 1px solid; BORDER-BOTTOM: #999999 1px solid }
			.BlankSrv TD { FONT-SIZE: 10pt }
			.BlankSupply { BORDER-RIGHT: #999999 1px solid; BORDER-TOP: #999999 1px solid }
			.BlankSupply TD { PADDING-LEFT: 3px; FONT-SIZE: 9pt; BORDER-LEFT: #999999 1px solid; BORDER-BOTTOM: #999999 1px solid }
			TD.BlankSmTitle { FONT-WEIGHT: bold; FONT-SIZE: 10pt; PADDING-TOP: 0px }
		</style>
		<script language="javascript" src="Calendar.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="javascript">
			function SameWindow()
				{
				SetTarget("form", "");
			}

			function NewWindow()
			{
				SetTarget("form", "_blank");
			}
			
		</script>
		<%
if (!fReport)
{		
%>
		<%=calDayDate.JavaScript()%>
		<%=calWeekDate.JavaScript()%>
		<%=calMonthDate.JavaScript()%>
		<%=calBegDateRange.JavaScript()%>
		<%=calEndDateRange.JavaScript()%>
	</head>
	<body>
		<form id="form" method="post" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<div class="FeatureMain">Jobs Entered</div>
			<table cellspacing="10" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
					<td></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoDay" runat="server" checked="True" groupname="rdoPeriod" text="Day"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtDayDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgDayDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoWeek" runat="server" checked="False" groupname="rdoPeriod" text="Week"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtWeekDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgWeekDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoMonth" runat="server" checked="False" groupname="rdoPeriod" text="Month"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtMonthDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgMonthDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoRange" runat="server" checked="False" groupname="rdoPeriod" text="Range"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtBegDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgBegDateRange" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif"></asp:image></td>
					<td align="center">thru</td>
					<td><asp:textbox id="txtEndDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgEndDateRange" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif"></asp:image></td>
				</tr>
			</table>
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
				<tr>
					<td align="center"><asp:button id="btnReport" runat="server" text="Report"></asp:button></td>
				</tr>
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
			</table>
			<% Pg.Bottom(); %>
		</form>
		<%=calDayDate.DefCalendar()%>
		<%=calWeekDate.DefCalendar()%>
		<%=calMonthDate.DefCalendar()%>
		<%=calBegDateRange.DefCalendar()%>
		<%=calEndDateRange.DefCalendar()%>
		<%
}
else
{
	Report();
}		
%>
	</body>
</html>
