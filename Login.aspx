<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect_201128S.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <link rel="stylesheet" href="~/css/login.css" />

    <script src="https://www.google.com/recaptcha/api.js"></script>
</head>
<body>
    <ul>
        <a class="navbar-brand" style="float:left" href=""><strong><span>SIT</span>Connect</strong></a>
        <li><a href="/Registration.aspx">Register</a></li>
        <li><a href="/Login.aspx">Login</a></li>
        <li><a href="#contact">Contact</a></li>
        <li><a href="#about">About</a></li>
        <li><a href="/Index.aspx">Home</a></li>
    </ul>
    <form id="form1" runat="server">
        <div id="container">
            <div id="inner-container">
                <div id="row">
                    <div>
                        <asp:Label ID="email" runat="server" Text="Email"></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox ID="emailTB" runat="server" Width="250px"></asp:TextBox>
                    </div>
                </div>

                <div id="row">
                    <div>
                        <asp:Label ID="password" runat="server" Text="Password"></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox ID="passwordTB" runat="server" TextMode="Password" Width="250px"></asp:TextBox>
                    </div>
                </div>
                <div class="g-recaptcha" data-sitekey="6Lf1rEoeAAAAAAuTVJUdFm_tchrRwRhaK71drzna"></div>
                <asp:Label ID="error" runat="server" ForeColor="Red"></asp:Label>
                <br />
 
                <div id="login">
                    <asp:Button ID="loginBtn" runat="server" Text="Login" OnClick="loginBtn_Click"/>
                </div>
            </div>

        </div>
    </form>
</body>
</html>
