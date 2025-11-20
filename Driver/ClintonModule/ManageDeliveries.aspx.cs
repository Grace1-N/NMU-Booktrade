using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.Driver.ClintonModule
{
    public partial class ManageDeliveries : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Updated session-based authentication check for drivers
                if (Session["AccessID"] == null || Session["AccessID"].ToString() != "4" || Session["DriverID"] == null)
                {
                    Response.Redirect("~/UserManagement/Login.aspx");
                    return;
                }
                else
                {
                    // Set a broader date range to show more deliveries
                    string today = DateTime.Today.ToString("yyyy-MM-dd");
                    string nextWeek = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd");
                    txtStartDate.Attributes["min"] = today;
                    txtEndDate.Attributes["min"] = today;
                    txtStartDate.Text = today;
                    txtEndDate.Text = nextWeek;
                    
                    // Test database connection for debugging
                    TestDatabaseConnection();
                    
                    LoadDeliveries();
                    UpdateSummary();
                }
            }
        }

        private void LoadDeliveries()
        {
            DataTable deliveries = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    // First, let's check if we have any deliveries at all
                    string checkQuery = "SELECT COUNT(*) FROM Delivery";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        int totalDeliveries = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (totalDeliveries == 0)
                        {
                            // No deliveries in the database
                            gvDeliveries.DataSource = deliveries;
                            gvDeliveries.DataBind();
                            return;
                        }
                    }

                    // Check if there are any deliveries for this specific driver
                    string driverCheckQuery = "SELECT COUNT(*) FROM Delivery WHERE driverID = @DriverID";
                    using (SqlCommand driverCheckCmd = new SqlCommand(driverCheckQuery, connection))
                    {
                        driverCheckCmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        int driverDeliveries = Convert.ToInt32(driverCheckCmd.ExecuteScalar());
                        if (driverDeliveries == 0)
                        {
                            // No deliveries for this driver
                            gvDeliveries.DataSource = deliveries;
                            gvDeliveries.DataBind();
                            return;
                        }
                    }

                    string query = @"SELECT 
                                    d.deliveryID AS DeliveryID, 
                                    ISNULL(b.title, 'Unknown Book') AS BookTitle, 
                                    ISNULL(seller.sellerName, 'Unknown Seller') AS SellerName, 
                                    ISNULL(buyer.buyerName, 'Unknown Buyer') AS BuyerName,
                                    ISNULL(seller.sellerAddress, 'No Address') AS PickupAddress, 
                                    ISNULL(buyer.buyerAddress, 'No Address') AS DeliveryAddress,
                                    CASE 
                                        WHEN d.status = 0 THEN 'Pending'
                                        WHEN d.status = 1 THEN 'Assigned'
                                        WHEN d.status = 2 THEN 'In Transit'
                                        WHEN d.status = 3 THEN 'Completed'
                                        WHEN d.status = 4 THEN 'Failed'
                                        WHEN d.status = 5 THEN 'Cancelled'
                                        ELSE 'Pending'
                                    END AS Status,
                                    ISNULL(drv.driverName + ' ' + drv.driverSurname, 'Not Assigned') AS DriverName,
                                    d.deliveryDate AS ScheduledDate
                                    FROM Delivery d
                                    LEFT JOIN Sale s ON d.saleID = s.saleID
                                    LEFT JOIN Book b ON s.bookISBN = b.bookISBN
                                    LEFT JOIN Seller seller ON s.sellerID = seller.sellerID
                                    LEFT JOIN Buyer buyer ON s.buyerID = buyer.buyerID
                                    LEFT JOIN Driver drv ON d.driverID = drv.driverID
                                    WHERE d.deliveryDate BETWEEN @StartDate AND @EndDate 
                                    AND d.driverID = @DriverID";

                    // Apply status filter if not "all"
                    if (ddlStatus.SelectedValue != "all")
                    {
                        query += " AND (";
                        switch (ddlStatus.SelectedValue)
                        {
                            case "pending":
                                query += "d.status = 0";
                                break;
                            case "assigned":
                                query += "d.status = 1";
                                break;
                            case "transit":
                                query += "d.status = 2";
                                break;
                            case "completed":
                                query += "d.status = 3";
                                break;
                            case "failed":
                                query += "d.status = 4";
                                break;
                            case "cancelled":
                                query += "d.status = 5";
                                break;
                        }
                        query += ")";
                    }

                    query += " ORDER BY d.deliveryDate DESC";

                    // Validate and clamp dates to today or later
                    DateTime startDate, endDate;
                    GetValidatedDateRange(out startDate, out endDate);

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(deliveries);
                        }
                    }
                }

                // Debug: Log the number of rows returned
                if (deliveries.Rows.Count == 0)
                {
                    ShowAlert("No deliveries found for the selected criteria. Please check your date range and status filter.");
                }

                gvDeliveries.DataSource = deliveries;
                gvDeliveries.DataBind();
            }
            catch (Exception ex)
            {
                // Log the error or show a user-friendly message
                ShowAlert($"Error loading deliveries: {ex.Message}");
            }
        }

        // Alternative method using stored procedure (uncomment if you want to use the stored procedure approach)
        /*
        private void LoadDeliveriesWithStoredProcedure(int? statusFilter = null)
        {
            try
            {
                int driverID = Convert.ToInt32(Session["DriverID"]);
                
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();
                    
                    using (SqlCommand cmd = new SqlCommand("GetDriverDeliveries", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DriverID", driverID);
                        
                        if (statusFilter.HasValue)
                            cmd.Parameters.AddWithValue("@StatusFilter", statusFilter.Value);
                        
                        DateTime startDate, endDate;
                        GetValidatedDateRange(out startDate, out endDate);
                        
                        if (!string.IsNullOrEmpty(txtStartDate.Text))
                            cmd.Parameters.AddWithValue("@StartDate", startDate);
                        
                        if (!string.IsNullOrEmpty(txtEndDate.Text))
                            cmd.Parameters.AddWithValue("@EndDate", endDate.AddDays(1).AddSeconds(-1));
                        
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        
                        gvDeliveries.DataSource = dt;
                        gvDeliveries.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Error loading deliveries: {ex.Message}");
            }
        }
        */

        private void UpdateSummary()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    DateTime startDate, endDate;
                    GetValidatedDateRange(out startDate, out endDate);

                    // Total deliveries
                    string totalQuery = "SELECT COUNT(*) FROM Delivery WHERE deliveryDate BETWEEN @StartDate AND @EndDate AND driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(totalQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        lblTotalDeliveries.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Pending deliveries (status = 0)
                    string pendingQuery = "SELECT COUNT(*) FROM Delivery WHERE status = 0 AND deliveryDate BETWEEN @StartDate AND @EndDate AND driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(pendingQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        lblPendingDeliveries.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Assigned deliveries (status = 1)
                    string assignedQuery = "SELECT COUNT(*) FROM Delivery WHERE status = 1 AND deliveryDate BETWEEN @StartDate AND @EndDate AND driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(assignedQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        lblAssignedDeliveries.Text = cmd.ExecuteScalar().ToString();
                    }

                    // In Transit deliveries (status = 2)
                    string transitQuery = "SELECT COUNT(*) FROM Delivery WHERE status = 2 AND deliveryDate BETWEEN @StartDate AND @EndDate AND driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(transitQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        lblInTransitDeliveries.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Completed deliveries (status = 3)
                    string completedQuery = "SELECT COUNT(*) FROM Delivery WHERE status = 3 AND deliveryDate BETWEEN @StartDate AND @EndDate AND driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(completedQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        lblCompletedDeliveries.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Failed deliveries (status = 4)
                    string failedQuery = "SELECT COUNT(*) FROM Delivery WHERE status = 4 AND deliveryDate BETWEEN @StartDate AND @EndDate AND driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(failedQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        lblFailedDeliveries.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Cancelled deliveries (status = 5)
                    string cancelledQuery = "SELECT COUNT(*) FROM Delivery WHERE status = 5 AND deliveryDate BETWEEN @StartDate AND @EndDate AND driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(cancelledQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        lblCancelledDeliveries.Text = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Set all summary values to "0" in case of error
                lblTotalDeliveries.Text = "0";
                lblPendingDeliveries.Text = "0";
                lblAssignedDeliveries.Text = "0";
                lblInTransitDeliveries.Text = "0";
                lblCompletedDeliveries.Text = "0";
                lblFailedDeliveries.Text = "0";
                lblCancelledDeliveries.Text = "0";
                
                // Log the error or show a user-friendly message
                ShowAlert($"Error updating summary: {ex.Message}");
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDeliveries();
            UpdateSummary();
        }

        protected void btnApplyDate_Click(object sender, EventArgs e)
        {
            if (IsValidDateRange())
            {
                LoadDeliveries();
                UpdateSummary();
            }
            else
            {
                ShowAlert("Dates cannot be in the past and End date must be after Start date");
            }
        }

        private bool IsValidDateRange()
        {
            DateTime startDate, endDate;
            if (DateTime.TryParse(txtStartDate.Text, out startDate) &&
                DateTime.TryParse(txtEndDate.Text, out endDate))
            {
                // Only check that end date is after or equal to start date
                return endDate >= startDate;
            }
            return false;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDeliveries();
            UpdateSummary();
        }

        protected void gvDeliveries_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Row command handling removed as we no longer have assign driver or view details buttons
        }

        protected void gvDeliveries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the status dropdown and hidden field
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatusUpdate");
                HiddenField hfDeliveryID = (HiddenField)e.Row.FindControl("hfDeliveryID");

                if (ddlStatus != null && hfDeliveryID != null)
                {
                    // Get the current status from the data source
                    string currentStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();

                    // Set the dropdown to match the current status
                    switch (currentStatus)
                    {
                        case "Pending":
                            ddlStatus.SelectedValue = "0";
                            break;
                        case "Assigned":
                            ddlStatus.SelectedValue = "1";
                            break;
                        case "In Transit":
                            ddlStatus.SelectedValue = "2";
                            break;
                        case "Completed":
                            ddlStatus.SelectedValue = "3";
                            break;
                        case "Failed":
                            ddlStatus.SelectedValue = "4";
                            break;
                        case "Cancelled":
                            ddlStatus.SelectedValue = "5";
                            break;
                        default:
                            ddlStatus.SelectedValue = "0";
                            break;
                    }
                }
            }
        }

        protected void ddlStatusUpdate_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.NamingContainer;
            HiddenField hfDeliveryID = (HiddenField)row.FindControl("hfDeliveryID");
            
            int deliveryID = Convert.ToInt32(hfDeliveryID.Value);
            int newStatus = Convert.ToInt32(ddl.SelectedValue);
            
            UpdateDeliveryStatus(deliveryID, newStatus);
        }

        private void UpdateDeliveryStatus(int deliveryID, int newStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();
                    
                    string query = "UPDATE Delivery SET status = @Status WHERE deliveryID = @DeliveryID AND driverID = @DriverID";
                    
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@DeliveryID", deliveryID);
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                
                LoadDeliveries();
                UpdateSummary();
                
                // Show success message
                ShowAlert("Delivery status updated successfully!", "success");
            }
            catch (Exception ex)
            {
                ShowAlert($"Error updating delivery status: {ex.Message}");
            }
        }



        protected void gvDeliveries_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDeliveries.PageIndex = e.NewPageIndex;
            LoadDeliveries();
        }

        protected string FormatStatus(string status)
        {
            switch (status)
            {
                case "0": return "Pending";
                case "1": return "Assigned";
                case "2": return "In Transit";
                case "3": return "Completed";
                case "4": return "Failed";
                case "5": return "Cancelled";
                default: return status;
            }
        }

        private void ShowAlert(string message, string type = "error")
        {
            string script = $@"Swal.fire({{
                icon: '{(type == "success" ? "success" : "error")}',
                title: '{(type == "success" ? "Success!" : "Error")}',
                text: '{message.Replace("'", "\\'")}',
                timer: 2000,
                showConfirmButton: false
            }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowAlert", script, true);
        }

        private void GetValidatedDateRange(out DateTime startDate, out DateTime endDate)
        {
            DateTime today = DateTime.Today;
            DateTime parsedStart = today;
            DateTime parsedEnd = today;

            DateTime temp;
            if (DateTime.TryParse(txtStartDate.Text, out temp))
            {
                // Allow past dates for testing, but default to today if invalid
                parsedStart = temp;
            }
            if (DateTime.TryParse(txtEndDate.Text, out temp))
            {
                // Allow past dates for testing, but default to today if invalid
                parsedEnd = temp;
            }

            if (parsedEnd < parsedStart)
            {
                parsedEnd = parsedStart;
            }

            // Reflect values back to the inputs
            txtStartDate.Text = parsedStart.ToString("yyyy-MM-dd");
            txtEndDate.Text = parsedEnd.ToString("yyyy-MM-dd");

            startDate = parsedStart;
            endDate = parsedEnd;
        }

        // Debug method to test database connectivity and data
        private void TestDatabaseConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();
                    
                    // Test basic queries
                    string[] testQueries = {
                        "SELECT COUNT(*) FROM Delivery",
                        "SELECT COUNT(*) FROM Sale", 
                        "SELECT COUNT(*) FROM Book",
                        "SELECT COUNT(*) FROM Seller",
                        "SELECT COUNT(*) FROM Buyer",
                        "SELECT COUNT(*) FROM Driver"
                    };
                    
                    foreach (string query in testQueries)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            int count = Convert.ToInt32(cmd.ExecuteScalar());
                            // You can add logging here if needed
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Database connection test failed: {ex.Message}");
            }
        }
    }
}