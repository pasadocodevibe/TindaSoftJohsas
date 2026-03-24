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

public partial class ManageStock : System.Web.UI.Page
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
    public int setid = 0;
   
    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }
    public enum Sweet_Alert_Type { basic = 1, success = 2, warning = 3, error = 4 }
    public void sweetmsg(string mgs, string title, Sweet_Alert_Type type)
    {
        string str_alert_Msg = @" swal.fire({
                                    type: '" + type + @"',
                                    title:'" + title + @"',
                                    text: '" + mgs.ToString() + @"',
                                    confirmButtonColor: '#DD6B55',
                                    animation: 'slide-from-top',
                                    allowEscapeKey: true,
                                    allowOutsideClick: true
                                
                               });";

        ScriptManager.RegisterStartupScript(this, GetType(), "Popup", str_alert_Msg, true);

    }
   
    protected void Page_Load(object sender, EventArgs e)
    {
        add = rp.access_user(User.Identity.Name, "Managestock", "padd");
        edit = rp.access_user(User.Identity.Name, "Managestock", "pedit");
        delete = rp.access_user(User.Identity.Name, "Managestock", "pdelete");
        view = rp.access_user(User.Identity.Name, "Managestock", "pview");
        print = rp.access_user(User.Identity.Name, "Managestock", "pprint");
        //sweetmsg("test", "", Sweet_Alert_Type.success);
        if (!IsPostBack)
        {
            setid = rp.get_usersetid(User.Identity.Name);
            dropdown_product(txt_itemname);
            rp.bindlist_filterdate(dp_filterby);
            rp.dropdown_idtext(txt_searchcategory, "ref_productcategory where pcategsetid = " + rp.get_usersetid(User.Identity.Name) + " and pcategvoid=0 and pcategstatus ='Active' ", "pcategid", "pcategname");
            branch_table("");
        }
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

    public void dropdown_product(DropDownList dplist)
    {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        dplist.Items.Clear();
       // string uomqry = "(select uomname from ref_units where uomid=productunit and uomvoid=0) ";
       // string com = "Select productid,( productname + ' (' + " + uomqry + " + ' x' + CONVERT(VARCHAR(50), prodbaseunit,20) + ')') as [name] from trans_product where producttype='Product' and prodsetid = " + rp.get_usersetid(User.Identity.Name) + " and prodstatus='Active' and prodvoid=0 ";
        string com = "select productid, itemnamedesc as name from vw_product " +
            "where producttype='Product' and prodsetid = " + rp.get_usersetid(User.Identity.Name) + " and prodstatus='Active' and prodvoid=0 ";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        dplist.DataSource = dt;
        dplist.DataBind();
        dplist.DataTextField = "name";
        dplist.DataValueField = "productid";
        dplist.DataBind();

        dplist.Items.Insert(0, new ListItem("Select...", "Select..."));

        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
    }
    protected void btn_add_Click(object sender, EventArgs e)
    {
        try
        {
            if (hd_id.Value == "")
            {
               
                if (stock_add() == 1)
                {
                    rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Inventory added item: " + txt_itemname.Text, rp.get_usersetid(User.Identity.Name).ToString());
                
                    sweetmsg("Successfully added!", "Stock", Sweet_Alert_Type.success);
                    stock_reset();
                   // ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop2", "$('#myModal_search').modal('hide')", true);
                }
            }
            else
            {

                //if (rp.identify_counter(" trans_expenses where expdate = '" + txt_date.Text + "' and expensecateg ='" + txt_expense.SelectedValue + "' and expsetid= " + rp.get_usersetid(User.Identity.Name) + " and expid != " + hd_id.Value + " ") > 0)
                //{
                //    ShowMessage("Already exist!", MessageType.Warning);
                //    txt_itemname.Focus();
                //    return;
                //}
                if (stock_update() == 1)
                {
                    rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Edit", "Inventory updated item: " + txt_itemname.Text, rp.get_usersetid(User.Identity.Name).ToString());
                    sweetmsg("Successfully updated!", "Stock", Sweet_Alert_Type.success);
                    stock_reset();
                }
            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }
    public void stock_reset()
    {
       
        txt_searchkeyword.Text = "";
        dp_filterby.SelectedIndex = 0;
        txt_datefrom.Text = "";
        txt_dateto.Text = "";
        txt_datefrom.Enabled = false;
        txt_dateto.Enabled = false; 
        branch_table("");
        txt_note.Text = "";
        hd_id.Value = "";
        txt_cost.Text = "";
        txt_itemname.SelectedIndex = 0;
        txt_expirydate.Text = "";
        txt_lotnumber.Text = "";
        txt_inventorytype.ClearSelection();
        txt_qty.Text = "";

       // txt_searchkeyword.Focus();
    }
    public int stock_add()
    {
        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("insert into [trans_inventory]  (inproductid,inventorytype,inventoryqty, " +
                "inventorycostperunit,inventorysetid,inventoryentryby,inventoryvoid,inventorydate,inventorynote,invent_expirydate,invent_lotno) values(@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11)", con);
          
            cmd.Parameters.AddWithValue("@d1", txt_itemname.SelectedValue);
            double qty = 0;
            bool isqty= double.TryParse(txt_qty.Text, out qty);
            if (isqty == true && qty < 0)
            {
                cmd.Parameters.AddWithValue("@d2", "Out");
                cmd.Parameters.AddWithValue("@d3", qty);
            }
            else
            {
                cmd.Parameters.AddWithValue("@d2", "In");
                cmd.Parameters.AddWithValue("@d3", txt_qty.Text);
            }
            cmd.Parameters.AddWithValue("@d4", txt_cost.Text);
            cmd.Parameters.AddWithValue("@d5", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d6", rp.get_userid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d7", "0");
            cmd.Parameters.AddWithValue("@d8", pacificdatenow);
            cmd.Parameters.AddWithValue("@d9", txt_note.Text);
            DateTime dtexpiry;
            bool isdate = DateTime.TryParse(txt_expirydate.Text, out dtexpiry);
            if (isdate)
            {
                cmd.Parameters.AddWithValue("@d10", txt_expirydate.Text);
            }
            else
            {
                cmd.Parameters.AddWithValue("@d10", DBNull.Value);

            }
            cmd.Parameters.AddWithValue("@d11", txt_lotnumber.Text);

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
    public int stock_update()
    {

        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("update trans_inventory set inproductid =@d1,inventorytype=@d2,inventoryqty=@d3,inventorycostperunit=@d4, " +
                " inventorynote=@d9, invent_expirydate=@d10, invent_lotno=@d11 where inventoryid = @idss and inventorysetid=@d6 ", con);

            cmd.Parameters.AddWithValue("@d1", txt_itemname.SelectedValue);
            double qty = 0;
            bool isqty = double.TryParse(txt_qty.Text, out qty);
            if (isqty == true && qty < 0)
            {
                cmd.Parameters.AddWithValue("@d2", "Out");
                cmd.Parameters.AddWithValue("@d3", qty);
            }
            else
            {
                cmd.Parameters.AddWithValue("@d2", "In");
                cmd.Parameters.AddWithValue("@d3", txt_qty.Text);
            }
           // cmd.Parameters.AddWithValue("@d2", "In");
           // cmd.Parameters.AddWithValue("@d3", txt_qty.Text);
            cmd.Parameters.AddWithValue("@d4", txt_cost.Text);
            cmd.Parameters.AddWithValue("@d9", txt_note.Text);
            cmd.Parameters.AddWithValue("@d6", rp.get_usersetid(User.Identity.Name));
            DateTime dtexpiry;
            bool isdate= DateTime.TryParse(txt_expirydate.Text, out dtexpiry);
            if (isdate)
            {
                cmd.Parameters.AddWithValue("@d10", txt_expirydate.Text);
            }

            else
            {
                cmd.Parameters.AddWithValue("@d10", DBNull.Value);

            }
            cmd.Parameters.AddWithValue("@d11", txt_lotnumber.Text);
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

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {

        

            //string name = "(select productname from trans_product where trans_product.productid=inproductid)";
            //string baseunit = "(select case when prodbaseunit !=1 then   CONVERT(VARCHAR(50), prodbaseunit,20) else '' end from trans_product where trans_product.productid=inproductid)";
            //string categqry = "(select pcategname from ref_productcategory,trans_product where trans_product.productid=inproductid and pcategid =productcateg)";
            //string unitqry = "(select ref_units.uomname from ref_units,trans_product where trans_product.productid=inproductid and ref_units.uomid = trans_product.productunit)";
            //string sql = "SELECT " + top10 + " trans_inventory.*, ABS(inventoryqty) as [qtys]   " +
            //    ", (SELECT ufullname from  ref_account where uid = inventoryentryby) as [usname] " +
            //    ", " + categqry + " as [category] " +
            //    ", "  + baseunit + " + ' ' + " + unitqry + " as [unit] " +
            //        ", " + name + " as [name] " +
            //    "FROM trans_inventory where (inventoryvoid = 0 and inventorysetid =@setid)  ";
            string sql = "select * from vw_inventory   where (inventoryvoid = 0 and inventorysetid =@setid) ";
            if (!string.IsNullOrEmpty(txt_searchkeyword.Text.Trim()))
            {
                sql += " and ( itemnamedesc  LIKE '%' + @m1 + '%' or invent_lotno  LIKE '%' + @m1 + '%' or inventorytype  LIKE '%' + @m1 + '%' " +
                    " or inventorynote =@m1 " +
                ") ";

                cmd.Parameters.AddWithValue("@m1", txt_searchkeyword.Text.Trim());


            }
            if (txt_searchcategory.SelectedIndex != 0)
            {
                sql += " and  (productcateg =@m2 ) ";
                cmd.Parameters.AddWithValue("@m2", txt_searchcategory.Text);
          
            }
            if (!string.IsNullOrEmpty(txt_datefrom.Text) && !string.IsNullOrEmpty(txt_dateto.Text))
            {


                string from = txt_datefrom.Text;
                string to = txt_dateto.Text;
                string filterdate = " and (invdateonly BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }
         
            sql += " order by inventorydate desc";
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
        txt_note.Text = "";
       
        txt_cost.Text = "";
        txt_itemname.SelectedIndex = 0;
        txt_expirydate.Text = "";
        txt_lotnumber.Text = "";
        txt_inventorytype.ClearSelection();
        txt_qty.Text = "";

        lbl_headeradd.Text = "Edit Stock";

        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
        HiddenField hd_prodid = (HiddenField)item.FindControl("hd_prodid");
        HiddenField hd_qty = (HiddenField)item.FindControl("hd_qty");
        HiddenField hd_costunit = (HiddenField)item.FindControl("hd_costunit");
        HiddenField hd_note = (HiddenField)item.FindControl("hd_note");
        HiddenField hd_expirydate = (HiddenField)item.FindControl("hd_expirydate");
        HiddenField hd_lotno = (HiddenField)item.FindControl("hd_lotno");
        HiddenField hd_inventorytype = (HiddenField)item.FindControl("hd_inventorytype");
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop3", "$('#mymodal_addstock').modal('show')", true);
        hd_id.Value = hd_idselect.Value;
        txt_note.Text = hd_note.Value;
        txt_qty.Text = hd_qty.Value;
        txt_cost.Text = hd_costunit.Value;
        txt_lotnumber.Text = hd_lotno.Value;
        txt_inventorytype.Text = hd_inventorytype.Value;
        DateTime dtexpiry;

        bool isdate =DateTime.TryParse(hd_expirydate.Value, out dtexpiry);
        if (isdate)
        {
            txt_expirydate.Text = dtexpiry.ToString("yyyy-MM-dd");
        }
        txt_itemname.SelectedValue = hd_prodid.Value;

        // txt_c = hd_contact.Value;
    }
    protected void btn_deletebranch_Click(object sender, EventArgs e)
    {
        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
        HiddenField hd_prodname = (HiddenField)item.FindControl("hd_prodname");
        HiddenField hd_prodid = (HiddenField)item.FindControl("hd_prodid");

        string lastid = rp.get_onestringvalue("select max(inventoryid) as lastid from trans_inventory where inventoryvoid=0 and inproductid = " + hd_prodid.Value + " and inventorysetid =" + rp.get_usersetid(User.Identity.Name).ToString() + " group by inproductid");
      // ShowMessage(lastid + " " + hd_idselect.Value, MessageType.Success);
       if (lastid != hd_idselect.Value)
       {
           ShowMessage("Unable to delete stock, already used by another process!", MessageType.Warning);
           return;
       }
        //if (rp.identify_counter(" trans_inventory where inproductid = " + hd_prodid.Value + " and inventorytype ='Out' and inventorysetid =" + rp.get_usersetid(User.Identity.Name).ToString() + " ") > 0)
        //{
        //    ShowMessage("Unable to delete stock, already used by another process!", MessageType.Warning);
        //    return;

        //}
      
           con.Open();
           String cb = "Update [trans_inventory] set inventoryvoid ='1' where inventoryid = " + hd_idselect.Value + " and inventorysetid =" + rp.get_usersetid(User.Identity.Name).ToString() + "";
           cmd = new SqlCommand(cb);
           cmd.Connection = con;

           int result1 = cmd.ExecuteNonQuery();

           con.Close();
           if (result1 >= 1)
           {
               rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Void", "Inventory item void value: " + hd_prodname.Value, rp.get_usersetid(User.Identity.Name).ToString());
               ShowMessage("Successfully deleted!", MessageType.Success);
               branch_table("");
               ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#modalPopUp_Delete').modal('hide')", true);
           }
           con.Close();
       


    }

    protected void gv_masterlist_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // {LinkButton lnk2 = (LinkButton)e.Row.FindControl("LinkButton2");



        //if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        //{
        //    if (edit == 0 && delete == 0 && view == 1)
        //    {
        //        e.Row.Cells[0].Visible = false;//this is your templatefield column.
        //    }
        //    else
        //    {
        //        e.Row.Cells[0].Visible = true;
        //    }


        //}
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    LinkButton lnk2 = (LinkButton)e.Row.FindControl("btn_selectbranch");
        //    LinkButton lnk3 = (LinkButton)e.Row.FindControl("btn_deletebranch");
        //    HiddenField hd_salesid = (HiddenField)e.Row.FindControl("hd_salesid");

        //    if (hd_salesid.Value != "")
        //    {
        //        lnk2.Visible = false;
        //        lnk3.Visible = false;
        //    }
        //    else
        //    {
        //        if (edit == 1)
        //        {
        //            lnk2.Visible = true;
        //        }
        //        else
        //        {
        //            lnk2.Visible = false;
        //        }
        //        if (delete == 1)
        //        {
        //            lnk3.Visible = true;
        //        }
        //        else
        //        {
        //            lnk3.Visible = false;
        //        }

        //    }
        //    //foreach (GridViewRow row in gv_masterlist.Rows)
        //    //{
        //    //    //  string Namecolumnvalue = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "modelname"));

        //    //    Label lblno = (Label)e.Row.FindControl("Label1");

        //    //    int no = Convert.ToInt32(lblno.Text);
        //    //    lblno.Text = (no + 1).ToString();
        //    //}


        //}
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
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop2", "$('#myModal_search').modal('hide')", true);
    }
    protected void btn_search_Click(object sender, EventArgs e)
    {
        dp_filterby.Focus();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModalsearch();", true);


    }
    protected void btn_addshow_Click(object sender, EventArgs e)
    {
        lbl_headeradd.Text = "Add Stock";
        stock_reset();
        txt_itemname.Focus();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop2", "openModaladdstock();", true);


    }
    protected void btn_reset_Click(object sender, EventArgs e)
    {
        dp_filterby.SelectedIndex = 0;
        txt_searchcategory.SelectedIndex = 0;
        txt_datefrom.Enabled = false;
        txt_dateto.Enabled = false;
        txt_searchkeyword.Text = "";
        txt_datefrom.Text = "";
        txt_dateto.Text = "";
        branch_table("");
        //  txt_searchkeyword.Focus();

    }
}