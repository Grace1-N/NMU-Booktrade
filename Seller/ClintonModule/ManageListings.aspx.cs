using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.Seller.ClintonModule
{
    public partial class ManageListings : System.Web.UI.Page
    {
        private int currentBookId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Session-based authentication check for sellers
                if (Session["AccessID"] == null || Session["AccessID"].ToString() != "3" || Session["SellerID"] == null)
                {
                    Response.Redirect("~/UserManagement/Login.aspx");
                    return;
                }
                LoadCategories();
                BindListings();
            }
        }

        private void LoadCategories()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT categoryID, categoryName FROM Category ORDER BY categoryName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        ddlCategory.Items.Clear();
                        ddlCategory.Items.Add(new ListItem("Select a category", ""));

                        foreach (DataRow row in dt.Rows)
                        {
                            ddlCategory.Items.Add(new ListItem(
                                row["categoryName"].ToString(),
                                row["categoryID"].ToString()
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading categories: " + ex.Message);
            }
        }

        private void LoadGenres(int categoryId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT genreID, genreName FROM Genre WHERE categoryID = @CategoryID ORDER BY genreName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            ddlGenre.Items.Clear();
                            ddlGenre.Items.Add(new ListItem("Select a genre", ""));

                            foreach (DataRow row in dt.Rows)
                            {
                                ddlGenre.Items.Add(new ListItem(
                                    row["genreName"].ToString(),
                                    row["genreID"].ToString()
                                ));
                            }

                            ddlGenre.Enabled = dt.Rows.Count > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading genres: " + ex.Message);
                ddlGenre.Enabled = false;
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlCategory.SelectedValue))
            {
                int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                LoadGenres(categoryId);
            }
            else
            {
                ddlGenre.Items.Clear();
                ddlGenre.Items.Add(new ListItem("Select a category first", ""));
                ddlGenre.Enabled = false;
            }
            ShowEditModal(); // Keep the modal open
        }

        private void BindListings()
        {
            int sellerId = Convert.ToInt32(Session["SellerID"]);
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    // First, check if the seller has any books
                    string checkQuery = "SELECT COUNT(*) FROM Book WHERE sellerID = @SellerID";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@SellerID", sellerId);
                        int bookCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (bookCount == 0)
                        {
                            // No books found for this seller
                            gvListings.DataSource = dt;
                            gvListings.DataBind();
                            return;
                        }
                    }

                    string query = "SELECT bookISBN, title, author, price, condition, coverImage FROM Book WHERE sellerID = @SellerID ORDER BY title";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SellerID", sellerId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                gvListings.DataSource = dt;
                gvListings.DataBind();
            }
            catch (Exception ex)
            {
                // Log the error or show a user-friendly message
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    $"alert('Error loading listings: {ex.Message}');", true);
            }
        }

        protected void gvListings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string bookISBN = e.CommandArgument.ToString();
            if (e.CommandName == "EditListing")
            {
                LoadListingData(bookISBN);
                ShowEditModal();
            }
            else if (e.CommandName == "DeleteListing")
            {
                DeleteListing(bookISBN);
                BindListings();
            }
        }

        protected void gvListings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the data item for this row
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Add tooltip for better UX
                e.Row.ToolTip = $"Click Edit to modify or Delete to remove this listing";

                // Format and style the condition column
                string condition = rowView["condition"].ToString().ToLower();
                Label lblCondition = (Label)e.Row.FindControl("lblCondition");
                if (lblCondition == null)
                {
                    // If no label control exists, we'll style the cell directly
                    TableCell conditionCell = e.Row.Cells[3]; // Condition is the 4th column (index 3)
                    if (conditionCell != null)
                    {
                        // Apply CSS classes based on condition
                        switch (condition)
                        {
                            case "excellent":
                                conditionCell.CssClass = "condition-excellent";
                                conditionCell.Text = "Excellent";
                                break;
                            case "very-good":
                                conditionCell.CssClass = "condition-very-good";
                                conditionCell.Text = "Very Good";
                                break;
                            case "good":
                                conditionCell.CssClass = "condition-good";
                                conditionCell.Text = "Good";
                                break;
                            case "fair":
                                conditionCell.CssClass = "condition-fair";
                                conditionCell.Text = "Fair";
                                break;
                            case "poor":
                                conditionCell.CssClass = "condition-poor";
                                conditionCell.Text = "Poor";
                                break;
                            default:
                                conditionCell.Text = condition;
                                break;
                        }
                    }
                }

                // Add hover effect class
                e.Row.CssClass = "listing-row";

                // Validate price and add warning if needed
                if (rowView["price"] != DBNull.Value)
                {
                    decimal price = Convert.ToDecimal(rowView["price"]);
                    if (price <= 0)
                    {
                        e.Row.CssClass += " price-warning";
                        e.Row.ToolTip += " - Warning: Price is set to zero or negative";
                    }
                }

                // Add confirmation for delete button
                Button btnDelete = (Button)e.Row.FindControl("btnDelete");
                if (btnDelete != null)
                {
                    btnDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this listing?');");
                }
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                // Style the header row
                e.Row.CssClass = "listing-header";
            }
        }

        private void LoadListingData(string bookISBN)
        {
            int sellerId = Convert.ToInt32(Session["SellerID"]);
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                connection.Open();
                string query = @"SELECT bookISBN, title, author, price, condition, coverImage, categoryID, genreID FROM Book WHERE bookISBN = @BookISBN AND sellerID = @SellerID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@BookISBN", bookISBN);
                    cmd.Parameters.AddWithValue("@SellerID", sellerId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTitle.Text = reader["title"].ToString();
                            txtISBN.Text = reader["bookISBN"].ToString();
                            txtAuthor.Text = reader["author"]?.ToString() ?? "";
                            decimal price = Convert.ToDecimal(reader["price"]);
                            txtPrice.Text = $"R{price:0.00}";
                            ddlCondition.SelectedValue = reader["condition"].ToString();

                            // Set category and genre
                            if (reader["categoryID"] != DBNull.Value)
                            {
                                ddlCategory.SelectedValue = reader["categoryID"].ToString();
                                int categoryId = Convert.ToInt32(reader["categoryID"]);
                                LoadGenres(categoryId);

                                if (reader["genreID"] != DBNull.Value)
                                {
                                    string genreId = reader["genreID"].ToString();
                                    // Check if the genre exists in the dropdown before setting SelectedValue
                                    if (ddlGenre.Items.FindByValue(genreId) != null)
                                    {
                                        ddlGenre.SelectedValue = genreId;
                                    }
                                    else
                                    {
                                        // Genre not found, select the first item (default)
                                        ddlGenre.SelectedIndex = 0;
                                    }
                                }
                            }

                            // Handle cover image
                            string coverImagePath = reader["coverImage"]?.ToString();
                            if (!string.IsNullOrEmpty(coverImagePath))
                            {
                                imgCoverImage.ImageUrl = coverImagePath;
                                imgCoverImage.Visible = true;
                                lblNoImage.Visible = false;
                            }
                            else
                            {
                                imgCoverImage.Visible = false;
                                lblNoImage.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    ShowAlert("Please enter a book title");
                    ShowEditModal();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtISBN.Text))
                {
                    ShowAlert("Please enter the book ISBN");
                    ShowEditModal();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtAuthor.Text))
                {
                    ShowAlert("Please enter the book author");
                    ShowEditModal();
                    return;
                }
                string priceText = txtPrice.Text.Replace("R", "").Trim();
                if (!decimal.TryParse(priceText, out decimal price) || price <= 0)
                {
                    ShowAlert("Please enter a valid price");
                    ShowEditModal();
                    return;
                }
                if (string.IsNullOrEmpty(ddlCategory.SelectedValue))
                {
                    ShowAlert("Please select a category");
                    ShowEditModal();
                    return;
                }
                if (string.IsNullOrEmpty(ddlGenre.SelectedValue))
                {
                    ShowAlert("Please select a genre");
                    ShowEditModal();
                    return;
                }
                int sellerId = Convert.ToInt32(Session["SellerID"]);

                // Handle cover image upload
                string coverImagePath = imgCoverImage.ImageUrl; // default to existing
                if (fuCoverImage.HasFile)
                {
                    string ext = System.IO.Path.GetExtension(fuCoverImage.FileName).ToLower();
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif")
                    {
                        ShowAlert("Only image files (jpg, jpeg, png, gif) are allowed.");
                        ShowEditModal();
                        return;
                    }
                    string fileName = "cover_" + Guid.NewGuid().ToString() + ext;
                    string savePath = Server.MapPath("~/Images/" + fileName);
                    fuCoverImage.SaveAs(savePath);
                    coverImagePath = "~/Images/" + fileName;
                }

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Book SET title = @Title, author = @Author, price = @Price, condition = @Condition, coverImage = @CoverImage, categoryID = @CategoryID, genreID = @GenreID WHERE bookISBN = @BookISBN AND sellerID = @SellerID";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
                        cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Condition", ddlCondition.SelectedValue);
                        cmd.Parameters.AddWithValue("@CoverImage", coverImagePath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryID", Convert.ToInt32(ddlCategory.SelectedValue));
                        cmd.Parameters.AddWithValue("@GenreID", Convert.ToInt32(ddlGenre.SelectedValue));
                        cmd.Parameters.AddWithValue("@BookISBN", txtISBN.Text);
                        cmd.Parameters.AddWithValue("@SellerID", sellerId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ShowAlert("Listing updated successfully!", "success");
                            BindListings();
                        }
                        else
                        {
                            ShowAlert("Failed to update listing. Please try again.");
                            ShowEditModal();
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ShowAlert("Database error: " + sqlEx.Message);
                ShowEditModal();
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating listing: " + ex.Message);
                ShowEditModal();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteListing(txtISBN.Text);
            BindListings();
        }

        private void DeleteListing(string bookISBN)
        {
            try
            {
                int sellerId = Convert.ToInt32(Session["SellerID"]);
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();
                    string checkSalesQuery = @"SELECT COUNT(*) FROM Sale WHERE bookISBN = @BookISBN";
                    using (SqlCommand checkCmd = new SqlCommand(checkSalesQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@BookISBN", bookISBN);
                        int saleCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (saleCount > 0)
                        {
                            ShowAlert("Cannot delete this book as it has existing sales. Please contact support.");
                            return;
                        }
                    }
                    string deleteQuery = @"DELETE FROM Book WHERE bookISBN = @BookISBN AND sellerID = @SellerID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@BookISBN", bookISBN);
                        cmd.Parameters.AddWithValue("@SellerID", sellerId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ShowAlert("Listing deleted successfully!", "success");
                        }
                        else
                        {
                            ShowAlert("Failed to delete listing. Please try again.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ShowAlert("Database error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                ShowAlert("Error deleting listing: " + ex.Message);
            }
        }

        private void ShowEditModal()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal", "showEditModal();", true);
        }

        private void HideEditModal()
       {
           ScriptManager.RegisterStartupScript(this, GetType(), "HideEditModal", "hideEditModal();", true);
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
    }
}
