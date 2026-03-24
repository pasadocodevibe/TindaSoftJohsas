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

public partial class CheckCashDrawer : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    repeatedcode rp = new repeatedcode();
    public enum MessageType { Success, Error, Info, Warning };
    public int add, edit, delete, view, print;
    public string setid;
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
        try
        {
            add = rp.access_user(User.Identity.Name, "CheckDrawer", "padd");
            edit = rp.access_user(User.Identity.Name, "CheckDrawer", "pedit");
            delete = rp.access_user(User.Identity.Name, "CheckDrawer", "pdelete");
            view = rp.access_user(User.Identity.Name, "CheckDrawer", "pview");
            print = rp.access_user(User.Identity.Name, "CheckDrawer", "pprint");
            setid = rp.get_usersetid(User.Identity.Name).ToString();

            if (!IsPostBack)
            {

                txt_dateasof.Text = pacificdatenow.ToString("yyyy-MM-dd");

            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }

    }
    public double get_sumcashin()
    {
        double val = 0;
        try
        {

            con = new SqlConnection(con.ConnectionString);
            con.Open();
            using (SqlCommand cmd = new SqlCommand())
            {
                string from = txt_dateasof.Text + " 00:00:00.000";
                string to = txt_dateasof.Text + " 23:59:59.999";
               string filterdate = " (invoicedate BETWEEN '" + from + "' and '" + to + "' )  ";

                string sql = "SELECT sum(invoicetotal) FROM trans_invoice where " + filterdate +"  and invoicesetid =@id and invoicevoid=0 ";


                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", rp.get_usersetid(User.Identity.Name));

                cmd.Connection = con;
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    if (rdr[0] != DBNull.Value)
                    {
                        val = Convert.ToDouble(rdr[0]);
                    }
                    else
                    {
                        val = 0;

                    }

                }

                else
                {
                    ShowMessage("invalid!", MessageType.Warning);
                }
                rdr.Close();
            }
            con.Close();

        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
        return val;
    }
    protected void btn_cancel_Click(object sender, EventArgs e)
    {
        txt_dateasof.Text = "";
        txt_beginbalance.Text = "";
        txt_amountdrawer.Text = "";

        lbl_remarks.Text = "";
        lbl_yourammount.Text = "0.00";
        lbl_cashin.Text = "0.00";
        lbl_begbal.Text = "0.00";
        lbl_amountshouldbe.Text = "0.00";
        txt_dateasof.Focus();
    }
    protected void btn_add_Click(object sender, EventArgs e)
    {
        double begbal = Convert.ToDouble(txt_beginbalance.Text);
        double amountindrawer = Convert.ToDouble(txt_amountdrawer.Text);
        double cashin = get_sumcashin();
        double amountshouldbe = (begbal + cashin);
        lbl_begbal.Text = begbal.ToString("N2");
        lbl_cashin.Text = cashin.ToString("N2");
        lbl_amountshouldbe.Text = amountshouldbe.ToString("N2");
        lbl_yourammount.Text = amountindrawer.ToString("N2");

        if (amountshouldbe == amountindrawer)
        {
            lbl_remarks.Text = "Cash drawer's balance match";
            lbl_remarks.ForeColor = Color.LightGreen;
        }
        else if (amountshouldbe > amountindrawer)
        {
            string ret = (amountshouldbe - amountindrawer).ToString("N2");
            lbl_remarks.Text = "Cash drawer's balance did not match with the system, short: " + ret;
           

            lbl_remarks.ForeColor = Color.OrangeRed;
        }
        else if (amountindrawer > amountshouldbe)
        {
            string ret = (amountindrawer - amountshouldbe).ToString("N2");
            lbl_remarks.Text = "Cash drawer's balance did not match with the system, too large: " + ret;
            lbl_remarks.ForeColor = Color.OrangeRed;
        }
        else
        {
            lbl_remarks.Text = "Cash drawer's balance did not match with the system";
            lbl_remarks.ForeColor = Color.OrangeRed;
        }

        



        //if (branch_update() == 1)
        //{
        //    rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Edit", "Settings updated item: " + txt_storename.Text, setid);
        //    ShowMessage("Successfully Changed", MessageType.Success);

        //}
    }
   
}