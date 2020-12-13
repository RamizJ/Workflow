<template>
  <div class="user-statistics">
    <el-row :gutter="20">
      <el-col :span="8">
        <GoalsPieChart
          title="Статус задач по всем проектам"
          v-if="!goalsPieChartLoading && !isPieChartEmpty"
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
    <el-row :gutter="20">
      <el-col :span="4">
        <project-select v-model="projectId" @change="updateStatistics" />
      </el-col>
      <el-col :span="4">
        <el-date-picker
          v-model="dateRange"
          type="daterange"
          format="dd.MM.yyyy"
          range-separator="-"
          start-placeholder="Начальная дата"
          end-placeholder="Конечная дата"
          @change="updateStatistics"
        />
      </el-col>
    </el-row>
    <el-row v-loading="loading" :gutter="20">
      <el-col :span="8">
        <goal-completion :data="goalCompletion" />
      </el-col>
      <el-col :span="8">
        <workload-by-projects :data="workloadByProjects" />
      </el-col>
      <el-col :span="8">
        <participation-in-projects />
      </el-col>
    </el-row>
    <el-row v-loading="loading" :gutter="20">
      <el-col :span="24">
        <workload-by-days :data="workloadByDays" />
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import moment from 'moment'
import GoalsPieChart from '@/modules/goals/components/goals-pie-chart.vue'
import { Statistics } from '@/core/types/statistics.model'
import GoalsLineChart from '@/modules/goals/components/goals-line-chart.vue'
import GoalCompletion from '@/modules/users/components/user-statistics/goal-completion.vue'
import WorkloadByProjects from '@/modules/users/components/user-statistics/workload-by-projects.vue'
import WorkloadByDays from '@/modules/users/components/user-statistics/workload-by-days.vue'
import ParticipationInProjects from '@/modules/users/components/user-statistics/participation-in-projects.vue'
import ProjectSelect from '@/modules/projects/components/project-select.vue'
import { GoalCompletionStatistics } from '@/modules/users/models/goal-completion-statistics.interface'
import { WorkloadByProjectsStatistics } from '@/modules/users/models/workload-by-projects-statistics.interface'
import { WorkloadByDaysStatistics } from '@/modules/users/models/workload-by-days-statistics.interface'
import usersStore from '../../store/users.store'
import statisticsStore from '@/modules/users/store/statistics.store'

@Component({
  components: {
    ProjectSelect,
    ParticipationInProjects,
    WorkloadByDays,
    WorkloadByProjects,
    GoalCompletion,
    GoalsLineChart,
    GoalsPieChart,
  },
})
export default class UserStatistics extends Vue {
  private loading = false
  private projectId: number | null = null
  private dateRange: Date[] = [moment().subtract(1, 'month').toDate(), moment().toDate()]

  private get userId(): string {
    return this.$route.params.userId
  }

  private get goalCompletion(): GoalCompletionStatistics {
    // return statisticsStore.goalCompletion[this.userId]
    return {
      completedOnTime: 5,
      completedNotOnTime: 10,
      inProcess: 3,
      notCompleted: 1,
    }
  }

  private get workloadByProjects(): WorkloadByProjectsStatistics {
    // return statisticsStore.workloadByProjects[this.userId]
    return {
      totalHours: 100,
      projectHours: {
        ['ВГВ']: 28,
        ['Девон']: 29,
        ['ВВН']: 29,
      },
    }
  }

  private get workloadByDays(): WorkloadByDaysStatistics {
    return statisticsStore.workloadByDays[this.userId]
  }

  protected async mounted(): Promise<void> {
    await this.updateStatistics()
    await this.loadGoalsPieChart()
  }

  private async updateStatistics(): Promise<void> {
    this.loading = true
    await statisticsStore.getTotal({
      dateBegin: moment.utc(this.dateRange[0]).format(),
      dateEnd: moment.utc(this.dateRange[1]).format(),
      projectIds: this.projectId ? [this.projectId] : [],
      userIds: [this.userId],
    })
    this.loading = false
  }

  private goalsPieChartLoading = false
  private goalsPieChartData: number[] = []

  private goalsLineChartLoading = false
  private goalsLineChartData: { date: string; goalCountForState: number[] }[] = []
  private goalsLineChartDateRange: string[] = [
    moment.utc(moment().subtract('2', 'weeks')).format(),
    moment.utc(moment().add('1', 'day')).format(),
  ]

  private get isPieChartEmpty(): boolean {
    return !this.goalsPieChartData.some((n: number) => n > 0)
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

<style lang="scss">
.user-statistics {
  overflow-y: auto;
  .el-row {
    margin-bottom: 20px;
    &:last-child {
      margin-bottom: 0;
    }
  }
  .card {
    min-width: 350px;
    min-height: 350px;
    &__title {
      cursor: default;
      color: var(--text);
      font-size: 12px;
      font-weight: 600;
      letter-spacing: 0.3px;
      text-transform: uppercase;
      margin-bottom: 15px;
    }
  }
}
</style>
