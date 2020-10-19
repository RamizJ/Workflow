import Attachment from '@/modules/goals/models/attachment.type'

export interface IGoalData {
  readonly id?: number
  readonly goalNumber?: number
  readonly title?: string
  readonly description?: string
  readonly projectId?: number
  readonly projectName?: string
  readonly parentGoalId?: number
  readonly ownerId?: string
  readonly ownerFio?: string
  readonly performerId?: string
  readonly performerFio?: string
  readonly creationDate?: string
  readonly expectedCompletedDate?: string
  readonly estimatedPerformingTime?: string
  readonly state: Status
  readonly priority?: Priority
  readonly hasChildren?: boolean
  readonly isAttachmentsExist?: boolean
  readonly isRemoved?: boolean
}

export interface IGoal extends IGoalData {
  readonly children?: IGoal[]
  readonly attachments?: Attachment[]
}

export enum Priority {
  Low = 'Low',
  Normal = 'Normal',
  High = 'High',
}

export enum Status {
  New = 'New',
  Perform = 'Perform',
  Delay = 'Delay',
  Testing = 'Testing',
  Succeed = 'Succeed',
  Rejected = 'Rejected',
}
