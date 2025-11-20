using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NMU_BookTrade;

namespace NMU_BookTrade
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthorizationHelper.Authorize("1"); // 1 = Admin
            // Only load drivers if it's the first time the page is loading (not on postbacks)
            if (!IsPostBack)
            {

                LoadDrivers();
                try
                {
                    lblCustomerName.Text = GetUserName().ToUpper();
                }
                catch (Exception ex)
                {
                    lblCustomerName.Text = "GUEST";
                }
            }
        }


        private void LoadDrivers()
        {
            // Get connection string from Web.config
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ToString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // SQL query to get all drivers and their profile images
                string query = "SELECT driverID, driverName, driverSurname, driverProfileImage FROM Driver";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();

                // Fill the DataTable with driver data
                da.Fill(dt);

                // Bind the data to the Repeater
                rptDrivers.DataSource = dt;
                rptDrivers.DataBind();
            }
        }

        private string GetUserName()
        {

            if (Session["AccessID"] == null || Session["UserID"] == null)
            {
                throw new Exception("Session expired or user not logged in.");
            }

            string accessId = Session["AccessID"].ToString();
            string userId = Session["UserID"].ToString();
            string Name = "User";
            string query = "";


            switch (accessId)
            {
                case "1":

                    query = "SELECT adminUsername FROM Admin WHERE adminID = @UserID";
                    break;

                case "2":
                    query = "SELECT buyerName FROM Buyer WHERE buyerID = @UserID";
                    break;

                case "3":
                    query = "SELECT sellerName FROM Seller WHERE sellerID = @UserID";
                    break;

                case "4":
                    query = "SELECT driverName FROM Driver WHERE driverID = @UserID";
                    break;

                default:
                    throw new Exception("Invalid Access ID.");
            }

            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Name = reader[0].ToString();
                    }
                    conn.Close();
                }
            }

            return Name;
        }



    }
}