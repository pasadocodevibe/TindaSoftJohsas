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
using System.Drawing;
public partial class UsersEntry : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    repeatedcode rp = new repeatedcode();
    public enum MessageType { Success, Error, Info, Warning };
    public int add, edit, delete, view, print;
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
    int branchid = 0;
    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
       


        add = rp.access_user(User.Identity.Name, "UserEntry", "padd");
        edit = rp.access_user(User.Identity.Name, "UserEntry", "pedit");
        delete = rp.access_user(User.Identity.Name, "UserEntry", "pdelete");
        view = rp.access_user(User.Identity.Name, "UserEntry", "pview");
        print = rp.access_user(User.Identity.Name, "UserEntry", "pprint");
        if (add == 0)
        {
            Page.Response.Redirect("Users.aspx");
        }
        if (!IsPostBack)
        {

      

            rp.dropdown_idtext(dp_aclevel, "ref_role where (lsetid =0 or lsetid = " + rp.get_usersetid(User.Identity.Name) + ") and lvoid=0 and lstatus='Active' ", "levelid", "levelname");

            if (Request.QueryString["id"] != null)
            {

                hd_id.Value = rp.Decrypt(Request.QueryString["id"].ToString());


                if (hd_id.Value != "")
                {
                    read_user();


                }
            }
        }

    }
    public void read_user()
    {
        con = new SqlConnection(con.ConnectionString);
        con.Open();

        string qry = "select *, (select levelname from  ref_role where levelid=uroleid) as [aclevel] " + 
            "from ref_account where uid =@id and uvoid =0 and usetid=@setid ";
        cmd = new SqlCommand(qry, con);
        cmd.Parameters.AddWithValue("@id", hd_id.Value);
        cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
        rdr = cmd.ExecuteReader();

        if (rdr.Read())
        {

            lbl_title.Text = "Edit User";
            txt_fname.Text = rdr["ufullname"].ToString();
       
           
            txt_email.Text = rdr["uemail"].ToString();
            
            txt_active.Text= rdr["ustatus"].ToString();
            txt_usname.Text = rdr["usname"].ToString();
            txt_psword.Text = rp.Decrypt(rdr["upass"].ToString()).ToString();
            dp_aclevel.SelectedValue = rdr["uroleid"].ToString();
            if (rdr["aclevel"].ToString() == "Administrator")
            {
                dp_aclevel.Enabled = false;
                txt_active.Enabled = false;
            }
           
            if (rdr["uimageurl"] != DBNull.Value && rdr["uimageurl"] != "")
            {
                btn_removeimg.Visible = true;
                impPrev.ImageUrl = rdr["uimageurl"].ToString();
                hd_imgurl.Value = rdr["uimageurl"].ToString();
            }
            else
            {
                hd_imgurl.Value = "";
                btn_removeimg.Visible = false; ;
                impPrev.ImageUrl = "distribution/img/avataryblank.jpg";
            }
          

            RequiredFieldValidator9.Enabled = false;
          
        }
        else
        {
            Page.Response.Redirect("Home.aspx");
        }

        con.Close();
        rdr.Close();
       


    }
    protected void btn_removeimg_Click(object sender, EventArgs e)
    {

            string path = hd_imgurl.Value;
            System.IO.File.Delete(Server.MapPath(path));
            if (hd_imgurl.Value != "")
            {
                if (update_imgurl(hd_id.Value, "") > 0)
                {
                    ShowMessage("Successfully removed image!", MessageType.Success);
                    hd_imgurl.Value = "";
                    btn_removeimg.Visible = false; ;
                    impPrev.ImageUrl = "distribution/img/avataryblank.jpg";
                    lbl_uploadstat.ForeColor = Color.Red;
                    lbl_uploadstat.Text = "";
                }

            }
               
    }
    protected void btn_add_Click(object sender, EventArgs e)
    {
        lbl_uploadstat.ForeColor = Color.Red;
        lbl_uploadstat.Text = "";

        if (hd_id.Value == "")
        {
            btn_removeimg.Visible = false;
            if (rp.identify_counter(" ref_account where usname ='" + txt_usname.Text + "' ") == 1)
            {
                ShowMessage("Username already exist! Please try another username.", MessageType.Warning);
                txt_usname.Focus();
                return;
            }

            int addtrue = register_user();

            if (addtrue > 0)
            {
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Registered user: " + txt_usname.Text, rp.get_usersetid(User.Identity.Name).ToString());
              
                    hd_id.Value = addtrue.ToString();
               int statupload = uploadimg(addtrue.ToString());
               if (statupload == 0)
               {
                  lbl_uploadstat.Text = "Upload status: failed to upload image!";
               }
               if (statupload == -1)
               {
                   lbl_uploadstat.Text = "Upload status: The file has to be less than 10000 kb!";
               }
               else if (statupload == -2)
               {
                  lbl_uploadstat.Text = "Upload status: Only JPEG files are accepted!";
               }
               else if (statupload == -3)
               {
                   lbl_uploadstat.Text = "Upload status: The file could not be uploaded. The following error occured:!";
               }
               else if (statupload == -4)
               {
                   lbl_uploadstat.Text = "";
               }
               if (statupload > 0)
               {
                   lbl_uploadstat.ForeColor = Color.Green;
                   lbl_uploadstat.Text = "Image successfully uploaded.";
               }
               ShowMessage("Successfully registered user! ", MessageType.Success);
                // branch_reset();
            }
        }
        else
        {
           
            if (rp.identify_counter(" ref_account where usname ='" + txt_usname.Text + "' and uid != " + hd_id.Value + " ") == 1)
            {
                ShowMessage("Username already exist! Please try another username.", MessageType.Warning);
                txt_usname.Focus();
                return;
            }





            if (update_user() > 0)
            {

                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Edit", "Updated user: " + txt_usname.Text, rp.get_usersetid(User.Identity.Name).ToString());
                ShowMessage("Successfully updated", MessageType.Success);


                if (fileupload.HasFile)
                {
                    int statupload = uploadimg(hd_id.Value);
                    if (statupload == 0)
                    {
                        lbl_uploadstat.Text = "Upload status: failed to upload image!";
                    }
                    if (statupload == -1)
                    {
                        lbl_uploadstat.Text = "Upload status: The file has to be less than 10000 kb!";
                    }
                    else if (statupload == -2)
                    {
                        lbl_uploadstat.Text = "Upload status: Only JPEG files are accepted!";
                    }
                    else if (statupload == -3)
                    {
                        lbl_uploadstat.Text = "Upload status: The file could not be uploaded. The following error occured:!";
                    }
                    if (statupload > 0)
                    {
                        lbl_uploadstat.ForeColor = Color.Green;
                        lbl_uploadstat.Text = "Image successfully uploaded.";
                        btn_removeimg.Visible = true;
                        impPrev.ImageUrl = rp.get_onestringvalue("select uimageurl from ref_account where uid = " + hd_id.Value + "  ");
                    }
                }

            }
            else
            {
                ShowMessage("You are not authorized!", MessageType.Warning);
            }
        }

    }
    public void branch_reset()
    {
        lbl_uploadstat.ForeColor = Color.Red;
        lbl_uploadstat.Text = "";

        rp.dropdown_idtext(dp_aclevel, "ref_role where (lsetid =0 or lsetid = " + rp.get_usersetid(User.Identity.Name) + ") and lvoid=0 and lstatus='Active' ", "levelid", "levelname");
       
        hd_id.Value = "";
      
        txt_fname.Text = "";
        txt_email.Text = "";
     
        txt_usname.Text = "";
        txt_psword.Text = "";
        txt_active.Text = "Active";
        txt_fname.Focus();
    }
    public int update_imgurl(string uid, string imgurl)
    {
        int result = 0;

        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
  

            using (SqlCommand cmd = new SqlCommand("update [ref_account] set uimageurl=@uimageurl " +
                " where uid=@uid "))
            {
                cmd.CommandType = CommandType.Text;
            
                cmd.Parameters.AddWithValue("@uimageurl", imgurl);

                cmd.Parameters.AddWithValue("@uid", uid);
                cmd.Connection = conn;
                conn.Open();
                result = cmd.ExecuteNonQuery();




                conn.Close();
            }

        }
        return result;
    }
    public int uploadimg(string ids)
    {
        int imgstat = 0;
        if (fileupload.HasFile)
        {
            try
            {
                if (fileupload.PostedFile.ContentType == "image/jpeg")
                {
                    if (fileupload.PostedFile.ContentLength < 10240000)
                    {
                        string filename = Path.GetFileName(fileupload.FileName);
                        fileupload.SaveAs(Server.MapPath("~/images/profile/") + ids + "_" + filename);
                        //   StatusLabel.Text = "Upload status: File uploaded!";
                        string urlfile = "~/images/profile/" + ids + "_" + filename;

                        imgstat = update_imgurl(ids, urlfile);
                    }
                    else
                    {
                        imgstat = -1;
                        //  ShowMessage("Upload status: The file has to be less than 100 kb!", MessageType.Warning);
                    }
                    //   StatusLabel.Text = "Upload status: The file has to be less than 100 kb!";
                }
                else
                {
                    imgstat = -2;
                    //     ShowMessage("Upload status: Only JPEG files are accepted!", MessageType.Warning);
                    // StatusLabel.Text = "Upload status: Only JPEG files are accepted!";
                }
            }
            catch (Exception ex)
            {
                imgstat = -3;
                // ShowMessage("Upload status: The file could not be uploaded. The following error occured:!", MessageType.Warning);
                // StatusLabel.Text = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
            }
        }
        else
        {
            imgstat = -4;

        }
        return imgstat;
    }
    public int register_user()
    {
        int result = 0;
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("insert into [ref_account] (usname,upass,usetid,ufullname,uemail,uroleid,uimageurl,uregisterdate,uvoid,ustatus,uentryby) " +
                " output INSERTED.uid values (@usname,@upass,@usetid,@ufullname,@uemail,@uroleid,@uimageurl,@uregisterdate,@uvoid,@ustatus,@uentryby) "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@usname", txt_usname.Text);
                cmd.Parameters.AddWithValue("@upass", rp.Encrypt(txt_psword.Text));
                cmd.Parameters.AddWithValue("@usetid", rp.get_usersetid(User.Identity.Name));
                cmd.Parameters.AddWithValue("@ufullname", txt_fname.Text);
                cmd.Parameters.AddWithValue("@uemail", txt_email.Text);
                cmd.Parameters.AddWithValue("@uroleid", dp_aclevel.SelectedValue);
                cmd.Parameters.AddWithValue("@uimageurl", "");
                cmd.Parameters.AddWithValue("@uregisterdate", pacificdatenow);
                cmd.Parameters.AddWithValue("@uvoid", "0");
                cmd.Parameters.AddWithValue("@ustatus", txt_active.Text);
                cmd.Parameters.AddWithValue("@uentryby", rp.get_userid(User.Identity.Name));
                cmd.Connection = conn;
                conn.Open();
                int modifieduser_id = (int)cmd.ExecuteScalar();
                if (modifieduser_id > 0)
                {
                    result = modifieduser_id;
                  
                }
                else
                {
                    result = 0;
                }
                con.Close();

             
                conn.Close();
            }

        }
        return result;
    }
    public int update_user()
    {
        int result1 = 0;
      
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            string updatewithpass = "";
            if (!string.IsNullOrEmpty(txt_psword.Text.Trim()))
            {
                updatewithpass = ", upass =@upass";
            }
            else
            {
                updatewithpass = "";
            }


            using (SqlCommand cmd = new SqlCommand("update [ref_account] set usname =@usname " + updatewithpass + ",ufullname=@ufullname,uemail=@uemail,uroleid=@uroleid,ustatus=@ustatus " +
                " where uid=@uid and uentryby=@entryby "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@usname", txt_usname.Text);
                if (!string.IsNullOrEmpty(txt_psword.Text.Trim()))
                {
                    cmd.Parameters.AddWithValue("@upass", rp.Encrypt(txt_psword.Text));
                }
              
                cmd.Parameters.AddWithValue("@ufullname", txt_fname.Text);
                cmd.Parameters.AddWithValue("@uemail", txt_email.Text);
                cmd.Parameters.AddWithValue("@uroleid", dp_aclevel.SelectedValue);

               
                cmd.Parameters.AddWithValue("@ustatus", txt_active.Text);
                cmd.Parameters.AddWithValue("@entryby", rp.get_userid(User.Identity.Name));
                cmd.Parameters.AddWithValue("@uid", hd_id.Value);
                cmd.Connection = conn;
                conn.Open();
                result1 = cmd.ExecuteNonQuery();
               
             


                conn.Close();
            }

        }
        return result1;
    }
   
  
   
}