import LayoutDefault from '@/layouts/LayoutDefault.vue'
import LayoutLogin from '@/layouts/LayoutLogin.vue'

export const routes = [
  { path: '/', redirect: '/tasks' },
  {
    path: '/tasks',
    name: 'Tasks',
    component: () => import('@/views/Tasks.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
    meta: { layout: LayoutLogin }
  },
  {
    path: '/projects',
    name: 'Projects',
    component: () => import('@/views/Projects.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/projects/:projectId',
    name: 'Project',
    component: () => import('@/views/Project.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/teams',
    name: 'Teams',
    component: () => import('@/views/Teams.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/teams/:teamId',
    name: 'Team',
    component: () => import('@/views/Team.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/users',
    name: 'Users',
    component: () => import('@/views/Users.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/settings',
    name: 'Settings',
    component: () => import('@/views/Settings.vue'),
    meta: { layout: LayoutDefault }
  }
]
