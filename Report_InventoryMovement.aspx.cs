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

public partial class Report_InventoryMovement : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "InventoryMovement", "padd");
        edit = rp.access_user(User.Identity.Name, "InventoryMovement", "pedit");
        delete = rp.access_user(User.Identity.Name, "InventoryMovement", "pdelete");
        view = rp.access_user(User.Identity.Name, "InventoryMovement", "pview");
        print = rp.access_user(User.Identity.Name, "InventoryMovement", "pprint");

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

//            Style	How it’s displayed
//101	mm/dd/yyyy
//102	yyyy.mm.dd
//103	dd/mm/yyyy
//104	dd.mm.yyyy
//105	dd-mm-yyyy
//110	mm-dd-yyyy
//111	yyyy/mm/dd
//106	dd mon yyyy
//107	Mon dd, yyyy

            string name = "(select productname from trans_product where trans_product.productid=inproductid)";
            string baseunit = "(select case when prodbaseunit !=1 then   CONVERT(VARCHAR(50), prodbaseunit,20) else '' end from trans_product where trans_product.productid=inproductid)";
            string categqry = "(select pcategname from ref_productcategory,trans_product where trans_product.productid=inproductid and pcategid =productcateg)";
            string unitqry = "(select ref_units.uomname from ref_units,trans_product where trans_product.productid=inproductid and ref_units.uomid = trans_product.productunit)";
            string sql = "SELECT " + top10 + " CONVERT(VARCHAR(12), inventorydate, 107)  " +
                  ", " + name + " as [name] " +
             //   ", " + categqry + " as [category] " +
                ", (" + baseunit + " + ' ' + " + unitqry + ") as [unit] " +
                       ",inventorytype,ABS(inventoryqty),inventorycostperunit " +
                                 ",inventorycostperunit *  ABS(inventoryqty)  " +
                                   ",inventorynote " +
                      ", (SELECT ufullname from  ref_account where uid = inventoryentryby) as [usname] " +
                "FROM trans_inventory where (inventoryvoid = 0 and inventorysetid =@setid)  ";
           
            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                string d_start = (date.Split('-'))[0];
                string d_end = (date.Split('-'))[1];

                string from = d_start + " 00:00:00.000";
                string to = d_end + " 23:59:59.999";
                string filterdate = " and (inventorydate BETWEEN '" + from + "' and '" + to + "' ) ";

                sql += filterdate;

            }

            sql += " order by inventorydate asc";
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

        string sitename = "InventoryMovement";
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
        PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + rp.get_userid(User.Identity.Name) + "_inventorymovement.pdf", FileMode.Create));
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
        sglTblHdWidths[1] = 70f;
        sglTblHdWidths[2] = 30f;
        sglTblHdWidths[3] = 20f;
        sglTblHdWidths[4] = 20f;
        sglTblHdWidths[5] = 30f;
        sglTblHdWidths[6] = 30f;
        sglTblHdWidths[7] = 50f;
        sglTblHdWidths[8] = 50f;
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
        PdfPCell Cellspan = new PdfPCell(new Phrase("Inventory Movement " + " \n " + storename + " \n " + address + " \n\n Date filter: " + txtDateRange.Text, fntTableFont));
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

        PdfPCell CellOneHdr = new PdfPCell(new Phrase("Entry Date", fntTableFont));
        CellOneHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        CellOneHdr.BackgroundColor = Color.LIGHT_GRAY;
        myTable.AddCell(CellOneHdr);
        PdfPCell CellTwoHdr = new PdfPCell(new Phrase("Items ", fntTableFont));
        CellTwoHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTwoHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTwoHdr);
        PdfPCell CellTreeHdr = new PdfPCell(new Phrase("Units", fntTableFont));
        CellTreeHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTreeHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTreeHdr);
        PdfPCell Cell4Hdr = new PdfPCell(new Phrase("Type", fntTableFont));
        Cell4Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell4Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell4Hdr);
        PdfPCell Cell5Hdr = new PdfPCell(new Phrase("Qty", fntTableFont));
        Cell5Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell5Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell5Hdr);
        PdfPCell Cell6Hdr = new PdfPCell(new Phrase("Avg. Cost", fntTableFont));
        Cell6Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell6Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell6Hdr);
        PdfPCell Cell7Hdr = new PdfPCell(new Phrase("Asset Value", fntTableFont));
        Cell7Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell7Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell7Hdr);
        PdfPCell Cell8Hdr = new PdfPCell(new Phrase("Notes", fntTableFont));
        Cell8Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell8Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell8Hdr);
        PdfPCell Cell9Hdr = new PdfPCell(new Phrase("Entry By", fntTableFont));
        Cell9Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell9Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell9Hdr);

        ////================ END TABLE HEADER =============\\\


        ////================ TABLE ROWS =============\\\

        double valtot = 0;
        double sumvaltot = 0;
        double sumcostval = 0;
        foreach (DataRow row in dt.Rows)
        {


          

           // foreach (DataColumn column in dt.Columns)
           // {
                PdfPCell CellOne = new PdfPCell(new Phrase(row[0].ToString(), fntTableFont));
                CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
                myTable.AddCell(CellOne);


                PdfPCell Cell2 = new PdfPCell(new Phrase(row[1].ToString(), fntTableFont));
                myTable.AddCell(Cell2);

                PdfPCell Cell3 = new PdfPCell(new Phrase(row[2].ToString(), fntTableFont));
                Cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                myTable.AddCell(Cell3);
                PdfPCell Cell4 = new PdfPCell(new Phrase(row[3].ToString(), fntTableFont));
                Cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                myTable.AddCell(Cell4);
                PdfPCell Cell5 = new PdfPCell(new Phrase(row[4].ToString(), fntTableFont));
                Cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                myTable.AddCell(Cell5);
                double costval = Convert.ToDouble(row[5]);
                sumcostval += costval;
                PdfPCell Cell6 = new PdfPCell(new Phrase(costval.ToString("N2"), fntTableFont));
                Cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
                myTable.AddCell(Cell6);
                valtot = Convert.ToDouble(row[6]);
                sumvaltot += valtot;
                PdfPCell Cell7 = new PdfPCell(new Phrase(valtot.ToString("N2"), fntTableFont));
                Cell7.HorizontalAlignment = Element.ALIGN_RIGHT;
                myTable.AddCell(Cell7);
                PdfPCell Cell8 = new PdfPCell(new Phrase(row[7].ToString(), fntTableFont));
                myTable.AddCell(Cell8);
                PdfPCell Cell9 = new PdfPCell(new Phrase(row[8].ToString(), fntTableFont));
                myTable.AddCell(Cell9);
            
          //  }
         
        }
   
        for (int i = 1; i <= 9; i++)
        {
            if (i == 6)
            {
                PdfPCell Cellfoot = new PdfPCell(new Phrase(sumcostval.ToString("N2"), fntTableFont));
                Cellfoot.HorizontalAlignment = Element.ALIGN_RIGHT;
           
                myTable.AddCell(Cellfoot);
            }
            else if (i == 7)
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


        frameProfile.Attributes.Add("src", "ViewReport.aspx?id=" + rp.get_userid(User.Identity.Name) + "_inventorymovement.pdf");

       // File.Delete(Server.MapPath("~/temp_report/") + rp.get_userid(User.Identity.Name) + "_inventoryreport.pdf");
       // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('temp_report/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);

     
    }

    
    
}