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
        <a-breadcrumb-item>项目列表</a-breadcrumb-item>
      </a-breadcrumb>
    </div>
    <a-table
      :dataSource="list"
      :columns="designColumns"
      :scroll="{ x: '100%' }"
      bordered
      :loading="loading"
    >
      <template #operation="{ record }">
        <a-button type="primary" size="small" @click="handleEdit(record.guid)">
          编辑
        </a-button>
        <a-popconfirm
          title="确认删除?"
          ok-text="是"
          cancel-text="否"
          @confirm="handleDelete(record.guid)"
        >
          <a-button type="danger" size="small" class="ml-5"> 删除 </a-button>
        </a-popconfirm>
      </template>
      <template #title>
        <a-button type="primary" @click="handleCreate">
          创建新交叉路口
        </a-button>
      </template>
    </a-table>
  </Container>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { defineComponent, onMounted, reactive, toRefs } from "vue";
import { designColumns } from ".";
import Container from "../../../components/Container/index.vue";
import { deleteDesign, getDesignList } from "../../../request/api";
import { PageEnum } from "../../../router/data";
import { goRouterByParam } from "../../../utils/common";

export default defineComponent({
  components: { Container },
  setup() {
    //判断权限
    var token = localStorage.getItem("token");
    if (!token) {
      message.warning("请先登录");
      goRouterByParam(PageEnum.Login);
    }

    const states = reactive({
      list: [] as any[],
      loading: false,
    });

    const init = () => {
      states.loading = true;
      getDesignList()
        .then((res: any) => {
          states.list.length = 0;
          Object.assign(states.list, res.data);
        })
        .finally(() => {
          states.loading = false;
        });
    };

    const handleCreate = () => {
      goRouterByParam(PageEnum.Design);
    };

    const handleEdit = (guid: string) => {
      goRouterByParam(PageEnum.DesignEdit, { guid });
    };

    const handleDelete = (guid: string) => {
      deleteDesign({ guid }).then(() => {
        message.success("删除成功");
        init();
      });
    };

    onMounted(() => {
      init();
    });
    return {
      ...toRefs(states),
      designColumns,
      handleCreate,
      handleEdit,
      handleDelete,
      goRouterByParam,
      PageEnum,
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
@import "./index.less";
</style>
