import _ from "lodash";

//道路model
export const road_model = {
  canalize_info: [] as any[],

  flow_info: {
    colorScheme: 0, //颜色
    thickness: 5, //粗细
    width: 100, //长度
    space: 24, //间距
    font_size1: 14, //字号1
    font_size2: 16, //字号2
    line_info: [] as any[], //车道属性
    flow_detail: [] as any[], //进口道转向流量
    saturation: [] as any,
    flowColumns: [] as any[],
  },

  signal_info: {
    phase: 4, //默认4个相位
    period: 0, //默认周期共80s
    is_show_legend: 0,
    phase_list: [] as any[],
  },

  saturation_info: [] as any[],
  delay_info: [] as any[],
  queue_info: [] as any[],
};

//方案model
export const plans_model = {
  center: undefined as any, //地图中心
  zoom: 16, //地图比例尺
  road_name: "", //交叉口名称
  road_count: 0, //交叉口数量
  road_attr: [] as any[], //道路基础信息

  canalize_plans: [
    {
      index: 0,
      name: "渠化方案1",
      flow_plans: [
        {
          index: 0,
          name: "流量方案1",
          signal_plans: [
            {
              index: 0,
              name: "信号方案1",
              road_info: _.cloneDeep(road_model),
            },
          ],
        },
      ],
    },
  ],
  //饱和度对比方案
  saturationAnalysis: [
    {
      name: "方案1",
      canalize_plan: 0,
      flow_plan: 0,
      signal_plan: 0,
    },
  ],
  //延误对比方案
  delayAnalysis: [
    {
      name: "方案1",
      canalize_plan: 0,
      flow_plan: 0,
      signal_plan: 0,
    },
  ],
  //排队对比方案
  queueAnalysis: [
    {
      name: "方案1",
      canalize_plan: 0,
      flow_plan: 0,
      signal_plan: 0,
    },
  ],
};

// 统计报表的样式
export const echart_tooltip = {
  trigger: "axis",
  axisPointer: {
    type: "cross",
    crossStyle: {
      color: "#999",
    },
  },
};

export const echart_toolbox = {
  feature: {
    // magicType: { show: true, type: ["line", "bar"] },
    // restore: { show: true },
    saveAsImage: { title: "下载", show: true },
  },
};

export const DirectionsZh = {
  uturn: "掉头",
  left: "左转",
  straight_left: "直左",
  straight: "直行",
  straight_right: "直右",
  left_straight_right: "直左右",
  right: "右转",
  straight_uturn: "直行掉头",
  left_uturn: "左转掉头",
};
