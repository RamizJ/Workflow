import LayoutDefault from '~/layouts/LayoutDefault.vue';
import LayoutLogin from '~/layouts/LayoutLogin.vue';

export const routes = [
  { path: '/', redirect: '/tasks' },
  {
    path: '/',
    name: 'Dashboard',
    component: () => import('~/pages/Dashboard.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/tasks',
    name: 'Tasks',
    component: () => import('~/pages/Tasks.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/projects',
    name: 'Projects',
    component: () => import('~/pages/Projects.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/projects/:projectId',
    name: 'Project',
    component: () => import('~/pages/Project.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/teams',
    name: 'Teams',
    component: () => import('~/pages/Teams.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('~/pages/Login.vue'),
    meta: { layout: LayoutLogin }
  },
  {
    path: '/profile',
    name: 'Profile',
    component: () => import('~/pages/Profile.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/settings',
    name: 'Settings',
    component: () => import('~/pages/Settings.vue'),
    meta: { layout: LayoutDefault }
  },
  {
    path: '/users',
    name: 'Users',
    component: () => import('~/pages/Users.vue'),
    meta: { layout: LayoutDefault }
  }
];
