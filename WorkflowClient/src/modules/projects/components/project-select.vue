<template>
  <el-select
    v-model="selectedProjectId"
    :placeholder="placeholder || 'Найти проект...'"
    :remote="true"
    :remote-method="doProjectSearch"
    :clearable="true"
    :filterable="true"
    :default-first-option="true"
    @change="onProjectChange"
  >
    <el-option
      v-for="item in projects"
      :key="item.id"
      :label="item.name"
      :value="item.id"
    ></el-option>
  </el-select>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import Project from '@/modules/projects/models/project.type'
import projectsStore from '../store/projects.store'

@Component
export default class ProjectSelect extends Vue {
  @Prop() private readonly placeholder?: string
  @Prop() private readonly value!: number
  private projects: Array<Project> = []
  private selectedProjectId: number | null = null

  protected async mounted(): Promise<void> {
    this.selectedProjectId = this.value
    await this.doProjectSearch()
  }

  private async doProjectSearch(searchText?: string): Promise<void> {
    this.projects = await projectsStore.findAll({
      filter: searchText,
      pageNumber: 0,
      pageSize: 10,
    })
  }

  private onProjectChange(projectId: number) {
    this.$emit('value', projectId)
    this.$emit('change', projectId)
  }
}
</script>

<style scoped></style>
