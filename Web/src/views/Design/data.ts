import _ from "lodash";
import { buildUUID } from "../../utils/uuid";

//道路model
export const road_model = {
  basic_info: {
    createDate: "",
    updateDate: "",
    count: 0,
  },

  road_attr: [] as any[],

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
    phase: 2, //默认2个相位
    period: 0, //默认周期共80s
    is_show_legend: false,
    phase_list: [] as any[],
  },

  saturation_info: [] as any[],
};

//方案model
export const plans_model = {
  id: buildUUID(),
  road_name: "", //交叉口名称
  road_count: 0, //交叉口数量
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
};
