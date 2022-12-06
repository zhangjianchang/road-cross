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
                    v-model:value="flow_info.color"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option
                      v-for="(_, index) in road_attr"
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
                    v-model:value="flow_info.font_weight"
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
                    v-model:value="flow_info.width1"
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
                    v-model:value="flow_info.font_size1"
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
                    v-model:value="flow_info.width2"
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
                    v-model:value="flow_info.font_size2"
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
                    v-model:value="flow_info.space"
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
                    v-model:value="flow_info.font_size3"
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
            :dataSource="flow_info.line_info"
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
            :dataSource="flow_info.flow_detail"
            :columns="flow_info.flowColumns"
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
              <div>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 700 700"
                  class="road-sign"
                >
                  <defs>
                    <marker
                      :id="'arrow'"
                      markerUnits="strokeWidth"
                      markerWidth="3"
                      markerHeight="3"
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
                  <path
                    :id="'direction'"
                    :d="record[col].d"
                    fill="none"
                    stroke="#4f48ad"
                    stroke-width="100"
                    :marker-end="'url(#arrow)'"
                  ></path>
                </svg>
              </div>
              <a-input-number
                v-model:value="record[col].number"
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
                v-for="(rec, index) in flow_info.saturation"
                :key="rec"
              >
                <a-form-item :label="'车道' + (index + 1)">
                  <a-input-number
                    v-model:value="flow_info.saturation[index]"
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
import { defineComponent, onMounted, reactive, toRefs } from "vue";
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
import { road_info } from "..";

export default defineComponent({
  components: { Container },
  setup() {
    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      curvature: 0.04, //右转道路曲率
      road_r_k: 30, //每条车道宽度
      start_pts1: [] as any[], //所有路口交叉点外侧一层
      start_pts2: [] as any[], //所有路口交叉点内侧一层
      road_lines: [] as any[],
      sign_pts: [] as any[],
    });

    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      render();
    };

    function render() {
      for (var i = 0; i < road_info.road_attr.length; i++) {
        var angle = road_info.road_attr[i].angle;
        drawRoadSign(angle, i);
      }
      //画路
      drawMainRoad();
    }

    //车辆标识
    function drawRoadSign(angle: number, i: number) {
      var road_r = states.road_r_k * road_info.road_attr.length;
      var radian = (Math.PI / 180) * angle; // 角度转弧度
      var x2 = Math.cos(radian) * 250 + 350; // 外层200
      var y2 = -Math.sin(radian) * 250 + 350;
      var x3 = Math.cos(radian) * 150 + 350; // 内层150
      var y3 = -Math.sin(radian) * 150 + 350;
      var color = getRandomColor(); //颜色
      var right_line = []; //左边点
      var left_line = []; //右边点
      //道路左侧
      const pt_fl1 = getPoint("fl", angle, x3, y3, road_r);
      const pt_fl2 = getPoint("fl", angle, x2, y2, road_r);
      left_line.push(pt_fl1);
      left_line.push(pt_fl2);
      //道路右侧
      const pt_fr1 = getPoint("fr", angle, x3, y3, road_r);
      const pt_fr2 = getPoint("fr", angle, x2, y2, road_r);
      right_line.push(pt_fr1);
      right_line.push(pt_fr2);
      const road_line = { right_line, left_line, color };
      states.road_lines.push(road_line);
    }

    function drawMainRoad() {
      for (let i = 0; i < road_info.road_attr.length; i++) {
        const road1 = states.road_lines[i]; //第一条路
        //右侧道路
        for (let j = i; j < road_info.road_attr.length + i; j++) {
          let road2_index = j;
          if (j > road_info.road_attr.length - 1) {
            road2_index = j - road_info.road_attr.length;
          }
          const road2 = states.road_lines[road2_index]; //连接第二条路          const curvature = getCurvByAngle(
          const angle1 = road_info.road_attr[i].angle;
          const angle2 = road_info.road_attr[road2_index].angle;
          const point11 = road1.right_line[0];
          const point12 = road1.right_line[1];
          const point21 = road2.left_line[0];
          const point22 = road2.left_line[1];
          const curvature = getCurvByAngle(
            states.curvature,
            angle1,
            angle2,
            point11,
            point21
          );
          const Q = getQByPathCurv(point11, point21, curvature);
          const d_str = `M${point12[0]} ${point12[1]} L ${point11[0]} ${point11[1]} Q ${Q} ${point21[0]} ${point21[1]} L ${point22[0]} ${point22[1]}`;
          const line = document.createElementNS(states.ns, "path");
          drawPath(line, "right_road", d_str, road1.color);
        }
      }
    }
    //画主路路径
    function drawMainRoadCopy() {
      let hasDraw = [];
      for (let i = 0; i < road_info.road_attr.length; i++) {
        const road1 = states.road_lines[i]; //第一条路
        //右侧道路
        for (let j = i; j < road_info.road_attr.length + i; j++) {
          let road2_j = j;
          if (j > road_info.road_attr.length - 1) {
            road2_j = j - road_info.road_attr.length;
          }
          const angle1 = road_info.road_attr[i].angle;
          const angle2 = road_info.road_attr[road2_j].angle;
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
        const width = (road_info.road_attr.length * 3).toString();
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
      //道路有更换的时候重新绘制
      if (road_info.road_attr.length !== road_info.basic_info.count) {
        initFlowDetail();
      }
      drawText();
      initDirections();
      //标记当前道路数量
      road_info.basic_info.count = road_info.road_attr.length;
    }

    function initFlowDetail() {
      Object.assign(road_info.flow_info.flowColumns, flowColumnsPart);
      for (let i = 0; i < road_info.road_attr.length; i++) {
        let dataIndex = "turn" + i;
        road_info.flow_info.flowColumns.push({
          title: "转向" + (i + 1),
          dataIndex: dataIndex,
          width: 30,
          slots: { customRender: dataIndex },
        });
        flowDataIndex.push(dataIndex);
      }
      //先清空
      road_info.flow_info.line_info.length = 0;
      road_info.flow_info.flow_detail.length = 0;
      for (let i = 0; i < road_info.road_attr.length; i++) {
        let line_info = _.cloneDeep(lineInfoModel);
        line_info.direction = "方向" + (i + 1);
        line_info.road_name = "方向" + (i + 1);
        road_info.flow_info.line_info.push(line_info);

        let flow_detail = {} as any;
        flow_detail.road_name = "方向" + (i + 1);
        flow_detail["turn0"] = { number: 0 };
        for (let j = 1; j < states.road_lines.length; j++) {
          flow_detail["turn" + j] = { number: 450 };
        }
        road_info.flow_info.flow_detail.push(flow_detail);
      }
    }

    //写数字
    function drawText() {
      states.road_lines.forEach((r, i) => {
        r.center_text.forEach((midPoint: any, j: number) => {
          const point = getMiddlePoint(midPoint[0], midPoint[1]);
          const text = document.createElementNS(states.ns, "text");
          let content =
            road_info.flow_info.flow_detail[i]["turn" + (j + 1)].number;
          let x = (point[0] - (content.length === 3 ? 10 : 15)).toString();
          let y = (point[1] + 5).toString();
          text.setAttribute("id", "turn" + i + "" + j);
          text.setAttribute("x", x);
          text.setAttribute("y", y);
          text.setAttribute("fill", "#000");
          text.setAttribute(
            "transform",
            `rotate(${360 - road_info.road_attr[i].angle} ${point[0]},${
              point[1]
            })`
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

    //加载各道路之间的方向
    const initDirections = () => {
      const roadCount = road_info.road_attr.length;
      for (let i = 0; i < roadCount; i++) {
        const sign_pt = [] as any[];
        for (let j = 0; j < roadCount; j++) {
          const road = road_info.road_attr[i];
          const nextRoad = road_info.road_attr[j];
          const d = `M${road.coordinate[0]} ${road.coordinate[1]} L${states.cx} ${states.cy} L${nextRoad.coordinate[0]} ${nextRoad.coordinate[1]}`;
          const order = j - i < 0 ? j - i + roadCount : j - i;
          const path = { d, order };
          sign_pt.push(path);
        }
        sign_pt.sort(function (a, b) {
          return a.order - b.order;
        });
        states.sign_pts.push(sign_pt);
      }
      road_info.flow_info.flow_detail.map((item, index) => {
        for (let i = 0; i < roadCount; i++) {
          item["turn" + i].d = states.sign_pts[index][i].d;
        }
      });
    };

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

    onMounted(() => {
      initRoads();
      // initRoadInfo();
    });

    return {
      ...toRefs(states),
      ...toRefs(road_info),
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      lineColumns,
      flowDataIndex,
      onChangeFlow,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
