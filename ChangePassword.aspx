<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="SITConnect_201128S.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration Form</title>
     <link rel="stylesheet" href="~/css/register.css" />

    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=nPasswordTB.ClientID %>').value;

            if (str.length < 8) {
                document.getElementById("pwdchecker").style.visibility = "visible";
                document.getElementById("pwdchecker").innerHTML = "Password Length Must be at least 8 Character";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("too_short");
            }

            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("pwdchecker").style.visibility = "visible";
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 number";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("no_number");
            }

            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("pwdchecker").style.visibility = "visible";
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 uppercase";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("no_uppercase");
            }

            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("pwdchecker").style.visibility = "visible";
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 lowercase";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("no_lowercase");
            }

            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("pwdchecker").style.visibility = "visible";
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 special characters";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("no_special_character");
            }

            document.getElementById("pwdchecker").style.visibility = "visible";
            document.getElementById("pwdchecker").innerHTML = "Excellent!";
            document.getElementById("pwdchecker").style.color = "Green";
        }
    </script>
</head>
<body>
    <ul>
        <a class="navbar-brand" style="float:left" href=""><strong><span>SIT</span>Connect</strong></a>
        <li><a><asp:Button ID="logout" runat="server" Text="Logout" OnClick="Logout" /></a></li>
        <li><a href="/ChangePassword.aspx">Change Password</a></li>
        <li><a href="/Homepage.aspx">Home</a></li>
    </ul>

    <form id="form1" runat="server">
        <div id="container">
            <div id="inner-container">
                <h2 id="title">Change Password</h2>
                <div id="row" class="form-group">
                    <asp:Label ID="cPassword" runat="server" Text="Current Password"></asp:Label>
                    <br />
                    <asp:TextBox ID="cPasswordTB" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:Label ID="cPasswordError" runat="server" Text=""></asp:Label>
                    <br />
                </div>

                <div id="row">
                    <asp:Label ID="nPassword" runat="server" Text="New Password"></asp:Label>
                    <br />
                    <asp:TextBox ID="nPasswordTB" runat="server" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
                    <asp:Label ID="pwdchecker" runat="server" Text="vaildate"></asp:Label>
                </div>

                <div id="row">
                    <asp:Label ID="sPassword" runat="server" Text="Confirm Password"></asp:Label>
                    <br />
                    <asp:TextBox ID="sPasswordTB" runat="server" TextMode="Password"></asp:TextBox>
                </div>

                <div id="submit">
                    <asp:Button ID="SubmitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
