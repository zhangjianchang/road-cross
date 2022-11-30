<template>
  <div class="basic-main">开发中</div>
</template>

<script lang="ts">
import { defineComponent, inject } from "vue";
import Container from "../../../components/Container/index.vue";
import { DragOutlined } from "@ant-design/icons-vue";
import { RoadInfo } from "..";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    const roadDir = inject("RoadDir") as any[];
    //全局道路信息
    const road_info = inject("road_info") as RoadInfo;

    return {};
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
