using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class WeXinInfo
    {
        public string appId { get; set; }
        public string jsapi_ticket { get; set; }
        public long timestamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }
        public string jsApiList { get; set; }
    }

    public class TemplateInfo
    {
        public string touser { get; set; }
        public string template_id { get; set; }
        /// <summary>
        /// 如果不跳转至小程序可不填
        /// </summary>
        public string url { get; set; }
        public TemplateDetailInfo data { get; set; }
    }

    public class TemplateDetailInfo
    {
        public TemplateDetailSet first { get; set; }
        public TemplateDetailSet keyword1 { get; set; }
        public TemplateDetailSet keyword2 { get; set; }
        public TemplateDetailSet remark { get; set; }
    }

    public class TemplateDetailSet
    {
        public string value { get; set; }
        public string color { get; set; }
    }
}
