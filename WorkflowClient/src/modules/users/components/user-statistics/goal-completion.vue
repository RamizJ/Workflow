<template>
  <el-card class="card card-doughnut-chart" shadow="never">
    <div class="card__title">Статистика выполнения задач</div>
    <base-chart-doughnut :chart-data="chartData" :chart-options="chartOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import BaseChartDoughnut from '@/core/components/base-chart/base-chart-doughnut.vue'
import { GoalsCompletion } from '@/modules/users/models/goal-completion-statistics.interface'

@Component({ components: { BaseChartDoughnut } })
export default class GoalCompletion extends Vue {
  @Prop() readonly data: GoalsCompletion | null = null
  private statisticsData: GoalsCompletion | null = null
  private chartData: ChartData = {}
  private chartOptions: ChartOptions = {}

  protected mounted(): void {
    this.refreshChart(this.data)
  }

  @Watch('data', { deep: true })
  refreshChart(data?: GoalsCompletion | null): void {
    this.statisticsData = data || null
    this.chartData = this.getChartData()
  }

  private getChartData(): ChartData {
    const valuesPcs: number[] = this.statisticsData ? Object.values(this.statisticsData) : []
    const totalPcs: number = valuesPcs.reduce((a, b) => a + b, 0)
    const valuesPercents: number[] = valuesPcs.map((value) => Math.round((value * 100) / totalPcs))
    const data: number[] = valuesPcs
    const labels: string[] = data.length
      ? ['Выполнены вовремя', 'Выполнены с отклонением', 'В процессе выполнения', 'Не выполнены']
      : []
    return { datasets: [{ backgroundColor: this.getChartDataColors(), data }], labels }
  }

  private getChartDataColors(): string[] {
    return ['#4E79CB', '#F47926', '#A4A4A4', '#FEC500']
  }
}
</script>

<style lang="scss" scoped></style>
