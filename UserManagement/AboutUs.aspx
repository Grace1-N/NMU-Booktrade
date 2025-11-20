<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AboutUs.aspx.cs" Inherits="NMU_BookTrade.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <!-- Main container -->
<div class="about-container">
    <!-- Hero section with main heading -->
    <section class="hero">
        <h1 class="reveal">This is Us</h1>
        <p class="reveal">Where textbook trade meets trust, innovation, and affordability.</p>
        <br />
        <div class="scroll-down">
            <span>Scroll down to explore</span>
           
        </div>
    </section>

    <!-- Mission section with rotating books -->
    <section class="mission reveal">
        <h2>Our Mission</h2>
        <p>
            To empower NMU students by creating a secure, efficient, and community-driven platform where textbooks get a second life.<br />
            From verified profiles to driver-based book pickup, we make textbook trading easy and safe.
        </p>

        <!-- 3D rotating book animation -->
        <div class="book-animation">
            <div class="bookcover-container">
                <!-- First book with front and back covers -->
                <div class="bookcover">
                    <div class="bookcover-style front">
                        <img src='<%: ResolveUrl("~/Images/textbook240 .png") %>' alt="Book Cover 1" />
                        <div class="bookcover-title"></div>
                    </div>
                    <div class="bookcover-style back">
                        <img src='<%: ResolveUrl("~/Images/textbook241.png") %>' alt="Book Cover 1 Back" />
                    </div>
                </div>

                <!-- Second book with front and back covers -->
                <div class="bookcover">
                    <div class="bookcover-style front">
                        <img src='<%: ResolveUrl("~/Images/textbook242.png") %>' alt="Book Cover 2" />
                        <div class="bookcover-title"></div>
                    </div>
                    <div class="bookcover-style back">
                        <img src='<%: ResolveUrl("~/Images/textbook244.png") %>' alt="Book Cover 2 Back" />
                    </div>
                </div>
           </div>

         </div>

     </section>

           <section class="features reveal">
    <h2>What We Offer</h2>
    <div class="Aboutus-cards">
        <!-- Card 1 -->
        <div class="Aboutus-card">
            
    <h3>Smart Listings</h3>
    <p>
        As a seller you can list your new or used textbooks (hardcopy)  by title, module, or author. 
        Our intelligent system helps you manage listings with ease and visibility. 
     </p>


        </div>
        <!-- Card 2 -->
        <div class="Aboutus-card">
            <h3>Trusted Delivery</h3>
            <p>Say goodbye to awkward campus meetups. Our student drivers handle collection and delivery, making your book trade stress-free and secure.</p>
        </div>
        <!-- Card 3 -->
        <div class="Aboutus-card">
            <h3>Verified Reviews</h3>
            <p>Only students who’ve completed a purchase can leave reviews. This ensures every rating is authentic, helping you choose reliable buyers and sellers.</p>
        </div>

         <!-- Card 4 -->
 <div class="Aboutus-card">
     <h3>Easy Search</h3>
     <p>As a buyer you can find any textbook quickly using our categories section and genre filters, add to your cart and purchase. When you purchase, you pay securely via EFT — no cash needed — and the buyer will receive delivery status updates via email.</p>
 </div>
    </div>
</section>

 



<!-- Team section -->
<section class="team reveal">
    <h2>Meet the Team</h2>
    <p>Driven by students, for students. From design to development, we are building with love.</p>

    <div class="team-members">
        <!-- Member 1 -->
        <div class="team-member">
            <img src='<%: ResolveUrl("~/Images/clint.jpg") %>' alt="Clinton Simbani" />
            <h3>Clinton Simbani</h3>
        </div>

        <!-- Member 2 -->
        <div class="team-member">
            <img src='<%: ResolveUrl("~/Images/pabi.jpeg") %>' alt="Paballo Nkanyane" />
            <h3>Paballo Nkanyane</h3>
        </div>

        <!-- Member 3 -->
        <div class="team-member">
            <img src='<%: ResolveUrl("~/Images/grace.png") %>' alt="Grace Manyonganise" />
            <h3>Grace Manyonganise</h3>
        </div>
    </div>
</section>


<!-- How to get started section with steps -->
<section class="get-started reveal">
    <h2>How to Get Started</h2>
    <div class="steps">
        <!-- Step 1 -->
        <div class="step" style="--i:1">
            <div class="step-number">1</div>
            <h3>Sign Up</h3>
            <p>Create your account using your student email to join our community.</p>
        </div>
        <!-- Step 2 -->
        <div class="step" style="--i:2">
            <div class="step-number">2</div>
            <h3>Browse Books</h3>
            <p>Search for textbooks by course, title, or author.</p>
        </div>
        <!-- Step 3 -->
        <div class="step" style="--i:3">
            <div class="step-number">3</div>
            <h3>Sell or Buy</h3>
            <p>List your books as a Seller, or purchase a textbook at reasonable prices from other students.</p>
        </div>
        <!-- Step 4 -->
        <div class="step" style="--i:4">
            <div class="step-number">4</div>
            <h3>Exchange</h3>
            <p>Drivers will handle the pickups and delivery for a seamless experience.</p>
        </div>
        
    </div>
    <div class="image-wrapper">
    <img src="/Images/guide.png" alt="Getting started Guide" class="guide-img" />

</div>

</section>
</div>

<section class="cta reveal">
    <h2>Join the Movement</h2>
    <p class="aboutus-p">Be part of Nelson Mandela Universities smarter student economy. Sign up,purchase a textbook or trade, and thrive.</p>
    <a class="Aboutus-btn" href="<%= ResolveUrl("~/UserManagement/Register.aspx") %>">Get Started</a>
</section>
    


















    <script>
        // Reveal elements on scroll
        document.addEventListener("DOMContentLoaded", function () {
            function reveal() {
                const reveals = document.querySelectorAll('.reveal');
                reveals.forEach(el => {
                    const windowHeight = window.innerHeight;
                    const elementTop = el.getBoundingClientRect().top;
                    const elementVisible = 150;

                    if (elementTop < windowHeight - elementVisible) {
                        el.classList.add('active');
                        el.classList.add('in-view');

                        // Extra: activate cards if inside .features
                        if (el.classList.contains('features')) {
                            const cards = el.querySelectorAll('.Aboutus-card');
                            cards.forEach(card => {
                                card.classList.add('active');
                            });
                        }
                    }
                });
            }

            window.addEventListener('scroll', reveal);
            reveal(); // run on load
        });


        // Smooth scroll for down arrow
        document.querySelector('.scroll-down')?.addEventListener('click', function () {
            window.scrollTo({
                top: window.innerHeight,
                behavior: 'smooth'
            });
        });

        // Delay animations for cards
        const cards = document.querySelectorAll('.Aboutus-card');
        let delay = 0.2;
        cards.forEach((card, index) => {
            card.style.transitionDelay = `${delay * index}s`;
        });

        // 3D tilt effect on hover
        cards.forEach(card => {
            card.addEventListener('mousemove', function (e) {
                const rect = this.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;

                const centerX = rect.width / 2;
                const centerY = rect.height / 2;

                const angleX = (y - centerY) / 20;
                const angleY = (centerX - x) / 20;

                this.style.transform = `perspective(1000px) rotateX(${angleX}deg) rotateY(${angleY}deg) translateY(-5px)`;
            });

            card.addEventListener('mouseleave', function () {
                this.style.transform = '';
            });
        });
    </script>





   
</asp:Content>

   