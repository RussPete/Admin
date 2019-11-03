<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Job.aspx.cs" Inherits="Job" Async="true" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head>
		<title><%= Globals.g.Company.Name %> - Job</title> 
		<!-- Job.aspx -->
		<!-- Copyright (c) 2005-2019 PeteSoft, LLC. All Rights Reserved -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Styles.css" type="text/css" rel="stylesheet" />
		<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet" />
		<link href="css/jquery.ui.timepicker.css" rel="stylesheet" type="text/css" />
		<link href="css/all.min.css" rel="stylesheet" type="text/css" />
       	<link href="css/font-awesome.min.css" type="text/css" rel="stylesheet">
        <style type="text/css">
            .PrevNext, .PrevNext:hover { text-decoration: none; font-weight: normal; color: rgba(77, 121, 8, 0.31); }
            .PrevNext:hover { color: #CC0000; }
            .Add { color: blue; cursor: pointer; }
            .EventType { width: 115px; }
            .EventTypeDetails { width: 300px; padding-left: 2px;  }
            .JobDesc { width: 421px; }
            .NumBuffets { width: 30px; text-align: center; }
            .BuffetSpace { width: 385px; }
            .Demographics { width: 365px; padding-left: 2px; }
            .JobServings { text-align: right; }
            .JobPrice { text-align: right; }
            .JobSubTitle { text-align: right; }
            .Crew { width: 420px; }
            #FindJob { display: none; }
            #FindJob #JobRno { width:70px; }
            #AddCustomer { display: none; }
            #AddContact { display: none; }
            #btnSave, #btnDelete { display: none; }
            .ui-dialog dl { margin-bottom: 5px; }
            .ui-dialog dt, .ui-dialog dd { display: inline-block; margin-inline-start: unset; }
            .ui-dialog dt { width: 50px; }
            .ui-dialog #AddContact dt { width: 70px; }
            .ui-dialog dd { width: 170px; margin-top: 5px; }
            .ui-dialog dd:first-of-type { margin-top: 0px; }
            .aspNetDisabled
            {
                background-color: rgb(235, 235, 228);
                border-style: solid;
                border-width: 1px;
                border-color: #aaa;
                padding: 2px 1px;
                color: rgb(84, 84, 84);            
            }
        </style>
		
		<!--script language="javascript" src="SelectList.js" type="text/javascript"></script-->
		<!--script language="javascript" src="Calendar.js" type="text/javascript"></script-->
		<script language="JavaScript" src="Web.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
        <%--<script language="javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>--%>
		<script language="JavaScript" src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="js/jquery.ui.timepicker.js" type="text/javascript"></script>
    	<script language="JavaScript" src="js/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
        <script language="JavaScript" src="https://use.fontawesome.com/ed45f487e9.js" type="text/javascript"></script>
        <script language="JavaScript" src="RecentJobs.js" type="text/javascript"></script>
        <script language="javascript" src="Sling.js" type="text/javascript"></script>
        <script language="javascript" src="Misc.js" type="text/javascript"></script>
		<script language="JavaScript" type="text/javascript">	
            var fNew = <%=(fNew ? "true" : "false")%>;
            var aJobDates = Array(<%=JobDates()%>);
            var InvEventCount = <%=InvEventCount%>;
            var InvPricePerPerson = <%=InvPricePerPerson%>;

            var Customers = <%= CustomerData() %>;
            var Contacts = <%= ContactData() %>;
            var EventTypes = <%= Misc.jsArray(aEventTypes) %>;
            var ServiceTypes = <%= Misc.jsArray(aServiceTypes) %>;
		    var Vehicles = <%= Misc.jsArray(aVehicles) %>;
        </script>

<script language="JavaScript" type="text/javascript">

var origStartDateTime;
var fDoSling = false;

//----------------------------------------------------------------------------
    $(document).ready(function ()
    {
        var fCustomerSelected = false;
        var fContactSelected = false;

        SetupSearch();
        //SearchList("#txtSearch", "#lstList");

        // additional search fields
        $("#upSearch").on("click", "#MoreSearch", function ()
        {
            $(this).hide("slow");
            $("#hfAddlSearch").val(true);
            $("#ExpandedSearch").slideDown("slow", function () 
            {
                $("#btnSearch").click();
            });

        });

        // less search fields
        $("#upSearch").on("click", "#LessSearch", function ()
        {
            $("#MoreSearch").show();
            $("#hfAddlSearch").val(false);
            $("#ExpandedSearch").slideUp("slow", function () 
            {
                $("#btnSearch").click();
            });
        });

        // clear search fields
        $("#upSearch").on("click", "#btnClear", function ()
        {
            $("#txtSearchCustomer, #txtSearchContact, #txtSearchPhone, #txtSearchEmail, #txtSearchLocation, #txtSearchDate").val("");
            $("#btnSearch").click();
        });

        // dirty bit
        $(".EditData :input:not([readonly='readonly']):not([disabled='disabled'])").change(function () 
        {
            SetDirty();
        });

        $(window).bind("beforeunload", function () 
        {
            return CheckDirty();
        });

        $("#lnkAddCustomer").click(function ()
        {
            $("#AddCustomer").dialog(
                {
                    buttons:
                    {
                        Cancel: function ()
                        {
                            $(this).dialog("close");
                        },

                        Save: function ()
                        {
                            $(this).dialog("close");
                            $("#hfCustomerRno").val("");
                            $("#txtCustomer").val($("#txtAddCustomerName").val());
                            $("#hfCustCompany").val($("#chkAddCustomerCompany").prop("checked"));
                            $("#hfCustTaxExempt").val($("#chkAddCustomerTaxExempt").prop("checked"));
                            $("#hfCustOnlineOrders").val($("#chkAddCustomerOnlineOrders").prop("checked"));
                            $("#hfCustEmailConfirmations").val($("#chkAddCustomerEmailConfirmations").prop("checked"));
                            $("#txtContact,#hfContactRno,#txtPhone,#txtCell,#txtEmail,#hfFax,#hfTitle").val("");  // clear contact & info
                            $("#txtContact").focus();
                        }
                    },
                    modal: true,
                    open: function ()
                    {
                        // set default button to Save
                        //$(this).siblings(".ui-dialog-buttonpane").find("button:eq(1)").focus();
                    }
                });
        });

        $("#lnkAddContact").click(function ()
        {
            $("#spnCustomer").text($("#txtCustomer").val());

            $("#AddContact").dialog(
                {
                    buttons:
                    {
                        Cancel: function ()
                        {
                            $(this).dialog("close");
                        },
                        Save: function ()
                        {
                            $(this).dialog("close");
                            $("#hfContactRno").val("");
                            $("#txtContact").val($("#txtAddContactName").val());
                            $("#txtPhone").val($("#txtAddContactPhone").val());
                            $("#txtCell").val($("#txtAddContactCell").val());
                            $("#txtEmail").val($("#txtAddContactEmail").val());
                            $("#hfFax").val($("#txtAddContactFax").val());
                            $("#hfTitle").val($("#txtAddContactTitle").val());
                        }
                    },
                    modal: true,
                    open: function ()
                    {
                        $("#txtAddContactName").focus();
                        // set default button to Save
                        //$(this).siblings(".ui-dialog-buttonpane").find("button:eq(1)").focus();
                    }
                });
        });

        // customer autocomplete
        $("#txtCustomer")
            .autocomplete({
                source: Customers,
                // use the Customers as a source, but match on the start of the values instead of anywhere in the value
                //source: function(req, response) { 
                //	var re = $.ui.autocomplete.escapeRegex(req.term); 
                //	var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                //	response($.grep(Customers, function(Item, ItemIndex){ 
                //		return Matcher.test(Item.value); 
                //	}) ); 
                //},
                //autoFocus: true,
                delay: 0,
                minLength: 0,
                select: function (event, ui)
                {
                    fCustomerSelected = true;
                    $("#hfCustomerRno").val(ui.item.id);
                    $("#txtContact,#hfContactRno,#txtPhone,#txtCell,#txtEmail,#hfFax,#hfTitle").val("");
                }
                /*
                change: function()
                {
                    fCustomerSelected = false;
                },
                close: function()
                {
                    if (fCustomerSelected)
                    {
                        fCustomerSelected = false;
                        $("#txtContact").focus();
                    }
                }
                */
            });
        if ($("#txtCustomer").val().length == 0)
        {
            // drop down on focus
            $("#txtCustomer").focus(function ()
            {
                $(this).autocomplete("search", "");
            });
        }
        $("#txtCustomer").focus(function ()
        {
            $(this).prop("prev", $(this).val());
        });
        $("#txtCustomer").blur(function ()
        {
            // if clearing the customer
            if ($(this).val().length == 0 && $(this).prop("prev").length != 0)  // current value is blank and previous value was not blank
            {
                // clear the contact too
                $("#hfCustomerRno,#txtContact,#hfContactRno,#txtPhone,#txtCell,#txtEmail,#hfFax,#hfTitle").val("");
            }
            else
            {
                let CustomerRno = parseInt($("#hfCustomerRno").val());
                if (CustomerRno)
                {       // if a customer has been selected, make sure the name isn't left messed up if user has been 
                    var iCustomer;
                    for (iCustomer = 0; iCustomer < Customers.length; iCustomer++)
                    {
                        if (Customers[iCustomer].id == CustomerRno)
                        {
                            $("#txtCustomer").val(Customers[iCustomer].value);
                        }
                    }
                }
            }
        });
        /*
		// filter out tabs, they were messing up the flow when a customer is selected in the drop down list.
		.keydown(function(event){
			if (event.which == 9){
				event.preventDefault();
			}
		});
		*/

        // contact autocomplete
        $("#txtContact")
            .focus(function ()
            {
                //var CustomersContacts = Contacts();
                $(this)
                    .autocomplete({
                        source: ContactData(),
                        // use the Customer's contacts as a source, but match on the start of the values instead of anywhere in the value
                        //source: function(req, response) { 
                        //	var re = $.ui.autocomplete.escapeRegex(req.term); 
                        //	var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                        //	response($.grep(CustomersContacts, function(Item, ItemIndex){ 
                        //		return Matcher.test(Item.value); 
                        //	}) ); 
                        //},
                        //autoFocus: true,
                        delay: 0,
                        minLength: 0,
                        select: function (event, ui)
                        {
                            //fContactSelected = true;
                            $(this).val(ui.item.value.replace(" *", ""));
                            $("#hfContactRno").val(ui.item.id);
                            $("#txtPhone").val(ui.item.phone);
                            $("#txtCell").val(ui.item.cell);
                            $("#txtFax").val(ui.item.fax);
                            $("#txtEmail").val(ui.item.email);
                            $("#hfFax").val(ui.item.fax);
                            $("hfTitle").val(ui.item.title);
                            //$("#txtEventType").focus();
                            return false;   // use the value we just set, not the value in the source
                        },
						/*
						change: function(){
							fContactSelected = false;
						},
						close: function(){
							if (fContactSelected){
								fContactSelected = false;
								$("#txtEventType").focus();
							}
						}
						*/
                    })
                    .autocomplete("search", $(this).val());
            });
        /*
        // filter out tabs, they were messing up the flow when a contact is selected in the drop down list.
        .keydown(function(event){
            if (event.which == 9){
                event.preventDefault();
            }
        });
        */

        $("#txtContact").blur(function ()
        {
            // if clearing the contact
            if ($(this).val().length == 0)
            {
                // clear the contact values too
                $("#hfContactRno,#txtPhone,#txtCell,#txtEmail,#hfFax,#hfTitle").val("");
            }
        });

        // job type
        CheckJobType();
        $("#ddlJobType").change(function ()
        {
            CheckJobType();
        });

        // event type autocomplete
        $("#txtEventType")
            .autocomplete({
                //source: EventTypes,
                source: function (req, response)
                {
                    var re = $.ui.autocomplete.escapeRegex(req.term);
                    var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
                    response($.grep(EventTypes, function (Item, ItemIndex)
                    {
                        return Matcher.test(Item);
                    }));
                },
                change: function ()
                {
                    CheckCeremonyField();
                },
                autoFocus: true,
                delay: 0,
                minLength: 0
            });
		/*
		// drop down on focus
		.focus(function(){
		//	$(this).autocomplete("search", "");
			alert("eventtype");
		});
		*/

        // job description synch
        $("#txtJobDesc")
            .focus(function ()
            {
                if ($(this).val().length == 0)
                {
                    var Customer = $("#txtCustomer").val();
                    var Contact = $("#txtContact").val();
                    var EventType = $("#txtEventType").val();
                    var JobDesc = "";
                    if (Customer.length > 0 && EventType.length > 0)
                        JobDesc = Customer + ' - ' + EventType;
                    else if (Customer.length > 0 && Contact.length > 0)
                        JobDesc = Customer + ' - ' + Contact;
                    else if (Customer.length == 0 && Contact.length > 0 && EventType.length > 0)
                        JobDesc = Contact + ' - ' + EventType;
                    else if (Customer.length == 0)
                        JobDesc = Contact;
                    else
                        JobDesc = Customer;
                    $(this).val(JobDesc);
                    $("#lblJobDesc").text(JobDesc);
                }
            })
            .change(function ()        
            {
                $("#lblJobDesc").text($(this).val());
            });

	// service type autocomplete
	$("#txtServiceType")
		.autocomplete({
			//source: ServiceTypes,
			source: function(req, response) { 
				var re = $.ui.autocomplete.escapeRegex(req.term); 
				var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
				response($.grep(ServiceTypes, function(Item, ItemIndex){ 
					return Matcher.test(Item); 
				}) ); 
			},			
			autoFocus: true,
			delay: 0,
			minLength: 0		
		});
		
		/*
		// drop down on focus
		.focus(function(){
			alert("service type");
		//	$(this).autocomplete("search", "");
		});
		*/
		

	// vehicle autocomplete
	$("#txtVehicle")
		.autocomplete({
			//source: Vehicles,
			source: function(req, response) { 
				var re = $.ui.autocomplete.escapeRegex(req.term); 
				var Matcher = new RegExp("^" + re, "i");	// search for given term at start of data items
				response($.grep(Vehicles, function(Item, ItemIndex){ 
					return Matcher.test(Item); 
				}) ); 
			},			
			autoFocus: true,
			delay: 0,
			minLength: 0		
		});
		/*
		// drop down on focus
		.focus(function(){
			$(this).autocomplete("search", "");
		})
		*/
	
	$("#txtJobDate,#txtConfirmDeadlineDate").datepicker({
		showOn: "both",
		buttonImage: "Images/CalendarIcon.gif",
		buttonImageOnly: true
	});	
	
	$(".JobTime").timepicker({
		showPeriod: true,
		showLeadingZero: false
	});

    // jobs & proposals checked
	$("#chkJobs, #chkProposals").click(function()
	{
        // if both are not checked
	    if (!$("#chkJobs").prop("checked") && !$("#chkProposals").prop("checked"))
	    {
	        // force the other one checked
	        if ($(this).prop("id") == "chkJobs")
	        {
	            $("#chkProposals").prop("checked", true);
	        }
            else
	        {
	            $("#chkJobs").prop("checked", true);
	        }
        }
	});

	$("#chkProposal").click(function()
	{
	    $(".FeatureMain").html($(this).prop("checked") ? "Proposal" : "Job");
	});

    // search for job # 
	$("#imgSearch").click(function()
	{
	    $("#FindJob").dialog(
	    {
	        buttons:
	        {
	            Find: function()
	            {
	                document.location.href = "Job.aspx?JobRno=" + $("#JobRno").val();
	            }
	        },
            modal: true
	    });
	});
	$("#JobRno").keypress(function(e)
	{
	    if (e.which == 13)  // Enter key while in Job # field
	    {
	        document.location.href = "Job.aspx?JobRno=" + $("#JobRno").val();
	    }
	});

	SetupRecentJobs();

	CheckCeremonyField();
	$("#txtEventType, #rbCeremonyYes, #rbCeremonyNo").change(function()  
	{
	    CheckCeremonyField();
	});

	if (fDoSling)
	{
	    setupSlingShift();
	}

	$("#btnPreDelete,#btnCopy").prop("disabled", ($("#txtJobRno").val().length == 0));
	//$("#btnPreSave").prop("disabled", !($("#txtCustomer").val().length > 0 && $("#txtJobDate").val().length > 0));

	$("#btnPreSave").click(() =>
	{
	    if (CanSave())
	    {
	        if (sling)
	        {
	            updateSlingShift();     // triggers btnSave_Click() if data is valid after creating new shift in sling
	        }
	        else
	        {
	            $("#btnSave").trigger("click");     // save on the server
	        }
	    }
	});

	$("#btnPreDelete").click(() =>
	{
	    if (window.confirm('Delete this Job?'))
	    {
	        if (sling)
	        {
	            deleteSlingShift();     // triggers btnDelete_Click() after updating sling
	        }
	        else
	        {
	            $("#btnDelete").trigger("click");    // delete on the server
	        }
	    }
	});

	origStartDateTime = startDateTime();
});

//----------------------------------------------------------------------------
function SetupSearch()
{
    if ($("#hfAddlSearch").val().toLowerCase() == "true")
    {
        $("#MoreSearch").hide();
    }
    else
    {
        $("#ExpandedSearch").hide();
    }

    $("#txtSearchDate").datepicker({
        showOn: "both",
        buttonImage: "Images/CalendarIcon.gif",
        buttonImageOnly: true
    });		
}
		
//----------------------------------------------------------------------------
function ContactData()
{
	var ContactData = [];
    var CustomerRno = parseInt($("#hfCustomerRno").val());
    if (CustomerRno)
    {
        var iCustomer;
        for (iCustomer = 0; iCustomer < Customers.length; iCustomer++)
        {
            if (Customers[iCustomer].id == CustomerRno)
            {
                ContactData = Customers[iCustomer].contacts;
                break;
            }
        }
    }
    else
    {
        if ($("#txtCustomer").val().length == 0)    // if not customer rno, and not a new customer waiting to be created
        {
            ContactData = Contacts;     // use all the contacts without a customer
        }
    }
	
	return ContactData;
}

//----------------------------------------------------------------------------
function CheckJobType()
{
    switch ($("#ddlJobType").val())
    {
        case "Private":
            $("#PmtTerms").hide();
            break;
        case "Corporate":
            $("#PmtTerms").show();
            break;
    }
}

//----------------------------------------------------------------------------
function Init()
{
	RestoreDirty();
	
	<%=Warning()%>
	<%=SetFocus()%>
	
	CheckEventCount();
	CheckPricePerPerson();
	iSetDis("btnPrint", iGetStr("txtJobRno") == "");
}

//----------------------------------------------------------------------------
function lstListOnChange()
{
	try
	{
		window.location.href = "Job.aspx?JobRno=" + iGetSel("lstList");
	}
	catch (err)
	{
		iSetSel("lstList", iGetStr("txtJobRno"));
	}
}

//----------------------------------------------------------------------------
function CheckJobDate()
{
	var JobDate = iGetStr("txtJobDate");
	var DayOfWeek = "";

	if (JobDate != "")
	{
		var Dt = new Date(JobDate);
		if (!isNaN(Dt))
		{
			var Days = new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");
			DayOfWeek = Days[Dt.getDay()];
			
			var Today = new Date();
			if (Dt < Today)
			{
				//alert("Warning: You are changing Job Date to a date in the past.");
			}
		}		
	}
	
	iSetStr("txtJobDayOfWeek", DayOfWeek);
}
            
//----------------------------------------------------------------------------
function CheckCeremonyField()
{
    var EventType = $("#txtEventType").val();
    var fWedding = (EventType.indexOf("Wedding", 0) == 0);
    var fCeremony = $("#rbCeremonyYes").prop("checked");

    $("#rbCeremonyYes, #rbCeremonyNo").prop("disabled", !fWedding);
    $("#txtWeddingTime, #txtCeremonyLocation").prop("disabled", !fCeremony);

    if (!fWedding)
    {
        $("#rbCeremonyNo").prop("checked", true);
        $(".ceremony").hide("slow");
    }
    else
    {
        $(".ceremony").show("slow");
    }
    if (!fWedding || !fCeremony)
    {
        $("#txtWeddingTime, #txtCeremonyLocation").val("");
    }
}

//----------------------------------------------------------------------------
function CheckRequiredField(FieldName, FieldDesc)
{
	//var Customer = Trim(iGetStr("txtCustomer"));
	var JobDate = iGetStr("txtJobDate");
	var Dt = new Date(JobDate);
	var fDisabled = false;
	//fDisabled = (Customer == "" || isNaN(Dt));
	fDisabled = isNaN(Dt);
	iSetDis("btnPreSave", fDisabled)

	var Text = Trim(iGetStr(FieldName));
	if (Text == "")
	{
		alert("A " + FieldDesc + " is required to save the Job.");
	}
	
	return !fDisabled;
}

//----------------------------------------------------------------------------
function JobDateChange(FieldName)
{
	CheckDate(FieldName);
	
	var Field = GetDocObj(FieldName);
	
	if (Field)
	{		
		if (!fNew)
		{
			alert("Warning: You are changing the date of a scheduled job.");
		}
		
		var JobDate = new Date(iGetStr("txtJobDate"));
		if (!isNaN(JobDate))
		{
			for (var iDates = 0; iDates < aJobDates.length; iDates++)
			{
				if (JobDate.valueOf() == aJobDates[iDates].valueOf())
				{
					alert("Warning: This contact already has a job scheduled on this date.");
					break;
				}
			}
		}

		ValidateDate(Field);
	}
	
	CheckRequiredField('txtJobDate', 'Job Date');
	CheckJobDate();
}

//----------------------------------------------------------------------------
function CanSave()
{
	var fOk = true;
	
	if (fOk &&
        $("#ddlJobType").val().length == 0)
	{
	    fOk = false;
	    alert("Job can not be saved until a Job Type is selected.");
    }

    if (fOk &&
        ($("#hfContactRno").val() == "" || $("#hfContactRno").val() == 0) &&    // if not a selected contact and
        $("#txtContact").val().length == 0)                                     // not a new contact entered
    {
        fOk = false;
        alert("Job can not be saved until a Contact has been added or selected.");
    }

	let fProposal = $("#chkProposal").prop("checked");

	if (!fProposal)
	{
	    if (fOk && 
            $("#ddlJobType").val() == "Corporate" &&
            $("#ddlPmtTerms").val() == "")
	    {
	        fOk = false;
	        alert("Corporate job requires Payment Terms.");
	    }

	    if (fOk && 
            iGetChk("chkCancelled") && 
            iGetStr("txtCanDt") == "")
	    {
	        fOk = confirm(
                "Warning: Job is set as cancelled.\n\n" +
                "It will not appear on reports.\n\n" +
                "OK to save as Cancelled anyway?");
	    }
	
	    if (fOk &&
            iGetStr("txtDepartTime") == "")
	    {
	        fOk = confirm(
                "Warning: Job has no Depart Time.\n\n" +
                "Some of the reports sort by Depart Time so\n " +
                "this job may not appear in proper sequence.\n\n" +
                "OK to save with no Depart Time anyway?");
	    }
		 
	    if (fOk &&
	        //Num(iGetStr("txtServingTotal")) == 0)
            Num(iGetStr("txtServingMen")) == 0)
	    {
	        fOk = confirm(
                "Warning: Job has 0 servings.\n\n" +
                "Some of the reports emphasize or\n" +
                "total # servings to help with \n" +
                "cooking and other preparations.\n\n" +
                "OK to save with 0 servings anyway?");
	    }

	    if (fOk &&
            iGetChk("chkInvoiced") &&
            iGetStr("txtInvCreatedDt") == "")
	    {
	        fOk = confirm(
                "Warning: Job is set as Invoiced but no invoice \n" +
                "has been saved from the Invoice tab. This job \n" +
                "may never be found to be invoiced.\n\n" +
                "OK to Save as Invoiced anyway?");
	    }
	}

	if (fOk)
	{
	    ClearDirty();
	}

	return fOk;
}

//----------------------------------------------------------------------------
function SetSubtotal()
{
	var	cMen     = iGetNum("txtServingMen");
	//var	cWomen   = iGetNum("txtServingWomen");
	//var	cChild   = iGetNum("txtServingChild");
	var Price    = iGetNum("txtPricePerPerson");
	//var SubTotal = Price * (cMen + cWomen + cChild);
	var SubTotal = Price * cMen;
	
	iSetStr("txtPricePerPerson", FmtDollar(Price, ""));
	//iSetStr("txtServingTotal", FmtNum(cMen + cWomen + cChild));
	iSetStr("txtSubTotal", FmtDollar(SubTotal, ""));
}

//----------------------------------------------------------------------------
function CheckEventCount()
{
	var Class = "JobServings";
	var Title = "";
	
	var TotalCount = iGetNum("txtServingMen") + 
					 iGetNum("txtServingWomen") + 
					 iGetNum("txtServingChild");
	if (TotalCount != InvEventCount && InvEventCount > 0)
	{
		//alert("Warning: The number of servings set on the invoice is " + InvEventCount + ".");
		Class = "JobServingsNotSame";
		Title = "Invoice Event Count is " + FmtNum(InvEventCount);
	}

	SetClass("txtServingMen",   Class);
	//SetClass("txtServingWomen", Class);
	//SetClass("txtServingChild", Class);
	SetTitle("txtServingMen",   Title);
	//SetTitle("txtServingWomen", Title);
	//SetTitle("txtServingChild", Title);
	
	SetSubtotal();
}

//----------------------------------------------------------------------------
function CheckPricePerPerson()
{
	var Class = "JobPrice";
	var Title = "";
	
	var PricePerPerson = iGetNum("txtPricePerPerson");
	if (PricePerPerson != InvPricePerPerson && InvPricePerPerson > 0)
	{
		//alert("Warning: The price per person on the invoice is " + FmtDollar(InvPricePerPerson) + ".");
		Class = "JobPriceNotSame";
		Title = "Invoice Price Per Person is " + FmtDollar(InvPricePerPerson);
	}

	SetClass("txtPricePerPerson", Class);
	SetTitle("txtPricePerPerson", Title);
	
	SetSubtotal();
}

//----------------------------------------------------------------------------
function SetStatus()
{
	var Status = "New";

	if (iGetChk("chkCompleted"))	{ Status = "Completed"; }
	if (iGetChk("chkConfirmed"))	{ Status = "Confirmed"; }
	if (iGetChk("chkPrinted"))		{ Status = "Printed"; }
	if (iGetChk("chkInvoiced"))		{ Status = "Invoiced"; }
	if (iGetChk("chkCancelled"))	{ Status = "Cancelled"; }

	iSetStr("txtStatus", Status);
}

//----------------------------------------------------------------------------		
function CheckDate(FieldName)
{
	var dt = new Date(iGetStr(FieldName));
	if (!isNaN(dt))
	{
		var Today = new Date();
		Today.setHours(0);
		Today.setMinutes(0);
		Today.setSeconds(0);
		Today.setMilliseconds(0);

		var FutureDate = new Date(Today);
		FutureDate.setFullYear(FutureDate.getFullYear() + 1);
			
		if (dt.valueOf() < Today.valueOf())
		{
			alert("Warning: This date is in the past.");
		}
		else
		if (dt.valueOf() > FutureDate.valueOf())
		{
			alert("Warning: This date is more than one year in the future.");
		}
	}
}

//----------------------------------------------------------------------------
function Print()
{
	var fFinalPrint = false;
	
	if (fDirty)
	{
		alert("Some data has changed, please save before printing.");
	}
	else
	{
		if (iGetStr("txtPrintDt") == "")
		{			
			if (confirm("Is this the final job sheet print?\n\nIf Yes, click OK to set the Job Status to 'Printed'.\n\nif No, click Cancel to Print without changing the Job Status."))
			{
				fFinalPrint = true;
			}
		}

//		window.open("PrintJobSheet.aspx");
		window.open(
			"JobSheets.aspx?JobRno=" + iGetStr("txtJobRno") +
			"&FinalPrint=" + (fFinalPrint ? "1" : "0"), 
			"_blank");
			
		// get the correct state for the invoiced data
		if (fFinalPrint)
		{
			window.location.href("Job.aspx?JobRno=" + iGetStr("txtJobRno"));	
		}
	}
}

var sling = null;

//----------------------------------------------------------------------------
function setupSlingShift()
{
    if (slingEmail && slingPassword)
    {
        sling = new Sling();
        sling.login(slingEmail, slingPassword, slingOrgId, () =>
        {
            sling.getLocationId(slingLocation, (locationId) =>
            {
                sling.locationId = locationId;
            });

            sling.getPositionId(slingPosition, (positionId) =>
            {
                sling.positionId = positionId;
            });

            sling.loadUsers();
        });
    }
}

//----------------------------------------------------------------------------
function startDateTime()
{
    var jobDate = $("#txtJobDate").val();
    var begTime = $("#txtLoadTime").val() || $("#txtDepartTime").val() || $("#txtArrivalTime").val() || $("#txtMealTime").val();
    begTime = new Date(jobDate + " " + begTime);

    return begTime;
}

//----------------------------------------------------------------------------
function updateSlingShift()
{
    // save button has been clicked
    var slingShiftId = 0;
    try
    {
        slingShiftId = parseInt($("#hfSlingShiftId").val(), 10);
    }
    catch (e)
    {
    }

    let fSaveAtSling = true,
        jobDate = $("#txtJobDate").val(),
        begTime = startDateTime();
        //notCustomers = ["molly's", "mollys", "institute", "fhe freedom 1st ward", "mcc staff meals", "f.h.e. singles ward"],
        //notServiceTypes = ["pick up", "pick-up"],
        //notLocations = ["here"];
        
    if ($("#chkProposal").prop("checked") ||                                                // a proposal
        //oneOf($("#txtCustomer").val().toLowerCase(), notCustomers) ||                       // customers
        //oneOf($("#txtServiceType").val().toLowerCase().substr(0, 7), notServiceTypes) ||    // service types
        //oneOf($("#txtLocation").val().toLowerCase(), notLocations))                         // locations
        $("#txtCrew").val().length == 0)
    {
        fSaveAtSling = false;
    }

    // 15 sec timeout to do the sling stuff
    let failSafe = setTimeout(()=> { $("#btnSave").trigger("click"); }, 15000);

    // if we have a date & time and this job should be saved at Sling
    if (jobDate && begTime && fSaveAtSling)
    {
        let endTime = $("#txtEndTime").val(),
            summaryArray = [];
        for (let part of [$("#txtCustomer").val(), `Job #${$("#txtJobRno").val()}`, `${$("#txtServingMen").val()} Servings`, $("#txtLocation").val(), $("#txtCrew").val(), $("#txtProdNotes").val()])
        {
            if (part.length > 0)
            {
                summaryArray.push(part);
            }
        }
        let summary = summaryArray.join(" - \n");

        if (endTime)
        {
            endTime = new Date(jobDate + " " + endTime);
        }
        else
        {
            endTime = new Date(begTime);
            endTime.setHours(endTime.getHours() + 1);
        }

        // if not previously saved on Sling, createit
        if (!slingShiftId)
        {
            // create
            sling.createShift(begTime, endTime, sling.locationId, sling.positionId, summary, (slingShiftId) =>
            {
                $("#hfSlingShiftId").val(slingShiftId);
                sling.publishShift(slingShiftId, () =>
                {
                    sendNewJobNotification(begTime, true, () =>
                    {
                        clearTimeout(failSafe);
                        $("#btnSave").trigger("click");    // save on server
                    });
                });
            });
        }
        else
        {
            // update
            sling.updateShift(slingShiftId, begTime, endTime, sling.locationId, sling.positionId, summary, 
                () =>       // completed
            {
                sling.publishShift(slingShiftId, () =>
                {
                    if (begTime != origStartDateTime)
                    {
                        sendNewJobNotification(begTime, false, () =>
                        {
                            clearTimeout(failSafe);
                            $("#btnSave").trigger("click");    // save on server
                        });
                    }
                    else
                    {
                        clearTimeout(failSafe);
                        $("#btnSave").trigger("click");    // save on server
                    }
                });
            }, 
            () =>       // error
            {
                // shift is no longer available for updating, clear the shift id and create a new one
                $("#hfSlingShiftId").val(0);
                updateSlingShift();
            });
        }
    }
    else
    {
        // shouldn't be saved at Sling
        // if this job has been previously saved on Sling, remove it
        if (slingShiftId)
        {
            $("#hfSlingShiftId").val(0);            // zero out in the db
            sling.deleteShift(slingShiftId, () =>   // delete on Sling
            {
                sling.publishShift(slingShiftId, () =>
                {
                    clearTimeout(failSafe);
                    $("#btnSave").trigger("click");    // save on server
                });
            });
        }
        else
        {
            clearTimeout(failSafe);
            $("#btnSave").trigger("click");    // save on server
        }
    }
}

//----------------------------------------------------------------------------
function deleteSlingShift()
{
    var slingShiftId = 0;
    try
    {
        slingShiftId = parseInt($("#hfSlingShiftId").val(), 10);
    }
    catch (e)
    {
    }

    if (slingShiftId)
    {
        // 15 sec timeout to do the sling stuff
        let failSafe = setTimeout(()=> { $("#btnDelete").trigger("click"); }, 15000);
        sling.deleteShift(slingShiftId, () =>
        {
            sling.publishShift(slingShiftId, () =>
            {
            });

            clearTimeout(failSafe);
            $("#btnDelete").trigger("click");    // delete on server
        });
    }
    else
    {
        $("#btnDelete").trigger("click");    // delete on server
    }
}

//----------------------------------------------------------------------------
function oneOf(value, values)
{
    let fFound = false;
    for (let srch of values)
    {
        if (value == srch)
        {
            fFound = true;
            break;
        }
    }

    return fFound;
}

//----------------------------------------------------------------------------
function sendNewJobNotification(begTime, fNew, callback)
{
    let fSent = false;

    // test if Sling Admin should be notified of a new job 
    if (begTime.getTime() > Date.now())
    {
        let dtCheck = new Date(begTime.getFullYear(), begTime.getMonth(), begTime.getDate());    // find the beginning of the day
        dtCheck.setDate(dtCheck.getDate() - 7);     // go back a week
        dtCheck.setDate(dtCheck.getDate() + 6 - dtCheck.getDay()) // slide up to the next saturday
        let today = new Date(Date.now());
        if (today >= dtCheck.getTime())  // is this new job being added after the previous planning day for the comming week
        {
            // notify the planner about this new job
            // lookup user's id

            let user = sling.userCollection.findUserBy("email", SlingUserEmail);
            if (user)
            {
                let days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
                let start =  `${days[begTime.getDay()]} ${begTime.getHours() % 12}:${("0" + begTime.getMinutes()).right(2)}${begTime.getHours() < 12 ? "am" : "pm"} ${begTime.getMonth() + 1}/${begTime.getDate()}/${begTime.getFullYear()}`;
                let rno = parseInt($("#txtJobRno").val());
                let msg = 
                {
                    "content": `${fNew ? "New" : "Updated"} Job\n${start}\n${$("#txtCustomer").val()}\nJob #${rno}\n${location.protocol}//${location.host}/DailyJobs.aspx?JobRno=${encodeURIComponent(MangleNumber(rno))}`,
                    "name": `${fNew ? "New" : "Updated"} Job`,
                    "personas": [{
                        "id": user.id,
                        "type": "user" }],
                    private: true
                };
                sling.addToPrivateConversation(msg, function(data)
                {
                    if (callback)
                    {
                        callback();
                    }
                });

                fSent = true;
            }
        }
    }

    if (!fSent && callback)
    {
        callback();
    }
}

//----------------------------------------------------------------------------
function MangleNumber(number)
{
    var hex = number.toString(16).toUpperCase();
    var a = hex.split("");
    for (var i = 0; i < hex.length; i++)
    {
        a[i] = String.fromCharCode(a[i].charCodeAt(0) + 5);
    }
    var val = a.join("");
    return val;
}

</script>
		<%//=Utils.SelectList.JavaScript()%>
		<%//=calJobDate.JavaScript()%>
		<script language="javascript" type="text/javascript">
<%--		    var slingEmail = "<%=SlingEmail%>";
		    var slingPassword = "<%=SlingPassword%>";
		    var slingOrgId = "<%=SlingOrgId%>";
		    var slingLocation = "<%=SlingLocation%>";
		    var slingPosition = "<%=SlingPosition%>";
		    var SlingUserEmail = "<%=SlingUserEmail%>";--%>

//calJobDate.DateSet = JobDateChange;
		</script>
	</head>
	<body onload="Init();">
		<form id="form" method="post" autocomplete="off" runat="server">
            <asp:ScriptManager ID="ScriptManager1" EnablePartialRendering="true" runat="server"></asp:ScriptManager>
			<% Pg.Top(); %>
			<table cellspacing="0" cellpadding="0" align="center" border="0">
				<tr>
					<td valign="top">
                        <asp:UpdatePanel id="upSearch" runat="Server">
                            <ContentTemplate>
                                <script type="text/javascript">
                                    Sys.Application.add_load(function()
                                    {
                                        SearchList("#txtSearch", "#lstList");
                                    });
                                </script>
						        <table cellspacing="0" cellpadding="0" align="center" border="0">
                                    <tr>
                                        <td colspan="3">
                                            <asp:Label ID="lblInfo" runat="server" />
                                            <asp:Label ID="lblSql" runat="server" Visible="false" />
                                            <div id="ExpandedSearch">
                                                <asp:HiddenField ID="hfAddlSearch" runat="server" />
                                                <table cellspacing="0" cellpadding="0" align="center" border="0" width="100%">
                                                    <tr><td align="right">Search:</td><td><img height="1" src="Images/Space.gif" alt="" width="5"></td><td><span id="LessSearch" title="Hide additonal search fields">-</span></td></tr>
                                                    <tr><td align="right">Customer</td><td /><td><asp:TextBox ID="txtSearchCustomer" CssClass="SearchValue" runat="server" /></td></tr>
                                                    <tr><td align="right">Contact</td><td /><td><asp:TextBox ID="txtSearchContact" CssClass="SearchValue" runat="server" /></td></tr>
                                                    <tr><td align="right">Phone</td><td /><td><asp:TextBox ID="txtSearchPhone" CssClass="SearchValue" runat="server" /></td></tr>
                                                    <tr><td align="right">Email</td><td /><td><asp:TextBox ID="txtSearchEmail" CssClass="SearchValue" runat="server" /></td></tr>
                                                    <tr><td align="right">Location</td><td /><td><asp:TextBox ID="txtSearchLocation" CssClass="SearchValue" runat="server" /></td></tr>
                                                    <tr><td align="right">Date</td><td /><td><asp:TextBox ID="txtSearchDate" CssClass="SearchDate" runat="server" /></td></tr>
                                                    <tr><td colspan="2" /><td align="right"><input type="button" id="btnClear" value="Clear" /><asp:Button ID="btnSearch" Text="Search" runat="server" OnClick="UpdateList" /></td></tr>
                                                </table>
	                                        </div>
                                        </td>
                                    </tr>
    						        <tr>
								        <td align="left">
									        <table class="SelectFilter" cellspacing="0" cellpadding="0" border="0">
										        <tr>
											        <td>Sort By:</td>
										        </tr>
										        <tr>
											        <td><asp:radiobutton id="rbSortDate" runat="server" text="Date" autopostback="True" groupname="rbSortBy" OnCheckedChanged="UpdateList"></asp:radiobutton></td>
										        </tr>
										        <tr>
											        <td><asp:radiobutton id="rbSortName" runat="server" text="Name" autopostback="True" groupname="rbSortBy" OnCheckedChanged="UpdateList"></asp:radiobutton></td>
										        </tr>
									        </table>
								        </td>
								        <td><img height="1" src="Images/Space.gif" alt="" width="5" /></td>
								        <td align="right">
									        <table class="SelectFilter" cellspacing="0" cellpadding="0" border="0">
										        <tr>
											        <td>Include:</td>
                                                    <td><span id="MoreSearch" title="Show additional search fields">+</span></td>
										        </tr>
										        <tr>
											        <td><asp:checkbox id="chkPastJobs" runat="server" text="Past" autopostback="True" cssclass="SelectFilter" OnCheckedChanged="UpdateList" textalign="right"></asp:checkbox></td>
											        <td><asp:checkbox id="chkCancelledJobs" runat="server" text="Cancelled" autopostback="True" cssclass="SelectFilter" OnCheckedChanged="UpdateList" textalign="right"></asp:checkbox></td>
										        </tr>
										        <tr>
											        <td><asp:checkbox id="chkJobs" runat="server" text="Jobs" Checked="true" autopostback="True" cssclass="SelectFilter" OnCheckedChanged="UpdateList" textalign="right"></asp:checkbox></td>
											        <td><asp:checkbox id="chkProposals" runat="server" text="Proposals" autopostback="True" cssclass="SelectFilter" OnCheckedChanged="UpdateList" textalign="right"></asp:checkbox></td>
										        </tr>
									        </table>
								        </td>
							        </tr>
					                <tr>
						                <td colspan="3">
                                            <asp:TextBox ID="txtSearch" CssClass="Search" runat="server" />
						                </td>
					                </tr>
							        <tr>
								        <td colspan="3"><asp:listbox id="lstList" runat="server" autopostback="false" cssclass="SelectJob"></asp:listbox></td>
							        </tr>
							        <tr>
								        <td colspan="3"><asp:label id="lblRecCount" runat="server" cssclass="SelectCount">RecCount</asp:label></td>
							        </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
					</td>
					<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
					<td style="position: relative;">
                        <%= RecentJobs.Html() %>
						<table cellspacing="0" cellpadding="0" align="center" border="0" class="EditData">
							<tr>
								<td>
									<table cellspacing="0" cellpadding="0" border="0">
                                        <tr id="trMsg" runat="server">
                                            <td colspan="5"><asp:Label ID="lblMsg" runat="server" /></td>
                                        </tr>
										<tr>
											<td class="FeatureMain" style="padding-bottom: 0px;" colspan="4"><asp:HyperLink id="lnkPrev" Text="<" CssClass="PrevNext" runat="server" />&nbsp;&nbsp;<%= (fProposal ? "Proposal" : "Job") %>&nbsp;&nbsp;<asp:HyperLink id="lnkNext" Text=">"  CssClass="PrevNext" runat="server" /></td>
										</tr>
										<tr>
											<td class="FeatureSub" style="padding-bottom: 10px;" colspan="4" align="center"><asp:Label ID="lblJobDesc" runat="server" /></td>
										</tr>
                                        <tr>
                                            <td>
                                                <table cellspacing="0" cellpadding="0" border="0">
										            <tr>
											            <td><img height="1" src="Images/Space.gif" alt="" width="100" /></td>
											            <td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
                                                    </tr>
										            <tr>
											            <td align="right">Customer</td>
											            <td></td>
											            <td><asp:textbox id="txtCustomer" runat="server" cssclass="MaintName" maxlength="50" placeholder="Optional" /><asp:image id="ddCustomer" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image><asp:HiddenField ID="hfCustomerRno" runat="server" /><asp:HiddenField ID="hfCustTaxExempt" runat="server" /><asp:HiddenField ID="hfCustCompany" runat="server" /><asp:HiddenField ID="hfCustOnlineOrders" runat="server" /><asp:HiddenField ID="hfCustEmailConfirmations" runat="server" />
                                                            <i id="lnkAddCustomer" class="Add far fa-plus-circle" title="Add a new Customer"></i>
											            </td>
										            </tr>
										            <tr>
											            <td align="right">Contact</td>
											            <td></td>
											            <td><asp:textbox id="txtContact" runat="server" cssclass="MaintName" maxlength="50"/><asp:image id="ddContact" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image><asp:HiddenField ID="hfContactRno" runat="server" />
                                                            <i id="lnkAddContact" class="Add far fa-plus-circle" title="Add a new Contact"></i>
											            </td>
										            </tr>
										            <tr>
											            <td align="right"></td>
											            <td></td>
											            <td>
												            <table cellspacing="0" cellpadding="0" border="0">
													            <tr>
														            <td><img height="1" src="Images/Space.gif" alt="" width="35" /></td>
														            <td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
													            </tr>
													            <tr>
														            <td align="right">Phone</td>
														            <td></td>
														            <td><asp:textbox id="txtPhone" runat="server" cssclass="JobPhone" maxlength="20"/></td>
													            </tr>
													            <tr>
														            <td align="right">Cell</td>
														            <td></td>
														            <td><asp:textbox id="txtCell" runat="server" cssclass="JobPhone" maxlength="20"/></td>
													            </tr>

            <%--													<tr>
														            <td align="right">Fax</td>
														            <td></td>
														            <td><asp:textbox id="txtFax" runat="server" cssclass="JobPhone" maxlength="20"/></td>
													            </tr>
            --%>
													            <tr>
														            <td align="right">Email</td>
														            <td></td>
														            <td><asp:textbox id="txtEmail" runat="server" cssclass="JobEmail" maxlength="80"/></td>
													            </tr>
												            </table>
                                                            <asp:HiddenField ID="hfFax" runat="server" />
                                                            <asp:HiddenField ID="hfTitle" runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
											</td>
                                            <td />
                                            <td valign="top">
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="1" src="Images/Space.gif" alt="" width="100" /></td>
														<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
													</tr>
													<tr>
														<td align="right">Job #</td>
														<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
														<td>
                                                            <asp:textbox id="txtJobRno" runat="server" cssclass="MaintRno" Enabled="false"/>
                                                            <img id="imgSearch" src="Images/Search.png" alt="Search" style="vertical-align: text-bottom; cursor: pointer;" title="Find Job #" />
														</td>
													</tr>
													<tr>
														<td align="right">Status</td>
														<td></td>
														<td><asp:textbox id="txtStatus" runat="server" cssclass="MaintShort" enabled="False" /></td>
													</tr>
													<tr>
														<td align="right">Job Type</td>
														<td></td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlJobType" runat="server" Width="100">
                                                                <asp:ListItem Text="" Value="" />
                                                                <asp:ListItem Text="Private" Value="Private" />
                                                                <asp:ListItem Text="Corporate" Value="Corporate" />
                                                            </asp:DropDownList>
                                                        </td>
													</tr>
													<tr id="PmtTerms">
														<td align="right">Payment Terms</td>
														<td></td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlPmtTerms" runat="server" Width="100">
                                                                <asp:ListItem Text="" Value="" />
                                                                <asp:ListItem Text="Net 30" />
                                                                <asp:ListItem Text="Net 15" />
                                                                <asp:ListItem Text="Net 10" />
                                                                <asp:ListItem Text="On Delivery" />
                                                            </asp:DropDownList>
                                                        </td>
													</tr>
													<tr>
														<td align="right"></td>
														<td></td>
                                                        <td>
                                                            <asp:CheckBox ID="chkProposal" Text="Proposal" runat="server" />
                                                        </td>
													</tr>
                                                </table>
                                            </td>
										</tr>
                                        <tr>
                                            <td colspan="3">
                                                <table cellspacing="0" cellpadding="0" border="0">
                                                    <tbody>
										                <tr>
											                <td><img height="1" src="Images/Space.gif" alt="" width="107" /></td>
											                <td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
                                                        </tr>
										                <tr>
											                <td align="right">Responsible Party</td>
											                <td></td>
											                <td colspan="2"><asp:textbox id="txtResponsibleParty" runat="server" cssclass="MaintName" maxlength="50"/></td>
										                </tr>
										                <tr>
											                <td align="right">Event Type</td>
											                <td></td>
											                <td colspan="2"><asp:textbox id="txtEventType" runat="server" cssclass="EventType" maxlength="50"/><asp:image id="ddEventType" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image><asp:textbox id="txtEventTypeDetails" runat="server" cssclass="EventTypeDetails" maxlength="200" placeholder="Details" /></td>
										                </tr>
										                <tr>
											                <td align="right">Job Description</td>
											                <td></td>
											                <td colspan="2"><asp:textbox id="txtJobDesc" runat="server" cssclass="JobDesc" maxlength="80"/></td>
										                </tr>
                                                    </tbody>
                                                    <tbody class="ceremony">
										                <tr>
											                <td align="right">Ceremony</td>
											                <td></td>
											                <td><asp:RadioButton ID="rbCeremonyYes" Text="Yes" GroupName="rbCeremony" runat="server" /><asp:RadioButton ID="rbCeremonyNo" Text="No" GroupName="rbCeremony" runat="server" /></td>
										                </tr>
                                                        <tr>
											                <td align="right">Time</td>
											                <td></td>
											                <td><asp:textbox id="txtWeddingTime" runat="server" cssclass="JobTime"/></td>
                                                        </tr>
										                <tr>
											                <td align="right">Location</td>
											                <td></td>
											                <td><asp:textbox id="txtCeremonyLocation" runat="server" cssclass="MaintName" maxlength="80"/></td>
										                </tr>
                                                    </tbody>
                                                    <tbody>
										                <tr>
											                <td align="right">Service Type</td>
											                <td></td>
											                <td><asp:textbox id="txtServiceType" runat="server" cssclass="MaintName" maxlength="50"/><asp:image id="ddServiceType" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image></td>
										                </tr>
										                <tr>
											                <td align="right">Disposable</td>
											                <td></td>
											                <td><asp:RadioButton ID="rbDisposableYes" Text="Yes" GroupName="rbDisposable" runat="server" /><asp:RadioButton ID="rbDisposableNo" Text="No" GroupName="rbDisposable" runat="server" /></td>
										                </tr>
										                <tr>
											                <td align="right">Buffets</td>
											                <td></td>
											                <td colspan="2"><asp:textbox id="txtNumBuffets" runat="server" cssclass="NumBuffets" maxlength="5" placeholder="#"/><asp:textbox id="txtBuffetSpace" runat="server" cssclass="BuffetSpace" maxlength="200" placeholder="Buffet Space"/></td>
										                </tr>
										                <tr>
											                <td align="right">Location</td>
											                <td></td>
											                <td><asp:textbox id="txtLocation" runat="server" cssclass="MaintName" maxlength="80"/></td>
										                </tr>
										                <tr>
											                <td align="right">Booked by</td>
											                <td></td>
											                <td><asp:textbox id="txtBookedBy" runat="server" cssclass="MaintName" maxlength="50"/></td>
										                </tr>
										                <tr>
											                <td align="right"><label for="chkConfirmed">Confirmed</label></td>
											                <td></td>
											                <td><asp:CheckBox id="chkConfirmed" runat="server" /></td>
										                </tr>
<%--										                <tr>
											                <td align="right">Conf Deadline</td>
											                <td></td>
											                <td><asp:textbox id="txtConfirmDeadlineDate" runat="server" cssclass="JobDate"/></td>
										                </tr>
										                <tr>
											                <td align="right">Confirmed by</td>
											                <td></td>
											                <td><asp:textbox id="txtConfirmedBy" runat="server" cssclass="MaintName" maxlength="50"/></td>
										                </tr>--%>
                <%--
    										                <tr>
											                <td align="right">Directions</td>
											                <td></td>
											                <td rowspan="2"><asp:textbox id="txtDirections" runat="server" cssclass="JobDir" maxlength="1024" textmode="MultiLine"/></td>
											                <td></td>
											                <td><asp:checkbox id="chkPrintDir" runat="server" text="Print on Job sheet"></asp:checkbox></td>
										                </tr>
										                <tr>
											                <td>&nbsp;</td>
											                <td></td>
											                <td></td>
											                <td>&nbsp;</td>
										                </tr>
                --%>
                <%--										<tr>
											                <td align="right">Vehicle</td>
											                <td></td>
											                <td><asp:textbox id="txtVehicle" runat="server" cssclass="MaintName" maxlength="50"/><asp:image id="ddVehicle" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif" Visible="false"></asp:image></td>
										                </tr>
										                <tr>
											                <td align="right">Carts</td>
											                <td></td>
											                <td><asp:textbox id="txtCarts" runat="server" cssclass="MaintName" maxlength="50"/><!--<asp:image id="ddCarts" runat="server" imagealign="AbsMiddle" imageurl="Images/DropDown.gif"></asp:image>--></td>
										                </tr>
                --%>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
									</table>
								</td>
							</tr>
							<tr>
								<td>
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="1" src="Images/Space.gif" alt="" width="107" /></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
											<td></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
											<td></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
											<td></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
											<td></td>
											<td><img height="1" src="Images/Space.gif" alt="" width="10" /></td>
										</tr>
										<tr>
											<td align="right">Job Date</td>
											<td></td>
											<td colspan="3">
												<table cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr>
														<td class="tdJobDate"><asp:textbox id="txtJobDate" runat="server" cssclass="JobDate"/><asp:image id="imgJobDate" runat="server" imagealign="AbsMiddle" imageurl="Images/CalendarIcon.gif" Visible="false"></asp:image></td>
														<td align="right">Day</td>
													</tr>
												</table>
											</td>
											<td></td>
											<td><asp:textbox id="txtJobDayOfWeek" runat="server" cssclass="JobDate" enabled="False"/></td>
										</tr>

										<tr>
											<td align="right">Times:&nbsp;&nbsp;&nbsp;Meal</td>
											<td></td>
											<td><asp:textbox id="txtMealTime" runat="server" cssclass="JobTime"/></td>
											<td></td>
											<td align="right">End</td>
											<td></td>
											<td><asp:textbox id="txtEndTime" runat="server" cssclass="JobTime"/></td>
                                        </tr>
                                        <tr>
											<td align="right">Guest Arrival</td>
											<td></td>
											<td><asp:textbox id="txtGuestArrival" runat="server" cssclass="JobTime"/></td>
										</tr>
										<tr>
											<td align="right"><%= Globals.g.Company.Initials %> Arrival</td>
											<td></td>
											<td><asp:textbox id="txtArrivalTime" runat="server" cssclass="JobTime"/></td>
											<td></td>
											<td align="right">Load</td>
											<td></td>
											<td><asp:textbox id="txtLoadTime" runat="server" cssclass="JobTime"/></td>
											<td></td>
											<td align="right">Depart</td>
											<td></td>
											<td><asp:textbox id="txtDepartTime" runat="server" cssclass="JobTime"/></td>
										</tr>
										<tr>
											<%--<td align="right"># Servings: Men</td>--%>
											<td align="right"># Servings</td>
											<td></td>
											<td colspan="9"><asp:textbox id="txtServingMen" runat="server" cssclass="JobServings"/><asp:textbox id="txtDemographics" runat="server" cssclass="Demographics" maxlength="1024" placeholder="Demographics"/></td>
<%--
											<td></td>
											<td align="right">Women</td>
											<td></td>
											<td><asp:textbox id="txtServingWomen" runat="server" cssclass="JobServings"/></td>
											<td></td>
											<td align="right">Children</td>
											<td></td>
											<td><asp:textbox id="txtServingChild" runat="server" cssclass="JobServings"/></td>
--%>
										</tr>
										<tr>
											<td align="right">Price per Person</td>
											<td></td>
											<td><asp:textbox id="txtPricePerPerson" runat="server" cssclass="JobPrice"/></td>
											<td></td>
<%--
											<td align="right">Servings</td>
											<td></td>
											<td><asp:textbox id="txtServingTotal" runat="server" cssclass="JobServings" enabled="False"/><asp:HiddenField ID="hfServingTotal" runat="server" /></td>
											<td></td>
--%>
											<td align="right">SubTotal</td>
											<td></td>
											<td><asp:textbox id="txtSubTotal" runat="server" cssclass="JobSubTotal" enabled="False"/></td>
										</tr>
										<tr>
											<td align="right">Payment Type</td>
											<td></td>
											<td colspan="9"><asp:radiobuttonlist id="rbPaymentType" runat="server" repeatdirection="Horizontal">
													<asp:listitem value="Check" selected="True">Check</asp:listitem>
													<asp:listitem value="Cash">Cash</asp:listitem>
													<asp:listitem value="CC">CC</asp:listitem>
													<asp:listitem value="Other">Other</asp:listitem>
												</asp:radiobuttonlist></td>
										</tr>
										<tr>
											<td align="right">Menu Type</td>
											<td></td>
											<td colspan="1"><asp:DropDownList ID="ddlMenuType" runat="server">
                                                <asp:ListItem Text="" />
                                                <asp:ListItem Text="Monthly Special" />
                                                <asp:ListItem Text="Lunch" />
                                                <asp:ListItem Text="Dinner" />
                                                <asp:ListItem Text="Delivery/Pick up" />
                                                <asp:ListItem Text="Corporate" />
                                                <asp:ListItem Text="Breakfast" />
                                                <asp:ListItem Text="BBQ" />
                                                <asp:ListItem Text="Summer" />
                                                <asp:ListItem Text="Large Pickup" />
                                                <asp:ListItem Text="Appetizer" />
                                                <asp:ListItem Text="Spring/Summer Reception" />
                                                <asp:ListItem Text="Fall/Winter Reception" />
                                                <asp:ListItem Text="Large Reception" />
                                                <asp:ListItem Text="Small Celebration" />
                                                <asp:ListItem Text="Beverage Station" />
                                                <asp:ListItem Text="Dessert" />
                                                <asp:ListItem Text="Savory Station" />
                                                <asp:ListItem Text="Sweet Station" />
                                                <asp:ListItem Text="Holiday" />
											</asp:DropDownList></td>
										</tr>
										<tr>
											<td align="right">PO #</td>
											<td></td>
											<td colspan="1"><asp:TextBox ID="txtPONumber" MaxLength="50" CssClass="MaintName" runat="server" /></td>
										</tr>
										<tr>
											<td><img height="15" src="Images/Space.gif" alt="" width="1" /></td>
										</tr>
										<tr>
											<td valign="top" align="right">Job Notes</td>
											<td></td>
											<td colspan="9"><asp:textbox id="txtJobNotes" runat="server" cssclass="JobNotes" maxlength="4096" textmode="MultiLine"/></td>
										</tr>
                                        <tr>
                                            <td align="right">Crew</td>
                                            <td></td>
                                            <td colspan="9"><asp:TextBox ID="txtCrew" CssClass="Crew" MaxLength="200" runat="server" /></td>
                                        </tr>
										<tr>
											<td valign="top" align="right">Prod Notes</td>
											<td></td>
											<td colspan="9"><asp:textbox id="txtProdNotes" runat="server" cssclass="JobNotes" maxlength="4096" textmode="MultiLine"/></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
<%--
    					<table style="MARGIN-TOP: 10px" cellspacing="0" cellpadding="0" width="500" align="center"
							border="0">
							<tr>
								<td align="center"><asp:checkbox id="chkCompleted" runat="server" text="Completed"></asp:checkbox></td>
								<td align="center"><asp:checkbox id="chkConfirmed" runat="server" text="Confirmed"></asp:checkbox></td>
								<td align="center"><asp:checkbox id="chkPrinted" runat="server" text="Printed"></asp:checkbox></td>
								<td align="center"><asp:checkbox id="chkInvoiced" runat="server" text="Invoiced"></asp:checkbox></td>
								<td align="center"><asp:checkbox id="chkCancelled" runat="server" text="Cancelled"></asp:checkbox></td>
							</tr>
						</table>
--%>
						<table style="MARGIN-TOP: 10px; MARGIN-BOTTOM: 20px" cellspacing="0" cellpadding="0" width="300"
							align="center" border="0">
							<tr>
								<td align="center"><input id="btnPreSave" type="button" value="Save" /><asp:button id="btnSave" runat="server" text="Save"  OnClick="btnSave_Click"></asp:button></td>
								<td align="center"><asp:button id="btnNew" runat="server" text="New" OnClick="btnNew_Click"></asp:button></td>
								<td align="center"><input id="btnPrint" onclick="Print();" type="button" value="Print" /></td>
								<td align="center"><asp:button id="btnCopy" runat="server" text="Copy" OnClick="btnCopy_Click"></asp:button></td>
								<td align="center"><input id="btnPreDelete" type="button" value="Delete" /><asp:button id="btnDelete" runat="server" text="Delete" OnClick="btnDelete_Click"></asp:button></td>
							</tr>
						</table>
						<table cellspacing="0" cellpadding="0" align="center" border="0">
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
								<td align="right">Entered</td>
								<td></td>
								<td><asp:textbox id="txtEntDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
								<td></td>
								<td><asp:textbox id="txtEntUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
								<td></td>
								<td align="right">Updated</td>
								<td></td>
								<td><asp:textbox id="txtUpdDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
								<td></td>
								<td><asp:textbox id="txtUpdUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
							</tr>
<%--
							<tr>
								<td align="right">Completed</td>
								<td></td>
								<td><asp:textbox id="txtCompDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
								<td></td>
								<td><asp:textbox id="txtCompUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
								<td></td>
								<td align="right">Confirmed</td>
								<td></td>
								<td><asp:textbox id="txtConfDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
								<td></td>
								<td><asp:textbox id="txtConfUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
							</tr>
--%>
							<tr>
								<td align="right">Printed</td>
								<td></td>
								<td><asp:textbox id="txtPrintDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
								<td></td>
								<td><asp:textbox id="txtPrintUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
								<td></td>
								<td align="right">Invoiced</td>
								<td></td>
								<td><asp:textbox id="txtInvDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
								<td></td>
								<td><asp:textbox id="txtInvUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
							</tr>
							<tr>
								<td align="right">Conf Emailed</td>
								<td></td>
						        <td><asp:textbox id="txtConfirmEmailedDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
						        <td></td>
						        <td><asp:textbox id="txtConfirmEmailedUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
								<td></td>
								<td align="right">Final Email</td>
								<td></td>
						        <td><asp:textbox id="txtFinalConfirmEmailedDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
						        <td></td>
						        <td><asp:textbox id="txtFinalConfirmEmailedUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
							</tr>
							<tr>
								<td align="right"></td>
								<td></td>
								<td></td>
						        <td></td>
								<td></td>
								<td></td>
								<td align="right">Confirmed</td>
								<td></td>
						        <td><asp:textbox id="txtConfirmedDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
						        <td></td>
						        <td><asp:textbox id="txtConfirmedUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
							</tr>
<%--
							<tr>
								<td align="right">Cancelled</td>
								<td></td>
								<td><asp:textbox id="txtCanDt" runat="server" cssclass="MaintDate" enabled="False"/></td>
								<td></td>
								<td><asp:textbox id="txtCanUser" runat="server" cssclass="MaintUser" enabled="False"/></td>
							</tr>
--%>
						</table>
					</td>
				</tr>
			</table>
            <br />
			<input id="txtDirty" type="hidden" value="false" name="txtDirty" runat="server" />
            <asp:HiddenField ID="hfOrigStartDateTime" runat="server" />
			<% Pg.Bottom(); %>
			<input id="txtInvCreatedDt" type="hidden" runat="server" />
		</form>
		<%//=Utils.SelectList.DefValues()%>
		<%//=calJobDate.DefCalendar()%>

        <div id="AddCustomer" title="Add Customer">
            <dl>
                <dt>Name</dt>
                <dd><input type="text" id="txtAddCustomerName" maxlength="50" /></dd>
                <dt></dt>
                <dd><input type="checkbox" id="chkAddCustomerCompany" /><label for="chkAddCustomerCompany">Company</label></dd>
                <dt></dt>
                <dd><input type="checkbox" id="chkAddCustomerTaxExempt" /><label for="chkAddCustomerTaxExempt">Tax Exempt</label></dd>
                <dt></dt>
                <dd><input type="checkbox" id="chkAddCustomerOnlineOrders" /><label for="chkAddCustomerOnlineOrders">Online Orders</label></dd>
                <dt></dt>
                <dd><input type="checkbox" id="chkAddCustomerEmailConfirmations" /><label for="chkAddCustomerEmailConfirmations">Email Confirmations</label></dd>
            </dl>
        </div>

        <div id="AddContact" title="Add Contact">
            <dl>
                <dt>Customer</dt>
                <dd><span id="spnCustomer"/></dd>
                <dt>Name</dt>
                <dd><input type="text" id="txtAddContactName" maxlength="50" /></dd>
                <dt>Email</dt>
                <dd><input type="text" id="txtAddContactEmail" maxlength="80" /></dd>
                <dt>Phone</dt>
                <dd><input type="text" id="txtAddContactPhone" maxlength="20" /></dd>
                <dt>Cell</dt>
                <dd><input type="text" id="txtAddContactCell" maxlength="20" /></dd>
                <dt>Fax</dt>
                <dd><input type="text" id="txtAddContactFax" maxlength="20" /></dd>
                <dt>Title</dt>
                <dd><input type="text" id="txtAddContactTitle" maxlength="30" /></dd>
            </dl>
        </div>

        <div id="FindJob" title="Find Job #">
            Job # <input type="text" id="JobRno" />
        </div>

	</body>
</html>
