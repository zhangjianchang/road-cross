import { reactive } from "vue";

export const userStates = reactive({
  user_info: undefined as any, //用户信息
  code_info: undefined as any, //授权信息
  can_edit: false, //是否可编辑
});
