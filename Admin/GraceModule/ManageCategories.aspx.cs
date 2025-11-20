using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class WebForm17 : System.Web.UI.Page
    {
        // Connection string from Web.config
        private readonly string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            lblFeedback.Visible = false;
            if (!IsPostBack)
            {
                LoadCategories();   // Load all categories on page load
            }
        }

        // READ – Populate GridView
        private void LoadCategories()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Category", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvCategories.DataSource = dt;
                gvCategories.DataBind();
            }
        }

        // CREATE – Add new category
        protected void btnAddCategory_Click(object sender, EventArgs e)
        {

            // Check if all validators (RequiredFieldValidator + RegexValidator) passed
            if (!Page.IsValid)
                return;
            string name = txtCategoryName.Text.Trim();
            if (CategoryExists(name))
            {
                lblFeedback.ForeColor = System.Drawing.Color.OrangeRed;
                lblFeedback.Text = $"Category '{name}' already exists.";
                lblFeedback.Visible = true;
                return;
            }


            //if (string.IsNullOrEmpty(name)) return;  // Don’t add empty strings

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Category (categoryName) VALUES (@name)", con))
            {
                cmd.Parameters.AddWithValue("@name", name);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            txtCategoryName.Text = "";  // Clear input

            lblFeedback.ForeColor = System.Drawing.Color.Lime;
            lblFeedback.Text = $"Category '{name}' added successfully.";
            lblFeedback.Visible = true;
            lblMessage.Visible = false;

            LoadCategories();  // Refresh
           
        }

        private bool CategoryExists(string name)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Category WHERE LTRIM(RTRIM(LOWER(categoryName))) = LTRIM(RTRIM(LOWER(@name)))", con))
            {
                cmd.Parameters.AddWithValue("@name", name);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        // UPDATE – Begin edit
        protected void gvCategories_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCategories.EditIndex = e.NewEditIndex;
            LoadCategories();
        }

        // UPDATE – Save changes
        protected void gvCategories_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = (int)gvCategories.DataKeys[e.RowIndex].Value;
            TextBox txtEditCategory = (TextBox)gvCategories.Rows[e.RowIndex].FindControl("txtEditCategory");
            string name = txtEditCategory.Text.Trim();
            try
            {

                            using (SqlConnection con = new SqlConnection(connStr))
                            using (SqlCommand cmd = new SqlCommand("UPDATE Category SET categoryName = @name WHERE categoryID = @id", con))
                            {
                                cmd.Parameters.AddWithValue("@name", name);
                                cmd.Parameters.AddWithValue("@id", id);
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }

                            gvCategories.EditIndex = -1;
                            LoadCategories();


                //  Success message — CSS will auto fade it after 5 seconds
                lblMessage.Text = $"Category '{name}' updated successfully.";
                lblMessage.CssClass = "fade-message"; // success green
                lblMessage.Visible = true;
                
            }
            catch (Exception ex)
            {
                //  Error message
                lblMessage.Text = "Error updating category: " + ex.Message;
                lblMessage.CssClass = "fade-message error"; // red background
                lblMessage.Visible = true;
            }
        }

        // Cancel edit
        protected void gvCategories_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCategories.EditIndex = -1;
            LoadCategories();
        }

        // DELETE – Trigger modal
        protected void gvCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {


            int id = Convert.ToInt32(gvCategories.DataKeys[e.RowIndex].Value);
            string categoryName = gvCategories.Rows[e.RowIndex].Cells[0].Text;

            Session["PendingDeleteCategoryID"] = id;
            Session["PendingDeleteCategoryName"] = categoryName;

            lblConfirmText.Text = $"Are you sure you want to delete category '{categoryName}'?";
            ShowModal();
        }

        // Modal confirm deletion
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
      
            if (Session["PendingDeleteCategoryID"] != null)
            {
                int id = Convert.ToInt32(Session["PendingDeleteCategoryID"]);

                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Category WHERE categoryID = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                lblFeedback.ForeColor = System.Drawing.Color.Lime;
                lblFeedback.Text = $"Category '{Session["PendingDeleteCategoryName"]}' deleted successfully.";
                lblFeedback.Visible = true;
                lblMessage.Visible = false;
                
            }

            Session.Remove("PendingDeleteCategoryID");
            Session.Remove("PendingDeleteCategoryName");

            lblConfirmText.Text = "";
            LoadCategories();
        }

        // Modal cancel
        protected void btnCancelDelete_Click(object sender, EventArgs e)
        {
            lblConfirmText.Text = "";
            lblFeedback.ForeColor = System.Drawing.Color.Gray;
            lblFeedback.Text = "Delete action cancelled.";
            lblFeedback.Visible = true;
            LoadCategories();
            lblFeedback.Text = "";
            lblMessage.Visible= false;
           

        }

        // Reusable method to launch modal from server
        private void ShowModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "document.querySelector('.genre-modal-overlay').style.display = 'block';", true);
        }
    }
}
