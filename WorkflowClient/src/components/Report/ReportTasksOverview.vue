<template>
  <el-card class="card" shadow="never">
    <div class="card__title">Обзор задач</div>
    <chart-doughnut v-if="!loading" :data="chartPieData" :options="chartPieOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import ChartDoughnut from '@/components/Chart/ChartDoughnut.vue'
import { ChartData, ChartOptions } from 'chart.js'

@Component({ components: { ChartDoughnut } })
export default class ReportTasksOverview extends Vue {
  @Prop() readonly data!: number[]

  private loading = true
  private chartPieData: ChartData = {}
  private chartPieOptions: ChartOptions = {}

  protected mounted(): void {
    this.loading = true
    const data: number[] = [...this.data]
    const labels: string[] = [
      'Новые',
      'Выполняется',
      'Отложено',
      'Проверяется',
      'Выполнено',
      'Отклонено',
    ]

    const emptyIndexes: number[] = []
    this.data.forEach((value, index) => {
      if (!value) emptyIndexes.push(index)
    })
    for (let i = emptyIndexes.length - 1; i >= 0; i--) {
      data.splice(emptyIndexes[i], 1)
      labels.splice(emptyIndexes[i], 1)
    }

    const colors: string[] = [
      'rgba(33, 150, 243, 0.5)',
      '#f1de33',
      '#ff6d37',
      'lightgrey',
      '#00cf3a',
      '#ca0000',
    ]

    const borderColor: string[] = [
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
    ]

    this.chartPieData = {
      datasets: [
        {
          backgroundColor: colors,
          borderColor,
          data,
        },
      ],
      labels,
    }
    this.loading = false
  }
}
</script>

<style lang="scss" scoped>
.card {
  min-width: 350px;
  min-height: 350px;
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
