<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="NMU_BookTrade.UserManagement.AccessDenied" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/Styles/AccessDenied.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <div class="ad-container">
        <div class="ad-box">
            <h2 class="ad-title">🚫 Access Denied</h2>
            <p class="ad-text">
                You do not have permission to access this page.<br />
                This area is restricted based on your account level.
            </p>
            <p class="ad-text-small">
                If you think this is a mistake, please contact support or switch accounts.
            </p>

            <div class="ad-actions">
                <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/UserManagement/Login.aspx" CssClass="ad-btn-primary">
                    Return to Login
                </asp:HyperLink>

                <asp:HyperLink ID="lnkHome" runat="server" NavigateUrl="~/UserManagement/Home.aspx" CssClass="ad-btn-secondary">
                    Back to Home
                </asp:HyperLink>

            </div>
        </div>
    </div>
</asp:Content>
