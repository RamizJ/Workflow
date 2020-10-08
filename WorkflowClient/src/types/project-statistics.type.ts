import { Status } from '@/types/task.type'

export type ProjectStatistics = {
  goalsCountForState: number[]
  byDateStatistics: { date: string; goalCountForState: number[] }[]
}
