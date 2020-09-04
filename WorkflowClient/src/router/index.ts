import Vue from 'vue'
import VueRouter from 'vue-router'

import { authGuard } from '@/router/guards'
import { routes } from '@/router/routes'

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  base: process.env.VUE_APP_BASE_URL,
  routes,
})

router.beforeEach(authGuard)

export default router
