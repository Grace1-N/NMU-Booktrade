using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace NMU_BookTrade
{
    public partial class ViewTextBookDetails : System.Web.UI.Page
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string bookISBN = Request.QueryString["bookISBN"];
                if (!string.IsNullOrEmpty(bookISBN))
                {
                    LoadBookDetails(bookISBN);
                    LoadReviews(bookISBN);
                }
                else
                {
                    lblMessage.Text = "No book selected.";
                    lblMessage.CssClass = "error-message";
                }
            }
        }

        private void LoadBookDetails(string bookISBN)
        {
            using (SqlConnection con = new SqlConnection(connString))
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
                    b.condition,
                    c.categoryName,
                    g.genreName,
                    s.sellerName,
                    s.sellerSurname
                FROM Book b
                LEFT JOIN Category c ON b.categoryID = c.categoryID
                LEFT JOIN Genre g ON b.genreID = g.genreID
                LEFT JOIN Seller s ON b.sellerID = s.sellerID
                WHERE b.bookISBN = @bookISBN";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblTitle.Text = reader["title"] != DBNull.Value ? reader["title"].ToString() : "N/A";
                                lblAuthor.Text = reader["author"] != DBNull.Value ? reader["author"].ToString() : "N/A";
                                lblISBN.Text = reader["bookISBN"] != DBNull.Value ? reader["bookISBN"].ToString() : "N/A";
                                lblCategory.Text = reader["categoryName"] != DBNull.Value ? reader["categoryName"].ToString() : "N/A";
                                lblGenre.Text = reader["genreName"] != DBNull.Value ? reader["genreName"].ToString() : "N/A";
                                lblCondition.Text = reader["condition"] != DBNull.Value ? reader["condition"].ToString() : "N/A";
                                lblPrice.Text = reader["price"] != DBNull.Value ? Convert.ToDecimal(reader["price"]).ToString("F2") : "N/A";
                                lblSeller.Text = reader["sellerName"] != DBNull.Value && reader["sellerSurname"] != DBNull.Value
                                    ? $"{reader["sellerName"]} {reader["sellerSurname"]}" : "N/A";

                                // Build the relative path to the image
                                string coverImage = reader["coverImage"] != DBNull.Value
                                    ? reader["coverImage"].ToString().Trim()
                                    : "";

                                string imagePath = "~/Images/no-image.png";

                                if (!string.IsNullOrEmpty(coverImage))
                                {
                                    if (System.IO.File.Exists(Server.MapPath(coverImage)))
                                    {
                                        imagePath = coverImage;
                                    }
                                }
                                bookCover.ImageUrl = ResolveUrl(imagePath);

                                // ✅ Set the <img> Src property
                                //bookCover.Src = ResolveUrl(imagePath);
                                //System.Diagnostics.Debug.WriteLine($"✅ BookCover: ISBN={bookISBN}, Src={bookCover.Src}");

                                litPageTitle.Text = $"Textbook Details: {lblTitle.Text}";

                                // ✅ Enable or disable Add to Cart button based on availability
                                btnAddToCart.Enabled = reader["status"] != DBNull.Value && reader["status"].ToString().ToLower() == "available";
                                if (!btnAddToCart.Enabled)
                                {
                                    lblMessage.Text = "This book is not available.";
                                    lblMessage.CssClass = "error-message";
                                }
                            }
                            else
                            {
                                lblMessage.Text = "Book not found.";
                                lblMessage.CssClass = "error-message";
                                btnAddToCart.Enabled = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Error loading book details: {ex.Message}";
                    lblMessage.CssClass = "error-message";
                    btnAddToCart.Enabled = false;
                    System.Diagnostics.Debug.WriteLine($"❌ LoadBookDetails Error: ISBN={bookISBN}, Exception={ex.Message}");
                }
            }
        }


        private void LoadReviews(string bookISBN)
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            r.reviewComment,
                            r.reviewRating,
                            b.buyerName,
                            b.buyerSurname,
                            b.buyerProfileImage
                        FROM Review r
                        INNER JOIN Sale s ON r.saleID = s.saleID
                        INNER JOIN Buyer b ON s.buyerID = b.buyerID
                        WHERE s.bookISBN = @bookISBN";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Normalize buyerProfileImage paths
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["buyerProfileImage"] == DBNull.Value || string.IsNullOrEmpty(row["buyerProfileImage"]?.ToString()))
                            {
                                row["buyerProfileImage"] = "~/Images/no-profile.png";
                            }
                            else
                            {
                                string profileImage = row["buyerProfileImage"].ToString().Trim();
                                if (!profileImage.StartsWith("~/UploadedImages/"))
                                {
                                    row["buyerProfileImage"] = "~/UploadedImages/" + profileImage;
                                }
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"LoadReviews: Rows returned for bookISBN={bookISBN}: {dt.Rows.Count}");

                        rptTestimonials.DataSource = dt;
                        rptTestimonials.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Error loading reviews: {ex.Message}";
                    lblMessage.CssClass = "error-message";
                    System.Diagnostics.Debug.WriteLine($"LoadReviews Error: ISBN={bookISBN}, Exception={ex.Message}, StackTrace={ex.StackTrace}");
                }
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (Session["buyerID"] == null)
            {
                Response.Redirect("~/Login");
                return;
            }

            string bookISBN = Request.QueryString["bookISBN"];
            int buyerID = Convert.ToInt32(Session["buyerID"]);

            if (!IsBookAvailable(bookISBN))
            {
                lblMessage.Text = "Sorry, this book is no longer available.";
                lblMessage.CssClass = "error-message";
                return;
            }

            AddToCart(buyerID, bookISBN, 1);

            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT title, coverImage FROM Book WHERE bookISBN = @bookISBN", con);
                    cmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblCartBookTitle.Text = reader["title"] != DBNull.Value ? reader["title"].ToString() : "N/A";
                            string coverImage = reader["coverImage"] != DBNull.Value ? reader["coverImage"].ToString() : "";
                            string imagePath = string.IsNullOrEmpty(coverImage) ? "~/Images/no-image.png" : coverImage.Trim();
                            imgCartBook.ImageUrl = ResolveUrl(imagePath);
                            System.Diagnostics.Debug.WriteLine($"btnAddToCart: ISBN={bookISBN}, Raw coverImage='{coverImage}', Resolved ImageUrl='{imgCartBook.ImageUrl}'");
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Error loading cart image: {ex.Message}";
                    lblMessage.CssClass = "error-message";
                    System.Diagnostics.Debug.WriteLine($"btnAddToCart Error: ISBN={bookISBN}, Exception={ex.Message}, StackTrace={ex.StackTrace}");
                }
            }

            CartPanel.Visible = true;
            CartPanel.CssClass = "slide-panel slide-panel-visible";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "PanelOpen",
                "document.body.classList.add('panel-open');", true);
            lblMessage.Text = "";
            ((Site1)this.Master).UpdateCartCount();
        }

        private bool IsBookAvailable(string bookISBN)
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Book WHERE bookISBN = @bookISBN AND status = 'available'", con);
                    cmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"IsBookAvailable Error: ISBN={bookISBN}, Exception={ex.Message}, StackTrace={ex.StackTrace}");
                    return false;
                }
            }
        }

        private void AddToCart(int buyerID, string bookISBN, int quantity)
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                try
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
                catch (Exception ex)
                {
                    lblMessage.Text = $"Error adding to cart: {ex.Message}";
                    lblMessage.CssClass = "error-message";
                    System.Diagnostics.Debug.WriteLine($"AddToCart Error: ISBN={bookISBN}, BuyerID={buyerID}, Exception={ex.Message}, StackTrace={ex.StackTrace}");
                }
            }
        }

        private int CreateCart(SqlConnection con, int buyerID)
        {
            try
            {
                SqlCommand createCartCmd = new SqlCommand("INSERT INTO Cart (buyerID) OUTPUT INSERTED.cartID VALUES (@buyerID)", con);
                createCartCmd.Parameters.AddWithValue("@buyerID", buyerID);
                return (int)createCartCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateCart Error: BuyerID={buyerID}, Exception={ex.Message}, StackTrace={ex.StackTrace}");
                throw;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            CartPanel.Visible = false;
            CartPanel.CssClass = "slide-panel";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "PanelClose",
                "document.body.classList.remove('panel-open');", true);
        }

        protected string GetStarHtml(int rating)
        {
            System.Text.StringBuilder starsHtml = new System.Text.StringBuilder();

            for (int i = 0; i < 5; i++)
            {
                if (i < rating)
                    starsHtml.Append("<i class='fa-solid fa-star'></i>");
                else
                    starsHtml.Append("<i class='fa-regular fa-star'></i>");
            }

            return $"<span class='stars'>{starsHtml}</span>";
        }
    }
}