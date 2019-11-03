<%@ Page AutoEventWireup="true" CodeFile="Purchases.aspx.cs" Inherits="Purchases" Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Ingredients</title>
	<!-- Purchases.aspx -->
	<!-- Copyright (c) 2013-2019 PeteSoft, LLC. All Rights Reverved -->
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">

	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="js/jquery.formatCurrency-1.4.0.min.js" type="text/javascript"></script>
   	<script language="JavaScript" src="js/jquery.validate.js" type="text/javascript"></script>

	<script language="JavaScript" type="text/javascript">

	    function MyPostBack(id, parameter)
	    {
	        __doPostBack(id, parameter)
	    }

	    var NewIngredRno = <%= NewIngredRno %>;

        var Vendors = <%= VendorData() %>;
	    var Ingredients = <%= IngredientData() %>;
	    var Units = <%= UnitData() %>;

	    var AddRowHtml = "<%= AddRowHtml() %>";

	    var PrevSelectAllTime = 0;

	    //----------------------------------------------------------------------------
	    $(document).ready(function()
	    {
	        // auto select text box contents
	        $("#content").on("focus", "input:text", function()
	        {
	            if ($(this).val().length > 0)
	            {
	                var dt = new Date().getTime();
	                if (dt - PrevSelectAllTime > 100)
	                {
	                    setTimeout("$('#" + this.id + "').select();", 50);
	                }

	                PrevSelectAllTime = dt;
	            }
	        });

	        $("#tabs").tabs({ selected: $("#hfTab").val() });
	        SearchList("#txtSearch", "#lstList");

	        if (NewIngredRno == 0)
	        {
	            $("#txtPurchaseDate").focus();
	        }
	        $("#txtPurchaseDate").datepicker({
	            showOn: "both",
	            buttonImage: "Images/CalendarIcon.gif",
	            buttonImageOnly: true,
	            onClose: function()
	            {
	                $("#txtVendor").focus();
	            }
	        });	

	        // insert a new row for an ingredient
	        $("#tblDetails").on("blur", "input.Ingredient:last", AddBlankRow);

	        AutoCompleteVendor("#txtVendor");
	        AutoCompleteIngredient("#tblDetails input.Ingredient");
	        AutoCompleteUnit("#tblDetails input.Unit");

	        $("#tblDetails").on("blur", "input.Price", function()
	        {
	            $(this).formatCurrency();
	        });

	        $("#form").validate({
	            rules: {
	                txtPurchaseDate: "required",
	                txtVendor: "required",
	            }
	        });

	        $("#content").on("change", "input", SetDirty);

	        $("#btnSave").click(SkipDirty);
	        $("#btnDelete").click(function()
	        {
	            var rc = false;

	            if (confirm("Are you sure you want to delete this purchase?"))
	            {
	                SkipDirty();
	                rc = true;
	            }

	            return rc;
	        });

	        if (NewIngredRno != 0)
	        {
	            var Ingredient = "";
	            for (var i = 0; i < Ingredients.length; i++)
	            {
	                if (Ingredients[i].id == NewIngredRno)
	                {
	                    Ingredient = Ingredients[i].label;
	                }
	            }
	            if (Ingredient.length > 0)
	            {
	                var Ingred = $("#tblDetails input.Ingredient:last");
	                Ingred.val(Ingredient);
	                Ingred.parent().find("input.IngredRno").val(NewIngredRno);
	                Ingred.closest("tr").find("input[id^='txtPurchaseQty']").val(1)
	                Ingred.closest("tr").find("input[id^='txtUnitQty']").focus();
                }
	        }

	        window.onbeforeunload = CheckDirty;
	    });

	    //----------------------------------------------------------------------------
	    function AutoCompleteVendor(Selector)
	    {
	        $(Selector).autocomplete(
            {
                source: Vendors,
                //source: function(req, response) { 
                //    var re = $.ui.autocomplete.escapeRegex(req.term); 
                //    var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                //    response($.grep(Vendors, function(Item, ItemIndex) { 
                //        return Matcher.test(Item.label); 
                //    }) ); 
                //},
                select: function(event, ui)
                {
                    $(this).val(ui.item.label);
                    $(this).closest("li").find("#hfVendorRno").val(ui.item.id);
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

	    //----------------------------------------------------------------------------
	    function AutoCompleteIngredient(Selector)
	    {
	        $(Selector).autocomplete(
            {
                source: Ingredients,
                //source: function(req, response) { 
                //    var re = $.ui.autocomplete.escapeRegex(req.term); 
                //    var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                //    response($.grep(Ingredients, function(Item, ItemIndex) { 
                //        return Matcher.test(Item.label); 
                //    }) ); 
                //},
                select: function(event, ui)
                {
                    tr = $(this).closest("tr");
                    FillInIngredientInfo(ui.item, tr);

                    /*
                    $(this).val(ui.item.label);
                    tr = $(this).closest("tr");
                    tr.find(".IngredRno").val(ui.item.id);
                    tr.find(".Unit").prop("disabled", false);

                    var VendorRno = $("#hfVendorRno").asNumber();
                    for (var i = 0; i < ui.item.prev.length; i++) 
                    {
                        var Prev = ui.item.prev[i];
                        if (Prev.vendor == VendorRno)
                        {
                            var Unit = Units[0];
                            for (var iUnit = 0; iUnit < Units.length; iUnit++) 
                            {
                                if (Units[iUnit].id == Prev.unitRno)
                                {
                                    Unit = Units[iUnit];
                                    break;
                                }
                            }

                            tr.find(".Qty").val(Prev.qty);
                            tr.find(".UnitQty").val(Prev.unitQty);
                            tr.find(".UnitRno").val(Prev.unitRno);
                            tr.find(".Unit").val(Prev.unitQty > 1 ? Unit.single : Unit.plural);
                            tr.find(".Price").val(Prev.price).formatCurrency();
                            break;
                        }
                    }

                    if (ui.item.asis == 1)
                    {
                        var Unit = tr.find(".Unit");
                        Unit.val("ea");
                        Unit.prop("disabled", true);
                        tr.find(".UnitRno").val(UnitRno("ea"));

                    }

                    var SetFocus = "jQuery('#" + $(this).closest("tr").find(".Qty").prop("id") + "').focus();";
                    setTimeout(SetFocus, 50);

                    SetDirty();
                    */
                    return false;
                },
                change: function(event, ui)
                {
                    if (ui.item == null)
                    {
                        tr = $(this).closest("tr");
                        var Value = $(this).val();
                        if (!CheckIngredient(Value, tr))        // if the ingredient not in the list of ingredients
                        {
                            $("#txtIngredient").val(Value);
                            $(this).val("");  

                            var fSkipCloseFocus = false;

                            // prep new ingredient popup
                            $("#NewIngredient").dialog(
                            {
                                buttons:
                                {
                                    "Save": function()
                                    {								
                                        tr.find("input.Ingredient").val($("#txtIngredient").val());								
                                        tr.find("input.Stocked").val($("#chkStocked").prop("checked"));
                                        tr.find("input.Qty:first").focus();
                                        fSkipCloseFocus = true;
								
                                        $(this).dialog("close");
                                    }
                                },
                                modal: true,
                                width: 450,
                                beforeClose: function(event, ui)
                                {
                                    if (!fSkipCloseFocus)
                                    {
                                        tr.find("input.Ingredient").focus();
                                    }
                                }
                            });
                            $("#txtIngredient").focus();
                        }
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

	    //----------------------------------------------------------------------------
	    function AutoCompleteUnit(Selector)
	    {
	        $(Selector).autocomplete(
            {
                source: Units,
                //source: function(req, response) { 
                //    var re = $.ui.autocomplete.escapeRegex(req.term); 
                //    var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                //    response($.grep(Units, function(Item, ItemIndex) {                         
                //        return Matcher.test(Item.label); 
                //    }) ); 
                //},
                select: function(event, ui)
                {                    
                    $(this).val($(this).closest("tr").find(".Qty").val() == "1" ? ui.item.single : ui.item.plural);
                    $(this).closest("tr").find(".UnitRno").val(ui.item.id);
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
                //autoFocus: true,
                delay: 0,
                minLength: 0
            })
            .focus(function()
            {
                $(this).autocomplete("search", $(this).val());
            });
        }

	    //----------------------------------------------------------------------------
	    function AddBlankRow()
	    {
	        if ($("#tblDetails input.Ingredient:last").val().length > 0)
	        {
	            var NumDetails = $("#tblDetails tr").length;
	            $("#tblDetails tbody").append(AddRowHtml.replace(/~ID~/g, NumDetails));
	            $("#hfNumDetails").val(NumDetails);

	            AutoCompleteIngredient("#tblDetails input.Ingredient:last");
	            AutoCompleteUnit("#tblDetails input.Unit:last");
            }
	    }

	    //----------------------------------------------------------------------------
	    function UnitRno(Label)
	    {
	        var Rno = 0;
	        for (var i = 0; i < Units.length; i++)
	        {
	            if (Units[i].label == Label)
	            {
	                Rno = Units[i].id;
	                break;
	            }
	        }

	        return Rno;
	    }

	    //----------------------------------------------------------------------------
	    function CheckIngredient(Name, tr)
	    {
	        var Found = false;
	        for (var i = 0; i < Ingredients.length; i++) 
	        {
	            if (Ingredients[i].label == Name)
	            {
	                Found = true;
	                FillInIngredientInfo(Ingredients[i], tr)
	                break;
	            }
	        }

	        return Found;
	    }

	    //----------------------------------------------------------------------------
	    function FillInIngredientInfo(item, tr)
	    {   
	        tr.find(".Ingredient").val(item.label);
	        tr.find(".IngredRno").val(item.id);
	        tr.find(".Unit").prop("disabled", false);

	        var VendorRno = $("#hfVendorRno").asNumber();
	        for (var i = 0; i < item.prev.length; i++) 
	        {
	            var Prev = item.prev[i];
	            if (Prev.vendor == VendorRno)
	            {
	                var Unit = Units[0];
	                for (var iUnit = 0; iUnit < Units.length; iUnit++) 
	                {
	                    if (Units[iUnit].id == Prev.unitRno)
	                    {
	                        Unit = Units[iUnit];
	                        break;
	                    }
	                }

	                tr.find(".Qty").val(Prev.qty);
	                tr.find(".UnitQty").val(Prev.unitQty);
	                tr.find(".UnitRno").val(Prev.unitRno);
	                tr.find(".Unit").val(Prev.unitQty > 1 ? Unit.single : Unit.plural);
	                tr.find(".Price").val(Prev.price).formatCurrency();
	                break;
	            }
	        }

	        if (item.asis == 1)
	        {
	            var Unit = tr.find(".Unit");
	            Unit.val("ea");
	            Unit.prop("disabled", true);
	            tr.find(".UnitRno").val(UnitRno("ea"));

	        }

	        var SetFocus = "jQuery('#" + $(this).closest("tr").find(".Qty").prop("id") + "').focus();";
	        setTimeout(SetFocus, 50);

	        SetDirty();	    }

	</script>

	<style>
		.FeatureSub
		{
			padding-bottom: 5px;
		}

	    #content
	    {
            min-width: 400px;
            margin-left: 40px;
	    }

	    .FeatureMain
	    {
	        margin: 0px auto;
	    }

	    ul.Fields
	    {
            list-style: none;
            padding: 0px;
	    }

	    ul.Fields label
	    {
	        margin-right: 7px;
            margin-left: 5px;
	    }

	    .Prompt
	    {
            display: inline-block;
            width: 100px;
            text-align: right;
	    }

	    .Date
	    {
            width: 70px;
	    }

	    .Vendor
	    {
            width: 250px;
	    }

	    h3
	    {
            text-align: center;
            color: #667B3E;
            margin-bottom: 0px;
	    }

	    #tblConv
	    {
            margin: 10px auto;
	    }

	    .Ingredient
	    {
            width: 200px
	    }

	    .Qty
	    {
            width: 40px;
            text-align: right;
	    }

	    .Unit
	    {
            width: 60px;
	    }

	    .Price
	    {
            width: 60px;
            text-align: right;
	    }

	    .Remove
	    {
            display: block;
            text-align: center;
	    }

	    .PO
	    {
            width: 80px;
	    }

	    .Buttons
	    {
            margin-top: 15px;
            text-align: center;
	    }

	    #Dates
	    {
            margin: 15px auto;
	    }

	</style>
</head>
<body>
	<form id="form" method="post" runat="server" autocomplete="off">
	    <% Pg.Top(); %>
	    <table cellspacing="0" cellpadding="0" align="center" border="0">
		    <tr>
			    <td valign="top">
                    <div id="tabs">
                        <asp:HiddenField ID="hfTab" runat="server" />
                        <ul>
                            <li><a href="#ShoppingLists">Shopping Lists</a></li>
                            <li><a href="#Receipts">Receipts</a></li>
                        </ul>
                        <div id="ShoppingLists">
				            <table cellspacing="0" cellpadding="0" align="center" border="0">
					            <tr>
						            <td align="left">
							            <table class="SelectFilter" cellspacing="0" cellpadding="0" border="0" width="100%">
								            <tr>
									            <td>
										            Sort By:
									            </td>
								            </tr>
								            <tr>
									            <td>
										            <asp:RadioButton ID="rbShoppingDate" Checked="true" runat="server" GroupName="rbShoppingSortBy" AutoPostBack="True"
											            Text="Date" OnCheckedChanged="UpdateShoppingList" />
									            </td>
                                                <td>
                                                    <asp:RadioButton ID="rbShoppingVendor" runat="server" GroupName="rbShoppingSortBy" AutoPostBack="True"
											            Text="Vendor" OnCheckedChanged="UpdateShoppingList" />
                                                </td>
								            </tr>
							            </table>
						            </td>
					            </tr>
					            <tr>
						            <td>
                                        <asp:TextBox ID="txtShoppingSearch" CssClass="Search" runat="server" />
						            </td>
					            </tr>
					            <tr>
						            <td>
							            <asp:ListBox ID="lstShoppingList" runat="server" AutoPostBack="true" CssClass="SelectJob"
								            OnSelectedIndexChanged="lstShoppingList_SelectedIndexChanged" onclick="MyPostBack('lstShoppingList', '')"></asp:ListBox>
						            </td>
					            </tr>
					            <tr>
						            <td>
							            <asp:Label ID="lblShoppingRecCount" runat="server" CssClass="SelectCount">RecCount</asp:Label>
						            </td>
					            </tr>
				            </table>
                        </div>
                        <div id="Receipts">
                            <table cellspacing="0" cellpadding="0" align="center" border="0">
					            <tr>
						            <td align="left">
							            <table class="SelectFilter" cellspacing="0" cellpadding="0" border="0" width="100%">
								            <tr>
									            <td>
										            Sort By:
									            </td>
								            </tr>
								            <tr>
									            <td>
										            <asp:RadioButton ID="rbDate" Checked="true" runat="server" GroupName="rbSortBy" AutoPostBack="True"
											            Text="Date" OnCheckedChanged="UpdateList" />
									            </td>
                                                <td>
                                                    <asp:RadioButton ID="rbSortVendor" runat="server" GroupName="rbSortBy" AutoPostBack="True"
											            Text="Vendor" OnCheckedChanged="UpdateList" />
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
							            <asp:ListBox ID="lstList" runat="server" AutoPostBack="true" CssClass="SelectJob"
								            OnSelectedIndexChanged="lstList_SelectedIndexChanged" onclick="MyPostBack('lstList', '')"></asp:ListBox>
						            </td>
					            </tr>
					            <tr>
						            <td>
							            <asp:Label ID="lblRecCount" runat="server" CssClass="SelectCount">RecCount</asp:Label>
						            </td>
					            </tr>
				            </table>
                        </div>
                    </div>
			    </td>
                <td style="vertical-align: top;">
                    <div id="content">
                        <div class="FeatureMain">Receipts</div>
                        <ul class="Fields">
                            <li><label class="Prompt">Purchase Date</label><asp:TextBox ID="txtPurchaseDate" CssClass="Date" runat="server" /></li>
                            <li><label class="Prompt">Vendor</label><asp:TextBox ID="txtVendor" CssClass="Vendor" runat="server" /><asp:HiddenField ID="hfVendorRno" runat="server" /></li>
                        </ul>

                        <h3>Ingredients</h3>
                        <asp:Table id="tblDetails" runat="server">                     
                            <asp:TableRow>
                                <asp:TableHeaderCell>Ingredient</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Purch<br />Qty</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Unit<br />Qty</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Unit</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Extended<br />Price</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Remove</asp:TableHeaderCell>
                            </asp:TableRow>
                        </asp:Table>

                        <ul class="Fields">
                            <li>
                                <label class="Prompt">PO #</label><asp:TextBox ID="txtOrderNum" CssClass="PO" runat="server" />
                                <label class="Prompt">Vendor Invoice #</label><asp:TextBox ID="txtVendorInvoice" CssClass="PO" runat="server" />
                            </li>
                        </ul>

                        <div class="Buttons">
						    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                            <asp:Button ID="btnNew" runat="server" Text="New" OnClick="btnNew_Click" />
						    <asp:button id="btnDelete" runat="server" text="Delete" onclick="btnDelete_Click" />
                        </div>

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
                    </div>
                </td>
		    </tr>
	    </table>
	    <input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
	    <input type="hidden" id="hfRno" name="hfRno" runat="server" />
	    <input type="hidden" id="hfShoppingListRno" name="hfShoppingListRno" runat="server" />
        <asp:HiddenField ID="hfNumDetails" runat="server" />
	    <% Pg.Bottom(); %>
	</form>
	<div id="NewIngredient" title="New Ingredient" style="display: none;">
		<p>This is a new ingredient. Please double check the spelling and set the given values.</p>
		<p>If you think the ingredient exists, click the X to go back and try to find it.</p>
		<dl>
			<dt>Ingredient</dt>
			<dd><input id="txtIngredient" type="text" /></dd>
			<dt></dt>
            <dd />
            <dt />
			<dd><input id="chkStocked" type="checkbox" value="false" /><label for="chkStocked">Stocked</label></dd>
		</dl>
	</div>		
</body>
</html>
