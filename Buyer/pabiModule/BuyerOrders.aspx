<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BuyerOrders.aspx.cs" Inherits="NMU_BookTrade.BuyerOrders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <div class="orders">
        <div class="orders-header">
            My Orders
                <hr class="section-line" />
                </div>
        </div>
        
        <div class="order-filter">
            <p>Orders placed in:</p>
            <asp:DropDownList ID="orderDate" runat="server" AutoPostBack="true"
                OnSelectedIndexChanged="dateSelected_SelectedIndexChanged"></asp:DropDownList>
        </div>

        <asp:Panel ID="pnlNoOrders" runat="server" CssClass="no-orders-card" Visible="false">
    <div class="no-orders-container">
         <div class="no-orders-image"> 
     <img src='<%= ResolveUrl("~/Images/No-orders.jpg") %>' alt="No Orders" class="no-orders-image" />
 </div>
        <p class="no-orders-message">
            <asp:Label ID="lblNoOrdersMessage" runat="server" Text=""></asp:Label>
        </p>
        <asp:Button ID="btnStartShopping" runat="server" Text="Start Shopping"
            CssClass="btn-start-shopping"
            OnClick="btnStartShopping_Click" />
    </div>
</asp:Panel>


        <!-- Outer repeater: Orders -->
        <asp:Repeater ID="rptOrders" runat="server"
            OnItemCommand="rptOrders_ItemCommand"
            OnItemDataBound="rptOrders_ItemDataBound">
            <ItemTemplate>
    <div class="order-summary">
        <div class="order-container">

            <!-- Delivery Status -->
            <div class="delivery-status">
                <%# Eval("deliveryDate") == DBNull.Value 
                    ? "Ordered on " + Eval("saleDate", "{0:MMM dd, yyyy}") 
                    : "Delivered on " + Eval("deliveryDate", "{0:MMM dd, yyyy}") %>
            </div>

            <!-- Book details (cover + info) -->
            <asp:Repeater ID="rptBookCovers" runat="server">
                <ItemTemplate>
                    <div class="book-item">
                        <a href='<%# "~/SearchResult.aspx?isbn=" + Eval("bookISBN") %>'>
                            <%--<asp:Image ID="imgBookCover" runat="server"
                                ImageUrl='<%# ResolveUrl(string.IsNullOrEmpty(Eval("coverImage")?.ToString()) 
                                    ? "~/Images/no-image.png" 
                                    : Eval("coverImage").ToString()) %>'
                                CssClass="cover-image" />--%>
                        </a>
                       <%-- <div class="book-info">
                            <p class="book-title"><%# Eval("title") %></p>
                            <p class="book-author"><%# "By " + Eval("author") %></p>
                            <p class="book-price">R<%# Eval("price", "{0:F2}") %></p>
                        </div>--%>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <!-- Button -->
            <asp:Button ID="btnViewDetails" runat="server"
                CommandName="ToggleDetails"
                CommandArgument='<%# Eval("orderGroupId") %>'
                Text="Order Details" CssClass="btn-view-details" />
        </div>
    </div>

    <!-- Details Panel (unchanged) -->
    <asp:Panel ID="pnlDetails" runat="server" CssClass="order-details" Visible="false">
        <div class="order-card">
            <div class="order-header">
                <strong>Order #<%# Eval("orderGroupId") %></strong> |
                Ordered: <%# Eval("saleDate", "{0:dd MMM yyyy}") %>
            </div>

            <p><strong>Shipping To:</strong> <%# Eval("buyerAddress") %></p>
            <p><strong>Payment Method:</strong> Credit/Debit Card</p>

            <div class="order-items">
                <asp:Repeater ID="rptOrderItems" runat="server">
                    <ItemTemplate>
                        <div class="order-item">
                            <img src='<%# string.IsNullOrEmpty(Eval("coverImage")?.ToString()) ? ResolveUrl("~/Images/no-image.png") : ResolveUrl(Eval("coverImage").ToString()) %>' class="order-image-large" />
                            <div>
                                <p><strong> <%# Eval("title") %> </strong></p>
                                <p><strong>Qty:</strong> <%# Eval("quantity")  %></p>                                            
                                <p><strong>Price:</strong> R<%# Eval("price", "{0:F2}") %></p>
                                <p><strong>Seller:</strong> <%# Eval("sellerName") + " " + Eval("sellerSurname") %></p>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="delivery-info-card">
                <div class="delivery-header">
                    <strong>Delivery Details</strong><br />
                    <%# Eval("deliveryDate") == DBNull.Value
                        ? "<span class='status-pending'>Delivery Pending</span>"
                        : "<span class='status-delivered'>Delivered: " + string.Format("{0:dd MMM yyyy}", Eval("deliveryDate")) + "</span>" %>
                </div>
            </div>
        </div>
    </asp:Panel>
</ItemTemplate>

        </asp:Repeater>
</asp:Content>