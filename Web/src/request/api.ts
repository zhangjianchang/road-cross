/**
 * api接口统一管理
 */
import { get, post } from "./http";

/*********************用户相关 *****************/
//检查token合法性
export const checkToken = () => post("/account/checkToken");
//注册
export const userSignIn = (params: any) => post("/account/signIn", params);
//登录
export const userLogin = (params: any) => post("/account/login", params);
//登出
export const userLogout = () => post("/account/logout");
//重置密码
export const reSetPwd = (params: any) => post("/account/reSetPwd", params);
//查询用户信息
export const getUserInfo = () => post("/account/getUserInfo");
/*********************用户相关 *****************/

/*********************设计相关 *****************/
//保存
export const saveDesign = (params: any) => post("/design/save", params);
//删除
export const deleteDesign = (params: any) => post("/design/delete", params);
//获取当前
export const getDesignByGuid = (params: any) =>
  post("/design/getByGuid", params);
//获取我的
export const getDesignList = () => post("/design/getList");
/*********************设计相关 *****************/

/*********************授权码相关 *****************/
//生成授权码
export const generateCode = (params: any) =>
  post("/AuthotizationCode/generateCode", params);
//激活授权码
export const activeCode = (params: any) =>
  post("/AuthotizationCode/activeCode", params);
//使用授权码
export const useCode = (params: any) =>
  post("/AuthotizationCode/useCode", params);
//获取当前授权码信息
export const getCodeInfo = () => post("/AuthotizationCode/getCodeInfo");
//查找用户全部授权码
export const getCodeInfosByUser = () =>
  post("/AuthotizationCode/getCodeInfosByUser");
/*********************授权码相关 *****************/
