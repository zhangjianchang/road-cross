<template>
  <Container class="main-container">
    <div style="display: flex">
      <a-card hoverable class="card" @click="handleSetting">
        <template #cover>
          <img
            height="180"
            alt="example"
            src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQXgwiNXb8x47hLT_6oHHGAcplZBq3q9meS-g&usqp=CAU"
          />
        </template>
        <a-card-meta title="设置">
          <template #description>个人信息及授权码激活</template>
        </a-card-meta>
      </a-card>
      <a-card hoverable class="card" @click="handleMyList">
        <template #cover>
          <img
            height="180"
            alt="example"
            src="https://img.zcool.cn/community/011c4e600ebc1c11013f7928fead49.jpg?x-oss-process=image/auto-orient,1/resize,m_lfit,w_1280,limit_1/sharpen,100"
          />
        </template>
        <a-card-meta title="项目列表">
          <template #description>我已录入的全部道路</template>
        </a-card-meta>
      </a-card>
    </div>
  </Container>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, onMounted } from "vue";
import Container from "../../components/Container/index.vue";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";

export default defineComponent({
  components: { Container },
  setup() {
    //判断权限
    var token = localStorage.getItem("token");
    if (!token) {
      message.warning("请先登录");
      goRouterByParam(PageEnum.Login);
    }

    const handleSetting = () => {
      goRouterByParam(PageEnum.Settings);
    };

    const handleMyList = () => {
      goRouterByParam(PageEnum.DesignRoadList);
    };

    onMounted(() => {
      // window.location.reload();
    });

    return {
      handleSetting,
      handleMyList,
    };
  },
});
</script>
<style scoped lang="less">
@import "./index.less";
</style>
