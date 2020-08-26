import Attachment from '@/types/attachment.type'

export default interface Task {
  id?: number
  goalNumber?: number
  title?: string
  description?: string
  projectId?: number
  projectName?: string
  parentGoalId?: number
  ownerId?: string
  ownerFio?: string
  performerId?: string
  performerFio?: string
  creationDate: string
  expectedCompletedDate?: string
  estimatedPerformingTime?: string
  state: Status
  priority?: Priority
  isChildsExist?: boolean
  isAttachmentsExist?: boolean
  isRemoved?: boolean

  attachments?: Attachment[]
  parent?: Task[]
  child?: Task[]

  index?: number
  completed?: boolean
}

export enum Priority {
  Low = 'Low',
  Normal = 'Normal',
  High = 'High'
}

export enum Status {
  New = 'New',
  Perform = 'Perform',
  Delay = 'Delay',
  Testing = 'Testing',
  Succeed = 'Succeed',
  Rejected = 'Rejected'
}
