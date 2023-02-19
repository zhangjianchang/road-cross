<template>
  <Container class="main-container">
    <a-table
      :dataSource="list"
      :columns="designColumns"
      :scroll="{ x: '100%' }"
    >
      <template #name="{ record }">
        {{ record.basic_info.name }}
      </template>
      <template #createDate="{ record }">
        {{ record.basic_info.createDate }}
      </template>
      <template #updateDate="{ record }">
        {{ record.basic_info.updateDate }}
      </template>
      <template #operation="{ record }">
        <a-button
          type="primary"
          size="small"
          @click="handleEdit(record.basic_info.id)"
        >
          编辑
        </a-button>
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
import { defineComponent, onMounted, reactive, toRefs } from "vue";
import { designColumns } from ".";
import Container from "../../components/Container/index.vue";
import { PageEnum } from "../../router/data";
import { goRouterByParam } from "../../utils/common";

export default defineComponent({
  components: { Container },
  setup() {
    const states = reactive({
      list: [] as any[],
    });

    const initMyDesignRoadList = () => {
      const road_list_str = localStorage.getItem("road_list");
      if (road_list_str) {
        const road_list = JSON.parse(road_list_str);
        if (road_list) {
          Object.assign(states.list, road_list);
        }
      }
    };

    const handleCreate = () => {
      goRouterByParam(PageEnum.Design);
    };

    const handleEdit = (id: string) => {
      goRouterByParam(PageEnum.DesignEdit, { id });
    };

    onMounted(() => {
      initMyDesignRoadList();
    });
    return {
      ...toRefs(states),
      designColumns,
      handleCreate,
      handleEdit,
    };
  },
});
</script>
<style scoped lang="less">
@import "./index.less";
</style>
