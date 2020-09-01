<template>
  <el-card class="card" shadow="never" v-if="!loading">
    <div class="card__title">Обзор задач</div>
    <chart-doughnut :data="chartPieData" :options="chartPieOptions" />
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

  private async mounted() {
    this.loading = true
    const projectId = parseInt(this.$route.params.projectId)
    if (!projectId) return
    const data: number[] = []
    const newTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.New
    })
    data.push(newTasks)
    const performTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Perform
    })
    data.push(performTasks)
    const testingTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Testing
    })
    data.push(testingTasks)
    const delayTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Delay
    })
    data.push(delayTasks)
    const succeedTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Succeed
    })
    data.push(succeedTasks)
    const rejectedTasks = await projectsModule.getTasksCountByStatus({
      projectId,
      status: Status.Rejected
    })
    data.push(rejectedTasks)

    const labels: string[] = [
      'Новые',
      'Выполняется',
      'Проверяется',
      'Отложено',
      'Выполнено',
      'Отклонено'
    ]

    const colors: string[] = [
      'rgba(33, 150, 243, 0.5)',
      '#f1de33',
      '#ff6d37',
      'lightgrey',
      '#00cf3a',
      '#ca0000'
    ]

    const borderColor: string[] = [
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)',
      'rgba(255,255,255,0.1)'
    ]

    this.chartPieData = {
      datasets: [
        {
          backgroundColor: colors,
          borderColor,
          data
        }
      ],
      labels
    }
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
