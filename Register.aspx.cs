using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

public partial class Register : System.Web.UI.Page
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
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btn_register_Click(object sender, EventArgs e)
    {
       

        if(rp.identify_counter("ref_account where usname ='" + txt_usname.Text + "' ") > 0)
        {
            ShowMessage("Username is in used, please try another username!", MessageType.Warning);
            txt_usname.Text = "";
            txt_usname.Focus();
            return;
        }
        if (rp.identify_counter("ref_account where uemail ='" + txt_email.Text + "' ") > 0)
        {
            ShowMessage("Email address already used, please try another email address!", MessageType.Warning);
            txt_email.Text = "";
            txt_email.Focus();
            return;
        }
        int res = register_generalsettings();
        if (res > 0)
        {
            rp.audit_trail_add("0", "Register", "General Settings, store name : " + txt_storename.Text, res.ToString());

            int register =   register_user(res.ToString());
            if (register > 0)
            {
                rp.audit_trail_add("0", "Register", "User Account, fullname: " + txt_fname.Text + ", "  +  txt_lname.Text + " , username: " + txt_usname.Text , res.ToString());
                ShowMessage("Successfully registered", MessageType.Success);
               
            }
        }
        else
        {

        }
    }
    public int register_generalsettings()
    {
        int result =0;
         string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
         using (SqlConnection conn = new SqlConnection(constr))
         {
             using (SqlCommand cmd = new SqlCommand("insert into [ref_generalsettings] (setcompanyname,setaddress, setcontact, setlogo,setmail,setcode,setcreated,setvoid,setstatus,setnote,setstyle) " +
                 " output INSERTED.setid values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11) "))
             {
                 cmd.CommandType = CommandType.Text;
                 cmd.Parameters.AddWithValue("@d1", txt_storename.Text);
                 cmd.Parameters.AddWithValue("@d2", txt_storeaddress.Text);
                 cmd.Parameters.AddWithValue("@d3", txt_contact.Text);
                 cmd.Parameters.AddWithValue("@d4", "");
                 cmd.Parameters.AddWithValue("@d5", txt_email.Text);
                 cmd.Parameters.AddWithValue("@d6", "");
                 cmd.Parameters.AddWithValue("@d7", pacificdatenow);
                 cmd.Parameters.AddWithValue("@d8", "0");
                 cmd.Parameters.AddWithValue("@d9", "Active");
                 cmd.Parameters.AddWithValue("@d10", txt_retailer.Text);
                 cmd.Parameters.AddWithValue("@d11", "distribution/css/style.default.css");
                 cmd.Connection = conn;
                 conn.Open();
                 result = (int)cmd.ExecuteScalar();


                 conn.Close();
             }
         }



         return result;
    }
    public int register_user(string setid)
    {
        int result = 0;
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("insert into [ref_account] (usname,upass,usetid,ufullname,uemail,uroleid,uimageurl,uregisterdate,uvoid,ustatus,uentryby) " +
                "values (@usname,@upass,@usetid,@ufullname,@uemail,@uroleid,@uimageurl,@uregisterdate,@uvoid,@ustatus,@uentryby) "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@usname", txt_usname.Text);
                cmd.Parameters.AddWithValue("@upass", rp.Encrypt(txt_password.Text));
                cmd.Parameters.AddWithValue("@usetid", setid.ToString());
                cmd.Parameters.AddWithValue("@ufullname", txt_fname.Text + ", " + txt_lname.Text);
                cmd.Parameters.AddWithValue("@uemail", txt_email.Text);
                cmd.Parameters.AddWithValue("@uroleid", "1");
                   cmd.Parameters.AddWithValue("@uimageurl", "");
                   cmd.Parameters.AddWithValue("@uregisterdate", pacificdatenow );
                   cmd.Parameters.AddWithValue("@uvoid", "0");
                   cmd.Parameters.AddWithValue("@ustatus","Active");
                   cmd.Parameters.AddWithValue("@uentryby", "0");
                cmd.Connection = conn;
                conn.Open();
                result = cmd.ExecuteNonQuery();
             
                    result = 1;

              
                conn.Close();
            }

        }
        return result;
    }
}