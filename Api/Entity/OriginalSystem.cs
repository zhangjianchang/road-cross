using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class OriginalSystem
    {
    }

    public class UserResult
    {
        public int code { get; set; }
        public int count { get; set; }
        public List<UserData> data { get; set; }

        public class UserData
        {
            public string account_type { get; set; }
            public string agreement_no { get; set; }
            public string balance { get; set; }
            public string created_at { get; set; }
            public string deposit { get; set; }
            public string external_agreement_no { get; set; }
            public string headimgurl { get; set; }
            public string id { get; set; }
            public int level_id { get; set; }
            public string level_name { get; set; }
            public string mobile { get; set; }
            public string nickname { get; set; }
            public string openid { get; set; }
            public decimal point { get; set; }
            public string real_name { get; set; }
            public string store_id { get; set; }
            public string vip_amount { get; set; }
            public string wx_authorization_code { get; set; }
            public string wx_payscore_openid { get; set; }
            public string wx_payscore_state { get; set; }
        }
    }

    public class CabinetResult
    {
        public int code { get; set; }
        public int count { get; set; }
        public List<CabinetData> data { get; set; }

        public class CabinetData
        {
            public string address { get; set; }
            public string created_at { get; set; }
            public string imei { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string machine_name { get; set; }
            public string machine_no { get; set; }
            public string machine_type { get; set; }
            public string offline_time { get; set; }
            public string online_status { get; set; }
            public string online_time { get; set; }
            public string pay_amount { get; set; }
            public string updated_at { get; set; }
            public string upload_log_switch { get; set; }
        }
    }

    public class BottleResult
    {
        public int code { get; set; }
        public int count { get; set; }
        public List<BottleData> data { get; set; }

        public class BottleData
        {
            public string created_at { get; set; }
            public string goods_name { get; set; }
            public string img { get; set; }
            public string is_del { get; set; }
            public string qrcode { get; set; }
            public string sale_price { get; set; }
            public string spec_no { get; set; }
        }
    }

    public class OrderResult
    {
        public int code { get; set; }
        public int count { get; set; }
        public List<OrderData> data { get; set; }

        public class OrderData
        {
            //public string address                                    "拉萨市城关区宇拓路38号"
            //public string complete_time                                    "2022-05-03 14                                   43                                   10"
            //public string coupon_amount                                    "0.00"
            //public string coupon_id                                    0
            //public string created_at                                    "2022-05-02 22                                   11                                   03"
            //public string created_date                                    "2022-05-02"
            //public string deliver_status                                    1
            //public string deliver_status_text                                    "已出货"
            //public string late_fee                                    "0.00"
            //public string machine_id                                    28
            //public string machine_name                                    "唐卡酒店柜机"
            //public string machine_no                                    "21091300286"
            //public string member_discount                                    "0.00"
            //public string mobile                                    "13908208685"
            //public string open_time                                    "2022-05-02 22                                   11                                   47"
            //public string openid                                    ""
            //public string order_amount                                    "69.00"
            //public string order_goods_list                                    [{id                                    776, order_id                                    674, goods_id                                    5, spec_id                                    580, spec_no                                    "968200510", goods_num                                    1,…}]
            //public string order_no                                    "22050222110324712"
            //public string order_status                                    5
            //public string order_status_text                                    "已完成"
            //public string order_type                                    1
            //public string pay_amount                                    "69.00"
            //public string pay_code                                    "wx_payscore"
            //public string pay_name                                    "微信支付分"
            //public string pay_status                                    1
            //public string pay_status_text                                    "已支付"
            //public string pay_time                                    "2022-05-03 14                                   43                                   10"
            //public string remark                                    ""
            //public string return_time                                    null
            //public string store_id                                    2
            //public string total_amount                                    "69.00"
            //public string total_num                                    1
            //public string transaction_id                                    "4200001453202205037954099163"
            //public string user_id                                    279
            //public string wx_template_id                                    "WNZsZEEqdwLqd6FS9EYeysmBmOibq08cmjombylrN-g"
            //public string wx_touser                                    "oIEFc4zAZPZgYmkaS2kUYHaVjzfE"
        }
    }
}
