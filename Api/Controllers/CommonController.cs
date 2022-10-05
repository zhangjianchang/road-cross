using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Api.BLL;
using Api.Entity;


namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CommonController : Controller
    {
        // GET: api/<controller>
        [HttpGet("FilterOptions/ContactPerson")]
        public MyResult GetContactNameOptions()
        {
            List<FilterOptions> options = CommonBLL.GetContactPersonOptions();
            return MyResult.OK(options);
        }
        [HttpGet("FilterOptions/CabinetNum")]
        public MyResult GetCabinetNumOptions(string deviceType)
        {
            List<FilterOptions> options = CommonBLL.GetCabinetNumOptions(deviceType);
            return MyResult.OK(options);
        }
        [HttpGet("FilterOptions/CabinetNumWithID")]
        public MyResult GetCabinetNumWithIDOptions()
        {
            List<FilterOptions> options = CommonBLL.GetCabinetNumWithIDOptions();
            return MyResult.OK(options);
        }
        [HttpGet("FilterOptions/CabinetStatus")]
        public MyResult GetCabinetStatusOptions()
        {
            return MyResult.OK(CabinetStatus.GetStatusOptions());
        }
        [HttpGet("FilterOptions/GridStatus")]
        public MyResult GetGridStatusOptions()
        {
            return MyResult.OK(GridStatus.GetStatusOptions());
        }
        [HttpGet("FilterOptions/BottleStatus")]
        public MyResult GetBottleStatusOptions()
        {
            return MyResult.OK(BottleStatus.GetStatusOptions());
        }
        [HttpGet("FilterOptions/BottleSpec")]
        public MyResult GetBottleSpecOptions()
        {
            List<FilterOptions> options = CommonBLL.GetBottleSpecOptions();
            return MyResult.OK(options);
        }

        [HttpGet("FilterOptions/PartnerOrganization")]
        public MyResult GetPartnerOrganizationOptions(string type)
        {
            List<FilterOptions> options = CommonBLL.GetPartnerOrganizationOptions(type);
            return MyResult.OK(options);
        }

        [HttpGet("FilterOptions/OrderStatus")]
        public MyResult GetOrderStatusOptions()
        {
            List<FilterOptions> options = OrderStatus.GetOrderStatusOptions();
            return MyResult.OK(options);
        }

        [HttpGet("FilterOptions/DepositStatus")]
        public MyResult GetDepositStatusOptions()
        {
            List<FilterOptions> options = OrderStatus.GetDepositStatusOptions();
            return MyResult.OK(options);
        }

        [HttpGet("FilterOptions/Coupons")]
        public MyResult GetCouponsOptions()
        {
            List<FilterOptions> options = CommonBLL.GetCouponsOptions();
            return MyResult.OK(options);
        }
    }
}

