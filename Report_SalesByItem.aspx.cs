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

public partial class Report_SalesByItem : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "SalesReport_ByItem", "padd");
        edit = rp.access_user(User.Identity.Name, "SalesReport_ByItem", "pedit");
        delete = rp.access_user(User.Identity.Name, "SalesReport_ByItem", "pdelete");
        view = rp.access_user(User.Identity.Name, "SalesReport_ByItem", "pview");
        print = rp.access_user(User.Identity.Name, "SalesReport_ByItem", "pprint");

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

        try
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
       
            string productname = "(select productname from trans_product where productid =cartproductid) ";
            string baseunit = "(select case when prodbaseunit !=1 then   CONVERT(VARCHAR(50), prodbaseunit,20) else '' end from trans_product where trans_product.productid=cartproductid)";
           // string categqry = "(select pcategname from ref_productcategory,trans_product where trans_product.productid=cartproductid and pcategid =productcateg)";
            string unitqry = "(select ref_units.uomname from ref_units,trans_product where trans_product.productid=cartproductid and ref_units.uomid = trans_product.productunit)";

            string cost = "(select inventorycostperunit from trans_inventory where invent_soldcartid=cartid) ";

           
            string discountrate ="(SELECT CASE WHEN proddiscountid IS NULL THEN 0 ELSE " +
                      " (SELECT discountrate FROM ref_discount WHERE discountid = proddiscountid) END  " +
                       " FROM  trans_product WHERE trans_product.productid = cart.cartproductid) ";

            string taxrate = "(SELECT CASE WHEN prodtaxid IS NULL THEN 0 ELSE " +
                    " (SELECT taxrate FROM ref_tax WHERE taxid = prodtaxid) END  " +
                     " FROM  trans_product WHERE trans_product.productid = cart.cartproductid) ";



            string sql = "Select   CONVERT(VARCHAR(14), CAST(invoicedate AS DATE), 107) as [date1], " + productname + " as [name], " +
                " (" + baseunit + " + ' ' + " + unitqry + ") as [unit], " +
                " cartqty , " +
                "(" + cost + " * cartqty) as [costofgoodsold], cartamount, " +
                "( ( " + discountrate  + " / 100 ) * cartamount) as [Discount], " +
                 "( ( " + taxrate + " / 100 ) * cartamount) as [tax], " +
                "cartamount - (" + cost + " * cartqty) as [Profit]  " +
                "from trans_invoicecart as cart, trans_invoice as b  where b.invoiceid =cart.cartinvoiceid and b.invoicevoid=0 and cartvoid=0 and cartsetid=@setid";

            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                

                string from = d_start + " 00:00:00.000";
                string to = d_end + " 23:59:59.999";
               // string filterdate = " and (invoicedate BETWEEN '" + from + "' and '" + to + "' ) ";
                string filterdate = " and (CAST(b.invoicedate AS DATE) BETWEEN '" + d_start + "' AND '" + d_end + "') ";
                sql += filterdate;

            }

            sql += "  order by CAST(b.invoicedate AS DATE) asc";
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
       }
        catch (Exception ex)
        {
            WriteErrorLog(ex);
            ShowMessage("Error" + ex.Message, MessageType.Success);
         // ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);
         }
    }
    public void generate_pdf()
    {
        try
        {
        string sitename = "SalesReport_ByItem";
        string setids = rp.get_usersetid(User.Identity.Name).ToString();

        string leftlogo = rp.get_logo("logolefturl", sitename, setids);
        string rightlogo = rp.get_logo("logorighturl", sitename, setids);


        Document doc = rp.documentstyleload(sitename, User.Identity.Name);
        


        iTextSharp.text.Font fntTableFontHdr = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTitleHeader = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTableFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, Color.BLACK);
        string strReportName = "myPdf" + DateTime.Now.Ticks + ".pdf";
        //Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 20, 20, 20);
        string pdfFilePath = Server.MapPath(".") + "\\temp_report\\";
        PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + rp.get_userid(User.Identity.Name) + "_salesbyitem.pdf", FileMode.Create));
        wri.PageEvent = new ITextEvents();
        doc.Open();
        PdfPTable myTable = new PdfPTable(9);
        // Table size is set to 100% of the page
        myTable.WidthPercentage = 100;
        //Left aLign
        myTable.HorizontalAlignment = 0;
        myTable.SpacingAfter = 10;
        float[] sglTblHdWidths = new float[9];
        sglTblHdWidths[0] = 30f;
        sglTblHdWidths[1] = 40f;
        sglTblHdWidths[2] = 30f;
        sglTblHdWidths[3] = 30f;
        sglTblHdWidths[4] = 30f;
        sglTblHdWidths[5] = 30f;
        sglTblHdWidths[6] = 30f;
        sglTblHdWidths[7] = 30f;
        sglTblHdWidths[8] = 30f;
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
        PdfPCell Cellspan = new PdfPCell(new Phrase("Sales Summary By Item " + " \n " + storename + " \n " + address + " \n\n Date filter: " + txtDateRange.Text, fntTableFont));
        Cellspan.Colspan =4;
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
        PdfPCell CellOneHdr1 = new PdfPCell(new Phrase("Date", fntTableFont));
        CellOneHdr1.BackgroundColor = Color.LIGHT_GRAY;
        CellOneHdr1.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellOneHdr1);
        PdfPCell CellOneHdr = new PdfPCell(new Phrase("Item Name", fntTableFont));
        CellOneHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellOneHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellOneHdr);
        PdfPCell CellTwoHdr = new PdfPCell(new Phrase("Unit ", fntTableFont));
        CellTwoHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTwoHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTwoHdr);
        PdfPCell CellTreeHdr = new PdfPCell(new Phrase("Qty", fntTableFont));
        CellTreeHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTreeHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTreeHdr);
        PdfPCell Cell4Hdr = new PdfPCell(new Phrase("Cost of Good Sold", fntTableFont));
        Cell4Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell4Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell4Hdr);
        PdfPCell Cell5Hdr = new PdfPCell(new Phrase("Subtotal", fntTableFont));
        Cell5Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell5Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell5Hdr);
        PdfPCell Cell6Hdr = new PdfPCell(new Phrase("Discount", fntTableFont));
        Cell6Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell6Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell6Hdr);
        PdfPCell Cell7Hdr = new PdfPCell(new Phrase("Tax", fntTableFont));
        Cell7Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell7Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell7Hdr);
        PdfPCell Cell8Hdr = new PdfPCell(new Phrase("Profit", fntTableFont));
        Cell8Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell8Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell8Hdr);
  

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
                if (i <= 3)
                {
                    PdfPCell CellOne = new PdfPCell(new Phrase(row[i].ToString(), fntTableFont));
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
            if (i <= 3)
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase(" ", fntTableFont));
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


        frameProfile.Attributes.Add("src", "ViewReport.aspx?id=" + rp.get_userid(User.Identity.Name) + "_salesbyitem.pdf");

        // File.Delete(Server.MapPath("~/temp_report/") + rp.get_userid(User.Identity.Name) + "_inventoryreport.pdf");
        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('temp_report/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);
 }
        catch (Exception ex)
        {
            WriteErrorLog(ex);
            ShowMessage("Error" + ex.Message, MessageType.Success);
         // ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + "Sorry some error occured please check log file." + "');", true);
         }

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


}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               