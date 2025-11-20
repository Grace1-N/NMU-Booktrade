<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BuyerProfile.aspx.cs" Inherits="NMU_BookTrade.WebForm13" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

    <div class ="profile-container">
    <h2>Buyer Profile</h2>
    <asp:Image ID="imgProfile" runat="server" CssClass="profile-image" />

    <br />

    <div class="form-group">
                    <label>Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="input-field" />
                    <asp:RequiredFieldValidator 
                        ID="rfvUsername" 
                        runat="server" 
                        ControlToValidate="txtUsername" 
                        ErrorMessage="Username is required." 
                        CssClass="form_errormessage" 
                        ForeColor="Red" 
                        Display="Dynamic" />
                    <asp:RegularExpressionValidator 
                        ID="revUsername" 
                        runat="server" 
                        ControlToValidate="txtUsername" 
                        ErrorMessage="Username must be exactly 9 digits." 
                        CssClass="form_errormessage" 
                        ForeColor="Red" 
                        Display="Dynamic"
                        ValidationExpression="^\d{9}$" />
            </div>

            <div class="form-group">
                <label>Name</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="input-field" />
                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ErrorMessage="Name is required." CssClass="form_errormessage" ForeColor="Red" Display="Dynamic" />
            </div>

            <div class="form-group">
                <label>Surname</label>
                <asp:TextBox ID="txtSurname" runat="server" CssClass="input-field" />
                <asp:RequiredFieldValidator ID="rfvSurname" runat="server" ControlToValidate="txtSurname" ErrorMessage="Surname is required." CssClass="form_errormessage" ForeColor="Red" Display="Dynamic" />
            </div>

            <div class="form-group">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field" TextMode="Email" />
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
                <label>Address</label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="input-field" />
                <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="txtAddress" ErrorMessage="Address is required." CssClass="form_errormessage" ForeColor="Red" Display="Dynamic" />
            </div>


    <div class="form-group"><label>Change Profile Picture</label><asp:FileUpload ID="fuProfileImage" runat="server" /></div>

    <br />
    <br />
<asp:Button ID="btnUpdate" runat="server" Text="Update Profile"
    CssClass="btn-update" OnClick="btnUpdate_Click" CausesValidation="true" />

        <br />
        <br />
     <asp:Button ID="btnDeleteProfile" runat="server" Text="Delete My Account" CssClass="btn-delete" OnClientClick="return confirm('Are you sure you want to delete your account? This action cannot be undone.And you will be re-directed to the home page');" OnClick="btnDeleteProfile_Click" />

    <br />
    <br />
     <asp:Label ID="lblMessage" runat="server" CssClass="message" />

</div>

</asp:Content>
