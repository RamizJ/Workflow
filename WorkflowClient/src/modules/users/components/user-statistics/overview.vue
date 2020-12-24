<template>
  <el-card class="card card-doughnut-chart" shadow="never">
    <div class="card__title">Обзор</div>
    <div class="card__content">
      <div class="param">
        <div class="param__caption">Выполнено задач:</div>
        <div class="param__value">20<span class="param__detail"> (на 5 больше)</span></div>
      </div>
      <div class="param">
        <div class="param__caption">Выполнено в срок:</div>
        <div class="param__value">19<span class="param__detail"> (1 вне срока)</span></div>
      </div>
      <div class="param">
        <div class="param__caption">Недельная загрузка:</div>
        <div class="param__value">39,1 час</div>
      </div>
    </div>
  </el-card>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'

@Component
export default class Overview extends Vue {
  @Prop() readonly data?: { date: string; goalCountForState: number[] }[]
  private statisticsData?: { date: string; goalCountForState: number[] }[]

  protected mounted(): void {
    this.refreshData(this.data)
  }

  @Watch('data', { deep: true })
  refreshData(data?: { date: string; goalCountForState: number[] }[]): void {
    this.statisticsData = data
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
