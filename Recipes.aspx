<%@ Page AutoEventWireup="true" CodeFile="Recipes.aspx.cs" Inherits="Recipes" Language="C#" validateRequest="false" %>

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
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
	<link href="css/jquery-te-1.4.0.css" type="text/css" rel="stylesheet">
   	<link href="css/font-awesome.min.css" type="text/css" rel="stylesheet">

	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery.formatCurrency-1.4.0.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
   	<script language="JavaScript" src="js/jquery.validate.js" type="text/javascript"></script>
  	<script language="JavaScript" src="js/jquery-te-1.4.0.min.js" type="text/javascript"></script>
    <script language="JavaScript" src="js/jquery.autosize-min.js" type="text/javascript"></script>

	<script language="JavaScript" type="text/javascript">

	    function MyPostBack(id, parameter)
	    {
	        __doPostBack(id, parameter)
	    }

	    var MenuItems = <%= MenuItemData() %>;
	    var Ingredients = <%= IngredientData() %>;
	    var Units = <%= UnitData() %>;
        var Categories = <%= CategoryData() %>;
        var AddMenuItemHtml = "<%= AddMenuItemHtml() %>";
	    var AddRowHtml = "<%= AddRowHtml() %>";
	    var AddTitleRowHtml = "<%= AddTitleRowHtml() %>";

	    var PrevSelectAllTime = 0;

	    //----------------------------------------------------------------------------
	    $(document).ready(function()
	    {
	        $("#<%= FocusField.Length > 0 ? FocusField : "txtName" %>").focus();

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

            $("#lstList").on("change", function()
            {
                location.replace("Recipes.aspx?Rno=" + $(this).val());
            });

	        SearchList("#txtSearch", "#lstList");

	        SetFilters();

	        Qtips("#lblRecipes, .qtp");

	        $("#lblRecipes").click(function ()
	        {
	            $("#RecipesDialog").dialog(
                {
                    modal: true
                });
	        });

	        Qtips(".icon-warning-sign, .icon-dollar");


	        // allow ingredients to be sorted
	        $("#tblIngredients tbody").sortable(
            {
                placeholder: "ui-state-highlight",
                stop: function (event, ui)
                {
                    var RecipeSeq = 1;
                    $("#tblIngredients .Seq").each(function(index)
                    {
                        $(this).val(RecipeSeq++);
                    });
                }
            });

	        // insert a new menu item 
	        $("#tblMenuItems").on("blur", "input.Name:last", AddBlankMenuItem);

	        $("#tblIngredients").on("change", "input", NeedsCostCalc);
	        //$("#txtNumServings").change(NeedsCostCalc);

	        // insert a new row for an ingredient
	        $("#tblIngredients").on("blur", "input.Ingredient:last", AddBlankRow);

	        AutoCompleteMenuItem("#tblMenuItems input.Name");
	        AutoCompleteUnit("#tblIngredients input.Unit");
	        AutoCompleteIngredient("#tblIngredients input.Ingredient");

	        $("#btnAddTitle").click(AddBlankTitleRow);

	        AutoCompleteUnit("#txtYieldUnit, #txtPortionUnit");
	        $("#txtPortion").focus(CalcServingSize);

	        SetServicePrice();
	        $("#txtNumServings, #txtBaseCostPct, #chkUseDefaultBaseCostPct").change(SetServicePrice);

	        $("#txtInstructions").jqte(
            {
                color: false,
                sub: false,
                sup: false,
                strike: false,
                link: false,
                unlink: false,
                left: false,
                center: false,
                right: false,
                remove: false,
                source: false,
                change: SetDirty,
            });

	        $(".Title").autosize();

	        AutoCompleteCategory("#txtCategory");

	        $("#form").validate({
	            rules: 
                {
                    txtName: "required",
                    txtBaseCostPct: 
                    {
                        required: true,
                        min: 0.01,
                    }
                },
	            messages:
                {
                    txtBaseCostPct: 
                    {
                        required: "Can't give it away for free. Enter a value greater than 0. Use 100% for an at cost price.",
                        min: "Can't give it away for free. Enter a value greater than 0. Use 100% for an at cost price."
                    }
                }
	        });

	        $("#Print").parent().attr("href", $("#lnkScale").attr("href"));

	        $("#content").on("change", "input, textarea, select", SetDirty);

	        $("#btnSave").click(SkipDirty);

	        window.onbeforeunload = CheckDirty;
	    });

	    //----------------------------------------------------------------------------
	    function SetFilters()
	    {
	        if ($("#rbSortRecipe:checked").length > 0)
	        {
	            $(".RecipeFilter").show();
	            $(".MenuItemFilter").hide();
	        }
	        else
	        {
	            $(".RecipeFilter").hide();
	            $(".MenuItemFilter").show();
	        }

	        $(".SelectFilter input:radio:checked").parent().addClass("Checked");
	    }

	    //----------------------------------------------------------------------------
	    function AutoCompleteMenuItem(Selector)
	    {
	        $(Selector).autocomplete(
            {
                source: MenuItems,
                //source: function(req, response) { 
                //    var re = $.ui.autocomplete.escapeRegex(req.term); 
                //    var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                //    response($.grep(MenuItems, function(Item, ItemIndex) {                         
                //        return Matcher.test(Item.label); 
                //    }) ); 
                //},
                select: function(event, ui)
                {                    
                    var fContinue = true;
                    if (ui.item.used)
                    {
                        fContinue = confirm("'" + ui.item.label + "' already has a recipe. Do you want to reassign it?");
                    }

                    if (fContinue)
                    {
                        var PrimaryID = ui.item.id;
                        $(this).val(ui.item.label);
                        $(this).parent().find("input[id^='hfMenuItemRno']").val(ui.item.id);


                        // find other menuitems with the same name in other categories, add them
                        var aLabelParts = ui.item.label.split(" - ");
                        var MenuItem = aLabelParts[0].toLowerCase();
                        var fAssignOtherCategories = false;

                        // look for the same menu item in other categories
                        //for (var i = 0; i < MenuItems.length; i++) 
                        //{
                        //    var aLabelParts = MenuItems[i].label.split(" - ");
                        //    if (MenuItems[i].id != PrimaryID && aLabelParts[0].toLowerCase() == MenuItem)
                        //    {
                        //        fAssignOtherCategories = confirm("'" + aLabelParts[0] + "' is in one or more other categories, do you want to connect this recipe with all of them?");
                        //        break;
                        //    }
                        //}

                        if (fAssignOtherCategories)
                        {
                            for (var i = 0; i < MenuItems.length; i++) 
                            {
                                var aLabelParts = MenuItems[i].label.split(" - ");
                                if (MenuItems[i].id != PrimaryID && aLabelParts[0].toLowerCase() == MenuItem)
                                {
                                    // now make sure this menu item is not already assigned to the recipe
                                    if ($("input[name^=hfMenuItemRno][value='" + MenuItems[i].id + "']").length == 0)
                                    {
                                        // add blank line
                                        AddBlankMenuItem();
                                        var Next = $("#tblMenuItems input.Name:last");
                                        Next.val(MenuItems[i].label);
                                        Next.parent().find("input[id^='hfMenuItemRno']").val(MenuItems[i].id);
                                    }
                                }
                            }
                        }
                        AddBlankMenuItem();
                        $("#tblMenuItems input.Name:last").focus();
                        SetDirty();
                    }
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
			// auto drop down on focus of Category input box
			.focus(function()
			{
			    $(this).autocomplete("search", $(this).val());
			});
        }

	    //----------------------------------------------------------------------------
	    function AddBlankMenuItem()
	    {
	        if ($("#tblMenuItems input.Name:last").val().length > 0)
	        {
	            var NumMenuItems = $("#tblMenuItems tr").length;
	            $("#tblMenuItems tbody").append(AddMenuItemHtml.replace(/~ID~/g, NumMenuItems));
	            $("#hfNumMenuItems").val(NumMenuItems);

	            AutoCompleteMenuItem("#tblMenuItems input.Name:last");
	            setTimeout('$("#tblMenuItems input.Name:last").focus();', 100);
	        }
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
                    $(this).val($(this).closest("tr, li").find(".Qty").val() == "1" ? ui.item.single : ui.item.plural);
                    $(this).closest("tr, li").find(".UnitRno").val(ui.item.id);
                    NeedsCostCalc();
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
                    $(this).val(ui.item.label);
                    $(this).closest("tr").find(".IngredRno").val(ui.item.ingredId);
                    $(this).closest("tr").find(".SubrecipeRno").val(ui.item.subrecipeId);
                    var span = $(this).closest("tr").find("span[id^='spnSubrecipe");
                    span.empty();

                    if (ui.item.subrecipeId > 0)
                    {
                        span.append("<a href=\"Recipes.aspx?Rno=" + ui.item.subrecipeId + "\" title=\"Subrecipe\"><i class=\"icon-folder-close\"></i></a>");
                    }
                    NeedsCostCalc();

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
	    function AddBlankRow()
	    {
	        if ($("#tblIngredients input.Ingredient:last").val().length > 0)
	        {
	            var NumIngredients = $("#tblIngredients tr").length;
	            $("#tblIngredients tbody").append(AddRowHtml.replace(/~ID~/g, NumIngredients));
	            $("#hfNumIngredients").val(NumIngredients);

	            AutoCompleteUnit("#tblIngredients input.Unit:last");
	            AutoCompleteIngredient("#tblIngredients input.Ingredient:last");
            }
	    }

	    //----------------------------------------------------------------------------
	    function AddBlankTitleRow()
	    {
	        var NumIngredients = $("#tblIngredients tr").length;
	        $("#tblIngredients tbody").append(AddTitleRowHtml.replace(/~ID~/g, NumIngredients));
	        $("#hfNumIngredients").val(NumIngredients);
	        $("#tblIngredients .Title:last")
                .autosize()
                .focus();
        }

	    //----------------------------------------------------------------------------
	    function CalcServingSize()
	    {
	        var NumServings = $("#txtNumServings").asNumber();
	        var Yield = $("#txtYield").asNumber();
	        var Portion = $("#txtPortion").asNumber();
	        if (Portion == 0 && $("#txtPortionUnit").val().length == 0 && NumServings > 0 && Yield > 0)
	        {
	            var ServingSize = Yield / NumServings;
	            $("#txtPortion").val(ServingSize);
	            $("#hfPortionUnitRno").val($("#hfYieldUnitRno").val());
	            $("#txtPortionUnit").val($("#txtYieldUnit").val());
	        }
	    }

	    //----------------------------------------------------------------------------
	    function NeedsCostCalc()
	    {
	        $("#txtBaseCostPrice, #txtRecipeBaseCostPrice, #txtPortionPrice, #txtRecipePrice").val("");
	        $("#lblCostNote").html(" Save to recalculate");
	    }

	    //----------------------------------------------------------------------------
	    function SetServicePrice()
	    {
	        var fUseDefault = $("#chkUseDefaultBaseCostPct").prop("checked");
	        if (fUseDefault)
	        {
	            $("#txtBaseCostPct").hide();
	            $("#lblBaseCostPct").show();
	        }
	        else
	        {
	            $("#txtBaseCostPct").show();
	            $("#lblBaseCostPct").hide();
	        }

	        var txtBaseCostPct = $("#txtBaseCostPct");
	        var BaseCostPct = (!fUseDefault ? txtBaseCostPct.asNumber() : $("#lblBaseCostPct").asNumber());
	        var RecipeBaseCost = $("#txtRecipeBaseCostPrice").asNumber();
	        var RecipePrice = (BaseCostPct == 0 ? 0 : RecipeBaseCost / (BaseCostPct / 100));
	        var NumServings = $("#txtNumServings").asNumber();
	        var BaseCostPrice = (NumServings == 0 ? 0 : RecipeBaseCost / NumServings);
	        var PortionPrice = (BaseCostPct == 0 ? 0 : BaseCostPrice / (BaseCostPct / 100));

	        $("#txtBaseCostPrice").val(BaseCostPrice).formatCurrency({roundToDecimalPlace: 2});
	        $("#txtPortionPrice").val(PortionPrice).formatCurrency({roundToDecimalPlace: 2});
	        $("#txtRecipePrice").val(RecipePrice).formatCurrency({roundToDecimalPlace: 2});
        }

	    //----------------------------------------------------------------------------
	    function AutoCompleteCategory(Selector)
	    {
	        $(Selector).autocomplete(
            {
                source: Categories,
                select: function(event, ui)
                {
                    $(this).val(ui.item.value);
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
	    .MenuList
	    {
            background-color: #E5EBE0;
	    }

	    .SelectFilter .Radio label
	    {
            color: #AAA;
	    }

	    .SelectFilter .Radio.Checked label 
	    {
            color: initial;
	    }

	    .Has
	    {
            color: #55692E;
	    }

	    .RecipeFilter, .MenuItemFilter
	    {
            text-align: right;
	    }

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

	    th
	    {
            color: #667B3E;
	    }

	    ul.Fields
	    {
            list-style: none;
            padding: 0px;
	    }

	    ul.Fields li>label
	    {
	        margin-right: 7px;
            margin-left: 5px;
	    }

	    ul.Fields li.MenuItem
	    {
            width :initial;
	    }

	    .Prompt
	    {
            display: inline-block;
            width: 115px;
            text-align: right;
	    }

	    ul.Fields li>label.FieldTitle
	    {
            width: 68px;
            text-align: right;
            display: inline-block;
            margin: 5px 0px 0px 0px;
            padding: 0px;
            font-weight: bold;
            color: #667B3E;
	    }

	    .Name
	    {
            width: 250px;
	    }

	    .Source
	    {
            width: 425px;
	    }

	    span.TextLink
	    {
            display: inline-block;
            margin-left: 55px;
	    }

	    #tblMenuItems
	    {
            margin: 10px 20px 20px;
	    }

	    #tblMenuItems th, #tblIngredients th
	    {
            text-align: left;
	    }

	    #tblIngredients a:hover
	    {
            text-decoration: none;
	    }

	    .Qty, #tblIngredients th.Qty
	    {
            width: 40px;
            text-align: right;
	    }

	    .Unit
	    {
            width: 75px;
	    }

        .icon-warning-sign, .icon-dollar
        {
            color: firebrick;
        }

	    .Ingredient, .DupIngredient
	    {
            width: 200px;
	    }

	    .Title
	    {
            width: 100%;
            font-weight: bold;
            overflow-y: hidden;
	    }

	    .Disabled
	    {
            background-color: #DDD;
	    }

	    .Note
	    {
            width: 150px;
	    }

	    .Remove
	    {
            display: block;
            text-align: center;
	    }

	    #tblIngredients .Price
	    {
            text-align: right;
            width: auto;
            min-width: 45px;
	    }

        tr.ui-state-highlight { height: 2.1em; line-height: 1.5em; }

	    #pnlInstructions
	    {
            margin-top: 15px;
	    }

	    #txtInstructions
	    {
            width: 530px;
            height: 90px;
	    }

	    .Servings
	    {
            width: 45px;
            text-align: right;
	    }

	    span.Servings
	    {
            display: inline-block;
            width: 49px;
            text-align: right;
	    }

	    input.Price
	    {
            width: 65px;
            text-align: right;
	    }

	    #RecipesDialog
	    {
            margin-left: 20px;
	    }

	    .qtip ul, #RecipesDialog ul
	    {
            list-style-type: none;
            padding: 0px;
            margin: 0px 0px 0px 10px;
	    }

	    .qtip dl
	    {
            width: 190px;
	    }

	    .qtip li label
	    {
            display: inline-block;
            width: 80px;
            text-align: right;
	    }

	    .qtip li label:after
	    {
            content: ":";
	    }

	    .qtip li span
	    {
            display: inline-block;
            margin-left: 5px;
	    }

	    .Default
	    {
            color: #888;
            padding-left: 10px;
	    }

	    .RecipeNote
	    {
            width: 530px;
            height: 60px;
	    }

	    .jqte
	    {
            margin: initial;
            border: initial;
            width: 530px;
	    }

	    .Buttons
	    {
            margin-top: 15px;
            text-align: center;
	    }

	    #Dates
	    {
            margin: 0px auto 15px;
	    }

	    #RecipesDialog
	    {
            display: none;
	    }

        #RecipesDialog a:hover
        {
            text-decoration: none;
            color: #667B3E;
        }

	</style>
</head>
<body>
	<form id="form" method="post" runat="server" autocomplete="off">
	<% Pg.Top(); %>
	<table cellspacing="0" cellpadding="0" align="center" border="0">
		<tr>
			<td valign="top">
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
                                        <asp:RadioButton ID="rbSortRecipe" Checked="true" CssClass="Radio" runat="server" GroupName="rbSortBy" AutoPostBack="True"
											Text="Recipe" OnCheckedChanged="UpdateList" />
                                    </td>
                                    <td class="RecipeFilter Has">
                                        <asp:checkbox id="chkWithMenuItem" text="♦ Has Menu Item" Checked="false" textalign="left" oncheckedchanged="UpdateList" autopostback="true" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="RecipeFilter">
                                        <asp:RadioButton ID="rbAllRecipes" Checked="true" GroupName="rbRecipeType" Text="All" TextAlign="Left" CssClass="Radio" runat="server" AutoPostBack="true" OnCheckedChanged="UpdateList" />
                                        <asp:RadioButton ID="rbRecipes" GroupName="rbRecipeType" Text="Recipes" TextAlign="Left" runat="server" CssClass="Radio" AutoPostBack="true" OnCheckedChanged="UpdateList" />
                                        <asp:RadioButton ID="rbSubrecipes" GroupName="rbRecipeType" CssClass="Radio" Text="↓ Subrecipes" TextAlign="Left" runat="server" AutoPostBack="true" OnCheckedChanged="UpdateList" />
                                   </td>
                                </tr>
								<tr>
									<td>
										<asp:RadioButton ID="rbSortCategory" runat="server" CssClass="Radio" GroupName="rbSortBy" AutoPostBack="True"
											Text="Category" OnCheckedChanged="UpdateList" />
									</td>
                                    <td class="RecipeFilter">
                                    </td>
                                    <td class="MenuItemFilter Has">
                                        <asp:checkbox id="chkWithRecipe" text="♦ Has Recipe" Checked="false" textalign="left" oncheckedchanged="UpdateList" autopostback="true" runat="server" />
                                    </td>
								</tr>
								<tr>
									<td style="height: 20px">
										<asp:RadioButton ID="rbSortMenuItem" runat="server" CssClass="Radio" GroupName="rbSortBy" AutoPostBack="True"
											Text="Menu Item" OnCheckedChanged="UpdateList" />
									</td>
									<td class="RecipeFilter">
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
							<asp:ListBox ID="lstList" runat="server" CssClass="SelectJob"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblRecCount" runat="server" CssClass="SelectCount">RecCount</asp:Label>
						</td>
					</tr>
				</table>
			</td>
            <td style="vertical-align: top;">
                <div id="content">
                    <% if (ErrMsg.Length > 0) { %>
                    <div class="ErrMsg"><%= ErrMsg %></div>
                    <% } %>

                    <div class="Rno"><asp:Label ID="lblRno" runat="server" /></div>
                    <div class="FeatureMain">Recipes</div>
                    <ul class="Fields">
                        <li>
                            <label class="Prompt">Name</label><asp:TextBox ID="txtName" CssClass="Name" runat="server" />
                            <asp:CheckBox ID="chkHide" Text="Hide" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Type</label><asp:DropDownList ID="ddlType" runat="server"><asp:ListItem Text="Menu Item" /><asp:ListItem Text="Subrecipe" /></asp:DropDownList>
                            <asp:Label ID="lblRecipes" Text="Used in n recipes" runat="server" CssClass="TextLink" Visible="false" />
                        </li>
                    </ul>

                    <asp:Table ID="tblMenuItems" runat="server">
                        <asp:TableHeaderRow TableSection="TableHeader">
                            <asp:TableHeaderCell Text="Menu Item" />
                            <asp:TableHeaderCell Text="Remove" />
                        </asp:TableHeaderRow>
                    </asp:Table>

                    <asp:Panel ID="pnlIngredients" runat="server">
                        <asp:Table ID="tblIngredients" runat="server">
                            <asp:TableHeaderRow TableSection="TableHeader">
                                <asp:TableHeaderCell />
                                <asp:TableHeaderCell Text="Qty" CssClass="Qty" />
                                <asp:TableHeaderCell Text="Unit" />
                                <asp:TableHeaderCell />
                                <asp:TableHeaderCell Text="Ingredient / Subrecipe" />
                                <asp:TableHeaderCell Text="Note" />
                                <asp:TableHeaderCell Text="Remove" />
                                <asp:TableHeaderCell Text="Price" CssClass="Price" />
                            </asp:TableHeaderRow>
                        </asp:Table>
                        <button id="btnAddTitle" type="button">Add Title</button>
                    </asp:Panel>

                    <asp:Panel ID="pnlInstructions" runat="server">
                        <div>Directions</div>
                        <asp:TextBox ID="txtInstructions" TextMode="MultiLine" runat="server" />
                    </asp:Panel>

                    <ul class="Fields">
                        <li>
                            <label class="Prompt"># Servings</label><asp:TextBox ID="txtNumServings" CssClass="Servings" runat="server" /><asp:HiddenField ID="hfOrigNumServings" runat="server" />
<%--
                            <label class="">Serving Ratios: Men</label><asp:TextBox ID="txtMenRatio" CssClass="Ratio" runat="server" />%
                            <label class="">Women</label><asp:TextBox ID="txtWomenRatio" CssClass="Ratio" runat="server" />%
                            <label class="">Children</label><asp:TextBox ID="txtChildRatio" CssClass="Ratio" runat="server" />%
--%>
                            <asp:HyperLink id="lnkScale" NavigateUrl="~/RecipeScale.aspx" Target="ScaleRecipe" runat="server"><button type="button">Scaling</button></asp:HyperLink>
                        </li>
                        <li>
                            <input type="hidden" id="hfYieldUnitRno" class="UnitRno" runat="server" />
                            <label class="Prompt">Yield</label><asp:TextBox ID="txtYield" CssClass="Servings" runat="server" />
                            <asp:TextBox ID="txtYieldUnit" CssClass="Unit" runat="server" />
                        </li>
                        <li>
                            <input type="hidden" id="hfPortionUnitRno" class="UnitRno" runat="server" />
                            <label class="Prompt">Serving Size</label><asp:TextBox ID="txtPortion" CssClass="Servings" runat="server" />
                            <asp:TextBox ID="txtPortionUnit" CssClass="Unit" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">% Base Cost</label><asp:TextBox ID="txtBaseCostPct" CssClass="Servings" runat="server" /><asp:Label ID="lblBaseCostPct" CssClass="Servings" Text="25" runat="server" />%                            
                            <label class="Default"><asp:CheckBox ID="chkUseDefaultBaseCostPct" Text="Use 25% default" runat="server" /></label>
                        </li>
                        <li>
                            <label class="Prompt"></label><label class="FieldTitle">Serving</label>
                            <label class="FieldTitle">Recipe</label>
                        </li>
                        <li>
                            <label class="Prompt">Cost</label><asp:TextBox ID="txtBaseCostPrice" CssClass="Price" Enabled="false" TabIndex="-1" runat="server" />
                            <asp:TextBox ID="txtRecipeBaseCostPrice" CssClass="Price" Enabled="false" TabIndex="-1" runat="server" />
                            <asp:Label ID="lblCost" runat="server" />
                            <asp:Label ID="lblCostNote" runat="server" />

                        </li>
                        <li>
                            <label class="Prompt">Price</label><asp:TextBox ID="txtPortionPrice" CssClass="Price" Enabled="false" TabIndex="-1" runat="server" />
                            <asp:TextBox ID="txtRecipePrice" CssClass="Price" Enabled="false" TabIndex="-1" runat="server" />
                        </li>
                        <li>&nbsp;</li>
                        <li>
                            <label class="Prompt" for="chkGlutenFree">Gluten Free</label><asp:CheckBox ID="chkGlutenFree" CssClass="Source" runat="server" /><span class="GlutenFree"></span>
                        </li>
                        <li>
                            <label class="Prompt" for="chkVegan">Vegan</label><asp:CheckBox ID="chkVegan" CssClass="Source" runat="server" /><span class="Vegan"></span>
                        </li>
                        <li>
                            <label class="Prompt" for="chkVegetarian">Vegetarian</label><asp:CheckBox ID="chkVegetarian" CssClass="Source" runat="server" /><span class="Vegetarian"></span>
                        </li>
                        <li>
                            <label class="Prompt" for="chkDairyFree">Dairy Free</label><asp:CheckBox ID="chkDairyFree" CssClass="Source" runat="server" /><span class="DairyFree"></span>
                        </li>
                        <li>
                            <label class="Prompt" for="chkNuts">Nuts</label><asp:CheckBox ID="chkNuts" CssClass="Source" runat="server" /><span class="Nuts"></span>
                        </li>
                        <li>
                            <label class="Prompt" for="chkIncludeInBook">Include In Book</label><asp:CheckBox ID="chkIncludeInBook" CssClass="Source" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Book Category</label><asp:TextBox ID="txtCategory" CssClass="Source" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Recipe Source</label><asp:TextBox ID="txtSource" CssClass="Source" runat="server" />
                        </li>
                        <li>
                            <div>Note</div>
                            <asp:TextBox ID="txtNote" TextMode="MultiLine" CssClass="RecipeNote" runat="server" />
                        </li>
                        <li class="Buttons">
                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                            <asp:Button ID="btnNew" runat="server" Text="New" OnClick="btnNew_Click" />
                            <a href="" target="ScaleRecipe"><button id="Print" type="button">Scale / Print</button></a>
                            <asp:Button ID="btnCopy" runat="server" Text="Copy" OnClick="btnCopy_Click" />
                        </li>
                    </ul>

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
    <asp:HiddenField ID="hfRecipeRno" runat="server" />
    <asp:HiddenField ID="hfNumMenuItems" runat="server" />
    <asp:HiddenField ID="hfOrigNumMenuItems" runat="server" />
    <asp:HiddenField ID="hfNumIngredients" runat="server" />
    <asp:HiddenField ID="hfOrigNumIngredients" runat="server" />
	<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />

    <div id="RecipesDialog" title="Subrecipe in Recipes">
        <ul id="ulRecipes" runat="server" />
    </div>


	<% Pg.Bottom(); %>
	</form>
</body>
</html>
