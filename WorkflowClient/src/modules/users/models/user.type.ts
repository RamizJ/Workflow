import Entity from '@/core/types/entity.type'

export default class User extends Entity {
  id?: string
  firstName?: string
  middleName?: string
  lastName?: string
  userName?: string
  password?: string
  email?: string
  phone?: string
  positionId?: number
  position?: string
  isRemoved?: boolean
  roles?: Role[]
  canEditGoals?: boolean
  canCloseGoals?: boolean
  canEditUsers?: boolean
  teamIds?: number[]
}

export enum Role {
  EDIT_PROJECTS = 'EDIT_PROJECTS',
  EDIT_TASKS = 'EDIT_TASKS',
  EDIT_TEAMS = 'EDIT_TEAMS',
  EDIT_USERS = 'EDIT_USERS',
}
