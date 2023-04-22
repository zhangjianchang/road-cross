<template>
  <div>
    <a-form
      :label-col="{ span: 2 }"
      :wrapper-col="{ span: 22 }"
      labelAlign="left"
    >
      <a-form-item label="登录账号">
        {{ userInfo.userName }}
      </a-form-item>
      <a-form-item label="用户名">
        {{ userInfo.chineseName }}
      </a-form-item>
      <a-form-item label="账号类型">
        {{ userInfo.roleName }}
      </a-form-item>
      <a-form-item label="联系邮箱">
        {{ userInfo.eMail }}
      </a-form-item>
      <a-form-item label="联系电话">
        {{ userInfo.telPhone }}
      </a-form-item>
    </a-form>
  </div>
</template>

<script lang="ts">
import { defineComponent, toRefs } from "vue";
import { settingStates } from "..";

export default defineComponent({
  components: {},
  setup() {
    return {
      ...toRefs(settingStates),
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
</style>
