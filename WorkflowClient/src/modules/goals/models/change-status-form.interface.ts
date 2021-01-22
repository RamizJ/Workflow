import { Status } from '@/modules/goals/models/goal.type'

export interface ChangeStatusForm {
  goalId?: number
  goalState?: Status
  comment: string
  estimatedPerformingHours?: number
  actualPerformingHours?: number
}
