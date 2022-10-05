using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Api.BLL
{
    public class OrderDepositBLL
    {
        internal static List<OrderDeposit> GetOrderDepositList(OrderInfoParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<OrderDeposit> recordList = new List<OrderDeposit>();
            total = 0;

            return recordList;
        }
        internal static OrderDeposit GetOrderDeposit(OrderInfoParam searchParam)
        {
            string sql = @"SELECT `ID`,
                                `OrderNo`,
                                `OpenID`,
                                `PayTotal`,
                                `ApplyTotal`,
                                `ApplyTime`,
                                `DeductionTotal`,
                                `DeductionReason`,
                                `RefundTotal`,
                                `RefundTime`,
                                `PenaltyTotal`,
                                `Status`
                            FROM `mt_order_deposit`
                            WHERE `OrderNo` = @OrderNo; ";

            List<MySqlParameter> param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@OrderNo", searchParam.OrderNo));

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                return GetOrderDepositByRow(dt.Rows[0]);
            }
            else
            {
                throw new MsgException("订单押金不存在！");
            }
        }

        public static bool Refund(OrderDeposit param)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"update mt_order_deposit 
                    set Status=@Status,
                        ApplyTotal=@ApplyTotal,
                        ApplyTime=now(),
                        DeductionTotal=@DeductionTotal,
                        DeductionReason=@DeductionReason,
                        RefundTotal=@RefundTotal,
                        PenaltyTotal=@PenaltyTotal
                    where ID = @ID",
            new MySqlParameter("@ID", param.ID),
                new MySqlParameter("@Status", param.Status),
                new MySqlParameter("@ApplyTotal", param.ApplyTotal),
                new MySqlParameter("@DeductionTotal", param.DeductionTotal),
                new MySqlParameter("@DeductionReason", param.DeductionReason),
                new MySqlParameter("@RefundTotal", param.RefundTotal),
                new MySqlParameter("@PenaltyTotal", param.PenaltyTotal));
            return true;
        }


        private static OrderDeposit GetOrderDepositByRow(DataRow row)
        {
            OrderDeposit item = new OrderDeposit
            {
                ID = Converter.TryToInt64(row["ID"]),
                OrderNo = Converter.TryToString(row["OrderNo"]),
                OpenID = Converter.TryToString(row["OpenID"]),
                PayTotal = Converter.TryToDecimal(row["PayTotal"]),
                ApplyTotal = Converter.TryToDecimal(row["ApplyTotal"]),
                ApplyTime = Converter.TryToDateTime(row["ApplyTime"]).ToString("yyyy-MM-dd HH:MM:ss"),
                DeductionTotal = Converter.TryToInt64(row["DeductionTotal"]),
                DeductionReason = Converter.TryToString(row["DeductionReason"]),
                RefundTotal = Converter.TryToInt64(row["RefundTotal"]),
                RefundTime = Converter.TryToDateTime(row["RefundTime"]).ToString("yyyy-MM-dd HH:MM:ss"),
                PenaltyTotal = Converter.TryToInt64(row["PenaltyTotal"]),
                Status = Converter.TryToInt32(row["Status"]),
            };

            return item;
        }

    }
}
