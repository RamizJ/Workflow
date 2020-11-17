import Attachment from '@/modules/goals/models/attachment.type'
import Entity from '@/core/types/entity.type'
import moment from 'moment'
import { Metadata } from '@/core/types/metadata.model'

export default class Goal extends Entity {
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
  priority: Priority = Priority.Normal
  hasChildren?: boolean
  isSection?: boolean
  isAttachmentsExist?: boolean
  isRemoved?: boolean
  children?: Goal[]
  metadataList: Metadata[] = []

  attachments?: Attachment[]
  parent?: Goal[]
  completed?: boolean

  constructor() {
    super()
  }
}

export enum Priority {
  Low = 'Low',
  Normal = 'Normal',
  High = 'High',
}

export const priorities = [
  { value: Priority.Low, label: 'Низкий' },
  { value: Priority.Normal, label: 'Средний' },
  { value: Priority.High, label: 'Высокий' },
]

export enum Status {
  New = 'New',
  Perform = 'Perform',
  Delay = 'Delay',
  Testing = 'Testing',
  Succeed = 'Succeed',
  Rejected = 'Rejected',
}

export const statuses = [
  { value: Status.New, label: 'Новое' },
  { value: Status.Perform, label: 'Выполняется' },
  { value: Status.Testing, label: 'Проверяется' },
  { value: Status.Delay, label: 'Отложено' },
  { value: Status.Succeed, label: 'Выполнено' },
  { value: Status.Rejected, label: 'Отклонено' },
]
