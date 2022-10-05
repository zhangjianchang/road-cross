using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Utilities;

namespace Api.Entity
{
    public class OrderBottle
    {
        public string ID { get; set; }
        public string OrderNo { get; set; }
        public string GridId { get; set; }
        public string GridNumber { get; set; }
        public string BottleId { get; set; }
        public string BottleCode { get; set; }
        public string ReturnTime { get; set; }
        public string ReturnAddress { get; set; }
        private int _status;
        public int Status
        {
            set { _status = value; }
            get
            {
                if (_status == (int)OrderBottleEnum.Processing && DateTime.Now > Converter.TryToDateTime(OverdueTime))
                {
                    // 已逾期
                    return (int)OrderBottleEnum.Overdue;
                }
                else
                {
                    return _status;
                }
            }
        }
        public string GetBottleTime { get; set; }
        public string OverdueTime { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public string StatusDesc => OrderStatus.OrderBottleStatus_Zh(Status);
        /// <summary>
        /// 已逾期小时数
        /// </summary>
        public int OverdueHour
        {
            get
            {
                if (Status == (int)OrderBottleEnum.Overdue)
                {
                    try
                    {
                        var v = (int)Math.Floor((DateTime.Now - Convert.ToDateTime(OverdueTime)).TotalHours);
                        return v > 0 ? v : 0;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }

            }
        }
        /// <summary>
        /// 已产生的逾期费用
        /// </summary>
        public Decimal OverdueFee
        {
            get
            {
                return OverdueHour * 1;
            }
        }
    }
}
