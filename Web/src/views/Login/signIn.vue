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
          <a-form-item label="确认密码" name="password" class="login-item">
            <a-input-password
              placeholder="请再次输入密码"
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
              注册
            </a-button>
          </a-form-item>
        </a-form>
      </div>
      <div class="login-create" @click="gotoLogIn">已有账号，跳转登录</div>
    </div>
  </div>
</template>
  
  <script lang="ts">
import { defineComponent, reactive } from "vue";
import { userLogin } from "../../request/api";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";

export default defineComponent({
  setup() {
    const formState = reactive({
      username: "",
      password: "",
    });

    const handleLogin = () => {
      userLogin(formState).then((res: any) => {
        console.log(1111);
      });
    };

    const gotoLogIn = () => {
      goRouterByParam(PageEnum.Login);
    };

    return {
      formState,
      handleLogin,
      gotoLogIn,
    };
  },
});
</script>
  
<style scoped lang="less">
@import url("./index.less");
</style>