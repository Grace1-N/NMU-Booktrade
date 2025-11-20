<%@ Page Title="Delivery Schedule" Language="C#" MasterPageFile="~/Site1.Master" 
    AutoEventWireup="true" CodeBehind="ViewSchedule.aspx.cs" 
    Inherits="NMU_BookTrade.Driver.ClintonModule.ViewSchedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle_section" runat="server">
     <div class="vs-container">
     <!-- Header Section -->
     <div class="vs-header">
         <h1 class="vs-title">My Delivery Schedule</h1>
         <div class="vs-date-navigation">
             <asp:LinkButton ID="lnkPrevWeek" runat="server" CssClass="vs-nav-btn" OnClick="lnkPrevWeek_Click">
                 <i class="fas fa-chevron-left"></i> Previous Week
             </asp:LinkButton>
             <h2 class="vs-current-week"><asp:Label ID="lblWeekRange" runat="server" /></h2>
             <asp:LinkButton ID="lnkNextWeek" runat="server" CssClass="vs-nav-btn" OnClick="lnkNextWeek_Click">
                 Next Week <i class="fas fa-chevron-right"></i>
             </asp:LinkButton>
         </div>
     </div>

     <!-- Error Message -->
     <asp:Label ID="lblErrorMessage" runat="server" CssClass="alert alert-danger" Visible="false" />

     <!-- Filters -->
     <div class="vs-filters">
         <div class="vs-filter-group">
             <asp:Button ID="btnPrintSchedule" runat="server" Text="Print Schedule" 
                 CssClass="vs-action-btn" OnClick="btnPrintSchedule_Click" />
             <asp:Button ID="btnExportCSV" runat="server" Text="Export CSV" 
                 CssClass="vs-action-btn vs-secondary-btn" OnClick="btnExportCSV_Click" />
         </div>
     </div>

     <!-- Schedule Display -->
     <div class="vs-schedule-container">
         <asp:Repeater ID="rptDays" runat="server" OnItemDataBound="rptDays_ItemDataBound">
             <ItemTemplate>
                 <div class="vs-day-section">
                     <h3 class="vs-day-header">
                         <%# Eval("DayName") %>
                         <span class="vs-date"><%# ((DateTime)Eval("Date")).ToString("MMM dd") %></span>
                     </h3>
                     
                     <asp:Repeater ID="rptTimeSlots" runat="server" OnItemDataBound="rptTimeSlots_ItemDataBound">
                         <ItemTemplate>
                             <div class="vs-time-slot-group">
                                 <h4><%# Eval("TimeSlot") %></h4>
                                 <div class="vs-deliveries-container">
                                     <asp:Repeater ID="rptDeliveries" runat="server">
                                         <ItemTemplate>
                                             <div class="vs-delivery-card" style='<%# GetDeliveryCardStyle(Eval("Status")) %>'>
                                                 <div class="vs-delivery-id">#<%# Eval("DeliveryID") %></div>
                                                 <div class="vs-delivery-book"><%# Eval("BookTitle") %></div>
                                                 <div class="vs-delivery-location"><%# Eval("Location") %></div>
                                                 <div class="vs-delivery-status"><%# Eval("Status") %></div>
                                             </div>
                                         </ItemTemplate>
                                     </asp:Repeater>
                                 </div>
                             </div>
                         </ItemTemplate>
                     </asp:Repeater>
                 </div>
             </ItemTemplate>
         </asp:Repeater>
     </div>

     <!-- Legend -->
     <div class="vs-legend">
         <div class="vs-legend-title">Status Legend:</div>
         <div class="vs-legend-item"><span class="vs-legend-color pending"></span> Pending</div>
         <div class="vs-legend-item"><span class="vs-legend-color assigned"></span> Assigned</div>
         <div class="vs-legend-item"><span class="vs-legend-color transit"></span> In Transit</div>
         <div class="vs-legend-item"><span class="vs-legend-color completed"></span> Completed</div>
         <div class="vs-legend-item"><span class="vs-legend-color failed"></span> Failed</div>
         <div class="vs-legend-item"><span class="vs-legend-color cancelled"></span> Cancelled</div>
     </div>
 </div>
</asp:Content>