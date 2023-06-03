import { notification } from "ant-design-vue";
import { duration } from "moment";

export const openNotification = (
  type: string,
  description: string,
  message = "错误提醒",
  duration = 10
) => {
  notification[
    type === "warning"
      ? "warning"
      : type === "error"
      ? "error"
      : type === "success"
      ? "success"
      : "info"
  ]({
    message,
    description,
    duration,
  });
};
