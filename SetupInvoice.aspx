<%@ Page AutoEventWireup="true" CodeFile="SetupInvoice.aspx.cs" Inherits="SetupInvoice"
    Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title><%= Globals.g.Company.Name %> - Maintance Food</title>
    <!-- SetupInvoice.aspx -->
    <!-- Copyright (c) 2007-2019 PeteSoft, LLC. All Rights Reserved -->
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <link href="Styles.css" type="text/css" rel="stylesheet">
    <link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
    <style type="text/css">
        .MenuItem
        {
            width: 300px;
        }
    </style>

    <script language="javascript" src="js/jquery-1.7.1.min.js"></script>
    <script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script language="JavaScript" src="js/jquery.validate.js" type="text/javascript"></script>
    <script language="JavaScript" src="Web.js" type="text/javascript"></script>
    <script language="JavaScript" type="text/javascript">

        //----------------------------------------------------------------------------		
        $(document).ready(function ()
        {
            $("#PageContent").on("change", "input", SetDirty);
            $("#btnUpdate").click(SkipDirty);
            window.onbeforeunload = CheckDirty;
        });

    </script>
    <style>
        div.Title
        {
            display: inline-block;
            font-weight: bold;
            width: 80px;
            margin-left: 10px;
        }

        ul
        {
            list-style-type: none;
            width: 500px;
            display: inline-block;
            margin: 0px;
            padding: 0px;
            text-align: left;
        }

        label
        {
            width: 170px;
            text-align: right;
            display: inline-block;
        }

        div.JobPct
        {
            display: inline-block;
            width: 90px;
        }

        input.JobPct
        {
            margin-left: 10px;
        }

        .Buttons
        {
            margin: 10px 0px;
        }

    </style>
</head>
<body>
    <form id="form" method="post" runat="server" autocomplete="off">
        <% Pg.Top(); %>
        <div id="PageContent">
            <h1 class="FeatureMain">Invoice</h1>

            <ul>
                <li>
                    <label></label>
                    <div class="Title">% Subtotal</div>
                    <div class="Title">Tax Pct</div>
                </li>
                <li>
                    <label>Subtotal</label><div class="JobPct"></div>
                    <asp:TextBox ID="txtSubTotTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>On-site Service Fee</label>
                    <asp:TextBox ID="txtServiceSubTotPct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtServiceTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Delivery</label>
                    <asp:TextBox ID="txtDeliverySubTotPct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtDeliveryTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>China</label>
                    <asp:TextBox ID="txtChinaSubTotPct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtChinaTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Addl Service Fee	</label>
                    <asp:TextBox ID="txtAddServiceSubTotPct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtAddServiceTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Fuel & Travel</label>
                    <asp:TextBox ID="txtFuelTravelSubTotPct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtFuelTravelTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Venue Fee</label>
                    <asp:TextBox ID="txtFacilitySubTotPct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtFacilityTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Gratuity</label>
                    <asp:TextBox ID="txtGratuitySubTotPct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtGratuityTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Rentals</label>
                    <asp:TextBox ID="txtRentalsSubTotPct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtRentalsTaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Adjustment 1</label><div class="JobPct"></div>
                    <asp:TextBox ID="txtAdj1TaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Adjustment 2</label><div class="JobPct"></div>
                    <asp:TextBox ID="txtAdj2TaxPct" CssClass="JobPct" runat="server" />%</li>
                <li>
                    <label>Credit Card Fee</label>
                    <asp:TextBox ID="txtCCFeePct" CssClass="JobPct" runat="server" />%
                    <asp:TextBox ID="txtCCFeeTaxPct" CssClass="JobPct" runat="server" />%</li>
            </ul>

            <div class="Buttons"><asp:Button ID="btnUpdate" runat="server" Text="Save" OnClick="btnUpdate_Click" /></div>
        </div>
        <asp:HiddenField ID="hfRno" runat="server" />
        <% Pg.Bottom(); %>
    </form>
</body>
</html>
