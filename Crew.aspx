<%@ Page AutoEventWireup="true" CodeFile="Crew.aspx.cs" Inherits="Crew" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Crew</title> 
		<!-- Crew.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="SelectList.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="JavaScript" type="text/javascript">
		
//----------------------------------------------------------------------------		
function Init()
{
<%	if (FocusField != "")
	{ %>
	SetFocus("<%=FocusField%>");
<%	} %>
}

		</script>
		<%=Utils.SelectList.JavaScript()%>
	</head>
	<body onbeforeunload="CheckDirty();" onload="Init();">
		<form id="form" method="post" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td valign="top">
						<table cellspacing="0" cellpadding="0" align="center" border="0">
							<tr>
								<td colspan="3"><asp:listbox id="lstList" runat="server" cssclass="SelectCrew" autopostback="True" OnSelectedIndexChanged="lstList_SelectedIndexChanged"></asp:listbox></td>
							</tr>
							<tr>
								<td colspan="3"><asp:label id="lblRecCount" runat="server" cssclass="SelectCount">RecCount</asp:label></td>
							</tr>
						</table>
					</td>
					<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
					<td valign="top" align="center">
						<% Pg.JobSubPage("Crew"); %>
						<asp:table id="tblCrew" runat="server" cellpadding="0" cellspacing="0">
							<asp:tablerow>
								<asp:tablecell></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Remove" font-bold="true"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Crew Member" font-bold="true"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Assignment" font-bold="true"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Report Time" font-bold="true" horizontalalign="Center"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Note" font-bold="true"></asp:tablecell>
							</asp:tablerow>
						</asp:table>
						<table cellspacing="0" cellpadding="0" align="center" border="0">
							<tr>
								<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
							</tr>
							<tr>
								<td><asp:button id="btnUpdate" runat="server" text="Update" OnClick="btnUpdate_Click"></asp:button></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<asp:textbox id="txtcCrew" runat="server" visible="False"></asp:textbox>
			<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server">
			<% Pg.Bottom(); %>
		</form>
		<%=Utils.SelectList.DefValues()%>
	</body>
</html>
