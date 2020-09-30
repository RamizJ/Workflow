<template>
  <el-card class="card" shadow="never" v-loading="loading">
    <div class="card__title">Статистика по дням (в разработке)</div>
    <el-date-picker
      v-model="dateRange"
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
import { Component, Vue } from 'vue-property-decorator'

import ChartLine from '@/components/Chart/ChartLine.vue'
import projectsModule from '@/store/modules/projects.module'
import { Status } from '@/types/task.type'
import moment from 'moment'

@Component({ components: { ChartLine } })
export default class ReportDailyStatistics extends Vue {
  private chartPieData = {}
  private chartPieOptions = {
    maintainAspectRatio: false,
    scales: {
      yAxes: [
        {
          stacked: true,
          gridLines: {
            display: true,
            color: 'rgba(255,99,132,0.2)',
          },
        },
      ],
      xAxes: [
        {
          gridLines: {
            display: false,
          },
        },
      ],
    },
  }
  private loading = true
  private dateRange: Date[] = [moment().subtract(1, 'week').toDate(), moment().toDate()]
  private colors: string[] = [
    '#FFCC33',
    '#CC9966',
    '#FF0066',
    '#6666CC',
    '#00CCFF',
    '#339999',
    '#66FFFF',
    '#00CC99',
    '#CCFF33',
    '#999966',
    '#999999',
  ]

  protected async mounted(): Promise<void> {
    this.loading = true
    const projectId = parseInt(this.$route.params.projectId)
    if (!projectId) return
    await this.renderChart()
    this.loading = false
  }

  private async onDateRangeChange(): Promise<void> {
    await this.renderChart()
  }

  private async renderChart(): Promise<void> {
    this.loading = true
    const dateFrom: Date = this.dateRange[0]
    const dateTo: Date = this.dateRange[1]
    const days = moment(dateTo).diff(moment(dateFrom), 'days')
    const labels: string[] = []
    let tempDate: Date = dateFrom
    for (let day = 0; day <= days; day++) {
      labels.push(moment(tempDate).format('DD.MM.YYYY'))
      tempDate = moment(tempDate).add(1, 'day').toDate()
    }

    // TODO: Fetch statistics from API

    const completedTasksDataset = {
      label: `Завершённые задачи`,
      borderColor: this.colors[this.getRandomInt(this.colors.length)],
      data: [
        this.getRandomInt(10),
        this.getRandomInt(10),
        this.getRandomInt(10),
        this.getRandomInt(10),
        this.getRandomInt(10),
      ],
      fill: false,
    }

    const createdTasksDataset = {
      label: `Созданные задачи`,
      borderColor: this.colors[this.getRandomInt(this.colors.length)],
      data: [
        this.getRandomInt(10),
        this.getRandomInt(10),
        this.getRandomInt(10),
        this.getRandomInt(10),
        this.getRandomInt(10),
      ],
      fill: false,
    }

    this.chartPieData = {
      datasets: [completedTasksDataset, createdTasksDataset],
      labels,
    }
    setTimeout(() => (this.loading = false), 1)
    // this.loading = false
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
