<%@ Page AutoEventWireup="true" CodeFile="UnpaidJobs.aspx.cs" Inherits="UnpaidJobs"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Unpaid Jobs</title> 
		<!-- UnpaidJobs.aspx -->
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
	<body class="UnpaidJobs">
		<form id="form" method="post" autocomplete="off" runat="server">
			<% Pg.Top(); %>
			<div class="FeatureMain">Unpaid Jobs</div>
                <table class="Acct">
				<%= Jobs() %>
				<tr class="NoPrint">
					<td colspan="3">
						<div class="CheckAll">Check All</div>
						<div class="ClearAll">Clear All</div>
						<asp:Button ID="btnSave" Text="Save" runat="server" />
					</td>
                    <td colspan="8"></td>
				</tr>
                </tbody>
			</table>
			<% Pg.Bottom(); %>
		</form>
	</body>
</html>
