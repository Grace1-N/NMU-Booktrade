using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.Seller.ClintonModule
{
    public partial class CreateListings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Updated session-based authentication check for sellers
                if (Session["AccessID"] == null || Session["AccessID"].ToString() != "3" || Session["SellerID"] == null)
                {
                    Response.Redirect("~/UserManagement/Login.aspx");
                    return;
                }
                else
                {
                    LoadCategories();
                }
            }

            // Ensure form supports file uploads
            if (Page.Form != null)
            {
                Page.Form.Enctype = "multipart/form-data";
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
        }

        

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    ShowAlert("Please enter a book title");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtAuthor.Text))
                {
                    ShowAlert("Please enter the book author");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtISBN.Text))
                {
                    ShowAlert("Please enter the book ISBN");
                    return;
                }
                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                {
                    ShowAlert("Please enter a valid price");
                    return;
                }
                if (string.IsNullOrEmpty(ddlCategory.SelectedValue))
                {
                    ShowAlert("Please select a category");
                    return;
                }
                if (string.IsNullOrEmpty(ddlGenre.SelectedValue))
                {
                    ShowAlert("Please select a genre");
                    return;
                }

                // Handle image upload
                string imagePath = null;
                if (fuImage.HasFile)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(fuImage.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ShowAlert("Only JPG, PNG, and GIF images are allowed");
                        return;
                    }

                    // Create uploads directory if it doesn't exist
                    string uploadDir = Server.MapPath("~/Images/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // Generate unique filename
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    imagePath = "/Images/" + fileName;
                    fuImage.SaveAs(Server.MapPath(imagePath));
                }
                lblFileName.Text = fuImage.FileName;
                // Get current seller ID from session
                int sellerId = Convert.ToInt32(Session["SellerID"]);

                // Save to database
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Book 
                            (bookISBN, title, author, price, condition, sellerID, coverImage, categoryID, genreID)
                            VALUES 
                            (@ISBN, @Title, @Author, @Price, @Condition, @SellerID, @CoverImage, @CategoryID, @GenreID)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ISBN", txtISBN.Text);
                        cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
                        cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Condition", ddlCondition.SelectedValue);
                        cmd.Parameters.AddWithValue("@SellerID", sellerId);
                        cmd.Parameters.AddWithValue("@CoverImage", imagePath != null ? (object)imagePath : DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryID", Convert.ToInt32(ddlCategory.SelectedValue));
                        cmd.Parameters.AddWithValue("@GenreID", Convert.ToInt32(ddlGenre.SelectedValue));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowAlert("Book listing created successfully!", "success");
                            ClearForm();
                        }
                        else
                        {
                            ShowAlert("Failed to create listing. Please try again.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627) // Unique key violation (duplicate ISBN)
                {
                    ShowAlert("A book with this ISBN already exists in the system.");
                }
                else
                {
                    ShowAlert("Database error: " + sqlEx.Message);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error creating listing: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtTitle.Text = "";
            txtAuthor.Text = "";
            txtISBN.Text = "";
            txtPrice.Text = "";
            ddlCondition.SelectedIndex = 0; // Reset to first option
            ddlCategory.SelectedIndex = 0; // Reset to "Select a category"
            ddlGenre.Items.Clear();
            ddlGenre.Items.Add(new ListItem("Select a category first", ""));
            ddlGenre.Enabled = false;
            
            lblFileName.Text = "";

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Seller/ClintonModule/SellerDashboard.aspx");
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

        // Removed SetActiveTab method as the required tab controls don't exist in this page
    }
}