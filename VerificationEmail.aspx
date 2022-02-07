<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerificationEmail.aspx.cs" Inherits="SITConnect_201128S.VerificationEmail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verification Email</title>
    <link rel="stylesheet" href="~/css/login.css" />
</head>
<body>
     <ul>
        <a class="navbar-brand" style="float:left" href=""><strong><span>SIT</span>Connect</strong></a>
    </ul>
    <form id="form1" runat="server">
        <div id="container">
            <div id="inner-container">
                <asp:Label ID="Label1" runat="server" Text="Enter the 6-digit One-time Password (OTP) sent to your email"></asp:Label>
                <div id="row">
                    <div>
                        <asp:Label ID="otp" runat="server" Text="Enter your OTP"></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox ID="otpTB" runat="server" Width="250px"></asp:TextBox>
                    </div>
                </div>
            
                <asp:Label ID="error" runat="server" ForeColor="Red"></asp:Label>
                <br />
 
                <div id="verify">
                    <asp:Button ID="loginBtn" runat="server" Text="Send" OnClick="verifyBtn_Click"/>
                </div>
            </div>

        </div>
    </form>
</body>
</html>
