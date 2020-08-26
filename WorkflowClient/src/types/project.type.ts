import Team from '@/types/team.type'

export default interface Project {
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

  index?: number
  teams?: Team[]
  teamId?: number
  teamIds?: number[]
}
