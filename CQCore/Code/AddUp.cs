using System.Collections.Generic;
using System.Dynamic;
using System;
using CQCore.Helper;
using System.Data.SqlClient;
using System.Data;

namespace CQCore.Code
{
    /// <summary>
    /// 加权类
    /// </summary>
    public class AddUp
    {
        /// <summary>
        /// 大加权
        /// </summary>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> djq(string checkItemCode)
        {
            //jyz 检验值  jz 净重  main 结果集
            SqlParameter[] parameters = { new SqlParameter("@checkItemCode", checkItemCode) };
            var list = SqlHelper.ExecuteDataTable(
                "select (sum(jyz*jz)/sum(jz)) as val from dbo.main ",
                parameters
            );
            var result = SqlHelper.ToDynamicList(list)[0];
            return new Dictionary<string, List<decimal>> { { "result", new List<decimal>() { Convert.ToDecimal(result.val) } } };
        }

        /// <summary>
        /// 简单加权
        /// </summary>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> jdjq(string checkItemCode)
        {
            //jyz 检验值  jz 净重  main 结果集
            SqlParameter[] parameters = { new SqlParameter("@checkItemCode", checkItemCode) };
            var list = SqlHelper.ExecuteDataTable(
                "select (sum(jyz)/count(*)) as val from dbo.main ",
                parameters
            );
            dynamic result = SqlHelper.ToDynamicList(list)[0];
            return new Dictionary<string, List<decimal>> { { "result", new List<decimal>() { Convert.ToDecimal(result.val) } } };
        }

        /// <summary>
        /// 同价格简单加权
        /// </summary>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> sameJdjq(string checkItemCode)
        {
            Dictionary<string, List<decimal>> result = new Dictionary<string, List<decimal>>();
            //jyz 检验值  jz 净重  main 结果集 price 价格
            SqlParameter[] parameters = { new SqlParameter("@checkItemCode", checkItemCode) };
            var list = SqlHelper.ExecuteDataTable(
                "select (sum(jyz)/count(*)) as val,price from dbo.main group by price",
                parameters
            );
            foreach (DataRow row in list.Rows)
            {
                result.Add(row["price"].ToString(), new List<decimal> { Convert.ToDecimal(row["val"]) });
            }

            return result;
        }

        /// <summary>
        /// 同价格大加权
        /// </summary>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> sameDjq(string checkItemCode)
        {
            Dictionary<string, List<decimal>> result = new Dictionary<string, List<decimal>>();
            //jyz 检验值  jz 净重  main 结果集 price 价格
            SqlParameter[] parameters = { new SqlParameter("@checkItemCode", checkItemCode) };
            var list = SqlHelper.ExecuteDataTable(
                "select (sum(jyz*jz)/sum(jz)) as val from dbo.main group by price",
                parameters
            );
            foreach (DataRow row in list.Rows)
            {
                result.Add(row["price"].ToString(), new List<decimal> { Convert.ToDecimal(row["val"]) });
            }

            return result;
        }

        /// <summary>
        /// 时间加权
        /// </summary>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> timeJq(string checkItemCode)
        {
            //取间隔天数
            SqlParameter[] parameters = { new SqlParameter() };
            int timeInterval = Convert.ToInt16(SqlHelper.ExecuteScalar("sql", parameters));
            //确定最小时间
            parameters = new SqlParameter[] { new SqlParameter() };
            DateTime timeStart = Convert.ToDateTime(SqlHelper.ExecuteScalar("sql", parameters));
            //确定最大时间
            parameters = new SqlParameter[] { new SqlParameter() };
            DateTime timeEnd = Convert.ToDateTime(SqlHelper.ExecuteScalar("sql", parameters));

            List<decimal> list = new List<decimal>();
            Dictionary<string, List<decimal>> dic = new Dictionary<string, List<decimal>>();
            for (int i = 0; i < (timeEnd - timeStart).Days; i += timeInterval)
            {
                DateTime curStart = timeStart.AddDays(i);
                DateTime curEnd = timeStart.AddDays(i + timeInterval);
                string sql = "select * from db.main where time between @curStart and @curEnd"; //按时间分段
                string jqsql = $"select (sum(jyz*jz)/sum(jz)) as val from {sql} group by price"; //对分段进行同价格大加权
                DataTable dt = SqlHelper.ExecuteDataTable(jqsql, parameters);
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(Convert.ToDecimal(dr["val"]));
                }

                dic.Add(curStart.ToString("yyyy-MM-dd"), list);
            }

            return dic;
        }

        /// <summary>
        /// 数量加权
        /// </summary>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> numJq(string checkItemCode)
        {
            //取间隔天数
            SqlParameter[] parameters = { new SqlParameter() };
            int timeInterval = Convert.ToInt16(SqlHelper.ExecuteScalar("sql", parameters));
            //确定最小时间
            parameters = new SqlParameter[] { new SqlParameter() };
            DateTime timeStart = Convert.ToDateTime(SqlHelper.ExecuteScalar("sql", parameters));
            //确定最大时间
            parameters = new SqlParameter[] { new SqlParameter() };
            DateTime timeEnd = Convert.ToDateTime(SqlHelper.ExecuteScalar("sql", parameters));

            List<decimal> list = new List<decimal>();
            Dictionary<string, List<decimal>> dic = new Dictionary<string, List<decimal>>();
            for (int i = 0; i < (timeEnd - timeStart).Days; i += timeInterval)
            {
                DateTime curStart = timeStart.AddDays(i);
                DateTime curEnd = timeStart.AddDays(i + timeInterval);
                string sql = "select * from db.main where time between @curStart and @curEnd"; //按时间分段
                string jqsql = $"select (sum(jyz*jz)/sum(jz)) as val from {sql}"; //对分段进行大加权（不考虑价格）
                DataTable dt = SqlHelper.ExecuteDataTable(jqsql, parameters);
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(Convert.ToDecimal(dr["val"]));
                }

                dic.Add(curStart.ToString("yyyy-MM-dd"), list);
            }

            return dic;
        }
        /// <summary>
        /// 批次加权
        /// </summary>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> batJq(string checkItemCode)
        {
            Dictionary<string, List<decimal>> result = new Dictionary<string, List<decimal>>();
            //jyz 检验值  jz 净重  main 结果集 pici 批次
            SqlParameter[] parameters = { new SqlParameter("@checkItemCode", checkItemCode) };
            var list = SqlHelper.ExecuteDataTable(
                "select (sum(jyz)/count(*)) as val,price from dbo.main group by pici",
                parameters
            );
            foreach (DataRow row in list.Rows)
            {
                result.Add(row["price"].ToString(), new List<decimal> { Convert.ToDecimal(row["val"]) });
            }

            return result;
        }
        /// <summary>
        /// 批次+同价位加权
        /// </summary>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> batSamejq(string checkItemCode)
        {
            Dictionary<string, List<decimal>> result = new Dictionary<string, List<decimal>>();
            //jyz 检验值  jz 净重  main 结果集 pici 批次 price 价格
            SqlParameter[] parameters = { new SqlParameter("@checkItemCode", checkItemCode) };
            var list = SqlHelper.ExecuteDataTable(
                "select (sum(jyz)/count(*)) as val,price from dbo.main group by pici,price",
                parameters
            );
            foreach (DataRow row in list.Rows)
            {
                result.Add(row["price"].ToString(), new List<decimal> { Convert.ToDecimal(row["val"]) });
            }

            return result;
        }
    }
}