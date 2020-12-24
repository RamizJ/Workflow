import MainLayout from '@/core/layouts/main.layout.vue'
import BlankLayout from '@/core/layouts/blank.layout.vue'

export default [
  {
    name: 'login',
    path: '/login',
    component: (): Promise<typeof import('../pages/login.vue')> =>
      import(/* webpackChunkName: "login" */ '../pages/login.vue'),
    meta: { layout: BlankLayout },
  },
  {
    name: 'users',
    path: '/users',
    component: (): Promise<typeof import('../pages/users.vue')> =>
      import(/* webpackChunkName: "users" */ '../pages/users.vue'),
    meta: { layout: MainLayout },
  },
  {
    name: 'user',
    path: '/users/:userId',
    component: (): Promise<typeof import('../pages/user.vue')> =>
      import(/* webpackChunkName: "user" */ '../pages/user.vue'),
    meta: { layout: MainLayout },
    children: [
      {
        name: 'user-info',
        path: 'info',
        component: (): Promise<typeof import('../components/user-info.vue')> =>
          import(/* webpackChunkName: "user-info" */ '../components/user-info.vue'),
        meta: { layout: MainLayout },
      },
      {
        name: 'user-statistics',
        path: 'statistics',
        component: (): Promise<
          typeof import('../components/user-statistics/user-statistics.vue')
        > =>
          import(
            /* webpackChunkName: "user-statistics" */ '../components/user-statistics/user-statistics.vue'
          ),
        meta: { layout: MainLayout },
      },
    ],
  },
]
