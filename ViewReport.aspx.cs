using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;

public partial class ViewReport : System.Web.UI.Page
{
    repeatedcode rp = new repeatedcode();
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Request.QueryString["id"] != null)
        {

           string id =Request.QueryString["id"].ToString();


            string FilePath = Server.MapPath("temp_report/" + id );
            WebClient User = new WebClient();
            Byte[] filebuffer = User.DownloadData(FilePath);
            if (filebuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", filebuffer.Length.ToString());
                Response.BinaryWrite(filebuffer);


             //   File.Delete(Server.MapPath("~/temp_report/") + id);
            }
        }
        else if (Request.QueryString["receiptid"] != null)
        {

            string id = Request.QueryString["receiptid"].ToString();


            string FilePath = Server.MapPath("temp_report/receipt/" + id);
            WebClient User = new WebClient();
            Byte[] filebuffer = User.DownloadData(FilePath);
            if (filebuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", filebuffer.Length.ToString());
                Response.BinaryWrite(filebuffer);

              //  File.Delete(Server.MapPath("~/temp_report/receipt/") + id);
            }
        }
        else
        {
            Page.Response.Redirect("Home.aspx");
        }
    }
}