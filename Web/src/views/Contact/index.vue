<template>
  <div class="content">
    <h3>官方QQ群</h3>
    <div class="label">官方交流QQ群：1111111</div>
    <h3>邮箱</h3>
    <div class="label">对应用有任何意见或者建议欢迎发送至邮箱：xxx@xxx.com</div>
    <h3>企业账号授权</h3>
    <div class="label">企业账号授权请联系：13333332222</div>
    <h3>意见与建议</h3>
    <div class="label">
      <a-textarea
        :rows="6"
        show-count
        allow-clear
        placeholder="请填写您在系统使用中遇到的问题或对系统的建议"
        :maxlength="500"
        style="width: 400px"
        v-model:value="param.suggestion"
      />
    </div>
    <div class="button">
      <a-button type="primary" @click="onSubmit">提交</a-button>
      <!-- <a class="a-text">查看用户全部意见与建议</a> -->
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from "vue";
import { suggestion } from "../../request/api";
import { openNotfication } from "../../utils/message";
// const imgUrl = "../../src/assets/image/doc/";
const param = reactive({
  suggestion: "",
});
const onSubmit = () => {
  suggestion(param).then(() => {
    openNotfication(
      "success",
      "管理员回复后会进行展示，请耐心等待",
      "提交成功",
      15
    );
  });
};
</script>
<style lang="less" scoped>
@import url("./index.less");
</style>
