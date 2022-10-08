<template>
  <div class="main">
    <slot></slot>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  setup() {},
});
</script>

<style scoped lang="less">
.main {
  margin: 0 auto;
  margin-top: 100px;
  width: 1440px;
  background-color: #f8f8f8;
  min-height: 830px;
  height: 100%; //calc(100vh - 64px);
  padding: 32px;
  box-shadow: #d0d0d0 0px 0px 15px;
  border-radius: 8px;
}
</style>
