<template>
  <el-card class="card" shadow="never" v-if="!loading">
    <div class="card__title">Статистика по дням (в разработке)</div>
    <chart-line :data="chartPieData" :options="chartPieOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

import ChartLine from '@/components/Chart/ChartLine.vue'

@Component({ components: { ChartLine } })
export default class ReportDailyStatistics extends Vue {
  private chartPieData = {}
  private chartPieOptions = {}
  private loading = true

  private async mounted() {
    this.loading = true
    const projectId = parseInt(this.$route.params.projectId)
    if (!projectId) return
    const data: number[] = []
    this.loading = false
  }
}
</script>

<style lang="scss" scoped>
.card {
  &__title {
    color: var(--text);
    font-size: 12px;
    font-weight: 600;
    letter-spacing: 0.3px;
    text-transform: uppercase;
    margin-bottom: 15px;
  }
}
</style>
