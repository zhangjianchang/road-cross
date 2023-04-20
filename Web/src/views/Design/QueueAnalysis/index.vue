<template>
  <div class="basic-main">
    <div class="main-canvas" v-if="isNaN(Number(total_saturation))">
      <div class="error-msg">请先设置信号方案</div>
    </div>
    <div
      v-show="!showAnalysis && !isNaN(Number(total_saturation))"
      class="main-canvas"
    >
      <!-- 功能区 -->
      <div class="func">
        <div class="gradient-horizontal">
          <div class="gradient-horizontal-A rect">A</div>
          <div class="gradient-horizontal-B rect">B</div>
          <div class="gradient-horizontal-C rect">C</div>
          <div class="gradient-horizontal-D rect">D</div>
          <div class="gradient-horizontal-E rect">E</div>
          <div class="gradient-horizontal-F rect">F</div>
        </div>
      </div>
      <!-- 底部功能区 -->
      <div class="text-info">单位：m</div>
      <!-- 图示 -->
      <svg id="canvas">
        <text v-for="(_, index) in plans.road_attr" :key="index" x="330">
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
            v-if="road.rect.saturation != 0"
            x="200"
            y="-100"
            rx="100"
            ry="100"
            width="640"
            height="1200"
            :fill="road.rect.background"
            stroke="#ddd"
            stroke-width="2"
            opacityTag="1"
          />
          <path
            :d="road.sign.path"
            fill="#fff"
            opacityTag="1"
            v-if="road.rect.saturation != 0"
          />
          <text
            x="250"
            y="1400"
            fill="#000"
            style="font-size: 260px"
            opacityTag="1"
            v-if="road.rect.saturation != 0"
          >
            {{ road.rect.saturation.toFixed(1) }}
          </text>
        </g>
        <circle
          cx="350"
          cy="350"
          r="30"
          :fill="total_color"
          stroke="#eee"
          stroke-width="1"
          id="total_saturation"
          opacityTag="1"
        />
        <text x="335" y="355" fill="#fff" opacityTag="1">
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
      <div v-for="(analysis, index) in plans.queueAnalysis" :key="index">
        <div class="menu-header">
          <div class="menu-title">
            方案{{ index + 1 }}
            <SwapOutlined
              @click="handleChange(index)"
              title="切换为当前方案"
              style="color: #4f48ad"
            />
          </div>
          <div class="change"></div>
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
                    @change="handleChangeAnalysis"
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
                    @change="handleChangeAnalysis"
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
                    @change="handleChangeAnalysis"
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

<script lang="ts">
import { defineComponent, onMounted, onUnmounted, reactive, toRefs } from "vue";
import Container from "../../../components/Container/index.vue";
import {
  DragOutlined,
  PlusOutlined,
  DeleteOutlined,
  SwapOutlined,
} from "@ant-design/icons-vue";
import { insect_pt, unique } from "../../../utils/common";
import {
  getBackgroundByDelay,
  getDirectionZhName,
  get_green_yellow,
  get_λ,
  mergeWays,
  plans,
  roadStates,
} from "..";
import { openNotfication } from "../../../utils/message";
import * as echarts from "echarts";
import { echart_toolbox, echart_tooltip, road_model } from "../data";
import { roadSigns } from "../Canalize";
import { get_V, get_x } from "../Saturation";
import { get_queue } from ".";
import _ from "lodash";

export default defineComponent({
  components: {
    Container,
    DragOutlined,
    PlusOutlined,
    DeleteOutlined,
    SwapOutlined,
  },
  setup() {
    //道路信息
    const road_info = reactive(_.cloneDeep(road_model));

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      d: 120, //离圆心距离
      far_d: 240, //离圆心距离
      road_width: 80, //路宽
      road_sign_pts: [] as any[], //路标
      total_color: "#fff", //中心总颜色
      total_saturation: "0.00", //中心总数值
      ratio: 4, //比例尺
      showAnalysis: false,
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
            axisLabel: {
              interval: 0, //显示全部
              rotate: 40, //太多了斜着显示
            },
            data: [] as string[],
            axisPointer: { type: "shadow" },
          },
        ],
        //y轴
        yAxis: [
          {
            type: "value",
            name: "排队对比分析",
            min: 0,
            max: 21,
            interval: 3,
            axisLabel: {
              formatter: "{value}",
            },
          },
        ],
        //具体显示数据
        series: [] as any[],
      },
      reportData: [] as any[],
    });

    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      render();
    };

    function render() {
      //先清空路径
      clearRoadPath();
      //画路径
      drawRoadLine();
      //画路标
      drawRoadSign();
      //写路名
      drawRoadText();
    }

    //清空道路svg
    const clearRoadPath = () => {
      document.querySelectorAll("path").forEach((e) => {
        const is_delete = e.getAttribute("deleteTag") === "1";
        if (is_delete) e.remove();
      });
      document.querySelectorAll("g").forEach((e) => {
        e.remove();
      });
      document.querySelectorAll("text").forEach((e) => {
        const is_delete = e.getAttribute("deleteTag") === "1";
        if (is_delete) e.remove();
      });
    };

    //画路径
    const drawRoadLine = () => {
      for (let i = 0; i < plans.road_count; i++) {
        const dr = Math.PI * 0.5;
        const len = states.road_width;
        //第一条路
        let dw = getDW(i);
        //夹角过小后移距离
        let offset = getOffset(dw.diff_angle);
        let d = states.far_d;
        let pt_r10 = cal_point(dw, d, -dr, len); //这点用来连接远端虚线

        let pt_r11 = cal_point(dw, d, dr, len);
        let d_str = `M${pt_r11.x},${pt_r11.y} `;

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
        } else {
          d_str = "";
        }
        drawPath(d_str, false);
        //远端两点虚线连接
        d_str = `M${pt_r10.x},${pt_r10.y} L${pt_r11.x},${pt_r11.y}`;
        drawPath(d_str, true);
      }
    };
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
        offset = (90 - diff_angle) * 2.5;
      } else if (diff_angle < 90) {
        offset = (90 - diff_angle) * 1.5;
      }
      return offset > 120 ? 120 : offset;
    };
    function drawPath(d_str: string, dasharray: boolean) {
      // 创建SVG元素——交叉口
      const path = document.createElementNS(states.ns, "path");
      path.setAttribute("deleteTag", `1`);
      path.setAttribute("id", "road_path");
      path.setAttribute("d", d_str);
      path.setAttribute("fill", "none");
      path.setAttribute("stroke", "#ddd");
      path.setAttribute("stroke-width", "2");
      if (dasharray) {
        path.setAttribute("stroke-dasharray", "5,5");
      }
      states.cvs?.appendChild(path);
    }

    //画路标
    function drawRoadSign() {
      //先清空
      states.road_sign_pts.length = 0;
      for (let i = 0; i < plans.road_count; i++) {
        var rc = road_info.canalize_info[i];
        var angle = plans.road_attr[i].angle;
        const dw = {
          dir: { radian: (Math.PI / 180) * angle },
          origin: { x: states.cx },
          road_sign: { enter: [] as any[] },
        };

        //获取合并后方向的key
        const all_keys = mergeWays(rc);
        //根据key取路标
        const road_signs = all_keys.map((key: any) => {
          const path = roadSigns.find((rs: any) => rs.key === key)?.path;
          return { key, path };
        });

        //绘制
        for (var j = 0; j < road_signs.length; j++) {
          //增加偏移系数k，微调路标位置
          const k = 8 * (1 - j);
          const d = 140;
          const dr = Math.PI * 0.5;
          const len = k * states.ratio;
          const pt = cal_point(dw, d, dr, len);
          const d_key = road_signs[road_signs.length - 1 - j].key;
          const d_path = road_signs[road_signs.length - 1 - j].path;
          create_road_sign(pt, i, j, d_key, d_path);
        }
      }

      //计算交叉口平局饱和度
      let total_xq = 0;
      let total_q = 0;
      states.road_sign_pts.forEach((r) => {
        if (r.rect.saturation != 0) {
          total_xq += Number(r.rect.saturation) * Number(r.rect.q);
          total_q += Number(r.rect.q);
        }
      });
      states.total_saturation = (total_xq / total_q).toFixed(1);
      states.total_color = getBackgroundByDelay(states.total_saturation);
    }

    /**
     * 绘制路标
     * @param way_pt 绘制坐标原点
     * @param index 整条道路index
     * @param way_index 道路进口index
     * @param road_key 路标key
     * @param road_path 路标内容
     */
    function create_road_sign(
      way_pt: { x: number; y: number },
      index: number,
      way_index: number,
      road_key: string,
      road_path: string
    ) {
      const g = {
        transform: `rotate(${270 - plans.road_attr[index].angle} ${way_pt.x},${
          way_pt.y
        }) translate(${way_pt.x},${way_pt.y}) scale(0.04)`,
        id: `g${index}${way_index}`,
      };
      //路标
      const sign = {
        key: road_key,
        path: road_path,
      };
      let q = get_q(index, road_key, road_info);
      let saturation = getRatio(index, way_index, road_key, q, road_info);
      let background = getBackgroundByDelay(saturation);
      //圆角矩形背景
      const rect = {
        q,
        saturation,
        background,
      };
      states.road_sign_pts.push({ g, sign, rect });
    }

    //写数字
    const drawRoadText = () => {
      for (let i = 0; i < road_info.canalize_info.length; i++) {
        const rc = road_info.canalize_info[i];
        var angle = -plans.road_attr[i].angle;

        const dr = Math.PI * 0.5;
        const len = states.road_width;
        let dw = getDW(i);
        let d = states.far_d - 40;
        const pt = cal_point(dw, d, -dr, len + 20);

        var txt = document.createElementNS(states.ns, "text"); // 方向n文本
        txt.innerHTML = rc.name;
        txt.setAttribute("deleteTag", "1");
        txt.setAttribute("fill", "rgb(0,0,0)");
        txt.setAttribute("text-anchor", "middle");
        if (angle < -120 && angle > -270) angle = angle + 180; // 文字朝上
        txt.setAttribute(
          "transform",
          "translate(" + pt.x + "," + pt.y + ") rotate(" + angle + ")"
        );
        states.cvs?.appendChild(txt);
      }
    };

    //重构延误
    function getRatio(
      roadIndex: number,
      wayIndex: number,
      road_key: string,
      road_q: number,
      rf: any
    ) {
      //绿信比数据
      const g_y = get_green_yellow(rf, roadIndex, road_key);
      let green = g_y.green;
      let yellow = g_y.yellow;
      const C = rf.signal_info.period;
      const λ = get_λ(green, yellow, C);

      const flow_line = rf.flow_info.line_info[roadIndex];
      const q = road_q;
      const d = flow_line.truck_ratio / 100;
      const V = get_V(q, d);
      const PHF = flow_line.peak_period;
      const S = rf.flow_info.saturation[roadIndex][wayIndex].number;
      const x = get_x(V, PHF, S, λ); /**饱和度 */

      const queue = get_queue(q, PHF, d, C, λ, x, S, green);
      return queue;
    }

    const get_q = (roadIndex: number, road_key: string, rf: any) => {
      const direction_number = rf.flow_info.flow_detail[roadIndex].turn.map(
        (t: any) => {
          const k = rf.canalize_info[roadIndex].direction_num[t.direction];
          return {
            direction: t.direction,
            number: t.number,
            k,
          };
        }
      );

      //进口道流量数据
      let number = 0;
      let k = 0;
      direction_number.map((dm: any) => {
        if (dm.direction === road_key) {
          number += dm.number;
          k += dm.k;
        } else if (road_key.indexOf("_") > -1) {
          const d = road_key.split("_");
          if (d.indexOf(dm.direction) > -1) {
            number += dm.number;
            k += dm.k;
          }
        }
      });
      number = number / k; //流量/车道数量得到平均流量
      return number;
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
      if (plans.queueAnalysis.length === 3) {
        openNotfication("warning", "最多提供三个对比方案");
        return;
      }
      const analysis = {
        name: "",
        canalize_plan: 0,
        flow_plan: 0,
        signal_plan: 0,
      };
      plans.queueAnalysis.push(analysis);
      //重新按序起名
      plans.queueAnalysis.map((item, index) => {
        item.name = "方案" + (index + 1);
      });
      //加载echarts
      initEcharts();
    };

    //切换数据为当前方案
    const handleChange = (index: number) => {
      const current = plans.queueAnalysis[index];
      const rf =
        plans.canalize_plans[current.canalize_plan].flow_plans[
          current.flow_plan
        ].signal_plans[current.signal_plan].road_info;
      onChangeQueue(rf);
    };

    //执行切换
    const onChangeQueue = (rf: any) => {
      Object.assign(road_info, rf);
      render();
    };

    //删除方案
    const handleDelete = (index: number) => {
      if (plans.queueAnalysis.length === 1) {
        openNotfication("warning", "至少保留一个方案");
        return;
      }
      plans.queueAnalysis = plans.queueAnalysis.filter(
        (_, idx) => idx !== index
      );
      //重新按序起名
      plans.queueAnalysis.map((item, index) => {
        item.name = "方案" + (index + 1);
      });

      //加载echarts
      initEcharts();
    };

    //切换页面展示方案
    const handleChangeAnalysis = () => {
      if (isNaN(Number(states.total_saturation))) {
        openNotfication("warning", "请先设置信号方案");
        states.showAnalysis = false;
        return;
      }
      if (states.showAnalysis) {
        initEcharts();
      }
    };
    /**页面操作事件 end*/

    /**报表相关 start*/
    // 声明定义一下echart
    let echart = echarts;
    //初始化echarts
    function initEcharts() {
      const report = document.getElementById("report")!;
      //TODO 发到服务器会出现空白的情况，网络方案，暂采用
      report.removeAttribute("_echarts_instance_");
      let chart = echart.init(report);
      //填充配置和数据
      setEchartOption();
      //先清空
      chart.clear();
      //再渲染
      chart.setOption(states.analysisOption);
    }
    //设置echarts数据
    const setEchartOption = () => {
      //先初始化数据
      initPlansData();

      //填充legend
      let legendData = [] as string[];
      legendData = plans.queueAnalysis.map((item) => item.name);
      legendData.push("平均值");
      states.analysisOption.legend.data = legendData;

      //填充xAxis
      states.reportData.map((rd) => {
        rd.items.map((r: any) => {
          states.analysisOption.xAxis[0].data.push(r.x);
        });
      });
      //去重
      states.analysisOption.xAxis[0].data = unique(
        states.analysisOption.xAxis[0].data
      );

      //填充series
      states.analysisOption.series.length = 0;
      states.reportData.map((r) => {
        const seriesItemData = r.items.map((r: any) => r.y);
        const seriesItem = {
          name: r.name,
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
      // const seriesItem = {
      //   name: "平均值",
      //   type: "line",
      //   tooltip: {
      //     valueFormatter: function (value: number) {
      //       return value;
      //     },
      //   },
      //   data: [0.6, 0.3, 0.5, 0.8, 0.2, 0.5, 0.8, 0.1],
      // };
      // states.analysisOption.series.push(seriesItem);
    };

    //计算数据
    const initPlansData = () => {
      states.reportData.length = 0;
      plans.queueAnalysis.forEach((a) => {
        var rf =
          plans.canalize_plans[a.canalize_plan].flow_plans[a.flow_plan]
            .signal_plans[a.signal_plan].road_info;
        let reportItems = [] as any[];
        rf.queue_info.map((si, i) => {
          si.map((s: any) => {
            reportItems.push({
              x: getDirectionZhName(i, s.key),
              y: s.number.toFixed(2),
            });

            //y轴取数据最高
            states.analysisOption.yAxis[0].max = Math.max(
              Number(states.analysisOption.yAxis[0].max.toFixed(2)),
              s.number
            );
            //每级高度
            states.analysisOption.yAxis[0].interval = Number(
              (states.analysisOption.yAxis[0].max / 7).toFixed(2)
            );
          });
        });
        states.reportData.push({ name: a.name, items: reportItems });
      });
    };
    /**报表相关 end*/

    /**基础方法 */
    //加载所有方案下的饱和度
    const initSaturation = () => {
      plans.canalize_plans.map((c, cidx) => {
        c.flow_plans.map((f, fidx) => {
          f.signal_plans.map((s, sidx) => {
            var rf =
              plans.canalize_plans[cidx].flow_plans[fidx].signal_plans[sidx]
                .road_info;

            //先清空
            rf.queue_info.length = 0;

            for (let i = 0; i < plans.road_count; i++) {
              const si = [] as any[];
              const rc = rf.canalize_info[i];
              //获取合并后方向的key
              const all_keys = mergeWays(rc);
              all_keys.map((key: any, j: number) => {
                let q = get_q(i, key, rf);
                let number = getRatio(i, j, key, q, rf);
                si.push({ key, number });
              });
              rf.queue_info.push(si);
            }
          });
        });
      });
    };
    /**基础方法 end*/

    onMounted(() => {
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans[roadStates.current_signal].road_info;
      //渲染路
      Object.assign(road_info, rf);
      initRoads();
      initSaturation();
    });

    onUnmounted(() => {
      echart.dispose;
    });

    function drawPoint(x: number, y: number, color: string) {
      var g = document.createElementNS(states.ns, "g");
      g.setAttribute("stroke", color);
      g.setAttribute("stroke-width", "2");
      g.setAttribute("fill", "black");
      var circle = document.createElementNS(states.ns, "circle");
      circle.setAttribute("cx", x.toString());
      circle.setAttribute("cy", y.toString());
      circle.setAttribute("r", "2");
      g.appendChild(circle);
      states.cvs?.appendChild(g);
    }

    return {
      ...toRefs(states),
      ...toRefs(road_info),
      ...toRefs(roadStates),
      plans,
      labelCol: { span: 2 },
      wrapperCol: { span: 22 },
      handleAddNew,
      handleDelete,
      handleChange,
      handleChangeAnalysis,
      onChangeQueue,
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
}
</style>
