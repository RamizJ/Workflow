import { Status } from '@/modules/goals/models/task.type'

export type ProjectStatistics = {
  goalsCountForState: number[]
  byDateStatistics: { date: string; goalCountForState: number[] }[]
}
