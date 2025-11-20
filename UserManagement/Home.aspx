<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="NMU_BookTrade.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Add any page-specific CSS or script here -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

    <!-- Hero Section -->
    <div class="hero-section">
        <div class="hero-banner">
            <asp:Image ID="imgHero" runat="server" ImageUrl="~/Images/Image1.png" CssClass="hero-image" AlternateText="Students reading" />
            <div class="hero-content">
                <h2>
                    BUY OR SELL YOUR USED TEXTBOOKS WITH FELLOW STUDENTS – FAST, SAFE, AND HASSLE-FREE.
                </h2>
                <div class="hero-icons">
                    <asp:Image ID="imgIcon1" runat="server" ImageUrl="~/Images/Image2.png" CssClass="icon" AlternateText="Money" />
                </div>
            </div>
        </div>
    </div>

<div class="slider-wrapper">
    <h3 class="section-title-slider">Check our recently uploaded textbooks</h3>

    <!-- Arrows -->
    <button class="slider-arrow left-arrow" type="button" onclick="scrollSlider(-1)">❮</button>
    <button class="slider-arrow right-arrow" type="button" onclick="scrollSlider(1)">❯</button>

    <!-- Book slider -->
    <div class="book-slider" id="bookSlider">
        <asp:Repeater ID="rptBooks" runat="server">
            <ItemTemplate>
                <div class="book-slide">
                                    <asp:LinkButton ID="lnkCover" runat="server" 
CommandName="ViewBook" 
CommandArgument='<%# Eval("bookISBN") %>'>
                <asp:Image runat="server"
                    ImageUrl='<%# ResolveUrl(Eval("coverImage").ToString()) %>'
                    CssClass="book-img" />
                    </asp:LinkButton>

                <div class="book-info">
                    <h4><%# Eval("title") %></h4>
                    <p class="book-price">R<%# Eval("price") %></p>
                    <a href='<%# ResolveUrl("~/Buyer/pabiModule/SearchResult.aspx?query=" + Eval("bookISBN")) %>' class="interested-btn">Interested</a>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    </div>
</div>



    <!-- Reviews Section -->
<div class="testimonial-section">

    <!-- Left side: text -->
    <div class="testimonial-text">
        <h2>Read what our buyers have to say about us</h2>
        <p>Over 200 buyers from diverse backgrounds trust us to help them find the right books at the best prices. We’ve helped students succeed and save money with our service.</p>
        <p>We’re proud to share what they say about us!</p>
        <asp:Button ID="btnReadStories" runat="server" 
                    CssClass="testimonial-button" 
                    Text="Read our success stories" 
                    OnClick="btnReadStories_Click" />
    </div>

    <!-- Right side: testimonial cards -->
    <div class="testimonial-cards">
        <asp:Repeater ID="rptTestimonials" runat="server">
            <ItemTemplate>
                <div class="testimonial-card">
                    <asp:Image ID="imgProfile" runat="server"
                               ImageUrl='<%# ResolveUrl("~/UploadedImages/" + Eval("buyerProfileImage")) %>'
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
        </asp:Repeater>
    </div>

</div>

    <br />
    <br />
    <br />
    <br />
    <br />


    <h3 class="section-title"> Our deliveries are always reaching students on time </h3> 

    <!-- Delivery Performance Section -->
<div class="delivery-stats-section">

    <div class="stats-left">
        <h2>Our Delivery Performance</h2>
        <p>We take pride in delivering your textbooks on time and keeping our promise to make trading seamless.</p>

          <div class="quick-stats">
            <div class="stat-block">
                <h3><asp:Label ID="lblTotalDeliveries" runat="server" Text="0"></asp:Label>+</h3>
                <p>Deliveries Completed</p>
            </div>
         </div>
        <br />
            <div class="stat-block">
                <h3>98%</h3>
                <p>On-Time Delivery Rate</p>
            </div>
        </div>
     <img src="<%= ResolveUrl("~/Images/map.png") %>" alt="Delivery Map" class="delivery-map" />
    </div>

    <div class="stats-right">
   

</div>



    <br />
    <br />















   <script>
       function scrollSlider(direction) {
           const slider = document.getElementById('bookSlider');
           const scrollAmount = 280; // Approximate width of one book card
           slider.scrollBy({
               left: direction * scrollAmount,
               behavior: 'smooth'
           });
       }
   </script>



</asp:Content>

