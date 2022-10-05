using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Api.BLL
{
    public class OrderBLL
    {
        internal static List<OrderInfo> GetOrderList(OrderInfoParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<OrderInfo> recordList = new List<OrderInfo>();
            string sql = @" SELECT * FROM (
								SELECT o.`OrderNo`,
									o.`CabinetName`,
									o.`OriginalPrice`,
									o.`Count`,
									o.`TotalAmount`,
									o.`PayStatus`,
									o.`Status`,
									o.`PayWay`,
									o.`Deposit`,
									o.`OverdueTime`,
									o.`CreateTime`,
									o.`UpdateTime`,
									c.`Telphone`,
									d.`Status` as DepositStatus,
									c.`MemberType`,
									(SELECT COUNT(*) FROM `mt_order_bottle` WHERE OrderNo = o.OrderNo AND `Status` = 201 AND now()>OverdueTime) as OverdueCount
								FROM `mt_order` o
								LEFT JOIN `mt_customer` c ON o.`OpenID`=c.`OpenID`
								LEFT JOIN `mt_order_deposit` d ON d.`OrderNo`=o.`OrderNo`
								{0}
                            ) as t
							{1}
                            ORDER by t.CreateTime DESC
                            LIMIT {2},{3} ";

            string where = " WHERE 1 = 1 ";
            string where2 = "";
            List<MySqlParameter> param = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchParam.OrderNo))
            {
                where += " AND o.`OrderNo` = @OrderNo ";
                param.Add(new MySqlParameter("@OrderNo", searchParam.OrderNo));
            }
            if (!string.IsNullOrEmpty(searchParam.Status))
            {
                where += " AND o.`Status` = @Status ";
                param.Add(new MySqlParameter("@Status", searchParam.Status));
            }
            if (!string.IsNullOrEmpty(searchParam.Telphone))
            {
                where += " AND c.`Telphone` = @Telphone ";
                param.Add(new MySqlParameter("@Telphone", searchParam.Telphone));
            }
            if (!string.IsNullOrEmpty(searchParam.CreateTimeStartStr))
            {
                where += " AND o.CreateTime > @CreateTimeStart";
                param.Add(new MySqlParameter("@CreateTimeStart", Convert.ToDateTime(searchParam.CreateTimeStartStr)));
            }
            if (!string.IsNullOrEmpty(searchParam.CreateTimeEndStr))
            {
                where += " AND o.CreateTime < @CreateTimeEnd";
                param.Add(new MySqlParameter("@CreateTimeEnd", Convert.ToDateTime(searchParam.CreateTimeEndStr)));
            }
            if (!string.IsNullOrEmpty(searchParam.MemberType))
            {
                where += " AND c.`MemberType` = @MemberType ";
                param.Add(new MySqlParameter("@MemberType", searchParam.MemberType));
            }
            if (searchParam.IsOverdue != null)
            {
                where2 = searchParam.IsOverdue == 1 ? " WHERE t.OverdueCount > 0" : " WHERE t.OverdueCount = 0";
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, where2, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    OrderInfo item = GetOrderByRow(row);
                    item.CanRefund = CanRefund(item) && item.DepositStatus == (int)DepositStatusEnum.Paid ? 1 : 0;
                    recordList.Add(item);
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) FROM (
								    SELECT o.`OrderNo`,
									    (SELECT COUNT(*) FROM `mt_order_bottle` WHERE OrderNo = o.OrderNo AND `Status` = 201 AND now()>OverdueTime) as OverdueCount
								    FROM `mt_order` o
								    LEFT JOIN `mt_customer` c ON o.`OpenID`=c.`OpenID`
								    LEFT JOIN `mt_order_deposit` d ON d.`OrderNo`=o.`OrderNo`
								    {0}
                                ) as t
							    {1}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where, where2), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static OrderDeposit GetDepositInfo(string orderNo)
        {
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                   Config.DBConnection,
                   @"SELECT *
                    FROM
                        mt_order_deposit
                    WHERE OrderNo = @OrderNo",
                   new MySqlParameter("@OrderNo", orderNo));

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new OrderDeposit
                {
                    //ID = Converter.TryToInt64(row["ID"]),
                    //OrderNo = Converter.TryToString(row["OrderNo"]),
                    //OpenID = Converter.TryToString(row["OpenID"]),
                    PayTotal = Converter.TryToDecimal(row["PayTotal"]),
                    DeductionTotal = Converter.TryToDecimal(row["DeductionTotal"]),
                    DeductionReason = Converter.TryToString(row["DeductionReason"]),
                    Status = Converter.TryToInt32(row["Status"]),
                    RefundTime = Converter.TryToDateTime(row["RefundTime"]).ToString("yyyy-MM-dd HH:mm")
                };
            }
            return null;
        }

        internal static List<OrderBottle> GetOrderBottleList(string orderNo)
        {
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT
	                     * 
                     FROM
	                     mt_order_bottle
                     WHERE
	                      orderNo = @OrderNo AND BottleId is NOT NULL ",
                new MySqlParameter("@OrderNo", orderNo));
            List<OrderBottle> orderBottles = new List<OrderBottle>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var orderBottle = new OrderBottle()
                    {
                        OrderNo = Converter.TryToString(row["OrderNo"]),
                        BottleId = Converter.TryToString(row["BottleId"]),
                        BottleCode = Converter.TryToString(row["BottleCode"]),
                        GridId = Converter.TryToString(row["GridId"]),
                        GridNumber = Converter.TryToString(row["GridNumber"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm"),
                        ReturnAddress = Converter.TryToString(row["ReturnAddress"]),
                        ReturnTime = Converter.TryToDateTime(row["ReturnTime"]).ToString("yyyy-MM-dd HH:mm"),
                        GetBottleTime = Converter.TryToDateTime(row["GetBottleTime"]).ToString("yyyy-MM-dd HH:mm"),
                        OverdueTime = Converter.TryToDateTime(row["OverdueTime"]).ToString("yyyy-MM-dd HH:mm"),
                        Status = Converter.TryToInt32(row["Status"]),
                    };
                    orderBottles.Add(orderBottle);
                }
            }
            return orderBottles;
        }

        private static OrderInfo GetOrderByRow(DataRow row)
        {
            OrderInfo item = new OrderInfo
            {
                OrderNo = Converter.TryToInt64(row["OrderNo"]).ToString(),
                CabinetName = Converter.TryToString(row["CabinetName"]),
                Count = Converter.TryToInt32(row["Count"]),
                TotalAmount = Converter.TryToDecimal(row["TotalAmount"]),
                PayStatus = Converter.TryToInt32(row["PayStatus"]),
                Status = Converter.TryToInt32(row["Status"]),
                PayWay = Converter.TryToInt32(row["PayWay"]),
                Deposit = Converter.TryToDecimal(row["Deposit"]),
                OverdueTime = Converter.TryToDateTime(row["OverdueTime"]).ToString("yyyy-MM-dd HH:MM:ss"),
                CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:MM:ss"),
                Telphone = Converter.TryToString(row["Telphone"]),
                DepositStatus = Converter.TryToInt32(row["DepositStatus"]),
                OverdueCount = Converter.TryToInt32(row["OverdueCount"]),
                MemberType = Converter.TryToString(row["MemberType"]),
            };
            item.StatusDesc = OrderStatus.OrderStatus_Zh(item.Status);
            item.PayStatusDesc = OrderStatus.PayStatus_Zh(item.PayStatus);
            item.MemberType = item.MemberType == "gold" ? "金卡会员" : item.MemberType == "silver" ? "银卡会员" : "普通会员";
            return item;
        }

        internal static List<OrderSmallGoods> GetSmallGooodsList(string orderNo)
        {
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                   Config.DBConnection,
                   @"SELECT
	                     * 
                     FROM
	                     mt_order_smallgoods
                     WHERE
	                      orderNo = @OrderNo",
                   new MySqlParameter("@OrderNo", orderNo));
            List<OrderSmallGoods> orderGoods = new List<OrderSmallGoods>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var record = new OrderSmallGoods()
                    {
                        GoodsName = Converter.TryToString(row["GoodsName"]),
                        PicPath = Converter.TryToString(row["PicPath"]),
                        Price = Converter.TryToDecimal(row["Price"]),
                        PayCount = Converter.TryToInt32(row["PayCount"]),
                    };
                    orderGoods.Add(record);
                }
            }
            return orderGoods;
        }

        internal static async Task<bool> Refund(OrderInfoParam param)
        {
            // 查询订单状态
            OrderInfo order = GetOrderDetail(param);
            if (!CanRefund(order))
            {
                throw new MsgException($"当前订单[{order.StatusDesc}]，不支持结单退款！");
            }
            // 查询押金信息
            OrderDeposit deposit = OrderDepositBLL.GetOrderDeposit(param);
            if (deposit.Status != (int)DepositStatusEnum.Paid)
            {
                throw new MsgException($"当前订单押金[{deposit.StatusDesc}]，不支持结单退款！");
            }
            if (param.RefundTotal <= 0)
            {
                throw new MsgException($"退款金额必须大于0");
            }
            param.TotalAmount = order.TotalAmount;

            // 调取还款接口
            bool re = await VxinUtils.GetWxRefundRequest(param);
            if (re)
            {
                if (order.Status == (int)OrderStatusEnum.Canceled)
                {
                    // 已取消订单，订单状态不变，修改支付状态为已退款
                    // TODO:应该改为退款中
                    order.PayStatus = (int)PayStatusEnum.Refunded;
                    UpdatePayStatus(order);
                }
                else
                {
                    // 其它订单，订单状态改为已关闭
                    order.Status = (int)OrderStatusEnum.Closed;
                    UpdateStatus(order);
                }

                // 更新押金状态
                // TODO: 应该改为退款中
                deposit.Status = (int)DepositStatusEnum.Refunded;
                deposit.ApplyTotal = param.RefundTotal;
                deposit.DeductionTotal = param.DeductionTotal;
                deposit.PenaltyTotal = param.PenaltyTotal;
                deposit.DeductionReason = param.Remark;
                deposit.RefundTotal = param.RefundTotal;
                OrderDepositBLL.Refund(deposit);
            }

            return true;
        }

        internal static OrderInfo GetOrderDetail(OrderInfoParam searchParam)
        {
            string sql = @" SELECT o.`OrderNo`,
                                o.`CabinetName`,
                                o.`OriginalPrice`,
                                o.`Count`,
                                o.`TotalAmount`,
                                o.`PayStatus`,
                                o.`Status`,
                                o.`PayWay`,
                                o.`Deposit`,
                                o.`OverdueTime`,
                                o.`CreateTime`,
	                            '' as `Telphone`,
                                0 as DepositStatus,
								'' as`MemberType`,
                                (SELECT COUNT(*) FROM `mt_order_bottle` WHERE OrderNo = o.OrderNo AND `Status` = 201 AND now()>OverdueTime) as OverdueCount
                            FROM `mt_order` o
                            WHERE o.`OrderNo` = @OrderNo; ";

            List<MySqlParameter> param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@OrderNo", searchParam.OrderNo));

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                return GetOrderByRow(dt.Rows[0]);
            }
            else
            {
                throw new MsgException("订单不存在！");
            }
        }

        public static bool UpdateStatus(OrderInfo param)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @$"UPDATE `mt_order` set `STATUS`= @Status WHERE OrderNo=@OrderNo",
                new MySqlParameter("@OrderNo", param.OrderNo),
                new MySqlParameter("@Status", param.Status));
            return true;
        }
        public static bool UpdatePayStatus(OrderInfo param)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @$"UPDATE `mt_order` set `PayStatus`= @PayStatus WHERE OrderNo=@OrderNo",
                new MySqlParameter("@OrderNo", param.OrderNo),
                new MySqlParameter("@PayStatus", param.PayStatus));
            return true;
        }

        private static bool CanRefund(OrderInfo item)
        {
            return item.Status == (int)OrderStatusEnum.Finished
                || item.Status == (int)OrderStatusEnum.Canceled
                || item.Status == (int)OrderStatusEnum.Unusual
                || (item.Status == (int)OrderStatusEnum.Processing && item.OverdueCount > 0);
        }
    }
}
