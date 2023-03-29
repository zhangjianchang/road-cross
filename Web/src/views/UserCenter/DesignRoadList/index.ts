export const designColumns = [
  {
    title: "交叉口名称",
    dataIndex: "roadName",
    width: 800,
  },
  {
    title: "创建时间",
    dataIndex: "createDate",
    with: 15,
  },
  {
    title: "修改时间",
    dataIndex: "updateDate",
    with: 15,
  },
  {
    title: "操作",
    dataIndex: "operation",
    slots: { customRender: "operation" },
    with: 10,
  },
];
