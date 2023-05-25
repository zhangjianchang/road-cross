<template>
  <div class="content">
    <h2>1、个人账号授权</h2>
    <h3>相关说明</h3>
    <div>
      1.个人账号授权分为月卡和年卡两种，月卡授权有效期为 31 天，限定使用次数 50
      次，费用为 99 元； 年卡授权有效期为 366，限定使用次数 599 次，费用为 599
      元。
    </div>
    <div>
      2.账号需授权方可正常使用，未授权用户能使用部分功能，不支持保存、复制和分享项目等功能。
    </div>
    <h3>购买须知</h3>
    <div>
      1.新建或编辑项目消耗1次使用次数，不关闭页面多次保存项目不重复消耗授权。
    </div>
    <div>
      2.授权时间超期或使用次数超过授权次数，授权失效，失效后原项目依旧保存。
    </div>
    <div>3.支持地图模式，不限定使用终端，支持多个终端同时使用。</div>
    <div>4.授权码为虚拟产品，激活后不支持退款。</div>
    <div>5.建议使用Chrome浏览器或MicrosoftEdge。</div>
    <div>6.如需发票请将订单号、发票抬头、发票税号等信息发送至客服邮箱。</div>
    <h3>购买方式</h3>
    <div>电脑端：点击链接至 淘宝店铺 购买授权码。</div>
    <div></div>
    <div>手机端：使用手机淘宝APP扫描二维码进入店铺。</div>
    <div>二维码</div>
    <h2 style="padding-top: 36px">2、企业版账号授权</h2>
    <h3>相关说明</h3>
    <div>
      1.企业版可配置子账号，设置子账号授权次数，所有子账号共享企业授权，无需再单独购买。
    </div>
    <div>
      2.企业版拥有个人版所有功能，同时支持交叉口
      ，支持地图模式，在地图中绘制交叉口、浏览管理项目、查看路况更加直观准确。
    </div>
    <h3>购买方式</h3>
    <div>如需购买企业授权，请联系客服微信。</div>
    <div>微信二维码</div>
  </div>
</template>

<script setup lang="ts">
import { basic_config } from "../../request/http";

const imgUrl = `${basic_config.img_url}/doc/`;
</script>
<style lang="less" scoped>
@import url("./index.less");
</style>
