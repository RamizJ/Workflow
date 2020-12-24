export interface WorkloadByDaysStatistics {
  dayWorkloads: {
    [key: string]: {
      selectedProjectsWorkload: number
      otherProjectsWorkload: number
      totalWorkload: number
    }
  }
}
