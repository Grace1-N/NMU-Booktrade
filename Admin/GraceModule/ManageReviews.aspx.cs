using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.Admin.GraceModule
{
    public partial class ManageReviews : System.Web.UI.Page
    {// Use your existing connection string
        private readonly string _cs = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

        // we need to set the list for inappropriate words we want to filter out. The idea is that, if a review contains any of these words, it will be flagged.
        private readonly string[] _badWords = { "spam", "fake", "terrible", "awful", "hate", "stupid", "garbage", "ridiculous", "idiots", "damaged", "dumb", "shocked", "avoid", "never", "regret", "concerned", "swear", "don't", "poor", "not", "horrible", "difficult", "hard", "unsure", "delay", "inaccurate", "worthless", "trash", "nonsense", "fraud", "slow", "unresponsive", "driver", "seller", "admin" };


        protected void Page_Load(object sender, EventArgs e)
        {
            //run this only when the page first loads(not on every button click)
            if (!IsPostBack)
            {
                
                LoadStats();
                LoadFlaggedReviews();
            }

        }

        


        private void LoadStats()
        {

            using (SqlConnection con = new SqlConnection(_cs))
            {

                con.Open();
                // here we are going to count the total reviews 

                SqlCommand totalcmd = new SqlCommand("Select COUNT(*) FROM Review", con);
                int TotalReviews = (int)totalcmd.ExecuteScalar();

                //Count the number of reviews that have been flagged

                SqlCommand isflaggedcmd = new SqlCommand("SELECT COUNT (*) FROM Review WHERE isFlagged = 1", con);
                int TotalFlagged = (int)isflaggedcmd.ExecuteScalar();


                // Counting reviews that are flagged today

                SqlCommand RemovedCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Review WHERE isFlagged = 1 AND CAST(reviewDate AS DATE) = CAST(GETDATE() AS DATE)", con);
                int FlaggedToday = (int)RemovedCmd.ExecuteScalar();

                //Here we are geting the percentage (that is not flagged) of the reviews 
                // Clean Rate = ((TotalReviews - TotalFlagged) /TotalReviews)*100)

                int cleanRate = TotalReviews > 0 ? (int)(((double)(TotalReviews - TotalFlagged) / TotalReviews) * 100) : 0;

                // After getting the stats and calculating them we send them to the front-end 

                litTotalFlagged.Text = TotalFlagged.ToString();
                litTotalReviews.Text = TotalReviews.ToString();
                litFlaggedToday.Text = FlaggedToday.ToString();
                litCleanRate.Text = cleanRate + "%";

            }
        }

        private void LoadFlaggedReviews()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                // Here we are getting all flagged reviews from the database  
                SqlCommand FlaggedReviewsCmd = new SqlCommand(@"
                                            SELECT r.reviewID, r.reviewRating, r.reviewComment, r.reviewDate, 
                                                   b.buyerName, b.buyerSurname
                                            FROM Review r
                                            INNER JOIN Sale s ON r.saleID = s.saleID
                                            INNER JOIN Buyer b ON s.buyerID = b.buyerID
                                            WHERE r.isFlagged = 1", con);
                SqlDataReader dataReader = FlaggedReviewsCmd.ExecuteReader();

                // Now we want to bind them to the repeater (so they display in your design)
                rptFlaggedReviews.DataSource = dataReader;
                rptFlaggedReviews.DataBind();
            }
        }

        protected void rptFlaggedReviews_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int reviewID = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                if (e.CommandName == "Approve")
                {
                    // Approve: just unflag it (or add another column if you want approved status)
                    SqlCommand cmd = new SqlCommand("UPDATE Review SET isFlagged = 0, isApproved = 1 WHERE reviewID = @id", con);
                    cmd.Parameters.AddWithValue("@id", reviewID);
                  //  cmd.Parameters.AddWithValue("@isApproved", 1);
                    cmd.ExecuteNonQuery();
                }
                else if (e.CommandName == "Remove")
                {
                    // Remove: delete review permanently
                    SqlCommand cmd = new SqlCommand("DELETE FROM Review WHERE reviewID = @id", con);
                    cmd.Parameters.AddWithValue("@id", reviewID);
                    cmd.ExecuteNonQuery();
                }
            }

            // Reload stats + flagged reviews after action
            LoadStats();
            LoadFlaggedReviews();
        }




    }
}