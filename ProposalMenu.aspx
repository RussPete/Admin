<%@ Page AutoEventWireup="true" CodeFile="ProposalMenu.aspx.cs" Inherits="ProposalMenu" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Client Menu</title> 
		<!-- Dishes.aspx -->
		<!-- Copyright (c) 2007-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />
    	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet" />
        <style type="text/css">
            .PdfPreview { display: none; }
        </style>

		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	    <script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	    <script language="javascript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="https://use.fontawesome.com/ed45f487e9.js" type="text/javascript"></script>
        <script language="JavaScript" src="RecentJobs.js" type="text/javascript"></script>
		<script language="JavaScript" type="text/javascript">
		
		    var NewTitleHtml = "<%= NewTitleHtml %>";
		    var NewItemHtml = "<%= NewItemHtml %>";

		    $(document).ready(function ()
		    {
		        // make the categories sortable
		        $("ul.MenuItems").sortable(
                {
                    stop: SetSortOrder
                });

		        // uncheck all the delete checkboxes, don't know why they default to checked.
		        //$("ul.MenuItems input.Delete").attr("checked", false);

		        // hide the proposal label and show the edit text box
		        $("ul.MenuItems").on("click", "li>label.Name", function ()
		        {
		            $(this).hide();
		            $(this).next().show().focus();
		        });

		        // hide the edit text box and show the label
		        $("ul.MenuItems").on("focusout", "li>input.Name", function ()
		        {
		            $(this).hide();
		            $(this).prev().text($(this).val());
		            $(this).prev().show();
		        });

		        // add a new group title
		        $("#btnNew").click(function ()
		        {
		            SetDirty();
		            var NumLines = $("#hfNumLines").val();

		            var Str = NewTitleHtml.replace(/xxxx/g, NumLines);
		            $("ul.MenuItems").append(Str);

		            NumLines++;
		            $("#hfNumLines").val(NumLines);

		            SetSortOrder();

		            // make the new one an active text box
		            $("ul.MenuItems li").last().find("label.Name").click();
		            $("ul.MenuItems li:last input.Delete").attr("checked", false);
		        });

		        // add a new menu item (blank line)
		        $("#btnNewItem").click(function () {
		            SetDirty();
		            var NumLines = $("#hfNumLines").val();

		            var Str = NewItemHtml.replace(/xxxx/g, NumLines);
		            $("ul.MenuItems").append(Str);

		            NumLines++;
		            $("#hfNumLines").val(NumLines);

		            SetSortOrder();

		            // make the new one an active text box
		            $("ul.MenuItems li").last().find("label.Name").click();
		            $("ul.MenuItems li:last input.Delete").attr("checked", false);
		        });


		        //// delete a group title
		        //$("ul.MenuItems").on("click", "li>input.Delete, li>label.Delete", function () 
		        //{
		        //    var IsChecked = $(this).parent().find("input.Delete")[0].checked;
		        //    $(this).parent().find("label.Name").css("text-decoration", (IsChecked ? "line-through" : "none"));
		        //});

                $("#btnSavePreview").click(function ()
                {
                    ClearDirty();
                });

		        // show menu items on the proposal
		        $("ul.MenuItems").on("click", "li>img.Show", function ()
                {
                    SetDirty();
		            var li = $(this).parent();
		            li.removeClass("NotShow");
		            li.find("input[id^='hfHide']").val(false);
		        });

		        // hide menu items on the proposal
		        $("ul.MenuItems").on("click", "li>img.NotShow", function ()
		        {
                    SetDirty();
		            var li = $(this).parent();
		            li.addClass("NotShow");
		            li.find("input[id^='hfHide']").val(true);
		        });

                $("#btnPreview").click(function ()
                {
                    var JobRno = $("#hfJobRno").val();
                    $("#frmPreview").attr("src", "ProposalPreview.aspx?Rno=" + JobRno)
                    ShowPreview();
                });

                if ($(".ShowPreview").length > 0)
                {
                    ShowPreview();
                }

		        $("#hfEmail").val(false);
		        $("#btnEmail, #btnEmailConfirm").click(Email);

		        SetupRecentJobs();

		        // dirty bit
		        $(".EditData :input:not([readonly='readonly']):not([disabled='disabled'])").change(function () {
		            SetDirty();
		        });

		        $(window).bind("beforeunload", function () {
		            return CheckDirty();
		        });
		    });

		    function SetSortOrder()
		    {
		        $("ul.MenuItems input.SortOrder").each(function (Index, Element)
		        {
		            $(this).val(Index);
		        });
		    }

		    function Email()
            {
                if (fDirty)
                {
                    alert("The data on this page has changed, you might want to save before emailing the proposal/confirmation, otherwise the changes will not be included.")
                }

		    	$("#EmailDialog").dialog
				({
				    title: "Email <%= EmailType %>",
				    buttons:
					{
						Send: function ()
						{
						    if ($("#txtEmail", $(this)).val().length > 0)
						    {
						        $(this).dialog("close");
						        $(this).css("display", "none");
							    $("form").append($(this).detach());
							    $("#hfEmail").val(true);
							    $("#btnUpdate").click();
						    }
						    else
						    {
						        alert("Email address is missing.");
						    }
						},

						Cancel: function ()
						{
							$(this).dialog("close");
						}
					},
				    width: 620,
				    resizable: false,
				    modal: true
				});
            }

            function ShowPreview()
            {
                // show PDF preview
		        $(".PdfPreview").dialog(
                {
                    modal: true,
                    width: 1000
                });
            }

		</script>
	</head>
	<body>
		<form id="form" method="post" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<asp:panel id="pnlEdit" runat="server">
			    <div style="position: relative;">
                    <%= RecentJobs.Html() %>
                </div>
				<% Pg.JobSubPage("Client Menu"); %>
                <asp:HiddenField ID="hfNumLines" runat="server" />

                <% if (ErrMsg.Length > 0) { %>
                <div class="ErrMsg"><%= ErrMsg %></div>
                <% } %>

                <asp:HiddenField ID="hfJobRno" runat="server" />

                <ul id="ulMenu" class="MenuItems EditData" runat="server"></ul>

                <div class="Update" style="width: 570px;">
				    <input id="btnNew" type="button" value="New Title" style="margin-right: 5px;"/>
				    <input id="btnNewItem" type="button" value="New Item" style="margin-right: 20px;"/>
                    <asp:Button ID="btnUpdate" Text="Save" runat="server" OnClick="btnUpdate_Click" style="margin-right: 5px;"/>
                    <asp:Button ID="btnSavePreview" Text="Save & Preview" runat="server" OnClick="btnSavePreview_Click" style="margin-right: 20px;"/>
                    <input id="btnPreview" type="button" value="Preview" style="display: none;" />
					<input id="btnEmail" type="button" value="Email Proposal" title="Email the proposal to the customer." runat="server" /><asp:HiddenField ID="hfEmail" Value="false" runat="server" />
					<input id="btnEmailConfirm" type="button" value="Email Confirm" title="Email the confirmation to the customer." runat="server" />
                </div>

				<table cellspacing="0" cellpadding="0" align="center" border="0" style="margin-bottom: 10px;">
					<tr>
						<td></td>
						<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
						<td></td>
						<td><img height="1" src="Images/Space.gif" alt="" width="5" /></td>
						<td></td>
						<td><img height="1" src="Images/Space.gif" alt="" width="45" /></td>
						<td></td>
						<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
						<td></td>
						<td><img height="1" src="Images/Space.gif" alt="" width="5" /></td>
						<td></td>
					</tr>
					<tr>
						<td align="right">Created</td>
						<td></td>
						<td><asp:textbox id="txtCreatedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtCreatedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
						<td></td>
						<td align="right">Updated</td>
						<td></td>
						<td><asp:textbox id="txtUpdatedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtUpdatedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
					</tr>
					<tr>
						<td align="right">Generated</td>
						<td></td>
						<td><asp:textbox id="txtGeneratedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtGeneratedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
						<td></td>
						<td align="right">Prop Emailed</td>
						<td></td>
						<td><asp:textbox id="txtPropEmailedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtPropEmailedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
					</tr>
					<tr>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td></td>
						<td align="right">Conf Emailed</td>
						<td></td>
						<td><asp:textbox id="txtConfirmEmailedDt" runat="server" cssclass="MaintDate" enabled="False"></asp:textbox></td>
						<td></td>
						<td><asp:textbox id="txtConfirmEmailedUser" runat="server" cssclass="MaintUser" enabled="False"></asp:textbox></td>
					</tr>
				</table>

			</asp:panel>
			<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
			<% Pg.Bottom(); %>

            <asp:Panel ID="pnlPreview" CssClass="PdfPreview" ToolTip="Proposal/Confirmation Preview" runat="server">
                <iframe id="frmPreview" width="970" height="650" frameborder="0" runat="server" />    
            </asp:Panel>

			<div id="EmailDialog" class="Hide">
				<dl>
					<dt>Email To</dt>
					<dd><asp:TextBox ID="txtEmail" runat="server" /></dd>
					<dt>Subject</dt>
					<dd><asp:TextBox ID="txtSubject" runat="server" /></dd>
					<dt>Message</dt>
					<dd><asp:TextBox ID="txtMessage" TextMode="MultiLine" runat="server" /></dd>
                    <dt>Attach</dt>
                    <dd><div style="margin: 2px 0px 6px 0px;"><asp:CheckBox ID="chkTermsOfService" Text="Terms of Service" runat="server" /></div></dd>
					<dt>Attachment</dt>
					<dd><asp:FileUpload ID="fuAttachment" runat="server" /></dd>
				</dl>
			</div>

		</form>
	</body>
</html>
