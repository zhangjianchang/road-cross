<template>
  <Container>
    <div class="header">
      <div class="header-logo">LOGO</div>
      <a-input
        v-model:value="name"
        placeholder="请输入交叉口名"
        style="width: 400px"
      />
      <a-button type="primary" class="ml-5" @click="onSave"> 保存 </a-button>
    </div>
    <div class="content">
      <div class="content-menu">
        <div
          class="content-menu-item"
          :class="menu.url === currentUrl ? 'content-menu-item-active' : ''"
          v-for="menu in menuList"
          :key="menu.index"
          @click="handleChangeMenu(menu)"
        >
          {{ menu.name }}
        </div>
      </div>
      <div class="content-main">
        <!-- 方向 -->
        <Basic v-if="currentUrl === MenuListEnum.Basic" />
        <!-- 渠化 -->
        <Canalize v-else-if="currentUrl === MenuListEnum.Canalize" />
        <!-- 流量 -->
        <Flow v-else-if="currentUrl === MenuListEnum.Flow" />
        <!-- 信号 -->
        <Signal v-else-if="currentUrl === MenuListEnum.Signal" />
        <!-- 饱和度 -->
        <Saturation v-else-if="currentUrl === MenuListEnum.Saturation" />
      </div>
    </div>
  </Container>
</template>

<script lang="ts">
import { defineComponent, provide, reactive, toRefs } from "vue";
import { MenuListEnum, menuList } from "./index";
import { notification } from "ant-design-vue";
import Container from "../../components/Container/index.vue";
import Basic from "./Basic/index.vue";
import Flow from "./Flow/index.vue";
import Canalize from "./Canalize/index.vue";
import Saturation from "./Saturation/index.vue";
import Signal from "./Signal/index.vue";

export default defineComponent({
  components: { Container, Basic, Flow, Canalize, Saturation, Signal },
  setup() {
    //全局保存道路定位
    const RoadDir = reactive([] as any[]);
    provide("RoadDir", RoadDir);

    const states = reactive({
      currentUrl: "Basic",
      name: "", //交叉口名
    });

    const handleChangeMenu = (item: any) => {
      if (RoadDir.length < 2) {
        notification["warning"]({
          message: "错误提醒",
          description: "相交道路不能少于两条",
          duration: 10,
        });
        return;
      }
      states.currentUrl = item.url;
    };

    const onSave = () => {
      console.log("保存", states);
    };

    return {
      ...toRefs(states),
      MenuListEnum,
      menuList,
      handleChangeMenu,
      onSave,
    };
  },
});
</script>

<style scoped lang="less">
@import url("./index.less");
</style>
