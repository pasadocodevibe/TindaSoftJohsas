using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Security;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using System.Net.NetworkInformation;
using System.Data;

public partial class MasterPage : System.Web.UI.MasterPage
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr;
    repeatedcode rp = new repeatedcode();
    public string sqlstr;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.Page.User.Identity.IsAuthenticated)
        {
            FormsAuthentication.RedirectToLoginPage();
            // Page.Response.Redirect("Login");

        }
        //  purchaseorder.Visible = false;
        if (!IsPostBack)
        {
         
            user_checkstatus();

           // badge();
          
        string style =    rp.get_style(rp.get_usersetid(LoginName1.Page.User.Identity.Name).ToString());
        styleselect.Attributes["href"] = style.ToString();
        }
        SetCurrentPage();

    }
    private void SetCurrentPage()
    {
        var pageName = GetPageName();
     
        switch (pageName)
        {
            case "Home.aspx":
                home.Attributes["class"] = "active";
                break;
            case "SalesRecord.aspx":
                sales.Attributes["class"] = "active";
                break;
            case "CheckCashDrawer.aspx":
                cashdrawer.Attributes["class"] = "active";
                break;
            case "Changepass.aspx":
                changepass.Attributes["class"] = "active";
                break;
            case "Customer.aspx":
                customer.Attributes["class"] = "active";
                break;
            case "Inventory.aspx":
                //  option.Attributes["class"] = "sub-menu active";
             //   myinventory.Attributes["class"] = "active";
                inventory_area.Attributes["aria-expanded"] = "true";
                inventory_area.Attributes["class"] = "Collapse";
                Uls3.Attributes["class"] = "list-unstyled collapse show";
                break;


           //================ Point of Sales ===============\\
            case "pos1.aspx":
               pos.Attributes["class"] = "active";
                break;
            





         //===========Product and Services MENU =============\\
            case "ProductEntry.aspx":
                addproduct.Attributes["class"] = "sub-menu active";
                product_area.Attributes["aria-expanded"] = "true";
                product_area.Attributes["class"] = "Collapse";
                Uls1.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Productlist.aspx":
                listofitem.Attributes["class"] = "sub-menu active";
                product_area.Attributes["aria-expanded"] = "true";
                product_area.Attributes["class"] = "Collapse";
                Uls1.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "pCategory.aspx":
                itemcategory.Attributes["class"] = "sub-menu active";
                product_area.Attributes["aria-expanded"] = "true";
                product_area.Attributes["class"] = "Collapse";
                Uls1.Attributes["class"] = "list-unstyled collapse show";
                break;


            //===========Inventory MENU =============\\
            case "ManageStock.aspx":
                stock.Attributes["class"] = "sub-menu active";
                inventory_area.Attributes["aria-expanded"] = "true";
                inventory_area.Attributes["class"] = "Collapse";
                Uls3.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "mStockSheet.aspx":
                stocksheet.Attributes["class"] = "sub-menu active";
                inventory_area.Attributes["aria-expanded"] = "true";
                inventory_area.Attributes["class"] = "Collapse";
                Uls3.Attributes["class"] = "list-unstyled collapse show";
                break;

            //=========== Expenses MENU =============\\
            case "ExpensesEntry.aspx":
                addexpenses.Attributes["class"] = "sub-menu active";
                expenses_area.Attributes["aria-expanded"] = "true";
                expenses_area.Attributes["class"] = "Collapse";
                Uls4.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Expenseslist.aspx":
                listofexpenses.Attributes["class"] = "sub-menu active";
                expenses_area.Attributes["aria-expanded"] = "true";
                expenses_area.Attributes["class"] = "Collapse";
                Uls4.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "exCategory.aspx":
                categoryexpenses.Attributes["class"] = "sub-menu active";
                expenses_area.Attributes["aria-expanded"] = "true";
                expenses_area.Attributes["class"] = "Collapse";
                Uls4.Attributes["class"] = "list-unstyled collapse show";
                break;


            //============== Report MENU =============\\

            case "Report_expense.aspx":
                reportexpense.Attributes["class"] = "sub-menu active";
                option_report.Attributes["aria-expanded"] = "true";
                option_report.Attributes["class"] = "Collapse";
                ulreport.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Report_SalesByItem.aspx":
                reportsalesitem.Attributes["class"] = "sub-menu active";
                option_report.Attributes["aria-expanded"] = "true";
                option_report.Attributes["class"] = "Collapse";
                ulreport.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Report_SalesSummary.aspx":
                reportsalessummary.Attributes["class"] = "sub-menu active";
                option_report.Attributes["aria-expanded"] = "true";
                option_report.Attributes["class"] = "Collapse";
                ulreport.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Report_SalesTaxLiability.aspx":
                reportsalestax.Attributes["class"] = "sub-menu active";
                option_report.Attributes["aria-expanded"] = "true";
                option_report.Attributes["class"] = "Collapse";
                ulreport.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Report_InventoryMovement.aspx":
                reportinventorymovement.Attributes["class"] = "sub-menu active";
                option_report.Attributes["aria-expanded"] = "true";
                option_report.Attributes["class"] = "Collapse";
                ulreport.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Report_IncomeStatement.aspx":
                reportincomestatement.Attributes["class"] = "sub-menu active";
                option_report.Attributes["aria-expanded"] = "true";
                option_report.Attributes["class"] = "Collapse";
                ulreport.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Report_InventoryValuation.aspx":
                reportinventoryvaluation.Attributes["class"] = "sub-menu active";
                option_report.Attributes["aria-expanded"] = "true";
                option_report.Attributes["class"] = "Collapse";
                ulreport.Attributes["class"] = "list-unstyled collapse show";
                break;
        //============== Setup MENU =============\\

            case "Units.aspx":
                units.Attributes["class"] = "sub-menu active";
                setup_area.Attributes["aria-expanded"] = "true";
                setup_area.Attributes["class"] = "Collapse";
                ulsetup.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Discount.aspx":
                discount.Attributes["class"] = "sub-menu active";
                setup_area.Attributes["aria-expanded"] = "true";
                setup_area.Attributes["class"] = "Collapse";
                ulsetup.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Taxes.aspx":
                taxes.Attributes["class"] = "sub-menu active";
                setup_area.Attributes["aria-expanded"] = "true";
                setup_area.Attributes["class"] = "Collapse";
                ulsetup.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "GeneralSettings.aspx":
                settings.Attributes["class"] = "sub-menu active";
                setup_area.Attributes["aria-expanded"] = "true";
                setup_area.Attributes["class"] = "Collapse";
                ulsetup.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "UsersEntry.aspx":
                user.Attributes["class"] = "sub-menu active";
                setup_area.Attributes["aria-expanded"] = "true";
                setup_area.Attributes["class"] = "Collapse";
                ulsetup.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "Users.aspx":
                user.Attributes["class"] = "sub-menu active";
                setup_area.Attributes["aria-expanded"] = "true";
                setup_area.Attributes["class"] = "Collapse";
                ulsetup.Attributes["class"] = "list-unstyled collapse show";
                break;
            case "UserRole.aspx":
                userrole.Attributes["class"] = "sub-menu active";
                setup_area.Attributes["aria-expanded"] = "true";
                setup_area.Attributes["class"] = "Collapse";
                ulsetup.Attributes["class"] = "list-unstyled collapse show";
                break;
                //--------- help -------------\\
            case "Help.aspx":
                help.Attributes["class"] = "active";
                break;
            
        }
    }
    private string GetPageName()
    {
        return Request.Url.ToString().Split('/').Last();
    }
    public void user_checkstatus()
    {

        if (rp.identify_counter(" ref_account where usname = '" + LoginName1.Page.User.Identity.Name + "' ") > 0)
        {
            get_lastaccess();

      
           role_access(rp.get_userroleid(LoginName1.Page.User.Identity.Name).ToString());

        }
        else
        {

            FormsAuthentication.RedirectToLoginPage();
            Page.Response.Redirect("Default.aspx");

        }

    }
    public void get_lastaccess()
    {
        //try
        //{
        string setid="";

        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn1 = new SqlConnection(constr))
        {
            string ct = "Select ufullname, (select levelname from ref_role where levelid = uroleid) as [aclevel], uid, uroleid, uimageurl, usetid  from [ref_account] where usname = '" + LoginName1.Page.User.Identity.Name + "'";
            using (SqlCommand cmd = new SqlCommand(ct))
            {


                cmd.Connection = conn1;
                conn1.Open();
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {

                    lbl_fullname.Text = rdr[0].ToString();
                    lbl_position.Text = rdr[1].ToString();
                    hd_uid.Value = rdr[2].ToString();
                    hd_roleid.Value = rdr[3].ToString();
                    setid = rdr[5].ToString();
                    if (rdr["uimageurl"] != DBNull.Value && rdr["uimageurl"] != "")
                    {
                        //string imageUrl = "Showimagecustomer.ashx?id=" + hd_uid.Value;
                        impPrev.ImageUrl = rdr["uimageurl"].ToString();
                    }
                    else
                    {
                        impPrev.ImageUrl = "distribution/img/avataryblank.jpg";
                    }


                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Script", " alert('Please logout your account, Valid only after a logoff!');", true);
                }
                conn1.Close();
                rdr.Close();
            }
         //lbl_storename.Text = rp.get_onestringvalue("select setcompanyname from ref_generalsettings where setid ='" + setid.ToString()+ "' ");
            get_storedetails(lbl_storename, lbl_storeaddress, setid);
        }

      //  lbl_branch.Text = rp.get_onestringvalue("select branchname from ref_branch,ref_info where ref_info.branchids = ref_branch.branchid and ref_info.usname = '" + LoginName1.Page.User.Identity.Name + "' ");
        //}
        //catch (Exception ex)
        //{
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Script", " alert(" + ex.ToString() + ");", true);
        //}
    }
    public void get_storedetails(Label title,Label address, string setid)
    {
            con = new SqlConnection(con.ConnectionString);
            con.Open();

            cmd = new SqlCommand("select * from ref_generalsettings where setid =@id");
            cmd.Parameters.AddWithValue("@id", setid);
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                title.Text = rdr[1].ToString();
                address.Text = " <i class='fa fa-map-marker'></i> " + rdr[2].ToString() + " | <i class='fa fa-phone'></i> " + rdr[3].ToString() + " | <i class='fa fa-send-o'></i> " + rdr[5].ToString();
            }
          
            con.Close();
            rdr.Close();
    }


    public void role_access(string roleid)
    {
        try

        {

        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("Select * FROM [ref_rolepermission] WHERE proleid =@proleid"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@proleid", roleid);

                cmd.Connection = conn;
                conn.Open();
                rdr = cmd.ExecuteReader();
              
                while (rdr.Read())
                {
                    string menu = rdr["psitename"].ToString();
                 //   int act_add = Convert.ToInt32(rdr["padd"]);
                  //  int act_edit = Convert.ToInt32(rdr["pedit"]);
                   // int act_delete = Convert.ToInt32(rdr["pdelete"]);
                    int act_view = Convert.ToInt32(rdr["pview"]);
                   // int act_print = Convert.ToInt32(rdr["pprint"]);

   
                   menu_view(menu, act_view);
                   

                }
          
                conn.Close();
                rdr.Close();
            }

        }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Script", " alert(" + ex.ToString() + ");", true);
        }

    }
   
    public void menu_view(string menu, int act_view)
    {
        try
        {

            //============= sales record menu ===============\\
        
            hide_submenu(menu, "CheckDrawer", act_view, cashdrawer, "CheckCashDrawer.aspx");

           // //============= sales record menu ===============\\

            hide_submenu(menu, "SalesRecord", act_view, sales, "SalesRecord.aspx");



            //======= Point of Sales menu =========\\
            hide_submenu(menu, "pos1", act_view, pos, "pos1.aspx");



            //======= Customers menu =========\\
            hide_submenu(menu, "Customer", act_view, customer, "Customer.aspx");



            //======= Product and Services menu =========\\
            hide_mainmenu("Product&Services", product);
            hide_submenu(menu, "ItemCategory", act_view, itemcategory, "pCategory.aspx");
            hide_submenu(menu, "AddItem", act_view, addproduct, "ProductEntry.aspx");
            hide_submenu(menu, "Items", act_view, listofitem, "Productlist.aspx");
     


            //======= Inventory menu =========\\
            hide_mainmenu("Inventory", inventory);
            hide_submenu(menu, "Managestock", act_view, stock, "ManageStock.aspx");
            hide_submenu(menu, "StockSheet", act_view, stocksheet, "mStockSheet.aspx");

            //======= Expenses menu =========\\
            hide_mainmenu("Expenses", expenses);
            hide_submenu(menu, "ExpenseCategory", act_view, categoryexpenses, "exCategory.aspx");
            hide_submenu(menu, "ExpenseEntry", act_view, addexpenses, "ExpensesEntry.aspx");
            hide_submenu(menu, "Expenselist", act_view, listofexpenses, "Expenseslist.aspx");
         


            //======= Setup menu =========\\
            hide_mainmenu("Setup", setup);
            hide_submenu(menu, "Units", act_view, units, "Units.aspx");
            hide_submenu(menu, "Discount", act_view, discount, "Discount.aspx");
            hide_submenu(menu, "Taxes", act_view, taxes, "Taxes.aspx");
            hide_submenu(menu, "Settings", act_view, settings, "GeneralSettings.aspx");
            hide_submenu(menu, "UserEntry", act_view, user, "UsersEntry.aspx");
            hide_submenu(menu, "UserEntry", act_view, user, "Users.aspx");
            hide_submenu(menu, "UserRole", act_view, userrole, "UserRole.aspx");


            //======= Report menu =========\\

            hide_mainmenu("Report", report);
            hide_submenu(menu, "InventoryMovement", act_view, reportinventorymovement, "Report_InventoryMovement.aspx");
            hide_submenu(menu, "SalesReport_Summary", act_view, reportsalessummary, "Report_SalesSummary.aspx");
            hide_submenu(menu, "SalesReport_ByItem", act_view, reportsalesitem, "Report_SalesByItem.aspx");
            hide_submenu(menu, "ExpenseReport", act_view, reportexpense, "Report_expense.aspx");
            hide_submenu(menu, "InventoryValuation", act_view, reportinventoryvaluation, "Report_InventoryValuation.aspx");
            hide_submenu(menu, "IncomeStatement", act_view, reportincomestatement, "Report_IncomeStatement.aspx");
            hide_submenu(menu, "SalesTaxLiability", act_view, reportsalestax, "Report_SalesTaxLiability.aspx");
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Script", " alert(" + ex.ToString() + ");", true);
        }
    }
    public void hide_mainmenu(string mainmenu, System.Web.UI.HtmlControls.HtmlGenericControl li_tag)
    {
        if (rp.identify_counter(" ref_rolepermission where pview > 0 and mainmenu ='" + mainmenu + "' and  proleid= " + rp.get_userroleid(LoginName1.Page.User.Identity.Name) + " ") > 0)
        {
            li_tag.Visible = true;
        }
        else
        {
            li_tag.Visible = false;
        }
    }
    public void hide_submenu(string menu, string pagename, int act_view, System.Web.UI.HtmlControls.HtmlGenericControl li_tag, string pagenameaspx)
    {
        if (menu == pagename)
        {
            if (act_view == 1)
            {
                li_tag.Visible = true;
            }
            else
            {
                li_tag.Visible = false;
                if (GetPageName() == pagenameaspx)
                {
                    Page.Response.Redirect("Home.aspx");
                }
            }
        }
    }
}
