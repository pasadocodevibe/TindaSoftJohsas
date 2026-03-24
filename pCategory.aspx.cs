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
public partial class pCategory : System.Web.UI.Page
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
            add = rp.access_user(User.Identity.Name, "ItemCategory", "padd");
            edit = rp.access_user(User.Identity.Name, "ItemCategory", "pedit");
            delete = rp.access_user(User.Identity.Name, "ItemCategory", "pdelete");
            view = rp.access_user(User.Identity.Name, "ItemCategory", "pview");
            print = rp.access_user(User.Identity.Name, "ItemCategory", "pprint");
            setid = rp.get_usersetid(User.Identity.Name).ToString();

            if (!IsPostBack)
            {


                this.branch_table("");

            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }

    }
    protected void btn_reset_Click(object sender, EventArgs e)
    {
        branch_reset();
    }
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        this.branch_table("");
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        this.branch_table("");
    }

    protected void btn_add_Click(object sender, EventArgs e)
    {
        try
        {
            if (hd_id.Value == "")
            {
                if (rp.identify_counter(" ref_productcategory where pcategname ='" + txt_brandname.Text + "' and pcategsetid = " + rp.get_usersetid(User.Identity.Name) + " ") > 0)
                {
                    ShowMessage("Already exist!", MessageType.Warning);
                    txt_brandname.Focus();
                    return;
                }

                if (branch_add() == 1)
                {
                    rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Product category added item: " + txt_brandname.Text, setid);
                    ShowMessage("Successfully added!", MessageType.Success);
                    branch_reset();
                }
            }
            else
            {

                if (rp.identify_counter(" ref_productcategory where pcategname ='" + txt_brandname.Text + "' and pcategsetid = " + rp.get_usersetid(User.Identity.Name) + " and pcategid != " + hd_id.Value + " ") > 0)
                {
                    ShowMessage("Already exist!", MessageType.Warning);
                    txt_brandname.Focus();
                    return;
                }
                if (branch_update() == 1)
                {
                    rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Edit", "Product category updated item: " + txt_brandname.Text, setid);
                    ShowMessage("Successfully updated", MessageType.Success);
                    branch_reset();
                }
            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }
    public void branch_reset()
    {
        hd_id.Value = "";
        this.branch_table("");
        txt_brandname.Text = "";
        txt_status.Text = "Active";
        txt_brandname.Focus();

    }
    public int branch_add()
    {
        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("insert into [ref_productcategory]  (pcategname,pcategsetid,pcategvoid,pcategstatus,pcategdatecreated,pentryby) " +
                " values(@d1,@d2,@d3,@d4,@d5,@d6)", con);
            //   DateTime dtnow = DateTime.Now;

            cmd.Parameters.AddWithValue("@d1", txt_brandname.Text);
            cmd.Parameters.AddWithValue("@d2", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d3", "0");
            cmd.Parameters.AddWithValue("@d4", txt_status.Text);
            cmd.Parameters.AddWithValue("@d5", pacificdatenow.ToString());
            cmd.Parameters.AddWithValue("@d6", rp.get_userid(User.Identity.Name));


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
            SqlCommand cmd = new SqlCommand("update ref_productcategory set pcategname=@d1,pcategstatus =@d2 where pcategid = @idss and pcategsetid=@d3 ", con);


            cmd.Parameters.AddWithValue("@d1", txt_brandname.Text);
            cmd.Parameters.AddWithValue("@d2", txt_status.Text);
    
            cmd.Parameters.AddWithValue("@d3", rp.get_usersetid(User.Identity.Name));
        

            cmd.Parameters.AddWithValue("@idss", hd_id.Value);

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
    private void branch_table(string top10)
    {
        try
        {
            string searchqry = " and pcategname like '%" + txt_search.Text + "%' or pcategstatus ='" + txt_search.Text + "' ";
            string orderasc = " order by pcategname asc ";
            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = "SELECT " + top10 + "  * FROM ref_productcategory where pcategvoid =0 and pcategsetid = " + rp.get_usersetid(User.Identity.Name) + "  ";

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
        HiddenField hd_brandname = (HiddenField)item.FindControl("HiddenField1");
        HiddenField hd_status = (HiddenField)item.FindControl("hd_status");

        //if (rp.identify_counter(" trans_product where prodvoid=0 and productcateg ='" + hd_idselect.Value + "' ") > 0)
        //{
        //    ShowMessage("Unable to edit, already used by another process!", MessageType.Warning);
        //    return;
        //}


        hd_id.Value = hd_idselect.Value;
        txt_brandname.Text = hd_brandname.Value;
        txt_status.Text = hd_status.Value;

    }
    protected void btn_deletebranch_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton btn_select = (LinkButton)sender;
            GridViewRow item = (GridViewRow)btn_select.NamingContainer;
            HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
            HiddenField hd_brandname = (HiddenField)item.FindControl("HiddenField1");


            if (rp.identify_counter("trans_product WHERE productcateg = " + hd_idselect.Value +"  ") > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#modalPopUp_Delete').modal('hide')", true);
                ShowMessage("Unable to delete item, already used by another process!", MessageType.Warning);
            }
            else
            {



            con.Open();
            String cb = "Delete from [ref_productcategory] where pcategid = " + hd_idselect.Value + " and pcategsetid = " + rp.get_usersetid(User.Identity.Name) + " ";
            cmd = new SqlCommand(cb);
            cmd.Connection = con;

            int result = cmd.ExecuteNonQuery();
            if (result >= 1)
            {
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Delete", "Product category deleted value: " + hd_brandname.Value, rp.get_usersetid(User.Identity.Name).ToString());
                ShowMessage("Successfully deleted!", MessageType.Success);
                this.branch_table("");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#modalPopUp_Delete').modal('hide')", true);
            }
            con.Close();
            }
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
}