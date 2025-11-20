using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.Admin.GraceModule
{
    public partial class DeleteDriver : System.Web.UI.Page
    {
        protected void btnAddDriver_Click(object sender, EventArgs e)
        {
            
            // Validate the page to make sure all fields are filled
            if (!Page.IsValid)
            {
                return; // Stop if validation fails
            }

            // Collect input from the form
            string name = txtName.Text.Trim();
            string surname = txtSurname.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim(); // Consider hashing password in production
            string hashedPassword = HashPassword(password);
            string fileName = "";

            // Handle image upload
            if (fuImage.HasFile)
            {
                try
                {
                    fileName = Path.GetFileName(fuImage.FileName);
                    string filePath = Server.MapPath("~/UploadedImages/" + fileName);
                    fuImage.SaveAs(filePath);
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Image upload failed: " + ex.Message;
                    return;
                }
            }

            // Insert data into the database
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ToString();

            // Check duplicate username or email
            if (DriverExists(username, email))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "A driver with this username or email already exists.";
                return;
            }
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "INSERT INTO Driver (driverName, driverSurname, driverEmail, driverPhoneNumber, driverUsername, driverPassword, driverProfileImage, accessID) " +
               "VALUES (@Name, @Surname, @Email, @Phone, @Username, @Password, @Image, @AccessID)";


                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Image", fileName);
                cmd.Parameters.AddWithValue("@AccessID", 4);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                conn.Close();

                if (rows > 0)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = "Driver added successfully.";
                }
                else
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Failed to add driver.";
                }
            }
        }

        private bool DriverExists(string username, string email)
        {
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ToString();
            string query = "SELECT 1 FROM Driver WHERE driverUsername = @Username OR driverEmail = @Email";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null;
            }
        }

        //   This function hashes a plain-text password using SHA256 encryption
        public string HashPassword(string password)
        {
            // Create a SHA256 object that will handle the hashing
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string(password) into a byte array using UTF-8 encoding
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Create a StringBuilder to build the hashed string
                StringBuilder builder = new StringBuilder();

                // Loop through each byte in the byte array
                foreach (byte b in bytes)
                {
                    //  Convert each byte to a hexadecimal string(2 characters) and append to the builder
                    builder.Append(b.ToString("x2"));
                }

                // Return the final hashed string(e.g., "a3c5b4d6...")
                return builder.ToString();
            }
        }


    }
}