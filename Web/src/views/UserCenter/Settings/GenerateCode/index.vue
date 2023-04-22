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
      <a-form-item label="购买时长" name="duration" v-if="param.type === '3'">
        <a-select
          v-model:value="param.duration"
          :options="durationOptions"
          placeholder="请输入购买时长"
          class="pwd-input"
        />
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

export default defineComponent({
  components: {},
  setup() {
    const formRef = ref();
    const states = reactive({
      loading: false,
      typeOptions: [
        { label: "月卡", value: "1" },
        { label: "年卡", value: "2" },
        { label: "自定义购买时长", value: "3" },
      ],
      durationOptions: [
        { label: "2个月", value: 2 },
        { label: "3个月", value: 3 },
        { label: "4个月", value: 4 },
        { label: "5个月", value: 5 },
        { label: "6个月", value: 6 },
        { label: "7个月", value: 7 },
        { label: "8个月", value: 8 },
        { label: "9个月", value: 9 },
        { label: "10个月", value: 10 },
        { label: "11个月", value: 11 },
      ],
      param: {
        memberName: "", //购买用户的登录账号
        type: "1", //授权码类型
        duration: undefined, //自定义时长
      },
      data: {} as any,
      rules: {
        type: [{ required: true, message: "请选择类型", trigger: "change" }],
        duration: [
          { required: true, message: "请选择购买时长", trigger: "change" },
        ],
        memberName: [
          {
            required: true,
            message: "请输入购买授权码用户的登录账号",
            trigger: "blur",
          },
        ],
      },
    });

    const handleGenterate = () => {
      formRef.value.validate().then(() => {
        generateCode(states.param).then((res: any) => {
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
