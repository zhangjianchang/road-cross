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

export const lineColumns = [
  {
    title: "服务水平",
    dataIndex: "serviceLevel",
    width: 40,
  },
  {
    title: "饱和度S",
    dataIndex: "saturation",
    width: 40,
  },
  {
    title: "延误时间T(秒)",
    dataIndex: "delay",
    width: 40,
  },
];

export const lineData = [
  {
    serviceLevel: "A",
    saturation: "[0,0.25]",
    delay: "[0,10]",
  },
  {
    serviceLevel: "B",
    saturation: "(0.25,0.5]",
    delay: "(10,20]",
  },
  {
    serviceLevel: "C",
    saturation: "(0.5,0.7]",
    delay: "(20,35]",
  },
  {
    serviceLevel: "D",
    saturation: "(0.7,0.85]",
    delay: "(35,55]",
  },
  {
    serviceLevel: "E",
    saturation: "(0.85,0.95]",
    delay: "(55,80]",
  },
  {
    serviceLevel: "F",
    saturation: "(0.95,+∞]",
    delay: "(80,+∞]",
  },
];
