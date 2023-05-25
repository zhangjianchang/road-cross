<template>
  <div class="form-data">
    <a-form
      :model="data"
      :label-col="{ span: 2 }"
      :wrapper-col="{ span: 22 }"
      :rules="rules"
      ref="formRef"
      labelAlign="left"
    >
      <a-form-item label="原密码" name="originalPassword">
        <a-input-password
          placeholder="请输入原密码"
          v-model:value="data.originalPassword"
          class="pwd-input"
        />
      </a-form-item>
      <a-form-item label="新密码" name="password">
        <a-input-password
          placeholder="请输入新密码"
          v-model:value="data.password"
          class="pwd-input"
        />
      </a-form-item>
      <a-form-item label="确认密码" name="rePassword">
        <a-input-password
          placeholder="请再次输入密码"
          v-model:value="data.rePassword"
          class="pwd-input"
        />
      </a-form-item>
      <a-form-item :wrapper-col="{ offset: 2, span: 16 }">
        <a-button
          type="primary"
          class="pwd-button"
          block
          @click="handleResetPwd"
        >
          确认修改
        </a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { Md5 } from "ts-md5";
import { defineComponent, reactive, ref, toRefs } from "vue";
import { reSetPwd } from "../../../../request/api";
import { openNotification } from "../../../../utils/message";

export default defineComponent({
  components: {},
  setup() {
    const formRef = ref();
    const states = reactive({
      data: {
        originalPassword: "",
        password: "",
        rePassword: "",
      },
      rules: {
        originalPassword: [
          { required: true, message: "请输入原始密码", trigger: "blur" },
        ],
        password: [
          { required: true, message: "请输入新密码", trigger: "blur" },
        ],
        rePassword: [
          { required: true, message: "请输入确认密码", trigger: "blur" },
        ],
      },
    });

    const handleResetPwd = () => {
      formRef.value.validate().then(() => {
        if (states.data.password !== states.data.rePassword) {
          openNotification("warning", "两次输入密码不一致");
          return;
        }
        const param = {
          originalPassword: Md5.hashStr(states.data.originalPassword),
          password: Md5.hashStr(states.data.password),
          rePassword: Md5.hashStr(states.data.rePassword),
        };
        reSetPwd(param).then(() => {
          message.success("重置成功");
        });
      });
    };

    return {
      ...toRefs(states),
      formRef,
      handleResetPwd,
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
</style>
