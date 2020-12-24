import MainLayout from '@/core/layouts/main.layout.vue'

export default [
  {
    name: 'groups',
    path: '/groups',
    component: (): Promise<typeof import('../pages/groups.vue')> =>
      import(/* webpackChunkName: "groups" */ '../pages/groups.vue'),
    meta: { layout: MainLayout },
  },
  {
    name: 'group',
    path: '/groups/:groupId',
    component: (): Promise<typeof import('../pages/group.vue')> =>
      import(/* webpackChunkName: "group" */ '../pages/group.vue'),
    meta: { layout: MainLayout },
    children: [
      {
        name: 'group-projects',
        path: 'projects',
        component: (): Promise<typeof import('../components/group-projects.vue')> =>
          import(/* webpackChunkName: "group-projects" */ '../components/group-projects.vue'),
        meta: { layout: MainLayout },
      },
    ],
  },
]
