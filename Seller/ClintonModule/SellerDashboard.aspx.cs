using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using NMU_BookTrade;

namespace NMU_BookTrade
{
    public partial class SellerDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["AccessID"] = "3";
            AuthorizationHelper.Authorize("3"); // 3 = Seller
            if (!IsPostBack)
            {
                if (Session["AccessID"] == null || Session["AccessID"].ToString() != "3" || Session["SellerID"] == null)
                {
                    Response.Redirect("~/UserManagement/Login.aspx");
                    return;
                }
                else
                {
                    lblUsername.Text = "Seller";
                    LoadSellerStats();
                    LoadRecentActivity();
                }
            }
        }

        private void LoadSellerStats()
        {
            int sellerId = Convert.ToInt32(Session["SellerID"]);

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                connection.Open();

                // Get active listings count
                string activeListingsQuery = @"SELECT COUNT(*) FROM Book 
                                               WHERE sellerID = @SellerID 
                                               AND bookISBN NOT IN (SELECT bookISBN FROM Sale)";

                using (SqlCommand cmd = new SqlCommand(activeListingsQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);
                    lblActiveListings.Text = cmd.ExecuteScalar().ToString();
                }

                // Get sold books count
                string soldBooksQuery = @"SELECT COUNT(*) FROM Sale s
                                          JOIN Book b ON s.bookISBN = b.bookISBN
                                          WHERE b.sellerID = @SellerID";

                using (SqlCommand cmd = new SqlCommand(soldBooksQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);
                    lblSoldBooks.Text = cmd.ExecuteScalar().ToString();
                }

            }
        }

        private void LoadRecentActivity()
        {
            int sellerId = Convert.ToInt32(Session["SellerID"]);

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                connection.Open();

                string query = @"
                    -- Recent sales (last 5)
                    SELECT TOP 5 
                        'money-bill-wave' AS Icon,
                        'Sale completed for ''' + b.title + '''' AS Message,
                        CASE 
                            WHEN DATEDIFF(HOUR, s.saleDate, GETDATE()) < 24 
                            THEN CAST(DATEDIFF(HOUR, s.saleDate, GETDATE()) AS VARCHAR) + ' hours ago'
                            ELSE CAST(DATEDIFF(DAY, s.saleDate, GETDATE()) AS VARCHAR) + ' days ago'
                        END AS Time,
                        s.saleDate AS SortDate
                    FROM Sale s
                    JOIN Book b ON s.bookISBN = b.bookISBN
                    WHERE b.sellerID = @SellerID
                    
                    
                    -- Recent deliveries (last 5)
                    SELECT TOP 5 
                        'truck' AS Icon,
                        CASE 
                            WHEN d.status = 1 THEN 'Driver assigned for ''' + b.title + ''''
                            WHEN d.status = 2 THEN '''' + b.title + ''' is in transit'
                            WHEN d.status = 3 THEN '''' + b.title + ''' delivered successfully'
                            ELSE 'Delivery status updated for ''' + b.title + ''''
                        END AS Message,
                        CASE 
                            WHEN DATEDIFF(HOUR, d.deliveryDate, GETDATE()) < 24 
                            THEN CAST(DATEDIFF(HOUR, d.deliveryDate, GETDATE()) AS VARCHAR) + ' hours ago'
                            ELSE CAST(DATEDIFF(DAY, d.deliveryDate, GETDATE()) AS VARCHAR) + ' days ago'
                        END AS Time,
                        d.deliveryDate AS SortDate
                    FROM Delivery d
                    JOIN Sale s ON d.saleID = s.saleID
                    JOIN Book b ON s.bookISBN = b.bookISBN
                    WHERE b.sellerID = @SellerID
                    
                    ORDER BY SortDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        var recentActivities = dt.AsEnumerable()
                            .Take(10)
                            .Select(row => new
                            {
                                Icon = row["Icon"].ToString(),
                                Message = row["Message"].ToString(),
                                Time = row["Time"].ToString()
                            })
                            .ToList();

                        rptRecentActivity.DataSource = recentActivities;
                        rptRecentActivity.DataBind();
                    }
                }
            }
        }
    }
}
