import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    name: 'projects',
    path: '/projects',
    component: (): Promise<typeof import('../pages/projects.vue')> =>
      import(/* webpackChunkName: "projects" */ '../pages/projects.vue'),
    meta: { layout: MainLayout },
  },
  {
    name: 'project',
    path: '/projects/:projectId',
    component: (): Promise<typeof import('../pages/project.vue')> =>
      import(/* webpackChunkName: "project" */ '../pages/project.vue'),
    meta: { layout: MainLayout },
    children: [
      {
        name: 'project-overview',
        path: 'overview',
        component: (): Promise<typeof import('../components/project-overview.vue')> =>
          import(/* webpackChunkName: "project-overview" */ '../components/project-overview.vue'),
        meta: { layout: MainLayout },
      },
      {
        name: 'project-goals',
        path: 'goals',
        alias: 'goals/*',
        component: (): Promise<typeof import('../components/project-tasks.vue')> =>
          import(/* webpackChunkName: "project-tasks" */ '../components/project-tasks.vue'),
        meta: { layout: MainLayout },
      },
      {
        name: 'project-teams',
        path: 'teams',
        component: (): Promise<typeof import('../components/project-teams.vue')> =>
          import(/* webpackChunkName: "project-teams" */ '../components/project-teams.vue'),
        meta: { layout: MainLayout },
      },
      {
        name: 'project-statistics',
        path: 'statistics',
        component: (): Promise<typeof import('../components/project-statistics.vue')> =>
          import(
            /* webpackChunkName: "project-statistics" */ '../components/project-statistics.vue'
          ),
        meta: { layout: MainLayout },
      },
    ],
  },
]
