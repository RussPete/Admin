<%@ page autoeventwireup="true" codefile="SetupMenuCategories.aspx.cs" inherits="SetupMenuCategories"
	language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Setup Menu Categories</title>
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

    $("#EditMenuItemsSortOrder").click(ShowItemsPopup);

	$("#MenuItemsSortOrder ul.Items").sortable(
	{
		stop: SetSortOrder
	});

	$("#MenuItemsSortOrder").on("submit", "form", function ()
	{
		$.ajax(
		{
			url: $(this).attr("action"),
			type: $(this).attr("method"),
			datatype: "json",
			data: $(this).serialize()
		});
		return false;
	});

    <% if (fMerging) { %>
    $("#MergeCategories").dialog(
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
        width: 450
    });
    <% } %>

    $("#btnDelete").click(function ()
    {
        return confirm("Just checking.\n\nAre you sure you want to delete this category?");
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
            txtCategory: "required",
        }
    });
});
	
function ShowItemsPopup()
{
	// show the popup
	$("#MenuItemsSortOrder").dialog(
	{
		buttons:
		{
			"Save": function ()
			{
				$("#hfMenuItemsSaved").val(true);
				$(this).dialog("close");
				$("form", $(this)).submit();
			}
		},
		modal: true,
		width: 450,
		open: function (event, ui)
		{
			$(this).css({ "max-height": 600, "overflow-y": "auto" });
		}
	});
}

function SetSortOrder()
{
	$("ul.Items input.SortOrder").each(function (Index, Element)
	{
		$(this).val(Index);
	});
}

function MyPostBack(id, parameter)
{
    __doPostBack(id, parameter)
}
	</script>
</head>
<body onbeforeunload="CheckDirty();" onload="Init();">
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
											<asp:radiobutton id="rbSortMenuOrder" runat="server" groupname="rbSortBy" autopostback="True"
												text="Menu Order" oncheckedchanged="UpdateList"></asp:radiobutton></td>
									</tr>
									<tr>
										<td style="height: 20px">
											<asp:radiobutton id="rbSortAlpha" runat="server" groupname="rbSortBy" autopostback="True"
												text="Alpha" oncheckedchanged="UpdateList"></asp:radiobutton></td>
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
					<img height="1" src="Images/Space.gif" alt="" width="40"></td>
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
								Menu Categories</td>
							<td>
                                <asp:HyperLink NavigateUrl="~/OrderMenuCategories.aspx" runat="server">Categories Sort Order</asp:HyperLink>
							</td>
						</tr>
						<tr>
							<td align="right">
								Category</td>
							<td>
							</td>
							<td align="left">
								<asp:TextBox ID="txtCategory" CssClass="MenuItem" runat="server" />
							</td>
						</tr>
						<tr><td height="10"></td></tr>
						<tr>
							<td align="right">
								Menu Items</td>
							<td>
							</td>
							<td align="left">
								<span class="TextLink" id="EditMenuItemsSortOrder">Edit Sort Order</span>
							</td>
						</tr>
						<tr><td height="10"></td></tr>
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
		<input type="hidden" id="txtCurrCategory" name="txtCurrCategory" runat="server" />
        <asp:HiddenField ID="hfMerge" Value="false" runat="server" />
		<% Pg.Bottom(); %>
	</form>
	
	<div id="MenuItemsSortOrder" title="Menu Items Sort Order">
		<form method="post" action="OrderMenuItems.aspx">
			<input type="hidden" id="hfNumMenuItems" name="hfNumMenuItems" value="<%= NumMenuItems %>" />
			<input type="hidden" id="hfMenuItemsSaved" name="hfMenuItemsSaved" value="false" />
			<ul class="Items" id="ulItems" runat="server"></ul>
		</form>
	</div>
    <div id="MergeCategories" title="Merge Two Categories">
        Merge the <b><%= MergeCategory %></b><br />category into <b><%= MergeIntoCategory %></b>?
    </div>
    <div id="ErrMsg" title="Error Message">
        <%= ErrorMsg %>
    </div>
</body>
</html>
