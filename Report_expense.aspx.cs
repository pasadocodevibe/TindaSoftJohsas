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
using System.Net;

public partial class Report_expense : System.Web.UI.Page
{
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    int branchid = 0;
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
    repeatedcode rp = new repeatedcode();
    public int add, edit, delete, view, print;
    public enum MessageType { Success, Error, Info, Warning };
    public DataTable dt = new DataTable();
    protected void ShowMessage(string Message, MessageType type)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), System.Guid.NewGuid().ToString(), "ShowMessage('" + Message + "','" + type + "');", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        add = rp.access_user(User.Identity.Name, "ExpenseReport", "padd");
        edit = rp.access_user(User.Identity.Name, "ExpenseReport", "pedit");
        delete = rp.access_user(User.Identity.Name, "ExpenseReport", "pdelete");
        view = rp.access_user(User.Identity.Name, "ExpenseReport", "pview");
        print = rp.access_user(User.Identity.Name, "ExpenseReport", "pprint");

        if (view == 0)
        {
            Page.Response.Redirect("Home.aspx");
        }
        if (!IsPostBack)
        {

        }
    }

    private void branch_table(string top10)
    {

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {

            //       
            string qryname = " (select excategoryname from ref_expensescategory where excategid =expensecateg) ";
            string sql = "SELECT " + top10 + "  CONVERT(VARCHAR(12), expdate, 107), " + qryname + " as [name] , expamount  " +

                "FROM trans_expenses where (expvoid = 0 and expsetid =@setid)  ";

            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                string d_start = (date.Split('-'))[0];
                string d_end = (date.Split('-'))[1];

                string from = d_start;
                string to = d_end;
                string filterdate = " and (expdate BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }

            sql += " order by expdate asc";
            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.CommandText = sql;
            cmd.Connection = con;
            //rdr = cmd.ExecuteReader();
            //while (rdr.Read())
            //{

            //}
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {

                sda.Fill(dt);
                con.Close();
                sda.Dispose();

            }


        }
        generate_pdf();
        // lbl_item.Text = rp.footerinfo_gridview(gv_masterlist);
        //}
        //catch (Exception ex)
        //{

        //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);

        //        }
    }
    protected void btn_generate_Click(object sender, EventArgs e)
    {
        branch_table("");
    }
    
    public void generate_pdf()
    {
       
        string sitename = "ExpenseReport";
        string setids = rp.get_usersetid(User.Identity.Name).ToString();
       
        string leftlogo = rp.get_logo("logolefturl", sitename, setids);
        string rightlogo = rp.get_logo("logorighturl", sitename, setids);
        Document doc = rp.documentstyleload(sitename, User.Identity.Name);
    
       
      //  Font Fnt_tableHeader = FontFactory.GetFont("Arial", 11, Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTableFontHdr = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTitleHeader = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTableFont = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.NORMAL, Color.BLACK);
        string strReportName = "myPdf" + DateTime.Now.Ticks + ".pdf";

        
       // Document doc = get_reportsettings();
        string pdfFilePath = Server.MapPath(".") + "\\temp_report\\";
        PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + rp.get_userid(User.Identity.Name) + "_expensereport.pdf", FileMode.Create));
        wri.PageEvent = new ITextEvents();
        doc.Open();
        PdfPTable myTable = new PdfPTable(3);
        // Table size is set to 100% of the page
        myTable.WidthPercentage = 100;
        //Left aLign
        myTable.HorizontalAlignment = 0;
        myTable.SpacingAfter = 10;
        float[] sglTblHdWidths = new float[3];
        sglTblHdWidths[0] = 40f;
        sglTblHdWidths[1] = 70f;
        sglTblHdWidths[2] = 40f;
       
        myTable.HeaderRows = 3;
        //  myTable.FooterRows = 1;
        myTable.SetWidths(sglTblHdWidths);



        ///////===== HEADER ============\\\\\
        PdfPCell imghead = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(leftlogo.ToString())), true);
        imghead.HorizontalAlignment = Element.ALIGN_LEFT;
       // imghead.Colspan = 2;
        imghead.FixedHeight = 70f;
        imghead.PaddingBottom = 30f;
        //imghead.Width(50,50);
        //  imghead.ScaleAbsolute(50, 50);
        imghead.Border = Rectangle.NO_BORDER;
        myTable.AddCell(imghead);

        // Header title
        string storename = rp.get_onestringvalue("select setcompanyname from ref_generalsettings where setid=" + rp.get_usersetid(User.Identity.Name) + " and setvoid=0 and setstatus='Active' ");
        string address = rp.get_onestringvalue("select setaddress from ref_generalsettings where setid=" + rp.get_usersetid(User.Identity.Name) + " and setvoid=0 and setstatus='Active' ");
        string contact = rp.get_onestringvalue("select setcontact from ref_generalsettings where setid=" + rp.get_usersetid(User.Identity.Name) + " and setvoid=0 and setstatus='Active' ");
        PdfPCell Cellspan = new PdfPCell(new Phrase("Expense Report " + " \n " + storename + " \n " + address + " \n\n Date filter: " + txtDateRange.Text + " ", fntTableFont));
     
        Cellspan.Colspan = 1;
        Cellspan.HorizontalAlignment = Element.ALIGN_CENTER;
        Cellspan.VerticalAlignment = Element.ALIGN_CENTER;
        Cellspan.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspan);

        PdfPCell imgheadRIGHT = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(rightlogo.ToString())), true);
        imgheadRIGHT.Colspan = 2;
        imgheadRIGHT.HorizontalAlignment = Element.ALIGN_RIGHT;
        imgheadRIGHT.FixedHeight = 70f;
        imgheadRIGHT.PaddingBottom = 30f;
        imgheadRIGHT.Border = Rectangle.NO_BORDER;
        myTable.AddCell(imgheadRIGHT);



        ////================ END HEADER =============\\\

        doc.Add(Chunk.NEWLINE);

        ////================ TABLE HEADER =============\\\

        PdfPCell CellOneHdr = new PdfPCell(new Phrase("Date", fntTableFont));

        CellOneHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        CellOneHdr.BackgroundColor = Color.LIGHT_GRAY;
        myTable.AddCell(CellOneHdr);
        PdfPCell CellTwoHdr = new PdfPCell(new Phrase("Name ", fntTableFont));
        CellTwoHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        CellTwoHdr.BackgroundColor = Color.LIGHT_GRAY;
        myTable.AddCell(CellTwoHdr);
        PdfPCell CellTreeHdr = new PdfPCell(new Phrase("Amount", fntTableFont));
        CellTreeHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        CellTreeHdr.BackgroundColor = Color.LIGHT_GRAY;
        myTable.AddCell(CellTreeHdr);
       

        ////================ END TABLE HEADER =============\\\


        ////================ TABLE ROWS =============\\\

        double valtot = 0;
        double sumvaltot = 0;
   
        foreach (DataRow row in dt.Rows)
        {




            // foreach (DataColumn column in dt.Columns)
            // {
            PdfPCell CellOne = new PdfPCell(new Phrase(row[0].ToString(), fntTableFont));
            CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
            myTable.AddCell(CellOne);
            PdfPCell Celltwo = new PdfPCell(new Phrase(row[1].ToString(), fntTableFont));
            CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
            myTable.AddCell(Celltwo);

      
            valtot = Convert.ToDouble(row[2]);
            sumvaltot += valtot;
            PdfPCell Cell7 = new PdfPCell(new Phrase(valtot.ToString("N2"), fntTableFont));
            Cell7.HorizontalAlignment = Element.ALIGN_RIGHT;
            myTable.AddCell(Cell7);
        

            //  }

        }

        for (int i = 1; i <= 3; i++)
        {
           
            if (i == 3)
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase(sumvaltot.ToString("N2"), fntTableFont));
            
                Cellfoot.HorizontalAlignment = Element.ALIGN_RIGHT;
                myTable.AddCell(Cellfoot);
            }
            else
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase("", fntTableFont));
                myTable.AddCell(Cellfoot);
            }
        }

        doc.Add(myTable);




        // Paragraph footer = new Paragraph("footer test ");
        //footer.Alignment = Element.ALIGN_CENTER;
        //doc.Add(footer);


        ////================ END TABLE ROWS =============\\\

        ///================== FOOTER ===================\\\



        ////=========== END FOOTER =====================\\\\



        doc.Close();


        frameProfile.Attributes.Add("src", "ViewReport.aspx?id=" + rp.get_userid(User.Identity.Name) + "_expensereport.pdf");

        // File.Delete(Server.MapPath("~/temp_report/") + rp.get_userid(User.Identity.Name) + "_inventoryreport.pdf");
        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('temp_report/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);


    }

}