<template>
  <el-card class="card card-bar-chart" shadow="never">
    <div class="card__title">Загрузка пользователя по дням</div>
    <base-chart-bar :chart-data="chartData" :chart-options="chartOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import BaseChartBar from '@/core/components/base-chart/base-chart-bar.vue'
import { WorkloadByDaysStatistics } from '@/modules/users/models/workload-by-days-statistics.interface'

@Component({ components: { BaseChartBar } })
export default class WorkloadByDays extends Vue {
  @Prop() readonly data?: WorkloadByDaysStatistics
  private statisticsData?: WorkloadByDaysStatistics
  private chartData: ChartData = {}
  private chartOptions: ChartOptions = {
    maintainAspectRatio: false,
  }

  protected mounted(): void {
    this.refreshChart(this.data)
  }

  @Watch('data', { deep: true })
  refreshChart(data?: WorkloadByDaysStatistics): void {
    this.statisticsData = data
    this.chartData = this.getChartData()
  }

  private getChartData(): ChartData {
    const labels: string[] = []
    return { datasets: [], labels }
  }

  private getChartDataColors(): string[] {
    return ['#4E79CB', '#F47926', '#A4A4A4', '#FEC500']
  }
}
</script>

<style lang="scss" scoped></style>
