import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    name: 'groups',
    path: '/groups',
    component: (): Promise<typeof import('../pages/groups.vue')> =>
      import(/* webpackChunkName: "goals" */ '../pages/groups.vue'),
    meta: { layout: MainLayout },
  },
]
