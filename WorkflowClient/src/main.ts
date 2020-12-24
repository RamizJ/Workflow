import Vue from 'vue'
import { App, store, router } from '@/core'

Vue.config.productionTip = false

new Vue({
  store,
  router,
  render: (h) => h(App),
}).$mount('#app')
