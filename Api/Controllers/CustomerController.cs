using Api.BLL;
using Api.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpGet]
        public MyResult GetList([FromQuery] CustomerParam param)
        {
            int total;
            List<Customer> list = CustomerBLL.GetList(param, out total);

            var result = new
            {
                total,
                list = list
            };
            return MyResult.OK(result);
        }
        [HttpGet("LevelupRecord")]
        public MyResult GetLevelupRecord([FromQuery] string id)
        {
            List<LevelupRecord> result = CustomerBLL.GetLevelupRecord(id);
            return MyResult.OK(result);
        }

        [HttpGet("pointsList")]
        public MyResult GetPointsList([FromQuery] CustomerParam param)
        {
            int total;
            List<PointsRecord> list = CustomerBLL.GetPointsList(param, out total);

            var result = new
            {
                total,
                list = list
            };
            return MyResult.OK(result);
        }


        [HttpPost("updateStatus")]
        public MyResult UpdateStatus([FromBody] Customer data)
        {
            if (ModelState.IsValid)
            {
                bool re = CustomerBLL.UpdateStatus(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }
    }
}
