import { createApp } from "vue";
import "./style.css";
import App from "./App.vue";
import router from "./router";
import Antd from "ant-design-vue";
import "ant-design-vue/dist/antd.css";
import axios from "axios";
import VueAxios from "vue-axios";
import "tailwindcss/tailwind.css";

const app = createApp(App);
app.use(router).use(Antd).use(VueAxios, axios).mount("#app");
