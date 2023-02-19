<template>
  <div class="header">
    <Menu />
  </div>
  <div class="container">
    <router-view></router-view>
  </div>
</template>

<script lang="ts">
import { defineComponent, provide } from "vue";
import Menu from "./views/Menu/menu.vue";
export default defineComponent({
  components: { Menu },
  setup() {
    //菜单切换强行刷新页面
    const routerRefresh = () => {
      location.reload();
    };
    //提供子组件调用
    provide("routerRefresh", routerRefresh);
    return {};
  },
});
</script>
