<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="LogOut.aspx.cs" Inherits="NMU_BookTrade.WebForm18" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
        <h1 class="h1-logout">Leaving so soon!</h1>
<div class="logoutMessage-container">
    <div class="emoji-wrapper">
        <asp:Image ID="imgGoodbye" runat="server" CssClass="emoji-img" ImageUrl="~/Images/goodbye.png" />
    </div>

    <div class="message-box">
        <p class="farewell">
            THANK YOU FOR VISITING NMU BOOKTRADE <asp:Label ID="lblCustomerName" runat="server" />.<br />
            WE HOPE YOU FOUND WHAT YOU WERE LOOKING FOR!
        </p>
        <div class="button-container">
            <asp:Button ID="btnYes" runat="server" CssClass="btn" Text="YES ➤" OnClick="btnYes_Click" />
            <asp:Button ID="btnNo" runat="server" CssClass="btn" Text="NO ➤" OnClick="btnNo_Click" />
        </div>
    </div>
</div>
</asp:Content>