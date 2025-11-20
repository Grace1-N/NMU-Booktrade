using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using NMU_BookTrade;


namespace NMU_BookTrade.Driver.ClintonModule
{
    public partial class DriverDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthorizationHelper.Authorize("4"); // 4 = Driver
            if (!IsPostBack)
            {
                // Validate session before proceeding
                if (!IsDriverAuthenticated())
                {
                    Response.Redirect("~/User Management/Login.aspx");
                    return;
                }

                try
                {
                    LoadDriverData();
                    LoadActiveDeliveries();
                    CheckDatabaseStatus(); // Check database status for debugging
                    SetActiveTab(tabPending);
                }
                catch (SqlException)
                {
                    lblErrorMessage.Text = "Database error occurred. Please try again later.";
                    lblErrorMessage.CssClass = "alert alert-danger";
                    lblErrorMessage.Visible = true;
                    // Log the error: LogError(sqlEx);
                }
                catch (Exception)
                {
                    lblErrorMessage.Text = "An unexpected error occurred. Please contact support.";
                    lblErrorMessage.CssClass = "alert alert-danger";
                    lblErrorMessage.Visible = true;
                    // Log the error: LogError(ex);
                }
            }
        }

        private bool IsDriverAuthenticated()
        {
            return Session["AccessID"] != null &&
                   Session["AccessID"].ToString() == "4" &&
                   Session["DriverID"] != null;
        }

        private void LoadDriverData()
        {
            string driverId = Session["DriverID"].ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    // Get driver info
                    string driverQuery = "SELECT driverName, driverSurname FROM Driver WHERE driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(driverQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblDriverName.Text = $"{reader["driverName"]} {reader["driverSurname"]}";
                            }
                            else
                            {
                                lblErrorMessage.Text = "Driver information not found.";
                                lblErrorMessage.CssClass = "alert alert-danger";
                                lblErrorMessage.Visible = true;
                            }
                        }
                    }

                    // Get current date for today's calculations
                    DateTime today = DateTime.Today;
                    DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                    DateTime endOfWeek = startOfWeek.AddDays(7);

                    // OPERATIONAL CARDS - Today's Work
                    
                    // Pending assignments (status = 0, today's date)
                    string pendingTodayQuery = @"SELECT COUNT(*) FROM Delivery 
                                                WHERE driverID = @DriverID AND status = 0
                                                AND CONVERT(DATE, deliveryDate) = @Today";
                    using (SqlCommand cmd = new SqlCommand(pendingTodayQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        cmd.Parameters.AddWithValue("@Today", today);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        lblPendingToday.Text = count.ToString();
                    }

                    // Ready to start (status = 1, today's date)
                    string assignedTodayQuery = @"SELECT COUNT(*) FROM Delivery 
                                                 WHERE driverID = @DriverID AND status = 1
                                                 AND CONVERT(DATE, deliveryDate) = @Today";
                    using (SqlCommand cmd = new SqlCommand(assignedTodayQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        cmd.Parameters.AddWithValue("@Today", today);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        lblAssignedToday.Text = count.ToString();
                    }

                    // In Transit (status = 2, today's date)
                    string transitTodayQuery = @"SELECT COUNT(*) FROM Delivery 
                                                WHERE driverID = @DriverID AND status = 2
                                                AND CONVERT(DATE, deliveryDate) = @Today";
                    using (SqlCommand cmd = new SqlCommand(transitTodayQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        cmd.Parameters.AddWithValue("@Today", today);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        lblInTransitToday.Text = count.ToString();
                    }

                    // Completed deliveries today
                    string completedTodayQuery = @"SELECT COUNT(*) FROM Delivery 
                                                WHERE driverID = @DriverID AND status = 3
                                                AND CONVERT(DATE, deliveryDate) = @Today";
                    using (SqlCommand cmd = new SqlCommand(completedTodayQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        cmd.Parameters.AddWithValue("@Today", today);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        lblCompletedDeliveries.Text = count.ToString();
                    }

                    // PERFORMANCE CARDS - Historical Metrics
                    
                    // Completed deliveries this week
                    string completedWeekQuery = @"SELECT COUNT(*) FROM Delivery 
                                                WHERE driverID = @DriverID AND status = 3
                                                AND deliveryDate >= @StartOfWeek AND deliveryDate < @EndOfWeek";
                    using (SqlCommand cmd = new SqlCommand(completedWeekQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        cmd.Parameters.AddWithValue("@StartOfWeek", startOfWeek);
                        cmd.Parameters.AddWithValue("@EndOfWeek", endOfWeek);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        lblCompletedThisWeek.Text = count.ToString();
                    }

                    // Total completed deliveries (all time)
                    string totalCompletedQuery = "SELECT COUNT(*) FROM Delivery WHERE driverID = @DriverID AND status = 3";
                    using (SqlCommand cmd = new SqlCommand(totalCompletedQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        lblTotalCompleted.Text = count.ToString();
                    }

                    // Failed deliveries this week
                    string failedThisWeekQuery = @"SELECT COUNT(*) FROM Delivery 
                                                  WHERE driverID = @DriverID AND status = 4
                                                  AND deliveryDate >= @StartOfWeek AND deliveryDate < @EndOfWeek";
                    using (SqlCommand cmd = new SqlCommand(failedThisWeekQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        cmd.Parameters.AddWithValue("@StartOfWeek", startOfWeek);
                        cmd.Parameters.AddWithValue("@EndOfWeek", endOfWeek);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        lblFailedThisWeek.Text = count.ToString();
                    }

                    // Cancelled deliveries this week
                    string cancelledThisWeekQuery = @"SELECT COUNT(*) FROM Delivery 
                                                     WHERE driverID = @DriverID AND status = 5
                                                     AND deliveryDate >= @StartOfWeek AND deliveryDate < @EndOfWeek";
                    using (SqlCommand cmd = new SqlCommand(cancelledThisWeekQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        cmd.Parameters.AddWithValue("@StartOfWeek", startOfWeek);
                        cmd.Parameters.AddWithValue("@EndOfWeek", endOfWeek);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        lblCancelledThisWeek.Text = count.ToString();
                    }


                    // Clear any previous error messages
                    lblErrorMessage.Text = "";
                    lblErrorMessage.Visible = false;
                }
            }
            catch (SqlException)
            {
                lblErrorMessage.Text = $"Database error";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
                // Log the error: LogError(sqlEx);
            }
            catch (Exception)
            {
                lblErrorMessage.Text = $"Error loading driver data";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
                // Log the error: LogError(ex);
            }
        }

        private void LoadActiveDeliveries()
        {
            string driverId = Session["DriverID"].ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    // First check if there are any active deliveries for this driver (not completed)
                    string checkQuery = "SELECT COUNT(*) FROM Delivery WHERE driverID = @DriverID AND status != 3";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@DriverID", driverId);
                        int activeCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        
                        if (activeCount == 0)
                        {
                            rptPendingDeliveries.DataSource = null;
                            rptPendingDeliveries.DataBind();
                            lblNoPending.Visible = true;
                            return;
                        }
                    }

                    string query = @"SELECT d.deliveryID, 
                                    ISNULL(b.title, 'Unknown Book') AS BookTitle, 
                                    ISNULL(seller.sellerName, 'Unknown Seller') AS SellerName, 
                                    ISNULL(buyer.buyerName, 'Unknown Buyer') AS BuyerName,
                                    ISNULL(seller.sellerAddress, 'No Address') AS PickupAddress, 
                                    ISNULL(buyer.buyerAddress, 'No Address') AS DeliveryAddress,
                                    d.deliveryDate,
                                    d.status,
                                    CASE 
                                        WHEN d.status = 0 THEN 'Pending'
                                        WHEN d.status = 1 THEN 'Assigned'
                                        WHEN d.status = 2 THEN 'In Transit'
                                        WHEN d.status = 4 THEN 'Failed'
                                        WHEN d.status = 5 THEN 'Cancelled'
                                        ELSE 'Unknown'
                                    END AS StatusText
                                    FROM Delivery d
                                    LEFT JOIN Sale s ON d.saleID = s.saleID
                                    LEFT JOIN Book b ON s.bookISBN = b.bookISBN
                                    LEFT JOIN Seller seller ON s.sellerID = seller.sellerID
                                    LEFT JOIN Buyer buyer ON s.buyerID = buyer.buyerID
                                    WHERE d.driverID = @DriverID AND d.status != 3
                                    ORDER BY d.deliveryDate";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                rptPendingDeliveries.DataSource = dt;
                                rptPendingDeliveries.DataBind();
                                lblNoPending.Visible = false;
                            }
                            else
                            {
                                rptPendingDeliveries.DataSource = null;
                                rptPendingDeliveries.DataBind();
                                lblNoPending.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = $"Error loading active deliveries: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
            }
        }

        private void LoadCompletedDeliveries()
        {
            string driverId = Session["DriverID"].ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    string query = @"SELECT d.deliveryID, 
                                    ISNULL(b.title, 'Unknown Book') AS BookTitle, 
                                    ISNULL(seller.sellerName, 'Unknown Seller') AS SellerName, 
                                    ISNULL(buyer.buyerName, 'Unknown Buyer') AS BuyerName,
                                    ISNULL(seller.sellerAddress, 'No Address') AS PickupAddress, 
                                    ISNULL(buyer.buyerAddress, 'No Address') AS DeliveryAddress,
                                    d.deliveryDate,
                                    d.status,
                                    'Completed' AS StatusText
                                    FROM Delivery d
                                    LEFT JOIN Sale s ON d.saleID = s.saleID
                                    LEFT JOIN Book b ON s.bookISBN = b.bookISBN
                                    LEFT JOIN Seller seller ON s.sellerID = seller.sellerID
                                    LEFT JOIN Buyer buyer ON s.buyerID = buyer.buyerID
                                    WHERE d.driverID = @DriverID AND d.status = 3
                                    ORDER BY d.deliveryDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                rptCompletedDeliveries.DataSource = dt;
                                rptCompletedDeliveries.DataBind();
                                lblNoCompleted.Visible = false;
                            }
                            else
                            {
                                rptCompletedDeliveries.DataSource = null;
                                rptCompletedDeliveries.DataBind();
                                lblNoCompleted.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = $"Error loading completed deliveries: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
            }
        }

        // Fix for the CS1061 error
        private void LoadWeeklySchedule()
        {
            string driverId = Session["DriverID"].ToString();
            DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime endOfWeek = startOfWeek.AddDays(7);

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    // First check if there are any deliveries for this driver in the current week
                    string checkQuery = @"SELECT COUNT(*) FROM Delivery 
                                        WHERE driverID = @DriverID
                                        AND deliveryDate >= @StartDate
                                        AND deliveryDate < @EndDate";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@DriverID", driverId);
                        checkCmd.Parameters.AddWithValue("@StartDate", startOfWeek);
                        checkCmd.Parameters.AddWithValue("@EndDate", endOfWeek);
                        int weekCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        
                        if (weekCount == 0)
                        {
                            rptWeeklySchedule.DataSource = null;
                            rptWeeklySchedule.DataBind();
                            lblNoSchedule.Visible = true;
                            return;
                        }
                    }

                    string query = @"SELECT d.deliveryID, 
                                    ISNULL(b.title, 'Unknown Book') AS BookTitle, 
                                    ISNULL(buyer.buyerAddress, 'No Address') AS Location, 
                                    d.deliveryDate
                                    FROM Delivery d
                                    LEFT JOIN Sale s ON d.saleID = s.saleID
                                    LEFT JOIN Book b ON s.bookISBN = b.bookISBN
                                    LEFT JOIN Buyer buyer ON s.buyerID = buyer.buyerID
                                    WHERE d.driverID = @DriverID
                                    AND d.deliveryDate >= @StartDate
                                    AND d.deliveryDate < @EndDate
                                    ORDER BY d.deliveryDate";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", driverId);
                        cmd.Parameters.AddWithValue("@StartDate", startOfWeek);
                        cmd.Parameters.AddWithValue("@EndDate", endOfWeek);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // Convert DataTable to a list of dynamic objects
                            var scheduleByDay = dt.Rows.Cast<DataRow>()
                                .GroupBy(r => ((DateTime)r["deliveryDate"]).DayOfWeek)
                                .Select(g => new
                                {
                                    Day = g.Key.ToString(),
                                    Deliveries = g.Select(r => new
                                    {
                                        Time = ((DateTime)r["deliveryDate"]).ToString("HH:mm"),
                                        BookTitle = r["BookTitle"].ToString(),
                                        Location = r["Location"].ToString()
                                    }).ToList()
                                }).ToList();

                            if (scheduleByDay.Count > 0)
                            {
                                rptWeeklySchedule.DataSource = scheduleByDay;
                                rptWeeklySchedule.DataBind();
                                lblNoSchedule.Visible = false;
                            }
                            else
                            {
                                rptWeeklySchedule.DataSource = null;
                                rptWeeklySchedule.DataBind();
                                lblNoSchedule.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = $"Error loading weekly schedule: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
            }
        }

        private string FormatDuration(int minutes)
        {
            if (minutes >= 60)
                return $"{(minutes / 60)}h {(minutes % 60)}m";
            return $"{minutes}m";
        }

        protected void tabPending_Click(object sender, EventArgs e)
        {
            mvDriverContent.ActiveViewIndex = 0;
            LoadActiveDeliveries();
            LoadDriverData(); // Refresh summary data
            SetActiveTab(tabPending);
        }

        protected void tabCompleted_Click(object sender, EventArgs e)
        {
            mvDriverContent.ActiveViewIndex = 1;
            // Refresh summary first so the labels used below are up-to-date
            LoadDriverData();
            LoadCompletedDeliveries();
            LoadDriverData(); // Refresh summary data
            SetActiveTab(tabCompleted);
        }

        protected void tabSchedule_Click(object sender, EventArgs e)
        {
            mvDriverContent.ActiveViewIndex = 2;
            LoadWeeklySchedule();
            LoadDriverData(); // Refresh summary data
            SetActiveTab(tabSchedule);
        }

        private void SetActiveTab(LinkButton activeTab)
        {
            tabPending.CssClass = "dd-tab" + (tabPending == activeTab ? " active" : "");
            tabCompleted.CssClass = "dd-tab" + (tabCompleted == activeTab ? " active" : "");
            tabSchedule.CssClass = "dd-tab" + (tabSchedule == activeTab ? " active" : "");
        }

        protected void btnStartDelivery_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                if (!int.TryParse(btn.CommandArgument, out int deliveryId))
                {
                    lblErrorMessage.Text = "Invalid delivery ID.";
                    lblErrorMessage.CssClass = "alert alert-danger";
                    lblErrorMessage.Visible = true;
                    return;
                }

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    string updateQuery = @"UPDATE Delivery 
                                          SET status = 2 
                                          WHERE deliveryID = @DeliveryID AND driverID = @DriverID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DeliveryID", deliveryId);
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"].ToString());

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Refresh the summary data after status change
                            LoadDriverData();
                            LoadActiveDeliveries();
                            lblErrorMessage.Text = $"Delivery #{deliveryId} started successfully!";
                            lblErrorMessage.CssClass = "alert alert-success";
                            lblErrorMessage.Visible = true;
                        }
                        else
                        {
                            lblErrorMessage.Text = "Delivery could not be started. It may have been assigned to another driver.";
                            lblErrorMessage.CssClass = "alert alert-danger";
                            lblErrorMessage.Visible = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblErrorMessage.Text = "Error starting delivery. Please try again.";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
                // Log the error: LogError(ex);
            }
        }

        protected void btnCompleteDelivery_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                if (!int.TryParse(btn.CommandArgument, out int deliveryId))
                {
                    lblErrorMessage.Text = "Invalid delivery ID.";
                    lblErrorMessage.CssClass = "alert alert-danger";
                    lblErrorMessage.Visible = true;
                    return;
                }

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    string updateQuery = @"UPDATE Delivery 
                                          SET status = 3 
                                          WHERE deliveryID = @DeliveryID AND driverID = @DriverID AND status = 2";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DeliveryID", deliveryId);
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"].ToString());

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Refresh the summary data after status change
                            LoadDriverData();
                            LoadActiveDeliveries();
                            lblErrorMessage.Text = $"Delivery #{deliveryId} completed successfully!";
                            lblErrorMessage.CssClass = "alert alert-success";
                            lblErrorMessage.Visible = true;
                        }
                        else
                        {
                            lblErrorMessage.Text = "Delivery could not be completed. It may not be in transit or assigned to another driver.";
                            lblErrorMessage.CssClass = "alert alert-danger";
                            lblErrorMessage.Visible = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblErrorMessage.Text = "Error completing delivery. Please try again.";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
                // Log the error: LogError(ex);
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/User Management/Login.aspx");
        }

        protected void btnRefreshSummary_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDriverData();
                LoadActiveDeliveries();
                // Clear any error messages
                lblErrorMessage.Text = "";
                lblErrorMessage.Visible = false;
            }
            catch (Exception)
            {
                lblErrorMessage.Text = "Error refreshing summary data. Please try again.";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
                // Log the error: LogError(ex);
            }
        }

        //protected void btnRefreshSummary_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        LoadDriverData();
        //        LoadActiveDeliveries();
        //        // Clear any error messages
        //        lblErrorMessage.Text = "";
        //        lblErrorMessage.Visible = false;
        //    }
        //    catch (Exception)
        //    {
        //        lblErrorMessage.Text = "Error refreshing summary data. Please try again.";
        //        lblErrorMessage.CssClass = "alert alert-danger";
        //        lblErrorMessage.Visible = true;
        //        // Log the error: LogError(ex);
        //    }
        //}

        protected void rptPendingDeliveries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Additional formatting if needed
        }

        protected void rptCompletedDeliveries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Additional formatting if needed
        }

        protected void rptWeeklySchedule_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dayData = (dynamic)e.Item.DataItem;
                var dayDeliveries = (Repeater)e.Item.FindControl("rptDayDeliveries");
                dayDeliveries.DataSource = dayData.Deliveries;
                dayDeliveries.DataBind();
            }
        }


        private void RefreshSummaryData()
        {
            LoadDriverData();
            LoadActiveDeliveries();
        }

        private void CheckDatabaseStatus()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();
                    
                    // Check if Delivery table exists and has data
                    string checkDeliveryQuery = "SELECT COUNT(*) FROM Delivery";
                    using (SqlCommand cmd = new SqlCommand(checkDeliveryQuery, connection))
                    {
                        int deliveryCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (deliveryCount == 0)
                        {
                            lblErrorMessage.Text = "No deliveries found in database. Dashboard summary will show 0 for all counts.";
                            lblErrorMessage.CssClass = "alert alert-danger";
                            lblErrorMessage.Visible = true;
                        }
                    }

                    // Check if Driver table exists and has the current driver
                    string checkDriverQuery = "SELECT COUNT(*) FROM Driver WHERE driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(checkDriverQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        int driverCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (driverCount == 0)
                        {
                            lblErrorMessage.Text = "Driver not found in database. Please contact administrator.";
                            lblErrorMessage.CssClass = "alert alert-danger";
                            lblErrorMessage.Visible = true;
                        }
                    }

                    // Check if there are any deliveries for this specific driver
                    string checkDriverDeliveriesQuery = "SELECT COUNT(*) FROM Delivery WHERE driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(checkDriverDeliveriesQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        int driverDeliveries = Convert.ToInt32(cmd.ExecuteScalar());
                        if (driverDeliveries == 0)
                        {
                            lblErrorMessage.Text = "No deliveries assigned to this driver. All tabs will show empty.";
                            lblErrorMessage.CssClass = "alert alert-danger";
                            lblErrorMessage.Visible = true;
                        }
                    }

                    // Test the JOIN relationships
                    string testJoinQuery = @"SELECT COUNT(*) FROM Delivery d
                                           LEFT JOIN Sale s ON d.saleID = s.saleID
                                           LEFT JOIN Book b ON s.bookISBN = b.bookISBN
                                           WHERE d.driverID = @DriverID";
                    using (SqlCommand cmd = new SqlCommand(testJoinQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
                        //Will help identify if the JOIN issue is resolved
                        int joinCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = $"Database connection test failed: {ex.Message}";
                lblErrorMessage.CssClass = "alert alert-danger";
                lblErrorMessage.Visible = true;
            }
        }

        //private void RefreshSummaryData()
        //{
        //    LoadDriverData();
        //    LoadActiveDeliveries();
        //}

        //private void CheckDatabaseStatus()
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
        //        {
        //            connection.Open();

        //            // Check if Delivery table exists and has data
        //            string checkDeliveryQuery = "SELECT COUNT(*) FROM Delivery";
        //            using (SqlCommand cmd = new SqlCommand(checkDeliveryQuery, connection))
        //            {
        //                int deliveryCount = Convert.ToInt32(cmd.ExecuteScalar());
        //                if (deliveryCount == 0)
        //                {
        //                    lblErrorMessage.Text = "No deliveries found in database. Dashboard summary will show 0 for all counts.";
        //                    lblErrorMessage.CssClass = "alert alert-danger";
        //                    lblErrorMessage.Visible = true;
        //                }
        //            }

        //            // Check if Driver table exists and has the current driver
        //            string checkDriverQuery = "SELECT COUNT(*) FROM Driver WHERE driverID = @DriverID";
        //            using (SqlCommand cmd = new SqlCommand(checkDriverQuery, connection))
        //            {
        //                cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
        //                int driverCount = Convert.ToInt32(cmd.ExecuteScalar());
        //                if (driverCount == 0)
        //                {
        //                    lblErrorMessage.Text = "Driver not found in database. Please contact administrator.";
        //                    lblErrorMessage.CssClass = "alert alert-danger";
        //                    lblErrorMessage.Visible = true;
        //                }
        //            }

        //            // Check if there are any deliveries for this specific driver
        //            string checkDriverDeliveriesQuery = "SELECT COUNT(*) FROM Delivery WHERE driverID = @DriverID";
        //            using (SqlCommand cmd = new SqlCommand(checkDriverDeliveriesQuery, connection))
        //            {
        //                cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
        //                int driverDeliveries = Convert.ToInt32(cmd.ExecuteScalar());
        //                if (driverDeliveries == 0)
        //                {
        //                    lblErrorMessage.Text = "No deliveries assigned to this driver. All tabs will show empty.";
        //                    lblErrorMessage.CssClass = "alert alert-danger";
        //                    lblErrorMessage.Visible = true;
        //                }
        //            }

        //            // Test the JOIN relationships
        //            string testJoinQuery = @"SELECT COUNT(*) FROM Delivery d
        //                                   LEFT JOIN Sale s ON d.saleID = s.saleID
        //                                   LEFT JOIN Book b ON s.bookISBN = b.bookISBN
        //                                   WHERE d.driverID = @DriverID";
        //            using (SqlCommand cmd = new SqlCommand(testJoinQuery, connection))
        //            {
        //                cmd.Parameters.AddWithValue("@DriverID", Session["DriverID"]);
        //                int joinCount = Convert.ToInt32(cmd.ExecuteScalar());
        //                // This will help identify if the JOIN issue is resolved
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lblErrorMessage.Text = $"Database connection test failed: {ex.Message}";
        //        lblErrorMessage.CssClass = "alert alert-danger";
        //        lblErrorMessage.Visible = true;
        //    }
        //}
    }
}
