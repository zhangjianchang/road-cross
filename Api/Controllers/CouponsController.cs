using Api.BLL;
using Api.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        [HttpGet]
        public MyResult GetList([FromQuery] CouponsParam param)
        {
            int total;
            List<Coupons> list = CouponsBLL.GetList(param, out total);

            var result = new
            {
                total,
                list = list
            };
            return MyResult.OK(result);
        }


        [HttpPost]
        public MyResult Post([FromBody] Coupons data)
        {
            //数据处理
            if (data.RuleType == 2)
            {
                data.Discount = data.Discount / 10;
            }

            bool re = data.IsNew ? CouponsBLL.AddCoupons(data) : CouponsBLL.UpdateCoupons(data);
            return re ? MyResult.OK() : MyResult.Error();
        }

        [HttpGet("getCustomerOptions")]
        public MyResult GetCustomerOptions([FromQuery] CouponsCustomerParam param)
        {
            int total;
            List<CouponsCustomer> list = CouponsBLL.GetCustomerOptions(param, out total);

            var result = new
            {
                total,
                list = list
            };
            return MyResult.OK(result);
        }

        [HttpPost("sendBatch")]
        public MyResult SendBatch([FromBody] CouponsCustomerParam data)
        {
            bool re = CouponsBLL.SendBatch(data);
            return re ? MyResult.OK() : MyResult.Error();
        }
        [HttpPost("sendSingle")]
        public MyResult SendSingle([FromBody] CouponsCustomerParam data)
        {
            bool re = CouponsBLL.SendSingle(data);
            return re ? MyResult.OK() : MyResult.Error();
        }

        [HttpGet("couponsCustomer")]
        public MyResult GetCouponsCustomer([FromQuery] CouponsCustomerParam param)
        {
            int total;
            List<CouponsCustomer> list = CouponsBLL.GetCouponsCustomer(param, out total);

            var result = new
            {
                total,
                list = list
            };
            return MyResult.OK(result);
        }


    }
}
