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
      <a-form-item label="授权类型" name="type">
        <a-radio-group v-model:value="param.type" :options="typeOptions" />
      </a-form-item>
      <a-form-item label="授权账号" name="memberName">
        <a-input
          placeholder="输入购买授权码会员的登录账号"
          v-model:value="param.memberName"
          class="pwd-input"
        />
      </a-form-item>
      <a-form-item :wrapper-col="{ offset: 2, span: 20 }">
        <a-button
          type="primary"
          class="pwd-button"
          block
          @click="handleGenterate"
        >
          确认生成
        </a-button>
      </a-form-item>
    </a-form>
    <div class="generate-code" v-if="data.code">
      <p>
        该授权码只能供用户
        <span class="generate-code-user">{{ data.userName }}</span>
        使用，其余用户无法激活；
      </p>
      <p>确认无误后将该授权码提供给用户激活使用</p>
      <div class="generate-code-text">
        {{ data.code }}
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, reactive, ref, toRefs } from "vue";
import { settingStates } from "..";
import { generateCode } from "../../../../request/api";
import { scrollX } from "../../../../utils/common";
import { openNotfication } from "../../../../utils/message";

export default defineComponent({
  components: {},
  setup() {
    const formRef = ref();
    const states = reactive({
      loading: false,
      typeOptions: [
        { label: "月卡", value: "1" },
        { label: "年卡", value: "2" },
      ],
      param: {
        memberName: "", //购买用户的登录账号
        type: "1", //授权码类型
      },
      data: {} as any,
      rules: {
        type: [{ required: true, message: "请选择类型", trigger: "change" }],
        memberName: [
          {
            required: true,
            message: "请输入购买授权码会员的登录账号",
            trigger: "blur",
          },
        ],
      },
    });

    const handleGenterate = () => {
      formRef.value.validate().then(() => {
        generateCode(states.param).then((res: any) => {
          console.log(res);
          states.data = res.data;
        });
      });
    };

    return {
      ...toRefs(states),
      ...toRefs(settingStates),
      formRef,
      scrollX,
      handleGenterate,
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
</style>
