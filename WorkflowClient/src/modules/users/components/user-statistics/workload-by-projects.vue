<template>
  <el-card class="card card-doughnut-chart" shadow="never">
    <div class="card__title">Загрузка пользователя по проектам</div>
    <base-chart-doughnut :chart-data="chartData" :chart-options="chartOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import BaseChartDoughnut from '@/core/components/base-chart/base-chart-doughnut.vue'
import { WorkloadByProjectsStatistics } from '@/modules/users/models/workload-by-projects-statistics.interface'

@Component({ components: { BaseChartDoughnut } })
export default class WorkloadByProjects extends Vue {
  @Prop() readonly data!: WorkloadByProjectsStatistics
  private statisticsData?: WorkloadByProjectsStatistics
  private chartData: ChartData = {}
  private chartOptions: ChartOptions = {}

  protected mounted(): void {
    this.refreshChart(this.data)
  }

  @Watch('data', { deep: true })
  refreshChart(data: WorkloadByProjectsStatistics): void {
    this.statisticsData = data
    this.chartData = this.getChartData()
  }

  private getChartData(): ChartData {
    const statistics = this.getStatistics()
    const dataUnit: 'hours' | 'percentage' = 'percentage'
    const dataValues = Array.from(statistics, ([key, value]) => value[dataUnit])
    const labelsValues = Array.from(statistics.keys())

    return {
      datasets: [
        {
          backgroundColor: this.getChartDataColors(),
          data: dataValues,
        },
      ],
      labels: labelsValues,
    }
  }

  private getChartDataColors(): string[] {
    return ['#4E79CB', '#F47926', '#A4A4A4', '#FEC500']
  }

  private getStatistics(): Map<string, { hours: number; percentage: number }> {
    const statistics = new Map<string, { hours: number; percentage: number }>()
    if (!this.statisticsData) return statistics

    for (let projectName of Object.keys(this.statisticsData.projectHours)) {
      const hours = this.statisticsData.projectHours[projectName]
      const percentage =
        (this.statisticsData.projectHours[projectName] * 100) / this.statisticsData.totalHours
      statistics.set(projectName, { hours, percentage })
    }

    const hoursValues = Array.from(statistics, ([key, value]) => value.hours)
    const percentageValues = Array.from(statistics, ([key, value]) => value.percentage)
    const hoursOther = this.statisticsData.totalHours - hoursValues.reduce((a, b) => a + b, 0)
    const percentageOther = 100 - percentageValues.reduce((a, b) => a + b, 0)
    statistics.set('Прочее', { hours: hoursOther, percentage: percentageOther })

    return statistics
  }
}
</script>

<style lang="scss" scoped></style>
