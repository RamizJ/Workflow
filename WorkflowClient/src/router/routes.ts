import LayoutDefault from '@/layouts/LayoutDefault.vue'
import LayoutEmpty from '@/layouts/LayoutEmpty.vue'

export const routes = [
  { path: '/', redirect: '/tasks' },
  {
    path: '/login',
    name: 'Login',
    component: (): Promise<typeof import('@/views/Login.vue')> => import('@/views/Login.vue'),
    meta: { layout: LayoutEmpty },
  },
  {
    path: '/tasks',
    name: 'Tasks',
    component: (): Promise<typeof import('@/views/Tasks.vue')> => import('@/views/Tasks.vue'),
    meta: { layout: LayoutDefault },
  },
  {
    path: '/projects',
    name: 'Projects',
    component: (): Promise<typeof import('@/views/Projects.vue')> => import('@/views/Projects.vue'),
    meta: { layout: LayoutDefault },
  },
  {
    path: '/projects/:projectId',
    name: 'Project',
    component: (): Promise<typeof import('@/views/Project.vue')> => import('@/views/Project.vue'),
    meta: { layout: LayoutDefault },
  },
  {
    path: '/teams',
    name: 'Teams',
    component: (): Promise<typeof import('@/views/Teams.vue')> => import('@/views/Teams.vue'),
    meta: { layout: LayoutDefault },
  },
  {
    path: '/teams/:teamId',
    name: 'Team',
    component: (): Promise<typeof import('@/views/Team.vue')> => import('@/views/Team.vue'),
    meta: { layout: LayoutDefault },
  },
  {
    path: '/users',
    name: 'Users',
    component: (): Promise<typeof import('@/views/Users.vue')> => import('@/views/Users.vue'),
    meta: { layout: LayoutDefault },
  },
]
