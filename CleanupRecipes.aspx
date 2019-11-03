<%@ Page AutoEventWireup="true" CodeFile="CleanupRecipes.aspx.cs" Inherits="CleanupRecipes"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Cleanup Shopping</title> 
		<!-- CleanupReceipes.aspx -->
		<!-- Copyright (c) 2014-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />	
		<link href="ReportStyles.css" type="text/css" rel="stylesheet" />
        <link href="css/font-awesome.min.css" rel="stylesheet" />
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
        <script type="text/javascript">
            $(document).ready(function ()
            {
                $("#tblIngreds a, #tblMenuItems a, #tblSubrecipe a, #tblPurchaseUnits a, #tblRecipes a").click(function ()
                {
                    $(this).addClass("clicked");
                });

                // Non-Purchase
                $(".set-non-purchase").click(function ()
                {
                    var IngredRno = $(this).data("ingredient-rno");
                    var NonPurchase = $("input:checkbox", $(this)).prop("checked");

                    $.post("Service/Ingredient.aspx", { Action: "SetNonPurchase", Rno: IngredRno, NonPurchase: NonPurchase });
                });
                    
                // recipe select
                $(".select-recipe").addClass("prompt");
                $(".select-recipe").change(function ()
                {
                    if ($(this).val() == "0")
                    {
                        $(this).addClass("prompt");
                    }
                    else
                    {
                        $(this).removeClass("prompt");
                    }

                    var MenuItemRno = $(this).data("item-menu-rno");
                    var RecipeRno = $(this).val();

                    $.post("Service/MenuItem.aspx", { Action: "Recipe", Rno: MenuItemRno, RecipeRno: RecipeRno });

                    // mark all the other similar menu items
                    var MenuItem = $(this).closest("tr").find(".menu-item").text();

                    // search back from this position for surrounding rows with the same menu item
                    var tr = $(this).closest("tr").prev();
                    while (tr.find("td.menu-item[data-name='" + MenuItem + "']").length > 0)
                    {
                        var Select = tr.find(".select-recipe").val(RecipeRno);
                        if (Select.val() == "0")
                        {
                            Select.addClass("prompt");
                        }
                        else
                        {
                            Select.removeClass("prompt");
                        }
                        tr = tr.prev();
                    }

                    // search forward
                    var tr = $(this).closest("tr").next();
                    while (tr.find("td.menu-item[data-name='" + MenuItem + "']").length > 0)
                    {
                        var Select = tr.find(".select-recipe").val(RecipeRno);
                        if (Select.val() == "0")
                        {
                            Select.addClass("prompt");
                        }
                        else
                        {
                            Select.removeClass("prompt");
                        }
                        tr = tr.next();
                    }
                });

                // Hide
                $(".set-as-hidden").click(function () {
                    var MenuItemRno = $(this).data("item-menu-rno");
                    var Hide = $("input:checkbox", $(this)).prop("checked");

                    $.post("Service/MenuItem.aspx", { Action: "SetAsHidden", Rno: MenuItemRno, Hide: Hide });

                    // hide all the other similar menu items
                    var MenuItem = $(this).closest("tr").find(".menu-item").text();

                    // search back from this position for surrounding rows with the same menu item
                    var tr = $(this).closest("tr").prev();
                    while (tr.find("td.menu-item[data-name='" + MenuItem + "']").length > 0) {
                        tr.find(".set-as-hidden input").prop("checked", Hide);
                        tr = tr.prev();
                    }

                    // search forward
                    var tr = $(this).closest("tr").next();
                    while (tr.find("td.menu-item[data-name='" + MenuItem + "']").length > 0) {
                        tr.find(".set-as-hidden input").prop("checked", Hide);
                        tr = tr.next();
                    }
                });

                // As Is
                $(".mark-as-is").click(function ()
                {
                    var MenuItemRno = $(this).data("item-menu-rno");
                    var AsIs = $("input:checkbox", $(this)).prop("checked");

                    $.post("Service/MenuItem.aspx", { Action: "MarkAsIs", Rno: MenuItemRno, AsIs: AsIs });

                    // mark all the other similar menu items
                    var MenuItem = $(this).closest("tr").find(".menu-item").text();

                    // search back from this position for surrounding rows with the same menu item
                    var tr = $(this).closest("tr").prev();
                    while (tr.find("td.menu-item[data-name='" + MenuItem + "']").length > 0)
                    {
                        tr.find(".mark-as-is input").prop("checked", AsIs);
                        tr = tr.prev();
                    }

                    // search forward
                    var tr = $(this).closest("tr").next();
                    while (tr.find("td.menu-item[data-name='" + MenuItem + "']").length > 0)
                    {
                        tr.find(".mark-as-is input").prop("checked", AsIs);
                        tr = tr.next();
                    }
                });
            });


        </script>

        <style type="text/css">
            .section
            {
                margin-top: 20px;
            }

            table
            {
                margin: 0px auto;
            }

            tr.title th
            {
                text-align: left;
                text-decoration: underline;
            }

            .rpt tr:hover
            {
                background-color: #DDE6D0;
            }

            .select
            {
                padding-right: 30px;
            }

            a.clicked
            {
                color: rgba(77, 121, 8, 0.5);
            }

            .category
            {
                width: 150px;
            }

            .menu-item
            {
                width: 200px;
            }

            .select-recipe
            {
                width: 200px;
                margin-right: 20px;
            }

            .select-recipe.prompt
            {
                color: #888;
                /*font-style: italic;*/
            }

            .select-recipe.prompt option
            {
                color: #000;
                font-style: normal;
            }

            .mark-as-is
            {
                margin-left: 10px;
                margin-right: 20px;
            }

            .set-non-purchase
            {
                margin-left: 10px;
                margin-right: 20px;
            }
        </style>
	</head>
	<body>
		<form id="form" method="post" autocomplete="off" runat="server">
    	    <% Pg.Top(); %>
			    <div class="FeatureMain">Recipes Cleanup</div>

                <div id="divMenuItems" class="section" runat="server">
			        <div class="FeatureSub">Menu Items with no Recipe</div>
                    <table id="tblMenuItems" class="rpt" runat="server">
                        <thead>
                            <tr>
                                <th colspan="5" style="width: 400px;">The suggested fix is to click the Hide or As Is checkbox for this menu item or select its recipe.</th>
                            </tr>
                            <tr class="title">
                                <th>Category</th>
                                <th>Menu Item</th>
                                <th>Hide</th>
                                <th style="padding-right: 40px;">"As Is"</th>
                                <th>Select Recipe</th>
                            </tr>
                        </thead>
                    </table>
                </div>

                <div id="divSubrecipe" class="section" runat="server">
			        <div class="FeatureSub">Recipe & Subrecipe Units Can't Convert</div>
                    <table id="tblSubrecipe" class="rpt" runat="server">
                        <thead>
                            <tr>
                                <th colspan="4" style="width: 400px;">The suggested fix is to click the Recipe and set the ingredient quantity and unit to use the subrecipe's yield unit.</th>
                            </tr>
                            <tr class="title">
                                <th>Recipe</th>
                                <th>Recipe Unit</th>
                                <th>Yield Unit</th>
                                <th>Ingredient / Subrecipe</th>
                            </tr>
                        </thead>
                    </table>
                </div>

                <div id="divRecipes" class="section" runat="server">
			        <div class="FeatureSub">Recipes with Zero Servings</div>
                    <table id="tblRecipes" class="rpt" runat="server">
                        <thead>
                            <tr>
                                <th style="width: 400px;">The suggested fix is to click the Recipe and set the number of servings.</th>
                            </tr>
                            <tr class="title">
                                <th>Recipe</th>
                            </tr>
                        </thead>
                    </table>
                </div>

		    <% Pg.Bottom(); %>
		</form>
	</body>
</html>
