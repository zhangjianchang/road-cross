using Api.BLL;
using Api.Entity;
using Api.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // GET: api/User
        [HttpGet]
        public MyResult GetList([FromQuery] OrderInfoParam param)
        {
            int total;
            List<OrderInfo> roleList = OrderBLL.GetOrderList(param, out total);

            var result = new
            {
                total,
                list = roleList
            };
            return MyResult.OK(result);
        }

        [HttpGet("detail")]
        public MyResult GetDetail([FromQuery] OrderInfoParam param)
        {
            List<OrderBottle> bottleList = OrderBLL.GetOrderBottleList(param.OrderNo);
            List<OrderSmallGoods> smallGooodsList = OrderBLL.GetSmallGooodsList(param.OrderNo);
            OrderDeposit depositInfo = OrderDepositBLL.GetOrderDeposit(param);
            return MyResult.OK(new
            {
                bottleList,
                smallGooodsList,
                depositInfo
            });
        }

        [HttpPost("refund")]
        public async Task<MyResult> Refund([FromBody] OrderInfoParam param)
        {
            if (ModelState.IsValid)
            {
                bool re = await OrderBLL.Refund(param);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }


    }
}
