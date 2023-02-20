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
    </div>
    <div class="login" v-if="!userInfo?.userName">
      <router-link to="/login">
        <a-tooltip color="#f50" placement="left" title="点击登录">
          <user-outlined />
        </a-tooltip>
      </router-link>
      <!-- <router-link to="/contact">创建账号</router-link> -->
    </div>
    <div v-else>
      <a-dropdown placement="bottomLeft" size="large">
        <a class="ant-dropdown-link" @click.prevent>
          {{ userInfo.userName }}
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
import { defineComponent, ref } from "vue";
import { UserOutlined } from "@ant-design/icons-vue";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";

export default defineComponent({
  components: { UserOutlined },
  setup() {
    const userInfo = ref({}) as any;
    var strUser = localStorage.getItem("userInfo");
    if (strUser) {
      userInfo.value = JSON.parse(strUser);
    }
    //路由跳转
    const handleRouterClick = (routerName: string) => {
      goRouterByParam(routerName);
    };
    const handleLogout = () => {
      localStorage.removeItem("userInfo");
      localStorage.removeItem("token");
      goRouterByParam(PageEnum.Login);
    };
    return {
      userInfo,
      PageEnum,
      handleRouterClick,
      handleLogout,
    };
  },
});
</script>

<style lang="less" scoped>
@import "./index.less";
</style>
