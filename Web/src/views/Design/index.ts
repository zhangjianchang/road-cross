import _ from "lodash";
import { reactive } from "vue";
import { getQuadrantByAngle } from "../../utils/common";
import { getRoadDefaultSign, RoadCross } from "./Canalize";
import { DirectionsZh, plans_model } from "./data";
import {
  flowColumnsPart,
  flowDataIndex,
  lineInfoModel,
  uturn_path,
} from "./Flow";
import { DirectionItemModel, phaseModel } from "./Signal";

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
  { index: 8, url: "DelayAnalysis", name: "延误时间" },
  { index: 7, url: "QueueAnalysis", name: "排队长度" },
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

//道路信息
export const roadStates = reactive({
  cx: 350, //圆心x
  cy: 350, //圆心y
  road_list: [] as any[], //TODO缓存方案，后期删
  currentUrl: "Basic",
  is_flow_init: false, //是否已经初始化过流量信息
  showSelect: {
    showCanalize: false,
    showFlow: false,
    showSignal: false,
  },
  current_canalize: 0, //当前渠化方案
  current_flow: 0, //当前流量方案
  current_signal: 0, //当前信号方案
});

//所有方案
export const plans = reactive(plans_model);

/**公用部分 */
//四个基本方向
export const basicDirection = ["uturn", "left", "straight", "right"];

//获取车道转向名称
export const getDirectionZhName = (i: number, road_key: string) => {
  return "方向" + (i + 1) + DirectionsZh[road_key];
};

//根据程度取颜色(饱和度)
export function getBackground(number: number | string) {
  number = Number(number);
  if (number <= 0.1) {
    return "rgb(0,255,0)";
  } else if (number <= 0.2) {
    return "rgb(100,255,0)";
  } else if (number <= 0.3) {
    return "rgb(150,255,0)";
  } else if (number <= 0.4) {
    return "rgb(200,255,0)";
  } else if (number <= 0.5) {
    return "rgb(255,255,0)";
  } else if (number <= 0.6) {
    return "rgb(255,200,0)";
  } else if (number <= 0.7) {
    return "rgb(255,150,0)";
  } else if (number <= 0.8) {
    return "rgb(255,100,0)";
  } else if (number <= 0.9) {
    return "rgb(255,50,0)";
  } else {
    return "rgb(255,0,0)";
  }
}

export function getBackgroundByDelay(number: number | string) {
  number = Number(number);
  if (number <= 10) {
    return "#7cfc00";
  } else if (number <= 20) {
    return "#008000";
  } else if (number <= 35) {
    return "#2ad100";
  } else if (number <= 55) {
    return "#f8f40f";
  } else if (number <= 80) {
    return "#ffa500";
  } else {
    return "#ff0000";
  }
}
/**公用部分 */

/**渠化相关 */
//初始化渠化信息
export function create_road_cross(road_info: any) {
  var dir = road_info.road_attr.map((r: { angle: any }) => r.angle);
  road_info.canalize_info.length = 0;
  var temp = JSON.stringify(RoadCross); // 数据结构模板
  for (var i = 0; i < dir.length; i++) {
    var rc = JSON.parse(temp); // road cross对象
    var angle = parseFloat(dir[i]); // 角度
    rc.angle = angle;
    rc.name = "方向" + (i + 1);
    create_sign(rc);
    road_info.canalize_info.push(rc);
  }
}

//加载渠化信息
export function update_road_corss(road_info: any) {
  var dir = road_info.road_attr.map((r: { angle: any }) => r.angle);
  const canalize_info_list = [];
  for (var i = 0; i < dir.length; i++) {
    let rc = {} as any;
    if (i < road_info.canalize_info.length) {
      rc = road_info.canalize_info[i]; //存在则取原有数据
    } else {
      rc = JSON.parse(JSON.stringify(RoadCross)); // 不存在则取模板对象
      create_sign(rc);
    }
    var angle = parseFloat(dir[i]); // 角度
    rc.angle = angle;
    canalize_info_list.push(rc);
  }
  road_info.canalize_info.length = 0;
  Object.assign(road_info.canalize_info, canalize_info_list);
}

//创建路标
export function create_sign(rc: any) {
  rc.road_sign.enter.length = 0;
  rc.road_sign.exit.length = 0;
  //左边路标
  for (var j = 0; j < rc.enter.num; j++) {
    const rs = getRoadDefaultSign(j, false, j === rc.enter.num - 1);
    rc.road_sign.enter.push(rs);
  }
  //左边路标
  for (var j = 0; j < rc.exit.num; j++) {
    const rs = getRoadDefaultSign(j, true, false);
    rc.road_sign.exit.push(rs);
  }
}
/**渠化相关 */

/**流量相关 */
export function create_flow_detail(road_info: any) {
  initFlowDetail(road_info);
  initEnterNum(road_info);
}

//加载流量相关
export function initFlowDetail(road_info: any) {
  road_info.flow_info.flowColumns.length = 0;
  Object.assign(road_info.flow_info.flowColumns, flowColumnsPart);
  for (let i = 0; i < road_info.road_attr.length; i++) {
    let dataIndex = i.toString();
    road_info.flow_info.flowColumns.push({
      title: "转向" + (i + 1),
      dataIndex: dataIndex,
      width: 30,
      slots: { customRender: dataIndex },
      align: "center",
    });
    flowDataIndex.push(dataIndex);
  }
  //先清空
  road_info.flow_info.line_info.length = 0;
  road_info.flow_info.flow_detail.length = 0;
  const roadCount = road_info.road_attr.length;
  for (let i = 0; i < roadCount; i++) {
    let line_info = _.cloneDeep(lineInfoModel);
    line_info.direction = "方向" + (i + 1);
    line_info.road_name = road_info.canalize_info[i].name;
    road_info.flow_info.line_info.push(line_info);

    let flow_detail = {} as any;
    flow_detail.road_name = road_info.canalize_info[i].name;
    const turn = [] as any[];
    for (let j = 0; j < roadCount; j++) {
      //转向属性
      const turn_detail = getTurnDetail(road_info, i, j);
      turn.push(turn_detail);
    }
    flow_detail.turn = turn;
    //排序
    flow_detail.turn.sort(function (a: any, b: any) {
      return b.order - a.order;
    });
    flow_detail.turn.map((item: any, index: number) => {
      //TODO 暂按照四条路来设计
      if (index === 0) {
        item.direction = "uturn";
      } else if (index === 1) {
        item.direction = "left";
      } else if (index === 2) {
        item.direction = "straight";
      } else if (index === 3) {
        item.direction = "right";
      }
    });
    road_info.flow_info.flow_detail.push(flow_detail);
  }
}

//加载饱和流量
export function initEnterNum(road_info: any) {
  road_info.flow_info.saturation.length = 0;
  road_info.canalize_info.forEach((item: any) => {
    const currentSaturation = [];
    for (let i = 0; i < item.enter.num; i++) {
      currentSaturation.push({ number: 1650 });
    }
    road_info.flow_info.saturation.push(currentSaturation);
  });
}

//加载各道路之间的方向
const getTurnDetail = (road_info: any, i: number, j: number) => {
  const roadCount = road_info.road_attr.length;
  const road = road_info.road_attr[i];
  const nextRoad = road_info.road_attr[j];
  const translate = getUturnTranslate(road_info, i);
  //转向属性
  const turn_detail = {
    number: i === j ? 0 : 450,
    d:
      i === j
        ? uturn_path
        : `M${road.coordinate[0]} ${road.coordinate[1]} Q${roadStates.cx} ${roadStates.cy} ${nextRoad.coordinate[0]} ${nextRoad.coordinate[1]}`,
    tag: `${i}#${j}`, //标记从哪个车道到哪个车道
    order: j - i <= 0 ? j - i + roadCount : j - i, //排序（为了把掉头车道放在第一个）
    translate,
  } as any;
  return turn_detail;
};

//掉头车道平移计算
const getUturnTranslate = (road_info: any, i: number) => {
  const angle = road_info.road_attr[i].angle;
  const quadrant = getQuadrantByAngle(angle);
  if (quadrant === 0) {
    return `${-(roadStates.cx + 270 - angle)},${roadStates.cy - angle}`;
  } else if (quadrant === 1) {
    return `${-(roadStates.cx + 270 - angle)},${-(roadStates.cy - angle)}`;
  } else if (quadrant === 2) {
    return `${roadStates.cx - (270 - angle)},${-(270 - angle + roadStates.cy)}`;
  } else if (quadrant === 3) {
    return `${roadStates.cx - (270 - angle)},${270 - angle - roadStates.cy}`;
  } else if (quadrant === 4) {
    return `${-(roadStates.cx - angle)},${roadStates.cy}`;
  }
};
/**流量相关 */

/**信号相关 */
export function create_signal_info(road_info: any) {
  for (let p = 0; p < road_info.signal_info.phase; p++) {
    insert_phase(road_info, p);
  }
}

export function insert_phase(road_info: any, p: number) {
  //数据
  let roadCount = road_info.road_attr.length;
  let phaseItem = _.cloneDeep(phaseModel);
  phaseItem.index = p;
  phaseItem.name = `第${p + 1}相位`;
  for (let d1 = 0; d1 < roadCount; d1++) {
    let directions = [];
    for (let d2 = 0; d2 < roadCount; d2++) {
      let directionItem = _.cloneDeep(DirectionItemModel);
      directionItem.direction = getDirection(d1, d2, roadCount);
      directionItem.order = getDirectionOerder(directionItem.direction);
      directions.push(directionItem);
    }
    //排序
    directions.sort(function (a: any, b: any) {
      return a.order - b.order;
    });
    phaseItem.directions.push(directions);
  }
  road_info.signal_info.phase_list.push(phaseItem);
  road_info.signal_info.period +=
    phaseItem.green + phaseItem.yellow + phaseItem.red;
}

export const getDirection = (i: number, j: number, roadCount: number) => {
  if (i === j) {
    return "uturn";
  } else if (i - j === 1 || i - j + roadCount === 1) {
    return "left";
  } else if (j - i === 1 || j - i + roadCount === 1) {
    return "right";
  }
  return "straight";
};

export const getDirectionOerder = (direction: string) => {
  switch (direction) {
    case "uturn":
      return 0;
    case "right":
      return 1;
    case "straight":
      return 2;
    case "left":
      return 3;
    default:
      return 2;
  }
};

/**信号相关 */

/**计算 */
//绿信比
export function get_λ(green: number, yellow: number, period: number) {
  const λ = (green + yellow - 3) / period;
  return λ < 0 ? 0 : λ;
}

//绘制之前合并同时并行道路
export const mergeWays = (rc: any) => {
  //绘制之前先确定合并
  let all_keys = rc.road_sign.enter.map((rs: any) => rs.key); //全部方向
  const multiple_way_keys = rc.road_sign.enter
    .filter((rs: any) => basicDirection.indexOf(rs.key) === -1)
    .map((rs: any) => rs.key); //两种及以上转向的方向

  //剔除多方向已包含的方向
  multiple_way_keys.map((mk: any) => {
    const mks = mk.split("_");
    mks.forEach((s: any) => {
      let index = all_keys.indexOf(s);
      if (index > -1) {
        all_keys = all_keys.filter((k: any) => k != s);
      }
    });
  });

  //针对多方向再次合并
  const sl_index = all_keys.indexOf("straight_left");
  let sr_index = all_keys.indexOf("straight_right");
  if (sl_index > -1 && sr_index > -1) {
    all_keys.splice(sl_index, 1);
    sr_index = all_keys.indexOf("straight_right");
    all_keys.splice(sr_index, 1);
    all_keys.push("left_straight_right");
  }

  //掉头类型合并
  mergeReturnWays(rc);

  return all_keys;
};
//TODO
export const mergeReturnWays = (rc: any) => {};
