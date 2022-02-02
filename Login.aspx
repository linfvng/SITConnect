<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect_201128S.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <link rel="stylesheet" href="~/css/login.css" />

    <script src="https://www.google.com/recaptcha/api.js?render=6Lf291MeAAAAAAH_KnOISmhrEQVGiDb1EiquKySU"></script>
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
                <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                <asp:Label ID="error" runat="server" ForeColor="Red"></asp:Label>
                <br />
 
                <div id="login">
                    <asp:Button ID="loginBtn" runat="server" Text="Login" OnClick="loginBtn_Click"/>
                </div>
            </div>

        </div>
    </form>

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6Lf291MeAAAAAAH_KnOISmhrEQVGiDb1EiquKySU', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
