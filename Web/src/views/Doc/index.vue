<template>
  <div class="content">
    <div class="menu">
      <h3 class="menu-title">目录</h3>
      <ul>
        <li
          class="menu-item"
          v-for="item in menus"
          :key="item.key"
          @click="onChangeDoc(item.key)"
        >
          {{ item.title }}
        </li>
      </ul>
    </div>
    <div class="doc">
      <div id="1">
        <h2>交叉口几何绘制</h2>
        <h3>1.选择道路</h3>
        <div>在交叉口几何绘制面板中依次点击交叉口各个进口几何方向</div>
        <img :src="`${imgUrl}1.1.jpg`" />
        <h3>2.修改道路方向</h3>
        <div>鼠标按住方向箭头，则可以拖动改变车道的进口方向</div>
        <img :src="`${imgUrl}1.2.jpg`" />
        <h3>3.删除道路</h3>
        <div>鼠标右键点击方向箭头，即可删除</div>
        <img :src="`${imgUrl}1.3.jpg`" />
      </div>
      <div id="2">
        <h2>渠化方案设计</h2>
        <h3>1.交叉口属性</h3>
        <div>
          可以选择各方向，可以为各道路统一设置或取消非机动车道，可以设置交叉口大小，可以设置各道路右转曲度。右转曲度：右转线条绘制的弯曲程度，取值范围为[0-1]，0为直线
        </div>
        <img :src="`${imgUrl}2.1.jpg`" />
        <h3>2.道路属性</h3>
        <div>对该道路名称、人行道、速度、左转待转、直行待转属性进行设置。</div>
        <img :src="`${imgUrl}2.2.1.jpg`" />
        <img :src="`${imgUrl}2.2.2.jpg`" />
        <h3>3.道路进口属性</h3>
        <div>
          对该道路的进口进行设置，包含进口车道数量、进口车道宽度、展宽数量、展宽车道宽度、展宽段长、内侧偏移
        </div>
        <img :src="`${imgUrl}2.3.jpg`" />
        <h3>4.出口属性</h3>
        <div>
          对该道路的出口进行设置，包含进口车道数量、进口车道宽度、展宽数量、展宽车道宽度、展宽段长
        </div>
        <img :src="`${imgUrl}2.4.jpg`" />
        <h3>5.渠化属性</h3>
        <div>
          对道路的渠化属性进行设置，包含渠化类型和入口宽度。渠化类型分为划线渠化-有单独出口、划线渠化-无单独出口、固体渠化-有单独出口和固体渠化-无单独出口4种
        </div>
        <img :src="`${imgUrl}2.5.1.jpg`" />
        <img :src="`${imgUrl}2.5.2.jpg`" />
        <h3>6.隔离带</h3>
        <div>设置道路中间隔离带的类型、宽度以及安全岛、提前掉头等属性</div>
        <img :src="`${imgUrl}2.6.jpg`" />
        <h3>7.非机动车道</h3>
        <div>
          可以对每条道路两侧非机动车道分别进行单独设置，也可以通过“交叉口属性”中的“非机动车道”功能为所有道路设置或取消非机动车到属性
          <img :src="`${imgUrl}2.7.jpg`" />
        </div>
        <h3>8.增减渠化方案</h3>
        <div>不同渠化方案可进行对比分析</div>
        <img :src="`${imgUrl}2.8.jpg`" />
      </div>
      <div id="3">
        <h2>交叉口转向流量输入</h2>
        <h3>
          1.交叉口转向流量以及饱和流量输入，支持多种流量方案设置，流量输入为自然车数，在相应转向类型中输入各个转向流量，也可直接双击转向流量标签输入流量
        </h3>
        <img :src="`${imgUrl}3.1.1.jpg`" />
        <img :src="`${imgUrl}3.1.2.jpg`" />
        <img :src="`${imgUrl}3.1.3.jpg`" />
        <h3>
          2.每个进口方向各车道饱和流率数值，其中从最内侧车道开始编号，根据转向车道情况预设初始值，用户可根据实际交叉口近况调整饱和流率数值。其中大车比例、高峰小时系数将影响评价分析结果
        </h3>
        <img :src="`${imgUrl}3.2.1.jpg`" />
        <img :src="`${imgUrl}3.2.2.jpg`" />
      </div>
      <div id="4">
        <h2>交叉口信号方案输入</h2>
        <h3>
          1.交叉口信号方案输入支持多种信号方案，其中可设定每个相位绿灯时间、黄灯时间和全红时间，也可点击相位图可快速设置
        </h3>
        <img :src="`${imgUrl}4.1.1.jpg`" />
        <img :src="`${imgUrl}4.1.1.jpg`" />
        <h3>2.可针对十字信号交叉口，自动生成配时方案</h3>
        <div>
          交叉口信号自动配时的基本原理是根据周期内信号总损失、关键流率比之和、高峰小时系数以及设计目标饱和度计算出周期时长和有效绿灯时长，然后将有效绿灯时长按比例分配至各相位，其中搭接相位时长分配要有别于普通相位。
          最终达到的效果是各相位的关键车道组（关键流率比对应的车道组）的饱和度等于设计目标饱和度，交叉口整体饱和度略小于设计目标饱和度。
        </div>
      </div>
      <div id="5">
        <h2>交叉口饱和度评价分析</h2>
        <h3>支持多种方案评价分析，也可对不同方案下饱和度进行对比分析</h3>
        <img :src="`${imgUrl}5.1.1.jpg`" />
        <img :src="`${imgUrl}5.1.2.jpg`" />
      </div>
      <div id="6">
        <h2>交叉口延误评价分析</h2>
        <h3>与饱和度评价分析类似，单位为：秒</h3>
      </div>
      <div id="7">
        <h2>交叉口延排队长度评价分析</h2>
        <h3>与饱和度评价分析类似，单位为：米</h3>
      </div>
      <div id="8">
        <h2>交叉口服务水平评价分析</h2>
        <h3>与饱和度评价分析类似</h3>
      </div>
      <div id="9">
        <h2>地图模式</h2>
        <h3>
          支持地图模式，在地图中绘制交叉口，浏览管理项目，查看路况，更加直观准确
        </h3>
        <div>
          交叉口信号自动配时的基本原理是根据周期内信号总损失、关键流率比之和、高峰小时系数以及设计目标饱和度计算出周期时长和有效绿灯时长，然后将有效绿灯时长按比例分配至各相位，其中搭接相位时长分配要有别于普通相位。
          最终达到的效果是各相位的关键车道组（关键流率比对应的车道组）的饱和度等于设计目标饱和度，交叉口整体饱和度略小于设计目标饱和度。
        </div>
        <img :src="`${imgUrl}9.1.1.jpg`" />
        <img :src="`${imgUrl}9.1.2.jpg`" />
      </div>
    </div>
    <!-- 返回顶部 -->
    <a-back-top :target="targetFn" />
  </div>
</template>

<script setup lang="ts">
import { basic_config } from "../../request/http";

const imgUrl = `${basic_config.img_url}/doc/`;
const menus = [
  { title: "交叉口几何绘制", key: "1" },
  { title: "渠化方案设计", key: "2" },
  { title: "交叉口转向流量输入", key: "3" },
  { title: "交叉口信号方案输入", key: "4" },
  { title: "交叉口饱和度评价分析", key: "5" },
  { title: "交叉口延误评价分析", key: "6" },
  { title: "交叉口延排队长度评价分析", key: "7" },
  { title: "交叉口服务水平评价分析", key: "8" },
  { title: "地图模式", key: "9" },
];

const onChangeDoc = (key: any) => {
  document
    .getElementById(`${key}`)
    ?.scrollIntoView({ behavior: "smooth", block: "start" });
};

const targetFn = () => {
  return document.querySelector(".doc");
};
</script>
<style lang="less" scoped>
@import url("./index.less");
</style>
