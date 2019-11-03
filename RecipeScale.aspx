<%@ Page AutoEventWireup="true" CodeFile="RecipeScale.aspx.cs" Inherits="RecipeScale"
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
		<script language="javascript" src="web.js" type="text/javascript"></script>
		<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
		<script language="javascript" src="js/jquery.formatCurrency-1.4.0.min.js" type="text/javascript"></script>
        <script type="text/javascript">
            $(document).ready(function ()
            {
                if ($("#hfOrigServings").asNumber() == 0)
                {
                    $(".Servings button, .Servings input").prop("disabled", true);
                }

                $(".Servings button").click(function ()
                {
                    $("#txtServings").val($(this).text());
                    $("#txtMultiplier").val($("#txtServings").asNumber() / $("#hfOrigServings").asNumber()).toNumber();
                    $("#btnScale").click();
                });

                $(".Multiplier button").click(function ()
                {
                    $("#txtMultiplier").val($(this).text());
                    $("#txtServings").val($("#txtMultiplier").asNumber() * $("#hfOrigServings").asNumber()).toNumber();
                    $("#btnScale1").click();
                });

                $(".Multiplier input[type=submit]").click(function ()
                {
                    $("#txtServings").val($("#txtMultiplier").asNumber() * $("#hfOrigServings").asNumber()).toNumber();
                });

                $(".Print").click(function ()
                {
                    window.print();
                });
            });

        </script>

        <style type="text/css">            
            .Page
            {
                text-align: center;
            }

            .ParmsBlock, .YieldInfo
            {
                display: inline-block;
                /*vertical-align: top;*/
                margin: 5px 20px;
                padding: 0px 20px;
                border: solid 1px #CCC;
                border-radius: 20px;
            }

            .YieldInfo
            {
                padding: 20px;
            }

            .Parms
            {
                display: inline-block;
                margin: 5px 20px;
            }

            .Parms label
            {
                display: block;
            }

            .Parms .NumServings
            {
                width: 60px;
                text-align: center;
            }

            .Parms button
            {
                width: 40px;
                margin: 2px;
            }

            .Submit
            {
                width: 70px;
            }

            .Submit button, .Submit input[type=submit]
            {
                width: 60px;
                margin-top: 10px;
            }

            .ScaleInfo
            {
                margin: 20px;
            }

            .ScaleInfo table
            {
                display: inline-block;
                vertical-align: top;
                margin: 0px 20px;
                text-align: right;
            }

            .ScaleInfo table td:nth-child(1)
            {
                text-align: right;
            }

            .ScaleInfo #tdPrmSizeUnit
            {
                text-align: left;
            }

            .ScaleInfo .Title
            {
                color: #888;
            }

            #tblIngredients
            {
                margin: 0px auto;
                text-align: left;
            }

            #tblIngredients td:nth-child(1)
            {
                text-align: right;
            }

            #tblIngredients td:nth-child(2)
            {
                padding-right: 10px;
            }

            #tblIngredients .Title
            {
                display: block;
                text-align: left;
                font-weight: bold;
                margin-top: 10px;
            }

            .Directions
            {
                text-align: left;
                margin: 20px auto;
                padding: 0px;
                display: block;
                width: 800px;
                list-style: none;
            }

            .Directions li
            {
                clear: both;
            }

            .Directions label
            {
                width: 65px;
                text-align: right;
                display: block;
                line-height: 1.7em;
                margin-right: 8px;
                color: #888;
                font-size: 8pt;
                float: left;
            }

            .Directions li div
            {
                display: inline-block;
            }

            .Directions li div p:first-child
            {
                margin-top: 0px;
            }

            #tblServings td:nth-child(3), #tblServings #tdServings
            {
                text-align: left;
            }

            @media print
            {
                .Header, .TopMenu, .LeftSideMenu, .FeatureMain, .ParmsBlock, .HorizLine, .User, button.Print
                {
                    display: none;
                }

                .Print
                {
                    display: normal;
                }

                .Page, .Page table td
                {
                    font-size: 14pt;
                }

                .Directions label
                {
                    width: 75px;
                    font-size: 12pt;
                    color: #ccc;
                }

                .ScaleInfo .Title
                {
                    color: #ccc;
                }
            }

        </style>
	</head>
	<body>
		<form id="form" method="post" autocomplete="off" runat="server">
			<%//= Misc.Parms(this) %>
			<% Pg.Top(); %>
			<div class="FeatureMain">Recipe Scaling</div>
                <div class="ParmsBlock">
                    <h2><asp:Label ID="lblPrmRecipeName" runat="server" /></h2>
                
                    <div class="Servings Parms">
                        <asp:HiddenField id="hfOrigServings" runat="server" />
                        <label for="txtServings">Servings</label>
                        <asp:TextBox ID="txtServings" CssClass="NumServings" runat="server" />
                        <asp:Button ID="btnScale" Text="Go" runat="server" />
                        <div class="">
                            <button type="button">25</button>
                            <button type="button">50</button>
                            <button type="button">100</button>
                            <button type="button">150</button>
                            <button type="button">200</button>
                        </div>
                    </div>

                    <div class="Submit Parms">
                        <button type="button" class="Print">Print</button>
                    </div>

                    <div class="Multiplier Parms">
                        <label for="txtMultiplier">Multiplier</label>
                        <asp:TextBox ID="txtMultiplier" CssClass="NumServings" runat="server" />
                        <asp:Button ID="btnScale1" Text="Go" runat="server" />
                        <div>
                            <button type="button">.5</button>
                            <button type="button">1</button>
                            <button type="button">1.5</button>
                            <button type="button">2</button>
                        </div>
                    </div>

                    <div class="ScaleInfo">
                        <asp:Table ID="tblPrmServings" runat="server">
                            <asp:TableRow>
                                <asp:TableCell Text="Yield" CssClass="Title" runat="server" />
                                <asp:TableCell ID="tdPrmYield" runat="server" />
                                <asp:TableCell ID="tdPrmYieldUnit" runat="server" />
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell Text="Serving Size" CssClass="Title" runat="server" />
                                <asp:TableCell ID="tdPrmSize" runat="server" />
                                <asp:TableCell ID="tdPrmSizeUnit" runat="server" />
                            </asp:TableRow>
                        </asp:Table>

                        <asp:Table ID="tblPrmPrice" runat="server">
                            <asp:TableRow>
                                <asp:TableCell runat="server" />
                                <asp:TableCell Text="Serving" CssClass="Title" runat="server" />
                                <asp:TableCell Text="Recipe" CssClass="Title" runat="server" />
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell Text="Cost" CssClass="Title" runat="server" />
                                <asp:TableCell ID="tdPrmServingCost" runat="server" />
                                <asp:TableCell ID="tdPrmRecipeCost" runat="server" />
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell Text="Price" CssClass="Title" runat="server" />
                                <asp:TableCell ID="tdPrmServingPrice" runat="server" />
                                <asp:TableCell ID="tdPrmRecipePrice" runat="server" />
                            </asp:TableRow>
                        </asp:Table>
                    </div>
                </div>

                <h2><asp:Label ID="lblRecipeName" runat="server" /></h2>

                <asp:Table ID="tblIngredients" CssClass="Print" runat="server">
                </asp:Table>

                <ul class="Directions">
                    <li id="liDirections" runat="server">
                        <label>Directions</label>
                        <div><asp:Literal ID="ltlDirections" runat="server" /></div>
                    </li>
                    <li id="liNotes" runat="server">
                        <label>Notes</label>
                        <div><asp:Literal ID="ltlNotes" runat="server" /></div>
                    </li>
                    <li id="liSource" runat="server">
                        <label>Source</label>
                        <div><asp:Literal ID="ltlSource" runat="server" /></div>
                    </li>
                </ul>

                <div class="ScaleInfo YieldInfo">
                    <asp:Table ID="tblServings" runat="server">
                        <asp:TableRow>
                            <asp:TableCell Text="# Servings" CssClass="Title" runat="server" />
                            <asp:TableCell ID="tdServings" ColumnSpan="2" runat="server" />
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell Text="Yield" CssClass="Title" runat="server" />
                            <asp:TableCell ID="tdYield" runat="server" />
                            <asp:TableCell ID="tdYieldUnit" runat="server" />
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell Text="Serving Size" CssClass="Title" runat="server" />
                            <asp:TableCell ID="tdSize" runat="server" />
                            <asp:TableCell ID="tdSizeUnit" runat="server" />
                        </asp:TableRow>
                    </asp:Table>

                    <asp:Table ID="tblPrice" runat="server">
                        <asp:TableRow>
                            <asp:TableCell runat="server" />
                            <asp:TableCell Text="Serving" CssClass="Title" runat="server" />
                            <asp:TableCell Text="Recipe" CssClass="Title" runat="server" />
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell Text="Cost" CssClass="Title" runat="server" />
                            <asp:TableCell ID="tdServingCost" runat="server" />
                            <asp:TableCell ID="tdRecipeCost" runat="server" />
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell Text="Price" CssClass="Title" runat="server" />
                            <asp:TableCell ID="tdServingPrice" runat="server" />
                            <asp:TableCell ID="tdRecipePrice" runat="server" />
                        </asp:TableRow>
                    </asp:Table>
                </div>

                <div style="margin-bottom: 20px;">
                    <button type="button" class="Print">Print</button>
                </div>
			<% Pg.Bottom(); %>
		</form>
	</body>
</html>
