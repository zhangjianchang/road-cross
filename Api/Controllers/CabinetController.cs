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
    public class CabinetController : ControllerBase
    {
        [HttpGet]
        public MyResult GetList([FromQuery] CabinetParam param)
        {
            int total;
            List<Cabinet> roleList = CabinetBLL.GetCabinetList(param, out total);

            var result = new
            {
                total,
                list = roleList
            };
            return MyResult.OK(result);
        }


        [HttpPost("deleteById")]
        public MyResult DeleteById([FromBody] Cabinet data)
        {
            bool re = CabinetBLL.DeleteById(data);
            return re ? MyResult.OK() : MyResult.Error();
        }

        // POST: api/User
        [HttpPost]
        public MyResult Post([FromBody] Cabinet data)
        {
            if (ModelState.IsValid)
            {
                bool re = data.IsAdd ? CabinetBLL.AddNewCabinet(data) : CabinetBLL.UpdateCabinet(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }

        [HttpPost("reGenerateQRCode")]
        public MyResult ReGenerateQRCode([FromBody] Cabinet data)
        {
            if (ModelState.IsValid)
            {
                bool re = CabinetBLL.ReGenerateQRCode(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }

        //[HttpGet("detail")]
        //public MyResult GetDetail([FromQuery] OrderInfoParam param)
        //{
        //    OrderInfo orderInfo = OrderBLL.GetOrderDetail(param);
        //    return MyResult.OK(orderInfo);
        //}

        //[HttpGet("batchNos")]
        //public MyResult GetBatchNos(string action)
        //{
        //    List<Object> result = OrderBLL.GetBatchNos(action);
        //    return MyResult.OK(result);
        //}

        //// POST: api/User
        //[HttpPost]
        //public MyResult Post([FromBody] OrderInfo data)
        //{

        //    if (string.IsNullOrWhiteSpace(data.OrderNumber))
        //    {
        //        return MyResult.Error("快递单号不能为空！");
        //    }
        //    if (string.IsNullOrWhiteSpace(data.JBBWAddress))
        //    {
        //        return MyResult.Error("津巴布韦地址不能为空！");
        //    }

        //    bool re = OrderBLL.AddNewOrder(data);
        //    return re ? MyResult.OK() : MyResult.Error();
        //}

        //[HttpPost("update")]
        //public MyResult Update([FromBody] OrderInfo data)
        //{
        //    bool re = OrderBLL.UpdateOrder(data);
        //    return re ? MyResult.OK() : MyResult.Error();
        //}

        //[HttpPost("updatestatus")]
        //public MyResult BatchUpdateStatus([FromBody] OrderStatusParam param)
        //{
        //    bool re = OrderBLL.BatchUpdateStatus(param);
        //    return re ? MyResult.OK() : MyResult.Error();
        //}

        //[HttpPost("updateStatusByBatchNumber")]
        //public MyResult UpdateStatusByBatchNumber([FromBody] BatchUpdateParam param)
        //{
        //    bool re = OrderBLL.UpdateStatusByBatchNumber(param);
        //    return re ? MyResult.OK() : MyResult.Error();
        //}

        //[HttpPost("addExpressInfo")]
        //public MyResult AddExpressInfo([FromBody] ExpressInfoParam param)
        //{
        //    bool re = OrderBLL.AddExpressInfo(param);
        //    return re ? MyResult.OK() : MyResult.Error();
        //}


        //[Produces("application/json")]
        //[Consumes("application/json", "multipart/form-data")]
        //[HttpPost("importOrder")]
        //public IActionResult importOrder(IFormFile file, [FromQuery] string userName)
        //{
        //    try
        //    {
        //        DataTable dt = ExcelHelper.ReadExcelToDataTable(file.OpenReadStream(), "Sheet1");

        //        string sql = @"INSERT INTO tmp_importorder
        //                        (`BatchID`,
        //                        `ORDER_NUM`,
        //                        `SENDER_PHONE`,
        //                        `SENDER_NAME`,
        //                        `SENDER_ADDRESS`,
        //                        `JBBW_PHONE`,
        //                        `JBBW_NAME`,
        //                        `JBBW_ADDRESS`,
        //                        `BATCH_NUMBER`,
        //                        `WEIGHT`,
        //                        `WARE_HOUSE`,
        //                        `CreatedBy`) VALUES ";
        //        List<string> values = new List<string>();
        //        string batchId = Guid.NewGuid().ToString("N");
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            string orderNum = Convert.ToString(row["快递单号"]);
        //            string jbbwPhone = Convert.ToString(row["津巴布韦电话"]);
        //            string jbbwAddress = Convert.ToString(row["津巴布韦地址"]);
        //            string jbbwName = Convert.ToString(row["津巴布韦姓名"]);
        //            string senderName = Convert.ToString(row["寄件人姓名"]);
        //            string senderPhone = Convert.ToString(row["寄件人电话"]);
        //            string senderAddress = Convert.ToString(row["寄件人地址"]);
        //            string batchNumber = Convert.ToString(row["物流批次"]);
        //            string weight = Convert.ToString(row["重量"]);
        //            string wareHouse = Convert.ToString(row["发货仓库"]);

        //            values.Add($"('{batchId}','{orderNum}','{senderPhone}','{senderName}','{senderAddress}','{jbbwPhone}','{jbbwName}','{jbbwAddress}','{batchNumber}','{weight}','{wareHouse}','{userName}')");
        //        }
        //        if (values.Count == 0)
        //        {
        //            throw new MsgException("导入人员信息为空！");
        //        }
        //        JabMySqlHelper.ExecuteNonQuery(Config.DBConnection, sql + string.Join(",", values));

        //        DataTable resultdt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, $"call Sp_confirmImportOrder('{batchId}')");
        //        List<OrderInfo> recordList = new List<OrderInfo>();
        //        if (resultdt != null && resultdt.Rows.Count > 0)
        //        {
        //            foreach (DataRow row in resultdt.Rows)
        //            {
        //                recordList.Add(new OrderInfo()
        //                {
        //                    OrderNumber = Converter.TryToString(row["ORDER_NUM"]),
        //                    JBBWPhone = Converter.TryToString(row["JBBW_PHONE"]),
        //                    JBBWName = Converter.TryToString(row["JBBW_NAME"]),
        //                    JBBWAddress = Converter.TryToString(row["JBBW_ADDRESS"]),
        //                    SenderName = Converter.TryToString(row["SENDER_NAME"]),
        //                    SenderPhone = Converter.TryToString(row["SENDER_PHONE"]),
        //                    SenderAddress = Converter.TryToString(row["SENDER_ADDRESS"]),
        //                    Status = Converter.TryToString(row["Status"]),
        //                    BatchNo = Converter.TryToString(row["BATCH_NUMBER"]),
        //                    Weight = Converter.TryToString(row["WEIGHT"]),
        //                    WareHouse = Converter.TryToString(row["WARE_HOUSE"]),
        //                });
        //            }
        //        }
        //        JabMySqlHelper.ExecuteNonQuery(Config.DBConnection, $"delete from tmp_importorder where BatchID = '{batchId}'");
        //        return Ok(new
        //        {
        //            status = 200,
        //            msg = $"导入完成",
        //            data = recordList
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        return Ok(new
        //        {
        //            status = 500,
        //            msg = "导入失败:" + e.Message,
        //            error = "导入失败:" + e.Message + Environment.NewLine + e.StackTrace
        //        });
        //    }
        //}

        //[HttpPost("deleteById")]
        //public MyResult DeleteById([FromBody] OrderInfo data)
        //{
        //    bool re = OrderBLL.DeleteById(data);
        //    return re ? MyResult.OK() : MyResult.Error();
        //}
    }
}
