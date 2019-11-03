<%@ page autoeventwireup="true" codefile="EditProfile.aspx.cs" inherits="EditProfile" language="C#" %>
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
                        minlength: 3
                    },
                    txtNewPassword: {
                        required: true,
                        minlength: 6
                    }
                }
            });

            if ($("#chkChangePassword").is(":checked"))
            {
                $(".password-related").show();
                $("#bottom-spacer").hide();
            }
            else
            {
                $(".password-related").hide();
                $("#bottom-spacer").show();
            }

            $("#chkChangePassword").change(function ()
            {
                if ($(this).is(":checked"))
                {
                    $(".password-related").show();
                    $("#bottom-spacer").hide();
                }
                else
                {
                    $(".password-related").hide();
                    $("#bottom-spacer").show();
                }
            });
        });
    </script>
</head>
<body onload="Init();">
	<form id="form" method="post" runat="server" autocomplete="off">
		<% Pg.Top(); %>
		<table cellspacing="0" cellpadding="0" align="center" border="0">
			<tr>
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
							<td colspan="3" align="center" class="FeatureMain">Edit Profile</td>
						</tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td>
                                <asp:Panel ID="pnlChangedPassword" Visible="false" runat="server" style="color: red;">Successfully changed password.</asp:Panel>
                            </td>
                        </tr>
                        <tr><td height="10"></td></tr>
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
							<td align="right">Current Password</td>
							<td></td>
							<td align="left">
								<asp:TextBox ID="txtCurrentPassword" CssClass="MenuItem" runat="server" type="password"/>
							</td>
						</tr>
                        <tr class="password-related"><td height="10"></td></tr>
                        <tr class="password-related">
							<td align="right">New Password</td>
							<td></td>
							<td align="left">
								<asp:TextBox ID="txtNewPassword" CssClass="MenuItem" runat="server" type="password"/>
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
							<td>
							</td>
							<td>
							</td>
							<td align="left">
								<asp:button id="btnUpdate" runat="server" text="Save" onclick="btnUpdate_Click" />
							</td>
                            <td height="50"></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
        <div><img id="bottom-spacer" src="Images/Space.gif" style="height: 93px;" /></div>
		<% Pg.Bottom(); %>
	</form>
    <div id="ErrMsg" title="Error Message">
        <%= ErrorMsg %>
    </div>
</body>
</html>