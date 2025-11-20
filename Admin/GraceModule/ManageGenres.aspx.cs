using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class WebForm8 : System.Web.UI.Page
    {
        // Connection string from Web.config
        private readonly string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGenres(); // Load all genres when the page loads
                LoadCategories(); // Load category options into the dropdown
            }
        }

        // Load all genre records from database
        private void LoadGenres()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT g.genreID, g.genreName, c.categoryName FROM Genre g LEFT JOIN Category c ON g.categoryID = c.categoryID", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvGenres.DataSource = dt;
                gvGenres.DataBind();
            }
        }

        // Load all available categories into dropdown
        private void LoadCategories()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT categoryID, categoryName FROM Category", con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlCategories.DataSource = reader;
                ddlCategories.DataTextField = "categoryName";
                ddlCategories.DataValueField = "categoryID";
                ddlCategories.DataBind();
            }

            // Add a default prompt
            ddlCategories.Items.Insert(0, new ListItem("-- Select Category --", ""));
        }

        // Add new genre (with selected category)
        protected void BtnAddGenre_Click(object sender, EventArgs e)
        {
            string name = txtGenreName.Text.Trim();
            string selectedCat = ddlCategories.SelectedValue;

            if (string.IsNullOrEmpty(name))
            {
                lblFeedback.ForeColor = System.Drawing.Color.OrangeRed;
                lblFeedback.Text = "Genre name is required.";
                lblFeedback.Visible = true;
                return;
            }

            if (!Regex.IsMatch(name, @"^[a-zA-Z\s]{1,50}$"))
            {
                lblFeedback.ForeColor = System.Drawing.Color.OrangeRed;
                lblFeedback.Text = "Genre name should only contain letters and spaces (up to 50 characters).";
                lblFeedback.Visible = true;
                return;
            }

            if (string.IsNullOrEmpty(selectedCat))
            {
                lblFeedback.ForeColor = System.Drawing.Color.OrangeRed;
                lblFeedback.Text = "Please select a category.";
                lblFeedback.Visible = true;
                return;
            }

            if (GenreExists(name))
            {
                lblFeedback.ForeColor = System.Drawing.Color.OrangeRed;
                lblFeedback.Text = $"Genre '{name}' already exists.";
                lblFeedback.Visible = true;
                return;
            }


            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(selectedCat)) return; // Don't add if empty

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Genre (genreName, categoryID) VALUES (@name, @catID)", con))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@catID", selectedCat);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            txtGenreName.Text = "";
            ddlCategories.SelectedIndex = 0; // Reset dropdown

            lblFeedback.ForeColor = System.Drawing.Color.LightGreen;
            lblFeedback.Text = $"Genre '{name}' added successfully";
            lblFeedback.Visible = true;

            LoadGenres();


        }

        private bool GenreExists(string name)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Genre WHERE genreName = @name", con))
            {
                cmd.Parameters.AddWithValue("@name", name);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Edit genre (start edit mode)
        protected void GvGenres_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvGenres.EditIndex = e.NewEditIndex;
            LoadGenres();
        }

        // Save updated genre
        protected void GvGenres_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvGenres.DataKeys[e.RowIndex].Value);
            TextBox txtName = (TextBox)gvGenres.Rows[e.RowIndex].FindControl("txtEditGenre");
            string name = txtName.Text.Trim();


            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("UPDATE Genre SET genreName = @name WHERE genreID = @id", con))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            gvGenres.EditIndex = -1;
            LoadGenres();
        }

        // Cancel edit mode
        protected void GvGenres_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvGenres.EditIndex = -1;
            LoadGenres();
        }

        // Delete genre
        protected void GvGenres_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvGenres.DataKeys[e.RowIndex].Value);
            string genreName = gvGenres.Rows[e.RowIndex].Cells[0].Text;

            Session["PendingDeleteGenreID"] = id;
            Session["PendingDeleteGenreName"] = genreName;

            lblFeedback.Text = "";
            lblConfirmText.Text = $"Are you sure you want to delete genre '{genreName}'?";
            ShowModal();
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            if (Session["PendingDeleteGenreID"] != null)
            {
                int id = Convert.ToInt32(Session["PendingDeleteGenreID"]);

                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Genre WHERE genreID = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                lblFeedback.ForeColor = System.Drawing.Color.LightGreen;
                lblFeedback.Text = $"Genre '{Session["PendingDeleteGenreName"]}' deleted successfully.";
                lblFeedback.Visible = true;
            }

            Session.Remove("PendingDeleteGenreID");
            Session.Remove("PendingDeleteGenreName");

            lblConfirmText.Text = "";
            LoadGenres();
        }

        protected void btnCancelDelete_Click(object sender, EventArgs e)
        {
            lblConfirmText.Text = "";
            lblFeedback.ForeColor = System.Drawing.Color.Gray;
            lblFeedback.Text = "Delete action cancelled.";
            lblFeedback.Visible = true;
            LoadGenres();
        }

        private void ShowModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "GenreModalScript", "document.querySelector('.genre-modal-overlay').style.display = 'block';", true);
        }
    }
}
