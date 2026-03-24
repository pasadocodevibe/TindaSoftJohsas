using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

public partial class Defaultchart : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // Monthly Bind Gridview  
          //  Bind_Ch_Data_Monthly();

            // Monthly Bind Charts  
            BindChart_Monthly();
        }

    }
    //Monthly Channeling Paid and UnPaid 
    private void Bind_Ch_Data_Monthly()
    {
        Ch_Data_Monthly.DataSource = GetChartDataMonthly();
        Ch_Data_Monthly.DataBind();
    }


    private void BindChart_Monthly()
    {
        DataTable dsChartDataMonthly = new DataTable();
        StringBuilder strScriptDataMonthly = new StringBuilder();

      //  try
       // {
            dsChartDataMonthly = GetChartDataMonthly();

            strScriptDataMonthly.Append(@"<script type='text/javascript'>  
                     google.charts.load('visualization', '1', {packages: ['bar']});
                       google.charts.setOnLoadCallback(drawChart);</script>  
                    <script type='text/javascript'>  
                    function drawChart() {   
                    var data = google.visualization.arrayToDataTable([  
                    ['Month', 'Sales', 'Expense'],");

            foreach (DataRow row in dsChartDataMonthly.Rows)
            {
                strScriptDataMonthly.Append("['" + row["Month"] + "'," + row["Sales"] + "," + row["Expense"] + "],");
            }
            strScriptDataMonthly.Remove(strScriptDataMonthly.Length - 1, 1);
            strScriptDataMonthly.Append("]);");
            strScriptDataMonthly.Append("var options = {chart: {title: 'Store Performance',subtitle: 'Sales & Expenses: Monthly',}};");
        //    strScriptDataMonthly.Append("var chart = new google.charts.Bar(document.getElementById(document.getElementById('Ch_BarChart_Monthly'));  chart.draw(data, options); } google.setOnLoadCallback(drawChart)");
        strScriptDataMonthly.Append("var chart = new google.charts.Bar(document.getElementById('Ch_BarChart_Monthly'));  chart.draw(data, options); } google.setOnLoadCallback(drawChart);");   
        strScriptDataMonthly.Append(" </script>");

            ltScriptsDataMonthly.Text = strScriptDataMonthly.ToString();
        //}
        //catch
        //{
        //}
        //finally
        //{
        //    dsChartDataMonthly.Dispose();
        //    strScriptDataMonthly.Clear();
        //}
    }

    /// <summary>  
    /// fetch data from mdf file saved in app_data  
    /// </summary>  
    /// <returns>DataTable</returns>  
    private DataTable GetChartDataMonthly()
    {
        DataSet dsData = new DataSet();
        try
        {
            SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);

            string qrysales = "(select case when sum(invoicetotal) > 0 then sum(invoicetotal) else 0 end from trans_invoice  " +
                "where invoicesetid= " + "2" + " and invoicevoid=0 and DATEPART(Month, invoicedate) = A.monthid and DATEPART(Year, invoicedate) =2019  group by DATEPART(Month, invoicedate) )  ";
            // string qryexpense = "(select case WHEN Month(expdate) = monthid then sum(expamount) else 0 end from  strans_expenses " +
            //    " where (case when Month(expdate) = monthid then monthid else 0 end) and Year(expdate) =2019  group by Month(expdate) )  ";

            string exp = "(SELECT sum(expamount) FROM trans_expenses  WHERE expsetid=" + "2" + " and  expvoid=0 and (MONTH(expdate) = A.monthid) GROUP BY MONTH(expdate))";
            string qryexpense = " (CASE WHen " + exp + " is null then 0 else " + exp + " end) ";

            // string qry = "select   DateName( month , DateAdd( month , monthid , -1 ) ) as [Month], " + qrysales + " as [Sales], " + qryexpense + " as [Expense] from  ref_month as A";
            string qry = "select   mname as [Month], " + qrysales + " as [Sales], " + qryexpense + " as [Expense] from  ref_month as A where monthid <= " + "7" + " ";
            SqlCommand cmd = new SqlCommand(qry, sqlCon);

            SqlDataAdapter sqlad = new SqlDataAdapter();
            sqlad.SelectCommand = cmd;
            sqlCon.Open();

            sqlad.Fill(dsData);

            sqlCon.Close();
        }
        catch
        {
            throw;
        }
        return dsData.Tables[0];
    }
    
}