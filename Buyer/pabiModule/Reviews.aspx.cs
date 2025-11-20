using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class WebForm7 : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["buyerID"] == null)
            {
                Response.Redirect("~/UserManagement/Login.aspx");
                return;
            }
            // Hide messages on every page load
            lblSuccess.Visible = false;
            lblError.Visible = false;
            if (!IsPostBack)
            {
                
                int buyerId = Convert.ToInt32(Session["buyerID"]);
                LoadPurchases(buyerId);
                LoadFilterOptions();
                LoadReviewHistory(buyerId);


                string qsIsbn = Request.QueryString["isbn"];
                if (!string.IsNullOrEmpty(qsIsbn))
                    LoadProductSummary(qsIsbn);

                lblFirstName.Text += GetBuyerFirstName(buyerId);

                pnlPurchasesTab.Visible = true;
                pnlHistoryTab.Visible = false;

                pnlReviewPanel.CssClass = "side-panel";
                pnlGuidelines.CssClass = "guidelines hidden";
                btnShowPurchases.CssClass = "tab-btn active";
                btnShowHistory.CssClass = "tab-btn";

                if (!IsPostBack)
                {
                    if (Session["PreviousPage"] == null && Request.UrlReferrer != null)
                        Session["PreviousPage"] = Request.UrlReferrer.ToString();
                }
            }
        }

        private string GetBuyerFirstName(int buyerId)
        {
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("SELECT buyerName FROM Buyer WHERE buyerID = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", buyerId);
                con.Open();
                object o = cmd.ExecuteScalar();
                return o?.ToString() ?? string.Empty;
            }
        }

        private void LoadPurchases(int buyerId)
        {
            // Hide messages on every page load
            lblSuccess.Visible = false;
            lblError.Visible = false;
            var items = new List<dynamic>();
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(@"
                SELECT s.bookISBN, MAX(s.saleDate) AS lastPurchased,
                       b.title, b.coverImage
                FROM Sale s
                JOIN Book b ON b.bookISBN = s.bookISBN
                WHERE s.buyerID = @buyerID
                GROUP BY s.bookISBN, b.title, b.coverImage
                ORDER BY lastPurchased DESC;", con))
            {
                cmd.Parameters.AddWithValue("@buyerID", buyerId);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        items.Add(new
                        {
                            bookISBN = r["bookISBN"].ToString(),
                            title = r["title"].ToString(),
                            coverImage = r["coverImage"].ToString()
                        });
                    }
                }
            }

            rptPurchases.DataSource = items;
            rptPurchases.DataBind();
        }

        private void LoadFilterOptions()
        {
            ddlReviewFilter.Items.Clear();
            ddlReviewFilter.Items.Add(new ListItem("Last 3 months", "3m"));
            ddlReviewFilter.Items.Add(new ListItem("Last 6 months", "6m"));

            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < 5; i++)
                ddlReviewFilter.Items.Add(new ListItem((currentYear - i).ToString(), (currentYear - i).ToString()));

            ddlReviewFilter.SelectedIndex = 0;
        }

        private void LoadReviewHistory(int buyerId)
        {
            lblSuccess.Visible = false;
            lblError.Visible = false;
            string filter = ddlReviewFilter.SelectedValue;
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.Now;

            if (filter == "3m")
                fromDate = DateTime.Now.AddMonths(-3);
            else if (filter == "6m")
                fromDate = DateTime.Now.AddMonths(-6);
            else if (int.TryParse(filter, out int year))
            {
                fromDate = new DateTime(year, 1, 1);
                toDate = new DateTime(year, 12, 31, 23, 59, 59);
            }

            var reviews = new List<dynamic>();
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(@"
                SELECT R.reviewID, R.reviewRating, R.reviewComment, R.reviewDate, B.title
                FROM Review R
                JOIN Sale S ON R.saleID = S.saleID
                JOIN Book B ON S.bookISBN = B.bookISBN
                WHERE S.buyerID = @buyerID
                  AND R.reviewDate BETWEEN @fromDate AND @toDate
                ORDER BY R.reviewDate DESC;", con))
            {
                cmd.Parameters.AddWithValue("@buyerID", buyerId);
                cmd.Parameters.AddWithValue("@fromDate", fromDate);
                cmd.Parameters.AddWithValue("@toDate", toDate);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        reviews.Add(new
                        {
                            reviewID = Convert.ToInt32(r["reviewID"]),
                            title = r["title"].ToString(),
                            reviewRating = Convert.ToInt32(r["reviewRating"]),
                            reviewComment = r["reviewComment"].ToString(),
                            reviewDate = Convert.ToDateTime(r["reviewDate"])
                        });
                    }
                }
            }

            rptReviewHistory.DataSource = reviews;
            rptReviewHistory.DataBind();
            lblError.Visible = !reviews.Any();
            lblError.Text = !reviews.Any() ? "No reviews in the selected period." : string.Empty;
        }

        protected void ddlReviewFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            int buyerId = Convert.ToInt32(Session["buyerID"]);
            LoadReviewHistory(buyerId);
        }

        private void LoadProductSummary(string bookISBN)
        {
            LoadAverageRating(bookISBN);
            LoadRatingBreakdown(bookISBN);
            LoadReviews(bookISBN);
        }

        private void LoadReviews(string bookISBN)
        {
            lblSuccess.Visible = false;
            lblError.Visible = false;
            var reviews = new List<dynamic>();
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(@"
                SELECT R.reviewID, B.buyerName, R.reviewRating, R.reviewComment, R.reviewDate
                FROM Review R
                JOIN Sale S ON R.saleID = S.saleID
                JOIN Buyer B ON S.buyerID = B.buyerID
                WHERE S.bookISBN = @bookISBN
                ORDER BY R.reviewDate DESC;", con))
            {
                cmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        reviews.Add(new
                        {
                            reviewID = Convert.ToInt32(r["reviewID"]),
                            buyerName = r["buyerName"].ToString(),
                            reviewRating = Convert.ToInt32(r["reviewRating"]),
                            reviewComment = r["reviewComment"].ToString(),
                            reviewDate = Convert.ToDateTime(r["reviewDate"])
                        });
                    }
                }
            }

            rptReviews.DataSource = reviews;
            rptReviews.DataBind();
            lblTotalReviews.Text = $"{reviews.Count} {(reviews.Count == 1 ? "Review" : "Reviews")}";
        }

        private void LoadAverageRating(string bookISBN)
        {
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(@"
                SELECT AVG(CAST(R.reviewRating AS DECIMAL(3,2)))
                FROM Review R
                JOIN Sale S ON R.saleID = S.saleID
                WHERE S.bookISBN = @bookISBN;", con))
            {
                cmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                con.Open();
                object result = cmd.ExecuteScalar();
                lblAverageRating.Text = (result != DBNull.Value && result != null)
                    ? $"⭐ {Convert.ToDecimal(result):0.0} / 5"
                    : "No Ratings Yet";
            }
        }

        private void LoadRatingBreakdown(string bookISBN)
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(connStr))
            using (var da = new SqlDataAdapter(@"
                SELECT R.reviewRating, COUNT(*) AS CountReviews
                FROM Review R
                JOIN Sale S ON R.saleID = S.saleID
                WHERE S.bookISBN = @bookISBN
                GROUP BY R.reviewRating
                ORDER BY R.reviewRating DESC;", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@bookISBN", bookISBN);
                da.Fill(dt);
            }

            int total = dt.AsEnumerable().Sum(r => r.Field<int>("CountReviews"));
            var breakdown = dt.AsEnumerable().Select(r => new
            {
                reviewRating = r.Field<int>("reviewRating"),
                CountReviews = r.Field<int>("CountReviews"),
                percentage = total > 0 ? (int)(r.Field<int>("CountReviews") * 100.0 / total) : 0
            });

            rptBreakdown.DataSource = breakdown;
            rptBreakdown.DataBind();
        }

        protected void btnWriteReview_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string bookISBN = btn.CommandArgument;
            hfBookISBN.Value = bookISBN;

            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT title, coverImage FROM Book WHERE bookISBN = @ISBN", con))
                {
                    cmd.Parameters.AddWithValue("@ISBN", bookISBN);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblProductName.Text = reader["title"].ToString();
                            imgProduct.ImageUrl = reader["coverImage"].ToString();
                        }
                    }
                }
            }

            int buyerId = Convert.ToInt32(Session["buyerID"]);
            lblFirstName.Text = "First Name: " + GetBuyerFirstName(buyerId);

            pnlReviewPanel.CssClass = "side-panel active";
            pnlPurchasesTab.Visible = true;
            pnlHistoryTab.Visible = false;

        }

        protected void btnShowPurchases_Click(object sender, EventArgs e)
        {
            // Hide messages on every page load
            lblSuccess.Visible = false;
            lblError.Visible = false;
            pnlPurchasesTab.Visible = true;
            pnlHistoryTab.Visible = false;
            pnlReviewPanel.CssClass = "side-panel";

            btnShowPurchases.CssClass = "tab-btn active";
            btnShowHistory.CssClass = "tab-btn";
        }

        protected void btnShowHistory_Click(object sender, EventArgs e)
        {
            // Hide messages on every page load
            lblSuccess.Visible = false;
            lblError.Visible = false;

            pnlPurchasesTab.Visible = false;
            pnlHistoryTab.Visible = true;
            pnlReviewPanel.CssClass = "side-panel";

            btnShowPurchases.CssClass = "tab-btn";
            btnShowHistory.CssClass = "tab-btn active";

            int buyerId = Convert.ToInt32(Session["buyerID"]);
            LoadReviewHistory(buyerId);
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            if (Session["buyerID"] == null || string.IsNullOrEmpty(hfBookISBN.Value))
                return;

            int buyerId = Convert.ToInt32(Session["buyerID"]);
            string bookISBN = hfBookISBN.Value;
            int rating = int.Parse(ddlRating.SelectedValue);
            string comment = txtReviewComment.Text?.Trim();

            try
            {
                using (var con = new SqlConnection(connStr))
                {
                    con.Open();
                    using (var tx = con.BeginTransaction())
                    {
                        int saleID;
                        using (var findSale = new SqlCommand(@"
                            SELECT TOP 1 saleID FROM Sale
                            WHERE buyerID = @buyer AND bookISBN = @bookISBN
                            ORDER BY saleDate DESC;", con, tx))
                        {
                            findSale.Parameters.AddWithValue("@buyer", buyerId);
                            findSale.Parameters.AddWithValue("@bookISBN", bookISBN);
                            object o = findSale.ExecuteScalar();
                            if (o == null) throw new Exception("No matching purchase found.");
                            saleID = Convert.ToInt32(o);
                        }

                        int nextReviewID;
                        using (var getNext = new SqlCommand(@"
                            SELECT ISNULL(MAX(reviewID), 0) + 1 FROM Review WITH (UPDLOCK, HOLDLOCK);", con, tx))
                        {
                            nextReviewID = Convert.ToInt32(getNext.ExecuteScalar());
                        }

                        string[] _badWords = { "spam", "fake", "terrible", "awful", "hate", "stupid", "garbage", "ridiculous", "idiots", "damaged", "dumb", "shocked", "avoid", "never", "regret", "concerned", "swear", "don't", "poor", "not", "horrible", "difficult", "hard", "unsure", "delay", "inaccurate", "worthless", "trash", "nonsense", "fraud", "slow", "unresponsive", "driver", "seller", "admin" };

                        int isFlagged = 0;

                        if (_badWords.Any(word => Regex.IsMatch(comment, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase)))
                        {
                            isFlagged = 1;
                        }

                        using (var insert = new SqlCommand(@"
                            INSERT INTO Review (reviewID, reviewRating, reviewComment, saleID, isFlagged, reviewDate)
                            VALUES (@id, @rating, @comment, @saleID, @isFlagged, GETDATE());", con, tx))
                        {
                            insert.Parameters.AddWithValue("@id", nextReviewID);
                            insert.Parameters.AddWithValue("@rating", rating);
                            insert.Parameters.AddWithValue("@comment", (object)comment ?? DBNull.Value);
                            insert.Parameters.AddWithValue("@saleID", saleID);
                            insert.Parameters.AddWithValue("@isFlagged", isFlagged);
                            insert.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                pnlReviewPanel.CssClass = "side-panel";
               

                LoadProductSummary(bookISBN);
                LoadReviewHistory(buyerId);
                lblError.Visible = false;

                txtReviewComment.Text = string.Empty;
                ddlRating.SelectedValue = "5";
                pnlReviewPanel.CssClass = "side-panel";

                lblSuccess.Text = " Your review has been submitted successfully!";
                lblSuccess.Visible = true;
                lblError.Visible = false;
                // Hide after 5 seconds using JavaScript
                ClientScript.RegisterStartupScript(this.GetType(), "HideMessage",
                    "setTimeout(function() { document.getElementById('" + lblSuccess.ClientID + "').style.display='none'; }, 5000);", true);

            }
            catch (Exception ex)
            {
                lblError.Text = " Error: " + ex.Message;
                lblError.Visible = true;
            }
        }


        protected void btnDeleteReview_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                hfDeleteReviewID.Value = btn.CommandArgument;
                pnlDeleteConfirm.Visible = true; // Use Visible to show the panel
                lblError.Visible = false;
            }
            catch (Exception ex)
            {
                lblError.Text = " Error preparing delete: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnConfirmYes_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfDeleteReviewID.Value))
            {
                lblError.Text = " No review selected for deletion.";
                lblError.Visible = true;
                pnlDeleteConfirm.Visible = false;
                return;
            }

            try
            {
                int reviewID = Convert.ToInt32(hfDeleteReviewID.Value);

                using (var con = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("DELETE FROM Review WHERE reviewID = @reviewID", con))
                {
                    cmd.Parameters.AddWithValue("@reviewID", reviewID);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        lblError.Text = " Review not found or already deleted.";
                        lblError.Visible = true;
                        pnlDeleteConfirm.Visible = false;
                        return;
                    }
                }

                int buyerId = Convert.ToInt32(Session["buyerID"]);
                LoadReviewHistory(buyerId); // Refresh history

                lblSuccess.Text = " Review deleted successfully.";
                lblSuccess.Visible = true;

                // Auto-hide success message after 5 seconds
                ClientScript.RegisterStartupScript(this.GetType(), "HideMessage",
                    $"setTimeout(function() {{ document.getElementById('{lblSuccess.ClientID}').style.display='none'; }}, 5000);", true);

                pnlDeleteConfirm.Visible = false;
                hfDeleteReviewID.Value = string.Empty;
            }
            catch (Exception ex)
            {
                lblError.Text = " Error deleting review: " + ex.Message;
                lblError.Visible = true;
                pnlDeleteConfirm.Visible = false;
            }
        }

        protected void btnConfirmNo_Click(object sender, EventArgs e)
        {
            pnlDeleteConfirm.Visible = false; // Just hide the panel, no redirect
            hfDeleteReviewID.Value = string.Empty;
        }
        protected void btnCloseReview_Click(object sender, EventArgs e)
        {
            pnlReviewPanel.CssClass = "side-panel";
        }

        protected void btnViewGuidelines_Click(object sender, EventArgs e)
        {
            pnlGuidelines.CssClass = "guidelines show";
        }

        protected void btnCloseGuidelines_Click(object sender, EventArgs e)
        {
            pnlGuidelines.CssClass = "guidelines hidden";
        }

        protected void btnViewPastOrders_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Buyer/pabiModule/BuyerOrders.aspx");
        }
    }
}
