import Vue from "vue";
import VueRouter from "vue-router";

import Welcome from "../components/Welcome.vue";

Vue.use(VueRouter);

export default new VueRouter({
  routes: [
    {
      path: "/",
      component: Welcome
    }
  ]
});
