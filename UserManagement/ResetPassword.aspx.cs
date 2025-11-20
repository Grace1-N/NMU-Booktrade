using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.UserManagement
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            // Step 1: Get values
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (newPassword != confirmPassword)
            {
                lblMessage.Text = "Passwords do not match.";
                return;
            }


           
            // Step 2: Retrieve session values
            string userId = Session["ResetUserID"] as string;
            string role = Session["ResetRole"] as string;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            {
                lblMessage.Text = "Session expired or invalid access.";
                return;
            }

            
             newPassword = HashPassword(newPassword);

            // Step 3: Build update query
            string query = $"UPDATE {role} SET {role.ToLower()}Password = @Password WHERE {role.ToLower()}ID = @ID";

            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ToString();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Password", newPassword);
                cmd.Parameters.AddWithValue("@ID", userId);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    lblMessage.ForeColor = System.Drawing.Color.GhostWhite;
                    lblMessage.Text = "Password successfully reset. You may now log in.";
                }
                else
                {
                    lblMessage.Text = "Failed to reset password. Try again.";
                }
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

        protected void btnClear4_Click(object sender, EventArgs e)
        {
            txtNewPassword.Text= " ";
            txtConfirmPassword.Text= "";
            
        }

    }
}