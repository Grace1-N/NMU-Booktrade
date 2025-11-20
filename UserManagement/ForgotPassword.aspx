<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="NMU_BookTrade.UserManagement.ForgotPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

    <div class ="register-container">
         <div class="left-side">
             <asp:Image 
             ID="imgPersonReading" 
             runat="server" 
             CssClass="person-reading"
             ImageUrl="~/Images/Person reading.png" 
             AlternateText="Person Reading"/>

           </div> 
        <div class="left-side">
                <div class="form-container">
                    <h2 class="LR-formheadings">Forgot Password</h2>
                    <p class="p-Passwordpages">Enter your email or username. If it's valid, you'll be redirected to reset your password.</p>

                    <asp:TextBox ID="txtEmailOrUsername" runat="server" CssClass="input-field" placeholder="Email or Username"></asp:TextBox>

                 <br/>
                 <br />
                    
                    <asp:Button ID="btnSubmit" runat="server" Text="Check" OnClick="btnSubmit_Click" CssClass="form-button"/>
                    
                     <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btnClear_forgetPasswordpage" OnClick="btnClear3_Click" CausesValidation="false" Width="89px"/>
                    <br />
                    <br />
                    <asp:Label ID="lblMessage" runat="server" CssClass="form_errormessage" ForeColor="Red"/>
                </div>

        </div>
         
    </div>
   

</asp:Content>
