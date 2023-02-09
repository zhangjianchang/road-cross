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
import { getQByPathCurv } from "../../../utils/common";
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

    const initRoads = () => {
      states.ns = "http://www.w3.org/2000/svg";
      states.cvs = document.getElementById("canvas");
      road_info.canalize_info.entranceAttr.length = 0;
      road_info.canalize_info.exitAttr.length = 0;
      for (let index = 0; index < road_info.road_attr.length; index++) {
        //初始化6车道//0.5为中间双黄线
        road_info.canalize_info.entranceAttr.push({
          wayCount: states.defaultCount,
        });
        road_info.canalize_info.exitAttr.push({
          wayCount: states.defaultCount,
        });
        states.wayCount[index] =
          road_info.canalize_info.entranceAttr[index].wayCount +
          road_info.canalize_info.exitAttr[index].wayCount +
          0.5;
      }
      render();
    };

    // //画路
    // function createRoad(angle: number) {
    //   const g = document.createElementNS(states.ns, "g");
    //   g.setAttribute(
    //     "transform",
    //     `rotate(-${angle} ${states.cx} ${states.cy})`
    //   );
    //   const polygon = document.createElementNS(states.ns, "polygon");
    //   polygon.setAttribute("style", "fill: #989898");
    //   polygon.setAttribute("points", "350,300 650,300 650,400 350,400");
    //   g.appendChild(polygon);
    //   states.cvs?.appendChild(g);
    // }

    function render() {
      for (var i = 0; i < road_info.road_attr.length; i++) {
        //
        var road_r = states.road_r_k * states.wayCount[i];
        var cross_r = states.road_r_k * states.wayCount[i] - 10;
        var zebra_r = states.road_r_k * states.wayCount[i] - 15;
        var angle = road_info.road_attr[i].angle;
        var radian = (Math.PI / 180) * angle; // 角度转弧度
        // 基线起（圆心）止（圆上）点
        var x2 = Math.cos(radian) * 300 + 350; // 大圆半径300
        var y2 = -Math.sin(radian) * 300 + 350;
        var x3 = Math.cos(radian) * road_r + 350; // 交叉口圆半径100
        var y3 = -Math.sin(radian) * road_r + 350;
        var x4 = Math.cos(radian) * zebra_r + 350; // 斑马线圆半径50
        var y4 = -Math.sin(radian) * zebra_r + 350;
        //画路
        drawRoad(i, angle, x2, y2);
        // 获取交叉口圆plus和路边相交的点
        setPts(states.cross_pts, angle, x3, y3, road_r);
        // 获取交叉口圆plus和路边相交的点(内侧边缘线)
        setPts(states.cross_line_pts, angle, x3, y3, cross_r);
        //获取路标一圈的定位
        setPts(states.road_zebra_pts, angle, x4, y4, zebra_r);
      }
      // 交叉口
      drawCross();
    }

    //direction：方向，nr 近右,nl 近左, fr 远右, fl 远左 //road_width默认100（小于100即画路边线时用到）
    function getPoint(
      direction: string,
      angle: number,
      x: number,
      y: number,
      road_width: number
    ) {
      // 路宽
      var radian = (Math.PI / 180) * (angle - 90);
      var coefficient = direction === "nl" || direction === "fr" ? -1 : 1;
      var point_x = coefficient * road_width * 0.5 * Math.cos(radian) + x;
      var point_y = -coefficient * road_width * 0.5 * Math.sin(radian) + y;
      return [point_x, point_y];
    }

    function setPts(
      pts: any[],
      angle: number,
      x: number,
      y: number,
      road_width: number
    ) {
      var point = getPoint("nr", angle, x, y, road_width);
      pts.push(point);
      point = getPoint("nl", angle, x, y, road_width);
      pts.push(point);
    }

    //画路
    function drawRoad(index: number, angle: number, x: number, y: number) {
      var road_width = states.road_r_k * states.wayCount[index];
      var cross_line_width = states.road_r_k * states.wayCount[index] - 10;
      var g = document.createElementNS(states.ns, "g");
      g.setAttribute("id", `g${index + 1}`);
      //画主路
      drawMainRoad(g, angle, x, y, index, road_width);
      //路边缘线
      drawRoadLine(g, angle, x, y, cross_line_width);
      //双黄线
      drawRoadLine(g, angle, x, y, 5, "#FFA500");
      //单侧车道分界线
      let all_count = (states.wayCount[index] - 0.5) / 2;
      for (let i = 1; i < all_count; i++) {
        let right_d = "150 30 20 30 20 30";
        let left_d = "20 30";
        drawRoadLine(g, angle, x, y, 30 * i, "#FFFFFF", right_d, left_d);
      }
      states.cvs?.appendChild(g);
    }

    //画主路
    function drawMainRoad(
      g: any,
      angle: number,
      x: number,
      y: number,
      index: number,
      road_width: number
    ) {
      //路
      var road = document.createElementNS(states.ns, "path"); // 创建SVG元素——路
      var d_str = "";
      // 圆心右侧点
      var point = getPoint("nr", angle, states.cx, states.cy, road_width);
      d_str += `M ${point[0]}, ${point[1]}, `;
      // 圆心左侧点
      point = getPoint("nl", angle, states.cx, states.cy, road_width);
      d_str += `L ${point[0]}, ${point[1]}, `;
      // 远端右侧点
      point = getPoint("fr", angle, x, y, road_width);
      d_str += `L ${point[0]}, ${point[1]}, `;
      // 远端左侧点
      point = getPoint("fl", angle, x, y, road_width);
      d_str += `L ${point[0]}, ${point[1]}, Z`;
      road.setAttribute("d", d_str);
      road.setAttribute("id", `road_${index + 1}`);
      road.setAttribute("fill", `rgb(162,162,162)`);
      road.addEventListener("click", onRoad, false);
      g.appendChild(road);
    }

    //画路面上的线
    function drawRoadLine(
      g: any,
      angle: number,
      x: number,
      y: number,
      road_width: number,
      color = "#FFFFFF", //路线颜色
      dasharray_right = "0", //路线虚实，0实线，x间隔x的虚线
      dasharray_left = "0" //路线虚实，0实线，x间隔x的虚线
    ) {
      var rightLine = document.createElementNS(states.ns, "line");
      //右侧道路
      const left1 = getPoint("nl", angle, states.cx, states.cy, road_width);
      const left2 = getPoint("fr", angle, x, y, road_width);
      rightLine.setAttribute("x1", left1[0].toString());
      rightLine.setAttribute("y1", left1[1].toString());
      rightLine.setAttribute("x2", left2[0].toString());
      rightLine.setAttribute("y2", left2[1].toString());
      rightLine.setAttribute("stroke", color);
      rightLine.setAttribute("stroke-width", "2");
      rightLine.setAttribute("stroke-dasharray", dasharray_right);
      g.appendChild(rightLine);
      //左侧道路
      var leftLine = document.createElementNS(states.ns, "line");
      const right1 = getPoint("nr", angle, states.cx, states.cy, road_width);
      const right2 = getPoint("fl", angle, x, y, road_width);
      leftLine.setAttribute("x1", right1[0].toString());
      leftLine.setAttribute("y1", right1[1].toString());
      leftLine.setAttribute("x2", right2[0].toString());
      leftLine.setAttribute("y2", right2[1].toString());
      leftLine.setAttribute("stroke", color);
      leftLine.setAttribute("stroke-width", "2");
      leftLine.setAttribute("stroke-dasharray", dasharray_left);
      g.appendChild(leftLine);
    }

    function drawCross() {
      drawCrossRoad();
      drawCrossLine();
      //右侧道路停止线
      drawStopLine();
      //路标
      drawRoadSign();
      //斑马线
      drawZebraCross();
    }

    //画交叉口
    function drawCrossRoad() {
      //先清空交叉路口
      document.querySelectorAll("path").forEach((e) => {
        if (e.id === "cross") e.remove();
      });
      var cross = document.createElementNS(states.ns, "path"); // 创建SVG元素——交叉口
      var d_str = "";
      for (var i = 0; i < states.cross_pts.length; i++) {
        var pt = states.cross_pts[i];
        if (i == 0) {
          //第一个点
          d_str += `M ${pt[0]},${pt[1]} `;
        } else if (i % 2 !== 0) {
          //第奇数个点为同一道路的点，用直线连接
          d_str += `L ${pt[0]},${pt[1]} `;
          //最后一个点需要用曲线连接第一个点
          if (i === states.cross_pts.length - 1) {
            var firstpt = states.cross_pts[0];
            var Q = getQByPathCurv(
              firstpt,
              pt,
              road_info.canalize_info.curvature
            );
            d_str += `Q ${Q} ${firstpt[0]}, ${firstpt[1]} `;
          }
        } else {
          //第偶数个点为相邻道路的点，用曲线连接
          var prevPt = states.cross_pts[i - 1];
          var Q = getQByPathCurv(prevPt, pt, road_info.canalize_info.curvature);
          d_str += `Q ${Q} ${pt[0]}, ${pt[1]} `;
        }
      }
      cross.setAttribute("d", d_str);
      cross.setAttribute("id", "cross");
      // cross.setAttribute("fill", "rgba(162,162,0,0.5)");
      cross.setAttribute("fill", "rgb(162,162,162)");
      states.cvs?.appendChild(cross);
    }

    //路口弧线
    function drawCrossLine() {
      //先清空交叉路口
      document.querySelectorAll("path").forEach((e) => {
        if (e.id.indexOf("cross_line") > -1) e.remove();
      });
      var corss_d_str = "";
      for (var i = 0; i < states.cross_line_pts.length; i++) {
        var pt = states.cross_line_pts[i];
        if (i == 0) {
          //第一个点跳过
        } else if (i % 2 !== 0) {
          corss_d_str = `M ${pt[0]},${pt[1]} `;
          if (i === states.cross_line_pts.length - 1) {
            //最后一个点需要用曲线连接第一个点
            var firstpt = states.cross_line_pts[0];
            var Q = getQByPathCurv(
              firstpt,
              pt,
              road_info.canalize_info.curvature
            );
            corss_d_str += `Q ${Q} ${firstpt[0]}, ${firstpt[1]} `;
          }
        } else {
          //交叉弧线
          var prevPt = states.cross_line_pts[i - 1];
          var Q = getQByPathCurv(prevPt, pt, road_info.canalize_info.curvature);
          corss_d_str = `M ${prevPt[0]},${prevPt[1]} Q ${Q} ${pt[0]},${pt[1]} `;
        }
        //交叉路口弧线
        if (corss_d_str) {
          var cross_line = document.createElementNS(states.ns, "path"); // 创建SVG元素——交叉口
          cross_line.setAttribute("d", corss_d_str);
          cross_line.setAttribute("id", `cross_line_${i}`);
          cross_line.setAttribute("stroke", "#ffffff");
          cross_line.setAttribute("stroke-width", "2");
          cross_line.setAttribute("fill", "none");
          states.cvs?.appendChild(cross_line);
        }
      }
    }

    //画右侧停止线
    function drawStopLine() {
      //先清空交叉路口
      document.querySelectorAll("path").forEach((e) => {
        if (e.id.indexOf("stop_line") > -1) e.remove();
      });
      for (var i = 0; i < states.cross_line_pts.length; i++) {
        var pt = states.cross_line_pts[i];
        if (i > 0 && i % 2 !== 0) {
          var prevPt = states.cross_line_pts[i - 1];
          //取中点
          var middlePt = [(prevPt[0] + pt[0]) / 2, (prevPt[1] + pt[1]) / 2];
          var stopLine = document.createElementNS(states.ns, "line");
          stopLine.setAttribute("id", `stop_line_${i}`);
          stopLine.setAttribute("x1", pt[0].toString());
          stopLine.setAttribute("y1", pt[1].toString());
          stopLine.setAttribute("x2", middlePt[0].toString());
          stopLine.setAttribute("y2", middlePt[1].toString());
          stopLine.setAttribute("stroke", "#FFFFFF");
          stopLine.setAttribute("stroke-width", "2");
          states.cvs?.appendChild(stopLine);
        }
      }
    }

    //画路标
    function drawRoadSign() {
      let road = 0;
      var way_count = 0; //每条路单行三条道
      for (var i = 0; i < states.cross_line_pts.length; i++) {
        let all_count = states.wayCount[road] - 0.5; //每条路全部数量六条道
        way_count = road_info.canalize_info.exitAttr[road].wayCount; //出口车道数量
        var pt = states.cross_line_pts[i];
        if (i > 0 && i % 2 !== 0) {
          var prevPt = states.cross_line_pts[i - 1];
          //几条道路（默认双向六条）
          for (let way_idx = 0; way_idx < all_count; way_idx++) {
            var is_reverse = way_idx < way_count;
            var right_idx = way_idx - way_count;
            var is_last = all_count === way_idx + 1;
            //左侧道路、右侧道路离中心距离微调
            var k = way_idx >= way_count ? way_idx : way_idx * 0.9;
            //(x1+k(x2-x1)/n,y1+k(y2-y1)/n)线段n等分公式
            var wayPt = [
              prevPt[0] + (k * (pt[0] - prevPt[0])) / all_count,
              prevPt[1] + (k * (pt[1] - prevPt[1])) / all_count,
            ];
            var path = document.createElementNS(states.ns, "path");
            path.setAttribute("id", `road_sign_${i}`);
            path.setAttribute(
              "d",
              getRoadDefaultSign(right_idx, is_reverse, is_last)
            );
            path.setAttribute("fill", "#ffffff");
            path.setAttribute("width", "100");
            path.setAttribute("height", "100");

            var svg = document.createElementNS(states.ns, "svg");
            svg.appendChild(path);
            svg.setAttribute("viewBox", "0 0 1024 1024");
            svg.setAttribute("height", "40");
            svg.setAttribute("width", "18");
            svg.setAttribute("x", wayPt[0].toString());
            svg.setAttribute("y", wayPt[1].toString());
            var g = document.createElementNS(states.ns, "g");
            g.setAttribute(
              "transform",
              `rotate(${270 - road_info.road_attr[road].angle} ${wayPt[0]} ${
                wayPt[1]
              })`
            );
            g.setAttribute("style", "cursor:pointer");
            if (way_idx >= way_count) {
              g.addEventListener("click", handleChoose, false);
            }
            g.appendChild(svg);
            states.cvs?.appendChild(g);
          }
          road++;
        }
      }
    }

    //斑马线
    function drawZebraCross() {
      for (var i = 0; i < states.road_zebra_pts.length; i++) {
        var pt = states.road_zebra_pts[i];
        var nextPt = states.road_zebra_pts[i + 1];
        if (i == 0 || i % 2 === 0) {
          var stopLine = document.createElementNS(states.ns, "line");
          stopLine.setAttribute("id", `stop_line_${i}`);
          stopLine.setAttribute("x1", pt[0].toString());
          stopLine.setAttribute("y1", pt[1].toString());
          stopLine.setAttribute("x2", nextPt[0].toString());
          stopLine.setAttribute("y2", nextPt[1].toString());
          stopLine.setAttribute("stroke", "#FFFFFF");
          stopLine.setAttribute("stroke-width", "15");
          stopLine.setAttribute("stroke-dasharray", "3 5");
          states.cvs?.appendChild(stopLine);
        }
      }
    }

    //选中道路
    function onRoad(e: any) {
      states.currentRoad = e.path[0];
      road_info.canalize_info.direction = e.path[0].id;
      onDirectionChange();
    }

    //更改方向
    function onDirectionChange() {
      road_info.canalize_info.direction_index = getDirectionIndex(
        road_info.canalize_info.direction
      );
    }

    //选中路标
    function handleChoose(e: any) {
      states.currentSign = e.path[0];
      states.modalVisible = true;
    }

    function handleConfirmSign(rowKey: string) {
      states.currentSign.setAttribute(
        "d",
        roadSigns.find((s) => s.key === rowKey)?.path
      );
      states.modalVisible = false;
    }

    function onChangeRoadCount() {
      initRoads();
    }

    onMounted(() => {
      initRoads();
    });

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
