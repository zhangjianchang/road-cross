//默认相位
export const phaseModel = {
  name: "",
  green: 17,
  yellow: 3,
  red: 0,
  is_lap: false,
};
//渐变属性
export const signalColor = {
  green: ["#00CC00", "#00FF99"],
  yellow: ["#CCCC00", "#CCFF99"],
  red: ["#C00000", "#FF0000"],
};
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
