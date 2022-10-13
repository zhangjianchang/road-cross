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
  } catch {}
}

// 求角度
export function getAngle(cx: number, cy: number, x: number, y: number) {
  let quadrant = getQuadrant(x, y);
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
  var angle = Math.floor(180 / (Math.PI / radian)); //弧度转角度
  //每增加一个象限加90度
  angle += (quadrant - 1) * 90;
  return angle;
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
  k2 = -(b[0] - a[0]) / (b[1] - a[1]);
  /*
   * 弯曲程度是根据中垂线斜率决定固定控制点的X坐标或者Y坐标,通过中垂线公式求出另一个坐标
   * 默认A/B中点的坐标+curv*10,可以通过改基数10改变传入的参数范围
   */

  if (k2 < 2 && k2 > -2) {
    controX = Math.abs((b[0] + a[0]) / 2 + curv * 20);
    controY = Math.abs(k2 * (controX - (a[0] + b[0]) / 2) + (a[1] + b[1]) / 2);
  } else {
    controY = Math.abs((b[1] + a[1]) / 2 + curv * 20);
    controX = Math.abs((controY - (a[1] + b[1]) / 2) / k2 + (a[0] + b[0]) / 2);
  }
  //定义控制点的位置
  var Q = `${controX},${controY}`;
  return Q;
}

//根据坐标获取象限
export function getQuadrant(x: number, y: number) {
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
  const quadrant = getQuadrant(x, y);
  if ((quadrant === 1 || quadrant === 4) && controX >= x) {
    flag = false;
  }
  if ((quadrant === 2 || quadrant === 3) && controX <= x) {
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
