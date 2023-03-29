<template>
  <div class="form-data">
    <div v-if="userInfo.roleId === 1">
      您是系统管理员，可使用系统全部功能，无需激活
    </div>
    <div v-else-if="userInfo.roleId === 5">
      <div class="success-text">您的授权码已激活!</div>
      <a-table
        class="mt-5"
        bordered
        size="small"
        :columns="codeColumns"
        :dataSource="memberInfo"
        :scroll="scrollX(codeColumns)"
        :loading="loading"
        :pagination="false"
      />
    </div>
    <a-form
      v-else
      :model="data"
      :label-col="{ span: 2 }"
      :wrapper-col="{ span: 22 }"
      :rules="rules"
      ref="formRef"
    >
      <a-form-item label="激活码" name="code">
        <a-input
          placeholder="请输入激活码"
          v-model:value="data.code"
          class="pwd-input"
        />
        <a-button
          type="primary"
          class="ml-5 pwd-button"
          block
          @click="handleActivate"
        >
          确认激活
        </a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, onMounted, reactive, ref, toRefs } from "vue";
import { scrollX } from "../../../../utils/common";
import { openNotfication } from "../../../../utils/message";
import { codeColumns } from "./data";

export default defineComponent({
  components: {},
  setup() {
    const formRef = ref();
    const states = reactive({
      loading: false,
      memberInfo: [{ code: "111111111111112" }] as any[],
      userInfo: {} as any,
      data: {
        code: "",
      },
      rules: {
        code: [{ required: true, message: "请输入激活码", trigger: "blur" }],
      },
    });

    const handleActivate = () => {
      formRef.value.validate().then(() => {
        openNotfication("warning", "授权码不正确");
      });
    };

    const init = () => {
      const userInfo = localStorage.getItem("userInfo");
      states.userInfo = JSON.parse(userInfo!);
    };

    onMounted(() => {
      init();
    });

    return {
      ...toRefs(states),
      formRef,
      codeColumns,
      scrollX,
      handleActivate,
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
</style>
