using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Utilities
{
    public static class AppSettings  //总的类 对应 json大文件 这里负责一一绑定json模块与对象
    {
        public static DBSettings DB { get; set; } = new DBSettings();
        public static WeChatSettings WeChat { get; set; } = new WeChatSettings();
        public static WebSettings Web { get; set; } = new WebSettings();
        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="configuration"></param>
        public static void Init(IConfiguration configuration)
        {
            configuration.Bind("DB", DB);
            configuration.Bind("WeChat", WeChat);

            // TODO: 从文件中读取MerchantCertPrivateKey
            configuration.Bind("Web", Web);
        }
    }

    public class DBSettings
    {
        public string DBName { get; set; }
        public string DBPort { get; set; }
        public string DBPwd { get; set; }
        public string DBServer { get; set; }
        public string DBUser { get; set; }
    }

    public class WeChatSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string MerchantId { get; set; }
        public string MerchantV3Secret { get; set; }
        public string MerchantCertSerialNumber { get; set; }
        public string MerchantCertPrivateKey { get; set; }
        public string Token { get; set; }
    }
    public class WebSettings
    {
        public string AttachPath { get; set; }
        public string DomainURL { get; set; }
        public string EncodingAESKey { get; set; }
        public string WechatHost { get; set; }
    }

}
