<%@ Page Title="Driver Dashboard" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" 
    CodeBehind="DriverDashboard.aspx.cs" Inherits="NMU_BookTrade.Driver.ClintonModule.DriverDashboard" %>
<%@ Import Namespace="System.Data" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <div class="dd-container">
        <!-- Error Message Display -->
        <asp:Label ID="lblErrorMessage" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label>
        
        <!-- Header Section -->
        <div class="dd-header">
            <h1 class="dd-welcome">Welcome <asp:Label ID="lblDriverName" runat="server" Text="Driver"></asp:Label>!</h1>
            <p class="dd-subtitle">MANAGE YOUR DELIVERIES AND SCHEDULE</p>
            <div class="dd-refresh-container">
                <asp:Button ID="btnRefreshSummary" runat="server" 
                    CssClass="dd-refresh-btn" 
                    Text="⟳" 
                    OnClick="btnRefreshSummary_Click" />
            </div>
        </div>

        <!-- Stats Cards - Organized by Purpose -->
        <div class="dd-stats">
            <!-- OPERATIONAL SECTION - Today's Work -->
            <div class="dd-section-header">
                <h3>Today's Work</h3>
                <p>Your current assignments and active deliveries</p>
            </div>
            <div class="dd-operational-cards">
                <div class="dd-stat-card dd-operational">
                    <div class="dd-stat-icon">
                        <i class="fas fa-clock"></i>
                    </div>
                    <div class="dd-stat-content">
                        <div class="dd-stat-number"><asp:Label ID="lblPendingToday" runat="server" Text="0"></asp:Label></div>
                        <div class="dd-stat-label">Pending Assignments</div>
                        <div class="dd-stat-timeframe">Today</div>
                    </div>
                </div>
                <div class="dd-stat-card dd-operational">
                    <div class="dd-stat-icon">
                        <i class="fas fa-play-circle"></i>
                    </div>
                    <div class="dd-stat-content">
                        <div class="dd-stat-number"><asp:Label ID="lblAssignedToday" runat="server" Text="0"></asp:Label></div>
                        <div class="dd-stat-label">Ready to Start</div>
                        <div class="dd-stat-timeframe">Today</div>
                    </div>
                </div>
                <div class="dd-stat-card dd-operational">
                    <div class="dd-stat-icon">
                        <i class="fas fa-route"></i>
                    </div>
                    <div class="dd-stat-content">
                        <div class="dd-stat-number"><asp:Label ID="lblInTransitToday" runat="server" Text="0"></asp:Label></div>
                        <div class="dd-stat-label">In Transit</div>
                        <div class="dd-stat-timeframe">Today</div>
                    </div>
                </div>
            </div>

            <!-- PERFORMANCE SECTION - Historical Metrics -->
            <div class="dd-section-header">
                <h3>Performance Metrics</h3>
                <p>Your delivery statistics and ratings</p>
            </div>
            <div class="dd-performance-cards">
                <div class="dd-stat-card dd-performance">
                    <div class="dd-stat-icon">
                        <i class="fas fa-check-circle"></i>
                    </div>
                    <div class="dd-stat-content">
                        <div class="dd-stat-number"><asp:Label ID="lblCompletedDeliveries" runat="server" Text="0"></asp:Label></div>
                        <div class="dd-stat-label">Completed Today</div>
                        <div class="dd-stat-timeframe">Today</div>
                    </div>
                </div>
                <div class="dd-stat-card dd-performance">
                    <div class="dd-stat-icon">
                        <i class="fas fa-calendar-week"></i>
                    </div>
                    <div class="dd-stat-content">
                        <div class="dd-stat-number"><asp:Label ID="lblCompletedThisWeek" runat="server" Text="0"></asp:Label></div>
                        <div class="dd-stat-label">Completed This Week</div>
                        <div class="dd-stat-timeframe">This Week</div>
                    </div>
                </div>
                <div class="dd-stat-card dd-performance">
                    <div class="dd-stat-icon">
                        <i class="fas fa-truck"></i>
                    </div>
                    <div class="dd-stat-content">
                        <div class="dd-stat-number"><asp:Label ID="lblTotalCompleted" runat="server" Text="0"></asp:Label></div>
                        <div class="dd-stat-label">Total Completed</div>
                        <div class="dd-stat-timeframe">All Time</div>
                    </div>
                </div>
                <div class="dd-stat-card dd-performance">
                    <div class="dd-stat-icon">
                        <i class="fas fa-exclamation-triangle"></i>
                    </div>
                    <div class="dd-stat-content">
                        <div class="dd-stat-number"><asp:Label ID="lblFailedThisWeek" runat="server" Text="0"></asp:Label></div>
                        <div class="dd-stat-label">Failed Deliveries</div>
                        <div class="dd-stat-timeframe">This Week</div>
                    </div>
                </div>
                <div class="dd-stat-card dd-performance">
                    <div class="dd-stat-icon">
                        <i class="fas fa-ban"></i>
                    </div>
                    <div class="dd-stat-content">
                        <div class="dd-stat-number"><asp:Label ID="lblCancelledThisWeek" runat="server" Text="0"></asp:Label></div>
                        <div class="dd-stat-label">Cancelled Deliveries</div>
                        <div class="dd-stat-timeframe">This Week</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Tabs Navigation -->
        <div class="dd-tabs">
            <asp:LinkButton ID="tabPending" runat="server" CssClass="dd-tab active" OnClick="tabPending_Click">
                <i class="fas fa-truck"></i> Active Deliveries
            </asp:LinkButton>
            <asp:LinkButton ID="tabCompleted" runat="server" CssClass="dd-tab" OnClick="tabCompleted_Click">
                <i class="fas fa-check"></i> Completed Deliveries
            </asp:LinkButton>
            <asp:LinkButton ID="tabSchedule" runat="server" CssClass="dd-tab" OnClick="tabSchedule_Click">
                <i class="fas fa-calendar-alt"></i> Weekly Schedule
            </asp:LinkButton>
        </div>

        <!-- Main Content Area -->
        <div class="dd-content-area">
            <asp:MultiView ID="mvDriverContent" runat="server" ActiveViewIndex="0">
                
                <!-- Active Deliveries View -->
                <asp:View ID="viewPending" runat="server">
                    <h2 class="dd-section-title">Active Deliveries</h2>
                    <p class="dd-section-description">All deliveries that are assigned, in transit, or pending - everything except completed deliveries.</p>
                    <asp:Label ID="lblNoPending" runat="server" Text="No active deliveries found." 
                        CssClass="dd-empty-message" Visible="false"></asp:Label>
                    
                    <asp:Repeater ID="rptPendingDeliveries" runat="server" OnItemDataBound="rptPendingDeliveries_ItemDataBound">
                        <ItemTemplate>
                            <div class="dd-delivery-card">
                                <div class="dd-delivery-header">
                                    <span class="dd-delivery-id">#<%# Eval("deliveryID") %></span>
                                    <span class="dd-delivery-date"><%# ((DateTime)Eval("deliveryDate")).ToString("dd MMM yyyy") %></span>
                                    <span class="dd-delivery-status dd-status-<%# Eval("StatusText").ToString().ToLower().Replace(" ", "-") %>"><%# Eval("StatusText") %></span>
                                </div>
                                <div class="dd-delivery-body">
                                    <div class="dd-delivery-info">
                                        <div><i class="fas fa-book"></i> <%# Eval("BookTitle") %></div>
                                        <div><i class="fas fa-user"></i> <%# Eval("SellerName") %> → <%# Eval("BuyerName") %></div>
                                        <div><i class="fas fa-map-marker-alt"></i> <%# Eval("PickupAddress") %> to <%# Eval("DeliveryAddress") %></div>
                                    </div>
                                    <div class="dd-delivery-actions">
                                        <asp:Button ID="btnStartDelivery" runat="server" 
                                            CommandArgument='<%# Eval("deliveryID") %>'
                                            CssClass="dd-action-btn" 
                                            Text="Start Delivery" 
                                            OnClick="btnStartDelivery_Click"
                                            Visible='<%# Eval("status").ToString() == "1" %>' />
                                        <asp:Button ID="btnCompleteDelivery" runat="server" 
                                            CommandArgument='<%# Eval("deliveryID") %>'
                                            CssClass="dd-action-btn dd-success-btn" 
                                            Text="Complete Delivery" 
                                            OnClick="btnCompleteDelivery_Click"
                                            Visible='<%# Eval("status").ToString() == "2" %>' />
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:View>

                <!-- Completed Deliveries View -->
                <asp:View ID="viewCompleted" runat="server">
                    <h2 class="dd-section-title">Completed Deliveries</h2>
                    <p class="dd-section-description">A list of all your completed deliveries.</p>
                    <asp:Label ID="lblNoCompleted" runat="server" Text="No completed deliveries found." 
                              CssClass="dd-empty-message" Visible="false"></asp:Label>
                              
                    <asp:Repeater ID="rptCompletedDeliveries" runat="server">
                        <ItemTemplate>
                            <div class="dd-delivery-card">
                                <div class="dd-delivery-header">
                                    <span class="dd-delivery-id">#<%# Eval("deliveryID") %></span>
                                    <span class="dd-delivery-date"><%# ((DateTime)Eval("deliveryDate")).ToString("dd MMM yyyy") %></span>
                                    <span class="dd-delivery-status dd-status-<%# Eval("StatusText").ToString().ToLower().Replace(" ", "-") %>"><%# Eval("StatusText") %></span>
                                </div>
                                <div class="dd-delivery-body">
                                    <div class="dd-delivery-info">
                                        <div><i class="fas fa-book"></i> <%# Eval("BookTitle") %></div>
                                        <div><i class="fas fa-user"></i> <%# Eval("SellerName") %> → <%# Eval("BuyerName") %></div>
                                        <div><i class="fas fa-map-marker-alt"></i> <%# Eval("PickupAddress") %> to <%# Eval("DeliveryAddress") %></div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:View>

            <!-- Weekly Schedule View -->
            <asp:View ID="viewSchedule" runat="server">
                <h2 class="dd-section-title">Weekly Schedule</h2>
                <asp:Label ID="lblNoSchedule" runat="server" Text="No deliveries scheduled for this week." 
                    CssClass="dd-empty-message" Visible="false"></asp:Label>
                
                <div class="dd-schedule-container">
                    <asp:Repeater ID="rptWeeklySchedule" runat="server" OnItemDataBound="rptWeeklySchedule_ItemDataBound">
                        <ItemTemplate>
                            <div class="dd-schedule-day">
                                <div class="dd-day-header"><%# Eval("Day") %></div>
                                <div class="dd-day-content">
                                    <asp:Repeater ID="rptDayDeliveries" runat="server">
                                        <ItemTemplate>
                                            <div class="dd-schedule-item">
                                                <span class="dd-schedule-time"><%# Eval("Time") %></span>
                                                <span class="dd-schedule-book"><%# Eval("BookTitle") %></span>
                                                <span class="dd-schedule-location"><%# Eval("Location") %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</div>
</asp:Content>