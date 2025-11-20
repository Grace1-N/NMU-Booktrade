<%@ Page Title="Inbox" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Inbox.aspx.cs" Inherits="NMU_BookTrade.WebForm9" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="inbox.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Services>
        <asp:ServiceReference Path="Inbox.aspx" />
    </Services>
</asp:ScriptManagerProxy>



    <div class="inbox-container">
        <!-- Header -->
        <div class="inbox-header">
            <h1>Admin Inbox</h1>
            <p>Manage communications from all users</p>
        </div>

        <!-- Layout -->
        <div class="inbox-layout">
            <!-- Sidebar -->
            <div class="inbox-sidebar">
                <div class="inbox-sidebar-box">
                    <button class="inbox-compose-btn" onclick="openCompose(event)">Compose</button>

                    <div class="inbox-nav-links">
                      <a id="inboxLink" href="#" class="inbox-nav-link active" onclick="showInboxSection()">📥 Inbox
                          
                           <span class="inbox-badge" runat="server" id="inboxCountSpan" ClientIDMode="Static"></span>
                          <!-- badge shows the count or number of unread messages-->


                      </a>

                             <a id="sentLink" href="#" class="inbox-nav-link" onclick="showSentSection()">📤 Sent</a>     

                    </div>
                </div>
            </div>

            <!-- Main Content Shown -->
            <div class="inbox-main-content">

                 <!-- Preview Panel -->
                     <div class="inbox-preview-panel" id="messagePreviewPanel" style="display:none;">
                         <div class="inbox-preview-header">
                             <div>
                                 <h2 class="inbox-preview-title" id="previewSubject"></h2>

                                 <div class="inbox-preview-meta" id="previewSender" ></div> 

                                 <div class="inbox-preview-meta" id="previewTime"></div>
                             </div>
                             <div class="inbox-preview-actions">
                              

                             </div>
                         </div>
                         <div class="inbox-preview-body" id="previewBody"></div>

                         <div class="inbox-preview-footer">
                             <button type="button" id="replyMessage" class="inbox-preview-btn primary"
                                    data-id="" data-sender="" data-email="" onclick="replyToMessage(this)">Reply</button>

                                                        
                         </div>
                     </div>


                <!-- Search bar and Refresh -->
                <div class="inbox-actions-bar">
                    <div class="inbox-search-box">
                        <input type="text" id="searchInput" class="inbox-search-input" placeholder="Search messages..." onkeyup="applySearchFilter()">
                        <span class="inbox-search-icon">🔍</span>
                    </div>
                    <div class="inbox-action-buttons">
                        
                        <button class="inbox-action-btn" onclick="location.reload()">🔄 Refresh</button>
                    </div>
                </div>

               
                <!-- Inbox Section NEW --> 
                <div id="inboxSection" class="inbox-messages-list">
                   <asp:Repeater ID="rptInbox" runat="server" OnItemCommand="rptInbox_ItemCommand">
                                                                                 <ItemTemplate>
        <div class="inbox-message-row <%# Convert.ToBoolean(Eval("isRead")) ? "read" : "unread" %>"
             data-id='<%# Eval("messageID") %>'
             data-sender='<%# Eval("senderEmail") %>'                     
             data-time='<%# string.Format("{0:MMM dd, yyyy hh:mm tt}", Eval("dateSent")) %>'
             data-body='<%# HttpUtility.HtmlEncode(Eval("messageContent")) %>'
             data-role='<%# (Eval("senderEmail").ToString() == "gracamanyonganise@gmail.com")   %>'
             data-read='<%# Eval("isRead") %>'
             data-email='<%# Eval("senderEmail") %>'
             ">
          
            <div class="message-header">
                From: <%# Eval("senderEmail") %>
                <span class="message-time"><%# string.Format("{0:MMM dd, yyyy}", Eval("dateSent")) %></span>

                <!-- Delete button -->
                <asp:Button ID="btnDeleteInbox" runat="server" Text="🗑️"
                    CommandName="DeleteMessage"
                    CommandArgument='<%# Eval("messageID") %>'
                    CssClass="inbox-preview-action-btn"
                    OnClientClick="event.stopPropagation(); return confirm('Delete this message?');"
                    CausesValidation="false" />
            </div>

            <div class="message-snippet">
                <%# Eval("messageContent").ToString().Length > 50 
                    ? Eval("messageContent").ToString().Substring(0, 50) + "..." 
                    : Eval("messageContent") %>
            </div>
        </div>
    </ItemTemplate>

                 </asp:Repeater>

                </div>

                <!-- Sent Section (hidden by default) NEW -->
                <div id="sentSection" class="inbox-messages-list" style="display: none;">
                    <asp:Repeater ID="rptSent" runat="server" OnItemCommand="rptSent_ItemCommand">
                                        <ItemTemplate>
        <div class="inbox-message-row read"
             data-id='<%# Eval("messageID") %>'
             data-sender="Me (Admin)"
             data-time='<%# string.Format("{0:MMM dd, yyyy hh:mm tt}", Eval("dateSent")) %>'
             data-body='<%# HttpUtility.HtmlEncode(Eval("messageContent")) %>'
             data-role="admin"
             data-read="1"
             data-email='<%# Eval("senderEmail") %>'
            ">

            <div class="message-header">
                From: <%# Eval("senderEmail") %>
                <span class="message-time"><%# string.Format("{0:MMM dd, yyyy}", Eval("dateSent")) %></span>

                <!-- Delete button -->
                <asp:Button ID="btnDeleteSent" runat="server" Text="🗑️"
                    CommandName="DeleteMessage"
                    CommandArgument='<%# Eval("messageID") %>'
                    CssClass="inbox-preview-action-btn"
                    OnClientClick="event.stopPropagation(); return confirm('Delete this message?');"
                    CausesValidation="false" />
            </div>

            <div class="message-snippet">
                <%# Eval("messageContent").ToString().Length > 50 
                    ? Eval("messageContent").ToString().Substring(0, 50) + "..." 
                    : Eval("messageContent") %>
            </div>
        </div>
    </ItemTemplate>
                        </asp:Repeater>

                </div>



               
            </div>
        </div>
    </div>

    <!-- Compose Modal -->
    <div class="compose-modal" id="composeModal">
        <div class="compose-content">
            <h2>Compose New Message</h2>
            <input type="text" id="composeTo" placeholder="To (email)">
            <input type="text" id="composeSubject" placeholder="Subject">
            <textarea id="composeBody" rows="5" placeholder="Type your message..."></textarea>
            <div class="compose-buttons">
                <button type="button" onclick="sendComposedMessage()">Send</button>
                <button type="button" onclick="closeCompose()">Cancel</button>
            </div>
        </div>
    </div>











<script>
    
    // Function to handle selecting a message and displaying its preview
    function selectMessage(element) {
        document.querySelectorAll('.inbox-message-row').forEach(msg => msg.classList.remove('selected')); // here we are selecting all html elements with the class inbox-message-row 
        element.classList.add('selected'); 
        let selectedMessageId = element.dataset.id;   

        //deletion icon 
        document.getElementById("delete-SingleMessage").setAttribute("data-id", selectedMessageId);

        document.getElementById("delete-SingleMessage").setAttribute("data-sender", element.dataset.sender);

        // updating reply button

        document.getElementById("replyMessage").setAttribute("data-id", selectedMessageId);
        document.getElementById("replyMessage").setAttribute("data-email", element.dataset.email);
        document.getElementById("replyMessage").setAttribute("data-sender", element.dataset.sender);

               
        document.getElementById("previewSubject").innerText = "Message";
                   
        
        document.getElementById("previewTime").innerText = element.dataset.time; 
        document.getElementById("previewBody").innerHTML = element.dataset.body;
        document.getElementById("messagePreviewPanel").style.display = "block"; 
        // Check if role is "admin" (i.e., sent by me)
        const role = element.dataset.role;
        // Remove that block entirely OR replace it with a safe check:
        if (role === "admin") {
            console.log("This message was sent by the admin.");
        }



        // checking if message has been read
        //if (element.dataset.read === "0") {
            // telling the server to mark as read
            fetch("Inbox.aspx/MarkMessageAsRead", {
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ messageID: parseInt(selectedMessageId) })
            })
                .then(res => res.json())
                .then(data => {
                    // Update UI immediately
                    //element.classList.add("read");
                    //element.dataset.read = "1"; // Mark as read locally
                   
                    updateInboxCount();         // Decrease the counter
                })
                .catch(err => console.error("Mark read failed:", err));
        //}


    }



    function LoadSentMessages() { 
        showSentSection();
        fetch("Inbox.aspx/LoadSentMessages",  {
        method: "POST",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    })
        .then(res => res.text())
        .then(data => { 
             
        })
        .catch(err => console.error("Failed to load sent messages:", err)); 
    }

    function updateInboxCount() {
        fetch("Inbox.aspx/GetUnreadMessageCount", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({})
        })
            .then(res => res.json())
            .then(data => { 
                const badge = document.getElementById("inboxCountSpan");
                if (badge) {
                    badge.innerText = data.d;
                }
            });
    }

    function updateInboxCount2() {
        PageMethods.GetUnreadMessageCount(
            function (response) { // success callback
                const badge = document.getElementById("inboxCountSpan");
                if (badge) {
                    badge.innerText = response;
                }
            },
            function (error) { // error callback
                console.error("Error fetching inbox count:", error);
            }
        );
    }




    // Function to apply the search filter
    function applySearchFilter() {
        const keyword = document.getElementById("searchInput").value.toLowerCase();
        document.querySelectorAll(".inbox-message-row").forEach(row => {
            const text = row.innerText.toLowerCase();
            row.style.display = text.includes(keyword) ? '' : 'none';
        });
    }

    // Function to open the compose modal
    function openCompose(e) {
        if (e) e.preventDefault();
        document.getElementById("composeModal").style.display = "block";
    }

    // Function to close the compose modal
    function closeCompose() {
        document.getElementById("composeModal").style.display = "none";
    }

    // Function to send composed message
    function sendComposedMessage() {
        const to = document.getElementById("composeTo").value;
        const subject = document.getElementById("composeSubject").value;
        const body = document.getElementById("composeBody").value;

        if (to && subject && body) {
            fetch("Inbox.aspx/SendEmail", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ to, subject, body })
            })
                .then(res => res.json())
                .then(data => {
                    if (data.d.includes("successfully")) {
                        // Also save to database
                        fetch("Inbox.aspx/SendMessage", {
                            method: "POST",
                            headers: { "Content-Type": "application/json" },
                            body: JSON.stringify({ to, subject, body })
                        })
                            .then(res => res.json())
                            .then(() => {
                                alert("Message Sent Successfully")
                                // LoadSentMessages();
                                sessionStorage.setItem('sentMessages', 1);
                                closeCompose();
                                location.reload();
                            });
                    } else {
                        alert("Failed to send email: " + data.d);
                    }
                });

        } else {
            alert("Please complete all fields.");
        }
    }
     
     

    function replyToMessage(e) {
        let selectedMessageId = e.dataset.id;   // same as delete
        let selectedSender = e.dataset.sender;  // sender info
        let senderEmail = e.dataset.email;      // optional if you stored email

        if (!selectedMessageId) return;

        openCompose();
        document.getElementById("composeTo").value = senderEmail || "";
        document.getElementById("composeSubject").value = "RE: " + document.getElementById("previewSubject").innerText;
        document.getElementById("composeBody").value = "";
    }


    

    window.onload = function () {
        updateInboxCount();
        let sentMessages = sessionStorage.getItem('sentMessages');
        if (sentMessages == 1) {
            showSentSection();
        }
    };

    function showSentSection() {
        
        
            document.getElementById("inboxSection").style.display = "none";
            document.getElementById("sentSection").style.display = "block";
            document.getElementById("messagePreviewPanel").style.display = "none";
            document.getElementById("sentLink").classList.add("active");
            document.getElementById("inboxLink").classList.remove("active");
            sessionStorage.setItem('sentMessages',0);
      
    }

    function showInboxSection() {
        document.getElementById("sentSection").style.display = "none";
        document.getElementById("inboxSection").style.display = "block";
        document.getElementById("messagePreviewPanel").style.display = "none";

        document.getElementById("inboxLink").classList.add("active");
        document.getElementById("sentLink").classList.remove("active");
    }

    window.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.inbox-message-row').forEach(row => {
            row.addEventListener('click', function () {
                document.querySelectorAll('.inbox-message-row').forEach(msg => msg.classList.remove('selected'));
                row.classList.add('selected');

                let selectedMessageId = row.dataset.id;

                // Update reply button
                document.getElementById("replyMessage").dataset.id = selectedMessageId;
                document.getElementById("replyMessage").dataset.email = row.dataset.email;
                document.getElementById("replyMessage").dataset.sender = row.dataset.sender;

                
                // Update preview panel content
                document.getElementById("previewSubject").innerText = "Message";
                document.getElementById("previewSender").innerText = row.dataset.sender + " (" + row.dataset.role + ")";
                document.getElementById("previewTime").innerText = row.dataset.time;
                document.getElementById("previewBody").innerHTML = row.dataset.body;

                // Show preview panel
                document.getElementById("messagePreviewPanel").style.display = "block";

                // Mark as read if unread
                //if (row.dataset.read === "0") {
                    fetch("Inbox.aspx/MarkMessageAsRead", {
                        method: "POST",
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ messageID: parseInt(selectedMessageId) })
                    })
                        .then(res => res.json())
                        .then(() => {
                            //row.classList.add("read");
                            //row.dataset.read = "1";
                            
                            updateInboxCount();
                    });
                //}
            });
        });
    });

   
</script>

</asp:Content>
