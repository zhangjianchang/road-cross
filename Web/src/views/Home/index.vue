<template>
  <div class="header">
    <h2>道路交叉口设计分析软件</h2>
    <h3>设计、评价分析、自动配时、基于地图</h3>
  </div>
  <div class="content">
    <a-carousel arrows dots-class="slick-dots slick-thumb" :autoplay="true">
      <template #customPaging="props">
        <a>
          <img :src="getImgUrl(props.i)" />
        </a>
      </template>
      <div v-for="(item, index) in imgNames" :key="item">
        <img :src="getImgUrl(index)" />
      </div>
    </a-carousel>
  </div>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { basic_config } from "../../request/http";

export default defineComponent({
  components: {},
  setup() {
    const imgUrl = `${basic_config.img_url}/home/`;
    const imgNames = [
      "1.jpg",
      "2.jpg",
      "3.jpg",
      "4.jpg",
      "5.jpg",
      "6.jpg",
      "7.jpg",
      "8.jpg",
      "9.jpg",
    ];

    const getImgUrl = (i: number) => {
      return `${imgUrl}${i + 1}.jpg`;
    };
    return { imgNames, getImgUrl };
  },
});
</script>
<style lang="less" scoped>
@import url("./index.less");
</style>
