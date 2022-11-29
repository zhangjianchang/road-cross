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
      >
        <rect width="80%" height="680" fill="#FFFF99" />
        <path
          :d="road.path.d"
          :id="road.path.id"
          fill="rgb(162,162,162)"
        ></path>
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
        <!-- <circle
          v-for="p in road.point"
          :key="p"
          :cx="p.right_point[0]"
          :cy="p.right_point[1]"
          r="10"
        ></circle>
        <circle
          v-for="p in road.point"
          :key="p"
          :cx="p.left_point[0]"
          :cy="p.left_point[1]"
          r="10"
        ></circle> -->
      </g>
    </svg>
    <!-- 参数 -->
    <div class="menu">
      <div class="form">
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="相位总数">
                  <a-select
                    v-model:value="signal_info.phase"
                    size="small"
                    class="form-width"
                    @change="onPhaseChange"
                  >
                    <a-select-option
                      v-for="item in [1, 2, 3, 4, 5, 6, 7, 8]"
                      :key="item"
                      :value="item"
                    >
                      {{ item }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
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
              <a-col :span="12">
                <a-form-item label="图例">
                  <a-select
                    v-model:value="signal_info.is_show_legend"
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
        <div class="content mt-2">
          <a-table
            :dataSource="signal_info.phase_list"
            :columns="phaseColumns"
            :scroll="{ x: 430 }"
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
        <div class="content mt-2" v-if="signal_info.phase_list.length > 0">
          <a-form>
            <a-row>
              <a-col :span="24">
                <a-form-item
                  label="进口方向"
                  :label-col="labelCol"
                  :wrapper-col="wrapperCol"
                >
                  <a-select
                    v-model:value="
                      signal_info.phase_list[currentPhase].in_direction
                    "
                    size="small"
                    class="form-width"
                    @change="onDirectionChange"
                  >
                    <a-select-option
                      v-for="(item, index) in angleSet"
                      :key="item"
                      :value="index"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="mt-2" v-if="signal_info.phase_list.length > 0">
          <div>机动车</div>
          <div>
            <svg
              v-for="(item, index) in sign_pts[
                signal_info.phase_list[currentPhase].in_direction
              ]"
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
                    signal_info.phase_list[currentPhase].in_direction +
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
                  signal_info.phase_list[currentPhase].in_direction +
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
                  signal_info.phase_list[currentPhase].in_direction +
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
              v-for="(item, index) in sign_pts[
                signal_info.phase_list[currentPhase].in_direction
              ]"
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
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, inject, onMounted, reactive, toRefs } from "vue";
import Container from "../../../components/Container/index.vue";
import { DragOutlined } from "@ant-design/icons-vue";
import { getQByPathCurv } from "../../../utils/common";
import {
  signalColor,
  getStartX,
  phaseModel,
  phaseColumns,
  DirectionItemModel,
} from ".";
import _ from "lodash";
import { getCurvByAngle } from "../Flow";
import { RoadInfo } from "..";

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
      phase_height: 80, //每个相位的间距
      curvature: 2, //路口弧度
      cross_pts: [] as any[], //所有路口交叉点
      road_pts: [] as any[], //道路缩略
      sign_pts: [] as any[], //路标（掉头右转之类）
      angleSet: [] as number[], //所有道路倾斜角，以此绘制
      currentPhase: 0, //当前选中相位
      currentDirection: 0, //当前选中方向（相位的子集）
    });
    //全局道路信息
    const road_info = inject("road_info") as RoadInfo;

    //加载道路
    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      roadDir.sort(function (a, b) {
        return a.angle - b.angle;
      });
      roadDir.map((r) => {
        states.angleSet.push(r.angle);
      });
    };

    function render() {
      initDirections();
      drawScale();
      drawMain();
    }

    function initPhase() {
      road_info.signal_info.phase_list.length = 0;
      for (let i = 0; i < road_info.signal_info.phase; i++) {
        insertPhase(i);
      }
    }

    //初始化相位数据
    function insertPhase(i: number) {
      let phaseItem = _.cloneDeep(phaseModel);
      phaseItem.index = i;
      phaseItem.name = `第${i + 1}相位`;
      for (let d = 0; d < roadDir.length; d++) {
        let directions = [];
        for (let d = 0; d < roadDir.length; d++) {
          let directionItem = _.cloneDeep(DirectionItemModel);
          directions.push(directionItem);
        }
        phaseItem.directions.push(directions);
      }
      road_info.signal_info.phase_list.push(phaseItem);
      road_info.signal_info.period +=
        phaseItem.green + phaseItem.yellow + phaseItem.red;
    }

    //加载各道路之间的方向
    const initDirections = () => {
      for (let i = 0; i < roadDir.length; i++) {
        const sign_pt = [];
        for (let j = 0; j < roadDir.length; j++) {
          const road = roadDir[i];
          const nextRoad =
            j === roadDir.length - 1 ? roadDir[0] : roadDir[j + 1];
          const d = `M${road.coordinate[0]} ${road.coordinate[1]} L${states.cx} ${states.cy} L${nextRoad.coordinate[0]} ${nextRoad.coordinate[1]}`;
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
      //默认四个相位
      for (let p = 0; p < road_info.signal_info.phase; p++) {
        let width = states.svg_width / road_info.signal_info.period; //每个刻度的宽度
        let x1, y1, x2, y2, w, h;
        for (let i = 0; i <= road_info.signal_info.period; i++) {
          let tick_len = 10; // 小刻度=10
          if (i % 5 == 0) tick_len = 20; // 长刻度=20
          /**上边缘线 */
          x1 = 25 + i * width;
          y1 = 250 + p * states.phase_height;
          x2 = 25 + i * width;
          y2 = 250 + p * states.phase_height + tick_len;
          createLine(x1, y1, x2, y2);
          /**上边缘线 */
          /**下边缘线 */
          y1 = 310 + p * states.phase_height - tick_len;
          y2 = 310 + p * states.phase_height;
          createLine(x1, y1, x2, y2);
          /**下边缘线 */
        }
        /**外层上边缘线 */
        x1 = 25;
        y1 = 250 + p * states.phase_height;
        x2 = 25 + states.svg_width;
        y2 = y1;
        createLine(x1, y1, x2, y2);
        /**外层上边缘线 */
        /**外层下边缘线 */
        x1 = 25;
        y1 = 310 + p * states.phase_height;
        x2 = 25 + states.svg_width;
        y2 = y1;
        createLine(x1, y1, x2, y2);
        /**外层下边缘线 */
        /**中心线 */
        x1 = 25;
        y1 = 280 + p * states.phase_height;
        x2 = 25 + states.svg_width;
        y2 = y1;
        createLine(x1, y1, x2, y2, "#4f48ad", "3");
        /**中心线 */
        /**绿色信号 */
        x1 = 25 + getStartX(road_info.signal_info.phase_list, p, "g") * width;
        y1 = 265 + p * states.phase_height;
        w =
          25 + getStartX(road_info.signal_info.phase_list, p, "y") * width - x1;
        h = 30;
        createRect(x1, y1, w, h, "green");
        /**绿色信号 */
        /**黄色信号 */
        x1 = 25 + getStartX(road_info.signal_info.phase_list, p, "y") * width;
        y1 = 265 + p * states.phase_height;
        w =
          25 + getStartX(road_info.signal_info.phase_list, p, "r") * width - x1;
        createRect(x1, y1, w, h, "yellow");
        /**黄色信号 */
        /**红色信号 */
        x1 = 25 + getStartX(road_info.signal_info.phase_list, p, "r") * width;
        y1 = 265 + p * states.phase_height;
        w =
          25 +
          getStartX(road_info.signal_info.phase_list, p + 1, "g") * width -
          x1;
        createRect(x1, y1, w, h, "red");
        /**红色信号 */
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
      states.cvs?.appendChild(rect);
    }

    //画相位路径
    function drawMain() {
      for (var a = 0; a < states.angleSet.length; a++) {
        var angle = states.angleSet[a];
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        var x3 = Math.cos(radian) * states.road_width + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * states.road_width + 350;
        //获取交叉口圆plus和路边相交的点
        setPts(states.cross_pts, angle, x3, y3);
      }
      for (let p = 0; p < road_info.signal_info.phase; p++) {
        let d_str = "";
        let roadIdx = 0;
        let roadEdgePoints = [];
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
        //添加路左右两侧点
        for (let ag = 0; ag < states.angleSet.length; ag++) {
          const roadEdgePoint = setRoadEdgePoint(ag);
          roadEdgePoints.push(roadEdgePoint);
        }
        setPath(d_str, p, roadEdgePoints);
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

    const setRoadEdgePoint = (i: number) => {
      var angle = states.angleSet[i];
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
        transform: `translate(${204 * i + 20},10) scale(0.29)`,
      };
      states.road_pts.push({ g, path, point });
    }

    //相位总数变更
    const onPhaseChange = () => {
      const count =
        road_info.signal_info.phase - road_info.signal_info.phase_list.length;
      if (count > 0) {
        //增加
        let s = road_info.signal_info.phase_list.length + 1;
        for (let i = s; i <= road_info.signal_info.phase; i++) {
          insertPhase(i);
        }
      } else {
        //减少
        road_info.signal_info.phase_list.splice(
          road_info.signal_info.phase,
          -count
        );
      }
      calculatePeriod();
    };

    //灯时间变更
    const onItemPeriodBlur = () => {
      calculatePeriod();
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
          onDirectionChange(
            road_info.signal_info.phase_list[states.currentPhase].in_direction
          );
        },
      };
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

    //设置点击方向的颜色及划线
    const setDirection = (index: number) => {
      //当前相位
      const phase_item = road_info.signal_info.phase_list[states.currentPhase];
      const in_direction = phase_item.in_direction; //方向
      const direction = phase_item.directions[in_direction][index]; //方向数据
      //写数据
      direction.is_enable = !direction.is_enable;
      if (direction.is_enable) {
        direction.green = phase_item.green;
        direction.yellow = phase_item.yellow;
        direction.red = phase_item.red;
      } else {
        direction.green = 0;
        direction.yellow = 0;
        direction.red = 0;
      }
      setDirectionLine();
    };

    //点击后画线
    const setDirectionLine = () => {
      const in_direction =
        road_info.signal_info.phase_list[states.currentPhase].in_direction; //方向
      road_info.signal_info.phase_list[states.currentPhase].directions[
        in_direction
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
        }
      });
    };

    const drawDirectionPath = (index: number) => {
      const index1 = states.currentDirection;
      const index2 = index + 1 === states.angleSet.length ? 0 : index + 1;
      const point1 =
        states.road_pts[states.currentPhase].point[index1].right_point;
      const point2 =
        states.road_pts[states.currentPhase].point[index2].left_point;
      const curvature = getCurvByAngle(
        0.06,
        states.angleSet[index1],
        states.angleSet[index2],
        point1,
        point2
      );
      const Q = getQByPathCurv(point1, point2, curvature);
      const d_str = `M ${point1[0]} ${point1[1]} Q ${Q} ${point2[0]} ${point2[1]}`;
      const id = "direction_line";
      // states.currentPhase +
      // "_" +
      // road_info.signal_info.phase_list[states.currentPhase].in_direction +
      // "_" +
      // index;
      const line = document.createElementNS(states.ns, "path");
      line.setAttribute("id", id);
      line.setAttribute("d", d_str);
      line.setAttribute("stroke", "#fff");
      line.setAttribute("stroke-width", "5");
      line.setAttribute("fill", `none`);
      line.setAttribute("marker-end", `url(#direction_line)`);
      document.querySelectorAll("g").forEach((g) => {
        if (g.id === "phase_" + states.currentPhase) {
          g.appendChild(line);
        }
      });
    };

    const drawDirectionColor = (index: number, is_enable: boolean) => {
      const in_direction =
        road_info.signal_info.phase_list[states.currentPhase].in_direction; //方向
      const idx = states.currentPhase + "_" + in_direction + "_" + index; //相位+方向+点击转向
      const currentDirection = document.querySelector(`#direction${idx}`);
      const currentArrow = document.querySelector(`#arrow${idx}>path`);
      const currentColor = is_enable ? "#4f48ad" : "#a2a2a2";
      currentDirection?.setAttribute("stroke", currentColor);
      currentArrow?.setAttribute("style", "fill:" + currentColor);
    };

    //初始化加载
    if (!road_info.signal_info) {
      road_info.signal_info = {
        phase: 2, //默认4个相位
        period: 0, //默认周期共80s
        is_show_legend: false,
        phase_list: [] as any[],
      };
      //加载相位数据
      initPhase();
    } else {
      //延时加载，等待dom加载完毕再渲染
      setTimeout(() => {
        road_info.signal_info.phase_list.forEach((p: any, index: number) => {
          states.currentPhase = index;
          p.directions.forEach((rec: any, index: number) => {
            // onDirectionChange(index);
          });
        });
      }, 10);
    }

    //初始化加载
    onMounted(() => {
      //渲染路
      initRoads();
      //渲染
      render();
    });

    return {
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      ...toRefs(states),
      ...toRefs(road_info),
      signalColor,
      phaseColumns,
      onRowClick,
      onPhaseChange,
      onItemPeriodBlur,
      onDirectionChange,
      onDirectionClick,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
@import url("../index.less");
</style>
