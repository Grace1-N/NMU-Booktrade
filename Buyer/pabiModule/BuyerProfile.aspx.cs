using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace NMU_BookTrade
{
    public partial class WebForm13 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBuyerProfile();
            }
        }

        protected void LoadBuyerProfile()
        {

            int buyerID = Convert.ToInt32(Session["UserID"]);

            string connectionString = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT buyerUsername, buyerName, buyerSurname, buyerEmail, buyerNumber, buyerAddress, buyerProfileImage FROM Buyer WHERE buyerID = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", buyerID);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // grabbing the information and storing it in the textfields
                    txtUsername.Text = reader["buyerUsername"].ToString();

                    txtName.Text = reader["buyerName"].ToString();
                    txtSurname.Text = reader["buyerSurname"].ToString();
                    txtEmail.Text = reader["buyerEmail"].ToString();
                    txtNumber.Text = reader["buyerNumber"].ToString();
                    txtAddress.Text = reader["buyerAddress"].ToString();

                    string imageName = reader["buyerProfileImage"].ToString();

                    imgProfile.ImageUrl = string.IsNullOrEmpty(imageName) ? "~/Images/default.png" : "~/UploadedImages/" + imageName;


                }

            }
        }



        // Now we want to update and store everything When the button update is clicked this event happens
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int buyerID = Convert.ToInt32(Session["UserID"]);
            string username = txtUsername.Text.Trim();

            string name = txtName.Text.Trim();
            string surname = txtSurname.Text.Trim();
            string email = txtEmail.Text.Trim();
            string number = txtNumber.Text.Trim();
            string address = txtAddress.Text.Trim();
            string newImageName = "";

            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^\d{9}$"))
            {
                lblMessage.Text = "Username must be exactly 9 digits.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(number, @"^\+?\d{8,15}$"))
            {
                lblMessage.Text = "Enter a valid phone number with digits only (optional + at the start, no spaces).";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Get the file extension of the uploaded file and convert it to lowercase
            if (fuProfileImage.HasFile)
            {
                string ext = Path.GetExtension(fuProfileImage.FileName).ToLower();

                //validation check to see if its jpeg
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    lblMessage.Text = "Only JPG, JPEG, or PNG images are allowed.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                // Generate a unique file name using GUID to avoid overwriting existing files
                newImageName = Guid.NewGuid().ToString() + ext;


                // Create the physical path on the server where the file will be saved
                string path = Server.MapPath("~/UploadedImages/" + newImageName);
                // file is then saved in that path
                fuProfileImage.SaveAs(path);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = string.IsNullOrEmpty(newImageName)
                    ? "UPDATE Buyer SET buyerUsername=@Username, buyerName=@Name, buyerSurname=@Surname, buyerEmail=@Email, buyerNumber=@Number, buyerAddress=@Address WHERE buyerID=@ID"
                    : "UPDATE Buyer SET buyerUsername=@Username, buyerName=@Name, buyerSurname=@Surname, buyerEmail=@Email, buyerNumber=@Number, buyerAddress=@Address, buyerProfileImage=@Image WHERE buyerID=@ID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Number", number);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@ID", buyerID);

                if (!string.IsNullOrEmpty(newImageName))
                    cmd.Parameters.AddWithValue("@Image", newImageName);

                con.Open();
                cmd.ExecuteNonQuery();

                lblMessage.Text = "Profile updated successfully!";
                lblMessage.ForeColor = System.Drawing.Color.Lime;
                LoadBuyerProfile();

            }

        }


        protected void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            string buyerID = Session["buyerID"] as string;

            if (string.IsNullOrEmpty(buyerID))
            {
                lblMessage.Text = "Session expired. Please log in again.";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ToString();
            string query = "DELETE FROM Buyer WHERE buyerID = @BuyerID";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BuyerID", buyerID);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    // Clear session and redirect
                    Session.Clear();
                    Response.Redirect("~/UserManagement/Home.aspx");
                }
                else
                {
                    lblMessage.Text = "Failed to delete account.";
                }
            }

        }
    }
}