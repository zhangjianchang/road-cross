<template>
  <Container>
    <div class="header">
      <a-input
        v-model:value="basic_info.name"
        placeholder="请输入交叉口名称"
        style="width: 530px"
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
        <Basic v-if="currentUrl === MenuListEnum.Basic" ref="basicRef" />
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
import { defineComponent, inject, onMounted, reactive, ref, toRefs } from "vue";
import { MenuListEnum, menuList, road_info, roadStates } from "./index";
import { message, notification } from "ant-design-vue";
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
import { buildUUID } from "../../utils/uuid";
import { useRoute } from "vue-router";
import moment from "moment";
import { goRouterByParam } from "../../utils/common";
import { PageEnum } from "../../router/data";

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
    const route = useRoute();
    const id = (route.params.id ?? "").toString();
    //子页面
    const basicRef = ref();
    roadStates.currentUrl = MenuListEnum.Basic;
    //强刷路由
    const routerRefresh = inject("routerRefresh") as any;

    const handleChangeMenu = (item: any) => {
      if (item.url != MenuListEnum.Basic && road_info.road_attr.length < 2) {
        notification["warning"]({
          message: "错误提醒",
          description: "相交道路不能少于两条",
          duration: 10,
        });
        return;
      }
      if (
        item.url != MenuListEnum.Basic &&
        item.url != MenuListEnum.Canalize &&
        road_info.canalize_info.length === 0
      ) {
        notification["warning"]({
          message: "错误提醒",
          description: "请先初始化渠化信息",
          duration: 10,
        });
        return;
      }
      roadStates.currentUrl = item.url;
    };

    const onSave = () => {
      if (!road_info.basic_info.name) {
        notification["warning"]({
          message: "错误提醒",
          description: "请输入交叉口名称",
          duration: 10,
        });
        return;
      }
      if (!road_info.basic_info.count) {
        notification["warning"]({
          message: "错误提醒",
          description: "请绘制道路后再进行保存",
          duration: 10,
        });
        return;
      }
      if (id) {
        road_info.basic_info.updateDate = moment().format(
          "YYYY-MM-DD HH:mm:ss"
        );
        roadStates.road_list.map((item: any) => {
          if (item.basic_info.id === road_info.basic_info.id) {
            item = road_info;
          }
        });
      } else {
        road_info.basic_info.createDate = moment().format(
          "YYYY-MM-DD HH:mm:ss"
        );
        road_info.basic_info.updateDate = moment().format(
          "YYYY-MM-DD HH:mm:ss"
        );
        roadStates.road_list.push(road_info);
        goRouterByParam(PageEnum.DesignEdit, { id: road_info.basic_info.id });
      }
      localStorage.setItem("road_list", JSON.stringify(roadStates.road_list));
      message.success("保存成功");
      routerRefresh();
    };

    onMounted(() => {
      /*TODO: 缓存中的方案，后期改接口*/
      const road_list_str = localStorage.getItem("road_list");
      if (road_list_str) {
        Object.assign(roadStates.road_list, JSON.parse(road_list_str!));
      }
      if (id) {
        roadStates.road_list.map((item) => {
          if (item.basic_info.id === id) {
            Object.assign(road_info, item);
          }
        });
      } else {
        road_info.basic_info.id = buildUUID();
      }
      basicRef.value.init();
      /*TODO: 缓存中的方案，后期改接口*/
    });

    return {
      ...toRefs(roadStates),
      ...toRefs(road_info),
      basicRef,
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
