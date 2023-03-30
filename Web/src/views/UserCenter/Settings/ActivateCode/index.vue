<template>
  <div class="form-data">
    <div v-if="userInfo.roleId === 1">
      您是系统管理员，可使用系统全部功能，无需激活
    </div>
    <div v-else-if="data.isActivated" class="success-text">
      您的授权码已激活!
    </div>
    <a-form
      v-else
      :model="data"
      :label-col="{ span: 2 }"
      :wrapper-col="{ span: 22 }"
      :rules="rules"
      ref="formRef"
      labelAlign="left"
    >
      <a-form-item label="激活码" name="code">
        <a-input
          placeholder="请输入激活码"
          v-model:value="data.code"
          class="code-input"
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
    <a-table
      v-if="userInfo.roleId !== 1 && memberInfo.length > 0"
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
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, onMounted, reactive, ref, toRefs } from "vue";
import { settingStates } from "..";
import { activeCode, getCodeInfosByUser } from "../../../../request/api";
import { scrollX } from "../../../../utils/common";
import { codeColumns } from "./data";

export default defineComponent({
  components: {},
  setup() {
    const formRef = ref();
    const states = reactive({
      loading: false,
      memberInfo: [] as any[],
      data: {
        code: "",
        isActivated: false,
      },
      rules: {
        code: [{ required: true, message: "请输入激活码", trigger: "blur" }],
      },
    });

    const handleActivate = () => {
      formRef.value.validate().then(() => {
        activeCode({ code: states.data.code }).then(() => {
          message.success("激活成功");
          initData();
        });
      });
    };

    const initData = () => {
      getCodeInfosByUser().then((res: any) => {
        states.memberInfo.length = 0;
        Object.assign(states.memberInfo, res.data);
        states.data.isActivated = states.memberInfo.some(
          (m) => m.status === "200"
        );
      });
    };

    onMounted(() => {
      initData();
    });

    return {
      ...toRefs(states),
      ...toRefs(settingStates),
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
