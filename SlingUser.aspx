<%@ page autoeventwireup="true" codefile="SlingUser.aspx.cs" inherits="SlingUser"
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

	    $(document).ready(function ()
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
	                txtUser: {
	                    required: true,
	                    minlength: 3
	                },
	                txtPassword: {
	                    required: true,
	                    minlength: 6
	                }
	            }
	        });

	        $("#txtPassword").prop("type", "password");
	        $("#txtPassword")
                .focus(function ()
	            {
                    $(this).prop("type", "text");
                })
	            .blur(function ()
	            {
	                $(this).prop("type", "password");
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
							<td colspan="3" align="center" class="FeatureMain">Sling User</td>
						</tr>
						<tr>
							<td align="right">Login Email</td>
							<td></td>
							<td align="left">
								<asp:TextBox ID="txtUser" CssClass="MenuItem" runat="server" />
							</td>
						</tr>
						<tr><td height="10"></td></tr>
                        <tr>
							<td align="right" class="UserNameLabel">Password</td>
							<td></td>
							<td align="left">
								<asp:TextBox ID="txtPassword" CssClass="MenuItem" runat="server" />
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
        <asp:HiddenField ID="hfEmailRno" runat="server" />
        <asp:HiddenField ID="hfPasswordRno" runat="server" />
		<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
		<% Pg.Bottom(); %>
	</form>
    <div id="ErrMsg" title="Error Message">
        <%= ErrorMsg %>
    </div>
</body>
</html>