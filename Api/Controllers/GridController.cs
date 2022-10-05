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
    public class GridController : ControllerBase
    {
        [HttpGet]
        public MyResult GetList([FromQuery] GridParam param)
        {
            int total;
            List<Grid> roleList = GridBLL.GetGridList(param, out total);

            var result = new
            {
                total,
                list = roleList
            };
            return MyResult.OK(result);
        }


        [HttpPost("deleteById")]
        public MyResult DeleteById([FromBody] Grid data)
        {
            bool re = GridBLL.DeleteById(data);
            return re ? MyResult.OK() : MyResult.Error();
        }

        // POST: api/User
        [HttpPost]
        public MyResult Post([FromBody] Grid data)
        {
            if (ModelState.IsValid)
            {
                bool re = data.IsAdd ? GridBLL.AddNewGrid(data) : GridBLL.UpdateGrid(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }

        [HttpPost("unbind")]
        public MyResult Unbind([FromBody] Grid data)
        {
            bool re = GridBLL.Unbind(data);
            return re ? MyResult.OK() : MyResult.Error();
        }
        [HttpPost("bind")]
        public MyResult Bind([FromBody] Grid data)
        {
            bool re = GridBLL.Bind(data);
            return re ? MyResult.OK() : MyResult.Error();
        }

    }
}
