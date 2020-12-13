import api from '@/core/api'
import { StatisticsQuery } from '@/modules/users/models/statistics-query.interface'
import { AxiosPromise } from 'axios'
import { GoalCompletionStatistics } from '@/modules/users/models/goal-completion-statistics.interface'
import { WorkloadByProjectsStatistics } from '@/modules/users/models/workload-by-projects-statistics.interface'
import { WorkloadByDaysStatistics } from '@/modules/users/models/workload-by-days-statistics.interface'

export default {
  getTotal: (
    query: StatisticsQuery
  ): AxiosPromise<{
    [userId: string]: {
      goalCompletion: GoalCompletionStatistics
      workloadByProjects: WorkloadByProjectsStatistics
      workloadByDays: WorkloadByDaysStatistics
    }
  }> => {
    return api.request({
      url: `/api/Statistic/GetTotal`,
      method: 'POST',
      data: query,
    })
  },
  getGoalCompletion: (
    query: StatisticsQuery
  ): AxiosPromise<{ [userId: string]: GoalCompletionStatistics }> => {
    return api.request({
      url: `/api/Statistic/GetGoalCompletion`,
      method: 'POST',
      data: query,
    })
  },
  getWorkloadByProjects: (
    query: StatisticsQuery
  ): AxiosPromise<{ [userId: string]: WorkloadByProjectsStatistics }> => {
    return api.request({
      url: `/api/Statistic/GetWorkloadByProject`,
      method: 'POST',
      data: query,
    })
  },
  getWorkloadByDays: (
    query: StatisticsQuery
  ): AxiosPromise<{ [userId: string]: WorkloadByDaysStatistics }> => {
    return api.request({
      url: `/api/Statistic/GetWorkloadByDays`,
      method: 'POST',
      data: query,
    })
  },
}
