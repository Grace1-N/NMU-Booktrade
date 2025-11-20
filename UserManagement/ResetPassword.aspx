<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="NMU_BookTrade.UserManagement.ResetPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

<div class="register-container">
        <!-- LEFT SIDE IMAGE -->
        <div class="left-side">
            <asp:Image 
                ID="imgPersonReading" 
                runat="server" 
                CssClass="person-reading"
                ImageUrl="~/Images/Person reading.png" 
                AlternateText="Person Reading" />
        </div>

        <!-- RIGHT SIDE FORM -->
        <div class="left-side">
            <div class="form-container">
                <h2 class="LR-formheadings">Reset Your Password</h2>
                <p class="p-Passwordpages">
                    Now you can reset your password below. Remember to save it so that you do not forget it again.
                </p>

                <!-- New Password -->
                <div class="password-wrapper">
                    <asp:TextBox ID="txtNewPassword" ClientIDMode="Static" runat="server"
                        TextMode="Password"
                        CssClass="password-input"
                        placeholder="Enter your new password" />
                    <span class="toggle-password" onclick="toggleVisibility('txtNewPassword', this)">
                        <i class="fas fa-eye"></i>
                    </span>
                </div>
                <asp:RequiredFieldValidator 
                    ID="rfvNewPassword" 
                    runat="server" 
                    ControlToValidate="txtNewPassword" 
                    ErrorMessage="New password is required." 
                    CssClass="form_errormessage" 
                    Display="Dynamic" 
                    ForeColor="Red" />

                <br />

                <!-- Confirm Password -->
                <div class="password-wrapper">
                    <asp:TextBox ID="txtConfirmPassword" ClientIDMode="Static" runat="server"
                        TextMode="Password"
                        CssClass="password-input"
                        placeholder="Confirm your new password" />
                    <span class="toggle-password" onclick="toggleVisibility('txtConfirmPassword', this)">
                        <i class="fas fa-eye"></i>
                    </span>
                </div>
                <asp:RequiredFieldValidator 
                    ID="rfvConfirmPassword" 
                    runat="server" 
                    ControlToValidate="txtConfirmPassword" 
                    ErrorMessage="Please confirm your password." 
                    CssClass="form_errormessage" 
                    Display="Dynamic" 
                    ForeColor="Red" />
                <asp:CompareValidator 
                    ID="cvPasswords" 
                    runat="server"
                    ControlToCompare="txtNewPassword"
                    ControlToValidate="txtConfirmPassword"
                    ErrorMessage="Passwords do not match"
                    Operator="Equal"
                    Type="String"
                    CssClass="form_errormessage"
                    Display="Dynamic" 
                    ForeColor="Red" />

                <br /><br />

                <!-- Buttons -->
                <asp:Button ID="btnReset" runat="server" Text="Reset Password" OnClick="btnReset_Click" CssClass="form-button" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btnClear_forgetPasswordpage" OnClick="btnClear4_Click" CausesValidation="false" Width="89px" />
                
                <br /><br />

                <!-- Feedback -->
                <asp:Label ID="lblMessage" runat="server" CssClass="form_errormessage" ForeColor="Red" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Toggle password visibility
        function toggleVisibility(inputId, iconSpan) {
            var input = document.getElementById(inputId);

            if (input.type === "password") {
                input.type = "text";
                iconSpan.innerHTML = '<i class="fas fa-eye"></i>'; //  Eye icon = visible now
            } else {
                input.type = "password";
                iconSpan.innerHTML = '<i class="fas fa-eye-slash"></i>'; //  Eye-slash = hidden now
            }
        }

    </script>

   

</asp:Content>
