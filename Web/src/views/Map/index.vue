<template>
  <!-- 用来获取当前定位 -->
  <iframe
    id="geoPage"
    width="0"
    height="0"
    frameborder="0"
    style="display: none"
    scrolling="no"
    src="https://apis.map.qq.com/tools/geolocation?key=T7UBZ-LUBWT-57QXM-V3VZE-WY7U6-C5BLB&referer=road_design"
  >
  </iframe>
  <!-- 地图主界面 -->
  <div id="container" class="map-container"></div>
  <div class="drawer-text" @click="showDrawer" v-show="!states.visible">
    <DoubleLeftOutlined />
  </div>
  <a-drawer
    :width="500"
    title="设置"
    placement="right"
    :visible="states.visible"
    :closable="false"
    @close="onClose"
  >
    <!-- <template #extra>
      <a-button style="margin-right: 8px" @click="onClose">取消</a-button>
      <a-button type="primary" @click="onClose">确定</a-button>
    </template> -->
    <div class="search-address">
      <p>搜索需要匹配的地址</p>
      <a-input-search
        v-model:value="states.address"
        placeholder="请输入需要匹配的地点或街道名称"
        enter-button
        @search="onSearch"
      />
    </div>
    <div class="bg-setting">
      <p>设置背景透明度</p>
      <a-slider v-model:value="states.opacity" @afterChange="setOpacity" />
    </div>
    <div>
      <p>选择需要编辑的项目</p>
      <a-select
        class="large-form-width"
        placeholder="选择需要编辑的项目"
        v-model:value="states.currentPaln"
        :options="states.planData"
        @change="onChangePlan"
        style="width: 100%"
      />
    </div>
  </a-drawer>
  <div id="design" class="design-content">
    <div class="fold" @click="states.is_fold = !states.is_fold">
      <span v-if="states.is_fold">
        点击收起
        <FullscreenExitOutlined />
      </span>
      <span v-else>
        点击展开
        <FullscreenOutlined />
      </span>
    </div>
    <Design ref="refDesign" v-show="states.is_fold" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, onBeforeUnmount, reactive, ref } from "vue";
import { getDesignList } from "../../request/api";
import {
  DoubleLeftOutlined,
  FullscreenExitOutlined,
  FullscreenOutlined,
} from "@ant-design/icons-vue";
import Design from "../Design/index.vue";
const refDesign = ref();
const states = reactive({
  visible: false,
  is_fold: true, //默认展开
  opacity: 90,
  address: undefined, //搜索定位地点
  currentPaln: undefined as any,
  planData: [] as any[],
});

//打开抽屉
const showDrawer = () => {
  states.visible = true;
};

//关闭抽屉
const onClose = () => {
  states.visible = false;
};

//搜索地点
const onSearch = () => {
  console.log(states.address);
};

//设置透明度
const setOpacity = () => {
  // 获取父元素
  var parent = document.getElementById("design")!;
  parent.style.opacity = (states.opacity / 100).toString();

  // 遍历所有子孙元素，并设置它们的透明度
  var elements = parent.getElementsByTagName("*");
  for (var i = 0; i < elements.length; i++) {
    if (elements[i].tagName.toLowerCase() !== "path") {
      elements[i].style.opacity = (states.opacity / 100).toString();
    }
  }

  parent = document.getElementById("canvas")!;
  //TODO 没生效，优化
  //遍历所有 <path> 标签，并将它们的透明度设置为1
  var paths = document.getElementsByTagName("path");
  for (var j = 0; j < paths.length; j++) {
    paths[j].setAttribute("opacity", "1");
  }
};

//获取我的方案
const initData = () => {
  getDesignList().then((res: any) => {
    states.planData = res.data.map((item: any) => {
      return {
        label: item.roadName,
        value: item.guid,
      };
    });
  });
};

//选择方案
const onChangePlan = (item: any) => {
  refDesign.value.loadData(item);
};

//加载当前位置
const Tmap = () => {
  return new Promise((resolve: any, reject: any) => {
    let loc = undefined as any;
    window.addEventListener(
      "message",
      function (event) {
        if (event.data) {
          loc = event.data; // 接收位置信息
          if (loc) {
            return resolve(loc);
          }
        }
      },
      false
    );
    // 设置6s超时，防止定位组件长时间获取位置信息未响应
    setTimeout(function () {
      if (!loc) return reject(false);
    }, 6000); // 6s为推荐值，不建议太短
  });
};

onMounted(() => {
  Tmap().then((loc: any) => {
    const TMap = (window as any).TMap; // TMap地图实例
    const LatLng = TMap.LatLng; // 用于创建经纬度坐标实例
    const center = new LatLng(loc.lat, loc.lng); //设置中心点坐标

    //初始化地图
    const map = new TMap.Map("container", { center: center });
    // map.removeControl(TMap.constants.DEFAULT_CONTROL_ID.ZOOM); // 从地图容器移出 缩放控件
    // map.removeControl(TMap.constants.DEFAULT_CONTROL_ID.SCALE); // 从地图容器移出 比例尺控件
    // map.removeControl(TMap.constants.DEFAULT_CONTROL_ID.ROTATION); // 从地图容器移出 旋转控件

    //创建并初始化MultiMarker，表示地图上的多个标注点，可自定义标注的图标。
    new TMap.MultiMarker({
      map: map, //指定地图容器
      geometries: [], //点标记数据数组
    });

    // 发现把这个生命周期钩子写在onMounted生命周期钩子内，没有报错，而且还有效果
    onBeforeUnmount(() => {
      // 切换路由的时候可能创建了多个实例，可以使用destroy销毁地图
      map.destroy();
    });
  });
  //加载数据
  initData();
});
</script>

<style lang="less" scoped>
@import url("./index.less");
</style>
