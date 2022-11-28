export enum MenuListEnum {
  Basic = "Basic",
  Canalize = "Canalize",
  Flow = "Flow",
  Saturation = "Saturation",
  Signal = "Signal",
  Delay = "Delay",
}

export const menuList = [
  { index: 1, url: "Basic", name: "道路" },
  { index: 2, url: "Canalize", name: "渠化" },
  { index: 3, url: "Flow", name: "流量" },
  { index: 4, url: "Signal", name: "信号" },
  { index: 5, url: "5", name: "断面" },
  { index: 6, url: "Saturation", name: "饱和度" },
  { index: 7, url: "7", name: "排队分析" },
  { index: 8, url: "Delay", name: "延误分析" },
  { index: 9, url: "9", name: "服务水平" },
];

export interface RoadInfo {
  canalize_info: {} | any;
  flow_info: {} | any;
  saturation_info: [] | any;
  signal_info: {} | any;
}
