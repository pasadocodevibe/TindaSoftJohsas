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

public partial class _Default : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr;
    repeatedcode rp = new repeatedcode();
    public string sqlstr;
    public enum MessageType { Success, Error, Info, Warning };
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
    protected void ShowMessage(string Message, MessageType type)
    {
        string safeMessage = HttpUtility.JavaScriptStringEncode(Message ?? string.Empty);
        ScriptManager.RegisterStartupScript(
            this,
            this.GetType(),
            System.Guid.NewGuid().ToString(),
            string.Format("ShowMessage('{0}','{1}');", safeMessage, type),
            true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
            con.Open();
            if (con.State == ConnectionState.Open)
            {
         //       ShowMessage(" connected", MessageType.Success);

                //Login2.UserName = "connected";
            }
            else
            {

             //   ShowMessage("not connected", MessageType.Success);
                //  Login2.UserName = "not connected";
            }
        }
    }
    protected void ValidateUser_PO(object sender, EventArgs e)
    {
        if (Login2.UserName == "" && Login2.Password == "")
        {
            Login2.FailureText = "Please input Username & Password!";

            return;
        }
        if (Login2.UserName == "")
        {
            Login2.FailureText = "Please input Username!";

            return;
        }
        if (Login2.Password == "")
        {
            Login2.FailureText = "Please input Password!";
            return;
        }

        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ref_account WHERE usname=@USNAME AND ustatus='Inactive'", conn))
            {
                cmd.Parameters.AddWithValue("@USNAME", Login2.UserName);
                Int32 counts = Convert.ToInt32(cmd.ExecuteScalar());
                if (counts > 0)
                {
                    Login2.FailureText = "Your account was deactivated!";
                    return;
                }
            }

            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ref_account WHERE usname=@USNAME AND uvoid=1", conn))
            {
                cmd.Parameters.AddWithValue("@USNAME", Login2.UserName);
                Int32 deleted = Convert.ToInt32(cmd.ExecuteScalar());
                if (deleted > 0)
                {
                    Login2.FailureText = "Your acccount already deleted!";
                    return;
                }
            }
        }
        if (Login2.UserName != "" && Login2.Password != "")
        {
            string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("Select usname,upass,uid FROM [ref_account] WHERE usname =@Username and upass =@Password and ustatus ='Active' "))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Username", Login2.UserName);
                    cmd.Parameters.AddWithValue("@Password", rp.Encrypt(Login2.Password));
                    cmd.Connection = conn;
                    conn.Open();
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        string usid = rdr[2].ToString();
                        loguser(usid);

                    }
                    else
                    {
                        if (rp.audit_trail_add("0", "Login", "Login failed user: " + Login2.UserName, "") >= 1)
                        {
                            Login2.FailureText = "Username and/or password is incorrect.";
                        }
                    }
                    conn.Close();
                }

            }
        }
        else
        {
            Login2.FailureText = "Please input username or password!";
        }

    }
    public void loguser(string userid)
    {

        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        SqlCommand cmd = new SqlCommand("UPDATE [ref_account] SET ulastlog=@D1 Where usname =@USNAME AND upass =@PSWORD", con);
        //  DateTime dtnow = DateTime.Now;
        cmd.Parameters.AddWithValue("@D1", pacificdatenow.ToString());
        cmd.Parameters.AddWithValue("@USNAME", Login2.UserName);
        cmd.Parameters.AddWithValue("@PSWORD", rp.Encrypt(Login2.Password));

        int res = cmd.ExecuteNonQuery();
        if (res > 0)
        {
            if (rp.audit_trail_add(userid, "Login", "Successfully login", rp.get_usersetid(Login2.UserName).ToString()) >= 1)
            {
                //Login2.UserName = "test";
                FormsAuthentication.RedirectFromLoginPage(Login2.UserName, Login2.RememberMeSet);
            }
            else
            {
                ShowMessage("invalid code", MessageType.Warning);
            }
        }
        else
        {

        }
        con.Close();
    }

   
}
