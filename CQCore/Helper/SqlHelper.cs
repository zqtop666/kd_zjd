using System.Data.SqlClient;
using System.Configuration;
using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Dynamic;

namespace CQCore.Helper
{
    class SqlHelper
    {
        public static int ExecuteNonQuery(string sql, params SqlParameter[] paramters)
        {
            String connStr = ConfigurationManager.AppSettings["KingDee"];
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    foreach (SqlParameter parameter in paramters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] paramters)
        {
            String connStr = ConfigurationManager.AppSettings["KingDee"];
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    foreach (SqlParameter parameter in paramters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }

        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters)
        {
            String connStr = ConfigurationManager.AppSettings["KingDee"];
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    foreach (SqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(ds);
                    return ds.Tables[0];
                }
            }
        }
        public static List<dynamic> ToDynamicList(DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;
        }
    }
}