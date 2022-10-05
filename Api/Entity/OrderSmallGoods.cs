using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class OrderSmallGoods
    {
        public string OrderNo { get; set; }
        public string GoodsId { get; set; }
        public int PayCount { get; set; }
        public string GoodsName { get; set; }
        public string PicPath { get; set; }
        public decimal Price { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
    }
}
