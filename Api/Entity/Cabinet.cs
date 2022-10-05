using Api.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class Cabinet
    {
        public bool IsAdd { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string DeviceCode { get; set; }
        public string QRCode { get; set; }
        public string DeviceStatus { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }
        public string Status { get; set; }
        public string ContactPerson { get; set; }
        public string CCSStoreID { get; set; }
        public string SFStoreID { get; set; }
        public string ContactPhone { get; set; }
        public int IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        /// <summary>
        /// 经纬度坐标
        /// </summary>
        public string coordinates
        {
            get
            {
                return $"纬度:{Latitude.ToString("F3")},经度:{Longitude.ToString("F3")}";
            }
        }
        /// <summary>
        /// 柜子所在区域
        /// </summary>
        public string AreaName
        {
            get
            {
                return $"{Province}{City}{Country}";
            }
        }
        /// <summary>
        /// 柜子状态描述
        /// </summary>
        public string StatusDesc
        {
            get
            {
                return CabinetStatus.StatusDesc(Status);
            }
        }
        /// <summary>
        /// 柜子设备状态描述
        /// </summary>
        public string DeviceStatusDesc
        {
            get
            {
                return CabinetDeviceStatus.DeviceStatusDesc(Status);
            }
        }
        /// <summary>
        /// 箱子/柜子二维码内容
        /// </summary>
        public string QRCodeContent
        {
            get
            {
                List<string> list = new List<string>();
                list.Add(Config.WechatHost.TrimEnd('/'));
                list.Add("c");
                list.Add(Number);

                return string.Join('/', list.ToArray());
            }
        }
    }


    public class CabinetParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Number { get; set; }
        public string ContactPhone { get; set; }
        public string ContactPerson { get; set; }
        public string Status { get; set; }
        public string DeviceType { get; set; }
    }
}
