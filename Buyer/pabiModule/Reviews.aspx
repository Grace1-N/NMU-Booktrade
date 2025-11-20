<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Reviews.aspx.cs" Inherits="NMU_BookTrade.WebForm7" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

       <div class="reviews-title">
       <h2>Reviews</h2>
   </div>
        <hr class="section-line" />
<div class="reviews-main">

        <div class="tabs">
            <asp:Button ID="btnShowPurchases" runat="server" Text="Order Items" CssClass="tab-btn active" OnClick="btnShowPurchases_Click" />
            <asp:Button ID="btnShowHistory" runat="server" Text="Reviews History" CssClass="tab-btn" OnClick="btnShowHistory_Click" />
        </div>

        <!-- Success/Error Messages -->
        <asp:Label ID="lblSuccess" runat="server" CssClass="alert alert-success" Visible="false"></asp:Label>
        <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label>

        <!-- Purchases Tab -->
        <asp:Panel ID="pnlPurchasesTab" runat="server" Visible="true">
            <asp:Repeater ID="rptPurchases" runat="server">
                <ItemTemplate>
                    <div class="purchase-item">
                        <asp:Image ID="imgPurchaseProduct" runat="server" ImageUrl='<%# Eval("coverImage") %>' Width="60" />
                        <span class="product-name"><%# Eval("title") %></span>
                        <asp:Button ID="btnWriteReview" runat="server" CssClass="btn-reviews" Text="Write Review" CommandArgument='<%# Eval("bookISBN") %>' OnClick="btnWriteReview_Click" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlNoReviews" runat="server" Visible="false">
                <asp:Button ID="btnViewPastOrders" runat="server" Text="View Past Orders" OnClick="btnViewPastOrders_Click" />
            </asp:Panel>
        </asp:Panel>

        <!-- Review History Tab -->
        <asp:Panel ID="pnlHistoryTab" runat="server" Visible="false">
            <div>
            <asp:DropDownList ID="ddlReviewFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlReviewFilter_SelectedIndexChanged" CssClass="reviews-filter" />
                </div>
            <asp:Repeater ID="rptReviewHistory" runat="server">
                <ItemTemplate>
                    <div class="review-history-item">
                        <span class="stars"><%# new string('★', (int)Eval("reviewRating")) %></span>
                        <strong><%# Eval("title") %></strong>
                        <p><%# Eval("reviewComment") %></p>
                        <small><%# Eval("reviewDate", "{0:dd MMM yyyy}") %></small><br />
                        <asp:Button ID="btnDeleteReview" runat="server" Text="Delete" CommandArgument='<%# Eval("reviewID") %>' OnClick="btnDeleteReview_Click" CssClass="delete-btn" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

    <!-- Hidden field to store selected review ID -->
<asp:HiddenField ID="hfDeleteReviewID" runat="server" />

    <!-- Delete Confirmation Panel -->
<asp:Panel ID="pnlDeleteConfirm" runat="server" CssClass="confirm-panel" Visible="false">
    <div class="confirm-box">
        <h4>Are you sure you want to delete this review?</h4>
        <div class="confirm-buttons">
            <asp:Button ID="btnConfirmYes" runat="server" Text="Yes" CssClass="btn btn-primary" OnClick="btnConfirmYes_Click" CausesValidation="false" UseSubmitBehavior="false" />
            <asp:Button ID="btnConfirmNo" runat="server" Text="No" CssClass="btn btn-secondary" OnClick="btnConfirmNo_Click" CausesValidation="false" UseSubmitBehavior="false" />
        </div>
    </div>
</asp:Panel>

        <!-- Product Summary Section (used when viewing reviews for a specific book) -->
        <asp:Panel ID="pnlProductSummary" runat="server" Visible="false" CssClass="product-summary">
            <h3><asp:Label ID="lblProductNameSummary" runat="server" /></h3>
            <p><asp:Label ID="lblAverageRating" runat="server" CssClass="avg-rating" /></p>
            <p><asp:Label ID="lblTotalReviews" runat="server" CssClass="total-reviews" /></p>

            <div class="rating-breakdown">
                <asp:Repeater ID="rptBreakdown" runat="server">
                    <ItemTemplate>
                        <div>
                            <span><%# Eval("reviewRating") %> ★</span> –
                            <span><%# Eval("CountReviews") %> reviews</span>
                            (<%# Eval("percentage") %>%)
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="reviews-list">
                <asp:Repeater ID="rptReviews" runat="server">
                    <ItemTemplate>
                        <div class="review-item">
                            <strong><%# Eval("buyerName") %></strong>
                            <span class="stars"><%# new string('★', (int)Eval("reviewRating")) %></span><br />
                            <p><%# Eval("reviewComment") %></p>
                            <small><%# Eval("reviewDate", "{0:dd MMM yyyy}") %></small>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </asp:Panel>

        <!-- Slide-in Review Panel -->
        <asp:Panel ID="pnlReviewPanel" runat="server" CssClass="side-panel">
            <div class="panel-content">
                <h4><strong>Write Review</strong></h4>
                        <asp:Button ID="btnClose" runat="server" Text="x" CssClass="close-btn-rp" OnClick="btnCloseReview_Click" />
                <div class="product-head">
                    <asp:Image ID="imgProduct" runat="server" Width="50" />
                    <asp:Label ID="lblProductName" runat="server" CssClass="product-name" />
                </div>
                <div class="field">
                    <label>Choose a Rating</label>
                    <asp:DropDownList ID="ddlRating" runat="server" CssClass="stars-select">
                        <asp:ListItem Value="5">★★★★★</asp:ListItem>
                        <asp:ListItem Value="4">★★★★☆</asp:ListItem>
                        <asp:ListItem Value="3">★★★☆☆</asp:ListItem>
                        <asp:ListItem Value="2">★★☆☆☆</asp:ListItem>
                        <asp:ListItem Value="1">★☆☆☆☆</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="field">
                    <asp:Label ID="lblFirstName" runat="server" CssClass="buyer-name" Text="Name: " />
                    <asp:TextBox ID="txtReviewComment" runat="server" TextMode="MultiLine" Rows="5" Width="100%" Placeholder="Your review..."></asp:TextBox>
                </div>
                <asp:HiddenField ID="hfBookISBN" runat="server" />

                <div class="button-row">
                    <asp:Button ID="btnSubmitReview" runat="server" Text="Submit Review" CssClass="btn-reviews" OnClick="btnSubmitReview_Click" />
                    <asp:Button ID="btnViewGuidelines" runat="server" Text="View Guidelines" CssClass="btn-reviews" OnClick="btnViewGuidelines_Click" />
                   </div>
                </div>
        </asp:Panel>

        <!-- Review Guidelines Modal -->
        <asp:Panel ID="pnlGuidelines" runat="server" CssClass="guidelines">
                    <div class="reviews-image"> 
    <img src='<%= ResolveUrl("~/Images/Ratings.jpg") %>' alt="No Orders" class="reviews-image" />
</div>
            <h3>Review Guidelines</h3>
            <h6> Do</h6>
            <ul>
                <li>Keep content useful and relevant to others.</li>
                <li>Focus on the product and your experience with it.</li>
                <li>Share details about what you like or dislike.</li>
                <li>Complete your review in English.</li>
            </ul>
            <h6> Don’t</h6>
            <ul>
                <li>Include your personal details.</li>
                <li>Share prices from NMUBookTrade.</li>
                <li>Use inappropriate, discriminatory, or other language not suitable for public forums.</li>
                <li>Rate service or delivery aspects.</li>
            </ul>
            <asp:Button ID="btnCloseGuidelines" runat="server" Text="I Understand" OnClick="btnCloseGuidelines_Click" CssClass="guidelines-button" />
        </asp:Panel>
    </div>
</asp:Content>