<template>
  <div class="basic-main">
    <div class="func">功能区</div>
    <!-- 图示 -->
    <svg id="canvas">
      <text v-for="(_, index) in roadDir" :key="index" x="300">
        <textPath :xlink:href="'#road_' + (index + 1)" y="50">
          方向{{ index + 1 }}
        </textPath>
      </text>
    </svg>
    <!-- 参数 -->
    <div class="menu">
      <div class="form">
        <div class="header">交叉口属性</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
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
                      :value="'road_' + (index + 1)"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="机动车道">
                  <a-button
                    type="primary"
                    size="small"
                    class="line-button"
                    @click="modalVisible = true"
                  >
                    设置
                  </a-button>
                  <a-button
                    type="default"
                    size="small"
                    class="line-button"
                    style="margin-left: 3px"
                  >
                    取消
                  </a-button>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="交叉口大小">
                  <a-input-number
                    v-model:value="modelRef.size"
                    :min="0"
                    :max="10"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
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
    <a-modal
      :visible="modalVisible"
      width="1000px"
      title="设置道路交通标识"
      :footer="null"
      @cancel="modalVisible = false"
    >
      <div class="modal-container">
        <!-- 绘制路标 -->
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 1024 1024"
          class="road-sign"
          v-for="road in roadSigns"
          :key="road.key"
          @click="handleConfirmSign(road.key)"
        >
          <path :d="road.path" fill="#ffffff" :title="road.name"></path>
        </svg>
      </div>
    </a-modal>
  </div>
</template>

<script lang="ts">
import { defineComponent, inject, onMounted, reactive, toRefs } from "vue";
import { getRoadDefaultSign, roadSigns } from "./index";
import Container from "../../../components/Container/index.vue";
import { notification } from "ant-design-vue";
import { DragOutlined } from "@ant-design/icons-vue";
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
      cross_line_pts: [] as any[], //所有路口交叉点内侧一层
      road_sign_pts: [] as any[], //所有路口交叉点内侧一层
      currentRoad: {} as any, //当前选中道路
      currentSign: {} as any, //当前选中路标
      modalVisible: false, //车道弹窗
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
      states.angleSet.sort(function (a, b) {
        return a - b;
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
        drawRoad(i, angle, x2, y2);

        // 获取交叉口圆plus和路边相交的点
        setPts(states.cross_pts, angle, x3, y3);
        // 获取交叉口圆plus和路边相交的点(内侧边缘线)
        setPts(states.cross_line_pts, angle, x3, y3, 90);
        //获取路标一圈的定位
        setPts(states.road_sign_pts, angle, x3, y3, 110);
      }
      // 交叉口
      drawCross();
    }

    //direction：方向，nr 近右,nl 近左, fr 远右, fl 远左 //road_width默认100（小于100即画路边线时用到）
    function getPoint(
      direction: string,
      angle: number,
      x: number,
      y: number,
      road_width = 100
    ) {
      // 路宽
      var radian = (Math.PI / 180) * (angle - 90);
      var coefficient = direction === "nl" || direction === "fr" ? -1 : 1;
      var point_x = coefficient * road_width * 0.5 * Math.cos(radian) + x;
      var point_y = -coefficient * road_width * 0.5 * Math.sin(radian) + y;
      return [point_x, point_y];
    }

    function setPts(
      pts: any[],
      angle: number,
      x: number,
      y: number,
      road_width = 100
    ) {
      var point = getPoint("nr", angle, x, y, road_width);
      pts.push(point);
      point = getPoint("nl", angle, x, y, road_width);
      pts.push(point);
    }

    //画路
    function drawRoad(index: number, angle: number, x: number, y: number) {
      var g = document.createElementNS(states.ns, "g");
      g.setAttribute("id", `g${index + 1}`);
      //画主路
      drawMainRoad(g, angle, x, y, index);
      //路边缘线
      drawRoadLine(g, angle, x, y, 90);
      //双黄线
      drawRoadLine(g, angle, x, y, 5, "#FFA500");
      //单侧车道分界线
      drawRoadLine(g, angle, x, y, 33, "#FFFFFF", "25");
      drawRoadLine(g, angle, x, y, 60, "#FFFFFF", "25");
      states.cvs?.appendChild(g);
    }

    //画主路
    function drawMainRoad(
      g: any,
      angle: number,
      x: number,
      y: number,
      index: number
    ) {
      //路
      var road = document.createElementNS(states.ns, "path"); // 创建SVG元素——路
      var d_str = "";
      // 圆心右侧点
      var point = getPoint("nr", angle, states.cx, states.cy);
      d_str += `M ${point[0]}, ${point[1]}, `;
      // 圆心左侧点
      point = getPoint("nl", angle, states.cx, states.cy);
      d_str += `L ${point[0]}, ${point[1]}, `;
      // 远端右侧点
      point = getPoint("fr", angle, x, y);
      d_str += `L ${point[0]}, ${point[1]}, `;
      // 远端左侧点
      point = getPoint("fl", angle, x, y);
      d_str += `L ${point[0]}, ${point[1]}, Z`;
      road.setAttribute("d", d_str);
      road.setAttribute("id", `road_${index + 1}`);
      road.setAttribute("fill", `rgb(162,162,162)`);
      road.addEventListener("click", onRoad, false);
      g.appendChild(road);
    }

    //画路面上的线
    function drawRoadLine(
      g: any,
      angle: number,
      x: number,
      y: number,
      road_width: number,
      color = "#FFFFFF", //路线颜色
      dasharray = "0" //路线虚实，0实线，x间隔x的虚线
    ) {
      var leftLine = document.createElementNS(states.ns, "line");
      //左边道路
      const left1 = getPoint("nl", angle, states.cx, states.cy, road_width);
      const left2 = getPoint("fr", angle, x, y, road_width);
      leftLine.setAttribute("x1", left1[0].toString());
      leftLine.setAttribute("y1", left1[1].toString());
      leftLine.setAttribute("x2", left2[0].toString());
      leftLine.setAttribute("y2", left2[1].toString());
      leftLine.setAttribute("stroke", color);
      leftLine.setAttribute("stroke-width", "2");
      leftLine.setAttribute("stroke-dasharray", dasharray);
      g.appendChild(leftLine);
      //右边道路
      var rightLine = document.createElementNS(states.ns, "line");
      const right1 = getPoint("nr", angle, states.cx, states.cy, road_width);
      const right2 = getPoint("fl", angle, x, y, road_width);
      rightLine.setAttribute("x1", right1[0].toString());
      rightLine.setAttribute("y1", right1[1].toString());
      rightLine.setAttribute("x2", right2[0].toString());
      rightLine.setAttribute("y2", right2[1].toString());
      rightLine.setAttribute("stroke", color);
      rightLine.setAttribute("stroke-width", "2");
      rightLine.setAttribute("stroke-dasharray", dasharray);
      g.appendChild(rightLine);
    }

    function drawCross() {
      drawCrossRoad();
      drawCrossLine();
      //右侧道路停止线
      drawStopLine();
      //路标
      drawRoadSign();
    }

    //画交叉口
    function drawCrossRoad() {
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
            var Q = getQByPathCurv(firstpt, pt, modelRef.curvature);
            d_str += `Q ${Q} ${firstpt[0]}, ${firstpt[1]} `;
          }
        } else {
          //第偶数个点为相邻道路的点，用曲线连接
          var prevPt = states.cross_pts[i - 1];
          var Q = getQByPathCurv(prevPt, pt, modelRef.curvature);
          d_str += `Q ${Q} ${pt[0]}, ${pt[1]} `;
        }
      }
      cross.setAttribute("d", d_str);
      cross.setAttribute("id", "cross");
      // cross.setAttribute("fill", "rgba(162,162,0,0.5)");
      cross.setAttribute("fill", "rgb(162,162,162)");
      states.cvs?.appendChild(cross);
    }

    //路口弧线
    function drawCrossLine() {
      //先清空交叉路口
      document.querySelectorAll("path").forEach((e) => {
        if (e.id.indexOf("cross_line") > -1) e.remove();
      });
      var corss_d_str = "";
      for (var i = 0; i < states.cross_line_pts.length; i++) {
        var pt = states.cross_line_pts[i];
        if (i == 0) {
          //第一个点跳过
        } else if (i % 2 !== 0) {
          corss_d_str = `M ${pt[0]},${pt[1]} `;
          if (i === states.cross_line_pts.length - 1) {
            //最后一个点需要用曲线连接第一个点
            var firstpt = states.cross_line_pts[0];
            var Q = getQByPathCurv(firstpt, pt, modelRef.curvature);
            corss_d_str += `Q ${Q} ${firstpt[0]}, ${firstpt[1]} `;
          }
        } else {
          //交叉弧线
          var prevPt = states.cross_line_pts[i - 1];
          var Q = getQByPathCurv(prevPt, pt, modelRef.curvature);
          corss_d_str = `M ${prevPt[0]},${prevPt[1]} Q ${Q} ${pt[0]},${pt[1]} `;
        }
        //交叉路口弧线
        if (corss_d_str) {
          var cross_line = document.createElementNS(states.ns, "path"); // 创建SVG元素——交叉口
          cross_line.setAttribute("d", corss_d_str);
          cross_line.setAttribute("id", `cross_line_${i}`);
          cross_line.setAttribute("stroke", "#ffffff");
          cross_line.setAttribute("stroke-width", "2");
          cross_line.setAttribute("fill", "none");
          states.cvs?.appendChild(cross_line);
        }
      }
    }

    //画右侧停止线
    function drawStopLine() {
      //先清空交叉路口
      document.querySelectorAll("path").forEach((e) => {
        if (e.id.indexOf("stop_line") > -1) e.remove();
      });
      for (var i = 0; i < states.cross_line_pts.length; i++) {
        var pt = states.cross_line_pts[i];
        if (i > 0 && i % 2 !== 0) {
          var prevPt = states.cross_line_pts[i - 1];
          //取中点
          var middlePt = [(prevPt[0] + pt[0]) / 2, (prevPt[1] + pt[1]) / 2];
          var stopLine = document.createElementNS(states.ns, "line");
          stopLine.setAttribute("id", `stop_line_${i}`);
          stopLine.setAttribute("x1", pt[0].toString());
          stopLine.setAttribute("y1", pt[1].toString());
          stopLine.setAttribute("x2", middlePt[0].toString());
          stopLine.setAttribute("y2", middlePt[1].toString());
          stopLine.setAttribute("stroke", "#FFFFFF");
          stopLine.setAttribute("stroke-width", "2");
          states.cvs?.appendChild(stopLine);
        }
      }
    }

    //画路标
    function drawRoadSign() {
      let road = 0;
      for (var i = 0; i < states.road_sign_pts.length; i++) {
        var pt = states.cross_line_pts[i];
        if (i > 0 && i % 2 !== 0) {
          console.log(states.angleSet, i);
          var prevPt = states.cross_line_pts[i - 1];
          //几条道路（默认双向六条）
          for (let way_idx = 0; way_idx < 6; way_idx++) {
            //左侧道路、右侧道路离中心距离微调            
            var k = way_idx >= 3 ? way_idx + 0.01 : way_idx * 0.9;
            //(x1+k(x2-x1)/n,y1+k(y2-y1)/n)线段n等分公式
            var wayPt = [prevPt[0] + k * (pt[0] - prevPt[0]) / 6, prevPt[1] + k * (pt[1] - prevPt[1]) / 6];
            var path = document.createElementNS(states.ns, "path");
            path.setAttribute("id", `road_sign_${i}`);
            path.setAttribute("d", getRoadDefaultSign(way_idx));
            path.setAttribute("fill", "#ffffff");
            path.setAttribute("width", "100");
            path.setAttribute("height", "100");
            path.addEventListener("click", handleChoose, false);

            var svg = document.createElementNS(states.ns, "svg");
            svg.appendChild(path);
            svg.setAttribute("viewBox", "0 0 1024 1024");
            svg.setAttribute("height", "40");
            svg.setAttribute("width", "18");
            svg.setAttribute("x", wayPt[0].toString());
            svg.setAttribute("y", wayPt[1].toString());
            var g = document.createElementNS(states.ns, "g");
            g.setAttribute(
              "transform",
              `rotate(${270 - states.angleSet[road]} ${wayPt[0]} ${wayPt[1]
              })`
            );
            g.appendChild(svg);
            states.cvs?.appendChild(g);
          }
          road++;
        }
      }
    }

    //选中道路
    function onRoad(e: any) {
      states.currentRoad = e.path[0];
      modelRef.direction = e.path[0].id;
    }

    //选中路标
    function handleChoose(e: any) {
      console.log(e.path[0]);
      states.currentSign = e.path[0];
      states.modalVisible = true;
    }

    function handleConfirmSign(rowKey: string) {
      console.log(rowKey);
      states.currentSign.setAttribute(
        "d",
        roadSigns.find((s) => s.key === rowKey)?.path
      );
      states.modalVisible = false;
    }

    onMounted(() => {
      initRoads();
    });

    return {
      ...toRefs(states),
      modelRef,
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      roadDir,
      roadSigns,
      drawCross,
      handleConfirmSign,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
