import { reactive } from "vue";
import { plans } from "..";
import { cal_point } from "../../../utils/common";

export const roadSigns = reactive([
  {
    key: "uturn",
    name: "掉头",
    path: "M370.08 193.376C370.08 53.088 462.048 0 550.72 0c88.672 0 180.896 53.84 180.896 194.144V1024H619.04V194.144c0-65.472-26.944-89.008-68.32-89.008-41.376 0-75.104 22.864-74.8 88.24h64.224L421.648 573.136 303.152 193.376H370.08z",
  },
  {
    key: "left_uturn",
    name: "左转+掉头",
    path: "M697.056 1024h-128.448V640c0-17.52-16.224-41.76-48.672-41.76-32.448 0-49.36 24.08-49.36 41.76v156.576h65.92L439.84 1024 343.824 796.576h66.592V640c0-41.76 29.744-75.456 60.848-91.616 31.088-16.16 74.352-8.096 97.344 3.52V448l-96.336-96v128L311.712 192.256 472.272 0v128.768L695.84 352l1.216 672z",
  },
  {
    key: "left",
    name: "左转",
    path: "M456.784 146.352V0.24l-136.96 250.56 136.96 250.448v-146.096l137.12 92.752V1023.04h109.488V313.392z",
  },
  {
    key: "straight_left",
    name: "直行+左转",
    path: "M619.824 1024V446.784h84.464L577.408 0 446 446.784h84.144v303.728l-135.104-136.096V446.352L320.016 718.736l75.04 258.688v-153.504l135.104 128.16V1024z",
  },
  {
    key: "straight_uturn",
    name: "直行+掉头",
    path: "M588.864 0l115.168 447.872h-77.216V1024H544.448V447.872H471.792L588.864 0z m-44.56 551.92V640c0-17.52-16.16-41.76-48.512-41.76-32.32 0-49.168 24.08-49.168 41.76v156.576h65.68L415.984 1024 320.304 796.576h66.352V640c0-41.76 29.648-75.456 60.64-91.616 30.992-16.16 74.096-8.096 97.008 3.52z",
  },
  {
    key: "straight",
    name: "直行",
    path: "M461.536 447.872H376.192L512.896 0l133.12 447.872h-89.248V1024H461.536z",
  },
  {
    key: "right",
    name: "右转",
    path: "M559.356062 144.257969V0.236308l135.483076 247.004554-135.483076 246.862769v-144.021662l-135.640616 91.435323v566.980923H315.423508V308.901415z",
  },
  {
    key: "straight_right",
    name: "直行+右转",
    path: "M404.176 1024V446.784H319.712L446.592 0l131.408 446.784h-84.144v303.728l135.104-136.096V446.352l75.04 272.384-75.04 258.688v-153.504l-135.104 128.16V1024z",
  },
  {
    key: "left_right",
    name: "左转+右转",
    path: "M512.064 288.48l159.392-159.68V0l160 192.256-160 287.744v-128l-96 96v576h-126.784V448l-96-96v128l-160-287.744L352.672 0v128.768z",
  },
  {
    key: "left_straight_right",
    name: "所有方向",
    path: "M456.944 657.488V447.872h-90.832L512.464 0l143.952 447.872h-96.528v209.04l112.16-95.296v-128.768l160.288 192.256-160.304 287.744v-128l-112.144 95.104V1024H456.944v-143.472l-104.304-95.68v128L192.336 625.104l160.32-192.256V561.6z",
  },
  // {
  //   key: "variable",
  //   name: "可变车道",
  //   path: "",
  // },
  // {
  //   key: "tidal_lane",
  //   name: "潮汐车道",
  //   path: "",
  // },
  // {
  //   key: "bus_only",
  //   name: "公交专用",
  //   path: "",
  // },
  // {
  //   key: "none",
  //   name: "无",
  //   path: "",
  // },
  {
    hide: true,
    key: "reverse_straight",
    name: "逆行",
    path: "m465.999916,1.936573l2.000937,575.397813l-94.666839,-1.333336l138.666919,446.667481l137.333584,-445.334145l-85.333489,-1.333336l0,-576.00105",
  },
]);

export const bicycle_path =
  "M661.332 262.404c37.336 0 67.196-29.87 67.196-67.202S698.668 128 661.332 128C624 128 594.14 157.87 594.14 195.202s29.86 67.202 67.192 67.202z m112 260.264c-102.664 0-186.664 84-186.664 186.666s84 186.666 186.664 186.666C876 896 960 812 960 709.334s-84-186.666-186.668-186.666z m0 317.332c-72.804 0-130.664-57.86-130.664-130.666s57.86-130.666 130.664-130.666c72.808 0 130.668 57.86 130.668 130.666S846.14 840 773.332 840z m-162.138-392H768v-64h-117.69l-69.24-120.268c-11.21-18.666-31.738-31.728-54.14-31.728-16.798 0-33.596 7.464-44.798 18.666L338.668 388.8c-11.202 11.202-18.666 28-18.666 44.798 0 24.262 18.404 42.93 37.07 54.13L480 564.268V736h64V512l-78.666-64 85.858-89.066L611.194 448z m-360.528 74.668C148 522.668 64 606.668 64 709.334S148 896 250.666 896s186.666-84 186.666-186.666-83.998-186.666-186.666-186.666z m0 317.332C177.868 840 120 782.14 120 709.334s57.868-130.666 130.666-130.666 130.666 57.86 130.666 130.666S323.464 840 250.666 840z";

export enum roadSignKey {
  uturn = "uturn",
  left_uturn = "left_uturn",
  left = "left",
  straight_uturn = "straight_uturn",
  straight_left = "straight_left",
  straight = "straight",
  straight_right = "straight_right",
  right = "right",
  left_right = "left_right",
  left_straight_right = "left_straight_right",
}

export const throughTypeOption = [
  { label: "无", value: "无" },
  { label: "贯通", value: "贯通" },
  { label: "隔离桩", value: "隔离桩" },
  { label: "斑马线", value: "斑马线" },
];

export const canalizeTypeOption = [
  { label: "否", value: "否" },
  { label: "划线渠化", value: "划线渠化" },
  { label: "固体渠化", value: "固体渠化" },
];

export const medianStripTypeOption = [
  { label: "双黄线", value: "双黄线" },
  { label: "单黄线", value: "单黄线" },
  { label: "护栏", value: "护栏" },
  { label: "鱼肚线", value: "鱼肚线" },
  { label: "黄斜线", value: "黄斜线" },
  { label: "绿化带", value: "绿化带" },
];

const states = {
  cx: 350,
  cy: 350,
};

//定义每条道路
export const RoadCross = {
  name: "方向1",

  angle: 0, // 方向角度
  origin: { x: 0, y: 0 }, // 原点，绘图中心，单位
  offset: 0.5, // 偏移量
  length: 80, // 路长125m
  cross_len: 20, // 交叉口长度m（距离原点），TODO：当右转路夹角很小时，该长度变长
  cross_len_new: 20, // 根据真实情况会对原始cross_len做后移调整
  stop_line_length: 30, //长实线长度

  direction_num: {
    uturn: 0, //掉头
    left: 1, //专左
    straight: 1.5, //直行
    right: 0.5, //右转
  },

  speed: 40, //路段速度

  walk: { has: 1, zebra_len: 5 }, // 人行道
  margin: 1, // 路边

  canalize: {
    //渠化属性
    type: "否", //右转渠化
    right_enter_count: 0, //右转单独入口
    right_exit_count: 0, //右转单独出口
    lane_width: 3.5, //出入口宽度
  },

  enter: {
    // 进口
    num: 3, // 进口车道数
    lane_width: 3.5, //车道宽度，米
    extend_num: 0, // 展宽数量
    extend_len: 30, // 展宽段长
    extend_width: 3, // 展宽车道宽度
    in_curv: 10, // 内侧渐变段长
    out_curv: 10, // 外侧渐变段长
    offset: 0, // 内侧偏移
    bike_lane: {
      // 非机动车道
      has: 0, // 有否1/0
      width: 4, // 车道宽度，米
      div_type: "划线", // 分割形式与宽度：划线-0.25m，护栏-1m，绿化带-1m
    },
    right_curv: 0.5, // 右转曲度[0, 1]
  },
  exit: {
    // 出口
    num: 3, // 出口车道数
    lane_width: 3.5, //车道宽度
    extend_num: 0, // 展宽数量
    extend_len: 30, // 展宽段长
    extend_width: 3, // 展宽车道宽度
    in_curv: 10, // 内侧渐变段长
    out_curv: 10, // 外侧渐变段长
    bike_lane: {
      // 非机动车道
      has: 0, // 有否1/0
      width: 4, // 车道宽度，米
      div_type: "划线", // 分割形式与宽度：划线-0.25m，护栏-1m，绿化带-1m
    },
  },

  median_strip: {
    type: "双黄线", // 分割形式：双黄线，单黄线，护栏，鱼肚线，黄斜线，绿化带
    width: 0.5, // 分割带宽：双黄线3像素，单黄线1像素，护栏4像素，鱼肚线4像素，
    safe_land: 0, // 安全岛
    turn: "否", // 提前掉头：否，停车线位置，停车线上游
  },

  wait: {
    left: 0, // 左转待转
    straight: 0, // 直行等待
    through: -1, // no wait，当>0时，穿越到方向1-n
    through_type: "无", // 穿越方式：无，分隔贯通，隔离桩，斑马线
  },

  road_sign: {
    enter: [],
    exit: [],
  },
};

export const Draw = {
  // 绘制相关的属性
  dir: {
    // 方向
    angle: 0,
    radian: 0,
  },
  ratio: 4, // m到canvas的系数

  origin: { x: states.cx, y: states.cy }, // 原点，length和cross_len都是基于原点的
  base_line: { x1: states.cx, y1: states.cy, x2: 0, y2: 0 }, // 基线
  length: 300,

  enter_side: {
    // 外边形状：带margin
    walk1: { x: 0, y: 0 },
    walk2: { x: 0, y: 0 },
    ext1: { x: 0, y: 0 },
    ext2: { x: 0, y: 0 },
    far: { x: 0, y: 0 },
  },
  enter_side2: {
    // 不带margin
    walk1: { x: 0, y: 0 },
    walk2: { x: 0, y: 0 },
    ext1: { x: 0, y: 0 },
    ext2: { x: 0, y: 0 },
    far: { x: 0, y: 0 },
  },
  enter_bike_div: {
    // 入口非机动车道分隔外边
    walk1: { x: 0, y: 0 },
    walk2: { x: 0, y: 0 },
    ext1: { x: 0, y: 0 },
    ext2: { x: 0, y: 0 },
    far: { x: 0, y: 0 },
  },
  enter_bike_div2: {
    // 入口非机动车道分隔内边
    walk1: { x: 0, y: 0 },
    walk2: { x: 0, y: 0 },
    ext1: { x: 0, y: 0 },
    ext2: { x: 0, y: 0 },
    far: { x: 0, y: 0 },
  },

  exit_side: {
    // 外边形状：带margin
    walk1: { x: 0, y: 0 },
    walk2: { x: 0, y: 0 },
    ext1: { x: 0, y: 0 },
    ext2: { x: 0, y: 0 },
    far: { x: 0, y: 0 },
  },
  exit_side2: {
    // 不带margin
    walk1: { x: 0, y: 0 },
    walk2: { x: 0, y: 0 },
    ext1: { x: 0, y: 0 },
    ext2: { x: 0, y: 0 },
    far: { x: 0, y: 0 },
  },
  exit_bike_div: {
    // 出口非机动车道分隔外边
    walk1: { x: 0, y: 0 },
    walk2: { x: 0, y: 0 },
    ext1: { x: 0, y: 0 },
    ext2: { x: 0, y: 0 },
    far: { x: 0, y: 0 },
  },
  exit_bike_div2: {
    // 出口非机动车道分隔内边
    walk1: { x: 0, y: 0 },
    walk2: { x: 0, y: 0 },
    ext1: { x: 0, y: 0 },
    ext2: { x: 0, y: 0 },
    far: { x: 0, y: 0 },
  },

  txt: {
    pos: { x: 0, y: 0 },
    color: "#000",
  },

  road_sign: {
    enter: [],
    exit: [],
  },
};

//根据车道获取默认路标 k系数，>=1正向路，否则反向路
export function getRoadDefaultSign(
  wayIndex: number,
  is_reverse: boolean,
  is_last: boolean
): { key: string; path: string } {
  let roadSignKey = "";
  if (is_reverse) {
    //反向车道
    roadSignKey = "reverse_straight";
  } else if (is_last) {
    //右边最后一条路
    roadSignKey = "straight_right";
  } else {
    switch (wayIndex) {
      //右边第一条路
      case 0:
        roadSignKey = "left";
        break;
      //右边其余路
      default:
        roadSignKey = "straight";
    }
  }
  let roadSignPath = roadSigns.find((s) => s.key === roadSignKey)?.path;
  if (roadSignPath) {
    return { key: roadSignKey, path: roadSignPath };
  }
  return { key: "", path: "" };
}

export function getDirectionIndex(direction: string) {
  return Number(direction.replace("road_", "")) - 1;
}

/**
 *设置隔离带样式
 * @param path 路径对象
 * @param div_type 分割类型
 * @param width 可设宽度的路口宽度
 */
export function setIsolationStyle(
  path: any,
  div_type: string,
  width: number = 2
) {
  if (
    div_type === "双黄线" ||
    div_type === "单黄线" ||
    div_type === "鱼肚线" ||
    div_type === "黄斜线" ||
    div_type === "护栏"
  ) {
    path.setAttribute("stroke", "rgb(255,165,0)");
  } else if (div_type === "划线") {
    path.setAttribute("stroke", "rgb(255,255,255)");
  } else if (div_type === "绿化带") {
    path.setAttribute("stroke", "rgb(50,205,50)");
    path.setAttribute("stroke-width", width.toString());
  } else {
    path.setAttribute("stroke", "rgb(255,255,255)");
  }
}

//获取cross向后偏移距离
export function getCrossLenByTwoRoad(road_info: any, index: number) {
  //和下一条路相比需要后移的数量
  const is_last = index === plans.road_count - 1;
  const next_index = is_last ? 0 : index + 1;
  let angle1 =
    Number(plans.road_attr[next_index].angle) -
    Number(plans.road_attr[index].angle);
  angle1 = is_last ? angle1 + 360 : angle1;

  //和上一条路相比需要后移的数量
  const is_first = index === 0;
  const prev_index = is_first ? plans.road_count - 1 : index - 1;
  let angle2 =
    Number(plans.road_attr[index].angle) -
    Number(plans.road_attr[prev_index].angle);
  angle2 = is_first ? angle2 + 360 : angle2;

  //取夹角更小的
  const angle = angle1 < angle2 ? angle1 : angle2;

  //路宽*数量作为系数依据
  const enter1 = road_info.canalize_info[index].enter;
  const exit1 = road_info.canalize_info[next_index].exit;
  const c1 = enter1.num * enter1.lane_width + exit1.num * exit1.lane_width;

  const enter2 = road_info.canalize_info[prev_index].enter;
  const exit2 = road_info.canalize_info[index].exit;
  const c2 = enter2.num * enter2.lane_width + exit2.num * exit2.lane_width;

  const c = c1 > c2 ? c1 : c2;
  //系数计算
  const k = 0.04 * c;

  let len = angle >= 60 ? (k > 0.9 ? 0.4 * c : 0) : (70 - angle) * k;
  return len;
}

//通过渠化方式获取cross向后偏移距离
export function getCrossLenByCanalize(road_info: any, index: number) {
  let len = 0;
  const rc = road_info.canalize_info[index];

  if (rc.canalize.type === "划线渠化") {
    const right_exit_count = Number(rc.canalize.right_exit_count);
    len = (2 - right_exit_count) * 4;
  } else if (rc.canalize.type === "固体渠化") {
    const right_exit_count = Number(rc.canalize.right_exit_count);
    len = (2 - right_exit_count) * 4;
  }
  return len;
}

//通过渠化方式获取cross向后偏移距离
export function getCrossLenByPrevCanalize(road_info: any, index: number) {
  let len = 0;
  let i = index - 1 < 0 ? plans.road_count - 1 : index - 1;
  const rc = road_info.canalize_info[i];

  if (rc.canalize.type === "划线渠化") {
    const right_enter_count = Number(rc.canalize.right_enter_count);
    len = (2 - right_enter_count) * 3.5;
  } else if (rc.canalize.type === "固体渠化") {
    const right_exit_count = Number(rc.canalize.right_exit_count);
    len = (2 - right_exit_count) * 4;
  }
  return len;
}

//固体渠化后移的点(入口)
export const getEnterPointByCanalize_GT = (
  dw: any,
  rc: any,
  is_out_side: boolean
) => {
  var dr = Math.PI * 0.5;
  var k = 6;
  var d = (rc.cross_len_new + rc.canalize.lane_width + k) * dw.ratio;
  var len =
    (rc.enter.num * rc.enter.lane_width +
      rc.enter.bike_lane.has * rc.enter.bike_lane.width +
      rc.median_strip.width +
      rc.margin * (is_out_side ? 1 : 0)) *
    dw.ratio;
  return cal_point(dw, d, dr, len);
};

//固体渠化后移的点(出口)
export const getExitPointByCanalize_GT = (
  dw: any,
  rc: any,
  width: number,
  is_out_side: boolean
) => {
  var dr = Math.PI * 0.5;
  var k = 6;
  var d = (rc.cross_len_new + width + k) * dw.ratio;
  var len =
    (rc.exit.num * rc.exit.lane_width +
      (rc.exit.bike_lane.has ? rc.exit.bike_lane.width : 0) +
      rc.median_strip.width +
      rc.margin * (is_out_side ? 1 : 0)) *
    dw.ratio;
  return cal_point(dw, d, -dr, len);
};
