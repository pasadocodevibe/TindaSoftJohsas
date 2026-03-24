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

public partial class Productlist : System.Web.UI.Page
{

    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    int branchid = 0;
    repeatedcode rp = new repeatedcode();
    public int add, edit, delete, view, print;
    public enum MessageType { Success, Error, Info, Warning };
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        // modalPopUp_add.Attributes["class"] = "modal fade show";
        // modalPopUp_add.Attributes["style"] = "display: block;";

        //     branchid = rp.get_userbranchid(User.Identity.Name);
        add = rp.access_user(User.Identity.Name, "Items", "padd");
        edit = rp.access_user(User.Identity.Name, "Items", "pedit");
        delete = rp.access_user(User.Identity.Name, "Items", "pdelete");
        view = rp.access_user(User.Identity.Name, "Items", "pview");
        print = rp.access_user(User.Identity.Name, "Items", "pprint");

        if (view == 0)
        {
            Page.Response.Redirect("Home.aspx");
        }
        if (!IsPostBack)
        {
            rp.dropdown_idtext(txt_searchcategory, "ref_productcategory where pcategsetid = " + rp.get_usersetid(User.Identity.Name) + " and pcategvoid=0 and pcategstatus ='Active' ", "pcategid", "pcategname");


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

            rp.bindlist_filterdate(dp_filterby);
            branch_table("");

        }
    }



    private void branch_table(string top10)
    {

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string stockqty = "(select case when sum(inventoryqty) > 0 then sum(inventoryqty) else 0 end from trans_inventory where inproductid =productid and inventoryvoid=0)";
            string categqry = "(select pcategname from ref_productcategory where pcategid =productcateg)";
            string unitqry = "(select uomname from ref_units where uomid =productunit)";
            string brandqry = "(select name from ref_brand where id =prodbrand)";
            string sql = "SELECT " + top10 + " trans_product.*  " +
                ", (SELECT ufullname from  ref_account where uid = prodentryby) as [usname] " +
                ", " + categqry +" as [category] " +
                ",  case when prodbaseunit !=1 then CONVERT(VARCHAR(50), prodbaseunit,20) else '' end + ' ' + " + unitqry + " as [baseunit] " +
                 ", " + stockqty + " as [stockqty] " +
                 ", " + brandqry + " as [brandname] " +
                "FROM trans_product where (prodvoid = 0 and prodsetid =@setid)  ";
            if (!string.IsNullOrEmpty(txt_searchkeyword.Text.Trim()))
            {
                sql += " and ( productname  LIKE '%' + @m1 + '%' " +
                    " or (prodstatus =@m1 ) or producttype=@m1  or " + brandqry + " LIKE '%' + @m1 + '%' " +
                ") ";

                cmd.Parameters.AddWithValue("@m1", txt_searchkeyword.Text.Trim());
               
        
            }

            if (txt_searchcategory.SelectedIndex != 0)
            {
                sql += " and  productcateg =@m2";
                cmd.Parameters.AddWithValue("@m2", txt_searchcategory.SelectedValue);
            }
            if (!string.IsNullOrEmpty(txt_datefrom.Text) && !string.IsNullOrEmpty(txt_dateto.Text))
            {
          

                string from = txt_datefrom.Text + " 00:00:00.000";
                string to = txt_dateto.Text + " 23:59:59.999";
                string filterdate = " and (proddatecreated BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }
            sql += " order by proddatecreated asc";
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

        Page.Response.Redirect("ProductEntry.aspx?id=" + rp.Encrypt(hd_idselect.Value));


        // txt_c = hd_contact.Value;
    }
    protected void btn_deletebranch_Click(object sender, EventArgs e)
    {
        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
         HiddenField hd_prodname = (HiddenField)item.FindControl("hd_prodname");
        int countcartsold = rp.identify_counter(" trans_invoicecart where cartvoid=0 and  cartproductid = " + hd_idselect.Value + " ");
        int countstockin = rp.identify_counter(" trans_inventory where inventoryvoid=0 and  inproductid = " + hd_idselect.Value + " ");
        if (countcartsold > 0 || countstockin > 0)
         {
             ShowMessage("Unable to delete item, already used by another process!", MessageType.Warning);
             return;

         }

        con.Open();
        String cb = "Update [trans_product] set prodvoid ='1' where productid = " + hd_idselect.Value + "";
        cmd = new SqlCommand(cb);
        cmd.Connection = con;

        int result1 = cmd.ExecuteNonQuery();

        con.Close();
        if (result1 >= 1)
        {
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Void", "Product item void value: " + hd_prodname.Value, rp.get_usersetid(User.Identity.Name).ToString());
            ShowMessage("Successfully deleted!", MessageType.Success);
            branch_table("");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#modalPopUp_Delete').modal('hide')", true);
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


            //foreach (GridViewRow row in gv_masterlist.Rows)
            //{
            //    //  string Namecolumnvalue = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "modelname"));

            //    Label lblno = (Label)e.Row.FindControl("Label1");

            //    int no = Convert.ToInt32(lblno.Text);
            //    lblno.Text = (no + 1).ToString();
            //}


        }
    }

    // filter search \\

    protected void dp_daterange_SelectedIndexChanged(object sender, EventArgs e)
    {
        rp.filterdate_template(dp_filterby, txt_datefrom, txt_dateto);

    }
    protected void btn_adsearch_Click(object sender, EventArgs e)
    {
        if (txt_datefrom.Text != "" && txt_dateto.Text == "")
        {
            txt_dateto.Focus();
            return;
        }
        if (txt_datefrom.Text == "" && txt_dateto.Text != "")
        {
            txt_datefrom.Focus();
            return;
        }
        if (txt_datefrom.Text != "" && txt_dateto.Text != "")
        {
            DateTime dtfrom = Convert.ToDateTime(txt_datefrom.Text);
            DateTime dtto = Convert.ToDateTime(txt_dateto.Text);

            if (dtfrom > dtto)
            {
                ShowMessage("Invalid date range!", MessageType.Warning);
            }
            else
            {
                branch_table("");
            }
        }
        else
        {
            branch_table("");
        }
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#myModal_search').modal('hide')", true);
    }
    protected void btn_search_Click(object sender, EventArgs e)
    {
        dp_filterby.Focus();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModalsearch();", true);


    }
    protected void btn_reset_Click(object sender, EventArgs e)
    {
        txt_searchcategory.SelectedIndex = 0;
        dp_filterby.SelectedIndex = 0;
        txt_datefrom.Enabled = false;
        txt_dateto.Enabled = false;
        txt_searchkeyword.Text = "";
        txt_datefrom.Text = "";
        txt_dateto.Text = "";
        branch_table("");
        //  txt_searchkeyword.Focus();

    }
}