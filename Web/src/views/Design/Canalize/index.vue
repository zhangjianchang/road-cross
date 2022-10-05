<template>
  <div class="basic-main">
    <div class="func">功能区</div>
    <!-- 图示 -->
    <svg id="canvas"></svg>

    <!-- 参数 -->
    <div class="menu">
      <div class="form">
        <div class="header">交叉口属性</div>
        <div class="content">
          <a-form
            :label-col="labelCol"
            :wrapper-col="wrapperCol"
            layout="vertical"
          >
            <a-row>
              <a-col :span="12">
                <a-form-item label="方向">
                  <a-select
                    v-model:value="modelRef.direction"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option
                      v-for="(_, index) in roadDir"
                      :key="(index + 1).toString()"
                      :value="(index + 1).toString()"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="交叉口大小">
                  <a-input-number
                    v-model:value="modelRef.size"
                    :min="-10"
                    :max="10"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="12">
                <a-form-item label="右转曲度">
                  <a-input-number
                    v-model:value="modelRef.curvature"
                    @change="drawCross"
                    :min="0"
                    :max="1"
                    :step="0.1"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import {
  createVNode,
  defineComponent,
  inject,
  onMounted,
  reactive,
  ref,
  toRefs,
} from "vue";
import { columns } from "./index";
import Container from "../../../components/Container/index.vue";
import { Modal, notification } from "ant-design-vue";
import { DragOutlined, ExclamationCircleOutlined } from "@ant-design/icons-vue";
import { getAngle, getQByPathCurv } from "../../../utils/common";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    const roadDir = inject("RoadDir") as any[];

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      angleSet: [] as number[], //所有道路倾斜角，以此绘制
      cross_pts: [] as any[], //所有路口交叉点
      currentRoad: {} as any, //当前选中道路
      isDrag: false,
    });

    //参数设置
    const modelRef = reactive({
      direction: "1", //方向
      size: 5, //交叉路口大小
      curvature: 0.5, //右转道路曲率
    } as any);

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
      render();
    };

    // //画路
    // function createRoad(angle: number) {
    //   const g = document.createElementNS(states.ns, "g");
    //   g.setAttribute(
    //     "transform",
    //     `rotate(-${angle} ${states.cx} ${states.cy})`
    //   );
    //   const polygon = document.createElementNS(states.ns, "polygon");
    //   polygon.setAttribute("style", "fill: #989898");
    //   polygon.setAttribute("points", "350,300 650,300 650,400 350,400");
    //   g.appendChild(polygon);
    //   states.cvs?.appendChild(g);
    // }

    //
    const onReset = () => {
      Modal.confirm({
        title: "确定重新绘制",
        icon: createVNode(ExclamationCircleOutlined),
        content: "确定后将清空所有已做操作",
        okText: "确认",
        cancelText: "取消",
        onOk() {
          confirm();
        },
      });
    };

    const confirm = () => {
      console.log(1111);
    };

    function render() {
      var cross_r = 100;
      for (var i = 0; i < states.angleSet.length; i++) {
        var angle = states.angleSet[i];
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        // 基线起（圆心）止（圆上）点
        var x2 = Math.cos(radian) * 300 + 350; // 大圆半径300
        var y2 = -Math.sin(radian) * 300 + 350;
        var x3 = Math.cos(radian) * cross_r + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * cross_r + 350;

        var d_str = "";
        // 圆心右侧点
        var point = getPoint("nr", angle, states.cx, states.cy);
        d_str += `M ${point[0]}, ${point[1]}, `;
        // 圆心左侧点
        point = getPoint("nl", angle, states.cx, states.cy);
        d_str += `L ${point[0]}, ${point[1]}, `;
        // 远端右侧点
        point = getPoint("fr", angle, x2, y2);
        d_str += `L ${point[0]}, ${point[1]}, `;
        // 远端左侧点
        point = getPoint("fl", angle, x2, y2);
        d_str += `L ${point[0]}, ${point[1]}, Z`;
        drawRoad(i, d_str);

        // 交叉口圆plus和路边相交的点
        point = getPoint("nl", angle, x3, y3);
        states.cross_pts.push(point);
        point = getPoint("nr", angle, x3, y3);
        states.cross_pts.push(point);
      }

      // 交叉口圆plus
      drawCross();
    }

    //direction：方向，nr 近右,nl 近左, fr 远右, fl 远左
    function getPoint(direction: string, angle: number, x: number, y: number) {
      var road_width = 100; // 路宽
      var radian = (Math.PI / 180) * (angle - 90);
      var coefficient = direction === "nl" || direction === "fr" ? -1 : 1;
      var point_x = coefficient * road_width * 0.5 * Math.cos(radian) + x;
      var point_y = -coefficient * road_width * 0.5 * Math.sin(radian) + y;
      return [point_x, point_y];
    }

    //画路
    function drawRoad(index: number, d_str: string) {
      var road = document.createElementNS(states.ns, "path"); // 创建SVG元素——路
      road.setAttribute("id", (index + 1).toString());
      road.setAttribute("d", d_str);
      road.setAttribute("stroke", "rgb(162,162,162)");
      road.setAttribute("fill", `rgb(162,162,162)`);
      road.addEventListener("click", onRoad, false);
      states.cvs?.appendChild(road);
    }

    //画交叉口
    function drawCross() {
      //先清空交叉路口
      document.querySelectorAll("path").forEach((e) => {
        if (e.id === "cross") e.remove();
      });
      var cross = document.createElementNS(states.ns, "path"); // 创建SVG元素——交叉口
      var d_str = "";
      for (var i = 0; i < states.cross_pts.length; i++) {
        var pt = states.cross_pts[i];
        if (i == 0) {
          //第一个点
          d_str += `M ${pt[0]},${pt[1]} `;
        } else if (i % 2 !== 0) {
          //第奇数个点为同一道路的点，用直线连接
          d_str += `L ${pt[0]},${pt[1]} `;
          //最后一个点需要用曲线连接第一个点
          if (i === states.cross_pts.length - 1) {
            var firstpt = states.cross_pts[0];
            var Q = getQByPathCurv(pt, firstpt, modelRef.curvature);
            d_str += `Q ${Q} ${firstpt[0]}, ${firstpt[1]} `;
            // drawPoint(Q);
          }
        } else {
          //第偶数个点为相邻道路的点，用曲线连接
          var prevPt = states.cross_pts[i - 1];
          var Q = getQByPathCurv(pt, prevPt, modelRef.curvature);
          d_str += `Q ${Q} ${pt[0]}, ${pt[1]} `;
          // drawPoint(Q);
        }
      }
      cross.setAttribute("d", d_str);
      cross.setAttribute("id", "cross");
      cross.setAttribute("fill", "rgba(162,162,0,0.5)");
      states.cvs?.appendChild(cross);
    }

    function onRoad(e: any) {
      states.currentRoad = e.path[0];
      modelRef.direction = e.path[0].id;
    }

    function drawPoint(Q: string) {
      //test画出点
      var g = document.createElementNS(states.ns, "g");
      g.setAttribute("stroke", "black");
      g.setAttribute("stroke-width", "3");
      g.setAttribute("fill", "black");

      var circle = document.createElementNS(states.ns, "circle");
      circle.setAttribute("cx", Q.split(",")[0]);
      circle.setAttribute("cy", Q.split(",")[1]);
      circle.setAttribute("r", "3");
      g.appendChild(circle);

      states.cvs?.appendChild(g);
    }

    onMounted(() => {
      initRoads();
    });

    return {
      ...toRefs(states),
      modelRef,
      labelCol: { span: 24 },
      wrapperCol: { span: 24 },
      roadDir,
      columns,
      drawCross,
      onReset,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
