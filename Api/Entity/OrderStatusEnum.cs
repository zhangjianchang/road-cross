using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class OrderStatus
    {
        public static string OrderStatus_Zh(int status)
        {
            return status switch
            {
                (int)OrderStatusEnum.ToBePaid => "未支付",
                (int)OrderStatusEnum.ToBeTaken => "未取瓶",
                (int)OrderStatusEnum.Processing => "租赁中",
                (int)OrderStatusEnum.Finished => "已归还",
                //(int)OrderStatusEnum.Overdue => "已逾期",
                (int)OrderStatusEnum.Delivering => "配送中",
                (int)OrderStatusEnum.Canceled => "已取消",
                (int)OrderStatusEnum.Unusual => "异常处理中",
                (int)OrderStatusEnum.Closed => "已关闭",
                _ => "异常处理中",
            };
        }
        public static string PayStatus_Zh(int status)
        {
            return status switch
            {
                (int)PayStatusEnum.ToBePaid => "未支付",
                (int)PayStatusEnum.Paid => "已支付",
                (int)PayStatusEnum.Cancel => "已取消",
                (int)PayStatusEnum.Refunded => "已退款",
                (int)PayStatusEnum.Refunding => "退款中",
                (int)PayStatusEnum.RefundFail => "退款失败",
                _ => "订单异常",
            };
        }
        public static string OrderBottleStatus_Zh(int status)
        {
            return status switch
            {
                (int)OrderBottleEnum.ToBeTaken => "未取瓶",
                (int)OrderBottleEnum.Processing => "租赁中",
                (int)OrderBottleEnum.Finished => "已归还",
                (int)OrderBottleEnum.Overdue => "已逾期",
                (int)OrderBottleEnum.Delivering => "配送中",
                _ => "订单异常",
            };
        }
        public static string DepositStatus_Zh(int status)
        {
            return status switch
            {
                (int)DepositStatusEnum.Paid => "已支付",
                (int)DepositStatusEnum.Refunding => "退还中",
                (int)DepositStatusEnum.RefundFail => "退还失败",
                (int)DepositStatusEnum.Refunded => "已退还",
                _ => "订单异常",
            };
        }

        public static List<FilterOptions> GetOrderStatusOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            options.Add(new FilterOptions { Label = "未支付", Value = "100" });
            options.Add(new FilterOptions { Label = "未取瓶", Value = "200" });
            options.Add(new FilterOptions { Label = "租赁中", Value = "201" });
            options.Add(new FilterOptions { Label = "已归还", Value = "202" });
            //options.Add(new FilterOptions { Label = "已逾期", Value = "203" });
            options.Add(new FilterOptions { Label = "配送中", Value = "204" });
            options.Add(new FilterOptions { Label = "已取消", Value = "205" });
            options.Add(new FilterOptions { Label = "异常处理中", Value = "301" });
            options.Add(new FilterOptions { Label = "已关闭", Value = "400" });
            return options;
        }

        public static List<FilterOptions> GetDepositStatusOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            options.Add(new FilterOptions { Label = "已支付", Value = "100" });
            options.Add(new FilterOptions { Label = "退还中", Value = "200" });
            options.Add(new FilterOptions { Label = "退还失败", Value = "201" });
            options.Add(new FilterOptions { Label = "已退还", Value = "300" });
            return options;
        }
    }

    /// <summary>
    /// 订单状态: 100-未支付；200-未取瓶；201-租赁中；202-已归还；203-已逾期；204-配送中；205-取消订单；301-异常处理中；400-已关闭
    /// </summary>
    public enum OrderStatusEnum
    {
        ToBePaid = 100,
        ToBeTaken = 200,
        Processing = 201,
        Finished = 202,
        //Overdue = 203,
        Delivering = 204,
        Canceled = 205,
        Unusual = 301,
        Closed = 400,
    }

    /// <summary>
    /// 支付状态：100-未支付；200-已支付；300-已取消；400-已退款；401-退款中；402-退款失败；
    /// </summary>
    public enum PayStatusEnum
    {
        ToBePaid = 100,
        Paid = 200,
        Cancel = 300,
        Refunded = 400,
        Refunding = 401,
        RefundFail = 402
    }

    /// <summary>
    /// 商品信息状态 200-未取瓶；201-租赁中；202-已归还；203-已逾期；204-配送中
    /// </summary>
    public enum OrderBottleEnum
    {
        ToBeTaken = 200,
        Processing = 201,
        Finished = 202,
        Overdue = 203,
        Delivering = 204
    }

    /// <summary>
    /// 押金状态 100-已支付  200-退还中  201-退还失败 300-已退还
    /// </summary>
    public enum DepositStatusEnum
    {
        /// <summary>
        /// 100-已支付
        /// </summary>
        Paid = 100,
        /// <summary>
        /// 200-退还中
        /// </summary>
        Refunding = 200,
        /// <summary>
        /// 201-退还失败
        /// </summary>
        RefundFail = 201,
        /// <summary>
        /// 300-已退还
        /// </summary>
        Refunded = 300
    }

}
