using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//for encryption
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Security;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using System.Net.NetworkInformation;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Web.Services.Protocols;
using System.Diagnostics;
public partial class ForgotPass : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr;
    repeatedcode rp = new repeatedcode();
    public string sqlstr;
    public enum MessageType { Success, Error, Info, Warning };

    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btn_recover_Click(object sender, EventArgs e)
    {


        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        String sql = "Select upass,uemail,ulastlog From ref_account Where usname = '" + txt_usname.Text + "' and uemail = '" + txt_email.Text + "' ";
        cmd = new SqlCommand(sql, con);
        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        if (rdr.Read() == true)
        {
            if (rdr.GetString(1).ToString() == txt_email.Text)
            {
                // your remote SMTP server IP.
                SmtpClient smtp = new SmtpClient();
                try
                {
                    string pass = rp.Decrypt(rdr.GetString(0));


                    MailMessage Msg = new MailMessage();
                    // Sender e-mail address.
                    Msg.From = new MailAddress("atodaconesoft@gmail.com");
                    // Recipient e-mail address.
                    Msg.To.Add(txt_email.Text);
                    Msg.Subject = "AtodacOne Software solution | Password Recovery!";

                    Msg.Body = "<h3>Welcome to account recovery!</h3><h3>You recently requested to recover your password for your account!</h3><br> We sent you an email to recover decrypted password. " +
                                "<br> Your username: " + txt_usname.Text + " and password: " + pass.ToString() + " " +
                                "<br> Last login attempt: " + rdr[2].ToString() + " " +
                                 "<br> <h4>System Support. Copyright © 2019 AtodacOne Software solution | TindaSoft-POS, Allright reserved.</h4>" + "";
                    Msg.IsBodyHtml = true;

                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential("atodaconesoft@gmail.com", "deadkickassone02");
                    smtp.EnableSsl = true;
                    smtp.Send(Msg);

                    //  ShowMessage("Password Successfully sent to your email!... Thank you!", MessageType.Success);
                    lbl_mgs.ForeColor = Color.Green;
                    lbl_mgs.Text = "Successfully sent, we just sent a encrypted password to " + txt_email.Text + " Check your email for a messege from System Provider account.";
                }
                catch (Exception ex)
                {
                    lbl_mgs.Text = "Internet Connection failed, retry if busy!" + ex.ToString();
                    lbl_mgs.ForeColor = Color.Orange;


                }
                finally
                {
                    smtp.Dispose();

                }

            }
            else
            {
                lbl_mgs.Text = "Invalid email address!";
                lbl_mgs.ForeColor = Color.Red;

                txt_email.Focus();
            }

        }
        else
        {
            lbl_mgs.Text = "Invalid authentication! Please check your username or email!";
            lbl_mgs.ForeColor = Color.Red;
            txt_usname.Focus();

        }
        con.Close();
    }
}