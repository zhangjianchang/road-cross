<template>
  <Container class="main-container">
    用户中心
    <div class="footer">
      <a-button block type="primary" @click="onLogout">安全退出</a-button>
    </div>
  </Container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Container from "../../components/Container/index.vue";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";

export default defineComponent({
  components: { Container },
  setup() {
    const onLogout = () => {
      localStorage.removeItem("userInfo");
      localStorage.removeItem("token");
      goRouterByParam(PageEnum.Login);
    };
    return {
      onLogout,
    };
  },
});
</script>
<style scoped lang="less">
.main-container {
  position: relative;
}
.footer {
  width: calc(100% - 64px);
  position: absolute;
  bottom: 12px;
}
</style>
