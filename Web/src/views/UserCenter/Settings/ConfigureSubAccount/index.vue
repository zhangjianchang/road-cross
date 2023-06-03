<template>
  <div class="form-data">
    <a-table
      bordered
      size="small"
      :columns="accountColumns"
      :dataSource="accountList"
      :scroll="scrollX(accountColumns)"
      :loading="loading"
      :pagination="false"
    >
      <template #title>
        <div style="display: flex">
          <div class="tips-area">
            <a-alert type="info">
              <template #message>
                授权到期时间：{{ adminExpireDate }}， 授权可用次数：{{
                  frequency
                }}， 剩余使用次数：{{ adminRemainingTimes }}， 已分配：{{
                  adminDistribute
                }}，可分配：{{ frequency - adminDistribute }}
              </template>
            </a-alert>
          </div>
          <div class="button-area">
            <a-button type="primary" @click="handleOpenAddModal">
              添加子账号
            </a-button>
            <a-button class="ml-2" type="default" @click="handleSplitEqually">
              一键平分
            </a-button>
            <a-button
              class="ml-2"
              type="primary"
              @click="handleSave"
              :loading="loading"
            >
              保存数据
            </a-button>
          </div>
        </div>
      </template>
      <!-- 序号 -->
      <template #index="{ index }">
        {{ index + 1 }}
      </template>
      <!-- 姓名 -->
      <template #accountName="{ record }">
        <span v-if="record.memberName === userInfo.userName">
          {{ record.accountName }}
        </span>
        <a-input
          v-else
          v-model:value="record.accountName"
          placeholder="请输入姓名"
        />
      </template>
      <!-- 授权次数 -->
      <template #usageTimes="{ record }">
        <a-input
          v-model:value="record.usageTimes"
          placeholder="请输入授权次数"
          @blur="onChangeUsageTimes(record)"
        />
      </template>
      <template #operation="{ record, index }">
        <a-popconfirm
          title="确认删除?"
          ok-text="是"
          cancel-text="否"
          @confirm="onDeleteSubAccount(record, index)"
        >
          <a-button
            v-if="record.memberName !== userInfo.userName"
            type="danger"
            size="small"
            class="ml-5"
            :loading="loading"
          >
            删除
          </a-button>
        </a-popconfirm>
      </template>
    </a-table>
    <a-modal
      v-model:visible="visible"
      title="添加子账号"
      width="600px"
      ok-text="确认"
      cancel-text="取消"
      @ok="handleAddSubAccount"
    >
      <div>
        <a-form
          :model="subAccount"
          :rules="rules"
          ref="formRef"
          layout="vertical"
        >
          <a-form-item label="账号" name="memberName">
            <a-input
              v-model:value="subAccount.memberName"
              placeholder="请输入账号进行匹配"
            />
          </a-form-item>
          <a-form-item label="姓名" name="accountName">
            <a-input
              v-model:value="subAccount.accountName"
              placeholder="请输入备注姓名"
            />
          </a-form-item>
          <a-form-item label="授权次数" name="usageTimes">
            <a-input-number
              v-model:value="subAccount.usageTimes"
              :min="0"
              :step="1"
              placeholder="请输入授权次数"
              style="width: 100%"
            />
          </a-form-item>
        </a-form>
      </div>
    </a-modal>
  </div>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import dayjs from "dayjs";
import _ from "lodash";
import { defineComponent, onMounted, reactive, ref, toRefs } from "vue";
import { settingStates } from "..";
import {
  deleteSubAccount,
  getSubAccountList,
  getUserByUserName,
  saveSubAccount,
} from "../../../../request/api";
import { scrollX } from "../../../../utils/common";
import { openNotification } from "../../../../utils/message";
import { Account, accountColumns } from "./data";
import { userStates } from "../..";

export default defineComponent({
  components: {},
  setup() {
    const formRef = ref();
    const states = reactive({
      loading: false,
      visible: false,
      frequency: 0, //总次数
      adminValidDate: 0, //总授权天数
      adminExpireDate: undefined, //过期时间
      adminRemainingTimes: 0, //剩余次数
      adminDistribute: 0, //已分配
      accountList: [] as Account[],
      subAccount: {} as Account,
      rules: {
        memberName: [
          { required: true, message: "请输入账号", trigger: "blur" },
        ],
        accountName: [
          { required: true, message: "请输入姓名", trigger: "blur" },
        ],
        usageTimes: [
          { required: true, message: "请输入授权次数", trigger: "blur" },
        ],
      },
    });

    const initData = () => {
      states.loading = true;
      getSubAccountList()
        .then((res: any) => {
          states.accountList = res.data;
          const adminData = res.data[0];
          states.frequency = adminData.frequency;
          states.adminValidDate = adminData.validDate;
          states.adminExpireDate = adminData.adminExpireDate;
          states.adminRemainingTimes = adminData.frequency;
          res.data.map((item: any) => {
            states.adminRemainingTimes -= item.usedTimes;
          });
          setAdminDistribute();
        })
        .finally(() => {
          states.loading = false;
        });
    };

    //页面加载
    onMounted(() => {
      initData();
    });

    //添加
    const handleOpenAddModal = () => {
      states.visible = true;
    };

    //确认添加
    const handleAddSubAccount = () => {
      formRef.value.validate().then(async () => {
        const memberNames = states.accountList.map((item) => item.memberName);
        if (memberNames.indexOf(states.subAccount.memberName) > -1) {
          openNotification(
            "error",
            "当前账号已添加，无需重复添加",
            "添加失败",
            10
          );
          return;
        }
        //判断账号是否存在
        const params = { userName: states.subAccount.memberName };
        var userRes = (await getUserByUserName(params)) as any;
        if (!userRes?.data) {
          openNotification("error", "当前账号不存在", "添加失败", 10);
          return;
        }
        let remainingTimes = states.subAccount.usageTimes!;
        states.accountList.map((item) => {
          remainingTimes += item.remainingTimes!;
        });
        if (remainingTimes > states.adminRemainingTimes) {
          openNotification(
            "error",
            "可授权次数不足，请修改授权次数或回列表页调整其余账户的授权次数后再进行添加",
            "添加失败",
            10
          );
          return;
        }

        states.subAccount.activeDate = dayjs().format("YYYY/MM/DD hh:mm:ss");
        states.subAccount.usedTimes = 0;
        states.subAccount.remainingTimes = states.subAccount.usageTimes;
        states.subAccount.validDate = states.adminValidDate;
        states.subAccount.expireDate = states.adminExpireDate;
        states.accountList.push(_.cloneDeep(states.subAccount));
        openNotification(
          "success",
          "如全部子账号已设置完毕，请点击“保存数据”按钮进行数据保存",
          "添加成功",
          10
        );
        setAdminDistribute();
        states.visible = false;
      });
    };

    //一键平分
    const handleSplitEqually = () => {
      let times = Math.floor(states.frequency / states.accountList.length);
      states.accountList.map((item) => {
        item.usageTimes = times;
        item.remainingTimes = item.usageTimes - item.usedTimes!;
      });
      setAdminDistribute();
    };

    //保存
    const handleSave = () => {
      const params = {
        code: userStates.code_info.code,
        accountList: states.accountList,
      };
      states.loading = true;
      saveSubAccount(params)
        .then(() => {
          initData();
          message.success("保存成功");
        })
        .finally(() => {
          states.loading = false;
        });
    };

    //删除
    const onDeleteSubAccount = (record: any, index: any) => {
      if (record.id!) {
        deleteSubAccount({ id: record.id }).then((res: any) => {
          if (res.data) {
            message.success("删除成功");
            states.accountList.splice(index, 1);
          } else {
            message.error("删除失败，请稍后重试");
          }
        });
      } else {
        message.success("删除成功");
        states.accountList.splice(index, 1);
      }
    };

    //修改授权次数
    const onChangeUsageTimes = (record: Account) => {
      if (record.usedTimes! > record.usageTimes!) {
        openNotification(
          "warning",
          "授权次数最小值不能小于已使用次数",
          "温馨提示",
          10
        );
        record.usageTimes = record.usedTimes;
      }
      record.remainingTimes = record.usageTimes! - record.usedTimes!;

      let remainingTimes = 0;
      states.accountList.map((item) => {
        remainingTimes += item.remainingTimes!;
      });
      if (remainingTimes > states.adminRemainingTimes) {
        record.usageTimes = record.usedTimes;
        record.remainingTimes = record.usageTimes! - record.usedTimes!;
        openNotification("error", "可授权次数不足，请重新填写", "添加失败", 10);
      }
      setAdminDistribute();
    };

    const setAdminDistribute = () => {
      states.adminDistribute = 0;
      states.accountList.map((item) => {
        states.adminDistribute += Number(item.usageTimes!);
      });
    };

    return {
      ...toRefs(states),
      ...toRefs(settingStates),
      formRef,
      accountColumns,
      scrollX,
      handleOpenAddModal,
      handleAddSubAccount,
      handleSplitEqually,
      handleSave,
      onDeleteSubAccount,
      onChangeUsageTimes,
    };
  },
});
</script>
<style scoped lang="less">
@import "../index.less";
:deep(.ant-form-item) {
  line-height: 3;
}
</style>
