<template>
  <div class="project-overview">
    <el-row :gutter="20">
      <el-col :span="10">
        <el-card class="card" shadow="never">
          <div class="card__title">Описание</div>
          <div class="card__body">
            <el-input
              v-model="project.description"
              :autosize="{ minRows: 2 }"
              type="textarea"
              placeholder="Подробно опишите ваш проект..."
              @change="$emit('description', project.description)"
            ></el-input>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="card" shadow="never">
          <div class="card__title">Задач выполнено</div>
          <div class="card__body">
            <div class="tasks-stats">{{ completedTasksCount }} из {{ totalTasksCount }}</div>
            <el-progress :percentage="progressPercentage"></el-progress>
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

import projectModule from '@/store/modules/projects.module'
import Project from '@/types/project.type'
import { Status } from '@/types/task.type'

@Component
export default class ProjectOverview extends Vue {
  @Prop() readonly data: Project | undefined

  private project: Project = {
    name: '',
    description: '',
    ownerId: '',
    ownerFio: '',
    teamIds: []
  }
  private totalTasksCount = 0
  private completedTasksCount = 0

  private get progressPercentage(): number {
    const total: number = this.totalTasksCount
    const completed: number = this.completedTasksCount
    const result = Math.round((completed * 100) / total)
    return result || 0
  }

  private async mounted() {
    this.project = { ...this.data } as Project
    if (this.project.id) {
      this.totalTasksCount = await projectModule.getTasksCount(this.project.id)
      this.completedTasksCount = await projectModule.getTasksCountByStatus({
        projectId: this.project.id,
        status: Status.Succeed
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
    font-size: 15px;
    font-weight: 600;
    color: var(--text);
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
