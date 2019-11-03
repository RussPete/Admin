<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Availability.aspx.cs" Inherits="Availability" Async="true" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.12.1/jquery-ui.min.js"></script>    
    <script src="Sling.js" type="text/javascript"></script>
	<script src="js/jquery.ui.timepicker.js" type="text/javascript"></script>
    <script src="Availability.js" type="text/javascript"></script>
    <!--link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.12.1/themes/smoothness/jquery-ui.css"-->
	<link href="css/ui-lightness/jquery-ui-1.8.18.custom.css" type="text/css" rel="stylesheet" />
    <style>
        #Prompt, #Page 
        {
            margin: 20px;
        }
        #Prompt, #txtDate, #Page 
        {
            font-family: sans-serif;
        }
        h1
        {
            text-align: center;
            margin-bottom: 10px;
            font-size: 12pt;
        }
        table
        {
            width: 100%;
            margin-bottom: 40px;
            border-collapse: collapse;
        }
        th, td
        {
            width: 12.5%;
            text-align: center;
            padding: 5px 0px;
            font-size: 8pt;
        }
        table, td, th
        {
            border: 1px solid #BBB;
        }
        th
        {
            border-bottom-color: black;
        }
        th:first-child, td:first-child
        {
            border-right-color: black;
        }
        #txtDate
        {
            width: 70px;
            text-align: center;
        }
        .ui-datepicker-trigger
        {
            vertical-align: bottom;
        }
        @media print 
        {
            #Prompt
            {
                display: none;
            }
            table
            {
                page-break-after: always;
            }
            table:last-child
            {
                page-break-after: initial;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="Prompt"><asp:TextBox ID="txtDate" runat="server" OnTextChanged="txtDate_TextChanged" AutoPostBack="true" /> Times Unavailable</div>
    <div id="Page">
        <asp:PlaceHolder ID="phSchedule" runat="server" />
    </div>
    </form>
</body>
</html>
