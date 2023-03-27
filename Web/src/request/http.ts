import axios from "axios"; // 引入axios
import QS from "qs"; // 引入qs模块，用来序列化post类型的数据
import { openNotfication } from "../utils/message";

// 环境的切换
if (process.env.NODE_ENV == "development") {
  axios.defaults.baseURL = "https://localhost:44373/api";
} else if (process.env.NODE_ENV == "production") {
  axios.defaults.baseURL = "https://diorest.top/api/api";
}

// 请求超时时间
axios.defaults.timeout = 10000;

// 请求拦截器
axios.interceptors.request.use(
  (config: any) => {
    // 每次发送请求之前判断是否存在token，如果存在，则统一在http请求的header都加上token，不用每次请求都手动添加了
    // 即使本地存在token，也有可能token是过期的，所以在响应拦截器中要对返回状态进行判断
    const token = localStorage.getItem("token");
    const userName = JSON.parse(localStorage.getItem("userInfo")!).userName;
    token &&
      (config.headers.Authorization = token) &&
      (config.headers.userName = userName);
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// post请求头
axios.defaults.headers.post["Content-Type"] =
  "application/x-www-form-urlencoded;charset=UTF-8";

/**
 * get方法，对应get请求
 * @param {String} url [请求的url地址]
 * @param {Object} params [请求时携带的参数]
 */
export function get(url: string, params: any) {
  return new Promise((resolve, reject) => {
    axios
      .get(url, {
        params: params,
      })
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        reject(err.data);
      });
  });
}

/**
 * post方法，对应post请求
 * @param {String} url [请求的url地址]
 * @param {Object} params [请求时携带的参数]
 */
export function post(url: string, params: any) {
  return new Promise((resolve, reject) => {
    axios
      .post(url, params)
      .then((res) => {
        if (res.data.code === 100) {
          resolve(res.data);
        } else {
          openNotfication("warning", res.data.msg);
          reject(res.data);
        }
      })
      .catch((err) => {
        openNotfication("warning", err.message);
        reject(err.data);
      });
  });
}
