<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect_201128S.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration Form</title>
     <link rel="stylesheet" href="~/css/register.css" />

    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=passwordTB.ClientID %>').value;

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
        <li><a href="/Registration.aspx">Register</a></li>
        <li><a href="/Login.aspx">Login</a></li>
        <li><a href="#contact">Contact</a></li>
        <li><a href="#about">About</a></li>
        <li><a href="/Index.aspx">Home</a></li>
    </ul>

    <form id="form1" runat="server">
        <div id="container">
            <div id="inner-container">
                <h2 id="title">Registration</h2>
                <div id="row">
                    <asp:Label ID="fname" runat="server" Text="First Name"></asp:Label>
                    <br />
                    <asp:TextBox ID="fnameTB" runat="server"></asp:TextBox>
                </div>

                <div id="row">
                    <asp:Label ID="lname" runat="server" Text="Last Name"></asp:Label>
                    <br />
                    <asp:TextBox ID="lnameTB" runat="server"></asp:TextBox>
                </div>

                <div id="row">
                    <asp:Label ID="ccard" runat="server" Text="Credit Card"></asp:Label>
                    <br />
                    <asp:TextBox ID="ccardTB" runat="server"></asp:TextBox>
                </div>

                <div id="row">
                    <asp:Label ID="email" runat="server" Text="Email"></asp:Label>
                    <br />
                    <asp:TextBox ID="emailTB" runat="server"></asp:TextBox>
                </div>

                <div id="row">
                    <asp:Label ID="password" runat="server" Text="Password"></asp:Label>
                    <br />
                    <asp:TextBox ID="passwordTB" runat="server" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
                    <asp:Label ID="pwdchecker" runat="server" Text="vaildate"></asp:Label>
                    <br />
                    <asp:Button ID="chkpwd" runat="server" Text="Check Password" OnClick="Chkpwd"/>
                    <br />
                    <asp:Label ID="pwdstatus" runat="server" Text=""></asp:Label>
                </div>

                <div id="row">
                    <asp:Label ID="dob" runat="server" Text="Date of Birth"></asp:Label>
                    <br />
                    <input id="dobTB"  type="date" />
                </div>

                <div id="row">
                    <asp:Label ID="photo" runat="server" Text="Photo"></asp:Label>
                    <br />
                    <asp:FileUpload ID="photoTB" runat="server" />
                </div>

                <div id="submit">
                    <asp:Button ID="SubmitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
