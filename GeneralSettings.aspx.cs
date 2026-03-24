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
using iTextSharp.text;

public partial class GeneralSettings : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    repeatedcode rp = new repeatedcode();
    public enum MessageType { Success, Error, Info, Warning };
    public int add, edit, delete, view, print;
    public string setid;
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }

    public void WriteErrorLog(Exception ex)
    {
        string webPageName = Path.GetFileName(Request.Path);
        string errorLogFilename = "ErrorLog_" + webPageName + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
        string path = Server.MapPath("~/log/" + errorLogFilename);
        if (File.Exists(path))
        {
            using (StreamWriter stwriter = new StreamWriter(path, true))
            {
                stwriter.WriteLine("-------------------Error Log Start-----------as on " + DateTime.Now.ToString("hh:mm tt"));
                stwriter.WriteLine("WebPage Name :" + webPageName);
                stwriter.WriteLine("Message:" + ex.ToString());
                stwriter.WriteLine("-------------------End----------------------------");
            }
        }
        else
        {
            StreamWriter stwriter = File.CreateText(path);
            stwriter.WriteLine("-------------------Error Log Start-----------as on " + DateTime.Now.ToString("hh:mm tt"));
            stwriter.WriteLine("WebPage Name :" + webPageName);
            stwriter.WriteLine("Message: " + ex.ToString());
            stwriter.WriteLine("-------------------End----------------------------");
            stwriter.Close();
        }
    }
  
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
       
            add = rp.access_user(User.Identity.Name, "Settings", "padd");
            edit = rp.access_user(User.Identity.Name, "Settings", "pedit");
            delete = rp.access_user(User.Identity.Name, "Settings", "pdelete");
            view = rp.access_user(User.Identity.Name, "Settings", "pview");
            print = rp.access_user(User.Identity.Name, "Settings", "pprint");
            setid = rp.get_usersetid(User.Identity.Name).ToString();

            if (!IsPostBack)
            {
                load_settings();
                dropdown_report(txt_reportname);
                rp.list_papersize(txt_papertype);
                this.branch_table("");
            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }

    }
    public void dropdown_report(DropDownList dplist)
    {
       
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        dplist.Items.Clear();
        int roleid = rp.get_userroleid(User.Identity.Name);
        string com = "select permissionid,psitename from ref_rolepermission where mainmenu='Report' and proleid= " + roleid.ToString() + " ";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);

        DataTable dt = new DataTable();
        adpt.Fill(dt);
        dplist.DataSource = dt;
        dplist.DataBind();
        dplist.DataTextField = "psitename";
        dplist.DataValueField = "permissionid";
        dplist.DataBind();

        dplist.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select...", "Select..."));

        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
    }
    protected void load_settings()
    {
        try
        {
         
            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = "SELECT * FROM ref_generalsettings where setid =@id and setvoid=0 ";

              
                    cmd.CommandText = sql ;
                    cmd.Parameters.AddWithValue("@id", rp.get_usersetid(User.Identity.Name));
                
                cmd.Connection = con;
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    txt_storename.Text = rdr["setcompanyname"].ToString();
                    txt_address.Text = rdr["setaddress"].ToString();
                    txt_contact.Text = rdr["setcontact"].ToString();
                    txt_details.Text = rdr["setnote"].ToString();
                    txt_email.Text = rdr["setmail"].ToString();

                    txt_status.Text = rdr["setstatus"].ToString();
                    if (rdr["setstyle"] != DBNull.Value)
                    {
                        dp_style.SelectedValue = rdr["setstyle"].ToString();
                    }

                }

                else
                {
                    ShowMessage("Unable to view already remove by system adminstrator!", MessageType.Warning);
                }
                rdr.Close();
            }
            con.Close();
         
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }
    protected void btn_add_Click(object sender, EventArgs e)
    {
     
        if (branch_update() == 1)
        {
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Edit", "Settings updated item: " + txt_storename.Text, setid);
            ShowMessage("Successfully Changed", MessageType.Success);
        
        }
    }
    public int branch_update()
    {

        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("update ref_generalsettings set setcompanyname=@d1,setaddress=@d2,setcontact=@d3,setmail=@d4,setnote=@d5,setstatus=@d6, setstyle=@d7 where setid = @idss  ", con);


            cmd.Parameters.AddWithValue("@d1", txt_storename.Text);
            cmd.Parameters.AddWithValue("@d2", txt_address.Text);

            cmd.Parameters.AddWithValue("@d3", txt_contact.Text);
            cmd.Parameters.AddWithValue("@d4", txt_email.Text);
    
            cmd.Parameters.AddWithValue("@d5", txt_details.Text);
            cmd.Parameters.AddWithValue("@d6", txt_status.Text);
            cmd.Parameters.AddWithValue("@d7", dp_style.SelectedValue);
            cmd.Parameters.AddWithValue("@idss", rp.get_usersetid(User.Identity.Name));

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
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
        return stat;
    }
    protected void btn_reportsettingadd_Click(object sender, EventArgs e)
    {
        

        if (hd_id.Value == "")
        {
            if (rp.identify_counter(" ref_reportsettings where reportname= '" + txt_reportname.Text + "' and reportsetid =" + rp.get_usersetid(User.Identity.Name) + " ") > 0)
            {
                ShowMessage("Report Settings already exist!", MessageType.Warning);
                return;
            }
            int c = reportsettings_add();
            if (c > 0)
            {
                ShowMessage("Successfully added!", MessageType.Success);
                reset_reportsettings();
            }
        }
        else
        {
            if (rp.identify_counter(" ref_reportsettings where reportid != " + hd_id.Value + " and reportname= '" + txt_reportname.Text + "' and reportsetid =" + rp.get_usersetid(User.Identity.Name) + " ") > 0)
            {
                ShowMessage("Report Settings already exist!", MessageType.Warning);
                return;
            }
              int c=  reportsettings_update();
              if (c > 0)
              {
                  ShowMessage("Successfully updated!", MessageType.Success);
                  reset_reportsettings();
              }
        }
        //upload_img(leftlogo, "test_");


    }
    public void reset_reportsettings()
    {
        this.branch_table("");
        txt_search.Text = "";
        txt_reportname.SelectedIndex = 0;
        txt_papertype.Text = "Select...";
      
        txt_bbottom.Text = "";
        txt_bleft.Text = "";
        txt_bright.Text = "";
        txt_btop.Text = "";
        txt_bbottom.Text = "";
        txt_pageorientation.Text = "Portrait";

        hd_id.Value = "";
        txt_reportname.Focus();
       
    }
    public string upload_img(FileUpload fl, string ab)
    {
        string url = "";
        try
        {

            if (fl.HasFile && (Path.GetExtension(fl.FileName) == ".jpg" || Path.GetExtension(fl.FileName) == ".jpeg" || Path.GetExtension(fl.FileName) == ".png"))
            {
               
               
                    string folderPath = Server.MapPath("~/images/storelogo/");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                string setid = rp.get_usersetid(User.Identity.Name).ToString();
                    //Save the File to the Directory (Folder).
                    fl.SaveAs(folderPath + setid + "_" + ab + Path.GetFileName(fl.FileName));

                    //Display the Picture in Image control.
                    url = "~/images/storelogo/" + setid + "_" + ab + Path.GetFileName(fl.FileName);
                
            }
            else
            {
                if (fl.HasFile && (Path.GetExtension(fl.FileName) != ".jpg" || Path.GetExtension(fl.FileName) != ".jpeg" || Path.GetExtension(fl.FileName) != ".png"))
                {
                    ShowMessage("jpeg file are allowed!", MessageType.Warning);
                }

                url = "";
            }
        }
        catch (Exception ex)
        {
            url = "";
        }
        return url;
    }
    public int reportsettings_add()
    {

        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("Insert Into ref_reportsettings (reportname,papertype,borderleft,borderright,bordertop,borderbottom,reportsetid," +
                "reportuserid,logolefturl,logorighturl,pageOrientation) values (@d1,@d2,@d5,@d6,@d7,@d8,@d9,@d10,@d11,@d12,@d13)", con);


            cmd.Parameters.AddWithValue("@d1", txt_reportname.Text);
            cmd.Parameters.AddWithValue("@d2", txt_papertype.Text);


            cmd.Parameters.AddWithValue("@d5", txt_bleft.Text);
            cmd.Parameters.AddWithValue("@d6", txt_bright.Text);
            cmd.Parameters.AddWithValue("@d7", txt_btop.Text);
            cmd.Parameters.AddWithValue("@d8", txt_bbottom.Text);
            cmd.Parameters.AddWithValue("@d9", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d10",  rp.get_userid(User.Identity.Name));
            
            cmd.Parameters.AddWithValue("@d11", upload_img(leftlogo, "left_").ToString() );
            cmd.Parameters.AddWithValue("@d12", upload_img(rightlogo, "right_").ToString() );
            cmd.Parameters.AddWithValue("@d13", txt_pageorientation.Text);

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
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
        return stat;
    }
    public int reportsettings_update()
    {

        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            string updateleftqry = "";
            string updaterightqry = "";
            if (leftlogo.HasFile)
            {
                updateleftqry = ",logolefturl=@d9 ";
            }
            else
            {
                updateleftqry = "";
            }
            if (rightlogo.HasFile)
            {
                updaterightqry = ",logorighturl=@d10 ";
            }
            else
            {
                updaterightqry = "";
            }
            SqlCommand cmd = new SqlCommand("Update ref_reportsettings set reportname=@d1,papertype=@d2,borderleft=@d5,borderright=@d6,bordertop=@d7,borderbottom=@d8," +
                "pageOrientation=@d11 " + updateleftqry + "" + updaterightqry + " where reportid=@id ", con);

            cmd.Parameters.AddWithValue("@id", hd_id.Value);
            cmd.Parameters.AddWithValue("@d1", txt_reportname.Text);
            cmd.Parameters.AddWithValue("@d2", txt_papertype.Text);

       
            cmd.Parameters.AddWithValue("@d5", txt_bleft.Text);
            cmd.Parameters.AddWithValue("@d6", txt_bright.Text);
            cmd.Parameters.AddWithValue("@d7", txt_btop.Text);
            cmd.Parameters.AddWithValue("@d8", txt_bbottom.Text);



            if (leftlogo.HasFile)
            {
                cmd.Parameters.AddWithValue("@d9", upload_img(leftlogo, "left_").ToString());
            }
           
            if (rightlogo.HasFile)
            {
                cmd.Parameters.AddWithValue("@d10", upload_img(rightlogo, "right_").ToString());
            }
          
           
            cmd.Parameters.AddWithValue("@d11", txt_pageorientation.Text);

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
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
        return stat;
    }
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        this.branch_table("");
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        this.branch_table("");
    }
    private void branch_table(string top10)
    {
        try
        {
            string reportname = " (select psitename from ref_rolepermission where permissionid=reportname) ";
            string searchqry = " and " + reportname + " like '%" + txt_search.Text + "%' or papertype like '%" + txt_search.Text + "%' ";
            string orderasc = " order by reportname asc ";
            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
            
                string sql = "SELECT " + top10 + "  reportid, " + reportname + " as [rpname] ,papertype + ' (' + pageOrientation + ') ' as [psizeorientation] , " +
                    " CONVERT(VARCHAR(50), borderleft) + 'L, ' + CONVERT(VARCHAR(50), borderright) + 'R, ' + CONVERT(VARCHAR(50), bordertop) + 'T, ' + CONVERT(VARCHAR(50), borderbottom) + 'B ' as [Margin] " +
                    "FROM ref_reportsettings where reportsetid = " + rp.get_usersetid(User.Identity.Name) + "  ";

                if (txt_search.Text.Length == 0)
                {
                    cmd.CommandText = sql + orderasc;
                }
                else
                {
                    cmd.CommandText = sql + searchqry + orderasc;

                }
                cmd.Connection = con;
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    gv_branch.DataSource = dt;
                    gv_branch.DataBind();
                }
            }

            lbl_item.Text = rp.footerinfo_gridview(gv_branch);
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }
    protected void OnPaging(object sender, GridViewPageEventArgs e)
    {

        gv_branch.PageIndex = e.NewPageIndex;
        this.branch_table("");
    }
    protected void btn_selectbranch_Click(object sender, EventArgs e)
    {
        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
        hd_id.Value = hd_idselect.Value;

          con = new SqlConnection(con.ConnectionString);
            con.Open();
            string qry = "select * from ref_reportsettings where reportid=@id";
            using (SqlCommand cmd = new SqlCommand(qry, con))
            {

                cmd.Parameters.AddWithValue("@id", hd_id.Value);
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    txt_reportname.Text = rdr[1].ToString();
                    txt_papertype.Text = rdr[2].ToString();
                  
                    txt_bleft.Text = rdr[5].ToString();
                    txt_bright.Text = rdr[6].ToString();
                    txt_btop.Text = rdr[7].ToString();
                    txt_bbottom.Text = rdr[8].ToString();
                    txt_pageorientation.Text = rdr["pageOrientation"].ToString();
                   
                }

                con.Close();
                rdr.Close();
            }
       

        //if (rp.identify_counter(" ref_motor where motorbrand ='" + hd_brandname.Value + "' ") > 0)
        //{
        //    ShowMessage("Unable to edit, already used by another process!", MessageType.Warning);
        //    return;
        //}


     
       

    }
    protected void btn_deletebranch_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton btn_select = (LinkButton)sender;
            GridViewRow item = (GridViewRow)btn_select.NamingContainer;
            HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
          

            //if (countexist(hd_brandname.Value) == 1)
            //{
            //    ShowMessage("Unable to delete, already used by another process!", MessageType.Warning);
            //}
            //else
            //{



            con.Open();
            String cb = "Delete from [ref_reportsettings] where reportid = " + hd_idselect.Value + " and reportuserid = " + rp.get_usersetid(User.Identity.Name) + " ";
            cmd = new SqlCommand(cb);
            cmd.Connection = con;

            int result = cmd.ExecuteNonQuery();
            if (result >= 1)
            {
              
                ShowMessage("Successfully deleted!", MessageType.Success);
                reset_reportsettings();
            }
            con.Close();
            //}
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }


    protected void gv_branch_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            // {LinkButton lnk2 = (LinkButton)e.Row.FindControl("LinkButton2");



            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                if (edit == 0 && delete == 0 && view == 1)
                {
                    e.Row.Cells[0].Visible = false;//this is your templatefield column.
                }
                else
                {
                    e.Row.Cells[0].Visible = true;
                }

            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {


                LinkButton lnk2 = (LinkButton)e.Row.FindControl("btn_selectbranch");
                LinkButton lnk3 = (LinkButton)e.Row.FindControl("btn_deletebranch");


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

                //   foreach (GridViewRow row in gv_masterlist.Rows)
                //{
                //     string Namecolumnvalue = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "modelname"));


                // }


            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured! please contact system administrator or check log file.", MessageType.Error);
        }
    }
    public void load_permission()
    {
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        string qry = "select * from ref_rolepermission where proleid=1";
        using (SqlCommand cmd = new SqlCommand(qry, con))
        {

          //  cmd.Parameters.AddWithValue("@id", hd_id.Value);
            rdr = cmd.ExecuteReader();
           while(rdr.Read())
            {

                insert_role(rdr[1].ToString(), rdr[2].ToString(), "2", rdr[4].ToString(), rdr[5].ToString(), rdr[6].ToString(), rdr[7].ToString(), rdr[8].ToString(), rdr[9].ToString());
            }

            con.Close();
            rdr.Close();
        }
       


    }
    public void insert_role(string d1,string d2,string d3,string d4,string d5,string d6,string d7,string d8,string d9)
    {
         con = new SqlConnection(con.ConnectionString);
        con.Open();
        string qry = "insert into ref_rolepermission(mainmenu,psitename,proleid,padd,pedit,pdelete,pview,pprint,pentryby) values(@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9) ";
        using (SqlCommand cmd = new SqlCommand(qry, con))
        {
            cmd.Parameters.AddWithValue("@d1", d1);
            cmd.Parameters.AddWithValue("@d2", d2);
            cmd.Parameters.AddWithValue("@d3", d3);
            cmd.Parameters.AddWithValue("@d4", d4);
            cmd.Parameters.AddWithValue("@d5", d5);
            cmd.Parameters.AddWithValue("@d6", d6);
            cmd.Parameters.AddWithValue("@d7", d7);
            cmd.Parameters.AddWithValue("@d8", d8);
           
            cmd.Parameters.AddWithValue("@d9", d9);
            int a = cmd.ExecuteNonQuery();
            if (a > 0)
            {
                ShowMessage("Successfully added role", MessageType.Success);
            }
        }
        //on.Close();
    }
}