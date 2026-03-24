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
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

public partial class mStockSheet : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "StockSheet", "padd");
        edit = rp.access_user(User.Identity.Name, "StockSheet", "pedit");
        delete = rp.access_user(User.Identity.Name, "StockSheet", "pdelete");
        view = rp.access_user(User.Identity.Name, "StockSheet", "pview");
        print = rp.access_user(User.Identity.Name, "StockSheet", "pprint");

        if (view == 0)
        {
            Page.Response.Redirect("Home.aspx");
        }
        if (!IsPostBack)
        {

            rp.dropdown_idtext(txt_searchcategory, "ref_productcategory where pcategsetid = " + rp.get_usersetid(User.Identity.Name) + " and pcategvoid=0 and pcategstatus ='Active' ", "pcategid", "pcategname");

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
            string instock = "(select case when sum(inventoryqty) > 0 then sum(inventoryqty) else 0 end from trans_inventory where inventorytype='In' and inproductid =productid and inventoryvoid=0 and inventorysetid=@setid)";
            string outstock = "(select case when sum(inventoryqty * inventoryqty) > 0 then  ABS(sum(inventoryqty)) else 0 end from trans_inventory where inventorytype='Out' and inproductid =productid and inventoryvoid=0 and inventorysetid=@setid)";
            string sql = "SELECT " + top10 + " trans_product.*  " +
              
                ", " + categqry + " as [category] " +
                ",  case when prodbaseunit !=1 then CONVERT(VARCHAR(50), prodbaseunit,20) else '' end + ' ' + " + unitqry + " as [baseunit] " +
                 ", " + stockqty + " as [stockqty] " +
                 ", " + instock + " as [in] " +
                 ", " + outstock + " as [out] " +
                "FROM trans_product where (producttype='Product' and prodvoid = 0 and prodsetid =@setid)  ";
            if (!string.IsNullOrEmpty(txt_search.Text.Trim()))
            {
                sql += " and ( productname  LIKE '%' + @m1 + '%' " +
                    " or (prodstatus =@m1 ) or producttype=@m1 " +
                ") ";

                cmd.Parameters.AddWithValue("@m1", txt_search.Text.Trim());


            }
            if (txt_searchcategory.SelectedIndex != 0)
            {
                sql += " and  productcateg =@m2";
                cmd.Parameters.AddWithValue("@m2", txt_searchcategory.SelectedValue);
            }
            //if (txt_searchcategory.SelectedIndex != 0)
            //{
            //    sql += " and  productcateg =@m2";
            //    cmd.Parameters.AddWithValue("@m2", txt_searchcategory.SelectedValue);
            //}
            sql += " order by productname asc";
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
        //    LinkButton lnk2 = (LinkButton)e.Row.FindControl("btn_selectbranch");

            HiddenField hd_productid = (HiddenField)e.Row.FindControl("hd_productid");

            Repeater Repeater_aging = (Repeater)e.Row.FindControl("Repeater_aging") as Repeater;
            branch_table_repeater("", Repeater_aging, hd_productid.Value); 
           


        }
    }

    private void branch_table_repeater(string top10, Repeater repeater, string productid)
    {

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {



            string name = "(select productname from trans_product where trans_product.productid=inproductid)";
            string baseunit = "(select case when prodbaseunit !=1 then   CONVERT(VARCHAR(50), prodbaseunit,20) else '' end from trans_product where trans_product.productid=inproductid)";
            string categqry = "(select pcategname from ref_productcategory,trans_product where trans_product.productid=inproductid and pcategid =productcateg)";
            string unitqry = "(select ref_units.uomname from ref_units,trans_product where trans_product.productid=inproductid and ref_units.uomid = trans_product.productunit)";
            string sql = "SELECT " + top10 + " trans_inventory.*, ABS(inventoryqty) AS [abqty]  " +
                ", (SELECT ufullname from  ref_account where uid = inventoryentryby) as [usname] " +
                ", " + categqry + " as [category] " +
                ", " + baseunit + " + ' ' + " + unitqry + " as [unit] " +
                    ", " + name + " as [name] " +
                "FROM trans_inventory where ( inventoryvoid = 0 and inventorysetid =@setid and inproductid=@prodid)  ";
            if (!string.IsNullOrEmpty(txt_search.Text.Trim()))
            {
                sql += " and ( " + name + "  LIKE '%' + @m1 + '%' " +
                    " or " + categqry + " =@m1 or inventorynote =@m1 " +
                ") ";

                cmd.Parameters.AddWithValue("@m1", txt_search.Text.Trim());


            }
         
            sql += " order by inventorydate asc";
            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
                cmd.Parameters.AddWithValue("@prodid", productid);
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
                HiddenField hd_inventoryid = repeater.Items[items].FindControl("hd_inventoryid") as HiddenField;
                HiddenField hd_inproductid = repeater.Items[items].FindControl("hd_inproductid") as HiddenField;
             


            }
        }
      //  lbl_item.Text = rp.footerinfo_gridview(gv_masterlist);
        //}
        //catch (Exception ex)
        //{

        //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);

        //        }
    }
    protected void btn_search_Click(object sender, EventArgs e)
    {

        branch_table("");

    }
    protected void btn_reset_Click(object sender, EventArgs e)
    {
        txt_search.Text = "";
        txt_searchcategory.SelectedIndex = 0;
        branch_table("");
        txt_search.Focus();

    }
    protected void txt_search_TextChanged(object sender, EventArgs e)
    {
        branch_table("");
      
    }

  
}