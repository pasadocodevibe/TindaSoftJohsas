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
using System.Web.Services;
using System.Globalization;


public partial class Home : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    repeatedcode rp = new repeatedcode();
    public enum MessageType { Success, Error, Info, Warning };
    public int add, edit, delete, view, print;
    public string setid;
 
    public static string setidss;
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
    [WebMethod]
    public static List<object> GetChartDataquickstat()
    {
     
        string qryproduct = "(select count(productid) from trans_product where producttype='Product' and  prodvoid=0 and prodsetid= " + setidss + " and prodstatus='Active' )";
        string qryservice = "(select count(productid) from trans_product where producttype='Service' and  prodvoid=0 and prodsetid= " + setidss + " and prodstatus='Active' )";
        string qrycustomer = "(select count(customerid) from trans_customer where cvoid=0 and csetid= " + setidss + " and cstatus='Active' )";
        string qryusers = "(select count(uid) from ref_account where uvoid=0 and usetid= " + setidss + " and ustatus='Active' )";
        string query = "SELECT name , case when name = 'Products' Then " + qryproduct + " when name = 'Services' then  " + qryservice + " when name = 'Customers' then " + qrycustomer + " when name='Users' then " + qryusers + " else '0' end as [total] ";
        query += " FROM view_quickstat   ";
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        List<object> chartData = new List<object>();
        chartData.Add(new object[]
    {
          "name", "total"
    });


        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        chartData.Add(new object[]
                    {
                        sdr["name"], sdr["total"]
                    });
                    }
                }
                con.Close();
                return chartData;
            }
        }
    }
    [WebMethod]
    public static List<object> GetChartData()
    {
        string qryname = "(select excategoryname from ref_expensescategory where excategid=expensecateg) ";
        string query = "SELECT " + qryname + " as [name] , SUM(expamount) as [total]";
        query += " FROM trans_expenses where expvoid=0 GROUP BY expensecateg ";
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        List<object> chartData = new List<object>();
        chartData.Add(new object[]
    {
        "name", "total"
    });
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        chartData.Add(new object[]
                    {
                        sdr["name"], sdr["total"]
                    });
                    }
                }
                con.Close();
                return chartData;
            }
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
            setidss = rp.get_usersetid(User.Identity.Name).ToString();
            if (!IsPostBack)
            {
             
               // decimal amount = 100;
               //// string currentPrice = String.Format("\u20B1{0:N2}", amount);
               // var currentPrice = String.Format(CultureInfo.GetCultureInfo("en-PH"),
               //                  "{0:c} ", amount);
               // var cultureInfo = new System.Globalization.CultureInfo("en-PH");
               // double plain = Double.Parse(currentPrice, cultureInfo);

               // ShowMessage(currentPrice.ToString() + " " + plain.ToString("N2"), MessageType.Success);
                string setids = rp.get_usersetid(User.Identity.Name).ToString();
                lbl_qservice.Text = rp.identify_counter(" trans_product WHERE producttype='Service' and prodsetid= " + setids + " and prodvoid=0 and prodstatus='Active' ").ToString("N0");
                lbl_qproduct.Text = rp.identify_counter(" trans_product WHERE producttype='Product' and prodsetid= " + setids + " and prodvoid=0 and prodstatus='Active' ").ToString("N0");
                lbl_qcustomer.Text = rp.identify_counter(" trans_customer WHERE csetid= " + setids + " and cvoid=0 and cstatus='Active' ").ToString("N0");
                lbl_qusers.Text = rp.identify_counter(" ref_account WHERE usetid= " + setids + " and uvoid=0 and ustatus='Active' ").ToString("N0");

                double dsales = get_incomesales("Daily");
                double ddiscount = get_incomediscount("Daily");
                double dcostofgoodsold = get_costofgoodsold("Daily");
                double dgrossprofit = dsales + ddiscount - dcostofgoodsold;
                lbl_ieincome.Text = dgrossprofit.ToString("N2");

                double msales = get_incomesales("Monthly");
                double mdiscount = get_incomediscount("Monthly");
                double mcostofgoodsold = get_costofgoodsold("Monthly");
                double mgrossprofit = msales + mdiscount - mcostofgoodsold;
                lbl_ieincomemonth.Text = mgrossprofit.ToString("N2");

                lbl_ieexpense.Text = rp.identify_sum("expamount", " trans_expenses WHERE expdate ='" + pacificdatenow.ToShortDateString() + "' and expsetid= " + setids + " and expvoid=0 ").ToString("N2");
                lbl_ieexpensemonth.Text = rp.identify_sum("expamount", " trans_expenses WHERE ( MONTH(expdate) ='" + pacificdatenow.Month + "' AND  YEAR(expdate) ='" + pacificdatenow.Year + "') and expsetid= " + setids + " and expvoid=0 ").ToString("N2");


                lbl_tstotsales.Text = rp.identify_sum("invoicetotal", " trans_invoice WHERE (MONTH(invoicedate) ='" + pacificdatenow.Month + "' AND DAY(invoicedate) ='" + pacificdatenow.Day + "' AND YEAR(invoicedate) ='" + pacificdatenow.Year + "' ) and invoicesetid= " + setids + " and invoicevoid=0 ").ToString("N2");
                lbl_tstransaction.Text = rp.identify_counter(" trans_invoice WHERE (MONTH(invoicedate) ='" + pacificdatenow.Month + "' AND DAY(invoicedate) ='" + pacificdatenow.Day + "' AND YEAR(invoicedate) ='" + pacificdatenow.Year + "' )  and invoicesetid= " + setids + " and invoicevoid=0 ").ToString("N0");
                lbl_tsitemsold.Text = rp.identify_counter(" trans_invoicecart WHERE (MONTH(cartdatecreated) ='" + pacificdatenow.Month + "' AND DAY(cartdatecreated) ='" + pacificdatenow.Day + "' AND YEAR(cartdatecreated) ='" + pacificdatenow.Year + "' )  and cartsetid= " + setids + " and cartvoid=0 AND cartstatus='Active' ").ToString("N0");
                lbl_tscashin.Text = rp.identify_sum("invoicetotal", " trans_invoice WHERE (MONTH(invoicedate) ='" + pacificdatenow.Month + "' AND DAY(invoicedate) ='" + pacificdatenow.Day + "' AND YEAR(invoicedate) ='" + pacificdatenow.Year + "' ) and invoicesetid= " + setids + " and invoicevoid=0 ").ToString("N2");
                load_top10sold(" top 5 ", setids.ToString());
                load_top5expenses(" top 5", setids.ToString());
                load_lowinstock(" ", setids.ToString());
                load_nearexpiry("", setids.ToString());
            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }

    }

    private void load_top10sold(string top10, string setid)
    {
        try
        {
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string uomqry1 = "(select case when prodbaseunit !=1 then uomname + ' of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else uomname end from ref_units where uomid=productunit and uomvoid=0) ";
            string com = " (Select  productname + ' - ' + " + uomqry1 + " from trans_product where productid =cartproductid and prodsetid = " + setid + " and prodstatus='Active' and prodvoid=0) ";
        
            string sql = "SELECT " + top10 + "  " +
                " " + com + " as [name] " +
                 ", sum(cartqty) as [count] " +
                "FROM trans_invoicecart where (cartvoid = 0 and cartsetid =@setid)  GROUP BY cartproductid ";
           
            cmd.Parameters.AddWithValue("@setid", setid);
            cmd.CommandText = sql;
            cmd.Connection = con;
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                sda.Fill(dt);
                gv_top10solditems.DataSource = dt;
                gv_top10solditems.DataBind();
            }
        }
      
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }
    private void load_top5expenses(string top10, string setid)
    {
        try
        {
            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
            
                string name = " (Select excategoryname from ref_expensescategory where excategid =expensecateg) ";

                string sql = "SELECT " + top10 + "  " +
                    " " + name + " as [name] " +
                     ", sum(expamount) as [amount] " +
                    "FROM trans_expenses where (expvoid = 0 and expsetid =@setid)  GROUP BY expensecateg order by [amount] desc ";

                cmd.Parameters.AddWithValue("@setid", setid);
                cmd.CommandText = sql;
                cmd.Connection = con;
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    gv_top5expenses.DataSource = dt;
                    gv_top5expenses.DataBind();
                }
            }

        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }
    private void load_lowinstock(string top10, string setid)
    {
        try
        {
            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                string uomqry1 = "(select case when prodbaseunit !=1 then uomname + ' of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else uomname end from ref_units where uomid=productunit and uomvoid=0) ";
               // string com = " (Select ' - ' + " + uomqry1 + " from trans_product where productid =inproductid) ";
                string stockqry = "(select sum(inventoryqty) from trans_inventory where inproductid=productid and inventoryvoid=0 and inventorysetid= " + setid.ToString() + " )";
                string sql = "SELECT " + top10 + "  " +
                    " productname + ' - ' + " + uomqry1 + " as [name] " +
                     ", " + stockqry  + " as [remainingqty] " +
                    "FROM trans_product where (" + stockqry + " <=prodreorderlevel  and prodvoid = 0 and prodsetid =@setid and prodstatus='Active' and producttype='Product') ";

                cmd.Parameters.AddWithValue("@setid", setid);
                cmd.CommandText = sql;
                cmd.Connection = con;
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    gv_lowinstock.DataSource = dt;
                    gv_lowinstock.DataBind();
                }
            }

        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }
    public double get_incomesales(string filterby)
    {
        double val = 0;
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string filterdate = "";
            string sql = "select  case when sum(invoicetotal) > 0 then sum(invoicetotal) else 0 end from trans_invoice where invoicesetid=@setid and invoicevoid=0 ";
            if (!string.IsNullOrEmpty(pacificdatenow.ToShortDateString()))
            {
                DateTime first_date = new DateTime(pacificdatenow.Year, pacificdatenow.Month, 1);
                DateTime last_date = new DateTime(pacificdatenow.Year, pacificdatenow.Month, DateTime.DaysInMonth(pacificdatenow.Year, pacificdatenow.Month));

                if (filterby == "Daily")
                {
                    string dfrom = pacificdatenow.ToShortDateString() + " 00:00:00.000";
                    string dto = pacificdatenow.ToShortDateString() + " 23:59:59.999";
                    filterdate = " and (invoicedate BETWEEN '" + dfrom + "' and '" + dto + "' ) ";

                }
                if (filterby == "Monthly")
                {
                    string from = first_date.ToShortDateString() + " 00:00:00.000";
                    string to = last_date.ToShortDateString() + " 23:59:59.999";
                    filterdate = " and (invoicedate BETWEEN '" + from + "' and '" + to + "' ) ";
                }
                sql += filterdate;

            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.CommandText = sql;
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                val = Convert.ToDouble(rdr[0]);
            }

        }

        return val;
    }
    public double get_incomediscount(string filterby)
    {
        double val = 0;
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string filterdate = "";
            string sql = "select  case when sum(invoicediscountamt) > 0 then sum(invoicediscountamt) else 0 end from trans_invoice where invoicesetid=@setid and invoicevoid=0 ";
            if (!string.IsNullOrEmpty(pacificdatenow.ToShortDateString()))
            {
                DateTime first_date = new DateTime(pacificdatenow.Year, pacificdatenow.Month, 1);
                DateTime last_date = new DateTime(pacificdatenow.Year, pacificdatenow.Month, DateTime.DaysInMonth(pacificdatenow.Year, pacificdatenow.Month));
               
                if (filterby == "Daily")
                {
                    string dfrom = pacificdatenow.ToShortDateString() + " 00:00:00.000";
                    string dto = pacificdatenow.ToShortDateString() + " 23:59:59.999";
                    filterdate = " and (invoicedate BETWEEN '" + dfrom + "' and '" + dto + "' ) ";

                }
                if (filterby == "Monthly")
                {
                    string from = first_date.ToShortDateString() + " 00:00:00.000";
                    string to = last_date.ToShortDateString() + " 23:59:59.999";
                   filterdate = " and (invoicedate BETWEEN '" + from + "' and '" + to + "' ) ";
                }
                sql += filterdate;

            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.CommandText = sql;
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                val = Convert.ToDouble(rdr[0]);
            }

        }

        return val;
    }
    public double get_costofgoodsold(string filterby)
    {
        double val = 0;
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string filterdate = "";
            string cost = "(select inventorycostperunit from trans_inventory where invent_soldcartid=A.cartid) ";

            string sql = "select   " + cost + " * cartqty  " +
            " from trans_invoicecart AS A, trans_invoice as B where A.cartinvoiceid=B.invoiceid  AND B.invoicesetid=@setid and B.invoicevoid=0 ";
            if (!string.IsNullOrEmpty(pacificdatenow.ToShortDateString()))
            {
                DateTime first_date = new DateTime(pacificdatenow.Year, pacificdatenow.Month, 1);
                DateTime last_date = new DateTime(pacificdatenow.Year, pacificdatenow.Month, DateTime.DaysInMonth(pacificdatenow.Year, pacificdatenow.Month));
               
                if (filterby == "Daily")
                {
                    string dfrom = pacificdatenow.ToShortDateString() + " 00:00:00.000";
                    string dto = pacificdatenow.ToShortDateString() + " 23:59:59.999";
                    filterdate = " and (B.invoicedate BETWEEN '" + dfrom + "' and '" + dto + "' ) ";

                }
                if (filterby == "Monthly")
                {
                    string from = first_date.ToShortDateString() + " 00:00:00.000";
                    string to = last_date.ToShortDateString() + " 23:59:59.999";
                   filterdate = " and (B.invoicedate BETWEEN '" + from + "' and '" + to + "' ) ";
                }
                sql += filterdate;

            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.CommandText = sql;
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (rdr[0] != DBNull.Value)
                        val += Convert.ToDouble(rdr[0]);
            }

        }

        return val;
    }
    private void load_nearexpiry(string top10, string setid)
    {
        try
        {
            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {

                //string name = " (Select excategoryname from ref_expensescategory where excategid =expensecateg) ";

                //string sql = "SELECT " + top10 + "  " +
                //    " " + name + " as [name] " +
                //     ", sum(expamount) as [amount] " +
                //    "FROM trans_expenses where (expvoid = 0 and expsetid =@setid)  GROUP BY expensecateg order by [amount] desc ";
                string sql = "select itemnamedesc,pcategname,daysleft, dtexpiredesc from vw_inventory where (daysleft >= 0 and daysleft <= 100) and inventorysetid =@setid order by daysleft asc";
                cmd.Parameters.AddWithValue("@setid", setid);
                cmd.CommandText = sql;
                cmd.Connection = con;
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    gv_expiry.DataSource = dt;
                    gv_expiry.DataBind();
                }
            }

        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }

}