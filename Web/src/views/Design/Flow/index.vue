<template>
  <div class="basic-main">
    <div class="func">功能区</div>
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
          />
        </marker>
      </defs>
      <!-- 文字路径 -->
      <!-- <defs>
        <path
          v-for="text in road_texts"
          :key="text.id"
          :id="text.id"
          :d="text.path"
        ></path>
      </defs> -->
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
                    v-model:value="flow_info.colorScheme"
                    size="small"
                    class="form-width"
                    @change="onChangeFlow"
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
              <a-col :span="12">
                <a-form-item label="粗细">
                  <a-input-number
                    v-model:value="flow_info.thickness"
                    :min="1"
                    :max="8"
                    size="small"
                    class="form-width"
                    @change="onChangeFlow"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="长度">
                  <a-input-number
                    v-model:value="flow_info.width"
                    :min="50"
                    :max="120"
                    :step="1"
                    size="small"
                    class="form-width"
                    @change="onChangeFlow"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="间距">
                  <a-input-number
                    v-model:value="flow_info.space"
                    :min="20"
                    :max="40"
                    :step="1"
                    size="small"
                    class="form-width"
                    @change="onChangeFlow"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="字号1">
                  <a-input-number
                    v-model:value="flow_info.font_size1"
                    :min="10"
                    :max="18"
                    :step="2"
                    size="small"
                    class="form-width"
                    @change="onChangeFlow"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="字号2">
                  <a-input-number
                    v-model:value="flow_info.font_size2"
                    :min="14"
                    :max="18"
                    :step="2"
                    size="small"
                    class="form-width"
                    @change="onChangeFlow"
                  />
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="header mt-2">车道属性</div>
        <div class="content mt-2" v-if="is_init">
          <a-table
            :dataSource="flow_info.line_info"
            :columns="lineColumns"
            :pagination="false"
            :bordered="true"
            :customRow="onRowClick"
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
                @change="onChangeFlow"
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
        <div class="header mt-5">进口道转向流量</div>
        <div class="content mt-2" v-if="is_init">
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
                <div>
                  <a-input-number
                    v-model:value="
                      flow_info.flow_detail[index].turn[Number(col)].number
                    "
                    :min="0"
                    :max="9999"
                    :step="10"
                    size="small"
                    class="small-form-width"
                    @change="onChangeFlow"
                  />
                </div>
              </div>
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
} from "../../../utils/common";
import {
  getCurvByAngle,
  lineColumns,
  flowColumnsPart,
  lineInfoModel,
  flowDataIndex,
  getLineWidth,
  getK,
  colorSchemes,
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
      currentRoad: 0, //当前选中道路
    });

    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      render();
    };

    async function render() {
      //道路数据填充
      await initRoadInfo();
      //进口车道设置
      initEnterNum();
      //设置节点数据
      setRoadPoint();
      //画路
      drawMainRoad();
    }

    function initEnterNum() {
      road_info.flow_info.saturation.length = 0;
      road_info.canalize_info.forEach((item) => {
        const currentSaturation = [];
        for (let i = 0; i < item.enter.num; i++) {
          currentSaturation.push({ number: 1650 });
        }
        road_info.flow_info.saturation.push(currentSaturation);
      });
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
          const pt_fl = getPoint("fl", angle, x3, y3, road_r);
          left_line.push(pt_fl);
          //道路右侧
          const pt_fr = getPoint("fr", angle, x3, y3, road_r);
          right_line.push(pt_fr);
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

    //填充表格
    async function initRoadInfo() {
      if (!states.is_init) {
        //道路有更换的时候重新绘制
        // if (road_info.road_attr.length !== road_info.basic_info.count) {
        initFlowDetail();
        // }
        //表格中方向绘制
        // initDirections();
      }
      //标记已经初始化过了
      states.is_init = true;
    }

    function initFlowDetail() {
      road_info.flow_info.flowColumns.length = 0;
      Object.assign(road_info.flow_info.flowColumns, flowColumnsPart);
      for (let i = 0; i < road_info.road_attr.length; i++) {
        let dataIndex = i.toString();
        road_info.flow_info.flowColumns.push({
          title: "转向" + (i + 1),
          dataIndex: dataIndex,
          width: 30,
          slots: { customRender: dataIndex },
          align: "center",
        });
        flowDataIndex.push(dataIndex);
      }
      //先清空
      road_info.flow_info.line_info.length = 0;
      road_info.flow_info.flow_detail.length = 0;
      const roadCount = road_info.road_attr.length;
      for (let i = 0; i < roadCount; i++) {
        let line_info = _.cloneDeep(lineInfoModel);
        line_info.direction = "方向" + (i + 1);
        line_info.road_name = road_info.canalize_info[i].name;
        road_info.flow_info.line_info.push(line_info);

        let flow_detail = {} as any;
        flow_detail.road_name = road_info.canalize_info[i].name;
        const turn = [] as any[];
        for (let j = 0; j < roadCount; j++) {
          //转向属性
          const turn_detail = getTurnDetail(i, j);
          turn.push(turn_detail);
        }
        flow_detail.turn = turn;
        //排序
        flow_detail.turn.sort(function (a: any, b: any) {
          return b.order - a.order;
        });
        road_info.flow_info.flow_detail.push(flow_detail);
      }
    }

    //加载各道路之间的方向
    const getTurnDetail = (i: number, j: number) => {
      const roadCount = road_info.road_attr.length;
      const road = road_info.road_attr[i];
      const nextRoad = road_info.road_attr[j];
      //转向属性
      const turn_detail = {
        number: i === j ? 0 : 450,
        d: `M${road.coordinate[0]} ${road.coordinate[1]} L${states.cx} ${states.cy} L${nextRoad.coordinate[0]} ${nextRoad.coordinate[1]}`,
        tag: `${i}#${j}`, //标记从哪个车道到哪个车道
        order: j - i <= 0 ? j - i + roadCount : j - i, //排序（为了把掉头车道放在第一个）
      } as any;
      return turn_detail;
    };

    //绘制道路
    function drawMainRoad() {
      drawMRRoad();
      drawRoadCross();
      drawMLRoad();
      setTimeout(() => drawText(), 30);
    }

    //绘制道路左右两侧(右侧)
    function drawMRRoad() {
      for (let i = 0; i < states.road_lines.length; i++) {
        const flow_count = getFlowCountR(i);
        const line_width = getLineWidth(road_info.road_attr.length, flow_count);
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
          setText(i, midPoint, angle, content);
        }
        //取两侧道路中点标记道路
        const point_l1 = road.middle_line.ml_line[1];
        midPoint = getMiddlePoint(point_l1, point_r1);
        angle = 270 - road_info.road_attr[i].angle;
        content = road_info.canalize_info[i].name;
        setText(i, midPoint, angle, content, "road_name", -15, 70);
      }
    }
    //绘制道路左右两侧(左侧)
    function drawMLRoad() {
      for (let i = 0; i < states.road_lines.length; i++) {
        let flow_count = getFlowCountL(i);
        let line_width = getLineWidth(road_info.road_attr.length, flow_count);
        const road = states.road_lines[i];
        //道路左侧
        let content = 0;
        road_info.flow_info.flow_detail.map((f) => {
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
          setText(i, midPoint, angle, content);
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
          const angle1 = road_info.road_attr[i].angle;
          const angle2 = road_info.road_attr[road2_index].angle;
          const point1 = road1.right_line[k];
          const point2 = road2.left_line[k];
          const curvature = getCurvByAngle(
            states.curvature,
            angle1,
            angle2,
            point1,
            point2
          );
          //道路两两连接
          const line = document.createElementNS(states.ns, "path");
          const Q = getQByPathCurv(point1, point2, curvature);
          let d_str = `M ${point1[0]} ${point1[1]} Q ${Q} ${point2[0]} ${point2[1]}`;
          drawPath(line, `main_path_${i}_${k}`, d_str, road1.color);
          //路径文字
          const tag = `${i}#${road2_index}`;
          const content = road_info.flow_info.flow_detail[i].turn.find(
            (t: any) => t.tag === tag
          ).number;
          const angle = 360 - angle1;
          setText(
            `${i}_${road2_index}`,
            point1,
            angle,
            content,
            "main_road",
            -30,
            7 * k
          );
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
      type = "main_road",
      translateX = -10,
      translateY = 20
    ) {
      const road_text = {
        id: `left_text_${i}`,
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
        if (t.type === "road_name") {
          text.setAttribute(
            "style",
            `font-size:${road_info.flow_info.font_size2}`
          );
        } else {
          text.setAttribute(
            "style",
            `font-size:${road_info.flow_info.font_size1}`
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

    const onChangeFlow = () => {
      clearRoadPath();
      render();
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
      road_info.flow_info.flow_detail.map((f) => {
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
        if (["direction", "direction_arrow"].indexOf(e.id) === -1) e.remove();
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
      initRoads();
    });

    return {
      ...toRefs(states),
      ...toRefs(road_info),
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      lineColumns,
      flowDataIndex,
      onChangeFlow,
      onRowClick,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
