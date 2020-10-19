<template>
  <div class="project-overview">
    <el-row :gutter="20">
      <el-col :span="6">
        <el-card class="card" shadow="never">
          <div class="card__title">Задач выполнено</div>
          <div class="card__body">
            <div class="tasks-stats">{{ completedTasksCount }} из {{ totalTasksCount }}</div>
            <el-progress :percentage="progressPercentage || 0"></el-progress>
          </div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card class="card" shadow="never">
          <div class="card__title">Информация</div>
          <div class="card__body">
            <div>Руководитель проекта: {{ project.ownerFio }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

import projectModule from '@/modules/projects/store/projects.store'
import Project from '@/modules/projects/models/project.type'
import { Status } from '@/modules/goals/models/task.type'

@Component
export default class ProjectOverview extends Vue {
  @Prop() readonly data: Project | undefined

  private project: Project = {
    name: '',
    description: '',
    ownerId: '',
    ownerFio: '',
    teamIds: [],
  }
  private totalTasksCount = 0
  private completedTasksCount = 0

  private get progressPercentage(): number {
    const total: number = this.totalTasksCount
    const completed: number = this.completedTasksCount
    const result = Math.round((completed * 100) / total)
    if (result > 100) return 100
    else return result || 0
  }

  protected async mounted(): Promise<void> {
    this.project = { ...this.data } as Project
    if (this.project.id) {
      this.totalTasksCount = await projectModule.getTasksCount(this.project.id)
      this.completedTasksCount = await projectModule.getTasksCountByStatus({
        projectId: this.project.id,
        status: Status.Succeed,
      })
    }
  }
}
</script>

<style lang="scss" scoped>
.el-row {
  margin-right: 0 !important;
}
.card {
  &__title {
    cursor: default;
    color: var(--text);
    font-size: 12px;
    font-weight: 600;
    letter-spacing: 0.3px;
    text-transform: uppercase;
  }
  &__body {
    padding-top: 14px;
    font-size: 14px;
    line-height: 18px;
    color: var(--text);
  }
}
.tasks-stats {
  font-size: 22px;
  font-weight: 600;
  color: var(--color-primary);
  margin-bottom: 10px;
}
</style>

<style lang="scss">
.project-overview {
  .el-textarea__inner,
  .el-textarea__inner:hover,
  .el-textarea__inner:focus {
    background-color: transparent;
    padding: 0;
    border: none;
  }
}
</style>
