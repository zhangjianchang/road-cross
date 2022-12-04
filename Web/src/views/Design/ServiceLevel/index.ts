import { Ln } from "../Saturation";

export const roadSigns = [
  {
    key: "uturn",
    name: "掉头",
    path: "M370.08 193.376C370.08 53.088 462.048 0 550.72 0c88.672 0 180.896 53.84 180.896 194.144V1024H619.04V194.144c0-65.472-26.944-89.008-68.32-89.008-41.376 0-75.104 22.864-74.8 88.24h64.224L421.648 573.136 303.152 193.376H370.08z",
  },
  {
    key: "left_uturn",
    name: "左转+掉头",
    path: "M697.056 1024h-128.448V640c0-17.52-16.224-41.76-48.672-41.76-32.448 0-49.36 24.08-49.36 41.76v156.576h65.92L439.84 1024 343.824 796.576h66.592V640c0-41.76 29.744-75.456 60.848-91.616 31.088-16.16 74.352-8.096 97.344 3.52V448l-96.336-96v128L311.712 192.256 472.272 0v128.768L695.84 352l1.216 672z",
  },
  {
    key: "left",
    name: "左转",
    path: "M456.784 146.352V0.24l-136.96 250.56 136.96 250.448v-146.096l137.12 92.752V1023.04h109.488V313.392z",
  },
  {
    key: "straight_left",
    name: "直行+左转",
    path: "M619.824 1024V446.784h84.464L577.408 0 446 446.784h84.144v303.728l-135.104-136.096V446.352L320.016 718.736l75.04 258.688v-153.504l135.104 128.16V1024z",
  },
  {
    key: "straight_uturn",
    name: "直行+掉头",
    path: "M588.864 0l115.168 447.872h-77.216V1024H544.448V447.872H471.792L588.864 0z m-44.56 551.92V640c0-17.52-16.16-41.76-48.512-41.76-32.32 0-49.168 24.08-49.168 41.76v156.576h65.68L415.984 1024 320.304 796.576h66.352V640c0-41.76 29.648-75.456 60.64-91.616 30.992-16.16 74.096-8.096 97.008 3.52z",
  },
  {
    key: "straight",
    name: "直行",
    path: "M461.536 447.872H376.192L512.896 0l133.12 447.872h-89.248V1024H461.536z",
  },
  {
    key: "right",
    name: "右转",
    path: "M559.356062 144.257969V0.236308l135.483076 247.004554-135.483076 246.862769v-144.021662l-135.640616 91.435323v566.980923H315.423508V308.901415z",
  },
  {
    key: "straight_right",
    name: "直行+右转",
    path: "M404.176 1024V446.784H319.712L446.592 0l131.408 446.784h-84.144v303.728l135.104-136.096V446.352l75.04 272.384-75.04 258.688v-153.504l-135.104 128.16V1024z",
  },
  {
    key: "left_right",
    name: "左转+右转",
    path: "M512.064 288.48l159.392-159.68V0l160 192.256-160 287.744v-128l-96 96v576h-126.784V448l-96-96v128l-160-287.744L352.672 0v128.768z",
  },
  {
    key: "all_direction",
    name: "所有方向",
    path: "M456.944 657.488V447.872h-90.832L512.464 0l143.952 447.872h-96.528v209.04l112.16-95.296v-128.768l160.288 192.256-160.304 287.744v-128l-112.144 95.104V1024H456.944v-143.472l-104.304-95.68v128L192.336 625.104l160.32-192.256V561.6z",
  },
  {
    key: "variable",
    name: "可变车道",
    path: "",
  },
  {
    key: "tidal_lane",
    name: "潮汐车道",
    path: "",
  },
  {
    key: "bus_only",
    name: "公交专用",
    path: "",
  },
  {
    key: "none",
    name: "无",
    path: "",
  },
  {
    hide: true,
    key: "reverse_straight",
    name: "逆行",
    path: "m465.999916,1.936573l2.000937,575.397813l-94.666839,-1.333336l138.666919,446.667481l137.333584,-445.334145l-85.333489,-1.333336l0,-576.00105",
  },
];

//根据车道获取默认路标 k系数，>=1正向路，否则反向路
export function getRoadDefaultSign(
  wayIndex: number,
  is_reverse: boolean,
  is_last: boolean
): string {
  let roadSignKey = "";
  if (is_reverse) {
    //反向车道
    roadSignKey = "reverse_straight";
  } else if (is_last) {
    //右边最后一条路
    roadSignKey = "straight_right";
  } else {
    switch (wayIndex) {
      //右边第一条路
      case 0:
        roadSignKey = "left";
        break;
      //右边其余路
      default:
        roadSignKey = "straight";
    }
  }
  let roadSignPath = roadSigns.find((s) => s.key === roadSignKey)?.path;
  if (roadSignPath) {
    return roadSignPath;
  }
  return "";
}

//根据程度取颜色
export function getBackground(number: string | undefined) {
  if (number === "A") {
    return "rgb(0,255,0)";
  } else if (number === "B") {
    return "rgb(100,255,0)";
  } else if (number === "C") {
    return "rgb(150,255,0)";
  } else if (number === "D") {
    return "rgb(255,200,0)";
  } else if (number === "E") {
    return "rgb(255,100,0)";
  } else if (number === "F") {
    return "rgb(255,50,0)";
  } else {
    return "rgb(255,0,0)";
  }
}

export function d_d(d1: number, d2: number) {
  return d1 + d2;
}

export function d_d1(C: number, x: number, λ: number) {
  var d1 = 0.5 * C * (Math.pow(1 - λ, 2) / (1 - Math.min(1, x) * λ));
  return d1;
}

export function d_d2(x: number, CAP: number) {
  var T = 0.25;
  var e = 0.5;
  var sqrt_number = Math.pow(x - 1, 2) + T * ((8 * e * x) / CAP);
  var dm = Math.pow(x - 1, 2) + Math.sqrt(sqrt_number);
  return 900 * T * dm;
}

export function getLevelByd(d: number) {
  if (d <= 10) {
    return "A";
  } else if (d > 10 && d <= 20) {
    return "B";
  } else if (d > 20 && d <= 35) {
    return "C";
  } else if (d > 35 && d <= 55) {
    return "D";
  } else if (d > 55 && d <= 80) {
    return "E";
  } else if (d > 80) {
    return "F";
  }
}

// 查表法计算大车比率ti
export function get_ti(cart: number) {
  // 查找表
  var arr = [
    [0, 10],
    [10, 20],
    [20, 35],
    [35, 55],
    [55, 80],
    [80, 9999999],
  ];
  cart = Math.min(1, Math.max(0, cart));
  var ti = 2.5; // 默认2.5
  var idx = 0;
  for (var i = 0; i < arr.length; i++) {
    // 定位
    if (cart > arr[i][0]) {
      idx = i;
    } else if (cart == arr[i][0]) {
      ti = arr[i][1];
      return ti;
    } else break;
  }
  ti = Ln(arr[idx][0], arr[idx][1], arr[idx + 1][0], arr[idx + 1][1], cart);
  return ti;
}
