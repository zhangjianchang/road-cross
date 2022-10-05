using Api.BLL;
using Api.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        [HttpGet]
        public MyResult GetList([FromQuery] PartnerParam param)
        {
            int total;
            List<Partner> list = PartnerBLL.GetList(param, out total);

            var result = new
            {
                total,
                list = list
            };
            return MyResult.OK(result);
        }
        [HttpGet("boxList")]
        public MyResult GetBoxList([FromQuery] string id)
        {
            List<Cabinet> result = PartnerBLL.GetBindBoxList(id);
            return MyResult.OK(result);
        }

        [HttpPost("updateStatus")]
        public MyResult UpdateStatus([FromBody] Partner data)
        {
            if (ModelState.IsValid)
            {
                bool re = PartnerBLL.UpdateStatus(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }
        [HttpPost("reset")]
        public MyResult ResetPassword([FromBody] Partner data)
        {
            if (ModelState.IsValid)
            {
                bool re = PartnerBLL.ResetPassword(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }

        #region 提现

        [HttpGet("withdrawList")]
        public MyResult WithdrawList([FromQuery] PartnerParam param)
        {
            int total;
            List<Partner> list = PartnerBLL.WithdrawList(param, out total);

            var result = new
            {
                total,
                list = list
            };
            return MyResult.OK(result);
        }
        [HttpPost("updateWithdrawStatus")]
        public MyResult UpdateWithdrawStatus([FromBody] Partner data)
        {
            if (ModelState.IsValid)
            {
                bool re = PartnerBLL.UpdateWithdrawStatus(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }


        #endregion


    }
}
