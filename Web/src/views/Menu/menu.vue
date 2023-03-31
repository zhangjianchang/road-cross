<template>
  <div id="nav">
    <div class="menu">
      <router-link @click="handleRouterClick(PageEnum.Home)" to="/">
        首页
      </router-link>
      <router-link @click="handleRouterClick(PageEnum.Design)" to="/design">
        设计
      </router-link>
      <router-link @click="handleRouterClick(PageEnum.Doc)" to="/doc">
        文档
      </router-link>
      <router-link
        @click="handleRouterClick(PageEnum.Authorize)"
        to="/authorize"
      >
        软件授权
      </router-link>
      <router-link @click="handleRouterClick(PageEnum.Contact)" to="/contact">
        联系我们
      </router-link>
      <router-link @click="handleRouterClick(PageEnum.Map)" to="/map">
        地图模式
      </router-link>
    </div>
    <div class="code" v-if="code_info">
      剩余可用：{{ code_info.remainingDays }}天，{{
        code_info.remainingTimes
      }}次
    </div>
    <div v-if="!user_info?.userName">
      <a-tooltip color="#f50" placement="left" title="点击登录">
        <user-outlined @click="handleLogin" />
      </a-tooltip>
      <!-- <router-link to="/contact">创建账号</router-link> -->
    </div>
    <div v-else>
      <a-dropdown placement="bottomLeft" size="large">
        <a class="ant-dropdown-link" @click.prevent>
          {{ user_info.chineseName }}
        </a>
        <template #overlay>
          <a-menu>
            <a-menu-item>
              <a @click="handleRouterClick(PageEnum.UserCenter)"> 个人中心 </a>
            </a-menu-item>
            <a-menu-divider />
            <a-menu-item>
              <a @click="handleLogout"> 退出登录 </a>
            </a-menu-item>
          </a-menu>
        </template>
      </a-dropdown>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive, ref, toRefs } from "vue";
import { UserOutlined } from "@ant-design/icons-vue";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";
import { message } from "ant-design-vue";
import { getCodeInfo, getUserInfo, userLogout } from "../../request/api";
import { userStates } from "../UserCenter";

export default defineComponent({
  components: { UserOutlined },
  setup() {
    const init = () => {
      getUserInfo().then((res: any) => {
        userStates.user_info = res.data;
        if (res.data.roleId === 1) {
          userStates.can_edit = true;
        }
      });
      getCodeInfo().then((res: any) => {
        userStates.code_info = res.data;
      });
    };

    //路由跳转
    const handleRouterClick = (routerName: string) => {
      //判断权限
      var token = localStorage.getItem("token");
      if (
        !token &&
        (routerName === PageEnum.Design || routerName === PageEnum.UserCenter)
      ) {
        message.warning("请先登录");
        goRouterByParam(PageEnum.Login);
      } else {
        goRouterByParam(routerName);
      }
    };

    //登录
    const handleLogin = () => {
      //先加载
      init();
      if (!userStates.user_info) {
        goRouterByParam(PageEnum.Login);
      }
    };

    //登出
    const handleLogout = () => {
      userLogout().then(() => {
        localStorage.removeItem("userInfo");
        localStorage.removeItem("token");
        userStates.user_info = undefined;
        userStates.code_info = undefined;
        userStates.can_edit = false;
        goRouterByParam(PageEnum.Login);
      });
    };

    onMounted(() => {
      init();
    });

    return {
      ...toRefs(userStates),
      PageEnum,
      handleRouterClick,
      handleLogout,
      handleLogin,
    };
  },
});
</script>

<style lang="less" scoped>
@import "./index.less";
</style>
