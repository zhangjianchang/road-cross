<template>
  <!-- 用来获取当前定位 -->
  <iframe
    id="geoPage"
    width="0"
    height="0"
    frameborder="0"
    style="display: none"
    scrolling="no"
    :src="`https://apis.map.qq.com/tools/geolocation?key=${mapKey}&referer=road_design`"
  >
  </iframe>
  <!-- 地图主界面 -->
  <div id="container" class="map-container"></div>
  <!-- 抽屉打开键 -->
  <div class="drawer-text" @click="showDrawer" v-show="!states.visible">
    <DoubleLeftOutlined />
  </div>
  <!-- 抽屉 -->
  <a-drawer
    :width="650"
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
      <p>搜索定位</p>
      <a-select
        v-model:value="states.currentAddress"
        style="width: 100%"
        allow-clear
        placeholder="可以输入地点名称或街道名称等进行地图中心定位"
        :show-search="true"
        :filter-option="false"
        :not-found-content="states.fetching ? undefined : null"
        @search="onSearch"
        :options="states.options"
      >
        <template #dropdownRender>
          <div
            class="select"
            v-for="item in states.options"
            :key="item.title"
            @click="onSelect(item)"
          >
            <div class="select-title">{{ item.title }}</div>
            <div class="select-address">
              {{ item.address }}
            </div>
          </div>
        </template>
        <template v-if="states.fetching" #notFoundContent>
          <a-spin size="small" />
        </template>
      </a-select>
    </div>
    <div>
      <p>选择需要编辑的项目</p>
      <a-select
        allowClear
        :showSearch="true"
        :filterOption="filterOptionLabel"
        class="large-form-width"
        placeholder="选择需要编辑的项目"
        v-model:value="states.currentPaln"
        :options="states.planData"
        @change="onChangePlan"
        style="width: 100%"
      />
    </div>
  </a-drawer>
  <!-- 透明度调节器 -->
  <div class="slider-content" v-show="states.currentPaln">
    <div style="display: inline-block; height: 300px; margin-left: 70px">
      <a-slider
        :min="0"
        :max="1"
        :step="0.1"
        v-model:value="states.opacity"
        vertical
        @afterChange="setOpacity"
      />
    </div>
  </div>
  <!-- 主设计窗口 -->
  <div class="design-content" v-show="states.currentPaln">
    <!-- 右上角收起/展开键 -->
    <div class="collapse" @click="states.is_collapse = !states.is_collapse">
      <FullscreenExitOutlined v-if="states.is_collapse" title="收起" />
      <FullscreenOutlined v-else title="展开" />
    </div>
    <Design
      id="design"
      ref="refDesign"
      v-show="states.is_collapse"
      @changeMenu="changeMenu"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, onBeforeUnmount, reactive, ref } from "vue";
import { getDesignList, searchMap } from "../../request/api";
import {
  DoubleLeftOutlined,
  FullscreenExitOutlined,
  FullscreenOutlined,
} from "@ant-design/icons-vue";
import Design from "../Design/index.vue";
import { mapKey } from "../../request/http";
import { filterOptionLabel } from "../../utils/options";
import { debounce } from "lodash";
import { notOpacityClass } from "./data";

const refDesign = ref();
const states = reactive({
  map: undefined as any, //地图
  loc: undefined as any, //当前定位点
  pathTags: ["path", "circle", "line", "text", "span"],
  opacityClass: ["header", "content-menu", "menu"],
  notOpacityClass: notOpacityClass,
  visible: false,
  is_collapse: true, //默认展开
  opacity: 0.9, //默认透明度
  fetching: false, //延时访问
  currentAddress: undefined as any,
  options: [] as any[], //城市下拉
  currentPaln: undefined as any, //选中项目
  planData: [] as any[], //项目下拉
});

//打开抽屉
const showDrawer = () => {
  states.visible = true;
};

//关闭抽屉
const onClose = () => {
  states.visible = false;
};

//搜索地点(增加防抖)
let lastFetchId = 0;
const onSearch = debounce((value: any) => {
  if (!value) {
    return;
  }
  lastFetchId += 1;
  const fetchId = lastFetchId;
  states.fetching = true;

  const param = {
    key: mapKey,
    keyword: value,
    page_size: 20, //地图接口最多支持20条
    // boundary: `region(全国,0)`,
    boundary: `nearby(${states.loc.lat},${states.loc.lng},1000,1)`, //附近1000m内
  };
  if (fetchId !== lastFetchId) return;
  //调用腾讯接口
  states.options.length = 0;
  searchMap(param).then((res: any) => {
    Object.assign(states.options, res);
    states.fetching = false;
  });
}, 600);

//切换地点
const onSelect = (item: any) => {
  //切换地点的时候收起面板，更好的展示
  states.is_collapse = false;
  //切换地图位置
  states.currentAddress = item.title;
  states.loc = item.location;
  reLoadMap();
};

//设计页面切换菜单
const changeMenu = () => {
  setOpacity();
};

//设置透明度
const setOpacity = () => {
  // 获取父元素
  var parent = document.getElementById("design")!;
  parent["style"].background = `rgba(248,248,248,${states.opacity})`;
  // 遍历所有子孙元素，并设置它们的透明度
  var elements = parent.getElementsByTagName("*");
  for (var i = 0; i < elements.length; i++) {
    const className = elements[i].getAttribute("class")?.toLowerCase()!;
    const tagName = elements[i].tagName.toLowerCase();
    if (states.notOpacityClass.indexOf(className) === -1) {
      if (states.pathTags.indexOf(tagName) > -1) {
        elements[i].setAttribute("opacity", `${states.opacity}`);
      } else if (states.opacityClass.indexOf(className) > -1) {
        elements[i]["style"]["opacity"] = `${states.opacity}`;
      } else {
        elements[i]["style"][
          "background-color"
        ] = `rgba(255,255,255,${states.opacity})`;
      }
    }
  }

  parent = document.getElementById("canvas")!;
  //TODO 没生效，优化
  //遍历所有 <path> 标签，并将它们的透明度设置为1
  states.pathTags.forEach((tag) => {
    var paths = document.getElementsByTagName(tag);
    for (var j = 0; j < paths.length; j++) {
      if (
        paths[j].getAttribute("deleteTag") === "1" ||
        paths[j].getAttribute("opacityTag") === "1"
      ) {
        paths[j].setAttribute("opacity", "1");
      }
    }
  });
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
  states.is_collapse = true;
  refDesign.value.loadData(item);
};

//加载当前位置
const Tmap = () => {
  return new Promise((resolve: any, reject: any) => {
    window.addEventListener(
      "message",
      function (event) {
        if (event.data) {
          states.loc = event.data; // 接收位置信息
          if (states.loc) {
            return resolve(states.loc);
          }
        }
      },
      false
    );
    // 设置6s超时，防止定位组件长时间获取位置信息未响应
    setTimeout(function () {
      if (!states.loc) return reject(false);
    }, 6000); // 6s为推荐值，不建议太短
  });
};

const reLoadMap = () => {
  states.map.destroy();
  loadMap(states.loc);
};

const loadMap = (loc: any) => {
  const TMap = (window as any).TMap; // TMap地图实例
  const LatLng = TMap.LatLng; // 用于创建经纬度坐标实例
  const center = new LatLng(loc.lat, loc.lng); //设置中心点坐标

  //初始化地图
  states.map = new TMap.Map("container", {
    zoom: 17, //设置地图缩放级别
    center: center, //地图中心
  });
  // map.removeControl(TMap.constants.DEFAULT_CONTROL_ID.ZOOM); // 从地图容器移出 缩放控件
  // map.removeControl(TMap.constants.DEFAULT_CONTROL_ID.SCALE); // 从地图容器移出 比例尺控件
  // map.removeControl(TMap.constants.DEFAULT_CONTROL_ID.ROTATION); // 从地图容器移出 旋转控件

  //创建并初始化MultiMarker，表示地图上的多个标注点，可自定义标注的图标。
  new TMap.MultiMarker({
    map: states.map, //指定地图容器
    geometries: [], //点标记数据数组
  });
};

onMounted(() => {
  //加载地图
  Tmap().then((loc: any) => {
    loadMap(loc);
  });
  //加载数据
  initData();
});

// 发现把这个生命周期钩子写在onMounted生命周期钩子内，没有报错，而且还有效果
onBeforeUnmount(() => {
  // 切换路由的时候可能创建了多个实例，可以使用destroy销毁地图
  states.map.destroy();
});
</script>

<style lang="less" scoped>
@import url("./index.less");
</style>
