<template>
  <Container>
    <div class="header">
      <div class="header-name">
        <a-input
          v-model:value="plans.road_name"
          placeholder="请输入交叉口名称"
          style="width: 520px"
        />
        <a-button type="primary" class="ml-5" @click="onSave"> 保存 </a-button>
      </div>
      <div class="header-plan" v-if="currentUrl !== MenuListEnum.Basic">
        <div class="header-plan2">
          <a-select
            v-if="showSelect.showCanalize"
            class="large-form-width"
            placeholder="请选择渠化方案"
            v-model:value="current_canalize"
            @change="changeCanalize(current_canalize, false)"
          >
            <a-select-option
              v-for="(item, index) in plans.canalize_plans"
              :key="index"
              :value="index"
            >
              {{ item.name }}
            </a-select-option>
          </a-select>
          <a-select
            v-if="showSelect.showFlow"
            class="large-form-width ml-5"
            placeholder="请选择流量方案"
            v-model:value="current_flow"
            @change="changeFlow(current_flow, false)"
          >
            <a-select-option
              v-for="(item, index) in plans.canalize_plans[current_canalize]
                .flow_plans"
              :key="index"
              :value="index"
            >
              {{ item.name }}
            </a-select-option>
          </a-select>
          <a-select
            v-if="showSelect.showSignal"
            class="large-form-width ml-5"
            placeholder="请选择信号方案"
            v-model:value="current_signal"
            @change="changeSignal(current_signal, false)"
          >
            <a-select-option
              v-for="(item, index) in plans.canalize_plans[current_canalize]
                .flow_plans[current_flow].signal_plans"
              :key="index"
              :value="index"
            >
              {{ item.name }}
            </a-select-option>
          </a-select>
        </div>
        <div class="header-plan1 ml-5">
          <!-- 渠化方案： -->
          <div class="flex" v-if="currentUrl === MenuListEnum.Canalize">
            <span class="title">渠化方案：</span>
            <div
              class="mr-2"
              v-for="(item, index) in plans.canalize_plans"
              :key="index"
            >
              <a-input
                class="large-form-width2"
                :class="index === current_canalize ? 'form-actitve' : ''"
                placeholder="请输入渠化方案"
                v-model:value="item.name"
                @focus="changeCanalize(index)"
              >
                <template #addonBefore>
                  <copy-outlined
                    class="copy-btn"
                    title="复制"
                    @click="handleAddC(true)"
                  />
                </template>
                <template #addonAfter>
                  <delete-outlined
                    class="minus-btn"
                    title="删除"
                    @click="handleDeleteC(index)"
                  />
                </template>
              </a-input>
            </div>
            <a-button type="default" title="添加" @click="handleAddC(false)">
              <template #icon><plus-outlined /></template>
            </a-button>
          </div>

          <!-- 流量方案： -->
          <div class="flex" v-else-if="currentUrl === MenuListEnum.Flow">
            <span class="title">流量方案：</span>
            <div
              class="mr-2"
              :class="index === current_flow ? 'form-actitve' : ''"
              v-for="(item, index) in plans.canalize_plans[current_canalize]
                .flow_plans"
              :key="index"
            >
              <a-input
                class="large-form-width2"
                placeholder="请输入流量方案"
                v-model:value="item.name"
                @focus="changeFlow(index)"
              >
                <template #addonBefore>
                  <copy-outlined
                    class="copy-btn"
                    title="复制"
                    @click="handleAddF(true)"
                  />
                </template>
                <template #addonAfter>
                  <delete-outlined
                    class="minus-btn"
                    title="删除"
                    @click="handleDeleteF(index)"
                  />
                </template>
              </a-input>
            </div>
            <a-button type="default" title="添加" @click="handleAddF(false)">
              <template #icon><plus-outlined /></template>
            </a-button>
          </div>

          <!-- 信号方案： -->
          <div class="flex" v-else-if="currentUrl === MenuListEnum.Signal">
            <span class="title">信号方案：</span>
            <div
              class="mr-2"
              :class="index === current_signal ? 'form-actitve' : ''"
              v-for="(item, index) in plans.canalize_plans[current_canalize]
                .flow_plans[current_flow].signal_plans"
              :key="index"
            >
              <a-input
                class="large-form-width2"
                placeholder="请输入信号方案"
                v-model:value="item.name"
                @focus="changeSignal(index)"
              >
                <template #addonBefore>
                  <copy-outlined
                    class="copy-btn"
                    title="复制"
                    @click="handleAddS(true)"
                  />
                </template>
                <template #addonAfter>
                  <delete-outlined
                    class="minus-btn"
                    title="删除"
                    @click="handleDeleteS(index)"
                  />
                </template>
              </a-input>
            </div>
            <a-button type="default" title="添加" @click="handleAddS(false)">
              <template #icon><plus-outlined /></template>
            </a-button>
          </div>
        </div>
      </div>
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
      <a-spin :spinning="loading">
        <div class="content-main">
          <!-- 方向 -->
          <Basic v-if="currentUrl === MenuListEnum.Basic" ref="basicRef" />
          <!-- 渠化 -->
          <Canalize
            v-else-if="currentUrl === MenuListEnum.Canalize"
            ref="canalizeRef"
          />
          <!-- 流量 -->
          <Flow v-else-if="currentUrl === MenuListEnum.Flow" ref="flowRef" />
          <!-- 信号 -->
          <Signal
            v-else-if="currentUrl === MenuListEnum.Signal"
            ref="signalRef"
          />
          <!-- 断面 -->
          <Section v-else-if="currentUrl === MenuListEnum.Section" />
          <!-- 饱和度 -->
          <Saturation
            v-else-if="currentUrl === MenuListEnum.Saturation"
            ref="saturationRef"
          />
          <!-- 延误分析 -->
          <DelayAnalysis
            v-else-if="currentUrl === MenuListEnum.DelayAnalysis"
            ref="delayRef"
          />
          <!-- 排队分析 -->
          <QueueAnalysis
            v-else-if="currentUrl === MenuListEnum.QueueAnalysis"
            ref="queueRef"
          />
          <!-- 服务水平 -->
          <ServiceLevel
            v-else-if="currentUrl === MenuListEnum.ServiceLevel"
            ref="serviceRef"
          />
        </div>
      </a-spin>
    </div>
  </Container>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive, ref, toRefs, watch } from "vue";
import {
  MenuListEnum,
  menuList,
  roadStates,
  plans,
  create_road_cross,
  create_flow_detail,
  create_signal_info,
  update_road_corss,
  update_flow_detail,
} from "./index";
import { message } from "ant-design-vue";
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
import { useRoute } from "vue-router";
import { goRouterByParam } from "../../utils/common";
import { PageEnum } from "../../router/data";
import {
  CopyOutlined,
  DeleteOutlined,
  PlusOutlined,
} from "@ant-design/icons-vue";
import _ from "lodash";
import { plans_model, road_model } from "./data";
import { openNotification } from "../../utils/message";
import { saveDesign, getDesignByGuid } from "../../request/api";
import { userStates } from "../UserCenter";

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
    CopyOutlined,
    DeleteOutlined,
    PlusOutlined,
  },

  emits: ["changeMenu"],
  setup(_props, context) {
    const route = useRoute();
    let plan_guid = (route.params.guid ?? "").toString();
    //判断权限
    var token = localStorage.getItem("token");
    if (!token) {
      message.warning("请先登录");
      goRouterByParam(PageEnum.Login);
    }

    //全局道路信息
    const road_info = _.cloneDeep(road_model);

    //子页面
    const basicRef = ref();
    const canalizeRef = ref();
    const flowRef = ref();
    const signalRef = ref();
    const saturationRef = ref();
    const delayRef = ref();
    const queueRef = ref();
    const serviceRef = ref();
    roadStates.currentUrl = MenuListEnum.Basic;
    const currentRouteName = route.name;

    //切换菜单
    const handleChangeMenu = (item: any) => {
      //如果是地图设置背景属性
      if (currentRouteName === PageEnum.Map) {
        setTimeout(() => {
          context.emit("changeMenu");
        }, 5); //延时加载，留时间加载dom
      }
      //道路属性判断
      if (item.url != MenuListEnum.Basic && plans.road_count < 2) {
        openNotification("warning", "相交道路不能少于两条");
        return;
      }
      if (
        item.url !== MenuListEnum.Basic &&
        item.url !== MenuListEnum.Canalize
      ) {
        //切换其余页面时需要初始化渠化信息
        if (road_info.canalize_info.length === 0) {
          create_road_cross(road_info);
        } else {
          update_road_corss(road_info);
        }
      }
      if (
        item.url !== MenuListEnum.Basic &&
        item.url !== MenuListEnum.Canalize &&
        item.url !== MenuListEnum.Flow
      ) {
        //切换其余页面时需要初始化流量信息
        if (road_info.flow_info.flow_detail.length === 0) {
          create_flow_detail(road_info);
        } else {
          update_flow_detail(road_info);
        }
      }
      if (
        !userStates.code_info &&
        !userStates.can_edit &&
        !userStates.is_super_edit &&
        (item.url === MenuListEnum.Saturation ||
          item.url === MenuListEnum.DelayAnalysis ||
          item.url === MenuListEnum.QueueAnalysis ||
          item.url === MenuListEnum.ServiceLevel)
      ) {
        openNotification("warning", "请购买授权码激活后使用");
        return;
      }
      roadStates.currentUrl = item.url;
      changeShowSelect();
    };

    //切换菜单时下拉组件的显示隐藏
    const changeShowSelect = () => {
      if (
        roadStates.currentUrl === MenuListEnum.Basic ||
        roadStates.currentUrl === MenuListEnum.Canalize
      ) {
        roadStates.showSelect.showCanalize = false;
        roadStates.showSelect.showFlow = false;
        roadStates.showSelect.showSignal = false;
      } else if (roadStates.currentUrl === MenuListEnum.Flow) {
        roadStates.showSelect.showCanalize = true;
        roadStates.showSelect.showFlow = false;
        roadStates.showSelect.showSignal = false;
        //流量页面需要依赖渠化页面信息
        update_road_corss(road_info);
      } else if (roadStates.currentUrl === MenuListEnum.Signal) {
        roadStates.showSelect.showCanalize = true;
        roadStates.showSelect.showFlow = true;
        roadStates.showSelect.showSignal = false;
      } else {
        roadStates.showSelect.showCanalize = true;
        roadStates.showSelect.showFlow = true;
        roadStates.showSelect.showSignal = true;
      }
    };

    //添加渠化方案
    const handleAddC = (is_copy = false) => {
      if (plans.canalize_plans.length === 3) {
        openNotification("warning", "最多可增加三种渠化方案");
        return;
      }
      const index = plans.canalize_plans.length;
      const canalize_plan = _.cloneDeep(plans_model.canalize_plans[0]);
      canalize_plan.index = index;
      canalize_plan.name = "渠化方案" + (index + 1);
      const signal_plan = canalize_plan.flow_plans[0].signal_plans[0];
      setRoadInfo(is_copy, signal_plan, "canalize");
      plans.canalize_plans.push(canalize_plan);

      //手动触发初始化
      changeCanalize(index);
    };
    //删除渠化方案
    const handleDeleteC = (index: number) => {
      if (plans.canalize_plans.length === 1) {
        openNotification("warning", "至少保留一条渠化方案");
        return;
      }
      plans.canalize_plans = plans.canalize_plans.filter(
        (_, idx) => index != idx
      );
      roadStates.current_canalize =
        roadStates.current_canalize > plans.canalize_plans.length - 1
          ? plans.canalize_plans.length - 1
          : roadStates.current_canalize;
    };

    //添加流量
    const handleAddF = (is_copy = false) => {
      const flow_plans =
        plans.canalize_plans[roadStates.current_canalize].flow_plans;
      if (flow_plans.length === 3) {
        openNotification("warning", "最多可增加三种流量方案");
        return;
      }
      const index = flow_plans.length;
      const flow_plan = _.cloneDeep(
        plans_model.canalize_plans[0].flow_plans[0]
      );
      flow_plan.name = "流量方案" + (index + 1);
      setRoadInfo(is_copy, flow_plan.signal_plans[0], "flow");
      flow_plans.push(flow_plan);

      //手动触发初始化
      changeFlow(index);
    };
    //删除流量方案
    const handleDeleteF = (index: number) => {
      if (
        plans.canalize_plans[roadStates.current_canalize].flow_plans.length ===
        1
      ) {
        openNotification("warning", "至少保留一条流量方案");
        return;
      }
      plans.canalize_plans[roadStates.current_canalize].flow_plans =
        plans.canalize_plans[roadStates.current_canalize].flow_plans.filter(
          (_, idx) => index != idx
        );
      const flow_plans =
        plans.canalize_plans[roadStates.current_canalize].flow_plans;
      roadStates.current_flow =
        roadStates.current_flow > flow_plans.length - 1
          ? flow_plans.length - 1
          : roadStates.current_flow;
    };

    //添加信号
    const handleAddS = (is_copy = false) => {
      const signal_plans =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans;
      if (signal_plans.length === 3) {
        openNotification("warning", "最多可增加三种信号方案");
        return;
      }

      const index = signal_plans.length;
      const signal_plan = _.cloneDeep(
        plans_model.canalize_plans[0].flow_plans[0].signal_plans[0]
      );
      signal_plan.name = "信号方案" + (index + 1);
      setRoadInfo(is_copy, signal_plan, "signal");
      signal_plans.push(signal_plan);

      changeSignal(index);
    };
    //删除信号方案
    const handleDeleteS = (index: number) => {
      if (
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans.length === 1
      ) {
        openNotification("warning", "至少保留一条信号方案");
        return;
      }
      plans.canalize_plans[roadStates.current_canalize].flow_plans[
        roadStates.current_flow
      ].signal_plans = plans.canalize_plans[
        roadStates.current_canalize
      ].flow_plans[roadStates.current_flow].signal_plans.filter(
        (_, idx) => index != idx
      );
      const signal_plans =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans;
      roadStates.current_flow =
        roadStates.current_flow > signal_plans.length - 1
          ? signal_plans.length - 1
          : roadStates.current_flow;
    };

    //设置全局路口信息 from,哪个模块在新增
    const setRoadInfo = (is_copy: boolean, signal_plan: any, from: string) => {
      if (is_copy) {
        signal_plan.road_info = _.cloneDeep(road_info);
      } else {
        //模板内容
        const rf = _.cloneDeep(road_model);
        //渠化信息
        if (from === "canalize") {
          create_road_cross(rf);
        } else {
          rf.canalize_info =
            plans.canalize_plans[
              roadStates.current_canalize
            ].flow_plans[0].signal_plans[0].road_info.canalize_info;
        }
        //流量信息
        if (from === "canalize" || from === "flow") {
          create_flow_detail(rf);
        } else {
          rf.flow_info =
            plans.canalize_plans[roadStates.current_canalize].flow_plans[
              roadStates.current_flow
            ].signal_plans[0].road_info.flow_info;
        }
        //信号信息
        create_signal_info(rf);

        signal_plan.road_info = rf;
      }
    };

    //切换或点击渠化时
    const changeCanalize = (index: number, is_load = true) => {
      roadStates.current_canalize = index;
      roadStates.current_flow = 0;
      roadStates.current_signal = 0;
      const rf =
        plans.canalize_plans[index].flow_plans[0].signal_plans[0].road_info;
      //渠化需要做出的反应
      if (is_load) {
        canalizeRef.value.onLoadChange(rf);
      } else {
        changePlan(rf);
      }
    };
    //切换或点击流量时
    const changeFlow = (index: number, is_load = true) => {
      roadStates.current_flow = index;
      roadStates.current_signal = 0;
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[index]
          .signal_plans[0].road_info;
      //流量需要做出的反应
      if (is_load) {
        flowRef.value.onChangeFlow(rf);
      } else {
        changePlan(rf);
      }
    };
    //点击或切换信号时
    const changeSignal = (index: number, is_load = true) => {
      roadStates.current_signal = index;
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[
          roadStates.current_flow
        ].signal_plans[index].road_info;
      if (is_load) {
        signalRef.value.onChangeSignal(rf);
      } else {
        changePlan(rf);
      }
    };

    // 切换方案时页面及分析切换
    const changePlan = (rf: any) => {
      if (roadStates.currentUrl === MenuListEnum.Flow) {
        //流量需要做出的反应
        flowRef.value.onChangeFlow(rf);
      } else if (roadStates.currentUrl === MenuListEnum.Signal) {
        //信号需要做出的反应
        signalRef.value.onChangeSignal(rf);
      } else if (roadStates.currentUrl === MenuListEnum.Saturation) {
        //饱和度需要做出的反应
        saturationRef.value.onChangeSatuation(rf);
      } else if (roadStates.currentUrl === MenuListEnum.DelayAnalysis) {
        //饱和度需要做出的反应
        delayRef.value.onChangeDelay(rf);
      } else if (roadStates.currentUrl === MenuListEnum.QueueAnalysis) {
        //饱和度需要做出的反应
        queueRef.value.onChangeQueue(rf);
      } else if (roadStates.currentUrl === MenuListEnum.ServiceLevel) {
        //饱和度需要做出的反应
        serviceRef.value.onChangeService(rf);
      }
    };

    //保存
    const onSave = () => {
      if (!plans.road_name) {
        openNotification("warning", "请输入交叉口名称");
        return;
      }
      if (!userStates.code_info && !userStates.is_super_edit) {
        openNotification("warning", "请购买授权码激活后使用");
        return;
      }
      roadStates.loading = true;
      const param = {
        guid: plan_guid,
        roadName: plans.road_name,
        designJson: JSON.stringify(plans),
      };
      saveDesign(param)
        .then((res: any) => {
          message.success("保存成功");
          if (roadStates.from_map) {
            plan_guid = res.data;
          } else {
            plan_guid = res.data;
            goRouterByParam(PageEnum.DesignEdit, { guid: res.data });
          }
        })
        .finally(() => {
          roadStates.loading = false;
        });
    };

    //加载道路基础信息
    const init = () => {
      setTimeout(() => {
        basicRef.value.init();
        context.emit("changeMenu");
      }, 10);
    };

    const loadData = (guid: any, from_map: boolean) => {
      //地图加载的话需要赋值给全局guid
      plan_guid = guid;
      //是否从地图过来，保存的时候要不要跳页面
      roadStates.from_map = from_map;
      clearRoadInfo();
      roadStates.currentUrl = MenuListEnum.Basic;
      if (guid) {
        roadStates.loading = true;
        getDesignByGuid({ guid })
          .then((res: any) => {
            const designJson = JSON.parse(res.data.designJson);
            Object.assign(plans, designJson);
            init();
            //同时标记流量已有数据
            roadStates.is_flow_init = true;
          })
          .finally(() => {
            roadStates.loading = false;
          });
      } else {
        Object.assign(plans, _.cloneDeep(plans_model));
        const rf =
          plans.canalize_plans[0].flow_plans[0].signal_plans[0].road_info;
        Object.assign(road_info, rf);
        init();
      }
    };

    onMounted(() => {
      loadData(plan_guid, false);
    });

    /**************路由切换，道路还存在问题修复 *******************/
    //切新路由清空道路信息
    const clearRoadInfo = () => {
      //清空方案信息
      plans.road_attr.length = 0;
      plans.road_count = 0;
      plans.road_name = "";
      plans.center = undefined;
      //清空道路信息
      road_info.canalize_info.length = 0;
      road_info.flow_info.flow_detail.length = 0;
      road_info.flow_info.line_info.length = 0;
      road_info.signal_info.phase_list.length = 0;
      road_info.signal_info.period = 0;
      road_info.saturation_info.length = 0;
      road_info.delay_info.length = 0;
      road_info.queue_info.length = 0;

      const rf =
        plans.canalize_plans[0].flow_plans[0].signal_plans[0].road_info;
      rf.canalize_info.length = 0;
      rf.flow_info.flow_detail.length = 0;
      rf.flow_info.line_info.length = 0;
      rf.signal_info.phase_list.length = 0;
      rf.saturation_info.length = 0;
      rf.delay_info.length = 0;
      rf.queue_info.length = 0;
    };

    watch(
      () => route.name,
      (newName) => {
        if (newName === PageEnum.Design) {
          clearRoadInfo();
          roadStates.currentUrl = MenuListEnum.Basic;
          if (basicRef.value) {
            init();
          }
        }
      },
      { immediate: true }
    );
    /**************路由切换，道路还存在问题修复 *******************/

    return {
      ...toRefs(roadStates),
      ...toRefs(road_info),
      plans,
      basicRef,
      canalizeRef,
      flowRef,
      signalRef,
      saturationRef,
      delayRef,
      queueRef,
      serviceRef,
      MenuListEnum,
      menuList,
      handleChangeMenu,
      handleAddC,
      handleDeleteC,
      handleAddF,
      handleDeleteF,
      handleAddS,
      handleDeleteS,
      changeCanalize,
      changeFlow,
      changeSignal,
      onSave,
      loadData,
    };
  },
});
</script>

<style scoped lang="less">
@import url("./index.less");
</style>
