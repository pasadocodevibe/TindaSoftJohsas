using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Text;

public partial class pdf : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public void htmlstr()
    {
        StringBuilder sb = new StringBuilder();
        string filename = "A";
        sb.Append(" <html><body><table width='100%' cellspacing='0' cellpadding='2'  >");
        sb.Append("<tr><td colspan='4'></td></tr>");
       // sb.Append("<tr><td></td><th><img id='alogo'  height='75' width=75' src=" + Server.MapPath("~/images/ logo.png") + "></th><td><img id='xlogo'  height='75' width=75' src=" + Server.MapPath("~/images/ logo.png") + "></td><td></td></tr>");
        sb.Append("<tr><td colspan='4'>Date :" + DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) + "</td></tr>");
        sb.Append("<tr><td colspan='4'></td></tr>");
        sb.Append("<tr><td colspan='4'>Hello</td></tr>");
        sb.Append("<tr><td colspan='4'></td></tr>");
        sb.Append("<tr><td colspan='4'></td></tr>");
        sb.Append("<tr><td></td><td colspan='2'>test matter</td><td></td></tr>");
        sb.Append("<tr><td></td><td colspan='2'>test matter</td><td></td></tr>");
        sb.Append("<tr><td></td><td colspan='2'>test matter</td><td></td></tr>");

        sb.Append("<tr><td colspan='4'></td></tr>");
        sb.Append("<tr><td></td><td colspan='2'> test</td><td></td></tr>");
        sb.Append("<tr><td></td><td colspan='2'>test</td><td></td></tr>");
        sb.Append("<tr><td></td><td colspan='2'>test</td><td></td></tr>");
        sb.Append("<tr><td colspan='4'></td></tr>");
        sb.Append("<tr><td colspan='4'></td></tr>");
        sb.Append("<tr><td></td><td colspan='2'>Test footer</td><td></td></tr>");
        sb.Append(" </table></body></html>");
        divpdf.InnerHtml = sb.ToString();
    }
    protected void btnHTML_Click(object sender, EventArgs e)
    {
        htmlstr();
        string filename = "A";
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".pdf");
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        StringWriter sw = new StringWriter();
        HtmlTextWriter hw = new HtmlTextWriter(sw);
        divpdf.RenderControl(hw);
        StringReader sr = new StringReader(sw.ToString());
        Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 100f, 0f);
        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        pdfDoc.Open();
        htmlparser.Parse(sr);
        pdfDoc.Close();
        Response.Write(pdfDoc);
        Response.End();
    }
}