<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="SITConnect_201128S.Homepage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect</title>
    <link rel="stylesheet" href="~/css/homepage.css" />
</head>
<body>
    <form id="form1" runat="server">
        <ul>
            <a class="navbar-brand" style="float:left" href=""><strong><span>SIT</span>Connect</strong></a>
            <li><a><asp:Button ID="logout" runat="server" Text="Logout" OnClick="Logout" /></a></li>
            <li><a href="#contact">Contact</a></li>
            <li><a href="#about">About</a></li>
            <li><a href="/Homepage.aspx">Home</a></li>
        </ul>
    </form>
    
</body>
</html>
