<template>
  <el-card class="card daily-statistics" shadow="never">
    <div class="card__title">Статистика по дням</div>
    <el-date-picker
      v-model="range"
      type="daterange"
      format="dd.MM.yyyy"
      range-separator="-"
      start-placeholder="Начальная дата"
      end-placeholder="Конечная дата"
      @change="onDateRangeChange"
    >
    </el-date-picker>
    <chart-line
      class="chart-container"
      v-if="!loading"
      :data="chartPieData"
      :options="chartPieOptions"
    />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'

import ChartLine from '@/core/components/base-chart-line.vue'
import moment from 'moment'
import { ChartData, ChartDataSets, ChartOptions } from 'chart.js'

@Component({ components: { ChartLine } })
export default class ReportDailyStatistics extends Vue {
  @Prop() readonly data!: { date: string; goalCountForState: number[] }[]
  @Prop() readonly dateRange!: string[]

  private loading = true
  private statistics: { date: string; goalCountForState: number[] }[] = []
  private range: Date[] = [moment().subtract(1, 'week').toDate(), moment().toDate()]
  private chartPieData: ChartData = {}
  private chartPieOptions: ChartOptions = {
    maintainAspectRatio: false,
  }
  private colors: string[] = [
    'rgba(33, 150, 243, 0.5)',
    '#f1de33',
    'lightgrey',
    '#ff6d37',
    '#00cf3a',
    '#ca0000',
  ]

  protected mounted(): void {
    this.range = [moment(this.dateRange[0]).toDate(), moment(this.dateRange[1]).toDate()]
    this.statistics = [...this.data]
    this.renderChart()
  }

  @Watch('data')
  private onDataChanged(data: { date: string; goalCountForState: number[] }[]): void {
    this.statistics = [...data]
    this.renderChart()
  }

  @Watch('dateRange')
  private onRangeChanged(range: string[]): void {
    this.range = [moment(range[0]).toDate(), moment(range[1]).toDate()]
    this.renderChart()
  }

  private onDateRangeChange(dateRange: Date[]): void {
    const dateBegin: string = moment.utc(dateRange[0]).format()
    const dateEnd: string = moment.utc(dateRange[1]).format()
    this.$emit('rangeChange', [dateBegin, dateEnd])
  }

  private renderChart(): void {
    this.loading = true
    const labels: string[] = []
    for (let stat of this.statistics) {
      labels.push(moment(stat.date).format('DD.MM.YYYY'))
    }

    let datasets: ChartDataSets[] = []
    for (let i = 0; i <= 5; i++) {
      const data = this.statistics.map((item) => item.goalCountForState[i])
      const isNotEmpty = data.some((item) => item !== 0)
      if (isNotEmpty)
        datasets.push({
          label: this.getLabelByStatusIndex(i),
          borderColor: this.colors[i],
          data,
          fill: false,
        })
    }

    this.chartPieData = {
      datasets,
      labels,
    }
    this.loading = false
  }

  private getLabelByStatusIndex(index: number): string {
    switch (index) {
      case 0:
        return 'Новые'
      case 1:
        return 'Выполняется'
      case 2:
        return 'Отложено'
      case 3:
        return 'Проверяется'
      case 4:
        return 'Выполнено'
      case 5:
        return 'Отклонено'
      default:
        return ''
    }
  }

  private getRandomInt(max: number): number {
    return Math.floor(Math.random() * Math.floor(max))
  }
}
</script>

<style lang="scss" scoped>
.card {
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
</style>

<style lang="scss">
.daily-statistics {
  .el-input__inner:hover,
  .el-range-input:hover {
    background-color: transparent;
  }
  .el-range-editor {
    width: 290px;
  }
  .el-range-editor.is-active {
    border-color: transparent;
  }
}
</style>
