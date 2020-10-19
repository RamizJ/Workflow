import { goalsRoutes } from '@/modules/goals'
import { projectsRoutes } from '@/modules/projects'
import { teamsRoutes } from '@/modules/teams'
import { usersRoutes } from '@/modules/users'

export const routes = [
  { path: '/', redirect: '/goals' },
  ...goalsRoutes,
  ...projectsRoutes,
  ...teamsRoutes,
  ...usersRoutes,
]
