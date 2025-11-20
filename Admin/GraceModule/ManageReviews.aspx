<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageReviews.aspx.cs" Inherits="NMU_BookTrade.Admin.GraceModule.ManageReviews" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    
      <div class="aa-admin-container">
        <div class="aa-header">
            <h1>🛡️ Review Management</h1>
            <p>Monitor and manage inappropriate content</p>
        </div>

        <!-- Stats Section -->
        <div class="aa-stats">
            <div class="aa-stat-card">
                <div class="stat-number">
                    <asp:Literal ID="litTotalFlagged" runat="server"></asp:Literal>
                </div>
                <div class="stat-label">Flagged Reviews</div>
            </div>
            <div class="aa-stat-card">
                <div class="stat-number">
                    <asp:Literal ID="litTotalReviews" runat="server"></asp:Literal>
                </div>
                <div class="stat-label">Total Reviews</div>
            </div>
            <div class="aa-stat-card">
                <div class="stat-number">
                    <asp:Literal ID="litFlaggedToday" runat="server"></asp:Literal>
                </div>
                <div class="stat-label">Flagged Today</div>
            </div>
            <div class="aa-stat-card">
                <div class="stat-number">
                    <asp:Literal ID="litCleanRate" runat="server"></asp:Literal>
                </div>
                <div class="stat-label">Clean Rate</div>
            </div>
        </div>

        <!-- Filter Section -->
        <div class="aa-content">
            <div class="aa-filter-section">
                <div class="aa-filter-title">🚫 Current Bad Words Filter:</div>
                <div class="aa-word-list">
                    <span class="aa-word-tag">spam</span>
                    <span class="aa-word-tag">fake</span>
                    <span class="aa-word-tag">scam</span>
                    <span class="aa-word-tag">terrible</span>
                    <span class="aa-word-tag">awful</span>
                    <span class="aa-word-tag">hate</span>
                    <span class="aa-word-tag">stupid</span>
                    <span class="aa-word-tag">garbage</span>
                    <span class="aa-word-tag">useless</span>
                    <span class="aa-word-tag">fraud</span>
                    <span class="aa-word-tag">ripoff</span>
                    <span class="aa-word-tag">pathetic</span>
                    <span class="aa-word-tag">horrible</span>
                    <span class="aa-word-tag">idiot</span>
                    <span class="aa-word-tag">dumb</span>
                    <span class="aa-word-tag">worthless</span>
                    <span class="aa-word-tag">trash</span>
                    <span class="aa-word-tag">nonsense</span>
                </div>
            </div>

            <!-- Flagged Reviews Section -->
            <h2 class="aa-section-title">🚨 Flagged Reviews</h2>

            <asp:Repeater ID="rptFlaggedReviews" runat="server" OnItemCommand="rptFlaggedReviews_ItemCommand">
                <ItemTemplate>
                    <div class="aa-review-item">
                        <div class="aa-review-header">
                            <div class="aa-reviewer-info">
                                <!-- Show stars (repeat ★ based on reviewRating) -->
                                <div class="aa-stars">
                                    <%# new string('★', Convert.ToInt32(Eval("reviewRating"))) %>
                                </div>
                                <div>
                                    <!-- Reviewer name is not stored, so just show Anonymous -->
                                    <div class="aa-reviewer-name">
                                            <%# Eval("BuyerName") + " " + Eval("BuyerSurname") %>
                                     </div>
                                    <div class="aa-review-date">
                                        <%# Eval("reviewDate", "{0:MMM dd, yyyy}") %>
                                    </div>
                                </div>
                            </div>
                            <span class="aa-flagged-badge">FLAGGED</span>
                        </div>

                        <!-- Review comment -->
                        <div class="aa-review-content">
                            <%# Eval("reviewComment") %>
                        </div>

                        <!-- Action buttons -->
                       <div class="aa-action-buttons">
                            <asp:LinkButton ID="btnRemove" runat="server" 
                                CssClass="aa-btn btn-remove" 
                                CommandName="Remove" 
                                CommandArgument='<%# Eval("reviewID") %>'
                                OnClientClick="return confirm('Are you sure you want to remove this review?');">
                                🗑️ Remove Review
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnApprove" runat="server" 
                                CssClass="aa-btn btn-approve" 
                                CommandName="Approve" 
                                CommandArgument='<%# Eval("reviewID") %>'>
                                ✅ Approve Anyway
                            </asp:LinkButton>
                        </div>

                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Literal ID="litNoFlagged" runat="server" Visible="false" />
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
 
</asp:Content>
