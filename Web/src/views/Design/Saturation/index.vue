<template>
  <div class="basic-main">
    <div class="main-canvas" v-show="!showAnalysis">
      <div class="func">
        <div class="gradient"></div>
        <div class="gradient-text">
          <div>1</div>
          <div class="dec">饱和度</div>
          <div>0</div>
        </div>
      </div>
      <!-- 图示 -->
      <svg id="canvas">
        <text v-for="(_, index) in road_attr" :key="index" x="330">
          <textPath :xlink:href="'#road_' + (index + 1)">
            方向{{ index + 1 }}
          </textPath>
        </text>
        <!-- 路标 -->
        <g
          v-for="road in road_sign_pts"
          :key="road.g"
          :transform="road.g.transform"
          class="road-sign"
          :id="road.g.id"
        >
          <rect
            x="200"
            y="-100"
            rx="100"
            ry="100"
            width="640"
            height="1200"
            :fill="road.rect.background"
            stroke="#ddd"
            stroke-width="2"
          />
          <path :d="road.sign.d" fill="#fff"></path>
          <text x="250" y="1400" fill="#000" style="font-size: 260px">
            {{ road.rect.saturation }}
          </text>
        </g>
        <circle
          cx="350"
          cy="350"
          r="30"
          :fill="total_color"
          stroke="#ddd"
          stroke-width="1"
          id="total_saturation"
        />
        <text x="335" y="355" fill="#fff" stroke-width="2">
          {{ total_saturation }}
        </text>
      </svg>
    </div>
    <div v-show="showAnalysis" class="main-canvas">
      <div class="report" id="report"></div>
    </div>
    <!-- 参数 -->
    <div class="menu">
      <div class="switch mb-5">
        <span class="switch-title"> 显示对比分析 </span>
        <a-switch
          v-model:checked="showAnalysis"
          checked-children="显示"
          un-checked-children="隐藏"
          @change="handleChangeAnalysis"
        />
      </div>
      <div v-for="(analysis, index) in analysisList" :key="index">
        <div class="menu-header">
          <div class="menu-title">方案{{ index + 1 }}</div>
          <div class="opration">
            <DeleteOutlined
              @click="handleDelete(index)"
              title="删除"
              style="color: #ff4d4f"
            />
          </div>
        </div>
        <div class="menu-content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="24" class="mb-2">
                <a-form-item label="名称">
                  <a-input v-model:value="analysis.name" />
                </a-form-item>
              </a-col>
              <a-col :span="24" class="mb-2">
                <a-form-item label="渠化">
                  <a-select
                    placeholder="请选择渠化方案"
                    v-model:value="analysis.canalize_plan"
                  >
                    <a-select-option
                      v-for="(item, index) in plans.canalize_plans"
                      :key="index"
                      :value="index"
                    >
                      {{ item.name }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="24" class="mb-2">
                <a-form-item label="流量">
                  <a-select
                    placeholder="请选择流量方案"
                    v-model:value="analysis.flow_plan"
                  >
                    <a-select-option
                      v-for="(item, index) in plans.canalize_plans[
                        analysis.canalize_plan
                      ].flow_plans"
                      :key="index"
                      :value="index"
                    >
                      {{ item.name }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="24" class="mb-2">
                <a-form-item label="信号">
                  <a-select
                    placeholder="请选择信号方案"
                    v-model:value="analysis.signal_plan"
                  >
                    <a-select-option
                      v-for="(item, index) in plans.canalize_plans[
                        analysis.canalize_plan
                      ].flow_plans[analysis.flow_plan].signal_plans"
                      :key="index"
                      :value="index"
                    >
                      {{ item.name }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </div>
      <div class="mt-5">
        <a-button class="add-line-btn" @click="handleAddNew">
          <template #icon><PlusOutlined /></template>
          增加对比方案
        </a-button>
      </div>
    </div>
  </div>
</template>
0

<script lang="ts">
import {
  defineComponent,
  inject,
  onMounted,
  onUnmounted,
  reactive,
  shallowRef,
  toRefs,
} from "vue";
import {
  getBackground,
  getCelr,
  get_V,
  get_x,
  tr_Cl,
  tr_Cs,
  tr_Csr,
  tr_VC,
} from "./index";
import Container from "../../../components/Container/index.vue";
import {
  DragOutlined,
  PlusOutlined,
  DeleteOutlined,
} from "@ant-design/icons-vue";
import { getQByPathCurv } from "../../../utils/common";
import { get_λ, plans, roadStates } from "..";
import { openNotfication } from "../../../utils/message";
import * as echarts from "echarts";
import { DirectionsEnum, echart_toolbox, echart_tooltip } from "../data";

export default defineComponent({
  components: { Container, DragOutlined, PlusOutlined, DeleteOutlined },
  setup() {
    //道路信息
    const road_info = reactive(
      plans.canalize_plans[roadStates.current_canalize].flow_plans[
        roadStates.current_flow
      ].signal_plans[roadStates.current_signal].road_info
    );

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      road_width: 160, //路宽
      curvature: 2, //路口弧度
      cross_pts: [] as any[], //所有路口交叉点
      road_sign_pts: [] as any[], //路标
      total_color: "#fff", //中心总颜色
      total_saturation: "0.00", //中心总数值
      ratio: 4, //比例尺
      showAnalysis: false,
      analysisList: [
        {
          name: "方案1",
          canalize_plan: 0,
          flow_plan: 0,
          signal_plan: 0,
        },
      ],
      analysisOption: {
        //提示的样式
        tooltip: echart_tooltip,
        //工具栏
        toolbox: echart_toolbox,
        //对比值
        legend: { data: [] as string[] },
        //x轴
        xAxis: [
          {
            type: "category",
            data: [] as string[],
            axisPointer: { type: "shadow" },
          },
        ],
        //y轴
        yAxis: [
          {
            type: "value",
            name: "饱和度",
            min: 0,
            max: 1.5,
            interval: 0.3,
            axisLabel: {
              formatter: "{value}",
            },
          },
        ],
        //具体显示数据
        series: [] as any[],
      },
    });

    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      render();
    };

    function render() {
      for (var i = 0; i < road_info.road_attr.length; i++) {
        var angle = road_info.road_attr[i].angle;
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        var x3 = Math.cos(radian) * states.road_width + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * states.road_width + 350;
        //获取交叉口圆plus和路边相交的点
        setPts(states.cross_pts, angle, x3, y3);
      }
      // 交叉口
      drawMain();
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

    function drawMain() {
      //画路径
      drawRoadLine();
      //画路标
      drawRoadSign();
    }

    //画路径
    function drawRoadLine() {
      const line = document.createElementNS(states.ns, "path"); // 创建SVG元素——交叉口
      let d_str = "";
      let roadIdx = 0;
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
      drawPath(line, d_str);
    }

    //画路标
    function drawRoadSign() {
      for (let i = 0; i < plans.road_count; i++) {
        //每条道路增加一个分析
        road_info.saturation_info.push([]);

        var rc = road_info.canalize_info[i];

        const dw = {
          dir: { radian: (Math.PI / 180) * rc.angle },
          origin: { x: states.cx },
          road_sign: { enter: [] as any[] },
        };
        // 入口路标
        for (var j = 0; j < rc.enter.num; j++) {
          dw.road_sign.enter[j] = rc.road_sign.enter[j];
        }

        //绘制
        for (var j = 0; j < rc.enter.num; j++) {
          //增加偏移系数k，微调路标位置
          const k = (j - 1) * 8;
          //增加偏移系数k2，调整路宽时调整路标位置
          const d = 120;
          const dr = Math.PI * 0.5;
          const len = k * states.ratio;
          const pt = cal_point(dw, d, dr, len);
          create_road_sign(pt, i, j, dw.road_sign.enter[j].path);
        }
      }

      let total_saturation = 0;
      states.road_sign_pts.forEach((r) => {
        total_saturation += Number(r.rect.saturation);
      });
      states.total_saturation = (total_saturation / (3 * 4)).toFixed(2); //目前有三种类型
      states.total_color = getBackground(states.total_saturation);
    }

    //画路面上的线
    function drawPath(line: Element, d_str: string) {
      line.setAttribute("id", "road_path");
      line.setAttribute("d", d_str);
      line.setAttribute("stroke", "#ddd");
      line.setAttribute("stroke-width", "2");
      line.setAttribute("fill", `none`);
      states.cvs?.appendChild(line);
    }

    //重构
    function getRatio(roadIndex: number, wayIndex: number) {
      //第r条路，第w条道是什么方向
      const direction =
        road_info.canalize_info[roadIndex].road_sign.enter[wayIndex].key;
      const direction_number = road_info.flow_info.flow_detail[
        roadIndex
      ].turn.map((t: any) => {
        // const k = road_info.canalize_info[roadIndex].road_direction;
        return {
          direction: t.direction,
          number: t.number,
        };
      });
      let number = 0;
      direction_number.map((dm: any) => {
        if (dm.direction === direction) {
          number = dm.number;
        } else if (dm.direction === "all_direction") {
          //TODO
        } else if (direction.indexOf("_") > -1) {
          const d1 = direction.split("_")[0];
          const d2 = direction.split("_")[1];
          if (dm.direction === d1) {
            number += dm.number * 0.5;
          }
          if (dm.direction === d2) {
            number += dm.number * 0.5;
          }
        }
      });

      const period = road_info.signal_info.period;
      let green = 0;
      let yellow = 0;
      road_info.signal_info.phase_list.forEach((p) => {
        p.directions[roadIndex].forEach((d: any) => {
          if (direction.indexOf(d.direction) > -1) {
            green = green | d.green;
            yellow = yellow | d.yellow;

            console.log(green, d.green);
            console.log(yellow, d.yellow);
          }
        });
      });

      const flow_line = road_info.flow_info.line_info[roadIndex];
      const q = number;
      const d = flow_line.truck_ratio / 100;
      const V = get_V(q, d);
      const PHF = flow_line.peak_period;
      const λ = get_λ(green, yellow, period);
      const S = road_info.flow_info.saturation[roadIndex][wayIndex].number;
      const x = get_x(V, PHF, S, λ);
      return x;
    }

    // //roadIndex路，wayIndex 丢弃
    // function getRatio(roadIndex: number, wayIndex: number) {
    //   const rc = road_info.canalize_info[roadIndex];
    //   const T = road_info.signal_info.period;
    //   const t = get_t(roadIndex);
    //   const cart = road_info.flow_info.line_info[roadIndex].truck_ratio / 100;
    //   const Bl = getBl(roadIndex);
    //   const c_trCs = tr_Cs(T, t, cart); //直行通行能力
    //   const c_trCsr = tr_Csr(c_trCs); //直右通行能力
    //   const celr = getCelr(c_trCs + c_trCsr, Bl);
    //   const c_trCl = tr_Cl(celr, Bl); //专左通行能力
    //   let v = getCurrentWayFlow(roadIndex, wayIndex);
    //   let trVC = 0; //饱和度
    //   if (wayIndex === 0) {
    //     //专左
    //     trVC = tr_VC(v, c_trCl);
    //     road_info.saturation_info[roadIndex].push({ vc: trVC, c: c_trCl });
    //   } else if (wayIndex === 1) {
    //     //直行
    //     trVC = tr_VC(v, c_trCs);
    //     road_info.saturation_info[roadIndex].push({ vc: trVC, c: c_trCs });
    //   } else if (wayIndex === 2) {
    //     //直右
    //     trVC = tr_VC(v, c_trCsr);
    //     road_info.saturation_info[roadIndex].push({ vc: trVC, c: c_trCsr });
    //   }
    //   console.log(wayIndex, trVC);
    //   return trVC;
    // }

    /**
     * 绘制路标
     * @param way_pt 绘制坐标原点
     * @param index 整条道路index
     * @param way_index 道路进口index
     * @param road_sign 路标
     */
    function create_road_sign(
      way_pt: { x: number; y: number },
      index: number,
      way_index: number,
      road_sign: string
    ) {
      const g = {
        transform: `rotate(${270 - road_info.road_attr[index].angle} ${
          way_pt.x
        },${way_pt.y}) translate(${way_pt.x},${way_pt.y}) scale(0.04)`,
        id: `g${index}${way_index}`,
      };
      //路标
      const sign = {
        d: road_sign,
      };
      let saturation = getRatio(index, way_index).toFixed(2);
      let background = getBackground(saturation);
      //圆角矩形背景
      const rect = {
        saturation,
        background,
      };
      states.road_sign_pts.push({ g, sign, rect });
    }

    //当前方向对应的绿灯时间
    const get_t = (roadIndex: number) => {
      let t = 0;
      for (let i = 0; i < road_info.signal_info.phase; i++) {
        const phase_item = road_info.signal_info.phase_list[i];
        //todo 暂定直行车道为当前索引对应车道
        const current =
          roadIndex === road_info.road_attr.length - 1 ? 0 : roadIndex + 1;
        t += phase_item.directions[roadIndex][current].green;
      }
      return t;
    };

    //左转车道占本面进口车道的比例
    const getBl = (roadIndex: number) => {
      const flow_detail = road_info.flow_info.flow_detail[roadIndex];
      var total = getCurrentRoadTotalFlow(roadIndex);
      //todo 暂定，后续需改为方向
      var left = flow_detail.turn[1].number;
      var Bl = left / total;
      return Bl;
    };

    //当前道路所有车道总流量
    const getCurrentRoadTotalFlow = (roadIndex: number) => {
      const flow_detail = road_info.flow_info.flow_detail[roadIndex];
      var total = 0;
      for (let i = 0; i < road_info.road_attr.length; i++) {
        total += flow_detail.turn[i].number;
      }
      return total;
    };

    //当前道路某条车道的车流量
    const getCurrentWayFlow = (roadIndex: number, wayIdx: number) => {
      const current = road_info.flow_info.saturation[roadIndex][wayIdx].number;
      return current;
    };

    // 路口Draw对象dw；距离基点d，弧度增量dr，长度len，返回新点
    function cal_point(
      dw: { dir: { radian: any }; origin: { x: number } },
      d: number,
      dr: number,
      len: number
    ) {
      // 计算
      var np = { x: 0, y: 0 }; // new point

      var tx, ty; // 临时点坐标
      var r = dw.dir.radian;
      // 基线上终点
      tx = Math.cos(r) * d + dw.origin.x;
      ty = -Math.sin(r) * d + dw.origin.x;
      // 垂直基线，交点在tx，ty，长度len的点
      np.x = len * Math.cos(r + dr) + tx;
      np.y = -len * Math.sin(r + dr) + ty;

      return np;
    }

    /**页面操作事件 start*/
    const handleAddNew = () => {
      if (states.analysisList.length === 3) {
        openNotfication("warning", "最多提供三个对比方案");
        return;
      }
      const analysis = {
        name: "",
        canalize_plan: 0,
        flow_plan: 0,
        signal_plan: 0,
      };
      states.analysisList.push(analysis);
      //重新按序起名
      states.analysisList.map((item, index) => {
        item.name = "方案" + (index + 1);
      });
      //加载echarts
      initEcharts();
    };

    const handleDelete = (index: number) => {
      if (states.analysisList.length === 1) {
        openNotfication("warning", "至少保留一个方案");
        return;
      }
      states.analysisList = states.analysisList.filter(
        (_, idx) => idx !== index
      );
      //重新按序起名
      states.analysisList.map((item, index) => {
        item.name = "方案" + (index + 1);
      });

      //加载echarts
      initEcharts();
    };

    const handleChangeAnalysis = () => {
      if (states.showAnalysis) {
        initEcharts();
      }
    };
    /**页面操作事件 end*/

    /**报表相关 start*/
    // 声明定义一下echart
    let echart = echarts;
    function initEcharts() {
      let chart = echart.init(document.getElementById("report")!);
      //填充配置和数据
      setEchartOption();
      //先清空
      chart.clear();
      //再渲染
      chart.setOption(states.analysisOption);
    }
    const setEchartOption = () => {
      //legend
      let legendData = [] as string[];
      legendData = states.analysisList.map((item) => item.name);
      legendData.push("平均值");
      states.analysisOption.legend.data = legendData;

      //xAxis
      let xAxisData = [] as string[]; //方向
      for (let i = 0; i < road_info.canalize_info.length; i++) {
        const rd = road_info.canalize_info[i].road_direction;
        if (rd.uturn >= 1) {
          xAxisData.push("方向" + (i + 1) + DirectionsEnum.uturn);
        }
        if (rd.left >= 1) {
          xAxisData.push("方向" + (i + 1) + DirectionsEnum.left);
        }
        if (rd.straight >= 1) {
          xAxisData.push("方向" + (i + 1) + DirectionsEnum.straight);
        }
        if (rd.right >= 1) {
          xAxisData.push("方向" + (i + 1) + DirectionsEnum.right);
        }
      }
      states.analysisOption.xAxis[0].data = xAxisData;

      //series
      states.analysisOption.series.length = 0;
      states.analysisList.forEach((a) => {
        const seriesItemData = [] as number[];
        //当前方案对应的道路信息
        const rf =
          plans.canalize_plans[a.canalize_plan].flow_plans[a.flow_plan]
            .signal_plans[a.signal_plan].road_info;
        rf.canalize_info.forEach((ci) => {
          //对应某种方向的数值
          //TODO 默认左转
          seriesItemData.push(Number(Math.random().toFixed(2)));
          //TODO 默认直右
          seriesItemData.push(Number(Math.random().toFixed(2)));
        });
        const seriesItem = {
          name: a.name,
          type: "bar",
          tooltip: {
            valueFormatter: function (value: number) {
              return value;
            },
          },
          data: seriesItemData,
        };
        states.analysisOption.series.push(seriesItem);
      });

      //平均值
      const seriesItem = {
        name: "平均值",
        type: "line",
        tooltip: {
          valueFormatter: function (value: number) {
            return value;
          },
        },
        data: [0.6, 0.3, 0.5, 0.8, 0.2, 0.5, 0.8, 0.1],
      };
      states.analysisOption.series.push(seriesItem);
    };
    /**报表相关 end*/

    onMounted(() => {
      initRoads();
    });

    onUnmounted(() => {
      echart.dispose;
    });

    return {
      ...toRefs(states),
      ...toRefs(road_info),
      ...toRefs(roadStates),
      plans,
      labelCol: { span: 2 },
      wrapperCol: { span: 22 },
      handleAddNew,
      handleDelete,
      handleChangeAnalysis,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
@import url("../index.less");

.road-sign {
  width: 32px;
  height: 60px;
  background-color: red;
  margin-left: 15px;
  border-radius: 4px;
  cursor: pointer;
}
</style>
