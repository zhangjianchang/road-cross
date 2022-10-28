<template>
  <div class="basic-main">
    <div class="func">功能区</div>
    <!-- 图示 -->
    <svg id="canvas">
      <!-- 箭头 -->
      <defs>
        <marker
          id="arrow"
          markerUnits="strokeWidth"
          :markerWidth="5"
          :markerHeight="5"
          viewBox="0 0 12 12"
          refX="6"
          refY="6"
          orient="auto"
        >
          <path
            xmlns="http://www.w3.org/2000/svg"
            d="M2,2 L10,6 L2,10 L2,6 L2,2"
            style="fill: #4f48ad"
          />
        </marker>
      </defs>
      <text v-for="(_, index) in roadDir" :key="index" x="430">
        <textPath :xlink:href="'#road_' + (index + 1)" y="50">
          方向{{ index + 1 }}
        </textPath>
      </text>
    </svg>
    <!-- 参数 -->
    <div class="menu">
      <div class="form">
        <div class="header">绘图属性</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="颜色">
                  <a-select
                    v-model:value="formModel.direction"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option
                      v-for="(_, index) in roadDir"
                      :key="(index + 1).toString()"
                      :value="'road_' + (index + 1)"
                    >
                      方案{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="粗细">
                  <a-input-number
                    v-model:value="formModel.size"
                    :min="0"
                    :max="10"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="长度1">
                  <a-input-number
                    v-model:value="formModel.curvature"
                    @change="onChangeCurv"
                    :min="0"
                    :max="1"
                    :step="0.01"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="字号1">
                  <a-input-number
                    v-model:value="formModel.curvature"
                    @change="onChangeCurv"
                    :min="20"
                    :max="150"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="长度2">
                  <a-input-number
                    v-model:value="formModel.curvature"
                    @change="onChangeCurv"
                    :min="20"
                    :max="150"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="字号2">
                  <a-input-number
                    v-model:value="formModel.curvature"
                    @change="onChangeCurv"
                    :min="20"
                    :max="150"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="间距">
                  <a-input-number
                    v-model:value="formModel.curvature"
                    @change="onChangeCurv"
                    :min="20"
                    :max="150"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="字号3">
                  <a-input-number
                    v-model:value="formModel.curvature"
                    @change="onChangeCurv"
                    :min="20"
                    :max="150"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="header mt-2">车道属性</div>
        <div class="content mt-2">
          <a-table :dataSource="dataSource" :columns="roadColumns" />
        </div>
        <div class="header">进口道转向流量</div>
        <div class="content mt-2">
          <div v-for="(item1, i) in angleSet" :key="item1" class="flow-info">
            <div class="flow-info-title">方向{{ i + 1 }}</div>
            <div
              class="flow-info-content"
              v-for="item2 in angleSet"
              :key="item2"
            >
              <div class="arrow">箭头偏移角度{{ item2 }}</div>
              <div>
                <a-input-number
                  :min="0"
                  :step="50"
                  size="small"
                  class="form-width"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="header">进口车道饱和流量（从内侧车道开始）</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="8">
                <a-form-item label="车道1">
                  <a-input-number
                    v-model:value="formModel.chedao1"
                    :min="0"
                    :step="50"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="8">
                <a-form-item label="车道2">
                  <a-input-number
                    v-model:value="formModel.chedao2"
                    :min="0"
                    :step="50"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="8">
                <a-form-item label="车道3">
                  <a-input-number
                    v-model:value="formModel.chedao3"
                    :min="0"
                    :step="50"
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
          v-for="road in roadSigns.filter((s) => !s.hide)"
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
import { getRoadDefaultSign, roadSigns } from "../Canalize/index";
import Container from "../../../components/Container/index.vue";
import { notification } from "ant-design-vue";
import { DragOutlined } from "@ant-design/icons-vue";
import { getAngle, getQByPathCurv } from "../../../utils/common";
import { roadColumns } from ".";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    const roadDir = inject("RoadDir") as any[];

    const states = reactive({
      isInit: true,
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      road_r_k: 15.4, //每条车道宽度
      angleSet: [] as number[], //所有道路倾斜角，以此绘制
      start_pts1: [] as any[], //所有路口交叉点
      start_pts2: [] as any[], //所有路口交叉点内侧一层
      currentRoad: {} as any, //当前选中道路
      currentSign: {} as any, //当前选中路标
      modalVisible: false, //车道弹窗
      defaultCount: 3, //初始化每条进口/出口3个车道
      wayCount: [] as number[], //每条路上的车道数量
    });

    //参数设置
    const formModel = reactive({
      direction: 1, //方向
      size: 5, //交叉路口大小
      curvature: 0.07, //右转道路曲率
      roadAttr: [] as any[], //道路属性
      entranceAttr: [] as any[], //进口属性
      exitAttr: [] as any[], //出口属性
    });

    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      roadDir.map((r, index) => {
        let angle = getAngle(
          states.cx,
          states.cy,
          r.coordinate[0],
          r.coordinate[1]
        );
        states.angleSet.push(angle);
        //初始化6车道//0.5为中间双黄线
        if (states.isInit) {
          formModel.entranceAttr.push({ wayCount: states.defaultCount });
          formModel.exitAttr.push({ wayCount: states.defaultCount });
        }
        states.wayCount[index] =
          formModel.entranceAttr[index].wayCount +
          formModel.exitAttr[index].wayCount +
          0.5;
      });
      states.angleSet.sort(function (a, b) {
        return a - b;
      });
      render();
      //标记已经初始化过了
      states.isInit = false;
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
      for (var i = 0; i < states.angleSet.length; i++) {
        //
        var road_r = states.road_r_k * states.wayCount[i];
        // var cross_r = states.road_r_k * states.wayCount[i] - 10;
        // var zebra_r = states.road_r_k * states.wayCount[i] - 15;
        var angle = states.angleSet[i];
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        // 基线起（圆心）止（圆上）点
        var x2 = Math.cos(radian) * 300 + 350; // 大圆半径300
        var y2 = -Math.sin(radian) * 300 + 350;
        var x3 = Math.cos(radian) * road_r * 2 + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * road_r * 2 + 350;
        //画路
        drawRoad(i, angle, x2, y2);
        //画路标
        // drawRoadSign(angle, i);
      }
    }

    //direction：方向，nr 近右,nl 近左, fr 远右, fl 远左 //road_width默认100（小于100即画路边线时用到）
    function getPoint(
      direction: string,
      angle: number,
      x: number,
      y: number,
      road_width: number
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
      road_width: number
    ) {
      var point = getPoint("nr", angle, x, y, road_width);
      pts.push(point);
      point = getPoint("nl", angle, x, y, road_width);
      pts.push(point);
    }

    //画路
    function drawRoad(index: number, angle: number, x: number, y: number) {
      //画主路
      drawMainRoad(angle, x, y, index);
    }

    function drawRoadSign(angle: number, i: number) {
      var road_r = states.road_r_k * states.wayCount[i];
      var angle = states.angleSet[i];
      var radian = (Math.PI / 180) * angle; // 角度转弧度
      // 基线起（圆心）止（圆上）点
      var x2 = Math.cos(radian) * 300 + 350; // 大圆半径300
      var y2 = -Math.sin(radian) * 300 + 350;
      var x3 = Math.cos(radian) * 200 + 350; //内层
      var y3 = -Math.sin(radian) * 200 + 350;

      var signCount =
        states.angleSet.length % 2 === 0
          ? states.angleSet.length / 2 + 1
          : states.angleSet.length / 2;
      //道路左侧
      for (let roadIdx = 0; roadIdx < signCount; roadIdx++) {
        //颜色
        const color = `rgb(${Math.random() * 256},${Math.random() * 256},${
          Math.random() * 256
        })`;
        const line = document.createElementNS(states.ns, "path");
        const pt_fr1 = getPoint("fr", angle, x2, y2, road_r + roadIdx * 30);
        const pt_fr2 = getPoint("fr", angle, x3, y3, road_r + roadIdx * 30);
        const pt_fr3 = getPoint("fr", angle, x2, y2, road_r + (roadIdx+1) * 30);
        const pt_fr4 = getPoint("fr", angle, x3, y3, road_r + (roadIdx+1) * 30);
        var d_str = `M ${pt_fr1[0]} ${pt_fr1[1]} L ${pt_fr2[1]} ${pt_fr2[0]} L ${pt_fr3[0]} ${pt_fr3[1]} L ${pt_fr4[1]} ${pt_fr4[0]}`;
        drawPath(line, d_str, color, "2", false);

        const pt_nr1 = getPoint("fr", angle, x2, y2, road_r - roadIdx * 30);
        const pt_nr2 = getPoint("fr", angle, x3, y3, road_r - roadIdx * 30);
        const pt_nr3 = getPoint("fr", angle, x3, y3, road_r - (roadIdx+1) * 30);
        const pt_nr4 = getPoint("fr", angle, x3, y3, road_r - (roadIdx+1) * 30);
        var d_str = `M ${pt_nr1[0]} ${pt_nr1[1]} L ${pt_nr2[1]} ${pt_nr2[0]} L ${pt_nr3[0]} ${pt_nr3[1]} L ${pt_nr4[1]} ${pt_nr4[0]}`;
        drawPath(line, d_str, color, "2", false);

        drawPoint(pt_fr1[0], pt_fr1[1], "#ddd");
        drawPoint(pt_fr2[0], pt_fr2[1], "#ddd");
        drawPoint(pt_nr1[0], pt_nr1[1], "#ddd");
        drawPoint(pt_nr2[0], pt_nr2[1], "#ddd");
      }
      //道路右侧
      const pt_fl1 = getPoint("fl", angle, x2, y2, road_r);
      const pt_fl2 = getPoint("fl", angle, x3, y3, road_r);
      drawPoint(pt_fl1[0], pt_fl1[1], "#000");
      drawPoint(pt_fl2[0], pt_fl2[1], "#000");
    }

    function drawPoint(x: number, y: number, color: string) {
      var g = document.createElementNS(states.ns, "g");
      g.setAttribute("stroke", color);
      g.setAttribute("stroke-width", "3");
      g.setAttribute("fill", "black");

      var circle = document.createElementNS(states.ns, "circle");
      circle.setAttribute("cx", x.toString());
      circle.setAttribute("cy", y.toString());
      circle.setAttribute("r", "3");
      g.appendChild(circle);

      states.cvs?.appendChild(g);
    }

    //画主路
    function drawMainRoad(angle: number, x: number, y: number, index: number) {
      const road_width = 100;
      // 起始点
      const point1 = getPoint("fr", angle, x, y, road_width);
      //颜色
      const color = `rgb(${Math.random() * 256},${Math.random() * 256},${
        Math.random() * 256
      })`;
      states.angleSet.forEach((angle2, i) => {
        if (i != index) {
          // 画线
          const line = document.createElementNS(states.ns, "path");
          // 终点
          const radian = (Math.PI / 180) * angle2;
          const x2 = Math.cos(radian) * 300 + 350;
          const y2 = -Math.sin(radian) * 300 + 350;
          const point2 = getPoint("fl", angle2, x2, y2, road_width);
          //求起始点旋转角度
          const curvature =
            formModel.curvature * (180 - Math.abs(angle2 - angle));
          const Q = getQByPathCurv(point1, point2, curvature);
          const d_str = `M ${point1[0]} ${point1[1]} Q ${Q} ${point2[0]} ${point2[1]}`;
          drawPath(line, d_str, color);
        }
      });
    }

    //画路面上的线
    function drawPath(
      line: Element,
      d_str: string,
      color: string,
      width = "5",
      has_arrow = true
    ) {
      line.setAttribute("id", "path");
      line.setAttribute("d", d_str);
      line.setAttribute("stroke", color);
      line.setAttribute("stroke-width", width);
      line.setAttribute("fill", `none`);
      if (has_arrow) {
        line.setAttribute("marker-end", `url(#arrow)`);
      }
      states.cvs?.appendChild(line);
    }

    function handleConfirmSign(rowKey: string) {
      states.currentSign.setAttribute(
        "d",
        roadSigns.find((s) => s.key === rowKey)?.path
      );
      states.modalVisible = false;
    }

    function onChangeRoadCount() {
      initRoads();
    }

    function onChangeCurv() {
      //先清空交叉路口
      document.querySelectorAll("path").forEach((e) => {
        if (e.id.indexOf("line") > -1) e.remove();
      });
      render();
    }

    onMounted(() => {
      initRoads();
    });

    return {
      ...toRefs(states),
      formModel,
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      roadColumns,
      roadDir,
      roadSigns,
      handleConfirmSign,
      onChangeRoadCount,
      onChangeCurv,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
