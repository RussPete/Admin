<%@ Page AutoEventWireup="true" CodeFile="CheckSearch.aspx.cs" Inherits="CheckSearch"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Accounting</title> 
		<!-- Accounting.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />	
		<link href="ReportStyles.css" type="text/css" rel="stylesheet" />
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
		    if ($(document).ready(function()
		    {
				$(".CheckAll").click(function()
		        {
					$(this).closest("table").find("input:checkbox").prop("checked", true);
		        });
				$(".ClearAll").click(function()
		        {
					$(this).closest("table").find("input:checkbox").prop("checked", false);
		        });

                $("table.Acct").each(function()
		        {
			        $("tr:odd", $(this)).addClass("odd");
		        });
		    }));
		</script>
	</head>
	<body>
		<form id="form" method="post" autocomplete="off" runat="server">
			<%//= Misc.Parms(this) %>
			<% Pg.Top(); %>
            
			<div class="FeatureMain">
                Check # Search 
			</div>

            <div style="text-align: center; margin-bottom: 20px;">
                Enter Check # <asp:TextBox ID="txtCheckNum" runat="server" /> <asp:Button ID="btnGo" Text="Go" runat="server" /> (see matches below)
            </div>

			<table class="Acct">
				<thead>
					<tr>
						<th style="white-space: nowrap;">Inv #</th>
						<th>Name</th>
						<th>Date</th>
						<th>Reference</th>
						<th>Subtotal</th>
						<th>Other</th>
						<th>Tax</th>
						<th>Total</th>
					</tr>
				</thead>
				<tbody>
					<%= FindCheckNum() %>
				</tbody>
			</table>
			<% Pg.Bottom(); %>
		</form>
	</body>
</html>
