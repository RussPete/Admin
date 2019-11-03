<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerConfirmation.aspx.cs" Inherits="CustomerConfirmation" %>

<!doctype html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <!-- Required meta tags -->
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title><%= Globals.g.Company.Name %> Customer Confirmation</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" integrity="sha384-MCw98/SFnGE8fJT3GXwEOngsV7Zt27NXFoaoApmYm81iuXoPkFOJwJ8ERdknLPMO" crossorigin="anonymous" />
    <style type="text/css">
        .Top
        {
        }
        .Above-Bar, In-Bar, .Below-Bar
        {
            /*width: 900px;
            display: block;
            margin: 0px auto;*/
        }
        .Above-Bar
        {
            position: relative;
            height: 15px;
        }
        .Ribbon
        {
            position: absolute;
            top: 5px;
            left: 65px;
            z-index: 10;
            width: 205px;
            height: 397px;
            background: <%= Globals.g.Company.CustomerComfirmationLogoBackground %>;
            cursor: pointer;
        }
        .Ribbon-Email
        {
            position: absolute;
            top: 282px;
            left: 14px;
            z-index: 100;
            font-size: 10pt;
            font-family: 'Joseph Sans', sans-serif;
        }
        .Ribbon-Email a
        {
            color: #989e59;
        }
        .Bar
        {
            position: relative;
            top: 0px;
            height: 42px;
            background: #989E59;
        }
        .In-Bar
        {
            color: white;
            position: absolute;
            left: 340px;
            font-size: x-large;
        }
        .Below-Bar
        {
            height: 320px;
        }
        .Instructions 
        {
            margin: 50px 0px;
        }
        .Main-Page
        {
            margin: 50px;
        }
        .Menu, .Prices, .Fees, .Total, .Tables
        {
            display: none;
        }
        .prompt
        {
            text-align: center;
        }
        .card
        {
            margin-top: 40px;
            margin-bottom: 15px;
        }
        .card-header
        {
            background-color: rgba(152, 158, 89, 0.19)
        }
        .card-title
        {
            margin-top: 1.20rem;
        }
        .card-title:first-child
        {
            margin-top: 0px;
        }
        .row
        {
            margin-bottom: 10px;
        }
        .Buffet
        {
            text-align: right;
        }
        .Buffet div
        {
            margin: 5px 0px;
        }
        .Buffet select, .Buffet input 
        {
            width: 175px;
        }
        .Buffet input 
        {
            padding: 0px 2px;
        }
        .Buffet .GuestTables
        {
            margin-top: 20px;
        }
        .GuestTables div div
        {
            margin: 0px;
            text-align: center;
        }
        .GuestTables input
        {
            border-width: 0px;
            border-bottom-width: 1px;
            border-bottom-style: solid;
            border-bottom-color: #AAA;
        }
        .TableSizeShape
        {
            width: 150px;
            display: inline-block;
        }
        .TableSizeShape input
        {
            width: 130px;
        }
        .TableSizeShape input, .TableCount input 
        {
            text-align: center;
        }
        .TableCount
        {
            width: 70px;
            display: inline-block;
        }
        .TableCount input 
        {
            width: 60px;
        }
        .Notes
        {
            display: none;
        }
        .Notes textarea
        {
            min-height: 80px;
        }
        .ColorRequired
        {
            font-size: small; 
            width: 360px; 
            margin-top: -2px;
        }
        .primary
        {
            color: #007bff;
        }
        .success
        {
            color: #28a745;
        }
        .danger
        {
            color: #dc3545;
        }
    </style>
</head>
<body>
    <div class="Top">
        <div class="Above-Bar">
            <div class="Ribbon">
                <div class="Ribbon-Email">
                    <a href="mailto:Info (<%= Globals.g.Company.InfoEmail %>)"><%= Globals.g.Company.InfoEmail %></a>
                </div>            
            </div>
        </div>
        <div class="Bar">
            <div class="In-Bar"><%= Globals.g.Company.Name %> Event Confirmation </div>
        </div>
        <div class="Below-Bar container">
            <div class="row">
                <div class="col-6 col-sm-5 col-md-4 col-lg-3"></div>
                <div class="Instructions col-6 col-sm-7 col-md-8 col-lg-9">
                    <asp:Label ID="lblInstructions" runat="server" />
                </div>
            </div>
        </div>
    </div>
    <div class="container Main-Page">
        <form id="form1" runat="server">
            <asp:Label ID="lblJob" runat="server" />
        </form>
    </div>
    <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js" integrity="sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js" integrity="sha384-ChfqqxuZUCnJSK3+MXmPNIyE6ZbWh2IMqE241rYiqJxyMiZ6OW/JmZQ5stwEULTy" crossorigin="anonymous"></script>
	<script type="text/javascript">
        $(document).ready(function ()
        {
            $(".Ribbon").click(function ()
            {
                window.location.href = Globals.g.Company.WebsiteUrl;
            });

            $(".Yes").on("click", function ()
            {
                var parent = $(this).parents(".Confirm");
                // hide notes
                parent.find(".Notes").hide();
                // show the finish button if in this card
                parent.find(".Notes.Finish").show();
                // show the next card and bring it into view
                var next = parent.parent().next();
                next.show().get(0).scrollIntoView();

                // if the finish button is in the next section
                if (next.find(".Notes.Finish").length > 0)
                {
                    // show the notes and the finish button
                    next.find(".Notes").show();
                }
            });

            $(".No").on("click", function ()
            {
                var parent = $(this).parents(".Confirm");
                // show the notes and next button
                parent.find(".Notes.Prompt, .Notes.Text").show().get(0).scrollIntoView();

                var next = parent.parent().next();
                // if the next section is not visible, then show the next button
                if (next.filter(":visible").length == 0)
                {
                    parent.find(".Notes.Next, .Notes.Finish").show();
                }
            });

            $(".btn.Next").on("click", function ()
            {
                $(this).hide();
                var next = $(this).parents(".Confirm").parent().next();
                next.show().get(0).scrollIntoView();

                // if the finish button is in the next section
                if (next.find(".Notes.Finish").length > 0)
                {
                    // show the notes and the finish button
                    next.find(".Notes").show();
                }
            });

            $("#ddlLinensProvidedBy").change(function ()
            {
                if ($(this).val() == "<%= Globals.g.Company.Name %>")
                {
                    $(".GuestTables").show();
                }
                else
                {
                    $(".GuestTables").hide();
                }
            });

            $(".Finish").click(function ()
            {
                var rc = true;

                if ($("#txtGuestTableColor").val().trim().length == 0)
                {
                    rc = false;
                    $("#lblFinish").text("The Linen color for guest tables is required, even if you provide them. <%= Globals.g.Company.Name %> will use complementary colors.");
                }
                else if ($("#ddlLinensProvidedBy").val() == "<%= Globals.g.Company.Name %>" &&
                         $("#txtTableSizeShape1").val().length == 0 && $("#txtTableCount1").val().length == 0 &&
                         $("#txtTableSizeShape2").val().length == 0 && $("#txtTableCount2").val().length == 0 &&
                         $("#txtTableSizeShape3").val().length == 0 && $("#txtTableCount3").val().length == 0 &&
                         $("#txtTableSizeShape4").val().length == 0 && $("#txtTableCount4").val().length == 0)
                {
                    rc = false;
                    $("#lblFinish").text("Please enter the size, shape and count of the tables for which <%= Globals.g.Company.Name %> will provide linens.");
                }

                return rc;
            });
        });

	</script>

</body>
</html>
