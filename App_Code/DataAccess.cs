using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

/// <summary>
/// Summary description for DataAccess
/// </summary>
public class DataAccess
{
   // SqlConnection con = new SqlConnection();
    public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString);
   
    SqlCommand cmd = new SqlCommand();
	public DataAccess()
	{
       
        cmd.Connection = con;
		//
		// TODO: Add constructor logic here
		//
	}
    public bool exe_cmd(string query, SqlParameter[] param)
    {
        try
        {
            con.Open();
            cmd.CommandText = query;
            cmd.Parameters.AddRange(param);
            cmd.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool exe_cmd(string query)
    {
        try
        {
            con.Open();
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public DataTable exe_select(string query)
    {
        try
        {
            cmd.CommandText = query;
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            DataTable t = new DataTable();
            adap.Fill(t);
            return t;
        }
        catch
        {
            return null;
        }
    }
}