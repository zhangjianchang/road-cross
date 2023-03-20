<template>
  <div class="basic-main">
    <div class="main-canvas" v-if="isNaN(Number(total_saturation))">
      <div class="error-msg">请先设置信号方案</div>
    </div>
    <div v-show="!showAnalysis" class="main-canvas">
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
          />
          <path
            :d="road.sign.path"
            fill="#fff"
            v-if="road.rect.saturation != 0"
          ></path>
          <text
            x="250"
            y="1400"
            fill="#000"
            style="font-size: 260px"
            v-if="road.rect.saturation != 0"
          >
            {{ road.rect.saturation.toFixed(2) }}
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
        />
        <text x="335" y="355" fill="#fff">
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

<script lang="ts">
import { defineComponent, onMounted, onUnmounted, reactive, toRefs } from "vue";
import {
  basicDirection,
  getBackground,
  getDirectionZhName,
  get_V,
  get_x,
} from "./index";
import Container from "../../../components/Container/index.vue";
import {
  DragOutlined,
  PlusOutlined,
  DeleteOutlined,
  SwapOutlined,
} from "@ant-design/icons-vue";
import { getQByPathCurv } from "../../../utils/common";
import { get_λ, plans, roadStates } from "..";
import { openNotfication } from "../../../utils/message";
import * as echarts from "echarts";
import { echart_toolbox, echart_tooltip, road_model } from "../data";
import { roadSigns } from "../Canalize";

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
    const road_info = reactive(JSON.parse(JSON.stringify(road_model)));

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
            axisLabel: {
              interval: 0, //显示全部
            },
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
            max: 1,
            interval: 0.25,
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
      initCrossData();
      render();
    };

    function initCrossData() {
      states.cross_pts.length = 0; //先清空
      for (var i = 0; i < road_info.road_attr.length; i++) {
        var angle = road_info.road_attr[i].angle;
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        var x3 = Math.cos(radian) * states.road_width + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * states.road_width + 350;
        //获取交叉口圆plus和路边相交的点
        setPts(states.cross_pts, angle, x3, y3);
      }
    }

    function render() {
      //先清空路径
      clearRoadPath();
      //画路径
      drawRoadLine();
      //画路标
      drawRoadSign();
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
      states.total_saturation = (total_xq / total_q).toFixed(2);
      states.total_color = getBackground(states.total_saturation);
    }

    //绘制之前合并同时并行道路
    const mergeWays = (rc: any) => {
      //绘制之前先确定合并
      let all_keys = rc.road_sign.enter.map((rs: any) => rs.key); //全部方向
      const multiple_way_keys = rc.road_sign.enter
        .filter((rs: any) => basicDirection.indexOf(rs.key) === -1)
        .map((rs: any) => rs.key); //两种及以上转向的方向

      //剔除多方向已包含的方向
      multiple_way_keys.map((mk: any) => {
        const mks = mk.split("_");
        mks.forEach((s: any) => {
          let index = all_keys.indexOf(s);
          if (index > -1) {
            all_keys = all_keys.filter((k: any) => k != s);
          }
        });
      });

      //针对多方向再次合并
      const sl_index = all_keys.indexOf("straight_left");
      let sr_index = all_keys.indexOf("straight_right");
      if (sl_index > -1 && sr_index > -1) {
        all_keys.splice(sl_index, 1);
        sr_index = all_keys.indexOf("straight_right");
        all_keys.splice(sr_index, 1);
        all_keys.push("left_straight_right");
      }
      return all_keys;
    };
    //掉头类型合并
    //TODO

    //画路面上的线
    function drawPath(line: Element, d_str: string) {
      line.setAttribute("id", "road_path");
      line.setAttribute("d", d_str);
      line.setAttribute("stroke", "#ddd");
      line.setAttribute("stroke-width", "2");
      line.setAttribute("fill", `none`);
      line.setAttribute("deleteTag", `1`);
      states.cvs?.appendChild(line);
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
        transform: `rotate(${270 - road_info.road_attr[index].angle} ${
          way_pt.x
        },${way_pt.y}) translate(${way_pt.x},${way_pt.y}) scale(0.04)`,
        id: `g${index}${way_index}`,
      };
      //路标
      const sign = {
        key: road_key,
        path: road_path,
      };
      let q = get_q(index, road_key);
      let saturation = getRatio(index, way_index, road_key, q);
      let background = getBackground(saturation);
      //圆角矩形背景
      const rect = {
        q,
        saturation,
        background,
      };
      states.road_sign_pts.push({ g, sign, rect });
    }

    //重构饱和度
    function getRatio(
      roadIndex: number,
      wayIndex: number,
      road_key: string,
      road_q: number
    ) {
      //绿信比数据
      const period = road_info.signal_info.period;
      let green = 0;
      let yellow = 0;
      road_info.signal_info.phase_list.forEach((p: any) => {
        p.directions[roadIndex].forEach((d: any) => {
          if (road_key.indexOf(d.direction) > -1) {
            green = green | d.green;
            yellow = yellow | d.yellow;
          }
        });
      });

      const flow_line = road_info.flow_info.line_info[roadIndex];
      const q = road_q;
      const d = flow_line.truck_ratio / 100;
      const V = get_V(q, d);
      const PHF = flow_line.peak_period;
      const λ = get_λ(green, yellow, period);
      const S = road_info.flow_info.saturation[roadIndex][wayIndex].number;
      const x = get_x(V, PHF, S, λ);
      return x;
    }

    const get_q = (roadIndex: number, road_key: string) => {
      const direction_number = road_info.flow_info.flow_detail[
        roadIndex
      ].turn.map((t: any) => {
        const k = road_info.canalize_info[roadIndex].direction_num[t.direction];
        return {
          direction: t.direction,
          number: t.number,
          k,
        };
      });

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

    //切换数据为当前方案
    const handleChange = (index: number) => {
      const current = states.analysisList[index];
      const rf =
        plans.canalize_plans[current.canalize_plan].flow_plans[
          current.flow_plan
        ].signal_plans[current.signal_plan].road_info;
      onChangeSatuation(rf);
    };

    //执行切换
    const onChangeSatuation = (rf: any) => {
      Object.assign(road_info, rf);
      render();
    };

    //删除方案
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

    //切换页面展示方案
    const handleChangeAnalysis = () => {
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
      let chart = echart.init(document.getElementById("report")!);
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
      legendData = states.analysisList.map((item) => item.name);
      legendData.push("平均值");
      states.analysisOption.legend.data = legendData;

      //填充xAxis TODO多方案去重
      states.analysisOption.xAxis[0].data = states.reportData[0].items.map(
        (r: any) => r.x
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
      states.analysisList.forEach((a) => {
        var road_info =
          plans.canalize_plans[a.canalize_plan].flow_plans[a.flow_plan]
            .signal_plans[a.signal_plan].road_info;

        let reportItems = [] as any[];
        for (let i = 0; i < plans.road_count; i++) {
          const rc = road_info.canalize_info[i];
          //获取合并后方向的key
          const all_keys = mergeWays(rc);
          all_keys.map((key: any, j: number) => {
            const item = getReportItem(i, j, key);
            if (item) {
              reportItems.push(item);
            }
          });
        }
        states.reportData.push({ name: a.name, items: reportItems });
      });
    };

    const getReportItem = (i: number, j: number, road_key: string) => {
      let q = get_q(i, road_key);
      let number = getRatio(i, j, road_key, q);
      states.analysisOption.yAxis[0].max = Math.max(
        states.analysisOption.yAxis[0].max,
        number
      );
      //写进统计数据中去
      if (number > 0) {
        const name = getDirectionZhName(i, road_key);
        return { x: name, y: number.toFixed(2) };
      }
      return undefined;
    };
    /**报表相关 end*/

    /**基础方法 */
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
    /**基础方法 end*/

    onMounted(() => {
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans[roadStates.current_signal].road_info;
      //渲染路
      Object.assign(road_info, rf);
      initRoads();
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
      onChangeSatuation,
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
