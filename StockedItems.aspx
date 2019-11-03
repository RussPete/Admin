<%@ Page AutoEventWireup="true" CodeFile="StockedItems.aspx.cs" Inherits="StockedItems"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Shopping List</title> 
		<!-- ServingCounts.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
		<link href="ReportStyles.css" type="text/css" rel="stylesheet">
    	<link href="css/font-awesome.min.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="javascript">
    	    var Units = <%= UnitData() %>;

			//----------------------------------------------------------------------------
			$(document).ready(function ()
			{
				$(".JobDate").datepicker(
				{
					showOn: "both",
					buttonImage: "Images/CalendarIcon.gif",
					buttonImageOnly: true
				});

				$("input:submit").button();

				Qtips(".qtp", { width: 650, target: "bottomMiddle", tooltip: "topMiddle", y: 15 });

				AutoCompleteUnit("input.Unit");

				$("#btnReset").click(function() 
				{
				    return confirm("OK to erase the purchase quantities for all of the stocked items?");
				});
			});

			//-----------------------------------------------------------------------------
			function ShowDtl(iBeg, iEnd)
			{
				var Style = GetStyle('Dtl' + iBeg);
				var fDisplay = (Style.display == "none");
	
				var i;
				for (i = iBeg; i <= iEnd; i++)
				{
					SetDisplay('Dtl' + i, fDisplay);
				}
			}

			//-----------------------------------------------------------------------------
			function ShowAll()
			{
				var i;
				for (i = 0; i <= iEndDtl; i++)
				{
					SetDisplay('Dtl' + i, true);
				}
			}

			//-----------------------------------------------------------------------------
			function HideAll()
			{
				var i;
				for (i = 0; i <= iEndDtl; i++)
				{
					SetDisplay('Dtl' + i, false);
				}
			}

			//----------------------------------------------------------------------------
			function AutoCompleteUnit(Selector)
			{
			    $(Selector).autocomplete(
                {
                    source: Units,
                    select: function(event, ui)
                    {                    
                        $(this).val($(this).closest("tr, li").find(".UnitQty").val() == "1" ? ui.item.single : ui.item.plural);
                        $(this).closest("tr, li").find(".UnitRno").val(ui.item.id);
                        SetDirty();
                        return false;
                    },
                    change: function(event, ui)
                    {
                        if (ui.item == null)
                        {
                            $(this).val("");  
                        }
                    },
                    autoFocus: true,
                    delay: 0,
                    minLength: 0
                })
                .focus(function()
                {
                    $(this).autocomplete("search", $(this).val());
                });
			}
		</script>
        <style>
            .RptTitle
            {
                margin: 10px 0px;
            }

            .RptSubTitle
            {
                font-size: 12pt;
            }

            table.Report
            {
                border-collapse: collapse;
            }

            @media print
            {
                table.Report
                {
                    page-break-before: always;
                }
                table.Report:first-of-type
                {
                    page-break-before: initial;
                }
            }

            table.Report .RptSubTitle
            {
                font-size: 14pt;
                margin: 20px 0px 5px;
                text-align: left;
            }


            table.Report tr.Underline th
            {
                border-bottom: solid 1px #000;
            }

            table.Report th, table.Report td
            {
                padding: 2px 3px 2px;
            }

            table.Report td.Stocked
            {
                font-weight: bold;
                padding-top: 15px;
                border-bottom: solid 1px #ccc;
                text-align: center;
            }

            table.Report th.Center
            {
                text-align: center;
            }

            table.Report th.Qty, table.Report td.Qty
            {
                text-align: right;
            }

            table.Report th.Price, table.Report td.Price
            {
                text-align: right;
                width: auto;
            }

            table.Report td.Notes
            {
                font-size: 9pt;
                font-style: italic;
                padding-left: 30px;
                width: 400px;
                color: #888;
            }

            table.Report td.Notes ul
            {
                margin: 0px;
            }

            table.Report td.Notes ul li
            {
                display: inline;
            }

            /* bullet sperator */
            table.Report td.Notes ul li:before
            {
                content: "\2022  ";
            }

            .qtp
            {
                cursor: pointer;
            }

            .Ingred a
            {
                color: #000;
            }

            .Ingred a:hover
            {
                color: #c00;
            }

            .qtip .Recipe
            {
                font-style: italic;
            }

            .qtip .Qty
            {
                text-align: right;
            }

            .icon-warning-sign
            {
                color: firebrick;
            }

            input.Qty 
            {
                width: 60px;
                text-align: right;
                border: none;
                border-bottom: 1px solid #CCC;
            }

            input.Unit 
            {
                width: 120px;
                border: none;
                border-bottom: 1px solid #CCC;
            }

            .btnSave
            {
                text-align: center;
                margin: 15px;
            }

            .btnSave input
            {
                margin: 0px 10px;
            }

            @media print
            {
                .btnSave 
                {
                    display: none;
                }
            }
        </style>
	</head>
	<body>
<%
if (!fReport)
{		
%>
		<form id="form1" method="post" target="_blank" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<div class="FeatureMain">Stocked Items</div>
			<table cellspacing="10" cellpadding="0" align="center" border="0">
				<tr>
					<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="1"></td>
					<td></td>
					<td><img height="1" src="Images/Space.gif" alt="" width="20"></td>
					<td></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoDay" runat="server" text="Day" groupname="rdoPeriod" checked="True"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtDayDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgDayDate" runat="server" imageurl="Images/CalendarIcon.gif" imagealign="AbsMiddle" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoWeek" runat="server" checked="False" groupname="rdoPeriod" text="Week"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtWeekDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgWeekDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoMonth" runat="server" checked="False" groupname="rdoPeriod" text="Month"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtMonthDate" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgMonthDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><asp:radiobutton id="rdoRange" runat="server" checked="True" groupname="rdoPeriod" text="Range"></asp:radiobutton></td>
					<td></td>
					<td><asp:textbox id="txtBegDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgBegDateRange" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
					<td align="center">thru</td>
					<td><asp:textbox id="txtEndDateRange" runat="server" cssclass="JobDate"></asp:textbox><asp:image id="imgEndDateRange" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
				</tr>
				<tr>
					<td><img height="20" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
				<tr>
					<td align="center" colspan="5"><asp:button id="btnQuantities" runat="server" text="Set Quantities" OnClick="btnQuantities_Click"></asp:button></td>
				</tr>
				<tr>
					<td><img height="80" src="Images/Space.gif" alt="" width="1"></td>
				</tr>
			</table>
			<% Pg.Bottom(); %>
		</form>
<%
}
else
{
%>
	    <form id="form" method="post" runat="server" autocomplete="off">
            <asp:HiddenField ID="hfBegDate" runat="server" />
            <asp:HiddenField ID="hfEndDate" runat="server" />
            <asp:HiddenField ID="hfParmCount" runat="server" />
            <div class="btnSave">
                <asp:button id="btnSave1" runat="server" text="Save" OnClick="btnSave_Click"></asp:button>
                <asp:button id="btnReset" runat="server" text="Reset" OnClick="btnReset_Click"></asp:button>
            </div>
		    <div class="RptTitle">Stocked Items</div>
		    <div class="RptSubTitle"><asp:Literal ID="ltlBegDate" runat="server" /> - <asp:Literal ID="ltlEndDate" runat="server" /></div>
            <asp:Table ID="tblReport" CssClass="Report" runat="server">
                <asp:TableHeaderRow runat="server">
                    <asp:TableHeaderCell runat="server" />
                    <asp:TableHeaderCell ColumnSpan="2" CssClass="Center" runat="server">Jobs</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" />
                    <asp:TableHeaderCell ColumnSpan="2" CssClass="Center" runat="server">Job Purchase</asp:TableHeaderCell>
                    <asp:TableHeaderCell ColumnSpan="1" CssClass="Center" runat="server">Purchase</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableHeaderRow CssClass="Underline" runat="server">
                    <asp:TableHeaderCell runat="server" />
                    <asp:TableHeaderCell CssClass="Qty" runat="server">Qty</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server">Unit</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server">Ingredient</asp:TableHeaderCell>
                    <asp:TableHeaderCell CssClass="Qty" runat="server">Qty</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server">Unit</asp:TableHeaderCell>
                    <asp:TableHeaderCell CssClass="Qty" runat="server">Qty</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
            <div class="btnSave"><asp:button id="btnSave2" runat="server" text="Save" OnClick="btnSave_Click"></asp:button></div>
	    </form>
<%
}		
%>
	</body>
</html>
