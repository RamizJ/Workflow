export interface GoalCompletionStatistics {
  userGoalsCompletions: { userId: string; goalsCompletion: GoalsCompletion }[]
}

export interface GoalsCompletion {
  completedOnTime: number
  completedNotOnTime: number
  inProcess: number
  notCompleted: number
}
