import _ from "lodash";
import { reactive } from "vue";
import { useCode } from "../../request/api";
import { cal_point, insect_pt } from "../../utils/common";
import { userStates } from "../UserCenter";
import { getRoadDefaultSign, RoadCross } from "./Canalize";
import { DirectionsZh, plans_model } from "./data";
import { flowColumnsPart, flowDataIndex, lineInfoModel } from "./Flow";
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

//道路信息
export const roadStates = reactive({
  loading: false,
  from_map: false,
  cx: 350, //圆心x
  cy: 350, //圆心y
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
    return "rgb(255,215,0)";
  } else if (number <= 0.6) {
    return "rgb(255,190,0)";
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
  if (number <= 10) return "#008000";
  else if (number <= 20) return "#2ad100";
  else if (number <= 35) return "#7cfc00";
  else if (number <= 55) return "#ffd700";
  else if (number <= 80) return "#ffa500";
  else return "#ff0000";
}

export function getBackgroundBySaturation(number: number | string) {
  number = Number(number);
  if (number <= 0.25) return "#008000";
  else if (number <= 0.5) return "#2ad100";
  else if (number <= 0.7) return "#7cfc00";
  else if (number <= 0.85) return "#ffd700";
  else if (number <= 0.95) return "#ffa500";
  else return "#ff0000";
}
/**公用部分 */

/**渠化相关 */
//初始化渠化信息
export function create_road_cross(road_info: any) {
  var dir = plans.road_attr.map((r: { angle: any }) => r.angle);
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
  var dir = plans.road_attr.map((r: { angle: any }) => r.angle);
  const canalize_info_list = [];
  for (var i = 0; i < dir.length; i++) {
    let rc = {} as any;
    if (i < road_info.canalize_info.length) {
      rc = road_info.canalize_info[i]; //存在则取原有数据
    } else {
      rc = JSON.parse(JSON.stringify(RoadCross)); // 不存在则取模板对象
      rc.name = "方向" + (i + 1);
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
  initFlowColumns(road_info); //表头
  initFlowDetail(road_info); //数据
  initEnterNum(road_info); //饱和流量
}

//加载流量信息
export function update_flow_detail(road_info: any) {
  initFlowColumns(road_info); //表头
  updateFlowDetail(road_info); //数据
  updateEnterNum(road_info); //饱和流量
}

//加载表头
function initFlowColumns(road_info: any) {
  road_info.flow_info.flowColumns.length = 0;
  Object.assign(road_info.flow_info.flowColumns, flowColumnsPart);
  for (let i = 0; i < plans.road_count; i++) {
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
}

//加载流量相关
function initFlowDetail(road_info: any) {
  //先清空
  road_info.flow_info.line_info.length = 0;
  road_info.flow_info.flow_detail.length = 0;
  const roadCount = plans.road_count;
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
function initEnterNum(road_info: any) {
  road_info.flow_info.saturation.length = 0;
  road_info.canalize_info.forEach((item: any) => {
    const currentSaturation = [];
    for (let i = 0; i < item.enter.num; i++) {
      currentSaturation.push({ number: 1650 });
    }
    road_info.flow_info.saturation.push(currentSaturation);
  });
}

//更新各方向之间的角度
function updateFlowDetail(road_info: any) {
  road_info.flow_info.flow_detail.map((fd: any) => {
    fd.turn.map((t: any) => {
      const ij = t.tag.split("#");
      const i = Number(ij[0]);
      const j = Number(ij[1]);
      t.d = getTurnDetail_D(road_info, i, j);
    });
  });
}
//更新进口车道饱和流量
function updateEnterNum(road_info: any) {
  road_info.flow_info.saturation.map((fs: any, index: number) => {
    const enter_num = road_info.canalize_info[index].enter.num;
    const fs_count = fs.length;
    if (enter_num > fs_count) {
      //新增了
      for (let i = 0; i < enter_num - fs_count; i++) {
        fs.push({ number: 1650 });
      }
    } else if (enter_num < fs_count) {
      //减少了
      fs.splice(0, fs_count - enter_num);
    }
  });
}

//加载各道路之间的方向
const getTurnDetail = (road_info: any, i: number, j: number) => {
  const roadCount = plans.road_count;
  const d = getTurnDetail_D(road_info, i, j);
  //转向属性
  const turn_detail = {
    number: i === j ? 0 : 450,
    d,
    tag: `${i}#${j}`, //标记从哪个车道到哪个车道
    order: j - i <= 0 ? j - i + roadCount : j - i, //排序（为了把掉头车道放在第一个）
  } as any;
  return turn_detail;
};

//机动车/非机动车穿越图标
export const getTurnDetail_D = (road_info: any, i: number, next_i: number) => {
  const states = {
    d: 120, //离圆心距离
    far_d: 240, //离圆心距离
    len: 80, //路宽
    road_count: plans.road_count, //路口数量
  };
  let k = 1;
  const dr = Math.PI * 0.5;
  //掉头
  if (i === next_i) {
    states.d = -40;
    states.len = 120;
    k = -k; //反向系数
  }
  //对面(目前只有4条路的时候会有这种情况)
  if (Math.abs(i - next_i) === states.road_count / 2) {
    states.len = 0;
  }
  //相邻
  const j = next_i === states.road_count - 1 ? -1 : next_i;
  if (i - j === 1) {
    k = -k; //反向系数
  }

  //第一条路
  let dw = getDW(road_info, i);
  let d = states.far_d;
  let pt_r11 = cal_point(dw, d, -dr * k, states.len);

  d = states.d;
  let pt_r12 = cal_point(dw, d, -dr * k, states.len);

  //第二条路
  dw = getDW(road_info, next_i);
  d = states.far_d;
  let pt_l11 = cal_point(dw, d, dr * k, states.len);

  d = states.d;
  let pt_l12 = cal_point(dw, d, dr * k, states.len);

  //连接两条路
  let Q = insect_pt(
    { point1: pt_r11, point2: pt_r12 },
    { point1: pt_l11, point2: pt_l12 }
  );

  let d_str = "";
  //连接第一条路左侧，Q点，第二条路左侧
  if (Q && i !== next_i) {
    d_str = `M${pt_r11.x},${pt_r11.y} L${pt_r12.x},${pt_r12.y} Q${Q.x},${Q.y} ${pt_l12.x},${pt_l12.y} L${pt_l11.x},${pt_l11.y}`;
  } else {
    let pt_c = cal_point(dw, d - 200, dr, 0);
    d_str = `M${pt_r11.x},${pt_r11.y} L${pt_r12.x},${pt_r12.y} Q${pt_c.x},${pt_c.y} ${pt_l12.x},${pt_l12.y} L${pt_l11.x},${pt_l11.y}`;
  }
  return d_str;
};

//行人穿越图标，j=1中间=>右边，j=2中间=>左边
export const getPedDetail_D = (road_info: any, i: number, j: number) => {
  const states = {
    d: 40, //离圆心距离
    far_d: 200, //离圆心距离
    far_d2: -200, //离圆心距离
  };
  const k = j === 1 ? 1 : -1;
  let d_str = "";
  const dr = Math.PI * 0.5;
  const dw = getDW(road_info, i);

  //竖线
  let d = states.far_d;
  let pt_s1 = cal_point(dw, d, k * dr, 60);
  d = states.far_d2;
  let pt_s2 = cal_point(dw, d, k * dr, 60);
  d_str += `M${pt_s1.x},${pt_s1.y} L${pt_s2.x},${pt_s2.y};`;

  //横线
  d = states.d;
  let pt_1 = cal_point(dw, d, -k * dr, 200);
  let pt_2 = cal_point(dw, d, k * dr, 0);
  d_str += `M${pt_1.x},${pt_1.y} L${pt_2.x},${pt_2.y}`;

  return d_str;
};

const getDW = (road_info: any, i: number) => {
  const angle = plans.road_attr[i].angle;
  const radian = (Math.PI / 180) * angle; // 角度转弧度
  const dw = {
    dir: { radian },
    origin: { x: 350 }, //圆心x
  };
  return dw;
};
/**流量相关 */

/**信号相关 */
export function create_signal_info(road_info: any) {
  road_info.signal_info.period = 0;
  for (let p = 0; p < road_info.signal_info.phase; p++) {
    insert_phase(road_info, p);
  }
}

export function insert_phase(road_info: any, p: number) {
  //数据
  let roadCount = plans.road_count;
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

export const getStraightTurnDetail = (road_info: any, i: number) => {
  const states = {
    d: 0, //离圆心距离
    len: 200, //路宽
    road_count: plans.road_count, //路口数量
  };
  const dr = Math.PI * 0.5;
  const dw = getDW(road_info, i);
  const d = states.d;
  let pt_1 = cal_point(dw, d, dr, states.len); //左边
  let pt_2 = cal_point(dw, d, -dr, states.len); //右边

  let d_str = `M${pt_1.x},${pt_1.y} L${pt_2.x},${pt_2.y}`;
  return d_str;
};
/**信号相关 */

/**计算 */
export const get_green_yellow = (
  rf: any,
  roadIndex: number,
  roadKey: string
) => {
  let greens = 0;
  let yellows = 0;
  rf.signal_info.phase_list.forEach((p: any, index: number) => {
    let green = 0;
    let yellow = 0;
    p.directions[roadIndex].forEach((d: any) => {
      if (roadKey.indexOf(d.direction) > -1) {
        green = d.green;
        yellow = yellow | d.yellow;
      }
    });
    //多相位相加
    greens += green;
    yellows = yellows | yellow;

    //确认搭接并且与上一方向有共同搭接转向
    if (p.is_lap) {
      let is_lap_s = false;
      p.lap.map((lp: any) => {
        if (roadIndex === lp.roadIndex && roadKey.indexOf(lp.roadKey) > -1) {
          is_lap_s = true;
        }
      });
      if (is_lap_s) {
        //如果搭接相位，需加上上一相位的黄灯和红灯
        const prev_p = rf.signal_info.phase_list[index - 1];
        greens += prev_p.yellow + prev_p.red;
      }
    }
  });
  return { green: greens, yellow: yellows };
};

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

/*********************************操作授权码*********************/
export const handleEdit = () => {
  useCode({ code: userStates.code_info.code }).then((res: any) => {
    userStates.code_info = res.data;
    userStates.can_edit = true;
  });
};
/*********************************操作授权码*********************/
