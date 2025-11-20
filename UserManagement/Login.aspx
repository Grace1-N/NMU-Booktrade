
<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NMU_BookTrade.WebForm5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

     <div class="register-container">
     <div class="left-side">
         <asp:Image 
    ID="imgPersonReading" 
    runat="server" 
    CssClass="person-reading"
    ImageUrl="~/Images/Person reading.png" 
    AlternateText="Person Reading" />

     </div>  


     <div class="right-side">
         <h2 class=" LR-formheadings" > Log In  </h2>
         <h3 class="LR-formheadings">Lets get started </h3>

         <table class="register-table">
            
             <tr>
                 <td>
                     <div class="input-wrapper">
                         <div class="input-icon">
                             <i class="fas fa-user"></i>
                             <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field" ToolTip="Student Number (Without the s)" placeholder="Student Number (Without the s)"></asp:TextBox>
                         </div>

                          <!-- SIDE NOTE -->
                          <asp:Label ID="lblUsernameNote" runat="server" CssClass="input-note"  Text="If you are a Driver, enter your Driver Number. If you are Admin, enter your Staff Number without the 's'." />

                         <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ErrorMessage="Enter your student number, without the s at the beginning" ControlToValidate="txtUsername" CssClass="form_errormessage" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                     </div>
                 </td>  
             </tr>

             <tr>
                <td>
                    <div class="input-wrapper">
                        <label>Password:</label>
                        <div class="password-wrapper">
                            <asp:TextBox ID="txtPassword" ClientIDMode="Static" runat="server"
                                CssClass="input-field password-input" TextMode="Password"
                                ToolTip="Password" placeholder="Password"></asp:TextBox>
                            <span class="toggle-password" onclick="toggleVisibility('txtPassword', this)">
                                <i class="fas fa-eye"></i>
                            </span>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Enter your password, it's required" ControlToValidate="txtPassword" CssClass="form_errormessage" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                </td>  
            </tr>


                <tr>
                    <td>
                        <asp:Button ID="btnLogin" runat="server" Text="Log In" CssClass="form-button" OnClick="btnLogin_Click"/>
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="form-button" OnClick="btnClear_Click" CausesValidation="false"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblMessage" runat="server" CssClass="form_errormessage" ForeColor="Red" />
                        <br />
                        <asp:Label ID="lblNote" runat="server" CssClass="input-note"  Text="If you forgot your password click on the link below"/>
                        <br />
                        <asp:HyperLink 
                            ID="lnkForgotPassword" 
                            runat="server" 
                            NavigateUrl="~/UserManagement/ForgotPassword.aspx" 
                            CssClass="input-note" 
                            Text="Forgot Password?"/>
                    </td>
                </tr>            

         </table>
     </div>
 </div>



<script type="text/javascript">
    function toggleVisibility(inputId, iconSpan) {
        var input = document.getElementById(inputId);
        var isHidden = input.type === "password";

        input.type = isHidden ? "text" : "password";

        
        iconSpan.innerHTML = isHidden
             
           ? '<i class="fas fa-eye"></i>'
           : '<i class="fas fa-eye-slash"></i>';      
    }
</script>




</asp:Content>
