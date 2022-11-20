<template>
  <div class="basic-main">
    <div class="drag">
      <a-tooltip>
        <template #title>
          {{ isDrag ? "取消拖拽模式" : "切换拖拽模式" }}
        </template>
        <drag-outlined
          :style="{ fontSize: '24px', color: isDrag ? '#4f48ad' : '' }"
          @click="isDrag = !isDrag"
        />
      </a-tooltip>
    </div>
    <!-- 图示 -->
    <svg
      id="canvas"
      :style="{ cursor: isDrag ? 'move' : '' }"
      @click="(event) => onClick(event)"
      @mousemove="(e) => onMouseMove(e)"
      @mouseup="(e) => onMouseUp(e)"
    >
      <!-- 箭头 -->
      <defs>
        <marker
          id="arrow"
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
        :columns="columns"
        :data-source="roadDir"
        :pagination="false"
        :scroll="{ x: '100%' }"
        bordered
      />
      <div class="button">
        <a-button
          type="primary"
          class="redraw-button"
          @click="onRedraw"
          :disabled="roadDir.length === 0"
        >
          重新绘制
        </a-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import {
  createVNode,
  defineComponent,
  inject,
  onMounted,
  reactive,
  toRefs,
} from "vue";
import { isPointInCircle, getCoordinate, setLineXY, columns } from "./index";
import Container from "../../../components/Container/index.vue";
import { Modal, notification } from "ant-design-vue";
import { DragOutlined, ExclamationCircleOutlined } from "@ant-design/icons-vue";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    const roadDir = inject("RoadDir") as any[];

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      radius: 260, //线长度
      isDrag: false, //当前模式属于拖拽还是建点
      //拖动相关
      currentLine: null as any,
      dragging: false,
      dragId: "",
    });

    function render() {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
          for (let i = 0; i < 360; i++) {
            let tick_len = 8; // 小刻度长度=8
            if (i % 5 == 0) tick_len = 16; // 长刻度=16
            let x1, y1, x2, y2; // 直线的2个端点
            x1 = Math.sin((Math.PI / 180) * i) * (300 - tick_len) + 350;
            y1 = Math.cos((Math.PI / 180) * i) * (300 - tick_len) + 350;
            x2 = Math.sin((Math.PI / 180) * i) * 300 + 350; // 大圆半径400
            y2 = Math.cos((Math.PI / 180) * i) * 300 + 350;
            let line = document.createElementNS(states.ns, "line"); // 创建SVG元素
            setLineXY(line, x1, y1, x2, y2);
            states.cvs?.appendChild(line);
          }
    }

    //编辑页进来需要反显线段
    function initIines() {
      if (roadDir.length > 0) {
        roadDir.map((road, index) => {
          createLine(road.coordinate, index);
        });
      }
    }

    function onClick(evt: any) {
      //拖动模式返回
      if (states.isDrag) {
        return;
      }
      let x = evt.offsetX;
      let y = evt.offsetY;
      //圆外不进行绘制
      if (!isPointInCircle([x, y], [states.cx, states.cy], 300)) {
        return;
      }
      // 绘制路口，路口数量2-5
      if (roadDir.length >= 5) {
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
      let index = roadDir.length;
      createLine(coordinate, index);

      //存数据至全局变量
      roadDir.push({
        index: index + 1,
        position: `X:${coordinate[0]}\n Y:${coordinate[1]}`,
        id: "line_" + index,
        coordinate,
      });
    }

    function createLine(coordinate: number[], index: number) {
      let line = document.createElementNS(states.ns, "line"); // 创建SVG元素
      let id = "line_" + index;
      setLineXY(line, states.cx, states.cy, coordinate[0], coordinate[1]);
      line.setAttribute("id", id);
      line.setAttribute("stroke-width", "15px");
      line.setAttribute("marker-end", "url(#arrow)");
      //事件
      line.addEventListener("mousedown", onMouseDown, false);
      states.cvs?.appendChild(line);
    }

    function onMouseDown(e: any) {
      states.currentLine = e.path[0];
      const event = window.event || e;
      states.dragging = true;
      states.dragId = event.path[0].id;
    }

    function onMouseMove(e: any) {
      const event = e || window.event;
      if (!states.dragging || !states.isDrag) {
        return;
      }
      let nx = event.offsetX;
      let ny = event.offsetY;
      //圆外不进行捕捉
      if (!isPointInCircle([nx, ny], [states.cx, states.cy], 300)) {
        return;
      }
      let coordinate = getXYByNxNy(nx, ny);
      setLineXY(
        states.currentLine,
        states.cx,
        states.cy,
        coordinate[0],
        coordinate[1]
      );
      setRoadDir(coordinate);
    }

    function onMouseUp(e: any) {
      states.dragging = false;
      const event = e || window.event;
      let nx = event.offsetX;
      let ny = event.offsetY;
      //圆外不进行捕捉
      if (!isPointInCircle([nx, ny], [states.cx, states.cy], 300)) {
        return;
      }
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
    }

    function setRoadDir(coordinate: number[]) {
      roadDir.map((c) => {
        if (c.id === states.dragId) {
          c.position = `X:${coordinate[0]}\n\n Y:${coordinate[1]}`;
          c.coordinate = coordinate;
        }
      });
    }

    //根据当前点位获取在圆上的点
    function getXYByNxNy(nx: number, ny: number) {
      return getCoordinate(states.cx, states.cy, states.radius, nx, ny);
    }

    //重新绘制
    function confirm() {
      // render();
      roadDir.length = 0;
      document.querySelectorAll("line").forEach((e) => {
        if (e.id) e.remove();
      });
    }

    const onRedraw = () => {
      Modal.confirm({
        title: "确定重新绘制",
        icon: createVNode(ExclamationCircleOutlined),
        content: "确定后将清空所有已做操作",
        okText: "确认",
        cancelText: "取消",
        onOk() {
          confirm();
        },
      });
    };

    onMounted(() => {
      render();
      initIines();
    });

    return {
      ...toRefs(states),
      roadDir,
      columns,
      onRedraw,
      onClick,
      onMouseDown,
      onMouseMove,
      onMouseUp,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
