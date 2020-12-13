<template>
  <el-card class="card card-bar-chart" shadow="never">
    <div class="card__title">Загрузка пользователя по дням</div>
    <base-chart-bar :chart-data="chartPieData" :chart-options="chartPieOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import BaseChartBar from '@/core/components/base-chart/base-chart-bar.vue'
import { WorkloadByDaysStatistics } from '@/modules/users/models/workload-by-days-statistics.interface'

@Component({ components: { BaseChartBar } })
export default class WorkloadByDays extends Vue {
  @Prop() readonly data!: WorkloadByDaysStatistics
  private chartPieData: ChartData = {}
  private chartPieOptions: ChartOptions = {}

  protected mounted(): void {
    console.log('workload-by-days')
    console.log(JSON.stringify(this.data, null, 2))
  }
}
</script>

<style lang="scss" scoped></style>
