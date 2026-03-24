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

public partial class Report_IncomeStatement : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "IncomeStatement", "padd");
        edit = rp.access_user(User.Identity.Name, "IncomeStatement", "pedit");
        delete = rp.access_user(User.Identity.Name, "IncomeStatement", "pdelete");
        view = rp.access_user(User.Identity.Name, "IncomeStatement", "pview");
        print = rp.access_user(User.Identity.Name, "IncomeStatement", "pprint");

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
            string sql = "SELECT " + top10 + " " + qryname + " as [name] , expamount  " +

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
    public double get_incomesales()
    {
        double val = 0;
          con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string sql = "select  case when sum(invoicetotal) > 0 then sum(invoicetotal) else 0 end from trans_invoice where invoicesetid=@setid and invoicevoid=0 ";
            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                string d_start = (date.Split('-'))[0];
                string d_end = (date.Split('-'))[1];
                string from = d_start + " 00:00:00.000";
                string to = d_end + " 23:59:59.999";
              
                string filterdate = " and (invoicedate BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.CommandText = sql;
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
              val = (rdr[0] == DBNull.Value) ? 0 : Convert.ToDouble(rdr[0]);
            }

        }

        return val;
    }
    public double get_incomediscount()
    {
        double val = 0;
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string sql = "select  case when sum(invoicediscountamt) > 0 then sum(invoicediscountamt) else 0 end from trans_invoice where invoicesetid=@setid and invoicevoid=0 ";
            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                string d_start = (date.Split('-'))[0];
                string d_end = (date.Split('-'))[1];
                string from = d_start + " 00:00:00.000";
                string to = d_end + " 23:59:59.999";

                string filterdate = " and (invoicedate BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.CommandText = sql;
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
             //   val = Convert.ToDouble(rdr[0]);
                val = (rdr[0] == DBNull.Value) ? 0 : Convert.ToDouble(rdr[0]);
            }

        }

        return val;
    }
    public double get_costofgoodsold()
    {
        double val = 0;
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
            string cost = "(select inventorycostperunit from trans_inventory where invent_soldcartid=A.cartid) ";

            string sql = "select   " + cost + " * cartqty  " +
            " from trans_invoicecart AS A, trans_invoice as B where A.cartinvoiceid=B.invoiceid  AND B.invoicesetid=@setid and B.invoicevoid=0 ";
            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                string d_start = (date.Split('-'))[0];
                string d_end = (date.Split('-'))[1];
                string from = d_start + " 00:00:00.000";
                string to = d_end + " 23:59:59.999";

                string filterdate = " and (B.invoicedate BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }

            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
            cmd.CommandText = sql;
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
              //  val += Convert.ToDouble(rdr[0]);
                val += (rdr[0] == DBNull.Value) ? 0 : Convert.ToDouble(rdr[0]);
            }

        }

        return val;
    }
    protected void btn_generate_Click(object sender, EventArgs e)
    {
        branch_table("");
    }

    public void generate_pdf()
    {

        string sitename = "IncomeStatement";
        string setids = rp.get_usersetid(User.Identity.Name).ToString();
   
        string leftlogo = rp.get_logo("logolefturl", sitename, setids);
        string rightlogo = rp.get_logo("logorighturl", sitename, setids);
   

        Document doc = rp.documentstyleload(sitename, User.Identity.Name);
        




        //  Font Fnt_tableHeader = FontFactory.GetFont("Arial", 11, Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTableFontHdr = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTitleHeader = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTableFont = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.NORMAL, Color.BLACK);
        string strReportName = "myPdf" + DateTime.Now.Ticks + ".pdf";
     
        string pdfFilePath = Server.MapPath(".") + "\\temp_report\\";
        PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + rp.get_userid(User.Identity.Name) + "_incomestatement.pdf", FileMode.Create));
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
        sglTblHdWidths[1] = 50f;
        sglTblHdWidths[2] = 40f;

        myTable.HeaderRows = 3;
        //  myTable.FooterRows = 1;
        myTable.SetWidths(sglTblHdWidths);



        ///////===== HEADER ============\\\\\
        PdfPCell imghead = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(leftlogo)), true);
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

    
        PdfPCell Cellspan = new PdfPCell(new Phrase(" \n " + storename + " \n " + address + "" , fntTableFont));

        Cellspan.Colspan = 1;
        Cellspan.HorizontalAlignment = Element.ALIGN_CENTER;
        Cellspan.VerticalAlignment = Element.ALIGN_CENTER;
        Cellspan.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspan);





        PdfPCell imgheadRIGHT = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(rightlogo)), true);
        imgheadRIGHT.Colspan = 1;
        imgheadRIGHT.HorizontalAlignment = Element.ALIGN_RIGHT;
        imgheadRIGHT.FixedHeight = 70f;
        imgheadRIGHT.PaddingBottom = 30f;
        imgheadRIGHT.Border = Rectangle.NO_BORDER;
        myTable.AddCell(imgheadRIGHT);



        ////================ END HEADER =============\\\

        doc.Add(Chunk.NEWLINE);
        iTextSharp.text.Font fntTableFont15 = FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.NORMAL, Color.BLACK);
        PdfPCell Cellspantitlehead = new PdfPCell(new Phrase(" Income Statement", fntTableFont15));

        Cellspantitlehead.Colspan = 3;
        Cellspantitlehead.HorizontalAlignment = Element.ALIGN_CENTER;
        Cellspantitlehead.VerticalAlignment = Element.ALIGN_CENTER;
        Cellspantitlehead.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspantitlehead);

        string date = txtDateRange.Text;
        string d_start = (date.Split('-'))[0];
        string d_end = (date.Split('-'))[1];
        PdfPCell Cellspantitle = new PdfPCell(new Phrase(" From " + d_start + " To " + d_end, fntTableFont));

        Cellspantitle.Colspan = 3;
        Cellspantitle.HorizontalAlignment = Element.ALIGN_CENTER;
        Cellspantitle.VerticalAlignment = Element.ALIGN_CENTER;
        Cellspantitle.PaddingBottom = 20f;
        Cellspantitle.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspantitle);

        ////================ TABLE HEADER =============\\\
        double sales = get_incomesales();
        double discount = get_incomediscount();
        double costofgoodsold = get_costofgoodsold();

        double grossprofit = sales + discount - costofgoodsold;
        PdfPCell cellsale = new PdfPCell(new Phrase("  +Sales", fntTableFont));
        cellsale.HorizontalAlignment = Element.ALIGN_LEFT;
        cellsale.BackgroundColor = Color.LIGHT_GRAY;
        cellsale.Colspan = 2;
        myTable.AddCell(cellsale);
        PdfPCell cellsale1 = new PdfPCell(new Phrase(sales.ToString("N2"), fntTableFont));
        cellsale1.HorizontalAlignment = Element.ALIGN_RIGHT;
        cellsale1.BackgroundColor = Color.LIGHT_GRAY;
        myTable.AddCell(cellsale1);

        PdfPCell celldiscount = new PdfPCell(new Phrase("-Discount Given", fntTableFont));
        celldiscount.PaddingLeft = 30f;
        celldiscount.HorizontalAlignment = Element.ALIGN_LEFT;
        celldiscount.Colspan = 2;
        myTable.AddCell(celldiscount);
        PdfPCell celldiscount1 = new PdfPCell(new Phrase(discount.ToString("N2"), fntTableFont));
        celldiscount1.HorizontalAlignment = Element.ALIGN_RIGHT;
        myTable.AddCell(celldiscount1);

        PdfPCell cellcost = new PdfPCell(new Phrase("-Cost of Good Sold", fntTableFont));
        cellcost.HorizontalAlignment = Element.ALIGN_LEFT;
        cellcost.PaddingLeft = 30f;
        cellcost.Colspan = 2;
        myTable.AddCell(cellcost);
        PdfPCell cellcost1 = new PdfPCell(new Phrase(costofgoodsold.ToString("N2"), fntTableFont));
        cellcost1.HorizontalAlignment = Element.ALIGN_RIGHT;
        myTable.AddCell(cellcost1);

        PdfPCell cellgross = new PdfPCell(new Phrase("  Gross Profit", fntTableFont));
        cellgross.HorizontalAlignment = Element.ALIGN_LEFT;
        cellgross.BackgroundColor = Color.LIGHT_GRAY;
        cellgross.Colspan = 2;
        myTable.AddCell(cellgross);
        PdfPCell cellgross1 = new PdfPCell(new Phrase(grossprofit.ToString("N2"), fntTableFont));
        cellgross1.HorizontalAlignment = Element.ALIGN_RIGHT;
        cellgross1.BackgroundColor = Color.LIGHT_GRAY;
        myTable.AddCell(cellgross1);




        PdfPCell CellTwoHdr1 = new PdfPCell(new Phrase("  -Expenses", fntTableFont));
        CellTwoHdr1.HorizontalAlignment = Element.ALIGN_LEFT;
        CellTwoHdr1.BackgroundColor = Color.LIGHT_GRAY;
        CellTwoHdr1.Colspan = 2;
        myTable.AddCell(CellTwoHdr1);
      
        PdfPCell CellTreeHdr = new PdfPCell(new Phrase(" ", fntTableFont));
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
  
            PdfPCell Celltwo = new PdfPCell(new Phrase(row[0].ToString(), fntTableFont));
            Celltwo.PaddingLeft = 30f;
            Celltwo.HorizontalAlignment = Element.ALIGN_LEFT;
            Celltwo.Colspan = 2;
            myTable.AddCell(Celltwo);

            valtot = row[1] != DBNull.Value ? Convert.ToDouble(row[1]) : 0;
            sumvaltot += valtot;
            // valtot = Convert.ToDouble(row[1]);
            // sumvaltot += valtot;
            PdfPCell Cell7 = new PdfPCell(new Phrase(valtot.ToString("N2"), fntTableFont));
            Cell7.HorizontalAlignment = Element.ALIGN_RIGHT;
            myTable.AddCell(Cell7);


            //  }

        }

        for (int i = 1; i <= 2; i++)
        {

            if (i == 2)
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase((grossprofit - sumvaltot).ToString("N2"), fntTableFont));
                Cellfoot.BackgroundColor = Color.LIGHT_GRAY;
                Cellfoot.HorizontalAlignment = Element.ALIGN_RIGHT;
               
                myTable.AddCell(Cellfoot);
            }
            else
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase("  Net Income", fntTableFont));
                Cellfoot.BackgroundColor = Color.LIGHT_GRAY;
                Cellfoot.Colspan = 2;
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


        frameProfile.Attributes.Add("src", "ViewReport.aspx?id=" + rp.get_userid(User.Identity.Name) + "_incomestatement.pdf");

        // File.Delete(Server.MapPath("~/temp_report/") + rp.get_userid(User.Identity.Name) + "_inventoryreport.pdf");
        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('temp_report/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);


    }

}