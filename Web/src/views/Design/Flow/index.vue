<template>
  <div class="basic-main">
    <div class="func">功能区</div>
    <!-- 图示 -->
    <svg id="canvas">
      <!-- 箭头 -->
      <defs v-for="(item, index) in road_lines" :key="item">
        <marker
          :id="'arrow' + index"
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
            :style="'fill: ' + item.color"
          />
        </marker>
      </defs>
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
                    v-model:value="road_info.flow_info.color"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option
                      v-for="(_, index) in angleSet"
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
                    v-model:value="road_info.flow_info.font_weight"
                    :min="0"
                    :max="50"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="长度1">
                  <a-input-number
                    v-model:value="road_info.flow_info.width1"
                    :min="20"
                    :max="150"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="字号1">
                  <a-input-number
                    v-model:value="road_info.flow_info.font_size1"
                    :min="0"
                    :max="30"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="长度2">
                  <a-input-number
                    v-model:value="road_info.flow_info.width2"
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
                    v-model:value="road_info.flow_info.font_size2"
                    :min="0"
                    :max="30"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="间距">
                  <a-input-number
                    v-model:value="road_info.flow_info.space"
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
                    v-model:value="road_info.flow_info.font_size3"
                    :min="0"
                    :max="30"
                    :step="1"
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
          <a-table
            :dataSource="road_info.flow_info.line_info"
            :columns="lineColumns"
            :pagination="false"
            :bordered="true"
            size="small"
          >
            <!-- 路名 -->
            <template #road_name="{ record }">
              <a-input
                v-model:value="record.road_name"
                size="small"
                class="form-width"
              />
            </template>
            <!-- 大车比率 -->
            <template #truck_ratio="{ record }">
              <a-input-number
                v-model:value="record.truck_ratio"
                :min="0"
                :max="100"
                :step="1"
                size="small"
                class="small-form-width"
              />
            </template>
            <!-- 高峰小时系数 -->
            <template #peak_period="{ record }">
              <a-input-number
                v-model:value="record.peak_period"
                :min="0"
                :max="1"
                :step="0.01"
                size="small"
                class="small-form-width"
              />
            </template>
          </a-table>
        </div>
        <div class="header mt-5">进口道转向流量</div>
        <div class="content mt-2">
          <a-table
            :dataSource="road_info.flow_info.flow_detail"
            :columns="flowColumns"
            :pagination="false"
            :showHeader="false"
            :bordered="true"
            size="small"
          >
            <template
              v-for="col in flowDataIndex"
              #[col]="{ record }"
              :key="col"
            >
              <a-input-number
                v-model:value="record[col]"
                :min="0"
                :step="10"
                size="small"
                class="small-form-width"
                @blur="onChangeFlow"
              />
            </template>
          </a-table>
        </div>
        <div class="header mt-5">进口车道饱和流量（从内侧车道开始）</div>
        <div class="content mt-5">
          <a-form
            :label-col="labelCol"
            :wrapper-col="wrapperCol"
            layout="horizontal"
          >
            <a-row>
              <a-col
                :span="8"
                v-for="(rec, index) in road_info.flow_info.saturation"
                :key="rec"
              >
                <a-form-item :label="'车道' + (index + 1)">
                  <a-input-number
                    v-model:value="road_info.flow_info.saturation[index]"
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
  </div>
</template>

<script lang="ts">
import { defineComponent, inject, onMounted, reactive, toRefs } from "vue";
import Container from "../../../components/Container/index.vue";
import { notification } from "ant-design-vue";
import {
  getAngle,
  getMiddlePoint,
  getPoint,
  getQByPathCurv,
  getRandomColor,
} from "../../../utils/common";
import {
  getCurvByAngle,
  lineColumns,
  flowColumnsPart,
  lineInfoModel,
  flowDataIndex,
} from ".";
import _ from "lodash";
import { RoadInfo } from "..";

export default defineComponent({
  components: { Container },
  setup() {
    const roadDir = inject("RoadDir") as any[];
    const flowColumns = reactive([] as any[]);
    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      angleSet: [] as number[], //所有道路倾斜角，以此绘制
      cx: 350, //圆心x
      cy: 350, //圆心y
      curvature: 0.04, //右转道路曲率
      road_r_k: 30, //每条车道宽度
      start_pts1: [] as any[], //所有路口交叉点外侧一层
      start_pts2: [] as any[], //所有路口交叉点内侧一层
      road_lines: [] as any[],
    });
    //全局道路信息
    const road_info = inject("road_info") as RoadInfo;
    //参数设置
    road_info.flow_info = reactive({
      color: 1, //颜色
      font_weight: 5, //粗细
      width1: 50, //长度1
      font_size1: 14, //字号1
      width2: 50, //长度2
      font_size2: 14, //字号2
      space: 40, //间距
      font_size3: 17, //字号3
      line_info: [] as any[], //车道属性
      flow_detail: [] as any[], //进口道转向流量
      saturation: [1650, 1650, 1650],
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
      for (var i = 0; i < states.angleSet.length; i++) {
        var angle = states.angleSet[i];
        drawRoadSign(angle, i);
      }
      //画路
      drawMainRoad();
    }

    //车辆标识
    function drawRoadSign(angle: number, i: number) {
      var road_r = states.road_r_k * states.angleSet.length;
      var angle = states.angleSet[i];
      var radian = (Math.PI / 180) * angle; // 角度转弧度
      var x2 = Math.cos(radian) * 250 + 350; // 外层200
      var y2 = -Math.sin(radian) * 250 + 350;
      var x3 = Math.cos(radian) * 200 + 350; // 内层150
      var y3 = -Math.sin(radian) * 200 + 350;
      var color = getRandomColor(); //颜色
      var right_line = []; //左边点
      var left_line = []; //右边点
      var center_text = []; //中心点
      const midCount = parseInt((states.angleSet.length / 2).toString());
      //道路左侧
      const pt_fl1 = getPoint("fl", angle, x3, y3, road_r);
      const pt_fl2 = getPoint("fl", angle, x2, y2, road_r);
      left_line.push(pt_fl1);
      left_line.push(pt_fl2);
      for (
        let roadIdx = -midCount;
        roadIdx < states.angleSet.length - midCount - 1;
        roadIdx++
      ) {
        //道路右侧
        let right_id = i.toString() + (roadIdx + midCount);
        const line = document.createElementNS(states.ns, "path");
        const insideWidth = road_r - roadIdx * 35;
        const outSideWidth = road_r - (roadIdx + 1) * 35;
        const pt_fr1 = getPoint("fr", angle, x2, y2, insideWidth);
        const pt_fr2 = getPoint("fr", angle, x2, y2, outSideWidth);
        const pt_fr3 = getPoint("fr", angle, x3, y3, outSideWidth);
        const pt_fr4 = getPoint("fr", angle, x3, y3, insideWidth);
        var d_str = `M ${pt_fr1[0]} ${pt_fr1[1]} L ${pt_fr2[0]} ${pt_fr2[1]} L ${pt_fr3[0]} ${pt_fr3[1]} L ${pt_fr4[0]} ${pt_fr4[1]} Z`;
        drawPath(line, "road_text" + right_id, d_str, color, "1");
        //将线段中点保存至数组
        const middlePoint = getMiddlePoint(pt_fr3, pt_fr4);
        right_line.push(middlePoint);
        //将远端中点也保存，用来写数字
        const middlePoint2 = getMiddlePoint(pt_fr1, pt_fr2);
        center_text.push([middlePoint, middlePoint2]);
      }
      const road_lines = { right_line, left_line, color, center_text };
      states.road_lines.push(road_lines);
    }

    //画主路路径
    function drawMainRoad() {
      let hasDraw = [];
      for (let i = 0; i < states.angleSet.length; i++) {
        const road1 = states.road_lines[i]; //第一条路
        //右侧道路
        for (let j = i; j < states.angleSet.length + i; j++) {
          let road2_j = j;
          if (j > states.angleSet.length - 1) {
            road2_j = j - states.angleSet.length;
          }
          const angle1 = states.angleSet[i];
          const angle2 = states.angleSet[road2_j];
          const road2 = states.road_lines[road2_j]; //连接第二条路
          if (i != road2_j) {
            for (let k = 0; k < road1.right_line.length; k++) {
              const ij = i + "" + road2_j;
              const ik = i + "" + k;
              if (
                hasDraw.filter((h) => h.ij === ij || h.ik === ik).length > 0
              ) {
                continue;
              }
              hasDraw.push({ ij, ik });
              const point1 = road1.right_line[k];
              const point2 = road2.left_line[0];
              const line = document.createElementNS(states.ns, "path");
              const curvature = getCurvByAngle(
                states.curvature,
                angle1,
                angle2,
                point1,
                point2
              );
              const Q = getQByPathCurv(point1, point2, curvature);
              const d_str = `M ${point1[0]} ${point1[1]} Q ${Q} ${point2[0]} ${point2[1]}`;
              drawPath(line, "right_road" + ij, d_str, road1.color);
            }
          }
        }
        //左侧箭头
        const line = document.createElementNS(states.ns, "path");
        const d_str = `M ${road1.left_line[0][0]} ${road1.left_line[0][1]} L ${road1.left_line[1][0]} ${road1.left_line[1][1]}`;
        const width = (states.angleSet.length * 3).toString();
        drawPath(line, "left_road" + i, d_str, road1.color, width, true, i);
      }
    }

    //画路面上的线
    function drawPath(
      line: Element,
      id: string,
      d_str: string,
      color: string,
      width = "5",
      has_arrow = false,
      id_index = 0
    ) {
      line.setAttribute("id", id);
      line.setAttribute("d", d_str);
      line.setAttribute("stroke", color);
      line.setAttribute("stroke-width", width);
      line.setAttribute("fill", `none`);
      if (has_arrow) {
        line.setAttribute("marker-end", `url(#arrow${id_index})`);
      }
      states.cvs?.appendChild(line);
    }

    //填充表格
    function initRoadInfo() {
      Object.assign(flowColumns, flowColumnsPart);
      for (let i = 1; i < states.road_lines.length; i++) {
        let dataIndex = "turn" + (i + 1);
        flowColumns.push({
          title: "转向" + (i + 1),
          dataIndex: dataIndex,
          width: 30,
          slots: { customRender: dataIndex },
        });
        flowDataIndex.push(dataIndex);
      }

      for (let i = 1; i <= states.road_lines.length; i++) {
        let line_info = _.cloneDeep(lineInfoModel);
        line_info.direction = "方向" + i;
        line_info.road_name = "方向" + i;
        road_info.flow_info.line_info.push(line_info);

        let flow_info = {} as any;
        flow_info.road_name = "方向" + i;
        for (let j = 1; j < states.road_lines.length; j++) {
          flow_info["turn1"] = 0;
          flow_info["turn" + (j + 1)] = 450;
        }
        road_info.flow_info.flow_detail.push(flow_info);
      }
      //drawText 写路面上的数字
      drawText();
    }

    //写数字
    function drawText() {
      states.road_lines.forEach((r, i) => {
        r.center_text.forEach((midPoint: any, j: number) => {
          const point = getMiddlePoint(midPoint[0], midPoint[1]);
          const text = document.createElementNS(states.ns, "text");
          let content = road_info.flow_info.flow_detail[i]["turn" + (j + 2)];
          let x = (point[0] - (content.length === 3 ? 10 : 15)).toString();
          let y = (point[1] + 5).toString();
          text.setAttribute("id", "turn" + i + "" + j);
          text.setAttribute("x", x);
          text.setAttribute("y", y);
          text.setAttribute("fill", "#000");
          text.setAttribute(
            "transform",
            `rotate(${360 - states.angleSet[i]} ${point[0]},${point[1]})`
          );
          text.appendChild(document.createTextNode(content)); //文本内容"450"
          states.cvs?.appendChild(text);
        });
      });
    }

    function onChangeFlow() {
      //先清空交叉路口
      document.querySelectorAll("text").forEach((e) => {
        if (e.id.indexOf("turn") > -1) e.remove();
      });
      drawText();
    }

    onMounted(() => {
      initRoads();
      initRoadInfo();
    });

    return {
      ...toRefs(states),
      road_info,
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      lineColumns,
      flowColumns,
      flowDataIndex,
      onChangeFlow,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
