using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NMU_BookTrade
{
    public partial class WebForm9 : System.Web.UI.Page
    { 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                
                LoadSentMessages(); 
                LoadMessages();
                GetMessageCount();
                
            }
        }

        private void LoadMessages()
        {
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            string query = "SELECT messageID, dateSent, messageContent, senderEmail, isRead " +
                           "FROM SupportMessages WHERE senderEmail IS NOT NULL AND senderEmail != @adminEmail " +
                           "ORDER BY dateSent DESC";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@adminEmail", "gracamanyonganise@gmail.com");
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                rptInbox.DataSource = reader;
                rptInbox.DataBind();
            }
        }

       

        private void LoadSentMessages()
        {
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            string query = "SELECT messageID, dateSent, messageContent, senderEmail " +
                            "FROM SupportMessages WHERE senderEmail = @senderEmail " +
                            "ORDER BY dateSent DESC";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@senderEmail", "gracamanyonganise@gmail.com");
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                rptSent.DataSource = reader;
                rptSent.DataBind();
            }
             
        }


        protected void rptInbox_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteMessage")
            {
                int messageID = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM SupportMessages WHERE messageID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", messageID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadMessages(); // refresh after delete
            }
        }

        protected void rptSent_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteMessage")
            {
                int messageID = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM SupportMessages WHERE messageID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", messageID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadSentMessages(); // refresh after delete
            }
        }

        [System.Web.Services.WebMethod]
        public static string DeleteMessage(int messageID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Messages WHERE messageID=@id", con);
                    cmd.Parameters.AddWithValue("@id", messageID);
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0 ? "Message deleted successfully." : "Message not found.";
                }
            }
            catch (Exception ex)
            {
                return "Error deleting: " + ex.Message;
            }
        }



        private int GetMessageCount()
        {
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            string query = "SELECT COUNT(*) FROM SupportMessages WHERE isRead = 0";

            using (SqlConnection conn = new SqlConnection(connStr))
            {

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    int messageCount = (int)cmd.ExecuteScalar(); 
                    inboxCountSpan.InnerText = messageCount.ToString();
                    return messageCount;
                }

            }

        }

        


        [WebMethod]
        public static string MarkAsRead(int[] ids)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    foreach (int id in ids)
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE SupportMessages SET isRead = 1 WHERE messageID = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return "Messages marked as read.";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }


        [WebMethod]
        public static string MarkMessageAsRead(int messageID)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("UPDATE SupportMessages SET isRead = 1 WHERE messageID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", messageID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                                      
                    return "Marked as read";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [WebMethod]
        public static int GetUnreadMessageCount()
        {
            string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
            string query = "SELECT COUNT(*) FROM SupportMessages WHERE isRead = 0";

            using (SqlConnection conn = new SqlConnection(connStr))
            {

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    int messageCount = (int)cmd.ExecuteScalar();
                    return messageCount;
                }

            }
        }


        [WebMethod(EnableSession = true)]
        public static string SendMessage(string to, string subject, string body)
        {
 
            try
            {
                string senderEmail = "gracamanyonganise@gmail.com"; // Hardcoded for now 
                string connStr = ConfigurationManager.ConnectionStrings["NMUBookTradeConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("INSERT INTO SupportMessages (messageContent, senderEmail, dateSent, isRead) VALUES (@content, @senderEmail, @date, 1)", conn))
                {
                    string messageContent = $"To: {to}\nSubject: {subject}\n{body}";
                    cmd.Parameters.AddWithValue("@content", messageContent);
                    cmd.Parameters.AddWithValue("@senderEmail", senderEmail);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    conn.Open();
                    cmd.ExecuteNonQuery(); 


                    return "Message Sent";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [WebMethod(EnableSession = true)]
        public static string SendEmail(string to, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("gracamanyonganise@gmail.com");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("gracamanyonganise@gmail.com", "swywlbrcvkkhisdh");
                smtp.EnableSsl = true;

                smtp.Send(mail);
                return "Message sent successfully!";
            }
            catch (Exception ex)
            {
                return "Error sending message: " + ex.Message;
            }
        }
    }
}
