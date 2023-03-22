/**
 *
 * @param C 信号周期时长
 * @param λ 进口道绿信比
 * @param x 进度道饱和度
 * @param e 信号控制类型校正系数，取0.5
 * @param Q 进口车道通行能力，即CAP
 * @param T 分析时段时长，取0.25
 */
export const get_d = (
  C: number,
  λ: number,
  x: number,
  e: number,
  Q: number,
  T: number
) => {
  //如果绿信比和饱和度为0，返回0
  if (λ === 0 || x === 0) return 0;
  const d1 = (0.5 * C * Math.pow(1 - λ, 2)) / (1 - Math.min(1, x) * λ);
  const d2 =
    900 * T * (x - 1 + Math.sqrt(Math.pow(x - 1, 2) + (8 * e * x) / (Q * T)));
  return d1 + d2;
};
