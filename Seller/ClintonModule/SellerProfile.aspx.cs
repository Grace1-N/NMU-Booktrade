using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class SellerProfile : System.Web.UI.Page
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
                    LoadSellerProfile();
                }
            }
        }

        protected void LoadSellerProfile()
        {
            int sellerID = Convert.ToInt32(Session["SellerID"]);

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string query = @"SELECT sellerUsername, sellerName, sellerSurname, sellerEmail, 
                               sellerNumber, sellerAddress, sellerProfileImage 
                               FROM Seller WHERE sellerID = @ID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", sellerID);

                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtUsername.Text = reader["sellerUsername"].ToString();
                                txtName.Text = reader["sellerName"].ToString();
                                txtSurname.Text = reader["sellerSurname"].ToString();
                                txtEmail.Text = reader["sellerEmail"].ToString();
                                txtNumber.Text = reader["sellerNumber"].ToString();
                                txtAddress.Text = reader["sellerAddress"].ToString();



                                // Handle profile image
                                if (!reader.IsDBNull(reader.GetOrdinal("sellerProfileImage")))
                                {
                                    string imageName = reader["sellerProfileImage"].ToString();
                                    imgProfile.ImageUrl = $"~/UploadedImages/{imageName}";
                                }
                                else
                                {
                                    imgProfile.ImageUrl = "~/Images/default.png";
                                }
                            }
                            else
                            {
                                ShowMessage("Seller profile not found", false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error loading profile: " + ex.Message, false);
                    }
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string username = txtUsername.Text.Trim();
            string number = txtNumber.Text.Trim();

            // Validate username (must be exactly 9 digits)
            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^\d{9}$"))
            {
                lblMessage.Text = "Username must be exactly 9 digits.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Validate phone number (E.164 format: 8–15 digits, optional +, no spaces)
            if (!System.Text.RegularExpressions.Regex.IsMatch(number, @"^\+?\d{8,15}$"))
            {
                lblMessage.Text = "Enter a valid phone number with digits only (optional + at the start, no spaces).";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }
            int sellerID = Convert.ToInt32(Session["SellerID"]);
            string newImageName = null;

            // Handle image upload
            if (fuProfileImage.HasFile)
            {
                string ext = Path.GetExtension(fuProfileImage.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    ShowMessage("Only JPG, JPEG, or PNG images are allowed", false);
                    return;
                }

                // Create directory if it doesn't exist
                string uploadDir = Server.MapPath("~/UploadedImages/");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                // Generate unique filename and save
                newImageName = Guid.NewGuid().ToString() + ext;
                string path = Path.Combine(uploadDir, newImageName);
                fuProfileImage.SaveAs(path);
            }

            // Update profile in database
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string query = @"UPDATE Seller SET 
                               sellerUsername = @Username,
                               sellerName = @Name,
                               sellerSurname = @Surname,
                               sellerEmail = @Email,
                               sellerNumber = @Number,
                               sellerAddress = @Address" +
                               (newImageName != null ? ", sellerProfileImage = @Image" : "") +
                               " WHERE sellerID = @ID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Surname", txtSurname.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Number", txtNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID", sellerID);


                    if (newImageName != null)
                    {
                        cmd.Parameters.AddWithValue("@Image", newImageName);
                    }

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Profile updated successfully!", true);
                            LoadSellerProfile(); // Refresh the displayed data
                        }
                        else
                        {
                            ShowMessage("No changes were made to your profile", false);
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        if (sqlEx.Number == 2627) // Unique constraint violation
                        {
                            ShowMessage("Username or email already exists. Please choose different ones.", false);
                        }
                        else
                        {
                            ShowMessage("Database error updating profile: " + sqlEx.Message, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error updating profile: " + ex.Message, false);
                    }
                }
            }
        }

        protected void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            int sellerID = Convert.ToInt32(Session["SellerID"]);

            // First check if seller has any active listings or sales
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string checkQuery = @"SELECT COUNT(*) FROM Book WHERE sellerID = @SellerID;
                                   SELECT COUNT(*) FROM Sale s 
                                   JOIN Book b ON s.bookISBN = b.bookISBN 
                                   WHERE b.sellerID = @SellerID";

                using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                {
                    cmd.Parameters.AddWithValue("@SellerID", sellerID);

                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            int activeListings = reader.GetInt32(0);

                            reader.NextResult();
                            reader.Read();
                            int salesCount = reader.GetInt32(0);

                            if (activeListings > 0 || salesCount > 0)
                            {
                                ShowMessage("Cannot delete account with active listings or sales history. Please contact support.", false);
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error checking account status: " + ex.Message, false);
                        return;
                    }
                }

                // Proceed with deletion if no active listings/sales
                string deleteQuery = "DELETE FROM Seller WHERE sellerID = @SellerID";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@SellerID", sellerID);

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Logout and redirect
                            Session.Clear();
                            Response.Redirect("~/Default.aspx");
                        }
                        else
                        {
                            ShowMessage("Failed to delete account. Please try again.", false);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error deleting account: " + ex.Message, false);
                    }
                }
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        protected void cvUsername_ServerValidate(object source, ServerValidateEventArgs args)
        {
            

            // Check if username already exists (excluding current user)
            int sellerID = Convert.ToInt32(Session["SellerID"]);

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM Seller WHERE sellerUsername = @Username AND sellerID != @ID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username",txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID", sellerID);

                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    args.IsValid = count == 0;
                    rfvUsername.ErrorMessage = "Username already taken";
                }
            }
        }
    }
}

