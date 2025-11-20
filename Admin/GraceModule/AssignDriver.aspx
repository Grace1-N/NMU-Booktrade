<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AssignDriver.aspx.cs" Inherits="NMU_BookTrade.Admin.GraceModule.WebForm14"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">

     <h2 class="assigned-heading">Assign the drivers:</h2>

 <p class="assigned-paragraph">Every driver will get an assignment, with a specified date and time from you.<br /> If a purchase has been made, the table below will show buyer and seller details. <br /> Make sure you pick the date and time, driver and click assign. They will receive the assignment/job, in their delivery schedule.
 </p>
    <br />


    


    

    <asp:GridView ID="gvDeliveries" runat="server" AutoGenerateColumns="false"
    OnRowCommand="gvDeliveries_RowCommand"
    OnRowDataBound="gvDeliveries_RowDataBound"
    DataKeyNames="deliveryID"
    CssClass="assigned-grid"
    GridLines="None">
    
  

    <HeaderStyle CssClass="table-header" />
    <RowStyle CssClass="table-row" />
    <AlternatingRowStyle CssClass="table-row-alt" />

    <Columns>
        <asp:BoundField DataField="title" HeaderText="Book" />
        <asp:BoundField DataField="sellerName" HeaderText="Seller" />
        <asp:BoundField DataField="buyerName" HeaderText="Buyer" />
        <asp:BoundField DataField="sellerAddress" HeaderText="Pickup Address" />
       <asp:TemplateField HeaderText="Delivery Address">
            <ItemTemplate>
                <asp:Label ID="lblBuyerAddress" runat="server" Text='<%# Eval("buyerAddress") %>' />
            </ItemTemplate>
        </asp:TemplateField>



        <asp:TemplateField HeaderText="Driver">
            <ItemTemplate>
                <asp:DropDownList ID="ddlDrivers" runat="server" CssClass="driver-dropdown"></asp:DropDownList>
            </ItemTemplate>

        </asp:TemplateField>

        <asp:TemplateField HeaderText="Delivery Date">
            

        <ItemTemplate>
           <asp:TextBox ID="txtDeliveryDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>


        </ItemTemplate>



</asp:TemplateField>


        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:Button ID="btnAssign" runat="server" Text="Assign" CommandName="AssignDriver"
                    CommandArgument='<%# Eval("deliveryID") %>' CssClass="assign-btn"
                    OnClientClick="return confirm('Are you sure you want to assign this driver?');" />
            </ItemTemplate>
        </asp:TemplateField>

        
    </Columns>
</asp:GridView>

      

    <br />
    <br />


    <h2 class="assigned-heading">Drivers You've Assigned:</h2>
    
    <p class="assigned-paragraph"> Keep track and view all the drivers you have assigned right here.</p>
    <br />

<asp:GridView ID="gvAssignedDrivers" runat="server" AutoGenerateColumns="false"
    OnRowDataBound="gvAssignedDrivers_RowDataBound"
    CssClass="assigned-grid">

    <Columns>
        <asp:BoundField DataField="BookTitle" HeaderText="Book" />
        <asp:BoundField DataField="SellerName" HeaderText="Seller" />
        <asp:BoundField DataField="BuyerName" HeaderText="Buyer" />
        <asp:BoundField DataField="DriverName" HeaderText="Assigned Driver" />
        <asp:BoundField DataField="deliveryDate" HeaderText="Delivery Date" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />

    </Columns>
</asp:GridView>



</asp:Content>
