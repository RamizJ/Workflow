import { Status } from '@/modules/goals/models/goal.type'

export type ProjectStatistics = {
  goalsCountForState: number[]
  byDateStatistics: { date: string; goalCountForState: number[] }[]
}
