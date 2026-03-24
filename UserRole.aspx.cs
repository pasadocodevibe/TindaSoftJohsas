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

public partial class UserRole : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    int branchid = 0;
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
    repeatedcode rp = new repeatedcode();
    public int add, edit, delete, view, print;
    public enum MessageType { Success, Error, Info, Warning };
    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //     branchid = rp.get_userbranchid(User.Identity.Name);
        add = rp.access_user(User.Identity.Name, "UserEntry", "padd");
        edit = rp.access_user(User.Identity.Name, "UserEntry", "pedit");
        delete = rp.access_user(User.Identity.Name, "UserEntry", "pdelete");
        view = rp.access_user(User.Identity.Name, "UserEntry", "pview");
        print = rp.access_user(User.Identity.Name, "UserEntry", "pprint");

        if (view == 0)
        {
            Page.Response.Redirect("Home.aspx");
        }
        if (!IsPostBack)
        {
            if (add == 1)
            {
                btn_add.Enabled = true;
                btn_add.Visible = true;
            }
            else
            {
                btn_add.Enabled = false;
                btn_add.Visible = false;
            }

          
                role_table("1");
         
           branch_table();

        }
    }
    private void role_table(string id)
    {

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {

            string sql = "select * FROM ref_rolepermission where proleid= '" + id + "' order by mainmenu asc";
          
            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.CommandText = sql;
            cmd.Connection = con;
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);
                gv_masterlist.DataSource = dt;
                gv_masterlist.DataBind();
            }
        }

        //}
        //catch (Exception ex)
        //{

        //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);

        //        }
    }
     protected void gv_masterlist_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // {LinkButton lnk2 = (LinkButton)e.Row.FindControl("LinkButton2");
         


        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {

        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lnk2 = (LinkButton)e.Row.FindControl("btn_selectbranch");
            LinkButton lnk3 = (LinkButton)e.Row.FindControl("btn_deletebranch");
            CheckBox checkadd = (CheckBox)e.Row.FindControl("checkadd");
            CheckBox checkedit = (CheckBox)e.Row.FindControl("checkedit");
            CheckBox checkdelete = (CheckBox)e.Row.FindControl("checkdelete");
            CheckBox checkview = (CheckBox)e.Row.FindControl("checkview");

            HiddenField hd_permissionid = (HiddenField)e.Row.FindControl("hd_permissionid");
            HiddenField hd_roleid = (HiddenField)e.Row.FindControl("hd_roleid");
            HiddenField hd_add = (HiddenField)e.Row.FindControl("hd_add");
            HiddenField hd_edit = (HiddenField)e.Row.FindControl("hd_edit");
            HiddenField hd_delete = (HiddenField)e.Row.FindControl("hd_delete");
            HiddenField hd_view = (HiddenField)e.Row.FindControl("hd_view");

          

                checkuncheck(hd_add, checkadd);
                checkuncheck(hd_edit, checkedit);
                checkuncheck(hd_delete, checkdelete);
                checkuncheck(hd_view, checkview);

                


        }
    }
     public void checkuncheck(HiddenField hd_add, CheckBox checkadd)
     {
         if (hd_add.Value == "1")
         {
             checkadd.Checked = true;

         }
         else
         {
             checkadd.Checked = false;

         }
     }
     public void reset()
     {
         branch_table();
         hd_id.Value = "";
         role_table("1");
         txt_active.Text = "Active";
         txt_rolename.Text = "";
         txt_rolename.Focus();
     }
     protected void btn_add_Click(object sender, EventArgs e)
     {
         if (hd_id.Value == "")
         {
             if (rp.identify_counter(" ref_role where levelname ='" + txt_rolename.Text + "' and lsetid= " + rp.get_usersetid(User.Identity.Name) + " and lvoid=0 ") == 1)
             {
                 ShowMessage("Role name already exist!", MessageType.Warning);
                 txt_rolename.Focus();
                 return;
             }

             int addrolestat = add_role();

             if (addrolestat > 0)
             {
                 int stataddedpermission = 0;
                 foreach (GridViewRow row in gv_masterlist.Rows)
                 {

                     HiddenField psitename = (HiddenField)row.FindControl("hd_sitename");
                     HiddenField hd_menu = (HiddenField)row.FindControl("hd_menu");
                     CheckBox checkadd = (CheckBox)row.FindControl("checkadd");
                     CheckBox checkedit = (CheckBox)row.FindControl("checkedit");
                     CheckBox checkdelete = (CheckBox)row.FindControl("checkdelete");
                     CheckBox checkview = (CheckBox)row.FindControl("checkview");
                     stataddedpermission += rolepermission_add(hd_menu.Value, psitename.Value, addrolestat.ToString(), checkadd, checkedit, checkdelete, checkview);

                 }
                 if (stataddedpermission > 0)
                 {
                     ShowMessage("Successfully added Role & Permission! ", MessageType.Success);
                     reset();
                 }
                 else
                 {
                     ShowMessage("Failed to add permission! ", MessageType.Success);
                 }


             }
             else
             {
                 ShowMessage("Failed to add role! ", MessageType.Success);
             }
         }
         else
         {
             // update


             if (rp.identify_counter(" ref_role where levelid != " + hd_id.Value + " and levelname ='" + txt_rolename.Text + "' and lsetid= " + rp.get_usersetid(User.Identity.Name) + " and lvoid=0 ") == 1)
             {
                 ShowMessage("Role name already exist!", MessageType.Warning);
                 txt_rolename.Focus();
                 return;
             }

             int updaterolestat = update_role();

             if (updaterolestat > 0)
             {
                 int statupdatepermission = 0;
                 foreach (GridViewRow row in gv_masterlist.Rows)
                 {
                      HiddenField hd_permissionid = (HiddenField)row.FindControl("hd_permissionid");
                     HiddenField psitename = (HiddenField)row.FindControl("hd_sitename");
                     HiddenField hd_menu = (HiddenField)row.FindControl("hd_menu");
                     CheckBox checkadd = (CheckBox)row.FindControl("checkadd");
                     CheckBox checkedit = (CheckBox)row.FindControl("checkedit");
                     CheckBox checkdelete = (CheckBox)row.FindControl("checkdelete");
                     CheckBox checkview = (CheckBox)row.FindControl("checkview");
                     statupdatepermission += rolepermission_update(hd_permissionid.Value, checkadd, checkedit, checkdelete, checkview);

                 }
                 if (statupdatepermission > 0)
                 {
                     ShowMessage("Successfully updated Role & Permission! ", MessageType.Success);
                     reset();
                 }
                 else
                 {
                     ShowMessage("Failed to update permission! ", MessageType.Success);
                 }


             }
             else
             {
                 ShowMessage("Failed to update role! ", MessageType.Success);
             }
         }
     }
     public int add_role()
     {
         int stat = 0;
         if (con.State == ConnectionState.Open)
         {
             con.Close();
         }
         con.Open();
         SqlCommand cmd = new SqlCommand("insert into [ref_role] (levelname,lvoid,lstatus,lsetid,luserid,ldatecreated) " +
             " output INSERTED.levelid values (@d1,@d2,@d3,@d4,@d5,@d6)", con);

         cmd.Parameters.AddWithValue("@d1", txt_rolename.Text);
         cmd.Parameters.AddWithValue("@d2", "0");
         cmd.Parameters.AddWithValue("@d3", txt_active.Text);
         cmd.Parameters.AddWithValue("@d4", rp.get_usersetid(User.Identity.Name));
         cmd.Parameters.AddWithValue("@d5", rp.get_userid(User.Identity.Name));
         cmd.Parameters.AddWithValue("@d6", pacificdatenow.ToString());
         int modifieduser_id = (int)cmd.ExecuteScalar();
         if (modifieduser_id > 0)
         {
             stat = modifieduser_id;

         }
         else
         {
             stat = 0;
         }
         con.Close();

         return stat;

     }
     public int rolepermission_add(string mainmenu, string sitename, string roleid, CheckBox ch_add,CheckBox ch_edit,CheckBox ch_delete, CheckBox ch_view)
     {
         int stat = 0;
         if (con.State == ConnectionState.Open)
         {
             con.Close();
         }
         con.Open();
         SqlCommand cmd = new SqlCommand("insert into [ref_rolepermission] (mainmenu,psitename,proleid,padd,pedit,pdelete,pview,pprint,pentryby) " +
             " values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9)", con);

         cmd.Parameters.AddWithValue("@d1", mainmenu);
         cmd.Parameters.AddWithValue("@d2", sitename);
         cmd.Parameters.AddWithValue("@d3", roleid);
         if (ch_add.Checked == true)
         {
             cmd.Parameters.AddWithValue("@d4", "1");
         }
         else
         {
             cmd.Parameters.AddWithValue("@d4", "0");
         }
         if (ch_edit.Checked == true)
         {
             cmd.Parameters.AddWithValue("@d5", "1");
         }
         else
         {
             cmd.Parameters.AddWithValue("@d5", "0");
         }
         if (ch_delete.Checked == true)
         {
             cmd.Parameters.AddWithValue("@d6", "1");
         }
         else
         {
             cmd.Parameters.AddWithValue("@d6", "0");
         }
         if (ch_view.Checked == true)
         {
             cmd.Parameters.AddWithValue("@d7", "1");
         }
         else
         {
             cmd.Parameters.AddWithValue("@d7", "0");
         }
         
             cmd.Parameters.AddWithValue("@d8", "1");
        
         cmd.Parameters.AddWithValue("@d9", rp.get_userid(User.Identity.Name));
         int res = cmd.ExecuteNonQuery();
         if (res > 0)
         {
             stat = 1;
         }
         else
         {
             stat = 0;
         }
         con.Close();
         return stat;
     }
     public int update_role()
     {
         int stat = 0;
         if (con.State == ConnectionState.Open)
         {
             con.Close();
         }
         con.Open();
         SqlCommand cmd = new SqlCommand("update [ref_role] set levelname=@d1,lstatus=@d2 where  levelid=@id", con);

         cmd.Parameters.AddWithValue("@d1", txt_rolename.Text);
         cmd.Parameters.AddWithValue("@d2", txt_active.Text);
         cmd.Parameters.AddWithValue("@id", hd_id.Value);
         int res = cmd.ExecuteNonQuery();
         if (res > 0)
         {
             stat = res;

         }
         else
         {
             stat = 0;
         }
         con.Close();

         return stat;

     }
     public int rolepermission_update(string permissionid, CheckBox ch_add, CheckBox ch_edit, CheckBox ch_delete, CheckBox ch_view)
     {
         int stat = 0;
         if (con.State == ConnectionState.Open)
         {
             con.Close();
         }
         con.Open();
         SqlCommand cmd = new SqlCommand("update [ref_rolepermission] set padd=@add,pedit=@edit,pdelete=@delete,pview=@view where permissionid=@id ", con);

         cmd.Parameters.AddWithValue("@id", permissionid);
   
         if (ch_add.Checked == true)
         {
             cmd.Parameters.AddWithValue("@add", "1");
         }
         else
         {
             cmd.Parameters.AddWithValue("@add", "0");
         }
         if (ch_edit.Checked == true)
         {
             cmd.Parameters.AddWithValue("@edit", "1");
         }
         else
         {
             cmd.Parameters.AddWithValue("@edit", "0");
         }
         if (ch_delete.Checked == true)
         {
             cmd.Parameters.AddWithValue("@delete", "1");
         }
         else
         {
             cmd.Parameters.AddWithValue("@delete", "0");
         }
         if (ch_view.Checked == true)
         {
             cmd.Parameters.AddWithValue("@view", "1");
         }
         else
         {
             cmd.Parameters.AddWithValue("@view", "0");
         }

   
         int res = cmd.ExecuteNonQuery();
         if (res > 0)
         {
             stat = 1;
         }
         else
         {
             stat = 0;
         }
         con.Close();
         return stat;
     }
     private void branch_table()
     {

         con = new SqlConnection(con.ConnectionString);
         con.Open();
         using (SqlCommand cmd = new SqlCommand())
         {
          
             string sql = "select * FROM ref_role where (lsetid =@setid or luserid=@userid) and lvoid=0";
            
             cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
             cmd.Parameters.AddWithValue("@userid", rp.get_userid(User.Identity.Name));
             cmd.CommandText = sql;
             cmd.Connection = con;
             using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
             {
                 DataTable dt = new DataTable();
                 sda.Fill(dt);
                 GridView_role.DataSource = dt;
                 GridView_role.DataBind();
             }
         }
         lbl_item.Text = rp.footerinfo_gridview(GridView_role);
         //}
         //catch (Exception ex)
         //{

         //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);

         //        }
     }
     protected void OnPaging(object sender, GridViewPageEventArgs e)
     {

         GridView_role.PageIndex = e.NewPageIndex;


         branch_table();


     }
     protected void GridView_role_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // {LinkButton lnk2 = (LinkButton)e.Row.FindControl("LinkButton2");
         


        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {

        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lnk2 = (LinkButton)e.Row.FindControl("btn_selectbranch");
            LinkButton lnk3 = (LinkButton)e.Row.FindControl("btn_deletebranch");
            if (edit == 1)
            {
                lnk2.Visible = true;
            }
            else
            {
                lnk2.Visible = false;
            }
            if (delete == 1)
            {
                lnk3.Visible = true;
            }
            else
            {
                lnk3.Visible = false;
            }
        }
    }
       protected void btn_selectbranch_Click(object sender, EventArgs e)
       {
           LinkButton btn_select = (LinkButton)sender;
           GridViewRow item = (GridViewRow)btn_select.NamingContainer;
           HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
           HiddenField hd_rolename = (HiddenField)item.FindControl("hd_rolename");
           HiddenField hd_status = (HiddenField)item.FindControl("hd_status");

           hd_id.Value = hd_idselect.Value;
           txt_rolename.Text = hd_rolename.Value;
           txt_active.Text = hd_status.Value;

           role_table(hd_idselect.Value);
       }
       protected void btn_deletebranch_Click(object sender, EventArgs e)
       {
           LinkButton btn_select = (LinkButton)sender;
           GridViewRow item = (GridViewRow)btn_select.NamingContainer;
           HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
           HiddenField hd_rolename = (HiddenField)item.FindControl("hd_rolename");

           con.Open();
           String cb = "Update [ref_role] set lvoid ='1' where levelid = " + hd_idselect.Value + " and lsetid = " + rp.get_usersetid(User.Identity.Name) + " ";
           cmd = new SqlCommand(cb);
           cmd.Connection = con;

           int result1 = cmd.ExecuteNonQuery();

           con.Close();
           if (result1 >= 1)
           {
               rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Void", "User role value: " + hd_rolename.Value, rp.get_usersetid(User.Identity.Name).ToString());
               ShowMessage("Successfully deleted!", MessageType.Success);
               ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#modalPopUp_Delete').modal('hide')", true);
               branch_table();
           }
           con.Close();

       }
}