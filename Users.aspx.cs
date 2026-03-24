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

public partial class Users : System.Web.UI.Page
{

    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    int branchid = 0;
    repeatedcode rp = new repeatedcode();
    public int add, edit, delete, view, print;
    public enum MessageType { Success, Error, Info, Warning };
    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        // modalPopUp_add.Attributes["class"] = "modal fade show";
        // modalPopUp_add.Attributes["style"] = "display: block;";

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
                btn_additem.Enabled = true;
                btn_additem.Visible = true;
            }
            else
            {
                btn_additem.Enabled = false;
                btn_additem.Visible = false;
            }


            branch_table("");

        }
    }



    private void branch_table(string top10)
    {
     
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string sql = "SELECT " + top10 + " ref_account.* " +
                ", (SELECT levelname from  ref_role where levelid = uroleid) as [role] " +
              //  ", CONVERT(varchar(50), CASE actstatus WHEN 1 THEN 'Active' ELSE 'Inactive' END) as [status]  " +
                //  ", CASE aclevel WHEN 'ADMIN' THEN 'Administrator' ELSE 'End User' END as [role]  " +
                "FROM ref_account where (uvoid = 0 and usetid =@setid)  ";
            if (!string.IsNullOrEmpty(txt_search.Text.Trim()))
            {
                sql += " and ( ufullname  LIKE '%' + @m1 + '%' " +
                    " or usname LIKE '%' + @m2 + '%'  or uemail LIKE '%' + @m3 + '%'  or (SELECT levelname from  ref_role where levelid = uroleid) LIKE '%' + @m3 + '%' or (ustatus =@m2 ) " +
                ") order by uregisterdate asc";
                cmd.Parameters.AddWithValue("@m1", txt_search.Text.Trim());
                cmd.Parameters.AddWithValue("@m2", txt_search.Text.Trim());
                cmd.Parameters.AddWithValue("@m3", txt_search.Text.Trim());
            }
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
        lbl_item.Text = rp.footerinfo_gridview(gv_masterlist);
        //}
        //catch (Exception ex)
        //{

        //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);

        //        }
    }
    protected void OnPaging(object sender, GridViewPageEventArgs e)
    {

        gv_masterlist.PageIndex = e.NewPageIndex;


        branch_table("");


    }
    protected void btn_selectbranch_Click(object sender, EventArgs e)
    {
        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");

        Page.Response.Redirect("UsersEntry.aspx?id=" + rp.Encrypt(hd_idselect.Value));


        // txt_c = hd_contact.Value;
    }
    protected void btn_deletebranch_Click(object sender, EventArgs e)
    {
        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");

        HiddenField hd_usname = (HiddenField)item.FindControl("hd_usname");

        //if (rp.identify_counter(" trans_stockrunning where s_entryby = " + hd_idselect.Value + " ") > 0)
        //{
        //    ShowMessage("Unable to delete user, already used by another process!", MessageType.Warning);
        //    return;

        //}

        con.Open();
        String cb = "Update [ref_account] set uvoid ='1' where uid = " + hd_idselect.Value + " and usetid = " + rp.get_usersetid(User.Identity.Name) + " ";
        cmd = new SqlCommand(cb);
        cmd.Connection = con;

        int result1 = cmd.ExecuteNonQuery();

        con.Close();
        if (result1 >= 1)
        {
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Void", "User account void value: " + hd_usname.Value, rp.get_usersetid(User.Identity.Name).ToString());
            ShowMessage("Successfully deleted!", MessageType.Success);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#modalPopUp_Delete').modal('hide')", true);
            branch_table("");
        }
        con.Close();


    }

    protected void gv_masterlist_RowDataBound(object sender, GridViewRowEventArgs e)
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
            HiddenField hd_roleid = (HiddenField)e.Row.FindControl("hd_roleid");



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
                if (hd_roleid.Value == "1")
                {
                    lnk3.Visible = false;
                }
                else
                {
                lnk3.Visible = true;
                }
            }
            else
            {
                lnk3.Visible = false;
            }


            //foreach (GridViewRow row in gv_masterlist.Rows)
            //{
            //    //  string Namecolumnvalue = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "modelname"));

            //    Label lblno = (Label)e.Row.FindControl("Label1");

            //    int no = Convert.ToInt32(lblno.Text);
            //    lblno.Text = (no + 1).ToString();
            //}


        }
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {

        branch_table("");

    }
    protected void btn_reset_Click(object sender, EventArgs e)
    {
        txt_search.Text = "";

        branch_table("");
        txt_search.Focus();

    }
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        branch_table("");
    }
}