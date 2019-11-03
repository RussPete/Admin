<%@ Page AutoEventWireup="true" CodeFile="Ingredients.aspx.cs" Inherits="Ingredients"
	Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Ingredients</title>
	<!-- Ingredients.aspx -->
	<!-- Copyright (c) 2012-2019 PeteSoft, LLC. All Rights Reserved -->
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
	<script language="JavaScript" src="js/jquery.validate.js" type="text/javascript"></script>
	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="JavaScript" type="text/javascript">
	    var Units = <%= UnitData() %>;

	    //----------------------------------------------------------------------------
	    $(document).ready(function()
	    {
	        SearchList("#txtSearch", "#lstList");

	        $("#txtName").focus();

	        $("#form").validate({
	            rules: {
	                txtName: "required",
	            }
	        });

	        $("#content").on("change", "input, textarea", SetDirty);

	        if ($("#chkStocked").is(":checked"))
	            $("#StockedPurchase").show();
	        else
	            $("#StockedPurchase").hide();
	        $("#chkStocked").change(function()
	        {
	            if ($("#chkStocked").is(":checked"))
	                $("#StockedPurchase").slideDown();
	            else
	                $("#StockedPurchase").slideUp();
	        });

	        $("#btnSave, #btnUpdate, #btnNext").click(SkipDirty);
	        $("#btnDelete").click(function ()
	        {
	            var rc = false;

	            if (confirm("Are you sure you want to delete this ingredient?"))
	            {
	                SkipDirty();
	                rc = true;
	            }

	            return rc;
	        });

	        window.onbeforeunload = CheckDirty;
	    });

	    //----------------------------------------------------------------------------
	    function AutoCompleteUnit(Selector)
	    {
	        $(Selector).autocomplete(
            {
                source: Units,
                select: function(event, ui)
                {                    
                    $(this).val($(this).closest("tr, li").find(".Qty").val() == "1" ? ui.item.single : ui.item.plural);
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
        .Inactive
        {
            float: right;
        }

        .Stocked
        {
            /*margin-left: 40px;*/
        }

        .NonPurchase
        {
            margin-left: 10px;
        }

	    #tblStockedPurchase, #tblConv, #tblPrices
	    {
            margin: 15px auto;
	    }

        #tblStockedPurchase .Qty, #tblConv .Qty, #tblPrices .Qty
        {
            text-align: right;
        }

        #tblStockedPurchase input.Qty, #tblConv input.Qty
        {
            width: 60px;
        }

        #tblStockedPurchase input.Unit
        {
            width: 75px;
        }

        #tblConv td:first-child
        {
            padding: 0px;
        }

        #tblPrices td
        {
            font-weight: normal;
        }

        #tblStockedPurchase th, #tblConv th, #tblPrices th
        {
            text-align: center;
            color: #667B3E;
        }

	    #tblConv td:nth-child(3), #tblConv th:nth-child(3), #tblConv tr:first-child th:nth-child(2)
	    {
            padding-left: 20px;
	    }

        #tblPrices .Price
        {
            text-align: right;
        }

        #tblPrices td:nth-child(3), #tblPrices td:nth-child(5)
        {
            padding-left: 10px;
        }

        .NeedConv
        {
            padding-right: 5px;
            color: firebrick;
        }

        #tblConv td.Remove
        {
            text-align: center;
        }

        table td.Buttons
        {
            text-align: center;
            padding: 10px 0px 20px;
        }

	    .qtip ul, #RecipesDialog ul
	    {
            list-style-type: none;
            padding: 0px;
            margin: 0px 0px 0px 10px;
	    }

        #RecipesDialog a:hover
        {
            text-decoration: none;
            color: #667B3E;
        }

    </style>

</head>
<body id="Ingredients" onbeforeunload="CheckDirty();">
	<form id="form" method="post" runat="server" autocomplete="off">
	<% Pg.Top(); %>
	<table cellspacing="0" cellpadding="0" align="center" border="0">
		<tr>
			<td valign="top">
				<table cellspacing="0" cellpadding="0" align="center" border="0">
					<tr>
						<td>
							<table class="SelectFilter" cellspacing="0" cellpadding="0" border="0" width="100%">
								<tr>
									<td align="right">
										<asp:CheckBox ID="chkShowHidden" Text ="Show Hidden" TextAlign="Left" runat="server"  AutoPostBack="true" oncheckedchanged="UpdateList" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td>
                            <asp:TextBox ID="txtSearch" CssClass="Search" runat="server" />
						</td>
					</tr>
					<tr>
						<td>
							<asp:ListBox ID="lstList" runat="server" AutoPostBack="True" CssClass="SelectJob"
								OnSelectedIndexChanged="lstList_SelectedIndexChanged"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblRecCount" runat="server" CssClass="SelectCount">RecCount</asp:Label>
						</td>
					</tr>
				</table>
			</td>
			<td>
				<img height="1" src="Images/Space.gif" alt="" width="40">
			</td>
			<td valign="top" align="center">
                <div id="content">
    				<table border="0" cellpadding="0" cellspacing="0" class="Edit">
					<tr>
						<td colspan="3" class="FeatureMain">
							Ingredients
						</td>
					</tr>
					<tr>
						<td>
							Name
						</td>
						<td>
							<asp:TextBox ID="txtName" runat="server" MaxLength="50" CssClass="Name"></asp:TextBox>
                            <asp:CheckBox ID="chkHide" Text="Hide" CssClass="Inactive" runat="server" />
                        </td>
					</tr>
					<tr>
						<td>							
						</td>
						<td>

                            <%--<asp:RadioButton ID="rbDry" Text="Dry" GroupName="DryLiquid" runat="server" />--%>
                            <%--<asp:RadioButton ID="rbLiquid" Text="Liquid" GroupName="DryLiquid" runat="server" />--%>
                            <asp:CheckBox ID="chkStocked" Text ="Stocked" CssClass="Stocked" runat="server" />
                            <asp:CheckBox ID="chkNonPurchase" Text ="Non Purchase" CssClass="NonPurchase" runat="server" />
                            <asp:Label ID="lblRecipes" Text="Used in n recipes" runat="server" CssClass="TextLink" Visible="false" />
                        </td>
					</tr>
                    <tr>
                        <td></td>
                        <td>
                            <div id="StockedPurchase">
                                <asp:table ID="tblStockedPurchase" runat="server">
                                    <asp:TableRow>
                                        <asp:TableHeaderCell ForeColor="Black">Stocked Purchase</asp:TableHeaderCell>
                                        <asp:TableHeaderCell ColumnSpan="2">Purchase</asp:TableHeaderCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableHeaderCell />
                                        <asp:TableHeaderCell CssClass="Qty">Qty</asp:TableHeaderCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell />
                                        <asp:TableCell>
                                            <asp:TextBox ID="txtStockedPurchaseQty" CssClass="Qty" runat="server" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:table ID="tblConv" runat="server">
                                <asp:TableRow>
                                    <asp:TableHeaderCell ForeColor="Black">Unit Conversions</asp:TableHeaderCell>
                                    <asp:TableHeaderCell ColumnSpan="2">Recipe</asp:TableHeaderCell>
                                    <asp:TableHeaderCell ColumnSpan="2">Purchase</asp:TableHeaderCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableHeaderCell />
                                    <asp:TableHeaderCell CssClass="Qty">Qty</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Unit</asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="Qty">Qty</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Unit</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Remove</asp:TableHeaderCell>
                                </asp:TableRow>

<%--                                <asp:TableRow>
                                    <asp:TableCell><input id="Qty1" class="Qty" /></asp:TableCell>
                                    <asp:TableCell>Cup</asp:TableCell>
                                    <asp:TableCell CssClass="Qty">1</asp:TableCell>
                                    <asp:TableCell>#10 can</asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell><input id="Text1" class="Qty" /></asp:TableCell>
                                    <asp:TableCell>Cup</asp:TableCell>
                                    <asp:TableCell CssClass="Qty">1</asp:TableCell>
                                    <asp:TableCell>10 lb bag</asp:TableCell>
                                </asp:TableRow>
--%>
                            </asp:table>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:table ID="tblPrices" runat="server">
                                <asp:TableRow>
                                    <asp:TableHeaderCell ForeColor="Black">Price History</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Date</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Vendor</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Qty</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Unit</asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="Price">Total $</asp:TableHeaderCell>
                                    <asp:TableHeaderCell CssClass="Price">Unit $</asp:TableHeaderCell>
                                </asp:TableRow>
                            </asp:table>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <div id="PrefVendors" class="ListWrapper">
                                <div class="Title">Preferred Vendors</div><div class="Edit">(<span class="TextLink">edit</span>)</div>
                                <asp:Panel ID="pnlPrefVendors" CssClass="List" runat="server" />
                            </div>
                            <div id="RejVendors" class="ListWrapper">
                                <div class="Title">Rejected Vendors</div><div class="Edit">(<span class="TextLink">edit</span>)</div>
                                <asp:Panel ID="pnlRejVendors" CssClass="List" runat="server" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <div id="PrefBrands" class="ListWrapper">
                                <div class="Title">Preferred Brands</div><%--<div class="Edit">(<span class="TextLink">edit</span>)</div>--%>
                                <%--<asp:Panel ID="pnlPrefBrands" CssClass="List" runat="server">--%>
                                    <asp:TextBox ID="txtPrefBrands" TextMode="MultiLine" runat="server" />
                                <%--</asp:Panel>--%>
                            </div>
                            <div id="RejBrands" class="ListWrapper">
                                <div class="Title">Rejected Brands</div><%--<div class="Edit">(<span class="TextLink">edit</span>)</div>--%>
                                <%--<asp:Panel ID="pnlRejBrands" CssClass="List" runat="server">--%>
                                    <asp:TextBox ID="txtRejBrands" TextMode="MultiLine" runat="server" />
                                <%--</asp:Panel>--%>                                
                            </div>
                        </td>
                    </tr>
					<tr>
						<td>
						</td>
						<td class="Buttons">
							<asp:Button ID="btnUpdate" runat="server" Text="Save" OnClick="btnUpdate_Click" />
							<asp:button id="btnNext" runat="server" text="Save & Next" onclick="btnUpdateNext_Click" />
							<asp:button id="btnNew" runat="server" text="New" onclick="btnNew_Click" />
							<asp:button id="btnDelete" runat="server" text="Delete" onclick="btnDelete_Click" />
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
                    <p></p>
                </div>
			</td>
		</tr>
	</table>
	<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
	<input type="hidden" id="txtRno" name="txtRno" runat="server" />
    <asp:HiddenField ID="hfNumConversions" runat="server" />
    <asp:HiddenField id="hfPrefVendors" runat="server" />
    <asp:HiddenField id="hfRejVendors" runat="server" />

    <div id="RecipesDialog" title="Ingredient in Recipes">
        <ul id="ulRecipes" runat="server" />
    </div>

    <div id="PrefVendorsDialog" title="Preferred Vendors">
        <ul id="ulPrefVendors" class="Vendors" runat="server"></ul>
    </div> 

    <div id="RejVendorsDialog" title="Rejected Vendors">
        <ul id="ulRejVendors" class="Vendors" runat="server"></ul>
    </div> 
	<% Pg.Bottom(); %>
	</form>

	<script language="JavaScript" type="text/javascript">

	    //----------------------------------------------------------------------------		
	    $(document).ready(function ()
	    {
	        // populate preferred and rejected vendors
	        SetVendors("#hfPrefVendors", "#PrefVendorsDialog");
	        SetVendors("#hfRejVendors", "#RejVendorsDialog");
	        ShowVendors("#hfPrefVendors", "#PrefVendorsDialog", "#pnlPrefVendors");
	        ShowVendors("#hfRejVendors", "#RejVendorsDialog", "#pnlRejVendors");
            
	        Qtips("#lblRecipes");

	        $("#lblRecipes").click(function ()
	        {
	            $("#RecipesDialog").dialog(
                {
                    modal: true
                });
	        });

	        $("#PrefVendors .TextLink").click(function ()
	        {
	            $("#PrefVendorsDialog").dialog(
                {
                    modal: true,
                    buttons:
                    {
                        Save: function ()
                        {
                            $(this).dialog("close");
                            ShowVendors("#hfPrefVendors", "#PrefVendorsDialog", "#pnlPrefVendors");
                            SetDirty();
                        }
                    }
                });
	        });

	        $("#RejVendors .TextLink").click(function ()
	        {
	            $("#RejVendorsDialog").dialog(
	            {
	                modal: true,
	                buttons:
                    {
                        Save: function ()
                        {
                            $(this).dialog("close");
                            ShowVendors("#hfRejVendors", "#RejVendorsDialog", "#pnlRejVendors");
                            SetDirty();
                        }
                    }
	            });
	        });
	    });

	    //----------------------------------------------------------------------------		
	    function SetVendors(hf, dialog)
	    {
	        var aIds = $(hf).val().split(",");
	        for (var i = 0; i < aIds.length; i++)
	        {
	            var span = $(".Vendors li span[data-id='" + aIds[i] + "']", $(dialog));
	            if (span.length > 0)
	            {
	                span.find("input")[0].checked = true;
	            }
	        }
	    }

	    //----------------------------------------------------------------------------		
	    function ShowVendors(hf, dialog, pnl)
	    {
	        var html = "";
	        var aIds = new Array();
	        $("input:checked", $(dialog)).each(function ()
	        {
	            html += "<li>" + $(this).next().html() + "</li>";
	            aIds.push($(this).parent().data("id"));
	        });
	        $(pnl).html("<ul class=\"Vendors\">" + html + "</ul>");
	        $(hf).val(aIds.join());
	    }
	</script>
</body>
</html>
