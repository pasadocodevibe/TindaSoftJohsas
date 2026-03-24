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
using ClosedXML.Excel;

public partial class SOA : System.Web.UI.Page
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
        add = rp.access_user(User.Identity.Name, "SOA", "padd");
        edit = rp.access_user(User.Identity.Name, "SOA", "pedit");
        delete = rp.access_user(User.Identity.Name, "SOA", "pdelete");
        view = rp.access_user(User.Identity.Name, "SOA", "pview");
        print = rp.access_user(User.Identity.Name, "SOA", "pprint");

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


            string sql = "select ROW_NUMBER() OVER(" + txt_sort.Text + ") AS RowNo,invdateonly, invoiceno, customerfullname, ctrlno, invoicetotal " +
                " from vw_soa where invoicesetid=@setid and invoicecustomerid >0 ";
            if (!string.IsNullOrEmpty(txtDateRange.Text.Trim()))
            {
                string date = txtDateRange.Text;
                
                string filterdate = " and (invdateonly BETWEEN '" + d_start + "' AND '" + d_end + "') ";
                sql += filterdate;

            }
            if (!string.IsNullOrEmpty(txt_customer.Text.Trim()))
            {
                sql += " and( customerfullname like '%" + txt_customer.Text + "%' ) ";
            }
            sql += " " + txt_sort.Text;

           
            cmd.Parameters.AddWithValue("@setid", rp.get_usersetid(User.Identity.Name));
           
            cmd.CommandText = sql;
            cmd.Connection = con;
        
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {

                sda.Fill(dt);
                con.Close();
                sda.Dispose();
                Session["table"] = dt;
            }
  

        }
     
        if (dt.Rows.Count > 0)
        {
            generate_pdf();
           

        }
        else
        {
            ShowMessage("No record found", MessageType.Warning);
            frameProfile.Attributes.Add("src", "");
        }
    
        }
        catch (Exception ex)
        {
           // TextBox1.Text = ex.Message;
          ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + ex.Message + "');", true);

                }
    }
    
    public void generate_pdf()
    {
        string sitename = "SOA";
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
        PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + rp.get_userid(User.Identity.Name) + "SOA.pdf", FileMode.Create));
        wri.PageEvent = new ITextEvents();
        doc.Open();


        //PdfPTable test = new PdfPTable(1);
        //PdfPCell c2 = new PdfPCell(new iTextSharp.text.Phrase("AAAAAAAAA\nBBBBBB")) { HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE };
        //test.AddCell(c2);
        //doc.Add(test);



        PdfPTable myTable = new PdfPTable(6);
        // Table size is set to 100% of the page
        myTable.WidthPercentage = 100;
        //Left aLign
        myTable.HorizontalAlignment = 0;
        myTable.SpacingAfter = 10;
        float[] sglTblHdWidths = new float[6];
        sglTblHdWidths[0] = 10f;
        sglTblHdWidths[1] = 30f;
        sglTblHdWidths[2] = 30f;
        sglTblHdWidths[3] = 50f;
        sglTblHdWidths[4] = 30f;
        sglTblHdWidths[5] = 30f;
       
        myTable.HeaderRows = 4;
        //  myTable.FooterRows = 1;
        myTable.SetWidths(sglTblHdWidths);


        PdfPCell imghead = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath("~/images/storelogo/johsas.JPG")), true);
        imghead.HorizontalAlignment = Element.ALIGN_CENTER;
        imghead.Colspan = 6;
      //  imghead.FixedHeight = 70f;
       // imghead.PaddingBottom = 30f;
        //imghead.Width(50,50);
        // imghead.ScaleAbsolute(50, 50);
        imghead.Border = Rectangle.NO_BORDER;
        myTable.AddCell(imghead);
   

        // Header title
        //string storename = rp.get_onestringvalue("select setcompanyname from ref_generalsettings where setid=" + rp.get_usersetid(User.Identity.Name) + " and setvoid=0 and setstatus='Active' ");
        //string address = rp.get_onestringvalue("select setaddress from ref_generalsettings where setid=" + rp.get_usersetid(User.Identity.Name) + " and setvoid=0 and setstatus='Active' ");
        //string contact = rp.get_onestringvalue("select setcontact from ref_generalsettings where setid=" + rp.get_usersetid(User.Identity.Name) + " and setvoid=0 and setstatus='Active' ");
        //PdfPCell Cellspan = new PdfPCell(new Phrase(" " + " \n " + storename + " \n " + address + " \n\n Date filtered: " + txtDateRange.Text + " \n\n", fntTableFont));
        //Cellspan.Colspan =6;
        //Cellspan.HorizontalAlignment = Element.ALIGN_CENTER;
        //Cellspan.VerticalAlignment = Element.ALIGN_CENTER;
        //Cellspan.Border = Rectangle.NO_BORDER;
        //myTable.AddCell(Cellspan);




        PdfPCell Cellspan1 = new PdfPCell(new Phrase(" STATEMENT OF ACCOUNT \n\n", fntTitleHeader));
        Cellspan1.Colspan =6;
        Cellspan1.HorizontalAlignment = Element.ALIGN_CENTER;
        Cellspan1.VerticalAlignment = Element.ALIGN_CENTER;
        Cellspan1.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspan1);


        PdfPCell Cellspanl1 = new PdfPCell(new Phrase("Account Name: " + txt_acctname.Text + " \n\n", fntTableFont));
        Cellspanl1.Colspan = 6;
        Cellspanl1.HorizontalAlignment = Element.ALIGN_LEFT;
        Cellspanl1.VerticalAlignment = Element.ALIGN_LEFT;
        Cellspanl1.Border = Rectangle.NO_BORDER;
        myTable.AddCell(Cellspanl1);

        ////================ END HEADER =============\\\

        doc.Add(Chunk.NEWLINE);



   

        ////================ TABLE HEADER =============\\\
        PdfPCell CellOneHdr1 = new PdfPCell(new Phrase("NO", fntTableFont));
        CellOneHdr1.BackgroundColor = Color.LIGHT_GRAY;
        CellOneHdr1.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellOneHdr1);
        PdfPCell CellOneHdr = new PdfPCell(new Phrase("DATE", fntTableFont));
        CellOneHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellOneHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellOneHdr);
        PdfPCell CellTwoHdr = new PdfPCell(new Phrase("CHARGE INVOICE ", fntTableFont));
        CellTwoHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTwoHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTwoHdr);
        PdfPCell CellTreeHdr = new PdfPCell(new Phrase("NAME", fntTableFont));
        CellTreeHdr.BackgroundColor = Color.LIGHT_GRAY;
        CellTreeHdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(CellTreeHdr);
        PdfPCell Cell4Hdr = new PdfPCell(new Phrase("CONTROL NUMBER", fntTableFont));
        Cell4Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell4Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell4Hdr);
        PdfPCell Cell5Hdr = new PdfPCell(new Phrase("AMOUNT", fntTableFont));
        Cell5Hdr.BackgroundColor = Color.LIGHT_GRAY;
        Cell5Hdr.HorizontalAlignment = Element.ALIGN_CENTER;
        myTable.AddCell(Cell5Hdr);

      //  doc.Add(myTable);

        ////================ END TABLE HEADER =============\\\


        ////================ TABLE ROWS =============\\\

       // double valtot = 0;
      //  double sumvaltot = 0;
        double[] sumcostval = new double[1];
        foreach (DataRow row in dt.Rows)
        {




            for (int i = 0; i <= 5; i++)
            {
             
                if (i == 1)
                {
                    PdfPCell CellOne = new PdfPCell(new Phrase(Convert.ToDateTime( row[i].ToString()).ToString("MMMM dd, yyyy"), fntTableFont));
                    CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
                    myTable.AddCell(CellOne);
                }
               
                else if (i == 5)
                {
                      // Check if the value is DBNull. If yes, use 0. If no, convert it.
                                double val = (row[i] == DBNull.Value) ? 0 : Convert.ToDouble(row[i]);

                                sumcostval[0] += val;

                                PdfPCell Cell2 = new PdfPCell(new Phrase(val.ToString("N2"), fntTableFont));
                                Cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                                myTable.AddCell(Cell2);
                    // double val = Convert.ToDouble(row[i]);

                    // sumcostval[0] += Convert.ToDouble(row[i]);

                    // PdfPCell CellOne = new PdfPCell(new Phrase(val.ToString("N2"), fntTableFont));
                    // CellOne.HorizontalAlignment = Element.ALIGN_RIGHT;
                    // myTable.AddCell(CellOne);
                }
                else
                {
                    PdfPCell CellOne = new PdfPCell(new Phrase(row[i].ToString(), fntTableFont));
                    CellOne.HorizontalAlignment = Element.ALIGN_CENTER;
                    myTable.AddCell(CellOne);



                }
              

            }


        }

        for (int i = 0; i <= 5; i++)
        {
            if (i <= 4)
            {
            PdfPCell Cellfoot = new PdfPCell(new Phrase(" ", fntTableFont));
            Cellfoot.HorizontalAlignment = Element.ALIGN_RIGHT;
            myTable.AddCell(Cellfoot);
            }

            else
            {

                PdfPCell Cellfoot1 = new PdfPCell(new Phrase(sumcostval[0].ToString("N2"), fntTitleHeader));
                Cellfoot1.HorizontalAlignment = Element.ALIGN_RIGHT;
                myTable.AddCell(Cellfoot1);

            }


        }


        doc.Add(Chunk.NEWLINE);
      




        // Paragraph footer = new Paragraph("footer test ");
        //footer.Alignment = Element.ALIGN_CENTER;
        //doc.Add(footer);


        ////================ END TABLE ROWS =============\\\

        ///================== FOOTER ===================\\\

        PdfPCell foot_prepared = new PdfPCell(new Phrase("Prepared By: " + txt_prepared.Text + " \n\n", fntTableFont));
        foot_prepared.Colspan = 6;
        foot_prepared.PaddingTop = 40;
        foot_prepared.PaddingLeft = 30;
        foot_prepared.HorizontalAlignment = Element.ALIGN_LEFT;
        foot_prepared.VerticalAlignment = Element.ALIGN_LEFT;
        foot_prepared.Border = Rectangle.NO_BORDER;
        myTable.AddCell(foot_prepared);

        PdfPCell foot_verified = new PdfPCell(new Phrase("Verified By: " + txt_verified.Text + " \n\n", fntTableFont));
        foot_verified.Colspan = 6;
        foot_verified.PaddingTop = 40;
        foot_verified.PaddingLeft = 30;
        foot_verified.HorizontalAlignment = Element.ALIGN_LEFT;
        foot_verified.VerticalAlignment = Element.ALIGN_LEFT;
        foot_verified.Border = Rectangle.NO_BORDER;
        myTable.AddCell(foot_verified);

        ////=========== END FOOTER =====================\\\\

        doc.Add(myTable);

        doc.Close();


        frameProfile.Attributes.Add("src", "ViewReport.aspx?id=" + rp.get_userid(User.Identity.Name) + "SOA.pdf");

        // File.Delete(Server.MapPath("~/temp_report/") + rp.get_userid(User.Identity.Name) + "_inventoryreport.pdf");
        // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('temp_report/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);


    }
       protected void btn_export_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = (DataTable)Session["table"];

            if (dt.Rows.Count > 0)
            {
                // RowNo,invdateonly, invoiceno, customerfullname, ctrlno, invoicetotal 
                string[] selectedColumns = new[] { "RowNo", "invdateonly", "invoiceno", "customerfullname", "ctrlno", "invoicetotal" };
                dt.Columns["RowNo"].ColumnName = "NO";
                dt.Columns["invdateonly"].ColumnName = "DATE";
                dt.Columns["invoiceno"].ColumnName = "CHARGE INVOICE";
                dt.Columns["customerfullname"].ColumnName = "NAME";
                dt.Columns["ctrlno"].ColumnName = "CONTROL NUMBER";
                dt.Columns["invoicetotal"].ColumnName = "AMOUNT";
                // DataTable dtexport = new DataView(dt).ToTable(false, selectedColumns);



                string filenames = "SOA_exported" + DateTime.Now.ToString("MMM-dd-yyyy hhmmss") + ".xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dt, "export_soa");

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + filenames);



                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        //   wb.SaveAs("~/report/xls/" + filenames);
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }



            }
        }
        catch (Exception ex)
        {

            ShowMessage(ex.Message, MessageType.Error);
        }
    }

}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               