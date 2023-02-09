<template>
  <div class="basic-main">
    <div class="func">功能区</div>
    <!-- 图示 -->
    <svg id="canvas">
      <text v-for="(_, index) in road_attr" :key="index" x="300">
        <textPath :xlink:href="'#road_' + (index + 1)" y="50">
          方向{{ index + 1 }}
        </textPath>
      </text>
    </svg>
    <!-- 参数 -->
    <div class="menu">
      <div class="form">
        <div class="header">交叉口属性</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="方向">
                  <a-select
                    v-model:value="canalize_info.direction"
                    size="small"
                    class="form-width"
                    @change="onDirectionChange"
                  >
                    <a-select-option
                      v-for="(_, index) in road_attr"
                      :key="(index + 1).toString()"
                      :value="'road_' + (index + 1)"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="机动车道">
                  <a-button
                    type="primary"
                    size="small"
                    class="line-button"
                    @click="modalVisible = true"
                  >
                    设置
                  </a-button>
                  <a-button
                    type="default"
                    size="small"
                    class="line-button"
                    style="margin-left: 3px"
                  >
                    取消
                  </a-button>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="交叉口大小">
                  <a-input-number
                    v-model:value="canalize_info.size"
                    :min="0"
                    :max="10"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="右转曲度">
                  <a-input-number
                    v-model:value="canalize_info.curvature"
                    @change="drawCross"
                    :min="0"
                    :max="1"
                    :step="0.1"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="header">道路属性</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="路名">
                  <a-input
                    v-model:value="canalize_info.luming"
                    class="form-width"
                    size="small"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="偏移量">
                  <a-input-number
                    v-model:value="canalize_info.pianyiliang"
                    :min="0"
                    :max="1"
                    :step="0.1"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="人行道">
                  <a-select
                    v-model:value="canalize_info.renxingdao"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="Y"> 是 </a-select-option>
                    <a-select-option value="N"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="路段速度">
                  <a-input-number
                    v-model:value="canalize_info.luduansudu"
                    @change="drawCross"
                    :min="0"
                    :max="300"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="左转待转">
                  <a-select
                    v-model:value="canalize_info.zuozhuandaizhuan"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="Y"> 是 </a-select-option>
                    <a-select-option value="N"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="直行待转">
                  <a-select
                    v-model:value="canalize_info.zhixingdaizhuan"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="Y"> 是 </a-select-option>
                    <a-select-option value="N"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="穿越到">
                  <a-select
                    v-model:value="canalize_info.chuanyuedao"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option
                      v-for="(_, index) in road_attr"
                      :key="(index + 1).toString()"
                      :value="'road_' + (index + 1)"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="穿越方式">
                  <a-select
                    v-model:value="canalize_info.chuanyuefangshi"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value=""> 无 </a-select-option>
                    <a-select-option value="guantong">贯通 </a-select-option>
                    <a-select-option value="gelizhuang">
                      隔离桩
                    </a-select-option>
                    <a-select-option value="banmaxian">
                      斑马线
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="header">渠化属性</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="渠化方式">
                  <a-select
                    v-model:value="canalize_info.quhuafangshi"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="N"> 否 </a-select-option>
                    <a-select-option value="1">
                      划线渠化-无单独出口
                    </a-select-option>
                    <a-select-option value="2">
                      划线渠化-有单独出口
                    </a-select-option>
                    <a-select-option value="3">
                      固体渠化-无单独出口
                    </a-select-option>
                    <a-select-option value="4">
                      固体渠化-有单独出口
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="右转车道">
                  <a-input-number
                    v-model:value="canalize_info.youzhuanchedao"
                    @change="drawCross"
                    :min="0"
                    :max="2"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="header">进口属性</div>
        <div class="content" v-if="canalize_info.exitAttr.length > 0">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="进口车道">
                  <a-input-number
                    v-model:value="
                      canalize_info.entranceAttr[canalize_info.direction_index]
                        .wayCount
                    "
                    @change="onChangeRoadCount"
                    :min="0"
                    :max="9"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">个</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="车道宽度">
                  <a-input-number
                    v-model:value="canalize_info.chedaokuandu"
                    @change="drawCross"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="展宽数量">
                  <a-input-number
                    v-model:value="canalize_info.zhankuanshuliang"
                    @change="drawCross"
                    :min="-2"
                    :max="4"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">个</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="展宽车道宽度">
                  <a-input-number
                    v-model:value="canalize_info.zhankuanchedaokuandu"
                    @change="drawCross"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="展宽段长">
                  <a-input-number
                    v-model:value="canalize_info.zhankuanduanchang"
                    @change="drawCross"
                    :min="0"
                    :max="100"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="外侧渐变段长">
                  <a-input-number
                    v-model:value="canalize_info.waicejianbianduanchang"
                    @change="drawCross"
                    :min="0"
                    :max="50"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="内侧偏移">
                  <a-input-number
                    v-model:value="canalize_info.neicepianyi"
                    @change="drawCross"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="内侧渐变段长">
                  <a-input-number
                    v-model:value="canalize_info.neicejianbianduanchang"
                    @change="drawCross"
                    :min="0"
                    :max="50"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="header">出口属性</div>
        <div class="content" v-if="canalize_info.exitAttr.length > 0">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="出口车道">
                  <a-input-number
                    v-model:value="
                      canalize_info.exitAttr[canalize_info.direction_index]
                        .wayCount
                    "
                    @change="onChangeRoadCount"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">个</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="车道宽度">
                  <a-input-number
                    v-model:value="canalize_info.chedaokuandu2"
                    @change="drawCross"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="出口展宽">
                  <a-input-number
                    v-model:value="canalize_info.chukouzhankuan"
                    @change="drawCross"
                    :min="-2"
                    :max="4"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">个</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="展宽车道宽度">
                  <a-input-number
                    v-model:value="canalize_info.zhankuanchedaokuandu2"
                    @change="drawCross"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="展宽段长">
                  <a-input-number
                    v-model:value="canalize_info.zhankuanduanchang2"
                    @change="drawCross"
                    :min="0"
                    :max="100"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="渐变段长">
                  <a-input-number
                    v-model:value="canalize_info.jianbianduanchang"
                    @change="drawCross"
                    :min="0"
                    :max="50"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="header">隔离带</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="分割形式">
                  <a-select
                    v-model:value="canalize_info.fengexingshi"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="0"> 双黄线 </a-select-option>
                    <a-select-option value="1"> 单黄线 </a-select-option>
                    <a-select-option value="2"> 护栏 </a-select-option>
                    <a-select-option value="3"> 鱼肚线 </a-select-option>
                    <a-select-option value="4"> 黄斜线 </a-select-option>
                    <a-select-option value="5"> 绿化带 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="分割宽度">
                  <a-input-number
                    v-model:value="canalize_info.fengekuandu"
                    @change="drawCross"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="安全岛">
                  <a-select
                    v-model:value="canalize_info.anquandao"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="N"> 否 </a-select-option>
                    <a-select-option value="Y"> 是 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="提前掉头">
                  <a-select
                    v-model:value="canalize_info.tiqiantiaotou"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="N"> 否 </a-select-option>
                    <a-select-option value="Y1"> 停车线位置 </a-select-option>
                    <a-select-option value="Y2"> 停车线上游 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
        <div class="header">非机动车道</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="进口">
                  <a-select
                    v-model:value="canalize_info.jinkou"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="N"> 否 </a-select-option>
                    <a-select-option value="Y"> 是 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="出口">
                  <a-select
                    v-model:value="canalize_info.chukou"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="N"> 否 </a-select-option>
                    <a-select-option value="Y"> 是 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="车道宽度">
                  <a-input-number
                    v-model:value="canalize_info.chedaokuandu3"
                    @change="drawCross"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="车道宽度">
                  <a-input-number
                    v-model:value="canalize_info.chedaokuandu4"
                    @change="drawCross"
                    :min="0"
                    :max="10"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="分割形式">
                  <a-select
                    v-model:value="canalize_info.fengexingshi1"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="1"> 划线 </a-select-option>
                    <a-select-option value="2"> 护栏 </a-select-option>
                    <a-select-option value="3"> 绿化带 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="渐变段长">
                  <a-select
                    v-model:value="canalize_info.fengexingshi2"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="1"> 划线 </a-select-option>
                    <a-select-option value="2"> 护栏 </a-select-option>
                    <a-select-option value="3"> 绿化带 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
            </a-row>
          </a-form>
        </div>
      </div>
    </div>
    <a-modal
      :visible="modalVisible"
      width="1000px"
      title="设置道路交通标识"
      :footer="null"
      @cancel="modalVisible = false"
    >
      <div class="modal-container">
        <!-- 绘制路标 -->
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 1024 1024"
          class="road-sign"
          v-for="road in roadSigns.filter((s) => !s.hide)"
          :key="road.key"
          @click="handleConfirmSign(road.key)"
        >
          <path :d="road.path" fill="#ffffff" :title="road.name"></path>
        </svg>
      </div>
    </a-modal>
  </div>
</template>

<script lang="ts">
import { defineComponent, inject, onMounted, reactive, toRefs } from "vue";
import { getDirectionIndex, getRoadDefaultSign, roadSigns } from "./index";
import Container from "../../../components/Container/index.vue";
import { notification } from "ant-design-vue";
import { DragOutlined } from "@ant-design/icons-vue";
import {
  getQByPathCurv,
  intersect_line_point,
  line_pt3,
} from "../../../utils/common";
import { road_info } from "..";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      road_r_k: 15.4, //每条车道宽度
      cross_pts: [] as any[], //所有路口交叉点
      cross_line_pts: [] as any[], //所有路口交叉点内侧一层
      road_zebra_pts: [] as any[], //所有路口交叉点内侧一层
      currentRoad: {} as any, //当前选中道路
      currentSign: {} as any, //当前选中路标
      modalVisible: false, //车道弹窗
      defaultCount: 3, //初始化每条进口/出口3个车道
      wayCount: [] as number[], //每条路上的车道数量
    });

    //定义每条道路
    var RoadCross = {
      name: "方向1",

      angle: 0, // 方向角度
      origin: { x: 0, y: 0 }, // 原点，绘图中心，单位
      offset: 0.5, // 偏移量
      length: 80, // 路长125m
      cross_len: 30, // 交叉口长度m（距离原点），待完善：当右转路夹角很小时，该长度变长

      speed: 40, //路段速度

      walk: { has: 1, zebra_len: 8 }, // 人行道
      margin: 1, // 路边

      enter: {
        // 进口
        num: 3, // 进口车道数
        lane_width: 3.5, //车道宽度，米
        extend_num: 0, // 展宽数量
        extend_len: 35, // 展宽段长
        extend_width: 3, // 展宽车道宽度
        in_curv: 6, // 内侧渐变段长
        out_curv: 6, // 外侧渐变段长
        offset: 0, // 内侧偏移
        bike_lane: {
          // 非机动车道
          has: 0, // 有否1/0
          width: 2, // 车道宽度，米
          div_type: "划线", // 分割形式与宽度：划线-0.25m，护栏-1m，绿化带-1m
        },
        right_curv: 1.0, // 右转曲度[0, 1]
      },
      exit: {
        // 出口
        num: 3, // 出口车道数
        lane_width: 3.5, //车道宽度
        extend_num: 0, // 展宽数量
        extend_len: 60, // 展宽段长
        extend_width: 3, // 展宽车道宽度
        in_curv: 6, // 内侧渐变段长
        out_curv: 6, // 外侧渐变段长
        bike_lane: {
          // 非机动车道
          has: 0, // 有否1/0
          width: 2, // 车道宽度，米
          div_type: "划线", // 分割形式与宽度：划线-0.25m，护栏-1m，绿化带-1m
        },
      },

      median_strip: {
        type: "双黄线", // 分割形式：双黄线，单黄线，护栏，鱼肚线，黄斜线，绿化带
        width: 0.5, // 分割带宽：双黄线3像素，单黄线1像素，护栏4像素，鱼肚线4像素，
        safe_land: 0, // 安全岛
        turn: "否", // 提前掉头：否，停车线位置，停车线上游
      },

      wait: {
        left: 0, // 左转待转
        straight: 0, // 直行等待
        through: 0, // no wait，当>0时，穿越到方向1-n
        thr_type: "无", // 穿越方式：无，分隔贯通，隔离桩，斑马线
      },
    };

    var Draw = {
      // 绘制相关的属性
      dir: {
        // 方向
        angle: 0,
        radian: 0,
      },
      ratio: 4, // m到canvas的系数

      origin: { x: 350, y: 350 }, // 原点，length和cross_len都是基于原点的
      base_line: { x1: 350, y1: 350, x2: 0, y2: 0 }, // 基线
      length: 300,

      enter_side: {
        // 外边形状：带margin
        walk1: { x: 0, y: 0 },
        walk2: { x: 0, y: 0 },
        ext1: { x: 0, y: 0 },
        ext2: { x: 0, y: 0 },
        far: { x: 0, y: 0 },
      },
      enter_side2: {
        // 不带margin
        walk1: { x: 0, y: 0 },
        walk2: { x: 0, y: 0 },
        ext1: { x: 0, y: 0 },
        ext2: { x: 0, y: 0 },
        far: { x: 0, y: 0 },
      },
      enter_bike_div: {
        // 入口非机动车道分隔外边
        walk1: { x: 0, y: 0 },
        walk2: { x: 0, y: 0 },
        ext1: { x: 0, y: 0 },
        ext2: { x: 0, y: 0 },
        far: { x: 0, y: 0 },
      },
      enter_bike_div2: {
        // 入口非机动车道分隔内边
        walk1: { x: 0, y: 0 },
        walk2: { x: 0, y: 0 },
        ext1: { x: 0, y: 0 },
        ext2: { x: 0, y: 0 },
        far: { x: 0, y: 0 },
      },

      exit_side: {
        // 外边形状：带margin
        walk1: { x: 0, y: 0 },
        walk2: { x: 0, y: 0 },
        ext1: { x: 0, y: 0 },
        ext2: { x: 0, y: 0 },
        far: { x: 0, y: 0 },
      },
      exit_side2: {
        // 不带margin
        walk1: { x: 0, y: 0 },
        walk2: { x: 0, y: 0 },
        ext1: { x: 0, y: 0 },
        ext2: { x: 0, y: 0 },
        far: { x: 0, y: 0 },
      },
      exit_bike_div: {
        // 出口非机动车道分隔外边
        walk1: { x: 0, y: 0 },
        walk2: { x: 0, y: 0 },
        ext1: { x: 0, y: 0 },
        ext2: { x: 0, y: 0 },
        far: { x: 0, y: 0 },
      },
      exit_bike_div2: {
        // 出口非机动车道分隔内边
        walk1: { x: 0, y: 0 },
        walk2: { x: 0, y: 0 },
        ext1: { x: 0, y: 0 },
        ext2: { x: 0, y: 0 },
        far: { x: 0, y: 0 },
      },

      txt: {
        pos: { x: 0, y: 0 },
        color: "#000",
      },
    };

    /**
     * 新代码
     */
    var arr_road_cross: any[] = []; // 路交叉口全局数组
    var arr_rc_draw: any[] = []; // 路交叉口绘制数组（与上面一一对应）
    var cur_road_dir = 0; // 当前道路方向

    function onLoad() {
      create_road_cross(); // 创建路口数据结构
      init_panel();
      update_panel(); // 初始化参数面板
      render(); // 渲染图形

      // setTimeout(initZoomPan, 500);
    }

    function _ge(id: string) {
      return document.getElementById(id);
    }

    function create_road_cross() {
      var dir = sessionStorage["RoadDir"]; // 获得
      dir = "90,20, 280,190";

      dir = dir.split(",");

      arr_road_cross = [];
      var temp = JSON.stringify(RoadCross); // 数据结构模板
      for (var i = 0; i < dir.length; i++) {
        var rc = JSON.parse(temp); // road cross对象
        var angle = parseFloat(dir[i]); // 角度
        rc.angle = angle;
        rc.name = "方向" + (i + 1);
        arr_road_cross.push(rc);
      }
    }

    function init_panel() {
      var dir = sessionStorage["RoadDir"]; // 获得
      dir = "90,20, 280,190";
      dir = dir.split(",");

      var opt_str = "";
      for (var i = 0; i < dir.length; i++) {
        opt_str += "<option value='" + i + "'>方向" + (i + 1) + "</option>";
      }
      // _ge("sel_dirs")?. = opt_str;
      // _ge("sel_through")?.innerHTML =
      //   "<option value='否'>否</option>" + opt_str;
    }

    function update_panel() {
      var rc = arr_road_cross[cur_road_dir];
      // _ge("cross_len")?.value = rc.cross_len;
      // _ge("tbx_right_curv")?.value = rc.enter.right_curv;

      // _ge("tbx_road_name")?.value = rc.name;
      // _ge("tbx_offset")?.value = rc.offset;
      // _ge("sel_walk_has")?.value = rc.walk.has;
      // _ge("tbx_speed")?.value = rc.speed;
    }

    // 创建用于渲染（绘制）的数据结构
    function create_draw() {
      arr_rc_draw = []; // 清空
      var temp = JSON.stringify(Draw); // Draw模板
      for (var i = 0; i < arr_road_cross.length; i++) {
        var rc = arr_road_cross[i]; // road cross对象
        var dw = JSON.parse(temp); // road cross对应的绘制对象

        dw.dir.angle = rc.angle;
        dw.dir.radian = (Math.PI / 180) * rc.angle;
        dw.enter_side.right_curv = rc.enter.right_curv;

        // 基线
        dw.base_line.x1 = dw.origin.x;
        dw.base_line.y1 = dw.origin.y;
        var d = rc.length * dw.ratio;
        var pt = cal_point(dw, d, 0, 0);
        dw.base_line.x2 = pt.x;
        dw.base_line.y2 = pt.y;

        // 进口车道右边
        // 人行道的（walk1和walk2）
        var sd = dw.enter_side,
          sd2 = dw.enter_side2,
          dr = Math.PI * 0.5;
        var len =
          (rc.enter.num * rc.enter.lane_width +
            rc.median_strip.width +
            rc.enter.bike_lane.has * rc.enter.bike_lane.width +
            rc.margin) *
          dw.ratio;
        var len2 =
          (rc.enter.num * rc.enter.lane_width +
            rc.enter.bike_lane.has * rc.enter.bike_lane.width +
            rc.median_strip.width) *
          dw.ratio; // 不带margin
        d = (rc.cross_len - rc.walk.zebra_len) * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.walk1 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.walk1 = pt;
        d = rc.cross_len * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.walk2 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.walk2 = pt;

        // 展宽
        d = (rc.cross_len + rc.enter.extend_len) * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.ext1 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.ext1 = pt;
        d = (rc.cross_len + rc.enter.extend_len + rc.enter.in_curv) * dw.ratio;
        len =
          (rc.enter.num * rc.enter.lane_width -
            rc.enter.extend_num * rc.enter.extend_width +
            rc.median_strip.width +
            rc.margin) *
          dw.ratio;
        len2 =
          (rc.enter.num * rc.enter.lane_width -
            rc.enter.extend_num * rc.enter.extend_width +
            rc.median_strip.width) *
          dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.ext2 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.ext2 = pt;

        // 远端
        d = rc.length * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.far = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.far = pt;

        // 出口车道右边
        // 远端
        (sd = dw.exit_side), (sd2 = dw.exit_side2), (dr = -Math.PI * 0.5);
        len =
          (rc.exit.num * rc.exit.lane_width -
            rc.exit.extend_num * rc.exit.extend_width +
            rc.median_strip.width +
            rc.margin) *
          dw.ratio;
        len2 =
          (rc.exit.num * rc.exit.lane_width -
            rc.exit.extend_num * rc.exit.extend_width +
            rc.median_strip.width) *
          dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.far = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.far = pt;

        // 展宽
        d = (rc.cross_len + rc.enter.extend_len + rc.enter.in_curv) * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.ext2 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.ext2 = pt;
        d = (rc.cross_len + rc.enter.extend_len) * dw.ratio;
        len =
          (rc.exit.num * rc.exit.lane_width +
            rc.median_strip.width +
            rc.margin) *
          dw.ratio;
        len2 =
          (rc.exit.num * rc.exit.lane_width + rc.median_strip.width) * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.ext1 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.ext1 = pt;

        // 人行道
        d = rc.cross_len * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.walk2 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.walk2 = pt;
        d = (rc.cross_len - rc.walk.zebra_len) * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.walk1 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.walk1 = pt;

        // 入口非机动车道分隔外边
        var ebd_len2 = rc.margin + rc.enter.bike_lane.width;
        dw.enter_bike_div.walk2 = line_pt3(
          dw.enter_side.walk2,
          dw.enter_side2.walk2,
          rc.margin,
          ebd_len2
        );
        dw.enter_bike_div.ext1 = line_pt3(
          dw.enter_side.ext1,
          dw.enter_side2.ext1,
          rc.margin,
          ebd_len2
        );
        dw.enter_bike_div.ext2 = line_pt3(
          dw.enter_side.ext2,
          dw.enter_side2.ext2,
          rc.margin,
          ebd_len2
        );
        dw.enter_bike_div.far = line_pt3(
          dw.enter_side.far,
          dw.enter_side2.far,
          rc.margin,
          ebd_len2
        );
        ebd_len2 = rc.margin + rc.exit.bike_lane.width;
        dw.exit_bike_div.walk2 = line_pt3(
          dw.exit_side.walk2,
          dw.exit_side2.walk2,
          rc.margin,
          ebd_len2
        );
        dw.exit_bike_div.ext1 = line_pt3(
          dw.exit_side.ext1,
          dw.exit_side2.ext1,
          rc.margin,
          ebd_len2
        );
        dw.exit_bike_div.ext2 = line_pt3(
          dw.exit_side.ext2,
          dw.exit_side2.ext2,
          rc.margin,
          ebd_len2
        );
        dw.exit_bike_div.far = line_pt3(
          dw.exit_side.far,
          dw.exit_side2.far,
          rc.margin,
          ebd_len2
        );

        // 文字：方向n
        d = (rc.cross_len + rc.length * 0.3) * dw.ratio;
        len =
          (rc.enter.num * rc.enter.lane_width -
            rc.enter.extend_num * rc.enter.extend_width +
            rc.median_strip.width +
            rc.margin) *
          dw.ratio;
        pt = cal_point(dw, d, -dr, len + 20);
        dw.txt.pos = pt;

        arr_rc_draw.push(dw);
      }
    }

    // 路口Draw对象dw；距离基点d，弧度增量dr，长度len，返回新点
    function cal_point(
      dw: { dir: { radian: any }; origin: { x: number } },
      d: number,
      dr: number,
      len: number
    ) {
      // 计算
      var np = { x: 0, y: 0 }; // new point

      var tx, ty; // 临时点坐标
      var r = dw.dir.radian;
      // 基线上终点
      tx = Math.cos(r) * d + dw.origin.x;
      ty = -Math.sin(r) * d + dw.origin.x;
      // 垂直基线，交点在tx，ty，长度len的点
      np.x = len * Math.cos(r + dr) + tx;
      np.y = -len * Math.sin(r + dr) + ty;

      return np;
    }

    function render() {
      // 先创建Draw对象数组，准备渲染
      create_draw();
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      // states.cvs?.remove(); // 清空所有

      // cross的背景
      var d_str = "";
      for (var i = 0; i < arr_rc_draw.length; i++) {
        var dw1 = arr_rc_draw[i];
        var sd = dw1.enter_side;
        var pt = sd.walk2;
        if (i == 0) d_str = "M" + pt.x + "," + pt.y + " ";
        else d_str += "L" + pt.x + "," + pt.y + " ";
        sd = dw1.exit_side;
        pt = sd.walk2;
        d_str += "L" + pt.x + "," + pt.y + " ";

        var dw2;
        if (i == arr_rc_draw.length - 1) dw2 = arr_rc_draw[0];
        else dw2 = arr_rc_draw[i + 1];
        sd = dw2.enter_side;
        pt = sd.walk2;
        var insect_pt = intersect_line_point(
          dw1.exit_side.walk2,
          dw1.exit_side.walk1,
          dw2.enter_side.walk2,
          dw2.enter_side.walk1
        ); // 相邻两直线的交点
        var mid_pt = {
          x: (dw1.exit_side.walk2.x + dw2.enter_side.walk2.x) * 0.5,
          y: (dw1.exit_side.walk2.y + dw2.enter_side.walk2.y) * 0.5,
        };
        // 曲度插值法计算Q的中间点
        if (insect_pt) {
          var qpt = {
            x: mid_pt.x + (insect_pt.x - mid_pt.x) * dw2.enter_side.right_curv,
            y: mid_pt.y + (insect_pt.y - mid_pt.y) * dw2.enter_side.right_curv,
          };
          d_str += "Q" + qpt.x + "," + qpt.y + " " + pt.x + "," + pt.y + " ";
          if (i == arr_rc_draw.length - 1) d_str += "Z";
        }
      }
      var cross = document.createElementNS(states.ns, "path"); // 交叉口
      cross.setAttribute("d", d_str);
      cross.setAttribute("stroke", "rgb(162,162,162)");
      cross.setAttribute("fill", "rgba(162,162,162, 1)");
      states.cvs?.appendChild(cross);

      for (var i = 0; i < /*1*/ arr_rc_draw.length; i++) {
        var dw = arr_rc_draw[i];
        var road = document.createElementNS(states.ns, "path"); // 创建路（方向）

        // 方向形状（轮廓与背景）
        var d_str = "";

        //var pt = dw.origin;
        //d_str += "M" + pt.x + "," + pt.y + " ";

        var sd = dw.enter_side;
        var pt = sd.walk1;
        d_str += "M" + pt.x + "," + pt.y + " ";
        pt = sd.walk2;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.ext1;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.ext2;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.far;
        d_str += "L" + pt.x + "," + pt.y + " ";

        sd = dw.exit_side;
        pt = sd.far;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.ext2;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.ext1;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.walk2;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.walk1;
        d_str += "L" + pt.x + "," + pt.y + " ";

        road.setAttribute("d", d_str);
        road.setAttribute("stroke", "rgb(162,162,162)");
        road.setAttribute("fill", "rgba(162,162,162, 1)");

        states.cvs?.appendChild(road);

        // 轮廓（白线）
        d_str = "";

        var sd = dw.enter_side2;
        var pt = sd.walk1;
        //d_str += "M" + pt.x + "," + pt.y + " ";
        pt = sd.walk2;
        d_str += "M" + pt.x + "," + pt.y + " ";
        pt = sd.ext1;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.ext2;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.far;
        d_str += "L" + pt.x + "," + pt.y + " ";

        sd = dw.exit_side2;
        pt = sd.far;
        d_str += "M" + pt.x + "," + pt.y + " ";
        pt = sd.ext2;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.ext1;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.walk2;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.walk1;
        //d_str += "L" + pt.x + "," + pt.y + " ";

        var side = document.createElementNS(states.ns, "path"); // 创建SVG元素——路边
        side.setAttribute("d", d_str);
        side.setAttribute("stroke", "rgb(255,255,255)");
        side.setAttribute("stroke-width", "2");
        side.setAttribute("fill", "none");

        states.cvs?.appendChild(side);

        // 交叉口形状（轮廓与背景）
        sd = dw.enter_side;
        pt = sd.walk2;
        d_str += "M" + pt.x + "," + pt.y + " ";
        pt = sd.ext1;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.ext2;
        d_str += "L" + pt.x + "," + pt.y + " ";
        pt = sd.far;
        d_str += "L" + pt.x + "," + pt.y + " ";

        cross = document.createElementNS(states.ns, "path"); // 创建SVG元素——路边
        cross.setAttribute("d", d_str);
        cross.setAttribute("stroke", "rgb(255,255,255)");
        cross.setAttribute("stroke-width", "2");
        cross.setAttribute("fill", "none");

        var rc = arr_road_cross[i];

        // 隔离带——判断类别
        var len = rc.median_strip.width * dw.ratio;
        var d = rc.cross_len * dw.ratio;
        var dr = Math.PI * 0.5;
        pt = cal_point(dw, d, dr, len);
        var ewkb_pt = { x: pt.x, y: pt.y }; // 进口人行道横白条靠近隔离带的点：enter walk bar point
        d_str = "M" + pt.x + "," + pt.y + " ";
        d = rc.length * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        d_str += "L" + pt.x + "," + pt.y + " ";
        d = rc.cross_len * dw.ratio;
        pt = cal_point(dw, d, -dr, len);
        d_str += "M" + pt.x + "," + pt.y + " ";
        d = rc.length * dw.ratio;
        pt = cal_point(dw, d, -dr, len);
        d_str += "L" + pt.x + "," + pt.y + " ";

        var sep = document.createElementNS(states.ns, "path"); // 隔离带
        sep.setAttribute("d", d_str);
        sep.setAttribute("stroke", "rgb(255,192,44)");
        sep.setAttribute("stroke-width", "1");
        sep.setAttribute("fill", "none");
        states.cvs?.appendChild(sep);

        // 进口人行道横白条
        pt = ewkb_pt;
        d_str = "M" + pt.x + "," + pt.y + " ";
        pt = dw.enter_side2.walk2;
        d_str += "L" + pt.x + "," + pt.y + " ";

        var bar = document.createElementNS(states.ns, "path");
        bar.setAttribute("d", d_str);
        bar.setAttribute("stroke", "rgb(255,255,255)");
        bar.setAttribute("stroke-width", "2");
        states.cvs?.appendChild(bar);

        // 进口车道白线
        for (var j = 1; j <= rc.enter.num; j++) {
          // 靠近人行道的白线
          len = (rc.median_strip.width + rc.enter.lane_width * j) * dw.ratio;
          d = rc.cross_len * dw.ratio;
          dr = Math.PI * 0.5;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d = (rc.cross_len + rc.enter.extend_len - 22) * dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          var line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          states.cvs?.appendChild(line);

          // 展宽前
          d = (rc.cross_len + rc.enter.extend_len - 13) * dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d = (rc.cross_len + rc.enter.extend_len - 3) * dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          states.cvs?.appendChild(line);

          // 展宽后
        }

        // 进口车道展宽后白线
        for (var j = 1; j <= rc.enter.num - rc.enter.extend_num - 1; j++) {
          // 展宽后
          len = (rc.median_strip.width + rc.enter.lane_width * j) * dw.ratio;
          d =
            (rc.cross_len + rc.enter.extend_len + rc.enter.in_curv + 3) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.cross_len + rc.enter.extend_len + rc.enter.in_curv + 13) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          states.cvs?.appendChild(line);

          // 展宽后第2部分
          len = (rc.median_strip.width + rc.enter.lane_width * j) * dw.ratio;
          d =
            (rc.cross_len + rc.enter.extend_len + rc.enter.in_curv + 20) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.cross_len + rc.enter.extend_len + rc.enter.in_curv + 30) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          states.cvs?.appendChild(line);
        }

        // 出口车道白线
        for (var j = 1; j <= rc.exit.num; j++) {
          // 靠近人行道的白线 —— 展宽前
          var num = rc.exit.extend_len / 15;
          for (var n = 0; n < num; n++) {
            len = (rc.median_strip.width + rc.exit.lane_width * j) * dw.ratio;
            d = (rc.cross_len + 16 * n) * dw.ratio;
            dr = -Math.PI * 0.5;
            pt = cal_point(dw, d, dr, len);
            d_str = "M" + pt.x + "," + pt.y + " ";
            d = (rc.cross_len + 16 * n + 9) * dw.ratio;
            pt = cal_point(dw, d, dr, len);
            d_str += "L" + pt.x + "," + pt.y + " ";

            line = document.createElementNS(states.ns, "path");
            line.setAttribute("d", d_str);
            line.setAttribute("stroke", "rgb(255,255,255)");
            line.setAttribute("stroke-width", "2");
            states.cvs?.appendChild(line);
          }
        }

        // 人行道
        if (rc.walk.has == 1) {
          var bkln_width = 0.25; // 非机动车道划线宽度 bike lane width 0.25m
          if (rc.enter.bike_lane.div_type != "划线") bkln_width = 1; // 护栏和绿化带宽度为1m
          // 方向n在近cross处总宽度（m）
          var w_total_width =
            rc.median_strip.width +
            rc.enter.num * rc.enter.lane_width +
            rc.enter.bike_lane.has * (rc.enter.bike_lane.width + bkln_width) +
            rc.exit.num * rc.exit.lane_width +
            rc.exit.bike_lane.has * (rc.exit.bike_lane.width + bkln_width);
          var zebra_num = parseInt((w_total_width / 2).toString()); // 斑马线条数（每2m一条）
          // 把入出口walk2均分为 zebra_num 段，从入口开始增量计算坐标
          var delta1_x =
            (dw.enter_side2.walk1.x - dw.exit_side2.walk1.x) / zebra_num;
          var delta1_y =
            (dw.enter_side2.walk1.y - dw.exit_side2.walk1.y) / zebra_num;
          var delta2_x =
            (dw.enter_side2.walk2.x - dw.exit_side2.walk2.x) / zebra_num;
          var delta2_y =
            (dw.enter_side2.walk2.y - dw.exit_side2.walk2.y) / zebra_num;
          pt = { x: 0, y: 0 };
          for (var zn = 1; zn < zebra_num; zn++) {
            pt.x = dw.enter_side2.walk1.x - delta1_x * zn;
            pt.y = dw.enter_side2.walk1.y - delta1_y * zn;
            d_str = "M" + pt.x + "," + pt.y + " ";
            pt.x = dw.enter_side2.walk2.x - delta2_x * zn;
            pt.y = dw.enter_side2.walk2.y - delta2_y * zn;
            d_str += "L" + pt.x + "," + pt.y + " ";

            line = document.createElementNS(states.ns, "path");
            line.setAttribute("d", d_str);
            line.setAttribute("stroke", "rgb(255,255,255)");
            line.setAttribute("stroke-width", "4"); // 线宽为4
            pt.x = -8 * Math.cos(dw.dir.radian);
            pt.y = 8 * Math.sin(dw.dir.radian);
            line.setAttribute(
              "transform",
              "translate(" + pt.x + "," + pt.y + ")"
            );
            states.cvs?.appendChild(line);
          }
        }

        // 出口车道展宽后白线
        for (var j = 1; j <= rc.exit.num - rc.exit.extend_num - 1; j++) {
          // 展宽后
          dr = -Math.PI * 0.5;
          len = (rc.median_strip.width + rc.exit.lane_width * j) * dw.ratio;
          d =
            (rc.cross_len + rc.exit.extend_len + rc.exit.in_curv + 3) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.cross_len + rc.exit.extend_len + rc.exit.in_curv + 13) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          states.cvs?.appendChild(line);

          // 展宽后第2部分
          len = (rc.median_strip.width + rc.exit.lane_width * j) * dw.ratio;
          d =
            (rc.cross_len + rc.exit.extend_len + rc.exit.in_curv + 20) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.cross_len + rc.exit.extend_len + rc.exit.in_curv + 30) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          states.cvs?.appendChild(line);
        }

        // 入口非机动车道
        if (rc.enter.bike_lane.has == 1) {
          var bk_div = dw.enter_bike_div;
          pt = bk_div.walk2;
          d_str = "M" + pt.x + "," + pt.y + " ";
          pt = bk_div.ext1;
          d_str += "L" + pt.x + "," + pt.y + " ";
          pt = bk_div.ext2;
          d_str += "L" + pt.x + "," + pt.y + " ";
          pt = bk_div.far;
          d_str += "L" + pt.x + "," + pt.y + " ";

          var bkdiv = document.createElementNS(states.ns, "path");
          bkdiv.setAttribute("d", d_str);
          bkdiv.setAttribute("stroke", "rgb(0,255,0)");
          bkdiv.setAttribute("stroke-width", "1");
          bkdiv.setAttribute("fill", "none");
          states.cvs?.appendChild(bkdiv);
        }

        // 出口口非机动车道
        if (rc.exit.bike_lane.has == 1) {
          bk_div = dw.exit_bike_div;
          pt = bk_div.walk2;
          d_str = "M" + pt.x + "," + pt.y + " ";
          pt = bk_div.ext1;
          d_str += "L" + pt.x + "," + pt.y + " ";
          pt = bk_div.ext2;
          d_str += "L" + pt.x + "," + pt.y + " ";
          pt = bk_div.far;
          d_str += "L" + pt.x + "," + pt.y + " ";

          var bkdiv = document.createElementNS(states.ns, "path");
          bkdiv.setAttribute("d", d_str);
          bkdiv.setAttribute("stroke", "rgb(0,255,0)");
          bkdiv.setAttribute("stroke-width", "1");
          bkdiv.setAttribute("fill", "none");
          states.cvs?.appendChild(bkdiv);
        }
      }

      // cross的右转白线 和旋转的文字“方向n”
      for (var i = 0; i < arr_rc_draw.length; i++) {
        rc = arr_road_cross[i];
        var dw = arr_rc_draw[i];
        var sd = dw.exit_side2;
        var pt = sd.walk2;
        d_str = "M" + pt.x + "," + pt.y + " ";

        var dw2;
        if (i == arr_rc_draw.length - 1) dw2 = arr_rc_draw[0];
        else dw2 = arr_rc_draw[i + 1];
        sd = dw2.enter_side2;
        pt = sd.walk2;
        var insect_pt = intersect_line_point(
          dw.exit_side2.walk2,
          dw.exit_side2.walk1,
          dw2.enter_side2.walk2,
          dw2.enter_side2.walk1
        ); // 相邻两直线的交点
        var mid_pt = {
          x: (dw.exit_side2.walk2.x + dw2.enter_side2.walk2.x) * 0.5,
          y: (dw.exit_side2.walk2.y + dw2.enter_side2.walk2.y) * 0.5,
        };
        // 曲度插值法计算Q的中间点
        var qpt = {
          x: mid_pt.x + (insect_pt.x - mid_pt.x) * dw2.enter_side.right_curv,
          y: mid_pt.y + (insect_pt.y - mid_pt.y) * dw2.enter_side.right_curv,
        };
        d_str += "Q" + qpt.x + "," + qpt.y + " " + pt.x + "," + pt.y + " ";

        var cross = document.createElementNS(states.ns, "path"); //交叉口右转白线
        cross.setAttribute("d", d_str);
        cross.setAttribute("stroke", "rgb(255,255,255)");
        cross.setAttribute("stroke-width", "2");
        cross.setAttribute("fill", "none");

        states.cvs?.appendChild(cross);

        var txt = document.createElementNS(states.ns, "text"); // 方向n文本
        txt.innerHTML = rc.name;
        var tcolor = "rgb(255,0,0)";
        //TODO
        if (1 != i) tcolor = "rgb(0,0,0)";
        txt.setAttribute("fill", tcolor);
        txt.setAttribute("text-anchor", "middle");
        var angle = -dw.dir.angle;
        if (angle < -120 && angle > -270) angle = angle + 180; // 文字朝上
        txt.setAttribute(
          "transform",
          "translate(" +
            dw.txt.pos.x +
            "," +
            dw.txt.pos.y +
            ") rotate(" +
            angle +
            ")"
        );
        //txt.setAttribute("stroke-width", "200");

        states.cvs?.appendChild(txt);
      }
    }

    function onRoad(ob: { id: any }) {
      alert(ob.id);
    }

    /**
     * 属性操作
     */
    function on_sel_dirs_change(ob: { value: string }) {
      // 方向选择
      cur_road_dir = parseInt(ob.value);
      update_panel();
      render();
    }

    function on_road_name_change(ob: { value: any }) {
      arr_road_cross[cur_road_dir].name = ob.value;
    }

    function on_prop_change(prop: string, t: any, ob: { value: string }) {
      var v;
      switch (t) {
        case "i": // int
          v = parseInt(ob.value);
          break;
        case "f": // float
          v = parseFloat(ob.value);
          break;
        case "s": // string
          v = ob.value;
          break;
      }
      eval("arr_road_cross[cur_road_dir]." + prop + " = v;");
      render();
    }

    // function initZoomPan() {
    //   var eventsHandler;

    //   eventsHandler = {
    //     haltEventListeners: [
    //       "touchstart",
    //       "touchend",
    //       "touchmove",
    //       "touchleave",
    //       "touchcancel",
    //     ],
    //     init: function (options) {
    //       var instance = options.instance,
    //         initialScale = 1,
    //         pannedX = 0,
    //         pannedY = 0;

    //       // Init Hammer
    //       // Listen only for pointer and touch events
    //       this.hammer = Hammer(options.svgElement, {
    //         inputClass: Hammer.SUPPORT_POINTER_EVENTS
    //           ? Hammer.PointerEventInput
    //           : Hammer.TouchInput,
    //       });

    //       // Enable pinch
    //       this.hammer.get("pinch").set({ enable: true });

    //       // Handle double tap
    //       this.hammer.on("doubletap", function (ev) {
    //         instance.zoomIn();
    //       });

    //       // Handle pan
    //       this.hammer.on("panstart panmove", function (ev) {
    //         // On pan start reset panned variables
    //         if (ev.type === "panstart") {
    //           pannedX = 0;
    //           pannedY = 0;
    //         }

    //         // Pan only the difference
    //         instance.panBy({ x: ev.deltaX - pannedX, y: ev.deltaY - pannedY });
    //         pannedX = ev.deltaX;
    //         pannedY = ev.deltaY;
    //       });

    //       // Handle pinch
    //       this.hammer.on("pinchstart pinchmove", function (ev) {
    //         // On pinch start remember initial zoom
    //         if (ev.type === "pinchstart") {
    //           initialScale = instance.getZoom();
    //           instance.zoomAtPoint(initialScale * ev.scale, {
    //             x: ev.center.x,
    //             y: ev.center.y,
    //           });
    //         }

    //         instance.zoomAtPoint(initialScale * ev.scale, {
    //           x: ev.center.x,
    //           y: ev.center.y,
    //         });
    //       });

    //       // Prevent moving the page on some devices when panning over SVG
    //       options.svgElement.addEventListener("touchmove", function (e) {
    //         e.preventDefault();
    //       });
    //     },

    //     destroy: function () {
    //       this.hammer.destroy();
    //     },
    //   };

    //   // Expose to window namespace for testing purposes
    //   window.panZoom = svgPanZoom("#rc_canvas", {
    //     zoomEnabled: true,
    //     controlIconsEnabled: true,
    //     fit: 0,
    //     center: 1,
    //     customEventsHandler: eventsHandler,
    //   });
    // }

    onMounted(() => {
      onLoad();
    });

    const drawCross = () => {};
    const handleConfirmSign = (key: any) => {
      console.log(key);
    };
    const onChangeRoadCount = () => {};
    const onDirectionChange = () => {};

    return {
      ...toRefs(states),
      ...toRefs(road_info),
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      roadSigns,
      drawCross,
      handleConfirmSign,
      onChangeRoadCount,
      onDirectionChange,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
