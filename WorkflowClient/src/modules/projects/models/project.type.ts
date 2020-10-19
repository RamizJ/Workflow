import Team from '@/modules/teams/models/team.type'
import Entity from '@/core/types/entity.type'

export default class Project extends Entity {
  id?: number
  name?: string
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
