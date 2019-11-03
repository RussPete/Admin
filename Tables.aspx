<%@ Page AutoEventWireup="true" CodeFile="Tables.aspx.cs" Inherits="Tables" Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Tables</title>
	<!-- Tables.aspx -->
	<!-- Copyright (c) 2007-2019 PeteSoft, LLC. All Rights Reserved -->
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
	<style type="text/css">
		.Field { padding-top: 3px; text-align: right; vertical-align: top; }
		.Tables { width: 150px; }
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
</head>
<body onbeforeunload="CheckDirty();" onload="Init();">
	<form id="form" method="post" runat="server" autocomplete="off">
		<% Pg.Top(); %>
		<table cellspacing="0" cellpadding="0" align="center" border="0" width="100%">
			<tr>
				<td>
					<div><img height="5" src="Images/Space.gif" alt="" width="1" /></div>
				    <div style="position: relative;">
                        <%= RecentJobs.Html() %>
                    </div>
					<% Pg.JobSubPage("Tables"); %>
				</td>
			</tr>
			<tr>
				<td>
                    <asp:Table ID="tblTables" BorderWidth="0" CellPadding="0" CellSpacing="0" HorizontalAlign="Center" CssClass="EditData" runat="server">
                        <asp:TableRow>
                            <asp:TableHeaderCell />
                            <asp:TableHeaderCell Width="10" />
                            <asp:TableHeaderCell><%= Globals.g.Company.Initials %></asp:TableHeaderCell>
                            <asp:TableHeaderCell>Other</asp:TableHeaderCell>
                        </asp:TableRow>
                    </asp:Table>
                    <div style="height: 10px;"></div>
                    <div style="text-align: center;">
						<asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click"></asp:Button>
                    </div>
                    <div style="height: 140px;"></div>
				</td>
			</tr>
		</table>
		<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
        <asp:HiddenField ID="hfCount" runat="server" />
        <asp:HiddenField ID="hfNewRecords" runat="server" />
		<% Pg.Bottom(); %>
	</form>
</body>
</html>
