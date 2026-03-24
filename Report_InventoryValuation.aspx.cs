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

public partial class Report_InventoryValuation : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "InventoryValuation", "padd");
        edit = rp.access_user(User.Identity.Name, "InventoryValuation", "pedit");
        delete = rp.access_user(User.Identity.Name, "InventoryValuation", "pdelete");
        view = rp.access_user(User.Identity.Name, "InventoryValuation", "pview");
        print = rp.access_user(User.Identity.Name, "InventoryValuation", "pprint");

        if (view == 0)
        {
            Page.Response.Redirect("Home.aspx");
        }
        if (!IsPostBack)
        {

        }
    }

    public void view_report()
    {
        string FilePath = Server.MapPath("temp_report/expensereport.pdf");
        WebClient User = new WebClient();
        Byte[] filebuffer = User.DownloadData(FilePath);
        if (filebuffer != null)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", filebuffer.Length.ToString());
            Response.BinaryWrite(filebuffer);
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


            string stockqty = "(select case when sum(inventoryqty) > 0 then sum(inventoryqty) else 0 end from trans_inventory where inproductid =productid and inventoryvoid=0)";

            string avgcost = "(select case when sum(inventorycostperunit) > 0 then avg(inventorycostperunit) else 0 end from trans_inventory where inventorytype ='In' and inproductid =productid and inventoryvoid=0)";


            string getassetavg = "(select  " +
            " (case when sum(inventoryqty * inventorycostperunit) > 0 then sum(inventoryqty * inventorycostperunit) else 0 end) " +
            " / (case when sum(inventoryqty) > 0 then sum(inventoryqty) else 1 end) " +
                " from trans_inventory where inventorytype ='In' and inproductid =productid and inventoryvoid=0) ";
          
            string unitqry = "(select uomname from ref_units where uomid =productunit)";
      
            string sql = "SELECT " + top10 + " trans_product.productname  " +
                ",  case when prodbaseunit !=1 then CONVERT(VARCHAR(50), prodbaseunit,20) else '' end + ' ' + " + unitqry + " as [baseunit] " +
                 ", " + stockqty + " as [stockqty]," +
                " " + getassetavg + " as [avgcost], " +
                " (" + getassetavg + "  * " + stockqty + " ) as [assets] " +
                "FROM trans_product where (producttype='Product' and prodvoid = 0 and prodsetid =@setid)  ";


         

            sql += "  order by productname asc";
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
    public void generate_pdf()
    {
        string sitename = "InventoryValuation";
        string setids = rp.get_usersetid(User.Identity.Name).ToString();

        string leftlogo = rp.get_logo("logolefturl", sitename, setids);
        string rightlogo = rp.get_logo("logorighturl", sitename, setids);


        Document doc = rp.documentstyleload(sitename, User.Identity.Name);
        


        iTextSharp.text.Font fntTableFontHdr = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTitleHeader = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, Color.BLACK);
        iTextSharp.text.Font fntTableFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, Color.BLACK);
        string strReportName = "myPdf" + DateTime.Now.Ticks + ".pdf";
       // Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 20, 20, 20);
        string pdfFilePath = Server.MapPath(".") + "\\temp_report\\";
        PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + rp.get_userid(User.Identity.Name) + "_inventoryvaluation.pdf", FileMode.Create));
        wri.PageEvent = new ITextEvents();
        doc.Open();
        PdfPTable myTable = new PdfPTable(5);
        // Table size is set to 100% of the page
        myTable.WidthPercentage = 100;
        //Left aLign
        myTable.HorizontalAlignment = 0;
        myTable.SpacingAfter = 10;
        float[] sglTblHdWidths = new float[5];
        sglTblHdWidths[0] = 30f;
        sglTblHdWidths[1] = 30f;
        sglTblHdWidths[2] = 30f;
        sglTblHdWidths[3] = 30f;
        sglTblHdWidths[4] = 30f;
       
        //  myTable.FooterRows = 1;
        myTable.SetWidths(sglTblHdWidths);



        ///////===== HEADER ============\\\\\
        PdfPCell imghead = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(leftlogo)), true);
        imghead.HorizontalAlignment = Element.ALIGN_LEFT;
        imghead.Colspan = 1;
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
        PdfPCell Cellspan = new PdfPCell(new Phrase("Inventory Valuation " + " \n " + storename + " \n " + address + " \n", fntTableFont));
        Cellspan.Colspan = 3;
        Cellspan.HorizontalAlignment = Element.ALIGN_CENTER;
        Cellspan.VerticalAlignment = Element.ALIGN_CENTER;
        Cellspan.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspan);

        PdfPCell imgheadRIGHT = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(rightlogo)), true);
        imgheadRIGHT.Colspan = 5;
        imgheadRIGHT.HorizontalAlignment = Element.ALIGN_RIGHT;
        imgheadRIGHT.FixedHeight = 70f;
        imgheadRIGHT.PaddingBottom = 30f;
        imgheadRIGHT.Border = Rectangle.NO_BORDER;
        myTable.AddCell(imgheadRIGHT);



        ////================ END HEADER =============\\\

        doc.Add(Chunk.NEWLINE);

        ////================ TABLE HEADER =============\\\

        PdfPCell CellOneHdr = new PdfPCell(new Phrase("Item Name", fntTableFont));
        CellOneHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellOneHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellOneHdr);
        PdfPCell CellTwoHdr = new PdfPCell(new Phrase("Base Unit", fntTableFont));
        CellTwoHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTwoHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTwoHdr);
        PdfPCell CellTreeHdr = new PdfPCell(new Phrase("Quantity on Hand", fntTableFont));
        CellTreeHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTreeHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTreeHdr);
        PdfPCell Cell4Hdr = new PdfPCell(new Phrase("Average Cost", fntTableFont));
        Cell4Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell4Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell4Hdr);
        PdfPCell Cell5Hdr = new PdfPCell(new Phrase("Assets", fntTableFont));
        Cell5Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell5Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell5Hdr);
       

        ////================ END TABLE HEADER =============\\\


        ////================ TABLE ROWS =============\\\

     
        double sumvaltot = 0;
        double sumvaltot1 = 0;
        foreach (DataRow row in dt.Rows)
        {




            // foreach (DataColumn column in dt.Columns)
            // {
            PdfPCell CellOne = new PdfPCell(new Phrase(row[0].ToString(), fntTableFont));
            CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
            myTable.AddCell(CellOne);


            PdfPCell Cell2 = new PdfPCell(new Phrase(row[1].ToString(), fntTableFont));
            Cell2.HorizontalAlignment = Element.ALIGN_CENTER;
            myTable.AddCell(Cell2);

            PdfPCell Cell3 = new PdfPCell(new Phrase(row[2].ToString(), fntTableFont));
            Cell3.HorizontalAlignment = Element.ALIGN_CENTER;
            myTable.AddCell(Cell3);
            PdfPCell Cell4 = new PdfPCell(new Phrase(Convert.ToDouble(row[3]).ToString("N2"), fntTableFont));
            Cell4.HorizontalAlignment = Element.ALIGN_CENTER;
            sumvaltot += Convert.ToDouble(row[3]);
            myTable.AddCell(Cell4);
            PdfPCell Cell5 = new PdfPCell(new Phrase(Convert.ToDouble(row[4]).ToString("N2"), fntTableFont));
            Cell5.HorizontalAlignment = Element.ALIGN_CENTER;
            sumvaltot1 += Convert.ToDouble(row[4]);
            myTable.AddCell(Cell5);
        

            //  }

        }

        for (int i = 1; i <= 5; i++)
        {
            if (i == 4)
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase(sumvaltot.ToString("N2"), fntTableFont));
                Cellfoot.HorizontalAlignment = Element.ALIGN_CENTER;

                myTable.AddCell(Cellfoot);
            }
            else if (i == 5)
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase(sumvaltot1.ToString("N2"), fntTableFont));
                Cellfoot.HorizontalAlignment = Element.ALIGN_CENTER;

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


        frameProfile.Attributes.Add("src", "ViewReport.aspx?id=" + rp.get_userid(User.Identity.Name) + "_inventoryvaluation.pdf");

        // File.Delete(Server.MapPath("~/temp_report/") + rp.get_userid(User.Identity.Name) + "_inventoryreport.pdf");
        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('temp_report/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);


    }



}