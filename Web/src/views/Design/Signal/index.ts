//默认相位
export const phaseModel = {
  index: 0,
  name: "",
  green: 17,
  yellow: 3,
  red: 0,
  is_lap: 0,
  in_direction: 0,
  directions: [] as any[],
};

export const DirectionItemModel = {
  is_enable: false, //机动车是否启用，渲染线段
  is_non_enable: false, //非机动车是否启用，渲染线段
  is_ped_enable: false, //行人是否启用，渲染线段
  direction: "", //方向
  order: 0, //针对方向重新排序
  green: 0,
  yellow: 0,
  red: 0,
};
//渐变属性
export const signalColor = {
  green: ["#00CC00", "#00FF99"],
  yellow: ["#CCCC00", "#CCFF99"],
  red: ["#C00000", "#FF0000"],
};
//相位表格
export const phaseColumns = [
  {
    title: "相位",
    dataIndex: "index",
    width: 40,
    slots: { customRender: "index" },
  },
  {
    title: "名称",
    dataIndex: "name",
    width: 80,
    slots: { customRender: "name" },
  },
  {
    title: "绿灯",
    dataIndex: "green",
    width: 60,
    slots: { customRender: "green" },
  },
  {
    title: "黄灯",
    dataIndex: "yellow",
    width: 60,
    slots: { customRender: "yellow" },
  },
  {
    title: "全红",
    dataIndex: "red",
    width: 60,
    slots: { customRender: "red" },
  },
  {
    title: "搭接相位",
    dataIndex: "is_lap",
    width: 60,
    slots: { customRender: "is_lap" },
  },
];
//获取起始x位置，signal，r红灯g绿灯y黄灯
export function getStartX(signalList: any[], index: number, signal: string) {
  var startX = 0;
  for (let i = 0; i < index; i++) {
    startX += signalList[i].green + signalList[i].yellow + signalList[i].red;
  }
  switch (signal) {
    case "g":
      return startX;
    case "y":
      return startX + signalList[index].green;
    case "r":
      return startX + signalList[index].green + signalList[index].yellow;
    default:
      return startX;
  }
}

export const getArrowId = (id: string) => {
  switch (id) {
    case "direction":
      return "arrow";
    case "non_direction":
      return "non_arrow";
    case "ped_direction":
      return "ped_arrow";
    default:
      return "";
  }
};

export const getColor = (id: string) => {
  switch (id) {
    case "direction_line":
      return "#ffffff";
    case "non_direction_line":
      return "#ffb90f";
    case "ped_direction_line":
      return "#00ff99";
    default:
      return "#ffffff";
  }
};
