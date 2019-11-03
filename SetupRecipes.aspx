<%@ page autoeventwireup="true" codefile="SetupRecipes.aspx.cs" inherits="SetupRecipes"
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
        }

        label
        {
            width: 190px;
            text-align: right;
            margin-right: 10px;
            display: inline-block;
        }

    </style>
</head>
<body>
	<form id="form" method="post" runat="server" autocomplete="off">
		<% Pg.Top(); %>
        <div id="PageContent">
            <h1 class="FeatureMain">Recipes Setup</h1>

            <ul>
                <li><label>Recipe Base Cost Pct</label><asp:TextBox ID="txtBaseCostPct" CssClass="JobPct" runat="server" />%</li>
                <li><label>As Is Menu Item Base Cost Pct</label><asp:TextBox ID="txtAsIsBaseCostPct" CssClass="JobPct" runat="server" />%</li>
            </ul>

    		<asp:Button ID="btnUpdate" runat="server" Text="Save" OnClick="btnUpdate_Click" />
        </div>
        <asp:HiddenField ID="hfRno" runat="server" />
		<% Pg.Bottom(); %>
	</form>
</body>
</html>
