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

public partial class pos1 : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    repeatedcode rp = new repeatedcode();
    public int add, edit, delete, view, print;
    public enum MessageType { Success, Error, Info, Warning };
    public static String sid;
    public static String autocomid;
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
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
     //   ShowMessage(get_cartcost("17").ToString(), MessageType.Success);
           add = rp.access_user(User.Identity.Name, "pos1", "padd");
           edit = rp.access_user(User.Identity.Name, "pos1", "pedit");
           delete = rp.access_user(User.Identity.Name, "pos1", "pdelete");
           view = rp.access_user(User.Identity.Name, "pos1", "pview");
           print = rp.access_user(User.Identity.Name, "pos1", "pprint");
        
        if (add == 0)
        {
            Page.Response.Redirect("Home.aspx");
        }
        
        if (!IsPostBack)
        {
            sid = rp.get_usersetid(User.Identity.Name).ToString();
            txt_searchitem.Focus();
            rp.dropdown(" name FROM ref_identification WHERE void =0 and setid =" + rp.get_usersetid(User.Identity.Name) + " ", txt_identification);
            rp.dropdown_idtextdefaultzero(txt_affiliation, "ref_affiliation", "id", "name");
            if (Request.QueryString["id"] != null)
            {

                hd_id.Value = rp.Decrypt(Request.QueryString["id"].ToString());


                if (hd_id.Value != "")
                {
                    read_invoice();
                    read_invoicecart();

                }
            }
            else
            {
                initial_cart();

                GridView1.DataSource = null;
                GridView1.DataBind();
            }

        }
      
    }
    public void read_invoice()
        {
            con = new SqlConnection(con.ConnectionString);
            con.Open();

            string subqryfname = " (select  convert(varchar(50),customerid) + ' | ' + cfullname from trans_customer where customerid= invoicecustomerid) as [customer] ";
            string qry = "select *, " + subqryfname + " " +
                "from trans_invoice where invoiceid =@id and invoicevoid =0 and invoicesetid=@setid ";
            cmd = new SqlCommand(qry, con);
            cmd.Parameters.AddWithValue("@id", hd_id.Value);
            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                txt_invoiceno.Text = rdr[1].ToString();
                txt_salesnote.Text = rdr["invoicenote"].ToString();
               txt_customer.Text = rdr["customer"].ToString();
               lbl_subtot.Text = Convert.ToDouble( rdr["invoicesubtotal"]).ToString("N2");
               lbl_discount.Text = Convert.ToDouble(rdr["invoicediscountamt"]).ToString("N2");
               lbl_tax.Text = Convert.ToDouble(rdr["invoicetax"]).ToString("N2");
               lbl_total.Text = Convert.ToDouble(rdr["invoicetotal"]).ToString("N2");
               lbl_totreceive.Text = Convert.ToDouble(rdr["invoicetotal"]).ToString("N0");
               lbl_cashround.Text = Convert.ToDouble(rdr["invoicecashround"]).ToString("N2");
               txt_amounttendered.Text = rdr["invoinceamounttendered"].ToString();
               lbl_change.Text = Convert.ToDouble(rdr["invoicechanged"]).ToString("N2");


            }
            else
            {
                Page.Response.Redirect("Home.aspx");
            }

            con.Close();
            rdr.Close();
        }
    public void read_invoicecart()
    {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con = new SqlConnection(con.ConnectionString);
        con.Open();


        //string uomcase = " case when prodbaseunit !=1 then  'of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else '' end as [extension]  ";
        //string uomqry = "(select uomname + ' ' + "  + uomcase + "  from ref_units,trans_product where productid=cartproductid and ref_units.uomid=trans_product.productunit and uomvoid=0) as [uomname] ";
      


        //string qry = "select cartid,cartproductid as[id], cartprice as [price], cartqty as [qty],cartamount as [Amount], " +
           
        //    " (Select productname + ' ' + " +  uomqry + " from trans_product where productid=cartproductid)  as [itemname]" +
        //    ", cartdatecreated from trans_invoicecart where cartinvoiceid =@id and cartvoid =0 and cartsetid=@setid ";
        string qry = "select * from vw_invoicecart where cartinvoiceid =@id and cartvoid =0 and cartsetid=@setid ";
        cmd = new SqlCommand(qry, con);
        cmd.Parameters.AddWithValue("@id", hd_id.Value);
        cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
      //  rdr = cmd.ExecuteReader();

        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        {
            DataTable dt = new DataTable();
            sda.Fill(dt);
            ViewState["dt"] = dt;

            GridView1.DataSource = dt;
            GridView1.DataBind();
            lbl_cartcount.Text = "<div class='badge badge-rounded bg-green'> " + GridView1.Rows.Count.ToString() + " cart" + " </div> ";
        }

        con.Close();
        rdr.Close();
    }
    private void initial_cart()
    {
        //creating DataTable  
        DataTable dt = new DataTable();
        DataRow dr;
        dt.TableName = "dt";

        //creating columns for DataTable  
        dt.Columns.Add("cartid", typeof(string));
        dt.Columns.Add("id", typeof(string));
        dt.Columns.Add("itemname", typeof(string));
        dt.Columns.Add("price", typeof(string));
        dt.Columns.Add("qty", typeof(string));
        dt.Columns.Add("amount", typeof(string));
        dt.Columns.Add("cartdatecreated", typeof(string));
       // dr = dt.NewRow();
        //dt.Rows.Add(dr);

        ViewState["dt"] = dt;
        GridView1.DataSource = dt;
        GridView1.DataBind();

     
    }
    private void add_new_row(string id, string itemname, string price,string qty,string amount)
    {
        if (ViewState["dt"] != null)
        {
          
            DataTable dtCurrentTable = (DataTable)ViewState["dt"];
            DataRow drCurrentRow = null;

            DataRow[] rows = null;

            if (dtCurrentTable.Rows.Count >= 0)
            {
               

                     for (int i = 0; i <= dtCurrentTable.Rows.Count; i++)
                     {

                    
                         //Creating new row and assigning values  

                         drCurrentRow = dtCurrentTable.NewRow();
                         drCurrentRow["cartid"] = "0";
                         drCurrentRow["id"] = id;
                         drCurrentRow["itemname"] = itemname;
                         drCurrentRow["price"] = price;
                         drCurrentRow["qty"] = qty;
                         drCurrentRow["amount"] = amount;
                         drCurrentRow["cartdatecreated"] = pacificdatenow.ToString();
                     }

                     //Identify exist Record to the DataTable  
                     string prodid = rp.textgetstringbefore(txt_searchitem);
                     rows = dtCurrentTable.Select("id='" + prodid + "'");
                     if (rows.Length > 0)
                     {
                         //update existing Record to the DataTable  
                         foreach (DataRow row in rows)
                         {
                             double addedqty = Convert.ToDouble( row["qty"]) + Convert.ToDouble( txt_itemqty.Text);
                             row["qty"] = addedqty.ToString();
                             double priceexist = Convert.ToDouble(row["price"]);
                             double changeamount = priceexist * addedqty;

                             row["amount"] = changeamount.ToString("N2");

                         }
                        
                     }

                     else
                     {
                         //Added New Record to the DataTable  
                     dtCurrentTable.Rows.Add(drCurrentRow);
                     }
                     //storing DataTable to ViewState  
                     ViewState["dt"] = dtCurrentTable;
                     //binding Gridview with New Row  
                     GridView1.DataSource = dtCurrentTable;
                     GridView1.DataBind();
                     lbl_cartcount.Text = "<div class='badge badge-rounded bg-green'> " + GridView1.Rows.Count.ToString() + " cart" + " </div> ";
                 
                 
            }
            else
            {
                ShowMessage("WARNING", MessageType.Success);
                lbl_cartcount.Text = "";
            }
          


        }
        else
        {
            ShowMessage("ViewState is null", MessageType.Warning);
            
        }
    }


    protected void BindGrid()
    {
        GridView1.DataSource = ViewState["dt"] as DataTable;
        GridView1.DataBind();

        lbl_cartcount.Text = "<div class='badge badge-rounded bg-green'> " + GridView1.Rows.Count.ToString() + " cart" + " </div> ";
        compute_subtotal(); 

      
    }
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> Searchcname(string prefixText, int count)
    {
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager
                    .ConnectionStrings["myconnection"].ConnectionString;
            using (SqlCommand cmd = new SqlCommand())
            {
             
                string uomqry = "(select uomname from ref_units where uomid=productunit and uomvoid=0) as [uomname] ";
                string uomcase = " case when prodbaseunit !=1 then  'of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else '' end as [extension] ";
                cmd.CommandText = "select productname, prodsellprice, productid, " + uomqry + ", " + uomcase + ", b.name as productbrand from trans_product a " +
                   " left join ref_brand b on b.id = a.prodbrand " +
                    "where prodsetid = " + sid + " and (prodstatus ='Active' and prodvoid =0) and( " +
                " productname like '%' + @SearchText + '%' or prodbarcode=@SearchText )";
                cmd.Parameters.AddWithValue("@SearchText", prefixText);

                cmd.Connection = conn;
                conn.Open();
                List<string> customerss = new List<string>();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        string brand = sdr["productbrand"].ToString();
                        string qrybrandshow = "";
                        if (brand.Length > 0)
                        {
                            qrybrandshow = " [" + sdr["productbrand"].ToString() + "]";
                        }
                        customerss.Add(sdr["productid"].ToString() + " | " + sdr["productname"].ToString() + " [" + sdr["uomname"].ToString() + "] " + sdr["extension"].ToString() + qrybrandshow + " @" + Convert.ToDouble(sdr["prodsellprice"]).ToString("N2"));
                    }
                }
                conn.Close();
                return customerss;

            }
        }
    }
   

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> Searchname(string prefixText, int count)
    {
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager
                    .ConnectionStrings["myconnection"].ConnectionString;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "select cfullname, customerid from trans_customer where csetid = " + sid + " and (cstatus ='Active' and cvoid =0) and( " +
                "cfullname like '%' + @SearchText + '%' )";
                cmd.Parameters.AddWithValue("@SearchText", prefixText);

                cmd.Connection = conn;
                conn.Open();
                List<string> customers = new List<string>();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        customers.Add(sdr["customerid"].ToString() + " | " + sdr["cfullname"].ToString());
                    }
                }
                conn.Close();
                return customers;

            }
        }
    }
     
    protected void btn_addc_Click(object sender, EventArgs e)
    {
        int exist = rp.identify_counter("trans_customer where cfullname ='" + txt_addcustomer.Text + "' and caddress ='" + txt_addaddress.Text + "' and csetid = " + rp.get_usersetid(User.Identity.Name) + "  ");
        if (exist > 0)
        {
        
            sweetmsg("Customer name already exist!", "", Sweet_Alert_Type.warning);
            return;
        }
        int modelcastid = modelitem_add();
        if (modelcastid > 0)
        {


            rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Customer info added: " + txt_addcustomer.Text, rp.get_usersetid(User.Identity.Name).ToString());
          
            sweetmsg("Successfully added", "Customer", Sweet_Alert_Type.success);
            txt_customer.Text = modelcastid.ToString() + " | " + txt_addcustomer.Text;
            txt_salesnote.Text = txt_refno.Text;
            modeladd_reset();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop2", "$('#modalpopup_customer').modal('hide')", true);

        }
        else
        {
            sweetmsg("Failed to add customer!", "Customer", Sweet_Alert_Type.warning);
        
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
        SqlCommand cmd = new SqlCommand("insert into [trans_customer] (cfullname,caddress,ccontact,cemail,csetid,cvoid,cstatus,cdatecreated,cnote,centryby,identityname,affiliation,refno) " +
            " output INSERTED.customerid values(@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9, @d10,@d11,@d12,@d13)", con);
        //DateTime dtnow = DateTime.Now;

        cmd.Parameters.AddWithValue("@d1", txt_addcustomer.Text);
        cmd.Parameters.AddWithValue("@d2", txt_addaddress.Text);
        cmd.Parameters.AddWithValue("@d3", txt_contact.Text);
        cmd.Parameters.AddWithValue("@d4", txt_email.Text);
        cmd.Parameters.AddWithValue("@d5", rp.get_usersetid(User.Identity.Name));
        cmd.Parameters.AddWithValue("@d6", "0");
        cmd.Parameters.AddWithValue("@d7", "Active");
        cmd.Parameters.AddWithValue("@d8", pacificdatenow.ToString());
        cmd.Parameters.AddWithValue("@d9", txt_note.Text);
        cmd.Parameters.AddWithValue("@d10", rp.get_userid(User.Identity.Name));
       // cmd.Parameters.AddWithValue("@d11", txt_identification.Text);
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
    
    public void modeladd_reset()
    {


        txt_addcustomer.Text = "";
        txt_refno.Text = "";
        txt_identification.SelectedIndex = 0;
        txt_affiliation.SelectedIndex = 0;
        txt_addaddress.Text = "";
        txt_contact.Text = "";
        txt_email.Text = "";
        txt_note.Text = "";

        txt_addcustomer.Focus();

    }
    public bool validate_amountreceived()
    {
        bool stat= false;
        double total = 0;
        double.TryParse(lbl_total.Text, out total);
        double tendered = 0;
        double.TryParse(txt_amounttendered.Text, out tendered);
        if (tendered < total)
        {
            stat = true;
            
        }

        return stat;
    }
    protected void btn_finish_Click(object sender, EventArgs e)
    {
        Page.Response.Redirect("~/pos1.aspx");
    }
    public bool validate_remainingstockadd()
    {
        bool stat = false;
        string warningmsg = "";
     
      //  bool stockempty = false;
        int count = 0;
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            double stockrmen = 0;
            double stockremaining = 0;
            HiddenField productid = (HiddenField)GridView1.Rows[i].FindControl("hd_id");
            HiddenField hd_itemname = (HiddenField)GridView1.Rows[i].FindControl("hd_itemname");
            TextBox txt_qtygrid = (TextBox)GridView1.Rows[i].FindControl("txt_qty");
            HiddenField hd_cart = (HiddenField)GridView1.Rows[i].FindControl("hd_cart");

         
                stockremaining = check_stockavailability(productid.Value, "");
                stockrmen = stockremaining;
          
            string producttype = identify_producttype(productid.Value);
            double qtyorder = Convert.ToDouble(txt_qtygrid.Text);
            if ((qtyorder > stockremaining || stockremaining <= 0) && producttype == "Product")
            {
                count++;
                //stockempty = true;
                warningmsg += hd_itemname.Value + " (remaining: " + stockremaining.ToString("N0")  + "), ";

            }
          
            else
            {
              ///  stockempty = false;
         
            }

        }
        if (count >= 1)
        {

            string tobewarning = warningmsg;

            if (tobewarning.Length > 2)
            {
                tobewarning = tobewarning.Remove(tobewarning.Length - 2);


            }
            sweetmsg("There is not enough " + tobewarning.ToString() + " in stock", "Inventory", Sweet_Alert_Type.success);
           // ShowMessage("There is not enough " + tobewarning.ToString() + " in stock", MessageType.Warning);
            stat = true;
        }



        return stat;

    }
    public bool validate_remainingstockupdate()
    {
        bool stat = false;
        string warningmsg = "";

        //  bool stockempty = false;
        int count = 0;
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            double stockrmen = 0;
            double stockremaining = 0;
            HiddenField productid = (HiddenField)GridView1.Rows[i].FindControl("hd_id");
            HiddenField hd_itemname = (HiddenField)GridView1.Rows[i].FindControl("hd_itemname");
            TextBox txt_qtygrid = (TextBox)GridView1.Rows[i].FindControl("txt_qty");
            HiddenField hd_cart = (HiddenField)GridView1.Rows[i].FindControl("hd_cart");

            stockremaining = check_stockavailability(productid.Value, " and (invent_soldcartid !=" + hd_cart.Value + " or invent_soldcartid is null)");
         //   stockremaining = check_stockavailability(productid.Value, "");
            stockrmen = stockremaining;

            string producttype = identify_producttype(productid.Value);
            double qtyorder = Convert.ToDouble(txt_qtygrid.Text);
            if ((qtyorder > stockremaining || stockremaining <= 0) && producttype == "Product")
            {
                count++;
                //stockempty = true;
                warningmsg += hd_itemname.Value + " (remaining: " + stockremaining.ToString("N0") + "), ";

            }

            else
            {
                ///  stockempty = false;

            }

        }
        if (count >= 1)
        {

            string tobewarning = warningmsg;

            if (tobewarning.Length > 2)
            {
                tobewarning = tobewarning.Remove(tobewarning.Length - 2);


            }
            sweetmsg("There is not enough " + tobewarning.ToString() + " in stock", "Inventory", Sweet_Alert_Type.warning);
           
            stat = true;
        }



        return stat;

    }
    public int getcustomerid()
    {

        int getcustomerid = 0;
        try
        {
         
            getcustomerid = Convert.ToInt32(rp.textgetstringbefore(txt_customer).ToString());
        }
        catch (Exception ex)
        {
            getcustomerid = 0;
        }
        return getcustomerid;
    }
    protected void btn_completesale_Click(object sender, EventArgs e)
    {

        if (hd_id.Value == "")
        {
            if (GridView1.Rows.Count <= 0)
            {
               // ShowMessage("Unable to save transaction, please add cart item!", MessageType.Warning);
                sweetmsg("Unable to save transaction, please add cart item!", "Warning", Sweet_Alert_Type.warning);
                return;
            }
            int customerid = getcustomerid();
            if (customerid <= 0 && txt_customer.Text !="")
            {
              //  ShowMessage("Invalid customer record!", MessageType.Warning);
                sweetmsg("Invalid customer record!", "Warning", Sweet_Alert_Type.warning);
                txt_customer.Text = "";
                txt_customer.Focus();
                return;
            }
            if (txt_invoiceno.Text.Trim().Length ==0)
            {
                sweetmsg("Please input invoice/receipt number", "Warning", Sweet_Alert_Type.warning);

                txt_invoiceno.Focus();
                return;
            }

            if (rp.identify_counter(" trans_customer where  customerid = " + customerid.ToString() + "  and LEN(identityname) >0  ") > 0 && txt_salesnote.Text.Trim().Length == 0)
            {
                sweetmsg("Please input control number", "Warning", Sweet_Alert_Type.warning);

                txt_salesnote.Focus();
                return;
            }

            if (rp.identify_counter(" trans_customer where customerid = " + customerid.ToString() + " " +
                " and csetid = " + rp.get_usersetid(User.Identity.Name) + " and cvoid =0 and cstatus='Active' ") <= 0 && txt_customer.Text != "")
            {
               // ShowMessage("Customer record not found!", MessageType.Warning);
                sweetmsg("Customer record not found!", "Warning", Sweet_Alert_Type.warning);
                txt_customer.Text = "";
                txt_customer.Focus();
                return;
            }
            if (rp.identify_counter(" trans_invoice where invoiceno = '" + txt_invoiceno.Text + "' " +
                " and invoicesetid = " + rp.get_usersetid(User.Identity.Name) + " and invoicevoid =0 ") == 1 && txt_invoiceno.Text.Trim().Length >0)
            {
             //   ShowMessage("Invoice number already exist!", MessageType.Warning);
                sweetmsg("Invoice number already exist!", "Warning", Sweet_Alert_Type.warning);
                btn_completesale.Focus();
                return;
            }
            //if (validate_amountreceived() == true)
            //{
            //    ShowMessage("The received amount cannot be less than the total balance", MessageType.Warning);
            //    txt_amounttendered.Focus();
            //    return;
            //}

            if (validate_remainingstockadd() == true)
            {
                return;
            }




            int addsalesid = add_sale();
            if (addsalesid > 0)
            {
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Sales added: " + txt_invoiceno.Text + ", customer: " + txt_customer.Text + ", total: " + lbl_total.Text, rp.get_usersetid(User.Identity.Name).ToString());

                int cart = add_cartmultiple(addsalesid.ToString());

                if (cart > 0)
                {
                 //   ShowMessage("Transaction completed.", MessageType.Success);
                    sweetmsg("Transaction completed", "Success", Sweet_Alert_Type.success);
                    generatereceipt();
                 
                   // ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

               
                    //reset_sale();
                }

            }

        }
        else
        {
            int customerid = getcustomerid();
            if (customerid <= 0 && txt_customer.Text != "")
            {
                //  ShowMessage("Invalid customer record!", MessageType.Warning);
                sweetmsg("Invalid customer record!", "Warning", Sweet_Alert_Type.warning);
                txt_customer.Text = "";
                txt_customer.Focus();
                return;
            }
            if (txt_invoiceno.Text.Trim().Length == 0)
            {
                sweetmsg("Please input invoice/receipt number", "Warning", Sweet_Alert_Type.warning);

                txt_invoiceno.Focus();
                return;
            }

            if (rp.identify_counter(" trans_customer where  customerid = " + customerid.ToString() + "  and LEN(identityname) >0 ") > 0 && txt_salesnote.Text.Trim().Length == 0)
            {
                sweetmsg("Please input control number", "Warning", Sweet_Alert_Type.warning);

                txt_salesnote.Focus();
                return;
            }

            if (rp.identify_counter(" trans_customer where customerid = " + customerid.ToString() + " " +
                " and csetid = " + rp.get_usersetid(User.Identity.Name) + " and cvoid =0 and cstatus='Active' ") <= 0 && txt_customer.Text != "")
            {
                // ShowMessage("Customer record not found!", MessageType.Warning);
                sweetmsg("Customer record not found!", "Warning", Sweet_Alert_Type.warning);
                txt_customer.Text = "";
                txt_customer.Focus();
                return;
            }
            if (rp.identify_counter(" trans_invoice where invoiceno = '" + txt_invoiceno.Text + "' " +
             " and invoicesetid = " + rp.get_usersetid(User.Identity.Name) + " and invoicevoid =0 and invoiceid != " + hd_id.Value + " ") == 1 && txt_invoiceno.Text.Trim().Length > 0)
            {
             
                sweetmsg("Invoice number already exist!", "Warning", Sweet_Alert_Type.warning);
                btn_completesale.Focus();
                return;
            }
            //if (validate_amountreceived() == true)
            //{
            //    ShowMessage("The received amount cannot be less than the total balance", MessageType.Warning);
            //    txt_amounttendered.Focus();
            //    return;
            //}
            if (validate_remainingstockupdate() == true)
            {
                return;
            }

            int updatesales = update_sale();
            if (updatesales > 0)
            {
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Update", "Sales Updated: " + txt_invoiceno.Text + ", customer: " + txt_customer.Text + ", total: " + lbl_total.Text, rp.get_usersetid(User.Identity.Name).ToString());

                int cart = update_cartmultiple();

                if (cart > 0)
                {
                   

                    if (hd_tobedeleted.Value != "" )
                    {
                        string tobedeleteid = hd_tobedeleted.Value;
            
                        if (tobedeleteid.Length > 1 )
                        {
                            tobedeleteid = tobedeleteid.Remove(tobedeleteid.Length - 1);
                        

                        }
                        String a = "cartid in (" + tobedeleteid.ToString() + ") ";
                        String inventortout = "invent_soldcartid in (" + tobedeleteid.ToString() + ") ";
                        int statdelete = tobe_deleted(a);
                        int statdeleteinvent = tobe_deletedinvent(inventortout);
                        if (statdelete == 0 || statdeleteinvent ==0)
                        {
                            resetcart();
                       
                            sweetmsg("Update Transaction completed.", "Success", Sweet_Alert_Type.success);
                        }
                        else
                        {

                            resetcart();
                     //       ShowMessage("Update completed with deleted items.", MessageType.Success);
                            sweetmsg("Update completed with deleted items..", "Success", Sweet_Alert_Type.success);
                        
                        }
                    }
                    else
                    {
                        resetcart();
                     //   ShowMessage("Update Transaction completed.", MessageType.Success);
                        sweetmsg("Update Transaction completed.", "Success", Sweet_Alert_Type.success);
                    }
                    //  reset_sale();

                }


            }

        }
    
       
    }
    public void resetcart()
    {
        if (Request.QueryString["id"] != null)
        {
            hd_tobedeleted.Value = "";
            hd_id.Value = rp.Decrypt(Request.QueryString["id"].ToString());

            if (hd_id.Value != "")
            {
                read_invoice();
                read_invoicecart();

            }
        }
    }
    public double value_render(Label txt)
    {
        double val = 0;
        try
        {
            val = Convert.ToDouble(txt.Text);
        }
        catch (Exception)
        { 
            val = 0; 
        
        }
        return val;
    }
    public int add_sale()
    {
        int result = 0;
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("insert into [trans_invoice] (invoiceno,invoicecustomer,invoicecustomerid,invoicetype,invoicesubtotal," +
                "invoicediscountamt,invoicetax,invoicetotal,invoicecashround,invoinceamounttendered,invoicechanged,invoicedate," +
                "invoicesetid,invoicentryby,invoicevoid,invoicenote) " +
                " output INSERTED.invoiceid values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11,@d12,@d13,@d14,@d15,@d16) "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@d1", txt_invoiceno.Text);
                if (txt_customer.Text.Length > 0)
                {
                string id = rp.textgetstringbefore(txt_customer).ToString();

                cmd.Parameters.AddWithValue("@d2", rp.get_onestringvalue("select cfullname from trans_customer where customerid = " + id +" "));

                    cmd.Parameters.AddWithValue("@d3", id);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@d2", "Walk-in");

                    cmd.Parameters.AddWithValue("@d3", "0");
                }
                cmd.Parameters.AddWithValue("@d4", "Walkin");

                cmd.Parameters.AddWithValue("@d5", value_render(lbl_subtot));
                cmd.Parameters.AddWithValue("@d6", value_render(lbl_discount));

                cmd.Parameters.AddWithValue("@d7", value_render(lbl_tax));

                cmd.Parameters.AddWithValue("@d8", value_render(lbl_total));

                cmd.Parameters.AddWithValue("@d9", value_render(lbl_cashround));
                cmd.Parameters.AddWithValue("@d10", Convert.ToDouble(txt_amounttendered.Text));

                cmd.Parameters.AddWithValue("@d11", value_render(lbl_change));
                cmd.Parameters.AddWithValue("@d12", pacificdatenow);
                cmd.Parameters.AddWithValue("@d13", rp.get_usersetid(User.Identity.Name));
                cmd.Parameters.AddWithValue("@d14", rp.get_userid(User.Identity.Name));
                cmd.Parameters.AddWithValue("@d15", "0");
                cmd.Parameters.AddWithValue("@d16", txt_salesnote.Text);
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
    public int update_sale()
    {
        int result = 0;
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("update [trans_invoice] set invoiceno=@d1,invoicecustomer=@d2,invoicecustomerid =@d3" +
                ",invoicesubtotal =@d5,invoicediscountamt=@d6,invoicetax=@d7,invoicetotal=@d8,invoicecashround=@d9, " +
                "invoinceamounttendered=@d10,invoicechanged=@d11," +
                "invoicenote=@d16 where invoiceid=@id and invoicesetid =@setid"))
            {
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id", hd_id.Value);
                cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));

                cmd.Parameters.AddWithValue("@d1", txt_invoiceno.Text);
             
                if (txt_customer.Text.Length > 0)
                {
                    string id = rp.textgetstringbefore(txt_customer).ToString();

                    cmd.Parameters.AddWithValue("@d2", rp.get_onestringvalue("select cfullname from trans_customer where customerid = " + id + " "));

                    cmd.Parameters.AddWithValue("@d3", id);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@d2", "Walk-in");

                    cmd.Parameters.AddWithValue("@d3", "0");
                }
            
                cmd.Parameters.AddWithValue("@d5", value_render(lbl_subtot));
                cmd.Parameters.AddWithValue("@d6", value_render(lbl_discount));

                cmd.Parameters.AddWithValue("@d7", value_render(lbl_tax));

                cmd.Parameters.AddWithValue("@d8", value_render(lbl_total));

                cmd.Parameters.AddWithValue("@d9", value_render(lbl_cashround));
                cmd.Parameters.AddWithValue("@d10", Convert.ToDouble(txt_amounttendered.Text));

                cmd.Parameters.AddWithValue("@d11", value_render(lbl_change));
         
                cmd.Parameters.AddWithValue("@d16", txt_salesnote.Text);
                cmd.Connection = conn;
                conn.Open();
                int s = cmd.ExecuteNonQuery();
                if (s > 0)
                {
                    result = s;

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
    public int add_cart(string invoiceid,string productid,string cartprice,string cartqty,string cartamnt)
    {
        int result = 0;
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("insert into [trans_invoicecart] (cartinvoiceid,cartproductid," +
                "cartprice,cartqty,cartvoid,cartdatecreated,cartstatus,cartnote,cartsetid,cartentryby,cartamount) " +
                " output INSERTED.cartid values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11) "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@d1", invoiceid);
               
                cmd.Parameters.AddWithValue("@d2", productid);
                cmd.Parameters.AddWithValue("@d3", Convert.ToDouble(cartprice));
                cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(cartqty));
                cmd.Parameters.AddWithValue("@d11", Convert.ToDouble(cartamnt));
                cmd.Parameters.AddWithValue("@d5", "0");

                cmd.Parameters.AddWithValue("@d6", pacificdatenow);

                cmd.Parameters.AddWithValue("@d7", "Active");
                cmd.Parameters.AddWithValue("@d8", "");

                cmd.Parameters.AddWithValue("@d9", rp.get_usersetid(User.Identity.Name));
                cmd.Parameters.AddWithValue("@d10", rp.get_userid(User.Identity.Name));
             
                cmd.Connection = conn;
                conn.Open();
                int modifieduser_id = (int)cmd.ExecuteScalar();
                result = modifieduser_id;
             

                conn.Close();
            }

        }
        return result;
    }
    public int update_cart(string cartid, string cartprice, string cartqty, string cartamnt)
    {
        int result = 0;
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("update [trans_invoicecart] set " +
                "cartprice=@d3,cartqty=@d4,cartamount=@d11 where cartid =@id and cartinvoiceid=@invoiceid "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@invoiceid", hd_id.Value);
                cmd.Parameters.AddWithValue("@id", cartid);
                cmd.Parameters.AddWithValue("@d3", Convert.ToDouble(cartprice));
                cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(cartqty));
                cmd.Parameters.AddWithValue("@d11", Convert.ToDouble(cartamnt));
       

                cmd.Connection = conn;
                conn.Open();
                result = cmd.ExecuteNonQuery();



                conn.Close();
            }

        }
        return result;
    }
    public int update_cartmultiple()    
    {
        int cartstat = 0;
        int cartstatupdate = 0;
        int cartstatadd = 0;
        int cartaddstockout = 0;
        int cartupdatestockout = 0;
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            //String ID = GridView1.Rows[i].Cells[0].Text.ToString();
            HiddenField hd_cartdateadded = (HiddenField)GridView1.Rows[i].FindControl("hd_cartdateadded");
            HiddenField hd_cart = (HiddenField)GridView1.Rows[i].FindControl("hd_cart");
            HiddenField productid = (HiddenField)GridView1.Rows[i].FindControl("hd_id");
            TextBox txt_price = (TextBox)GridView1.Rows[i].FindControl("txt_price");
            TextBox txt_qty = (TextBox)GridView1.Rows[i].FindControl("txt_qty");
            HiddenField amount = (HiddenField)GridView1.Rows[i].FindControl("hd_amount");
         
          
            if (hd_cart.Value != "0")
            {
                cartstatupdate = update_cart(hd_cart.Value, txt_price.Text, txt_qty.Text, amount.Value);
               
               // 1. Safely parse the quantity to a double first
                    double qtyInput = 0;
                    double.TryParse(txt_qty.Text, out qtyInput);
                    // Use "G" (General) format - it never adds thousands separators (commas)
                    string safeQty = (qtyInput * -1).ToString("G", System.Globalization.CultureInfo.InvariantCulture);

                    // 2. Get the cost and ensure it uses a DOT (.) as the decimal separator
                    double costValue = get_cartcost(productid.Value, hd_cartdateadded.Value);
                    string safeCost = costValue.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

                    // 3. Call the function with the cleaned strings
                    cartupdatestockout = stock_update(
                        hd_cart.Value, 
                        safeQty, 
                        safeCost, 
                        "Sales no: " + txt_invoiceno.Text
                    );
               
                //cartupdatestockout = stock_update(hd_cart.Value, Convert.ToDouble("-" + txt_qty.Text).ToString(), get_cartcost(productid.Value, hd_cartdateadded.Value).ToString("F2"), "Sales no: " + txt_invoiceno.Text);
            }
            else
            {
               cartstatadd = add_cart(hd_id.Value, productid.Value, txt_price.Text, txt_qty.Text, amount.Value);
             
             // 1. Parse the quantity safely
                double qtyValue = 0;
                double.TryParse(txt_qty.Text, out qtyValue);
                string cleanQty = (qtyValue * -1).ToString(); // No "N2" here! Just the number.

                // 2. Get the cost without formatting separators
                // Assuming get_cartcost returns a numeric type, don't use "N2"
                string cleanCost = get_cartcost(productid.Value, pacificdatenow.ToString()).ToString("F2"); 

                // 3. Call the function with cleaned strings
                cartaddstockout = stock_add(
                    productid.Value, 
                    cleanQty, 
                    cleanCost, 
                    "Sales no: " + txt_invoiceno.Text, 
                    hd_id.Value, 
                    cartstatadd.ToString()
                );
             
              // cartaddstockout = stock_add(productid.Value, Convert.ToDouble("-" + txt_qty.Text).ToString(), get_cartcost(productid.Value, pacificdatenow.ToString()).ToString("N2"), "Sales no: " + txt_invoiceno.Text, hd_id.Value, cartstatadd.ToString());
        
            
            }
            if ((cartstatupdate + cartstatadd + cartaddstockout + cartupdatestockout) > 0)
            {
                cartstat = 1;
            }
            else
            {
                cartstat = 0;
            }
        }
        return cartstat;
    }
    public double get_cartcost(string prodid, string dateadded)
    {
        double val = 0;
         con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmds = new SqlCommand())
        {


            String cb1 = "select top 1 inventorycostperunit from [trans_inventory]  where inproductid=" + prodid + "  " +
                " and inventorysetid=" + rp.get_usersetid(User.Identity.Name) + " and inventorydate <= '" + dateadded + "' and inventoryvoid=0 and inventorytype ='In' order by inventoryid desc ";


            cmds.CommandText = cb1;
            cmds.Connection = con;
            rdr = cmds.ExecuteReader();
            if (rdr.Read())
            {
                if (rdr[0] != DBNull.Value)
                {
                    val = Convert.ToDouble(rdr[0]);
                }
            }
        }
            con.Close();
        return val;
    }
    public int add_cartmultiple(string invoiceid)
    {
        int cartstat = 0;

        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
                //String ID = GridView1.Rows[i].Cells[0].Text.ToString();
             HiddenField hd_cartid = (HiddenField)GridView1.Rows[i].FindControl("hd_cart");

            HiddenField productid = (HiddenField)GridView1.Rows[i].FindControl("hd_id");
            TextBox txt_price = (TextBox)GridView1.Rows[i].FindControl("txt_price");
              TextBox txt_qty = (TextBox)GridView1.Rows[i].FindControl("txt_qty");
              HiddenField amount = (HiddenField)GridView1.Rows[i].FindControl("hd_amount");
           
            string negativeqty = "-" + txt_qty.Text;
              double deduct = Convert.ToDouble(negativeqty);
          
          int  cartstatadd =  add_cart(invoiceid, productid.Value, txt_price.Text, txt_qty.Text, amount.Value);
        
        // 1. Get the cost as a raw number first
            double rawCost = get_cartcost(productid.Value, pacificdatenow.ToString());

            // 2. Convert to string safely (F2 = Fixed point, 2 decimals, no commas)
            string safeCost = rawCost.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

            // 3. Convert deduct safely
            string safeDeduct = deduct.ToString("G", System.Globalization.CultureInfo.InvariantCulture);

            // 4. Call the function
            int stockout = stock_add(
                productid.Value, 
                safeDeduct, 
                safeCost, 
                "Sales no: " + txt_invoiceno.Text, 
                invoiceid, 
                cartstatadd.ToString()
            );
        
        //  int stockout = stock_add(productid.Value, deduct.ToString(), get_cartcost(productid.Value, pacificdatenow.ToString()).ToString("N2"), "Sales no: " + txt_invoiceno.Text, invoiceid, cartstatadd.ToString());
       
            if (cartstatadd > 0 || stockout >0)
            {
                cartstat = 1;
            }
            else
            {
                cartstat = 0;
            }
        }
        return cartstat;
    }

    public void reset_sale()
    {
        lbl_cartcount.Text = "";
        ViewState["dt"] = null;
        initial_cart();
        GridView1.DataSource = null;
        GridView1.DataBind();
       
        hd_id.Value = "";
        txt_customer.Text = "";
        txt_searchitem.Text = "";
        txt_contact.Text = "";
        txt_email.Text = "";
        txt_salesnote.Text = "";
        txt_note.Text = "";
        txt_invoiceno.Text = "";
        
        txt_addaddress.Text = "";
        txt_addcustomer.Text = "";
        txt_amounttendered.Text = "";

        lbl_subtot.Text = "0.00";
        lbl_cashround.Text = "0.00";
        lbl_change.Text = "0.00";
        lbl_discount.Text = "0.00";
        lbl_tax.Text = "0.00";
        lbl_total.Text = "0.00";
        lbl_totreceive.Text = "0.00";

        txt_searchitem.Focus();

    }

    protected void txt_amounttendered_TextChanged(object sender, EventArgs e)
    {
       
       
        txt_amounttendered.Text = Convert.ToDouble(txt_amounttendered.Text).ToString("N2") ;
        compute_change();

      
    }
    public void compute_change()
    {
        double totreceive = Convert.ToDouble(lbl_totreceive.Text);
        double change = 0;
        double tendered = 0;
        if (txt_amounttendered.Text != "")
        {

            tendered = Convert.ToDouble(txt_amounttendered.Text);
            change = (tendered - totreceive);
            lbl_change.Text = change.ToString("N2");
            
        }
        else
        {
            lbl_change.Text = "0.00";
           
        }
    }
    public int tobe_deleted(string qry)
    {
        int stat = 0;
        try
        {

            con.Open();
            string val = Convert.ToString(hd_tobedeleted.Value);

            String cb1 = "Update [trans_invoicecart] set cartvoid ='1' where " + qry + " and cartsetid=" + rp.get_usersetid(User.Identity.Name) + " ";
            cmd = new SqlCommand(cb1);
            cmd.Connection = con;

            int result1 = cmd.ExecuteNonQuery();
            if (result1 >= 1)
            {
                stat = 1;
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Void", "Sales cart item voided id: " + hd_tobedeleted.Value, rp.get_usersetid(User.Identity.Name).ToString());
                //ShowMessage("Successfully deleted!", MessageType.Success);

            }
            con.Close();
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);
            ShowMessage("Error" + ex.Message, MessageType.Success);

        }
        return stat;
    }
    public int tobe_deletedinvent(string qry)
    {
        int stat = 0;
        try
        {

            con.Open();


            String cb1 = "Update [trans_inventory] set inventoryvoid ='1' where " + qry + " and inventorytype= 'Out' and invent_soldsalesid =" + hd_id.Value + " and inventorysetid=" + rp.get_usersetid(User.Identity.Name) + " ";
            cmd = new SqlCommand(cb1);
            cmd.Connection = con;

            int result1 = cmd.ExecuteNonQuery();
            if (result1 >= 1)
            {
                stat = 1;
                rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Void", "Sales void cart id: " + hd_tobedeleted.Value + " with sales no: " + txt_invoiceno.Text , rp.get_usersetid(User.Identity.Name).ToString());
                //ShowMessage("Successfully deleted!", MessageType.Success);

            }
            con.Close();
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);
            ShowMessage("Error", MessageType.Success);

        }
        return stat;
    }
    
    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        HiddenField hd_cart = (HiddenField)GridView1.Rows[e.RowIndex].FindControl("hd_cart");
        HiddenField hd_prodid = (HiddenField)GridView1.Rows[e.RowIndex].FindControl("hd_id");
       // string itemname = GridView1.Rows[e.RowIndex].Cells[1].Text;

  
        if (hd_cart.Value != "0")
        {
            hd_tobedeleted.Value += hd_cart.Value + ",";
       

        }
        int index = Convert.ToInt32(e.RowIndex);
        DataTable dt = ViewState["dt"] as DataTable;
        dt.Rows[index].Delete();
        ViewState["dt"] = dt;
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Popd", "$('#modalPopUp_Delete2').modal('hide')", true);
        BindGrid();

    }
    protected void GridView1_DataBound(object sender, EventArgs e)
    {

    }

    public int getprodid()
    {
      
         int productdids =0;
        try
        {
             productdids = Convert.ToInt32( rp.textgetstringbefore(txt_searchitem));
        }
        catch(Exception ex){
            productdids = 0;
        }
        return productdids;
    }
    public int get_producdetails(string productid)
    {
      
        int stat = 0;
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager
                    .ConnectionStrings["myconnection"].ConnectionString;
            using (SqlCommand cmd = new SqlCommand())
            {

                //string uomqry = "(select uomname from ref_units where uomid=productunit and uomvoid=0) as [uomname] ";
                //string uomcase = " case when prodbaseunit !=1 then  'of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else '' end as [extension] ";
                //cmd.CommandText = "select productname, prodsellprice, productid, " + uomqry + ", " + uomcase + " " + 
                //" from trans_product where prodsetid = " + sid + " and (prodstatus ='Active' and prodvoid =0) and( " +
                //"productid =@id )";
                cmd.CommandText = "select productname, prodsellprice, productid,uomname, itemnamedesc from vw_product where prodsetid = " + sid + " and (prodstatus ='Active' and prodvoid =0) and ( " +
                    "productid =@id )";
                cmd.Parameters.AddWithValue("@id", productid);

                cmd.Connection = conn;
                conn.Open();
          
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.Read())
                    {
                        stat = 1;
                       // string productdesc = sdr["productname"].ToString() + " " + sdr["uomname"].ToString() + " " + sdr["extension"].ToString();
                        string productdesc = sdr["itemnamedesc"].ToString();
                        double price = Convert.ToDouble(sdr["prodsellprice"]);
                        double qtys = Convert.ToDouble(txt_itemqty.Text);
                        double amount = (price * qtys);


                        add_new_row(productid, productdesc, price.ToString(), qtys.ToString(), amount.ToString("N2"));

                        //customerss.Add(sdr["productid"].ToString() + "| " + ) + " @" + );
                    }
                }
                conn.Close();
           

            }
        }


        return stat;
    }
    protected void txt_itemqty_TextChanged(object sender, EventArgs e)
    {
        if (txt_itemqty.Text == "0")
        {
            txt_itemqty.Text = "";
            txt_itemqty.Focus();
            return;
        }
        int productid = getprodid();
        if (productid == 0)
        {
            sweetmsg("Record not found!", "Product", Sweet_Alert_Type.warning);
           // ShowMessage("Record not found!", MessageType.Warning);
            txt_itemqty.Text = "";
            txt_searchitem.Text = "";
            txt_searchitem.Focus();
            return;
        }
        

          int  addcart = get_producdetails(productid.ToString());

            if (addcart > 0)
            {
                compute_subtotal();
                // ShowMessage("added", MessageType.Success);
                txt_itemqty.Text = "";
                txt_searchitem.Text = "";
                txt_searchitem.Focus();
            }
            else
            {
                sweetmsg("Record not found!", "Product", Sweet_Alert_Type.warning);
              //  ShowMessage("Record not found!", MessageType.Success);
                txt_searchitem.Text = "";
                txt_itemqty.Text = "";
                txt_searchitem.Focus();
            }
      
        
      
       
            
    }
    public string identify_producttype(string productid)
    {
        string type = "";
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager
                    .ConnectionStrings["myconnection"].ConnectionString;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "select producttype  from trans_product where prodsetid = " + rp.get_usersetid(User.Identity.Name) + " and productid =" + productid + " and (prodstatus ='Active' and prodvoid =0)";


                cmd.Connection = conn;
                conn.Open();

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.Read())
                    {
                        if (sdr[0] != DBNull.Value)
                        {
                            type = sdr[0].ToString();
                        }

                    }
                }
                conn.Close();


            }
        }



        return type;
    }
    public double check_stockavailability(string productid, string qryupdate)
    {
        double stockcount = 0;
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager
                    .ConnectionStrings["myconnection"].ConnectionString;
            using (SqlCommand cmd = new SqlCommand())
            {
                string qrystock = " select SUM(inventoryqty) from trans_inventory where  inproductid=trans_product.productid and inventoryvoid=0 and inventorysetid= " + rp.get_usersetid(User.Identity.Name) + " " + qryupdate;
                cmd.CommandText = "select (" + qrystock + ")  as [stockremaining] from trans_product where producttype='Product' and  prodsetid = " + rp.get_usersetid(User.Identity.Name) + " and productid =" + productid + " and (prodstatus ='Active' and prodvoid =0)";
                
            
                cmd.Connection = conn;
                conn.Open();

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.Read())
                    {
                        if (sdr[0] != DBNull.Value)
                        {
                           stockcount = Convert.ToDouble(sdr[0]);
                        }

                    }
                }
                conn.Close();


            }
        }



        return stockcount;
    }
    public void compute_subtotal()
    {
        double subtot = 0;
        double discounttot = 0;
        double taxtot = 0;
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            //String ID = GridView1.Rows[i].Cells[0].Text.ToString();
            HiddenField productid = (HiddenField)GridView1.Rows[i].FindControl("hd_id");
            TextBox txt_price = (TextBox)GridView1.Rows[i].FindControl("txt_price");
            TextBox txt_qty = (TextBox)GridView1.Rows[i].FindControl("txt_qty");
            HiddenField amount = (HiddenField)GridView1.Rows[i].FindControl("hd_amount");
            Label lbl_amount = (Label)GridView1.Rows[i].FindControl("lbl_amount");
            subtot += Convert.ToDouble(lbl_amount.Text);

            discounttot += Convert.ToDouble(lbl_amount.Text) * (get_taxordiscount(productid.Value, "Discount") / 100);
            taxtot += Convert.ToDouble(lbl_amount.Text) * (get_taxordiscount(productid.Value, "Tax") / 100);
        }
        lbl_subtot.Text = subtot.ToString("N2");
        discounttot = compute_discount(subtot) + discounttot;

        lbl_discount.Text = discounttot.ToString("N2");
        taxtot = compute_tax(subtot) + taxtot;
        lbl_tax.Text = taxtot.ToString("N2");
        
        lbl_total.Text = (subtot - discounttot + taxtot).ToString("N2");
        string lastval_round = rp.txtgetstringafter_cashround(lbl_total);
        if (lastval_round != "00")
        {
            lbl_cashround.Text = "0." + (100 - Convert.ToDouble(lastval_round)).ToString();
        }
        else
        {
            lbl_cashround.Text = "0.00";
        }

        lbl_totreceive.Text = (subtot - discounttot + taxtot).ToString("N0");

        compute_change(); 
    }
    public double get_taxordiscount(string prodid, string taxordiscount)
    {
        double val = 0;
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager
                    .ConnectionStrings["myconnection"].ConnectionString;
            using (SqlCommand cmd = new SqlCommand())
            {
                if (taxordiscount == "Discount")
                {
                    cmd.CommandText = "select (select discountrate from ref_discount where discountid=proddiscountid and disstatus='Active' and disvoid=0) as [discrate] from trans_product where prodsetid = " + rp.get_usersetid(User.Identity.Name) + " and productid =" + prodid + " and (prodstatus ='Active' and prodvoid =0)";
                }
                if (taxordiscount == "Tax")
                {
                    cmd.CommandText = "select (select taxrate from ref_tax where taxid=prodtaxid and taxstatus='Active' and taxvoid=0) as [taxrate] from trans_product where prodsetid = " + rp.get_usersetid(User.Identity.Name) + " and productid =" + prodid + " and (prodstatus ='Active' and prodvoid =0)";
                }
                cmd.Connection = conn;
                conn.Open();
             
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                  if (sdr.Read())
                    {
                        if (sdr[0] != DBNull.Value)
                        {
                            val = Convert.ToDouble(sdr[0]);
                        }

                    }
                }
                conn.Close();
             

            }
        }


        return val;
    }
   
  



    private void calculationA(GridViewRow row)
    {
        DataTable dtCurrentTable = (DataTable)ViewState["dt"];
       
        DataRow[] rows = null;


        HiddenField hd_id = (HiddenField)row.FindControl("hd_id");
        TextBox txt_price = (TextBox)row.FindControl("txt_price");
        TextBox txt_qty = (TextBox)row.FindControl("txt_qty");
        Label lbl_amount = (Label)row.FindControl("lbl_amount");
        HiddenField hd_amount = (HiddenField)row.FindControl("hd_amount");
        decimal val = (Convert.ToDecimal(txt_price.Text.Trim()) * Convert.ToDecimal(txt_qty.Text.Trim()));
        lbl_amount.Text = val.ToString("N2");
        hd_amount.Value = val.ToString("N2");

        //Identify exist Record to the DataTable  
        string prodid = hd_id.Value;
        rows = dtCurrentTable.Select("id='" + prodid + "'");
        if (rows.Length > 0)
        {
            //update existing Record to the DataTable  
            foreach (DataRow row1 in rows)
            {
             //   double addedqty = Convert.ToDouble(row1["qty"]) + Convert.ToDouble(txt_itemqty.Text);
                row1["price"] = txt_price.Text;
                row1["qty"] = txt_qty.Text;
                row1["amount"] = Convert.ToDouble(lbl_amount.Text).ToString("N2");

            }
            //  ShowMessage("Already exist!", MessageType.Warning);
            //  return;

            ViewState["dt"] = dtCurrentTable;
            //binding Gridview with New Row  
            GridView1.DataSource = dtCurrentTable;
            GridView1.DataBind();
        }

       
      
          compute_subtotal();
    }
    protected void txt_price_TextChanged(object sender, EventArgs e)
    {


        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }
    protected void txt_qty_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        TextBox txt_price = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txt_price");
        TextBox txt_qty = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txt_qty");
        HiddenField hd_amount = (HiddenField)GridView1.Rows[e.RowIndex].FindControl("hd_amount");
        Label lbl_amount = (Label)GridView1.Rows[e.RowIndex].FindControl("lbl_amount");
    }
    protected void btn_createtransact_Click(object sender, EventArgs e)
    {
        Page.Response.Redirect("~/pos1.aspx");

        if (Request.QueryString["id"] != null)
        {
            hd_tobedeleted.Value = "";
            hd_id.Value = rp.Decrypt(Request.QueryString["id"].ToString());

            if (hd_id.Value != "")
            {
                read_invoice();
                read_invoicecart();

            }
        }
    }
   
    public void receipt()
    {
        con = new SqlConnection(con.ConnectionString);
        con.Open();


        string qry = "select setcompanyname,setaddress,setcontact from ref_generalsettings where setid=@setid and setvoid=0 ";
        cmd = new SqlCommand(qry, con);
       
        cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
        rdr = cmd.ExecuteReader();

        if (rdr.Read())
        {
       //  lbl_storename.Text = rdr[0].ToString();
        /// lbl_storeaddress.Text = rdr[1].ToString();
        // lbl_storecontactno.Text = " Contact: " + rdr[2].ToString();


        }
        else
        {
            Page.Response.Redirect("Home.aspx");
        }

        con.Close();
        rdr.Close();

    }
  
    public void generatereceipt()
    {

        string company = rp.get_onestringvalue("Select setcompanyname from ref_generalsettings " +
            "where setid =" + rp.get_usersetid(User.Identity.Name) + " ");
        string address = rp.get_onestringvalue("Select setaddress from ref_generalsettings " +
           "where setid =" + rp.get_usersetid(User.Identity.Name) + " ");
        string setcontact = rp.get_onestringvalue("Select setcontact from ref_generalsettings " +
           "where setid =" + rp.get_usersetid(User.Identity.Name) + " ");

        string fname = rp.get_onestringvalue("Select ufullname from ref_account " +
        "where usname ='" + User.Identity.Name + "' ");

        string salesreceipt = Convert.ToString(txt_invoiceno.Text);
        string subtot = Convert.ToString( lbl_subtot.Text);
        string tax = Convert.ToString(lbl_tax.Text);
        string disc = Convert.ToString(lbl_discount.Text);
        string total = Convert.ToString(lbl_total.Text);
        string cashround = Convert.ToString(lbl_cashround.Text);
        string totreceive = Convert.ToString(lbl_totreceive.Text);
        string change = Convert.ToString(lbl_change.Text);
        string tendered = Convert.ToString(txt_amounttendered.Text);
        string customer = "";
        if (txt_customer.Text == "")
        {
            customer = "Walk-in";
        }
        else
        {
           customer = Convert.ToString(txt_customer.Text);
        }
        DataTable dt = ViewState["dt"] as DataTable;



      

        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                StringBuilder sb = new StringBuilder();
              
                //Generate Invoice (Bill) Header.
                sb.Append("<table width='100%' style='margin: 0px; font-size: 10px;' cellspacing='0' cellpadding='0'>");
                //sb.Append("<tr><td align='center'  colspan = '2'><b>Expense Report</b></td></tr>");
                sb.Append("<tr><td align='center'  colspan = '2'> " + company + " </td></tr>");
                sb.Append("<tr><td align='center'  colspan = '2'>  " + address + " </td> </tr>");
                sb.Append("<tr><td align='center'  colspan = '2'> Contact: " + setcontact + "  </td></tr> ");
            
                sb.Append("</table>");

                sb.Append("<table width='100%' style='margin: 0px; font-size: 8px;' cellspacing='0' cellpadding='0'>");
               
                sb.Append("<tr><td align='center'  colspan = '2'> Served by: " + fname + " </td></tr>");
                sb.Append("<tr><td align='center'  colspan = '2'> Date : " + pacificdatenow + " </td></tr>");

                sb.Append("</table>");
                sb.Append("<table width='100%' style='margin: 0px; font-size: 10px;' cellspacing='0' cellpadding='0'>");
                sb.Append("<tr><td align='left'  colspan = '2'> Sales receipt : " + salesreceipt + " </td></tr>");
                sb.Append("<tr><td align='left'  colspan = '2'> Customer : " + customer + " </td></tr>");
                sb.Append("</table>");
                sb.Append("---------------------------------------------------");

                //Generate Invoice (Bill) Items Grid.
                sb.Append("<table width='100%' style='margin: 0px; font-size: 10px;' >");
                sb.Append("<tr><td align='left' colspan = '2'  width='90px'>  Item Description </td> " +
                "<td align='right'  width='10px'> Amount </td>");
                sb.Append("</tr>");
                
             
                
                foreach (DataRow row in dt.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append("<td  colspan = '2' style='display:inline-block' width='90px'> " + row["itemname"] + " @" + Convert.ToDouble(row["price"]).ToString("N2") + " x" + row["qty"] + " </td>");


                    sb.Append("<td align='right' style='display:inline-block' width='10px'> " + Convert.ToDouble(row["amount"]).ToString("N2") + " </td>");

                   
                    sb.Append("</tr>");
                }
                
            
                sb.Append("</table>");
                sb.Append("<p>---------------------------------------------------</p> </br>");
                sb.Append("<p align='right' style=' font-size: 10px; line-height: 1.2em;'> Sub total: " + subtot + " </p> </br>");
                sb.Append("<p align='right' style=' font-size: 10px; line-height: 1.2em;'> Discount: " + disc + " </p></br> ");
                sb.Append("<p align='right' style=' font-size: 10px; line-height: 1.5em;'> Tax: " + tax + " </p> </br>");
                sb.Append("<p align='right' style=' font-size: 10px; line-height: 1.5em;'> Total: " + total + " </p> </br>");
                sb.Append("<p align='right' style=' font-size: 10px; line-height: 1.5em;'> Cash round: " + cashround + " </p> </br>");
                sb.Append("<p align='right' style=' font-size: 10px; line-height: 1.2em;'> Balance: " + totreceive + " </p> </br>");
                sb.Append("<p align='right' style=' font-size: 10px; line-height: 1.2em;'> Tendered: " + tendered + " </p> </br>");
                sb.Append("<p align='right' style=' font-size: 10px; line-height: 1.2em;'> Change: " + change + " </p> </br>");
                sb.Append("<br />");
                sb.Append(" <p align='center' style=' font-size: 10px; color: #666; line-height: 1.2em;'> Thank you for your purchase! </p>");

                //Export HTML String as PDF.
                StringReader sr = new StringReader(sb.ToString());
                Rectangle pagesize = new Rectangle(226, 720);

                Document pdfDoc = new Document(pagesize, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);



                PdfWriter.GetInstance(pdfDoc, new FileStream(Context.Server.MapPath("~") + "/temp_report/receipt/" + rp.get_userid(User.Identity.Name) + "_receipt.pdf", FileMode.Create));
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();
                frameProfile.Attributes.Add("src", "ViewReport.aspx?receiptid=" + rp.get_userid(User.Identity.Name) + "_receipt.pdf");

                // Response.Write(pdfDoc);
                // Response.End();

            }
        }
    }
    protected void btn_retreive_Click(object sender, EventArgs e)
    {
        generatereceipt();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
     
    }
   
    public int stock_add(string prodid, string qty, string cost, string note,string salesid, string cartid)
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
                "inventorycostperunit,inventorysetid,inventoryentryby,inventoryvoid,inventorydate,inventorynote,invent_soldsalesid,invent_soldcartid) values(@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11)", con);

            cmd.Parameters.AddWithValue("@d1", prodid);
            cmd.Parameters.AddWithValue("@d2", "Out");
            cmd.Parameters.AddWithValue("@d3", qty);
            cmd.Parameters.AddWithValue("@d4", cost);
            cmd.Parameters.AddWithValue("@d5", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d6", rp.get_userid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d7", "0");
            cmd.Parameters.AddWithValue("@d8", pacificdatenow);
            cmd.Parameters.AddWithValue("@d9", note);
            cmd.Parameters.AddWithValue("@d10", salesid);
            cmd.Parameters.AddWithValue("@d11", cartid);
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
    public int stock_update(string cartid, string qty, string cost, string note)
    {

        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("update trans_inventory set inventorytype=@d2,inventoryqty=@d3,inventorycostperunit=@d4, inventorynote=@d9 where invent_soldsalesid = @salesid and invent_soldcartid=@invent_soldcartid ", con);

            
            cmd.Parameters.AddWithValue("@d2", "Out");
            cmd.Parameters.AddWithValue("@d3", qty);
            cmd.Parameters.AddWithValue("@d4", cost);
            cmd.Parameters.AddWithValue("@d9", note);

            cmd.Parameters.AddWithValue("@d6", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@invent_soldcartid", cartid);

            cmd.Parameters.AddWithValue("@salesid", hd_id.Value);

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
   

    #region SOA_RECORD
    protected void btn_customerrecord_Click(object sender, EventArgs e)
    {
        try
        {
            if (txt_customer.Text.Length > 0)
            {
                string id = rp.textgetstringbefore(txt_customer).ToString();

                if (rp.identify_counter("trans_customer where customerid = " + id + "") > 0)
                {
                    string fullname = rp.get_onestringvalue("select cfullname from trans_customer where customerid = " + id + " ");
                    if (fullname.Length > 0)
                    {
                        lbl_customername.Text = fullname;
                        ViewState["custid"] = id;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop1", "openModal_record();", true);
                        SOA_RECORD(id);
                    }
                    else
                    {
                        sweetmsg("Customer not found!", "Warning", Sweet_Alert_Type.warning);
                        //ShowMessage("Customer not found", MessageType.Warning);
                    }
                }
                else
                {
                   // ShowMessage("Customer not found", MessageType.Warning);
                    sweetmsg("Customer not found!", "Warning", Sweet_Alert_Type.warning);
                }
            }
            else
            {

             //   ShowMessage("Please search customer", MessageType.Warning);
                sweetmsg("Please search customer", "Warning", Sweet_Alert_Type.warning);
            }

        }
        catch (Exception ex)
        {
            ShowMessage("Customer not found", MessageType.Error);
        }


     
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
                    if (decimal.TryParse(stringValue, out d))
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
    protected void btn_ctrlnos_Click(object sender, EventArgs e)
    {
        try
        {
            if (txt_customer.Text.Length > 0)
            {
                string id = rp.textgetstringbefore(txt_customer).ToString();

                if (rp.identify_counter("trans_customer where customerid = " + id + "") > 0)
                {
                    string refno = rp.get_onestringvalue("select refno from trans_customer where customerid = " + id + " ");
                    if (refno.Length > 0)
                    {
                        txt_salesnote.Text = refno;
                    }
                   
                }
                else
                {
                    sweetmsg("Customer not found", "Warning", Sweet_Alert_Type.warning);
                   // ShowMessage("Customer not found", MessageType.Warning);
                }
            }
            else
            {
                sweetmsg("Please search customer", "Warning", Sweet_Alert_Type.warning);
              //  ShowMessage("Please search customer", MessageType.Warning);
            }

        }
        catch (Exception ex)
        {
            sweetmsg("Customer not found", "Warning", Sweet_Alert_Type.warning);
            //ShowMessage("Customer not found", MessageType.Error);
        }
    }
    protected void btn_showdiscount_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Popd", "$('#mymodal_discount').modal('show')", true);
    }
    protected void btn_computediscount_Click(object sender, EventArgs e)
    {
        compute_subtotal();
    }
    public double compute_discount(double subtotal)
    {
        double discountamt = 0;
        double dvalue = 0;
        //compute_subtotal()
        bool validdisc =  double.TryParse(txt_dvalue.Text, out dvalue);
        if (validdisc)
        {
            if (subtotal > 0)
            {
                if (txt_dtype.Text == "P")
                {
                    discountamt = subtotal * (dvalue / 100);
                }
                else
                {
                    discountamt = dvalue;
                }
            }
        }
        return discountamt;
    }
    protected void btn_showtax_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Popd", "$('#mymodal_tax').modal('show')", true);
    }
    protected void btn_computetax_Click(object sender, EventArgs e)
    {
        compute_subtotal();
    }
    public double compute_tax(double subtotal)
    {
        double taxamt = 0;
        double dvalue = 0;
        //compute_subtotal()
        bool validdisc = double.TryParse(txt_taxvalue.Text, out dvalue);
        if (validdisc)
        {
            if (subtotal > 0)
            {
                if (txt_taxtype.Text == "P")
                {
                    taxamt = subtotal * (dvalue / 100);
                }
                else
                {
                    taxamt = dvalue;
                }
            }
        }
        return taxamt;
    }
}