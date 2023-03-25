export const get_queue = (
  q: number,
  PHF: number,
  d: number,
  C: number,
  λ: number,
  x: number,
  s_L: number,
  g_E: number
) => {
  if (x === 0) return 0;
  const k = 5.5; //车长5.5m
  const V_L = (q * (1 + d)) / PHF;
  const C_L = V_L;
  const Q1 = get_Q1(V_L, C, λ, x);
  const Q2 = get_Q2(C_L, x, s_L, g_E);
  const Q = k * (Q1 + Q2);
  return Q;
};

/**
 * 均匀排队长度
 * @param V_L
 * @param C
 * @param λ
 * @param x
 * @returns
 */
export const get_Q1 = (V_L: number, C: number, λ: any, x: any) => {
  const Q1 = (((V_L * C) / 3600) * (1 - λ)) / (1 - Math.min(1, x) * λ);
  return Q1;
};

/**
 * 溢出排队长度
 * @param C_L
 * @param x
 * @param s_L
 * @param g_E
 * @returns
 */
export const get_Q2 = (C_L: number, x: number, s_L: number, g_E: number) => {
  const T = 0.25;
  const K_B = 0.12 * Math.pow((s_L * g_E) / 3600, 0.7);
  const a = x - 1;
  const b = Math.sqrt(Math.pow(x - 1, 2) + (8 * K_B * x) / (C_L * 0.25));
  const Q2 = 0.25 * C_L * T * (a + b);
  return Q2;
};
