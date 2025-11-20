using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class SearchResult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string searchTerm = Request.QueryString["query"];
                string categoryIdQS = Request.QueryString["categoryID"];
                string categoryName = Request.QueryString["categoryName"];

                lblSearched.Text = "";
                lblMessage.Text = "";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    string displayText = GetDisplayText(searchTerm);
                    lblSearched.Text = "Search results for: " + Server.HtmlEncode(displayText);
                    LoadSearchResults(searchTerm);
                }
                else if (!string.IsNullOrEmpty(categoryIdQS))
                {
                    if (int.TryParse(categoryIdQS, out int categoryId))
                    {
                        LoadBooksByCategory(categoryId);
                        lblSearched.Text = !string.IsNullOrEmpty(categoryName)
                            ? "Books in " + Server.HtmlEncode(categoryName)
                            : "Books in selected category";
                    }
                    else
                    {
                        lblSearched.Text = "Invalid category ID.";
                        lblMessage.CssClass = "error-message";
                    }
                }
                else
                {
                    lblSearched.Text = "No search or category selected.";
                    lblMessage.CssClass = "error-message";
                }

                LoadCategories();
                ((Site1)this.Master).UpdateCartCount();
            }
        }

        private void LoadBooksByCategory(int categoryId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                try
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            b.bookISBN,
                            b.title,
                            b.author,
                            b.price,
                            b.coverImage,
                            b.status,
                            ISNULL(AVG(CAST(r.reviewRating AS FLOAT)), 0) AS AvgRating,
                            COUNT(r.reviewID) AS ReviewCount
                        FROM Book b
                        LEFT JOIN Sale s ON b.bookISBN = s.bookISBN
                        LEFT JOIN Review r ON s.saleID = r.saleID
                        WHERE b.categoryID = @categoryID AND b.status = 'available'
                        GROUP BY b.bookISBN, b.title, b.author, b.price, b.coverImage, b.status";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@categoryID", categoryId);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            string coverImage = row["coverImage"] != DBNull.Value
                                    ? row["coverImage"].ToString().Trim()
                                    : "";

                            string imagePath = "~/Images/no-image.png";

                            if (!string.IsNullOrEmpty(coverImage))
                            {
                                if (System.IO.File.Exists(Server.MapPath(coverImage)))
                                {
                                    imagePath = coverImage;
                                }
                            }
                            imgCartBook.ImageUrl = ResolveUrl(imagePath);
                        }

                        System.Diagnostics.Debug.WriteLine($"LoadBooksByCategory: Rows returned for categoryId {categoryId}: {dt.Rows.Count}");

                        if (dt.Rows.Count == 0)
                        {
                            lblMessage.Text = "No books found in this category.";
                            lblMessage.CssClass = "error-message";
                            rptBooks.DataSource = null;
                        }
                        else
                        {
                            lblMessage.Text = "";
                            rptBooks.DataSource = dt;
                        }

                        rptBooks.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Database error in LoadBooksByCategory: {ex.Message}";
                    lblMessage.CssClass = "error-message";
                    rptBooks.DataSource = null;
                    rptBooks.DataBind();
                }
            }
        }

        private string GetDisplayText(string searchTerm)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string sqlIsbn = "SELECT TOP 1 title FROM Book WHERE bookISBN = @isbn";
                using (SqlCommand cmd = new SqlCommand(sqlIsbn, con))
                {
                    cmd.Parameters.AddWithValue("@isbn", searchTerm.Trim());
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString();
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string sqlLike = "SELECT TOP 1 title FROM Book WHERE title LIKE @term OR author LIKE @term";
                using (SqlCommand cmd = new SqlCommand(sqlLike, con))
                {
                    cmd.Parameters.AddWithValue("@term", "%" + searchTerm.Trim() + "%");
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString();
                    }
                }
            }

            return searchTerm;
        }

        private void LoadSearchResults(string searchTerm)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string query = @"
                    SELECT 
                        b.bookISBN, 
                        b.title, 
                        b.author, 
                        b.price, 
                        b.coverImage, 
                        b.status,
                        ISNULL(AVG(CAST(r.reviewRating AS FLOAT)), 0) AS AvgRating,
                        COUNT(r.reviewID) AS ReviewCount
                    FROM Book b
                    LEFT JOIN Sale s ON b.bookISBN = s.bookISBN
                    LEFT JOIN Review r ON s.saleID = r.saleID
                    WHERE (b.title LIKE @searchTerm OR b.author LIKE @searchTerm OR b.bookISBN LIKE @searchTerm) 
                        AND b.status = 'available'
                    GROUP BY b.bookISBN, b.title, b.author, b.price, b.coverImage, b.status";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm.Trim() + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Normalize coverImage paths
                foreach (DataRow row in dt.Rows)
                {
                    string raw = row["coverImage"] == DBNull.Value
                     ? "" : row["coverImage"].ToString().Trim();

                    string relPath = "~/Images/no-image.png";

                    if (!string.IsNullOrEmpty(raw))
                    {
                        relPath = raw;
                    }

                    string webUrl = ResolveUrl(relPath);
                    row["coverImage"] = webUrl;
                }

                System.Diagnostics.Debug.WriteLine($"LoadSearchResults: Rows returned for searchTerm '{searchTerm}': {dt.Rows.Count}");

                if (dt.Rows.Count > 0)
                {
                    rptBooks.DataSource = dt;
                    rptBooks.DataBind();
                }
                else
                {
                    lblSearched.Text = "No results found for '" + Server.HtmlEncode(searchTerm) + "'";
                    lblMessage.CssClass = "error-message";
                    rptBooks.DataSource = null;
                    rptBooks.DataBind();
                }
            }
        }

        protected void LoadCategories()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT categoryID, categoryName FROM Category ORDER BY categoryName", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                int splitIndex = dt.Rows.Count / 2;

                DataTable dt1 = dt.Clone();
                DataTable dt2 = dt.Clone();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i < splitIndex)
                        dt1.ImportRow(dt.Rows[i]);
                    else
                        dt2.ImportRow(dt.Rows[i]);
                }

                rptCategory1.DataSource = dt1;
                rptCategory1.DataBind();

                rptCategory2.DataSource = dt2;
                rptCategory2.DataBind();
            }
        }

        protected void rptCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectCategory")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int categoryId = Convert.ToInt32(args[0]);
                string categoryName = args[1];

                Response.Redirect($"~/Buyer/pabiModule/SearchResult.aspx?categoryID={categoryId}&categoryName={Server.UrlEncode(categoryName)}");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                lblSearched.Text = "Search results for: " + Server.HtmlEncode(GetDisplayText(searchTerm));
                lblMessage.Text = "";
                LoadSearchResults(searchTerm);
            }
            else
            {
                lblSearched.Text = "Please enter a search term.";
                lblMessage.CssClass = "error-message";
                rptBooks.DataSource = null;
                rptBooks.DataBind();
            }
        }

        protected void rptBooks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewReviews")
            {
                string bookISBN = e.CommandArgument.ToString();
                Response.Redirect("~/Buyer/pabiModule/ViewTextBookDetails.aspx?bookISBN=" + Server.UrlEncode(bookISBN));
            }

            if (e.CommandName == "AddToCart")
            {
                // 🔒 1. Check if user is logged in
                if (Session["AccessID"] == null)
                {
                    // Redirect anonymous users to Login, with return URL
                    Response.Redirect("~/UserManagement/Login.aspx?returnUrl=" + Request.RawUrl);
                    return;
                }

                // 🔒 2. Check that only Buyers can add to cart
                if (Session["AccessID"].ToString() != "2")
                {
                    // Logged in but wrong role → Access Denied
                    Response.Redirect("~/UserManagement/AccessDenied.aspx");
                    return;
                }

                int buyerID = Convert.ToInt32(Session["buyerID"]);
                string bookISBN = e.CommandArgument.ToString();

                if (!IsBookAvailable(bookISBN))
                {
                    lblMessage.Text = "Sorry, this book is no longer available.";
                    lblMessage.CssClass = "error-message";
                    return;
                }

                AddToCart(buyerID, bookISBN, 1);

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT title, coverImage, price FROM Book WHERE bookISBN = @bookISBN", con);
                    cmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblCartBookTitle.Text = reader["title"] != DBNull.Value ? reader["title"].ToString() : "N/A";
                        string coverImage = reader["coverImage"] != DBNull.Value ? reader["coverImage"].ToString() : "";
                        imgCartBook.ImageUrl = string.IsNullOrEmpty(coverImage) ? ResolveUrl("~/Images/no-image.png") : ResolveUrl(coverImage.Trim());
                    }
                }

                CartPanel.Visible = true;
                CartPanel.CssClass = "slide-panel slide-panel-visible";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "PanelOpen",
                    "document.body.classList.add('panel-open');", true);
                lblMessage.Text = "";
                ((Site1)this.Master).UpdateCartCount();
            }
        }

        private bool IsBookAvailable(string bookISBN)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Book WHERE bookISBN = @bookISBN AND status = 'available'", con);
                cmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        protected string GetStarIcons(double rating)
        {
            int fullStars = (int)Math.Floor(rating);
            bool hasHalfStar = (rating - fullStars) >= 0.5;
            int emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

            System.Text.StringBuilder starsHtml = new System.Text.StringBuilder();

            for (int i = 0; i < fullStars; i++)
                starsHtml.Append("<i class='fa-solid fa-star'></i>");

            if (hasHalfStar)
                starsHtml.Append("<i class='fa-solid fa-star-half-stroke'></i>");

            for (int i = 0; i < emptyStars; i++)
                starsHtml.Append("<i class='fa-regular fa-star'></i>");

            return $"<span class='stars'>{starsHtml}</span>";
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            CartPanel.Visible = false;
            CartPanel.CssClass = "slide-panel";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "PanelClose",
                "document.body.classList.remove('panel-open');", true);
        }

        private void AddToCart(int buyerID, string bookISBN, int quantity)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                con.Open();

                SqlCommand getCartCmd = new SqlCommand("SELECT cartID FROM Cart WHERE buyerID = @buyerID", con);
                getCartCmd.Parameters.AddWithValue("@buyerID", buyerID);
                object result = getCartCmd.ExecuteScalar();
                int cartID = result != null ? Convert.ToInt32(result) : CreateCart(con, buyerID);

                SqlCommand checkCmd = new SqlCommand("SELECT quantity FROM CartItems WHERE cartID = @cartID AND bookISBN = @bookISBN", con);
                checkCmd.Parameters.AddWithValue("@cartID", cartID);
                checkCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                object qtyResult = checkCmd.ExecuteScalar();

                if (qtyResult != null)
                {
                    SqlCommand updateCmd = new SqlCommand("UPDATE CartItems SET quantity = quantity + @qty WHERE cartID = @cartID AND bookISBN = @bookISBN", con);
                    updateCmd.Parameters.AddWithValue("@qty", quantity);
                    updateCmd.Parameters.AddWithValue("@cartID", cartID);
                    updateCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand insertCmd = new SqlCommand("INSERT INTO CartItems (cartID, bookISBN, quantity) VALUES (@cartID, @bookISBN, @qty)", con);
                    insertCmd.Parameters.AddWithValue("@cartID", cartID);
                    insertCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                    insertCmd.Parameters.AddWithValue("@qty", quantity);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        private int CreateCart(SqlConnection con, int buyerID)
        {
            SqlCommand createCartCmd = new SqlCommand("INSERT INTO Cart (buyerID) OUTPUT INSERTED.cartID VALUES (@buyerID)", con);
            createCartCmd.Parameters.AddWithValue("@buyerID", buyerID);
            return (int)createCartCmd.ExecuteScalar();
        }
    }
}