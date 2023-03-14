import Vue from "vue";

import config from "./config";
import makeCrudModule from "./store/crud";
import makeCrudRoutes from "./router/crud";
import makeService from "./services/api";
import router from "./router";
import store from "./store";

import App from "./App.vue";
import FormContainer from "./components/FormContainer.vue";
import ListingContainer from "./components/ListingContainer.vue";

Vue.config.productionTip = false;
var contentTypes;
var response = fetch(`https://localhost:7170/UIOMatic/GetAll`)
    .then(response => contentTypes = response.json());



config.contentTypes.forEach(contentType => {
  // Register dynamically generated store modules.
  store.registerModule(
    contentType.name,
    makeCrudModule({
        service: makeService(contentType)
    })
  );

  // Register dynamically generated routes.
  router.addRoutes(
    makeCrudRoutes({
      components: {
        FormContainer,
        ListingContainer
      },
      contentType
    })
  );
});

new Vue({
  render: h => h(App),
  router,
  store,
   
}).$mount("#app");
