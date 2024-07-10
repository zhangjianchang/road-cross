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
    <div class="code" v-if="code_info && user_info.roleId !== 5">
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
      <router-link
        @click="handleRouterClick(PageEnum.DesignRoadList)"
        to="/designRoadList"
      >
        项目列表
      </router-link>
      <a-dropdown placement="bottomLeft" size="large">
        <router-link class="ant-dropdown-link" @click.prevent to="/userCenter">
          <a-avatar :src="user_info.avatar" />
        </router-link>
        <template #overlay>
          <a-menu class="user-menu-item">
            <a-menu-item>
              <a
                @click="handleRouterClick(PageEnum.UserCenter)"
                :title="user_info.eMail"
              >
                {{ user_info.eMail }}
                <span v-if="user_info.roleId === 5">(企业管理员)</span>
              </a>
            </a-menu-item>
            <a-menu-divider />
            <a-menu-item>
              <a @click="handleRouterClick(PageEnum.BasicInfo)"> 账户信息 </a>
            </a-menu-item>
            <a-menu-item>
              <a @click="handleRouterClick(PageEnum.ResetPwd)"> 修改密码 </a>
            </a-menu-item>
            <a-menu-item>
              <a @click="handleRouterClick(PageEnum.ActivateCode)">
                激活授权码
              </a>
            </a-menu-item>
            <a-menu-item v-if="user_info.roleId === 5">
              <a @click="handleRouterClick(PageEnum.ConfigureSubAccount)">
                配置子账号
              </a>
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
import { defineComponent, onMounted, toRefs } from "vue";
import { UserOutlined } from "@ant-design/icons-vue";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";
import { message } from "ant-design-vue";
import { getCodeInfo, getUserInfo, userLogout } from "../../request/api";
import { userStates } from "../UserCenter";
import { basic_config } from "../../request/http";

export default defineComponent({
  components: { UserOutlined },
  setup() {
    const init = () => {
      getUserInfo().then((res: any) => {
        const imgUrl = `${basic_config.img_url}/avatar`;
        res.data.avatar = `${imgUrl}/${res.data.avatar}`;
        userStates.user_info = res.data;
        if (res.data.roleId === 1) {
          userStates.is_super_edit = true;
        }
      });
      getCodeInfo().then((res: any) => {
        userStates.code_info = res.data;
      });
    };

    //路由跳转
    const handleRouterClick = (routerName: string) => {
      var token = localStorage.getItem("token");
      if (
        !token &&
        (routerName === PageEnum.Design ||
          routerName === PageEnum.UserCenter ||
          routerName === PageEnum.Map)
      ) {
        //判断是否需要登录权限
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
