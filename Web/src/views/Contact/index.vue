<template>
  <div class="content">
    <!-- <h3>官方QQ群</h3>
    <div class="label">官方交流QQ群：1111111</div>
    <h3>微信</h3>
    <div class="label">二维码</div> -->
    <h3>邮箱</h3>
    <div class="label">
      对应用有任何意见或者建议欢迎发送至邮箱：zhouyuping@ai-cambrian.com
    </div>
    <h3>意见与建议</h3>
    <div class="label">
      <a-textarea
        :rows="6"
        show-count
        allow-clear
        placeholder="也可在此填写您在系统使用中遇到的问题或对系统的建议"
        :maxlength="500"
        style="width: 400px"
        v-model:value="param.suggestion"
      />
    </div>
    <div class="button">
      <a-button type="primary" @click="onSubmit">提交</a-button>
    </div>
    <div class="a-text">
      <a @click="initSuggestionList"> 查看全部用户反馈 </a>
    </div>
    <a-modal
      :visible="states.modalVisible"
      width="1000px"
      title="用户反馈"
      :footer="null"
      @cancel="states.modalVisible = false"
    >
      <div class="modal-container">
        <a-list
          item-layout="vertical"
          size="large"
          :pagination="pagination"
          :data-source="states.list"
        >
          <template #renderItem="{ item }">
            <a-list-item key="item.title">
              <a-list-item-meta :description="item.answer">
                <template #title>
                  <a :href="item.href">
                    <!-- {{ item.userName }} 反馈： -->
                    {{ item.suggestion }}
                  </a>
                </template>
                <template #avatar>
                  <a-avatar :src="item.avatar" />
                </template>
              </a-list-item-meta>
              {{ item.content }}
            </a-list-item>
          </template>
        </a-list>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { reactive } from "vue";
import { getSuggestionList, suggestion } from "../../request/api";
import { basic_config } from "../../request/http";
import { openNotification } from "../../utils/message";

const imgUrl = `${basic_config.img_url}/avatar`;
const states = reactive({
  list: [],
  modalVisible: false,
});

const param = reactive({
  suggestion: "",
});
const onSubmit = () => {
  if (!param.suggestion) {
    openNotification("warning", "请填写内容后提交", "温馨提示");
    return;
  }
  suggestion(param).then(() => {
    openNotification(
      "success",
      "管理员回复后会进行展示，请耐心等待",
      "提交成功",
      15
    );
  });
};

const pagination = {
  onChange: (page: number) => {
    console.log(page);
  },
  pageSize: 8,
};

const initSuggestionList = () => {
  getSuggestionList({ status: "200" }).then((res: any) => {
    res.data.map((d: any) => {
      d.avatar = `${imgUrl}/${d.avatar}`;
    });
    states.list = res.data;
    states.modalVisible = true;
  });
};
</script>
<style lang="less" scoped>
@import url("./index.less");
</style>
