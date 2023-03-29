<template>
  <div>
    <a-row :gutter="[16, 16]">
      <a-col :span="2"> 登录账号 </a-col>
      <a-col :span="22">{{ data.userName }}</a-col>
      <a-col :span="2"> 用户名 </a-col>
      <a-col :span="22">{{ data.chineseName }}</a-col>
      <a-col :span="2"> 账号类型 </a-col>
      <a-col :span="22">{{ data.roleName }}</a-col>
    </a-row>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive, toRefs } from "vue";

export default defineComponent({
  components: {},
  setup() {
    const states = reactive({
      activeKey: 1,
      data: {} as any,
    });

    const init = () => {
      const userInfo = localStorage.getItem("userInfo");
      states.data = JSON.parse(userInfo!);
    };

    onMounted(() => {
      init();
    });

    return {
      ...toRefs(states),
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
</style>
