<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CreateListings.aspx.cs" Inherits="NMU_BookTrade.Seller.ClintonModule.CreateListings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
    <div class="cl-container">
        <h1 class="cl-title">Create Listing</h1>
        
        <p class="cl-subtitle">Ready to share your book with others?</p>
        <p class="cl-description">List your book by filling in the details below — it's quick, easy, and helps fellow readers discover their next great read!</p>

        <div class="cl-form-row">
            <label class="cl-label">Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="cl-input" placeholder="Book title"></asp:TextBox>
        </div>
        <div class="cl-form-row">
            <label class="cl-label">Author</label>
            <asp:TextBox ID="txtAuthor" runat="server" CssClass="cl-input" placeholder="Book author"></asp:TextBox>
        </div>
        <div class="cl-form-row">
            <label class="cl-label">Book ISBN</label>
            <asp:TextBox ID="txtISBN" runat="server" CssClass="cl-input" placeholder="ISBN number"></asp:TextBox>
        </div>

        <div class="cl-form-row">
            <label class="cl-label">Price</label>
            <asp:TextBox ID="txtPrice" runat="server" CssClass="cl-input" placeholder="Price"></asp:TextBox>
        </div>

        <div class="cl-form-row">
            <label class="cl-label">Category</label>
            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="cl-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                <asp:ListItem Text="Select a category" Value=""></asp:ListItem>
            </asp:DropDownList>
        </div>
        
        <div class="cl-form-row">
            <label class="cl-label">Genre</label>
            <asp:DropDownList ID="ddlGenre" runat="server" CssClass="cl-select" Enabled="false">
                <asp:ListItem Text="Select a category first" Value=""></asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="cl-form-row">
            <label class="cl-label">Condition</label>
            <asp:DropDownList ID="ddlCondition" runat="server" CssClass="cl-select">
                <asp:ListItem Text="Excellent" Value="excellent"></asp:ListItem>
                <%--<asp:ListItem Text="Very Good" Value="very-good"></asp:ListItem>
                <asp:ListItem Text="Good" Value="good" Selected="True"></asp:ListItem>--%>
                <asp:ListItem Text="Fair" Value="fair"></asp:ListItem>
                <asp:ListItem Text="Poor" Value="poor"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="cl-condition-desc">
            Select the option that best describes the state of your book
        </div>

        <div class="cl-upload-section">
           
      </div>         

            <div class="cl-upload-box">
                <div class="cl-upload-title">If the book is hard copy,</div>
                <div class="cl-upload-subtitle">add the image of the book here </div>

                <!-- ✅ Clickable upload area with FileUpload inside -->
                <asp:FileUpload ID="fuImage" runat="server" CssClass="cl-drag-area"
                                onchange="this.nextElementSibling.textContent = this.files.length > 0 ? this.files[0].name : '';" />

                <div class="cl-file-info">Supported formats: JPG and PNG</div>
                <asp:Label ID="lblFileName" runat="server" CssClass="cl-file-preview"></asp:Label>
            </div>

        <div class="cl-button-group">
            <asp:Button ID="btnSave" runat="server" Text="SAVE ▶" CssClass="cl-btn cl-btn-save" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel ▶" CssClass="cl-btn cl-btn-cancel" OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>