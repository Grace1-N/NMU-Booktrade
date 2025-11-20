<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="NMU_BookTrade.Cart"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

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
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-bd" Placeholder="Search by Title or Author..." />
            <br />
            <asp:ListBox ID="lstSuggestions" runat="server" Visible="false" Width="300px"></asp:ListBox>
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

          <asp:Panel ID="pnlSearchResults" runat="server" Visible="false">
          
    <div class="results-tile">
        <asp:Label ID="lblSearchResults" runat="server" CssClass="results-label" Font-Bold="false"></asp:Label>
         <asp:Repeater ID="rptSearchResults" runat="server">
     <ItemTemplate>
         <div class="textbook">
              <asp:LinkButton ID="lnkBookCover" runat="server" 
                CommandName="ViewBook"
                CommandArgument='<%# Eval("bookISBN") %>'>
                <img src='<%# ResolveUrl(Eval("coverImage").ToString()) %>' 
                     alt='<%# Eval("title") %>' 
                     style="cursor:pointer;" />
            </asp:LinkButton>
            <br />
            <b><%# Eval("title") %></b><br />
            R<%# Eval("price") %>
        </div>
     </ItemTemplate>
 </asp:Repeater>
        
    </div>

   
</asp:Panel> 
    <br />
    <br />

        <div class="cart-header">
    <h2>CART</h2>
    <hr class="section-line" />
</div>

<asp:Panel ID="pnlCart" runat="server" CssClass="cart-container">
<asp:Repeater ID="rptCartItems" runat="server" OnItemDataBound="rptCartItems_ItemDataBound">
        <ItemTemplate>
            <div class="cart-item">
                <img src='<%# ResolveUrl(Eval("coverImage").ToString()) %>' class="cart-item-img" />
                <div class="cart-item-details">
                    <h4><%# Eval("title") %></h4>
<p>
    <strong> Qty: </strong>
    <asp:DropDownList ID="ddlQuantity" runat="server" 
        AutoPostBack="true" 
        OnSelectedIndexChanged="ddlQuantity_SelectedIndexChanged">
    </asp:DropDownList>
    | <strong> Price: </strong><%# Eval("price") %>
</p>

<!-- hidden field to store ISBN for dropdown -->
<asp:HiddenField ID="hfBookISBN" runat="server" Value='<%# Eval("bookISBN") %>' />
                    <asp:Button ID="btnRemove" runat="server" Text="🗑️ Remove"
                    CommandArgument='<%# Eval("bookISBN") %>'

                    OnClick="btnRemove_Click"
                    CssClass="remove-btn" />
                     <div class="reviews">
         <asp:Literal ID="litStars" runat="server" />
         <asp:HyperLink ID="lnkReviews" runat="server" NavigateUrl="#reviews"></asp:HyperLink>
       </div>
                </div>
                  <div class="moreInfo">
     <div class="label"> <strong> Condition: </strong> <%# Eval("condition") %></div>
    <br />
<div class="label"> <strong> Seller: </strong> <%# Eval("sellerName") + " " + Eval("sellerSurname") %></div>
<br />
<div class="delivery">
    <strong>Delivery schedule:</strong> Delivery will take 3 - 5 business days
    <p><asp:Literal ID="litDelivery" runat="server" /></p>
</div>

      </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

  
    <div class="cart-summary">
        <strong>Total: </strong> R<asp:Label ID="lblTotal" runat="server" Text="0.00" />
    </div>

    <asp:Button ID="btnShowPayment" runat="server" Text="Proceed to Checkout" OnClick="btnShowPayment_Click" CssClass="checkout-btn" />

    <asp:Panel ID="pnlPayment" runat="server" Visible="false" CssClass="checkout-section">
        <h3>Ready to Pay?</h3>
        <asp:Label AssociatedControlID="txtName" Text="Name on Card:" runat="server"  CssClass="checkout-lbl"/>
            <span style="color:red">*</span>
        <asp:TextBox ID="txtName" runat="server" CssClass="input" />

        <asp:RequiredFieldValidator ID="rfvName" runat="server"
        ControlToValidate="txtName"
        ErrorMessage="Name on card is required."
        ForeColor="Red"
        Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revName" runat="server"
        ControlToValidate="txtName"
        ErrorMessage="Name must contain only letters, spaces, hyphens, or apostrophes (2-50 characters)."
        ValidationExpression="^[a-zA-Z\s'-]{2,50}$"
        ForeColor="Red"
        Display="Dynamic" />

        <br />
        <br />

        <asp:Label AssociatedControlID="txtCard" Text="Card Number:" runat="server" CssClass="checkout-lbl"/>
            <span style="color:red">*</span>
        <asp:TextBox ID="txtCard" runat="server" CssClass="input" />

         <asp:RequiredFieldValidator ID="rfvCard" runat="server"
        ControlToValidate="txtCard"
        ErrorMessage="Card number is required."
        ForeColor="Red"
        Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revCard" runat="server"
        ControlToValidate="txtCard"
        ErrorMessage="Card number must be 13-19 digits."
        ValidationExpression="^\d{13,19}$"
        ForeColor="Red"
        Display="Dynamic" />
    <asp:CustomValidator ID="cvCard" runat="server"
        ControlToValidate="txtCard"
        ErrorMessage="Invalid card number (failed Luhn check)."
        ForeColor="Red"
        Display="Dynamic"
        OnServerValidate="cvCard_ServerValidate" />

    <br />
    <br />

        <asp:Button ID="btnPurchase" runat="server" Text="Purchase" OnClick="btnPurchase_Click" CssClass="checkout-btn" CausesValidation="true" />
    </asp:Panel>

    <asp:Label ID="lblConfirmation" runat="server" Visible="false" ForeColor="Green" Font-Bold="true" />
</asp:Panel>

    <!-- Thank You & Order Summary Panel -->
<asp:Panel ID="pnlOrderSummary" runat="server" CssClass="thankyou-card" Visible="false">
    <div class="thankyou-header">
        <h2> Thank You for Purchasing with NMUBookTrade!</h2>
        <p>We appreciate your trust in us.</p>
    </div>

    <div class="thankyou-body">
        <div class="summary-row">
            <strong>Order Number:</strong> <asp:Label ID="lblOrderNumber" runat="server" />
        </div>
        <div class="summary-row">
            <strong>Buyer Name:</strong> <asp:Label ID="lblBuyerName" runat="server" />
        </div>
        <div class="summary-row">
            <strong>Purchase Date:</strong> <asp:Label ID="lblOrderDate" runat="server" />
        </div>
        <div class="summary-row">
            <strong>Number of Items:</strong> <asp:Label ID="lblItemCount" runat="server" />
        </div>
        <div class="summary-row">
            <strong>Total Amount:</strong> <asp:Label ID="lblOrderTotal" runat="server" />
        </div>
        <div class="summary-row">
            <strong>Estimated Delivery:</strong> <asp:Label ID="lblEstimatedDelivery" runat="server" />
        </div>
    </div>

    <div class="thankyou-footer">
        <asp:Button ID="btnGoToOrders" runat="server" Text="View My Orders" CssClass="btn-primary" PostBackUrl="~/Buyer/pabiModule/BuyerOrders.aspx" />
        <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" CssClass="btn-secondary" PostBackUrl="~/Buyer/pabiModule/SearchTextBook.aspx" />
    </div>
</asp:Panel>

    <!-- ORDER DETAILS -->
<asp:Panel ID="pnlDetails" runat="server" CssClass="order-details" Visible="false">
    <asp:Repeater ID="rptOrderDetails" runat="server">
        <ItemTemplate>
            <div class="order-card">
                <div class="order-header">
                    <strong>Order #<%# Eval("saleID") %></strong> | 
                    Ordered: <%# Eval("saleDate", "{0:dd MMM yyyy}") %> | 
                    To be delivered: <%# Eval("estimatedDelivery", "{0:dd MMM yyyy}") %>
                </div>

                <div class="order-summary">
                    <img src='<%# ResolveUrl(Eval("coverImage").ToString()) %>' class="order-thumbnail" />
                    <p class="order-title"><%# Eval("title") %></p>
                </div>

                <div class="order-info">
                    <p><strong>Shipping To:</strong> <%# Eval("buyerAddress") %></p>
                    <p><strong>Price:</strong> R<%# Eval("amount", "{0:F2}") %></p>
                    <p><strong>Qty:</strong> <%# Eval("quantity") %></p>
                    <p><strong>Seller:</strong> <%# Eval("sellerName") + " " + Eval("sellerSurname") %></p>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>


    <br />
    <br />

</asp:Content>