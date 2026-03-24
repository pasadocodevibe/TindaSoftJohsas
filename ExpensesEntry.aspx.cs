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

public partial class ExpensesEntry : System.Web.UI.Page
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

        add = rp.access_user(User.Identity.Name, "ExpenseEntry", "padd");
        edit = rp.access_user(User.Identity.Name, "ExpenseEntry", "pedit");
        delete = rp.access_user(User.Identity.Name, "ExpenseEntry", "pdelete");
        view = rp.access_user(User.Identity.Name, "ExpenseEntry", "pview");
        print = rp.access_user(User.Identity.Name, "ExpenseEntry", "pprint");
             if (add == 0)
                {
                    Page.Response.Redirect("Home.aspx");
                }
             if (!IsPostBack)
             {

                 valDateRange.MinimumValue = DateTime.Today.AddYears(-10).ToShortDateString();
                 ///  valDateRange.MaximumValue = DateTime.Today.AddDays(5).ToShortDateString();
                 valDateRange.MaximumValue = DateTime.Today.ToShortDateString();
                 int setid = rp.get_usersetid(User.Identity.Name);
                 rp.dropdown_idtext(txt_expense, "ref_expensescategory where excategsetid = " + setid + " and excategvoid=0 and excategstatus ='Active' ", "excategid", "excategoryname");

                 if (Request.QueryString["id"] != null)
                 {

                     hd_id.Value = rp.Decrypt(Request.QueryString["id"].ToString());


                     if (hd_id.Value != "")
                     {
                         read_expense();


                     }
                 }
             
             
             }
    }
    public void read_expense()
    {
        con = new SqlConnection(con.ConnectionString);
        con.Open();

        string qry = "select * " +
            "from trans_expenses where expid =@id and expvoid =0 and expsetid=@setid ";
        cmd = new SqlCommand(qry, con);
        cmd.Parameters.AddWithValue("@id", hd_id.Value);
        cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
        rdr = cmd.ExecuteReader();

        if (rdr.Read())
        {

            txt_amount.Text = rdr[3].ToString();
            txt_expense.SelectedValue = rdr[1].ToString();
            txt_note.Text = rdr[2].ToString();
            txt_date.Text = rdr.GetDateTime(4).ToString("yyyy-MM-dd");

          

        }
        else
        {
            Page.Response.Redirect("Home.aspx");
        }

        con.Close();
        rdr.Close();
       

    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {

        try
        {
            if (hd_id.Value == "")
            {
                if (rp.identify_counter(" trans_expenses where expdate = '" + txt_date.Text + "' and expensecateg =" + txt_expense.SelectedValue + " and expsetid = " + rp.get_usersetid(User.Identity.Name) + "  ") > 0)
                {
                    ShowMessage("Already exist!", MessageType.Warning);
                    txt_expense.Focus();
                    return;
                }

                if (expense_add() == 1)
                {
                    rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Add", "Expense added item: " + txt_expense.Text, rp.get_usersetid(User.Identity.Name).ToString());
                    ShowMessage("Successfully added!", MessageType.Success);
                    expense_reset();
                }
            }
            else
            {

                if (rp.identify_counter(" trans_expenses where expdate = '" + txt_date.Text + "' and expensecateg ='" + txt_expense.SelectedValue + "' and expsetid= " + rp.get_usersetid(User.Identity.Name) + " and expid != " + hd_id.Value + " ") > 0)
                {
                    ShowMessage("Already exist!", MessageType.Warning);
                    txt_expense.Focus();
                    return;
                }
                if (expense_update() == 1)
                {
                    rp.audit_trail_add(rp.get_userid(User.Identity.Name).ToString(), "Edit", "Expenses updated item: " + txt_expense.Text, rp.get_usersetid(User.Identity.Name).ToString());
                    ShowMessage("Successfully updated", MessageType.Success);
                    expense_reset();
                }
            }
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex);

            ShowMessage("Sorry some error occured please contact system administrator or check log file.", MessageType.Error);
        }
    }
    public void expense_reset()
    {
        hd_id.Value = "";
        txt_amount.Text = "";
        txt_date.Text = "";
        txt_expense.SelectedIndex = 0;
        txt_note.Text = "";

        txt_date.Focus();
    }
    public int expense_add()
    {
        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("insert into [trans_expenses]  (expensecateg,expnote,expamount,expdate,expsetid,expvoid,expentryby) values(@d1,@d2,@d3,@d4,@d5,@d6,@d7)", con);
            //   DateTime dtnow = DateTime.Now;

            cmd.Parameters.AddWithValue("@d1", txt_expense.SelectedValue);
            cmd.Parameters.AddWithValue("@d2",txt_note.Text);
            cmd.Parameters.AddWithValue("@d3", txt_amount.Text);
            cmd.Parameters.AddWithValue("@d4", txt_date.Text);
            cmd.Parameters.AddWithValue("@d5", rp.get_usersetid(User.Identity.Name));
            cmd.Parameters.AddWithValue("@d6", "0");
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
    public int expense_update()
    {

        int stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("update trans_expenses set expensecateg=@d1,expnote=@d2,expamount=@d3,expdate=@d4 where expid = @idss and expsetid=@d6 ", con);

            cmd.Parameters.AddWithValue("@d1", txt_expense.SelectedValue);
            cmd.Parameters.AddWithValue("@d2", txt_note.Text);
            cmd.Parameters.AddWithValue("@d3", txt_amount.Text);
            cmd.Parameters.AddWithValue("@d4", txt_date.Text);
            cmd.Parameters.AddWithValue("@d6", rp.get_usersetid(User.Identity.Name));
           
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
}