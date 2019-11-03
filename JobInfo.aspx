<%@ Page AutoEventWireup="true" CodeFile="JobInfo.aspx.cs" Inherits="JobInfo"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title>Marvellous Catering - Job Info</title> 
		<!-- JobInfo.aspx -->
		<!-- Copyright (c) 2019 PeteSoft, LLC. All Rights Reverved -->
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
		<script language="javascript">

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
			function ShowDtl(iBeg, iEnd)
			{
				var Style = GetStyle('Dtl' + iBeg);
				var fDisplay = (Style.display == "none");
	
				var i;
				for (i = iBeg; i <= iEnd; i++)
				{
					SetDisplay('Dtl' + i, fDisplay);
				}
			}

			//-----------------------------------------------------------------------------
			function ShowAll()
			{
				var i;
				for (i = 0; i <= iEndDtl; i++)
				{
					SetDisplay('Dtl' + i, true);
				}
			}

			//-----------------------------------------------------------------------------
			function HideAll()
			{
				var i;
				for (i = 0; i <= iEndDtl; i++)
				{
					SetDisplay('Dtl' + i, false);
				}
			}

		</script>
		<%
if (!fReport)
{		
%>
	</head>
	<body>
		<form id="form" method="post" target="_blank" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<div class="FeatureMain">Job Info</div>
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
					<td><asp:radiobutton id="rdoWeek" runat="server" checked="True" groupname="rdoPeriod" text="Week"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtWeekDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgWeekDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoMonth" runat="server" checked="False" groupname="rdoPeriod" text="Month"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtMonthDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgMonthDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoRange" runat="server" checked="False" groupname="rdoPeriod" text="Range"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtBegDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgBegDateRange" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
					<td align="center">thru</td>
					<td><asp:textbox id="txtEndDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgEndDateRange" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
				<tr>
					<td>Job Type</td>
					<td></td>
					<td colspan="3">
                        <asp:RadioButton ID="rdoPrivate" Text="Private" GroupName="JobType" Checked="true" runat="server" />
                        <asp:RadioButton ID="rdoCorporate" Text="Corporate" GroupName="JobType" runat="server" />
					</td>
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
