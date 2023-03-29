<template>
  <Container class="main-container">
    <div class="breadcrumb">
      <a-breadcrumb>
        <a-breadcrumb-item
          href=""
          @click="goRouterByParam(PageEnum.UserCenter)"
        >
          个人中心
        </a-breadcrumb-item>
        <a-breadcrumb-item>设置</a-breadcrumb-item>
      </a-breadcrumb>
    </div>

    <div class="content">
      <a-tabs v-model:activeKey="activeKey" tab-position="left" animated>
        <a-tab-pane :key="1" tab="用户基本信息">
          <BasicInfo />
        </a-tab-pane>
        <a-tab-pane :key="2" tab="修改密码">
          <ResetPwd />
        </a-tab-pane>
        <a-tab-pane :key="3" tab="激活授权码">
          <ActivateCode />
        </a-tab-pane>
      </a-tabs>
    </div>
  </Container>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, onMounted, reactive, toRefs } from "vue";
import { PageEnum } from "../../../router/data";
import { goRouterByParam } from "../../../utils/common";
import Container from "../../../components/Container/index.vue";
import BasicInfo from "./BasicInfo/index.vue";
import ResetPwd from "./ResetPwd/index.vue";
import ActivateCode from "./ActivateCode/index.vue";

export default defineComponent({
  components: { Container, BasicInfo, ResetPwd, ActivateCode },
  setup() {
    //判断权限
    var token = localStorage.getItem("token");
    if (!token) {
      message.warning("请先登录");
      goRouterByParam(PageEnum.Login);
    }

    const states = reactive({
      activeKey: 1,
    });

    onMounted(() => {
      // window.location.reload();
    });

    return {
      ...toRefs(states),
      goRouterByParam,
      PageEnum,
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
@import "./index.less";
</style>
