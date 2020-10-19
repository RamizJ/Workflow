import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    path: '/teams',
    name: 'teams',
    component: (): Promise<typeof import('../pages/teams.vue')> =>
      import(/* webpackChunkName: "teams" */ '../pages/teams.vue'),
    meta: { layout: MainLayout },
  },
  {
    path: '/teams/:teamId',
    name: 'team',
    component: (): Promise<typeof import('../pages/team.vue')> =>
      import(/* webpackChunkName: "team" */ '../pages/team.vue'),
    meta: { layout: MainLayout },
  },
]
