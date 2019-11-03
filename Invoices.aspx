<%@ Page AutoEventWireup="true" CodeFile="Invoices.aspx.cs" Inherits="Invoices" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Invoices</title> 
		<!-- Invoices.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet" />
		<link href="ReportStyles.css" type="text/css" rel="stylesheet" />
		<style type="text/css">
			.PmtTotal { WIDTH: 95px; }
			.Underline { BORDER-BOTTOM: black 1px solid; }
            .EventInfo { margin: 10px 0px; }
            .EventInfo div {line-height: 1.5em; }
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

			//-----------------------------------------------------------------------------
			function EnableInvoice()
			{
				iSetDis("btnReport", false);
				iSetDis("btnJobsNotInv", true);
			}

			//-----------------------------------------------------------------------------
			function EnableJobsNotInv()
			{
				iSetDis("btnReport", true);
				iSetDis("btnJobsNotInv", false);
			}

			//-----------------------------------------------------------------------------
			function SameWindow()
			{
				SetTarget("form", "");
			}

			//-----------------------------------------------------------------------------
			function NewWindow()
			{
				SetTarget("form", "_blank");
			}

			//-----------------------------------------------------------------------------
			function Print()
			{
				var fOk = true;
	
				if (!iGetChk("chkCustomerCopy") &&
					!iGetChk("chkFileCopy"))
				{
					fOk = false;
					alert("Check either Customer or File Copy before printing.");
				}
	
				if (fOk)
				{
					NewWindow();
				}
	
				return fOk;
			}

			//-----------------------------------------------------------------------------
			function OptionSet(fChecked, Element)
			{
				SetClass(Element, (fChecked ? "OptionSet" : ""));
			}

			//-----------------------------------------------------------------------------
			function Init()
			{
				EnableInvoice();
			}

		</script>
		<%
if (!fReport)
{		
%>
	</head>
	<body onload="Init();">
		<form id="form" method="post" autocomplete="off" runat="server">
			<% Pg.Top(); %>
			<div class="FeatureMain">Invoices</div>
			<table cellspacing="10" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1" /></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="1" /></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoDay" runat="server" text="Day" groupname="rdoPeriod" checked="True"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtDayDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgDayDate" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoWeek" runat="server" text="Week" groupname="rdoPeriod" checked="False"></asp:radiobutton></td>
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
					<td>
						<asp:textbox id="txtBegDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgBegDateRange" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image>
						&nbsp;&nbsp;thru&nbsp;&nbsp;
						<asp:textbox id="txtEndDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgEndDateRange" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image>
					</td>
				</tr>
				<tr>
					<td colspan="2"></td>
					<td>
						<span id="spNotInvoicedOnly" class="OptionSet"><asp:checkbox id="chkNotInvoicedOnly" runat="server" text="Not Invoiced Only" checked="True"></asp:checkbox>&nbsp;</span>&nbsp;&nbsp;&nbsp;
						<span id="spSummaryOnly"><asp:checkbox id="chkSummaryOnly" runat="server" text="Summary Only"></asp:checkbox>&nbsp;</span>
					</td>
				</tr>
				<tr>
					<td colspan="2"></td>
					<td>
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><img height="1" src="Images/Space.gif" alt="" width="40" /></td>
								<td><hr width="175" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td valign="top"><asp:radiobutton id="rdoCustomerDate" runat="server" text="Job" groupname="rdoPeriod" checked="False"></asp:radiobutton></td>
					<td></td>
					<td>
						<asp:dropdownlist id="ddlCustomerDate" runat="server"></asp:dropdownlist>
						<asp:checkbox id="chkIncludeAll" runat="server" text="Include All" autopostback="true" OnCheckedChanged="chkIncludeAll_CheckedChanged"></asp:checkbox>
					</td>
				</tr>
				<tr>
					<td><img height="15" src="Images/Space.gif" alt="" width="1" /></td>
				</tr>
				<tr>
					<td colspan="2"></td>
					<td>
						<span id="spCustomerCopy" class="OptionSet"><asp:checkbox id="chkCustomerCopy" runat="server" text="Customer Copy" checked="true"></asp:checkbox>&nbsp;</span>&nbsp;&nbsp;&nbsp;
						<span id="spFileCopy"><asp:checkbox id="chkFileCopy" runat="server" text="File Copy"></asp:checkbox>&nbsp;</span>
					</td>
				</tr>
				<tr>
					<td><img height="1" src="Images/Space.gif" alt="" width="1" /></td>
				</tr>
				<tr>
					<td colspan="3" align="center"><asp:button id="btnReport" runat="server" text="Report" OnClick="btnReport_Click"></asp:button></td>
				</tr>
				<tr>
					<td colspan="3"><hr />
					</td>
				</tr>
				<tr>
					<td colspan="3">
						<asp:radiobutton id="rdoJobsNotInv" runat="server" text="Jobs not Invoiced thru" groupname="rdoPeriod"
							checked="False"></asp:radiobutton>&nbsp;
						<asp:textbox id="txtJobsNotInv" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgJobsNotInv" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image>
					</td>
				</tr>
				<tr>
					<td><img height="1" src="Images/Space.gif" alt="" width="1" /></td>
				</tr>
				<tr>
					<td colspan="3" align="center"><asp:button id="btnJobsNotInv" runat="server" text="Report" OnClick="btnJobsNotInv_Click"></asp:button></td>
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
