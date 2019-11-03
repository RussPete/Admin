<%@ Page AutoEventWireup="true" CodeFile="DailyCCPmts.aspx.cs" Inherits="DailyCCPmts" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Daily Credit Card Payments</title> 
		<!-- DailyCCPmts.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />	
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<link href="ReportStyles.css" type="text/css" rel="stylesheet" />
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
		    $(document).ready(function()
		    {
                $("table.Acct").each(function()
		        {
			        $("tr:odd", $(this)).addClass("odd");
    		    });

				$(".JobDate").datepicker(
				{
                    showOn: "both",
                    buttonImage: "Images/CalendarIcon.gif",
                    buttonImageOnly: true
                });
            });
		</script>
	</head>
	<body class="DailyCCPmts">
		<form id="form" method="post" autocomplete="off" runat="server">
			<% Pg.Top(); %>
            <div style="float: left" class="NoPrint">Start Date:&nbsp;<asp:TextBox ID="txtStartDate" CssClass="JobDate" AutoPostBack="true" runat="server" /></div>
			<div class="FeatureMain">Daily Credit Card Payments</div>
            <table class="Acct">
    			<%= CCPayments() %>
			</table>
			<% Pg.Bottom(); %>
		</form>
	</body>
</html>
