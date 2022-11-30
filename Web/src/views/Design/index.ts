export enum MenuListEnum {
  Basic = "Basic", //基础信息
  Canalize = "Canalize", //渠化
  Flow = "Flow", //流量
  Signal = "Signal", //信号
  Section = "Section", //断面
  Saturation = "Saturation", //饱和度
  QueueAnalysis = "QueueAnalysis", //排队分析
  DelayAnalysis = "DelayAnalysis", //延误分析
  ServiceLevel = "ServiceLevel", //服务水平
}

export const menuList = [
  { index: 1, url: "Basic", name: "道路" },
  { index: 2, url: "Canalize", name: "渠化" },
  { index: 3, url: "Flow", name: "流量" },
  { index: 4, url: "Signal", name: "信号" },
  { index: 5, url: "Section", name: "断面" },
  { index: 6, url: "Saturation", name: "饱和度" },
  { index: 7, url: "QueueAnalysis", name: "排队分析" },
  { index: 8, url: "DelayAnalysis", name: "延误分析" },
  { index: 9, url: "ServiceLevel", name: "服务水平" },
];

export interface RoadInfo {
  canalize_info: {} | any;
  flow_info: {} | any;
  saturation_info: [] | any;
  signal_info: {} | any;
}
