<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CCPayment.aspx.cs" Inherits="CCPayment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form action="https://sandbox.usaepay.com/interface/epayform/_aisi4iERwJa7aX8d29i8vLO2CLF2enl/">
    <input type="hidden" name="UMkey" value="_aisi4iERwJa7aX8d29i8vLO2CLF2enl">
    <input type="hidden" name="UMcommand" value="sale">
    <input type="hidden" name="UMamount" value="29.99">
    <input type="hidden" name="UMinvoice" value="12345">
    <input type="hidden" name="UMdescription" value="Mary & Joe">
    <input type="hidden" name="UMhash" value="#">
    <input type="submit" value="Continue to Payment Form">
    </form>
</body>
</html>
