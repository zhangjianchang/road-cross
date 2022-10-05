using System;
using System.IO;

namespace Api.Utilities
{
    public class Config
    {
        public static string DBConnection
        {
            get
            {
                string DBServer = AppSettings.DB.DBServer;
                string DBName = AppSettings.DB.DBName;
                string DBUser = AppSettings.DB.DBUser;
                string DBPwd = AppSettings.DB.DBPwd;
                string DBPort = AppSettings.DB.DBPort;

                return $"Server={DBServer};Database={DBName};User ID={DBUser};Password={DBPwd};port={DBPort};pooling=true;Charset=utf8";

            }
        }
        public static string DefaultPassword
        {
            get { return "123456"; }
        }

        public static string DomainURL
        {
            get { return AppSettings.Web.DomainURL; }
        }
        public static string AttachPath
        {
            get { return AppSettings.Web.AttachPath; }
        }
        public static string NewAttachFolder
        {
            get
            {
                var now = DateTime.Now;
                //文件存储路径
                var dataPath = string.Format("/{0}/{1}/{2}", now.ToString("yyyy"), now.ToString("MM"), now.ToString("dd"));
                //获取当前web目录
                var webRootPath = Config.AttachPath;

                string newFolder = webRootPath + dataPath;
                if (!Directory.Exists(newFolder))
                {
                    Directory.CreateDirectory(newFolder);
                }
                return newFolder;
            }
        }

        public static string WechatHost
        {
            get { return AppSettings.Web.WechatHost; }
        }
    }
}
