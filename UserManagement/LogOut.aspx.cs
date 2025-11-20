
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace NMU_BookTrade
{
    public partial class WebForm18 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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



        protected void btnYes_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnNo_Click(object sender, EventArgs e)
        {
            if (Session["PreviousPage"] != null)
            {
                Response.Redirect(Session["PreviousPage"].ToString());
            }
            else
            {
                Response.Redirect("Home.aspx");
            }
        }
    }
}
