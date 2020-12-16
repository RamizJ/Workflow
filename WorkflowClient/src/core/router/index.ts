import Vue from 'vue'
import VueRouter from 'vue-router'

import { authGuard, hubGuard } from './guards'
import { routes } from './routes'

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  base: process.env.VUE_APP_BASE_URL,
  routes,
})

router.beforeEach(authGuard)
router.beforeEach(hubGuard)

export default router
