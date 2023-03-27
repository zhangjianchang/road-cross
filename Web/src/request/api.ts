/**
 * api接口统一管理
 */
import { get, post } from "./http";

/**用户相关 */
//注册
export const userSignIn = (params: any) => post("/user/signIn", params);
//登录
export const userLogin = (params: any) => post("/account/login", params);
/**用户相关 */

/**设计相关 */
//保存
export const saveDesign = (params: any) => post("/design/save", params);
//删除
export const deleteDesign = (params: any) => post("/design/delete", params);
//获取当前
export const getDesignByGuid = (params: any) => post("/design/getByGuid", params);
//获取我的
export const getDesignList = () => post("/design/getList", {});
/**设计相关 */
