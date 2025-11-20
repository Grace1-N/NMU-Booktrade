<%@ Page Title="" Language="C#" MaintainScrollPositionOnPostBack="true" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManageGenres.aspx.cs" Inherits="NMU_BookTrade.WebForm8" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/Styles/ManageGenres.css" />
    <script>
        function showCustomModal() {
            document.getElementById('genreDeleteOverlay').style.display = 'block';
        }

        function hideCustomModal() {
            document.getElementById('genreDeleteOverlay').style.display = 'none';
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

    <h1 class="admin-heading">Manage Book Genres</h1>
    <hr />
    <br />

    <div class="Categories-Table">
        <div class="BookImage-category">
            <asp:Image ID="BookImage" CssClass="category-image" ImageUrl="~/Images/BookImage.png" runat="server" />
            <ol>
                <li>Type and add a new genre using the form.</li>
                <li>Select the category that the genre belongs to.</li>
                <li>Click 'Edit' or 'Delete' next to a genre to modify it.</li>
                <li>Changes will reflect immediately in the list below.</li>
            </ol>
        </div>

        <div class="add-wrapper">
            <asp:TextBox ID="txtGenreName" runat="server"
                         CssClass="auto-style3"
                         placeholder="Enter genre name..."
                         Width="156px" />

            <asp:RequiredFieldValidator ID="rfvGenreName" runat="server"
                ControlToValidate="txtGenreName"
                ErrorMessage="Genre name is required."
                CssClass="error-message"
                Display="Dynamic"
                ValidationGroup="AddGenre" />

            <asp:RegularExpressionValidator ID="revGenreName" runat="server"
                ControlToValidate="txtGenreName"
                ErrorMessage="Genre name should only contain letters and spaces (up to 50 characters)."
                ValidationExpression="^[a-zA-Z\s]{1,50}$"
                CssClass="error-message"
                Display="Dynamic"
                ValidationGroup="AddGenre" />

            <asp:DropDownList ID="ddlCategories" runat="server" CssClass="dropdown-categories"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvCategory" runat="server"
                ControlToValidate="ddlCategories"
                ErrorMessage="Please select a category"
                InitialValue=""
                CssClass="error-message"
                Display="Dynamic"
                ValidationGroup="AddGenre" />

            <asp:Button ID="btnAddGenre" runat="server"
                        Text="Add Genre"
                        CssClass="auto-style4"
                        OnClick="BtnAddGenre_Click"
                        Width="192px"
                        ValidationGroup="AddGenre" />

            <asp:Label ID="lblFeedback" runat="server"
                       CssClass="alert alert-info"
                       Visible="false" />
        </div>

        <h3 class="h3-categories">&nbsp;&nbsp;Existing genres listed below:</h3>
        <br />

        <!-- GRIDVIEW -->
        <asp:GridView ID="gvGenres" runat="server"
                      AutoGenerateColumns="False"
                      CssClass="manage-cat-table"
                      DataKeyNames="genreID"
                      OnRowEditing="GvGenres_RowEditing"
                      OnRowUpdating="GvGenres_RowUpdating"
                      OnRowCancelingEdit="GvGenres_RowCancelingEdit"
                      OnRowDeleting="GvGenres_RowDeleting">

            <Columns>
                <asp:TemplateField HeaderText="Genres">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditGenre" runat="server"
                                     Text='<%# Bind("genreName") %>'
                                     CssClass="edit-box" />

                        <asp:RequiredFieldValidator ID="rfvEditGenre" runat="server"
                            ControlToValidate="txtEditGenre"
                            ErrorMessage="Genre name required."
                            Display="Dynamic"
                            ForeColor="Red"
                            ValidationGroup="EditGenre" />

                        <asp:RegularExpressionValidator ID="revEditGenre" runat="server"
                            ControlToValidate="txtEditGenre"
                            ErrorMessage="Only letters and spaces are allowed."
                            ValidationExpression="^[A-Za-z\s]+$"
                            Display="Dynamic"
                            ForeColor="Red"
                            ValidationGroup="EditGenre" />
                    </EditItemTemplate>

                    <ItemTemplate>
                        <asp:Label ID="lblGenre" runat="server" Text='<%# Eval("genreName") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:CommandField ShowEditButton="True" ShowDeleteButton="True"
                                  EditText="Edit"
                                  UpdateText="Update"
                                  CancelText="Cancel"
                                  CausesValidation="true"
                                  ValidationGroup="EditGenre" />
            </Columns>
        </asp:GridView>

        <!-- DELETE MODAL -->
        <div id="genreDeleteOverlay" class="genre-modal-overlay">
            <div class="genre-modal-box">
                <h5 class="genre-modal-title">Confirm Deletion</h5>
                <asp:Label ID="lblConfirmText" runat="server" Text="" CssClass="form-control-static" />
                <div class="genre-modal-buttons">
                    <asp:Button ID="btnConfirmDelete" runat="server"
                                Text="Delete"
                                CssClass="genre-btn-delete"
                                OnClick="btnConfirmDelete_Click"
                                CausesValidation="false" />

                    <asp:Button ID="btnCancelDelete" runat="server"
                                Text="Cancel"
                                CssClass="genre-btn-cancel"
                                OnClick="btnCancelDelete_Click"
                                CausesValidation="false" />
                </div>
            </div>
        </div>

        <br />
        <br />
    </div>
</asp:Content>
