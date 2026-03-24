using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

public partial class pdftable : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    
      

      protected void Button2_Click(object sender, EventArgs e)
      {
        
         iTextSharp.text.Font fntTableFontHdr = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, Color.BLACK);
         iTextSharp.text.Font fntTitleHeader = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, Color.BLACK);
         iTextSharp.text.Font fntTableFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL,Color.BLACK);
         string strReportName = "myPdf" + DateTime.Now.Ticks + ".pdf";
         Document doc = new Document(iTextSharp.text.PageSize.LETTER, 20, 20, 20, 20);
         string pdfFilePath = Server.MapPath(".") + "\\Reports\\";
         PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + strReportName, FileMode.Create));
         wri.PageEvent = new ITextEvents();
         doc.Open();
        PdfPTable myTable = new PdfPTable(3);
         // Table size is set to 100% of the page
         myTable.WidthPercentage = 100;
         //Left aLign
         myTable.HorizontalAlignment = 0;
         myTable.SpacingAfter = 10;
         float[] sglTblHdWidths = new float[3];
         sglTblHdWidths[0] = 200f;
         sglTblHdWidths[1] = 200f;
         sglTblHdWidths[2] = 200f;
         myTable.HeaderRows = 3;
         myTable.FooterRows = 0;
         myTable.SetWidths(sglTblHdWidths);
    


          ///////===== HEADER ============\\\\\
         PdfPCell imghead = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath("images/storelogo/doh_icon.png")), true);
         imghead.HorizontalAlignment = Element.ALIGN_LEFT;
         imghead.FixedHeight = 60f;
         imghead.PaddingBottom = 10f;
         //imghead.Width(50,50);
         //  imghead.ScaleAbsolute(50, 50);
         imghead.Border = Rectangle.NO_BORDER;
         myTable.AddCell(imghead);

         // Header title
         PdfPCell Cellspan = new PdfPCell(new Phrase(" Sales Reports \n\n  Department of Health\n Soccsksargen Region \n ", fntTitleHeader));
         Cellspan.Colspan = 1;
         Cellspan.HorizontalAlignment = Element.ALIGN_CENTER;
         Cellspan.VerticalAlignment = Element.ALIGN_CENTER;
         Cellspan.Border = Rectangle.NO_BORDER;
         myTable.AddCell(Cellspan);

         PdfPCell imgheadRIGHT = new PdfPCell(iTextSharp.text.Image.GetInstance(Server.MapPath("images/storelogo/doh_icon.png")), true);
         imgheadRIGHT.Colspan = 3;
         imgheadRIGHT.HorizontalAlignment = Element.ALIGN_RIGHT;
         imgheadRIGHT.FixedHeight = 20f;
         imgheadRIGHT.PaddingBottom = 10f;
         imgheadRIGHT.Border = Rectangle.NO_BORDER;
         myTable.AddCell(imgheadRIGHT);



          ////================ END HEADER =============\\\

         doc.Add(Chunk.NEWLINE);

         ////================ TABLE HEADER =============\\\

         PdfPCell CellOneHdr = new PdfPCell(new Phrase(" ", fntTableFontHdr));
         myTable.AddCell(CellOneHdr);
         PdfPCell CellTwoHdr = new PdfPCell(new Phrase("cell 2 Hdr ", fntTableFontHdr));
         myTable.AddCell(CellTwoHdr);
         PdfPCell CellTreeHdr = new PdfPCell(new Phrase("cell 3 Hdr", fntTableFontHdr));
         myTable.AddCell(CellTreeHdr);

         ////================ END TABLE HEADER =============\\\


         ////================ TABLE ROWS =============\\\
         for (int i = 0; i <= 550; i++ )
         {
             PdfPCell CellOne = new PdfPCell(new Phrase("R1 C1", fntTableFont));
             myTable.AddCell(CellOne);

             PdfPCell CellTwo = new PdfPCell(new Phrase("R1 C2 " + i.ToString(), fntTableFont));
             myTable.AddCell(CellTwo);

             PdfPCell Celltree = new PdfPCell(new Phrase("R1 C3", fntTableFont));
             myTable.AddCell(Celltree);
         }
       
        // PdfPCell Celltree2 = new PdfPCell(new Phrase("R1 C3123 asdad asdas", fntTableFont));
        // Celltree2.Colspan = 3;
        // myTable.AddCell(Celltree2);
        doc.Add(myTable);

        //int pageN = wri.PageNumber;
        //String text = "Page " +  pageN;

        //Paragraph footer = new Paragraph("footer test " + text);
        //footer.Alignment = Element.ALIGN_CENTER;
        //doc.Add(footer);

        ////================ END TABLE ROWS =============\\\

          ///================== FOOTER ===================\\\

      
      
          ////=========== END FOOTER =====================\\\\

     

         doc.Close();




        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "myscript", "window.open('Reports/" + strReportName + "','MyPDFDocument','toolbar=1,location=1,status=1,scrollbars=1,menubar=1,resizable=1,left=10,top=10,width=860,height=640');", true);

     
      
      }
     

}
   
