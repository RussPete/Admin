<%@ Page AutoEventWireup="true" CodeFile="CheckSling.aspx.cs" Inherits="CheckSling" Async="true" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Production Sheets</title> 
		<!-- JobLeads.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<link href="ReportStyles.css" type="text/css" rel="stylesheet">
        <style>
            .Page .section
            {
                text-align: center;
                padding: 25px 0px 5px;
                color: #667B3E;
            }

            .Page td.checkbox 
            {
                text-align: center;
            }
        </style>
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
        <script language="javascript" src="Sling.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">

			//----------------------------------------------------------------------------
		    $(document).ready(function ()
		    {
		        $(".All.Jobs").click(() =>
		        {
		            $(".Job").prop("checked", "checked");
		        });
		        $(".None.Jobs").click(() =>
		        {
		            $(".Job").prop("checked", "");
		        });

		        $(".All.Shifts").click(() =>
		        {
		            $(".Shift").prop("checked", "checked");
		        });
		        $(".None.Shifts").click(() =>
		        {
		            $(".Shift").prop("checked", "");
		        });
		    });
		</script>
	</head>
	<body>
		<form id="form" method="post" autocomplete="off" runat="server">
			<% Pg.Top(); %>
			<div class="FeatureMain">Check Sling</div>
			<table cellspacing="10" cellpadding="0" align="center" border="0">
                <asp:Literal ID="ltlData" runat="server" />
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1" /></td>
				</tr>
                <tr>
                    <td align="center" colspan="5"><asp:button id="btnProcess" runat="server" text="Add &amp; Delete" OnClick="btnProcess_Click" /></td>
                </tr>
                <tr>
                    <td><img height="20" src="Images/Space.gif" alt="" width="1" /></td>
                </tr>

            </table>
            <% Pg.Bottom(); %>
            <asp:HiddenField ID="hfNumJobsIDs" runat="server" />
            <asp:HiddenField ID="hfNumSlingIDs" runat="server" />
		</form>
	</body>
</html>
