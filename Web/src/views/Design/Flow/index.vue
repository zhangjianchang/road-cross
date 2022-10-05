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
    <svg id="canvas">
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
      <!-- 方向一 -->
      <path
        d="M 300 90 L 300 610"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />
      <path
        d="M 300 90 L 300 190 Q 300 400 510 400 L 610 400"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />
      <path
        d="M 300 90 L 300 190 Q 300 300 190 300 L 90 300"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />

      <!-- 方向二 -->
      <path
        d="M 610 300 L 90 300"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />
      <path
        d="M 610 300 L 510 300 Q 300 300 300 510 L 300 610"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />
      <path
        d="M 610 300 L 510 300 Q 400 300 400 190 L 400 90"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />

      <!-- 方向三 -->
      <path
        d="M 400 610 L 400 90"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />
      <path
        d="M 400 610 L 400 560 Q 400 300 140 300 L 90 300"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />
      <path
        d="M 400 610 L 400 510 Q 400 400 510 400 L 610 400"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />

      <!-- 方向四 -->
      <path
        d="M 90 400 L 610 400"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />
      <path
        d="M 90 400 L 190 400 Q 300 400 300 510 L 300 610"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
      />
      <path
        d="M 90 400 L 190 400 Q 400 400 400 190 L 400 90"
        stroke="#4f48ad"
        stroke-width="5"
        fill="none"
        marker-end="url(#arrow)"
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
          @click="onReset"
          :disabled="roadDir.length === 0"
        >
          数据重置
        </a-button>
        <a-button
          type="default"
          class="redraw-button ml-5"
          @click="onReset"
          :disabled="roadDir.length === 0"
        >
          输出图片
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
import { columns } from "./index";
import Container from "../../../components/Container/index.vue";
import { Modal, notification } from "ant-design-vue";
import { DragOutlined, ExclamationCircleOutlined } from "@ant-design/icons-vue";
import { stat } from "fs/promises";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    const roadDir = inject("RoadDir") as any[];

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      isDrag: false,
    });

    const initIines = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      //起始点
      roadDir.map((road1, index1) => {
        //终点
        roadDir.map((road2, index2) => {
          if (index1 !== index2) {
            createLine(road1.coordinate, road2.coordinate);
          }
        });
      });
    };

    //两点求路径
    function createLine(x1y1: number[], x2y2: number[]) {
      var path = document.createElementNS(states.ns, "path"); // 创建SVG元素
      const d = getDByPostion(x1y1, x2y2);
      path.setAttribute("d", d);
      path.setAttribute("stroke", "#4f48ad");
      path.setAttribute("stroke-width", "5");
      path.setAttribute("marker-end", "url(#arrow)");
      path.setAttribute("fill", "none");
      states.cvs?.appendChild(path);
    }

    function getDByPostion(x1y1: number[], x2y2: number[]) {
      let start = `${x1y1[0] - 50} ${x1y1[1]}`;
      let startL = `${x1y1[0] - 50} ${x1y1[1] + 100}`;
      let center = `${states.cx - 50} ${states.cy + 50}`;
      let endL = `${x2y2[0] - 100} ${x2y2[1] + 50}`;
      let end = `${x2y2[0]} ${x2y2[1] + 50}`;
      console.log(`M ${start} L ${startL} Q ${center} ${endL} L ${end}`);
      return `M ${start} L ${startL} Q ${center} ${endL} L ${end}`;
    }

    //
    const onReset = () => {
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

    const confirm = () => {
      console.log(1111);
    };

    onMounted(() => {
      // initIines();
    });

    return {
      ...toRefs(states),
      roadDir,
      columns,
      onReset,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>