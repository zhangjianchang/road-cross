<template>
  <div class="basic-main">
    <div class="func">功能区</div>
    <!-- 图示 -->
    <svg id="canvas">
      <text v-for="(_, index) in road_attr" :key="index" x="330">
        <textPath :xlink:href="'#road_' + (index + 1)">
          方向{{ index + 1 }}
        </textPath>
      </text>
      <!-- 路标 -->
      <g
        v-for="road in road_sign_pts"
        :key="road.g"
        :transform="road.g.transform"
        class="road-sign"
        :id="road.g.id"
      >
        <rect
          x="200"
          y="-100"
          rx="100"
          ry="100"
          width="640"
          height="1200"
          :fill="road.rect.background"
          stroke="#ddd"
          stroke-width="2"
        />
        <path :d="road.sign.d" fill="#fff"></path>
        <text x="180" y="1400" fill="#000" style="font-size: 260px">
          {{ road.rect.saturation }}
        </text>
      </g>
    </svg>

    <!-- 参数 -->
    <div class="menu">延误分析</div>
  </div>
</template>

<script lang="ts">
import { defineComponent, inject, onMounted, reactive, toRefs } from "vue";
import { q_ql, getBackground, getRoadDefaultSign } from "./index";
import Container from "../../../components/Container/index.vue";
import { DragOutlined } from "@ant-design/icons-vue";
import { getAngle, getQByPathCurv } from "../../../utils/common";
import { RoadInfo, road_info } from "..";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      road_width: 160, //路宽
      curvature: 2, //路口弧度
      cross_pts: [] as any[], //所有路口交叉点
      road_sign_pts: [] as any[], //路标
      total_color: "#fff", //中心总颜色
      total_saturation: "0.00", //中心总数值
    });

    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      render();
    };

    function render() {
      for (var i = 0; i < road_info.road_attr.length; i++) {
        var angle = road_info.road_attr[i].angle;
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        var x3 = Math.cos(radian) * states.road_width + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * states.road_width + 350;
        //获取交叉口圆plus和路边相交的点
        setPts(states.cross_pts, angle, x3, y3);
      }
      // 交叉口
      drawMain();
    }

    //direction：方向，nr 近右,nl 近左, fr 远右, fl 远左 //road_width默认100（小于100即画路边线时用到）
    function getPoint(direction: string, angle: number, x: number, y: number) {
      var radian = (Math.PI / 180) * (angle - 90);
      var coefficient = direction === "nl" || direction === "fr" ? -1 : 1;
      var point_x =
        coefficient * states.road_width * 0.5 * Math.cos(radian) + x;
      var point_y =
        -coefficient * states.road_width * 0.5 * Math.sin(radian) + y;
      return [~~point_x, ~~point_y];
    }

    function setPts(pts: any[], angle: number, x: number, y: number) {
      var point = getPoint("nr", angle, x, y);
      pts.push(point);
      point = getPoint("nl", angle, x, y);
      pts.push(point);
    }

    function drawMain() {
      //画路径
      drawRoadLine();
      //画路标
      drawRoadSign();
    }

    //画路径
    function drawRoadLine() {
      const line = document.createElementNS(states.ns, "path"); // 创建SVG元素——交叉口
      let d_str = "";
      let roadIdx = 0;
      for (let i = 0; i < states.cross_pts.length; i++) {
        const pt = states.cross_pts[i];
        if (i === 0) {
          d_str += `M ${pt[0]} ${pt[1]} `;
        } else if (i % 2 !== 0) {
          /* 路边缘点 */
          let angle = road_info.road_attr[roadIdx].angle;
          const radian = (Math.PI / 180) * angle; // 角度转弧度
          const x = Math.cos(radian) * 300 + 350; // 大圆半径300
          const y = -Math.sin(radian) * 300 + 350;
          let point = getPoint("fl", angle, x, y);
          d_str += `L ${point[0]} ${point[1]} `;
          point = getPoint("fr", angle, x, y);
          d_str += `L ${point[0]} ${point[1]} `;
          //连接黄色点
          d_str += `L ${pt[0]} ${pt[1]} `;
          /* Q */
          const nextPt =
            i + 1 === states.cross_pts.length
              ? states.cross_pts[0]
              : states.cross_pts[i + 1];
          const Q = getQByPathCurv(pt, nextPt, states.curvature);
          d_str += `Q ${Q} `;
          if (i === states.cross_pts.length - 1) {
            //最后一个点曲线连接
            const firstPt = states.cross_pts[0];
            d_str += `${firstPt[0]} ${firstPt[1]} `;
          }
          /*标记第几条路 */
          roadIdx++;
        } else {
          d_str += ` ${pt[0]} ${pt[1]} `;
        }
      }
      drawPath(line, d_str);
    }

    //画路标
    function drawRoadSign() {
      //路标
      let roadIndex = 0;
      for (var i = 0; i < states.cross_pts.length; i++) {
        let all_count = 5; //路分为5份
        var pt = states.cross_pts[i];
        var prevPt = states.cross_pts[i - 1];
        if (i > 0 && i % 2 !== 0) {
          for (let way_idx = 1; way_idx < all_count - 1; way_idx++) {
            var is_last = all_count === way_idx + 2;
            var signIndex = way_idx - 1;
            //左侧道路、右侧道路离中心距离微调
            var k = way_idx;
            //(x1+k(x2-x1)/n,y1+k(y2-y1)/n)线段n等分公式
            var wayPt = [
              prevPt[0] + (k * (pt[0] - prevPt[0])) / all_count,
              prevPt[1] + (k * (pt[1] - prevPt[1])) / all_count,
            ];

            //外层
            const g = {
              transform: `rotate(${
                270 - road_info.road_attr[roadIndex].angle
              } ${wayPt[0]},${wayPt[1]}) translate(${wayPt[0]},${
                wayPt[1]
              }) scale(0.04)`,
              id: `g${i}${way_idx}`,
            };
            //路标
            const sign = {
              d: getRoadDefaultSign(signIndex, false, is_last),
            };
            let saturation = getQueue(roadIndex, signIndex).toFixed(1);
            let background = getBackground(saturation);
            //圆角矩形背景
            const rect = {
              saturation,
              background,
            };
            states.road_sign_pts.push({ g, sign, rect });
          }
          roadIndex++;
        }
      }
      let total_saturation = 0;
      states.road_sign_pts.forEach((r) => {
        total_saturation += Number(r.rect.saturation);
      });
      states.total_saturation = (total_saturation / (3 * 4)).toFixed(2); //目前有三种类型
      states.total_color = getBackground(states.total_saturation);
    }

    //画路面上的线
    function drawPath(line: Element, d_str: string) {
      line.setAttribute("id", "road_path");
      line.setAttribute("d", d_str);
      line.setAttribute("stroke", "#ddd");
      line.setAttribute("stroke-width", "2");
      line.setAttribute("fill", `none`);
      states.cvs?.appendChild(line);
    }

    const getQueue = (roadIndex: number, wayIndex: number) => {
      const q = getCurrentWayFlow(roadIndex, wayIndex);
      const r = get_red(roadIndex);
      const Q = road_info.flow_info.saturation[roadIndex][wayIndex].number;
      var ql = q_ql(q, r, Q);
      return ql;
    };

    //当前方向对应的绿灯时间
    const get_red = (roadIndex: number) => {
      let t = 0;
      for (let i = 0; i < road_info.signal_info.phase; i++) {
        const phase_item = road_info.signal_info.phase_list[i];
        //todo 暂定直行车道为当前索引对应车道
        const current =
          roadIndex === road_info.road_attr.length - 1 ? 0 : roadIndex + 1;
        t += phase_item.directions[roadIndex][current].green;
        t += phase_item.directions[roadIndex][current].yellow;
      }
      //总周期减绿灯黄灯时间
      const red = road_info.signal_info.period - t;
      return red;
    };

    //当前道路某条车道的车流量
    const getCurrentWayFlow = (roadIndex: number, wayIdx: number) => {
      const current =
        road_info.flow_info.flow_detail[roadIndex].turn[wayIdx + 1].number;
      return current;
    };

    onMounted(() => {
      initRoads();
    });

    return {
      ...toRefs(states),
      ...toRefs(road_info),
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");

.road-sign {
  width: 32px;
  height: 60px;
  background-color: red;
  margin-left: 15px;
  border-radius: 4px;
  cursor: pointer;
}
</style>
