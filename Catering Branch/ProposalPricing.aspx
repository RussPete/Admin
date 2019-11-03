<%@ Page AutoEventWireup="true" CodeFile="ProposalPricing.aspx.cs" Inherits="ProposalPricing" Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title>Marvellous Catering - Client Pricing</title> 
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
            .MissingPrice { box-shadow: 0 0 10px red; border: 1px solid #ff000088; border-radius: 3px; }
        </style>

		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
	    <script language="javascript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
	    <script language="javascript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
	    <script language="javascript" src="js/jquery.autosize-min.js" type="text/javascript"></script>
        <script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="https://use.fontawesome.com/ed45f487e9.js" type="text/javascript"></script>
        <script language="JavaScript" src="RecentJobs.js" type="text/javascript"></script>
		<script language="JavaScript" type="text/javascript">
		
		    $(document).ready(function ()
		    {
		    	// autoresize
		    	$("textarea").autosize({append: "\n"});

                var FeeDesc = $(".FeeDesc");

				// save prev value
		    	FeeDesc.focus(function ()
		    	{
		    		$(this).data("prev", $(this).val());
		    	});

		    	// detect if customized
		    	FeeDesc.blur(function ()
		    	{
                    SetCustomized(this, ($(this).val() != $(this).data("prev")));
                    CheckMissingPrice(this);
		    	});

		    	// add 'Customized' class to hidden fields
                FeeDesc.next().addClass("Customized");

				// hide all the "click Undo Custom" hints
		    	$(".Reset, .SeeOptions").hide();

		    	// hide options of customized descriptions
		    	$(".Customized").each(function ()
		    	{
					// customized
		    		if ($(this).val() == "1")
		    		{
		    			$(this).parent().find("ul.Options").hide();
		    			//if ($(this).parent().find("ul.Options").children().length > 0)
		    			//{
		    				$(this).closest("tr").find(".Reset, .SeeOptions").show();
		    			//}
					}
		    		else // not customized
		    		{
		    			BuildDesc(this);
		    			$(this).closest("tr").find(".Reset, .SeeOptions").hide();
		    		}
		    	});

                // look for and highlight textareas that don't contain the fee amount in the text
                FeeDesc.each(function ()
                {
                    CheckMissingPrice(this);
                });

				// reset clicked
		    	$("label.Reset, label.SeeOptions").click(function ()
		    	{
					//// find the input field
		    		//var Input = $(this).closest("tr").find(".FeeDesc");

					//// reset the value
		    		//Input.val(Input.data("default"));
		    		BuildDesc(this);
		    		ClearCustomized(this);
		    	});

		    	// option checked or value set
		    	$(".NumCrew, input:checkbox", $("ul.Options")).change(function ()
		    	{
		    		BuildDesc(this);
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
                    $(this).removeClass("ShowPreview");
                }

		    	$("#hfEmail").val(false);
		    	$("#btnEmail, #btnEmailConfirm").click(Email);

		    	SetupRecentJobs();

		        // dirty bit
		    	$(".EditData :input:not([readonly='readonly']):not([disabled='disabled'])").change(function () {
		    	    SetDirty();
		    	});

		    	$("#btnUpdate, #btnPreview").click(function ()
		    	{
		    	    ClearDirty();
		    	});

		    	$(window).bind("beforeunload", function () {
		    	    return CheckDirty();
		    	});
		    });

		    function BuildDesc(selector)
		    {
		    	var tr = $(selector).closest("tr");
		    	var txt = tr.find(".FeeDesc");
		    	var Desc = txt.data("default");
		    	var Join = " - includes ";

		    	var Crew = tr.find("ul.Options .NumCrew");
		    	var CrewValue = Crew.val();
		    	if (Crew.length > 0 && CrewValue.length > 0 && CrewValue != "0")
		    	{
		    		Desc += " - includes " + Crew.data("desc").replace("#", Crew.val());
		    		Join = " ";
		    	}

		    	var Options = tr.find("ul.Options input:checkbox:checked");
		    	for (var i = 0; i < Options.length; i++)
		    	{
					if (i > 0)
		    		{
						if (i == Options.length - 1)
						{
							Join = " and ";
						}
						else
						{
							Join = ", "
						}
					}

		    		Desc += Join + $(Options[i]).data("desc");
		    	}

		    	txt.val(Desc);
		    	txt.trigger("autosize");
		    }

		    function SetCustomized(selector, fCustomized)
		    {
		    	// current value of customized
		    	var hfCustomized = $(selector).parent().find("input.Customized");
		    	var fAlreadyCustomized = (hfCustomized.val() == "1");

				// is this item customized
		    	fCustomized = (fCustomized || fAlreadyCustomized);

		    	// save the customized value
		    	hfCustomized.val(fCustomized ? 1 : 0);

				// if newly customized, hide options
		    	if (fCustomized && !fAlreadyCustomized)
		    	{
		    		if ($(selector).parent().find("ul.Options").children().length > 0)
		    		{
		    			$(selector).parent().find("ul.Options").slideUp();
		    		}

		    		// show the reset instructions
		    		$(selector).closest("tr").find(".Reset, .SeeOptions").show();
				}
		    }

		    function ClearCustomized(selector)
		    {
		    	var tr = $(selector).closest("tr");

		    	// reset the customized flag
		    	tr.find("input.Customized").val(0);

		    	// show the options
		    	tr.find("ul.Options").slideDown();

		    	// hide the reset instructions
                tr.find(".Reset, .SeeOptions").hide();

                CheckMissingPrice(tr.find(".FeeDesc"));
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

            function CheckMissingPrice(textarea)
            {
                var Text = $(textarea);
                var Amt = Text.closest("tr").find(".Amt").text();
                if (Amt != "$0.00" && !Text.val().includes(Amt))
                {
                    Text.addClass("MissingPrice");
                }
                else
                {
                    Text.removeClass("MissingPrice");
                }
            }
        </script>
	</head>
	<body id="ProposalPricing">
		<form id="form" method="post" runat="server" autocomplete="off">
			<% Pg.Top(); %>
			<asp:panel id="pnlEdit" runat="server">
			    <div style="position: relative;">
                    <%= RecentJobs.Html() %>
                </div>
				<% Pg.JobSubPage("Client Pricing"); %>

                <% if (ErrMsg.Length > 0) { %>
                <div class="ErrMsg"><%= ErrMsg %></div>
                <% } %>

                <asp:HiddenField ID="hfJobRno" runat="server" />

				<table cellspacing="0" cellpadding="0" border="0" class="Inputs EditData" style="margin: 0px auto;">
					<tbody>
						<tr>
							<td><img height="1" src="Images/Space.gif" alt="" width="100" /></td>
							<td></td>
							<td><img height="1" src="Images/Space.gif" alt="" width="350" /></td>
						</tr>
					</tbody>
					<tbody>
						<asp:PlaceHolder ID="phPrices" runat="server" />
						<tr>
							<td align="right">On-site Service Fee (<asp:Label ID="lblServiceAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtServiceDesc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfServiceCustomized" runat="server" />
								<ul id="ulService" class="Options" runat="server">
								</ul>
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description and show all the available options.">Undo Custom</label>
                                <label class="SeeOptions">click Undo Custom to see available options</label> 
							</td>
						</tr>
						<tr>
							<td align="right">Delivery (<asp:Label ID="lblDeliveryAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtDeliveryDesc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfDeliveryCustomized" runat="server" />
								<ul id="ulDelivery" class="Options" runat="server">
								</ul>
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description and show all the available options.">Undo Custom</label>
                                <label class="SeeOptions">click Undo Custom to see available options</label> 
							</td>
						</tr>
						<tr>
							<td align="right">China Rental (<asp:Label ID="lblChinaAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtChinaDesc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfChinaCustomized" runat="server" />
								<ul id="ulChina" class="Options" runat="server">
								</ul>
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description and show all the available options.">Undo Custom</label>
                                <label class="SeeOptions">click Undo Custom to see available options</label> 
							</td>
						</tr>
						<tr>
							<td align="right">Additional On-site (<asp:Label ID="lblAddServiceAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtAddServiceDesc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfAddServiceCustomized" runat="server" />
								<ul id="ulAddService" class="Options" runat="server">
								</ul>
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description and show all the available options.">Undo Custom</label>
                                <label class="SeeOptions">click Undo Custom to see available options</label> 
							</td>
						</tr>
						<tr>
							<td align="right">Fuel & Travel (<asp:Label ID="lblFuelTravelAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtFuelTravelDesc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfFuelTravelCustomized" runat="server" />
								<ul id="ulFuelTravel" class="Options" runat="server">
								</ul>
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description and show all the available options.">Undo Custom</label>
							</td>
                        </tr>
						<tr>
							<td align="right">Venue Fee (<asp:Label ID="lblFacilityAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtFacilityDesc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfFacilityCustomized" runat="server" />
								<ul id="ulFacility" class="Options" runat="server">
								</ul>
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description and show all the available options.">Undo Custom</label>
							</td>
						</tr>
						<tr>
							<td align="right">Rentals (<asp:Label ID="lblRentalsAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:TextBox ID="txtRentalsDesc" CssClass="FeeDesc" MaxLength="100" runat="server" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfRentalsCustomized" runat="server" />
								<ul id="ulRentals" class="Options" runat="server">
								</ul>
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description and show all the available options.">Undo Custom</label>
							</td>
						</tr>
						<tr id="trAdj1" runat="server">
							<td align="right"><asp:Label ID="lblAdj1Desc" runat="server" /> (<asp:Label ID="lblAdj1Amt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtAdj1Desc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfAdj1Customized" runat="server" />
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description.">Undo Custom</label>
							</td>
						</tr>
						<tr id="trAdj2" runat="server">
							<td align="right"><asp:Label ID="lblAdj2Desc" runat="server" /> (<asp:Label ID="lblAdj2Amt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtAdj2Desc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfAdj2Customized" runat="server" />
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description.">Undo Custom</label>
							</td>
						</tr>
						<tr>
							<td align="right">Total (<asp:Label ID="lblTotalAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtTotalDesc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfTotalCustomized" runat="server" />
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description.">Undo Custom</label>
							</td>
						</tr>
						<tr>
							<td align="right">Deposit (<asp:Label ID="lblDepositAmt" CssClass="Amt" runat="server" />)</td>
							<td>
                                <asp:textbox id="txtDepositDesc" runat="server" cssclass="FeeDesc" TextMode="MultiLine" />
                                <asp:HiddenField ID="hfDepositCustomized" runat="server" />
							</td>
							<td>
                                <label class="Reset" title="Click to reset the description.">Undo Custom</label>
							</td>
						</tr>
                    </tbody>
                </table>

                <div style="margin: 10px auto; width: 450px;">
                    <asp:button id="btnUpdate" runat="server" text="Save" OnClick="btnSave_Click" />
                    <asp:button id="btnSavePreview" runat="server" text="Save & Preview" OnClick="btnSavePreview_Click" style="margin-right: 20px;" />
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
