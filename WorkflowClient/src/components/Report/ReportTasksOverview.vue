<template>
  <el-card class="card" shadow="never" v-loading="loading">
    <div class="card__title">Обзор задач</div>
    <chart-doughnut v-if="!loading" :data="chartPieData" :options="chartPieOptions" />
  </el-card>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

import projectsModule from '@/store/modules/projects.module'
import ChartDoughnut from '@/components/Chart/ChartDoughnut.vue'
import { Status } from '@/types/task.type'

@Component({ components: { ChartDoughnut } })
export default class ReportTasksOverview extends Vue {
  private chartPieData = {}
  private chartPieOptions = {}
  private loading = true

  protected async mounted(): Promise<void> {
    this.loading = true
    const projectId = parseInt(this.$route.params.projectId)
    if (!projectId) return
    const data: number[] = []
    const labels: string[] = []
    const newTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.New,
    })
    if (newTasks) {
      data.push(newTasks)
      labels.push('Новые')
    }
    const performTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Perform,
    })
    if (performTasks) {
      data.push(performTasks)
      labels.push('Выполняется')
    }
    const testingTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Testing,
    })
    if (testingTasks) {
      data.push(testingTasks)
      labels.push('Проверяется')
    }
    const delayTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Delay,
    })
    if (delayTasks) {
      data.push(delayTasks)
      labels.push('Отложено')
    }
    const succeedTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Succeed,
    })
    if (succeedTasks) {
      data.push(succeedTasks)
      labels.push('Выполнено')
    }
    const rejectedTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Rejected,
    })
    if (rejectedTasks) {
      data.push(rejectedTasks)
      labels.push('Отклонено')
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
