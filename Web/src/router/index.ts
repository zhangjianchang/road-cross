import { createRouter, createWebHashHistory } from "vue-router";
import Home from "../views/Home/index.vue";
import Design from "../views/Design/index.vue";
import DesignRoadList from "../views/DesignRoadList/index.vue";
import Doc from "../views/Doc/index.vue";
import Authorize from "../views/Authorize/index.vue";
import Contact from "../views/Contact/index.vue";
import Login from "../views/Login/index.vue";
import SignIn from "../views/Login/signIn.vue";
import UserCenter from "../views/UserCenter/index.vue";

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
];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

export default router;
