<template>
  <Container class="main-container">
    <div class="breadcrumb">
      <a-breadcrumb>
        <a-breadcrumb-item
          href=""
          @click="goRouterByParam(PageEnum.UserCenter)"
        >
          个人中心
        </a-breadcrumb-item>
        <a-breadcrumb-item>用户反馈</a-breadcrumb-item>
      </a-breadcrumb>
    </div>
    <div class="form-data">
      <a-table
        :dataSource="list"
        :columns="columns"
        :scroll="{ x: '100%' }"
        bordered
        :loading="loading"
      >
        <template #operation="{ record }">
          <a-button type="primary" size="small" @click="openEditModal(record)">
            查看
          </a-button>
        </template>
      </a-table>
    </div>
    <a-modal
      :visible="modalVisible"
      width="640px"
      title="回答用户反馈"
      @cancel="modalVisible = false"
      @ok="handleAnswer"
      okText="确定"
      cancelText="取消"
    >
      <a-form
        :model="data"
        :label-col="{ span: 3 }"
        :wrapper-col="{ span: 20 }"
        labelAlign="left"
      >
        <a-form-item label="用户反馈" name="type">
          {{ data.suggestion }}
        </a-form-item>
        <a-form-item label="是否展示" name="type">
          <a-radio-group v-model:value="data.status" :options="statusOptions" />
        </a-form-item>
        <a-form-item label="您的回复" name="memberName">
          <a-textarea
            placeholder="请输入对于该反馈的回答"
            v-model:value="data.answer"
            :rows="6"
            show-count
            allow-clear
            :maxlength="500"
          />
        </a-form-item>
      </a-form>
    </a-modal>
  </Container>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, onMounted, reactive, ref, toRefs } from "vue";
import { settingStates } from "..";
import { answer, getSuggestionList } from "../../../../request/api";
import { goRouterByParam } from "../../../../utils/common";
import Container from "../../../../components/Container/index.vue";
import { PageEnum } from "../../../../router/data";

export default defineComponent({
  components: { Container },
  setup() {
    const formRef = ref();
    const states = reactive({
      loading: false,
      modalVisible: false,
      statusOptions: [
        { label: "不展示", value: "100" },
        { label: "展示", value: "200" },
      ],
      columns: [
        {
          title: "用户建议与反馈",
          dataIndex: "suggestion",
          ellipsis: true,
          // width: 800,
        },
        {
          title: "回复",
          dataIndex: "answer",
          ellipsis: true,
          // width: 800,
        },
        {
          title: "用户账号",
          dataIndex: "userName",
          width: 120,
        },
        {
          title: "创建时间",
          dataIndex: "createDate",
          width: 110,
        },
        {
          title: "回答时间",
          dataIndex: "answerDate",
          width: 110,
        },
        {
          title: "操作",
          dataIndex: "operation",
          slots: { customRender: "operation" },
          width: 80,
        },
      ],
      list: [] as any[],
      data: {
        suggestion: "",
        answer: "",
        status: "",
      },
    });

    const initSuggestionList = () => {
      getSuggestionList(states.data).then((res: any) => {
        states.list = res.data;
      });
    };

    const openEditModal = (record: any) => {
      states.data = record;
      states.modalVisible = true;
    };

    const handleAnswer = () => {
      answer(states.data).then(() => {
        message.success("操作成功");
        states.modalVisible = false;
      });
    };

    onMounted(() => {
      initSuggestionList();
    });

    return {
      ...toRefs(states),
      ...toRefs(settingStates),
      formRef,
      openEditModal,
      handleAnswer,
      goRouterByParam,
      PageEnum,
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
@import "../../index.less";
</style>
