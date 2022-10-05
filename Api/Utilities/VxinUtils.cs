using Api.Entity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utilities
{
    /// <summary>
    /// 微信公众号工具类
    /// </summary>
    public class VxinUtils
    {

        #region 公共字段定义

        /// <summary>
        /// 微信token
        /// </summary>
        public static string WeiXinToken { get { return Environment.GetEnvironmentVariable("Token") ?? ConfigurationManager.AppSettings["Token"]; } }

        /// <summary>
        /// 微信小程序appID
        /// </summary>
        public static string AppID { get { return Environment.GetEnvironmentVariable("AppId") ?? ConfigurationManager.AppSettings["AppId"]; } }

        /// <summary>
        /// 微信小程序Aappsecret
        /// </summary>
        public static string AppSecret { get { return Environment.GetEnvironmentVariable("AppSecret") ?? ConfigurationManager.AppSettings["AppSecret"]; } }

        /// <summary>
        /// 微信小程序appID
        /// </summary>
        public static string OpenAppID { get { return Environment.GetEnvironmentVariable("OpenAppID") ?? ConfigurationManager.AppSettings["OpenAppID"]; } }

        /// <summary>
        /// 微信小程序Aappsecret
        /// </summary>
        public static string OpenAppSecret { get { return Environment.GetEnvironmentVariable("AppSecret") ?? ConfigurationManager.AppSettings["OpenAppSecret"]; } }

        /// <summary>
        /// 微信商户
        /// </summary>
        public static string MerchantId { get { return Environment.GetEnvironmentVariable("MerchantId") ?? ConfigurationManager.AppSettings["MerchantId"]; } }

        /// <summary>
        /// 微信商户 v3 API 密钥
        /// </summary>
        public static string MerchantV3Secret { get { return Environment.GetEnvironmentVariable("MerchantV3Secret") ?? ConfigurationManager.AppSettings["MerchantV3Secret"]; } }

        /// <summary>
        /// 微信商户证书序列号
        /// </summary>
        public static string MerchantCertSerialNumber { get { return Environment.GetEnvironmentVariable("MerchantCertSerialNumber") ?? ConfigurationManager.AppSettings["MerchantCertSerialNumber"]; } }

        /// <summary>
        /// 微信商户证书私钥
        /// </summary>
        public static string MerchantCertPrivateKey { get { return Environment.GetEnvironmentVariable("MerchantCertPrivateKey") ?? ConfigurationManager.AppSettings["MerchantCertPrivateKey"]; } }

        /// <summary>
        /// 获得公众号access_token地址
        /// </summary>
        public static string Access_Token_URL { get { return string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", OpenAppID, OpenAppSecret); } }

        /// <summary>
        /// 获得小程序access_token地址
        /// </summary>
        public static string App_Access_Token_URL { get { return string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", AppID, AppSecret); } }

        /// <summary>
        /// 通过code换取网页授权access_token地址
        /// </summary>
        public static string Web_Access_Token_URL { get { return string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&grant_type=authorization_code&code=", AppID, AppSecret); } }

        /// <summary>
        /// 小程序获取OPENID授权
        /// </summary>
        public static string Mini_Access_Token_URL { get { return string.Format("https://api.weixin.qq.com/sns/jscode2session?connect_redirect=1&appid={0}&secret={1}&grant_type=authorization_code&js_code=", AppID, AppSecret); } }

        /// <summary>
        /// 微信菜单创建提交地址
        /// </summary>
        public static string MENU_POST_URL { get { return "https://api.weixin.qq.com/cgi-bin/menu/create?access_token="; } }

        /// <summary>
        /// 微信获取用户分组地址
        /// </summary>
        public static string Group_Get_Url { get { return "https://api.weixin.qq.com/cgi-bin/groups/get?access_token="; } }

        /// <summary>
        /// 微信修改用户分组地址
        /// </summary>
        public static string Group_Update_Url { get { return "https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token="; } }

        /// <summary>
        /// 微信获取用户所属分组地址
        /// </summary>
        public static string Group_GetUserGroup_Url { get { return "https://api.weixin.qq.com/cgi-bin/groups/getid?access_token="; } }

        /// <summary>
        /// 拉取用户信息(需scope为 snsapi_userinfo)
        /// 如果网页授权作用域为snsapi_userinfo，则此时开发者可以通过access_token和openid拉取用户信息了。
        /// https://api.weixin.qq.com/sns/userinfo?access_token=ACCESS_TOKEN&openid=OPENID&lang=zh_CN
        /// </summary>
        public static string GetUserUserinfo_Url { get { return "https://api.weixin.qq.com/sns/userinfo?lang=zh_CN&access_token={0}&openid={1}"; } }

        /// <summary>
        /// 模板名称(公众号后台配置)
        /// </summary>
        public static string GetTemplate_Name { get { return "TM001"; } }//暂，申请下来替换

        /// <summary>
        /// 派送模板ID
        /// </summary>
        public static string Template_Deliver_ID { get { return "uJfeEXrBUa_PB0f04hP9XODyJhd8RFEU_NHpZIqKxwo"; } }

        /// <summary>
        /// 签收模板ID
        /// </summary>
        public static string Template_Sign_ID { get { return "RCnA81CyCZoaOADZV-Yb6dPw6-bm2f9w4JuSEjd0aRg"; } }

        /// <summary>
        /// 获取模板ID
        /// </summary>
        public static string GetTemplate_Url { get { return "https://api.weixin.qq.com/cgi-bin/template/api_add_template?access_token={0}"; } }

        /// <summary>
        /// 公众号发送消息
        /// </summary>
        public static string SendTemlpateMessage_Url { get { return "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}"; } }

        /// <summary>
        /// 小程序发送消息
        /// </summary>
        public static string Uniform_Send_Message_Url { get { return "https://api.weixin.qq.com/cgi-bin/message/wxopen/template/uniform_send?access_token={0}"; } }

        public static string WX_Response_Url { get { return "http://www.code-data.cn/#/search?order_num={0}"; } }

        /// <summary>
        /// 获取签名
        /// </summary>
        public static string Wx_Certificates_Url { get { return "https://api.mch.weixin.qq.com/v3/certificates"; } }

        /// <summary>
        /// V3支付接口
        /// </summary>
        public static string WX_PrePayment_Url { get { return "https://api.mch.weixin.qq.com/v3/pay/transactions/jsapi"; } }

        /// <summary>
        /// 获取自身的开票平台识别码
        /// </summary>
        public static string Invoice_Url { get { return "https://api.weixin.qq.com/card/invoice/seturl?access_token={0}"; } }

        /// <summary>
        /// 获取ticket
        /// </summary>
        public static string Get_Ticket_WX_Card_Url { get { return "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=wx_card"; } }

        /// <summary>
        /// 获取授权页链接
        /// </summary>
        public static string Get_Auth_Url { get { return "https://api.weixin.qq.com/card/invoice/getauthurl?access_token={0}"; } }

        /// <summary>
        /// 获取授权页链接
        /// </summary>
        public static string Set_Biz_Attr_Url { get { return "https://api.weixin.qq.com/card/invoice/setbizattr?action=set_contact&access_token={0}"; } }

        /// <summary>
        /// 获取授权页链接
        /// </summary>
        public static string Create_Card_Url { get { return "https://api.weixin.qq.com/card/invoice/platform/createcard?access_token={0}"; } }
        #endregion

        #region 支付相关
        public static async Task<bool> GetWxRefundRequest(OrderInfoParam orderInfo)
        {
            IDictionary<string, string> resDic = new Dictionary<string, string>();
            try
            {
                var certManager = new InMemoryCertificateManager();
                var options = new WechatTenpayClientOptions()
                {
                    MerchantId = MerchantId,
                    MerchantV3Secret = MerchantV3Secret,
                    MerchantCertSerialNumber = MerchantCertSerialNumber,
                    MerchantCertPrivateKey = MerchantCertPrivateKey,
                    CertificateManager = certManager // 证书管理器的具体用法请参阅下文的高级技巧
                };
                var client = new WechatTenpayClient(options);
                var request = new CreateRefundDomesticRefundRequest()
                {
                    OutTradeNumber = orderInfo.OrderNo,
                    OutRefundNumber = "RE_" + orderInfo.OrderNo,
                    Amount = new CreateRefundDomesticRefundRequest.Types.Amount()
                    {
                        Total =  (int)(orderInfo.TotalAmount * 100),
                        Refund =  (int)(orderInfo.RefundTotal * 100),
                    },
                    Reason = orderInfo.Remark ?? "押金退还",
                    // TODO: 修改通知接口
                    NotifyUrl = Config.DomainURL + "wechartapi/api/wx/GetOpenId",
                };
                var response = await client.ExecuteCreateRefundDomesticRefundAsync(request);
                if (!response.IsSuccessful())
                {
                    throw new MsgException($"状态码：{response.RawStatus}，错误代码：{response.ErrorCode}，错误描述：{response.ErrorMessage}");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new MsgException("微信退款申请失败：" + ex.Message);
            }
        }


        /// <summary>
        /// 把对象序列化 JSON 字符串 
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象实体</param>
        /// <returns>JSON字符串</returns>
        public static string GetJson<T>(T obj)
        {
            //记住 添加引用 System.ServiceModel.Web 
            /**
             * 如果不添加上面的引用,System.Runtime.Serialization.Json; Json是出不来的哦
             * */
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
            using MemoryStream ms = new MemoryStream();
            json.WriteObject(ms, obj);
            string szJson = Encoding.UTF8.GetString(ms.ToArray());
            return szJson;
        }
        #endregion
    }
}
