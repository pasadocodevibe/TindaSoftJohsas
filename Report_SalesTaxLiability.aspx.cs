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

public partial class Report_SalesTaxLiability : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "SalesTaxLiability", "padd");
        edit = rp.access_user(User.Identity.Name, "SalesTaxLiability", "pedit");
        delete = rp.access_user(User.Identity.Name, "SalesTaxLiability", "pdelete");
        view = rp.access_user(User.Identity.Name, "SalesTaxLiability", "pview");
        print = rp.access_user(User.Identity.Name, "SalesTaxLiability", "pprint");

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

        

         
            string sql = "select invoiceno , CONVERT(VARCHAR(12), invoicedate, 107), invoicesubtotal, invoicetax " +
       
                 " from trans_invoice where (invoicevoid = 0 and invoicesetid =@setid)  ";


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

            sql += "  order by invoicedate asc";
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
        string sitename = "SalesTaxLiability";
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
        PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + rp.get_userid(User.Identity.Name) + "_salestaxliability.pdf", FileMode.Create));
        wri.PageEvent = new ITextEvents();
        doc.Open();
        PdfPTable myTable = new PdfPTable(4);
        // Table size is set to 100% of the page
        myTable.WidthPercentage = 100;
        //Left aLign
        myTable.HorizontalAlignment = 0;
        myTable.SpacingAfter = 10;
        float[] sglTblHdWidths = new float[4];
        sglTblHdWidths[0] = 30f;
        sglTblHdWidths[1] = 30f;
        sglTblHdWidths[2] = 30f;
        sglTblHdWidths[3] = 30f;
      
              myTable.HeaderRows = 3;
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
        PdfPCell Cellspan = new PdfPCell(new Phrase("Sales Tax Liability " + " \n " + storename + " \n " + address + " \n\n Date filter: " + txtDateRange.Text, fntTableFont));
        Cellspan.Colspan = 2;
        Cellspan.HorizontalAlignment = Element.ALIGN_CENTER;
        Cellspan.VerticalAlignment = Element.ALIGN_CENTER;
        Cellspan.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspan);

        PdfPCell imgheadRIGHT = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath(rightlogo)), true);
        imgheadRIGHT.Colspan = 3;
        imgheadRIGHT.HorizontalAlignment = Element.ALIGN_RIGHT;
        imgheadRIGHT.FixedHeight = 70f;
        imgheadRIGHT.PaddingBottom = 30f;
        imgheadRIGHT.Border = Rectangle.NO_BORDER;
        myTable.AddCell(imgheadRIGHT);



        ////================ END HEADER =============\\\

        doc.Add(Chunk.NEWLINE);

        ////================ TABLE HEADER =============\\\

        PdfPCell CellOneHdr = new PdfPCell(new Phrase("Receipts", fntTableFont));
        CellOneHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellOneHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellOneHdr);
        PdfPCell CellTwoHdr = new PdfPCell(new Phrase("Date ", fntTableFont));
        CellTwoHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTwoHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTwoHdr);
        PdfPCell CellTreeHdr = new PdfPCell(new Phrase("Sub Total", fntTableFont));
        CellTreeHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTreeHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTreeHdr);
        PdfPCell Cell4Hdr = new PdfPCell(new Phrase("Tax Collected", fntTableFont));
        Cell4Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell4Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell4Hdr);
       

        ////================ END TABLE HEADER =============\\\


        ////================ TABLE ROWS =============\\\

        double[] sumcostval = new double[3];
        foreach (DataRow row in dt.Rows)
        {




            for (int i = 0; i <=3; i++)
            {
                if (i <=1)
                {
                    PdfPCell CellOne = new PdfPCell(new Phrase(row[i].ToString(), fntTableFont));
                    CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
                    myTable.AddCell(CellOne);
                }
                else
                {

                        // Check if the value is DBNull. If yes, use 0. If no, convert it.
                                double val = (row[i] == DBNull.Value) ? 0 : Convert.ToDouble(row[i]);
                                 if (i <= 3)
                                 {
                                sumcostval[i - 1] += val;
                                 }
                                PdfPCell Cell2 = new PdfPCell(new Phrase(val.ToString("N2"), fntTableFont));
                                Cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                                myTable.AddCell(Cell2);
                    // double val = Convert.ToDouble(row[i]);
                    // if (i <= 3)
                    // {

                    //     sumcostval[i - 1] += Convert.ToDouble(row[i]);
                    // }
                    // PdfPCell Cell2 = new PdfPCell(new Phrase(val.ToString("N2"), fntTableFont));
                    // Cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    // myTable.AddCell(Cell2);
                }

            }





        }

         for (int i = 0; i <=3; i++)
        {
            if (i <= 1)
            {
                if (i == 1)
                {
                    PdfPCell Cellfoot = new PdfPCell(new Phrase(" TOTAL:", fntTableFont));
                    Cellfoot.HorizontalAlignment = Element.ALIGN_RIGHT;
                    myTable.AddCell(Cellfoot);
                }
                else
                {
                    PdfPCell Cellfoot = new PdfPCell(new Phrase("", fntTableFont));
                    Cellfoot.HorizontalAlignment = Element.ALIGN_RIGHT;
                    myTable.AddCell(Cellfoot);
                }
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


        frameProfile.Attributes.Add("src", "ViewReport.aspx?id=" + rp.get_userid(User.Identity.Name) + "_salestaxliability.pdf");

        // File.Delete(Server.MapPath("~/temp_report/") + rp.get_userid(User.Identity.Name) + "_inventoryreport.pdf");
        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('temp_report/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);


    }



}