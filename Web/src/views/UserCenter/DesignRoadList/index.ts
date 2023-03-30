export const designColumns = [
  {
    title: "交叉口名称",
    dataIndex: "roadName",
    // width: 800,
  },
  {
    title: "创建时间",
    dataIndex: "createDate",
    width: 180,
  },
  {
    title: "修改时间",
    dataIndex: "updateDate",
    width: 180,
  },
  {
    title: "操作",
    dataIndex: "operation",
    slots: { customRender: "operation" },
    width: 150,
  },
];
