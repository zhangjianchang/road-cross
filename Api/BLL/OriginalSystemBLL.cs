using Newtonsoft.Json;
using Api.Entity;
using System.Net;
using System.Text;
using static Api.Entity.UserResult;
using System;
using static Api.Entity.OrderResult;
using static Api.Entity.CabinetResult;
using static Api.Entity.BottleResult;

namespace Api.BLL
{
    public class OriginalSystemBLL
    {
        static readonly string UserListUrl = "https://fbj.openvem.com/s/user/getList?page=1&limit=300&mobile=";
        static readonly string CabinetUrl = "https://fbj.openvem.com/s/machine?page=1&limit=20&machine_name=&machine_no=&imei=&online_status=";
        static readonly string BottleUrl = "https://fbj.openvem.com/s/goodsSpec/getList?page={0}&limit=500";


        static readonly string OrderListUrl = "https://fbj.openvem.com/s/user/getList?page=1&limit=527&mobile=";

        /// <summary>
        /// 同步用戶信息
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool SyncUsers(string cookie)
        {
            try
            {
                string res = WebRequestPostOrGet(UserListUrl, cookie);
                UserResult jsondata = new UserResult();
                jsondata = JsonConvert.DeserializeObject<UserResult>(res);
                foreach (var userData in jsondata.data)
                {
                    try
                    {
                        Customer customer = ConvertUserToCustomer(userData);
                        CustomerBLL.SyncCustomer(customer);
                    }
                    catch
                    {
                        continue;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 同步订单信息
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        //public static bool SyncOrders(string cookie)
        //{
        //    try
        //    {
        //        string res = WebRequestPostOrGet(OrderListUrl, cookie);
        //        OrderResult jsondata = new OrderResult();
        //        jsondata = JsonConvert.DeserializeObject<OrderResult>(res);
        //        foreach (var orderData in jsondata.data)
        //        {
        //            try
        //            {
        //                OrderInfo orderInfo = ConvertUserToCustomer(orderData);
        //                OrderBLL.SyncCustomer(orderInfo);
        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 同步柜子信息
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool SyncCabinet(string cookie)
        {
            try
            {
                string res = WebRequestPostOrGet(CabinetUrl, cookie);
                CabinetResult jsondata = new CabinetResult();
                jsondata = JsonConvert.DeserializeObject<CabinetResult>(res);
                foreach (var cabinetData in jsondata.data)
                {
                    try
                    {
                        Cabinet cabinet = ConvertMachineToCabinet(cabinetData);
                        CabinetBLL.AddNewCabinet(cabinet);
                    }
                    catch
                    {
                        continue;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 同步瓶子信息
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool SyncBottle(string cookie, string page)
        {
            try
            {
                string res = WebRequestPostOrGet(string.Format(BottleUrl, page), cookie);
                BottleResult jsondata = new BottleResult();
                jsondata = JsonConvert.DeserializeObject<BottleResult>(res);
                foreach (var bottleData in jsondata.data)
                {
                    try
                    {
                        Bottle bottle = ConvertGoodsToBottle(bottleData);
                        BottleBLL.AddNewBottle(bottle);
                    }
                    catch
                    {
                        continue;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region 方法
        /// <summary>
        /// 请求接口
        /// </summary>
        /// <returns></returns>
        public static string WebRequestPostOrGet(string url, string cookie = "", string requestType = "get", string jsonParam = "")
        {
            string res = string.Empty;
            using (var wl = new WebClient())
            {
                wl.Headers.Add(HttpRequestHeader.Accept, "json");
                wl.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
                wl.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0");
                //cookie 得实时登录写到请求接口里实时传递
                wl.Headers.Add(HttpRequestHeader.Cookie, cookie);
                wl.Encoding = Encoding.UTF8;
                if (requestType == "post")
                {
                    res = wl.UploadString(url, jsonParam);
                }
                else
                {
                    res = wl.DownloadString(url);
                }
            }
            return res;
        }
        public static Customer ConvertUserToCustomer(UserData userData)
        {
            Customer customer = new Customer()
            {
                OpenID = userData.openid,
                Telphone = userData.mobile,
                MemberType = userData.level_id == 1 ? "ordinary" : userData.level_id == 2 ? "silver" : "gold",
                MemberPoints = userData.point,
                WXAuthorizationCode = userData.wx_authorization_code,
                WXPayScoreOpenId = userData.wx_payscore_openid,
                WXPayScoreState = userData.wx_payscore_state,
                RegistTime = userData.created_at,
            };
            return customer;
        }

        public static Cabinet ConvertMachineToCabinet(CabinetData cabinetData)
        {
            Cabinet cabinet = new Cabinet()
            {
                Name = cabinetData.machine_name,
                Number = cabinetData.machine_no,
                Address = cabinetData.address,
                Longitude = (float)Convert.ToDecimal(cabinetData.lng),
                Latitude = (float)Convert.ToDecimal(cabinetData.lat),
                DeviceCode = cabinetData.imei,
                DeviceType = "cabinet",
                Status = "2",
            };
            return cabinet;
        }

        public static Bottle ConvertGoodsToBottle(BottleData bottleData)
        {
            Bottle bottle = new Bottle()
            {
                Spec = bottleData.goods_name,
                Code = bottleData.spec_no,
                Status = "1",
                PurchaseTime = bottleData.created_at,
                CreateTime = bottleData.created_at,
                QRCode = "https://fbj.openvem.com" + bottleData.qrcode,
                IsDeleted = bottleData.is_del,
                IsOldData = "1",
            };
            return bottle;
        }

        //public static OrderInfo ConvertUserToCustomer(OrderData orderData)
        //{
        //    Customer customer = new Customer()
        //    {
        //        OpenID = userData.openid,
        //        Telphone = userData.mobile,
        //        MemberType = userData.level_id == 1 ? "ordinary" : userData.level_id == 2 ? "silver" : "gold",
        //        MemberPoints = userData.point,
        //        WXAuthorizationCode = userData.wx_authorization_code,
        //        WXPayScoreOpenId = userData.wx_payscore_openid,
        //        WXPayScoreState = userData.wx_payscore_state,
        //        RegistTime = userData.created_at,
        //    };
        //    return customer;
        //}
        #endregion
    }
}
