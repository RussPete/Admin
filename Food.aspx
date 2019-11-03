<%@ Page AutoEventWireup="true" CodeFile="Food.aspx.cs" Inherits="Food" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Food</title> 
		<!-- Food.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Styles.css" type="text/css" rel="stylesheet">
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
       	<link href="css/font-awesome.min.css" type="text/css" rel="stylesheet">
        <style>
            .Quantity { width: 50px; text-align: right; }
            .icon-dollar { color: firebrick; padding-left: 3px; }
            .OverUnder { font-size: smaller; }
            .Over { color: black; }
            .Under { color: firebrick; }
            .Serving { border-top: solid 1px #CCC; padding-top: 3px; }
            .Total { padding-top: 3px; }
            .Notice { font-weight: bold; padding-top: 3px; }
            .lblIngredSel { display: none; cursor: pointer; }
            .IngredSelQtip { font-weight: bold; text-decoration: underline; white-space: nowrap; }
        </style>
		<!--<script language="javascript" src="SelectList.js" type="text/javascript"></script>-->
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery.formatCurrency-1.4.0.min.js" type="text/javascript"></script>
    	<script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="https://use.fontawesome.com/ed45f487e9.js" type="text/javascript"></script>
        <script language="JavaScript" src="RecentJobs.js" type="text/javascript"></script>
		<script language="JavaScript" type="text/javascript">
		
		//----------------------------------------------------------------------------		
		function Init()
		{
		<%	if (FocusField != "")
			{ %>
			SetFocus("<%=FocusField%>");
		<%	} %>
		}
		
		//var fCategorySelected = false;
		//var fMenuItemSelected = false;
		var AddRowHtml = "<%= AddRowHtml() %>";
        var NumServings = <%= NumServings %>;
		
		$(document).ready(function()
		{
		    SearchList("#txtSearch", "#lstList");

		    // sort food list by cateory or menu item depending radio buttons
		    $("#rbSortCategory, #rbSortCategoryAlpha, #rbSortMenuItem").change(function ()
			{
				SortList();
			});
			
			// sort the list
			SortList();
			
			// prep selection in food list
			$("#lstList").click(function()
			{
				var Food = $(":selected", this);
				var Row = AddFood(Food.attr("category"), Food.attr("menuitem"));
				
				if (Food.attr("multitems"))
				{
					CheckMenuItemMultiSelect(Row.find(".Quantity"));
				}
			});
			
			// remove tab stops on remove checkboxes
			$("input[type='checkbox']").attr("tabindex", -1);
			
			// prep autocompletes on Category inputs
			CategoryAutoComplete("#tblFood input.FoodCategory");
					
			// prep autocompletes on Menu Items and Category multi-select
			$("#tblFood input.MenuItem").each(function()
			{
				MenuItemFocus(this);
			});
						
			// prep Menu Item multi-select
			$("#tblFood input.Quantity").each(function()
			{
				QuantityFocus(this);
			});
						
			// prep auto add line after last Category is set			
			$("#tblFood").on("blur", "input.FoodCategory:last", AddBlankRow);

			CalcTotals();

			$("#tbFood").on("click", ".lblIngredSel", function()
			{
			    var qty = $(this).closest("tr").find(".Quantity")[0];
			    CheckMenuItemIngredSelect(qty, true);
			});

			$("#tbFood").on("change", ".Quantity", function()
			{
			    $(this).val(FmtNum(Num($(this).val()), 0, ""));
			    CalcTotals();
			});

			SetupRecentJobs();

			Qtips(".icon-dollar, .icon-check");

			AddBlankRow();

			$(".FoodCategory:last").focus();

		    // dirty bit
			$(".EditData :input:not([readonly='readonly']):not([disabled='disabled'])").change(function () 
			{
			    SetDirty();
			});

			$(window).bind("beforeunload", function () 
			{
			    return CheckDirty();
			});
		});
		
		var fSortByCategory;
		var fSortByCategoryAlpha;
		var fSortByMenuItem;

		function SortList()
		{
			fSortByCategory         = $("#rbSortCategory").is(":checked");
			fSortByCategoryAlpha    = $("#rbSortCategoryAlpha").is(":checked");
			fSortByMenuItem         = $("#rbSortMenuItem").is(":checked");
			var Items = $("#lstList option").detach();
			var Field1 = (fSortByCategory || fSortByCategoryAlpha ? "category" : "menuitem");
			var Field2 = (fSortByCategory || fSortByCategoryAlpha ? "menuitem" : "category");
			
			for (var i = 0; i < Items.length; i++)
			{
				var Item = $(Items[i]);
				Item.text(Item.attr(Field1) + " - " + Item.attr(Field2));
			}
						
			Items.sort(CompareListItems);			
			$("#lstList").append(Items);
		}
		
		function CompareListItems(a, b)
		{
		    if (fSortByCategory)
		    {
		        var A = ($(a).attr("sortorder") + "-" + $(a).attr("category") + "-" + $(a).attr("itemsortorder") + "-" + $(a).attr("menuitem")).toLowerCase();
		        var B = ($(b).attr("sortorder") + "-" + $(b).attr("category") + "-" + $(b).attr("itemsortorder") + "-" + $(b).attr("menuitem")).toLowerCase()
		    }
		    else if (fSortByCategoryAlpha) {
		        var A = ($(a).attr("category") + "-" + $(a).attr("itemsortorder") + "-" + $(a).attr("menuitem")).toLowerCase();
		        var B = ($(b).attr("category") + "-" + $(b).attr("itemsortorder") + "-" + $(b).attr("menuitem")).toLowerCase()
		    }
            else
		    {
		        var A = $(a).text().toLowerCase();
		        var B = $(b).text().toLowerCase();
		    }

			return ((A < B) ? -1 : ((A > B) ? 1 : 0));
		}
		
		function CompareCategoryMenuItem(a, b)
		{
			//var A = ($(a).attr("category") + " - " + $(a).attr("menuitem")).toLowerCase();
			//var B = ($(b).attr("category") + " - " + $(b).attr("menuitem")).toLowerCase();
		    var A = ($(a).attr("sortorder") + "-" + $(a).attr("category") + "-" + $(a).attr("itemsortorder") + "-" + $(a).attr("menuitem")).toLowerCase();
		    var B = ($(b).attr("sortorder") + "-" + $(b).attr("category") + "-" + $(b).attr("itemsortorder") + "-" + $(b).attr("menuitem")).toLowerCase();
			return ((A < B) ? -1 : ((A > B) ? 1 : 0));
		}
		
		function CategoryAutoComplete(Selector)
		{
			//fCategorySelected = false;
			var FoodCategories = Categories();
			$(Selector)
				.autocomplete({
				    //source: Categories(),
				    source: function(req, response) { 
				    	var re = $.ui.autocomplete.escapeRegex(req.term); 
				    	var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
				    	response($.grep(FoodCategories, function(Item, ItemIndex)
				    	{ 
				    		return Matcher.test(Item); 
				    	}) ); 
				    },			
				    autoFocus: true,
				    delay: 0,
				    minLength: 0,
					select: function()
					{
						//console.log("Category select");
					    //fCategorySelected = true;
					    SetDirty();
					},
				    /*
					change: function()
					{
						console.log("Category change")
						fCategorySelected = false;
					},
					close: function()
					{
						console.log("Category close")
						if (fCategorySelected)
						{
							fCategorySelected = false;
							$(this).parent("td").next().next().find("input.MenuItem").focus();
						}
					}
					*/
				})
				// auto drop down on focus of Category input box
				.focus(function ()
				{
				    $(this).autocomplete("search", $(this).val());
				});
                /*
				// filter out tabs, they were messing up the flow when a category is selected in the drop down list.
				.keydown(function(event)
				{
					if (event.which == 9)
					{
						event.preventDefault();
					}
				});
				*/
		}
		
		var fMenuItemAutocompleteIsOpen = false;

		function MenuItemFocus(elem)
		{			
			console.log("MenuItem focus");
			//fMenuItemSelected = false;
			$(elem)
				// auto drop down on focus of Menu Item input box
				.focus(function()
				{
					var fMultiSelectCategory = CheckCategoryMultiSelect(this);					
					CheckNewCategory(this);

					{
						console.log("MenuItem focus cateogry ok");
						//var FoodItems = MenuItems(this);
						$(this)
							.autocomplete({
							    source: MenuItems(this),
							    //source: function(req, response) { 
							    //	var re = $.ui.autocomplete.escapeRegex(req.term); 
							    //	var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
							    //	response($.grep(FoodItems, function(Item, ItemIndex)
							    //	{ 
							    //		return Matcher.test(Item); 
							    //	}) ); 
							    //},			
							    //autoFocus: true,
							    delay: 0,
							    minLength: 0,
								select: function(event, ui)
								{
									//console.log("MenuItem select");
								    //fMenuItemSelected = true;

								    $(elem).val(ui.item.value);

								    var tr = $(elem).closest("tr");
								    tr.find("input.IngredSelAutoPop").val(true);
								    tr.find("input.IngredSelFlg").val("");
								    tr.find(".lblIngredSel").hide();

								    tr.find("span[id^='lblQuote']").html(ui.item.Quote);
								    tr.find("span[id^='lblPrice']").html(ui.item.Price);
								    if (ui.item.InaccuratePrice)
								    {
								        var InaccuratePrice = tr.find("span[id^='lblInaccuratePrice']");
								        InaccuratePrice.addClass("icon-dollar")
								        InaccuratePrice.prop("title", "Cost and Price are inaccurate because there are ingredients with no price from receipts.");
								        Qtips(InaccuratePrice);
								    }
                                    
								    CalcTotals();
								    SetDirty();
								},
							    /*
								change: function()
								{
									console.log("MenuItem change")
									fMenuItemSelected = false;
								},
                                */
								open: function()
								{
								    fMenuItemAutocompleteIsOpen = true;
								},
								close: function()
								{
								    fMenuItemAutocompleteIsOpen = false;
									//console.log("MenuItem close")
									//if (fMenuItemSelected)
									//{
									//	fMenuItemSelected = false;
									//	$(this).parent("td").next().next().find("input").focus();
									//}
								}
							});
						if (!fMultiSelectCategory)
						{
						    $(this).autocomplete("search", $(this).val());
						}
					}
				});
				/*
				// filter out tabs, they were messing up the flow when a menuitem is selected in the drop down list.
				.keydown(function(event)
				{
					if (event.which == 9)
					{
						event.preventDefault();
					}
				});
				*/
		}
		
		// prep Menu Item for multi-select and new value
		function QuantityFocus(elem)
		{
			$(elem)
				// on focus, see if menu item is a multi-select and a new value
				.focus(function()
				{
				    CheckMenuItemMultiSelect(this);
				    CheckMenuItemIngredSelect(this);
					CheckNewMenuItem(this);
				});
		}
		
		function Categories()
		{
			var Items = $("#lstList option").sort(CompareCategoryMenuItem);
			var Cats = new Array();
			var iCats = 0;
			var Prev = "";
			for (var i = 0; i < Items.length; i++)
			{
				var Cat = $(Items[i]).attr("category");
				if (Cat != Prev)
				{
					Cats[iCats++] =
					Prev = Cat;
				}
			}
			return Cats;
		}
		
		function MenuItems(elem)
		{
			var Category = $(elem).closest("tr").find("input.FoodCategory").val();
			var Items = $('#lstList option[category="' + Category + '"]').sort(CompareCategoryMenuItem);
			var MenuItems = new Array();
			var iMenuItems = 0;
			var Prev = "";
			for (var i = 0; i < Items.length; i++)
			{
				var MenuItem = $(Items[i]).attr("menuitem");
				if (MenuItem != Prev)
                {
                    var Extra = "";
                    if ($(Items[i]).attr("GlutenFree"))
                    {
                        Extra += " [GF]";
                    }
                    if ($(Items[i]).attr("Vegan"))
                    {
                        Extra += " [V]";
                    }
                    if ($(Items[i]).attr("Vegetarian"))
                    {
                        Extra += " [Veg]";
                    }
                    if ($(Items[i]).attr("DairyFree"))
                    {
                        Extra += " [DF]";
                    }
                    if ($(Items[i]).attr("Nuts"))
                    {
                        Extra += " [N]";
                    }
				    MenuItems[iMenuItems++] = 
                    {
                        label: MenuItem + Extra,
                        value: MenuItem,
                        Quote: $(Items[i]).attr("Quote"),
                        Price: $(Items[i]).attr("Price"),
                        InaccuratePrice: $(Items[i]).attr("InaccuratePrice"),
				    };
					Prev = MenuItem;
				}
			}
			return MenuItems;
		}
		
		var fSkipCategoryMultiSelect = false;

		function CheckCategoryMultiSelect(elem)
		{
		    var fMultiSelect = false;

			// if this menu item is blank and the selected cateory is set for multi-select
			if ($(elem).val() == "")
			{
				var Category = $(elem).closest("tr").find("input.FoodCategory").val();
				var MultiSelect = $('#lstList option[category="' + Category + '"]').first().attr("catmultsel");
				
				if (!fSkipCategoryMultiSelect && !fMenuItemAutocompleteIsOpen)
				{
				    if (MultiSelect && MultiSelect.toLowerCase() == "true") {
				        var Popup = $("#MultiSelectMenuItems");
				        var MultiItems = Popup.find(".Items").html("");

				        // insert the menu items to select
				        var Items = $('#lstList option[category="' + Category + '"]').sort(CompareCategoryMenuItem);
				        Items.each(function (iItem, Item) {
				            var Attributes = 
                                ($(Item).attr("GlutenFree") ? "<span class='GlutenFree'>" : "") + 
                                ($(Item).attr("Vegan")      ? "<span class='Vegan'>" : "") + 
                                ($(Item).attr("Vegetarian") ? "<span class='Vegetarian'>" : "") + 
                                ($(Item).attr("DairyFree")  ? "<span class='DairyFree'>" : "") + 
                                ($(Item).attr("Nuts")       ? "<span class='Nuts'>" : "");
				            MultiItems.append("<li><input type='checkbox' id='MenuItem" + iItem + "' /> <label for='MenuItem" + iItem + "' menuitem='" + $(Item).attr("menuitem") + "'>" + $(Item).val() + "</label>" + Attributes + "</li>\n");
				        });

				        // show the popup
				        Popup.dialog({
				            buttons: {
				                "None": function()
				                {
				                    $("#tblFood tr:last input.FoodCategory").focus();
				                    $(this).dialog("close");
				                },
				                "Save": function () {
				                    // blank out the category of the current line
				                    $(elem).closest("tr").find("input.FoodCategory").val("");

				                    // add all the selected menu items
				                    $("input:checked", $(this)).each(function (iItem, Item) {
				                        AddFood(Category, $(Item).next().attr("menuitem"));
				                    });

				                    // set the focus on the new category field
				                    console.log("set focus last food category");
				                    $("#tblFood tr:last input.FoodCategory").focus();

				                    $(this).dialog("close");
				                }
				            },
				            modal: true,
				            width: 450,
				            open: function (event, ui) {
				                $(this).css({ "max-height": 600, "overflow-y": "auto" });
				            },
				            close: function (event, ui) {
				                // if nothing was checked
				                if ($("input:checked", $(this)).length == 0) {
				                    // set the cursor in the menu item field so a new menu item can be added to this category
				                    fSkipCategoryMultiSelect = true;
				                    $(elem).focus();
				                }
				            }
				        });
				        fMultiSelect = true;
				    }
				}
				fSkipCategoryMultiSelect = false;
			}
			return fMultiSelect;
		}
		
		function CheckMenuItemMultiSelect(elem)
		{
			console.log("CheckMenuItemMultiSelect");

			// if this quantity field is blank and the selected menu item is set for multi-select
			if ($(elem).val() == "")
			{
				var Category = $(elem).closest("tr").find("input.FoodCategory").val();
				var MenuItem = $(elem).closest("tr").find("input.MenuItem").val();
				var MultiSelect = $('#lstList option[category="' + Category + '"][menuitem="' + MenuItem + '"]').first().attr("multitems");
				
				if (MultiSelect && MultiSelect.length > 0)
				{
					var aItemRno = MultiSelect.split(",")
					var Popup = $("#MultiSelectMenuItems");
					var MultiItems = Popup.find(".Items").html("");
					
					var aItem = new Array();					
					var iItem = -1;
					var iItemRno;
					for (iItemRno = 0; iItemRno < aItemRno.length; iItemRno++)
					{
					  if (aItemRno[iItemRno] != "")
					  {
					    var Item = $('#lstList option[rno="' + aItemRno[iItemRno] + '"]');
					    if (Item.length > 0)
					    {
					      aItem[++iItem] = Item.first();
					    }
					  }
					}
				
					// insert the menu items to select
					var Items = aItem.sort(CompareCategoryMenuItem);
					for (iItem = 0; iItem < Items.length; iItem++)
					{
						var Item = $(Items[iItem]);
						MultiItems.append("<li><input type='checkbox' id='MenuItem" + iItem + "' category='" + Item.attr("category") + "' menuitem='" + Item.attr("menuitem") + "' /> <label for='MenuItem" + iItem + "'>" + Item.val() + "</label></li>\n");
					}
					
					// show the popup
					Popup.dialog({
						buttons:{
							"Save": function()
							{								
								// add all the selected menu items
								$("input:checked", $(this)).each(function(iItem, Item)
								{
								    var Food = $(Item);
									AddFood(Food.attr("category"), Food.attr("menuitem"));
								});
								
								// set the focus on the new category field
								//$("#tblFood tr:last input.FoodCategory").focus();
								console.log("set focus quantity");
								$(elem).focus();
								
								$(this).dialog("close");
							}
						},
						modal: true,
						width: 450,
						open: function(event, ui) 
						{
							$(this).css({"max-height": 600, "overflow-y": "auto"}); 
						}
					});
				}
			}				
		}

		function CheckMenuItemIngredSelect(elem, fEdit)
		{
		    console.log("CheckMenuItemIngredSelect");

		    // if auto popup and the selected menu item is set for ingred-select
		    var tr = $(elem).closest("tr");
		    var fIngredSelectAutoPop = (tr.find("input.IngredSelAutoPop").val().toLowerCase() == "true");
		    if (fIngredSelectAutoPop || fEdit)
		    {
		        var JobFoodRno = tr.find("input.JobFoodRno").val();
		        var Category = tr.find("input.FoodCategory").val();
		        var MenuItem = tr.find("input.MenuItem").val();
		        var MenuItemData = $('#lstList option[category="' + Category + '"][menuitem="' + MenuItem + '"]').first();
		        var MenuItemIngreds = MenuItemData.attr("Ingred");
		        var IngredSelectFlg = tr.find("input.IngredSelFlg").val();
		        var IngredSelect = tr.find("input.IngredSel").val();
		        var fNewItem = (IngredSelectFlg.length == 0);
				
		        if ((fNewItem || fEdit) && MenuItemIngreds && MenuItemIngreds.length > 0)
		        {
		            var aXref = MenuItemIngreds.split("~")
		            var Popup = $("#IngredSelect");
		            var IngredItems = Popup.find(".Ingred").html("");
		            var aIngredSelected = IngredSelect.split(",");
					
		            var aIngred = new Array();					
		            var iIngred = -1;
		            var iXref;
		            for (iXref = 0; iXref < aXref.length; iXref++)
		            {
		                if (aXref[iXref] != "")
		                {
		                    var Parts = aXref[iXref].split("|");
		                   //var Checked = (aIngredSelected.length == 1 && aIngredSelected[0].length == 0 ? "checked" : "");
		                    var Checked = (fIngredSelectAutoPop ? "checked" : "");
		                    for (var iSelected = 0; iSelected < aIngredSelected.length; iSelected++)
		                    {
		                        if (Parts[0] == aIngredSelected[iSelected])
		                        {
		                            Checked = "checked";
		                            break;
		                        }
		                    }
		                    var Attributes = 
                                (Parts[2] == "1" ? "<span class='GlutenFree'>" : "") +
                                (Parts[3] == "1" ? "<span class='Vegan'>" : "") +
                                (Parts[4] == "1" ? "<span class='Vegetarian'>" : "") +
                                (Parts[5] == "1" ? "<span class='DairyFree'>" : "") +
                                (Parts[6] == "1" ? "<span class='NutsFlg'>" : "");
		                    IngredItems.append("<li><input type='checkbox' id='Ingred" + iXref + "' XrefRno='" + Parts[0] + "' " + Checked + "/> <label for='Ingred" + iXref + "'>" + Parts[1] + "</label>" + Attributes + "</li>\n");
		                }
		            }
				
					
		            // show the popup
		            Popup.dialog({
		                buttons:{
		                    All: function()
		                    {
		                        $("input", $(this)).each(function (index)
		                        {
		                            $(this).prop("checked", true);
		                        });
		                    },
		                    None: function()
		                    {
		                        $("input", $(this)).each(function (index)
		                        {
		                            $(this).prop("checked", false);
		                        });
		                    },
		                    Save: function()
		                    {								
		                        SaveMenuItemIngredSelect(this, elem);
		                    }
		                },
		                modal: true,
		                width: 450,
		                open: function(event, ui) 
		                {
		                    $(this).css({"max-height": 600, "overflow-y": "auto"}); 
		                },
		                close: function(event, ui) 
		                {
		                    //setTimeout(function () { $(elem).focus(); }, 100);
		                    $(elem).focus();
		                }
		            });
		            tr.find(".lblIngredSel").show();
		            tr.find(".IngredSelFlg").val(true);
		        }
		        tr.find("input.IngredSelAutoPop").val(false);
		    }				
		}

		function SaveMenuItemIngredSelect(that, elem)
		{
		    var IngredSel = "";     // default to every ingredient is checked
		    var ToolTip = "<div class='IngredSelQtip'>Selected Ingredients</div>";

		    // what ingredients are checked
		    var checkboxes = $("input", $(that));
		    var checked = $("input:checked", $(that));

		    // is everything checked
		    //if (checkboxes.length != checked.length) 
		    {
		        // add all the selected menu items
		        $("input:checked", $(that)).each(function(iIngred, Ingred)
		        {
		            var Food = $(Ingred);
		            IngredSel += $(Ingred).attr("XrefRno") + ",";
		        });
		        // remove last unneeded ","
		        if (IngredSel.length > 0) 
		            IngredSel = IngredSel.substr(0, IngredSel.length - 1);
		    }

		    checked.each(function(iIngred, checked) 
		    {
		        ToolTip += $(checked).next().text() + "<br/>";
		    });

		    var tr = $(elem).closest("tr");
		    tr.find(".IngredSel").val(IngredSel);

		    var icon = tr.find(".lblIngredSel");
		    icon.show();
		    console.log("Tip");
		    //var id = icon.attr("id");
		    //setTimeout(function() { QtipUpdate("#" + id, ToolTip); }, 500);
		    //setTimeout(function() { QtipUpdate("#" + id, ToolTip); }, 5000);
		    QtipUpdate("#" + icon.attr("id"), ToolTip);
		    console.log("Tip Set");
								
		    // set the focus on the new category field
		    //$("#tblFood tr:last input.FoodCategory").focus();
		    console.log("set focus quantity");
								
		    $(that).dialog("close");
		}

		function CheckNewCategory(elem)
		{
			var fOK = true;
			// if this menu item is blank and the selected category is new
			if ($(elem).val() == "")
			{
				var tr = $(elem).closest("tr");
				var Category = tr.find("input.FoodCategory").val();
				//var fNew = ($('#lstList option[category="' + Category + '"]').length == 0);

			    // check to see if the category already exists
				var fNew = true;
				//var CAT = Category.toUpperCase();
				for (var iCat = 0; iCat < aCategories.length; iCat++)
				{
				    //if (CAT == aCategories[iCat].cat.toUpperCase())
			        if (Category == aCategories[iCat].cat)
			        {
				        fNew = false;
				        break;
				    }
				}
				
				if (fNew)
				{
					$("#NewCategory").dialog(
					{
						buttons:
						{
							"Yes": function()
							{
								tr.find("input.MenuItem").val("").focus();
								$(this).dialog("close");
							},
							"No": function()
							{
								tr.find("input.FoodCategory").val("").focus();
								$(this).dialog("close");
							}
						},
						modal: true,
						width: 350
					});
				}
			}
			
			console.log("CheckNewCategory " + fOK);
			return fOK;
		}
		
		var fSkipCloseFocus = false;
		
		function CheckNewMenuItem(elem)
		{
			fSkipCloseFocus = false;
			
			// if this quantity field is blank and the selected menu item is new
			if ($(elem).val() == "")
			{
				var tr = $(elem).closest("tr");
				var Category = tr.find("input.FoodCategory").val();
				var MenuItem = tr.find("input.MenuItem").val();
				var lowerMenuItem = MenuItem.toLowerCase();
				//var fNew = ($('#lstList option[category="' + Category + '"][menuitem="' + MenuItem + '"]').length == 0);
				
			    // check to see if the category and menu item already exists
				var fNew = true;
				for (var iCat = 0; iCat < aCategories.length; iCat++)
				{
				    var cat = aCategories[iCat];
				    if (Category == cat.cat)
				    {
				        for (var iMenuItem = 0; iMenuItem < cat.items.length; iMenuItem++)
				        {
				            if (lowerMenuItem == cat.items[iMenuItem].toLowerCase())
				            {
				                fNew = false;
				                tr.find("input.MenuItem").val(cat.items[iMenuItem]);
				                break;
				            }
				        }
			            break;
				    }
				}

				if (fNew)
				{
					$("#ddlCategory").val(Category);
					$("#txtMenuItem").val(MenuItem);
					$("#txtProposal").val(MenuItem);
					
					// prep new menu item popup
					$("#NewMenuItem").dialog(
					{
						buttons:
						{
							"Save": function()
							{								
								//tr.find("input.FoodCategory").val($("#ddlCategory").val());
								tr.find("input.MenuItem").val($("#txtMenuItem").val());								
								tr.find("input.Proposal").val($("#txtProposal").val());								
								tr.find("input.LocRno").val($("#ddlLocation").val());								
								tr.find("input.OneTime").val($("#chkOneTimeItem").prop("checked"));

								console.log("set focus on quantity after saving new item");
								$(elem).focus();
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
								console.log("set focus on Menu Item");
								tr.find("input.MenuItem").focus();
							}
						}
					});
					
					console.log("set focus on new menu item popup field");
					$("#txtMenuItem").focus();
				}
			}
		}
		
		function AddFood(Category, MenuItem)
		{
			var BlankRow = null;
			// find the last blank row
			$("#tblFood input.FoodCategory").each(function(iItem, Item)
			{
			    var Inputs = $(Item).closest("tr").find("input[type=text]");
				if (BlankRow == null && 
					$(Inputs[0]).val() == "" && 
					$(Inputs[1]).val() == "" && 
					$(Inputs[2]).val() == "" && 
					$(Inputs[3]).val() == "")
				{
					BlankRow = $(Item).closest("tr");
				}
			});
			
			if (BlankRow == null)
			{
				BlankRow = tr = $("#tblFood tr").last();
			}
			$("input.FoodCategory", BlankRow).val(Category);
			$("input.MenuItem", BlankRow).val(MenuItem);
			AddBlankRow();
			
			return BlankRow;
		}
		
		function AddBlankRow()
		{
		    if ($("#tbFood input.FoodCategory:last").length == 0 || 
                $("#tbFood input.FoodCategory:last").val().length > 0)
			{
				var NumItems = $("#tbFood tr").length;
				$("#tbFood").append(AddRowHtml.replace(/~ID~/g, NumItems));
				$("#tbFood input[type='checkbox']:last").attr("tabindex", -1);
				$("#hfNumItems").val(NumItems);
				CategoryAutoComplete("#tbFood input.FoodCategory:last");
				MenuItemFocus($("#tbFood input.MenuItem:last"));				
				QuantityFocus($("#tbFood input.Quantity:last"));				
				//Qtips("#lblIngredSel" + NumItems);
			}
		}

		function CalcTotals()
		{
		    if ($("#tblFood tbody").length == 1)
		    {
		        $("#tblFood tbody").attr("id", "tbFood");
		        $("#tblFood tbody").after(
                    "<tbody id='tbTotal'>" + 
                        "<tr><td colspan='10' /><td class='Right Notice'>Serving</td><td /><td id='QuoteAvg' class='Right Serving Notice' /><td class='Serving' /><td id='PriceAvg' class='Right Serving' /><td /><td class='Serving' /><td id='QuoteTot' class='Right Total Notice Serving' /><td class='Serving' /><td id='PriceTot' class='Right Total Serving' /><td class='Serving' /><td id='PriceNote' class='OverUnder Serving' /></tr>" + 
                        "<tr><td colspan='12' /><td colspan='10' id='AvgNote'><sup>*</sup><span style='font-size: smaller;'>Total Per Serving Quote has been averaged since some item quantities are different than job servings.</span></td></tr>" +
                    "</tbody>");
		    }

		    var QuoteTot = 0;
		    var PriceTot = 0;
		    var QuoteAvg = 0;
		    var PriceAvg = 0;
		    var fAverage = false;

		    $("#tbFood tr").each(function()
		    {
		        var jqQty = $(this).find(".Quantity");
		        if (jqQty.length > 0)
		        {
		            var Qty = jqQty.asNumber(); 
		            if (Qty == 0) Qty = NumServings;
		            var Quote = $(this).find("span[id^='lblQuote']").asNumber();
		            var Price = $(this).find("span[id^='lblPrice']").asNumber();

		            var OverUnder = (Price != 0 ? (Quote - Price) / Price * 100 : 0);
		            var Span = $(this).find("span[id^='lblOverUnder']");
		            Span.html(FmtPct(Math.abs(OverUnder), 0, "0") + (OverUnder >= 0 ? " over" : " under"));
		            Span.removeClass("Over Under").addClass(OverUnder >= 0 ? "Over" : "Under");

		            var ExtQuote = Qty * Quote;
		            var ExtPrice = Qty * Price;
		            QuoteAvg += ExtQuote / NumServings;
		            PriceAvg += ExtPrice / NumServings;
		            QuoteTot += ExtQuote;
		            PriceTot += ExtPrice;

		            $(this).find("span[id^='lblExtQuote']").html(ExtQuote).formatCurrency();
		            $(this).find("span[id^='lblExtPrice']").html(ExtPrice).formatCurrency();

		            if (Qty != NumServings)
		            {
		                fAverage = true;
		            }
                }
		    });

		    $("#QuoteAvg").html(QuoteAvg).formatCurrency();
		    $("#PriceAvg").html(PriceAvg).formatCurrency();
		    $("#QuoteTot").html(QuoteTot).formatCurrency();
		    $("#PriceTot").html(PriceTot).formatCurrency();

		    var OverUnder = (PriceAvg != 0 ? (QuoteAvg - PriceAvg) / PriceAvg * 100 : 0);
		    var Note = $("#PriceNote");
		    Note.html(FmtPct(Math.abs(OverUnder), 0, "0") + (OverUnder >= 0 ? " over" : " under"));
		    Note.removeClass("Over Under").addClass(OverUnder >= 0 ? "Over" : "Under");

		    if (fAverage)
		    {
		        $("#QuoteAvg").prepend("*");
		        $("#AvgNote").show();
		    }
		    else
		    {
		        $("#AvgNote").hide();
		    }
        }
		
        var aCategories = [ <%= CatInfo %> ];
		</script>
		<%//=Utils.SelectList.JavaScript()%>
	</head>
	<body onbeforeunload="CheckDirty();" onload="Init();">
		<form id="form" method="post" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
   					<td valign="top">
						<table cellspacing="0" cellpadding="0" align="center" border="0" style="display: none;">
							<tr>
								<td align="left">
									<table class="SelectFilter" cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>Sort By:</td>
										</tr>
										<tr>
											<td><asp:radiobutton id="rbSortCategory" runat="server" groupname="rbSortBy" text="Category Sort" /></td>
											<td><asp:radiobutton id="rbSortMenuItem" runat="server" groupname="rbSortBy" text="Menu Item" /></td>
										</tr>
										<tr>
											<td style="height: 20px"><asp:radiobutton id="rbSortCategoryAlpha" runat="server" groupname="rbSortBy" text="Category Alpha" /></td>
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
								<td><asp:listbox id="lstList" runat="server" cssclass="SelectJob"></asp:listbox></td>
							</tr>
							<tr>
								<td><asp:label id="lblRecCount" runat="server" cssclass="SelectCount">RecCount</asp:label></td>
							</tr>
						</table>
					</td>
					<td><img height="1" src="Images/Space.gif" alt="" width="10"></td>
					<td style="position: relative;">
                        <%= RecentJobs.Html() %>
						<% Pg.JobSubPage("Menu"); %>
						<asp:table id="tblFood" runat="server" cellspacing="0" cellpadding="0" CssClass="EditData">
							<asp:tablerow>
								<asp:tablecell></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Remove" font-bold="true"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Category" font-bold="true"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Menu Item" font-bold="true"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Quantity" font-bold="true"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Service Note" font-bold="true" runat="server" ID="tdNote"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Quote" font-bold="true" CssClass="Right"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Price" font-bold="true" CssClass="Right"></asp:tablecell>
                                <asp:tablecell></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Ext Quote" font-bold="true" CssClass="Right"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="Ext Price" font-bold="true" CssClass="Right"></asp:tablecell>
								<asp:tablecell width="10px"></asp:tablecell>
								<asp:tablecell text="" font-bold="true"></asp:tablecell>
							</asp:tablerow>
						</asp:table>
						<table cellspacing="0" cellpadding="0" align="center" border="0">
							<tr>
								<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
							</tr>
							<tr>
								<td><asp:button id="btnUpdate" runat="server" text="Save" OnClick="btnUpdate_Click"></asp:button></td>
							</tr>
							<tr>
								<td><img height="10" src="Images/Space.gif" alt="" width="1"></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<asp:HiddenField ID="hfNumItems" runat="server"></asp:HiddenField>
			<asp:TextBox ID="txtcItems" runat="server" Visible="false" />
			<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server">
			<% Pg.Bottom(); %>
			
			<div id="NewMenuItem" title="New Menu Item" style="display: none;">
				<p>This is a new menu item. Please double check the spelling and set the given values.</p>
				<p>If you think the item already exists, click the X to go back and try to find it.</p>
				<dl>
				<% /*
					<dt>Category</dt>
					<dd><asp:DropDownList ID="ddlCategory" runat="server" /></dd>
					*/ %>
					<dt>Menu Item</dt>
					<dd><input id="txtMenuItem" type="text" /></dd>
					<dt>Proposal</dt>
					<dd><input id="txtProposal" type="text" /></dd>
					<dt>Kitchen Location</dt>
					<dd><asp:DropDownList ID="ddlLocation" runat="server" /></dd>
                    <dt></dt>
                    <dd></dd>
					<dt></dt>
					<dd><input type="checkbox" id="chkOneTimeItem" value="true" /><label for="chkOneTimeItem">One Time Item</label></dd>
				</dl>
			</div>		
		</form>
		<%//=Utils.SelectList.DefValues()%>
		
		<div id="MultiSelectMenuItems" title="Select One or More Menu Items">
			<ul class="Items"></ul>
		</div>
		<div id="IngredSelect" title="Ingredient Selection">
			<ul class="Ingred"></ul>
		</div>
		<div id="NewCategory" title="Undefined Category" style="display: none;"><p>This category is not in the system. Do you want to add it?</p></div>
	</body>
</html>
