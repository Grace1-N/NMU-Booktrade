<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SearchTextBook.aspx.cs" Inherits="NMU_BookTrade.SearchTextBook" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
  <div class="search-wrapper">
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
</div>

      <asp:Panel ID="pnlSearchResults" runat="server" Visible="false">
          
    <div class="results-tile">
        <asp:Label ID="lblSearchResults" runat="server" CssClass="results-label" Font-Bold="false"></asp:Label>
<asp:LinkButton ID="lnkViewAllResults" runat="server" CssClass="view-all-link" OnClick="lnkViewAllResults_Click">
    View all results <i class="fas fa-arrow-right"></i>
</asp:LinkButton>
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
  <div class="results-grid">
    <asp:Repeater ID="rptOutNow" runat="server" OnItemCommand="rptOutNow_ItemCommand">
        <ItemTemplate>
            <div class="textbook">
                <asp:LinkButton ID="lnkCover" runat="server" 
    CommandName="ViewBook" 
    CommandArgument='<%# Eval("bookISBN") %>'>
                <img src='<%# ResolveUrl(Eval("coverImage").ToString()) %>' /><br />
                    </asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:Repeater>
            <div class="out-now">OUT <br />NOW!</div>
</div>

        <div class="section-title">Recently Added Textbooks!</div>
         <hr class="section-line" />
           <asp:Repeater ID="rptRecentlyAdded" runat="server" OnItemCommand="rptRecentlyAdded_ItemCommand">
    <ItemTemplate>
        <asp:LinkButton ID="lnkCover" runat="server" 
            CommandName="ViewBook" 
            CommandArgument='<%# Eval("bookISBN") %>'>
            <div class="textbook">
                <img src='<%# ResolveUrl(Eval("coverImage").ToString()) %>' alt="Book Cover" />
                <br />
                <b><%# Eval("title") %></b>
            </div>
        </asp:LinkButton>
    </ItemTemplate>
</asp:Repeater>
</asp:Content>
