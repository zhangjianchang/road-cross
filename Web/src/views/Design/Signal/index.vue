<template>
  <div class="basic-main">
    <!-- 图示 -->
    <svg id="canvas">
      <g v-for="road in road_pts" :key="road.g" :transform="road.g.transform">
        <rect width="80%" height="680" fill="#FFFF99" />
        <path
          :d="road.path.d"
          :id="road.path.id"
          fill="rgb(162,162,162)"
        ></path>
      </g>
    </svg>
    <!-- 参数 -->
    <div class="menu">信号设计</div>
  </div>
</template>

<script lang="ts">
import { defineComponent, inject, onMounted, reactive, toRefs } from "vue";
import Container from "../../../components/Container/index.vue";
import { DragOutlined } from "@ant-design/icons-vue";
import { getAngle, getQByPathCurv } from "../../../utils/common";
import { func } from "vue-types";
import { setLineXY } from "../Basic";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    const roadDir = inject("RoadDir") as any[];

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      svg_width: 800, //画布宽度
      road_width: 160, //路宽
      curvature: 2, //路口弧度
      angleSet: [] as number[], //所有道路倾斜角，以此绘制
      cross_pts: [] as any[], //所有路口交叉点
      road_pts: [] as any[], //路标
    });

    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      roadDir.map((r) => {
        let angle = getAngle(
          states.cx,
          states.cy,
          r.coordinate[0],
          r.coordinate[1]
        );
        states.angleSet.push(angle);
      });
      states.angleSet.sort(function (a, b) {
        return a - b;
      });
      render();
    };

    function render() {
      drawScale();
      drawMain();
    }

    function drawScale() {
      let totalScale = 80; //根据总时间计算
      for (let i = 0; i <= totalScale; i++) {
        let width = states.svg_width / totalScale;
        let tick_len = 10; // 小刻度长度=10
        if (i % 5 == 0) tick_len = 20; // 长刻度=20
        let x1, y1, x2, y2; // 直线的2个端点
        x1 = 25 + i * width;
        y1 = 230;
        x2 = 25 + i * width;
        y2 = 230 + tick_len;
        let line = document.createElementNS(states.ns, "line"); // 创建SVG元素
        setLineXY(line, x1, y1, x2, y2);
        states.cvs?.appendChild(line);
      }
    }

    //画路径
    function drawMain() {
      for (var i = 0; i < states.angleSet.length; i++) {
        var angle = states.angleSet[i];
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        var x3 = Math.cos(radian) * states.road_width + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * states.road_width + 350;
        //获取交叉口圆plus和路边相交的点
        setPts(states.cross_pts, angle, x3, y3);
      }
      for (let i = 0; i < 4; i++) {
        let d_str = "";
        let roadIdx = 0;
        for (let i = 0; i < states.cross_pts.length; i++) {
          const pt = states.cross_pts[i];
          if (i === 0) {
            d_str += `M ${pt[0]} ${pt[1]} `;
          } else if (i % 2 !== 0) {
            /* 路边缘点 */
            let angle = states.angleSet[roadIdx];
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
        drawPath(d_str, i);
      }
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

    //写路径至数组
    function drawPath(d_str: string, i: number) {
      const path = {
        id: "road_path",
        d: d_str,
      };
      const g = {
        transform: `translate(${204 * i + 20},10) scale(0.29)`,
      };
      states.road_pts.push({ g, path });
    }

    onMounted(() => {
      initRoads();
    });

    return {
      ...toRefs(states),
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");

.road-sign {
  cursor: pointer;
}
</style>
