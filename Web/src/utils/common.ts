import router from "../router";

//param自定义参数，openType打开类型 _blank/_self
export function goRouterByParam(
  name: string,
  param: any = {},
  openType = "_self"
) {
  try {
    const { href } = router.resolve({
      name: name,
      params: param,
    });
    window.open(href, openType);
    //路由跳转后都强刷页面
    // location.reload();
  } catch {}
}

// 数组去重
export function unique(arr: any[]) {
  return Array.from(new Set(arr));
}

// 求角度
export function getAngle(cx: number, cy: number, x: number, y: number) {
  let quadrant = getQuadrantByPoint(x, y);
  // 第一象限
  if (quadrant === 1) {
    x = x - cx;
    y = cy - y;
  }
  // 第二象限
  else if (quadrant === 2) {
    let ox = x;
    x = cy - y;
    y = cx - ox;
  }
  //第三象限
  else if (quadrant === 3) {
    x = cx - x;
    y = y - cy;
  }
  //第四象限
  else {
    let ox = x;
    x = cy - y;
    y = cx - ox;
  }
  var radian = Math.atan(y / x); //弧度
  var angle = parseAngle(180 / (Math.PI / radian)); //弧度转角度
  //每增加一个象限加90度
  angle += (quadrant - 1) * 90;
  return angle;
}

//角度以5°为一个单位
export function parseAngle(angle: number) {
  return parseInt((angle / 5).toString()) * 5;
}

/* 求中垂线 */
export function getQByPathCurv(a: number[], b: number[], curv: any) {
  /*
   * 弯曲函数.
   * a:a点的坐标[10,10]
   * b:b点的坐标[10,10]
   * curv:弯曲程度 取值 -1 到 1
   */
  curv = isWithInVerticalLine(a, b, curv) ? curv : -curv;
  var k2, controX, controY;
  /*
   * 控制点必须在line的中垂线上
   * 求出k2的中垂线(中垂线公式)
   */
  k2 = b[1] - a[1] === 0 ? 0 : -(b[0] - a[0]) / (b[1] - a[1]);
  /*
   * 弯曲程度是根据中垂线斜率决定固定控制点的X坐标或者Y坐标,通过中垂线公式求出另一个坐标
   * 默认A/B中点的坐标+curv*20,可以通过改基数10改变传入的参数范围
   */
  if (b[1] - a[1] === 0) {
    const x = (a[0] + b[0]) / 2;
    const y = (a[1] + b[1]) / 2;
    let quadrant = getQuadrantByPoint(x, y);
    curv = quadrant === 1 || quadrant === 2 ? curv : -curv;
    controX = Math.abs((b[0] + a[0]) / 2);
    controY = Math.abs(curv * 35 + (a[1] + b[1]) / 2);
  } else if (k2 === 0) {
    controX = Math.abs((b[0] + a[0]) / 2 + curv * 35);
    controY = Math.abs(k2 * (controX - (a[0] + b[0]) / 2) + (a[1] + b[1]) / 2);
  } else if (k2 < 2 && k2 > -2) {
    controX = Math.abs((b[0] + a[0]) / 2 + curv * 20);
    controY = Math.abs(k2 * (controX - (a[0] + b[0]) / 2) + (a[1] + b[1]) / 2);
  } else {
    controY = Math.abs((b[1] + a[1]) / 2 + curv * 20);
    controX = Math.abs((controY - (a[1] + b[1]) / 2) / k2 + (a[0] + b[0]) / 2);
  }
  //定义控制点的位置
  var Q = `${~~controX},${~~controY}`;
  return Q;
}

/* 求中点 */
export function getMiddlePoint(a: number[], b: number[]): number[] {
  var k2, controX, controY;
  k2 = b[1] - a[1] === 0 ? 0 : -(b[0] - a[0]) / (b[1] - a[1]);
  controX = Math.abs((b[0] + a[0]) / 2);
  controY = Math.abs(k2 * (controX - (a[0] + b[0]) / 2) + (a[1] + b[1]) / 2);
  return [parseInt(controX.toString()), parseInt(controY.toString())];
}

//根据坐标获取象限
export function getQuadrantByPoint(x: number, y: number) {
  let quadrant = 0;
  if (x >= 350 && y <= 350) {
    quadrant = 1;
  }
  // 第二象限
  else if (x <= 350 && y <= 350) {
    quadrant = 2;
  }
  //第三象限
  else if (x <= 350 && y >= 350) {
    quadrant = 3;
  }
  //第四象限
  else {
    quadrant = 4;
  }
  return quadrant;
}

//根据角度获取象限
export function getQuadrantByAngle(angle: number) {
  if (angle === 0) {
    return 0;
  } else if (angle > 0 && angle <= 90) {
    return 1;
  }
  // 第二象限
  else if (angle >= 90 && angle < 180) {
    return 2;
  }
  //第三象限
  else if (angle >= 180 && angle < 270) {
    return 3;
  }
  //第四象限
  else {
    return 4;
  }
}

// 查看点是否在中垂线以内
function isWithInVerticalLine(a: number[], b: number[], curv: any) {
  let flag = true;
  const x = (a[0] + b[0]) / 2;
  const y = (a[1] + b[1]) / 2;
  var k2, controX, controY;
  k2 = -(b[0] - a[0]) / (b[1] - a[1]);
  if (k2 < 2 && k2 > -2) {
    controX = Math.abs((b[0] + a[0]) / 2 + curv * 20);
    controY = Math.abs(k2 * (controX - (a[0] + b[0]) / 2) + (a[1] + b[1]) / 2);
  } else {
    controY = Math.abs((b[1] + a[1]) / 2 + curv * 20);
    controX = Math.abs((controY - (a[1] + b[1]) / 2) / k2 + (a[0] + b[0]) / 2);
  }
  const quadrant = getQuadrantByPoint(x, y);
  if ((quadrant === 1 || quadrant === 4) && controX > x) {
    flag = false;
  }
  if ((quadrant === 2 || quadrant === 3) && controX < x) {
    flag = false;
  }
  return flag;
}

// 恢复canvas坐标系
function moveCoordinate(point = []) {
  const width = 350;
  const height = 350;
  return [point[0] + width / 2, -point[1] + height / 2];
}

// 切换至原点坐标系
function removeCoordinate(point: any) {
  const width = 350;
  const height = 350;
  return point.map((i: any) => [i[0] - width / 2, -i[1] + height / 2]);
}

// 翻转坐标系，type - 0：x轴对称，1：y轴对称，2，y=x对称，3，y=-x对称
function rotateCoordinate(point: any, type: any) {
  switch (type) {
    case 0:
      return [point[0], -point[1]];
    case 1:
      return [-point[0], point[1]];
    case 2:
      return [point[1], point[0]];
    case 3:
      return [-point[1], -point[0]];
    case 4:
      return [
        (Math.sqrt(2) / 2) * (point[1] - point[0]),
        (Math.sqrt(2) / 2) * (point[0] + point[1]),
      ];
  }
}

//画点
// function drawPoint(x: number, y: number, color: string) {
//   var g = document.createElementNS(states.ns, "g");
//   g.setAttribute("stroke", color);
//   g.setAttribute("stroke-width", "3");
//   g.setAttribute("fill", "black");

//   var circle = document.createElementNS(states.ns, "circle");
//   circle.setAttribute("cx", x.toString());
//   circle.setAttribute("cy", y.toString());
//   circle.setAttribute("r", "3");
//   g.appendChild(circle);

//   states.cvs?.appendChild(g);
// }

//找点 //direction：方向，nr 近右,nl 近左, fr 远右, fl 远左 //road_width默认100（小于100即画路边线时用到）
export function getPoint(
  direction: string,
  angle: number,
  x: number,
  y: number,
  road_width: number
) {
  // 路宽
  var radian = (Math.PI / 180) * (angle - 90);
  var coefficient = direction === "nl" || direction === "fr" ? -1 : 1;
  var point_x = coefficient * road_width * 0.5 * Math.cos(radian) + x;
  var point_y = -coefficient * road_width * 0.5 * Math.sin(radian) + y;
  return [parseInt(point_x.toString()), parseInt(point_y.toString())];
}

//随机颜色
export function getRandomColor() {
  return `rgb(${Math.random() * 256},${Math.random() * 256},${
    Math.random() * 256
  })`;
}

// 2直线交点
export function intersect_line_point(
  a: { x: number; y: number },
  b: { x: number; y: number },
  c: { x: number; y: number },
  d: { x: number; y: number }
) {
  // 解线性方程组, 求线段交点
  // 如果分母为0 则平行或共线, 不相交
  var denominator = (b.y - a.y) * (d.x - c.x) - (a.x - b.x) * (c.y - d.y);
  if (denominator == 0) {
    return false;
  }

  // 线段所在直线的交点坐标 (x , y)
  var x =
    ((b.x - a.x) * (d.x - c.x) * (c.y - a.y) +
      (b.y - a.y) * (d.x - c.x) * a.x -
      (d.y - c.y) * (b.x - a.x) * c.x) /
    denominator;
  var y =
    -(
      (b.y - a.y) * (d.y - c.y) * (c.x - a.x) +
      (b.x - a.x) * (d.y - c.y) * a.y -
      (d.x - c.x) * (b.y - a.y) * c.y
    ) / denominator;

  return {
    x: x,
    y: y,
  };
}

// 已知2点求共线第3点
export function line_pt3(
  pt1: { x: number; y: number },
  pt2: { x: number; y: number },
  len1: number,
  len2: number
) {
  var fct = len2 / len1;
  var x = fct * (pt2.x - pt1.x) + pt1.x;
  var y = fct * (pt2.y - pt1.y) + pt1.y;

  return {
    x: x,
    y: y,
  };
}

//根据两条直线(pt1,pt2)求之间的夹角
export function getAngleBetweenLines(
  pt11: { x: any; y: number },
  pt12: { x: any; y: number },
  pt21: { x: any; y: number },
  pt22: { x: any; y: number }
) {
  // 将SVG坐标系转换为笛卡尔坐标系
  const ax = pt11.x;
  const ay = -pt11.y;
  const bx = pt12.x;
  const by = -pt12.y;
  const cx = pt21.x;
  const cy = -pt21.y;
  const dx = pt22.x;
  const dy = -pt22.y;

  // 计算向量AB和向量CD
  const ABx = bx - ax;
  const ABy = by - ay;
  const CDx = dx - cx;
  const CDy = dy - cy;

  // 计算AB和CD的长度
  const ABLength = Math.sqrt(ABx * ABx + ABy * ABy);
  const CDLength = Math.sqrt(CDx * CDx + CDy * CDy);

  // 计算向量AB和向量CD的点积
  const dot = ABx * CDx + ABy * CDy;

  // 计算AB和CD之间的夹角（以弧度表示）
  const angle = Math.acos(dot / (ABLength * CDLength));

  // 将弧度转换为角度
  const degree = angle * (180 / Math.PI);

  return degree;
}

// 路口Draw对象dw；距离基点d，弧度增量dr，长度len，返回新点
export function cal_point(
  dw: { dir: { radian: any }; origin: { x: number } },
  d: number,
  dr: number,
  len: number
) {
  // 计算
  var np = { x: 0, y: 0 }; // new point

  var tx, ty; // 临时点坐标
  var r = dw.dir.radian;
  // 基线上终点
  tx = Math.cos(r) * d + dw.origin.x;
  ty = -Math.sin(r) * d + dw.origin.x;
  // 垂直基线，交点在tx，ty，长度len的点
  np.x = len * Math.cos(r + dr) + tx;
  np.y = -len * Math.sin(r + dr) + ty;

  return np;
}

//曲度插值法计算Q的中间点
export function insect_pt(line1: any, line2: any) {
  var insect_pt = intersect_line_point(
    line1.point2,
    line1.point1,
    line2.point2,
    line2.point1
  );
  // 相邻两直线的交点
  var mid_pt = {
    x: (line1.point2.x + line2.point2.x) * 0.5,
    y: (line1.point2.y + line2.point2.y) * 0.5,
  };
  // 曲度插值法计算Q的中间点
  if (insect_pt) {
    var qpt = {
      x: mid_pt.x + (insect_pt.x - mid_pt.x),
      y: mid_pt.y + (insect_pt.y - mid_pt.y),
    };
    return qpt;
  }
  return false;
}
