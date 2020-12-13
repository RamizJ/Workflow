export interface WorkloadByProjectsStatistics {
  totalHours: number
  projectHours: {
    [projectName: string]: number
  }
}
