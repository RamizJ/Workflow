import Team from '@/types/team.type'
import Entity from '@/types/entity.type'

export default interface Project extends Entity {
  id?: number
  name: string
  description?: string
  ownerId?: string
  ownerFio?: string
  creationDate?: Date
  expectedCompletedDate?: Date
  groupId?: number
  groupName?: string
  isRemoved?: boolean

  teams?: Team[]
  teamId?: number
  teamIds?: number[]
}
