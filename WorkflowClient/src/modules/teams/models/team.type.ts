import Entity from '@/core/types/entity.type'

export default class Team extends Entity {
  id?: number
  name?: string
  description?: string
  groupId?: number
  groupName?: string
  isRemoved?: boolean

  userIds?: string[]
  projectIds?: number[]
}
