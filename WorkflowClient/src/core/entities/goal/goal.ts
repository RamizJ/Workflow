import moment from 'moment'
import { IGoal, IGoalData, Priority, Status } from './goal.types'
import Attachment from '@/modules/goals/models/attachment.type'

export class Goal implements IGoal {
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
  creationDate?: string
  expectedCompletedDate?: string
  estimatedPerformingTime?: string
  state: Status = Status.New
  priority?: Priority
  hasChildren?: boolean
  isAttachmentsExist?: boolean
  isRemoved?: boolean

  children?: IGoal[]
  attachments?: Attachment[]

  constructor(data: IGoalData) {
    if (data.id) this.id = data.id
    this.goalNumber = data.goalNumber
    this.title = data.title
    this.description = data.description
    this.projectId = data.projectId
    this.projectId = data.projectId
    this.parentGoalId = data.parentGoalId
    this.ownerId = data.ownerId
    this.ownerFio = data.ownerFio
    this.performerId = data.performerId
    this.performerFio = data.performerFio
    if (data.creationDate) this.creationDate = data.creationDate
    this.expectedCompletedDate = data.expectedCompletedDate
    this.estimatedPerformingTime = data.estimatedPerformingTime
    this.state = data.state
    this.priority = data.priority
    this.hasChildren = data.hasChildren
    this.isAttachmentsExist = data.isAttachmentsExist
    this.isRemoved = data.isRemoved
  }
}
