using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.BLL;
using Api.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/f")]
    [ApiController]
    public class FileController : ControllerBase
    {
        /// <summary>
        /// 根据id获取瓶子二维码
        /// </summary>
        [HttpGet("qrcode/b/{id}")]
        public ActionResult GetBottleQRcode(string id)
        {
            Bottle bottle = BottleBLL.GetById(id);
            if (bottle == null)
            {
                return PhysicalFile(@"c:\404.jpg", "image/jpeg");
            }
            else
            {
                return PhysicalFile(bottle.QRCode, "image/jpeg");
            }
        }
        [HttpGet("p")]
        public ActionResult GetAttach([FromQuery] string path)
        {
            // 判断路径是否存在
            if (path == null)
            {
                return PhysicalFile(@"c:\404.jpg", "image/jpeg");
            }
            else
            {
                return PhysicalFile(path, "image/jpeg");
            }
        }

        /// <summary>
        /// 根据id获取柜子二维码
        /// </summary>
        [HttpGet("qrcode/c/{id}")]
        public ActionResult GetCabinetQRcode(string id)
        {
            Cabinet cabinet = CabinetBLL.GetById(id);
            if (cabinet == null)
            {
                return PhysicalFile(@"c:\404.jpg", "image/jpeg");
            }
            else
            {
                return PhysicalFile(cabinet.QRCode, "image/jpeg");
            }
        }

    }
}
