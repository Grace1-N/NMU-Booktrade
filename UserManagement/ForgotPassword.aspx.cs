using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.UserManagement
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Step 1: Read the input
            string input = txtEmailOrUsername.Text.Trim();

            // Step 2: Set connection string
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ToString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Step 3: Check in each user table (Buyer, Seller, etc.)
                string[] tables = { "Buyer", "Seller", "Driver", "Admin" };
                foreach (string table in tables)
                {
                    string query = $"SELECT * FROM {table} WHERE {table.ToLower()}Email = @Input OR {table.ToLower()}Username = @Input";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Input", input);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Step 4: If found, store info in session and redirect
                        Session["ResetUserID"] = reader[$"{table.ToLower()}ID"].ToString(); // we are getting the primary key column and the table name as well as the actual ID logged in that table using reader[...].
                        Session["ResetRole"] = table; //storing the table into the session
                        reader.Close();

                        Response.Redirect("~/UserManagement/ResetPassword.aspx"); // redirect to reset page
                        return;
                    }

                    reader.Close(); // must close before trying another table
                }

                // Step 5: If no match found
                lblMessage.Text = "Account not found. Try again.";
            }
        }

        protected void btnClear3_Click(object sender, EventArgs e)
        {
            txtEmailOrUsername.Text = "";
        }    
    }
}