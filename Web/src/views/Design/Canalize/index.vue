<template>
  <div class="basic-main">
    <div class="func">功能区</div>
    <!-- 图示 -->
    <svg id="canvas"></svg>
    <!-- 参数 -->
    <div class="menu">
      <div class="form" v-if="canalize_info.length > 0">
        <div class="header">交叉口属性</div>
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="方向">
                  <a-select
                    v-model:value="cur_road_dir"
                    size="small"
                    class="form-width"
                    @change="on_sel_dirs_change"
                  >
                    <a-select-option
                      v-for="(_, index) in road_attr"
                      :key="index"
                      :value="index"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="非机动车道">
                  <a-button
                    type="primary"
                    size="small"
                    class="line-button"
                    @click="on_bike_lane_set(1)"
                  >
                    设置
                  </a-button>
                  <a-button
                    type="default"
                    size="small"
                    class="line-button"
                    style="margin-left: 3px"
                    @click="on_bike_lane_set(0)"
                  >
                    取消
                  </a-button>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="交叉口大小">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].cross_len"
                    :min="25"
                    :max="40"
                    size="small"
                    class="form-width"
                    @change="on_prop_change"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="右转曲度">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].enter.right_curv"
                    @change="on_prop_change"
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
                    v-model:value="canalize_info[cur_road_dir].name"
                    @change="on_prop_change"
                    class="form-width"
                    size="small"
                  />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="偏移量">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].offset"
                    @change="on_prop_change"
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
                    v-model:value="canalize_info[cur_road_dir].walk.has"
                    @change="on_prop_change"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option :value="1"> 是 </a-select-option>
                    <a-select-option :value="0"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="路段速度">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].speed"
                    @change="on_prop_change"
                    :min="0"
                    :max="300"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">Km/h</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="左转待转">
                  <a-select
                    v-model:value="canalize_info[cur_road_dir].wait.left"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option :value="1"> 是 </a-select-option>
                    <a-select-option :value="0"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="直行待转">
                  <a-select
                    v-model:value="canalize_info[cur_road_dir].wait.straight"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option :value="1"> 是 </a-select-option>
                    <a-select-option :value="0"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="穿越到">
                  <a-select
                    v-model:value="canalize_info[cur_road_dir].wait.through"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="否">否</a-select-option>
                    <a-select-option
                      v-for="(_, index) in road_attr"
                      :key="index"
                      :value="index"
                    >
                      方向{{ index + 1 }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="穿越方式">
                  <a-select
                    v-model:value="canalize_info[cur_road_dir].wait.thr_type"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="无"> 无 </a-select-option>
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
                  <a-tooltip>
                    <template #title>{{
                      canalize_info[cur_road_dir].canalize.type
                    }}</template>
                    <a-select
                      v-model:value="canalize_info[cur_road_dir].canalize.type"
                      size="small"
                      class="form-width"
                    >
                      <a-select-option
                        v-for="item in canalizeTypeOption"
                        :key="item.value"
                        :value="item.value"
                        :title="item.label"
                      >
                        {{ item.label }}
                      </a-select-option>
                    </a-select>
                  </a-tooltip>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="右转车道">
                  <a-input-number
                    v-model:value="
                      canalize_info[cur_road_dir].canalize.right_count
                    "
                    @change="on_prop_change"
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
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="进口车道">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].enter.num"
                    @change="on_prop_change"
                    :min="0"
                    :max="6"
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
                    v-model:value="canalize_info[cur_road_dir].enter.lane_width"
                    @change="on_prop_change"
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
                    v-model:value="canalize_info[cur_road_dir].enter.extend_num"
                    @change="on_prop_change"
                    :min="-2"
                    :max="2"
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
                    v-model:value="
                      canalize_info[cur_road_dir].enter.extend_width
                    "
                    @change="on_prop_change"
                    :min="0"
                    :max="6"
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
                    v-model:value="canalize_info[cur_road_dir].enter.extend_len"
                    @change="on_prop_change"
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
                    v-model:value="canalize_info[cur_road_dir].enter.out_curv"
                    @change="on_prop_change"
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
                    v-model:value="canalize_info[cur_road_dir].enter.offset"
                    @change="on_prop_change"
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
                    v-model:value="canalize_info[cur_road_dir].enter.in_curv"
                    @change="on_prop_change"
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
        <div class="content">
          <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
            <a-row>
              <a-col :span="12">
                <a-form-item label="出口车道">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].exit.num"
                    @change="on_prop_change"
                    :min="0"
                    :max="6"
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
                    v-model:value="canalize_info[cur_road_dir].exit.lane_width"
                    @change="on_prop_change"
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
                    v-model:value="canalize_info[cur_road_dir].exit.extend_num"
                    @change="on_prop_change"
                    :min="-2"
                    :max="2"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">个</div>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="展宽车道宽度">
                  <!-- todo -->
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].exit.todo"
                    @change="on_prop_change"
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
                    v-model:value="canalize_info[cur_road_dir].exit.extend_len"
                    @change="on_prop_change"
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
                  <!-- todo -->
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].exit.todo"
                    @change="on_prop_change"
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
                    v-model:value="
                      canalize_info[cur_road_dir].median_strip.type
                    "
                    size="small"
                    class="form-width"
                  >
                    <a-select-option
                      v-for="item in medianStripTypeOption"
                      :value="item.value"
                      :key="item.value"
                    >
                      {{ item.label }}
                    </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="分割宽度">
                  <a-input-number
                    v-model:value="
                      canalize_info[cur_road_dir].median_strip.width
                    "
                    @change="on_prop_change"
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
                    v-model:value="
                      canalize_info[cur_road_dir].median_strip.safe_land
                    "
                    size="small"
                    class="form-width"
                  >
                    <a-select-option :value="1"> 是 </a-select-option>
                    <a-select-option :value="0"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="提前掉头">
                  <a-select
                    v-model:value="
                      canalize_info[cur_road_dir].median_strip.turn
                    "
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="否"> 否 </a-select-option>
                    <a-select-option value="停车线位置">
                      停车线位置
                    </a-select-option>
                    <a-select-option value="停车线上游">
                      停车线上游
                    </a-select-option>
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
                    v-model:value="
                      canalize_info[cur_road_dir].enter.bike_lane.has
                    "
                    @change="on_prop_change"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option :value="1"> 是 </a-select-option>
                    <a-select-option :value="0"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="出口">
                  <a-select
                    v-model:value="
                      canalize_info[cur_road_dir].exit.bike_lane.has
                    "
                    @change="on_prop_change"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option :value="1"> 是 </a-select-option>
                    <a-select-option :value="0"> 否 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="车道宽度">
                  <a-input-number
                    v-model:value="
                      canalize_info[cur_road_dir].enter.bike_lane.width
                    "
                    @change="on_prop_change"
                    :disabled="!canalize_info[cur_road_dir].enter.bike_lane.has"
                    :min="1"
                    :max="4"
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
                    v-model:value="
                      canalize_info[cur_road_dir].exit.bike_lane.width
                    "
                    @change="on_prop_change"
                    :disabled="!canalize_info[cur_road_dir].exit.bike_lane.has"
                    :min="1"
                    :max="4"
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
                    v-model:value="
                      canalize_info[cur_road_dir].enter.bike_lane.div_type
                    "
                    :disabled="!canalize_info[cur_road_dir].enter.bike_lane.has"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="划线"> 划线 </a-select-option>
                    <a-select-option value="护栏"> 护栏 </a-select-option>
                    <a-select-option value="绿化带"> 绿化带 </a-select-option>
                  </a-select>
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="分割形式">
                  <a-select
                    v-model:value="
                      canalize_info[cur_road_dir].exit.bike_lane.div_type
                    "
                    :disabled="!canalize_info[cur_road_dir].exit.bike_lane.has"
                    size="small"
                    class="form-width"
                  >
                    <a-select-option value="划线"> 划线 </a-select-option>
                    <a-select-option value="护栏"> 护栏 </a-select-option>
                    <a-select-option value="绿化带"> 绿化带 </a-select-option>
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
import { defineComponent, inject, onMounted, reactive, ref, toRefs } from "vue";
import {
  canalizeTypeOption,
  getDirectionIndex,
  getRoadDefaultSign,
  medianStripTypeOption,
  roadSigns,
} from "./index";
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
      cur_road_dir: 0, //当前道路方向
      currentSign: {} as any, //当前选中路标
      modalVisible: false, //车道弹窗
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

      canalize: {
        //渠化属性
        type: "否", //右转渠化
        right_count: 1, //右转车道
      },

      enter: {
        // 进口
        num: 3, // 进口车道数
        lane_width: 3.5, //车道宽度，米
        extend_num: 0, // 展宽数量
        extend_len: 45, // 展宽段长
        extend_width: 3, // 展宽车道宽度
        in_curv: 12, // 内侧渐变段长
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
        extend_len: 20, // 展宽段长
        extend_width: 3, // 展宽车道宽度
        in_curv: 12, // 内侧渐变段长
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
        through: "否", // no wait，当>0时，穿越到方向1-n
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

      origin: { x: states.cx, y: states.cy }, // 原点，length和cross_len都是基于原点的
      base_line: { x1: states.cx, y1: states.cy, x2: 0, y2: 0 }, // 基线
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
    // var arr_road_cross: any[] = []; // 路交叉口全局数组
    var arr_rc_draw: any[] = []; // 路交叉口绘制数组（与上面一一对应）

    function onLoad() {
      create_road_cross(); // 创建路口数据结构
      // init_panel();
      // update_panel(); // 初始化参数面板
      render(); // 渲染图形
      // setTimeout(initZoomPan, 500);
    }

    function onEditLoad() {
      update_road_corss(); //更新路口角度
      render(); // 渲染图形
    }

    function _ge(id: string) {
      return document.getElementById(id);
    }

    function create_road_cross() {
      var dir = road_info.road_attr.map((r) => r.angle);
      road_info.canalize_info.length = 0;
      var temp = JSON.stringify(RoadCross); // 数据结构模板
      for (var i = 0; i < dir.length; i++) {
        var rc = JSON.parse(temp); // road cross对象
        var angle = parseFloat(dir[i]); // 角度
        rc.angle = angle;
        rc.name = "方向" + (i + 1);
        road_info.canalize_info.push(rc);
      }
    }

    function update_road_corss() {
      var dir = road_info.road_attr.map((r) => r.angle);
      road_info.canalize_info.map((c, index) => {
        var angle = parseFloat(dir[index]); // 角度
        c.angle = angle;
      });
    }

    // 创建用于渲染（绘制）的数据结构
    function create_draw() {
      arr_rc_draw = []; // 清空
      var temp = JSON.stringify(Draw); // Draw模板
      for (var i = 0; i < road_info.canalize_info.length; i++) {
        var rc = road_info.canalize_info[i]; // road cross对象
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
      states.cvs = _ge("canvas");
      document.querySelectorAll("path").forEach((e) => {
        //清空所有路径
        e.remove();
      });
      document.querySelectorAll("text").forEach((e) => {
        //清空所有文字
        e.remove();
      });
      document.querySelectorAll("g").forEach((e) => {
        //清空所有路标
        e.remove();
      });

      //画路标
      // create_road_sign();

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
        if (i == 0) dw2 = arr_rc_draw[arr_rc_draw.length - 1];
        else dw2 = arr_rc_draw[i - 1];
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
      // cross.setAttribute("stroke", "rgb(162,162,162)");
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
        // road.setAttribute("stroke", "rgb(162,162,162)");
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

        var rc = road_info.canalize_info[i];

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
        sep.setAttribute("stroke", "#ffa500");
        sep.setAttribute("stroke-width", "2");
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
          bkdiv.setAttribute("stroke-width", "2");
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
          bkdiv.setAttribute("stroke-width", "2");
          bkdiv.setAttribute("fill", "none");
          states.cvs?.appendChild(bkdiv);
        }
      }

      // cross的右转白线 和旋转的文字“方向n”
      for (var i = 0; i < arr_rc_draw.length; i++) {
        rc = road_info.canalize_info[i];
        var dw = arr_rc_draw[i];
        var sd = dw.exit_side2;
        var pt = sd.walk2;
        d_str = "M" + pt.x + "," + pt.y + " ";

        var dw2;
        if (i == 0) dw2 = arr_rc_draw[arr_rc_draw.length - 1];
        else dw2 = arr_rc_draw[i - 1];
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
        if (insect_pt) {
          // 曲度插值法计算Q的中间点
          var qpt = {
            x: mid_pt.x + (insect_pt.x - mid_pt.x) * dw2.enter_side.right_curv,
            y: mid_pt.y + (insect_pt.y - mid_pt.y) * dw2.enter_side.right_curv,
          };
          d_str += "Q" + qpt.x + "," + qpt.y + " " + pt.x + "," + pt.y + " ";
        }
        var cross = document.createElementNS(states.ns, "path"); //交叉口右转白线
        cross.setAttribute("d", d_str);
        cross.setAttribute("stroke", "rgb(255,255,255)");
        cross.setAttribute("stroke-width", "2");
        cross.setAttribute("fill", "none");

        states.cvs?.appendChild(cross);

        var txt = document.createElementNS(states.ns, "text"); // 方向n文本
        txt.innerHTML = rc.name;
        var tcolor = "rgb(255,0,0)";
        if (states.cur_road_dir != i) tcolor = "rgb(0,0,0)";
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

    // function create_road_sign() {
    //   let road = 0;
    //   var way_count = 0; //每条路单行三条道
    //   for (var i = 0; i < states.cross_line_pts.length; i++) {
    //     let all_count = states.wayCount[road] - 0.5; //每条路全部数量六条道
    //     way_count = road_info.canalize_info[road].exit.num; //出口车道数量
    //     var pt = states.cross_line_pts[i];
    //     if (i > 0 && i % 2 !== 0) {
    //       var prevPt = states.cross_line_pts[i - 1];
    //       //几条道路（默认双向六条）
    //       for (let way_idx = 0; way_idx < all_count; way_idx++) {
    //         var is_reverse = way_idx < way_count;
    //         var right_idx = way_idx - way_count;
    //         var is_last = all_count === way_idx + 1;
    //         //左侧道路、右侧道路离中心距离微调
    //         var k = way_idx >= way_count ? way_idx : way_idx * 0.9;
    //         //(x1+k(x2-x1)/n,y1+k(y2-y1)/n)线段n等分公式
    //         var wayPt = [
    //           prevPt[0] + (k * (pt[0] - prevPt[0])) / all_count,
    //           prevPt[1] + (k * (pt[1] - prevPt[1])) / all_count,
    //         ];
    //         var path = document.createElementNS(states.ns, "path");
    //         path.setAttribute("id", `road_sign_${i}`);
    //         path.setAttribute(
    //           "d",
    //           getRoadDefaultSign(right_idx, is_reverse, is_last)
    //         );
    //         path.setAttribute("fill", "#ffffff");
    //         path.setAttribute("width", "100");
    //         path.setAttribute("height", "100");

    //         var svg = document.createElementNS(states.ns, "svg");
    //         svg.appendChild(path);
    //         svg.setAttribute("viewBox", "0 0 1024 1024");
    //         svg.setAttribute("height", "40");
    //         svg.setAttribute("width", "18");
    //         svg.setAttribute("x", wayPt[0].toString());
    //         svg.setAttribute("y", wayPt[1].toString());
    //         var g = document.createElementNS(states.ns, "g");
    //         g.setAttribute(
    //           "transform",
    //           `rotate(${270 - road_info.road_attr[road].angle} ${wayPt[0]} ${
    //             wayPt[1]
    //           })`
    //         );
    //         g.setAttribute("style", "cursor:pointer");
    //         if (way_idx >= way_count) {
    //           g.addEventListener("click", handleChoose, false);
    //         }
    //         g.appendChild(svg);
    //         states.cvs?.appendChild(g);
    //       }
    //       road++;
    //     }
    //   }
    // }

    //选中路标
    function handleChoose(e: any) {
      states.currentSign = e.path[0];
      states.modalVisible = true;
    }

    /**
     * 属性操作
     */
    // 方向选择
    function on_sel_dirs_change() {
      // update_panel();
      render();
    }

    //设置机动车道
    function on_bike_lane_set(is_set: number) {
      road_info.canalize_info[states.cur_road_dir].enter.bike_lane.has = is_set;
      road_info.canalize_info[states.cur_road_dir].exit.bike_lane.has = is_set;
      render();
    }

    //页面属性变化
    function on_prop_change() {
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
      //已有数据，渲染
      if (road_info.canalize_info.length > 0) {
        onEditLoad();
      } else {
        //否则全量初始化
        onLoad();
      }
    });

    const handleConfirmSign = (key: any) => {
      console.log(key);
    };

    return {
      ...toRefs(states),
      ...toRefs(road_info),
      labelCol: { span: 10 },
      wrapperCol: { span: 12 },
      roadSigns,
      on_prop_change,
      on_sel_dirs_change,
      on_bike_lane_set,
      handleConfirmSign,
      canalizeTypeOption,
      medianStripTypeOption,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
