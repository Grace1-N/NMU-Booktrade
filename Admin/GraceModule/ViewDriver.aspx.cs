using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.Admin.GraceModule
{
    public partial class ViewDriver : System.Web.UI.Page
    {
        string driverID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            driverID = Request.QueryString["id"];

            if (!IsPostBack && !string.IsNullOrEmpty(driverID))
            {
                LoadDriverDetails(driverID);
            }
        }

        private void LoadDriverDetails(string id)
        {
            // Connect to the database using connection string from Web.config
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                con.Open();

                // Prepare SQL query to get driver by ID
                SqlCommand cmd = new SqlCommand("SELECT driverName, driverSurname, driverEmail, driverPhoneNumber, driverProfileImage FROM Driver WHERE driverID = @ID", con);
                cmd.Parameters.AddWithValue("@ID", id);

                // Execute the query and read the results
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    // Set driver details into labels
                    lblName.Text = rdr["driverName"].ToString();
                    lblSurname.Text = rdr["driverSurname"].ToString();
                    lblEmail.Text = rdr["driverEmail"].ToString();
                    lblPhone.Text = rdr["driverPhoneNumber"].ToString();

                    // Get and display the driver's image
                    string imgPath = rdr["driverProfileImage"].ToString();
                    imgProfile.ImageUrl = "~/UploadedImages/" + imgPath;
                }
            }
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Driver WHERE driverID = @ID", con);
                cmd.Parameters.AddWithValue("@ID", driverID);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    lblMessage.Text = "Driver deleted successfully.";
                    lblMessage.ForeColor = System.Drawing.Color.Green;

                    Response.Redirect("AdminDashboard.aspx");
                }
                else
                {
                    lblMessage.Text = "Error deleting driver.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
    }   }
}