using System.Data;
using System.Text;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using CQCore.Helper;

namespace CQCore.Code
{
    /// <summary>
    /// 价格计算类
    /// </summary>
    public class Price
    {
        /// <summary>
        /// 各种加权
        /// </summary>
        /// <param name="materialCode">物料编号</param>
        /// <param name="addUps">加权名称组合</param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> getAddUp(string materialCode, string checkItemCode, List<string> addUps)
        {
            Dictionary<string, List<decimal>> value = new Dictionary<string, List<decimal>>();
            AddUp addUpClass = new AddUp();
            foreach (string addUp in addUps)
            {
                string addUpLower = addUp.ToLower();
                switch (addUpLower)
                {
                    case "简单加权":
                        value = addUpClass.jdjq(checkItemCode);
                        break;
                    case "大加权":
                        value = addUpClass.djq(checkItemCode);
                        break;
                    case "同价格简单加权":
                        value = addUpClass.sameJdjq(checkItemCode);
                        break;
                    case "同价格大加权":
                        value = addUpClass.sameDjq(checkItemCode);
                        break;
                    case "时间加权":
                        value = addUpClass.timeJq(checkItemCode);
                        break;
                    case "数量加权":
                        value = addUpClass.numJq(checkItemCode);
                        break;
                    case "批次加权":
                        value = addUpClass.batJq(checkItemCode);
                        break;
                    case "批次同价位加权":
                        value = addUpClass.batSamejq(checkItemCode);
                        break;
                }
            }

            return value;
        }

        /// <summary>
        /// 获取物料和检验内容
        /// </summary>
        /// <param name="materialCode">物料编号</param>
        /// <param name="checkItemCode">质检单号</param>
        /// <returns></returns>
        public List<dynamic> getCheckItemInfo(string materialCode, string checkItemCode)
        {
            var list = new DataTable();
            if (string.IsNullOrEmpty(checkItemCode))
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@materialCode", materialCode),
                    new SqlParameter("@checkItemCode", checkItemCode)
                };
                list = SqlHelper.ExecuteDataTable(
                    "select * from dbo.main where materialCode=@materialCode and checkItemCode=@checkItemCode",
                    parameters
                );
            }
            else
            {
                SqlParameter[] parameters = { new SqlParameter("@materialCode", materialCode), };
                list = SqlHelper.ExecuteDataTable(
                    "select * from dbo.main where materialCode=@materialCode",
                    parameters
                );
            }

            var result = SqlHelper.ToDynamicList(list);
            return result;
        }
    }
}