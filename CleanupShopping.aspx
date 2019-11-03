<%@ Page AutoEventWireup="true" CodeFile="CleanupShopping.aspx.cs" Inherits="CleanupShopping"
	Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Cleanup Shopping</title> 
		<!-- CleanupShopping.aspx -->
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
			    <div class="FeatureMain">Shopping Cleanup</div>

                <div id="divIngreds" class="section" runat="server">
			        <div class="FeatureSub">Ingredients without a Receipt Price</div>
                    <table id="tblIngreds" class="rpt" runat="server">
                        <thead>
                            <tr>
                                <th colspan="3" style="width: 400px;">The suggested fix is to click the Receipt Price link to enter a "No Vendor" receipt with the last price or your best estimate or click the Non Purchase link to set the checkbox for this ingredient.</th>
                            </tr>
                            <tr class="title">
                                <th>Ingredient</th>
                                <th></th>
                                <th>Non-Purchase</th>
                            </tr>
                        </thead>
                    </table>
                </div>

                <div id="divPurchaseUnits" class="section" runat="server">
			        <div class="FeatureSub">Ingredients with no Price Calculation<br /> (Missing Recipe Unit to Purchase Unit Conversion)</div>
                    <table id="tblPurchaseUnits" class="rpt" runat="server">
                        <thead>
                            <tr>
                                <th colspan="3" style="width: 400px;">The suggested fix is to click the Ingredient and enter the missing quantity that allows the recipe unit to convert to the purchase unit.</th>
                            </tr>
                            <tr class="title">
                                <th>Ingredient</th>
                                <th>Recipe Unit</th>
                                <th>Purchase Unit</th>
                            </tr>
                        </thead>
                    </table>
                </div>

		    <% Pg.Bottom(); %>
		</form>
	</body>
</html>
