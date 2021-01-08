export type Statistics = {
  goalsCountForState: number[]
  byDateStatistics: {
    date: string
    goalsCompletedInTimeCount: number
    goalsCompletedLateCount: number
    goalsInProcessCount: number
    goalsNotCompletedCount: number
    goalsCountForState: number[]
  }[]
}
