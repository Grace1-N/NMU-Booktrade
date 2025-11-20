<%@ Page Title="Textbook Details" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ViewTextBookDetails.aspx.cs" Inherits="NMU_BookTrade.ViewTextBookDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <title><asp:Literal ID="litPageTitle" runat="server" Text="Textbook Details" /></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <div class="orders-header">Textbook Details</div>
    <div class="section-line"></div>
    <div class="book-details-container">
        <div class="book-header">
            <div class="cart-item">
   <asp:Image ID="bookCover" runat="server" CssClass="book-cover" />
            <div class="book-info">
                <p><strong>Title:</strong> <asp:Label ID="lblTitle" runat="server" /></p>
                <p><strong>Author:</strong> <asp:Label ID="lblAuthor" runat="server" /></p>
                <p><strong>ISBN:</strong> <asp:Label ID="lblISBN" runat="server" /></p>
                <p><strong>Category:</strong> <asp:Label ID="lblCategory" runat="server" /></p>
                <p><strong>Genre:</strong> <asp:Label ID="lblGenre" runat="server" /></p>
                <p><strong>Condition:</strong> <asp:Label ID="lblCondition" runat="server" /></p>
                <p><strong>Price:</strong> R<asp:Label ID="lblPrice" runat="server" /></p>
                <p><strong>Seller:</strong> <asp:Label ID="lblSeller" runat="server" /></p>

                <div class="book-actions">
                    <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart ➤"
                        CssClass="add-cart-btn" OnClick="btnAddToCart_Click" />
                </div>
            </div>
        </div>
            </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="error-message"></asp:Label>

        <asp:Panel ID="CartPanel" runat="server" CssClass="slide-panel" Visible="false">
            <div class="panel-header">
                <asp:Label ID="lblHeader" runat="server" Text="Added to Cart"></asp:Label>
                <asp:Button ID="btnClose" runat="server" Text="x" CssClass="close-btn" OnClick="btnClose_Click" />
            </div>
            <div class="panel-content">
                <asp:Label ID="lblCartMessage" runat="server" Text=""></asp:Label>
                <asp:Image ID="imgCartBook" runat="server" CssClass="cart-book-image" Width="80" />
                <asp:Label ID="lblCartBookTitle" runat="server" CssClass="cart-book-title"></asp:Label>
                <br /><br />
                <asp:HyperLink ID="lnkGoToCart" runat="server" NavigateUrl="~/Buyer/pabiModule/Cart.aspx" CssClass="go-to-cart">Go to Cart ➤</asp:HyperLink>
            </div>
        </asp:Panel>
    </div>

    <div class="orders-header">Customer Reviews</div>
    <div class="section-line"></div>

    <div class="reviews-section">
        <div class="testimonial-cards-vtbd">
            <asp:Repeater ID="rptTestimonials" runat="server">
                <ItemTemplate>
                    <div class="testimonial-card-vtbd">
                        <asp:Image ID="imgProfile" runat="server"
                                   ImageUrl='<%# ResolveUrl(string.IsNullOrEmpty(Eval("buyerProfileImage")?.ToString()) ? "~/Images/no-profile.png" : "~/UploadedImages/" + Eval("buyerProfileImage").ToString()) %>'
                                   CssClass="testimonial-img" AlternateText="Buyer Photo" />
                        <div class="testimonial-content">
                            <p class="testimonial-comment">"<%# Eval("reviewComment") %>"</p>
                            <div class="testimonial-stars">
                                <%# GetStarHtml(Convert.ToInt32(Eval("reviewRating"))) %>
                            </div>
                            <p class="testimonial-name">- <%# Eval("BuyerName") %> <%# Eval("BuyerSurname") %></p>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Panel ID="pnlNoReviews" runat="server" Visible='<%# rptTestimonials.Items.Count == 0 %>' CssClass="no-reviews">
                        <p>No reviews yet for this textbook.</p>
                    </asp:Panel>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>