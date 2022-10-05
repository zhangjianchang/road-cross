using Api.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class Bottle
    {
        public bool IsAdd { get; set; }
        public string ID { get; set; }
        public string Code { get; set; }
        public string Spec { get; set; }
        public string SpecName { get; set; }
        public string Status { get; set; }
        public string PurchaseTime { get; set; }
        public string PurchasePerson { get; set; }
        public string QRCode { get; set; }
        public string IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public string GridID { get; set; }
        public string GridNumber { get; set; }
        public string CabinetNumber { get; set; }
        public string CabinetName { get; set; }
        public string Error { get; set; }
        public string IsOldData { get; set; }


        /// <summary>
        /// 瓶子状态描述
        /// </summary>
        public string StatusDesc
        {
            get
            {
                return BottleStatus.StatusDesc(Status);
            }
        }

        public string GridDesc
        {
            get
            {
                return string.IsNullOrEmpty(GridNumber) ? "" : $"{CabinetName}/{GridNumber}";
            }
        }
        /// <summary>
        /// 瓶子二维码内容
        /// </summary>
        public string QRCodeContent
        {
            get
            {
                List<string> list = new List<string>();
                list.Add(Config.WechatHost.TrimEnd('/'));
                list.Add("b");
                list.Add(Code);

                return string.Join('/', list.ToArray());
            }
        }
        public string RFID { get; set; }

    }


    public class BottleParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string Spec { get; set; }
        public string NotBind { get; set; }
    }
}
