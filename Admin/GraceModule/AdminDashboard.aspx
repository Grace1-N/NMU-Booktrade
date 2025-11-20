<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="NMU_BookTrade.AdminDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

     <!-- Admin Welcome Section -->
<div class="admin-card admin-welcome-card">
  <h1 class="admin-welcome-title">
    Welcome, <asp:Label ID="lblCustomerName" runat="server"></asp:Label>!
  </h1>
  <p class="admin-welcome-text">
    On this dashboard, you can manage and oversee the reviews buyers have placed after their purchases, 
    and the drivers that were hired to deliver the textbooks.
  </p>
</div>

<!-- Admin Remove Reviews Section -->
<div class="admin-card admin-review-card">
  <h2 class="admin-section-heading">Remove Inappropriate Reviews</h2>
  <p class="admin-paragraph">
    To manage the <span class="admin-highlight">inappropriate reviews</span> from the buyers, 
    <a href="ManageReviews.aspx" class="admin-link">
      click here <i class="fa fa-external-link" aria-hidden="true"></i>
    </a>.
  </p>
</div>
    <br />
    <br />
    <br />
     <h2 class="section-heading">Available Drivers</h2>
   <p class="aa-paragraph">On this section you can view the drivers that were hired, click on the cards and manage their profiles.</p>
  
    <br />
    <br />
<div class="driver-grid">
    <asp:Repeater ID="rptDrivers" runat="server">
        <ItemTemplate>
            <div class="driver-card" onclick="location.href='<%# Eval("driverID", "ViewDriver.aspx?id={0}") %>'">
                <img src='<%# ResolveUrl("~/UploadedImages/" + Eval("driverProfileImage")) %>' alt="Driver" class="driver-img" />
                <div class="driver-name"><%# Eval("driverName") %> <%# Eval("driverSurname") %></div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <!-- Add Driver Card -->
    <div class="driver-card add-card" onclick="location.href='AddDriver.aspx'">
        <div class="add-icon">+</div>
        <div class="driver-name">Add Driver</div>
    </div>
</div>

  
    

    

</asp:Content>
