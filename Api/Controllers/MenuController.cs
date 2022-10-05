using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Api.BLL;
using Api.Entity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class MenuController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public MyResult Get()
        {
            List<MenuEntity> menuList = MenuBLL.GetMenuList();
            return MyResult.OK(menuList);
        }

        // GET: api/<controller>
        [HttpGet("all")]
        public MyResult GetAll()
        {
            List<MenuEntity> menuList = MenuBLL.GetAllMenuList();
            return MyResult.OK(menuList);
        }

        [HttpPost]
        public MyResult Post([FromBody] MenuEntity data)
        {
            bool re = data.MenuID == null ? MenuBLL.AddMenu(data) : MenuBLL.SaveMenu(data);
            return re ? MyResult.OK() : MyResult.Error();
        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{menuID}")]
        public MyResult Delete(int menuID)
        {
            bool re = MenuBLL.DeleteMenu(menuID);
            return re ? MyResult.OK() : MyResult.Error();
        }

    }
}
