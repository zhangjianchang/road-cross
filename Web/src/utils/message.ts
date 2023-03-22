import { notification } from "ant-design-vue";

export const openNotfication = (
  type: string,
  description: string,
  message = "错误提醒"
) => {
  notification[type === "success" ? "success" : "warning"]({
    message,
    description,
    duration: 10,
  });
};
