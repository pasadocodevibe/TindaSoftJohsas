using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;

public partial class Changepass : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    repeatedcode rp = new repeatedcode();
    public enum MessageType { Success, Error, Info, Warning };
    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //   ShowMessage("123", MessageType.Warning);
        if (!IsPostBack)
        {
            txt_oldpass.Focus();
            get_userinfo();
        }
    }
    public void get_userinfo()
    {
        if (User.Identity.Name.ToString() != null)
        {

            string usname = User.Identity.Name.ToString();

            con = new SqlConnection(con.ConnectionString);
            con.Open();

            string ct = "Select * from ref_account where usname = '" + usname + "'";
            cmd = new SqlCommand(ct);
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {

                hd_cpass.Value = rp.Decrypt(rdr["upass"].ToString());
                get_oldpass.Text = rp.Decrypt(rdr["upass"].ToString());

                hd_id.Value = rdr[0].ToString();


            }
            else
            {
                ShowMessage("Invalid account", MessageType.Success);
            }
            con.Close();
            rdr.Close();
        }
        else
        {

        }
    }

    protected void btn_changepass_Click(object sender, EventArgs e)
    {

        if (hd_id.Value != "")
        {

            using (SqlConnection cons = new SqlConnection(con.ConnectionString))
            {
                cons.Open();
                string sqlupdate = "update ref_account set upass =@password" +
                       " where uid = " + hd_id.Value + "";
                using (SqlCommand cms = new SqlCommand(sqlupdate, cons))
                {
                    cms.Parameters.AddWithValue("@password", rp.Encrypt(txt_newpass.Text));
                    cms.CommandText = sqlupdate;

                    int result = Convert.ToInt32(cms.ExecuteScalar());



                    ShowMessage("Successfully Changed password!", MessageType.Success);
                    rp.audit_trail_add(hd_id.Value, "Change password", "Change value: " + hd_cpass.Value + " to " + txt_newpass.Text, rp.get_usersetid(User.Identity.Name).ToString());


                }
            }



        }
        else
        {

            ShowMessage("Invalid user selection!", MessageType.Error);


        }
    }




}