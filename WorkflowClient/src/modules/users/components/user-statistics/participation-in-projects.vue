<template>
  <el-card class="card card-doughnut-chart" shadow="never">
    <div class="card__title">Статус участия в проектах</div>
    <base-chart-doughnut :data="chartPieData" :options="chartPieOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import BaseChartDoughnut from '@/core/components/base-chart/base-chart-doughnut.vue'

@Component({ components: { BaseChartDoughnut } })
export default class ParticipationInProjects extends Vue {
  @Prop() readonly data!: { date: string; goalCountForState: number[] }[]
  private chartPieData: ChartData = {}
  private chartPieOptions: ChartOptions = {}

  protected mounted(): void {
    console.log('goal-completion')
  }
}
</script>

<style lang="scss" scoped></style>
