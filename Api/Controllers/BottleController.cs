using Api.BLL;
using Api.Entity;
using Api.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BottleController : ControllerBase
    {
        [HttpGet]
        public MyResult GetList([FromQuery] BottleParam param)
        {
            int total;
            List<Bottle> roleList = BottleBLL.GetBottleList(param, out total);

            var result = new
            {
                total,
                list = roleList
            };
            return MyResult.OK(result);
        }


        [HttpPost("deleteById")]
        public MyResult DeleteById([FromBody] Bottle data)
        {
            bool re = BottleBLL.DeleteById(data);
            return re ? MyResult.OK() : MyResult.Error();
        }

        // POST: api/User
        [HttpPost]
        public MyResult Post([FromBody] Bottle data)
        {
            if (ModelState.IsValid)
            {
                bool re = data.IsAdd ? BottleBLL.AddNewBottle(data) : BottleBLL.UpdateBottle(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }

        [HttpPost("reGenerateQRCode")]
        public MyResult ReGenerateQRCode([FromBody] Bottle data)
        {
            if (ModelState.IsValid)
            {
                bool re = BottleBLL.ReGenerateQRCode(data);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }


        [Produces("application/json")]
        [Consumes("application/json", "multipart/form-data")]
        [HttpPost("import")]
        public IActionResult import(IFormFile file, [FromForm] Bottle param)
        {
            List<Bottle> errorList = new List<Bottle>();
            try
            {
                DataTable dt = ExcelHelper.ReadExcelToDataTable(file.OpenReadStream(), "Sheet1");

                string sql = @"INSERT INTO tmp_bottle
                                (`Code`,
                                `Spec`,
                                `PurchaseTime`,`CreateBy`) VALUES ";
                List<string> values = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string code = Convert.ToString(row["瓶子编号"]);
                    string spec = Convert.ToString(row["规格"]);
                    DateTime purchaseTime;
                    try
                    {
                        purchaseTime = Convert.ToDateTime(row["采购日期"]);
                    }
                    catch (Exception)
                    {
                        errorList.Add(new Bottle
                        {
                            Code = code,
                            Spec = spec,
                            PurchaseTime = Converter.TryToString(row["采购日期"]),
                            Error = "采购日期格式错误；"
                        });
                        continue;
                    }
                    values.Add($"('{code}','{spec}','{purchaseTime:yyyy-MM-dd}','{param.CreateBy}')");
                }
                if (errorList.Count > 0)
                {
                    return Ok(new
                    {
                        status = 500,
                        msg = "导入失败",
                        errorData = errorList
                    });
                }
                if (values.Count == 0)
                {
                    throw new Exception("导入数据为空！");
                }

                // 删除临时表数据
                JabMySqlHelper.ExecuteNonQuery(Config.DBConnection, "DELETE FROM tmp_bottle WHERE `CreateBy`=@CreateBy",
                    new MySqlParameter("@CreateBy", param.CreateBy));

                // 插入新的临时数据
                JabMySqlHelper.ExecuteNonQuery(Config.DBConnection, sql + string.Join(",", values));

                // 验证数据是否正确
                DataTable dtResult = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, CommandType.StoredProcedure, "Sp_verifyBottle",
                    new MySqlParameter("@p_CreateBy", param.CreateBy));

                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    return Ok(new
                    {
                        status = 500,
                        msg = "导入失败",
                        errorData = dtResult
                    });
                }

                // 确认导入数据
                JabMySqlHelper.ExecuteNonQuery(Config.DBConnection, CommandType.StoredProcedure, "Sp_confirmBottle",
                    new MySqlParameter("@p_CreateBy", param.CreateBy));


                //重新生成所有没有二维码的瓶子
                BottleBLL.GenerateQRCodeForEmpty(param.CreateBy);

                return Ok(new
                {
                    status = 200,
                    msg = "导入成功"
                });
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    status = 500,
                    msg = "导入失败:" + e.Message,
                    error = "导入失败:" + e.Message + Environment.NewLine + e.StackTrace
                });
            }
        }
    }
}
