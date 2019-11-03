<%@ page autoeventwireup="true" codefile="RecipeBook.aspx.cs" inherits="RecipeBook"
	language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Maintance Food</title>
	<!-- MaintFood.aspx -->
	<!-- Copyright (c) 2007-2019 PeteSoft, LLC. All Rights Reserved -->
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
	<style type="text/css">
	.MenuItem { width: 300px; }
	</style>

	<script language="javascript" src="js/jquery-1.7.1.min.js"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
   	<script language="JavaScript" src="js/jquery.validate.js" type="text/javascript"></script>
	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="JavaScript" type="text/javascript">	

        //----------------------------------------------------------------------------		
        $(document).ready(function()
        {
            $("#PageContent").on("change", "input", SetDirty);
            $("#btnUpdate").click(SkipDirty);
            window.onbeforeunload = CheckDirty;
        });

	</script>
    <style>
        ul
        {
            list-style-type: none;
            padding-left: 0px;
            text-align: left;
            margin: 0px auto;
            width: 370px;
        }

        ul li
        {
            margin: 10px 0px;
        }

        label
        {
            width: 100px;
            text-align: right;
            margin-right: 10px;
            display: inline-block;
        }

        li.Buttons
        {
            margin-top: 20px;
        }

        table#tbl td, th 
        {
            text-align: left;
        }

        table#tbl tr:hover 
        {
            background-color: #D0DCB8;
            cursor: default;
        }
    </style>
</head>
<body>
	<form id="form" method="post" runat="server" autocomplete="off">
		<% if (!fGenerating) { Pg.Top(); } %>
        <div id="PageContent">
            <h1 class="FeatureMain">Recipe Book</h1>

            <ul>
                <li>
                    <label for="ddlLocation">Kitchen Location</label>
                    <asp:DropDownList ID="ddlLocation" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="All" />
                        <asp:ListItem Text="Hot Kitchen" />
                        <asp:ListItem Text="Bakery" />
                        <asp:ListItem Text="Packing Area" />
                    </asp:DropDownList>
                </li>
                <li>
                    <label for="ddlInclude">Recipes to Print</label>
                    <asp:DropDownList ID="ddlInclude" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="Regular Recipes" Value="R" />
                        <asp:ListItem Text="All Recipes" Value="A" />
                    </asp:DropDownList>
                </li>
                <li class="Buttons">
                    <label></label>
                    <asp:Button ID="btnCreate" runat="server" Text="Create Recipe Book" OnClick="btnCreate_Click" />
                </li>
            </ul>

            <table id="tbl" style="margin: 30px auto;" runat="server">
                <tbody>
                    <tr>
                        <th>Category</th>
                        <th>Recipe</th>
                    </tr>
                </tbody>
            </table>
        </div>
		<% if (!fGenerating) { Pg.Bottom(); } %>
	</form>
</body>
</html>
