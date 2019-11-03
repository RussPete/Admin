<%@ Page AutoEventWireup="true" CodeFile="Units.aspx.cs" Inherits="Units" Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Ingredients</title>
	<!-- Ingredients.aspx -->
	<!-- Copyright (c) 2013-2019 PeteSoft, LLC. All Rights Reverved -->
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<link href="Styles.css" type="text/css" rel="stylesheet">
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet">
	<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
	<script language="JavaScript" src="js/jquery.validate.js" type="text/javascript"></script>

	<script language="JavaScript" type="text/javascript">

	    function MyPostBack(id, parameter)
	    {
	        __doPostBack(id, parameter)
	    }

	    //----------------------------------------------------------------------------
	    $(document).ready(function()
	    {
	        SearchList("#txtSearch", "#lstList");

	        $("#txtSingle").focus();
	        $("#txtPlural").focus(function ()
	        {
	            if ($(this).val().length == 0)
	            {
	                $(this).val($("#txtSingle").val());
	            }
	        });

	        // insert a new row for an ingredient
	        $("#tblIngredients").on("blur", "input.Ingredient:last", AddBlankRow);

	        AutoCompleteUnit("#tblIngredients input.Unit");
	        AutoCompleteIngredient("#tblIngredients input.Ingredient");

	        $("#form").validate({
	            rules: {
	                txtSingle: "required",
	                txtPlural: "required",
	            }
	        });

	        $("#content").on("change", "input, textarea", SetDirty);

	        $("#btnSave, #btnUpdate, #btnNext").click(SkipDirty);
	        $("#btnDelete").click(function ()
	        {
	            var rc = false;

	            if (confirm("Are you sure you want to delete this unit value?"))
	            {
	                SkipDirty();
	                rc = true;
	            }

	            return rc;
	        });

	        $(".Pointer").click(function ()
            {
                $(".WhereUsed").slideToggle();
            });

	        window.onbeforeunload = CheckDirty;
	    });

	    //----------------------------------------------------------------------------
	    function AutoCompleteUnit(Selector)
	    {
	        $(Selector).autocomplete(
            {
                source: function(req, response) { 
                    var re = $.ui.autocomplete.escapeRegex(req.term); 
                    var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                    response($.grep(Units, function(Item, ItemIndex) {                         
                        return Matcher.test(Item.label); 
                    }) ); 
                },
                select: function(event, ui)
                {
                    
                    $(this).val($(this).closest("tr").find(".Qty").val() == "1" ? ui.item.label : ui.item.plural);
                    $(this).closest("tr").find(".UnitRno").val(ui.item.value);
                    return false;
                },
                change: function(event, ui)
                {
                    if (ui.item == null)
                    {
                        $(this).val("");  
                    }
                },
                autoFocus: true,
                delay: 0,
                minLength: 0
            });
	    }

	    //----------------------------------------------------------------------------
	    function AutoCompleteIngredient(Selector)
        {
	        $(Selector).autocomplete(
            {
                source: function(req, response) { 
                    var re = $.ui.autocomplete.escapeRegex(req.term); 
                    var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                    response($.grep(Ingredients, function(Item, ItemIndex) { 
                        return Matcher.test(Item.label); 
                    }) ); 
                },
                select: function(event, ui)
                {
                    $(this).val(ui.item.label);
                    $(this).closest("tr").find(".IngredRno").val(ui.item.value);
                    return false;
                },
                change: function(event, ui)
                {
                    if (ui.item == null)
                    {
                        $(this).val("");  
                    }
                },
                autoFocus: true,
                delay: 0,
                minLength: 0
            });
        }

	    //----------------------------------------------------------------------------
	    function AddBlankRow()
	    {
	        if ($("#tblIngredients input.Ingredient:last").val().length > 0)
	        {
	            var NumIngredients = $("#tblIngredients tr").length;
	            $("#tblIngredients tbody").append(AddRowHtml.replace(/~ID~/g, NumIngredients));
	            //$("#tblIngredients input[type='checkbox']:last").attr("tabindex", -1);
	            $("#hfNumIngredients").val(NumIngredients);

	            AutoCompleteUnit("#tblIngredients input.Unit:last");
	            AutoCompleteIngredient("#tblIngredients input.Ingredient:last");
            }
	    }


	</script>

	<style>
		.FeatureSub
		{
			padding-bottom: 5px;
		}

	    #content
	    {
            min-width: 400px;
            margin-left: 40px;
	    }

	    .FeatureMain
	    {
	        margin: 0px auto;
	    }

	    ul.Fields
	    {
            list-style: none;
            padding: 0px;
	    }

	    ul.Fields label
	    {
	        margin-right: 7px;
            margin-left: 5px;
	    }

	    .Prompt
	    {
            display: inline-block;
            width: 85px;
            text-align: right;
	    }

	    .Name
	    {
            width: 250px;
	    }

	    .Buttons
	    {
            margin-top: 15px;
            text-align: center;
	    }

	    h3, h4
	    {
            text-align: center;
            color: #667B3E;
            margin-bottom: 0px;
	    }

        .WhereUsed
        {
            display: none;
        }

	    .WhereUsed h4
	    {
            text-align: left;
	    }

	    .Pointer
	    {
            cursor: pointer;
	    }

        #ulRecipes, #ulVendorIngred, #ulIngredConv
        {
            list-style: none;
            margin-top: 5px;
            padding-left: 25px;
        }

	    #tblConv
	    {
            margin: 0px auto;
	    }

	    #tblConv td.Qty
	    {
            text-align: right;
	    }

	    #tblConv td:nth-child(3)
	    {
            padding-left: 20px;
	    }

	    #tblConv td.Unit
	    {
            color: #667B3E;
	    }

	    .Buttons
	    {
            margin-top: 15px;
            text-align: center;
	    }

	</style>
</head>
<body onbeforeunload="CheckDirty();">
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
									<td align="right">
										<asp:CheckBox ID="chkShowHidden" Text ="Show Hidden" TextAlign="Left" runat="server"  AutoPostBack="true" oncheckedchanged="UpdateList" />
									</td>
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
							<asp:ListBox ID="lstList" runat="server" AutoPostBack="true" CssClass="SelectJob"
								OnSelectedIndexChanged="lstList_SelectedIndexChanged" onclick="MyPostBack('lstList', '')"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td>
							<asp:Label ID="lblRecCount" runat="server" CssClass="SelectCount">RecCount</asp:Label>
						</td>
					</tr>
				</table>
			</td>
            <td style="vertical-align: top;">
                <div id="content">
                    <div class="FeatureMain">Units</div>
                    <ul class="Fields">
                        <li><label class="Prompt">Single Unit</label><asp:TextBox ID="txtSingle" CssClass="Name" MaxLength="20" runat="server" /></li>
                        <li>
                            <label class="Prompt">Plural Units</label><asp:TextBox ID="txtPlural" CssClass="Name" MaxLength="20" runat="server" />
                            <asp:CheckBox ID="chkHide" Text="Hide" runat="server" />
                        </li>
                    </ul>

                    <h3>Standard Conversions</h3>
                    <asp:Table id="tblConv" runat="server" />                     

                    <h3 class="Pointer">Where Used</h3>
                    <div class="WhereUsed">
                        <h4>Recipes</h4>
                        <ul id="ulRecipes" runat="server"></ul>
<%--                        <h4>Vendor Ingredients</h4>
                        <ul id="ulVendorIngred" runat="server"></ul>--%>
                        <h4>Ingredient Conversions</h4>
                        <ul id="ulIngredConv" runat="server"></ul>
                    </div>

                    <div class="Buttons">
						<asp:Button ID="btnUpdate" runat="server" Text="Save" OnClick="btnUpdate_Click" />
						<asp:button id="btnNext" runat="server" text="Save & Next" onclick="btnUpdateNext_Click" />
                        <asp:Button ID="btnNew" runat="server" Text="New" OnClick="btnNew_Click" />
						<asp:button id="btnDelete" runat="server" text="Delete" onclick="btnDelete_Click" />
                    </div>

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
                </div>
            </td>
		</tr>
	</table>
	<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
	<input type="hidden" id="hfRno" name="hfRno" runat="server" />
	<% Pg.Bottom(); %>
	</form>
</body>
</html>
