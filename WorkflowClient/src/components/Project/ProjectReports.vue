<template>
  <div class="project-reports">
    <el-row :gutter="20">
      <el-col :span="8">
        <report-tasks-overview v-if="!tasksOverviewLoading" :data="tasksOverview" />
      </el-col>
      <el-col :span="16">
        <report-daily-statistics
          v-if="!dailyStatisticsLoading"
          :data="dailyStatistics"
          :date-range="dailyStatisticsRange"
          @rangeChange="onRangeChange"
        />
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import moment from 'moment'

import projectsModule from '@/store/modules/projects.module'
import ReportTasksOverview from '@/components/Report/ReportTasksOverview.vue'
import ReportDailyStatistics from '@/components/Report/ReportDailyStatistics.vue'
import { ProjectStatistics } from '@/types/project-statistics.type'

@Component({ components: { ReportDailyStatistics, ReportTasksOverview } })
export default class ProjectReports extends Vue {
  private tasksOverviewLoading = true
  private dailyStatisticsLoading = true

  private tasksOverview: number[] = []
  private dailyStatistics: { date: string; goalCountForState: number[] }[] = []
  private dailyStatisticsRange: string[] = [
    moment.utc(moment().subtract('2', 'weeks')).format(),
    moment.utc(moment()).format(),
  ]

  private get projectId(): number {
    return parseInt(this.$route.params.projectId)
  }

  protected async mounted(): Promise<void> {
    await this.loadTasksOverview()
    await this.loadDailyStatistics()
  }

  private async loadTasksOverview(): Promise<void> {
    const dateBegin: string = moment.utc(moment().subtract('5', 'years')).format()
    const dateEnd: string = moment.utc(moment()).format()
    this.tasksOverviewLoading = true
    const statistics: ProjectStatistics = await projectsModule.getStatistics({
      projectId: this.projectId,
      dateBegin,
      dateEnd,
    })
    this.tasksOverview = statistics.goalsCountForState
    this.tasksOverviewLoading = false
  }

  private async loadDailyStatistics(): Promise<void> {
    const dateBegin: string = this.dailyStatisticsRange[0]
    const dateEnd: string = this.dailyStatisticsRange[1]
    this.dailyStatisticsLoading = true
    const statistics: ProjectStatistics = await projectsModule.getStatistics({
      projectId: this.projectId,
      dateBegin,
      dateEnd,
    })
    this.dailyStatistics = statistics.byDateStatistics
    // console.log(JSON.stringify(this.dailyStatistics, null, 2))
    this.dailyStatisticsLoading = false
  }

  private async onRangeChange(dateRange: string[]): Promise<void> {
    this.dailyStatisticsRange = dateRange
    await this.loadDailyStatistics()
  }
}
</script>

<style lang="scss" scoped>
.el-row {
  margin-right: 0 !important;
}
</style>
