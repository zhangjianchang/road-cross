<template>
  <Container>
    <div class="header">
      <div class="header-logo">LOGO</div>
      <a-input
        v-model:value="basic_info.name"
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
        <!-- 断面 -->
        <Section v-else-if="currentUrl === MenuListEnum.Section" />
        <!-- 饱和度 -->
        <Saturation v-else-if="currentUrl === MenuListEnum.Saturation" />
        <!-- 排队分析 -->
        <QueueAnalysis v-else-if="currentUrl === MenuListEnum.QueueAnalysis" />
        <!-- 延误分析 -->
        <DelayAnalysis v-else-if="currentUrl === MenuListEnum.DelayAnalysis" />
        <!-- 服务水平 -->
        <ServiceLevel v-else-if="currentUrl === MenuListEnum.ServiceLevel" />
      </div>
    </div>
  </Container>
</template>

<script lang="ts">
import { defineComponent, reactive, toRefs } from "vue";
import { MenuListEnum, menuList, road_info } from "./index";
import { notification } from "ant-design-vue";
import Container from "../../components/Container/index.vue";
import Basic from "./Basic/index.vue"; //基础
import Canalize from "./Canalize/index.vue"; //渠化
import Flow from "./Flow/index.vue"; //流量
import Signal from "./Signal/index.vue"; //信号
import Section from "./Section/index.vue"; //断面
import Saturation from "./Saturation/index.vue"; //饱和度
import QueueAnalysis from "./QueueAnalysis/index.vue"; //排队分析
import DelayAnalysis from "./DelayAnalysis/index.vue"; //延误分析
import ServiceLevel from "./ServiceLevel/index.vue"; //服务水平

export default defineComponent({
  components: {
    Container,
    Basic,
    Flow,
    Canalize,
    Section,
    Saturation,
    Signal,
    QueueAnalysis,
    DelayAnalysis,
    ServiceLevel,
  },
  setup() {
    //道路信息
    const states = reactive({
      currentUrl: "Basic",
    });

    const handleChangeMenu = (item: any) => {
      if (road_info.road_attr.length < 2) {
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
      ...toRefs(road_info),
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
