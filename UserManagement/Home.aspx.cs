using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                LoadBooks(); // Load books for the slider
               // totalReviews();   // Call your method here
                LoadReviews();
                LoadDeliveryStats();
            }
        }

        private void LoadDeliveryStats()
        {
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Total deliveries completed (status = 3 = completed)
                string query = "SELECT COUNT(*) FROM Delivery WHERE status = 3";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    int totalDeliveries = Convert.ToInt32(cmd.ExecuteScalar());
                    lblTotalDeliveries.Text = totalDeliveries.ToString();
                }
            }
        }

        private void LoadBooks()
        {
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            string query = "SELECT TOP 6 bookISBN, title, price, coverImage FROM Book ORDER BY bookISBN DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                rptBooks.DataSource = reader;
                rptBooks.DataBind();
            }
        }

        protected void Interested_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string bookISBN = btn.CommandArgument;
            Response.Redirect("/UserManagement/Login.aspx?ReturnUrl=/BookDetails.aspx?query=" + Server.UrlEncode(bookISBN));

        }


        private void LoadReviews()
        {
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            string query = @"
                SELECT TOP 3 r.reviewID, r.reviewRating, r.reviewComment, r.reviewDate, 
                                                   b.buyerName, b.buyerSurname, b.buyerProfileImage
                                            FROM Review r
                                            INNER JOIN Sale s ON r.saleID = s.saleID
                                            INNER JOIN Buyer b ON s.buyerID = b.buyerID
                ORDER BY R.reviewID DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();  //  Reads multiple rows of data.
                rptTestimonials.DataSource = reader;
                rptTestimonials.DataBind();
            }
        }

        public static string GetStarHtml(int rating)
        {
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                if (i <= rating)
                    stars += "<span style='color: gold;'>&#9733;</span>"; // filled star
                else
                    stars += "<span style='color: lightgray;'>&#9734;</span>"; // empty star
            }
            return stars;
        }


        //private void totalReviews()
        //{
        //    string _cs = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
        //    using (SqlConnection conn = new SqlConnection(_cs))
        //    {
        //        conn.Open();
        //        // here we are going to count the total reviews 

        //        SqlCommand totalcmd = new SqlCommand("Select COUNT(*) FROM Review", conn);
        //        int TotalReviews = (int)totalcmd.ExecuteScalar();

        //        // After getting the stats and calculating them we send them to the front-end 

        //        litTotalReviews.Text = TotalReviews.ToString();
        //    }
        //}

        protected void btnReadStories_Click(object sender, EventArgs e)
        {
            // You can redirect or show more reviews here
            Response.Redirect("~/Buyer/pabiModule/Reviews.aspx?query=" + Server.UrlEncode(btnReadStories.Text.Trim()));
        }

        protected void rptBooks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewBook")
            {
                string bookISBN = e.CommandArgument.ToString();
                Response.Redirect("~/Buyer/pabiModule/SearchResult.aspx?query=" + Server.UrlEncode(bookISBN));
            }
        }
    }
}
