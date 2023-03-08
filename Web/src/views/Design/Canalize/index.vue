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
                    :min="20"
                    :max="25"
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
                    <template #title>
                      {{ canalize_info[cur_road_dir].canalize.type }}
                    </template>
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
                    @change="on_prop_change('enter_num')"
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
                    :disabled="!canalize_info[cur_road_dir].enter.num"
                    :min="3.5"
                    :max="5.5"
                    :step="0.25"
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
                    :disabled="!canalize_info[cur_road_dir].enter.num"
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
                    :disabled="!canalize_info[cur_road_dir].enter.num"
                    :min="0"
                    :max="6"
                    :step="0.25"
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
                    :disabled="!canalize_info[cur_road_dir].enter.num"
                    :min="30"
                    :max="40"
                    :step="5"
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
                    :disabled="!canalize_info[cur_road_dir].enter.num"
                    :min="0"
                    :max="4"
                    :step="0.5"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <!-- <a-col :span="12">
                <a-form-item label="外侧渐变段长">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].enter.out_curv"
                    @change="on_prop_change"
                    :disabled="!canalize_info[cur_road_dir].enter.num"
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
                <a-form-item label="内侧渐变段长">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].enter.in_curv"
                    @change="on_prop_change"
                    :disabled="!canalize_info[cur_road_dir].enter.num"
                    :min="0"
                    :max="50"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col> -->
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
                    @change="on_prop_change('exit_num')"
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
                    :disabled="!canalize_info[cur_road_dir].exit.num"
                    :min="3.5"
                    :max="5.5"
                    :step="0.25"
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
                    :disabled="!canalize_info[cur_road_dir].exit.num"
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
                      canalize_info[cur_road_dir].exit.extend_width
                    "
                    @change="on_prop_change"
                    :disabled="!canalize_info[cur_road_dir].exit.num"
                    :min="0"
                    :max="4"
                    :step="0.25"
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
                    :disabled="!canalize_info[cur_road_dir].exit.num"
                    :min="30"
                    :max="40"
                    :step="5"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col>
              <!-- <a-col :span="12">
                <a-form-item label="渐变段长">
                  <a-input-number
                    v-model:value="canalize_info[cur_road_dir].exit.out_curv"
                    @change="on_prop_change"
                    :disabled="!canalize_info[cur_road_dir].exit.num"
                    :min="0"
                    :max="50"
                    :step="1"
                    size="small"
                    class="form-width"
                  />
                  <div class="span-unit">米</div>
                </a-form-item>
              </a-col> -->
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
                    @change="on_prop_change"
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
                    :disabled="
                      ['黄斜线', '绿化带'].indexOf(
                        canalize_info[cur_road_dir].median_strip.type
                      ) === -1
                    "
                    @change="on_prop_change"
                    :min="0.5"
                    :max="2"
                    :step="0.25"
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
                    :step="0.25"
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
                    :step="0.25"
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
                    @change="on_prop_change"
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
                    @change="on_prop_change"
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
          <path
            :d="road.path"
            fill="#ffffff"
            :title="road.name"
            id="notDelete"
          ></path>
        </svg>
      </div>
    </a-modal>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, reactive, toRefs } from "vue";
import {
  canalizeTypeOption,
  getRoadDefaultSign,
  medianStripTypeOption,
  roadSigns,
  setIsolationStyle,
  getCrossLenByTwoRoad,
  roadSignKey,
  Draw,
} from "./index";
import Container from "../../../components/Container/index.vue";
import { DragOutlined } from "@ant-design/icons-vue";
import { intersect_line_point, line_pt3 } from "../../../utils/common";
import {
  create_road_cross,
  create_sign,
  plans,
  roadStates,
  update_road_corss,
} from "..";
import { road_model } from "../data";

export default defineComponent({
  components: { Container, DragOutlined },
  setup() {
    //道路信息
    const road_info = reactive(JSON.parse(JSON.stringify(road_model)));

    const states = reactive({
      ns: "",
      cvs: null as HTMLElement | null,
      cx: 350, //圆心x
      cy: 350, //圆心y
      cur_road_dir: 0, //当前道路方向
      cross_line_pts: [] as any[], //cross点集合
      currentSign: {} as any, //当前选中路标
      modalVisible: false, //车道弹窗
    });

    /**
     * 新代码
     */
    // var arr_road_cross: any[] = []; // 路交叉口全局数组
    var arr_rc_draw: any[] = []; // 路交叉口绘制数组（与上面一一对应）

    function onLoad(rf: any) {
      create_road_cross(rf); // 创建路口数据结构
      onLoadChange(rf); // 渲染图形
    }

    function onLoadEdit(rf: any) {
      update_road_corss(rf); //更新路口角度
      onLoadChange(rf); // 渲染图形
    }

    function onLoadChange(rf: any) {
      Object.assign(road_info, rf);
      removeAll(); //先清空，再绘制
      render(); // 渲染图形
    }

    function _ge(id: string) {
      return document.getElementById(id);
    }

    // 创建用于渲染（绘制）的数据结构
    function create_draw() {
      arr_rc_draw = []; // 清空
      var temp = JSON.stringify(Draw); // Draw模板
      for (var i = 0; i < road_info.canalize_info.length; i++) {
        var rc = road_info.canalize_info[i]; // road cross对象
        const addLen = getCrossLenByTwoRoad(road_info, i);
        rc.cross_len_new = rc.cross_len + addLen;
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
        d = (rc.cross_len_new - rc.walk.zebra_len) * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.walk1 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.walk1 = pt;
        d = rc.cross_len_new * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.walk2 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.walk2 = pt;

        // 展宽
        d = (rc.cross_len_new + rc.enter.extend_len) * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.ext1 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.ext1 = pt;
        d =
          (rc.cross_len_new + rc.enter.extend_len + rc.enter.in_curv) *
          dw.ratio;
        len =
          (rc.enter.num * rc.enter.lane_width -
            rc.enter.extend_num * rc.enter.extend_width +
            rc.median_strip.width +
            rc.margin +
            (rc.enter.bike_lane.has ? rc.enter.bike_lane.width : 0)) *
          dw.ratio;
        len2 =
          (rc.enter.num * rc.enter.lane_width -
            rc.enter.extend_num * rc.enter.extend_width +
            rc.median_strip.width +
            (rc.enter.bike_lane.has ? rc.enter.bike_lane.width : 0)) *
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
            rc.margin +
            (rc.exit.bike_lane.has ? rc.exit.bike_lane.width : 0)) *
          dw.ratio;
        len2 =
          (rc.exit.num * rc.exit.lane_width -
            rc.exit.extend_num * rc.exit.extend_width +
            rc.median_strip.width +
            (rc.exit.bike_lane.has ? rc.exit.bike_lane.width : 0)) *
          dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.far = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.far = pt;

        // 展宽
        d =
          (rc.cross_len_new + rc.exit.extend_len + rc.exit.in_curv) * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.ext2 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.ext2 = pt;
        d = (rc.cross_len_new + rc.exit.extend_len) * dw.ratio;
        len =
          (rc.exit.num * rc.exit.lane_width +
            rc.median_strip.width +
            rc.margin +
            (rc.exit.bike_lane.has ? rc.exit.bike_lane.width : 0)) *
          dw.ratio;
        len2 =
          (rc.exit.num * rc.exit.lane_width +
            rc.median_strip.width +
            (rc.exit.bike_lane.has ? rc.exit.bike_lane.width : 0)) *
          dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.ext1 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.ext1 = pt;

        // 人行道
        d = rc.cross_len_new * dw.ratio;
        pt = cal_point(dw, d, dr, len);
        sd.walk2 = pt;
        pt = cal_point(dw, d, dr, len2);
        sd2.walk2 = pt;
        d = (rc.cross_len_new - rc.walk.zebra_len) * dw.ratio;
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
        d = (rc.cross_len_new + rc.length * 0.3) * dw.ratio;
        len =
          (rc.enter.num * rc.enter.lane_width -
            rc.median_strip.width +
            rc.margin +
            rc.enter.bike_lane.width) *
          dw.ratio;
        pt = cal_point(dw, d, -dr, len + 20);
        dw.txt.pos = pt;

        //右边路标
        rc.road_sign.enter;
        for (var j = 0; j < rc.enter.num; j++) {
          dw.road_sign.enter[j] = rc.road_sign.enter[j];
        }
        //左边路标
        for (var j = 0; j < rc.exit.num; j++) {
          dw.road_sign.exit[j] = rc.road_sign.exit[j];
        }

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
      states.cross_line_pts.length = 0;

      // cross的背景
      var d_str = "";
      for (var i = 0; i < arr_rc_draw.length; i++) {
        var dw1 = arr_rc_draw[i];
        var sd = dw1.exit_side;
        var pt = sd.walk2;
        states.cross_line_pts.push(pt);
        if (i == 0) d_str = "M" + pt.x + "," + pt.y + " ";
        else d_str += "L" + pt.x + "," + pt.y + " ";
        sd = dw1.enter_side;
        pt = sd.walk2;
        states.cross_line_pts.push(pt);
        d_str += "L" + pt.x + "," + pt.y + " ";

        var dw2;
        if (i == arr_rc_draw.length - 1) dw2 = arr_rc_draw[0];
        else dw2 = arr_rc_draw[i + 1];

        sd = dw2.exit_side;
        pt = sd.walk2;
        var insect_pt = intersect_line_point(
          dw1.enter_side.walk2,
          dw1.enter_side.walk1,
          dw2.exit_side.walk2,
          dw2.exit_side.walk1
        ); // 相邻两直线的交点
        var mid_pt = {
          x: (dw1.enter_side.walk2.x + dw2.exit_side.walk2.x) * 0.5,
          y: (dw1.enter_side.walk2.y + dw2.exit_side.walk2.y) * 0.5,
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
      cross.setAttribute("deleteTag", "1");
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
        road.setAttribute("deleteTag", "1");
        road.setAttribute("id", i.toString());
        road.setAttribute("fill", "rgba(162,162,162, 1)");
        road.addEventListener("click", handleMainRoad, false);

        states.cvs?.appendChild(road);

        // 轮廓（白线）
        d_str = "";
        //入口边缘线
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

        //出口边缘线
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
        side.setAttribute("deleteTag", "1");
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
        //线宽
        var len =
          (["双黄线", "黄斜线", "绿化带"].indexOf(rc.median_strip.type) > -1
            ? 1
            : 0) *
          rc.median_strip.width *
          dw.ratio;
        var d = rc.cross_len_new * dw.ratio;
        var dr = Math.PI * 0.5;
        pt = cal_point(dw, d, dr, len);
        var ewkb_pt = { x: pt.x, y: pt.y }; // 进口人行道横白条靠近隔离带的点：enter walk bar point
        d_str = "M" + pt.x + "," + pt.y + " ";
        d = (rc.cross_len_new + rc.enter.extend_len) * dw.ratio;

        pt = cal_point(dw, d, dr, len);
        d_str += "L" + pt.x + "," + pt.y + " ";
        drawPoint(pt.x, pt.y, "blue");
        d =
          (rc.cross_len_new + rc.enter.extend_len + rc.enter.in_curv) *
          dw.ratio;
        var offset = len + road_info.canalize_info[i].enter.offset * dw.ratio;
        pt = cal_point(dw, d, dr, offset);
        d_str += "L" + pt.x + "," + pt.y + " ";
        drawPoint(pt.x, pt.y, "red");
        d = rc.length * dw.ratio;
        pt = cal_point(dw, d, dr, offset);
        d_str += "L" + pt.x + "," + pt.y + " ";

        d = rc.cross_len_new * dw.ratio;
        pt = cal_point(dw, d, -dr, len);
        d_str += "M" + pt.x + "," + pt.y + " ";
        d = rc.length * dw.ratio;
        pt = cal_point(dw, d, -dr, len);
        d_str += "L" + pt.x + "," + pt.y + " ";

        //如果没有入口或出口车道，不绘制隔离带
        if (rc.enter.num !== 0 && rc.exit.num !== 0) {
          var sep = document.createElementNS(states.ns, "path"); // 隔离带
          sep.setAttribute("d", d_str);
          sep.setAttribute("stroke-width", "2");
          sep.setAttribute("fill", "none");
          setIsolationStyle(sep, rc.median_strip.type);
          sep.setAttribute("deleteTag", "1");
          states.cvs?.appendChild(sep);
        }
        // 进口人行道横白条
        pt = ewkb_pt;
        d_str = "M" + pt.x + "," + pt.y + " ";
        pt = dw.enter_side2.walk2;
        d_str += "L" + pt.x + "," + pt.y + " ";

        var bar = document.createElementNS(states.ns, "path");
        bar.setAttribute("d", d_str);
        bar.setAttribute("stroke", "rgb(255,255,255)");
        bar.setAttribute("stroke-width", "2");
        bar.setAttribute("deleteTag", "1");
        states.cvs?.appendChild(bar);

        // 进口车道白线
        for (var j = 1; j < rc.enter.num; j++) {
          // 靠近人行道的白线
          len = (rc.median_strip.width + rc.enter.lane_width * j) * dw.ratio;
          d = rc.cross_len_new * dw.ratio;
          dr = Math.PI * 0.5;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.stop_line_length + 10 + (rc.cross_len_new - rc.cross_len)) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          var line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          line.setAttribute("deleteTag", "1");
          states.cvs?.appendChild(line);

          // 展宽前
          d = (rc.cross_len_new + rc.enter.extend_len - 13) * dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d = (rc.cross_len_new + rc.enter.extend_len - 3) * dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          line.setAttribute("deleteTag", "1");
          states.cvs?.appendChild(line);

          // 展宽后
        }

        // 进口车道展宽后白线
        for (var j = 1; j <= rc.enter.num - rc.enter.extend_num - 1; j++) {
          // 展宽后
          len = (rc.median_strip.width + rc.enter.lane_width * j) * dw.ratio;
          d =
            (rc.cross_len_new + rc.enter.extend_len + rc.enter.in_curv + 3) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.cross_len_new + rc.enter.extend_len + rc.enter.in_curv + 13) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          line.setAttribute("deleteTag", "1");
          states.cvs?.appendChild(line);

          // 展宽后第2部分
          len = (rc.median_strip.width + rc.enter.lane_width * j) * dw.ratio;
          d =
            (rc.cross_len_new + rc.enter.extend_len + rc.enter.in_curv + 20) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.cross_len_new + rc.enter.extend_len + rc.enter.in_curv + 30) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          line.setAttribute("deleteTag", "1");
          states.cvs?.appendChild(line);
        }

        // 出口车道白线
        for (var j = 1; j < rc.exit.num; j++) {
          // 靠近人行道的白线 —— 展宽前
          var num = rc.exit.extend_len / 15;
          for (var n = 0; n < num; n++) {
            len = (rc.median_strip.width + rc.exit.lane_width * j) * dw.ratio;
            d = (rc.cross_len_new + 16 * n) * dw.ratio;
            dr = -Math.PI * 0.5;
            pt = cal_point(dw, d, dr, len);
            d_str = "M" + pt.x + "," + pt.y + " ";
            d = (rc.cross_len_new + 16 * n + 9) * dw.ratio;
            pt = cal_point(dw, d, dr, len);
            d_str += "L" + pt.x + "," + pt.y + " ";

            line = document.createElementNS(states.ns, "path");
            line.setAttribute("d", d_str);
            line.setAttribute("stroke", "rgb(255,255,255)");
            line.setAttribute("stroke-width", "2");
            line.setAttribute("deleteTag", "1");
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
            line.setAttribute("deleteTag", "1");
            states.cvs?.appendChild(line);
          }
        }

        // 出口车道展宽后白线
        for (var j = 1; j <= rc.exit.num - rc.exit.extend_num - 1; j++) {
          // 展宽后
          dr = -Math.PI * 0.5;
          len = (rc.median_strip.width + rc.exit.lane_width * j) * dw.ratio;
          d =
            (rc.cross_len_new + rc.exit.extend_len + rc.exit.in_curv + 3) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.cross_len_new + rc.exit.extend_len + rc.exit.in_curv + 13) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          line.setAttribute("deleteTag", "1");
          states.cvs?.appendChild(line);

          // 展宽后第2部分
          len = (rc.median_strip.width + rc.exit.lane_width * j) * dw.ratio;
          d =
            (rc.cross_len_new + rc.exit.extend_len + rc.exit.in_curv + 20) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str = "M" + pt.x + "," + pt.y + " ";
          d =
            (rc.cross_len_new + rc.exit.extend_len + rc.exit.in_curv + 30) *
            dw.ratio;
          pt = cal_point(dw, d, dr, len);
          d_str += "L" + pt.x + "," + pt.y + " ";

          line = document.createElementNS(states.ns, "path");
          line.setAttribute("d", d_str);
          line.setAttribute("stroke", "rgb(255,255,255)");
          line.setAttribute("stroke-width", "2");
          line.setAttribute("deleteTag", "1");
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
          bkdiv.setAttribute("stroke-width", "2");
          bkdiv.setAttribute("fill", "none");
          setIsolationStyle(bkdiv, rc.enter.bike_lane.div_type);
          bkdiv.setAttribute("deleteTag", "1");
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
          bkdiv.setAttribute("stroke-width", "2");
          bkdiv.setAttribute("fill", "none");
          setIsolationStyle(bkdiv, rc.exit.bike_lane.div_type);
          bkdiv.setAttribute("deleteTag", "1");
          states.cvs?.appendChild(bkdiv);
        }

        // 入口路标
        for (var j = 0; j < rc.enter.num; j++) {
          //增加偏移系数k，微调路标位置
          const k = j === 0 ? -0.2 : j < 3 ? j * 0.9 : j * 0.97;
          //增加偏移系数k2，调整路宽时调整路标位置
          const k2 = (rc.enter.lane_width - 3.5) * 0.8;
          len =
            (rc.median_strip.width + rc.enter.lane_width * k + k2) * dw.ratio;
          d = rc.cross_len_new * dw.ratio;
          dr = Math.PI * 0.5;
          pt = cal_point(dw, d, dr, len);
          create_road_sign(pt, i, j, dw.road_sign.enter[j].path);
        }

        // 出口路标
        for (var j = 1; j <= rc.exit.num; j++) {
          //增加偏移系数k，微调路标位置
          const k = j === 1 ? j * 1.1 : j * 1.05;
          //增加偏移系数k2，调整路宽时调整路标位置
          const k2 = -(rc.exit.lane_width - 3.5) * 0.8;
          len =
            (rc.median_strip.width + rc.exit.lane_width * k + k2) * dw.ratio;
          d = rc.cross_len_new * dw.ratio;
          dr = -Math.PI * 0.5;
          pt = cal_point(dw, d, dr, len);
          create_road_sign(pt, i, j, dw.road_sign.exit[j - 1].path, true);
        }
      }

      // cross的右转白线 和旋转的文字“方向n”
      for (var i = 0; i < arr_rc_draw.length; i++) {
        rc = road_info.canalize_info[i];
        var dw = arr_rc_draw[i];
        var sd = dw.enter_side2;
        var pt = sd.walk2;
        d_str = "M" + pt.x + "," + pt.y + " ";

        var dw2;
        if (i == arr_rc_draw.length - 1) dw2 = arr_rc_draw[0];
        else dw2 = arr_rc_draw[i + 1];
        sd = dw2.exit_side2;
        pt = sd.walk2;
        var insect_pt = intersect_line_point(
          dw.enter_side2.walk2,
          dw.enter_side2.walk1,
          dw2.exit_side2.walk2,
          dw2.exit_side2.walk1
        ); // 相邻两直线的交点
        var mid_pt = {
          x: (dw.enter_side2.walk2.x + dw2.exit_side2.walk2.x) * 0.5,
          y: (dw.enter_side2.walk2.y + dw2.exit_side2.walk2.y) * 0.5,
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
        cross.setAttribute("deleteTag", "1");

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
        txt.setAttribute("deleteTag", "1");
        states.cvs?.appendChild(txt);
      }
    }

    //清空所有svg信息，重新绘制
    function removeAll() {
      document.querySelectorAll("path").forEach((e: any) => {
        const is_delete = e.getAttribute("deleteTag") === "1";
        if (e.id !== "notDelete" && is_delete) {
          //清空所有路径(保留弹窗内路标和不需要删除的元素)
          e.remove();
        }
      });
      document.querySelectorAll("text").forEach((e) => {
        //清空所有文字
        e.remove();
      });
      document.querySelectorAll("g").forEach((e) => {
        //清空所有路标
        e.remove();
      });
    }

    /**
     * 绘制路标
     * @param way_pt 绘制坐标原点
     * @param index 整条道路index
     * @param road_sign 路标
     * @param is_reverse 是否反转(即是否出口车道)
     */
    function create_road_sign(
      way_pt: { x: number; y: number },
      index: number,
      way_index: number,
      road_sign: string,
      is_reverse = false
    ) {
      var path = document.createElementNS(states.ns, "path");
      path.setAttribute("id", `${index};${way_index}`);
      path.setAttribute("d", road_sign);
      path.setAttribute("fill", "#ffffff");
      path.setAttribute("width", "100");
      path.setAttribute("height", "100");
      path.setAttribute("deleteTag", "1");

      var svg = document.createElementNS(states.ns, "svg");
      svg.appendChild(path);
      svg.setAttribute("viewBox", "0 0 1024 1024");
      svg.setAttribute("height", "40");
      svg.setAttribute("width", "18");
      svg.setAttribute("x", way_pt.x.toString());
      svg.setAttribute("y", way_pt.y.toString());
      var g = document.createElementNS(states.ns, "g");
      g.setAttribute(
        "transform",
        `rotate(${270 - road_info.road_attr[index].angle} ${way_pt.x} ${
          way_pt.y
        })`
      );
      if (!is_reverse) {
        g.setAttribute("style", "cursor:pointer");
        g.addEventListener("click", handleChoose, false);
      }
      g.appendChild(svg);
      states.cvs?.appendChild(g);
    }

    //选中某条道路
    function handleMainRoad(e: any) {
      const event = window.event || e;
      states.cur_road_dir = Number(event.target.id);
      onLoadChange(road_info);
    }

    //选中路标
    function handleChoose(e: any) {
      const event = window.event || e;
      states.currentSign = event.target;
      states.modalVisible = true;
    }

    //设置路标
    function handleConfirmSign(rowKey: string) {
      const index = states.currentSign.id.split(";");
      const road_index = Number(index[0]);
      const way_index = Number(index[1]);
      const road_sign = roadSigns.find((s) => s.key === rowKey)?.path;

      const rc = road_info.canalize_info[road_index];

      //设置界面图标
      states.currentSign.setAttribute("d", road_sign);
      states.currentSign.setAttribute("deleteTag", "1");
      //设置数据对应图标
      rc.road_sign.enter[way_index] = { key: rowKey, path: road_sign };
      // 设置路标后需要调整当前道路对应的数量
      setRoadDirection();

      states.modalVisible = false;
    }

    function setRoadDirection() {
      road_info.canalize_info.forEach((rc: any) => {
        //掉头车道
        const uturn = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.uturn
        ).length;
        //左转加掉头
        const left_uturn = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.left_uturn
        ).length;
        //直行加掉头
        const straight_uturn = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.straight_uturn
        ).length;
        //左转
        const left = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.left
        ).length;
        //左转加右转
        const left_right = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.left_right
        ).length;
        //左转加直行加右转
        const all_direction = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.all_direction
        ).length;
        //直行
        const straight = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.straight
        ).length;
        //直行加左转
        const straight_left = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.straight_left
        ).length;
        //直行加右转
        const straight_right = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.straight_right
        ).length;
        //右转
        const right = rc.road_sign.enter.filter(
          (rs: any) => rs.key === roadSignKey.right
        ).length;

        rc.road_direction = {
          uturn: uturn + 0.5 * left_uturn + 0.5 * straight_uturn,
          left:
            left +
            0.5 * left_uturn +
            0.5 * straight_left +
            0.5 * left_right +
            0.33 * all_direction,
          straight:
            straight +
            0.5 * straight_uturn +
            0.5 * straight_left +
            0.5 * straight_right +
            0.33 * all_direction,
          right:
            right +
            0.5 * straight_right +
            0.5 * left_right +
            0.33 * all_direction,
        };
      });
    }

    /**
     * 属性操作
     */
    // 方向选择
    function on_sel_dirs_change() {
      onLoadChange(road_info);
    }

    //设置机动车道
    function on_bike_lane_set(is_set: number) {
      road_info.canalize_info[states.cur_road_dir].enter.bike_lane.has = is_set;
      road_info.canalize_info[states.cur_road_dir].exit.bike_lane.has = is_set;
      onLoadChange(road_info);
    }

    //页面属性变化
    function on_prop_change(filed = "") {
      //黄斜线和绿化带可设宽度，其余默认
      if (
        ["黄斜线", "绿化带"].indexOf(
          road_info.canalize_info[states.cur_road_dir].median_strip.type
        ) === -1
      ) {
        road_info.canalize_info[states.cur_road_dir].median_strip.width = 0.5;
      }
      //入口道路数量为0时，展宽也为0
      if (!road_info.canalize_info[states.cur_road_dir].enter.num) {
        road_info.canalize_info[states.cur_road_dir].enter.extend_num = 0;
      }
      //出口道路数量为0时，展宽也为0
      if (!road_info.canalize_info[states.cur_road_dir].exit.num) {
        road_info.canalize_info[states.cur_road_dir].exit.extend_num = 0;
      }
      //路口数量变更时重绘路标
      if (filed === "enter_num" || filed === "exit_num") {
        create_sign(road_info.canalize_info[states.cur_road_dir]);
        setRoadDirection();
      }
      onLoadChange(road_info);
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
      const rf =
        plans.canalize_plans[roadStates.current_canalize].flow_plans[0]
          .signal_plans[0].road_info;
      //已有数据，渲染
      if (rf.canalize_info.length > 0) {
        onLoadEdit(rf);
      } else {
        //否则全量初始化
        onLoad(rf);
      }
    });

    function drawPoint(x: number, y: number, color: string) {
      var g = document.createElementNS(states.ns, "g");
      g.setAttribute("stroke", color);
      g.setAttribute("stroke-width", "2");
      g.setAttribute("fill", "black");
      var circle = document.createElementNS(states.ns, "circle");
      circle.setAttribute("cx", x.toString());
      circle.setAttribute("cy", y.toString());
      circle.setAttribute("r", "2");
      g.appendChild(circle);
      states.cvs?.appendChild(g);
    }

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
      onLoad,
      onLoadEdit,
      onLoadChange,
    };
  },
});
</script>
<style scoped lang="less">
@import url("./index.less");
</style>
