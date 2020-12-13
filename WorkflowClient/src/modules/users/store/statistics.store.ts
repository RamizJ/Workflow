import { getModule, Module, MutationAction, VuexModule } from 'vuex-module-decorators'
import store from '@/core/store'
import statisticsApi from '../api/statistics.api'
import { GoalCompletionStatistics } from '@/modules/users/models/goal-completion-statistics.interface'
import { WorkloadByProjectsStatistics } from '@/modules/users/models/workload-by-projects-statistics.interface'
import { WorkloadByDaysStatistics } from '@/modules/users/models/workload-by-days-statistics.interface'
import { StatisticsQuery } from '@/modules/users/models/statistics-query.interface'
import { Message } from 'element-ui'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'statisticsStore',
  store,
})
class StatisticsStore extends VuexModule {
  _total: {
    [userId: string]: {
      goalCompletion: GoalCompletionStatistics
      workloadByProjects: WorkloadByProjectsStatistics
      workloadByDays: WorkloadByDaysStatistics
    }
  } = {}
  _goalCompletion: { [userId: string]: GoalCompletionStatistics } = {}
  _workloadByProjects: { [userId: string]: WorkloadByProjectsStatistics } = {}
  _workloadByDays: { [userId: string]: WorkloadByDaysStatistics } = {}

  public get total() {
    return this._total
  }
  public get goalCompletion() {
    return this._goalCompletion
  }
  public get workloadByProjects() {
    return this._workloadByProjects
  }
  public get workloadByDays() {
    return this._workloadByDays
  }

  @MutationAction({ mutate: ['_total'] })
  public async getTotal(query: StatisticsQuery) {
    try {
      const response = await statisticsApi.getTotal(query)
      return {
        _total: response.data,
      }
    } catch (e) {
      Message.error(`Не удалось получить статистику`)
      return {
        _total: this._total,
      }
    }
  }

  @MutationAction({ mutate: ['_goalCompletion'] })
  public async getGoalCompletion(query: StatisticsQuery) {
    try {
      const response = await statisticsApi.getGoalCompletion(query)
      return {
        _goalCompletion: response.data,
      }
    } catch (e) {
      Message.error(`Не удалось получить статистику выполнения задач`)
      return {
        _goalCompletion: this._goalCompletion,
      }
    }
  }

  @MutationAction({ mutate: ['_workloadByProjects'] })
  public async getWorkloadByProjects(query: StatisticsQuery) {
    try {
      const response = await statisticsApi.getWorkloadByProjects(query)
      return {
        _workloadByProjects: response.data,
      }
    } catch (e) {
      Message.error(`Не удалось получить загрузку пользователя по проектам`)
      return {
        _workloadByProjects: this._workloadByProjects,
      }
    }
  }

  @MutationAction({ mutate: ['_workloadByDays'] })
  public async getWorkloadByDays(query: StatisticsQuery) {
    try {
      const response = await statisticsApi.getWorkloadByDays(query)
      return {
        _workloadByDays: response.data,
      }
    } catch (e) {
      Message.error(`Не удалось получить загрузку пользователя по дням`)
      return {
        _workloadByDays: this._workloadByDays,
      }
    }
  }
}

export default getModule(StatisticsStore)
