<template>
  <div class="basic-main">开发中</div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Container from "../../../components/Container/index.vue";
import { DragOutlined } from "@ant-design/icons-vue";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    return {};
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
