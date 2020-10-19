import MainLayout from '@/core/layouts/main.layout.vue'
import BlankLayout from '@/core/layouts/blank.layout.vue'

export default [
  {
    path: '/login',
    name: 'login',
    component: (): Promise<typeof import('../pages/login.vue')> =>
      import(/* webpackChunkName: "login" */ '../pages/login.vue'),
    meta: { layout: BlankLayout },
  },
  {
    path: '/users',
    name: 'users',
    component: (): Promise<typeof import('../pages/users.vue')> =>
      import(/* webpackChunkName: "users" */ '../pages/users.vue'),
    meta: { layout: MainLayout },
  },
]
