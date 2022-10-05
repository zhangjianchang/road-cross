using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Api.BLL;
using Api.Entity;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        [HttpGet("agreements")]
        public MyResult GetAgreements(string code)
        {
            object re = BusinessBLL.GetAgreements(code);
            return MyResult.OK(re);
        }
        // POST: api/Business
        [HttpPost("agreements")]
        public MyResult SaveAgreements([FromBody] object data)
        {
            JObject obj = JObject.FromObject(data);
            string code = Convert.ToString(obj["code"]);
            string content = Convert.ToString(obj["content"]);
            string updateBy = Convert.ToString(obj["updateBy"]);

            bool re = BusinessBLL.SaveAgreements(code, content, updateBy);
            return re ? MyResult.OK() : MyResult.Error();
        }

        #region 常见问题分类

        [HttpGet("articleCategory")]
        public MyResult GetArticleCategory()
        {
            List<Article> list = BusinessBLL.GetArticleCategoryList();
            return MyResult.OK(list);
        }

        [HttpPost("articleCategory")]
        public MyResult PostArticleCategory([FromBody] Article data)
        {
            bool re = data.ID == null ? BusinessBLL.AddArticleCategory(data) : BusinessBLL.SaveArticleCategory(data);
            return re ? MyResult.OK() : MyResult.Error();
        }

        [HttpDelete("articleCategory/{id}")]
        public MyResult DeleteArticleCategory(int id)
        {
            bool re = BusinessBLL.DeleteArticleCategory(id);
            return re ? MyResult.OK() : MyResult.Error();
        }
        #endregion

        #region 常见问题


        [HttpGet("article")]
        public MyResult GetArticle()
        {
            List<Article> list = BusinessBLL.GetArticleList();
            return MyResult.OK(list);
        }

        [HttpPost("article")]
        public MyResult PostArticle([FromBody] Article data)
        {
            bool re = data.ID == null ? BusinessBLL.AddArticle(data) : BusinessBLL.SaveArticle(data);
            return re ? MyResult.OK() : MyResult.Error();
        }

        [HttpDelete("article/{id}")]
        public MyResult DeleteArticle(int id)
        {
            bool re = BusinessBLL.DeleteArticle(id);
            return re ? MyResult.OK() : MyResult.Error();
        }

        #endregion
    }
}
