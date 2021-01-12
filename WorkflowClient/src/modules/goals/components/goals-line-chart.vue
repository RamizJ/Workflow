<template>
  <el-card class="card card-line-chart" shadow="never">
    <div class="card__title">{{ title || 'Статистика по дням' }}</div>
    <div class="card-toolbar">
      <ProjectSelect v-if="withProject" v-model="selectedProjectId" @change="emitProject" />
      <el-date-picker
        v-model="range"
        type="daterange"
        format="dd.MM.yyyy"
        range-separator="-"
        start-placeholder="Начальная дата"
        end-placeholder="Конечная дата"
        @change="onDateRangeChange"
      />
    </div>
    <ChartLine
      class="chart-container"
      v-if="!loading"
      :chart-data="chartPieData"
      :chart-options="chartPieOptions"
    />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import ChartLine from '@/core/components/base-chart/base-chart-line.vue'
import moment from 'moment'
import { ChartData, ChartDataSets, ChartOptions } from 'chart.js'
import ProjectSelect from '@/modules/projects/components/project-select.vue'

@Component({ components: { ProjectSelect, ChartLine } })
export default class GoalsLineChart extends Vue {
  @Prop() readonly title?: string
  @Prop() readonly data!: { date: string; goalsCountForState: number[] }[]
  @Prop() readonly dateRange!: string[]
  @Prop() readonly projectId?: number | null
  @Prop() readonly withProject?: boolean

  private loading = true
  private statistics: { date: string; goalsCountForState: number[] }[] = []
  private selectedProjectId: number | null = null
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

  protected async mounted(): Promise<void> {
    this.range = [moment(this.dateRange[0]).toDate(), moment(this.dateRange[1]).toDate()]
    this.statistics = [...this.data]
    this.selectedProjectId = this.projectId || null
    await this.renderChart()
  }

  @Watch('projectId')
  private async onProjectChanged(value: number | null): Promise<void> {
    this.selectedProjectId = value
    await this.renderChart()
  }

  @Watch('data')
  private async onDataChanged(
    data: { date: string; goalsCountForState: number[] }[]
  ): Promise<void> {
    this.statistics = [...data]
    await this.renderChart()
  }

  @Watch('dateRange')
  private async onRangeChanged(range: string[]): Promise<void> {
    this.range = [moment(range[0]).toDate(), moment(range[1]).toDate()]
    await this.renderChart()
  }

  private emitProject(id: number): void {
    this.$emit('project-change', id)
  }

  private onDateRangeChange(dateRange: Date[]): void {
    const dateBegin: string = moment.utc(dateRange[0]).format()
    const dateEnd: string = moment.utc(dateRange[1]).format()
    this.$emit('range-change', [dateBegin, dateEnd])
  }

  private async renderChart(): Promise<void> {
    this.loading = true
    const labels: string[] = []
    for (let stat of this.statistics) {
      labels.push(moment(stat.date).format('DD.MM.YYYY'))
    }

    let datasets: ChartDataSets[] = []
    for (let i = 0; i <= 5; i++) {
      const data = this.statistics.map((item) => item.goalsCountForState[i])
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
    await new Promise((resolve) => setTimeout(() => resolve(), 10))
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
.card-line-chart {
  .card-toolbar > div {
    margin-right: 20px;
    margin-bottom: 15px;
  }
  .el-range-input:hover {
    background-color: transparent;
    border: none;
    box-shadow: none;
  }
  .el-range-editor {
    width: 290px;
  }
  .el-range-editor.is-active {
    border: var(--input-focus-border);
  }
}
</style>
