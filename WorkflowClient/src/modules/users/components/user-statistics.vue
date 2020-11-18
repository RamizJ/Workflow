<template>
  <div class="user-statistics">
    <el-row :gutter="20">
      <el-col :span="8">
        <GoalsPieChart
          title="Статус задач по всем проектам"
          v-if="!goalsPieChartLoading"
          :data="goalsPieChartData"
        />
      </el-col>
      <el-col :span="16">
        <GoalsLineChart
          :project-id="projectId"
          :data="goalsLineChartData"
          :date-range="goalsLineChartDateRange"
          :with-project="true"
          @project-change="updateProject"
          @range-change="updateRange"
        />
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import moment from 'moment'
import usersStore from '../store/users.store'
import GoalsPieChart from '@/modules/goals/components/goals-pie-chart.vue'
import { Statistics } from '@/core/types/statistics.model'
import GoalsLineChart from '@/modules/goals/components/goals-line-chart.vue'

@Component({ components: { GoalsLineChart, GoalsPieChart } })
export default class UserStatistics extends Vue {
  private goalsPieChartLoading = false
  private goalsPieChartData: number[] = []

  private projectId: number | null = null
  private goalsLineChartLoading = false
  private goalsLineChartData: { date: string; goalCountForState: number[] }[] = []
  private goalsLineChartDateRange: string[] = [
    moment.utc(moment().subtract('2', 'weeks')).format(),
    moment.utc(moment().add('1', 'day')).format(),
  ]

  private get userId(): string {
    return this.$route.params.userId
  }

  protected async mounted(): Promise<void> {
    await this.loadGoalsPieChart()
  }

  private async loadGoalsPieChart(): Promise<void> {
    this.goalsPieChartLoading = true
    const userId: string = this.userId
    const dateBegin: string = moment.utc(moment().subtract('5', 'years')).format()
    const dateEnd: string = moment.utc(moment().add('1', 'day')).format()
    const statistics: Statistics = await usersStore.getStatistics({ userId, dateBegin, dateEnd })
    this.goalsPieChartData = statistics.goalsCountForState
    this.goalsPieChartLoading = false
  }

  private async loadGoalsLineChart(): Promise<void> {
    if (!this.projectId) return
    this.goalsLineChartLoading = true
    const projectId = this.projectId
    const userId: string = this.userId
    const dateBegin: string = this.goalsLineChartDateRange[0]
    const dateEnd: string = this.goalsLineChartDateRange[1]
    const statistics: Statistics = await usersStore.getProjectStatistics({
      userId,
      projectId,
      dateBegin,
      dateEnd,
    })
    this.goalsLineChartData = statistics.byDateStatistics
    this.goalsLineChartLoading = false
  }

  private async updateProject(id: number): Promise<void> {
    this.projectId = id
    await this.loadGoalsLineChart()
  }

  private async updateRange(dateRange: string[]): Promise<void> {
    this.goalsLineChartDateRange = dateRange
    await this.loadGoalsLineChart()
  }
}
</script>

<style scoped></style>
