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
import { ProjectHours } from '@/modules/users/models/workload-by-projects-statistics.interface'

@Component({ components: { BaseChartDoughnut } })
export default class WorkloadByProjects extends Vue {
  @Prop() readonly data: ProjectHours | null = null
  private statisticsData: ProjectHours | null = null
  private chartData: ChartData = {}
  private chartOptions: ChartOptions = {}

  protected mounted(): void {
    this.refreshChart(this.data)
  }

  @Watch('data', { deep: true })
  refreshChart(data?: ProjectHours | null): void {
    this.statisticsData = data || null
    this.chartData = this.getChartData()
  }

  private getChartData(): ChartData {
    const statistics = this.getStatistics()
    const dataUnit: 'hours' | 'percentage' = 'hours'
    const dataValues: number[] = Array.from(statistics, ([key, value]) => value[dataUnit])
    const labelsValues: string[] = Array.from(statistics.keys())

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

    for (let projectData of this.statisticsData.hoursForProject) {
      const hours: number = projectData.hours
      const percentage: number = Math.round(
        (projectData.hours * 100) / this.statisticsData.totalHours
      )
      statistics.set(projectData.projectName, { hours, percentage })
    }

    const topHours: number[] = this.statisticsData.hoursForProject
      .map((item) => item.hours)
      .sort((a, b) => b - a)
      .slice(0, 3)

    for (let [key, value] of statistics) if (!topHours.includes(value.hours)) statistics.delete(key)

    const hoursValues: number[] = Array.from(statistics, ([key, value]) => value.hours)
    const percentageValues: number[] = Array.from(statistics, ([key, value]) => value.percentage)
    const hoursOther: number =
      this.statisticsData.totalHours - hoursValues.reduce((a, b) => a + b, 0)
    const percentageOther: number = 100 - percentageValues.reduce((a, b) => a + b, 0)
    statistics.set('Прочее', { hours: hoursOther, percentage: percentageOther })

    return statistics
  }
}
</script>

<style lang="scss" scoped></style>
