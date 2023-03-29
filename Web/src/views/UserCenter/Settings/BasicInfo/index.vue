<template>
  <div>
    <a-form
      :label-col="{ span: 2 }"
      :wrapper-col="{ span: 22 }"
      labelAlign="left"
    >
      <a-form-item label="登录账号">
        {{ data.userName }}
      </a-form-item>
      <a-form-item label="用户名">
        {{ data.chineseName }}
      </a-form-item>
      <a-form-item label="账号类型">
        {{ data.roleName }}
      </a-form-item>
    </a-form>
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
