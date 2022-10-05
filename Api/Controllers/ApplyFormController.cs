using Api.BLL;
using Api.Entity;
using Api.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyFormController : ControllerBase
    {
        [HttpGet("guideList")]
        public MyResult GetGuideList([FromQuery] PartnerFormParam param)
        {
            int total;
            List<PartnerFormGuide> roleList = ApplyFormBLL.GetGuideList(param, out total);

            var result = new
            {
                total,
                list = roleList
            };
            return MyResult.OK(result);
        }
        [HttpGet("generalList")]
        public MyResult GetGeneralList([FromQuery] PartnerFormParam param)
        {
            int total;
            List<PartnerFormGeneral> roleList = ApplyFormBLL.GetGeneralList(param, out total);

            var result = new
            {
                total,
                list = roleList
            };
            return MyResult.OK(result);
        }
        [HttpGet("advancedList")]
        public MyResult GetAdvancedList([FromQuery] PartnerFormParam param)
        {
            int total;
            List<PartnerFormAdvanced> roleList = ApplyFormBLL.GetAdvancedList(param, out total);

            var result = new
            {
                total,
                list = roleList
            };
            return MyResult.OK(result);
        }

        [HttpPost("approve")]
        public MyResult Approve([FromBody] PartnerApproveData data)
        {
            if (ModelState.IsValid)
            {
                bool re = ApplyFormBLL.Approve(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }

        [HttpPost("reject")]
        public MyResult Reject([FromBody] PartnerApproveData data)
        {
            if (ModelState.IsValid)
            {
                bool re = ApplyFormBLL.Reject(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }

    }
}
