<template>
  <div id="nav">
    <div class="menu">
      <router-link to="/">首页</router-link>
      <router-link to="/design">设计</router-link>
      <router-link to="/doc">文档</router-link>
      <router-link to="/authorize">软件授权</router-link>
      <router-link to="/contact">联系我们</router-link>
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
      <router-link to="/userCenter"> {{ userInfo?.userName }}</router-link>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { UserOutlined } from "@ant-design/icons-vue";

export default defineComponent({
  components: { UserOutlined },
  setup() {
    const userInfo = ref(undefined) as any;
    var strUser = localStorage.getItem("userInfo");
    console.log(strUser);

    if (strUser) {
      userInfo.value = JSON.parse(strUser);
      console.log(userInfo);
    }
    return {
      userInfo,
    };
  },
});
</script>

<style lang="less" scoped>
@import "./index.less";
</style>
