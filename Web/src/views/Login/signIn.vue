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
          :model="formState"
          :rules="rules"
        >
          <a-form-item label="邮箱" name="eMail" class="login-item">
            <a-input
              v-model:value="formState.eMail"
              placeholder="请输入邮箱（接收授权码）"
              class="login-input"
            />
          </a-form-item>
          <a-form-item label="用户名" name="userName" class="login-item">
            <a-input
              v-model:value="formState.userName"
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
          <a-form-item label="确认密码" name="rePassword" class="login-item">
            <a-input-password
              placeholder="请再次输入密码"
              v-model:value="formState.rePassword"
              class="login-input"
            />
          </a-form-item>
          <a-form-item label="昵称" name="chineseName" class="login-item">
            <a-input
              v-model:value="formState.chineseName"
              placeholder="请输入昵称（选填）"
              class="login-input"
            />
          </a-form-item>
          <a-form-item label="联系电话" name="telPhone" class="login-item">
            <a-input
              v-model:value="formState.telPhone"
              placeholder="请输入联系电话（选填）"
              class="login-input"
            />
          </a-form-item>
          <a-form-item :wrapper-col="{ offset: 0, span: 24 }">
            <a-button
              type="primary"
              class="login-button"
              block
              @click="handleSignIn"
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
import { message } from "ant-design-vue";
import { defineComponent, reactive } from "vue";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";
import { openNotfication } from "../../utils/message";

export default defineComponent({
  setup() {
    const formState = reactive({
      userName: "",
      password: "",
      rePassword: "",
      chineseName: "",
      eMail: "",
      telPhone: "",
    });

    const rules = reactive({
      eMail: [{ required: true, message: "必填", trigger: "blur" }],
      userName: [{ required: true, message: "必填", trigger: "blur" }],
      password: [{ required: true, message: "必填", trigger: "blur" }],
      rePassword: [{ required: true, message: "必填", trigger: "blur" }],
    });

    const handleSignIn = () => {
      if (formState.password !== formState.rePassword) {
        openNotfication("warning", "两次输入的密码不一致");
        return;
      }
      message.warning("内测阶段暂未开放用户注册功能");
    };

    const gotoLogIn = () => {
      goRouterByParam(PageEnum.Login);
    };

    return {
      formState,
      rules,
      handleSignIn,
      gotoLogIn,
    };
  },
});
</script>

<style scoped lang="less">
@import url("./index.less");
</style>
