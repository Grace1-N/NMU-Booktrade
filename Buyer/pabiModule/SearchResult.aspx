<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SearchResult.aspx.cs" Inherits="NMU_BookTrade.SearchResult" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <div class="search-wrapper">
        <div class="categories-inline">
            <asp:Repeater ID="rptCategory1" runat="server" OnItemCommand="rptCategory_ItemCommand">
                <ItemTemplate>
                    <asp:LinkButton runat="server"
                        CommandName="SelectCategory"
                        CommandArgument='<%# Eval("categoryID") + "|" + Eval("categoryName") %>'
                        CssClass="category-link">
                        <%# Eval("categoryName") %>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>

            <div class="search-bar-bd">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-bd" Placeholder="Search textbook title..." />
                <asp:Button ID="btnSearch" runat="server" Text="🔍" OnClick="btnSearch_Click" CssClass="search-btn" />
            </div>

            <asp:Repeater ID="rptCategory2" runat="server" OnItemCommand="rptCategory_ItemCommand">
                <ItemTemplate>
                    <asp:LinkButton runat="server"
                        CommandName="SelectCategory"
                        CommandArgument='<%# Eval("categoryID") + "|" + Eval("categoryName") %>'
                        CssClass="category-link">
                        <%# Eval("categoryName") %>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <asp:Label ID="lblSearched" runat="server" CssClass="results-label" Font-Bold="true"></asp:Label>
               <div class="section-line"></div>
    <asp:Label ID="lblMessage" runat="server" CssClass="error-message"></asp:Label>
    <div class="book-grid">
        <asp:Repeater ID="rptBooks" runat="server" OnItemCommand="rptBooks_ItemCommand">
            <ItemTemplate>
                <div class="book-card">
                    <asp:LinkButton ID="lnkCover" runat="server" 
                        CommandName="ViewReviews" 
                        CommandArgument='<%# Eval("bookISBN") %>'
                        CssClass="view-link review-link">
                        <img src='<%# ResolveUrl(string.IsNullOrEmpty(Eval("coverImage")?.ToString()) ? "~/Images/no-image.png" : Eval("coverImage").ToString()) %>' 
                             alt="Book Cover" 
                             class="book-image" />
                        <div class="book-info">
                            <p class="book-title"><%# Eval("title") ?? "N/A" %></p>
                            <p class="book-author"><%# Eval("author") ?? "N/A" %></p>
                            <p>R<%# Eval("price", "{0:F2}") ?? "N/A" %></p>
                            <div class="stars"><%# GetStarIcons(Convert.ToDouble(Eval("AvgRating"))) %></div>
                            <p>(<%# Eval("ReviewCount") %> reviews)</p>
                        </div>
                    </asp:LinkButton>
                    <div class="book-actions">
                        <asp:Button ID="btnAddToCart" runat="server" 
                            CommandName="AddToCart" 
                            CommandArgument='<%# Eval("bookISBN") %>' 
                            Text="Add to Cart ➤" 
                            CssClass="add-cart-btn" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <asp:Panel ID="CartPanel" runat="server" CssClass="slide-panel" Visible="false">
        <div class="panel-header">
            <asp:Label ID="lblHeader" runat="server" Text="Added to Cart"></asp:Label>
            <asp:Button ID="btnClose" runat="server" Text="x" CssClass="close-btn" OnClick="btnClose_Click" />
        </div>
        <div class="panel-content">
            <asp:Label ID="lblCartMessage" runat="server" Text=""></asp:Label>
<%--            <asp:Image ID="imgCartBook" runat="server" CssClass="cart-book-image" Width="80" />--%>
            <asp:Image ID="imgCartBook"
           runat="server"
           ImageUrl='<%# Eval("coverImage") %>'
           AlternateText="Book cover"
           Style="width:120px;height:auto;" />
            <asp:Label ID="lblCartBookTitle" runat="server" CssClass="cart-book-title"></asp:Label>
            <br /><br />
            <asp:HyperLink ID="lnkGoToCart" runat="server" NavigateUrl="~/Buyer/pabiModule/Cart.aspx" CssClass="go-to-cart">Go to Cart ➤</asp:HyperLink>
        </div>
    </asp:Panel>
</asp:Content>