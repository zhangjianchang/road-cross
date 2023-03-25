<template>
  <div class="basic-main">
    <!-- 图示 -->
    <svg id="canvas">
      <defs>
        <linearGradient id="rect_green" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" :stop-color="signalColor.green[0]" />
          <stop offset="50%" :stop-color="signalColor.green[1]" />
          <stop offset="100%" :stop-color="signalColor.green[0]" />
        </linearGradient>
        <linearGradient id="rect_yellow" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" :stop-color="signalColor.yellow[0]" />
          <stop offset="50%" :stop-color="signalColor.yellow[1]" />
          <stop offset="100%" :stop-color="signalColor.yellow[0]" />
        </linearGradient>
        <linearGradient id="rect_red" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" :stop-color="signalColor.red[0]" />
          <stop offset="50%" :stop-color="signalColor.red[1]" />
          <stop offset="100%" :stop-color="signalColor.red[0]" />
        </linearGradient>
      </defs>
      <!-- 相位 -->
      <g
        v-for="(road, index) in road_pts"
        :key="road.g"
        :id="'phase_' + index"
        :transform="road.g.transform"
        @click="onGClick(index)"
        style="cursor: pointer"
      >
        <!-- 外层矩形轮廓 -->
        <rect width="700" height="700" fill="#FFFF99" />
        <!-- 道路路径 -->
        <path :d="road.path.d" :id="road.path.id" fill="rgb(162,162,162)" />
        <!-- 相位数据 -->
        <text x="120" y="800" class="phase-text">
          {{ road.text }}
        </text>
        <!-- 相位内部方向箭头 -->
        <defs>
          <marker
            id="direction_line"
            markerUnits="strokeWidth"
            markerWidth="6"
            markerHeight="6"
            viewBox="0 0 12 12"
            refX="6"
            refY="6"
            orient="auto"
          >
            <path
              xmlns="http://www.w3.org/2000/svg"
              d="M2,2 L10,6 L2,10 L2,6 L2,2"
              style="fill: #fff"
            />
          </marker>
        </defs>
      </g>
      <!-- 图例 -->
      <g v-if="signal_info.is_show_legend">
        <!-- 外层边框 -->
        <rect
          x="35"
          y="170"
          height="40"
          width="630"
          rx="4"
          ry="4"
          fill="rgba(0,0,0,0.3)"
        />
        <!-- 机动车文字 -->
        <text x="75" y="195" class="legend-text">机动车</text>
        <!-- 机动车图示 -->
        <rect x="120" y="185" height="12" width="70" fill="#FFFFFF" />
        <!-- 非机动车文字 -->
        <text x="240" y="195" class="legend-text">非机动车</text>
        <!-- 非机动车图示 -->
        <rect x="298" y="185" height="12" width="70" fill="#FFB90F" />
        <!-- 行人文字 -->
        <text x="410" y="195" class="legend-text">行人</text>
        <!-- 行人图示 -->
        <rect x="443" y="185" height="12" width="70" fill="#00FF99" />
        <!-- 待转文字 -->
        <text x="548" y="195" class="legend-text">待转</text>
        <!-- 待转图示 -->
        <line
          x1="581"
          y1="192"
          x2="631"
          y2="192"
          style="stroke: #ffffff; stroke-width: 12"
          stroke-dasharray="5,5"
        />
      </g>
    </svg>
    <!-- 参数 -->
    <div class="menu">
      <div class="form">
        <a-collapse v-model:activeKey="activeKey">
          <a-collapse-panel key="1" header="基础信息">
            <div>
              <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
                <a-row>
                  <a-col :span="7">
                    <a-form-item label="相位总数">
                      <a-select
                        v-model:value="signal_info.phase"
                        size="small"
                        class="form-width"
                        @change="onPhaseChange"
                      >
                        <a-select-option
                          v-for="item in [1, 2, 3, 4, 5]"
                          :key="item"
                          :value="item"
                        >
                          {{ item }}
                        </a-select-option>
                      </a-select>
                    </a-form-item>
                  </a-col>
                  <a-col :span="10">
                    <a-form-item label="周期">
                      <a-input-number
                        v-model:value="signal_info.period"
                        size="small"
                        class="form-width"
                        :disabled="true"
                      />
                      <div class="span-unit">秒</div>
                    </a-form-item>
                  </a-col>
                  <a-col :span="7">
                    <a-form-item label="图例">
                      <a-select
                        v-model:value="signal_info.is_show_legend"
                        @change="onShowLegendChange"
                        size="small"
                        class="form-width"
                      >
                        <a-select-option :value="true"> 是 </a-select-option>
                        <a-select-option :value="false"> 否 </a-select-option>
                      </a-select>
                    </a-form-item>
                  </a-col>
                </a-row>
              </a-form>
            </div>
          </a-collapse-panel>
          <a-collapse-panel key="2" header="相位信息">
            <div>
              <a-table
                :dataSource="signal_info.phase_list"
                :columns="phaseColumns"
                :scroll="{ x: 485 }"
                :customRow="onRowClick"
                :pagination="false"
                :bordered="true"
                :rowClassName="
              (_: any, index: number) => (index === currentPhase ? 'click-row' : null)
            "
                size="small"
              >
                <!-- 相位index -->
                <template #index="{ text }">
                  {{ text + 1 }}
                </template>
                <!-- 名称 -->
                <template #name="{ record }">
                  <a-input
                    v-model:value="record.name"
                    size="small"
                    class="form-width"
                    @blur="onItemPeriodBlur"
                  />
                </template>
                <!-- 绿灯 -->
                <template #green="{ record }">
                  <a-input-number
                    v-model:value="record.green"
                    :min="0"
                    :max="100"
                    :step="1"
                    @blur="onItemPeriodBlur"
                    size="small"
                    class="small-form-width"
                  />
                </template>
                <!-- 黄灯 -->
                <template #yellow="{ record }">
                  <a-input-number
                    v-model:value="record.yellow"
                    :min="0"
                    :max="100"
                    :step="1"
                    @blur="onItemPeriodBlur"
                    size="small"
                    class="small-form-width"
                  />
                </template>
                <!-- 全红 -->
                <template #red="{ record }">
                  <a-input-number
                    v-model:value="record.red"
                    :min="0"
                    :max="100"
                    :step="1"
                    @blur="onItemPeriodBlur"
                    size="small"
                    class="small-form-width"
                  />
                </template>
                <!-- 搭接相位 -->
                <template #is_lap="{ record }">
                  <a-select
                    v-model:value="record.is_lap"
                    size="small"
                    class="middle-form-width"
                  >
                    <a-select-option :value="true"> 是 </a-select-option>
                    <a-select-option :value="false"> 否 </a-select-option>
                  </a-select>
                </template>
              </a-table>
            </div>
          </a-collapse-panel>
          <a-collapse-panel key="3" header="进口设置">
            <div v-if="signal_info.phase_list.length > 0">
              <a-form>
                <a-form-item label="进口方向">
                  <a-select
                    v-model:value="currentDirection"
                    size="small"
                    class="form-width"
                    @change="onDirectionChange"
                  >
                    <a-select-option
                      v-for="(item, index) in road_attr"
                      :key="item"
                      :value="index"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-form>
            </div>
            <a-divider />
            <div v-if="signal_info.phase_list.length > 0">
              <div>机动车</div>
              <div>
                <svg
                  v-for="(item, index) in sign_pts[currentDirection]"
                  :key="index"
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 700 700"
                  class="road-sign"
                  @click="onDirectionClick(index)"
                >
                  <defs>
                    <marker
                      :id="
                        'arrow' +
                        currentPhase +
                        '_' +
                        currentDirection +
                        '_' +
                        index
                      "
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
                        style="fill: #a2a2a2"
                      />
                    </marker>
                  </defs>
                  <path
                    :id="
                      'direction' +
                      currentPhase +
                      '_' +
                      currentDirection +
                      '_' +
                      index
                    "
                    :d="item.d"
                    fill="none"
                    stroke="#a2a2a2"
                    stroke-width="100"
                    :marker-end="
                      'url(#arrow' +
                      currentPhase +
                      '_' +
                      currentDirection +
                      '_' +
                      index +
                      ')'
                    "
                  ></path>
                </svg>
              </div>
              <!-- <div>非机动车</div>
          <div>
            <svg
              v-for="(item, index) in sign_pts[currentDirection]"
              :key="index"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 700 700"
              class="road-sign"
              @click="onDirectionClick(index)"
            >
              <defs>
                <marker
                  id="arrow"
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
                    style="fill: #a2a2a2"
                  />
                </marker>
              </defs>
              <path
                :d="item.d"
                fill="none"
                stroke="#a2a2a2"
                stroke-width="100"
                marker-end="url(#arrow)"
              ></path>
            </svg>
          </div>
          <div>行人</div>
          <div>1</div>
          <div>待转</div>
          <div>2</div> -->
            </div>
          </a-collapse-panel>
        </a-collapse>
      </div>
    </div>
    <a-modal
      v-if="signal_info.phase_list.length > 0"
      v-model:visible="visible"
      :title="
        '修改【' +
        signal_info.phase_list[currentPhase].name +
        '】的' +
        currentTimeTypeName +
        '时间'
      "
      width="400px"
      centered
      ok-text="确认"
      cancel-text="取消"
      @ok="onTimeConfirm"
    >
      <div>
        <a-input-number
          v-model:value="updateTime"
          :min="0"
          :max="100"
          :step="1"
          placeholder="请输入修改的时间"
          class="modal-input-width"
        />
      </div>
    </a-modal>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive, toRefs } from "vue";
import Container from "../../../components/Container/index.vue";
import { DragOutlined } from "@ant-design/icons-vue";
import { getQByPathCurv } from "../../../utils/common";
import { signalColor, getStartX, phaseColumns } from ".";
import _ from "lodash";
import { getCurvByAngle } from "../Flow";
import { road_model } from "../data";
import {
  create_signal_info,
  getTurnDetail_D,
  get_λ,
  insert_phase,
  plans,
  roadStates,
} from "..";
import { message } from "ant-design-vue";
import { openNotfication } from "../../../utils/message";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    //道路信息
    const road_info = reactive(JSON.parse(JSON.stringify(road_model)));

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      svg_width: 590, //画布宽度
      road_width: 160, //路宽
      phase_height: 80, //每个相位的间距
      curvature: 2, //路口弧度
      cross_pts: [] as any[], //所有路口交叉点
      road_pts: [] as any[], //道路缩略
      sign_pts: [] as any[], //路标（掉头右转之类）
      currentPhase: 0, //当前选中相位
      currentTimeType: "", //当前修改时间类型（green/yellow/red）
      currentTimeTypeName: "", //当前修改时间类型（绿灯/黄灯/红灯）
      currentDirection: 0, //当前选中方向（相位的子集）
      activeKey: ["1", "2", "3"], //默认展开全部面板
      visible: false, //弹出框可见性
      updateTime: "", //修改的时间
    });
    //加载道路
    const initRoads = (rf: any) => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      onChangeSignal(rf); //渲染
    };

    function initSignal() {
      if (road_info.signal_info.phase_list.length === 0) {
        create_signal_info(road_info);
      } else {
        setTimeout(() => {
          for (let i = 0; i < road_info.signal_info.phase_list.length; i++) {
            for (let j = 0; j < road_info.road_attr.length; j++) {
              states.currentPhase = i;
              states.currentDirection = j;
              setDirectionLine();
            }
          }
          states.currentPhase = 0;
          states.currentDirection = 0;
        }, 10);
      }
      initDirections();
    }

    function onChangeSignal(rf: any) {
      Object.assign(road_info, rf);
      initSignal(); //初始化数据
      render();
    }

    function render() {
      drawScale();
      drawMain();
    }

    //加载各道路之间的方向
    const initDirections = () => {
      const road_count = road_info.road_attr.length;
      for (let i = 0; i < road_count; i++) {
        const sign_pt = [];
        for (let m = 0; m < road_count; m++) {
          let j = i + m >= road_count ? i + m - road_count : i + m;
          const d = getTurnDetail_D(road_info, i, j);
          const path = { d };
          sign_pt.push(path);
        }
        states.sign_pts.push(sign_pt);
      }
    };

    //时长图
    function drawScale() {
      //先清空
      document.querySelectorAll("line").forEach((e) => {
        if (e.id.indexOf("line") > -1) e.remove();
      });
      document.querySelectorAll("rect").forEach((e) => {
        if (e.id.indexOf("rect") > -1) e.remove();
      });
      document.querySelectorAll("text").forEach((e) => {
        if (e.id.indexOf("text") > -1) e.remove();
      });
      const legendHeight = road_info.signal_info.is_show_legend ? 40 : 0;
      let start_x = 85; //x起始位置
      let top = 220 + legendHeight; //上边缘线
      let signal = 235 + legendHeight; //灯
      let center = 250 + legendHeight; //中心线
      let bottom = 280 + legendHeight; //下边缘线
      //默认四个相位
      for (let p = 0; p < road_info.signal_info.phase; p++) {
        let width = states.svg_width / road_info.signal_info.period; //每个刻度的宽度
        let x1, y1, x2, y2, w, h, time;
        for (let i = 0; i <= road_info.signal_info.period; i++) {
          let tick_len = 10; // 小刻度=10
          if (i % 5 == 0) tick_len = 20; // 长刻度=20
          /**上边缘线 */
          x1 = start_x + i * width;
          y1 = top + p * states.phase_height;
          x2 = start_x + i * width;
          y2 = top + p * states.phase_height + tick_len;
          createLine(x1, y1, x2, y2);
          if (p === 0 && i % 10 == 0) {
            const left_x = i != 0 ? (i < 100 ? 7 : 10) : 0;
            createText(x1 - left_x, y1 - 3, p, i.toString());
          }
          /**上边缘线 */
          /**下边缘线 */
          y1 = bottom + p * states.phase_height - tick_len;
          y2 = bottom + p * states.phase_height;
          createLine(x1, y1, x2, y2);
          /**下边缘线 */
        }
        /**外层上边缘线 */
        x1 = start_x;
        y1 = top + p * states.phase_height;
        x2 = start_x + states.svg_width;
        y2 = y1;
        createLine(x1, y1, x2, y2);
        /**外层上边缘线 */
        /**外层下边缘线 */
        x1 = start_x;
        y1 = bottom + p * states.phase_height;
        x2 = start_x + states.svg_width;
        y2 = y1;
        createLine(x1, y1, x2, y2);
        /**外层下边缘线 */
        /**中心线 */
        x1 = start_x;
        y1 = center + p * states.phase_height;
        x2 = start_x + states.svg_width;
        y2 = y1;
        createLine(x1, y1, x2, y2, "red", "3");
        /**中心线 */
        /**绿色信号 */
        x1 =
          start_x + getStartX(road_info.signal_info.phase_list, p, "g") * width;
        y1 = signal + p * states.phase_height;
        w =
          start_x +
          getStartX(road_info.signal_info.phase_list, p, "y") * width -
          x1;
        h = 30;
        createRect(p, x1, y1, w, h, "green");
        time = road_info.signal_info.phase_list[p].green;
        if (Number(time) > 0) {
          createText(x1 + w / 2 - 3, y1 + h / 2 + 3, p, time, "green", true);
        }
        /**绿色信号 */
        /**黄色信号 */
        x1 =
          start_x + getStartX(road_info.signal_info.phase_list, p, "y") * width;
        y1 = signal + p * states.phase_height;
        w =
          start_x +
          getStartX(road_info.signal_info.phase_list, p, "r") * width -
          x1;
        createRect(p, x1, y1, w, h, "yellow");
        time = road_info.signal_info.phase_list[p].yellow;
        if (Number(time) > 0) {
          createText(x1 + w / 2 - 3, y1 + h / 2 + 3, p, time, "yellow", true);
        } /**黄色信号 */
        /**红色信号 */
        x1 =
          start_x + getStartX(road_info.signal_info.phase_list, p, "r") * width;
        y1 = signal + p * states.phase_height;
        w =
          start_x +
          getStartX(road_info.signal_info.phase_list, p + 1, "g") * width -
          x1;
        createRect(p, x1, y1, w, h, "red");
        time = road_info.signal_info.phase_list[p].red;
        if (Number(time) > 0) {
          createText(x1 + w / 2 - 3, y1 + h / 2 + 3, p, time, "red", true);
        } /**红色信号 */
        /**文字 */
        //相位
        x1 = start_x - 60;
        y1 = signal + 10 + p * states.phase_height;
        createText(x1, y1, p, road_info.signal_info.phase_list[p].name);
        //绿信比
        y1 = signal + 30 + p * states.phase_height;
        let λ = get_λ(
          road_info.signal_info.phase_list[p].green,
          road_info.signal_info.phase_list[p].yellow,
          road_info.signal_info.period
        ).toFixed(2);
        createText(x1, y1, p, `λ：${λ}`);
        /**文字 */
      }
    }

    function createLine(
      x1: number,
      y1: number,
      x2: number,
      y2: number,
      stroke = "rgb(162,162,162)",
      stroke_width = "1"
    ) {
      let line = document.createElementNS(states.ns, "line");
      line.setAttribute("id", `line`);
      line.setAttribute("x1", x1.toString());
      line.setAttribute("y1", y1.toString());
      line.setAttribute("x2", x2.toString());
      line.setAttribute("y2", y2.toString());
      line.setAttribute("stroke", stroke);
      line.setAttribute("stroke-width", stroke_width);
      states.cvs?.appendChild(line);
    }

    function createRect(
      p: number,
      x: number,
      y: number,
      width: number,
      height: number,
      stroke_type: string
    ) {
      let rect = document.createElementNS(states.ns, "rect");
      rect.setAttribute("id", `rect`);
      rect.setAttribute("x", x.toString());
      rect.setAttribute("y", y.toString());
      rect.setAttribute("width", width.toString());
      rect.setAttribute("height", height.toString());
      rect.setAttribute("fill", `url(#rect_${stroke_type})`);
      rect.addEventListener("click", () => onClickRect(p), false);
      states.cvs?.appendChild(rect);
    }

    //点击红绿黄条
    const onClickRect = (p: number) => {
      states.currentPhase = p;
      onDirectionChange(states.currentDirection);
    };

    /**
     * 绘制文字
     * @param x 绘制文字x坐标位置
     * @param y 绘制文字y坐标位置
     * @param i 索引
     * @param content 内容
     * @param type green/yellow/red
     * @param is_time 是否为相位对应时间
     */
    function createText(
      x: number,
      y: number,
      i: number,
      content: string,
      type = "",
      is_time = false
    ) {
      let text = document.createElementNS(states.ns, "text");
      text.setAttribute("id", `text_${i}`);
      text.setAttribute("x", x.toString());
      text.setAttribute("y", y.toString());
      if (is_time) {
        text.setAttribute(
          "style",
          `font-size:12px;cursor:pointer;font-weight:800`
        );
        text.addEventListener(
          "click",
          () => onClickTime(i, type, content),
          false
        );
      } else {
        text.setAttribute("style", `font-size:12px`);
      }
      text.appendChild(document.createTextNode(content));
      states.cvs?.appendChild(text);
    }

    //点击时间
    const onClickTime = (p: number, type: string, number: string) => {
      states.currentPhase = p;
      states.currentTimeType = type;
      states.updateTime = number;
      states.currentTimeTypeName =
        type === "green" ? "绿灯" : type === "yellow" ? "黄灯" : "红灯";
      onDirectionChange(states.currentDirection);
      states.visible = true;
    };

    //确定修改时间
    const onTimeConfirm = () => {
      if (!states.updateTime) {
        message.warning("请填写时间");
        return;
      }
      road_info.signal_info.phase_list[states.currentPhase][
        states.currentTimeType
      ] = states.updateTime;
      onItemPeriodBlur();
      states.visible = false;
    };

    //画相位路径
    function drawMain() {
      states.cross_pts.length = 0;
      states.road_pts.length = 0;
      for (var a = 0; a < road_info.road_attr.length; a++) {
        var angle = road_info.road_attr[a].angle;
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        var x3 = Math.cos(radian) * states.road_width + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * states.road_width + 350;
        //获取交叉口圆plus和路边相交的点
        setPts(states.cross_pts, angle, x3, y3);
      }
      for (let p = 0; p < road_info.signal_info.phase; p++) {
        drawPhase(p);
      }
    }

    //direction：方向，nr 近右,nl 近左, fr 远右, fl 远左 //road_width默认100（小于100即画路边线时用到）
    function getPoint(
      direction: string,
      angle: number,
      x: number,
      y: number,
      road_width = states.road_width
    ) {
      var radian = (Math.PI / 180) * (angle - 90);
      var coefficient = direction === "nl" || direction === "fr" ? -1 : 1;
      var point_x = coefficient * road_width * 0.5 * Math.cos(radian) + x;
      var point_y = -coefficient * road_width * 0.5 * Math.sin(radian) + y;
      return [~~point_x, ~~point_y];
    }

    function setPts(pts: any[], angle: number, x: number, y: number) {
      var point = getPoint("nr", angle, x, y);
      pts.push(point);
      point = getPoint("nl", angle, x, y);
      pts.push(point);
    }

    function drawPhase(p: number) {
      //svg图像
      let d_str = "";
      let roadIdx = 0;
      let roadEdgePoints = [];
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
      //添加路左右两侧点
      for (let ag = 0; ag < road_info.road_attr.length; ag++) {
        const roadEdgePoint = setRoadEdgePoint(ag);
        roadEdgePoints.push(roadEdgePoint);
      }
      setPath(d_str, p, roadEdgePoints);
      refreshLocation();
    }

    const setRoadEdgePoint = (i: number) => {
      var angle = road_info.road_attr[i].angle;
      var radian = (Math.PI / 180) * angle;
      var x = Math.cos(radian) * 250 + 350;
      var y = -Math.sin(radian) * 250 + 350;
      const insideWidth = states.road_width - 100;
      const right_point = getPoint("fr", angle, x, y, insideWidth);
      const left_point = getPoint("fl", angle, x, y, insideWidth);
      return { right_point, left_point };
    };

    //写路径至数组
    function setPath(d_str: string, i: number, point: any[]) {
      const path = {
        id: "road_path",
        d: d_str,
      };
      const g = {
        transform: `translate(${130 * i + 20},10) scale(0.18)`,
      };
      const text = `${road_info.signal_info.phase_list[i].name}：${road_info.signal_info.phase_list[i].green}秒`;
      states.road_pts.push({ g, path, point, text });
    }

    //重新调整相位位置
    const refreshLocation = () => {
      const paddingLeft = 370 - road_info.signal_info.phase * 70;
      states.road_pts.map((r: any, index: number) => {
        r.g = {
          transform: `translate(${130 * index + paddingLeft},10) scale(0.18)`,
        };
        r.text = `${road_info.signal_info.phase_list[index].name}：${road_info.signal_info.phase_list[index].green}秒`;
      });
    };

    //相位总数变更
    const onPhaseChange = () => {
      const count =
        road_info.signal_info.phase - road_info.signal_info.phase_list.length;
      if (count > 0) {
        //增加
        let s = road_info.signal_info.phase_list.length;
        for (let i = s; i < road_info.signal_info.phase; i++) {
          insert_phase(road_info, i);
          drawPhase(i);
        }
      } else {
        //减少
        road_info.signal_info.phase_list.splice(
          road_info.signal_info.phase,
          -count
        );
        states.road_pts.splice(road_info.signal_info.phase, -count);
      }
      refreshLocation();
      calculatePeriod();
    };

    //是否显示图例
    const onShowLegendChange = () => {
      drawScale();
    };

    //灯时间变更
    const onItemPeriodBlur = () => {
      calculatePeriod();
      //渲染路
      initRoads(road_info);
    };

    //计算总周期
    const calculatePeriod = () => {
      road_info.signal_info.period = 0;
      road_info.signal_info.phase_list.map((phaseItem: any) => {
        road_info.signal_info.period +=
          phaseItem.green + phaseItem.yellow + phaseItem.red;
      });
      drawScale();
    };

    //表格行点击事件
    const onRowClick = (record: any, index: number) => {
      return {
        onClick: () => {
          states.currentPhase = index;
          onDirectionChange(states.currentDirection);
        },
      };
    };

    //点击相位
    const onGClick = (index: number) => {
      states.currentPhase = index;
      onDirectionChange(states.currentDirection);
    };

    //点击切换方向
    const onDirectionChange = (value: any) => {
      states.currentDirection = value;
      //延时加载，等待dom加载完毕再渲染
      setTimeout(() => {
        setDirectionLine();
      }, 10);
    };

    //点击方向
    const onDirectionClick = (index: number) => {
      setDirection(index);
    };

    //非机动车点击方向
    const onDirectionClick2 = (index: number) => {
      setDirection(index);
    };

    //设置点击方向的颜色及划线
    const setDirection = (index: number) => {
      //当前相位
      const phase_item = road_info.signal_info.phase_list[states.currentPhase];
      const direction = phase_item.directions[states.currentDirection][index]; //方向数据

      //渠化中是否存在这个方向
      let is_exsit = false;
      //写数据
      const is_enable = !direction.is_enable; //用户点击结果
      const key = direction.direction; //当前用户选中方向
      //需要判断是否有两方向或三方向指示,如果有,需要全部统一赋值
      const rc = road_info.canalize_info[states.currentDirection];
      rc.road_sign.enter.map((item: any) => {
        if (item.key.indexOf(key) > -1) {
          //查找多方向其余联动方向
          phase_item.directions[states.currentDirection].map((d: any) => {
            if (item.key.indexOf(d.direction) > -1) {
              d.is_enable = is_enable;
              if (direction.is_enable) {
                d.green = phase_item.green;
                d.yellow = phase_item.yellow;
                d.red = phase_item.red;
              } else {
                d.green = 0;
                d.yellow = 0;
                d.red = 0;
              }
            }
          });
          is_exsit = true;
        }
      });
      if (is_exsit) {
        setDirectionLine();
      } else {
        openNotfication("warning", "请在渠化页面中设置相关方向道路路标");
      }
    };

    //点击后画线
    const setDirectionLine = () => {
      road_info.signal_info.phase_list[states.currentPhase].directions[
        states.currentDirection
      ].forEach((d: any, index: number) => {
        if (d.is_enable) {
          //颜色标记
          drawDirectionColor(index, true);
          //画路径
          drawDirectionPath(index);
        } else {
          //颜色取消标记
          drawDirectionColor(index, false);
          //删除路径
          deleteDirectionPath(index);
        }
      });
    };

    const drawDirectionPath = (index: number) => {
      const road_count = road_info.road_attr.length;
      const index1 = states.currentDirection;
      const index2 =
        index1 + index >= road_count
          ? index1 + index - road_count
          : index1 + index;
      const point1 =
        states.road_pts[states.currentPhase].point[index1].right_point;
      const point2 =
        states.road_pts[states.currentPhase].point[index2].left_point;
      const curvature = getCurvByAngle(
        0.06,
        road_info.road_attr[index1].angle,
        road_info.road_attr[index2].angle,
        point1,
        point2
      );
      const Q = getQByPathCurv(point1, point2, curvature);
      const d_str = `M ${point1[0]} ${point1[1]} Q ${Q} ${point2[0]} ${point2[1]}`;
      const id = "direction_line";
      const tag =
        "direction_line_" +
        states.currentPhase +
        "_" +
        states.currentDirection +
        "_" +
        index;
      const line = document.createElementNS(states.ns, "path");
      line.setAttribute("tag", tag);
      line.setAttribute("id", id);
      line.setAttribute("d", d_str);
      line.setAttribute("stroke", "#fff");
      line.setAttribute("stroke-width", "5");
      line.setAttribute("fill", `none`);
      line.setAttribute("marker-end", `url(#${id})`);
      document.querySelectorAll("g").forEach((g) => {
        if (g.id === "phase_" + states.currentPhase) {
          g.appendChild(line);
        }
      });
    };

    const deleteDirectionPath = (index: number) => {
      const tag =
        "direction_line_" +
        states.currentPhase +
        "_" +
        states.currentDirection +
        "_" +
        index;
      document.querySelectorAll("path").forEach((e: any) => {
        if (
          e.id.indexOf("direction_line") > -1 &&
          e.getAttribute("tag") === tag
        ) {
          e.remove();
        }
      });
    };

    const drawDirectionColor = (index: number, is_enable: boolean) => {
      const idx =
        states.currentPhase + "_" + states.currentDirection + "_" + index; //相位+方向+点击转向
      const currentDirection = document.querySelector(`#direction${idx}`);
      const currentArrow = document.querySelector(`#arrow${idx}>path`);
      const currentColor = is_enable ? "#4f48ad" : "#a2a2a2";
      currentDirection?.setAttribute("stroke", currentColor);
      currentArrow?.setAttribute("style", "fill:" + currentColor);
    };

    //初始化加载
    onMounted(() => {
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans[roadStates.current_signal].road_info;
      //渲染路
      initRoads(rf);
    });

    return {
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      ...toRefs(states),
      ...toRefs(road_info),
      signalColor,
      phaseColumns,
      onShowLegendChange,
      onRowClick,
      onPhaseChange,
      onItemPeriodBlur,
      onDirectionChange,
      onDirectionClick,
      onGClick,
      onChangeSignal,
      onTimeConfirm,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
@import url("../index.less");
</style>
