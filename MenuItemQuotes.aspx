<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MenuItemQuotes.aspx.cs" Inherits="MenuItemQuotes" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= Globals.g.Company.Name %> - Hide Menu Items</title>
	<!-- MaintFood.aspx -->
	<!-- Copyright (c) 2007-2019 PeteSoft, LLC. All Rights Reserved -->
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
   	<link href="css/font-awesome.min.css" type="text/css" rel="stylesheet">

	<script language="javascript" src="js/jquery-1.7.1.min.js"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function ()
        {
            $("#tblRpt a").click(function ()
            {
                $(this).addClass("clicked");
            });

            Qtips(".icon-dollar");
        });

    </script>

    <style type="text/css">
        #fields
        {
            margin: 0px auto;
            display: block;
            width: 130px;
        }

        label
        {
            margin-right: 10px;
        }

        #txtDiffPct
        {
            width: 40px;
            text-align: center;
        }

        #btnReport
        {
            text-align: center;
            display: block;
            margin: 0px auto 20px;
        }

        table
        {
            margin: 0px auto;
        }

        th
        {
            text-align: left;
        }

        #tblRpt tr:hover
        {
            background-color: #DDE6D0;
        }

        .amt
        {
            text-align: right;
        }

        .quote
        {
            padding-right: 20px;
        }

        .price
        {
            padding-right: 5px;
        }

        .icon-dollar
        {
            color: firebrick;
            padding-right: 5px;
        }

        a.clicked
        {
            color: rgba(77, 121, 8, 0.5);
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
		<% Pg.Top(); %>

		<div class="FeatureMain">Menu Item Quotes vs Prices</div>

        <div id="fields" runat="server">
            <p><label for="txtDiffPct">Difference</label><asp:TextBox ID="txtDiffPct" runat="server" />%</p>
            <asp:Button ID="btnReport" Text="Report" runat="server" />
        </div>
		
        <table id="tblRpt" runat="server">
            <thead>
                <tr>
                    <th>Category</th>
                    <th>Menu Item</th>
                    <th class="amt quote">Quote</th>
                    <th class="amt price">Price</th>
                    <th></th>
                    <th>Difference</th>
                </tr>
            </thead>
        </table>

		<% Pg.Bottom(); %>
    </form>
    
    <script type="text/javascript" language="javascript">
    	$(document).ready(function()
    	{
    	});

    </script>
</body>
</html>
