export default interface Team {
  id?: number
  name: string
  description?: string
  groupId?: number
  groupName?: string
  isRemoved?: boolean

  userIds?: string[]
  projectIds?: number[]

  index?: number
}
