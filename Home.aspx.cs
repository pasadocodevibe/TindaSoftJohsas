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
        string safeMessage = HttpUtility.JavaScriptStringEncode(Message ?? string.Empty);
        ScriptManager.RegisterStartupScript(
            this,
            this.GetType(),
            System.Guid.NewGuid().ToString(),
            string.Format("ShowMessage('{0}','{1}');", safeMessage, type),
            true);
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
        int setId = 0;
        int.TryParse(setidss, out setId);
        string qryproduct = "(select count(productid) from trans_product where producttype='Product' and prodvoid=0 and prodsetid=@setid and prodstatus='Active')";
        string qryservice = "(select count(productid) from trans_product where producttype='Service' and prodvoid=0 and prodsetid=@setid and prodstatus='Active')";
        string qrycustomer = "(select count(customerid) from trans_customer where cvoid=0 and csetid=@setid and cstatus='Active')";
        string qryusers = "(select count(uid) from ref_account where uvoid=0 and usetid=@setid and ustatus='Active')";
        string query = "SELECT name, case when name='Products' then " + qryproduct +
                       " when name='Services' then " + qryservice +
                       " when name='Customers' then " + qrycustomer +
                       " when name='Users' then " + qryusers +
                       " else 0 end as [total] ";
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
                cmd.Parameters.AddWithValue("@setid", setId);
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
            int setId = Convert.ToInt32(rp.get_usersetid(User.Identity.Name));
            setid = setId.ToString();
            setidss = setid;
            if (!IsPostBack)
            {
             
               // decimal amount = 100;
               //// string currentPrice = String.Format("\u20B1{0:N2}", amount);
               // var currentPrice = String.Format(CultureInfo.GetCultureInfo("en-PH"),
               //                  "{0:c} ", amount);
               // var cultureInfo = new System.Globalization.CultureInfo("en-PH");
               // double plain = Double.Parse(currentPrice, cultureInfo);

               // ShowMessage(currentPrice.ToString() + " " + plain.ToString("N2"), MessageType.Success);
                int setids = setId;
                lbl_qservice.Text = ExecuteScalarInt(
                    "SELECT COUNT(productid) FROM trans_product WHERE producttype=@type AND prodsetid=@setid AND prodvoid=0 AND prodstatus='Active'",
                    new SqlParameter("@type", "Service"),
                    new SqlParameter("@setid", setids)).ToString("N0");
                lbl_qproduct.Text = ExecuteScalarInt(
                    "SELECT COUNT(productid) FROM trans_product WHERE producttype=@type AND prodsetid=@setid AND prodvoid=0 AND prodstatus='Active'",
                    new SqlParameter("@type", "Product"),
                    new SqlParameter("@setid", setids)).ToString("N0");
                lbl_qcustomer.Text = ExecuteScalarInt(
                    "SELECT COUNT(customerid) FROM trans_customer WHERE csetid=@setid AND cvoid=0 AND cstatus='Active'",
                    new SqlParameter("@setid", setids)).ToString("N0");
                lbl_qusers.Text = ExecuteScalarInt(
                    "SELECT COUNT(uid) FROM ref_account WHERE usetid=@setid AND uvoid=0 AND ustatus='Active'",
                    new SqlParameter("@setid", setids)).ToString("N0");

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

                DateTime dayStart = pacificdatenow.Date;
                DateTime dayEnd = dayStart.AddDays(1);
                DateTime monthStart = new DateTime(pacificdatenow.Year, pacificdatenow.Month, 1);
                DateTime monthEnd = monthStart.AddMonths(1);
                lbl_ieexpense.Text = ExecuteScalarDecimal(
                    "SELECT ISNULL(SUM(expamount),0) FROM trans_expenses WHERE expdate>=@from AND expdate<@to AND expsetid=@setid AND expvoid=0",
                    new SqlParameter("@from", dayStart),
                    new SqlParameter("@to", dayEnd),
                    new SqlParameter("@setid", setids)).ToString("N2");
                lbl_ieexpensemonth.Text = ExecuteScalarDecimal(
                    "SELECT ISNULL(SUM(expamount),0) FROM trans_expenses WHERE expdate>=@from AND expdate<@to AND expsetid=@setid AND expvoid=0",
                    new SqlParameter("@from", monthStart),
                    new SqlParameter("@to", monthEnd),
                    new SqlParameter("@setid", setids)).ToString("N2");


                lbl_tstotsales.Text = ExecuteScalarDecimal(
                    "SELECT ISNULL(SUM(invoicetotal),0) FROM trans_invoice WHERE invoicedate>=@from AND invoicedate<@to AND invoicesetid=@setid AND invoicevoid=0",
                    new SqlParameter("@from", dayStart),
                    new SqlParameter("@to", dayEnd),
                    new SqlParameter("@setid", setids)).ToString("N2");
                lbl_tstransaction.Text = ExecuteScalarInt(
                    "SELECT COUNT(invoiceid) FROM trans_invoice WHERE invoicedate>=@from AND invoicedate<@to AND invoicesetid=@setid AND invoicevoid=0",
                    new SqlParameter("@from", dayStart),
                    new SqlParameter("@to", dayEnd),
                    new SqlParameter("@setid", setids)).ToString("N0");
                lbl_tsitemsold.Text = ExecuteScalarInt(
                    "SELECT COUNT(cartid) FROM trans_invoicecart WHERE cartdatecreated>=@from AND cartdatecreated<@to AND cartsetid=@setid AND cartvoid=0 AND cartstatus='Active'",
                    new SqlParameter("@from", dayStart),
                    new SqlParameter("@to", dayEnd),
                    new SqlParameter("@setid", setids)).ToString("N0");
                lbl_tscashin.Text = ExecuteScalarDecimal(
                    "SELECT ISNULL(SUM(invoicetotal),0) FROM trans_invoice WHERE invoicedate>=@from AND invoicedate<@to AND invoicesetid=@setid AND invoicevoid=0",
                    new SqlParameter("@from", dayStart),
                    new SqlParameter("@to", dayEnd),
                    new SqlParameter("@setid", setids)).ToString("N2");
                load_top10sold(5, setids);
                load_top5expenses(5, setids);
                load_lowinstock(0, setids);
                load_nearexpiry(setids);
            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }

    }

    private void load_top10sold(int topN, int setid)
    {
        try
        {
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string uomqry1 = "(select case when prodbaseunit !=1 then uomname + ' of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else uomname end from ref_units where uomid=productunit and uomvoid=0) ";
            string com = " (Select productname + ' - ' + " + uomqry1 + " from trans_product where productid=cartproductid and prodsetid=@setid and prodstatus='Active' and prodvoid=0) ";
            string topClause = topN > 0 ? " TOP (@topN) " : " ";
            string sql = "SELECT " + topClause +
                " " + com + " as [name] " +
                 ", sum(cartqty) as [count] " +
                "FROM trans_invoicecart where (cartvoid = 0 and cartsetid =@setid)  GROUP BY cartproductid ";
           
            cmd.Parameters.AddWithValue("@setid", setid);
            if (topN > 0)
            {
                cmd.Parameters.AddWithValue("@topN", topN);
            }
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
    private void load_top5expenses(int topN, int setid)
    {
        try
        {
            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
            
                string name = " (Select excategoryname from ref_expensescategory where excategid =expensecateg) ";

                string topClause = topN > 0 ? " TOP (@topN) " : " ";
                string sql = "SELECT " + topClause +
                    " " + name + " as [name] " +
                     ", sum(expamount) as [amount] " +
                    "FROM trans_expenses where (expvoid = 0 and expsetid =@setid)  GROUP BY expensecateg order by [amount] desc ";

                cmd.Parameters.AddWithValue("@setid", setid);
                if (topN > 0)
                {
                    cmd.Parameters.AddWithValue("@topN", topN);
                }
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
    private void load_lowinstock(int topN, int setid)
    {
        try
        {
            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                string uomqry1 = "(select case when prodbaseunit !=1 then uomname + ' of ' + CONVERT(VARCHAR(50), prodbaseunit,20)  else uomname end from ref_units where uomid=productunit and uomvoid=0) ";
               // string com = " (Select ' - ' + " + uomqry1 + " from trans_product where productid =inproductid) ";
                string stockqry = "(select sum(inventoryqty) from trans_inventory where inproductid=productid and inventoryvoid=0 and inventorysetid=@setid)";
                string topClause = topN > 0 ? " TOP (@topN) " : " ";
                string sql = "SELECT " + topClause +
                    " productname + ' - ' + " + uomqry1 + " as [name] " +
                     ", " + stockqry  + " as [remainingqty] " +
                    "FROM trans_product where (" + stockqry + " <=prodreorderlevel  and prodvoid = 0 and prodsetid =@setid and prodstatus='Active' and producttype='Product') ";

                cmd.Parameters.AddWithValue("@setid", setid);
                if (topN > 0)
                {
                    cmd.Parameters.AddWithValue("@topN", topN);
                }
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
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString))
        using (SqlCommand cmd = new SqlCommand())
        {
            string sql = "select isnull(sum(invoicetotal),0) from trans_invoice where invoicesetid=@setid and invoicevoid=0 ";
            DateTime from = DateTime.MinValue;
            DateTime to = DateTime.MinValue;
            if (filterby == "Daily")
            {
                from = pacificdatenow.Date;
                to = from.AddDays(1);
                sql += " and invoicedate>=@from and invoicedate<@to ";
            }
            else if (filterby == "Monthly")
            {
                from = new DateTime(pacificdatenow.Year, pacificdatenow.Month, 1);
                to = from.AddMonths(1);
                sql += " and invoicedate>=@from and invoicedate<@to ";
            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            if (filterby == "Daily" || filterby == "Monthly")
            {
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
            }
            cmd.CommandText = sql;
            cmd.Connection = conn;
            conn.Open();
            object result = cmd.ExecuteScalar();
            return (result == null || result == DBNull.Value) ? 0 : Convert.ToDouble(result);
        }
    }
    public double get_incomediscount(string filterby)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString))
        using (SqlCommand cmd = new SqlCommand())
        {
            string sql = "select isnull(sum(invoicediscountamt),0) from trans_invoice where invoicesetid=@setid and invoicevoid=0 ";
            DateTime from = DateTime.MinValue;
            DateTime to = DateTime.MinValue;
            if (filterby == "Daily")
            {
                from = pacificdatenow.Date;
                to = from.AddDays(1);
                sql += " and invoicedate>=@from and invoicedate<@to ";
            }
            else if (filterby == "Monthly")
            {
                from = new DateTime(pacificdatenow.Year, pacificdatenow.Month, 1);
                to = from.AddMonths(1);
                sql += " and invoicedate>=@from and invoicedate<@to ";
            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            if (filterby == "Daily" || filterby == "Monthly")
            {
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
            }
            cmd.CommandText = sql;
            cmd.Connection = conn;
            conn.Open();
            object result = cmd.ExecuteScalar();
            return (result == null || result == DBNull.Value) ? 0 : Convert.ToDouble(result);
        }
    }
    public double get_costofgoodsold(string filterby)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString))
        using (SqlCommand cmd = new SqlCommand())
        {
            string sql = "select isnull(sum(isnull(I.inventorycostperunit,0) * A.cartqty),0) " +
                         "from trans_invoicecart A " +
                         "inner join trans_invoice B on A.cartinvoiceid=B.invoiceid " +
                         "left join trans_inventory I on I.invent_soldcartid=A.cartid " +
                         "where B.invoicesetid=@setid and B.invoicevoid=0 ";
            DateTime from = DateTime.MinValue;
            DateTime to = DateTime.MinValue;
            if (filterby == "Daily")
            {
                from = pacificdatenow.Date;
                to = from.AddDays(1);
                sql += " and B.invoicedate>=@from and B.invoicedate<@to ";
            }
            else if (filterby == "Monthly")
            {
                from = new DateTime(pacificdatenow.Year, pacificdatenow.Month, 1);
                to = from.AddMonths(1);
                sql += " and B.invoicedate>=@from and B.invoicedate<@to ";
            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            if (filterby == "Daily" || filterby == "Monthly")
            {
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
            }
            cmd.CommandText = sql;
            cmd.Connection = conn;
            conn.Open();
            object result = cmd.ExecuteScalar();
            return (result == null || result == DBNull.Value) ? 0 : Convert.ToDouble(result);
        }
    }
    private void load_nearexpiry(int setid)
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

    private int ExecuteScalarInt(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            conn.Open();
            object result = cmd.ExecuteScalar();
            return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
        }
    }

    private decimal ExecuteScalarDecimal(string sql, params SqlParameter[] parameters)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            conn.Open();
            object result = cmd.ExecuteScalar();
            return (result == null || result == DBNull.Value) ? 0m : Convert.ToDecimal(result);
        }
    }

}
