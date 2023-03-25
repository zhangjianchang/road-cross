/**
 * 根据类型获取服务评价等级
 * @param type 统计类型：饱和度/延误时间
 * @param ratio 分析数值
 */
export const getServiceByTypeAndRatio = (type: string, ratio: number) => {
  if (type === "delay") {
    if (ratio <= 10) return "A";
    else if (ratio <= 20) return "B";
    else if (ratio <= 35) return "C";
    else if (ratio <= 55) return "D";
    else if (ratio <= 80) return "E";
    else return "F";
  } else if (type === "saturation") {
    if (ratio <= 0.25) return "A";
    else if (ratio <= 0.5) return "B";
    else if (ratio <= 0.7) return "C";
    else if (ratio <= 0.85) return "D";
    else if (ratio <= 0.95) return "E";
    else return "F";
  }
};
