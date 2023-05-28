<template>
  <Container class="main-container">
    <div style="display: flex">
      <a-card hoverable class="card" @click="handleMyList">
        <template #cover>
          <img height="180" alt="example" :src="`${imgUrl}/road-list.jpg`" />
        </template>
        <a-card-meta title="项目列表">
          <template #description>我已录入的全部道路</template>
        </a-card-meta>
      </a-card>
      <a-card hoverable class="card" @click="handleSetting">
        <template #cover>
          <img height="180" alt="example" :src="`${imgUrl}/setting.jpg`" />
        </template>
        <a-card-meta title="设置">
          <template #description>个人信息及授权码激活</template>
        </a-card-meta>
      </a-card>
      <a-card
        v-if="userInfo.roleId === 1"
        hoverable
        class="card"
        @click="handleSuggestion"
      >
        <template #cover>
          <img height="180" alt="example" :src="`${imgUrl}/suggestion.jpg`" />
        </template>
        <a-card-meta title="用户反馈">
          <template #description>查看并回答，展示用户反馈</template>
        </a-card-meta>
      </a-card>
    </div>
  </Container>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, onMounted, toRefs } from "vue";
import Container from "../../components/Container/index.vue";
import { checkToken, getUserInfo } from "../../request/api";
import { basic_config } from "../../request/http";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";
import { settingStates } from "./Settings";

export default defineComponent({
  components: { Container },
  setup() {
    const imgUrl = `${basic_config.img_url}/user-center`;
    //判断权限
    var token = localStorage.getItem("token");
    if (!token) {
      message.warning("请先登录");
      goRouterByParam(PageEnum.Login);
    }

    const handleSetting = () => {
      goRouterByParam(PageEnum.BasicInfo);
    };

    const handleMyList = () => {
      goRouterByParam(PageEnum.DesignRoadList);
    };

    const handleSuggestion = () => {
      goRouterByParam(PageEnum.Suggestion);
    };

    //判断权限
    const initUserInfo = () => {
      var token = localStorage.getItem("token");
      if (!token) {
        message.warning("请先登录");
        goRouterByParam(PageEnum.Login);
      } else {
        checkToken()
          .then(() => {
            getUserInfo().then((res: any) => {
              const imgUrl = `${basic_config.img_url}/avatar`;
              res.data.avatar = `${imgUrl}/${res.data.avatar}`;
              settingStates.userInfo = res.data;
            });
          })
          .catch(() => {
            localStorage.removeItem("token");
            localStorage.removeItem("userInfo");
            goRouterByParam(PageEnum.Login);
          });
      }
    };

    onMounted(() => {
      // window.location.reload();
      initUserInfo();
    });

    return {
      ...toRefs(settingStates),
      imgUrl,
      handleSetting,
      handleMyList,
      handleSuggestion,
    };
  },
});
</script>
<style scoped lang="less">
@import "./index.less";
</style>
