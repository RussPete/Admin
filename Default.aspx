<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Administration</title> 
		<!-- Default.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />
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
	</head>
	<body onload="Init();">
		<form id="form" method="post" runat="server">
			<% Pg.Top(true); %>
			<table cellspacing="10" cellpadding="0" align="center" border="0">
				<tbody>
					<tr>
						<td><img height="100" src="Images/Space.gif" width="1" alt="" /></td>
					</tr>
					<tr>
						<td align="right" class="UserNameLabel"><b>User</b></td>
						<td>
                            <asp:textbox id="txtUser" runat="server" width="100"></asp:textbox><asp:image id="ddUser" runat="server" imageurl="Images/DropDown.gif" imagealign="Middle" Visible="false"></asp:image>
                            <div class="UserName">Case sensitive</div>
						</td>
					</tr>
					<tr>
						<td align="right"><b>Password</b></td>
						<td><asp:textbox id="txtPassword" runat="server" width="100" textmode="Password"></asp:textbox></td>
						<td><asp:label id="lblBadPassword" runat="server" visible="False" forecolor="Red">Wrong user or password.</asp:label></td>
					</tr>
					<tr>
						<td></td>
						<td><asp:button id="btnLogin" runat="server" text="Login" OnClick="btnLogin_Click"></asp:button></td>
					</tr>
					<tr>
						<td><img height="100" src="Images/Space.gif" width="1" alt="" /></td>
					</tr>
				</tbody>
			</table>
			<% //Pg.Bottom(); %>
		</form>
	</body>
</html>
