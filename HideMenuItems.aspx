<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HideMenuItems.aspx.cs" Inherits="HideMenuItems" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= Globals.g.Company.Name %> - Hide Menu Items</title>
	<!-- MaintFood.aspx -->
	<!-- Copyright (c) 2007-2019 PeteSoft, LLC. All Rights Reserved -->
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">

	<script language="javascript" src="js/jquery-1.7.1.min.js"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
		<% Pg.Top(); %>

		<div class="FeatureMain">Hide Menu Items</div>
		
		<ul class="HideMenuItems" runat="server">
			<li class="Titles">
				<div>Category</div><div>Menu Item</div><div>Hide</div>
			</li>
		</ul>
		<ul id="ulMenuItems" class="HideMenuItems" runat="server">
		</ul>
				
		<% Pg.Bottom(); %>
    </form>
    
    <script type="text/javascript" language="javascript">
    	$(document).ready(function()
    	{
    		$("ul.HideMenuItems :checkbox").click(function()
    		{
    			var chk = this;
    			$.get("HideMenuItem.aspx", "Rno=" + $(this).data("rno"),
    			function()
    			{
    				$(chk).closest("li").slideUp();
    			});
    			//PageMethods.HideMenuItem($(this).data("rno"), HideMenuItemComplete, HideMenuItemError, this);
    		});
    	});

/*
    	function HideMenuItemComplete(result, chkHide, methodName)
    	{
    		$(chkHide).closest("li").slideUp();
    	}

    	function HideMenuItemError(error, chkHide, methodName)
    	{
    		if (error != null)
    		{
    			alert(error.get_message());
    		}
    	}    	
*/    	
    </script>
</body>
</html>
