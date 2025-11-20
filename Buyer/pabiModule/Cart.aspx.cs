using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class Cart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                LoadCart();
            }
        }

        private void LoadCart()
        {
            if (Session["buyerID"] == null)
            {
                lblTotal.Text = "0.00";
                rptCartItems.DataSource = null;
                rptCartItems.DataBind();
                return;
            }

            int buyerID = Convert.ToInt32(Session["buyerID"]);
            decimal total = 0;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
           SELECT 
        b.bookISBN AS bookISBN, 
        b.title AS Title, 
        b.price AS Price, 
        b.condition AS Condition, 
        ci.quantity AS Quantity, 
        b.coverImage, 
        ISNULL(s.SellerName, '') AS SellerName, 
        ISNULL(s.SellerSurname, '') AS SellerSurname
    FROM Cart c
    INNER JOIN CartItems ci ON c.cartID = ci.cartID
    LEFT JOIN Book b ON b.bookISBN = ci.bookISBN
    LEFT JOIN Seller s ON s.sellerID = b.sellerID
    WHERE c.buyerID = @buyerID", con);

                cmd.Parameters.Add("@buyerID", SqlDbType.Int).Value = buyerID;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0;
                        int qty = row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0;
                        total += price * qty;
                    }

                    rptCartItems.DataSource = dt;
                    rptCartItems.DataBind();

                    lblTotal.Text = total.ToString("0.00");
                }
                else
                {
                    // No items in the cart
                    rptCartItems.DataSource = null;
                    rptCartItems.DataBind();
                    lblTotal.Text = "0.00";
                }
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

        protected void rptCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectCategory")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int categoryId = Convert.ToInt32(args[0]);
                string categoryName = args[1];

                Response.Redirect($"~/Buyer/pabiModule/SearchResult.aspx?categoryID={categoryId}&categoryName={Server.UrlEncode(categoryName)}");
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
                    rptSearchResults.DataSource = dt;
                    rptSearchResults.DataBind();
                }
                else
                {
                    lblSearchResults.Text = $"No results found for \"{searchTerm}\"";
                    lblSearchResults.Visible = true;
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
                rptSearchResults.Visible = false;
                pnlSearchResults.Visible = false;
            }
            Response.Redirect("~/Buyer/pabiModule/SearchResult.aspx?query=" + Server.UrlEncode(searchTerm));
        }

        protected void btnPurchase_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            if (Session["buyerID"] == null)
            {
                Response.Redirect("~/UserManagement/Login.aspx");
                return;
            }

            int buyerID = Convert.ToInt32(Session["buyerID"]);

            Guid orderGroupId = Guid.NewGuid(); // one ID per purchase batch
            DateTime purchaseDate = DateTime.Now;

            List<(string ISBN, int Qty)> cartBooks = new List<(string, int)>();
            int cartID = 0;

            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                using (SqlCommand getCartCmd = new SqlCommand("SELECT cartID FROM Cart WHERE buyerID = @buyerID", con))
                {
                    getCartCmd.Parameters.AddWithValue("@buyerID", buyerID);
                    object o = getCartCmd.ExecuteScalar();
                    if (o == null)
                    {
                        lblConfirmation.Text = "⚠️ Your cart is emp.";
                        lblConfirmation.Visible = true;
                        return;
                    }
                    cartID = Convert.ToInt32(o);
                }

                using (SqlCommand getCartItemsCmd = new SqlCommand("SELECT bookISBN, quantity FROM CartItems WHERE cartID = @cartID", con))
                {
                    getCartItemsCmd.Parameters.AddWithValue("@cartID", cartID);
                    using (SqlDataReader reader = getCartItemsCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cartBooks.Add((reader["bookISBN"].ToString(), Convert.ToInt32(reader["quantity"])));
                        }
                    }
                }

                if (cartBooks.Count == 0)
                {
                    lblConfirmation.Text = " Your cart is empty.";
                    lblConfirmation.Visible = true;
                    return;
                }

                foreach (var item in cartBooks)
                {
                    string bookISBN = item.ISBN;
                    int qty = item.Qty;

                    using (SqlCommand getNextSaleIDCmd = new SqlCommand("SELECT ISNULL(MAX(saleID), 0) + 1 FROM Sale", con))
                    {
                        int nextSaleID = Convert.ToInt32(getNextSaleIDCmd.ExecuteScalar());

                        decimal price = 0;
                        int sellerID = 0;
                        using (SqlCommand getBookCmd = new SqlCommand("SELECT price, sellerID FROM Book WHERE bookISBN = @bookISBN", con))
                        {
                            getBookCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                            using (SqlDataReader bookReader = getBookCmd.ExecuteReader())
                            {
                                if (bookReader.Read())
                                {
                                    price = Convert.ToDecimal(bookReader["price"]);
                                    sellerID = Convert.ToInt32(bookReader["sellerID"]);
                                }
                            }
                        }

                        decimal totalAmount = price * qty;

                        using (SqlCommand insertSaleCmd = new SqlCommand(@"
                    INSERT INTO Sale (saleID, buyerID, sellerID, bookISBN, amount, saleDate, orderGroupId)
                    VALUES (@saleID, @buyerID, @sellerID, @bookISBN, @amount, @saleDate, @orderGroupId)", con))
                        {
                            insertSaleCmd.Parameters.AddWithValue("@saleID", nextSaleID);
                            insertSaleCmd.Parameters.AddWithValue("@buyerID", buyerID);
                            insertSaleCmd.Parameters.AddWithValue("@sellerID", sellerID);
                            insertSaleCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                            insertSaleCmd.Parameters.AddWithValue("@amount", totalAmount);
                            insertSaleCmd.Parameters.AddWithValue("@saleDate", purchaseDate);
                            insertSaleCmd.Parameters.AddWithValue("@orderGroupId", orderGroupId);
                            insertSaleCmd.ExecuteNonQuery();
                        }

                        using (SqlCommand getNextDeliveryIDCmd = new SqlCommand("SELECT ISNULL(MAX(deliveryID), 0) + 1 FROM Delivery", con))
                        {
                            int nextDeliveryID = Convert.ToInt32(getNextDeliveryIDCmd.ExecuteScalar());
                            using (SqlCommand insertDeliveryCmd = new SqlCommand(
                                "INSERT INTO Delivery (deliveryID, saleID, status) VALUES (@deliveryID, @saleID, 0)", con))
                            {
                                insertDeliveryCmd.Parameters.AddWithValue("@deliveryID", nextDeliveryID);
                                insertDeliveryCmd.Parameters.AddWithValue("@saleID", nextSaleID);
                                insertDeliveryCmd.ExecuteNonQuery();
                            }
                        }

                        using (SqlCommand updateCartItemsCmd = new SqlCommand(
                            "UPDATE CartItems SET saleID = @saleID WHERE cartID = @cartID AND bookISBN = @bookISBN", con))
                        {
                            updateCartItemsCmd.Parameters.AddWithValue("@saleID", nextSaleID);
                            updateCartItemsCmd.Parameters.AddWithValue("@cartID", cartID);
                            updateCartItemsCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                            updateCartItemsCmd.ExecuteNonQuery();
                        }

                        using (SqlCommand updateBookStatusCmd = new SqlCommand("UPDATE Book SET status = 'unavailable' WHERE bookISBN = @bookISBN", con))
                        {
                            updateBookStatusCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                            updateBookStatusCmd.ExecuteNonQuery();
                        }
                    }
                }

                using (SqlCommand clearCartCmd = new SqlCommand("DELETE FROM CartItems WHERE cartID = @cartID", con))
                {
                    clearCartCmd.Parameters.AddWithValue("@cartID", cartID);
                    clearCartCmd.ExecuteNonQuery();
                }
            }

            lblConfirmation.Text = "Your purchase was successful!";
            lblConfirmation.Visible = true;

            pnlPayment.Visible = false;
            pnlCart.Visible = false;

            lblOrderNumber.Text = orderGroupId.ToString().Substring(0, 8).ToUpper();
            lblBuyerName.Text = Session["buyerName"] != null ? Session["buyerName"].ToString() : "Valued Buyer";
            lblOrderDate.Text = purchaseDate.ToString("dd MMM yyyy");
            lblItemCount.Text = cartBooks.Sum(c => c.Qty).ToString();

            decimal orderTotal = 0;
            using (SqlConnection con2 = new SqlConnection(connStr))
            {
                con2.Open();
                foreach (var it in cartBooks)
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT price FROM Book WHERE bookISBN = @isbn", con2))
                    {
                        cmd.Parameters.AddWithValue("@isbn", it.ISBN);
                        object priceObj = cmd.ExecuteScalar();
                        if (priceObj != null && priceObj != DBNull.Value)
                        {
                            orderTotal += Convert.ToDecimal(priceObj) * it.Qty;
                        }
                    }
                }
            }

            lblOrderTotal.Text = orderTotal.ToString("0.00");
            lblEstimatedDelivery.Text = purchaseDate.AddDays(5).ToString("dd MMM yyyy");

            pnlOrderSummary.Visible = true;
            ((Site1)this.Master).UpdateCartCount();
        }

        protected void cvCard_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string cardNumber = args.Value.Replace(" ", "").Replace("-", ""); // Remove spaces or dashes
            if (!LuhnCheck(cardNumber))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        private bool LuhnCheck(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            int sum = 0;
            bool isEven = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(cardNumber[i]))
                    return false;

                int digit = cardNumber[i] - '0';

                if (isEven)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                isEven = !isEven;
            }

            return (sum % 10 == 0);
        }

        protected void btnShowPayment_Click(object sender, EventArgs e)
        {
            pnlPayment.Visible = true;
        }


        protected void btnRemove_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string bookISBN = btn.CommandArgument;
            int buyerID = Convert.ToInt32(Session["buyerID"]);

            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Get the buyer's cartID
                int cartID = 0;
                string getCartIDQuery = "SELECT cartID FROM Cart WHERE buyerID = @buyerID";
                using (SqlCommand getCartCmd = new SqlCommand(getCartIDQuery, conn))
                {
                    getCartCmd.Parameters.AddWithValue("@buyerID", buyerID);
                    object result = getCartCmd.ExecuteScalar();
                    if (result != null)
                    {
                        cartID = Convert.ToInt32(result);
                    }
                    else
                    {
                        return; // No cart found
                    }
                }

                // Delete the book completely from CartItems
                string deleteQuery = "DELETE FROM CartItems WHERE cartID = @cartID AND bookISBN = @bookISBN";
                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@cartID", cartID);
                    deleteCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                    deleteCmd.ExecuteNonQuery();
                }
            }

            // Refresh the cart repeater and cart count
            BindCartItems();
            ((Site1)this.Master).UpdateCartCount();
        }


        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddl = (DropDownList)e.Item.FindControl("ddlQuantity");

                if (ddl != null)
                {
                    ddl.Items.Clear();
                    ddl.Items.Add(new ListItem("0 (Remove)", "0"));

                    for (int i = 1; i <= 1; i++)
                    {
                        ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    DataRowView drv = (DataRowView)e.Item.DataItem;
                    string qty = drv["Quantity"].ToString();

                    if (ddl.Items.FindByValue(qty) != null)
                    {
                        ddl.SelectedValue = qty;
                    }
                }
            }
        }


        protected void ddlQuantity_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;

            HiddenField hfBookISBN = (HiddenField)item.FindControl("hfBookISBN");
            string bookISBN = hfBookISBN.Value;
            int newQuantity = Convert.ToInt32(ddl.SelectedValue);
            int buyerID = Convert.ToInt32(Session["buyerID"]);

            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                int cartID = 0;
                string getCartIDQuery = "SELECT cartID FROM Cart WHERE buyerID = @buyerID";
                using (SqlCommand getCartCmd = new SqlCommand(getCartIDQuery, conn))
                {
                    getCartCmd.Parameters.AddWithValue("@buyerID", buyerID);
                    object result = getCartCmd.ExecuteScalar();
                    if (result == null) return;
                    cartID = Convert.ToInt32(result);
                }

                if (newQuantity == 0)
                {
                    // Remove row
                    string deleteQuery = "DELETE FROM CartItems WHERE cartID = @cartID AND bookISBN = @bookISBN";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@cartID", cartID);
                        deleteCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                        deleteCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Update quantity
                    string updateQuery = "UPDATE CartItems SET quantity = @qty WHERE cartID = @cartID AND bookISBN = @bookISBN";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@qty", newQuantity);
                        updateCmd.Parameters.AddWithValue("@cartID", cartID);
                        updateCmd.Parameters.AddWithValue("@bookISBN", bookISBN);
                        updateCmd.ExecuteNonQuery();
                    }
                }
            }

            // Refresh cart display
            LoadCart();
            ((Site1)this.Master).UpdateCartCount();
        }

        private void BindCartItems()
        {
            int buyerID = Convert.ToInt32(Session["buyerID"]);
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = @"SELECT ci.bookISBN, ci.quantity, b.title, b.price, b.coverImage, b.condition, 
                         s.sellerName, s.sellerSurname
                         FROM CartItems ci
                         INNER JOIN Book b ON ci.bookISBN = b.bookISBN
                         INNER JOIN Seller s ON b.sellerID = s.sellerID
                         INNER JOIN Cart c ON ci.cartID = c.cartID
                         WHERE c.buyerID = @buyerID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@buyerID", buyerID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptCartItems.DataSource = dt;
                    rptCartItems.DataBind();

                    // Update total
                    decimal total = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        total += Convert.ToDecimal(row["price"]) * Convert.ToInt32(row["quantity"]);
                    }
                    lblTotal.Text = total.ToString("0.00");
                }
            }
        }
    }
}