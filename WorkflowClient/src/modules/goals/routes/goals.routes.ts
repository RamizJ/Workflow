import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    name: 'goals',
    path: '/goals',
    alias: '/goals/*',
    component: (): Promise<typeof import('../pages/goals.vue')> =>
      import(/* webpackChunkName: "goals" */ '../pages/goals.vue'),
    meta: { layout: MainLayout, basePath: '/goals' },
  },
]
