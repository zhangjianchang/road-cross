export const accountColumns = [
  {
    title: "序号",
    dataIndex: "index",
    slots: { customRender: "index" },
    width: 60,
  },
  {
    title: "姓名",
    dataIndex: "accountName",
    slots: { customRender: "accountName" },
    width: 150,
  },
  {
    title: "账号",
    dataIndex: "memberName",
    width: 100,
  },
  {
    title: "状态",
    customRender: () => {
      return "已加入";
    },
    width: 100,
  },
  {
    title: "添加时间",
    dataIndex: "activeDate",
    width: 130,
  },
  {
    title: "授权次数",
    dataIndex: "usageTimes",
    slots: { customRender: "usageTimes" },
    width: 120,
  },
  {
    title: "已用次数",
    dataIndex: "usedTimes",
    width: 100,
  },
  {
    title: "剩余次数",
    dataIndex: "remainingTimes",
    width: 100,
  },
  {
    title: "操作",
    dataIndex: "operation",
    slots: { customRender: "operation" },
    width: 100,
  },
];

export interface Account {
  accountName?: string;
  memberName?: string;
  activeDate?: string;
  usageTimes?: number;
  usedTimes?: number;
  remainingTimes?: number;
  validDate?: number;
  expireDate?: string;
}
