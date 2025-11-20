<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageDeliveries.aspx.cs" Inherits="NMU_BookTrade.Driver.ClintonModule.ManageDeliveries"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    
<div class="md-container">
    <div class="md-header">
        <h1 class="md-title">My Deliveries</h1>
    </div>

    <div class="md-filters">
        <div class="md-filter-group">
            <label class="md-filter-label">Status:</label>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="md-filter-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                <asp:ListItem Text="All Deliveries" Value="all" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Pending" Value="pending"></asp:ListItem>
                <asp:ListItem Text="Assigned" Value="assigned"></asp:ListItem>
                <asp:ListItem Text="In Transit" Value="transit"></asp:ListItem>
                <asp:ListItem Text="Completed" Value="completed"></asp:ListItem>
                <asp:ListItem Text="Failed" Value="failed"></asp:ListItem>
                <asp:ListItem Text="Cancelled" Value="cancelled"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="md-filter-group">
            <label class="md-filter-label">Date Range:</label>
            <asp:TextBox ID="txtStartDate" runat="server" CssClass="md-date-input" TextMode="Date"></asp:TextBox>
            <span class="md-date-separator">to</span>
            <asp:TextBox ID="txtEndDate" runat="server" CssClass="md-date-input" TextMode="Date"></asp:TextBox>
            <asp:Button ID="btnApplyDate" runat="server" Text="Apply" CssClass="md-filter-btn" OnClick="btnApplyDate_Click" />
        </div>
    </div>

    <div class="md-deliveries-container">
        <asp:GridView ID="gvDeliveries" runat="server" AutoGenerateColumns="false" CssClass="md-deliveries-grid"
            AllowPaging="true" PageSize="10" OnPageIndexChanging="gvDeliveries_PageIndexChanging"
            OnRowCommand="gvDeliveries_RowCommand" OnRowDataBound="gvDeliveries_RowDataBound" DataKeyNames="DeliveryID">
            <Columns>
                <asp:BoundField DataField="DeliveryID" HeaderText="ID" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell" />
                <asp:BoundField DataField="BookTitle" HeaderText="Book" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell" />
                <asp:BoundField DataField="SellerName" HeaderText="Seller" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell" />
                <asp:BoundField DataField="BuyerName" HeaderText="Buyer" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell" />
                <asp:BoundField DataField="PickupAddress" HeaderText="Pickup" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell" />
                <asp:BoundField DataField="DeliveryAddress" HeaderText="Delivery" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell" />
                <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell" />
                <asp:BoundField DataField="ScheduledDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell" />
                
                <asp:TemplateField HeaderText="Update Status" HeaderStyle-CssClass="md-grid-header" ItemStyle-CssClass="md-grid-cell">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlStatusUpdate" runat="server" CssClass="md-status-dropdown" 
                            AutoPostBack="true" OnSelectedIndexChanged="ddlStatusUpdate_SelectedIndexChanged">
                            <asp:ListItem Text="Pending" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Assigned" Value="1"></asp:ListItem>
                            <asp:ListItem Text="In Transit" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Completed" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Failed" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Cancelled" Value="5"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:HiddenField ID="hfDeliveryID" runat="server" Value='<%# Eval("DeliveryID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerStyle CssClass="md-grid-pager" />
            <EmptyDataTemplate>
                <div class="md-empty-message">
                    No deliveries found matching your criteria.
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

                <div class="md-summary">
            <div class="md-summary-card">
                <div class="md-summary-title">Total Deliveries</div>
                <div class="md-summary-value"><asp:Label ID="lblTotalDeliveries" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="md-summary-card">
                <div class="md-summary-title">Pending</div>
                <div class="md-summary-value"><asp:Label ID="lblPendingDeliveries" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="md-summary-card">
                <div class="md-summary-title">Assigned</div>
                <div class="md-summary-value"><asp:Label ID="lblAssignedDeliveries" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="md-summary-card">
                <div class="md-summary-title">In Transit</div>
                <div class="md-summary-value"><asp:Label ID="lblInTransitDeliveries" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="md-summary-card">
                <div class="md-summary-title">Completed</div>
                <div class="md-summary-value"><asp:Label ID="lblCompletedDeliveries" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="md-summary-card">
                <div class="md-summary-title">Failed</div>
                <div class="md-summary-value"><asp:Label ID="lblFailedDeliveries" runat="server" Text="0"></asp:Label></div>
            </div>
            <div class="md-summary-card">
                <div class="md-summary-title">Cancelled</div>
                <div class="md-summary-value"><asp:Label ID="lblCancelledDeliveries" runat="server" Text="0"></asp:Label></div>
            </div>
        </div>
        
        <div class="md-actions">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="md-filter-btn" OnClick="btnRefresh_Click" />
        </div>
    </div>
</asp:Content>