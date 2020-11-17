import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    name: 'settings',
    path: '/settings',
    component: (): Promise<typeof import('../pages/settings.vue')> =>
      import(/* webpackChunkName: "settings" */ '../pages/settings.vue'),
    meta: { layout: MainLayout },
  },
]
