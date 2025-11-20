<%@ Page Title="Driver Profile" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" 
    CodeBehind="DriverProfile.aspx.cs" Inherits="NMU_BookTrade.Driver.ClintonModule.DriverProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <div class="profile-container">
        <h2>Driver Profile</h2>
        
        <!-- Profile Image -->
        <asp:Image ID="imgProfile" runat="server" CssClass="profile-image" AlternateText="Driver Profile Picture" />
        
        <!-- Profile Form -->
        <div class="form-group">
            <label>Username</label>
           
            <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field" MaxLength="10" />
       
            <asp:RequiredFieldValidator ID="rfvUsername" runat="server" 
                ControlToValidate="txtUsername" ErrorMessage="Username is required." 
                CssClass="form_errormessage" ForeColor="Red" Display="Dynamic"  />
                      
             <asp:RegularExpressionValidator 
                     ID="revUsername" 
                     runat="server" 
                     ControlToValidate="txtUsername" 
                     ErrorMessage="Username must be exactly 9 digits." 
                     CssClass="form_errormessage" 
                     ForeColor="Red" 
                     Display="Dynamic"
                     ValidationExpression="^\d{10}$" />
        </div>

        <div class="form-group">
            <label>Name</label>
            
                <asp:TextBox ID="txtName" runat="server" CssClass="input-field" MaxLength="50" />
            
            <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                ControlToValidate="txtName" ErrorMessage="Name is required." 
                CssClass="form_errormessage" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Surname</label>
            <asp:TextBox ID="txtSurname" runat="server" CssClass="input-field" MaxLength="50" />
            <asp:RequiredFieldValidator ID="rfvSurname" runat="server" 
                ControlToValidate="txtSurname" ErrorMessage="Surname is required." 
                CssClass="form_errormessage" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field" TextMode="Email" MaxLength="100" />
           <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
        ControlToValidate="txtEmail"
        ErrorMessage="Email is required."
        CssClass="form_errormessage" Display="Dynamic" />

    <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtEmail"
        ValidationExpression="^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"
        ErrorMessage="Please enter a valid email address."
        CssClass="form_errormessage" Display="Dynamic" />
        </div>
                 <div class="form-group">
                                <label>Phone Number</label>
                                 <asp:TextBox ID="txtNumber" runat="server" CssClass="input-field" />
                                 <asp:RequiredFieldValidator 
                                     ID="rfvNumber" 
                                     runat="server" 
                                     ControlToValidate="txtNumber" 
                                     ErrorMessage="Phone number is required." 
                                     CssClass="form_errormessage" 
                                     ForeColor="Red" 
                                     Display="Dynamic" />
                                 <asp:RegularExpressionValidator 
                                     ID="revPhoneNumber" 
                                     runat="server" 
                                     ControlToValidate="txtNumber" 
                                     ErrorMessage="Enter a valid phone number (digits only, optional + at start)." 
                                     CssClass="form_errormessage" 
                                     ForeColor="Red" 
                                     Display="Dynamic"
                                     ValidationExpression="^\+?\d{8,15}$" />
                 
                            </div>
                            <div class="form-group">
                                <label>Change Profile Picture</label>
                                <asp:FileUpload ID="fuProfileImage" runat="server" />
                                <asp:Label ID="lblImageError" runat="server" 
                                    CssClass="form_errormessage" 
                                    ForeColor="Red" 
                                    Visible="false" />
                            </div>

        
       
 <br />
 <br />

<asp:Button ID="btnUpdate" runat="server" Text="Update Profile"
    CssClass="btn-update" OnClick="btnUpdate_Click" CausesValidation="true" />

 <br />
 <br />

 <asp:Button ID="btnDeleteProfile" runat="server" Text="Delete My Account" CssClass="btn-delete" 
     OnClientClick="return confirm('Are you sure you want to delete your account? This action cannot be undone. And you will be re-directed to the home page.');" 
     OnClick="btnDeleteProfile_Click" />
        
        
        <!-- Status Message -->
        <asp:Label ID="lblMessage" runat="server" CssClass="message" />

        </div>
  
</asp:Content>