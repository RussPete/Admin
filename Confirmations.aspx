<%@ Page AutoEventWireup="true" CodeFile="Confirmations.aspx.cs" Inherits="Confirmations" Async="true" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Confirmations</title> 
		<!-- JobLeads.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<link href="ReportStyles.css" type="text/css" rel="stylesheet">
        <style type="text/css">
            .Report tr td, .Report tr th
            {
                padding: 5px;
            }
            .Report tr th
            {
                text-align: center;
            }
            .Report tr:hover td
            {
                background-color: rgb(228, 235, 217);
            }
            .Send
            {
                width: 60px;
                text-align: center;
            }
            .Name
            {
                width: 150px;
            }
            .Sent
            {
                width: 130px;
            }
            .Contact
            {
                width: 120px;
            }
            .Confirmed 
            {
                width: 74px;
                text-align: center;
            }
            .Button
            {
                display: block;
                margin: 20px auto;            
            }
        </style>
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
        <script language="javascript" src="Sling.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
        <script type="text/javascript">
            if ($(document).ready(function ()
            {
                $(".Send input:checkbox").click(function ()
                {
                    if ($(this).prop("checked"))
                    {
                        $(this).closest("tr").find(".Confirmed input:checkbox").prop("checked", false);
                    }
                });
                $(".Confirmed input:checkbox").click(function ()
                {
                    if ($(this).prop("checked"))
                    {
                        $(this).closest("tr").find(".Send input:checkbox").prop("checked", false);
                    }
                });
            }));
        </script>
	</head>
	<body>
		<form id="form" method="post" autocomplete="off" runat="server">
			<% Pg.Top(); %>
			<div class="FeatureMain">Job Confirmations</div>
            <div style="text-align: center; color: #888;">Corporate Jobs or Pick Ups, Non-confirmed, and Not Canceled</div>

            <table class="Report" cellspacing="0" cellpadding="0" align="center" border="0">
                <thead>
                    <tr>
                        <th class="Send">Send Confirm Email</th>
                        <th>Job #</th>
                        <th class="Name">Name</th>
                        <th>Job Date</th>
                        <th class="Sent">Confirmation Sent</th>
                        <th class="Contact">Contact</th>
                        <th>Phone #</th>
                        <th>Cell #</th>
                        <%--<th>Svc Type</th>--%>
                        <th class="Confirmed">Mark Confirmed</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Label ID="lblReport" runat="server" />
                </tbody>
            </table>
            <asp:HiddenField ID="hfRow" runat="server" />
            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="Button" OnClick="btnSubmit_Click" />
            <% Pg.Bottom(); %>
		</form>
	</body>
</html>
