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

public partial class Customer : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "Customer", "padd");
        edit = rp.access_user(User.Identity.Name, "Customer", "pedit");
        delete = rp.access_user(User.Identity.Name, "Customer", "pdelete");
        view = rp.access_user(User.Identity.Name, "Customer", "pview");
        print = rp.access_user(User.Identity.Name, "Customer", "pprint");

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
            rp.dropdown(" name FROM ref_identification WHERE void =0 and setid =" + rp.get_usersetid(User.Identity.Name) + " ", txt_identification);
            rp.dropdown_idtextdefaultzero(txt_affiliation, "ref_affiliation", "id", "name");
            branch_table("");

        }
    }



    private void branch_table(string top10)
    {
        //   try
        // {

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string sql = "SELECT " + top10 + " a.*,(select usname from ref_account where uid = a.centryby) as [usname], b.name as affiliationname FROM trans_customer a " +
                " left join ref_affiliation b on b.id = a.affiliation" +
            
           " where cvoid=0 and csetid = " + rp.get_usersetid(User.Identity.Name) + "  ";
            if (!string.IsNullOrEmpty(txt_searchkeyword.Text.Trim()))
            {
                sql += " and (cfullname LIKE '%' + @m1 + '%' or cemail LIKE '%' + @m2 + '%' or caddress LIKE '%' + @m3 + '%' or refno LIKE '%' + @m3 + '%'  or b.name LIKE '%' + @m3 + '%'  " +
                "or cnote LIKE '%' + @m3 + '%' or cstatus =@m3) ";
                cmd.Parameters.AddWithValue("@m1", txt_searchkeyword.Text.Trim());
                cmd.Parameters.AddWithValue("@m2", txt_searchkeyword.Text.Trim());
                cmd.Parameters.AddWithValue("@m3", txt_searchkeyword.Text.Trim());
            }
            if (!string.IsNullOrEmpty(txt_datefrom.Text) && !string.IsNullOrEmpty(txt_dateto.Text))
            {

                string from = txt_datefrom.Text + " 00:00:00.000";
                string to = txt_dateto.Text + " 23:59:59.999";
                string filterdate = " and (cdatecreated BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }
            sql += "order by cdatecreated desc";
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

        lbl_item.Text = rp.footerinfo_gridview(gv_masterlist).ToString();




        //}
        //catch (Exception ex)
        //{

        //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);

        //        }
    }
    protected void OnPaging(object sender, GridViewPageEventArgs e)
    {
        gv_masterlist.PageIndex = e.NewPageIndex;
        this.branch_table("");
    }
    protected void btn_selectbranch_Click(object sender, EventArgs e)
    {

        txt_customername.Text = "";
        txt_address.Text = "";
        txt_contact.Text = "";
        txt_email.Text = "";
        txt_note.Text = "";
        txt_status.Text = "Active";
        txt_refno.Text = "";
        txt_affiliation.SelectedIndex = 0;
        txt_identification.SelectedIndex = 0;
        txt_customername.Focus();

        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
        HiddenField hd_cname = (HiddenField)item.FindControl("hd_customername");
        HiddenField hd_caddress = (HiddenField)item.FindControl("hd_address");
        HiddenField hd_ccontact = (HiddenField)item.FindControl("hd_contact");
        HiddenField hd_cemail = (HiddenField)item.FindControl("hd_email");
        HiddenField hd_cstatus = (HiddenField)item.FindControl("hd_status");
        HiddenField hd_cnote = (HiddenField)item.FindControl("hd_note");
        HiddenField hd_identity = (HiddenField)item.FindControl("hd_identity");
        HiddenField hd_refno = (HiddenField)item.FindControl("hd_refno");
        HiddenField hd_affiliation = (HiddenField)item.FindControl("hd_affiliation");
        hd_id.Value = hd_idselect.Value;

        txt_customername.Text = hd_cname.Value;
        txt_address.Text = hd_caddress.Value;
        txt_contact.Text = hd_ccontact.Value;
        txt_email.Text = hd_cemail.Value;
        txt_note.Text = hd_cnote.Value;
        txt_status.Text = hd_cstatus.Value;
        if (hd_refno.Value.Length > 0)
        {
            txt_refno.Text = hd_refno.Value;
        }
        if (hd_affiliation.Value.Length > 0)
        {
            txt_affiliation.Text = hd_affiliation.Value;
        }
        if (hd_identity.Value.Length > 0)
        {
            txt_identification.Text = hd_identity.Value;
        }
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
    }
    protected void btn_deletebranch_Click(object sender, EventArgs e)
    {
        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;

        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
        HiddenField hd_cname = (HiddenField)item.FindControl("hd_customername");
        HiddenField hd_caddress = (HiddenField)item.FindControl("hd_address");
        HiddenField hd_ccontact = (HiddenField)item.FindControl("hd_contact");
        HiddenField hd_cemail = (HiddenField)item.FindControl("hd_email");
        HiddenField hd_cstatus = (HiddenField)item.FindControl("hd_status");
        HiddenField hd_cnote = (HiddenField)item.FindControl("hd_note");


        if (rp.identify_counter(" trans_invoice where invoicecustomerid = " + hd_idselect.Value + " and invoicevoid=0 ") > 0)
        {
            ShowMessage("Unable to remove customer already used by another process.", MessageType.Warning);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#modalPopUp_Delete').modal('hide')", true);
            return;
        }

        con.Open();
        String cb = "Update [trans_customer] set cvoid=1 where customerid = " + hd_idselect.Value + "";
        cmd = new SqlCommand(cb);
        cmd.Connection = con;

        int result = cmd.ExecuteNonQuery();
        con.Close();

        if (result >= 1)
        {
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Void", "Customer info deleted value: " + hd_cname.Value, rp.get_usersetid(User.Identity.Name).ToString());
            ShowMessage("Successfully deleted!", MessageType.Success);
            this.branch_table("");

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

   
    protected void btn_additem_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        txt_customername.Focus();
    }
    protected void btn_finish_Click(object sender, EventArgs e)
    {

        if (hd_id.Value == "")
        {
            int exist = rp.identify_counter("trans_customer where cfullname ='" + txt_customername.Text + "' and caddress ='" + txt_address.Text + "' and csetid = " + rp.get_usersetid(User.Identity.Name) + "  ");
            if (exist > 0)
            {

                ShowMessage("Customer name already exist!", MessageType.Warning);
               
                return;
            }
            int modelcastid = modelitem_add();
            if (modelcastid > 0)
            {


                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Customer info added: " + txt_customername.Text, rp.get_usersetid(User.Identity.Name).ToString());
                ShowMessage("Successfully added", MessageType.Success);
                modeladd_reset();

            }
            else
            {
                ShowMessage("Failed to save!", MessageType.Warning);
            }
        }
        else
        {
            if (modelitem_update() == 1)
            {
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Edit", "Customer info updated: " + txt_customername.Text, rp.get_usersetid(User.Identity.Name).ToString());
                ShowMessage("Successfully updated", MessageType.Success);
                modeladd_reset();
            }
            else
            {
                ShowMessage("Failed to update", MessageType.Warning);
            }
        }
    }
    public int modelitem_add()
    {
        int stat = 0;
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        SqlCommand cmd = new SqlCommand("insert into [trans_customer] (cfullname,caddress,ccontact,cemail,csetid,cvoid,cstatus,cdatecreated,cnote,centryby, identityname,affiliation,refno) " +
            " output INSERTED.customerid values(@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9, @d10, @d11,@d12,@d13)", con);
        //DateTime dtnow = DateTime.Now;

        cmd.Parameters.AddWithValue("@d1", txt_customername.Text);
        cmd.Parameters.AddWithValue("@d2", txt_address.Text);
        cmd.Parameters.AddWithValue("@d3", txt_contact.Text);
        cmd.Parameters.AddWithValue("@d4", txt_email.Text);
        cmd.Parameters.AddWithValue("@d5", rp.get_usersetid(User.Identity.Name));
        cmd.Parameters.AddWithValue("@d6", "0");
        cmd.Parameters.AddWithValue("@d7", txt_status.Text);
        cmd.Parameters.AddWithValue("@d8", pacificdatenow.ToString());
        cmd.Parameters.AddWithValue("@d9", txt_note.Text);
        cmd.Parameters.AddWithValue("@d10", rp.get_userid(User.Identity.Name));
        string identityname = "";
        if (txt_identification.SelectedIndex != 0)
        {
            identityname = txt_identification.Text;
        }
        cmd.Parameters.AddWithValue("@d11", identityname);
       
        cmd.Parameters.AddWithValue("@d12", txt_affiliation.Text);
       
        cmd.Parameters.AddWithValue("@d13", txt_refno.Text);
        int modifieduser_id = (int)cmd.ExecuteScalar();

        //int res = cmd.ExecuteNonQuery();
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
    public int modelitem_update()
    {
        int stat = 0;
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        SqlCommand cmd = new SqlCommand("update [trans_customer] set cfullname=@d1,caddress=@d2,ccontact=@d3,cemail=@d4,cstatus=@d5,cnote=@d6, identityname=@d7, affiliation=@d8, refno =@d9 " +
            "  where customerid=@id", con);
        //DateTime dtnow = DateTime.Now;

        cmd.Parameters.AddWithValue("@d1", txt_customername.Text);
        cmd.Parameters.AddWithValue("@d2", txt_address.Text);
        cmd.Parameters.AddWithValue("@d3", txt_contact.Text);
        cmd.Parameters.AddWithValue("@d4", txt_email.Text);

        cmd.Parameters.AddWithValue("@d5", txt_status.Text);

        cmd.Parameters.AddWithValue("@d6", txt_note.Text);
     
        string identityname = "";
        if (txt_identification.SelectedIndex != 0)
        {
            identityname = txt_identification.Text;
        }
        cmd.Parameters.AddWithValue("@d7", identityname);
        cmd.Parameters.AddWithValue("@d8", txt_affiliation.Text);
        cmd.Parameters.AddWithValue("@d9", txt_refno.Text);
        cmd.Parameters.AddWithValue("@id", hd_id.Value);
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
    public void modeladd_reset()
    {

        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#myModal_customer').modal('hide')", true);

        //ScriptManager.RegisterStartupScript(this, this.GetType(), "HidePopup", "$('#myModal_customer').modal('hide')", true);
        this.branch_table("");
        hd_id.Value = "";
        txt_customername.Text = "";
        txt_address.Text = "";
        txt_contact.Text = "";
        txt_email.Text = "";
        txt_note.Text = "";
        txt_datefrom.Text = "";
        txt_dateto.Text = "";
        dp_filterby.SelectedIndex = 0;
        txt_datefrom.Enabled = false;
        txt_dateto.Enabled = false;
        txt_status.Text = "Active";
        txt_refno.Text = "";
        txt_affiliation.SelectedIndex = 0;
        txt_identification.SelectedIndex = 0;
        txt_customername.Focus();

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
        dp_filterby.SelectedIndex = 0;
        txt_datefrom.Enabled = false;
        txt_dateto.Enabled = false;
        txt_searchkeyword.Text = "";
        txt_datefrom.Text = "";
        txt_dateto.Text = "";
        branch_table("");
        //  txt_searchkeyword.Focus();

    }

    #region SOA_RECORD
    protected void btn_crecord_Click(object sender, EventArgs e)
    {
        LinkButton btn_select = (LinkButton)sender;
        GridViewRow item = (GridViewRow)btn_select.NamingContainer;
        HiddenField hd_idselect = (HiddenField)item.FindControl("hd_id");
        HiddenField hd_cname = (HiddenField)item.FindControl("hd_customername");
        lbl_customername.Text = hd_cname.Value;
        ViewState["custid"] = hd_idselect.Value;
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop1", "openModal_record();", true);
        //txt_customername.Focus();
        SOA_RECORD(hd_idselect.Value);
    }
    private void SOA_RECORD(string customerid)
    {
        //   try
        // {

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {

            string sql = "select * from vw_soa where invoicecustomerid=@d1 order by invdateonly desc";
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@d1", customerid);
            cmd.Connection = con;
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);

                gv_soarecord.DataSource = dt;

                gv_soarecord.DataBind();
                 decimal rowSum = 0;
                 foreach (DataRow row in dt.Rows)
                 {
                      string stringValue = row[5].ToString();
                    decimal d;
                    if(decimal.TryParse(stringValue, out d))
                        rowSum += d;
                 }
                 lbl_soatotal.Text = "Total Amount: " + rowSum.ToString("N2");
            }
        }
       


        lbl_soafooter.Text = rp.footerinfo_gridview(gv_soarecord).ToString();


        //}
        //catch (Exception ex)
        //{

        //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);

        //        }
    }
    protected void gv_soarecord_OnPaging(object sender, GridViewPageEventArgs e)
    {
        gv_soarecord.PageIndex = e.NewPageIndex;
        this.SOA_RECORD(ViewState["custid"].ToString());
    }
    #endregion
}