<%@ Page AutoEventWireup="true" CodeFile="MergeContacts.aspx.cs" Inherits="MergeContacts" Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title><%= Globals.g.Company.Name %> - Merge Contacts</title>
	<!-- MergeContacts.aspx -->
	<!-- Copyright (c) 2018-2019 PeteSoft, LLC. All Rights Reserved -->
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

	        $("#content").on("change", "input, select", SetDirty);

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

	<style type="text/css">
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
            width: 125px;
            text-align: right;
	    }

	    .Name
	    {
            width: 250px;
            display: inline-block;
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
                    <div class="FeatureMain">Merge Contacts</div>
                    <ul class="Fields">
                        <li>
                            <label class="Prompt">Contact #</label>
                            <asp:Label ID="lblRno1" CssClass="Name" runat="server" />
                            <asp:Label ID="lblRno2" CssClass="Name" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Name</label>
                            <asp:RadioButton ID="rdoName1" GroupName="Name" CssClass="Name" runat="server" />
                            <asp:RadioButton ID="rdoName2" GroupName="Name" CssClass="Name" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Email</label>
                            <asp:RadioButton ID="rdoEmail1" GroupName="Email" CssClass="Name" runat="server" />
                            <asp:RadioButton ID="rdoEmail2" GroupName="Email" CssClass="Name" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Phone</label>
                            <asp:RadioButton ID="rdoPhone1" GroupName="Phone" CssClass="Name" runat="server" />
                            <asp:RadioButton ID="rdoPhone2" GroupName="Phone" CssClass="Name" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Cell</label>
                            <asp:RadioButton ID="rdoCell1" GroupName="Cell" CssClass="Name" runat="server" />
                            <asp:RadioButton ID="rdoCell2" GroupName="Cell" CssClass="Name" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Fax</label>
                            <asp:RadioButton ID="rdoFax1" GroupName="Fax" CssClass="Name" runat="server" />
                            <asp:RadioButton ID="rdoFax2" GroupName="Fax" CssClass="Name" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Title</label>
                            <asp:RadioButton ID="rdoTitle1" GroupName="Title" CssClass="Name" runat="server" />
                            <asp:RadioButton ID="rdoTitle2" GroupName="Title" CssClass="Name" runat="server" />
                        </li>
                        <li>
                            <label class="Prompt">Customer</label>
                            <asp:RadioButton ID="rdoCustomer1" GroupName="Customer" CssClass="Name" runat="server" />
                            <asp:RadioButton ID="rdoCustomer2" GroupName="Customer" CssClass="Name" runat="server" />
                        </li> 
                    </ul>

                    <div class="Buttons">
						<asp:Button ID="btnUpdate" runat="server" Text="Save" OnClick="btnUpdate_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" />
                    </div>

                    <ul class="Fields">
                        <li>
                            <label class="Prompt">Created</label>
                            <div class="Name">
							    <asp:textbox id="txtCreatedDt1" runat="server" cssclass="MaintDate" enabled="False" />
							    <asp:textbox id="txtCreatedUser1" runat="server" cssclass="MaintUser" enabled="False" />
                            </div>
                            <div class="Name">
    							<asp:textbox id="txtCreatedDt2" runat="server" cssclass="MaintDate" enabled="False" />
                                <asp:textbox id="txtCreatedUser2" runat="server" cssclass="MaintUser" enabled="False" />
                            </div>
                        </li> 
                        <li>
                            <label class="Prompt">Updated</label>
                            <div class="Name">
							    <asp:textbox id="txtUpdatedDt1" runat="server" cssclass="MaintDate" enabled="False" />
                                <asp:textbox id="txtUpdatedUser1" runat="server" cssclass="MaintUser" enabled="False" />
							</div>
                            <div class="Name">
							    <asp:textbox id="txtUpdatedDt2" runat="server" cssclass="MaintDate" enabled="False"/>
                                <asp:textbox id="txtUpdatedUser2" runat="server" cssclass="MaintUser" enabled="False" />
                            </div>
                        </li> 
                    </ul>
                </div>
            </td>
		</tr>
	</table>
	<input type="hidden" id="txtDirty" name="txtDirty" value="false" runat="server" />
	<input type="hidden" id="hfRno1" name="hfRno" runat="server" />
	<input type="hidden" id="hfRno2" name="hfRno" runat="server" />
	<% Pg.Bottom(); %>
	</form>
</body>
</html>
