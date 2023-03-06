<template>
  <!-- <div class="basic-main">开发中...</div> -->
  <div class="echarts-box">
    <div id="myEcharts" :style="{ width: '900px', height: '300px' }"></div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, onUnmounted } from "vue";
import Container from "../../../components/Container/index.vue";
import * as echarts from "echarts";

export default defineComponent({
  name: "echartsBox",
  setup() {
    /// 声明定义一下echart
    let echart = echarts;

    onMounted(() => {
      initChart();
    });

    onUnmounted(() => {
      echart.dispose;
    });

    // 基础配置一下Echarts
    function initChart() {
      let chart = echart.init(document.getElementById("myEcharts")!, "dark");
      // 把配置和数据放这里
      chart.setOption({
        xAxis: {
          type: "category",
          data: [
            "一月",
            "二月",
            "三月",
            "四月",
            "五月",
            "六月",
            "七月",
            "八月",
            "九月",
            "十月",
            "十一月",
            "十二月",
          ],
        },
        tooltip: {
          trigger: "axis",
        },
        yAxis: {
          type: "value",
        },
        series: [
          {
            data: [
              820, 932, 901, 934, 1290, 1330, 1320, 801, 102, 230, 4321, 4129,
            ],
            type: "line",
            smooth: true,
          },
        ],
      });
      window.onresize = function () {
        //自适应大小
        chart.resize();
      };
    }

    return { initChart };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
