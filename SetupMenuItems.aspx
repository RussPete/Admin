<%@ page autoeventwireup="true" codefile="SetupMenuItems.aspx.cs" inherits="SetupMenuItems"
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
	<link href="Styles.css" type="text/css" rel="stylesheet" />
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet" />
   	<link href="css/font-awesome.min.css" type="text/css" rel="stylesheet">
	<style type="text/css">
	    .MenuItem { width: 300px; }
        .Price { width: 60px; text-align: right; }
        .Disabled { background-color: #DDD; }
        .Over { color: black; font-size: smaller; }
        .Under { color: firebrick; font-size: smaller; }
	    .qtip ul, #SimilarDialog ul { list-style-type: none; padding: 0px; margin: 0px 0px 0px 10px; }
        #SimilarDialog a:hover { text-decoration: none; color: #667B3E; }
        .icon-dollar { color: firebrick; }
	</style>

	<script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery.formatCurrency-1.4.0.min.js" type="text/javascript"></script>    
	<script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
   	<script language="JavaScript" src="js/jquery.validate.js" type="text/javascript"></script>
	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="JavaScript" type="text/javascript">	

	    var fSimilarMenuItemsWithRecipe = <%= fSimilarMenuItemsWithRecipe.ToString().ToLower() %>;
     
//----------------------------------------------------------------------------		
function Init()
{
<%	if (FocusField != "")
	{ %>
	SetFocus("<%=FocusField%>");
<%	} %>
}

$(document).ready(function()
{
    SearchList("#txtSearch", "#lstList");

    Qtips("#lblSimilar, .icon-dollar");

    $("#lblSimilar").click(function ()
    {
        $("#SimilarDialog").dialog(
        {
            modal: true
        });
    });

    $("#txtServingQuote, #txtServingPrice").focusout(function ()
    {
        $(this).toNumber().formatCurrency();
        CheckPricePct();
    });
    CheckPricePct();

    $("#chkAsIs").click(function()
    {
        if (this.checked && fSimilarMenuItemsWithRecipe)
        {
            alert("There are similar menu items in other categories that can't be marked 'As Is' because they have a recipe.");
        }
    });

    ShowAssocItems();

    <% if (fMerging) { %>
    $("#MergeMenuItems").dialog(
    {
        buttons:
        {
            "Yes": function ()
            {
                $("#hfMerge").val(true);
                $(this).dialog("close");
                $("#btnUpdate").click();
            },
            "No": function ()
            {
                $(this).dialog("close");
            }
        },
        modal: true,
        width: 650
    });
    <% } %>

	
    $("#btnEdit").click(ShowItemsPopup);
    $("#btnEdit").prop("disabled", !$("#chkMultiSelect").prop("checked"));
    $("#chkMultiSelect").change(function() { $("#btnEdit").prop("disabled", !this.checked); });

	$("#btnDelete").click(function ()
	{
	    return confirm("Just checking.\n\nAre you sure you want to delete this menu item?");
	});

    <% if (fError) { %>
    $("#ErrMsg").dialog(
    {
        buttons:
        {
            "OK": function ()
            {
                $(this).dialog("close");
            }
        },
        modal: true
    });
    <% } %>

    $("#form").validate({
        rules: {
            ddlCategory: "required",
            txtMenuItem: "required",
        }
    });
});

function ShowAssocItems()
{
	var aItemRno = SelectedItemRnos();
	var iItemRno;
	var aItem = new Array();
	var iItem = -1;

	for (iItemRno = 0; iItemRno < aItemRno.length; iItemRno++)
	{
		if (aItemRno[iItemRno] != "")
		{
			aItem[++iItem] = $("#lstList option[value='" + aItemRno[iItemRno] + "']").text();
		}
	}
	
	// sort selected items
	aItem.sort(function(a, b) 
	{ 
		var a = a.toLowerCase();
		var b = b.toLowerCase();
		return ((a < b ) ? -1 : ((a > b) ? 1 : 0)); 
	});
	
	var AssocItems = $("#ulAssocItems").html("");
	for (iItem = 0; iItem < aItem.length; iItem++)
	{
		AssocItems.append("<li>" + aItem[iItem] + "</li>");
	}
}
	
function SelectedItemRnos()
{
	var Items = $("#txtMultItems").val();
	if (!Items) { Items = ""; }

	var aItemRno = Items.split(",");
	
	return aItemRno;
}

function ShowItemsPopup()
{
	var aItemRno = SelectedItemRnos();
	var iItemRno;
	
	var Popup = $("#MultiSelectMenuItems");
	var MultiItems = Popup.find(".Items").html("");

	// insert the menu items to select
	var Items = $("#lstList option");
	Items.each(function(iItem, Item)
	{
		var ItemRno = $(Item).val();
		
		var fSelected = false;
		for (iItemRno = 0; iItemRno < aItemRno.length; iItemRno++)
		{
			if (ItemRno == aItemRno[iItemRno])
			{
				fSelected = true;
				break;
			}
		}
		
		MultiItems.append("<li><input type='checkbox' id='MenuItem" + iItem + "'" + (fSelected ? "checked='checked'" : "") + " value='" + ItemRno + "' /> <label for='MenuItem" + iItem + "'>" + $(Item).text() + "</label></li>\n");
	});
	
	// show the popup
	Popup.dialog({
		buttons:{
			"Save": function()
			{			
			    var SelectedItemRnos = "";
			    					
				$("#MultiSelectMenuItems input:checked").each(function()
				{
					SelectedItemRnos += $(this).val() + ",";
				});
				
				$("#txtMultItems").val(SelectedItemRnos.substr(0, SelectedItemRnos.length - 1));
				ShowAssocItems();
				
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

function CheckPricePct()
{
    var Quote = $("#txtServingQuote").asNumber();
    var Price = $("#txtServingPrice").asNumber();
    var Pct = (Price != 0 ? (Quote - Price) / Price * 100 : 0);
    $("#PctOverUnder").html(FmtPct(Math.abs(Pct), 0, "0") + (Pct >= 0 ? " over" : " under"));
    $("#PctOverUnder").removeClass("Over Under").addClass(Pct >= 0 ? "Over" : "Under");
}

function MyPostBack(id, parameter)
{
    __doPostBack(id, parameter)
}

	</script>

	<%/*=Utils.SelectList.JavaScript()*/%>
</head>
<body onload="Init();">
	<form id="form" method="post" runat="server" autocomplete="off">
		<% Pg.Top(); %>
		<table cellspacing="0" cellpadding="0" align="center" border="0">
			<tr>
				<td valign="top">
					<table cellspacing="0" cellpadding="0" align="center" border="0">
						<tr>
							<td align="left">
								<table class="SelectFilter" cellspacing="0" cellpadding="0" border="0" width="100%">
									<tr>
										<td>
											Sort By:</td>
									</tr>
									<tr>
										<td>
											<asp:radiobutton id="rbSortCategory" runat="server" groupname="rbSortBy" autopostback="True"
												text="Category" oncheckedchanged="UpdateList"></asp:radiobutton></td>
										<td align="right">
											<asp:checkbox id="chkShowNew" text="Only Show New" textalign="left" oncheckedchanged="UpdateList"
												autopostback="true" runat="server" /></td>
									</tr>
									<tr>
										<td style="height: 20px">
											<asp:radiobutton id="rbSortMenuItem" runat="server" groupname="rbSortBy" autopostback="True"
												text="Menu Item" oncheckedchanged="UpdateList"></asp:radiobutton></td>
										<td align="right">
											<asp:checkbox id="chkShowHidden" text="Show Hidden" textalign="left" oncheckedchanged="UpdateList"
												autopostback="true" runat="server" /></td>
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
							<td>
								<asp:listbox id="lstList" runat="server" autopostback="false" cssclass="SelectJob"
									onselectedindexchanged="lstList_SelectedIndexChanged" onclick="MyPostBack('lstList', '')"></asp:listbox></td>
						</tr>
						<tr>
							<td>
								<asp:label id="lblRecCount" runat="server" cssclass="SelectCount">RecCount</asp:label></td>
						</tr>
					</table>
				</td>
				<td>
					<img height="1" src="Images/Space.gif" alt="" width="40" /></td>
				<td valign="top" align="center">
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td>
							</td>
							<td width="10">
							</td>
							<td width="300">
							</td>
						</tr>
						<tr>
							<td colspan="3" align="center" class="FeatureMain">
								Menu Items</td>
							<td>
                                <asp:HyperLink NavigateUrl="~/MenuItemsRpt.aspx" runat="server">Report</asp:HyperLink> |
                                <asp:HyperLink NavigateUrl="~/HideMenuItems.aspx" runat="server">Hide Menu Items</asp:HyperLink>
							</td>
						</tr>
						<tr>
							<td align="right">
								Category</td>
							<td>
							</td>
							<td align="left">
								<asp:DropDownList ID="ddlCategory" CssClass="MenuItem" runat="server" />
							</td>
						</tr>
						<tr>
							<td align="right">
								Menu Item</td>
							<td>
							</td>
							<td align="left">
								<asp:TextBox ID="txtMenuItem" CssClass="MenuItem" runat="server" />
                            </td>
                            <td align="right">
                                <asp:Label ID="lblSimilar" Text="n similar menu items" runat="server" CssClass="TextLink" Visible="false" />
							</td>
						</tr>
						<tr>
							<td align="right">
								Proposal</td>
							<td>
							</td>
							<td align="left">
								<asp:TextBox ID="txtProposal" CssClass="MenuItem" runat="server" />
							</td>
						</tr>
						<tr>
							<td align="right">
								per Serving Quote</td>
							<td>
							</td>
							<td align="left">
								<asp:TextBox ID="txtServingQuote" CssClass="Price" runat="server" />
                                <span id="PctOverUnder"></span>
							</td>
						</tr>
						<tr>
							<td align="right">
								per Serving Price</td>
							<td>
							</td>
							<td align="left">
								<asp:TextBox ID="txtServingPrice" CssClass="Price" Enabled="false" runat="server" />
                                <asp:Label ID="lblInaccuratePrice" runat="server" />
							</td>
						</tr>
						<%--<tr><td height="10"></td></tr>--%>
						<tr>
							<td align="right">
                                Prep Location
							</td>
							<td>
							</td>
							<td align="left">
								<asp:DropDownList ID="ddlLocation" runat="server" /></td>
						</tr>
						<%--<tr><td height="10"></td></tr>--%>
						<tr>
							<td align="right">
								Recipe</td>
							<td>
							</td>
							<td align="left" style="line-height: 2.3em;">
                                <asp:HyperLink ID="lnkRecipe" runat="server" />
                                <asp:Literal ID="ltlRecipe" runat="server" />
							</td>
						</tr>
						<tr>
							<td align="right">
                                Misc
							</td>
							<td>
							</td>
							<td align="left">
								<asp:checkbox id="chkAsIs" text=" As Is" runat="server" /></td>
						</tr>
						<tr>
							<td>
							</td>
							<td>
							</td>
							<td align="left">
								<asp:checkbox id="chkMultiSelect" text=" Multi-Select" runat="server" /></td>
						</tr>
						<tr>
							<td>
							</td>
							<td>
							</td>
							<td align="left">
								<asp:checkbox id="chkIngredSelect" text=" Ingredient Selection" runat="server" /></td>
						</tr>
						<tr>
							<td>
							</td>
							<td>
							</td>
							<td align="left">
								<asp:checkbox id="chkHide" text=" Hide" runat="server" /></td>
						</tr>
						<tr><td height="10"></td></tr>
						<tr>
							<td>
							</td>
							<td>
							</td>
							<td align="left">
								<asp:button id="btnUpdate" runat="server" text="Save" onclick="btnUpdate_Click" />
								<asp:button id="btnNext" runat="server" text="Save & Next" onclick="btnUpdateNext_Click" />
								<asp:button id="btnNew" runat="server" text="New" onclick="btnNew_Click" />
								<asp:button id="btnDelete" runat="server" text="Delete" onclick="btnDelete_Click" />
							</td>
						</tr>
						<tr><td height="10"></td></tr>
						<tr>
							<td style="vertical-align: top; text-align: right;">
								Multi-Select Items<br />
								<input type="button" id="btnEdit" value="Edit" />
							</td>
							<td></td>
							<td>
								<ul id="ulAssocItems">
								
								</ul>
							</td>
						</tr>
					</table>

                    <table id="Dates">
						<tr>
							<td align="right">Created</td>
							<td></td>
							<td><asp:textbox id="txtCreatedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
							<td></td>
							<td><asp:textbox id="txtCreatedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
                        </tr>
                        <tr>
							<td align="right">Updated</td>
							<td></td>
							<td><asp:textbox id="txtUpdatedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
							<td></td>
							<td><asp:textbox id="txtUpdatedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
						</tr>
                    </table>
				</td>
			</tr>
		</table>
		<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
		<input type="hidden" id="txtRno" name="txtRno" runat="server" />
		<input type="hidden" id="txtCurrCategory" name="txtCurrCategory" runat="server" />
		<input type="hidden" id="txtCurrMenuItem" name="txtCurrMenuItem" runat="server" />
		<input type="hidden" id="txtMultItems" name="txtMultItems" runat="server" />
        <asp:HiddenField ID="hfMerge" Value="false" runat="server" />
		<% Pg.Bottom(); %>
	</form>
	
    <div id="SimilarDialog" title="Categories with Same Menu Item">
        <ul id="ulSimilar" runat="server" />
    </div>

	<div id="MultiSelectMenuItems" title="Select One or More Menu Items">
		<ul class="Items"></ul>
	</div>
    <div id="MergeMenuItems" title="Merge Two Menu Items">
        Merge the <b><%= MergeMenuItem %></b> menu item in the <b><%= MergeCategory %></b> category<br />into the <b><%= MergeIntoMenuItem%></b> menu item in the <b><%= MergeIntoCategory %></b> category?<br />
        This will affect <b><%= AffectedJobs %></b> catering jobs.
    </div>
    <div id="ErrMsg" title="Error Message">
        <%= ErrorMsg %>
    </div>
</body>
</html>
