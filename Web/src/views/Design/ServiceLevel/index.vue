<template>
  <div class="basic-main">
    <div class="main-canvas" v-if="isNaN(Number(total_saturation))">
      <div class="error-msg">请先设置信号方案</div>
    </div>
    <div v-show="!isNaN(Number(total_saturation))" class="main-canvas">
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
      <div class="text-info"></div>
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
            x="400"
            y="1400"
            fill="#000"
            style="font-size: 260px"
            opacityTag="1"
            v-if="road.rect.saturation != 0"
          >
            {{ getServiceByTypeAndRatio(ratioValue, road.rect.saturation) }}
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
        <text x="345" y="355" fill="#fff" opacityTag="1">
          {{ getServiceByTypeAndRatio(ratioValue, Number(total_saturation)) }}
        </text>
      </svg>
    </div>
    <!-- 参数 -->
    <div class="menu">
      <div style="width: 70px; display: inline-block">统计口径:</div>
      <a-radio-group v-model:value="ratioValue" @change="onChangeAnalysisType">
        <a-radio value="delay">延误时间</a-radio>
        <a-radio value="saturation">饱和度</a-radio>
      </a-radio-group>
      <div class="mt-16">
        <a-table
          class="ant-table-striped"
          :dataSource="lineData"
          :columns="lineColumns"
          :pagination="false"
          :bordered="true"
          :scroll="{ x: '100%' }"
          :row-class-name="(_:any, index:number) => (index % 2 === 1 ? 'table-striped' : null)"
        />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive, toRefs } from "vue";
import Container from "../../../components/Container/index.vue";
import {
  DragOutlined,
  PlusOutlined,
  DeleteOutlined,
  SwapOutlined,
} from "@ant-design/icons-vue";
import { insect_pt } from "../../../utils/common";
import {
  getBackgroundByDelay,
  getBackgroundBySaturation,
  get_λ,
  mergeWays,
  plans,
  roadStates,
} from "..";
import { road_model } from "../data";
import { roadSigns } from "../Canalize";
import { get_V, get_x } from "../Saturation";
import { get_d } from "../DelayAnalysis";
import { getServiceByTypeAndRatio, lineColumns, lineData } from ".";
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
      ratioValue: "delay",
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
      //同时清空页面数据
      states.road_sign_pts.length = 0;
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
      states.total_color =
        states.ratioValue === "delay"
          ? getBackgroundByDelay(states.total_saturation)
          : getBackgroundBySaturation(states.total_saturation);
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
      let background =
        states.ratioValue === "delay"
          ? getBackgroundByDelay(saturation)
          : getBackgroundBySaturation(saturation);
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
      let green = 0;
      let yellow = 0;
      rf.signal_info.phase_list.forEach((p: any) => {
        p.directions[roadIndex].forEach((d: any) => {
          if (road_key.indexOf(d.direction) > -1) {
            green = green | d.green;
            yellow = yellow | d.yellow;
          }
        });
      });
      const C = rf.signal_info.period;
      const λ = get_λ(green, yellow, C); /**饱和度 */
      const flow_line = rf.flow_info.line_info[roadIndex];
      const q = road_q;
      const d = flow_line.truck_ratio / 100;
      const V = get_V(q, d);
      const PHF = flow_line.peak_period;
      const S = rf.flow_info.saturation[roadIndex][wayIndex].number;
      const x = get_x(V, PHF, S, λ); /**饱和度 */
      const Q = S * λ; //TODO 存疑，确定是一个？
      const T = 0.25;
      const e = 0.5;

      let service_level = 0;
      if (states.ratioValue === "delay") {
        service_level = get_d(C, λ, x, e, Q, T);
      } else {
        service_level = x;
      }
      return service_level;
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

    //切换统计方法
    const onChangeAnalysisType = () => {
      render();
    };

    const onChangeService = () => {
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans[roadStates.current_signal].road_info;
      //渲染路
      Object.assign(road_info, rf);
      render();
    };

    onMounted(() => {
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans[roadStates.current_signal].road_info;
      //渲染路
      Object.assign(road_info, rf);
      initRoads();
    });

    return {
      ...toRefs(states),
      ...toRefs(road_info),
      ...toRefs(roadStates),
      lineColumns,
      lineData,
      plans,
      labelCol: { span: 2 },
      wrapperCol: { span: 22 },
      getServiceByTypeAndRatio,
      onChangeAnalysisType,
      onChangeService,
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
