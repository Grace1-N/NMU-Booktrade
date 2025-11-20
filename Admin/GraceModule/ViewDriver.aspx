<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ViewDriver.aspx.cs" Inherits="NMU_BookTrade.Admin.GraceModule.ViewDriver" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

    <asp:Label ID="lblMessage" runat="server" CssClass="message-label"></asp:Label>

<div class="driver-details">
    <asp:Image ID="imgProfile" runat="server" CssClass="driver-img" />
    
    <h2><asp:Label ID="lblName" runat="server" /></h2>
    <h3><asp:Label ID="lblSurname" runat="server" /></h3>
    <h3><asp:Label ID="lblEmail" runat="server" /></h3>
    <h3><asp:Label ID="lblPhone" runat="server" /></h3>

    <asp:Button ID="btnDelete" runat="server" Text="Delete Driver" CssClass="btn-delete" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete this driver?');" />
</div>

</asp:Content>
