using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace NMU_BookTrade.Driver.ClintonModule
{
    public partial class DriverProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null || Session["AccessID"] == null || Session["AccessID"].ToString() != "4")
                {
                    Response.Redirect("~/UserManagement/Login.aspx");
                }
                else
                {
                    LoadDriverProfile();
                }
            }
        }

        private void LoadDriverProfile()
        {
            int driverID = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string query = @"SELECT driverUsername, driverName, driverSurname, driverEmail, 
                               driverPhoneNumber, driverProfileImage 
                               FROM Driver WHERE driverID = @ID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", driverID);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtUsername.Text = reader["driverUsername"].ToString();
                        txtName.Text = reader["driverName"].ToString();
                        txtSurname.Text = reader["driverSurname"].ToString();
                        txtEmail.Text = reader["driverEmail"].ToString();
                        txtNumber.Text = reader["driverPhoneNumber"].ToString();

                        string imageName = reader["driverProfileImage"].ToString();
                        imgProfile.ImageUrl = string.IsNullOrEmpty(imageName)
                            ? "~/Images/default.png"
                            : "~/UploadedImages/" + imageName;
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Error loading profile: " + ex.Message);
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {


            if (!Page.IsValid) return;

            int driverID = Convert.ToInt32(Session["UserID"]);
            string username = txtUsername.Text.Trim();
            string name = txtName.Text.Trim();
            string surname = txtSurname.Text.Trim();
            string email = txtEmail.Text.Trim();
            string number = txtNumber.Text.Trim();
            string newImageName = null;

            // Validate username (must be exactly 9 digits)
            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^\d{10}$"))
            {
                lblMessage.Text = "Username must be exactly 9 digits.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Handle image upload
            if (fuProfileImage.HasFile)
            {
                string ext = Path.GetExtension(fuProfileImage.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    lblImageError.Text = "Only JPG, JPEG, or PNG images are allowed.";
                    lblImageError.Visible = true;
                    return;
                }

                // Delete old image if exists
                DeleteOldProfileImage(driverID);

                // Save new image
                newImageName = Guid.NewGuid().ToString() + ext;
                string path = Server.MapPath("~/UploadedImages/" + newImageName);
                fuProfileImage.SaveAs(path);
            }

            // Update profile in database
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string query = newImageName != null
                    ? @"UPDATE Driver SET driverUsername=@Username, driverName=@Name, 
                      driverSurname=@Surname, driverEmail=@Email, driverPhoneNumber=@Number, 
                      driverProfileImage=@Image WHERE driverID=@ID"
                    : @"UPDATE Driver SET driverUsername=@Username, driverName=@Name, 
                      driverSurname=@Surname, driverEmail=@Email, driverPhoneNumber=@Number 
                      WHERE driverID=@ID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Number", number);
                cmd.Parameters.AddWithValue("@ID", driverID);

                if (newImageName != null)
                    cmd.Parameters.AddWithValue("@Image", newImageName);

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowSuccessMessage("Profile updated successfully!");
                        LoadDriverProfile(); // Refresh the displayed data
                    }
                    else
                    {
                        ShowErrorMessage("No changes were made to your profile.");
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique key violation
                    {
                        ShowErrorMessage("Username already exists. Please choose a different one.");
                    }
                    else
                    {
                        ShowErrorMessage("Error updating profile: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Error updating profile: " + ex.Message);
                }
            }
        }

        protected void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            int driverID = Convert.ToInt32(Session["UserID"]);

            // Delete profile image first
            DeleteOldProfileImage(driverID);

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string query = "DELETE FROM Driver WHERE driverID = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", driverID);

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Session.Clear();
                        Response.Redirect("~/UserManagement/Login.aspx");
                    }
                    else
                    {
                        ShowErrorMessage("Failed to delete profile.");
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Error deleting profile: " + ex.Message);
                }
            }
        }

        private void DeleteOldProfileImage(int driverID)
        {
            // Get current image name
            string oldImageName = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                string query = "SELECT driverProfileImage FROM Driver WHERE driverID = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", driverID);

                con.Open();
                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    oldImageName = result.ToString();
                }
            }

            // Delete the old image file if it exists
            if (!string.IsNullOrEmpty(oldImageName))
            {
                string oldImagePath = Server.MapPath("~/UploadedImages/" + oldImageName);
                if (File.Exists(oldImagePath))
                {
                    try
                    {
                        File.Delete(oldImagePath);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't prevent the update
                        System.Diagnostics.Debug.WriteLine("Error deleting old image: " + ex.Message);
                    }
                }
            }
        }

        private void ShowSuccessMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Visible = true;
        }

        private void ShowErrorMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Visible = true;
        }
    }
}