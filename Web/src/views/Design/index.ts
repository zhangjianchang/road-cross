import { reactive } from "vue";

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
  basic_info: {} | any;
  road_attr: [] | any;
  canalize_info: {} | any;
  flow_info: {} | any;
  saturation_info: [] | any;
  signal_info: {} | any;
}

export const road_info = reactive({
  //基础信息
  basic_info: {
    name: "", //道路名称
    count: 0,
  },
  //交叉路口信息
  road_attr: [] as any[],
  //渠化信息
  canalize_info: {
    direction: "road_1", //当前方向
    direction_index: 0, //当前方向索引
    size: 5, //交叉路口大小
    curvature: 0.5, //右转道路曲率
    roadAttr: [] as any[], //道路属性
    entranceAttr: [] as any[], //进口属性
    exitAttr: [] as any[], //出口属性
  } as any,
  //流量信息
  flow_info: reactive({
    colorScheme: 0, //颜色
    thickness: 5, //粗细
    width: 100, //长度
    space: 24, //间距
    font_size1: 14, //字号1
    font_size2: 16, //字号2
    line_info: [] as any[], //车道属性
    flow_detail: [] as any[], //进口道转向流量
    saturation: [1650, 1650, 1650],
    flowColumns: [] as any[],
  }),
  //信号信息
  signal_info: {
    phase: 2, //默认2个相位
    period: 0, //默认周期共80s
    is_show_legend: false,
    phase_list: [] as any[],
  },
  //饱和度
  saturation_info: [] as any[],
});
