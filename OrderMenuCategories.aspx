<%@ page autoeventwireup="true" codefile="OrderMenuCategories.aspx.cs" inherits="OrderMenuCategories"
	language="C#" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>

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
	<link href="Styles.css" type="text/css" rel="stylesheet" />
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">

	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="javascript" src="js/jquery-1.7.1.min.js"></script>
	<script language="javascript" src="js/jquery-ui-1.8.18.custom.min.js"></script>
	<script language="JavaScript" type="text/javascript">

//----------------------------------------------------------------------------		
function Init()
{
<%	if (FocusField != "")
	{ %>
	SetFocus("<%=FocusField%>");
<%	} %>
}

	var NewHtml = "<%= NewHtml %>";
	
	$(document).ready(function()
	{
		// make the categories sortable
		$("ul.Categories").sortable(
		{
			stop: SetSortOrder
		});

/*		
		// hide the category label and show the edit text box
		$("ul.Categories").on("click", "li>label", function()
		{
			$(this).hide();
			$(this).next().show().focus();
		});
		
		// hide the edit text box and show the label
		$("ul.Categories").on("focusout", "li>input[type=text]", function()
		{
			$(this).hide();
			$(this).prev().text($(this).val());
			$(this).prev().show();			
		});

		// add a new category
		$("#btnNew").click(function()
		{
			var NumCats = $("#hfNumCats").val();
			
			var Str = NewHtml.replace(/xxxx/g, NumCats).replace(/New Category/, "");
			$("ul.Categories").append(Str);
			
			NumCats++;
			$("#hfNumCats").val(NumCats);
			
			SetSortOrder();
			
			// make the new one an active text box
			$("ul.Categories li").last().find("label").click();
		});
*/
	});
	
	function SetSortOrder()
	{
		$("ul.Categories input.SortOrder").each(function(Index, Element)
		{
			$(this).val(Index);
		});
	}
	</script>

	<%=Utils.SelectList.JavaScript()%>
</head>
<body onbeforeunload="CheckDirty();" onload="Init();">
	<form id="form" method="post" runat="server" autocomplete="off">
		<asp:HiddenField ID="hfNumCats" runat="server" />
		<% Pg.Top(); %>
			<div class="FeatureMain">Menu Categories</div>
			<div class="CategoryTitles">
				<b>Category</b>
<%--
    			<b>Multi-Select</b>
				<b>Hide</b>
--%>
			</div>
			<ul class="Categories" id="ulCats" runat="server" />

			<div class="Update" style="margin-bottom: 20px; width: auto;">
				<asp:Button ID="btnUpdate" runat="server" Text="Save" />
				<%--<input id="btnNew" type="button" value="New" />--%>
			</div>
		<% Pg.Bottom(); %>
	</form>
</body>
</html>
