
import Vue from 'vue'
import App from './App.vue'
import router from './router'
import axios from 'axios'

Vue.config.productionTip = false

// Load config from REST API
axios.get('https://api.example.com/config')
  .then(({ data: config }) => {
    // Set global config
    Vue.prototype.$config = config

    new Vue({
      router,
      render: h => h(App)
    }).$mount('#app')
  })

