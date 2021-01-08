<template>
  <el-card class="card card-doughnut-chart" shadow="never">
    <div class="card__title">Обзор</div>
    <div class="card__content">
      <div class="param">
        <div class="param__caption">Выполнено задач:</div>
        <div class="param__value">
          {{ completed }}
          <!--<span class="param__detail"> (на 5 больше)</span>-->
        </div>
      </div>
      <div class="param">
        <div class="param__caption">Выполнено в срок:</div>
        <div class="param__value">
          {{ completedOnTime }}
          <span class="param__detail">{{ completedNotOnTime }}</span>
        </div>
      </div>
      <div class="param">
        <div class="param__caption">Недельная загрузка:</div>
        <div class="param__value">--</div>
      </div>
    </div>
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import { GoalsCompletion } from '@/modules/users/models/goal-completion-statistics.interface'

@Component
export default class Overview extends Vue {
  @Prop() readonly data: GoalsCompletion | null = null
  private statisticsData: GoalsCompletion | null = null

  private get completed(): string {
    if (this.statisticsData) {
      const result = this.statisticsData.completedOnTime + this.statisticsData.completedNotOnTime
      return result.toString()
    } else return '--'
  }

  private get completedOnTime(): string {
    if (this.statisticsData) {
      return this.statisticsData.completedOnTime.toString()
    } else return '--'
  }

  private get completedNotOnTime(): string {
    if (this.statisticsData && this.statisticsData.completedNotOnTime > 0) {
      return `(${this.statisticsData.completedNotOnTime} вне срока)`
    } else return ''
  }

  protected mounted(): void {
    this.refreshData(this.data)
  }

  @Watch('data', { deep: true })
  refreshData(data?: GoalsCompletion | null): void {
    this.statisticsData = data || null
  }
}
</script>

<style lang="scss" scoped>
.card {
  &__content {
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
    .param {
      margin: 20px 0;
      &__caption {
        font-size: 20px;
        font-weight: 400;
        color: var(--text);
        margin-bottom: 10px;
      }
      &__value {
        font-size: 22px;
        font-weight: 700;
      }
      &__detail {
        font-size: 14px;
        font-weight: 500;
      }
    }
  }
}
</style>
