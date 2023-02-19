<template>
  <div id="nav">
    <div class="menu">
      <router-link @click="onRouterClick(PageEnum.Home)" to="/">
        首页
      </router-link>
      <router-link @click="onRouterClick(PageEnum.Design)" to="/design">
        设计
      </router-link>
      <router-link @click="onRouterClick(PageEnum.Doc)" to="/doc">
        文档
      </router-link>
      <router-link @click="onRouterClick(PageEnum.Authorize)" to="/authorize">
        软件授权
      </router-link>
      <router-link @click="onRouterClick(PageEnum.Contact)" to="/contact">
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
      <router-link to="/userCenter"> 个人中心 </router-link>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, inject, ref } from "vue";
import { UserOutlined } from "@ant-design/icons-vue";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";

export default defineComponent({
  components: { UserOutlined },
  setup() {
    const userInfo = ref(undefined) as any;
    var strUser = localStorage.getItem("userInfo");
    if (strUser) {
      userInfo.value = JSON.parse(strUser);
    }
    const routerRefresh = inject("routerRefresh") as any;

    //路由跳转
    const onRouterClick = (routerName: string) => {
      goRouterByParam(routerName);
      routerRefresh();
    };
    return {
      userInfo,
      PageEnum,
      onRouterClick,
    };
  },
});
</script>

<style lang="less" scoped>
@import "./index.less";
</style>
