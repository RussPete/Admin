<%@ Page AutoEventWireup="true" CodeFile="Supplies.aspx.cs" Inherits="Supplies" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Supplies</title> 
		<!-- Supplies.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
    	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
        <style>
            .Qty { width: 30px; text-align: center; border-style: none; border-bottom: 1px solid #C0C0C0; margin: 0px 4px; }
            .Note { width: 120px; border-style: none; border-bottom: 1px solid #C0C0C0; margin: 0px 4px; }
        </style>

		<!--script language="javascript" src="SelectList.js" type="text/javascript"></script-->
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	    <script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	    <script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="https://use.fontawesome.com/ed45f487e9.js" type="text/javascript"></script>
        <script language="JavaScript" src="RecentJobs.js" type="text/javascript"></script>
		<script language="JavaScript" type="text/javascript">
		
$(document).ready(function () {
	SetupRecentJobs();

    // dirty bit
	$(".EditData :input:not([readonly='readonly']):not([disabled='disabled'])").change(function () {
	    SetDirty();
	});

	$(window).bind("beforeunload", function () {
	    return CheckDirty();
	});
});

//----------------------------------------------------------------------------		
function Init()
{
<%	if (FocusField != "")
	{ %>
	SetFocus("<%=FocusField%>");
<%	} %>
}

		</script>
		<%//=Utils.SelectList.JavaScript()%>
	</head>
	<body onbeforeunload="CheckDirty();" onload="Init();">
		<form id="form" method="post" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<asp:panel id="pnlEdit" runat="server" Visible="false">
				<table cellspacing="0" cellpadding="0" align="center" border="0">
					<tr>
						<td valign="top">
							<table cellspacing="0" cellpadding="0" align="center" border="0">
								<tr>
									<td colspan="3">
										<asp:listbox id="lstList" runat="server" cssclass="SelectJob" autopostback="True" OnSelectedIndexChanged="lstList_SelectedIndexChanged"></asp:listbox></td>
								</tr>
								<tr>
									<td colspan="3">
										<asp:label id="lblRecCount" runat="server" cssclass="SelectCount">RecCount</asp:label></td>
								</tr>
							</table>
						</td>
						<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
						<td valign="top" align="center">
							<table class="TabMenu" cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img onmouseover="SetImg(this, 'Images/ShowAllLgt.gif');" onclick="OpenPage('Supplies.aspx?View=Show');"
														onmouseout="SetImg(this, 'Images/ShowAllDrk.gif');" src="Images/ShowAllDrk.gif"></td>
												<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
												<td><img src="Images/EditDis.gif"></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
							<div><img height="5" src="Images/Space.gif" alt="" width="1"></div>
							<% Pg.JobSubPage("Supplies"); %>
							<asp:table id="tblSupplies" runat="server" cellspacing="0" cellpadding="0">
								<asp:tablerow>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell width="10px"></asp:tablecell>
									<asp:tablecell text="Remove" font-bold="true"></asp:tablecell>
									<asp:tablecell width="10px"></asp:tablecell>
									<asp:tablecell text="Supply Item" font-bold="true"></asp:tablecell>
									<asp:tablecell width="10px"></asp:tablecell>
									<asp:tablecell text="Quantity" font-bold="true" horizontalalign="Right"></asp:tablecell>
									<asp:tablecell width="10px"></asp:tablecell>
									<asp:tablecell text="Note" font-bold="true"></asp:tablecell>
								</asp:tablerow>
							</asp:table>
							<table cellspacing="0" cellpadding="0" align="center" border="0">
								<tr>
									<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
								</tr>
								<tr>
									<td>
										<asp:button id="btnUpdate" runat="server" text="Update" OnClick="UpdateList"></asp:button></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
				<asp:textbox id="txtcSupplies" runat="server" visible="False"></asp:textbox>
			</asp:panel>
			<asp:panel id="pnlShow" runat="server">
				<table class="TabMenu" cellspacing="0" cellpadding="0" border="0" style="display: none;">
					<tr>
						<td align="center">
							<table cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><img src="Images/ShowAllDis.gif"></td>
									<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
									<td><img onmouseover="SetImg(this, 'Images/EditLgt.gif');" onclick="OpenPage('Supplies.aspx?View=Edit');"
											onmouseout="SetImg(this, 'Images/EditDrk.gif');" src="Images/EditDrk.gif"></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
				<div><img height="5" src="Images/Space.gif" alt="" width="1"></div>
				<div style="position: relative;">
                    <%= RecentJobs.Html() %>
                </div>
				<% Pg.JobSubPage("Supplies"); %>
				<asp:table id="tblShowSupplies" runat="server" cellspacing="0" cellpadding="0" horizontalalign="Center" CssClass="EditData">
					<asp:tablerow>
						<asp:tablecell verticalalign="Top" runat="server" id="tdShowSupplies1" cssclass="ColTable" visible="False">
							<asp:table id="tblShowSupplies1" runat="server" cellpadding="0" cellspacing="0" horizontalalign="Center"></asp:table>
						</asp:tablecell>
						<asp:tablecell>
							<asp:image id="imgShowSupplies" runat="server" width="0" height="0" imageurl="Images/Space.gif"></asp:image>
						</asp:tablecell>
						<asp:tablecell verticalalign="Top" runat="server" id="tdShowSupplies2" cssclass="ColTable" visible="False">
							<asp:table id="tblShowSupplies2" runat="server" cellpadding="0" cellspacing="0" horizontalalign="Center"></asp:table>
						</asp:tablecell>
					</asp:tablerow>
				</asp:table>
				<table cellspacing="0" cellpadding="0" align="center" border="0" style="margin-bottom: 20px;">
					<tr>
						<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
					</tr>
					<tr>
						<td>
							<asp:button id="btnShowUpdate" runat="server" text="Update" OnClick="UpdateList"></asp:button>
						</td>
					</tr>
				</table>
				<asp:textbox id="txtcShowSupplies" runat="server" visible="False"></asp:textbox>
			</asp:panel>
			<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server">
			<% Pg.Bottom(); %>
		</form>
		<%//=Utils.SelectList.DefValues()%>
	</body>
</html>
