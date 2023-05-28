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
        <a-tab-pane :key="1" tab="基本信息">
          <BasicInfo />
        </a-tab-pane>
        <a-tab-pane :key="2" tab="修改密码">
          <ResetPwd />
        </a-tab-pane>
        <a-tab-pane :key="3" tab="激活授权码">
          <ActivateCode />
        </a-tab-pane>
        <a-tab-pane v-if="userInfo.roleId === 1" :key="4" tab="生成授权码">
          <GenerateCode />
        </a-tab-pane>
      </a-tabs>
    </div>
  </Container>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, onMounted, reactive, toRefs, watch } from "vue";
import { PageEnum } from "../../../router/data";
import { goRouterByParam } from "../../../utils/common";
import Container from "../../../components/Container/index.vue";
import BasicInfo from "./BasicInfo/index.vue";
import ResetPwd from "./ResetPwd/index.vue";
import ActivateCode from "./ActivateCode/index.vue";
import GenerateCode from "./GenerateCode/index.vue";
import { checkToken, getUserInfo } from "../../../request/api";
import { settingStates } from ".";
import { useRoute } from "vue-router";
import { basic_config } from "../../../request/http";

export default defineComponent({
  components: {
    Container,
    BasicInfo,
    ResetPwd,
    ActivateCode,
    GenerateCode,
  },
  setup() {
    const route = useRoute();
    let type = (route.params.type ?? "").toString();

    const states = reactive({
      activeKey: 1,
    });

    const initUserInfo = () => {
      //判断权限
      var token = localStorage.getItem("token");
      if (!token) {
        message.warning("请先登录");
        goRouterByParam(PageEnum.Login);
      } else {
        checkToken()
          .then(() => {
            getUserInfo().then((res: any) => {
              const imgUrl = `${basic_config.img_url}/avatar`;
              res.data.avatar = `${imgUrl}/${res.data.avatar}`;
              settingStates.userInfo = res.data;
              if (type === "basic_info") {
                states.activeKey = 1;
              } else if (type === "reset_pwd") {
                states.activeKey = 2;
              } else if (type === "activate_code") {
                states.activeKey = 3;
              }
            });
          })
          .catch(() => {
            localStorage.removeItem("token");
            localStorage.removeItem("userInfo");
            goRouterByParam(PageEnum.Login);
          });
      }
    };

    watch(
      () => route.name,
      (newName) => {
        if (newName === PageEnum.BasicInfo) {
          states.activeKey = 1;
        } else if (newName === PageEnum.ResetPwd) {
          states.activeKey = 2;
        } else if (newName === PageEnum.ActivateCode) {
          states.activeKey = 3;
        }
      },
      { immediate: true }
    );

    onMounted(() => {
      initUserInfo();
    });

    return {
      ...toRefs(states),
      ...toRefs(settingStates),
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
