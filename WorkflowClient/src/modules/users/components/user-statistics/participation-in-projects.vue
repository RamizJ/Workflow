<template>
  <el-card class="card card-doughnut-chart" shadow="never">
    <div class="card__title">Статус участия в проектах</div>
    <base-chart-doughnut :data="chartData" :options="chartOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import BaseChartDoughnut from '@/core/components/base-chart/base-chart-doughnut.vue'

@Component({ components: { BaseChartDoughnut } })
export default class ParticipationInProjects extends Vue {
  @Prop() readonly data?: { date: string; goalCountForState: number[] }[]
  private statisticsData?: { date: string; goalCountForState: number[] }[]
  private chartData: ChartData = {}
  private chartOptions: ChartOptions = {}

  protected mounted(): void {
    this.refreshChart(this.data)
  }

  @Watch('data', { deep: true })
  refreshChart(data?: { date: string; goalCountForState: number[] }[]): void {
    this.statisticsData = data
    this.chartData = this.getChartData()
  }

  private getChartData(): ChartData {
    const data: number[] = []
    const labels: string[] = []
    return { datasets: [{ backgroundColor: this.getChartDataColors(), data }], labels }
  }

  private getChartDataColors(): string[] {
    return ['#4E79CB', '#F47926', '#A4A4A4', '#FEC500']
  }
}
</script>

<style lang="scss" scoped></style>
