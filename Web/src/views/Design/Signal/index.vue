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
        <rect width="700" height="700" fill="#FFFF99" deleteTag="1" />
        <!-- 道路路径 -->
        <path
          :d="road.path.d"
          :id="road.path.id"
          fill="rgb(162,162,162)"
          deleteTag="1"
        />
        <!-- 相位数据 -->
        <text x="120" y="800" class="phase-text" deleteTag="1">
          {{ road.text }}
        </text>
        <!-- 相位内部方向箭头 -->
        <defs>
          <!-- 机动车箭头 -->
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
              style="fill: #ffffff"
              deleteTag="1"
            />
          </marker>
          <!-- 非机动车箭头 -->
          <marker
            id="non_direction_line"
            markerUnits="strokeWidth"
            markerWidth="6"
            markerHeight="6"
            viewBox="0 0 12 12"
            refX="6"
            refY="6"
            orient="auto-start-reverse"
          >
            <path
              xmlns="http://www.w3.org/2000/svg"
              d="M2,2 L10,6 L2,10 L2,6 L2,2"
              style="fill: #ffb90f"
              deleteTag="1"
            />
          </marker>
          <!-- 行人箭头 -->
          <marker
            id="ped_direction_line"
            markerUnits="strokeWidth"
            markerWidth="6"
            markerHeight="6"
            viewBox="0 0 12 12"
            refX="6"
            refY="6"
            orient="auto-start-reverse"
          >
            <path
              xmlns="http://www.w3.org/2000/svg"
              d="M2,2 L10,6 L2,10 L2,6 L2,2"
              style="fill: #00ff99"
              deleteTag="1"
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
    <div class="menu" v-if="!userStates.code_info || userStates.can_edit">
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
                        <a-select-option :value="1"> 是 </a-select-option>
                        <a-select-option :value="0"> 否 </a-select-option>
                      </a-select>
                    </a-form-item>
                  </a-col>
                </a-row>
              </a-form>
            </div>
          </a-collapse-panel>
          <a-collapse-panel key="2" header="相位设置">
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
                <template #is_lap="{ record, index }">
                  <a-select
                    v-if="index !== 0"
                    v-model:value="record.is_lap"
                    size="small"
                    class="middle-form-width"
                    @change="onLapChange(record, index)"
                  >
                    <a-select-option :value="1"> 是 </a-select-option>
                    <a-select-option :value="0"> 否 </a-select-option>
                  </a-select>
                </template>
              </a-table>
            </div>
          </a-collapse-panel>
          <a-collapse-panel key="3" header="进口设置">
            <div v-if="signal_info.phase_list.length > 0">
              <a-form :label-col="labelMCol" :wrapper-col="wrapperMCol">
                <a-form-item
                  label="进口方向"
                  style="margin-bottom: 10px !important"
                >
                  <a-select
                    v-model:value="currentDirection"
                    size="small"
                    class="form-width"
                    @change="onDirectionChange"
                  >
                    <a-select-option
                      v-for="(item, index) in plans.road_attr"
                      :key="item"
                      :value="index"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
                <a-form-item
                  label="机动车"
                  style="margin-bottom: 5px !important"
                >
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
                        :id="`arrow_${currentPhase}_${currentDirection}_${index}`"
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
                          deleteTag="1"
                        />
                      </marker>
                    </defs>
                    <path
                      :id="`direction_${currentPhase}_${currentDirection}_${index}`"
                      :d="item.d"
                      fill="none"
                      stroke="#a2a2a2"
                      stroke-width="100"
                      :marker-end="`url(#arrow_${currentPhase}_${currentDirection}_${index})`"
                      deleteTag="1"
                    ></path>
                  </svg>
                </a-form-item>
                <a-form-item
                  label="非机动车"
                  style="margin-bottom: 5px !important"
                >
                  <svg
                    v-for="(item, index) in sign_non_pts[currentDirection]"
                    :key="index"
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 700 700"
                    class="road-sign"
                    @click="onNonDirectionClick(index)"
                  >
                    <defs>
                      <marker
                        :id="`non_arrow_${currentPhase}_${currentDirection}_${index}`"
                        markerUnits="strokeWidth"
                        markerWidth="3"
                        markerHeight="3"
                        viewBox="0 0 12 12"
                        refX="6"
                        refY="6"
                        orient="auto-start-reverse"
                      >
                        <path
                          xmlns="http://www.w3.org/2000/svg"
                          d="M2,2 L10,6 L2,10 L2,6 L2,2"
                          style="fill: #a2a2a2"
                          deleteTag="1"
                        />
                      </marker>
                    </defs>
                    <path
                      v-if="index === 0"
                      :id="`non_direction_${currentPhase}_${currentDirection}_${index}`"
                      :d="item.d"
                      fill="none"
                      stroke="#a2a2a2"
                      stroke-width="100"
                      :marker-end="`url(#non_arrow_${currentPhase}_${currentDirection}_${index})`"
                      :marker-start="`url(#non_arrow_${currentPhase}_${currentDirection}_${index})`"
                      deleteTag="1"
                    />
                    <path
                      v-else
                      :id="`non_direction_${currentPhase}_${currentDirection}_${index}`"
                      :d="item.d"
                      fill="none"
                      stroke="#a2a2a2"
                      stroke-width="100"
                      :marker-end="`url(#non_arrow_${currentPhase}_${currentDirection}_${index})`"
                      deleteTag="1"
                    />
                  </svg>
                </a-form-item>
                <a-form-item label="行人" style="margin-bottom: 5px !important">
                  <svg
                    v-for="(item, index) in sign_ped_pts[currentDirection]"
                    :key="index"
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 700 700"
                    class="road-sign"
                    @click="onPedDirectionClick(index)"
                  >
                    <defs>
                      <marker
                        :id="`ped_arrow_${currentPhase}_${currentDirection}_${index}`"
                        markerUnits="strokeWidth"
                        markerWidth="3"
                        markerHeight="3"
                        viewBox="0 0 12 12"
                        refX="6"
                        refY="6"
                        orient="auto-start-reverse"
                      >
                        <path
                          xmlns="http://www.w3.org/2000/svg"
                          d="M2,2 L10,6 L2,10 L2,6 L2,2"
                          style="fill: #a2a2a2"
                          deleteTag="1"
                        />
                      </marker>
                    </defs>
                    <path
                      v-if="index === 0"
                      :id="`ped_direction_${currentPhase}_${currentDirection}_${index}`"
                      :d="item.d"
                      fill="none"
                      stroke="#a2a2a2"
                      stroke-width="100"
                      :marker-end="`url(#ped_arrow_${currentPhase}_${currentDirection}_${index})`"
                      :marker-start="`url(#ped_arrow_${currentPhase}_${currentDirection}_${index})`"
                      deleteTag="1"
                    />
                    <path
                      v-if="index !== 0"
                      :id="`ped_direction_${currentPhase}_${currentDirection}_${index}`"
                      :d="item.d.split(';')[0]"
                      fill="none"
                      stroke="#a2a2a2"
                      stroke-width="60"
                      deleteTag="1"
                    />
                    <path
                      v-if="index !== 0"
                      :id="`ped_direction_${currentPhase}_${currentDirection}_${index}`"
                      :d="item.d.split(';')[1]"
                      fill="none"
                      stroke="#a2a2a2"
                      stroke-width="60"
                      :marker-end="`url(#ped_arrow_${currentPhase}_${currentDirection}_${index})`"
                      :marker-start="`url(#ped_arrow_${currentPhase}_${currentDirection}_${index})`"
                      deleteTag="1"
                    />
                  </svg>
                </a-form-item>
                <!-- <a-form-item label="待转"> </a-form-item> -->
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
import { cal_point, insect_pt } from "../../../utils/common";
import { signalColor, getStartX, phaseColumns, getArrowId, getColor } from ".";
import _ from "lodash";
import { road_model } from "../data";
import {
  create_signal_info,
  getPedDetail_D,
  getStraightTurnDetail,
  getTurnDetail_D,
  get_λ,
  handleEdit,
  insert_phase,
  plans,
  roadStates,
} from "..";
import { message } from "ant-design-vue";
import { openNotfication } from "../../../utils/message";
import { userStates } from "../../UserCenter";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    //道路信息
    const road_info = reactive(_.cloneDeep(road_model));

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      road_width: 110, //路宽
      d: 180, //离圆心距离
      far_d: 300, //离圆心距离(远端)
      svg_width: 590, //画布宽度
      phase_height: 80, //每个相位的间距
      curvature: 2, //路口弧度
      road_pts: [] as any[], //道路缩略
      sign_pts: [] as any[], //机动车路标
      sign_non_pts: [] as any[], //非机动车路标
      sign_ped_pts: [] as any[], //行人路标
      sign_trans_pts: [] as any[], //待转路标
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
            for (let j = 0; j < plans.road_count; j++) {
              states.currentPhase = i;
              states.currentDirection = j;
              //渠化中如果不存在该方向，则取消用户上次点击的数据
              const road_signs = [] as string[];
              //全部基础方向
              road_info.canalize_info[j].road_sign.enter.map((r: any) => {
                r.key.split("_").forEach((rs: any) => {
                  road_signs.push(rs);
                });
              });
              //修正用户数据
              road_info.signal_info.phase_list[i].directions[j].map(
                (d: any) => {
                  if (road_signs.indexOf(d.direction) === -1 && d.is_enable) {
                    d.is_enable = false;
                    d.green = 0;
                    d.yellow = 0;
                    d.red = 0;
                  }
                }
              );
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
      const road_count = plans.road_count;
      for (let i = 0; i < road_count; i++) {
        //机动车
        const sign_pt = [];
        for (let m = 0; m < road_count; m++) {
          let j = i + m >= road_count ? i + m - road_count : i + m;
          const d = getTurnDetail_D(road_info, i, j);
          const path = { d };
          sign_pt.push(path);
        }
        states.sign_pts.push(sign_pt);

        //非机动车
        const sign_non_pt = [];
        for (let m = 0; m < road_count; m++) {
          let j = i + m >= road_count ? i + m - road_count : i + m;
          let d = "";
          if (i === j) {
            d = getStraightTurnDetail(road_info, i);
          } else {
            d = getTurnDetail_D(road_info, i, j);
          }
          const path = { d };
          sign_non_pt.push(path);
        }
        states.sign_non_pts.push(sign_non_pt);

        //行人
        const sign_ped_pt = [];
        for (let m = 0; m < 3; m++) {
          let d = "";
          if (m === 0) {
            d = getStraightTurnDetail(road_info, i);
          } else {
            d = getPedDetail_D(i, m);
          }
          const path = { d };
          sign_ped_pt.push(path);
        }
        states.sign_ped_pts.push(sign_ped_pt);
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
      //默认2个相位
      for (let p = 0; p < road_info.signal_info.phase; p++) {
        const pl = road_info.signal_info.phase_list;
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
        x1 = start_x + getStartX(pl, p, "g") * width;
        y1 = signal + p * states.phase_height;
        w = start_x + getStartX(pl, p, "y") * width - x1;
        //正常
        if (!pl[p].is_lap) {
          h = 30;
          createRect(p, x1, y1, w, h, "green");
          time = pl[p].green;
          if (Number(time) > 0) {
            createText(
              x1 + w / 2 - 3,
              y1 + h / 2 + 3,
              p,
              time,
              "green",
              true,
              true
            );
          }
        } else {
          // 搭接相位
          /**上一相位绿色 */
          x1 = start_x + getStartX(pl, p - 1, "g") * width;
          y1 = signal + p * states.phase_height;
          w = start_x + getStartX(pl, p, "y") * width - x1;
          h = 13;
          createRect(p, x1, y1, w, h, "green");
          const prev_p_time =
            pl[p - 1].green + pl[p - 1].yellow + pl[p - 1].red;
          time = pl[p].green + prev_p_time;
          if (Number(time) > 0) {
            createText(
              x1 + w / 2 - 3,
              y1 + h / 2 + 3,
              p,
              time,
              "green",
              true,
              true
            );
          }
          /**上一相位绿色 */
          /**当前相位绿色 */
          x1 = start_x + getStartX(pl, p, "g") * width;
          y1 += 17;
          w = start_x + getStartX(pl, p, "y") * width - x1;
          createRect(p, x1, y1, w, h, "green");
          time = pl[p].green;
          if (Number(time) > 0) {
            createText(
              x1 + w / 2 - 3,
              y1 + h / 2 + 3,
              p,
              time,
              "green",
              true,
              true
            );
          }
          /**当前相位绿色 */
        }
        /**绿色信号 */
        /**黄色信号 */
        x1 = start_x + getStartX(pl, p, "y") * width;
        y1 = signal + p * states.phase_height;
        w = start_x + getStartX(pl, p, "r") * width - x1;
        time = pl[p].yellow;
        h = 30;
        if (!pl[p].is_lap) {
          createRect(p, x1, y1, w, h, "yellow");
          if (Number(time) > 0) {
            createText(
              x1 + w / 2 - 3,
              y1 + h / 2 + 3,
              p,
              time,
              "yellow",
              true,
              true
            );
          }
        } else {
          /**上一相位黄色 */
          h = 13;
          createRect(p, x1, y1, w, h, "yellow");
          if (Number(time) > 0) {
            createText(x1 + w / 2 - 3, y1 + h / 2 + 3, p, time, "yellow", true);
          }
          /**上一相位黄色 */
          /**当前相位黄色 */
          y1 += 17;
          createRect(p, x1, y1, w, h, "yellow");
          if (Number(time) > 0) {
            createText(
              x1 + w / 2 - 3,
              y1 + h / 2 + 3,
              p,
              time,
              "yellow",
              true,
              true
            );
          }
          /**当前相位黄色 */
        }
        /**黄色信号 */
        /**红色信号 */
        x1 = start_x + getStartX(pl, p, "r") * width;
        y1 = signal + p * states.phase_height;
        w = start_x + getStartX(pl, p + 1, "g") * width - x1;
        time = pl[p].red;
        if (!pl[p].is_lap) {
          createRect(p, x1, y1, w, h, "red");
          if (Number(time) > 0) {
            createText(
              x1 + w / 2 - 3,
              y1 + h / 2 + 3,
              p,
              time,
              "red",
              true,
              true
            );
          }
        } else {
          /**上一相位红色 */
          h = 13;
          createRect(p, x1, y1, w, h, "red");
          if (Number(time) > 0) {
            createText(x1 + w / 2 - 3, y1 + h / 2 + 3, p, time, "red", true);
          }
          /**上一相位红色 */
          /**当前相位红色 */
          y1 += 17;
          createRect(p, x1, y1, w, h, "red");
          if (Number(time) > 0) {
            createText(
              x1 + w / 2 - 3,
              y1 + h / 2 + 3,
              p,
              time,
              "red",
              true,
              true
            );
          }
          /**当前相位红色 */
        }

        /**红色信号 */
        /**文字 */
        //相位
        x1 = start_x - 60;
        y1 = signal + 10 + p * states.phase_height;
        createText(x1, y1, p, pl[p].name);
        //绿信比
        y1 = signal + 30 + p * states.phase_height;
        let λ = get_λ(
          pl[p].green,
          pl[p].yellow,
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
      line.setAttribute("deleteTag", "1");
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
      rect.setAttribute("deleteTag", "1");
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
     * @param can_click 是否可点击
     */
    function createText(
      x: number,
      y: number,
      i: number,
      content: string,
      type = "",
      is_time = false,
      can_click = false
    ) {
      let text = document.createElementNS(states.ns, "text");
      text.setAttribute("id", `text_${i}`);
      text.setAttribute("x", x.toString());
      text.setAttribute("y", y.toString());
      text.setAttribute("deleteTag", "1");
      if (is_time) {
        text.setAttribute("style", `font-size:12px;font-weight:800`);
      } else {
        text.setAttribute("style", `font-size:12px`);
      }
      if (can_click) {
        text.setAttribute(
          "style",
          `font-size:12px;font-weight:800;cursor:pointer;`
        );
        text.addEventListener(
          "click",
          () => onClickTime(i, type, content),
          false
        );
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
      states.road_pts.length = 0;
      for (let p = 0; p < road_info.signal_info.phase; p++) {
        drawPhase(p);
      }
    }

    function drawPhase(p: number) {
      //svg图像
      let d_str = "";
      for (let i = 0; i < plans.road_count; i++) {
        //画主路径
        const dr = Math.PI * 0.5;
        const len = states.road_width;
        //第一条路
        let dw = getDW(i);
        //夹角过小后移距离
        let offset = getOffset(dw.diff_angle);
        let d = states.far_d;
        let pt_r10 = cal_point(dw, d, -dr, len); //起始点,道路左侧远端点
        if (i === 0) {
          d_str += `M${pt_r10.x},${pt_r10.y} `;
        } else {
          d_str += `L${pt_r10.x},${pt_r10.y} `;
        }

        let pt_r11 = cal_point(dw, d, dr, len);
        d_str += `L${pt_r11.x},${pt_r11.y} `;

        d = states.d + offset;
        let pt_r12 = cal_point(dw, d, dr, len);
        d_str += `L${pt_r12.x},${pt_r12.y} `;
        //第二条路
        const next_i = i === plans.road_count - 1 ? 0 : i + 1;
        dw = getDW(next_i);
        d = states.far_d;
        let pt_l11 = cal_point(dw, d, -dr, len);

        d = states.d + offset;
        let pt_l12 = cal_point(dw, d, -dr, len);

        //连接两条路
        let Q = insect_pt(
          { point1: pt_r11, point2: pt_r12 },
          { point1: pt_l11, point2: pt_l12 }
        );
        //连接第一条路右侧，Q点，第二条路左侧
        if (Q) {
          d_str += `Q${Q.x},${Q.y} ${pt_l12.x},${pt_l12.y} L${pt_l11.x},${pt_l11.y}`;
        }

        // const roadEdgePoint = setRoadEdgePoint(i);
        // roadEdgePoints.push(roadEdgePoint);
      }
      setPath(d_str, p);
      refreshLocation();
    }
    const getDW = (i: number) => {
      const next_i = i === plans.road_count - 1 ? 0 : i + 1;
      const angle1 = plans.road_attr[i].angle;
      const angle2 = plans.road_attr[next_i].angle;
      const radian = (Math.PI / 180) * angle1; // 角度转弧度
      const dw = {
        dir: { radian },
        origin: { x: states.cx },
        diff_angle:
          angle2 - angle1 < 0 ? 360 + (angle2 - angle1) : angle2 - angle1,
      };
      return dw;
    };
    const getOffset = (diff_angle: number) => {
      let offset = 0;
      if (diff_angle < 50) {
        offset = (90 - diff_angle) * 2;
      } else if (diff_angle < 90) {
        offset = (90 - diff_angle) * 1.5;
      }
      return offset > 59 ? 59 : offset;
    };

    //写路径至数组
    function setPath(d_str: string, i: number) {
      const path = { id: "road_path", d: d_str };
      const g = { transform: `translate(${130 * i + 20},10) scale(0.18)` };
      const text = `${road_info.signal_info.phase_list[i].name}：${road_info.signal_info.phase_list[i].green}秒`;
      states.road_pts.push({ g, path, text });
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
          // drawPhase(i);
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
      initRoads(road_info);
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
        phaseItem.directions.forEach((d: any) => {
          d.forEach((item: any) => {
            if (item.is_enable) {
              item.green = phaseItem.green;
              item.yellow = phaseItem.yellow;
              item.red = phaseItem.red;
            } else {
              item.green = 0;
              item.yellow = 0;
              item.red = 0;
            }
          });
        });
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

    //点击机动车方向
    const onDirectionClick = (index: number) => {
      //只要更换方向就取消所有方向的搭接相位
      road_info.signal_info.phase_list.map((pl: any) => {
        pl.is_lap = 0;
      });
      setDirection(index);
      //设置好数据后重新渲染时序图
      drawScale();
    };

    //非机动车点击方向
    const onNonDirectionClick = (index: number) => {
      const phase_item = road_info.signal_info.phase_list[states.currentPhase];
      const direction = phase_item.directions[states.currentDirection][index]; //方向数据
      direction.is_non_enable = !direction.is_non_enable;
      if (direction.is_non_enable) {
        markDirection(index, "non_direction");
      } else {
        unmarkDirection(index, "non_direction");
      }
    };

    //行人点击方向
    const onPedDirectionClick = (index: number) => {
      const phase_item = road_info.signal_info.phase_list[states.currentPhase];
      const direction = phase_item.directions[states.currentDirection][index]; //方向数据
      direction.is_ped_enable = !direction.is_ped_enable;
      if (direction.is_ped_enable) {
        markDirection(index, "ped_direction");
      } else {
        unmarkDirection(index, "ped_direction");
      }
    };

    //标记
    const markDirection = (index: number, id: string) => {
      if (id === "direction") {
        drawDirectionPath(index);
      } else if (id === "non_direction") {
        drawNonDirectionPath(index); //画方向连线
      } else if (id === "ped_direction") {
        drawPedDirectionPath(index);
      }
      drawDirectionColor(id, index, true); //加深颜色
    };
    //取消标记
    const unmarkDirection = (index: number, id: string) => {
      deleteDirectionPath(index, id + "_line"); //取消方向连线
      drawDirectionColor(id, index, false); //取消颜色
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
        // 机动车
        if (d.is_enable) {
          markDirection(index, "direction");
        } else {
          unmarkDirection(index, "direction");
        }
        //非机动车
        if (d.is_non_enable) {
          markDirection(index, "non_direction");
        } else {
          unmarkDirection(index, "non_direction");
        }
        //行人
        if (d.is_ped_enable) {
          markDirection(index, "ped_direction");
        } else {
          unmarkDirection(index, "ped_direction");
        }
      });
    };

    //机动车方向路径
    const drawDirectionPath = (index: number) => {
      const d_str = drawP2P(index, 60, false);
      const id = "direction_line";
      drawP2PPath(index, id, d_str);
    };

    //非机动车方向路径
    const drawNonDirectionPath = (index: number) => {
      const d_str = drawP2P(index, 40, true);
      const id = "non_direction_line";
      drawP2PPath(index, id, d_str);
    };

    //行人方向路径
    const drawPedDirectionPath = (index: number) => {
      const d_str = drawPedP2P(index);
      const id = "ped_direction_line";
      drawP2PPath(index, id, d_str);
    };

    //连线, is_non非机动车
    const drawP2P = (index: number, width: number, is_non: boolean) => {
      const road_count = plans.road_count;
      const index1 = states.currentDirection;
      const index2 =
        index1 + index >= road_count
          ? index1 + index - road_count
          : index1 + index;

      const dr = Math.PI * 0.5;
      const len = states.road_width - width;
      const far = 60;
      let d_str = "";
      //第一条路
      let dw = getDW(index1);
      //夹角过小后移距离
      let offset = getOffset(dw.diff_angle);
      let d = states.far_d - far;
      let pt_r11 = cal_point(dw, d, dr, len);

      d = states.d + offset;
      let pt_r12 = cal_point(dw, d, dr, len);
      //第二条路
      dw = getDW(index2);
      d = states.far_d - far;
      let pt_l11 = cal_point(dw, d, -dr, len);

      d = states.d + offset;
      let pt_l12 = cal_point(dw, d, -dr, len);

      //连接两条路
      let Q = insect_pt(
        { point1: pt_r11, point2: pt_r12 },
        { point1: pt_l11, point2: pt_l12 }
      );

      //非机动车过马路
      if (is_non && index1 === index2) {
        dw = getDW(index1);
        d = states.d + offset - 40;
        let pt_1 = cal_point(dw, d, dr, len);
        let pt_2 = cal_point(dw, d, -dr, len);
        d_str = `M${pt_1.x},${pt_1.y} L${pt_2.x},${pt_2.y}`;
      }
      //连接第一条路右侧，Q点，第二条路左侧
      else if (Q) {
        d_str = `M${pt_r11.x},${pt_r11.y} L${pt_r12.x},${pt_r12.y} Q${Q.x},${Q.y} ${pt_l12.x},${pt_l12.y} L${pt_l11.x},${pt_l11.y}`;
      } else {
        if (index1 != index2) {
          d_str += `M${pt_r11.x},${pt_r11.y} L${pt_r12.x},${pt_r12.y} L${pt_l12.x},${pt_l12.y} L${pt_l11.x},${pt_l11.y}`;
        } else {
          let pt_c = cal_point(dw, d - 100, dr, 0);
          d_str += `M${pt_r11.x},${pt_r11.y} L${pt_r12.x},${pt_r12.y} Q${pt_c.x},${pt_c.y} ${pt_l12.x},${pt_l12.y} L${pt_l11.x},${pt_l11.y}`;
        }
      }
      return d_str;
    };

    //行人连线，j=0贯通1向右走2向左走
    const drawPedP2P = (index: number) => {
      const i = states.currentDirection;
      const s = {
        d: 180, //离圆心距离
        far_d: 260, //离圆心距离
        len: 70,
      };
      let d_str = "";
      const k = index === 1 ? 1 : -1;
      const dr = Math.PI * 0.5;
      const dw = getDW(i);
      const offset = getOffset(dw.diff_angle);
      let d = s.d + offset;
      let len = s.len;

      if (index === 0) {
        let pt_1 = cal_point(dw, d, dr, len);
        let pt_2 = cal_point(dw, d, -dr, len);
        d_str = `M${pt_1.x},${pt_1.y} L${pt_2.x},${pt_2.y}`;
      } else {
        //横线
        let pt_1 = cal_point(dw, d, k * dr, len - 80);
        let pt_2 = cal_point(dw, d, -k * dr, len);
        d_str += `M${pt_1.x},${pt_1.y} L${pt_2.x},${pt_2.y};`;

        //竖线
        let pt_s1 = cal_point(dw, d, k * dr, 0);
        d = s.far_d;
        let pt_s2 = cal_point(dw, d, k * dr, 0);
        d_str += `M${pt_s1.x},${pt_s1.y} L${pt_s2.x},${pt_s2.y};`;
      }
      return d_str;
    };

    //画连线路径
    const drawP2PPath = (index: number, id: string, d_str: string) => {
      const tag = `${id}_${states.currentPhase}_${states.currentDirection}_${index}`;
      const color = getColor(id);
      //非机动车同一方向会存在两条对向（需要加箭头的线段;不需要加箭头的线段）
      d_str.split(";").map((d, idx) => {
        const line = document.createElementNS(states.ns, "path");
        line.setAttribute("tag", tag);
        line.setAttribute("id", id);
        line.setAttribute("d", d);
        line.setAttribute("fill", "none");
        line.setAttribute("stroke", color);
        line.setAttribute("stroke-width", "10");
        if (idx === 0) {
          line.setAttribute("marker-end", `url(#${id})`);
          if (index === 0 && id === "non_direction_line") {
            //非机动车穿越马路双向箭头
            line.setAttribute("marker-start", `url(#${id})`);
            line.setAttribute("stroke-width", "10");
          } else if (id === "ped_direction_line") {
            //行人穿越马路双向箭头
            line.setAttribute("marker-start", `url(#${id})`);
            line.setAttribute("stroke-width", "6");
          }
        } else {
          line.setAttribute("stroke", "#ffffff");
        }
        document.querySelectorAll("g").forEach((g) => {
          if (g.id === "phase_" + states.currentPhase) {
            g.appendChild(line);
          }
        });
      });
    };

    const deleteDirectionPath = (index: number, id: string) => {
      const tag = `${id}_${states.currentPhase}_${states.currentDirection}_${index}`;
      document.querySelectorAll("path").forEach((e: any) => {
        if (e.id.indexOf(id) > -1 && e.getAttribute("tag") === tag) {
          e.remove();
        }
      });
    };

    const drawDirectionColor = (
      id: string,
      index: number,
      is_enable: boolean
    ) => {
      const currentColor = is_enable ? "#4f48ad" : "#a2a2a2";
      const idx = `${states.currentPhase}_${states.currentDirection}_${index}`; //相位+方向+点击转向
      const arrow_id = getArrowId(id);
      const currentDirection = document.querySelectorAll(`#${id}_${idx}`);
      currentDirection.forEach((item) => {
        item?.setAttribute("stroke", currentColor);
      });
      const currentArrow = document.querySelector(`#${arrow_id}_${idx}>path`);
      currentArrow?.setAttribute("style", "fill:" + currentColor);
    };

    //搭接相位
    const onLapChange = (record: any, index: number) => {
      if (record.is_lap) {
        //上一相位勾选的所有方向
        const prev_all_enabled = [] as any[];
        const prev_directions =
          road_info.signal_info.phase_list[index - 1].directions;
        prev_directions.map((d: any[], i: number) => {
          d.map((dd) => {
            if (dd.is_enable) {
              prev_all_enabled.push(i + "_" + dd.direction);
            }
          });
        });

        //当前相位勾选的所有方向
        const all_enabled = [] as any[];
        const directions = record.directions;
        directions.map((d: any[], i: number) => {
          d.map((dd) => {
            if (dd.is_enable) {
              all_enabled.push(i + "_" + dd.direction);
            }
          });
        });

        //判断两个相位是否有相同方向
        const lap = prev_all_enabled.filter((p) => all_enabled.indexOf(p) > -1);
        if (lap.length > 0) {
          record.lap = [] as any[];
          lap.map((lp) => {
            record.lap.push({
              roadIndex: Number(lp.split("_")[0]),
              roadKey: lp.split("_")[1],
            });
          });
          message.success("搭接成功");
          //渲染路
          initRoads(road_info);
        } else {
          openNotfication("warning", "当前相位与上一相位无共同放行方向");
          record.is_lap = 0;
        }
      } else {
        //渲染路
        initRoads(road_info);
      }
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
      wrapperCol: { span: 14 },
      labelMCol: { span: 3 },
      wrapperMCol: { span: 21 },
      ...toRefs(states),
      ...toRefs(road_info),
      plans,
      roadStates,
      userStates,
      signalColor,
      phaseColumns,
      onShowLegendChange,
      onRowClick,
      onPhaseChange,
      onItemPeriodBlur,
      onDirectionChange,
      onDirectionClick,
      onNonDirectionClick,
      onPedDirectionClick,
      onGClick,
      onChangeSignal,
      onTimeConfirm,
      onLapChange,
      handleEdit,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
@import url("../index.less");
</style>
