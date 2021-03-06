<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="SITConnect_201128S.Homepage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect</title>
    <link rel="stylesheet" href="~/css/homepage.css" />
</head>
<style>
    .intro {
      position: absolute;
      top: 20%;
      left: 50%;
      transform: translate(-50%, -50%);
      text-align: center;
    }
</style>
<body>
    <form id="form1" runat="server">
        <ul>
            <a class="navbar-brand" style="float:left" href=""><strong><span>SIT</span>Connect</strong></a>
            <li><a><asp:Button ID="logout" runat="server" Text="Logout" OnClick="Logout" /></a></li>
            <li><a href="/ChangePassword.aspx">Change Password</a></li>
            <li><a href="/Homepage.aspx">Home</a></li>
        </ul>
    </form>

    <div class="intro">
        <h1>Welcome <asp:Label ID="fullname" runat="server" Text="Label"></asp:Label></h1>
    </div>

    
</body>
</html>
