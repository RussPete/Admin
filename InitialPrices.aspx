<%@ Page AutoEventWireup="true" CodeFile="InitialPrices.aspx.cs" Inherits="InitialPrices" Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Ingredients</title>
	<!-- InitialPrices.aspx -->
	<!-- Copyright (c) 2013-2019 PeteSoft, LLC. All Rights Reserved -->
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">

	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>

	<script language="JavaScript" type="text/javascript">

	    //----------------------------------------------------------------------------
	    $(document).ready(function()
	    {
	    });
	</script>

</head>
<body>
	<form id="form" method="post" runat="server" autocomplete="off">
	<% Pg.Top(); %>
        <div class="FeatureMain">Initial Prices</div>
        <div style="text-align: center; margin: 20px;"><asp:button id="btnInitialize" runat="server" text="Initialize" OnClick="btnInitialize_Click" /></div>
	<% Pg.Bottom(); %>
	</form>
</body>
</html>
