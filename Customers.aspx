<%@ Page AutoEventWireup="true" CodeFile="Customers.aspx.cs" Inherits="Customers" Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Customers</title>
	<!-- Customers.aspx -->
	<!-- Copyright (c) 2019 PeteSoft, LLC. All Rights Reserved -->
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

	        $("#txtName").focus();


            $("#form").validate({
	            rules: {
	                txtName: "required",
	            }
	        });

	        $("#txtAnnualContractSignedDate").datepicker({
		        showOn: "both",
		        buttonImage: "Images/CalendarIcon.gif",
		        buttonImageOnly: true
            });	

	        $("#content").on("change", "input", SetDirty);

	        $("#btnSave, #btnUpdate, #btnNext").click(SkipDirty);
	        $("#btnDelete").click(function ()
	        {
	            var rc = false;

	            if (confirm("Are you sure you want to delete this customer?"))
	            {
	                SkipDirty();
	                rc = true;
	            }

	            return rc;
	        });

	        window.onbeforeunload = CheckDirty;

	    });

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

        ul.Fields li
        {
            margin-bottom: 5px;
        }

	    ul.Fields label
	    {
	        margin-right: 7px;
            margin-left: 5px;
	    }

	    .Prompt
	    {
            display: inline-block;
            width: 160px;
            text-align: right;
	    }

	    .Name
	    {
            width: 250px;
	    }

        .SignedDate
        {
	        width: 80px;
	        text-align: center;
        }

	    .Buttons
	    {
            margin-top: 15px;
            text-align: center;
	    }

	    #Dates
	    {
            margin: 15px auto;
	    }

        #pnlLists
        {
            width: 500px;
            margin: 30px auto 0px auto;
        }

        #pnlLists .FeatureSub
        {
            text-align: center;
        }

        #pnlLists ul
        {
            margin-top: 5px;
            list-style: none;
        }

        #pnlLists ul.Contacts
        {
            width: 200px;
            margin-left: auto;
            margin-right: auto;
            margin-bottom: 30px;
        }

        #pnlLists .Date
        {
            display: inline-block;
            vertical-align: top;
        }

        #pnlLists a 
        {
            width: 350px;
            display: inline-block;
            margin-left: 10px;
        }

        #pnlLists .Contacts a
        {
            width: 160px;
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
							<asp:ListBox ID="lstList" runat="server" CssClass="SelectJob"
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
                    <div class="FeatureMain">Customers</div>
                    <ul class="Fields">
                        <li>
                            <label class="Prompt">Name</label><asp:TextBox ID="txtName" CssClass="Name" MaxLength="50" runat="server" />
                            <asp:CheckBox ID="chkHide" Text="Hide" runat="server" />
                        </li>
                        <li>&nbsp;</li>
                        <li>
                            <label class="Prompt" for="chkCompany">Company</label><asp:CheckBox ID="chkCompany" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt" for="chkTaxExempt">Tax Exempt</label><asp:CheckBox ID="chkTaxExempt" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt" for="chkOnlineOrders">Online Orders</label><asp:CheckBox ID="chkOnlineOrders" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt" for="chkEmailConfirmations">Email Confirmations</label><asp:CheckBox ID="chkEmailConfirmations" runat="server" />
                        </li>
                        <li>&nbsp;</li>
                        <li>
                            <label class="Prompt">Annual Contract Signed by</label><asp:TextBox ID="txtAnnualContractSignedName" CssClass="Name" MaxLength="50" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Annual Contract Signed</label><asp:textbox id="txtAnnualContractSignedDate" runat="server" cssclass="SignedDate"/><asp:image id="imgSignedDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image>
                        </li>
                    </ul>

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

                    <asp:Panel ID="pnlLists" runat="server">
                        <div class="FeatureSub">Contacts</div>
                        <ul class="Contacts">
                            <asp:PlaceHolder ID="phContacts" runat="server" />
                        </ul>
                        <div class="FeatureSub">Jobs</div>
                        <ul>
                            <asp:PlaceHolder ID="phJobs" runat="server" />
                        </ul>
                    </asp:Panel>
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
