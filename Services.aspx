<%@ Page AutoEventWireup="true" CodeFile="Services.aspx.cs" Inherits="Services" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Services</title> 
		<!-- Services.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<!--script language="javascript" src="SelectList.js" type="text/javascript"></script-->
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    	<script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="https://use.fontawesome.com/ed45f487e9.js" type="text/javascript"></script>
        <script language="JavaScript" src="RecentJobs.js" type="text/javascript"></script>
		<script language="JavaScript" type="text/javascript">

$(document).ready(function()
{
    SetupRecentJobs();

    // dirty bit
    $(".EditData :input:not([readonly='readonly']):not([disabled='disabled'])").change(function () 
    {
        SetDirty();
    });

    $(window).bind("beforeunload", function () 
    {
        return CheckDirty();
    });
});

var cServices = <%=cServices%>;
		
//----------------------------------------------------------------------------		
function Init()
{
<%	if (FocusField != "")
	{ %>
	SetFocus("<%=FocusField%>");
<%	} %>

	//CheckResponsibility();
}

//----------------------------------------------------------------------------		
function CheckResponsibility()
{
	var fMCC;
	var fCust;
	
	for (var iServices = 1; iServices <= cServices; iServices++)
	{
		fMCC  = iGetChk("chkMCCResp"      + iServices);
		fCust = iGetChk("chkCustomerResp" + iServices);
		
		if (!fMCC && !fCust)
		{
			SetClass("chkMCCResp"      + iServices, "DupServiceItem");
			SetClass("chkCustomerResp" + iServices, "DupServiceItem");
		}
	}
}

		</script>
		<%//=Utils.SelectList.JavaScript()%>
	</head>
	<body onbeforeunload="CheckDirty();" onload="Init();">
		<form id="form" method="post" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<asp:panel id="pnlEdit" runat="server">
				<table cellspacing="0" cellpadding="0" align="center" border="0">
					<tr>
						<td valign="top">
							<table cellspacing="0" cellpadding="0" align="center" border="0">
								<tr>
									<td colspan="3">
										<asp:listbox id="lstList" runat="server" autopostback="True" cssclass="SelectJob" OnSelectedIndexChanged="lstList_SelectedIndexChanged"></asp:listbox></td>
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
												<td><img onmouseover="SetImg(this, 'Images/ShowAllLgt.gif');" onclick="OpenPage('Services.aspx?View=Show');"
														onmouseout="SetImg(this, 'Images/ShowAllDrk.gif');" src="Images/ShowAllDrk.gif"></td>
												<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
												<td><img src="Images/EditDis.gif"></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
							<div><img height="5" src="Images/Space.gif" alt="" width="1"></div>
							<% Pg.JobSubPage("Services"); %>
							<asp:table id="tblServices" runat="server" cellspacing="0" cellpadding="0">
								<asp:tablerow>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell text="Responsible Party" font-bold="true" columnspan="2" horizontalalign="Center"></asp:tablecell>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell></asp:tablecell>
								</asp:tablerow>
								<asp:tablerow>
									<asp:tablecell></asp:tablecell>
									<asp:tablecell width="10px"></asp:tablecell>
									<asp:tablecell text="Remove" font-bold="true"></asp:tablecell>
									<asp:tablecell width="10px"></asp:tablecell>
									<asp:tablecell text="Service Item" font-bold="true"></asp:tablecell>
									<asp:tablecell width="10px"></asp:tablecell>
									<asp:tablecell text="<%= Globals.g.Company.Initials %>" font-bold="true" horizontalalign="Center"></asp:tablecell>
									<asp:tablecell text="Cust" font-bold="true" horizontalalign="Center"></asp:tablecell>
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
				<asp:textbox id="txtcServices" runat="server" visible="False"></asp:textbox>
			</asp:panel>
			<asp:panel id="pnlShow" runat="server">
				<table class="TabMenu" cellspacing="0" cellpadding="0" border="0" style="display: none;">
					<tr>
						<td align="center">
							<table cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><img src="Images/ShowAllDis.gif"></td>
									<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
									<td><img onmouseover="SetImg(this, 'Images/EditLgt.gif');" onclick="OpenPage('Services.aspx?View=Edit');"
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
				<% Pg.JobSubPage("Services"); %>
				<asp:table id="tblShowServices" runat="server" cellspacing="0" cellpadding="0" horizontalalign="Center" CssClass="EditData">
					<asp:tablerow>
						<asp:tablecell verticalalign="Top" runat="server" id="tdShowServices1" cssclass="ColTable" visible="False">
							<asp:table id="tblShowServices1" runat="server" cellpadding="0" cellspacing="0" horizontalalign="Center"></asp:table>
						</asp:tablecell>
						<asp:tablecell>
							<asp:image id="imgShowServices" runat="server" width="0" height="0" imageurl="Images/Space.gif"></asp:image>
						</asp:tablecell>
						<asp:tablecell verticalalign="Top" runat="server" id="tdShowServices2" cssclass="ColTable" visible="False">
							<asp:table id="tblShowServices2" runat="server" cellpadding="0" cellspacing="0" horizontalalign="Center"></asp:table>
						</asp:tablecell>
					</asp:tablerow>
				</asp:table>
				<table cellspacing="0" cellpadding="0" align="center" border="0">
					<tr>
						<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
					</tr>
					<tr>
						<td>
							<asp:button id="btnShowUpdate" runat="server" text="Update" OnClick="UpdateList"></asp:button></td>
					</tr>
				</table>
				<asp:textbox id="txtcShowServices" runat="server" visible="False"></asp:textbox>
			</asp:panel>
			<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server">
			<% Pg.Bottom(); %>
		</form>
		<%//=Utils.SelectList.DefValues()%>
	</body>
</html>
