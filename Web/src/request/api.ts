/**
 * api接口统一管理
 */
import { get, post } from "./http";

//注册
export const userSignIn = (params: any) => post("/user/signIn", params);

//登录
export const userLogin = (params: any) => post("/account/login", params);
