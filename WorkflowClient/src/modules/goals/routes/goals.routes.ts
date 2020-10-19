import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    path: '/goals',
    name: 'goals',
    component: (): Promise<typeof import('../pages/goals.vue')> =>
      import(/* webpackChunkName: "goals" */ '../pages/goals.vue'),
    meta: { layout: MainLayout },
  },
]
