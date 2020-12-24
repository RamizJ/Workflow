import { groupsRoutes } from '@/modules/groups'
import { projectsRoutes } from '@/modules/projects'
import { goalsRoutes } from '@/modules/goals'
import { teamsRoutes } from '@/modules/teams'
import { usersRoutes } from '@/modules/users'
import { settingsRoutes } from '@/modules/settings'

export const routes = [
  { path: '/', redirect: '/goals' },
  ...groupsRoutes,
  ...projectsRoutes,
  ...goalsRoutes,
  ...teamsRoutes,
  ...usersRoutes,
  ...settingsRoutes,
]
