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
import { GoalCompletionStatistics } from '@/modules/users/models/goal-completion-statistics.interface'

@Component({ components: { BaseChartDoughnut } })
export default class GoalCompletion extends Vue {
  @Prop() readonly data!: GoalCompletionStatistics
  private statisticsData?: GoalCompletionStatistics
  private chartData: ChartData = {}
  private chartOptions: ChartOptions = {}

  protected mounted(): void {
    this.refreshChart(this.data)
  }

  @Watch('data', { deep: true })
  refreshChart(data: GoalCompletionStatistics): void {
    this.statisticsData = data
    this.chartData = this.getChartData()
  }

  private getChartData(): ChartData {
    const valuesPcs = this.statisticsData ? Object.values(this.statisticsData) : []
    const totalPcs = valuesPcs.reduce((a, b) => a + b, 0)
    const valuesPercents = valuesPcs.map((value) => Math.round((value * 100) / totalPcs))
    const data = valuesPcs
    const labels = [
      'Выполнены вовремя',
      'Выполнены с отклонением',
      'В процессе выполнения',
      'Не выполнены',
    ]
    return { datasets: [{ backgroundColor: this.getChartDataColors(), data }], labels }
  }

  private getChartDataColors(): string[] {
    return ['#4E79CB', '#F47926', '#A4A4A4', '#FEC500']
  }
}
</script>

<style lang="scss" scoped></style>
