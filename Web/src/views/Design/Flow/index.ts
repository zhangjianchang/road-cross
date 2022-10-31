export const lineColumns = [
  {
    title: "方向",
    dataIndex: "direction",
    width: 40,
  },
  {
    title: "路名",
    dataIndex: "road_name",
    width: 40,
    slots: { customRender: "road_name" },
  },
  {
    title: "大车比率(%)",
    dataIndex: "truck_ratio",
    width: 40,
    slots: { customRender: "truck_ratio" },
  },
  {
    title: "高峰小时系数",
    dataIndex: "peak_period",
    width: 60,
    slots: { customRender: "peak_period" },
  },
];

export const flowColumnsPart = [
  {
    title: "路名",
    dataIndex: "road_name",
    width: 30,
  },
  {
    title: "转向1",
    dataIndex: "turn1",
    width: 30,
    slots: { customRender: "turn1" },
  },
];

export const lineInfoModel = {
  direction: "",
  road_name: "", //路名
  truck_ratio: 2, //大车比率
  peak_period: 0.95, //高峰小时系数
};

export const flowDataIndex = ["turn1"];

//求曲率
export const getCurvByAngle = (
  formCurvature: number,
  angle1: number,
  angle2: number,
  point1: number[],
  point2: number[]
) => {
  //求起始点旋转角度
  let curvature = formCurvature * (180 - Math.abs(angle2 - angle1));
  //根据距离修正曲率，距离越短曲率降低到2
  const dep = getPointsDistance(point1, point2);
  if (dep < 200 && Math.abs(curvature) > 2.4) {
    curvature = curvature > 0 ? 2.4 : -2.4;
  } else if (dep > 300) {
    curvature *= 1.2;
  }
  return curvature;
};

//求两点之间的距离
function getPointsDistance(point1: number[], point2: number[]) {
  const dx = Math.abs(point2[0] - point1[0]);
  const dy = Math.abs(point2[1] - point1[1]);
  const distance = Math.sqrt(Math.pow(dx, 2) + Math.pow(dy, 2));
  return distance;
}
