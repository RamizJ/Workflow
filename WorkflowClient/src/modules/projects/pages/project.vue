<template>
  <BasePage>
    <BasePageHeader v-if="!loading">
      <input
        v-model="projectItem.name"
        v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
        @change="updateEntity"
      />
      <input
        slot="subtitle"
        placeholder="Описание"
        v-model="projectItem.description"
        v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
        @change="updateEntity"
      />
      <ProjectActions
        slot="action"
        @add-goal="createTask"
        @add-team="addTeam"
        @delete-project="deleteEntity"
      />
    </BasePageHeader>

    <el-tabs v-if="projectItem.id" ref="tabs" v-model="activeTab" @tab-click="setTab">
      <el-tab-pane name="overview" label="Обзор">
        <project-overview
          v-if="activeTab === 'overview'"
          :data="projectItem"
          @description="changeEntityDescription"
        ></project-overview>
      </el-tab-pane>
      <el-tab-pane name="tasks" label="Задачи">
        <project-tasks ref="projectTasks" v-if="activeTab === 'tasks'"></project-tasks>
      </el-tab-pane>
      <el-tab-pane name="teams" label="Команды">
        <project-teams ref="projectTeams" v-if="activeTab === 'teams'"></project-teams>
      </el-tab-pane>
      <el-tab-pane name="reports" label="Отчёты">
        <project-reports ref="projectReports" v-if="activeTab === 'reports'" />
      </el-tab-pane>
    </el-tabs>

    <project-dialog
      v-if="projectModalVisible"
      :data="projectItem"
      @close="projectModalVisible = false"
    ></project-dialog>
    <project-add-team-dialog
      v-if="teamModalVisible"
      @close="teamModalVisible = false"
      @submit="onTeamAdd"
    ></project-add-team-dialog>
    <task-dialog v-if="taskModalVisible" @close="taskModalVisible = false"></task-dialog>
  </BasePage>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { MessageBox } from 'element-ui'

import projectsModule from '@/modules/projects/store/projects.store'
import settingsModule from '@/modules/settings/store/settings.store'
import ProjectOverview from '@/modules/projects/components/project-overview.vue'
import ProjectTasks from '@/modules/projects/components/project-tasks.vue'
import ProjectTeams from '@/modules/projects/components/project-teams.vue'
import ProjectReports from '@/modules/projects/components/project-statistics.vue'
import ProjectDialog from '@/modules/projects/components/project-window.vue'
import ProjectAddTeamDialog from '@/modules/projects/components/project-add-team-window.vue'
import TaskDialog from '@/modules/goals/components/goal-window/goal-window.vue'
import TaskTable from '@/modules/goals/components/goal-table/goal-table.vue'
import Project from '@/modules/projects/models/project.type'
import TeamTable from '@/modules/teams/components/team-table-old.vue'
import ProjectUsers from '@/modules/projects/components/project-team-users.vue'
import BasePage from '@/core/components/base-page.vue'
import BasePageHeader from '@/core/components/base-page-header.vue'
import ProjectActions from '@/modules/projects/components/project-actions.vue'

@Component({
  components: {
    ProjectActions,
    BasePageHeader,
    BasePage,
    ProjectUsers,
    ProjectOverview,
    ProjectTasks,
    ProjectTeams,
    ProjectReports,
    ProjectDialog,
    ProjectAddTeamDialog,
    TaskDialog,
  },
})
export default class ProjectPage extends Vue {
  private loading = true
  private activeTab = 'overview'
  private projectItem: Project = {
    name: '',
    description: '',
    teamIds: [],
  }
  private projectModalVisible = false
  private teamModalVisible = false
  private taskModalVisible = false

  private get id(): number {
    return parseInt(this.$route.params.projectId)
  }

  protected async mounted(): Promise<void> {
    this.loading = true
    this.loadTab()
    const project = await projectsModule.findOneById(this.id)
    this.projectItem = { ...project }
    this.loading = false
  }

  private loadTab() {
    const query = { ...this.$route.query }
    query.tab = query.tab?.toString() || this.activeTab
    this.activeTab = query.tab
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) this.$router.replace({ query })
  }

  private setTab() {
    const query = { tab: this.activeTab }
    if (JSON.stringify({ tab: '' }) !== JSON.stringify(this.$route.query))
      this.$router.replace({ query })
  }

  private async editEntity() {
    this.projectModalVisible = true
  }

  private async deleteEntity() {
    const allowDelete = await this.confirmDelete()
    if (!allowDelete) return
    await projectsModule.deleteOne(this.id)
    await this.$router.replace({ name: 'Project' })
  }

  private async updateEntity() {
    await projectsModule.updateOne(this.projectItem)
  }

  private async changeEntityDescription(value: string) {
    this.projectItem.description = value
    this.projectItem.teamIds = projectsModule.project?.teamIds
    await projectsModule.updateOne(this.projectItem)
  }

  private async createTask() {
    const projectTasks = this.$refs.projectTasks as ProjectTasks
    if (projectTasks) (projectTasks.$refs.items as TaskTable).createEntity()
    else this.taskModalVisible = true
  }

  private async addTeam() {
    this.teamModalVisible = true
  }

  private onTeamAdd() {
    if (!this.$refs.projectTeams) return
    const projectTeams = this.$refs.projectTeams as ProjectTeams
    const projectTeamsTable = projectTeams.$refs.items as TeamTable
    projectTeamsTable.reloadData()
  }

  protected async confirmDelete(): Promise<boolean> {
    if (!settingsModule.confirmDelete) return true
    try {
      await MessageBox.confirm('Вы действительно хотите удалить элемент?', 'Предупреждение', {
        confirmButtonText: 'Удалить',
        cancelButtonText: 'Отменить',
        type: 'warning',
      })
      return true
    } catch (e) {
      return false
    }
  }
}
</script>
