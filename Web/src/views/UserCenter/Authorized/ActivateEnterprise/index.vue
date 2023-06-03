<template>
  <div class="form-data">
    <a-form
      v-if="userInfo.roleId === 1"
      :model="param"
      :label-col="{ span: 2 }"
      :wrapper-col="{ span: 20 }"
      :rules="rules"
      ref="formRef"
      labelAlign="left"
    >
      <a-form-item label="购买套餐" name="type">
        <a-select
          v-model:value="param.type"
          :options="durationOptions"
          placeholder="请选择购买套餐"
          class="pwd-input"
        />
      </a-form-item>
      <a-form-item label="授权账号" name="memberName">
        <a-input
          placeholder="输入购买企业账号的登录账号"
          v-model:value="param.memberName"
          class="pwd-input"
        />
      </a-form-item>
      <a-form-item :wrapper-col="{ offset: 2, span: 20 }">
        <a-button
          :loading="loading"
          type="primary"
          class="pwd-button"
          block
          @click="handleBuy"
        >
          确认授权
        </a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, reactive, ref, toRefs } from "vue";
import { settingStates } from "..";
import { activeEnterpriseAccount, generateCode } from "../../../../request/api";
import { scrollX } from "../../../../utils/common";

export default defineComponent({
  components: {},
  setup() {
    const formRef = ref();
    const states = reactive({
      loading: false,
      durationOptions: [
        { label: "A套餐（一年6,000次）", value: "A" },
        { label: "B套餐（一年12,000次）", value: "B" },
      ],
      param: {
        memberName: "", //购买用户的登录账号
        type: undefined, //套餐
      },
      data: {} as any,
      rules: {
        type: [{ required: true, message: "请选择套餐", trigger: "change" }],
        memberName: [
          {
            required: true,
            message: "请输入购买授权码用户的登录账号",
            trigger: "blur",
          },
        ],
      },
    });

    const handleBuy = () => {
      formRef.value.validate().then(() => {
        states.loading = true;
        activeEnterpriseAccount(states.param)
          .then((res) => {
            message.success("授权成功");
          })
          .finally(() => {
            states.loading = false;
          });
      });
    };

    return {
      ...toRefs(states),
      ...toRefs(settingStates),
      formRef,
      scrollX,
      handleBuy,
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
</style>
