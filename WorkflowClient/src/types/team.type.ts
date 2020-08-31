import Entity from '@/types/entity.type'

export default interface Team extends Entity {
  id?: number
  name: string
  description?: string
  groupId?: number
  groupName?: string
  isRemoved?: boolean

  userIds?: string[]
  projectIds?: number[]
}
