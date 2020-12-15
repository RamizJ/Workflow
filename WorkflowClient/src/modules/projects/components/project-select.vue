<template>
  <el-select
    v-bind="$attrs"
    v-model="selected"
    :placeholder="placeholder || 'Найти проект...'"
    :remote="true"
    :remote-method="doProjectSearch"
    :clearable="true"
    :filterable="true"
    :default-first-option="true"
    :loading="loading"
    :class="fullWidth ? 'full-width' : ''"
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
  @Prop() private readonly value!: number | number[]
  @Prop() private readonly fullWidth: boolean = false
  private loading = false
  private projects: Project[] = []
  private selected: number | number[] | null = null

  protected async mounted(): Promise<void> {
    this.selected = this.value
    await this.doProjectSearch()
  }

  private async doProjectSearch(searchText?: string): Promise<void> {
    this.loading = true
    this.projects = await projectsStore.findAll({
      filter: searchText,
      pageNumber: 0,
      pageSize: 10,
    })
    this.loading = false
  }

  private onProjectChange(selected: number | number[]) {
    this.$emit('value', selected)
    let selectedProjects =
      this.$attrs.multiple !== undefined
        ? this.projects.filter((project) => (selected as number[]).includes(project.id || 0))
        : this.projects.find((project) => project.id === (selected as number))
    this.$emit('change', selectedProjects)
  }
}
</script>

<style lang="scss" scoped>
.full-width {
  width: 100%;
}
</style>
