using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class SearchTextBook : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                string query = Request.QueryString["query"];
                string isbn = Request.QueryString["isbn"];
                if (!string.IsNullOrEmpty(query))
                {
                    txtSearch.Text = query;
                    PerformSearch(query);
                }
                LoadCategories();
                LoadOutNowTextbooks();
                LoadRecentlyAddedTextbooks();
            }
        }

        protected void LoadCategories()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT categoryID, categoryName FROM Category ORDER BY categoryName", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                int splitIndex = dt.Rows.Count / 2;

                DataTable dt1 = dt.Clone();
                DataTable dt2 = dt.Clone();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i < splitIndex)
                        dt1.ImportRow(dt.Rows[i]);
                    else
                        dt2.ImportRow(dt.Rows[i]);
                }

                rptCategory1.DataSource = dt1;
                rptCategory1.DataBind();

                rptCategory2.DataSource = dt2;
                rptCategory2.DataBind();
            }
        }

        private void LoadOutNowTextbooks()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT TOP 8 bookISBN, coverImage FROM Book", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                rptOutNow.DataSource = dt;
                rptOutNow.DataBind();
            }
        }

        private void LoadRecentlyAddedTextbooks()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT TOP 8 bookISBN, title, price, coverImage FROM Book", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                rptRecentlyAdded.DataSource = dt;
                rptRecentlyAdded.DataBind();
            }
        }
        private void PerformSearch(string searchTerm)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 2 bookISBN, title, author, coverImage, price " + "FROM Book " + " WHERE title LIKE @SearchTerm OR author LIKE @SearchTerm", con);
                cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    lblSearchResults.Text = $"Search results for \"{searchTerm}\"";
                    lblSearchResults.Visible = true;
                    lnkViewAllResults.Visible = true;
                    rptSearchResults.DataSource = dt;
                    rptSearchResults.DataBind();
                }
                else
                {
                    lblSearchResults.Text = $"No results found for \"{searchTerm}\"";
                    lblSearchResults.Visible = true;
                    lnkViewAllResults.Visible = false;
                    rptSearchResults.DataSource = null;
                    rptSearchResults.DataBind();
                }

                pnlSearchResults.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                PerformSearch(searchTerm);
            }
            else
            {
                lblSearchResults.Visible = false;
                lnkViewAllResults.Visible = false;
                rptSearchResults.Visible = false;
                pnlSearchResults.Visible = false;
            }
        }


        protected void lnkViewAllResults_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Buyer/pabiModule/SearchResult.aspx?query=" + Server.UrlEncode(txtSearch.Text.Trim()));
        }

        protected void rptCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectCategory")
            {
                int categoryId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("~/Buyer/pabiModule/SearchResult.aspx?categoryID=" + categoryId);
            }

            if (e.Item.ItemIndex == 2)
            {
                PlaceHolder phSearchBar = (PlaceHolder)e.Item.FindControl("phSearchBar");
                if (phSearchBar != null)
                {
                    phSearchBar.Visible = true;
                }
            }
        }

        protected void rptOutNow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewBook")
            {
                string bookISBN = e.CommandArgument.ToString();
                Response.Redirect("~/Buyer/pabiModule/SearchResult.aspx?isbn=" + Server.UrlEncode(bookISBN));
            }
        }

        protected void rptRecentlyAdded_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewBook")
            {
                string bookISBN = e.CommandArgument.ToString();
                Response.Redirect("~/Buyer/pabiModule/SearchResult.aspx?isbn=" + Server.UrlEncode(bookISBN));
            }
        }


    }
}
