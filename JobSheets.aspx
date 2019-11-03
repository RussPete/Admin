<%@ Page AutoEventWireup="true" CodeFile="JobSheets.aspx.cs" Inherits="JobSheets"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Job Sheets</title> 
		<!-- JobSheets.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<link href="Styles.css" type="text/css" rel="stylesheet" />
		<link href="ReportStyles.css" type="text/css" rel="stylesheet" />
		<style type="text/css">
			.CustTitle { WIDTH: 110px }
			.CustSpace { width: 8px; }
			.CustValue { width: 175px; }
			.CustShortValue { width: 75px; }
			.ul { BORDER-BOTTOM: #999999 1px solid }
			.BlankBody { PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px }
			.BlankBody, .BlankBody td, .JobBody, .JobBody td { FONT-SIZE: 10pt }
			.jsJobInfo .JobMainItem { font-weight: bold; font-size: 16pt; line-height: normal; }
            .jsJobInfo .JobMainSubItem { font-weight: bold; font-size: 11pt; line-height: normal; }
            .jsJobInfo td { line-height: 1.8em; }
			.jsJobInfo td.AttentionItem { font-weight: bold; }
            img.CheckBox { vertical-align: text-top; }
			TD.BlankSmall { FONT-SIZE: 10pt }
			TD.ulBlankSmall { FONT-SIZE: 7pt; BORDER-BOTTOM: #999999 1px solid }
			.BlankSec, td.BlankSec { FONT-WEIGHT: bold; FONT-SIZE: 12pt; font-style: italic; }
			.BlankSrv { BORDER-RIGHT: #999999 1px solid; BORDER-TOP: #999999 1px solid; BORDER-LEFT: #999999 1px solid; BORDER-BOTTOM: #999999 1px solid }
			.BlankSrv TD { FONT-SIZE: 7.5pt }
            .RptJobDate { font-weight: bold; font-style: italic; font-size: 16pt; }
            td.RptJobDate { font-size: 16pt; line-height: normal; }
		    td.JobSheetDate { font-size: 16pt; font-weight: bold; text-align: left; line-height: normal; }
            td.left { text-align: left; }
			table.JobMisc { width: 100%; /*padding-top: 5px;*/ }
			table.JobMisc td { font-size: 8pt; }
            table.PaymentType { padding-bottom: 5px; }
			table.PaymentType td { font-size: 8pt; line-height: 1.2em; }
			table.PaymentType tr td:first-child { text-align: right; width: 50px; padding-right: 5px; }
			table.PaymentType img { vertical-align: bottom; }
			table.Services { border-top: #999999 1px solid; border-left: #999999 1px solid; border-bottom: #999999 1px solid; }
			.Services td { border-right: #999999 1px solid; font-size: 8pt; line-height: 1.2em; padding-left: 3px; padding-right: 3px; }
			.Services td td { border-right: #999999 0px solid; padding-left: 0px; padding-right: 8px; }
			.Services td td.Note { border-right: #999999 0px solid; padding-left: 0px; padding-right: 0px; }
			.BlankLinens { margin-top: 0px; /* BORDER-RIGHT: #999999 1px solid; BORDER-TOP: #999999 1px solid; BORDER-LEFT: #999999 1px solid; BORDER-BOTTOM: #999999 1px solid */}
			/*td.BlankLinens, .BlankLinens td { FONT-SIZE: 10pt }*/
			/*td.BlankLinen { width: 300px; Border-Bottom: #999999 1px solid; }*/
			table.BlankLinens { border-collapse: collapse; }
			table.BlankLinens td { border: solid 1px #999999; FONT-SIZE: 7.5pt; line-height: 1.6em; padding-right: 2px; }
			.BlankDishes { BORDER-RIGHT: #999999 1px solid; BORDER-TOP: #999999 1px solid }
			.BlankDishes TD { PADDING-LEFT: 2px; padding-right: 2px; FONT-SIZE: 7pt; line-height: 1.4em; BORDER-LEFT: #999999 1px solid; BORDER-BOTTOM: #999999 1px solid }
			.BlankDishes table, .BlankDishes table td { border-width: 0px; }
            .BlankDishes img { vertical-align: text-bottom; }
			.BlankSupply { BORDER-RIGHT: #999999 1px solid; BORDER-TOP: #999999 1px solid }
			.BlankSupply TD { PADDING-LEFT: 2px; padding-right: 2px; FONT-SIZE: 7pt; line-height: 1.4em; BORDER-LEFT: #999999 1px solid; BORDER-BOTTOM: #999999 1px solid }
			.BlankSupply table, .BlankSupply table td { border-width: 0px; }
            .Supplies { float: left; width: 169px; border: solid 1px #999; margin: 0px 0px 0px -1px; font-size: 7pt; }
            .Supplies:first-child { margin-left: 0px; }
            .Supply { border-top: solid 1px #999; padding: 2px 0px; }
            .Supply:first-child { border-top-width: 0px; }
            .Supply .Qty { width: 15px; text-align: right; display: inline-block; }
            .Supply .Item { }
            .Supply .NoQtyItem { color: #DDDDDD; }
            .Supply .Note { font-style: italic; font-size: smaller; }
			TD.BlankSmTitle { FONT-WEIGHT: bold; FONT-SIZE: 8pt; PADDING-TOP: 0px }
			center.JobSubTitle, td.JobSubTitle { font-size: 12pt; font-weight: bold; }
			table.BlankCrew { width: 100%; padding-top: 0px; line-height: 2.1em; font-size: 8pt; }
			table .Crew td { font-size: 9pt; text-align: left; line-height: 1.7em; }
			.CrewSpace { width: 5px; height: 1px; }
			.SecBreak { width: 1px; height: 10px; }
			.BlankService { BORDER-BOTTOM: #999999 1px solid; width: 100px; }
			.QtyBox { border: solid 1px #999999; height: 14px; width: 40px; }
		</style>
		
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
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

			function SameWindow()
			{
				SetTarget("form", "");
			}

			function NewWindow()
			{
				SetTarget("form", "_blank");
			}

			function OptionsSet()
			{
				SetClass("tdJobDate", (iGetChk("rdoJobDate") ? "OptionSet" : ""));
				SetClass("tdEnteredDate", (iGetChk("rdoEnteredDate") ? "OptionSet" : ""));
			}

			function OptionSet(fChecked, Element)
			{
				SetClass(Element, (fChecked ? "OptionSet" : ""));
			}			
			
		</script>
		<%
if (!fReport)
{		
%>
	</head>
	<body>
		<form id="form" method="post" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<div class="FeatureMain">Job Sheets</div>
			<table cellspacing="10" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
					<td></td>
				</tr>
				<tr>
					<td colspan="5">
						<table cellspacing="3" cellpadding="0" align="center" border="0" style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid">
							<tr>
								<td id="tdJobDate" class="OptionSet"><asp:radiobutton id="rdoJobDate" runat="server" groupname="rdoDate" text="Use Job Date" checked="true"></asp:radiobutton>&nbsp;</td>
								<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
								<td id="tdEnteredDate"><asp:radiobutton id="rdoEnteredDate" runat="server" groupname="rdoDate" text="Use Entered Date" Visible="false"></asp:radiobutton>&nbsp;</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoDay" runat="server" checked="True" groupname="rdoPeriod" text="Day"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtDayDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgDayDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoWeek" runat="server" checked="False" groupname="rdoPeriod" text="Week"></asp:radiobutton></td>
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
					<td colspan="2"></td>
					<td colspan="3">
						<table border="0" cellspacing="0" cellpadding="0">
							<tr>
								<td id="tdNotPrintedOnly" class="OptionSet"><asp:checkbox id="chkNotPrintedOnly" runat="server" checked="True" text="Not Printed Only"></asp:checkbox>&nbsp;</td>
								<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
								<td id="tdSummaryOnly"><asp:checkbox id="chkSummaryOnly" runat="server" text="Summary only"></asp:checkbox>&nbsp;</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td colspan="2"></td>
					<td align="center" colspan="3">
						<hr width="50%">
					</td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoCustomerDate" runat="server" checked="False" groupname="rdoPeriod" text="Job"></asp:radiobutton></td>
					<td></td>
					<td colspan="3"><asp:dropdownlist id="ddlCustomerDate" runat="server"></asp:dropdownlist><asp:checkbox id="chkIncludeAll" runat="server" text="Include All" autopostback="true" OnCheckedChanged="chkIncludeAll_CheckedChanged"></asp:checkbox></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoBlank" runat="server" groupname="rdoPeriod" text="Blank"></asp:radiobutton></td>
					<td></td>
					<td><asp:dropdownlist id="ddlCopies" runat="server">
							<asp:listitem value="1">1</asp:listitem>
							<asp:listitem value="2">2</asp:listitem>
							<asp:listitem value="3">3</asp:listitem>
							<asp:listitem value="4">4</asp:listitem>
							<asp:listitem value="5">5</asp:listitem>
							<asp:listitem value="10">10</asp:listitem>
							<asp:listitem value="15">15</asp:listitem>
							<asp:listitem value="20">20</asp:listitem>
							<asp:listitem value="25">25</asp:listitem>
							<asp:listitem value="30">30</asp:listitem>
						</asp:dropdownlist>Copies</td>
				</tr>
			</table>
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
				<tr>
					<td align="center"><asp:button id="btnReport" runat="server" text="Report" OnClick="btnReport_Click"></asp:button></td>
				</tr>
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
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
