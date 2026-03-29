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

public partial class SalesRecord : System.Web.UI.Page
{

    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    int branchid = 0;
    repeatedcode rp = new repeatedcode();
    public int add, edit, delete, view, print;
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
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
        add = rp.access_user(User.Identity.Name, "SalesRecord", "padd");
        edit = rp.access_user(User.Identity.Name, "SalesRecord", "pedit");
        delete = rp.access_user(User.Identity.Name, "SalesRecord", "pdelete");
        view = rp.access_user(User.Identity.Name, "SalesRecord", "pview");
        print = rp.access_user(User.Identity.Name, "SalesRecord", "pprint");

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
          
            string entrybyqry = "(SELECT ufullname from  ref_account where uid = invoicentryby)";
            string sql = "SELECT " + top10 + "  " +
                    "invoiceid,invoicedate,invoiceno,invoicecustomer,invoicesubtotal,invoicediscountamt,invoicetax,invoicetotal,invoinceamounttendered,invoicechanged,invoicenote " +
                  ", " + entrybyqry + " as [entryby] " +
                //  ", CONVERT(varchar(50), CASE actstatus WHEN 1 THEN 'Active' ELSE 'Inactive' END) as [status]  " +
                //  ", CASE aclevel WHEN 'ADMIN' THEN 'Administrator' ELSE 'End User' END as [role]  " +
                "FROM trans_invoice where (invoicevoid = 0 and invoicesetid =@setid)  ";
            if (!string.IsNullOrEmpty(txt_searchkeyword.Text.Trim()))
            {
                sql += " and (invoicecustomer LIKE '%' + @m1 + '%' or invoicenote  LIKE '%' + @m1 + '%' " +
                    " or invoiceno LIKE '%' + @m1 + '%' " +
                ") ";
                cmd.Parameters.AddWithValue("@m1", txt_searchkeyword.Text);
                

            }
            if (!string.IsNullOrEmpty(txt_datefrom.Text) && !string.IsNullOrEmpty(txt_dateto.Text))
            {


                string from = txt_datefrom.Text + " 00:00:00.000";
                string to = txt_dateto.Text + " 23:59:59.999";
                string filterdate = " and (invoicedate BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }
            sql += "order by invoicedate desc";
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

        Page.Response.Redirect("pos1.aspx?id=" + rp.Encrypt(hd_idselect.Value));


        // txt_c = hd_contact.Value;
    }
    protected void btn_deletebranch_Click(object sender, EventArgs e)
    {
        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");

        HiddenField hd_name = (HiddenField)item.FindControl("hd_name");

        //if (rp.identify_counter(" trans_stockrunning where s_entryby = " + hd_idselect.Value + " ") > 0)
        //{
        //    ShowMessage("Unable to delete user, already used by another process!", MessageType.Warning);
        //    return;

        //}

        con.Open();
        String cb = "Update [trans_invoice] set invoicevoid ='1' where invoiceid = " + hd_idselect.Value + "";
        cmd = new SqlCommand(cb);
        cmd.Connection = con;

        int result1 = cmd.ExecuteNonQuery();

        con.Close();

        con.Open();
        String cb1 = "Update [trans_invoicecart] set cartvoid ='1' where cartinvoiceid = " + hd_idselect.Value + "";
        cmd = new SqlCommand(cb1);
        cmd.Connection = con;

        int result2 = cmd.ExecuteNonQuery();
        con.Close();

        con.Open();
        String cb2 = "Update [trans_inventory] set inventoryvoid ='1' where invent_soldsalesid = " + hd_idselect.Value + "";
        cmd = new SqlCommand(cb2);
        cmd.Connection = con;

        int result3 = cmd.ExecuteNonQuery();
        if (result1 >= 1 || result2 >= 1 || result3 >= 1)
        {
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Void", "Sales deleted value: " + hd_name.Value, rp.get_usersetid(User.Identity.Name).ToString());
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
            HiddenField hd_id = (HiddenField)e.Row.FindControl("hd_id");

            Repeater Repeater_cart = (Repeater)e.Row.FindControl("Repeater_cart") as Repeater;
            branch_table_repeater("", Repeater_cart, hd_id.Value); 


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
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModalsearch();", true);
       

    }
    protected void btn_reset_Click(object sender, EventArgs e)
    {
        txt_searchkeyword.Text = "";
        txt_datefrom.Text = "";
        txt_dateto.Text = "";
        branch_table("");
        dp_filterby.SelectedIndex = 0;
        txt_dateto.Enabled = false;
        txt_datefrom.Enabled = false;

      //  txt_searchkeyword.Focus();

    }
  


    private void branch_table_repeater(string top10, Repeater repeater, string invoiceid)
    {

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {

            string uomcase = " case when prodbaseunit !=1 then  'of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else '' end as [extension]  ";
            string uomqry = "(select uomname + ' ' + " + uomcase + "  from ref_units,trans_product where productid=cartproductid and ref_units.uomid=trans_product.productunit and uomvoid=0) as [uomname] ";



            string sql = "select cartid,cartproductid as[id], cartprice as [price], cartqty as [qty],cartamount as [Amount], " +

                " (Select productname + ' ' + " + uomqry + " from trans_product where productid=cartproductid)  as [itemname]" +
                ", cartdatecreated from trans_invoicecart where cartinvoiceid =@id and cartvoid =0 and cartsetid=@setid  ";
         
         

            sql += " order by cartdatecreated desc";
            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@id", invoiceid);
            cmd.CommandText = sql;
            cmd.Connection = con;
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);
                repeater.DataSource = dt;
                repeater.DataBind();
            }

            int c = repeater.Items.Count - 1;
            for (int items = 0; items < repeater.Items.Count; items++)
            {
             



            }
        }
        //  lbl_item.Text = rp.footerinfo_gridview(gv_masterlist);
        //}
        //catch (Exception ex)
        //{

        //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);

        //        }
    }

    public void read_invoicecart(Repeater repeater, string invoiceid)
    {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
       con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmds = new SqlCommand())
        {


            string uomcase = " case when prodbaseunit !=1 then  'of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else '' end as [extension]  ";
            string uomqry = "(select uomname + ' ' + " + uomcase + "  from ref_units,trans_product where productid=cartproductid and ref_units.uomid=trans_product.productunit and uomvoid=0) as [uomname] ";



            string qry = "select cartid,cartproductid as[id], cartprice as [price], cartqty as [qty],cartamount as [Amount], " +

                " (Select productname + ' ' + " + uomqry + " from trans_product where productid=cartproductid)  as [itemname]" +
                "from trans_invoicecart where cartinvoiceid =@id and cartvoid =0 and cartsetid=@setid  ";
         
     
            cmds.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmds.Parameters.AddWithValue("@id", invoiceid);

            cmds.CommandText = qry;
            cmds.Connection = con;
            using (SqlDataAdapter sda = new SqlDataAdapter(cmds))
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);
                repeater.DataSource = dt;
                repeater.DataBind();
            }
        }
    }
    protected void dp_daterange_SelectedIndexChanged(object sender, EventArgs e)
    {
        rp.filterdate_template(dp_filterby, txt_datefrom, txt_dateto);

    }
  
}