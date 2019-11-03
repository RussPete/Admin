<%@ Page AutoEventWireup="true" CodeFile="Invoice.aspx.cs" Inherits="Invoice" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title>Marvellous Catering - Invoice</title> 
		<!-- Invoice.aspx -->
		<!-- Copyright (c) 2005-2012 PeteSoft, LLC. All Rights Reverved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" rel="stylesheet" type="text/css" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />
        <style type="text/css">
            .aspNetDisabled
            {
                background-color: rgb(235, 235, 228);
                border-style: solid;
                border-width: 1px;
                border-color: #aaa;
                padding: 2px 1px;
                color: rgb(84, 84, 84);            
            }
        </style>
		<script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery.formatCurrency-1.4.0.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="https://use.fontawesome.com/ed45f487e9.js" type="text/javascript"></script>
        <script language="JavaScript" src="RecentJobs.js" type="text/javascript"></script>
		<!--script language="javascript" src="SelectList.js" type="text/javascript"></script-->
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
<script type="text/javascript">

    var fPrinted		= <%=(fPrinted ? "true" : "false")%>;
    var fJobCancelled	= <%=(fJobCancelled ? "true" : "false")%>;
    var EventCount		= <%=EventCount%>;
    var EventCountInfo	= "<%=EventCountInfo%>";
    var PricePerPerson	= <%=PricePerPerson%>;

    var iPrice = 0;
    var iPmtNum = 0;

    var fCalcTotals = true;

    //----------------------------------------------------------------------------		
    $(document).ready(function ()
    {
        console.log("document ready");

        iPrice = $("#PriceNum").asNumber();
        iPmtNum = $("#hfPmtNum").asNumber();

        Init();

        $("#AddPrice").click(AddOrEditPrice);
        $("#AddlPricing").on("click", ".EditPrice", AddOrEditPrice);
        $("#AddlPricing").on("click", ".DeletePrice", DeletePrice); 	// for some reason this triggers twice after the dialog popups

        $("#rdoPerPerson, #rdoPerItem, #rdoAllIncl").change(function ()
        {
            $("#dlPerPerson, #dlPerItem, #dlAllIncl").hide();
            $("#" + $(this).data("selector")).show();
        });

        $("#PriceDialog .Price, #DepositInvoiceDialog .PmtAmt").change(function ()
        {
            $(this).formatCurrency();
        });

        EnableSalesTax(!$("#chkSalesTaxExempt")[0].checked);
        $("#chkSalesTaxExempt").change(function ()
        {
            CalcSubTotal();
            EnableSalesTax(!this.checked);
        });

        $("#chkCCPmtFee").change(function ()
        {
            InvoiceTotal();
        });

        SetupPayment();

        $("#btnSave").click(function ()
        {
            let i = 1;
        });
        $("#btnSaveFinal").click(function ()
        {
            var rc = confirm('You are finished with this job and ready to send the invoice?');
            if(rc)
            {
                ClearDirty();
            }

            return rc;
        });
        $("#btnPrint").click(Print);
        $("#btnEmail").click(Email);
        $("#btnDepositInvoice").click(DepositInvoice);

        $(".JobDate").not(":disabled").not("#txtCCExpDt").datepicker(
		{
		    showOn: "both",
		    buttonImage: "Images/CalendarIcon.gif",
		    buttonImageOnly: true
		});

        // Adjustments
        var Adjustments = [ <%= slAdj1Desc.jsArray() %> ];
		$("#txtAdj1Desc, #txtAdj2Desc").autocomplete({
		    source: Adjustments,
		    autoFocus: true,
		    delay: 0,
		    minLength: 0		
		});

        // before submit
		$("#form").submit(function ()
		{
            // enable tax % fields so they can be saved
		    $(".TaxPct").attr("disabled", false);
		});

		$(".hfPostCC").each((i, e) =>
		{
		    let PmtNum = i + 1;
		    if ($(e).val() == "true")
		    {
		        $(e).val("");
		        $("#UMamount").val(FmtDollar($("#txtPmtAmt" + PmtNum).asNumber()));
		        $("#UMinvoice").val($("#UMinvoice").val() + "-" + PmtNum);
		        $("#CCPayment").click();
		    }
		});

		SetupRecentJobs();

        // dirty bit
		$(".EditData :input:not([readonly='readonly']):not([disabled='disabled']):not(#chkCustomerCopy):not(#chkFileCopy)").change(function () {
		    SetDirty();
		});

		$(window).bind("beforeunload", function () {
		    return CheckDirty();
		});

        <% if (ShowPayment > 0) { %>
        fCCCharged = true;
        $($(".EditPayment")[<%=ShowPayment%> - 1]).click();
        <% } %>
    });

    function EnableSalesTax(Enable)
    {
        $(".TaxPct").attr("disabled", !Enable).css("opacity", (Enable ? "1.0" : "0.5"));
    }
</script>

<%--
		<%=Utils.SelectList.JavaScript()%>
		<%=calInvDate.JavaScript()%>
		<%=calInvDeliveryDate.JavaScript()%>
		<%=calInvPmtDate.JavaScript()%>
--%>
		<script language="javascript" type="text/javascript">
//calInvDate.DateSet = InvDateChange;
		</script>
	</head>

	<body class="InvoicePage">
		<form id="form" method="post" runat="server" autocomplete="off" class="EditData">
			<% Pg.Top(); %>
            <asp:HiddenField ID="hfInvSubTotal" runat="server" />
            <asp:HiddenField ID="hfCCFeeAmt" runat="server" />
            <asp:HiddenField ID="hfPreTaxTotal" runat="server" />
            <asp:HiddenField ID="hfSalesTaxTotal" runat="server" />
            <asp:HiddenField ID="hfInvTotal" runat="server" />
            <asp:HiddenField ID="hfInvBalance" runat="server" />
            <asp:HiddenField ID="hfDepositAmt" runat="server" />
            <asp:HiddenField ID="hfDepositInvoice" runat="server" />
            <div style="position: relative;">
                <%= RecentJobs.Html() %>
            </div>
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td width="200"></td>
					<td class="FeatureMain" align="center">Invoice #<asp:label id="lblJobRno" runat="server"></asp:label></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="150" /></td>
					<td valign="middle" align="right" width="50">
						<div class="Reprint" id="divReprint">Reprint</div>
					</td>
				</tr>
			</table>
			<% Pg.JobSubPage(""); %>
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td colspan="2">Catering Services for:</td>
				</tr>
				<tr>
					<td><img height="1" src="Images/Space.gif" alt="" width="10" />
					</td>
					<td><b><asp:label id="lblCustomer" runat="server"></asp:label></b></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="300" /></td>
					<td>Invoice Date
						<asp:textbox id="txtInvDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgInvDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td></td>
					<td><asp:label id="lblName" runat="server"></asp:label></td>
				</tr>
			</table>
			<table cellspacing="0" cellpadding="0" align="center" border="0" class="Inputs">
				<tbody>
					<tr>
						<td></td>
						<td><img height="10" src="Images/Space.gif" alt="" width="25" /></td>
						<td></td>
						<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
						<td></td>
						<td><img height="1" src="Images/Space.gif" alt="" width="5" /></td>
						<td></td>
						<td><img height="1" src="Images/Space.gif" alt="" width="235" /></td>
					</tr>
					<tr>
						<td colspan="2"></td>
						<td align="right"><b>Event Date</b></td>
						<td></td>
						<td><asp:textbox id="txtJobDate" runat="server" cssclass="JobDate" enabled="false"></asp:textbox></td>

						<td></td>
						<td><asp:Image ID="AddPrice" ImageUrl="Images/Add16.png" BorderWidth="0" AlternateText="Add Price" ToolTip="Add an additional price" CssClass="AddPrice" runat="server" /></td>

					</tr>
<%--					
					<tr>
						<td colspan="3" align="right">
							<asp:textbox id="txtInvEventCount" runat="server" cssclass="Count" />
							servings @
							<asp:textbox id="txtInvPricePerPerson" runat="server" cssclass="Price" />
							per person
						</td>
						<td></td>
						<td><asp:textbox id="txtInvExtPrice" runat="server" cssclass="JobSubTotal" enabled="false" /></td>
						<td></td>
						<td><asp:Image ID="AddPrice" ImageUrl="Images/Add16.png" BorderWidth="0" AlternateText="Add Price" ToolTip="Add an additional pricing" CssClass="AddPrice" runat="server" /></td>
					</tr>
--%>					
				</tbody>
				<tbody id="AddlPricing">
					<asp:Literal ID="ltlPricing" runat="server" />
				</tbody>
				<tbody>
					<tr>
						<td>% of Subtotal</td>
						<td></td>
						<%--<td align="right"><button id="SetDefaults" type="button" onclick="window.location='SetupInvoice.aspx'">Set Default %s</button><asp:button id="btnSaveDefaults" runat="server" text="Set Default %s" OnClick="btnSaveDefaults_Click" Visible="false" ToolTip="Links to the Invoice Setup page to set default sales tax and subtotal percentages." /></td>--%>
                        <td></td>
						<td></td>
						<td colspan="1">
							<hr />
						</td>
						<td></td>
						<td>Sales Tax %</td>
                        <td><button id="Button1" type="button" onclick="window.location='SetupInvoice.aspx'">Set Default %s</button></td>
					</tr>
					<tr>
						<td colspan="2"></td>
						<td align="right"><b>Subtotal</b></td>
						<td></td>
						<td><asp:textbox id="txtInvSubTotal" runat="server" cssclass="JobSubTotal" enabled="false"></asp:textbox><asp:HiddenField ID="txtSubTotal" runat="server" /></td>
						<td></td>
						<td><asp:textbox id="txtSubTotTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtServiceSubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtServiceSubTotPctFlg" type="hidden" name="txtServiceSubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right">On-site Service Fee</td>
						<td></td>
						<td><asp:textbox id="txtServiceAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtServiceTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtDeliverySubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtDeliverySubTotPctFlg" type="hidden" name="txtDeliverySubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right">Delivery</td>
						<td></td>
						<td><asp:textbox id="txtDeliveryAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtDeliveryTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtChinaSubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtChinaSubTotPctFlg" type="hidden" name="txtChinaSubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right">China</td>
						<td></td>
						<td><asp:textbox id="txtChinaAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtChinaTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtAddServiceSubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtAddServiceSubTotPctFlg" type="hidden" name="txtAddServiceSubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right">Addl Service Fee</td>
						<td></td>
						<td><asp:textbox id="txtAddServiceAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtAddServiceTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtFuelTravelSubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtFuelTravelSubTotPctFlg" type="hidden" name="txtFuelTravelSubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right">Fuel & Travel</td>
						<td></td>
						<td><asp:textbox id="txtFuelTravelAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtFuelTravelTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtFacilitySubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtFacilitySubTotPctFlg" type="hidden" name="txtFacilitySubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right">Venue Fee</td>
						<td></td>
						<td><asp:textbox id="txtFacilityAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtFacilityTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
    				<tr>
						<td align="center"><asp:textbox id="txtGratuitySubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtGratuitySubTotPctFlg" type="hidden" name="txtGratuitySubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right">Gratuity</td>
						<td></td>
						<td><asp:textbox id="txtGratuityAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
    					<td></td>
						<td><asp:textbox id="txtGratuityTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
    				<tr>
						<td align="center"><asp:textbox id="txtVoluntaryGratuitySubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtVoluntaryGratuitySubTotPctFlg" type="hidden" name="txtVoluntaryGratuitySubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right"><b>Voluntary Gratuity</b></td>
						<td></td>
						<td><asp:textbox id="txtVoluntaryGratuityAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtRentalsSubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtRentalsSubTotPctFlg" type="hidden" name="txtRentalsSubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right">Rentals</td>
						<td></td>
						<td><asp:textbox id="txtRentalsAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtRentalsTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
					<tr>
						<td colspan="7" align="right"><asp:TextBox ID="txtRentalsDesc" CssClass="RentalsDesc" MaxLength="100" runat="server" /></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtAdj1SubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtAdj1SubTotPctFlg" type="hidden" name="txtAdj1SubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right"><asp:textbox id="txtAdj1Desc" runat="server" cssclass="MaintShort"></asp:textbox><asp:image id="ddAdj1Desc" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image></td>
						<td></td>
						<td><asp:textbox id="txtAdj1Amt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtAdj1TaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
					<tr>
						<td align="center"><asp:textbox id="txtAdj2SubTotPct" runat="server" cssclass="JobPct"></asp:textbox><input id="txtAdj2SubTotPctFlg" type="hidden" name="txtAdj2SubTotPctFlg" runat="server" /></td>
						<td></td>
						<td align="right"><asp:textbox id="txtAdj2Desc" runat="server" cssclass="MaintShort"></asp:textbox><asp:image id="ddAdj2Desc" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image></td>
						<td></td>
						<td><asp:textbox id="txtAdj2Amt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtAdj2TaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
<% if (fChargeCCFee) { %>
					<tr>
						<td align="center"><asp:textbox id="txtCCFeePct" runat="server" cssclass="JobPct"></asp:textbox></td>
						<td></td>
						<td align="right"><asp:CheckBox ID="chkCCPmtFee" Text=" Credit Card Fee" runat="server" /></td>
						<td></td>
						<td><asp:textbox id="txtCCFeeAmt" runat="server" cssclass="JobSubTotal" enabled="false"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtCCFeeTaxPct" runat="server" cssclass="JobPct TaxPct"></asp:textbox></td>
					</tr>
<% } %>
					<tr>
						<td colspan="2"></td>
						<td colspan="3">
							<hr />
						</td>
					</tr>
					<tr>
						<td colspan="2"></td>
						<td align="right"><b>Pre-Tax Subtotal</b></td>
						<td></td>
						<td><asp:textbox id="txtPreTaxTotal" runat="server" cssclass="JobSubTotal" enabled="false"></asp:textbox></td>
					</tr>
					<tr>
						<td colspan="2"></td>
						<td align="right"><b>Sales Tax</b></td>
						<td></td>
						<td><asp:textbox id="txtSalesTaxTotal" runat="server" cssclass="JobSubTotal" enabled="false"></asp:textbox></td>
                        <td></td>
						<td colspan="2" style="padding-left: 0px;"><asp:CheckBox ID="chkSalesTaxExempt" Text="Sales Tax Exempt" runat="server" /></td>
					</tr>
					<tr>
						<td colspan="2"></td>
						<td colspan="3">
							<hr />
						</td>
					</tr>
					<tr>
						<td colspan="2"></td>
						<td align="right"><b>Invoice Total</b></td>
						<td></td>
						<td><asp:textbox id="txtInvTotal" runat="server" cssclass="JobSubTotal" enabled="false"></asp:textbox></td>
					</tr>
                    <%-- 
					<tr style="display: none;">
						<!-- <td align="center"><asp:textbox id="txtDepositDate" runat="server" cssclass="JobDate" /></td> -->
						<td></td>
						<td></td>
						<td align="right">Deposit</td>
						<td></td>
						<td><asp:textbox id="txtDepositAmt" runat="server" cssclass="JobSubTotal"></asp:textbox></td>
						<td></td>
						<td colspan="2"><asp:textbox id="txtDepositRef" runat="server" /> Reference</td>
					</tr>
                    --%>
					<tr>
						<td colspan="2"></td>
						<td colspan="3">
							<hr />
						</td>
					</tr>
                    <tr>
						<td colspan="2"></td>
						<td align="right"><b>Payments</b></td>
                        <td></td>
                        <td><asp:HiddenField ID="hfPmtNum" runat="server" /></td>
                        <td></td>
						<td><asp:Image ID="imgAddPayment" ImageUrl="Images/Add16.png" BorderWidth="0" AlternateText="Add Payment" ToolTip="Add an additional payment" CssClass="AddPayment" runat="server" /></td>
                    </tr>
				</tbody>
				<tbody id="Payments">
                    <asp:Literal ID="ltlPayments" runat="server" />
				</tbody>
                <tbody>
					<tr>
						<td colspan="2"></td>
						<td colspan="3">
							<hr />
						</td>
                        <%--
						<td></td>
						<td colspan="2">
						 (date, method, check #)
						</td>
                        --%>
					</tr>
					<tr>
						<td colspan="2"></td>
						<td align="right"><b>Invoice Balance</b></td>
						<td></td>
						<td><asp:textbox id="txtInvBalance" runat="server" cssclass="JobSubTotal" Enabled="false"></asp:textbox></td>
					</tr>
<% if (lblPONumber.Text.Length > 0)
    { %>
					<tr>
						<td><img height="10" src="Images/Space.gif" alt="" width="1" /></td>
					</tr>
					<tr>
						<td colspan="2"></td>
						<td align="right">PO #</td>
						<td></td>
						<td><asp:Label ID="lblPONumber" runat="server" /></td>
					</tr>
<% } %>
					<tr>
						<td><img height="20" src="Images/Space.gif" alt="" width="1" /></td>
					</tr>
<%--
					<tr>
						<td></td>
						<td></td>
						<td valign="top" align="right">Invoice Message</td>
						<td></td>
						<td colspan="4"><asp:textbox id="txtInvMessage" runat="server" cssclass="InvMsg" maxlength="200" textmode="MultiLine"></asp:textbox></td>
						<td valign="top"><asp:image id="ddInvMessage" runat="server" imagealign="NotSet" imageurl="Images/DropDown.gif"></asp:image></td>
					</tr>
					<tr>
						<td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
					</tr>
--%>
					<tr>
						<td align="center" colspan="9">
							<table cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><asp:button id="btnSave" runat="server" text="Save Draft" OnClick="btnSave_Click" ToolTip="Ready to save changes, but not print an invoice yet." /></td>
									<td><asp:button id="btnSaveFinal" runat="server" text="Save Final" OnClick="btnSaveFinal_Click" ToolTip="Job is complete, ready to print or email the invoice." /></td>
									<td><img height="1" src="Images/Space.gif" alt="" width="30" /></td>
									<td><input id="btnPrint" type="button" value="Print" /></td>
									<td><img height="20" src="Images/Space.gif" alt="" width="10" /></td>
									<td>
										<asp:checkbox id="chkCustomerCopy" runat="server" text="Customer Copy"></asp:checkbox><br />
										<asp:checkbox id="chkFileCopy" runat="server" text="File Copy"></asp:checkbox>
									</td>
									<td><img height="20" src="Images/Space.gif" alt="" width="20" /></td>
									<td><input id="btnEmail" type="button" value="Email" title="Email the invoice to the customer." runat="server" /><asp:HiddenField ID="hfEmail" Value="false" runat="server" /></td>
									<td><img height="20" src="Images/Space.gif" alt="" width="20" /></td>
									<td><input id="btnDepositInvoice" type="button" value="Deposit Invoice" title="Prepare a deposit invoice." runat="server" /></td>
								</tr>
                                <tr>
                                    <td class="InvDtTm"><asp:Label ID="lblSaveDt" runat="server" /></td>
                                    <td class="InvDtTm"><asp:Label ID="lblSaveFinalDt" runat="server" /></td>
                                    <td></td>
                                    <td class="InvDtTm"><asp:Label ID="lblPrintDt" runat="server" /></td>
                                    <td colspan="3"></td>
                                    <td class="InvDtTm"><asp:Label ID="lblEmailDt" runat="server" /></td>
                                    <td></td>
                                    <td class="InvDtTm"><asp:Label ID="lblDepositDt" runat="server" /></td>
                                </tr>
							</table>
						</td>
					</tr>
				</tbody>					
			</table>
			<hr />
			<table cellspacing="0" cellpadding="0" align="center" border="0">
<%--
				<tr>
					<td></td>
					<td><img height="10" src="Images/Space.gif" alt="" width="10"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
				</tr>
				<tr>
					<td align="right">Invoice Delivery Method</td>
					<td></td>
					<td><asp:textbox id="txtInvDeliveryMethod" runat="server" cssclass="JobEmail" maxlength="20"></asp:textbox><asp:image id="ddInvDeliveryMethod" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif"></asp:image></td>
					<td></td>
					<td align="right">Date</td>
					<td></td>
					<td><asp:textbox id="txtInvDeliveryDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgInvDeliveryDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif"></asp:image></td>
				</tr>
				<tr>
					<td align="right">Email</td>
					<td></td>
					<td><asp:textbox id="txtInvEmail" runat="server" cssclass="JobEmail" maxlength="80"></asp:textbox></td>
					<td></td>
					<td align="right">Fax #</td>
					<td></td>
					<td><asp:textbox id="txtInvFax" runat="server" cssclass="JobPhone" maxlength="30"></asp:textbox></td>
				</tr>
				<tr>
					<td></td>
					<td><img height="10" src="Images/Space.gif" alt="" width="10"></td>
				</tr>
			</table>
			<hr>
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td></td>
					<td><img height="10" src="Images/Space.gif" alt="" width="10"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
				</tr>
				<tr>
					<td valign="top" align="right">Payment Message</td>
					<td></td>
					<td colspan="5"><asp:textbox id="txtPaymentMsg" runat="server" cssclass="InvMsg" maxlength="200" textmode="MultiLine"></asp:textbox><asp:image id="ddPaymentMsg" runat="server" imagealign="Top" imageurl="Images/DropDown.gif"></asp:image></td>
				</tr>
				<tr>
					<td align="right">Payment Date</td>
					<td><img height="1" src="Images/Space.gif" alt="" width="5"></td>
					<td><asp:textbox id="txtInvPmtDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgInvPmtDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td align="right">Payment Method</td>
					<td></td>
					<td><asp:textbox id="txtInvPmtMethod" runat="server" cssclass="JobSubTotal" maxlength="20"></asp:textbox><asp:image id="ddInvPmtMethod" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image></td>
					<td></td>
					<td align="right">Check #</td>
					<td><img height="1" src="Images/Space.gif" alt="" width="5"></td>
					<td><asp:textbox id="txtCheckRef" runat="server" cssclass="JobSubTotal" maxlength="50"></asp:textbox></td>
				</tr>
--%>
			</table>
<%
    /*
			<table style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; MARGIN: 10px auto; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid;"
				cellspacing="0" cellpadding="5" align="center" border="0">
				<tr>
					<td>
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
								<td></td>
								<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
								<td></td>
								<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
								<td></td>
								<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
							</tr>
							<tr>
								<td colspan="4"><b>For CC Payments:</b></td>
							</tr>
							<tr>
								<td></td>
								<td align="right">CC Type</td>
								<td></td>
								<td><asp:textbox id="txtCCType" runat="server" cssclass="JobSubTotal" maxlength="20"></asp:textbox><asp:image id="ddCCType" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image></td>
								<td></td>
								<td align="right">Card #</td>
								<td></td>
								<td><asp:textbox id="txtCCNum" runat="server" cssclass="CardNum" maxlength="20"></asp:textbox><label id="InvalidCCNum">Invalid credit card number</label></td>
							</tr>
							<tr>
								<td></td>
								<td align="right">Expires</td>
								<td></td>
								<td><asp:textbox id="txtCCExpDt" runat="server" cssclass="JobDate" maxlength="6"></asp:textbox>&nbsp;mm/yy</td>
								<td></td>
								<td align="right">Security Code</td>
								<td></td>
								<td><asp:textbox id="txtCCSecCode" runat="server" cssclass="JobSubTotal" maxlength="6"></asp:textbox></td>
							</tr>
							<tr>
								<td></td>
								<td align="right">Name on Card</td>
								<td></td>
								<td><asp:textbox id="txtCCName" runat="server" cssclass="JobEmail" maxlength="50"></asp:textbox></td>
							</tr>
							<tr>
								<td></td>
								<td align="right">Street Addr</td>
								<td></td>
								<td><asp:textbox id="txtCCStreet" runat="server" cssclass="JobEmail" maxlength="50"></asp:textbox></td>
								<td></td>
								<td align="right">Zip Code</td>
								<td></td>
								<td><asp:textbox id="txtCCZip" runat="server" cssclass="JobSubTotal" maxlength="15"></asp:textbox></td>
							</tr>
< %--
							<tr>
								<td colspan="8">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="1" src="Images/Space.gif" alt="" width="150"></td>
											<td>
												<hr width="250">
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan="4"><b>From the CC Receipt:</b></td>
							</tr>
							
							<tr>
								<td></td>
								<td align="right">Batch ID</td>
								<td></td>
								<td><asp:textbox id="txtCCBatchID" runat="server" cssclass="JobSubTotal" maxlength="10"></asp:textbox></td>
								<td></td>
								<td align="right">Invoice #</td>
								<td></td>
								<td><asp:textbox id="txtCCInvNum" runat="server" cssclass="JobSubTotal" maxlength="10"></asp:textbox></td>
							</tr>
							<tr>
								<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
							</tr>
							<tr>
								<td></td>
								<td align="right">Appr Code</td>
								<td></td>
								<td><asp:textbox id="txtCCApprCode" runat="server" cssclass="JobSubTotal" maxlength="10"></asp:textbox></td>
								<td></td>
								<% --<td align="right">AVS Resp</td>-- % >
								<td align="right">Settle Date</td>
								<td></td>
								<% --<td><asp:textbox id="txtCCAVSResp" runat="server" cssclass="JobSubTotal" maxlength="10"></asp:textbox></td>-- % >
								<td><asp:textbox id="txtCCSettleDate" runat="server" cssclass="JobDate" maxlength="10"></asp:textbox></td>
							</tr>
							<tr>
								<td colspan="8">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="1" src="Images/Space.gif" alt="" width="212"></td>
											<td>
												<table cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td align="right">
															<hr width="50">
														</td>
														<td align="center" width="30">Or</td>
														<td align="left">
															<hr width="50">
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td></td>
								<td align="right"></td>
								<td></td>
								<td><asp:checkbox id="chkFailedFlg" runat="server" text="Declined"></asp:checkbox></td>
								<td></td>
								<td align="right">Message</td>
								<td></td>
								<td><asp:textbox id="txtFailedMsg" runat="server" cssclass="JobEmail" maxlength="50"></asp:textbox></td>
							</tr>
--% >							
						</table>
					</td>
				</tr>
			</table>
*/     
 %>
<%
/*
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td rowspan="2"><asp:button id="btnSave1" runat="server" text="Save" OnClick="btnSave_Click" ToolTip="Ready to save changes, but not print an invoice yet." /></td>
					<td rowspan="2"><img height="1" src="Images/Space.gif" alt="" width="30"></td>
					<td rowspan="2"><input id="btnPrint1" onclick="Print();" type="button" value="Print"></td>
					<td><img height="20" src="Images/Space.gif" alt="" width="10"></td>
					<td><asp:checkbox id="chkCustomerCopy1" runat="server" text="Customer Copy"></asp:checkbox></td>
				</tr>
				<tr>
					<td></td>
					<td><asp:checkbox id="chkFileCopy1" runat="server" text="File Copy"></asp:checkbox></td>
				</tr>
			</table>
*/%>            
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td></td>
					<td><img height="20" src="Images/Space.gif" alt="" width="10" /></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="5" /></td>
				</tr>
				<tr>
					<td align="right">Invoice Created</td>
					<td></td>
					<td><asp:textbox id="txtInvCreatedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
					<td></td>
					<td><asp:textbox id="txtInvCreatedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
				</tr>
				<tr>
					<td align="right">Invoice Updated</td>
					<td></td>
					<td><asp:textbox id="txtInvUpdatedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
					<td></td>
					<td><asp:textbox id="txtInvUpdatedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
				</tr>
			</table>
            <br />
			<input id="txtDirty" type="hidden" value="false" name="txtDirty" runat="server" />
			<% Pg.Bottom(); %>		
			
			<div id="EmailDialog" class="Hide">
				<dl>
					<dt>Email To</dt>
					<dd><asp:TextBox ID="txtEmail" runat="server" /></dd>
					<dt>Subject</dt>
					<dd><asp:TextBox ID="txtSubject" runat="server" /></dd>
					<dt>Message</dt>
					<dd><asp:TextBox ID="txtMessage" TextMode="MultiLine" runat="server" /></dd>
				</dl>
			</div>

            <div id="DepositInvoiceDialog" class="Hide">
                <dl>
                    <dt>Deposit Amount</dt>
                    <dd>
                        <asp:TextBox ID="txtDepositAmt" CssClass="PmtAmt" runat="server" />                        
                    </dd>
                    <dt>&nbsp;</dt>
                    <dd />
                    <dt>Printed</dt>
                    <dd>
                        <asp:textbox id="txtDepositInvPrintedDtTm" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox>
                        <asp:textbox id="txtDepositInvPrintedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox>
                    </dd>
                    <dt>Emailed</dt>
                    <dd>
                        <asp:textbox id="txtDepositInvEmailedDtTm" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox>
                        <asp:textbox id="txtDepositInvEmailedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox>
                    </dd>
                </dl>
            </div>
		</form>
		<%//=Utils.SelectList.DefValues()%>
		<%=calInvDate.DefCalendar()%>
<%--
		<%=calInvDeliveryDate.DefCalendar()%>
		<%=calInvPmtDate.DefCalendar()%>
--%>
	    <div id="PriceDialog" class="Hide">
			<input type="hidden" id="Seq" />
			<input type="hidden" id="Rno" />
	        <div>
	            <input type="radio" id="rdoPerPerson" name="rdoPriceType" data-selector="dlPerPerson" data-price-type="<%= Misc.cnPerPerson %>" /><label for="rdoPerPerson"><%= Misc.cnPerPerson%></label> 
	            <input type="radio" id="rdoPerItem" name="rdoPriceType" data-selector="dlPerItem" data-price-type="<%= Misc.cnPerItem %>" /><label for="rdoPerItem"><%= Misc.cnPerItem%></label>
	            <input type="radio" id="rdoAllIncl" name="rdoPriceType" data-selector="dlAllIncl" data-price-type="<%= Misc.cnAllIncl %>" /><label for="rdoAllIncl"><%= Misc.cnAllIncl%></label>
		    </div>
		    <div class="SectionBreak"></div>
	        <dl id="dlPerPerson" class="Hide">
	            <dt></dt>
	            <dd><b>Per Person</b></dd>
	            <dt>Servings</dt>
	            <dd><input type="text" id="txtPerPersonServings" class="Count" data-default="<%= DefaultServings %>" /></dd>
	            <dt>Description</dt>
	            <dd><input type="text" id="txtPerPersonDesc" class="Desc" /></dd>
	            <dt>Price Per Person</dt>
	            <dd><input type="text" id="txtPerPersonPrice" class="Price" /></dd>
		    </dl>
	        <dl id="dlPerItem" class="Hide">
	            <dt></dt>
	            <dd><b>Per Item</b></dd>
	            <dt>Quantity</dt>
	            <dd><input type="text" id="txtPerItemQuantity" class="Count" /></dd>
	            <dt>Description</dt>
	            <dd><input type="text" id="txtPerItemDesc" class="Desc" /></dd>
	            <dt>Price Per Item</dt>
	            <dd><input type="text" id="txtPerItemPrice" class="Price" /></dd>
		    </dl>
	        <dl id="dlAllIncl" class="Hide">
	            <dt></dt>
	            <dd><b>All Inclusive</b></dd>
	            <dt>Description</dt>
	            <dd><input type="text" id="txtAllInclDesc" class="Desc" /></dd>
	            <dt>Price</dt>
	            <dd><input type="text" id="txtAllInclPrice" class="Price" /></dd>
		    </dl>
	    </div>		

	    <div id="Payment" class="Hide" title="Payment">
            <input type="hidden" id="txtPaymentRno" />
            <input type="hidden" id="txtPmtNum" />
            <dl>
                <dt>For</dt>
                <dd>
                    <input id="rdoDeposit" type="radio" name="PmtFor" value="Deposit" /><label for="rdoDeposit">Deposit</label>
                    <input id="rdoFinal" type="radio" name="PmtFor" value="Final" /><label for="rdoFinal">Final</label>
                    <input id="rdoOther" type="radio" name="PmtFor" value="Other" /><label for="rdoOther">Other</label>
                </dd>
                <dt>Amount</dt>
                <dd><input id="txtPmtAmt" type="text" class="PmtAmt" /></dd>
                <dt>Date</dt>
                <dd><input id="txtPmtDate" type="text" class="PmtDate" /></dd>
                <dt>Method</dt>
                <dd>
                    <input id="rdoCheck" type="radio" name="PmtMethod" value="Check" /><label for="rdoCheck">Check</label>
                    <input id="rdoCash" type="radio" name="PmtMethod" value="Cash" /><label for="rdoCash">Cash</label>
                    <input id="rdoCreditCard" type="radio" name="PmtMethod" value="CreditCard" /><label for="rdoCreditCard">Credit Card</label>
                    <input id="rdoOtherMethod" type="radio" name="PmtMethod" value="Other" /><label for="rdoOtherMethod">Other</label>
                </dd>
                <dt id="lblReference">Reference</dt>
                <dd><input id="txtPmtRef" type="text" class="PmtRef" /></dd>
                <dt>Purchase Order</dt>
                <dd><input id="txtPurchOrder" type="text" class="PmtRef" /></dd>
                <dt class="PmtCC">CC Type</dt>
                <dd class="PmtCC"><input id="txtCCType" type="text" class="CCShort" /></dd>
                <dt class="PmtCC">Note:</dt>
                <dd class="PmtCC" style="font-size: 0.9em; display: inline-block; color: #888; margin-top: 6px; margin-bottom: 8px;">Fields populated after CC processing.</dd>
                <dt class="PmtCC">Status</dt>
                <dd class="PmtCC"><input id="txtCCStatus" type="text" class="CCShort" /></dd>
                <dt class="PmtCC">Auth</dt>
                <dd class="PmtCC"><input id="txtCCAuth" type="text" class="CCShort" /></dd>
                <dt class="PmtCC">Result</dt>
                <dd class="PmtCC"><input id="txtCCResult" type="text" class="CCLong" /></dd>
                <dt class="PmtCC">CC Reference</dt>
                <dd class="PmtCC"><input id="txtCCRef" type="text" class="CCLong" /></dd>
                <dt class="PmtCC">Charged</dt>
                <dd class="PmtCC"><label id="lblCCPmtInfo" class="Label" /></dd>
                <dt class="PmtCC">Printed</dt>
                <dd class="PmtCC"><label id="lblCCPrintInfo" class="Label" /></dd>
            </dl>
        </div>
        <div class="Hide">
            <form action="<%= USAePayUrl %>/<%= USAePayKey %>">
                <input type="hidden" name="UMkey" value="<%= USAePayKey %>">
                <input type="hidden" name="UMcommand" value="sale">
                <input type="hidden" name="UMamount" id="UMamount" value="">
                <input type="hidden" name="UMinvoice" id="UMinvoice" value="<%= JobRno %>">
                <input type="hidden" name="UMdescription" value="<%= lblCustomer.Text %>">
                <input type="hidden" name="UMhash" value="#">
                <input type="hidden" name="UMclerk" value="<%= Globals.g.User %>">
                <input type="hidden" name="UMcustemail" value="customer@petesoft.com">
                <input type="hidden" name="UMcustreceipt" value="Yes">
                <input type="submit" id="CCPayment" value="Continue to Payment Form">
            </form>
        </div>
	</body>
</html>

<script type="text/javascript">
    //----------------------------------------------------------------------------		
    function AddOrEditPrice()
	{
		if ($(this).hasClass("EditPrice"))
		{
			var tr = $(this).closest("tr");
			switch (tr.find(".PriceType").val())
			{
				case "<%= Misc.cnPerPerson %>":
					$("#rdoPerPerson").prop("checked", true).change();
					break;

				case "<%= Misc.cnPerItem %>":
					$("#rdoPerItem").prop("checked", true).change();
					break;

				case "<%= Misc.cnAllIncl %>":
					$("#rdoAllIncl").prop("checked", true).change();
					break;
			}
			$("#PriceDialog #Seq").val(tr.find(".Seq").val());
			$("#PriceDialog #Rno").val(tr.find(".Rno").val());
			$("#PriceDialog .Count").val(tr.find(".Count").val());
			$("#PriceDialog .Desc").val(tr.find(".Desc").val());
			$("#PriceDialog .Price").val(tr.find(".Price").val()).formatCurrency();
		}
		else
		{
			$("#PriceDialog #Seq").val("");
			$("#PriceDialog #Rno").val("");
			$("#PriceDialog input[type=radio]").prop("checked", false);
			$(".Count, .Desc, .Price", $("#PriceDialog")).val("");
			$("#dlPerPerson, #dlPerItem, #dlAllIncl").hide();
			var PerPersonCount = $("#dlPerPerson .Count");
			PerPersonCount.val(PerPersonCount.data("default"));
		}

		$("#PriceDialog").dialog
        ({
        	buttons:
            {
            	Save: function ()
            	{
            		var Count = 0;
            		var Desc = "";
            		var Price = 0;

            		var Selected = $("#PriceDialog input[type=radio]:checked");
            		var PriceType = Selected.data("price-type");
            		var dl = Selected.data("selector");
            		Count = $("#" + dl + " .Count").asNumber({ parseType: "Int" });
            		Desc = $("#" + dl + " .Desc").val();
            		Price = $("#" + dl + " .Price").asNumber({ parseType: "Float" });

            		var Seq = $(this).find("#Seq").val();
            		var Rno = $(this).find("#Rno").val();
            		if (Seq == "")
            		{
            			AddNewPrice(PriceType, Count, Desc, Price);
            		}
            		else
            		{
            			var tr = $("#AddlPricing .Seq[value='" + Seq + "']").closest("tr");
            			SetPrice(tr, Rno, PriceType, Count, Desc, Price);
            		}

            		CalcSubTotal();
            		SetDirty();

            		$(this).dialog("close");
            	},
            	close: function () { $(this).dialog("close"); }
            },
        	minWidth: 380,
        	modal: true,
        	title: "Add Price",
        	close: function ()
        	{
        		$("#AddlPricing").off("click", ".DeletePrice", DeletePrice); // for some reason the delete is called twice after the popup is triggered, unless this code is here
        		//$("#AddlPricing").on("click", ".DeletePrice", DeletePrice);
        	}
        });
	}
	
    //----------------------------------------------------------------------------		
    function AddNewPrice(PriceType, Count, Desc, Price)
	{
		var CopyNewPrice = NewPrice.replace(/{n}/g, ++iPrice);
		var tr = $("#AddlPricing").append(CopyNewPrice).find("tr").last();
		SetPrice(tr, "", PriceType, Count, Desc, Price);
	}

	//----------------------------------------------------------------------------		
	function SetPrice(jqTr, Rno, PriceType, Count, Desc, Price)
	{
	    var ExtDesc = "";
	    var ExtPrice = Count * Price;
		
		switch (PriceType)
		{
			case "<%= Misc.cnPerPerson %>":
				ExtDesc = FmtNum(Count) + " " + Desc + " @ " + FmtDollar(Price) + " per person";
				break;

			case "<%= Misc.cnPerItem %>":
				ExtDesc = FmtNum(Count) + " " + Desc + " @ " + FmtDollar(Price) + " per item";
				break;

			case "<%= Misc.cnAllIncl %>":
				Count = 1;
				ExtDesc = "All inclusive " + Desc + " @ " + FmtDollar(Price);
				ExtPrice = AllInclusivePrice(Price);
				break;
		}

		$(".ExtDesc", jqTr).html(ExtDesc);
		$(".DispExtPrice", jqTr).val(ExtPrice).formatCurrency();
		$(".Rno", jqTr).val(Rno);
		$(".PriceType", jqTr).val(PriceType);
		$(".Count", jqTr).val(Count);
		$(".Price", jqTr).val(Price);
		$(".ExtPrice", jqTr).val(ExtPrice);
		$(".Desc", jqTr).val(Desc);
	}
	
	var NewPrice =
		"<tr>\n" +
		"<td colspan=\"3\" align=\"right\" class=\"ExtDesc\"></td>\n<td></td>\n" +
		"<td><input type=\"text\" value=\"\" disabled=\"disabled\" class=\"JobSubTotal DispExtPrice\" /></td>\n<td></td>\n" +
		"<td>\n" +
		"<img src=\"Images/Notepage16.png\" class=\"EditPrice\" alt=\"Edit Price\" title=\"Edit Price\" />\n" +
		"<img src=\"Images/Delete16.png\" class=\"DeletePrice\" alt=\"Delete Price\" title=\"Delete Price\" />\n" +
		"<input type=\"hidden\" value=\"{n}\" name=\"Seq{n}\" class=\"Seq\" />\n" +
		"<input type=\"hidden\" value=\"\" name=\"Rno{n}\" class=\"Rno\" />\n" +
		"<input type=\"hidden\" value=\"\" name=\"PriceType{n}\" class=\"PriceType\" />\n" +
		"<input type=\"hidden\" value=\"\" name=\"Count{n}\" class=\"Count\" />\n" +
		"<input type=\"hidden\" value=\"\" name=\"Price{n}\" class=\"Price\" />\n" +
		"<input type=\"hidden\" value=\"\" name=\"ExtPrice{n}\" class=\"ExtPrice\" />\n" +
		"<input type=\"hidden\" value=\"\" name=\"Desc{n}\" class=\"Desc\" />\n" +
		"<input type=\"hidden\" value=\"0\" name=\"Delete{n}\" class=\"Delete\" />\n" +
		"</td>\n" +
		"</tr>\n";

	//----------------------------------------------------------------------------		
	function DeletePrice()
	{
		if (confirm("Are you sure?"))
		{
			$(this).closest("tr").find(".Delete").val("1");
			$(this).closest("tr").hide();
			CalcSubTotal();
			SetDirty();
		}
	}

    //----------------------------------------------------------------------------		
	function AllInclusivePrice(Price)
	{
	    var TaxPct = parseFloat($("#txtSubTotTaxPct").val());
	    var fSalesTaxExempt  = $("#chkSalesTaxExempt")[0].checked;
	    if (fSalesTaxExempt)
	    {
	        TaxPct = 0;
	    }
	    var Tax = Price * (1 - 1 / (1 + TaxPct / 100));

	    return Price - Tax;
	}

	//----------------------------------------------------------------------------		
	function Email()
	{
	    if (fDirty)
	    {
	        alert("Some data has changed, please save before emailing.");
	    }
	    else
	    {
	        $("#EmailDialog").dialog
		    ({
		        title: "Email Invoice",
		        buttons:
			    {
			        Send: function ()
			        {
			            $(this).dialog("close");
			            $("form").append($(this).detach());
			            $("#hfEmail").val(true);
			            $("#btnSave").click();
			        },

			        Cancel: function ()
			        {
			            $(this).dialog("close");
			        }
			    },
		        minWidth: 630,
		        modal: true
		    });
	    }
	}
	
	//----------------------------------------------------------------------------		
	function DepositInvoice()
	{
	    if (fDirty)
	    {
	        alert("Some data has changed, please save before doing a deposit invoice.");
	    }
	    else
	    {
	        $("#DepositInvoiceDialog").dialog
		    ({
		        title: "Deposit Invoice",
		        buttons:
			    {
			        Print: function ()
			        {
			            $("#hfDepositAmt").val($("#txtDepositAmt").val());      // save new value
			            $(this).dialog("close");
			            window.open(
                            "Invoices.aspx?JobRno=" + oGetStr("lblJobRno") + 
                            "&CustomerCopy=" + (iGetChk("chkCustomerCopy") ? "1" : "0") +
                            "&FileCopy=" + (iGetChk("chkFileCopy") ? "1" : "0") +
                            "&FinalPrint=0" +
                            "&DepositInvoice=1" +
                            "&DepositAmt=" + $("#hfDepositAmt").asNumber({ parseType: "Float" }), 
                            "_blank");
			            $("#btnSave").click();
			        },

			        Email: function ()
			        {
			            $("#hfDepositAmt").val($("#txtDepositAmt").val());      // save new value
			            $("#hfDepositInvoice").val("true");
			            $(this).dialog("close");
			            var fPrevDirty = fDirty;
			            ClearDirty();
			            $("#txtSubject").val("Deposit Invoice from Marvellous Catering");
			            $("#txtMessage").val("Please see the attached deposit invoice from Marvellous Catering.\n\nWe appreciate your business.");
			            Email();
			            fDirty = fPrevDirty;
			        },

			        Save: function ()
			        {
			            $("#hfDepositAmt").val($("#txtDepositAmt").val());      // save new value
			            $(this).dialog("close");
			        },

			        Cancel: function ()
			        {
			            $("#txtDepositAmt").val($("#hfDepositAmt").val());      // save new value
			            $(this).dialog("close");
			        }
			    },
		        minWidth: 360,
		        modal: true
		    });
	    }
	}
	
	//----------------------------------------------------------------------------		
    function Init()
    {
        RestoreDirty();

	    <%=SetFocus()%>

        SetDisplay("divReprint", fPrinted);
	
        CheckEventCount();
        CheckPricePerPerson();
	
        var fPrintEnabled = (iGetStr("txtInvCreatedDt") == "");
        iSetDis("btnPrint", fPrintEnabled);
        iSetDis("btnPrint1", fPrintEnabled);

        iSetDis("btnEmail", fPrintEnabled);
	
        $("#hfEmail").val(false);
    }

    //----------------------------------------------------------------------------		
    function CheckEventCount()
    {
        //var Class = "";
        var Title = "";
	
        $("#txtInvEventCount").removeClass("JobSubTotalNotSame");
	
        var InvEventCount = iGetNum("txtInvEventCount");
        if (InvEventCount != EventCount)
        {
            //alert("Warning: The Event Count doesn't match Job Count - " + EventCountInfo + ".");
            //Class = "JobSubTotalNotSame";
            $("#txtInvEventCount").addClass("JobSubTotalNotSame");
            Title = EventCountInfo;
        }

        //SetClass("txtInvEventCount", Class);
        SetTitle("txtInvEventCount", Title);

        CalcSubTotal();
    }

    //----------------------------------------------------------------------------
    function CheckPricePerPerson()
    {
        //var Class = "JobSubTotal";
        var Title = "";
	
        $("#txtInvPricePerPerson").removeClass("JobSubTotalNotSame");
	
        var InvPricePerPerson = iGetNum("txtInvPricePerPerson");
        if (InvPricePerPerson != PricePerPerson)
        {
            //alert("Warning: The price per person on the job is " + FmtDollar(PricePerPerson) + ".");
            //Class = "JobSubTotalNotSame";
            $("#txtInvPricePerPerson").addClass("JobSubTotalNotSame");
            Title = "Job Price Per Person is " + FmtDollar(PricePerPerson);
        }

        //SetClass("txtInvPricePerPerson", Class);
        SetTitle("txtInvPricePerPerson", Title);
	
        CalcSubTotal();
    }

    //----------------------------------------------------------------------------		
    function CalcSubTotal()
    {
        if (!fCalcTotals) return;

        // double check "All Inclusive" prices in case the sales tax has changed
        $("input.PriceType").each (function ()
        {
            if ($(this).val() == "All Inclusive")
            {
                var tr       = $(this).closest("tr");
                var Price    = Num($(".Price", tr).val());
                var ExtPrice = AllInclusivePrice(Price);

                $(".DispExtPrice", tr).val(ExtPrice).formatCurrency();
                $(".ExtPrice", tr).val(ExtPrice);
            }
        });

        var Subtotal = 0;
        $(".ExtPrice").each(function ()
        {
            // check to see if it is deleted
            if ($(this).closest("tr").find(".Delete").val() != "1")
            {
                Subtotal += Num($(this).val());
            }
        });
	
        iSetStr("txtInvSubTotal", FmtDollar(Subtotal));
        iSetStr("txtSubTotal", Subtotal);

        InvoiceTotal();
    }

    //----------------------------------------------------------------------------		
    function ChangePct(PctField, AmtFieldName)
    {
        var SubTotal = iGetNum("txtInvSubTotal");
        var Pct = Num(PctField.value);
        iSetStr(AmtFieldName, FmtDollar(NumRnd(SubTotal * Pct / 100), 2));
        iSetStr(PctField.id + "Flg", "true");
        InvoiceTotal();
    }

    //----------------------------------------------------------------------------		
    function ChangeAmt(AmtField, PctFieldName)
    {
        var SubTotal = iGetNum("txtInvSubTotal");
        var Amt = Num(AmtField.value);
        var Pct = FmtNum(Amt / SubTotal * 100, 3)
        if (Pct != "") { Pct += "%"; }
        iSetStr(PctFieldName, Pct);
        iSetStr(PctFieldName + "Flg", "false");
        InvoiceTotal();
    }

    var FinalPaymentAmt = 0;

    //----------------------------------------------------------------------------		
    function InvoiceTotal()
    {
        if (!fCalcTotals) return;

        var ServiceSubTotPctFlg             = (iGetStr("txtServiceSubTotPctFlg")           == "true");
        var DeliverySubTotPctFlg            = (iGetStr("txtDeliverySubTotPctFlg")          == "true");
        var ChinaSubTotPctFlg               = (iGetStr("txtChinaSubTotPctFlg")             == "true");
        var AddServiceSubTotPctFlg          = (iGetStr("txtAddServiceSubTotPctFlg")        == "true");
        var FuelTravelSubTotPctFlg          = (iGetStr("txtFuelTravelSubTotPctFlg")        == "true");
        var FacilitySubTotPctFlg            = (iGetStr("txtFacilitySubTotPctFlg")          == "true");
        var GratuitySubTotPctFlg            = (iGetStr("txtGratuitySubTotPctFlg")          == "true");
        var VoluntaryGratuitySubTotPctFlg   = (iGetStr("txtVoluntaryGratuitySubTotPctFlg") == "true");
        var RentalsSubTotPctFlg             = (iGetStr("txtRentalsSubTotPctFlg")           == "true");
        var Adj1SubTotPctFlg                = (iGetStr("txtAdj1SubTotPctFlg")              == "true");
        var Adj2SubTotPctFlg                = (iGetStr("txtAdj2SubTotPctFlg")              == "true");
	
        var ServiceSubTotPct            = iGetNum("txtServiceSubTotPct");
        var DeliverySubTotPct           = iGetNum("txtDeliverySubTotPct");
        var ChinaSubTotPct              = iGetNum("txtChinaSubTotPct");
        var AddServiceSubTotPct         = iGetNum("txtAddServiceSubTotPct");
        var FuelTravelSubTotPct         = iGetNum("txtFuelTravelSubTotPct");
        var FacilitySubTotPct           = iGetNum("txtFacilitySubTotPct");
        var GratuitySubTotPct           = iGetNum("txtGratuitySubTotPct");
        var VoluntaryGratuitySubTotPct  = iGetNum("txtVoluntaryGratuitySubTotPct");
        var RentalsSubTotPct            = iGetNum("txtRentalsSubTotPct");
        var Adj1SubTotPct               = iGetNum("txtAdj1SubTotPct");
        var Adj2SubTotPct               = iGetNum("txtAdj2SubTotPct");
        var CCFeePct                    = iGetNum("txtCCFeePct");
	
        var SubTotalAmt          = iGetNum("txtSubTotal");
        var ServiceAmt           = iGetNum("txtServiceAmt");
        var DeliveryAmt          = iGetNum("txtDeliveryAmt");
        var ChinaAmt             = iGetNum("txtChinaAmt");
        var AddServiceAmt        = iGetNum("txtAddServiceAmt");
        var FuelTravelAmt        = iGetNum("txtFuelTravelAmt");
        var FacilityAmt          = iGetNum("txtFacilityAmt");
        var GratuityAmt          = iGetNum("txtGratuityAmt");
        var VoluntaryGratuityAmt = iGetNum("txtVoluntaryGratuityAmt");
        var RentalsAmt           = iGetNum("txtRentalsAmt");
        var Adj1Amt              = iGetNum("txtAdj1Amt");
        var Adj2Amt              = iGetNum("txtAdj2Amt");
        var CCFeeAmt             = iGetNum("txtCCFeeAmt");

        var fSalesTaxExempt  = $("#chkSalesTaxExempt")[0].checked;
        var SubTotalTaxPct   = iGetNum("txtSubTotTaxPct");
        var ServiceTaxPct    = iGetNum("txtServiceTaxPct");
        var DeliveryTaxPct   = iGetNum("txtDeliveryTaxPct");
        var ChinaTaxPct      = iGetNum("txtChinaTaxPct");
        var AddServiceTaxPct = iGetNum("txtAddServiceTaxPct");
        var FuelTravelTaxPct = iGetNum("txtFuelTravelTaxPct");
        var FacilityTaxPct   = iGetNum("txtFacilityTaxPct");
        var GratuityTaxPct   = iGetNum("txtGratuityTaxPct");
        var RentalsTaxPct    = iGetNum("txtRentalsTaxPct");
        var Adj1TaxPct       = iGetNum("txtAdj1TaxPct");
        var Adj2TaxPct       = iGetNum("txtAdj2TaxPct");
        var CCFeeTaxPct      = iGetNum("txtCCFeeTaxPct");
	
        if (ServiceSubTotPctFlg)
        {
            ServiceAmt = NumRnd(ServiceSubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtServiceAmt", FmtDollar(ServiceAmt, ""));
        }
        else
        {
            ServiceSubTotPct = (SubTotalAmt != 0 ? ServiceAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtServiceSubTotPct", FmtPct(ServiceSubTotPct, 3));
        }

        if (DeliverySubTotPctFlg)
        {
            DeliveryAmt = NumRnd(DeliverySubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtDeliveryAmt", FmtDollar(DeliveryAmt, ""));
        }
        else
        {
            DeliverySubTotPct = (SubTotalAmt != 0 ? DeliveryAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtDeliverySubTotPct", FmtPct(DeliverySubTotPct, 3));
        }

        if (ChinaSubTotPctFlg)
        {
            ChinaAmt = NumRnd(ChinaSubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtChinaAmt", FmtDollar(ChinaAmt, ""));
        }
        else
        {
            ChinaSubTotPct = (SubTotalAmt != 0 ? ChinaAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtChinaSubTotPct", FmtPct(ChinaSubTotPct, 3));
        }

        if (AddServiceSubTotPctFlg)
        {
            AddServiceAmt = NumRnd(AddServiceSubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtAddServiceAmt", FmtDollar(AddServiceAmt, ""));
        }
        else
        {
            AddServiceSubTotPct = (SubTotalAmt != 0 ? AddServiceAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtAddServiceSubTotPct", FmtPct(AddServiceSubTotPct, 3));
        }

        if (FuelTravelSubTotPctFlg)
        {
            FuelTravelAmt = NumRnd(FuelTravelSubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtFuelTravelAmt", FmtDollar(FuelTravelAmt, ""));
        }
        else
        {
            FuelTravelSubTotPct = (SubTotalAmt != 0 ? FuelTravelAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtFuelTravelSubTotPct", FmtPct(FuelTravelSubTotPct, 3));
        }

        if (FacilitySubTotPctFlg)
        {
            FacilityAmt = NumRnd(FacilitySubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtFacilityAmt", FmtDollar(FacilityAmt, ""));
        }
        else
        {
            FacilitySubTotPct = (SubTotalAmt != 0 ? FacilityAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtFacilitySubTotPct", FmtPct(FacilitySubTotPct, 3));
        }
	
        if (GratuitySubTotPctFlg)
        {
            GratuityAmt = NumRnd(GratuitySubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtGratuityAmt", FmtDollar(GratuityAmt, ""));
        }
        else
        {
            GratuitySubTotPct = (SubTotalAmt != 0 ? GratuityAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtGratuitySubTotPct", FmtPct(GratuitySubTotPct, 3));
        }

        if (VoluntaryGratuitySubTotPctFlg)
        {
            VoluntaryGratuityAmt = NumRnd(VoluntaryGratuitySubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtVoluntaryGratuityAmt", FmtDollar(VoluntaryGratuityAmt, ""));
        }
        else
        {
            VoluntaryGratuitySubTotPct = (SubTotalAmt != 0 ? VoluntaryGratuityAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtVoluntaryGratuitySubTotPct", FmtPct(VoluntaryGratuitySubTotPct, 3));
        }

        if (RentalsSubTotPctFlg)
        {
            RentalsAmt = NumRnd(RentalsSubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtRentalsAmt", FmtDollar(RentalsAmt, ""));
        }
        else
        {
            RentalsSubTotPct = (SubTotalAmt != 0 ? RentalsAmt / SubTotalAmt * 100 : 0);
            iSetStr("txtRentalsSubTotPct", FmtPct(RentalsSubTotPct, 3));
        }

        if (Adj1SubTotPctFlg)
        {
            Adj1Amt = NumRnd(Adj1SubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtAdj1Amt", FmtDollar(Adj1Amt, ""));
        }
        else
        {
            Adj1SubTotPct = (SubTotalAmt != 0 ? Adj1Amt / SubTotalAmt * 100 : 0);
            iSetStr("txtAdj1SubTotPct", FmtPct(Adj1SubTotPct, 3));
        }

        if (Adj2SubTotPctFlg)
        {
            Adj2Amt = NumRnd(Adj2SubTotPct / 100 * SubTotalAmt, 2);
            iSetStr("txtAdj2Amt", FmtDollar(Adj2Amt, ""));
        }
        else
        {
            Adj2SubTotPct = (SubTotalAmt != 0 ? Adj2Amt / SubTotalAmt * 100 : 0);
            iSetStr("txtAdj2SubTotPct", FmtPct(Adj2SubTotPct, 3));
        }

        //var CCFeeAmt1 = CalcCC(1);
        //var CCFeeAmt2 = CalcCC(2);
        //var CCFeeAmt3 = CalcCC(3);
        //CCFeeAmt = CCFeeAmt1 + CCFeeAmt2 + CCFeeAmt3; 

        var PmtAmt = 0;
        var i;
        for (i = 1; i <= iPmtNum; i++)
        {
            // skip deleted payments
            if ($("#hfDeletePmt" + i).val() != "true")
            {
                PmtAmt += $("#txtPmtAmt" + i).asNumber();
            }
        }

        var PreTaxTotal = 
			    SubTotalAmt + 
			    ServiceAmt + 
			    DeliveryAmt + 
			    ChinaAmt + 
			    AddServiceAmt + 
			    FuelTravelAmt + 
			    FacilityAmt + 
                GratuityAmt + 
                VoluntaryGratuityAmt +
			    RentalsAmt +
			    Adj1Amt + 
			    Adj2Amt;
                //CCFeeAmt;
        var SalesTax = 
            NumRnd(
			    SubTotalAmt   * SubTotalTaxPct   / 100 + 
			    ServiceAmt    * ServiceTaxPct    / 100 + 
			    DeliveryAmt   * DeliveryTaxPct   / 100 + 
			    ChinaAmt      * ChinaTaxPct      / 100 + 
			    AddServiceAmt * AddServiceTaxPct / 100 + 
			    FuelTravelAmt * FuelTravelTaxPct / 100 + 
			    FacilityAmt   * FacilityTaxPct   / 100 + 
			    GratuityAmt   * GratuityTaxPct   / 100 + 
			    RentalsAmt    * RentalsTaxPct    / 100 +
			    Adj1Amt       * Adj1TaxPct       / 100 + 
			    Adj2Amt       * Adj2TaxPct       / 100, 2);
            //NumRnd(CCFeeAmt1  * CCFeeTaxPct      / 100, 2) +
            //NumRnd(CCFeeAmt2  * CCFeeTaxPct      / 100, 2) +
            //NumRnd(CCFeeAmt3  * CCFeeTaxPct      / 100, 2);
        SalesTax = NumRnd(SalesTax, 2);
        if (fSalesTaxExempt)
        {
            SalesTax = 0;
        }

        var InvoiceTotal = PreTaxTotal + SalesTax;
        var InvoiceBalance = InvoiceTotal - PmtAmt;	

        FinalPaymentAmt = InvoiceBalance;

        // doulble check for an additional credit card fee
        //var fChargeCCFee = ($("#chkCCPmtFee").length ? $("#chkCCPmtFee")[0].checked : false);

        //if (fChargeCCFee)
        //{
        //    var Fee = NumRnd(InvoiceBalance * CCFeePct / 100, 2);
        //    var Tax = NumRnd(Fee * CCFeeTaxPct / 100, 2);

        //    if (fSalesTaxExempt)
        //    {
        //        Tax = 0;
        //    }

        //    CCFeeAmt        += Fee;
        //    PreTaxTotal     += Fee;
        //    SalesTax        +=       Tax;
        //    InvoiceTotal    += Fee + Tax;
        //    InvoiceBalance  += Fee + Tax;
        //}
	
        //iSetStr("txtCCFeeAmt", FmtDollar(CCFeeAmt));
        iSetStr("txtPreTaxTotal", FmtDollar(PreTaxTotal));
        iSetStr("txtSalesTaxTotal", FmtDollar(SalesTax));
        iSetStr("txtInvTotal", FmtDollar(InvoiceTotal));
        iSetStr("txtInvBalance", FmtDollar(InvoiceBalance));

        $("#hfInvSubTotal").val($("#txtInvSubTotal").asNumber());
        //$("#hfCCFeeAmt").val($("#txtCCFeeAmt").asNumber());
        $("#hfPreTaxTotal").val($("#txtPreTaxTotal").asNumber());
        $("#hfSalesTaxTotal").val($("#txtSalesTaxTotal").asNumber());
        $("#hfInvTotal").val($("#txtInvTotal").asNumber());
        $("#hfInvBalance").val($("#txtInvBalance").asNumber());
    }

    //----------------------------------------------------------------------------		
    function CalcCC(PmtNum)
    {
        var Fee = 0;

        if ($("#txtPmtMethod" + PmtNum).val() == "CreditCard")
        {
            //var Type   = $("#txtCCType" + PmtNum).val();
            //var Num    = $("#txtCCNum" + PmtNum).val();
            //var ReqAmt = iGetNum("txtCCReqAmt" + PmtNum);
            //var FeePct = iGetNum("txtCCFeePct");
            //var TaxPct = iGetNum("txtCCFeeTaxPct");

            //var fChargeCCFee     = ($("#chkCCPmtFee").length ? $("#chkCCPmtFee")[0].checked : false);
            //var fSalesTaxExempt  = $("#chkSalesTaxExempt")[0].checked;
            //if (!fChargeCCFee)
            //{
            //    FeePct = 0;
            //}
            //if (fSalesTaxExempt)
            //{
            //    TaxPct = 0;
            //}

            //Fee     = NumRnd(FeePct / 100 * ReqAmt, 2); 
            //var Tax = NumRnd(TaxPct / 100 * Fee, 2);
            //var Amt = ReqAmt + Fee + Tax;
            var Amt = iGetNum("txtPmtAmt" + PmtNum);
            //var Ref = Type + " x" + Num.substr(Num.length - 4, 4) + (Fee != 0 ? " Payment " + FmtDollar(ReqAmt) + " Fee " + FmtDollar(Fee) + " Tax " + FmtDollar(Tax) : "");
            var Ref = $("#txtCCStatus" + PmtNum).val();
            //$("#txtCCReqAmt"   + PmtNum).val(ReqAmt);
            //$("#lblPmtRef"     + PmtNum).html(Ref);
            //$("#txtPmtRef"     + PmtNum).val($("#txtPmtRef").val());
            $("#txtPmtAmt"     + PmtNum).val(Amt);
            $("#txtPmtAmtDisp" + PmtNum).val(FmtDollar(Amt));
        }

        return Fee;
    }

    //----------------------------------------------------------------------------		
    function Print()
    {
        var fFinalPrint = false;
        var fContinue = false;

        if (fDirty)
        {
            alert("Some data has changed, please save before printing.");
        }
        else
        {
            if (!iGetChk("chkCustomerCopy") &&
			    !iGetChk("chkFileCopy"))
            {
                alert("Check either Customer or File Copy before printing.");
            }
            else
            {
                fContinue = true;
			
                if (!fPrinted && iGetChk("chkCustomerCopy"))
                {
				
                    if (confirm("Is this the final invoice print?\n\nIf Yes, click OK to set the Job Status to 'Invoiced'.\n\nif No, click Cancel to Print without changing the Job Status."))
                    {
                        fFinalPrint = true;
                    }
                }

                if (fJobCancelled)
                {
                    fContinue = confirm("Warning: Job Status is Cancelled.\n\nClick OK to continue printing.\n\nClick Cancel abort printing.")
                }
            }
        }

        if (fContinue)
        {
            //		if (iGetChk("chkCustomerCopy"))
            {
                //			window.open("PrintInvoice.aspx" + (fFinalPrint ? "?Print=Final" : ""), "_blank");
                window.open(
				    "Invoices.aspx?JobRno=" + oGetStr("lblJobRno") + 
				    "&CustomerCopy=" + (iGetChk("chkCustomerCopy") ? "1" : "0") +
				    "&FileCopy=" + (iGetChk("chkFileCopy") ? "1" : "0") +
				    "&FinalPrint=" + (fFinalPrint ? "1" : "0"), 
				    "_blank");
			
                // get the correct state for the invoiced data
                if (fFinalPrint)
                {
                    var btnSave = GetDocObj("btnSave");
                    if (btnSave)
                    {
                        btnSave.click();
                    }
                }
            }
		
            //		if (iGetChk("chkFileCopy"))
            //		{
            //			window.open("PrintInvFileCopy.aspx", "_blank");
            //		}
        }
    }

    //----------------------------------------------------------------------------		
    function SyncCustomerCopy(Element)
    {
        var Checked = Element.checked;
	
        iSetChk("chkCustomerCopy", Checked);
        iSetChk("chkCustomerCopy1", Checked);
    }

    //----------------------------------------------------------------------------		
    function SyncFileCopy(Element)
    {
        var Checked = Element.checked;
	
        iSetChk("chkFileCopy", Checked);
        iSetChk("chkFileCopy1", Checked);
    }

    var fCCCharged = false;
    var fCCPrinted = false;

    //----------------------------------------------------------------------------		
    function EditPayment(PmtNum)
    {
        ClearPaymentFields();
        GetPaymentFields(PmtNum);

        if ($("#Payment .PmtDate").val() == "")
        {
            var Today = new Date();
            $("#Payment .PmtDate").val(Today.getMonth() + 1 + "/" + Today.getDate() + "/" + Today.getFullYear());
        }

        $("#Payment").dialog
		({
		    buttons:
			{
			    "Charge CC": function ()
			    {
			        //fCCCharged = true;    set to true after the payment returns 
			        SavePaymentFields();
			        InvoiceTotal();
			        SetDirty();
			        $(this).dialog("close");
			        $("#hfPostCC" + PmtNum).val(true);
			        $("#btnSave").click();
			    },
			    "Print": function ()
			    {
			        fCCPrinted = true;			        
			        var Customer   = $("#lblCustomer").text();
			        var JobRno     = $("#lblJobRno").text();
			        var PmtNum     = $("#txtPmtNum").val();
			        var PaymentRno = $("#txtPaymentRno").val();

			        var PrintHtml = 
                        "<div class=\"CCPrint\">" +
                            "<h1>Credit Card Transaction</h1>" +
                            "<dl>" +
                                "<dt>Customer:</dt>" +
                                "<dd>" + Customer + "</dd>" +
                                "<dt>Invoice:</dt>" +
                                "<dd>" + JobRno + "</dd>" +
                                "<dt>Payment #:</dt>" +
                                "<dd>" + PmtNum + "</dd>" +
                                "<dt>Amount:</dt>" +
                                "<dd>" + $("#txtPmtAmt").val() + "</dd>" +
                                "<dt>Date:</dt>" +
                                "<dd>" + $("#txtPmtDate").val() + "</dd>" +
                                "<dt>Reference:</dt>" +
                                "<dd>" + $("#txtPmtRef").val() + "&nbsp;</dd>" +
                                "<dt>Type:</dt>" +
                                "<dd>" + $("#txtCCType").val() + "</dd>" +
                                "<dt>Auth:</dt>" +
                                "<dd>" + $("#txtCCAuth").val() + "</dd>" +
                                "<dt>Status:</dt>" +
                                "<dd>" + $("#txtCCStatus").val() + "</dd>" +
                                "<dt>CC Reference:</dt>" +
                                "<dd>" + $("#txtCCRef").val() + "</dd>" +
                            "</dl>" +
                        "</div>";

			        var Body = $("body");
			        var BodyChildren = Body.children().detach();
			        Body.append(PrintHtml);

			        $.get("CCPrint.aspx?PaymentRno=" + PaymentRno);

			        window.print();
			        Body.empty();
			        BodyChildren.appendTo(Body);

			        var Tm = new Date();
			        var CCPrintDtTm = FmtDateTime(Tm);
			        var CCPrintUser = User();
			        $("#lblCCPrintInfo" ).text(DateTimeUser(CCPrintDtTm, CCPrintUser));

			    },
			    Save: function ()
			    {
			        var fSave = true;
			        if (fCCCharged && !fCCPrinted)
			        {
			            fSave = confirm("A credit card has been charged but the payment information has not been printed yet.\n\nPress OK save the data and close the popup without printing first.\n\nPress Cancel to go back and print the payment information.");
			        }

			        if (fSave)
			        {
			            SavePaymentFields();
			            InvoiceTotal();
			            SetDirty();
			            $(this).dialog("close");
			        }
			    },

			    Clear: function ()
			    {
			        var fClear = true;
			        if (fCCCharged && !fCCPrinted)
			        {
			            fClear = confirm("A credit card has been charged but the payment information has not been printed yet.\n\nPress OK to clear all the payment information without printing first.\n\nPress Cancel to go back and print the payment information.");
			        }

			        if (fClear)
			        {
			            ClearPaymentFields();
			        }
			    },
			    Close: function ()
			    {
			        var fClose = true;
			        if (fCCCharged && !fCCPrinted)
			        {
			            fClose = confirm("A credit card has been charged but the payment information has not been printed yet.\n\nPress OK to close the popup without printing first.\n\nPress Cancel to go back and print the payment information.");
			        }

			        if (fClose)
			        {
			            $(this).dialog("close");
			        }
			    }
			},
		    beforeClose(event, ui)
		    {
		        var fClose = true;
		        if (fCCCharged && !fCCPrinted)
		        {
		            fClose = confirm("A credit card has been charged but the payment information has not been printed yet.\n\nPress OK to close the popup without printing first.\n\nPress Cancel to go back and print the payment information.");
		        }

		        return fClose;
		    },
		    minWidth: 430,
		    modal: true
		});

        $("#Payment .PmtDate").datepicker(
        {
            showOn: "both",
            buttonImage: "Images/CalendarIcon.gif",
            buttonImageOnly: true
        });

        $("#Payment").parent().find("button span").each((i, e) =>
        {
            switch ($(e).text())
            {
                case "Charge CC":
                    $(e).parent().addClass("ChargeCC");
                    break;

                case "Print":
                    $(e).parent().addClass("PrintCC");
                    break;
            }
        });

        ShowPaymentFields();

        // type of payment method
        $("#rdoCheck, #rdoCash, #rdoCreditCard, #rdoOtherMethod", $("#Payment")).click(ShowPaymentFields);
    }

    //----------------------------------------------------------------------------		
    function ClearPaymentFields()
    {
        $("#Payment input[type='radio']").prop("checked", false);
        $("#Payment input[type='text'], #Payment input[type='hidden']").not("#txtPmtNum").val("").data("Num", "");
    }

    //----------------------------------------------------------------------------		
    function GetPaymentFields(PmtNum)
    {
        $("#txtPmtNum").val(PmtNum);

        $("#txtPaymentRno"  ).val( $("#txtPaymentRno"  + PmtNum).val());
        $("#txtPmtAmt"      ).val( FmtDollar($("#txtPmtAmt" + PmtNum).asNumber({ parseType: "Float" })));
        $("#txtPmtDate"     ).val( $("#txtPmtDt"       + PmtNum).val());
        $("#txtPmtRef"      ).val( $("#txtPmtRef"      + PmtNum).val());
        $("#txtPurchOrder"  ).val( $("#txtPurchOrder"  + PmtNum).val());
        $("#txtCCStatus"    ).val( $("#txtCCStatus"    + PmtNum).val());
        $("#txtCCAuth"      ).val( $("#txtCCAuth"      + PmtNum).val());
        $("#txtCCResult"    ).val( $("#txtCCResult"    + PmtNum).val());
        $("#txtCCRef"       ).val( $("#txtCCRef"       + PmtNum).val());

        var CCPmtDtTm   = $("#txtCCPmtDtTm"   + PmtNum).val();
        var CCPmtUser   = $("#txtCCPmtUser"   + PmtNum).val();
        var CCPrintDtTm = $("#txtCCPrintDtTm" + PmtNum).val();
        var CCPrintUser = $("#txtCCPrintUser" + PmtNum).val();

        $("#lblCCPmtInfo"   ).text(DateTimeUser(CCPmtDtTm, CCPmtUser));
        $("#lblCCPrintInfo" ).text(DateTimeUser(CCPrintDtTm, CCPrintUser));
        $("#txtCCType"      ).val($("#txtCCType"    + PmtNum).val());
        //$("#txtCCNum"       ).val($("#txtCCNum"     + PmtNum).val());
        //$("#txtCCExpDt"     ).val($("#txtCCExpDt"   + PmtNum).val());
        //$("#txtCCSecCode"   ).val($("#txtCCSecCode" + PmtNum).val());
        //$("#txtCCName"      ).val($("#txtCCName"    + PmtNum).val());
        //$("#txtCCAddr"      ).val($("#txtCCAddr"    + PmtNum).val());
        //$("#txtCCZip"       ).val($("#txtCCZip"     + PmtNum).val());

        switch ($("#txtPmtType" + PmtNum).val())
        {
            case "Deposit":
                $("#rdoDeposit").prop("checked", true);
                break;

            case "Final":
                $("#rdoFinal").prop("checked", true);
                break;

            case "Other":
                $("#rdoOther").prop("checked", true);
                break;
        }
        switch ($("#txtPmtMethod" + PmtNum).val())
        {
            case "Check":
                $("#rdoCheck").prop("checked", true);
                break;

            case "Cash":
                $("#rdoCash").prop("checked", true);
                break;

            case "CreditCard":
                $("#rdoCreditCard").prop("checked", true);
                //$("#txtPmtAmt").val(FmtDollar($("#txtCCReqAmt" + PmtNum).asNumber({ parseType: "Float" })));
                break;

            case "Other":
                $("#rdoOtherMethod").prop("checked", true);
                break;
        }

        //SetupCCNum();
    }

    //----------------------------------------------------------------------------		
    function DateTimeUser(DateTime, User)
    {
        var Fmt = "";
        if (DateTime && DateTime.length > 0 && User && User.length > 0)
        {
            Fmt = DateTime + " - " + User;
        }
        else
        if (DateTime && DateTime.length > 0)
        {
            Fmt = DateTime;
        }
        else
        if (User && User.length > 0)
        {
            Fmt = User;
        }

        return Fmt;
    }

    //----------------------------------------------------------------------------		
    function FmtDateTime(DateTime)
    {
        var Fmt =
            Right("0" + (DateTime.getMonth() + 1), 2) + "/" +
            Right("0" + DateTime.getDate(), 2)        + "/" +
            DateTime.getFullYear()                    + " " +
            Right("0" + DateTime.getHours(), 2)       + ":" +
            Right("0" + DateTime.getMinutes(), 2);

        return Fmt;
    }

    //----------------------------------------------------------------------------		
    function Right(Str, Len)
    {
        return Str.substr(-Len);
    }

    //----------------------------------------------------------------------------		
    function User()
    {
        var lbl = $(".User").text();
        var iBeg = lbl.indexOf("(") + 1;
        var iEnd = lbl.indexOf(")");
        var User = lbl.substring(iBeg, iEnd);

        return User;
    }

    //----------------------------------------------------------------------------		
    function SavePaymentFields()
    {
        //txtCCNum = $("#txtCCNum");
        //txtCCSecCode = $("#txtCCSecCode");

        //var Num = txtCCNum.data("Num");
        //if (Num.length > 0)
        //{
        //    txtCCNum.val(Num);
        //}

        //txtCCSecCode.val(txtCCSecCode.data("Num"));

        var PmtNum = $("#txtPmtNum").val();

        $("#lblPmtType"     + PmtNum).html("");
        $("#lblPmtType"     + PmtNum).html($("#rdoDeposit:checked, #rdoFinal:checked, #rdoOther:checked").val());
        $("#txtPmtType"     + PmtNum).val($("#rdoDeposit:checked, #rdoFinal:checked, #rdoOther:checked").val());
        $("#txtPmtMethod"   + PmtNum).val($("#rdoCheck:checked, #rdoCash:checked, #rdoCreditCard:checked, #rdoOtherMethod:checked").val());
        $("#txtPmtAmtDisp"  + PmtNum).val($("#txtPmtAmt"    ).val());
        $("#txtPmtAmt"      + PmtNum).val($("#txtPmtAmt"    ).val());
        $("#txtPmtDt"       + PmtNum).val($("#txtPmtDate"   ).val());
        $("#txtPmtRef"      + PmtNum).val($("#txtPmtRef"    ).val());
        $("#lblPmtRef"      + PmtNum).html($("#rdoCheck:checked, #rdoCash:checked, #rdoCreditCard:checked, #rdoOtherMethod:checked").val());
        $("#txtPurchOrder"  + PmtNum).val($("#txtPurchOrder").val());
        $("#txtCCStatus"    + PmtNum).val($("#txtCCStatus"  ).val());
        $("#txtCCAuth"      + PmtNum).val($("#txtCCAuth"    ).val());
        $("#txtCCResult"    + PmtNum).val($("#txtCCResult"  ).val());
        $("#txtCCRef"       + PmtNum).val($("#txtCCRef"     ).val());
        $("#txtCCType"      + PmtNum).val($("#txtCCType"    ).val());
        //$("#txtCCNum"       + PmtNum).val($("#txtCCNum"     ).val());
        //$("#txtCCExpDt"     + PmtNum).val($("#txtCCExpDt"   ).val());
        //$("#txtCCSecCode"   + PmtNum).val($("#txtCCSecCode" ).val());
        //$("#txtCCName"      + PmtNum).val($("#txtCCName"    ).val());
        //$("#txtCCAddr"      + PmtNum).val($("#txtCCAddr"    ).val());
        //$("#txtCCZip"       + PmtNum).val($("#txtCCZip"     ).val());

        if ($("#txtPmtMethod" + PmtNum).val() == "CreditCard")
        {
            var Type   = $("#txtCCType").val();
            //var Num    = $("#txtCCNum").val();
            //var ReqAmt = iGetNum("txtPmtAmt");
            //var FeePct = iGetNum("txtCCFeePct");
            //var TaxPct = iGetNum("txtCCFeeTaxPct");

            //var Fee = NumRnd(FeePct / 100 * ReqAmt, 2); 
            //var Tax = NumRnd(TaxPct / 100 * Fee, 2);
            //var Amt = ReqAmt + Fee + Tax;
            var Amt = iGetNum("txtPmtAmt");
            //var Ref = Type + " x" + Num.substr(Num.length - 4, 4) + " Payment " + FmtDollar(ReqAmt) + " Fee " + FmtDollar(Fee) + " Tax " + FmtDollar(Tax);
            var Ref = $("#txtCCStatus" + PmtNum).val();
            //$("#txtCCReqAmt"    + PmtNum).val(ReqAmt);
            //$("#lblPmtRef"      + PmtNum).html(Ref);
            $("#txtPmtRef"      + PmtNum).val($("#txtPmtRef" + PmtNum).val());
            $("#txtPurchOrder"  + PmtNum).val($("#txtPurchOrder" + PmtNum).val());
            $("#txtPmtAmt"      + PmtNum).val(Amt);
            $("#txtPmtAmtDisp"  + PmtNum).val(FmtDollar(Amt));

            HideCCDelete(PmtNum);
        }
    }

    //----------------------------------------------------------------------------		
    function ShowPaymentFields()
    {
        $(".PmtCC", $("#Payment")).hide();
        $(".ChargeCC, .PrintCC").hide();
        $("#Payment #lblReference").text("Reference");

        if ($("#rdoCheck:checked", $("#Payment")).length > 0)
        {
            $("#Payment #lblReference").text("Check #");

            var PmtNum = $("#txtPmtNum").val();
            $("#txtPmtRef").val($("#txtPmtRef" + PmtNum).val());
        }

        if ($("#rdoCreditCard:checked", $("#Payment")).length > 0)
        {
            var PmtNum = $("#txtPmtNum").val();
            $("#txtPmtRef").val($("#txtPmtRef" + PmtNum).val());

            $(".PmtCC input", $("#Payment")).not("#txtCCType").prop("disabled", true);
            if ($("#txtCCStatus").val() == "Approved")
            {
                $(".ChargeCC").prop("disabled", true).addClass("ui-state-disabled");
            }
            $("#Payment .PmtCC, .ChargeCC, .PrintCC").fadeIn();
        }
    }

    //----------------------------------------------------------------------------		
    //function CheckDeliveryMethod()
    //{
    //	if (iGetStr("txtInvDeliveryDate") == "")
    //	{
    //		if (iGetStr("txtInvDeliveryMethod") == "Deliver with Job")
    //		{
    //			iSetStr("txtInvDeliveryDate", iGetStr("txtJobDate"));
    //		}
    //		else
    //		{
    //			var Today = new Date();
    //			iSetStr("txtInvDeliveryDate", "" + (Today.getMonth() + 1) + "/" + Today.getDate() + "/" + Today.getFullYear());
    //		}
    //	}
    //}

    //----------------------------------------------------------------------------		
    //function CheckEmailDeliveryMethod()
    //{
    //	if (iGetStr("txtInvDeliveryMethod") == "Email" && 
    //		iGetStr("txtInvEmail") == "")
    //	{
    //		alert("Warning: An Email address is needed for a Delivery Method of 'Email'.");
    //	}
    //}

    //----------------------------------------------------------------------------		
    //function CheckFaxDeliveryMethod()
    //{
    //	if (iGetStr("txtInvDeliveryMethod") == "Fax" && 
    //		iGetStr("txtInvFax") == "")
    //	{
    //		alert("Warning: A Fax number is needed for a Delivery Method of 'Fax'.");
    //	}
    //}

    //----------------------------------------------------------------------------		
    function CheckCCExpDate()
    {
        var ExpDate = iGetStr("txtCCExpDt");
        var Parts = ExpDate.split("/");
        if (Parts.length == 2)
        {
            if (Parts[1].length == 1)
            {
                Parts[1] = "0" + Parts[1];
            }
            var ExpDt = new Date(Parts[0] + "/1/20" + Parts[1]);
		
            var ThisMonth = new Date();
            ThisMonth = new Date(ThisMonth.getFullYear(), ThisMonth.getMonth(), 1);
		
            var FutureDate = new Date();
            FutureDate.setFullYear(FutureDate.getFullYear() + 5);
		
            if (ExpDt < ThisMonth)
            {
                alert("Warning: The credit card has expired.");
            }
            else
                if (ExpDt > FutureDate)
                {
                    alert("Warning: The credit card expiration date is more than five years in the future.");
                }
        }
        else
        {
            alert("Invalid credit card expiration date.");
        }
    }

    //----------------------------------------------------------------------------		
    function CheckCCFailed()
    {
        if (iGetChk("chkFailedFlg"))
        {
            if (iGetStr("txtCCApprCode") != "")
            {
                alert("Warning: The credit card transaction has an approval code and declined is checked.");
            }
        }
    }

    //----------------------------------------------------------------------------		
    function CheckDate(FieldName)
    {
        var dt = new Date(iGetStr(FieldName));
        if (!isNaN(dt))
        {
            var Today = new Date();
            Today.setHours(0);
            Today.setMinutes(0);
            Today.setSeconds(0);
            Today.setMilliseconds(0);

            var FutureDate = new Date(Today);
            FutureDate.setFullYear(FutureDate.getFullYear() + 1);
			
            if (dt.valueOf() < Today.valueOf())
            {
                alert("Warning: This date is in the past.");
            }
            else
            if (dt.valueOf() > FutureDate.valueOf())
            {
                alert("Warning: This date is more than one year in the future.");
            }
        }
    }

    //----------------------------------------------------------------------------		
    function SetupPayment()
    {
        $(".AddPayment").click(AddPayment);
        $("#Payments").on("click", ".EditPayment", function ()
        {
            var PmtNum = $(this).data("pmtnum"); 
            EditPayment(PmtNum);
        });
        $("#Payments").on("click", ".DeletePayment", function ()
        {
            var PmtNum = $(this).data("pmtnum"); 
            DeletePayment(PmtNum);
        });

        // hide credit card delete icons
        var i;
        for (i = 1; i <= iPmtNum; i++)
        {
            HideCCDelete(i);
        }

        $("#rdoFinal").change(function ()
        {
            if ($("#rdoFinal:checked").length > 0)
            {
                var PmtAmt = $("#txtPmtAmt").val();
                if (PmtAmt == "$0.00" || PmtAmt == "")
                {
                    //$("#txtPmtAmt").val($("#txtInvBalance").val());
                    $("#txtPmtAmt").val(FmtDollar(FinalPaymentAmt)); 
                }
            }
        });

        $("#rdoCheck, #rdoCash, #rdoOtherMethod").change(function ()
        {
            //if ($("#rdoCheck:checked, #rdoCash:checked, #rdoOtherMethod:checked").length > 0)
            if ($("#rdoCash:checked, #rdoOtherMethod:checked").length > 0)
            {
                $("#txtPmtRef").val("");
            }
        });

        $("#txtPmtAmt").blur(function ()
        {
            $(this).val(FmtDollar($(this).asNumber({ parseType: "Float" })));
        });

/*
        // Payment Method
        var PmtMthds = [ "Check", "CC", "Cash" ];
        $("#txtInvPmtMethod").autocomplete(
		{
		    source: function(req, response) 
		    { 
		        var re = $.ui.autocomplete.escapeRegex(req.term); 
		        var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
		        response($.grep(PmtMthds, function(Item, ItemIndex)
		        { 
		            return Matcher.test(Item); 
		        }) ); 
		    },			
		    autoFocus: true,
		    delay: 0,
		    minLength: 0
		});
*/

        // CC Type
        var CCTypes = [ "VISA", "MC", "AMEX" ];
        $("#txtCCType").autocomplete(
		{
		    source: function(req, response) 
		    { 
		        var re = $.ui.autocomplete.escapeRegex(req.term); 
		        var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
		        response($.grep(CCTypes, function(Item, ItemIndex)
		        { 
		            return Matcher.test(Item); 
		        }) ); 
		    },			
		    autoFocus: true,
		    delay: 0,
		    minLength: 0
		});

        //var txtCCNum = $("#txtCCNum");
        //txtCCNum.focus(function ()
        //{
        //    console.log("focus");
        //    if ($(this).data("Loc") == "data")
        //    {
        //        var Num = $(this).data("Num");
        //        if (Num.length > 0)
        //        {
        //            $(this).val(Num);
        //        }
        //        txtCCNum.data("Loc", "val");
        //    }
        //});

        //txtCCNum.blur(function ()
        //{
        //    console.log("blur");
        //    $("#InvalidCCNum").hide();
        //    if ($(this).data("Loc") == "val")
        //    {
        //        CCNum = $(this).val();
        //        if (CCNum.length > 5)
        //        {
        //            if (!isValidCreditCard(CCNum))
        //            {
        //                $("#InvalidCCNum").show();
        //            }

        //            $(this).data("Num", CCNum);
        //            $(this).val(HiddenCCNum(CCNum));
        //            txtCCNum.data("Loc", "data");
        //        }
        //        else
        //        {
        //            $(this).data("Num", "");
        //        }
        //    }
        //});

        //$("#txtCCExpDt").blur(CheckCCExpDate);

        //var txtCCSecCode = $("#txtCCSecCode");
        //txtCCSecCode.focus(function ()
        //{
        //    if ($(this).data("Loc") == "data")
        //    {
        //        $(this).val($(this).data("Num"));
        //        $(this).data("Loc", "val");
        //    }
        //});

        //txtCCSecCode.blur(function ()
        //{
        //    if ($(this).data("Loc") == "val")
        //    {
        //        $(this).data("Num", $(this).val());
        //        $(this).val("**********".substring(0, $(this).data("Num").length));
        //        $(this).data("Loc", "data");
        //    }
        //});
    }

    //----------------------------------------------------------------------------		
    var NewPayment =
        "<tr>\n" +
            "<td colspan=\"2\"></td>\n" +
            "<td align=\"right\"><label id=\"lblPmtType{n}\"></label></td>\n" +
            "<td></td>\n" +
            "<td><input type=\"textbox\" id=\"txtPmtAmtDisp{n}\" class=\"JobSubTotal aspNetDisabled\" disabled=\"disabled\"></td>\n" +
            "<td></td>\n" +
            "<td colspan=\"2\">\n" +
                "<img src=\"Images/Notepage16.png\" class=\"EditPayment\" alt=\"Edit Payment\" title=\"Edit Payment\" data-pmtnum=\"{n}\" >\n" +
                "<img src=\"Images/Delete16.png\" class=\"DeletePayment\" alt=\"Delete Payment\" title=\"Delete Payment\" data-pmtnum=\"{n}\" >\n" +
                "<label id=\"lblPmtRef{n}\"></label>\n" +
                "<input type=\"hidden\" id=\"txtPaymentRno{n}\"  name=\"txtPaymentRno{n}\"  >\n" +
                "<input type=\"hidden\" id=\"txtSeq{n}\"         name=\"txtSeq{n}\"         >\n" +
                "<input type=\"hidden\" id=\"txtPmtAmt{n}\"      name=\"txtPmtAmt{n}\"      >\n" +
                "<input type=\"hidden\" id=\"txtPmtType{n}\"     name=\"txtPmtType{n}\"     >\n" +
                "<input type=\"hidden\" id=\"txtPurchOrder{n}\"  name=\"txtPurchOrder{n}\"  >\n" +
                "<input type=\"hidden\" id=\"txtPmtRef{n}\"      name=\"txtPmtRef{n}\"      >\n" +
                "<input type=\"hidden\" id=\"txtPmtDt{n}\"       name=\"txtPmtDt{n}\"       >\n" +
                "<input type=\"hidden\" id=\"txtPmtMethod{n}\"   name=\"txtPmtMethod{n}\"   >\n" +
                "<input type=\"hidden\" id=\"txtCCType{n}\"      name=\"txtCCType{n}\"      >\n" +
                "<input type=\"hidden\" id=\"txtCCStatus{n}\"    name=\"txtCCStatus{n}\"    >\n" +
                "<input type=\"hidden\" id=\"txtCCAuth{n}\"      name=\"txtCCAuth{n}\"      >\n" +
                "<input type=\"hidden\" id=\"txtCCResult{n}\"    name=\"txtCCResult{n}\"    >\n" +
                "<input type=\"hidden\" id=\"txtCCRef{n}\"       name=\"txtCCRef{n}\"       >\n" +
                "<input type=\"hidden\" id=\"txtCCPmtDtTm{n}\"   name=\"txtCCPmtDtTm{n}\"   >\n" +
                "<input type=\"hidden\" id=\"txtCCPmtUser{n}\"   name=\"txtCCPmtUser{n}\"   >\n" +
                "<input type=\"hidden\" id=\"txtCCPrintDtTm{n}\" name=\"txtCCPrintDtTm{n}\" >\n" +
                "<input type=\"hidden\" id=\"txtCCPrintUser{n}\" name=\"txtCCPrintUser{n}\" >\n" +
                "<input type=\"hidden\" id=\"hfPostCC{n}\"       name=\"hfPostCC{n}\"       >\n" +
                "<input type=\"hidden\" id=\"hfDeletePmt{n}\"    name=\"hfDeletePmt{n}\" class=\"DeletePmt\" >\n" +
            "</td>\n" +
        "</tr>\n";

    //----------------------------------------------------------------------------		
    function AddPayment()
    {
        var CopyNewPayment = NewPayment.replace(/{n}/g, ++iPmtNum);
        $("#Payments").append(CopyNewPayment);
        EditPayment(iPmtNum);
    }

    //----------------------------------------------------------------------------		
    function DeletePayment(PmtNum)
    {
        if (confirm("Are you sure?"))
        {
            var Delete = $("#hfDeletePmt" + PmtNum);
            Delete.val("true");
            Delete.closest("tr").hide();
            CalcSubTotal();
            SetDirty();
        }
    }

    //----------------------------------------------------------------------------		
    function SetupCCNum()
    {
        console.log("SetupCCNum");
        var txtCCNum = $("#txtCCNum");
        var CCNum = txtCCNum.val();

        if (CCNum.length > 5)
        {
            txtCCNum.data("Num", CCNum);
            txtCCNum.val(HiddenCCNum(CCNum));
            txtCCNum.data("Loc", "data");
        }
        else
        {
            txtCCNum.data("Num", "");
            txtCCNum.data("Loc", "val");
        }

        if (CCNum.length <= 5 || isValidCreditCard(CCNum))
        {
            $("#InvalidCCNum").hide();
        }

        var txtCCSecCode = $("#txtCCSecCode");
        txtCCSecCode.data("Num", txtCCSecCode.val());
        txtCCSecCode.val("**********".substring(0, txtCCSecCode.data("Num").length));
        txtCCSecCode.data("Loc", "data");


        /*
        txtCCNum.closest("form").submit(function ()
        {
            txtCCNum.hide();
            txtCCSecCode.hide();

            var Num = txtCCNum.data("Num");
            if (Num.length > 0)
            {
                txtCCNum.val(Num);
            }

            txtCCSecCode.val(txtCCSecCode.data("Num"));
        });
        */

        //txtCCNum.hover(
        //    function(e)
        //    {
        //        var Num = $(this).data("Num");
        //        if (Num.length > 0)
        //        {
        //            $(this).val(Num);
        //        }
        //    },
        //    function(e)
        //    {
        //        var Num = $(this).data("Num");
        //        if (Num.length > 0)
        //        {
        //            $(this).val(HiddenCCNum(Num));
        //        }
        //    }
        //);
    }

    //----------------------------------------------------------------------------		
    function HiddenCCNum(CCNum)
    {
        return "********************".substring(0, CCNum.length - 4) + CCNum.substring(CCNum.length - 4, CCNum.length);
    }

    //----------------------------------------------------------------------------		
    //function isValidCreditCard(type, ccnum) 
    function isValidCreditCard(ccnum) 
    {
        //if (type == "Visa") {
        //    // Visa: length 16, prefix 4, dashes optional.
        //    var re = /^4\d{3}-?\d{4}-?\d{4}-?\d{4}$/;
        //} else if (type == "MC") {
        //    // Mastercard: length 16, prefix 51-55, dashes optional.
        //    var re = /^5[1-5]\d{2}-?\d{4}-?\d{4}-?\d{4}$/;
        //} else if (type == "Disc") {
        //    // Discover: length 16, prefix 6011, dashes optional.
        //    var re = /^6011-?\d{4}-?\d{4}-?\d{4}$/;
        //} else if (type == "AmEx") {
        //    // American Express: length 15, prefix 34 or 37.
        //    var re = /^3[4,7]\d{13}$/;
        //} else if (type == "Diners") {
        //    // Diners: length 14, prefix 30, 36, or 38.
        //    var re = /^3[0,6,8]\d{12}$/;
        //}
        //if (!re.test(ccnum)) return false;
        //// Remove all dashes for the checksum checks to eliminate negative numbers
        //ccnum = ccnum.split("-").join("");

        // Checksum ("Mod 10")
        // Add even digits in even length strings or odd digits in odd length strings.
        var checksum = 0;
        for (var i=(2-(ccnum.length % 2)); i<=ccnum.length; i+=2) {
            checksum += parseInt(ccnum.charAt(i-1));
        }
        // Analyze odd digits in even length strings or even digits in odd length strings.
        for (var i=(ccnum.length % 2) + 1; i<ccnum.length; i+=2) {
            var digit = parseInt(ccnum.charAt(i-1)) * 2;
            if (digit < 10) { checksum += digit; } else { checksum += (digit-9); }
        }
        if ((checksum % 10) == 0) return true; else return false;
    }

    //----------------------------------------------------------------------------		
    function HideCCDelete(PmtNum)
    {
        if ($("#txtPmtMethod" + PmtNum).val() == "CreditCard" && $("txtCCStatus" + PmtNum).val() != "")
        {
            $("#txtPaymentRno" + PmtNum).closest("td").find(".DeletePayment").css("visibility", "hidden");
        }
    }
</script>