<template>
  <div class="main">
    <div class="login">
      <!-- <div class="login-text">登录</div> -->
      <div class="login-modal">
        <a-form
          name="basic"
          :label-col="{ span: 24 }"
          :wrapper-col="{ span: 24 }"
          autocomplete="off"
        >
          <a-form-item label="用户名" name="username" class="login-item">
            <a-input
              v-model:value="formState.username"
              placeholder="请输入用户名"
              class="login-input"
            />
          </a-form-item>
          <a-form-item label="密码" name="password" class="login-item">
            <a-input-password
              placeholder="请输入密码"
              v-model:value="formState.password"
              class="login-input"
            />
          </a-form-item>
          <a-form-item :wrapper-col="{ offset: 0, span: 24 }">
            <a-button
              type="primary"
              class="login-button"
              block
              @click="handleLogin"
            >
              登录
            </a-button>
          </a-form-item>
        </a-form>
      </div>
      <div class="login-create" @click="gotoSignIn">还没账号？立即创建</div>
    </div>
  </div>
</template>

<script lang="ts">
import { message, notification } from "ant-design-vue";
import { defineComponent, reactive } from "vue";
import { getCodeInfo, userLogin } from "../../request/api";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";
import { Md5 } from "ts-md5";
import { userStates } from "../UserCenter";

export default defineComponent({
  setup() {
    const formState = reactive({
      username: "",
      password: "",
    });

    const handleLogin = () => {
      const param = {
        username: formState.username,
        password: Md5.hashStr(formState.password),
      };
      userLogin(param).then((res: any) => {
        if (res.code === 100) {
          message.success("登录成功");
          localStorage.setItem("userInfo", JSON.stringify(res.data));
          localStorage.setItem("token", res.data.token);
          userStates.user_info = res.data;
          //超級管理員權限
          if (userStates.user_info.roleId === 1) {
            userStates.is_super_edit = true;
          }
          //加载授权信息
          getCodeInfo().then((res: any) => {
            userStates.code_info = res.data;
          });
          goRouterByParam(PageEnum.UserCenter);
        } else {
          notification["error"]({
            message: "错误提醒",
            description: res.msg,
            duration: 10,
          });
        }
      });
    };

    const gotoSignIn = () => {
      goRouterByParam(PageEnum.SignIn);
    };

    return {
      formState,
      handleLogin,
      gotoSignIn,
    };
  },
});
</script>

<style scoped lang="less">
@import url("./index.less");
</style>
