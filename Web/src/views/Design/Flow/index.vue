<template>
  <div class="basic-main">
    <div class="func">
      <!-- 功能区 -->
    </div>
    <!-- 图示 -->
    <svg id="canvas">
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
            id="direction_arrow"
            xmlns="http://www.w3.org/2000/svg"
            d="M2,2 L10,6 L2,10 L2,6 L2,2"
            style="fill: #4f48ad"
          />
        </marker>
      </defs>
      <!-- 箭头 -->
      <defs v-for="(item, index) in road_lines" :key="item">
        <marker
          :id="'arrow' + index"
          markerUnits="strokeWidth"
          :markerWidth="3"
          :markerHeight="3"
          viewBox="0 0 12 12"
          refX="6"
          refY="6"
          orient="auto"
        >
          <path
            xmlns="http://www.w3.org/2000/svg"
            d="M2,2 L10,6 L2,10 L2,6 L2,2"
            :style="'fill: ' + item.color"
            deleteTag="1"
          />
        </marker>
      </defs>
      <!-- 文字路径 -->
      <!-- <defs>
        <path
          v -for="text in road_texts"
          : key="text.id"
          : id="text.id"
          : d="text.path"
        ></path>
      </defs> -->
    </svg>
    <!-- 参数 -->
    <div class="menu" v-if="!code_info || can_edit">
      <div class="form">
        <a-collapse v-model:activeKey="activeKey">
          <a-collapse-panel key="1" header="绘图属性">
            <div>
              <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
                <a-row>
                  <a-col :span="8">
                    <a-form-item label="颜色">
                      <a-select
                        v-model:value="flow_info.colorScheme"
                        size="small"
                        class="form-width"
                        @change="onChangeFlow(undefined)"
                      >
                        <a-select-option
                          v-for="index in [0, 1, 2]"
                          :key="index"
                          :value="index"
                        >
                          方案{{ index + 1 }}
                        </a-select-option>
                      </a-select>
                    </a-form-item>
                  </a-col>
                  <a-col :span="8">
                    <a-form-item label="粗细">
                      <a-input-number
                        v-model:value="flow_info.thickness"
                        :min="1"
                        :max="8"
                        size="small"
                        class="form-width"
                        @change="onChangeFlow(undefined)"
                      />
                    </a-form-item>
                  </a-col>
                  <a-col :span="8">
                    <a-form-item label="长度">
                      <a-input-number
                        v-model:value="flow_info.width"
                        :min="50"
                        :max="120"
                        :step="1"
                        size="small"
                        class="form-width"
                        @change="onChangeFlow(undefined)"
                      />
                    </a-form-item>
                  </a-col>
                  <a-col :span="8">
                    <a-form-item label="间距">
                      <a-input-number
                        v-model:value="flow_info.space"
                        :min="16"
                        :max="40"
                        :step="1"
                        size="small"
                        class="form-width"
                        @change="onChangeFlow(undefined)"
                      />
                    </a-form-item>
                  </a-col>
                  <a-col :span="8">
                    <a-form-item label="字号1">
                      <a-input-number
                        v-model:value="flow_info.font_size1"
                        :min="10"
                        :max="18"
                        :step="2"
                        size="small"
                        class="form-width"
                        @change="onChangeFlow(undefined)"
                      />
                    </a-form-item>
                  </a-col>
                  <a-col :span="8">
                    <a-form-item label="字号2">
                      <a-input-number
                        v-model:value="flow_info.font_size2"
                        :min="14"
                        :max="18"
                        :step="2"
                        size="small"
                        class="form-width"
                        @change="onChangeFlow(undefined)"
                      />
                    </a-form-item>
                  </a-col>
                </a-row>
              </a-form>
            </div>
          </a-collapse-panel>
          <a-collapse-panel key="2" header="车道属性">
            <div v-if="is_flow_init">
              <a-table
                :dataSource="flow_info.line_info"
                :columns="lineColumns"
                :pagination="false"
                :bordered="true"
                :customRow="onRowClick"
                :scroll="{ x: '100%' }"
                :rowClassName="
              (_: any, index: number) => (index === currentRoad ? 'click-row' : null)
            "
                size="small"
              >
                <!-- 路名 -->
                <template #road_name="{ index }">
                  <a-input
                    v-model:value="canalize_info[index].name"
                    size="small"
                    class="form-width"
                    @blur="onChangeFlow(undefined)"
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
                    class="form-width"
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
                    class="form-width"
                  />
                </template>
              </a-table>
            </div>
          </a-collapse-panel>
          <a-collapse-panel key="3" header="进口道转向流量">
            <div v-if="is_flow_init">
              <a-table
                :dataSource="flow_info.flow_detail"
                :columns="flow_info.flowColumns"
                :pagination="false"
                :showHeader="false"
                :bordered="true"
                :customRow="onRowClick"
                :rowClassName="
              (_: any, index: number) => (index === currentRoad ? 'click-row' : null)
            "
                size="small"
              >
                <!-- 路名 -->
                <template #road_name="{ index }">
                  {{ canalize_info[index].name }}
                </template>
                <!-- 转向 -->
                <template
                  v-for="col in flowDataIndex"
                  #[col]="{ index }"
                  :key="col"
                >
                  <div v-if="flow_info.flow_detail[index].turn">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 700 700"
                      class="road-sign"
                    >
                      <path
                        :id="'direction'"
                        :d="flow_info.flow_detail[index].turn[Number(col)].d"
                        fill="none"
                        stroke="#4f48ad"
                        stroke-width="100"
                        :marker-end="'url(#arrow)'"
                      ></path>
                    </svg>
                    <div
                      :class="
                        currentEditText ===
                        `${flow_info.flow_detail[index].turn[Number(col)].tag}`
                          ? 'form-actitve'
                          : ''
                      "
                    >
                      <a-input-number
                        v-model:value="
                          flow_info.flow_detail[index].turn[Number(col)].number
                        "
                        :min="0"
                        :max="5000"
                        :step="10"
                        size="small"
                        class="small-form-width"
                        @blur="onChangeFlow(undefined)"
                        @focus="
                          onFocusFlow(
                            flow_info.flow_detail[index].turn[Number(col)].tag
                          )
                        "
                      />
                    </div>
                  </div>
                </template>
              </a-table>
            </div>
          </a-collapse-panel>
          <a-collapse-panel key="4" header="进口车道饱和流量（从内侧车道开始）">
            <div>
              <a-form
                :label-col="labelCol"
                :wrapper-col="wrapperCol"
                layout="horizontal"
              >
                <a-row>
                  <a-col
                    :span="8"
                    v-for="(rec, index) in flow_info.saturation[currentRoad]"
                    :key="rec"
                  >
                    <a-form-item :label="'车道' + (index + 1)">
                      <a-input-number
                        v-model:value="rec.number"
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
          </a-collapse-panel>
        </a-collapse>
      </div>
    </div>
    <div class="menu" v-else>
      <div style="padding-top: 320px; text-align: center">
        <a-button type="primary" @click="handleEdit"> 编辑 </a-button>
      </div>
    </div>
    <a-modal
      v-if="is_flow_init"
      v-model:visible="visible"
      :title="'修改进口道转向流量'"
      width="400px"
      centered
      ok-text="确认"
      cancel-text="取消"
      @ok="onNumberConfirm"
    >
      <div>
        <a-input-number
          v-model:value="updateNumber"
          :min="0"
          :max="5000"
          :step="1"
          placeholder="请输入对应进口道转向流量"
          class="modal-input-width"
        />
      </div>
    </a-modal>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive, toRefs } from "vue";
import Container from "../../../components/Container/index.vue";
import {
  getMiddlePoint,
  getPoint,
  getQByPathCurv,
  intersect_line_point,
} from "../../../utils/common";
import {
  lineColumns,
  flowDataIndex,
  getLineWidth,
  getK,
  colorSchemes,
} from ".";
import _ from "lodash";
import {
  create_flow_detail,
  handleEdit,
  plans,
  roadStates,
  update_flow_detail,
} from "..";
import { road_model } from "../data";
import { message } from "ant-design-vue";

export default defineComponent({
  components: { Container },
  setup() {
    //道路信息
    const road_info = reactive(JSON.parse(JSON.stringify(road_model)));

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      r: 350, //半径
      curvature: 0.04, //右转道路曲率
      road_r_k: 24, //每条车道宽度
      start_pts1: [] as any[], //所有路口交叉点外侧一层
      start_pts2: [] as any[], //所有路口交叉点内侧一层
      road_lines: [] as any[], //所有路径信息
      road_texts: [] as any[], //所有文字信息
      sign_pts: [] as any[],
      line_width: 8,
      is_init: false,
      activeKey: ["1", "2", "3", "4"], //默认展开面板
      currentRoad: 0, //当前选中道路
      currentEditText: "", //当前正在编辑的文字
      visible: false, //弹出框可见性
      updateNumber: "", //修改的时间
    });

    const initRoads = (rf: any) => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      onChangeFlow(rf); //渲染
    };

    //页面属性变动
    const onChangeFlow = async (rf: any) => {
      if (!rf) {
        rf =
          plans.canalize_plans[roadStates.current_canalize].flow_plans[
            roadStates.current_flow
          ].signal_plans[0].road_info;
      }
      await setRoadFlow(rf); //设置关键数据
      Object.assign(road_info, rf);

      //默认值
      if (!road_info.flow_info.thickness) {
        road_info.flow_info.thickness = 5;
      }
      if (!road_info.flow_info.width) {
        road_info.flow_info.width = 100;
      }
      if (!road_info.flow_info.space) {
        road_info.flow_info.space = 24;
      }
      if (!road_info.flow_info.font_size1) {
        road_info.flow_info.font_size1 = 14;
      }
      if (!road_info.flow_info.font_size2) {
        road_info.flow_info.font_size2 = 16;
      }
      clearRoadPath();
      render();
    };

    async function setRoadFlow(rf: any) {
      //道路数据填充
      await initRoadInfo(rf);
    }

    //填充表格
    async function initRoadInfo(rf: any) {
      if (
        !roadStates.is_flow_init ||
        plans.road_count !== rf.flow_info.line_info.length
      ) {
        //初始化时加载,道路数变化时重新加载
        create_flow_detail(rf);
      } else {
        //只变动了方向
        update_flow_detail(rf);
      }
      roadStates.is_flow_init = true; //标记已经初始化过了
    }

    function render() {
      //设置节点数据
      setRoadPoint();
      //画路
      drawMainRoad();
    }

    //设置道路关键节点
    function setRoadPoint() {
      states.road_lines.length = 0;
      states.road_texts.length = 0;
      let roadCount = road_info.road_attr.length;
      for (var i = 0; i < roadCount; i++) {
        var angle = road_info.road_attr[i].angle;
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        var width = 250 - road_info.flow_info.width;
        var x2 = Math.cos(radian) * 250 + states.r; // 外层200
        var y2 = -Math.sin(radian) * 250 + states.r;
        var x3 = Math.cos(radian) * width + states.r; // 内层150
        var y3 = -Math.sin(radian) * width + states.r;
        var x4 = Math.cos(radian) * (width - 1) + states.r; // 内层149
        var y4 = -Math.sin(radian) * (width - 1) + states.r;
        var color = colorSchemes[road_info.flow_info.colorScheme][i]; //颜色
        var right_line = []; //左边点
        var left_line = []; //右边点
        for (var j = 0; j < roadCount; j++) {
          const road_r =
            road_info.flow_info.space * roadCount - j * states.line_width;
          //道路左侧
          const l_lines = [];
          const pt_fl1 = getPoint("fl", angle, x3, y3, road_r);
          const pt_fl2 = getPoint("fl", angle, x2, y2, road_r);
          l_lines.push(pt_fl1);
          l_lines.push(pt_fl2);
          left_line.push(l_lines);
          //道路右侧
          const r_lines = [];
          const pt_fr1 = getPoint("fr", angle, x3, y3, road_r);
          const pt_fr2 = getPoint("fr", angle, x2, y2, road_r);
          r_lines.push(pt_fr1);
          r_lines.push(pt_fr2);
          right_line.push(r_lines);
        }
        var ml_line = [];
        var mr_line = [];
        const k = getK(roadCount);
        //道路左侧
        let flowCount = getFlowCountL(i);
        const road_ml =
          road_info.flow_info.space * roadCount -
          states.line_width * k +
          (roadCount - flowCount) * 2.2;
        const pt_ml1 = getPoint("fl", angle, x4, y4, road_ml);
        const pt_ml2 = getPoint("fl", angle, x2, y2, road_ml);
        ml_line.push(pt_ml1, pt_ml2);
        //道路中心右侧
        flowCount = getFlowCountR(i);
        const road_mr =
          road_info.flow_info.space * roadCount -
          states.line_width * k +
          (roadCount - flowCount) * 2.2;
        const pt_mr1 = getPoint("fr", angle, x4, y4, road_mr);
        const pt_mr2 = getPoint("fr", angle, x2, y2, road_mr);
        mr_line.push(pt_mr1, pt_mr2);
        const middle_line = { ml_line, mr_line }; //中心点
        const road_line = { right_line, left_line, middle_line, color };
        states.road_lines.push(road_line);
      }
    }

    //绘制道路
    function drawMainRoad() {
      drawMRRoad();
      drawRoadCross();
      drawMLRoad();
      setTimeout(() => drawText(), 30);
    }

    //绘制道路左右两侧(右侧)
    function drawMRRoad() {
      const width = road_info.flow_info.thickness;
      for (let i = 0; i < states.road_lines.length; i++) {
        const flow_count = getFlowCountR(i);
        const line_width = getLineWidth(
          road_info.road_attr.length,
          flow_count,
          width
        );
        const road = states.road_lines[i];
        //道路左侧

        let content = 0;
        road_info.flow_info.flow_detail[i].turn.map((t: any) => {
          content += t.number;
        });
        const point_r1 = road.middle_line.mr_line[0];
        const point_r2 = road.middle_line.mr_line[1];
        const rightLine = document.createElementNS(states.ns, "path");
        const d_str = `M ${point_r1[0]} ${point_r1[1]} L ${point_r2[0]} ${point_r2[1]}`;
        if (content !== 0) {
          drawPath(rightLine, "road_r", d_str, road.color, "none", line_width);
        }
        //取同一侧道路中点用来标记入口车道总数
        let midPoint = getMiddlePoint(point_r1, point_r2);
        let angle = 360 - road_info.road_attr[i].angle;
        if (content !== 0) {
          setText(i, midPoint, angle, content, "rightMRoad");
        }
        //取两侧道路中点标记道路
        const point_l1 = road.middle_line.ml_line[1];
        midPoint = getMiddlePoint(point_l1, point_r1);
        angle = 270 - road_info.road_attr[i].angle;
        content = road_info.canalize_info[i].name;
        setText(i, midPoint, angle, content, "roadName", -15, 70);
      }
    }
    //绘制道路左右两侧(左侧)
    function drawMLRoad() {
      const width = road_info.flow_info.thickness;
      for (let i = 0; i < states.road_lines.length; i++) {
        let flow_count = getFlowCountL(i);
        let line_width = getLineWidth(
          road_info.road_attr.length,
          flow_count,
          width
        );
        const road = states.road_lines[i];
        //道路左侧
        let content = 0;
        road_info.flow_info.flow_detail.map((f: any) => {
          content += f.turn.find(
            (t: any) => t.tag.indexOf(`#${i}`) > -1
          ).number;
        });
        const point_l1 = road.middle_line.ml_line[0];
        const point_l2 = road.middle_line.ml_line[1];
        const leftLine = document.createElementNS(states.ns, "path");
        const d_str = `M ${point_l1[0]} ${point_l1[1]} L ${point_l2[0]} ${point_l2[1]}`;
        if (content !== 0) {
          drawPath(
            leftLine,
            "road_l",
            d_str,
            road.color,
            "none",
            line_width,
            true,
            i
          );
          //取两线中点用来标记数字
          const midPoint = getMiddlePoint(point_l1, point_l2);
          let angle = 360 - road_info.road_attr[i].angle;
          setText(i, midPoint, angle, content, "leftMRoad");
        }
      }
    }
    //绘制交叉路
    function drawRoadCross() {
      for (let i = 0; i < states.road_lines.length; i++) {
        let k = 0; //标记第二条路的索引
        const road1 = states.road_lines[i]; //第一条路
        for (let j = i + 1; j <= states.road_lines.length + i; j++) {
          let road2_index = j;
          if (j > road_info.road_attr.length - 1) {
            road2_index = j - road_info.road_attr.length;
          }
          //流量为0跳过绘制
          const number = road_info.flow_info.flow_detail[i].turn.find(
            (t: any) => t.tag.indexOf(`#${road2_index}`) > -1
          ).number;
          if (number === 0) {
            continue;
          }
          const road2 = states.road_lines[road2_index]; //连接第二条路
          const angle = 360 - road_info.road_attr[i].angle;

          const point1 = road1.right_line[k][0];
          const point2 = road2.left_line[k][0];

          var insect_pt = intersect_line_point(
            { x: road1.right_line[k][0][0], y: road1.right_line[k][0][1] },
            { x: road1.right_line[k][1][0], y: road1.right_line[k][1][1] },
            { x: road2.left_line[k][0][0], y: road2.left_line[k][0][1] },
            { x: road2.left_line[k][1][0], y: road2.left_line[k][1][1] }
          ); // 相邻两直线的交点
          var mid_pt = {
            x: (road1.right_line[k][0][0] + road2.left_line[k][0][0]) * 0.5,
            y: (road1.right_line[k][0][1] + road2.left_line[k][0][1]) * 0.5,
          };
          let Q = "";
          // 曲度插值法计算Q的中间点
          if (insect_pt && i != road2_index) {
            var qpt = {
              x: mid_pt.x + (insect_pt.x - mid_pt.x) * 1,
              y: mid_pt.y + (insect_pt.y - mid_pt.y) * 1,
            };
            Q = qpt.x + "," + qpt.y + " ";
          } else {
            //掉头车道
            Q = getQByPathCurv(point1, point2, 2.5);
          }

          let d_str = `M ${point1[0]} ${point1[1]} Q ${Q} ${point2[0]} ${point2[1]}`;
          const line = document.createElementNS(states.ns, "path");
          drawPath(line, `main_path_${i}_${k}`, d_str, road1.color);
          //路径文字
          const tag = `${i}#${road2_index}`;
          const content = road_info.flow_info.flow_detail[i].turn.find(
            (t: any) => t.tag === tag
          ).number;
          setText(tag, point1, angle, content, "rightRoad", -30, 7 * k);
          k++;
        }
      }
    }

    //画路面上的线
    function drawPath(
      line: Element,
      id: string,
      d_str: string,
      color: string,
      fill = "none",
      thickness = road_info.flow_info.thickness,
      has_arrow = false,
      id_index = 0
    ) {
      line.setAttribute("id", id);
      line.setAttribute("d", d_str);
      line.setAttribute("stroke", color);
      line.setAttribute("stroke-width", thickness.toString());
      line.setAttribute("fill", fill);
      line.setAttribute("deleteTag", "1");
      if (has_arrow) {
        line.setAttribute("marker-end", `url(#arrow${id_index})`);
      }
      states.cvs?.appendChild(line);
    }

    function drawRect(has_arrow = false) {
      const rect = document.createElementNS(states.ns, "path");
      rect.setAttribute("id", "xxx");
      rect.setAttribute("x", "398");
      rect.setAttribute("y", "100");
      rect.setAttribute("width", "16");
      rect.setAttribute("height", "100");
      rect.setAttribute("stroke-width", "5");
      rect.setAttribute("fill", `blue`);
      if (has_arrow) {
        rect.setAttribute("marker-end", `url(#arrow${0})`);
      }
      states.cvs?.appendChild(rect);
    }

    //数字写入数组
    function setText(
      i: any,
      point: any,
      angle: any,
      content: any,
      type: string,
      translateX = -10,
      translateY = 20
    ) {
      const road_text = {
        id: `${type}_${i}`,
        point: point,
        angle: angle,
        content: content,
        type,
        translateX,
        translateY,
      };
      states.road_texts.push(road_text);
    }
    //写数字
    function drawText() {
      states.road_texts.forEach((t) => {
        const text = document.createElementNS(states.ns, "text");
        text.setAttribute("id", t.id);
        text.setAttribute("x", t.point[0]);
        text.setAttribute("y", t.point[1]);
        text.setAttribute("fill", "#000");
        // 主路径字体取字体1
        if (t.type === "roadName") {
          text.setAttribute(
            "style",
            `font-size:${road_info.flow_info.font_size2}`
          );
        } else if (t.type === "rightRoad") {
          text.setAttribute(
            "style",
            `font-size:${road_info.flow_info.font_size1};cursor:pointer`
          );
          text.addEventListener(
            "click",
            () => onClickText(t.id, t.content),
            false
          );
        } else {
          text.setAttribute(
            "style",
            `font-size:${road_info.flow_info.font_size1};`
          );
        }
        text.setAttribute(
          "transform",
          `rotate(${t.angle} ${t.point[0]},${t.point[1]})
          translate(${t.translateX},${t.translateY})`
        );
        text.appendChild(document.createTextNode(t.content)); //文本内容"450"
        states.cvs?.appendChild(text);
      });
    }

    //点击数字
    const onClickText = (id: string, number: string) => {
      const tag = id.split("_")[1];
      states.currentRoad = Number(tag.split("#")[0]);
      states.updateNumber = number;
      states.visible = true;
      onFocusFlow(tag);
    };

    //确认修改数字
    const onNumberConfirm = () => {
      if (!states.updateNumber) {
        message.warning("请填写进口道转向流量");
        return;
      }
      const ct = road_info.flow_info.flow_detail[states.currentRoad].turn.find(
        (t: any) => t.tag === states.currentEditText
      );
      ct.number = states.updateNumber;
      onChangeFlow(road_info);
      states.visible = false;
    };

    //流量框聚焦
    const onFocusFlow = (tag: string) => {
      states.currentEditText = tag;
    };

    //右侧道路数（流量大于0）
    function getFlowCountR(index: number) {
      return road_info.flow_info.flow_detail[index].turn.filter(
        (t: any) => t.number > 0
      ).length;
    }

    //左侧道路数（流量大于0）
    function getFlowCountL(index: number) {
      let flow_count = 0;
      road_info.flow_info.flow_detail.map((f: any) => {
        if (
          f.turn.find((t: any) => t.tag.indexOf(`#${index}`) > -1).number > 0
        ) {
          flow_count++;
        }
      });
      return flow_count;
    }
    //清空道路svg
    const clearRoadPath = () => {
      document.querySelectorAll("path").forEach((e) => {
        const is_delete = e.getAttribute("deleteTag") === "1";
        if (["direction", "direction_arrow"].indexOf(e.id) === -1 && is_delete)
          e.remove();
      });
      document.querySelectorAll("text").forEach((e) => {
        e.remove();
      });
    };

    function drawPoint(x: number, y: number, color: string) {
      var g = document.createElementNS(states.ns, "g");
      g.setAttribute("stroke", color);
      g.setAttribute("stroke-width", "1");
      g.setAttribute("fill", "black");
      var circle = document.createElementNS(states.ns, "circle");
      circle.setAttribute("cx", x.toString());
      circle.setAttribute("cy", y.toString());
      circle.setAttribute("r", "1");
      g.appendChild(circle);
      states.cvs?.appendChild(g);
    }

    //表格行点击事件
    const onRowClick = (record: any, index: number) => {
      return {
        onClick: () => {
          states.currentRoad = index;
        },
      };
    };

    onMounted(() => {
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans[0].road_info;
      initRoads(rf);
    });

    return {
      ...toRefs(states),
      ...toRefs(road_info),
      ...toRefs(roadStates),
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      lineColumns,
      flowDataIndex,
      onChangeFlow,
      onRowClick,
      onFocusFlow,
      onNumberConfirm,
      handleEdit,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
