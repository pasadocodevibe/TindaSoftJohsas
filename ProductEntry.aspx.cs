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
public partial class ProductEntry : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    repeatedcode rp = new repeatedcode();
    public int add, edit, delete, view, print;
    public enum MessageType { Success, Error, Info, Warning };
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
        add = rp.access_user(User.Identity.Name, "AddItem", "padd");
        edit = rp.access_user(User.Identity.Name, "AddItem", "pedit");
        delete = rp.access_user(User.Identity.Name, "AddItem", "pdelete");
        view = rp.access_user(User.Identity.Name, "AddItem", "pview");
        print = rp.access_user(User.Identity.Name, "AddItem", "pprint");
        if (add == 0)
        {
            Page.Response.Redirect("Home.aspx");
        }
        if (!IsPostBack)
        {
            int setid = rp.get_usersetid(User.Identity.Name);
            rp.dropdown_idtext(txt_category, "ref_productcategory where pcategsetid = " + setid + " and pcategvoid=0 and pcategstatus ='Active' ", "pcategid", "pcategname");
            rp.dropdown_idtext(txt_unit, "ref_units where uomsetid = " + setid + " and uomvoid=0 and uomstatus ='Active' ", "uomid", "uomname");


            rp.dropdown_idtextrate(txt_discount, "ref_discount where dissettingsid = " + setid + " and disvoid=0 and disstatus ='Active' order by val asc ", "discountid", "discountname + ' @' + CONVERT(VARCHAR(50), discountrate,20) + '%' ");
            rp.dropdown_idtextrate(txt_taxrate, "ref_tax where taxsetid = " + setid + " and taxvoid=0 and taxstatus ='Active' order by val asc ", "taxid", "taxname + ' @' + CONVERT(VARCHAR(50), taxrate,20) + '%' ");


            rp.dropdown_idtext(txt_uom1, "ref_units where uomsetid = " + setid + " and uomvoid=0 and uomstatus ='Active' ", "uomid", "uomname");
            rp.dropdown_idtext(txt_uom2, "ref_units where uomsetid = " + setid + " and uomvoid=0 and uomstatus ='Active' ", "uomid", "uomname");

            rp.dropdown_idtext(txt_brand, "ref_brand where setid = " + setid + " and void=0 and status ='Active' ", "id", "name");
            if (Request.QueryString["id"] != null)
            {

                hd_id.Value = rp.Decrypt(Request.QueryString["id"].ToString());


                if (hd_id.Value != "")
                {
                    read_product();


                }
            }
         
         
        }
    }
    public void read_product()
    {
        con = new SqlConnection(con.ConnectionString);
        con.Open();

        string qry = "select * " +
            "from trans_product where productid =@id and prodvoid =0 and prodsetid=@setid ";
        cmd = new SqlCommand(qry, con);
        cmd.Parameters.AddWithValue("@id", hd_id.Value);
        cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
        rdr = cmd.ExecuteReader();

        if (rdr.Read())
        {
            RadioButtonList1.Text = rdr[2].ToString();
            txt_itemname.Text = rdr[3].ToString();
            txt_unit.SelectedValue = rdr[5].ToString();
            txt_category.SelectedValue = rdr[4].ToString();
            txt_price.Text = rdr[6].ToString();
            txt_taxrate.SelectedValue = rdr[7].ToString();
            txt_discount.SelectedValue = rdr[8].ToString();
            txt_status.Text = rdr[13].ToString();
            txt_barcode.Text = rdr[1].ToString();
            txt_reorderlevel.Text  = rdr[15].ToString();
            hd_baseunit.Value = rdr["prodbaseunit"].ToString();
            chk_has.Visible = false;
            string brand = rdr["prodbrand"].ToString();
            if (brand.Length > 0)
            {
                txt_brand.Text = brand;
            }
            if (RadioButtonList1.Text == "Service")
            {
                reorder.Visible = false;
                txt_reorderlevel.Text = "";

            }
            else
            {
                reorder.Visible = true;
            }

        }
        else
        {
            Page.Response.Redirect("Home.aspx");
        }

        con.Close();
        rdr.Close();
       


    }
    protected void btn_save_Click(object sender, EventArgs e)
    {
        if (hd_id.Value == "")
        {

            if (rp.identify_counter(" trans_product where producttype ='" + RadioButtonList1.Text + "' " +
                "and productunit = " + txt_unit.SelectedValue + " and productcateg = " + txt_category.SelectedValue + 
                " and productname ='" + txt_itemname.Text + "' and prodbarcode = '" + txt_barcode.Text + "' " +
                " and prodsetid = " + rp.get_usersetid(User.Identity.Name) + " and prodvoid =0 ") == 1)
            {
                ShowMessage("Item already exist!", MessageType.Warning);
                txt_itemname.Focus();
                return;
            }

            int addtrue = register_product(txt_unit.SelectedValue, txt_price.Text, "1");
          
            if (addtrue > 0)
            {
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Product added item: " + txt_itemname.Text + "-" + txt_unit.SelectedItem.Text + " = " + txt_price.Text, rp.get_usersetid(User.Identity.Name).ToString());
                int addtrueuom1 = 0;
                int addtrueuom2 = 0;
                if (chk_has.Checked == true)
                {
                    addtrueuom1 = register_product(txt_uom1.SelectedValue, txt_selling1.Text, txt_uomno1.Text);

                    if (txt_uom2.SelectedIndex > 0)
                    {
                        addtrueuom2 = register_product(txt_uom2.SelectedValue, txt_selling2.Text, txt_uomno2.Text);
                    }
                }
                if (addtrueuom1 > 0 || addtrueuom2 > 0)
                {
                    ShowMessage("Successfully added with multiple units!", MessageType.Success);
                }
                else
                {

                    ShowMessage("Successfully added!", MessageType.Success);

                }
                reset();
            }
        }
        else
        {
            

            if (rp.identify_counter(" trans_product where producttype ='" + RadioButtonList1.Text + "' " +
              "and productunit = " + txt_unit.SelectedValue + " and productcateg = " + txt_category.SelectedValue +
              " and productname ='" + txt_itemname.Text + "' and prodbarcode = '" + txt_barcode.Text + "' " +
              " and prodsetid = " + rp.get_usersetid(User.Identity.Name) + " and prodvoid =0   and prodbaseunit = " + hd_baseunit.Value + " and productid != " + hd_id.Value + " ") == 1)
            {
                ShowMessage("Item already exist!", MessageType.Warning);
                txt_itemname.Focus();
                return;
            }

            if (update_item() > 0)
            {
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Edit", "Updated product item: " + txt_itemname.Text, rp.get_usersetid(User.Identity.Name).ToString());
                ShowMessage("Successfully updated", MessageType.Success);
            }
            else
            {
                ShowMessage("You are not authorized!", MessageType.Success);
            }
           
        }
    }




    protected void btn_cancel_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
        {
            Page.Response.Redirect("Productlist.aspx");
        }
        else
        {
            reset();
        }
    }
    public int register_product(string unitvalue, string price, string baseunit)
    {
        int result = 0;
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("insert into [trans_product] (prodbarcode,producttype,productname,productcateg,productunit,prodsellprice,prodtaxid,proddiscountid,proddetails,prodsetid,prodentryby,prodvoid,prodstatus,proddatecreated,prodreorderlevel,prodbaseunit,prodbrand) " +
                " output INSERTED.productid values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11,@d12,@d13,@d14,@d15,@d16,@d17) "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@d1", txt_barcode.Text);
                  cmd.Parameters.AddWithValue("@d2", RadioButtonList1.Text);
                  cmd.Parameters.AddWithValue("@d3", txt_itemname.Text);
                  cmd.Parameters.AddWithValue("@d4", txt_category.SelectedValue);
                  cmd.Parameters.AddWithValue("@d5", unitvalue);
                  cmd.Parameters.AddWithValue("@d6", price);
                  if (txt_taxrate.SelectedIndex != 0)
                  {
                      cmd.Parameters.AddWithValue("@d7", txt_taxrate.SelectedValue);
                  }
                  else
                  {
                      cmd.Parameters.AddWithValue("@d7", DBNull.Value);
                  }
                  if (txt_discount.SelectedIndex != 0)
                  {
                      cmd.Parameters.AddWithValue("@d8", txt_discount.SelectedValue);
                  }
                  else
                  {
                      cmd.Parameters.AddWithValue("@d8", DBNull.Value);
                  }
                  cmd.Parameters.AddWithValue("@d9", "");
                  cmd.Parameters.AddWithValue("@d10", rp.get_usersetid(User.Identity.Name));
                
                cmd.Parameters.AddWithValue("@d11", rp.get_userid(User.Identity.Name));
                  cmd.Parameters.AddWithValue("@d12", "0");
                  cmd.Parameters.AddWithValue("@d13", txt_status.Text);
                  cmd.Parameters.AddWithValue("@d14", pacificdatenow);
                  cmd.Parameters.AddWithValue("@d15", txt_reorderlevel.Text);
                cmd.Parameters.AddWithValue("@d16", baseunit);
                string brand = "0";
                if (txt_brand.SelectedIndex > 0)
                {
                    brand = txt_brand.Text;
                }
                cmd.Parameters.AddWithValue("@d17", brand);
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

    public int update_item()
    {
        int result1 = 0;

        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
           


            using (SqlCommand cmd = new SqlCommand("update [trans_product] set prodbarcode =@d1,producttype =@d2,productname =@d3,productcateg=@d4,productunit=@d5, " +
                "prodsellprice=@d6,prodtaxid =@d7,proddiscountid=@d8,prodstatus=@d10,prodreorderlevel=@d11, prodbrand=@d12 " +
                " where productid=@uid and prodentryby=@prodentryby "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@d1", txt_barcode.Text);
                cmd.Parameters.AddWithValue("@d2", RadioButtonList1.Text);
                cmd.Parameters.AddWithValue("@d3", txt_itemname.Text);
                cmd.Parameters.AddWithValue("@d4", txt_category.SelectedValue);
                cmd.Parameters.AddWithValue("@d5", txt_unit.SelectedValue);
                cmd.Parameters.AddWithValue("@d6", txt_price.Text);
                if (txt_taxrate.SelectedIndex != 0)
                {
                    cmd.Parameters.AddWithValue("@d7", txt_taxrate.SelectedValue);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@d7", DBNull.Value);
                }
                if (txt_discount.SelectedIndex != 0)
                {
                    cmd.Parameters.AddWithValue("@d8", txt_discount.SelectedValue);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@d8", DBNull.Value);
                }
                
                cmd.Parameters.AddWithValue("@d10", txt_status.Text);

                cmd.Parameters.AddWithValue("@d11", txt_reorderlevel.Text);

                string brand = "0";
                if (txt_brand.SelectedIndex > 0)
                {
                    brand = txt_brand.Text;
                }
                cmd.Parameters.AddWithValue("@d12", brand);




                cmd.Parameters.AddWithValue("@prodentryby", rp.get_userid(User.Identity.Name));
                cmd.Parameters.AddWithValue("@uid", hd_id.Value);
              

                cmd.Connection = conn;
                conn.Open();
                result1 = cmd.ExecuteNonQuery();




                conn.Close();
            }

        }
        return result1;
    }

    public void reset()
    {
        txt_addcategname.Text = "";
        txt_adddiscount.Text = "";
        txt_adddiscountrate.Text = "";
        txt_addtax.Text = "";
        txt_addtaxrate.Text = "";
        txt_addunitname.Text = "";

        hd_id.Value = "";
        RadioButtonList1.Text = "Product";
        txt_category.SelectedIndex = 0;
        txt_barcode.Text = "";
        txt_itemname.Text = "";
        txt_discount.SelectedIndex = 0;
        txt_price.Text = "";
        txt_reorderlevel.Text = "";
        txt_status.Text = "Active";
        txt_taxrate.SelectedIndex = 0;
        txt_unit.SelectedIndex = 0;
        txt_uom1.SelectedIndex = 0;
        txt_uom2.SelectedIndex = 0;
        txt_brand.SelectedIndex = 0;
        txt_uomno1.Text = "";
        txt_uomno2.Text = "";
        txt_selling1.Text = "";
        txt_selling2.Text = "";

        chk_has.Visible = true;
        chk_has.Checked = false;
        reorder.Visible = true;
        panel_has.Visible = false;

        txt_itemname.Focus();

    }
    protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RadioButtonList1.SelectedItem.Text == "Service")
        {
            chk_has.Visible = false;
            reorder.Visible = false;
            panel_has.Visible = false;
            chk_has.Checked = false;
        }
        else
        {
            if (hd_id.Value != "")
            {
                chk_has.Visible = false;
              
            }
            else
            {
                chk_has.Visible = true;
             
            
            }
            reorder.Visible = true;
        }
    }
    protected void chk_has_CheckedChanged(object sender, EventArgs e)
    {
        if (chk_has.Checked == true)
        {
            panel_has.Visible = true;
        }
        else
        {
            panel_has.Visible = false;
            txt_uom1.SelectedIndex = 0;
            txt_uom2.SelectedIndex = 0;
            txt_uomno1.Text = "";
            txt_uomno2.Text = "";
            txt_selling1.Text = "";
            txt_selling2.Text = "";
        }
    }
    protected void txt_uom2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (txt_uom2.SelectedIndex > 0)
        {
            RequiredFieldValidator12.Enabled = true;
            RequiredFieldValidator13.Enabled = true;
            RequiredFieldValidator14.Enabled = true;
        }
        else
        {
            txt_uomno2.Text = "";
            txt_selling2.Text = "";
            RequiredFieldValidator12.Enabled = false;
            RequiredFieldValidator13.Enabled = false;
            RequiredFieldValidator14.Enabled = false;
        }
    }
    protected void btn_addcategory_Click(object sender, EventArgs e)
    {

        if (rp.identify_counter(" ref_productcategory where pcategname ='" + txt_addcategname.Text + "' and pcategsetid = " + rp.get_usersetid(User.Identity.Name) + " ") > 0)
        {
            ShowMessage("Item category already exist!", MessageType.Warning);
            txt_addcategname.Focus();
            return;
        }

        if (itemcategory_add() >0)
        {
         
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Product category added: " + txt_addcategname.Text, rp.get_usersetid(User.Identity.Name).ToString());
            ShowMessage("Successfully added!", MessageType.Success);
          
            rp.dropdown_idtext(txt_category, "ref_productcategory where pcategsetid = " + rp.get_usersetid(User.Identity.Name) + " and pcategvoid=0 and pcategstatus ='Active' ", "pcategid", "pcategname");
        
            txt_category.Focus();
        }


       
    }
    public int itemcategory_add()
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

            cmd.Parameters.AddWithValue("@d1", txt_addcategname.Text);
            cmd.Parameters.AddWithValue("@d2", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d3", "0");
            cmd.Parameters.AddWithValue("@d4", "Active");
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
    protected void btn_addunit_Click(object sender, EventArgs e)
    {
        if (rp.identify_counter(" ref_units where uomname ='" + txt_addunitname.Text + "' and uomsetid = " + rp.get_usersetid(User.Identity.Name) + " ") > 0)
        {
            ShowMessage("Already exist!", MessageType.Warning);
            txt_addunitname.Focus();
            return;
        }

        if (unit_add() == 1)
        {
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Units added item: " + txt_addunitname.Text, rp.get_usersetid(User.Identity.Name).ToString());
            ShowMessage("Successfully added!", MessageType.Success);
            string setids = rp.get_usersetid(User.Identity.Name).ToString();
            rp.dropdown_idtext(txt_unit, "ref_units where uomsetid = " + setids + " and uomvoid=0 and uomstatus ='Active' ", "uomid", "uomname");
            rp.dropdown_idtext(txt_uom1, "ref_units where uomsetid = " + setids + " and uomvoid=0 and uomstatus ='Active' ", "uomid", "uomname");
            rp.dropdown_idtext(txt_uom2, "ref_units where uomsetid = " + setids + " and uomvoid=0 and uomstatus ='Active' ", "uomid", "uomname");
            txt_unit.Focus();
        }

    }
    public int unit_add()
    {
        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("insert into [ref_units]  (uomname,uomstatus,uomvoid,uomsetid,uomentryby) values(@d1,@d2,@d3,@d4,@d5)", con);
            //   DateTime dtnow = DateTime.Now;

            cmd.Parameters.AddWithValue("@d1", txt_addunitname.Text);
            cmd.Parameters.AddWithValue("@d2", "Active");
            cmd.Parameters.AddWithValue("@d3", "0");
            cmd.Parameters.AddWithValue("@d4", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d5", rp.get_userid(User.Identity.Name));


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
    protected void btn_addtax_Click(object sender, EventArgs e)
    {
        if (rp.identify_counter(" ref_tax where taxname ='" + txt_addtax.Text + "' and taxsetid = " + rp.get_usersetid(User.Identity.Name) + "  ") > 0)
        {
            ShowMessage("Tax name already exist!", MessageType.Warning);
            txt_addtax.Focus();
            return;
        }

        if (tax_add() == 1)
        {
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Tax added item: " + txt_addtax.Text, rp.get_usersetid(User.Identity.Name).ToString());
            ShowMessage("Successfully added!", MessageType.Success);
            rp.dropdown_idtextrate(txt_taxrate, "ref_tax where taxsetid = " + rp.get_usersetid(User.Identity.Name).ToString() + " and taxvoid=0 and taxstatus ='Active' order by val asc ", "taxid", "taxname + ' @' + CONVERT(VARCHAR(50), taxrate,20) + '%' ");
            txt_taxrate.Focus();
        }



    }
    public int tax_add()
    {
        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("insert into [ref_tax]  (taxname,taxrate,taxvoid,taxstatus,taxsetid,taxdatecreated,taxentryby) values(@d1,@d2,@d3,@d4,@d5,@d6,@d7)", con);
            //   DateTime dtnow = DateTime.Now;

            cmd.Parameters.AddWithValue("@d1", txt_addtax.Text);
            cmd.Parameters.AddWithValue("@d2", txt_addtaxrate.Text);
            cmd.Parameters.AddWithValue("@d3", "0");
            cmd.Parameters.AddWithValue("@d4", txt_status.Text);
            cmd.Parameters.AddWithValue("@d5", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d6", pacificdatenow);
            cmd.Parameters.AddWithValue("@d7", rp.get_userid(User.Identity.Name));

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
    protected void btn_adddiscount_Click(object sender, EventArgs e)
    {
        if (rp.identify_counter(" ref_discount where discountname ='" + txt_adddiscount.Text + "' and dissettingsid = " + rp.get_usersetid(User.Identity.Name) + " ") > 0)
        {
            ShowMessage("Discount name already exist!", MessageType.Warning);
            txt_adddiscount.Focus();
            return;
        }

        if (discount_add() == 1)
        {
            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Discount added item: " + txt_adddiscount.Text, rp.get_usersetid(User.Identity.Name).ToString());
            ShowMessage("Successfully added!", MessageType.Success);
            rp.dropdown_idtextrate(txt_discount, "ref_discount where dissettingsid = " + rp.get_usersetid(User.Identity.Name) + " and disvoid=0 and disstatus ='Active' order by val asc ", "discountid", "discountname + ' @' + CONVERT(VARCHAR(50), discountrate,20) + '%' ");
            txt_adddiscount.Focus();
        }
    }
    public int discount_add()
    {
        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("insert into [ref_discount]  (discountname,discountrate,disvoid,disstatus,dissettingsid,discountdate,disentryby) values(@d1,@d2,@d3,@d4,@d5,@d6,@d7)", con);
            //   DateTime dtnow = DateTime.Now;

            cmd.Parameters.AddWithValue("@d1", txt_adddiscount.Text);
            cmd.Parameters.AddWithValue("@d2", txt_adddiscountrate.Text);
            cmd.Parameters.AddWithValue("@d3", "0");
            cmd.Parameters.AddWithValue("@d4", "Active");
            cmd.Parameters.AddWithValue("@d5", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d6", pacificdatenow.ToShortDateString());
            cmd.Parameters.AddWithValue("@d7", rp.get_userid(User.Identity.Name));

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
}