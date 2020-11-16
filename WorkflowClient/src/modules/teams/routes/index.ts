import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    name: 'teams',
    path: '/teams',
    component: (): Promise<typeof import('../pages/teams.vue')> =>
      import(/* webpackChunkName: "teams" */ '../pages/teams.vue'),
    meta: { layout: MainLayout },
  },
  {
    name: 'team',
    path: '/teams/:teamId',
    component: (): Promise<typeof import('../pages/team.vue')> =>
      import(/* webpackChunkName: "team" */ '../pages/team.vue'),
    meta: { layout: MainLayout },
    children: [
      {
        name: 'team-users',
        path: 'users',
        component: (): Promise<typeof import('../components/team-users.vue')> =>
          import(/* webpackChunkName: "team-users" */ '../components/team-users.vue'),
        meta: { layout: MainLayout },
      },
      {
        name: 'team-projects',
        path: 'projects',
        component: (): Promise<typeof import('../components/team-projects.vue')> =>
          import(/* webpackChunkName: "team-projects" */ '../components/team-projects.vue'),
        meta: { layout: MainLayout },
      },
    ],
  },
]
