import Attachment from '@/types/attachment.type'
import Entity from '@/types/entity.type'
import moment from 'moment'

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

export default class Task extends Entity {
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
  creationDate: string = moment.utc(moment()).format()
  expectedCompletedDate?: string
  estimatedPerformingTime?: string
  state: Status = Status.New
  priority?: Priority
  isChildsExist?: boolean
  isAttachmentsExist?: boolean
  isRemoved?: boolean

  childTasks?: Task[]
  attachments?: Attachment[]
  parent?: Task[]
  completed?: boolean
}
