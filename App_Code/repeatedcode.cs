using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Globalization;


/// <summary>
/// Summary description for repeatedcode
/// </summary>
public class repeatedcode
{

    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
    public SqlCommand sqlcmd, cmd;
    public SqlDataReader rdr, dr;
    public string sqlstr;
    DateTime pacificdatenow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Taipei Standard Time");
	public repeatedcode()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public void list_papersize(DropDownList dropdown)
    {
        var items = new List<string> {
            "Select...",
        "Letter",
        "Legal",
        "Executive",
        "A3",
        "A4", 
        "A5", 
        "Tabloid"
        };
        //  items.Sort();
        dropdown.DataSource = items;

        dropdown.DataBind();

        //'' Dim pgSize As New iTextSharp.text.Rectangle(myWidth, myHeight) 
        // '''Dim doc As New iTextSharp.text.Document(pgSize, leftMargin, rightMargin, topMargin, bottomMargin)

        // ''  Rectangle pgSize = new Rectangle(myWidth, myHeight);

        //  Document doc = new Document(new iTextSharp.text.Rectangle(295f, 420f), 0f, 0f, 0f, 0f);
    }
    public string get_papersize(string sitename, string usersetid)
    {
        string papersize = "Letter";
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        string qryname = "(select psitename from ref_rolepermission where permissionid=reportname)";
        string qry = "select * from ref_reportsettings where reportsetid=@setid and " + qryname + " =@sitename  ";
        using (SqlCommand cmd = new SqlCommand(qry, con))
        {

            cmd.Parameters.AddWithValue("@setid", usersetid);
            cmd.Parameters.AddWithValue("@sitename", sitename);
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                if (rdr[2] != DBNull.Value)
                {
                    papersize = rdr[2].ToString();
                    //    decimal la= Convert.ToDecimal(rdr[5]); 
                    ///  float l = la;

                }
            
            }
         
            con.Close();
            rdr.Close();
        }

        return papersize;

    }

    public int get_margin(string margincolumn, string sitename, string setid)
    {
        int val = 40;
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        string qryname = "(select psitename from ref_rolepermission where permissionid=reportname)";
        string qry = "select " + margincolumn + " from ref_reportsettings where reportsetid=@setid and " + qryname + " =@sitename  ";
        using (SqlCommand cmd = new SqlCommand(qry, con))
        {

            cmd.Parameters.AddWithValue("@setid", setid);
            cmd.Parameters.AddWithValue("@sitename", sitename);
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                if (rdr[0] != DBNull.Value)
                {
                    val = Convert.ToInt32(rdr[0].ToString());

                }

            }
            con.Close();
            rdr.Close();
        }

        return val;

    }
    public string get_logo(string margincolumn, string sitename, string setid)
    {
        string val = "~/images/storelogo/blank.jpg";
        con = new SqlConnection(con.ConnectionString);
        con.Open();
        string qryname = "(select psitename from ref_rolepermission where permissionid=reportname)";
        string qry = "select " + margincolumn + " from ref_reportsettings where reportsetid=@setid and " + qryname + " =@sitename  ";
        using (SqlCommand cmd = new SqlCommand(qry, con))
        {

            cmd.Parameters.AddWithValue("@setid", setid);
            cmd.Parameters.AddWithValue("@sitename", sitename);
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                if (rdr[0] != DBNull.Value && rdr[0] !="")
                {
                    val =rdr[0].ToString();

                }

            }
            con.Close();
            rdr.Close();
        }

        return val;

    }
    public int audit_trail_add(string userid, string action, string details, string setid)
    {
        int stat = 0;
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        SqlCommand cmd = new SqlCommand("insert into [ref_auditlog]  (loguserid,logaction,logdetails,logdate,logsetid) values(@d1,@d2,@d3,@d4,@d5)", con);

        cmd.Parameters.AddWithValue("@d4", pacificdatenow.ToString());
        cmd.Parameters.AddWithValue("@d1", userid);
        cmd.Parameters.AddWithValue("@d2", action);
        cmd.Parameters.AddWithValue("@d3", details);
        cmd.Parameters.AddWithValue("@d5", setid);
        int res = cmd.ExecuteNonQuery();
        if (res > 0)
        {
            stat = 1;
        }
        else
        {
            stat = 0;
        }
        con.Close();
        return stat;

    }
 
    public string Decrypt(string str)
    {

        str = str.Replace(" ", "+");
        string DecryptKey = "2013;[pnuLIT)WebCodeExpert";
        MemoryStream ms = new MemoryStream();
        System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        try
        {
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
         
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
          
        }
        catch (Exception) { }
        return encoding.GetString(ms.ToArray());


    }
    public string Encrypt(string str)
    {
        string EncrptKey = "2013;[pnuLIT)WebCodeExpert";
        MemoryStream ms = new MemoryStream();
        try
        {

            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
         
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
        }
        catch (Exception)
        {

        }
        return Convert.ToBase64String(ms.ToArray());
    }
    public int get_userid(string user)
    {
        int uid = 0;
        if (user != null)
        {

            string usname =user;

            con = new SqlConnection(con.ConnectionString);
            con.Open();

            string ct = "Select uid  from ref_account where usname = '" + usname + "'";
            cmd = new SqlCommand(ct);
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {

                

                uid = Convert.ToInt32( rdr[0].ToString());


            }
           
            con.Close();
            rdr.Close();
        }
       
        return uid;
    }
    public int get_usersetid(string user)
    {
        int uid = 0;
        if (user != null)
        {

            string usname = user;

            con = new SqlConnection(con.ConnectionString);
            con.Open();

            string ct = "Select  usetid from ref_account where usname = '" + usname + "'";
            cmd = new SqlCommand(ct);
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {



                uid = Convert.ToInt32(rdr[0].ToString());


            }

            con.Close();
            rdr.Close();
        }

        return uid;
    }
    public string get_onestringvalue(string qry)
    {
        string uid = "";

            con = new SqlConnection(con.ConnectionString);
            con.Open();

            cmd = new SqlCommand(qry);
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {

                if (rdr[0] != DBNull.Value)
                {

                    uid = rdr[0].ToString();
                }
               


            }

            con.Close();
            rdr.Close();
        

        return uid;
    }
    public string get_userlevel(string user)
    {
        string uid ="";
        if (user != null)
        {

            string usname = user;

            con = new SqlConnection(con.ConnectionString);
            con.Open();

            string ct = "Select  uroleid, (Select  levelname ref_role levelid=uroleid) as [aclevel] from ref_accounts where usname = '" + usname + "'";
            cmd = new SqlCommand(ct);
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {



                uid = rdr[2].ToString();


            }

            con.Close();
            rdr.Close();
        }

        return uid;
    }
    public int get_userroleid(string user)
    {
      int uid = 0;
        if (user != null)
        {

            string usname = user;

            con = new SqlConnection(con.ConnectionString);
            con.Open();

            string ct = "Select  uroleid from ref_account where usname = '" + usname + "'";
            cmd = new SqlCommand(ct);
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {

                if (rdr[0] != DBNull.Value)
                {

                    uid = Convert.ToInt32(rdr[0].ToString());
                }
                else
                {
                    uid = 0;
                }


            }

            con.Close();
            rdr.Close();
        }

        return uid;
    }
    public string get_style(string setid)
    {
        string setstyle = "distribution/css/style.default.css";
        if (setid != null)
        {

          

            con = new SqlConnection(con.ConnectionString);
            con.Open();

            string ct = "Select  setstyle from ref_generalsettings where setid = " + setid + "";
            cmd = new SqlCommand(ct);
            cmd.Connection = con;
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {

                if (rdr[0] != DBNull.Value)
                {

                    setstyle = rdr[0].ToString();
                }
                else
                {
                    setstyle = "distribution/css/style.default.css";
                }


            }

            con.Close();
            rdr.Close();
        }

        return setstyle;
    }
    public void dropdown(string qry, DropDownList dplist)
    {

        //try
        //{
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            String sql = "SELECT " + qry + "";
            cmd = new SqlCommand(sql, con);
            rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            dplist.Items.Clear();
            while (rdr.Read() == true)
            {
                if (rdr[0] != DBNull.Value)
                {
                    dplist.Items.Add(rdr[0].ToString());
                }

            }
            dplist.Items.Insert(0, new ListItem("Select...", "Select..."));
            con.Close();
        //}
        //catch (Exception ex)
        //{
        //    //ShowMessage(ex.ToString(), MessageType.Error);
        //}



    }
    public string  pacificdaterange_minimun()
    {
      
        return pacificdatenow.AddDays(-31).ToShortDateString();
       
    }
    public string pacificdaterange_maximum()
    {
      return pacificdatenow.ToShortDateString();
    }
    public void dropdownnodefault(string qry, DropDownList dplist)
    {

        //try
        //{
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        String sql = "SELECT " + qry + "";
        cmd = new SqlCommand(sql, con);
        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        dplist.Items.Clear();
        while (rdr.Read() == true)
        {
            dplist.Items.Add(rdr.GetString(0));

        }
     
        con.Close();
        //}
        //catch (Exception ex)
        //{
        //    //ShowMessage(ex.ToString(), MessageType.Error);
        //}



    }
    public void dropdown_branch(string qry, DropDownList dplist)
    {

       // try
      //  {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            String sql = "SELECT " + qry + "";
            cmd = new SqlCommand(sql, con);
            rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            dplist.Items.Clear();
            while (rdr.Read() == true)
            {
                dplist.Items.Add(rdr["setcompanyname"].ToString() + " # " + rdr["setid"].ToString());

            }
            dplist.Items.Insert(0, new ListItem("All", "All"));
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        //}
        //catch (Exception ex)
        //{
        //    //ShowMessage(ex.ToString(), MessageType.Error);
        //}



    }
    public void dropdown_idtext(DropDownList dplist, string fromqry, string id, string text)
    {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        dplist.Items.Clear();
        string com = "Select " + id + ", " + text + " from " + fromqry + " order by " + text + " asc";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        dplist.DataSource = dt;
        dplist.DataBind();
        dplist.DataTextField = text;
        dplist.DataValueField = id;
        dplist.DataBind();
       
            dplist.Items.Insert(0, new ListItem("Select...", "Select..."));
        
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
    }
    public void dropdown_idtextdefaultzero(DropDownList dplist, string fromqry, string id, string text)
    {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        dplist.Items.Clear();
        string com = "Select " + id + ", " + text + " from " + fromqry + " order by " + text + " asc";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        dplist.DataSource = dt;
        dplist.DataBind();
        dplist.DataTextField = text;
        dplist.DataValueField = id;
        dplist.DataBind();

        dplist.Items.Insert(0, new ListItem("Select...", "0"));

        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
    }
    public void dropdown_idtextrate(DropDownList dplist, string fromqry, string id, string text )
    {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        dplist.Items.Clear();
        string com = "Select " + id + ",  " + text + " as [val] from " + fromqry + " ";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        dplist.DataSource = dt;
        dplist.DataBind();
        dplist.DataTextField = "val";
        dplist.DataValueField = id;
        dplist.DataBind();

        dplist.Items.Insert(0, new ListItem("Select...", "Select..."));

        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
    }
    public void dropdown_branchselect(string qry, DropDownList dplist)
    {

        // try
        //  {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        String sql = "SELECT " + qry + "";
        cmd = new SqlCommand(sql, con);
        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        dplist.Items.Clear();
        while (rdr.Read() == true)
        {
            dplist.Items.Add(rdr["branchname"].ToString() + " # " + rdr["branchid"].ToString());

        }
        dplist.Items.Insert(0, new ListItem("Select...", "Select..."));
        con.Close();
        //}
        //catch (Exception ex)
        //{
        //    //ShowMessage(ex.ToString(), MessageType.Error);
        //}



    }
    public string  loop_branch()
    {
        string val = "";
        // try
        //  {
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        String sql = "SELECT branchname from ref_branch where branchid !=0 ";
        cmd = new SqlCommand(sql, con);
        rdr = cmd.ExecuteReader();

      
        while (rdr.Read())
        {
            val += "'" + rdr["branchname"] + "', ";

        }
        if(val.Length > 2)
        {
            val = val.Remove(val.Length - 2);
        }

        con.Close();
        rdr.Close();
        //}
        //catch (Exception ex)
        //{
        //    //ShowMessage(ex.ToString(), MessageType.Error);
        //}

        return val;

    }
    public int access_user(string usname, string sitename, string columncrud)
    {
        int statrole = 0;
        string constr = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("Select * FROM [ref_rolepermission] WHERE proleid =@proleid and psitename=@psitename"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@proleid", get_userroleid(usname));
                cmd.Parameters.AddWithValue("@psitename", sitename);

                cmd.Connection = conn;
                conn.Open();
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    if (rdr[columncrud] != DBNull.Value)
                    {
                        statrole = Convert.ToInt32(rdr[columncrud]);
                    }
                    else
                    {
                        statrole = 0;
                    }


                }
                else
                {
                    statrole = 0;
                }

                conn.Close();
            }

        }
        return statrole;
    }
    public int identify_counter(string qry)
    {
        Int32 stat = 0;
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        string stm = "SELECT COUNT(*) FROM " + qry + ";";
        sqlcmd = new SqlCommand(stm, con);
        stat = Convert.ToInt32(sqlcmd.ExecuteScalar());

        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }

        return stat;
       
    }
    public double identify_sum(string column,string qry)
    {
        
        double stat = 0;
        try
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            string stm = "SELECT SUM(" + column + ") FROM " + qry + ";";
            sqlcmd = new SqlCommand(stm, con);
            stat = Convert.ToDouble(sqlcmd.ExecuteScalar());

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        catch (Exception ex) { stat = 0; }
        return stat;

    }
    public int get_sumvalue(string column,string afterfromqry)
    {
        Int32 stat = 0;
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        string stm = "SELECT sum(" + column + " ) FROM " + afterfromqry + " ";
        sqlcmd = new SqlCommand(stm, con);
        //stat = Convert.ToInt32(sqlcmd.ExecuteScalar());
        rdr = sqlcmd.ExecuteReader();
        if (rdr.Read())
        {
            if (rdr[0] != DBNull.Value)
            {
                stat = Convert.ToInt32(rdr[0]);
            }
            else
            {
                stat = 0;
            }
        }
        else
        {

            stat = 0;
        }
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }

        return stat;

    }
    public int get_sumvaluedynamic(string qry)
    {
        Int32 stat = 0;
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        string stm = qry;
        sqlcmd = new SqlCommand(stm, con);
        //stat = Convert.ToInt32(sqlcmd.ExecuteScalar());
        rdr = sqlcmd.ExecuteReader();
        if (rdr.Read())
        {
            if (rdr[0] != DBNull.Value)
            {
                stat = Convert.ToInt32(rdr[0]);
            }
            else
            {
                stat = 0;
            }
        }
        else
        {

            stat = 0;
        }
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }

        return stat;

    }
    public string dpgetstringafter(DropDownList txt, int characters)
    {
        string str = Convert.ToString(txt.Text);
        string after = null;
        if (characters == 1)
        {
            after = str.Split('#').LastOrDefault();

        }
        if (characters == 2)
        {
            after = str.Split('@').LastOrDefault();
        }
        if (characters == 3)
        {
            after = str.Split('{').LastOrDefault();
        }
        if (characters == 4)
        {
            after = str.Split('|').LastOrDefault();
        }
        if (characters == 5)
        {
            after = str.Split(':').LastOrDefault();
        }
        return after.ToString();
    }
    public string txtgetstringafter(TextBox txt, int characters)
    {
        string str = Convert.ToString(txt.Text);
        string after = null;
        if (characters == 1)
        {
            after = str.Split('#').LastOrDefault();

        }
        if (characters == 2)
        {
            after = str.Split('@').LastOrDefault();
        }
        if (characters == 3)
        {
            after = str.Split('{').LastOrDefault();
        }
        if (characters == 4)
        {
            after = str.Split('|').LastOrDefault();
        }
        if (characters == 5)
        {
            after = str.Split(':').LastOrDefault();
        }
        return after.ToString();
    }
    public string txtgetstringafter_cashround(Label txt)
    {
        string str = Convert.ToString(txt.Text);
        string after = null;
    
            after = str.Split('.').LastOrDefault();

        
        
        return after.ToString();
    }
    public string textgetstringbefore(TextBox txt)
    {
        string str = Convert.ToString(txt.Text);
        string extbefore = str.Substring(0, str.LastIndexOf("|") - 1);

        return extbefore.ToString();

            
    }
    public string dpgetstringbefore(DropDownList txt)
    {
        string str = Convert.ToString(txt.Text);
        string extbefore = str.Substring(0, str.LastIndexOf("|") - 1);

        return extbefore.ToString();


    }
    public string dpgetstringbeforeat(DropDownList txt)
    {
        string str = Convert.ToString(txt.Text);
        string extbefore = str.Substring(0, str.LastIndexOf("@") - 1);

        return extbefore.ToString();


    }
    public string betweenStrings(string text, string start, string end)
    {
        int p1 = text.IndexOf(start) + start.Length;
        int p2 = text.IndexOf(end, p1);

        if (end == "") return (text.Substring(p1));
        else return text.Substring(p1, p2 - p1);
    }
    public string footerinfo_gridview(GridView gv_masterlist)
    {
        string rvalue = "";
        //Showing Numbers in Label
        int iTotalRecords = ((DataTable)(gv_masterlist.DataSource)).Rows.Count;
        int iEndRecord = gv_masterlist.PageSize * (gv_masterlist.PageIndex + 1);
        int iStartsRecods = iEndRecord - gv_masterlist.PageSize;

        if (iEndRecord > iTotalRecords)
        {

            iEndRecord = iTotalRecords;

        }
        if (iStartsRecods == 0)
        {

            iStartsRecods = 1;

        }

        if (iEndRecord == 0)
        {

            iEndRecord = iTotalRecords;

        }
        int currentPage = gv_masterlist.PageIndex + 1;
        string footerval = "Page " + currentPage.ToString("N0") + " of " + gv_masterlist.PageCount.ToString("N0");
        if (gv_masterlist.Rows.Count > 0)
        {
            if (iStartsRecods >= 10)
            {
                string abs = (iStartsRecods + 1).ToString("N0") + " to " + iEndRecord.ToString("N0") + " of " + iTotalRecords.ToString("N0") + " entries ";
                rvalue = "Showing " + abs.ToString() + " | " + footerval.ToString();
            }
            else
            {
                string abs = iStartsRecods.ToString("N0") + " to " + iEndRecord.ToString("N0") + " of " + iTotalRecords.ToString("N0") + " entries ";
                rvalue = "Showing " + abs.ToString() + " | " + footerval.ToString();
            }
        }
        else
        {
            rvalue = "No record found.";
        }
        return rvalue;
    }


    public iTextSharp.text.Document documentstyleload(string sitename, string username)
    {
        iTextSharp.text.Document doc = new iTextSharp.text.Document();
        //string sitename = "ExpenseReport";
        string setids = get_usersetid(username).ToString();
        string pagesize = get_papersize(sitename, setids);
        //string pageorientation = rp.get_orientation("pageOrientation", sitename, setids);
        string leftlogo = get_logo("logolefturl", sitename, setids);
        string rightlogo = get_logo("logorighturl", sitename, setids);
        int ml = get_margin("borderleft", sitename, setids);
        int mr = get_margin("borderright", sitename, setids);
        int mt = get_margin("bordertop", sitename, setids);
        int mb = get_margin("borderbottom", sitename, setids);
        string pageOrientation = get_logo("pageOrientation", sitename, setids);
        if (pagesize == "Letter")
        {
            if (pageOrientation == "Landscape")
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER.Rotate(), ml, mr, mt, mb);
            }
            else
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, ml, mr, mt, mb);
            }
        }
        if (pagesize == "Legal")
        {
            if (pageOrientation == "Landscape")
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LEGAL.Rotate(), ml, mr, mt, mb);

            }
            else
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LEGAL, ml, mr, mt, mb);
            }
        }
        if (pagesize == "Executive")
        {
            if (pageOrientation == "Landscape")
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.EXECUTIVE.Rotate(), ml, mr, mt, mb);

            }
            else
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.EXECUTIVE, ml, mr, mt, mb);
            }

        }
        if (pagesize == "A3")
        {
            if (pageOrientation == "Landscape")
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A3.Rotate(), ml, mr, mt, mb);
            }
            else
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A3, ml, mr, mt, mb);
            }
        }
        if (pagesize == "A4")
        {
            if (pageOrientation == "Landscape")
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), ml, mr, mt, mb);
            }
            else
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, ml, mr, mt, mb);
            }
        }
        if (pagesize == "Tabloid")
        {
            if (pageOrientation == "Landscape")
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.TABLOID.Rotate(), ml, mr, mt, mb);
            }
            else
            {
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.TABLOID, ml, mr, mt, mb);
            }
        }
        return doc;
    }
    public void bindlist_filterdate(DropDownList DropDownList1)
    {
        string[] controlArray = {"Not Applicable", "Today", "Yesterday", "Last 7 Days", "Last 30 Days", 
                            "This Month", "Last Month", "This Year", "Custom Range" };

        DropDownList1.DataSource = controlArray;
        DropDownList1.DataBind();

    }
    public void filterdate_template(DropDownList dp_filterby, TextBox txt_datefrom, TextBox txt_dateto)
    {
        if (dp_filterby.SelectedIndex != 0)
        {
            DateTime dt_today = pacificdatenow;
            if (dp_filterby.Text == "Today")
            {
                txt_datefrom.Text = dt_today.ToString("yyyy-MM-dd");
                txt_dateto.Text = dt_today.ToString("yyyy-MM-dd");
            }
            if (dp_filterby.Text == "Yesterday")
            {
                txt_datefrom.Text = dt_today.AddDays(-1).ToString("yyyy-MM-dd");
                txt_dateto.Text = dt_today.AddDays(-1).ToString("yyyy-MM-dd");
            }
            if (dp_filterby.Text == "Last 7 Days")
            {
                txt_datefrom.Text = dt_today.AddDays(-7).ToString("yyyy-MM-dd");
                txt_dateto.Text = dt_today.ToString("yyyy-MM-dd");
            }
            if (dp_filterby.Text == "Last 30 Days")
            {
                txt_datefrom.Text = dt_today.AddDays(-30).ToString("yyyy-MM-dd");
                txt_dateto.Text = dt_today.ToString("yyyy-MM-dd");
            }
            if (dp_filterby.Text == "This Month")
            {
                var firstDayOfMonth = new DateTime(dt_today.Year, dt_today.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                txt_datefrom.Text = firstDayOfMonth.ToString("yyyy-MM-dd");
                txt_dateto.Text = lastDayOfMonth.ToString("yyyy-MM-dd");
            }
            if (dp_filterby.Text == "Last Month")
            {
                DateTime LastMonthLastDate = dt_today.AddDays(0 - dt_today.Day);
                DateTime LastMonthFirstDate = LastMonthLastDate.AddDays(1 - LastMonthLastDate.Day);

                txt_datefrom.Text = LastMonthFirstDate.ToString("yyyy-MM-dd");
                txt_dateto.Text = LastMonthLastDate.ToString("yyyy-MM-dd");
            }
            if (dp_filterby.Text == "This Year")
            {
                int year = dt_today.Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                DateTime lastDay = new DateTime(year, 12, 31);
                txt_datefrom.Text = firstDay.ToString("yyyy-MM-dd");
                txt_dateto.Text = lastDay.ToString("yyyy-MM-dd");

            }
            if (dp_filterby.Text == "Custom Range")
            {
                txt_datefrom.Enabled = true;
                txt_dateto.Enabled = true;
                txt_datefrom.Text = "";
                txt_dateto.Text = "";
                txt_datefrom.Focus();
            }
        }
        else
        {
            txt_datefrom.Enabled = false;
            txt_dateto.Enabled = false;
            txt_datefrom.Text = "";
            txt_dateto.Text = "";
        }
    }

}