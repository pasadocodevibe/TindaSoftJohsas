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

public partial class Report_SalesSummary : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "SalesReport_Summary", "padd");
        edit = rp.access_user(User.Identity.Name, "SalesReport_Summary", "pedit");
        delete = rp.access_user(User.Identity.Name, "SalesReport_Summary", "pdelete");
        view = rp.access_user(User.Identity.Name, "SalesReport_Summary", "pview");
        print = rp.access_user(User.Identity.Name, "SalesReport_Summary", "pprint");

        if (view == 0)
        {
            Page.Response.Redirect("Home.aspx");
        }
        if (!IsPostBack)
        {

        }
    }

 
    protected void btn_generate_Click(object sender, EventArgs e)
    {
        // generatelist();
        branch_table("");
    }
  
    private void branch_table(string top10)
    {

        con = new SqlConnection(con.ConnectionString);
        con.Open();
        using (SqlCommand cmd = new SqlCommand())
        {
         
            string d_start = "";
            string d_end = "";
            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                 d_start = (date.Split('-'))[0];
                d_end = (date.Split('-'))[1];

            }
         

            string avgcost = "(SELECT SUM(A.inventorycostperunit * ABS(A.inventoryqty))  FROM  trans_inventory AS A INNER JOIN " +
                  "trans_invoice AS B ON A.invent_soldsalesid = B.invoiceid " +
                  "WHERE  (A.inventorytype = 'Out') AND (A.inventoryvoid = 0) AND (CAST(B.invoicedate AS DATE) =CAST(C.invoicedate AS DATE)) and (B.invoicevoid = 0 and B.invoicesetid =@setid) " +
                   "GROUP BY CAST(B.invoicedate AS DATE)) ";

            string sql = "select  CONVERT(VARCHAR(14), CAST(invoicedate AS DATE), 107) as [date1], " + avgcost + " as [avgcost] , sum(invoicesubtotal), sum(invoicediscountamt)," +
                "sum(invoicetax), sum(invoicetotal), sum(invoicecashround), (sum(invoicetotal) + sum(invoicecashround)) as [balance]," +
                "(sum(invoicesubtotal) - " + avgcost + ")  as [profit] " +
                "from trans_invoice as C " +
              
               " where (invoicevoid = 0 and invoicesetid =@setid)  ";


            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                
          
                string filterdate = " and (CAST(invoicedate AS DATE) BETWEEN '" + d_start + "' AND '" + d_end + "') ";
                sql += filterdate;

            }

            sql += " group by CAST(invoicedate AS DATE) order by CAST(invoicedate AS DATE) asc";
            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
           
            cmd.CommandText = sql;
            cmd.Connection = con;
          
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {

                sda.Fill(dt);
                con.Close();
                sda.Dispose();

            }


        }
        generate_pdf();
     
       
    }
    public void generate_pdf()
    {
        string sitename = "SalesReport_Summary";
        string setids = rp.get_usersetid(User.Identity.Name).ToString();

        string leftlogo = rp.get_logo("logolefturl", sitename, setids);
        string rightlogo = rp.get_logo("logorighturl", sitename, setids);


        Document doc = rp.documentstyleload(sitename, User.Identity.Name);
        

        iTextSharp.text.Font fntTableFontHdr = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTitleHeader = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTableFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, Color.BLACK);
        string strReportName = "myPdf" + DateTime.Now.Ticks + ".pdf";
      //  Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 20, 20, 20);
        string pdfFilePath = Server.MapPath(".") + "\\temp_report\\";
        PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + rp.get_userid(User.Identity.Name) + "_salessummary.pdf", FileMode.Create));
        wri.PageEvent = new ITextEvents();
        doc.Open();
        PdfPTable myTable = new PdfPTable(9);
        // Table size is set to 100% of the page
        myTable.WidthPercentage = 100;
        //Left aLign
        myTable.HorizontalAlignment = 0;
        myTable.SpacingAfter = 10;
        float[] sglTblHdWidths = new float[9];
        sglTblHdWidths[0] = 40f;
        sglTblHdWidths[1] = 30f;
        sglTblHdWidths[2] = 40f;
        sglTblHdWidths[3] = 30f;
        sglTblHdWidths[4] = 30f;
        sglTblHdWidths[5] = 40f;
        sglTblHdWidths[6] = 20f;
        sglTblHdWidths[7] = 30f;
        sglTblHdWidths[8] = 40f;
        myTable.HeaderRows = 3;
        //  myTable.FooterRows = 1;
        myTable.SetWidths(sglTblHdWidths);



        ///////===== HEADER ============\\\\\
        PdfPCell imghead = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(leftlogo)), true);
        imghead.HorizontalAlignment = Element.ALIGN_LEFT;
        imghead.Colspan = 2;
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
        PdfPCell Cellspan = new PdfPCell(new Phrase("Sales Summary " + " \n " + storename + " \n " + address + " \n\n Date filter: " + txtDateRange.Text, fntTableFont));
        Cellspan.Colspan = 5;
        Cellspan.HorizontalAlignment = Element.ALIGN_CENTER;
        Cellspan.VerticalAlignment = Element.ALIGN_CENTER;
        Cellspan.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspan);

        PdfPCell imgheadRIGHT = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(rightlogo)), true);
        imgheadRIGHT.Colspan = 9;
        imgheadRIGHT.HorizontalAlignment = Element.ALIGN_RIGHT;
        imgheadRIGHT.FixedHeight = 70f;
        imgheadRIGHT.PaddingBottom = 30f;
        imgheadRIGHT.Border = Rectangle.NO_BORDER;
        myTable.AddCell(imgheadRIGHT);



        ////================ END HEADER =============\\\

        doc.Add(Chunk.NEWLINE);

        ////================ TABLE HEADER =============\\\

        PdfPCell CellOneHdr = new PdfPCell(new Phrase("Sales Date", fntTableFont));
        CellOneHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellOneHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellOneHdr);
        PdfPCell CellTwoHdr = new PdfPCell(new Phrase("Avg Cost ", fntTableFont));
        CellTwoHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTwoHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTwoHdr);
        PdfPCell CellTreeHdr = new PdfPCell(new Phrase("Subtotal", fntTableFont));
        CellTreeHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTreeHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTreeHdr);
        PdfPCell Cell4Hdr = new PdfPCell(new Phrase("Discount", fntTableFont));
        Cell4Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell4Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell4Hdr);
        PdfPCell Cell5Hdr = new PdfPCell(new Phrase("Tax", fntTableFont));
        Cell5Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell5Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell5Hdr);
        PdfPCell Cell6Hdr = new PdfPCell(new Phrase("Net Total", fntTableFont));
       Cell6Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell6Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell6Hdr);
        PdfPCell Cell7Hdr = new PdfPCell(new Phrase("Cash Round", fntTableFont));
       Cell7Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell7Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell7Hdr);
        PdfPCell Cell8Hdr = new PdfPCell(new Phrase("Balance", fntTableFont));
      Cell8Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell8Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell8Hdr);
        PdfPCell Cell9Hdr = new PdfPCell(new Phrase("Profit", fntTableFont));
        Cell9Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell9Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell9Hdr);

        ////================ END TABLE HEADER =============\\\


        ////================ TABLE ROWS =============\\\

       // double valtot = 0;
      //  double sumvaltot = 0;
        double[] sumcostval = new double[8];
        foreach (DataRow row in dt.Rows)
        {




            // foreach (DataColumn column in dt.Columns)
            // {

           
            //PdfPCell CellOne = new PdfPCell(new Phrase(row[0].ToString(), fntTableFont));
            //CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
            //myTable.AddCell(CellOne);

            for (int i = 0; i <= 8; i++)
            {
                if (i == 0)
                {
                    PdfPCell CellOne = new PdfPCell(new Phrase(row[0].ToString(), fntTableFont));
                    CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
                    myTable.AddCell(CellOne);
                }
                else
                {

            // Check if the value is DBNull. If yes, use 0. If no, convert it.
                                double val = (row[i] == DBNull.Value) ? 0 : Convert.ToDouble(row[i]);

                                sumcostval[i - 1] += val;

                                PdfPCell Cell2 = new PdfPCell(new Phrase(val.ToString("N2"), fntTableFont));
                                Cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                                myTable.AddCell(Cell2);
                    // double val = Convert.ToDouble(row[i]);
                
                    //     sumcostval[i-1] += Convert.ToDouble(row[i]);
                  
                    // PdfPCell Cell2 = new PdfPCell(new Phrase(val.ToString("N2"), fntTableFont));
                    // Cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    // myTable.AddCell(Cell2);
                }

            }


            

            //  }

        }

        for (int i = 0; i <= 8; i++)
        {
            if (i == 0)
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase("Total: ", fntTableFont));
                Cellfoot.HorizontalAlignment = Element.ALIGN_RIGHT;
                myTable.AddCell(Cellfoot);
            }

            else
            {

              
                    PdfPCell Cellfoot1 = new PdfPCell(new Phrase(sumcostval[i - 1].ToString("N2"), fntTableFont));
                    Cellfoot1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    myTable.AddCell(Cellfoot1);
              
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


        frameProfile.Attributes.Add("src", "ViewReport.aspx?id=" + rp.get_userid(User.Identity.Name) + "_salessummary.pdf");

        // File.Delete(Server.MapPath("~/temp_report/") + rp.get_userid(User.Identity.Name) + "_inventoryreport.pdf");
        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('temp_report/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);


    }



}