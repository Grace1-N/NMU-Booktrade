using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade.Admin.GraceModule
{
    public partial class WebForm14 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDeliveries();
                LoadAssignedRecords();
            }
        }
        private void LoadDeliveries()// hear we are loading deliveries that still need drivers
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {// we are loading deliveries with books that are sold but not assigned yet to the driver.  

                string query = @"
                         SELECT 
                                  s.saleID,
                                  b.title,
                                  se.sellerName,
                                  se.sellerAddress,
                                  bu.buyerName,
                                  bu.buyerAddress,
                                  d.deliveryID,
                                  d.status AS deliveryStatus,
                                  d.deliveryDate
                          FROM 
                                  Sale s
                                  INNER JOIN Book b ON s.bookISBN = b.bookISBN
                                  INNER JOIN Seller se ON s.sellerID = se.sellerID
                                  LEFT JOIN Buyer bu ON s.buyerID = bu.buyerID
                                  LEFT JOIN Delivery d ON s.saleID = d.saleID
                          WHERE b.status = 'unavailable' AND d.status =0 "; 
;


                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvDeliveries.DataSource = dt;
                    gvDeliveries.DataBind();
                    gvDeliveries.Visible = true;   // show table if there are pending deliveries
                                                   
                    


                }
                else
                {
                    gvDeliveries.Visible = false;  // hide if nothing to assign
                }
               
            }
        }




        // It's used here to load the driver list into each dropdown in the grid and going through each row.
        protected void gvDeliveries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Only work on data rows (ignore header/footer rows)
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // --- Populate DropDownList ---
                DropDownList ddl = (DropDownList)e.Row.FindControl("ddlDrivers");
                if (ddl != null)
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("SELECT driverID, driverName + ' ' + driverSurname AS FullName FROM Driver", con);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        ddl.DataSource = reader;
                        ddl.DataTextField = "FullName"; // What users see
                        ddl.DataValueField = "driverID"; // What gets saved
                        ddl.DataBind();

                        // Add placeholder item
                        ddl.Items.Insert(0, new ListItem("-- Select Driver --", ""));
                    }
                }

                // --- Set min date for delivery date TextBox ---
                TextBox txtDate = (TextBox)e.Row.FindControl("txtDeliveryDate");
                if (txtDate != null)
                {
                    txtDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
                    if (string.IsNullOrEmpty(txtDate.Text))
                        txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                }


                // --- Add data-label attributes for responsive layout ---
                for (int i = 0; i < gvDeliveries.Columns.Count; i++)
                {
                    string headerText = gvDeliveries.Columns[i].HeaderText;
                    
                    e.Row.Cells[i].Attributes["data-label"] = headerText;
                }
            }
        }





        // This handles the "Assign" button click in each row
        protected void gvDeliveries_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AssignDriver")
            {
                int rowIndex = ((GridViewRow)((Control)e.CommandSource).NamingContainer).RowIndex;
                GridViewRow row = gvDeliveries.Rows[rowIndex];

                DropDownList ddl = (DropDownList)row.FindControl("ddlDrivers");
                TextBox txtDate = (TextBox)row.FindControl("txtDeliveryDate");

                string deliveryID = e.CommandArgument.ToString();
                string driverID = ddl.SelectedValue;

                // --- VALIDATION CHECKS ---
                if (string.IsNullOrEmpty(driverID))
                {
                    // Driver not selected
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a driver before assigning.');", true);
                    return; // stop execution
                }

                DateTime deliveryDate;
                if (!DateTime.TryParse(txtDate.Text, out deliveryDate) || deliveryDate.Date < DateTime.Today)
                {
                    // Invalid or past date
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a valid delivery date (today or future).');", true);
                    return; // stop execution
                }

                // --- If valid, continue with DB update ---
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        @"UPDATE Delivery
                          SET driverID = @driverID,
                              status = 1,
                              deliveryDate = @date,
                              deliveryAddress = @address
                          WHERE deliveryID = @deliveryID", con);

                    cmd.Parameters.AddWithValue("@driverID", driverID);
                    cmd.Parameters.AddWithValue("@deliveryID", deliveryID);
                    cmd.Parameters.AddWithValue("@date", deliveryDate.Date);

                    // Get BuyerAddress from current row (already selected in LoadDeliveries)
                    string buyerAddress = ((Label)row.FindControl("lblBuyerAddress"))?.Text ?? "";
                    cmd.Parameters.AddWithValue("@address", buyerAddress);

                    cmd.ExecuteNonQuery();
                }

                LoadAssignedRecords();
                LoadDeliveries();
            }
        }


        // Load deliveries that were already assigned to drivers (history or confirmation)
        protected void LoadAssignedRecords()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                // we are only fetch deliveries where a driver has already been assigned (driverID is NOT null)
                string query = @"
                                  SELECT 
                b.title AS BookTitle, 
                se.sellerName,
                bu.buyerName,
                d.status,
                dr.driverID,   
                dr.driverName + ' ' + dr.driverSurname AS DriverName,
                d.deliveryDate
            FROM Delivery d
            LEFT JOIN Sale s ON d.saleID = s.saleID
            LEFT JOIN Book b ON s.bookISBN = b.bookISBN
            LEFT JOIN Seller se ON s.sellerID = se.sellerID
            LEFT JOIN Buyer bu ON s.buyerID = bu.buyerID
            LEFT JOIN Driver dr ON d.driverID = dr.driverID
            WHERE d.driverID IS NOT NULL AND d.status BETWEEN 1 AND 5
            ORDER BY d.deliveryDate DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvAssignedDrivers.DataSource = dt;
                    gvAssignedDrivers.DataBind();
                    gvAssignedDrivers.Visible = true; // show assigned table
                }
                else
                {
                    gvAssignedDrivers.Visible = false; // hide if no assigned deliveries
                }

            }



        }

        protected void gvAssignedDrivers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // --- Add data-label attributes for responsive layout ---
                for (int i = 0; i < gvAssignedDrivers.Columns.Count; i++)
                {
                    string headerText = gvAssignedDrivers.Columns[i].HeaderText;
                    e.Row.Cells[i].Attributes["data-label"] = headerText;
                }
            }
        }




    }
}