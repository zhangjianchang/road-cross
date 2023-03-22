//重构计算start
/**
 * 进口车道实际交通量
 * @param q 进口道转向流量
 * @param d 大车比率
 * @returns
 */
export function get_V(q: number, d: number) {
  //V = q * (1 - d) + q * d * 2;
  var V = q * (1 + d);
  return V;
}

/**
 * 进口车道饱和度
 * @param V 进口车道实际交通量
 * @param PHF 进口车道高峰小时流量系数(0.95)
 * @param S 进口车道饱和流率（1650）
 * @param λ 绿信比
 * @returns
 */
export function get_x(V: number, PHF: number, S: number, λ: number) {
  if (PHF === 0 || S === 0 || λ === 0) return 0;
  var x = V / PHF / (S * λ);
  return x;
}
//重构计算end

// 线性插值函数
export function Ln(x1: number, y1: number, x2: number, y2: number, x: number) {
  var y = 0;
  if (x1 != x2) {
    y = ((y2 - y1) * (x - x1)) / (x2 - x1) + y1;
  } else {
    y = (y2 + y1) * 0.5;
  }
  return y;
}

// 废弃
// // 车道通行能力 traffic_capacity
// // 只考虑7种车道类型：直行Cs、专右、专左、左右转、直右、直左、直左右

// // tr_Cs：直行通行能力
// // T：信号周期
// // t：对应相位的绿灯时间s
// // cart：大车比（%）
// export function tr_Cs(T: number, t: number, cart: any) {
//   // 获得大车比率ti，计算直行通行能力Cs - capacity straight
//   var ti = get_ti(cart);
//   var Cs = (3600 / T) * ((t - 2.3) / ti + 1) * 0.9;
//   return Cs;
// }

// // 查表法计算大车比率ti
// export function get_ti(cart: number) {
//   // 查找表
//   var arr = [
//     [0, 2.5],
//     [0.2, 2.65],
//     [0.3, 2.96],
//     [0.4, 3.12],
//     [0.5, 3.26],
//     [0.6, 3.3],
//     [0.7, 3.34],
//     [0.8, 3.42],
//     [1, 3.5],
//   ];
//   cart = Math.min(1, Math.max(0, cart));
//   var ti = 2.5; // 默认2.5
//   var idx = 0;
//   for (var i = 0; i < arr.length; i++) {
//     // 定位
//     if (cart > arr[i][0]) {
//       idx = i;
//     } else if (cart == arr[i][0]) {
//       ti = arr[i][1];
//       return ti;
//     } else break;
//   }
//   ti = Ln(arr[idx][0], arr[idx][1], arr[idx + 1][0], arr[idx + 1][1], cart);

//   return ti;
// }

// // tr_Csr：直右通行能力 straight right
// export function tr_Csr(Cs: any) {
//   // 与Cs一样
//   var Csr = Cs;
//   return Csr;
// }

// // tr_Csl：直左通行能力 straight left
// // Bl：为直左车道中左转车所占比例（可取：左转流量/（左转流量+该车道直线流量）
// export function tr_Csl(Cs: number, Bl: number) {
//   var Csl = Cs * (1 - Bl * 0.5);
//   return Csl;
// }

// // tr_Cslr：直左右通行能力 straight left right
// // 直左右车道只存在于单进口车道
// export function tr_Cslr(Cs: number) {
//   var Cslr = Cs * (1 - 0.25 / 2);
//   return Cslr;
// }

// // tr_Celr：同时有专用左转与专用右转车道 e=special
// // total_Cs：该进口直行车道通行能力之和
// // Bl，Br：分别为左、右转车占本面进口道车辆的比例 = 左(右)转车流量/本车道总流量
// export function tr_Celr(total_Cs: number, Bl: number, Br: number) {
//   var Cslr = total_Cs / (1 - Bl - Br);
//   return Cslr;
// }

// export function getCelr(total_Cs: number, Bl: number) {
//   var Cslr = total_Cs / (1 - Bl);
//   return Cslr;
// }

// // tr_Cl：专左通行能力
// export function tr_Cl(Celr: number, Bl: number) {
//   var Cl = Celr * Bl;
//   return Cl;
// }
// // tr_Cr：专右通行能力
// export function tr_Cr(Celr: number, Br: number) {
//   var Cr = Celr * Br;
//   return Cr;
// }

// // tr_Cel：只有专用左转，而无专用右转车道
// // total_Cs：该进口直行车道通行能力之和
// // Bl，Br：分别为左、右转车占本面进口道车辆的比例 = 左(右)转车流量/本车道总流量
// export function tr_Cel(total_Cs: any, Csr: any, Bl: number) {
//   var Cel = (total_Cs + Csr) / (1 - Bl);
//   return Cel;
// }
// // tr_Cer：只有专用右转，而无专用左转车道
// export function tr_Cer(total_Cs: any, Csr: any, Br: number) {
//   var Cer = (total_Cs + Csr) / (1 - Br);
//   return Cer;
// }

// // 计算饱和度V/C
// // 各车道饱和度（V/C）=该车道流量/该进口道的通行能力
// export function tr_VC(v: number, c: number) {
//   return v / c;
// }
// // 交叉口饱和度取各进口道饱和度乘以进口道流量为权的加权平均值
// export function cal_VC(d: string | any[], q: number[]) {
//   var sum1 = 0,
//     sum2 = 0;
//   for (var i = 0; i < d.length; i++) {
//     sum1 += d[i] * q[i];
//     sum2 += q[i];
//   }
//   var vc = sum1 / sum2;
//   return vc;
// }
