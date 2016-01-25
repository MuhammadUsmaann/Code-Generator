using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace CodeGenerator.MVP.DAL
{
    public class SPCreatorDAL
    {
        public static SqlConnection con = null;
        
        //SELECT name AS DATABASENAME FROM master.dbo.sysdatabases --SqlServer Database names
        //select * from all_users; OR select username from dba_users;  --Oracle user names
        
        public static DataTable GetTables()
        {
            string asdasd = Util.Utility.GetConnectionString();
            string asdasas = @"Data Source=USMAN;Initial Catalog=EducationSystemDesktop;User ID=admin;Pwd=Netsolpk1;Connect Timeout=15";
            con = new SqlConnection(asdasd);
            con.Open();
            SqlCommand cmd = null;

            if (Util.Utility.GetSelectedDB().Equals("Oracle"))
            {
                cmd = new SqlCommand("SELECT T.TABLE_NAME FROM tabs T Order by T.TABLE_NAME", con);
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                cmd = new SqlCommand("Select T.Name AS TABLE_NAME from sys.Tables T Order by Name", con);
            }

            SqlDataReader dr = null;

            DataTable dt = new DataTable();
            dt.Columns.Add("TABLE_NAME");

            try
            {
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DataRow drow = dt.NewRow();
                    drow["TABLE_NAME"] = dr["TABLE_NAME"].ToString();

                    dt.Rows.Add(drow);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
                //MessageBox.Show(ex.Message, "CodeGenerator.MVP", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dr.Close(); dr.Dispose();
                con.Close(); con.Dispose();
            }

            return dt;
        }

        public static DataTable GetColumnsDesc(string strTableName)
        {
            con = new SqlConnection(Util.Utility.GetConnectionString());
            con.Open();
            SqlCommand cmd = null;

            if (Util.Utility.GetSelectedDB().Equals("Oracle"))
            {
                cmd = new SqlCommand("SELECT t.COLUMN_NAME, t.DATA_TYPE, t.CHAR_LENGTH AS MAX_LENGTH, NVL(t.DATA_PRECISION, 0) AS PRECISION, NVL(t.DATA_SCALE, 0) AS SCALE from user_tab_columns t where t.TABLE_NAME = '" + strTableName + "'", con);
            }
            else if (Util.Utility.GetSelectedDB().Equals("SqlServer"))
            {
                cmd = new SqlCommand("SELECT cl.is_identity as IS_IDENTITY, cl.name AS COLUMN_NAME, tp.name AS DATA_TYPE, cl.Max_Length AS MAX_LENGTH, cl.Precision AS PRECISION, cl.Scale AS SCALE FROM sys.columns cl JOIN  sys.systypes tp ON tp.xtype = cl.system_type_id WHERE object_id = OBJECT_ID('dbo." + strTableName + "') AND tp.status = 0 Order by cl.Column_ID", con);
            }



            SqlDataReader dr = null;
            
            DataTable dt = new DataTable();
            dt.Columns.Add("COLUMN_NAME");
            dt.Columns.Add("DATA_TYPE");
            dt.Columns.Add("MAX_LENGTH");
            dt.Columns.Add("PRECISION");
            dt.Columns.Add("SCALE");
            dt.Columns.Add("IS_IDENTITY");

            try
            {
                string primaryKeyQuery = "SELECT column_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1 AND table_name = '" + strTableName + "'";
                SqlCommand m = new SqlCommand(primaryKeyQuery, con);
                SqlDataReader dr1 = m.ExecuteReader();

                string primary = "";
                while (dr1.Read())
                {
                    primary = dr1["column_name"].ToString();
                }
                dr1.Close();

                dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    DataRow drow = dt.NewRow();
                    if (primary.ToLower() == dr["COLUMN_NAME"].ToString().ToLower())
                    {
                        drow["IS_IDENTITY"] = "1";
                    }
                    drow["COLUMN_NAME"] = dr["COLUMN_NAME"].ToString();
                    drow["DATA_TYPE"] = dr["DATA_TYPE"].ToString();
                    drow["MAX_LENGTH"] = dr["MAX_LENGTH"].ToString();
                    drow["PRECISION"] = dr["PRECISION"].ToString();
                    drow["SCALE"] = dr["SCALE"].ToString();
                    //drow["IS_IDENTITY"] = dr["IS_IDENTITY"].ToString();

                    dt.Rows.Add(drow);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
                //MessageBox.Show(ex.Message, "CodeGenerator.MVP", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dr.Close();  dr.Dispose();
                con.Close(); con.Dispose();
            }

            return dt;
        }

    }
}
