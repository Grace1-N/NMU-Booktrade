<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AddDriver.aspx.cs" Inherits="NMU_BookTrade.Admin.GraceModule.DeleteDriver" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
  

    <div class="register-container-Admin">
        <div class="right-side">
            <h2>Add New Driver</h2>
            <p>Add your drivers details here to create their account.  </p>

            <asp:Label ID="lblMessage" runat="server" CssClass="form_errormessage" />

            <table class="register-table">
                <tr>
    <td>
        <div class="input-icon">
            <i class="fas fa-user"></i>
            <asp:TextBox ID="txtName" runat="server" CssClass="input-field"
    placeholder="Name" onkeypress="return /[a-zA-Z ]/.test(event.key);" />

        </div>
        <asp:RequiredFieldValidator ID="rfvName" runat="server"
            ControlToValidate="txtName"
            ErrorMessage="Name is required"
            ForeColor="Red" Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revName" runat="server"
            ControlToValidate="txtName"
            ValidationExpression="^[A-Za-z\s'-]+$"
            ErrorMessage="Name must contain letters only"
            ForeColor="Red" Display="Dynamic" />
    </td>
</tr>


                <tr>
    <td>
        <div class="input-icon">
            <i class="fas fa-user"></i>
            <asp:TextBox ID="txtSurname" runat="server" CssClass="input-field" placeholder="Surname" />
        </div>
        <asp:RequiredFieldValidator ID="rfvSurname" runat="server"
            ControlToValidate="txtSurname"
            ErrorMessage="Surname is required"
            ForeColor="Red" Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revSurname" runat="server"
            ControlToValidate="txtSurname"
            ValidationExpression="^[A-Za-z\s'-]+$"
            ErrorMessage="Surname must contain letters only"
            ForeColor="Red" Display="Dynamic" />
    </td>
</tr>

               <tr>
                            <td>
                                <div class="input-icon">
                                    <i class="fas fa-user-circle"></i>
                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field" placeholder="Username (10 digits)" MaxLength="10" />
                                </div>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txtUsername" ErrorMessage="Username is required"
                                    ForeColor="Red" Display="Dynamic" />
                                <asp:RegularExpressionValidator ID="revUsername" runat="server"
                                    ControlToValidate="txtUsername"
                                    ValidationExpression="^\d{10}$"
                                    ErrorMessage="Username must be exactly 10 digits"
                                    ForeColor="Red" Display="Dynamic" />
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <div class="input-icon">
                                    <i class="fas fa-envelope"></i>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field" placeholder="Email" TextMode="Email" />
                                </div>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                    ControlToValidate="txtEmail" ErrorMessage="Email is required"
                                    ForeColor="Red" Display="Dynamic" />
                                <asp:RegularExpressionValidator ID="revEmail" runat="server"
                                    ControlToValidate="txtEmail"
                                    ValidationExpression="^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"
                                    ErrorMessage="Enter a valid email address"
                                    ForeColor="Red" Display="Dynamic" />
                            </td>
                        </tr>

                <tr>
    <td>
        <div class="input-icon">
            <i class="fas fa-phone"></i>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="input-field"
    placeholder="Phone Number" MaxLength="16" onkeypress="return event.charCode >= 48 && event.charCode <= 57 || event.charCode == 43 || event.charCode == 32;"
 />
        </div>
        <asp:RequiredFieldValidator ID="rfvPhone" runat="server"
            ControlToValidate="txtPhone" ErrorMessage="Phone number is required"
            ForeColor="Red" Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revPhone" runat="server"
    ControlToValidate="txtPhone"
    ValidationExpression="^\+?[0-9 ]{10,20}$"
    ErrorMessage="Enter a valid phone number (digits only, spaces allowed)"
    ForeColor="Red" Display="Dynamic" />

    </td>
</tr>

               
                <tr>
                    <td>
                        <div class="input-icon">
                            <i class="fas fa-lock"></i>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="input-field" TextMode="Password" placeholder="Password" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Password is required" ControlToValidate="txtPassword" ForeColor="Red" Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label style="color:#c1f6ed;">Upload Profile Image:</label><br />
                        <asp:FileUpload ID="fuImage" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvImage" runat="server" ErrorMessage="Profile image is required" ControlToValidate="fuImage" ForeColor="Red" Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnAddDriver" runat="server" Text="Add Driver"
    CssClass="register-btn-AddDriver" OnClick="btnAddDriver_Click" CausesValidation="true" />

                    </td>
                        </tr>
            </table>
           
        </div>
 
    </div>
    <br />
    <br />

 

</asp:Content>
