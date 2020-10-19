import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    path: '/projects',
    name: 'projects',
    component: (): Promise<typeof import('../pages/projects.vue')> =>
      import(/* webpackChunkName: "projects" */ '../pages/projects.vue'),
    meta: { layout: MainLayout },
  },
  {
    path: '/projects/:projectId',
    name: 'project',
    component: (): Promise<typeof import('../pages/project.vue')> =>
      import(/* webpackChunkName: "project" */ '../pages/project.vue'),
    meta: { layout: MainLayout },
  },
]
