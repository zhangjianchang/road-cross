import { createRouter, createWebHashHistory } from "vue-router";
import Home from "../views/Home/index.vue";
import Design from "../views/Design/index.vue";
import Doc from "../views/Doc/index.vue";
import Authorize from "../views/Authorize/index.vue";
import Contact from "../views/Contact/index.vue";
import Login from "../views/Login/index.vue";
import SignIn from "../views/Login/signIn.vue";
import UserCenter from "../views/UserCenter/index.vue";
import Settings from "../views/UserCenter/Settings/index.vue";
import DesignRoadList from "../views/UserCenter/DesignRoadList/index.vue";
import Suggestion from "../views/UserCenter/Settings/Suggestion/index.vue";

import Map from "../views/Map/index.vue";

import { PageEnum } from "./data";

const routes = [
  {
    path: "/",
    name: PageEnum.Home,
    component: Home,
  },
  {
    path: "/design",
    name: PageEnum.Design,
    component: Design,
  },
  {
    path: "/design/edit/:guid?",
    name: PageEnum.DesignEdit,
    component: Design,
  },
  {
    path: "/designRoadList",
    name: PageEnum.DesignRoadList,
    component: DesignRoadList,
  },
  {
    path: "/doc",
    name: PageEnum.Doc,
    component: Doc,
  },
  {
    path: "/authorize",
    name: PageEnum.Authorize,
    component: Authorize,
  },
  {
    path: "/contact",
    name: PageEnum.Contact,
    component: Contact,
  },
  {
    path: "/login",
    name: PageEnum.Login,
    component: Login,
  },
  {
    path: "/signIn",
    name: PageEnum.SignIn,
    component: SignIn,
  },
  {
    path: "/userCenter",
    name: PageEnum.UserCenter,
    component: UserCenter,
  },
  {
    path: "/settings/basicInfo",
    name: PageEnum.BasicInfo,
    component: Settings,
  },
  {
    path: "/settings/resetPwd",
    name: PageEnum.ResetPwd,
    component: Settings,
  },
  {
    path: "/settings/activateCode",
    name: PageEnum.ActivateCode,
    component: Settings,
  },
  {
    path: "/settings/suggestion",
    name: PageEnum.Suggestion,
    component: Suggestion,
  },
  {
    path: "/map",
    name: PageEnum.Map,
    component: Map,
  },
];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

export default router;
