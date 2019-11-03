<%@ page autoeventwireup="true" codefile="Users.aspx.cs" inherits="Users"
	language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Setup Menu Categories</title>
	<!-- MaintFood.aspx -->
	<!-- Copyright (c) 2007-2019 PeteSoft, LLC. All Rights Reserved -->
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
	<style type="text/css">
	.MenuItem { width: 300px; }
    .password-related { display: none; }
	</style>

	<script language="javascript" src="js/jquery-1.7.1.min.js"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
   	<script language="JavaScript" src="js/jquery.validate.js" type="text/javascript"></script>
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

        $(document).ready(function()
        {
            $("#btnDelete").click(function ()
            {
                return confirm("Just checking.\n\nAre you sure you want to delete this user?");
            });

            <% if (fError) { %>
            $("#ErrMsg").dialog(
            {
                buttons:
                {
                    "OK": function ()
                    {
                        $(this).dialog("close");
                    }
                },
                modal: true
            });
            <% } %>

            $("#form").validate({
                rules: {
                    txtUserName: {
                        required: true,
                        minlength: 3 },
                    txtPassword: {
                        required: true,
                        minlength: 6 }
                }
            });

            if ($("#chkChangePassword").is(":checked")) {
                $(".password-related").show();
            }
            else {
                $(".password-related").hide();
            }

            $("#chkChangePassword").change(function () {
                if ($(this).is(":checked")) {
                    $(".password-related").show();
                }
                else {
                    $(".password-related").hide();
                }
            });
        });

        function MyPostBack(id, parameter)
        {
            __doPostBack(id, parameter)
        }
	</script>
</head>
<body onbeforeunload="CheckDirty();" onload="Init();">
	<form id="form" method="post" runat="server" autocomplete="off">
		<% Pg.Top(); %>
		<table cellspacing="0" cellpadding="0" align="center" border="0">
			<tr>
				<td valign="top">
					<table cellspacing="0" cellpadding="0" align="center" border="0">
                        <tr>
							<td>
                                <asp:checkbox id="chkShowDisabled" text="Show Disabled" textalign="right" oncheckedchanged="UpdateList" autopostback="true" runat="server" />
							</td>
						</tr>
						<tr>
							<td>
								<asp:listbox id="lstList" runat="server" autopostback="true" cssclass="SelectJob"
									onselectedindexchanged="lstList_SelectedIndexChanged" onclick="MyPostBack('lstList', '')"></asp:listbox></td>
						</tr>
						<tr>
							<td>
								<asp:label id="lblRecCount" runat="server" cssclass="SelectCount">RecCount</asp:label></td>
						</tr>
					</table>
				</td>
				<td>
					<img height="1" src="Images/Space.gif" alt="" width="40"></td>
				<td valign="top" align="center">
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td>
							</td>
							<td width="10">
							</td>
							<td width="300">
							</td>
						</tr>
						<tr>
							<td colspan="3" align="center" class="FeatureMain">Users</td>
						</tr>
						<tr>
							<td align="right">Display Name</td>
							<td></td>
							<td align="left">
								<asp:TextBox ID="txtUserCommonName" CssClass="MenuItem" runat="server" />
							</td>
						</tr>
						<tr><td height="10"></td></tr>
                        <tr>
							<td align="right" class="UserNameLabel">User Name</td>
							<td></td>
							<td align="left">
								<asp:TextBox ID="txtUserName" CssClass="MenuItem" runat="server" />
                                <div class="UserName">Case sensitive, minimum 3 characters</div>
							</td>
						</tr>
						<tr><td height="10"></td></tr>

                        <tr>
                            <td></td>
                            <td></td>
                            <td>
                                <asp:checkbox id="chkChangePassword" text=" Change Password" runat="server" />
                            </td>
                        </tr>
                        <tr class="password-related"><td height="10"></td></tr>
                        <tr class="password-related">
							<td align="right">New Password</td>
							<td></td>
							<td align="left">
								<asp:TextBox ID="txtPassword" CssClass="MenuItem" runat="server" type="password"/>
                                <div class="UserName">Case sensitive, minimum 6 characters</div>
							</td>
						</tr>
                        <tr class="password-related"><td height="10"></td></tr>
                        <tr class="password-related">
							<td align="right">Re-type Password</td>
							<td></td>
							<td align="left">
								<asp:TextBox ID="txtRetypePassword" CssClass="MenuItem" runat="server" type="password"/>
							</td>
						</tr>
						<tr><td height="10"></td></tr>
						<tr>
							<td>Access Level</td>
                            <td></td>
							<td align="left">
                                <asp:RadioButton ID="rdoGeneral" GroupName="rdoAccessLevel" Text="General" runat="server" /><br />
                                <asp:RadioButton ID="rdoOfficeStaff" GroupName="rdoAccessLevel" Text="Office Staff" runat="server" /><br />
                                <asp:RadioButton ID="rdoEverything" GroupName="rdoAccessLevel" Text="Everything" runat="server" /><br />
                                <asp:RadioButton ID="rdoAdmin" GroupName="rdoAccessLevel" Text="Admin" runat="server" />
							</td>
						</tr>
						<tr><td height="10"></td></tr>
						<tr>
							<td></td>
                            <td></td>
							<td align="left">
                                <asp:checkbox id="chkDisabled" text=" Disabled" runat="server" />
							</td>
						</tr>
						<tr><td height="10"></td></tr>
						<tr>
							<td>
							</td>
							<td>
							</td>
							<td align="left">
								<asp:button id="btnUpdate" runat="server" text="Save" onclick="btnUpdate_Click" />
								<asp:button id="btnNext" runat="server" text="Save & Next" onclick="btnUpdateNext_Click" />
								<asp:button id="btnNew" runat="server" text="New" onclick="btnNew_Click" />
								<asp:button id="btnDelete" runat="server" text="Delete" onclick="btnDelete_Click" />
							</td>
						</tr>
					</table>

                    <table id="Dates">
						<tr>
							<td align="right">Created</td>
							<td></td>
							<td><asp:textbox id="txtCreatedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
							<td></td>
							<td><asp:textbox id="txtCreatedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
                        </tr>
                        <tr>
							<td align="right">Updated</td>
							<td></td>
							<td><asp:textbox id="txtUpdatedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
							<td></td>
							<td><asp:textbox id="txtUpdatedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
						</tr>
                    </table>
				</td>
			</tr>
		</table>
		<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
		<input type="hidden" id="txtCurrUser" name="txtCurrUser" runat="server" />
		<% Pg.Bottom(); %>
	</form>
    <div id="ErrMsg" title="Error Message">
        <%= ErrorMsg %>
    </div>
</body>
</html>