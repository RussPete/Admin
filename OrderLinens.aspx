<%@ page autoeventwireup="true" codefile="OrderLinens.aspx.cs" inherits="OrderLinens"
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
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet" />

	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script language="javascript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="OrderItems.js" type="text/javascript"></script>
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

	</script>
</head>
<body onload="Init();">
	<form id="form" method="post" runat="server" autocomplete="off">
		<asp:HiddenField ID="hfNumItems" runat="server" />
		<% Pg.Top(); %>
			<div class="FeatureMain">Linens</div>
            <div>
                <div style="text-align: center;">
        			<ul class="Items" id="ulItems" runat="server" />
			    </div>

			    <div class="Update" style="margin-bottom: 20px; width: auto;">
				    <asp:Button ID="btnUpdate" runat="server" Text="Save" />
                    &nbsp;
                    <input id="btnNew" type="button" value="New" />
			    </div>
			</div>
		<% Pg.Bottom(); %>
	</form>
</body>
</html>
