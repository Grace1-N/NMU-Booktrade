<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BookConditions.aspx.cs" Inherits="NMU_BookTrade.WebForm3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <div class="BCcontainer">
        <div class="BCcard">
            <!-- Header Block -->
            <div class="BCheader">
                <h1 class="BCh1">NMU Textbook Exchange - Book Condition Guidelines</h1>
            </div>

            <!-- Main Section -->
            <h2 class="BCh2">CONDITION OF TEXTBOOKS</h2>

            <div class="BCcondition">
                <h3>Excellent</h3>
                <p>Describes the condition of a used book that shows minimal wear. There are no torn or missing pages, and no writing or highlighting throughout the book. The book appears almost new with minor signs of previous use.</p>
            </div>

            <div class="BCcondition">
                <h3>Fair</h3>
                <p>Describes the condition of a used book that shows normal wear from previous use. There are no large tears or missing pages, and it may contain limited writing or highlighting (maximum 20% of the book). The binding is intact and sturdy.</p>
            </div>

            <div class="BCcondition">
                <h3>Poor</h3>
                <p>Describes the condition of a used book that shows significant wear but remains usable. There are no missing pages and it may have considerable writing or highlighting, but the text must not be obscured. The binding may show signs of wear but remains intact.</p>
            </div>

            <!-- Important Note -->
            <div class="BCnote">
                <p><strong>Note:</strong> NMU Textbook Exchange does not facilitate the buying or selling of books in very poor condition where pages are missing or text is obscured. Please do not list these books on our platform.</p>
            </div>

            <!-- Not Accepted List -->
           <div class="not-accepted">
                <h3>NMU Textbook Exchange will not accept items with any of the following defects:</h3>
                <ul class="rejected-list">
                    <li>Fire or water damage</li>
                    <li>Missing or torn pages</li>
                    <li>Broken binding</li>
                    <li>Strong odors</li>
                    <li>Mold or mildew</li>
                    <li>Completed workbooks</li>
                    <li>Pirated or copied books</li>
                    <li>Heavily stained pages</li>
                </ul>
            </div>

            <!-- Final Note -->
            <div class="BCnote">
                <p>These guidelines help ensure that NMU students receive quality textbooks when purchasing from other students. By maintaining these standards, we create a reliable platform for textbook exchange within our campus community.</p>
            </div>
        </div>
    </div>

    <!-- FAQ Section -->
<div class="BCfaq fade-in-section">
    <h2 class="BCh2">Frequently Asked Questions (FAQs)</h2>
    <div class="faq-item">
        <h4>Q: Can I sell digital textbooks?</h4>
        <p>A: No, the NMU BookTrade platform only facilitates physical textbook exchanges at this time.</p>
    </div>
    <div class="faq-item">
        <h4>Q: How do I know if my textbook qualifies as 'Fair' or 'Poor'?</h4>
        <p>Refer to the seller’s book conditions and compare their list with the list on the 
       <asp:HyperLink ID="hlBookConditions" runat="server" NavigateUrl="~/UserManagement/BookConditions.aspx" Text="Book Conditions page" CssClass="italics-bookconditions"  Target="_blank" />. Fair books have limited wear with <strong>less than 20% highlighting</strong>, while Poor books show wear but are still usable and intact.</p>
    </div>
    <div class="faq-item">
        <h4>Q: What happens if I list a book with defects?</h4>
        <p>A: Listings that do not meet the quality guidelines on the   <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/UserManagement/BookConditions.aspx" Text="Book Conditions page" CssClass="italics-bookconditions"  Target="_blank" /> may be removed to protect buyer experience.</p>
    </div>
    <div class="faq-item">
        <h4>Q: Can I meet the buyer on campus?</h4>
        <p>A:No. Once the book has been purchased, a driver is assigned to deliver the textbook to you.You will receive a noticfication on your email or phone number letting you know that your book will be delivered on the day.
           

    And once the book is listed. Transactions happen between the system and the buyer. Not the Seller and the buyer.<br />

            <br />
            <br />
            <span class="BCnote"> NOTE:</span> We make sure that our books that we sell, which are listed have met the standard guidelines on the <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/UserManagement/BookConditions.aspx" Text="Book Conditions page" CssClass="italics-bookconditions"  Target="_blank" />          
        </p>
    </div>
</div>

<!-- Privacy Policy Section -->
<div class="BCprivacy fade-in-section">
    <h2 class="BCh2">Privacy Policy</h2>
    <p>We value your privacy. NMU BookTrade collects only the necessary information to support textbook exchanges. Your data will not be sold, shared, or used for third-party marketing. All transactions remain between the system and students, and user information is protected under South African data protection regulations (POPIA).</p>
</div>

    <br />
    <br />












    <script>
    const faders = document.querySelectorAll('.fade-in-section');

    const appearOptions = {
        threshold: 0.2,
        rootMargin: "0px 0px -20px 0px"
    };

    const appearOnScroll = new IntersectionObserver(function(entries, appearOnScroll) {
        entries.forEach(entry => {
            if (!entry.isIntersecting) {
                return;
            } else {
                entry.target.classList.add('is-visible');
                appearOnScroll.unobserve(entry.target);
            }
        });
    }, appearOptions);

    faders.forEach(fader => {
        appearOnScroll.observe(fader);
    });
    </script>

        
</asp:Content>
