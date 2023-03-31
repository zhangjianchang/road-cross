<template>
  <div class="basic-main">
    <div class="drag">
      <!-- 功能区 -->
    </div>
    <!-- 图示 -->
    <svg
      id="canvas"
      :style="{ cursor: deleting ? '' : dragging ? 'move' : '' }"
      @click="(event) => onClick(event)"
      @mousemove="(e) => onMouseMove(e)"
      @mouseup="(e) => onMouseUp(e)"
    >
      <!-- 箭头 -->
      <defs>
        <marker
          v-for="(item, index) in plans.road_attr"
          :key="index"
          :id="item.arrowId"
          markerUnits="strokeWidth"
          markerWidth="4"
          markerHeight="4"
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
      <!-- 圆 -->
      <circle
        r="300"
        cx="350"
        cy="350"
        fill="rgba(255,255,255,1)"
        stroke="#4f48ad"
      />
      <circle r="250" cx="350" cy="350" fill="none" stroke="#4f48ad" />
      <circle
        r="40"
        cx="350"
        cy="350"
        fill="none"
        stroke="#4f48ad"
        stroke-width="15"
      />
    </svg>

    <!-- 参数 -->
    <div class="menu">
      <a-table
        class="ant-table-striped"
        :row-class-name="(_:any, index:number) => (index % 2 === 1 ? 'table-striped' : null)"
        :columns="columns"
        :data-source="plans.road_attr"
        :pagination="false"
        :scroll="{ x: '100%' }"
        bordered
      >
        <template #index="{ index }"> 方向{{ index + 1 }} </template>
      </a-table>
      <div class="button">
        <a-button
          type="primary"
          class="redraw-button"
          @click="onRedraw"
          :disabled="plans.road_count === 0"
        >
          重新绘制
        </a-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { createVNode, defineComponent, onMounted, reactive, toRefs } from "vue";
import { isPointInCircle, getCoordinate, setLineXY, columns } from "./index";
import Container from "../../../components/Container/index.vue";
import { Modal, notification } from "ant-design-vue";
import {
  CloseOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons-vue";
import { getAngle } from "../../../utils/common";
import { plans } from "..";
import { road_model } from "../data";

export default defineComponent({
  components: { Container, CloseOutlined },
  setup() {
    //道路信息
    const road_info = reactive(JSON.parse(JSON.stringify(road_model)));
    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      radius: 260, //线长度
      deleting: false, //当前模式属于拖拽还是建点
      //拖动相关
      currentLine: null as any,
      isContextMenu: false,
      dragging: false,
      dragId: "",
      index: 0, //标记线段id，永远叠加
    });

    function render() {
      //先清空
      removeAll();

      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      for (let i = 0; i < 360; i++) {
        let tick_len = 8; // 小刻度长度=8
        if (i % 5 === 0) tick_len = 16; // 长刻度=16
        let x1, y1, x2, y2; // 直线的2个端点
        x1 = Math.sin((Math.PI / 180) * i) * (300 - tick_len) + 350;
        y1 = Math.cos((Math.PI / 180) * i) * (300 - tick_len) + 350;
        x2 = Math.sin((Math.PI / 180) * i) * 300 + 350; // 大圆半径400
        y2 = Math.cos((Math.PI / 180) * i) * 300 + 350;
        let line = document.createElementNS(states.ns, "line"); // 创建SVG元素
        setLineXY(line, x1, y1, x2, y2);
        states.cvs?.appendChild(line);
        let x3 = Math.sin((Math.PI / 180) * i) * 270 + 350; // 大圆半径400
        let y3 = Math.cos((Math.PI / 180) * i) * 270 + 350;
        if (i % 15 === 0) {
          //数字刻度
          const text = document.createElementNS(states.ns, "text");
          const content = i - 90 < 0 ? i + 270 : i - 90;
          const angle = getAngle(states.cx, states.cy, x3, y3);
          const translateX = content <= 10 ? -4 : content <= 100 ? -8 : -10;
          text.setAttribute("x", x3.toString());
          text.setAttribute("y", y3.toString());
          text.setAttribute("fill", "#000");
          text.setAttribute("style", "font-size:12px;user-select:none");
          text.setAttribute(
            "transform",
            `rotate(${90 - angle} ${x3},${y3}) translate(${translateX},0)`
          );
          text.appendChild(document.createTextNode(content.toString())); //文本内容"450"
          states.cvs?.appendChild(text);
        }
      }
    }

    //编辑页进来需要反显线段
    function initIines() {
      if (plans.road_count > 0) {
        plans.road_attr.map((road: any) => {
          road.id = `line_${states.index}`;
          road.arrowId = `arrow${states.index}`;
          createLine(road.coordinate);
          states.index++;
        });
      }
    }

    //清空所有svg信息，重新绘制
    function removeAll() {
      document.querySelectorAll("line").forEach((e) => {
        const is_delete = e.getAttribute("deleteTag") === "1";
        if (is_delete) {
          e.remove();
        }
      });
      document.querySelectorAll("text").forEach((e) => {
        e.remove();
      });
    }

    function onClick(evt: any) {
      if (states.dragging || states.deleting) {
        return;
      }
      let x = evt.offsetX;
      let y = evt.offsetY;
      //圆外不进行绘制
      if (!isPointInCircle([x, y], [states.cx, states.cy], 300)) {
        return;
      }
      // 绘制路口，路口数量2-5
      if (plans.road_count >= 5) {
        notification["warning"]({
          message: "错误提醒",
          description: "系统最多只支持五条相交道路",
          duration: 10,
        });
        return;
      }

      //根据点击点获取在圆上的坐标
      let coordinate = getCoordinate(states.cx, states.cy, states.radius, x, y);
      //画线
      createLine(coordinate);

      //根据坐标获取角度
      let angle = getAngle(states.cx, states.cy, coordinate[0], coordinate[1]);
      //存数据至全局变量
      plans.road_attr.push({
        position: `X:${coordinate[0]}\n Y:${coordinate[1]}`,
        id: `line_${states.index}`,
        arrowId: `arrow${states.index}`,
        coordinate,
        angle,
      });
      setRoadDir(coordinate);
      states.index++;
    }

    function createLine(coordinate: number[]) {
      let line = document.createElementNS(states.ns, "line"); // 创建SVG元素
      let id = "line_" + states.index;
      setLineXY(line, states.cx, states.cy, coordinate[0], coordinate[1]);
      line.setAttribute("id", id);
      line.setAttribute("stroke-width", "15px");
      line.setAttribute("marker-end", `url(#arrow${states.index})`);
      line.setAttribute("tag", `arrow${states.index}`);
      line.setAttribute("deleteTag", "1");
      //事件
      line.addEventListener("contextmenu", onContextMenu, false);
      line.addEventListener("mousedown", onMouseDown, false);
      states.cvs?.appendChild(line);
    }

    //右键事件
    function onContextMenu(e: any) {
      const event = window.event || e;
      states.currentLine = event.target;
      states.dragId = states.currentLine.id;
      plans.road_attr = plans.road_attr.filter((r) => r.id !== states.dragId);
      setRoadDir([], true);
      e.target.remove();
      e.preventDefault();
    }

    //鼠标按下
    function onMouseDown(e: any) {
      if (e.button === 2) return; //右键直接返回
      const event = window.event || e;
      states.currentLine = event.target;
      states.dragId = states.currentLine.id;
      states.dragging = true;
      //加深颜色
      states.currentLine.setAttribute("stroke", "rgb(48 44 102)");
      const arrowId = getArrowId();
      const currentArrow = document.querySelector(`#${arrowId}>path`);
      currentArrow?.setAttribute("style", "fill: rgb(48 44 102)");
    }

    //拖动鼠标
    function onMouseMove(e: any) {
      const event = e || window.event;
      if (!states.dragging || states.deleting) {
        return;
      }
      let nx = event.offsetX;
      let ny = event.offsetY;
      //圆外不进行捕捉
      if (!isPointInCircle([nx, ny], [states.cx, states.cy], 300)) {
        return;
      }
      let coordinate = getXYByNxNy(nx, ny);
      // const stroke = states.dragging ? "rgb(48 44 102)" : "#4f48ad";
      setLineXY(
        states.currentLine,
        states.cx,
        states.cy,
        coordinate[0],
        coordinate[1],
        "rgb(48 44 102)"
      );
      // setRoadDir(coordinate);
    }

    //鼠标抬起
    function onMouseUp(e: any) {
      if (states.deleting || !states.dragging) {
        return; //删除线段时直接返回
      }
      const event = e || window.event;
      let nx = event.offsetX;
      let ny = event.offsetY;
      //圆外不进行捕捉
      if (!isPointInCircle([nx, ny], [states.cx, states.cy], 300)) {
        return;
      }
      setTimeout(() => {
        //箭头颜色释放
        const arrowId = getArrowId();
        const currentArrow = document.querySelector(`#${arrowId}>path`);
        currentArrow?.setAttribute("style", "fill: rgb(79, 72, 173)");
        //重新定位
        let coordinate = getXYByNxNy(nx, ny);
        setLineXY(
          states.currentLine,
          states.cx,
          states.cy,
          coordinate[0],
          coordinate[1]
        );
        setRoadDir(coordinate);
        states.currentLine = null;
        states.dragging = false;
        states.dragId = "";
      }, 30);
    }

    function setRoadDir(coordinate: number[], is_delete = false) {
      if (!is_delete) {
        let angle = getAngle(
          states.cx,
          states.cy,
          coordinate[0],
          coordinate[1]
        );
        plans.road_attr.map((c) => {
          if (c.id === states.dragId) {
            c.position = `X:${coordinate[0]}\n\n Y:${coordinate[1]}`;
            c.coordinate = coordinate;
            c.angle = angle;
          }
        });
      }
      plans.road_attr.sort(function (
        a: { angle: number },
        b: { angle: number }
      ) {
        return a.angle - b.angle;
      });
      plans.road_count = plans.road_attr.length;
    }

    //根据当前点位获取在圆上的点
    function getXYByNxNy(nx: number, ny: number) {
      return getCoordinate(states.cx, states.cy, states.radius, nx, ny);
    }

    //重新绘制
    const onRedraw = () => {
      Modal.confirm({
        title: "确定重新绘制",
        icon: createVNode(ExclamationCircleOutlined),
        content: "确定后将清空所有已做操作",
        okText: "确认",
        cancelText: "取消",
        onOk() {
          location.reload();
        },
      });
    };

    //获取当前路口对应箭头
    function getArrowId() {
      return states.currentLine.getAttribute("tag");
    }

    const init = () => {
      const rf =
        plans.canalize_plans[0].flow_plans[0].signal_plans[0].road_info;
      Object.assign(road_info, rf);
      render();
      initIines();
    };

    onMounted(() => {
      //换在父页面中执行
      init();
    });

    return {
      ...toRefs(states),
      plans,
      init,
      columns,
      onRedraw,
      onClick,
      onMouseMove,
      onMouseUp,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
@import url("../index.less");
</style>
