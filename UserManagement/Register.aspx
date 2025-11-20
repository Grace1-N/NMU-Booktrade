<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="NMU_BookTrade.WebForm6" %>
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
            AlternateText="Person Reading"/>

        </div>  

        <div class="right-side">
            <h2 class=" LR-formheadings" > Create an account </h2>
            <h3 class=" LR-formheadings">Lets get started </h3>
            <asp:ValidationSummary ID="valSummary" runat="server" CssClass="form_errormessage" DisplayMode="List" />


            <table class="register-table">
                <tr>
                    <td>
                        <div class="input-wrapper">
                            <div class="input-icon">
                                <i class="fas fa-user-tag"></i>
                                <asp:DropDownList ID="ddlRole" runat="server" CssClass="input-field">
                                    <asp:ListItem Text="-- Select Role --" Value="" />
                                    <asp:ListItem Text="Buyer" Value="2" />
                                    <asp:ListItem Text="Seller" Value="3" />
                                    </asp:DropDownList>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvRole" runat="server" 
                                ErrorMessage="Please select a role" 
                                ControlToValidate="ddlRole"
                                InitialValue="" 
                                CssClass="form_errormessage" 
                                ForeColor="Red" 
                                Display="Dynamic" />
                        </div>
                    </td>
               </tr>



                <tr>
                    <td>
                        <div class="input-wrapper">
                            <div class="input-icon">
                                <i class="fas fa-user"></i>
                                <asp:TextBox ID="txtName" runat="server" CssClass="input-field" ToolTip="Name" placeholder="Name"></asp:TextBox>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Please enter your Name" ControlToValidate="txtName" CssClass="form_errormessage" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                    </td>  
                </tr>  

                <tr>
                    <td>
                        <div class="input-wrapper">
                            <div class="input-icon">
                                <i class="fas fa-user"></i>
                                <asp:TextBox ID="txtSurname" runat="server" CssClass="input-field" ToolTip="Surname" placeholder ="Surname"></asp:TextBox>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvSurname" runat="server" ErrorMessage="Please enter your Surname" ControlToValidate="txtSurname" CssClass="form_errormessage" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                    </td>  
                </tr>  

                <tr>
                    <td>
                        <div class="input-wrapper">
                            <div class="input-icon">
                                <i class="fas fa-envelope"></i>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field"
    ToolTip="Email" placeholder="Email" TextMode="Email"></asp:TextBox>

                            </div>
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Please enter your email address" ControlToValidate="txtEmail" CssClass="form_errormessage" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                           <asp:RegularExpressionValidator ID="revEmail" runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Invalid email address."
                            ValidationExpression="^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,24}$"
                            Display="Dynamic" ForeColor="Red" />

                        </div>
                    </td>  
                </tr>  

                <tr>
                    <td>
                        <div class="input-wrapper">
                            <div class="input-icon">
                                <i class="fas fa-phone"></i>
                                <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="input-field" ToolTip="Phone Number" placeholder="Phone Number"></asp:TextBox>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" ErrorMessage="Your phone number is required here" ControlToValidate="txtPhoneNumber" CssClass="form_errormessage" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revPhoneNumber" runat="server" ControlToValidate="txtPhoneNumber"
                                ErrorMessage="Invalid phone number" ValidationExpression="^\+?\d{10,15}$"
                                ForeColor="Red" Display="Dynamic" />
                        </div>
                    </td>  
                </tr>

                <tr>
                    <td>
                        <div class="input-wrapper">
                            <div class="input-icon">
                                <i class="fas fa-location-dot"></i>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="input-field" ToolTip="Home Address" placeholder ="Home Adress"></asp:TextBox>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ErrorMessage="Your home address is required here" ControlToValidate="txtAddress" CssClass="form_errormessage" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                    </td>  
                </tr>

                <tr>
                    <td>
                        <div class="input-wrapper">
                            <div class="input-icon">
                                <i class="fas fa-user"></i>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field" ToolTip="Student Number (Without the s)" placeholder="Student Number (Without the s)"></asp:TextBox>
                            </div>
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

                                <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                                    ErrorMessage="Enter your password, it's required"
                                    ControlToValidate="txtPassword"
                                    CssClass="form_errormessage" ForeColor="Red" Display="Dynamic" />
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <div class="input-wrapper">
                                <label>Confirm Password:</label>
                                <div class="password-wrapper">
                                    <asp:TextBox ID="txtConfirmPassword" ClientIDMode="Static" runat="server"
                                        CssClass="input-field password-input" TextMode="Password"
                                        placeholder="Re-enter your password"></asp:TextBox>

                                    <span class="toggle-password" onclick="toggleVisibility('txtConfirmPassword', this)">
                                        <i class="fas fa-eye"></i>
                                    </span>
                                </div>

                                <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server"
                                    ControlToValidate="txtConfirmPassword"
                                    ErrorMessage="Please confirm your password"
                                    ForeColor="Red" Display="Dynamic" />

                                <asp:CompareValidator ID="cvPasswords" runat="server"
                                    ControlToCompare="txtPassword"
                                    ControlToValidate="txtConfirmPassword"
                                    ErrorMessage="Passwords do not match"
                                    Operator="Equal"
                                    Type="String"
                                    ForeColor="Red" Display="Dynamic" />
                            </div>
                        </td>
                    </tr>



                                    <tr>
                                        <td>
                                            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="form-button" OnClick="btnRegister_Click" />
                                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="form-button" OnClick="btnClear_Click" CausesValidation="false" />
                                        </td>

                                    </tr>
               
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblMessage" runat="server" CssClass="form_errormessage" ForeColor="Red" />
                                        </td>
                                    </tr>

                                </table>
                            </div>
                        </div>

    <!-- Registration Confirmation Modal
  The popup box that will appear after registration and the id = roleType will identify whether its a seller/buyer/driver/admin       
        -->
        <div id="confirmationModal" class="modal-overlay" >
            <div class="modal-box">
                <h3>Registration Successful!</h3>
                <p>Congratulations! Your registration as a <span id="roleType"></span> has been completed successfully.<br/>
                You can now log in to access your account.</p>
                <p>Click OK to proceed to the login page.</p>
                <button onclick="redirectToLogin()">OK ➤</button>
            </div>
        </div>








 <script type="text/javascript">
     // Show confirmation popup modal is only done after the registration is success
     ful
              function showConfirmation(role) {
                  document.getElementById("roleType").innerText = role;
                  const modal = document.getElementById("confirmationModal");
                  modal.classList.add("show");
              }

              // Redirect to login page
              function redirectToLogin() {
                  // I used a absolute URL (ASP.NET won't process the ~ here in JS)
                  window.location.href = "/UserManagement/Login.aspx";
              }

              // Toggle password visibility for input fields
                 function toggleVisibility(inputId, iconSpan) {
                     var input = document.getElementById(inputId);
                     var isHidden = input.type === "password";

                     // Toggle input type
                     input.type = isHidden ? "text" : "password";

                     // Update icon based on new state
                     iconSpan.innerHTML = isHidden
                         ? '<i class="fas fa-eye"></i>'       // Password is now visible, show open eye
                         : '<i class="fas fa-eye-slash"></i>'; // Password is now hidden, show slashed eye
                 }

 </script>


  
        
   
    


</asp:Content>
