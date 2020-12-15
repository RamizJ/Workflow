export interface WorkloadByProjectsStatistics {
  totalHours: number
  projectHoursForUsers: {
    userId: string
    hoursForProject: { projectId: number; hours: number }[]
  }[]
}

export interface ProjectHours {
  totalHours: number
  hoursForProject: { projectName: string; hours: number }[]
}
