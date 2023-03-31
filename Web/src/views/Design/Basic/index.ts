import { parseAngle } from "../../../utils/common";

export const columns = [
  {
    title: "编号",
    dataIndex: "index",
    slots: { customRender: "index" },
    width: 30,
  },
  {
    title: "定位",
    dataIndex: "position",
    width: 50,
  },
  {
    title: "角度",
    dataIndex: "angle",
    width: 20,
  },
];

/**
 *  判断一个点是否在圆的内部
 *  @param point  测试点坐标
 *  @param circle 圆心坐标
 *  @param r 圆半径
 *  返回true为真，false为假
 *  */
export function isPointInCircle(
  point: number[],
  circle: number[],
  radius: number
) {
  if (radius === 0) return false;
  const dx = circle[0] - point[0];
  const dy = circle[1] - point[1];
  return dx * dx + dy * dy <= radius * radius;
}

/**
 *  根据圆心，半径，任意一点，获取任意一点在圆上的坐标
 *  @param cx  圆心坐标x
 *  @param cy  圆心坐标y
 *  @param radius  半径（线段长度）
 *  @param x  测试点坐标x
 *  @param y  测试点坐标y
 *  返回该点在圆上的坐标
 *  */
export function getCoordinate(
  cx: number,
  cy: number,
  radius: number,
  x: number,
  y: number
) {
  const dx = x - cx;
  const dy = y - cy;
  const radian = Math.atan2(dy, dx);
  const x1 = cx + Math.cos(radian) * radius;
  const y1 = cy + Math.sin(radian) * radius;
  return [parseAngle(x1), parseAngle(y1)];
}

export function setLineXY(
  line: Element | null,
  x1: number,
  y1: number,
  x2: number,
  y2: number,
  stroke = "#4f48ad"
) {
  if (!line) {
    return;
  }
  line.setAttribute("x1", x1.toString());
  line.setAttribute("y1", y1.toString());
  line.setAttribute("x2", x2.toString());
  line.setAttribute("y2", y2.toString());
  line.setAttribute("stroke", stroke);
  line.setAttribute("deleteTag", "1");
}
