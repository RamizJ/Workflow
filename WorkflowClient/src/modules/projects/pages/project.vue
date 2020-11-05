<template>
  <BasePage>
    <BasePageHeader>
      <input
        v-if="!loading"
        v-model="projectItem.name"
        v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
        @change="updateEntity"
      />
      <ProjectActions
        v-if="!loading"
        slot="action"
        @add-goal="createTask"
        @add-team="addTeam"
        @delete-project="deleteEntity"
      />
    </BasePageHeader>
    <BasePageSubheader>
      <input
        placeholder="Описание..."
        v-model="projectItem.description"
        v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
        @change="updateEntity"
      />
    </BasePageSubheader>

    <BaseTabs v-model="currentTab" :tabs="tabs" @tab-click="setTab" />

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

import projectsStore from '@/modules/projects/store/projects.store'
import settingsModule from '@/modules/settings/store/settings.store'
import ProjectOverview from '@/modules/projects/components/project-overview.vue'
import ProjectTasks from '@/modules/projects/components/project-tasks.vue'
import ProjectTeams from '@/modules/projects/components/project-teams.vue'
import ProjectReports from '@/modules/projects/components/project-statistics.vue'
import ProjectDialog from '@/modules/projects/components/project-window.vue'
import ProjectAddTeamDialog from '@/modules/projects/components/project-add-team-window.vue'
import TaskDialog from '@/modules/goals/components/goal-window/goal-window.vue'
import TaskTable from '@/modules/goals/components/goal-table/goal-table-old.vue'
import Project from '@/modules/projects/models/project.type'
import TeamTable from '@/modules/teams/components/team-table-old.vue'
import ProjectUsers from '@/modules/projects/components/project-team-users.vue'
import BasePage from '@/core/components/base-page/base-page.vue'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import ProjectActions from '@/modules/projects/components/project-actions.vue'
import BasePageSubheader from '@/core/components/base-page/base-page-subheader.vue'
import BaseTabs from '@/core/components/base-tabs/base-tabs.vue'
import goalsStore from '@/modules/goals/store/goals.store'
import Goal from '@/modules/goals/models/goal.type'

@Component({
  components: {
    BaseTabs,
    BasePageSubheader,
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
  private currentTab = 'overview'
  private tabs: Array<{ label: string; name: string; component: any }> = [
    { label: 'Обзор', name: 'overview', component: ProjectOverview },
    { label: 'Задачи', name: 'goals', component: ProjectTasks },
    { label: 'Команды', name: 'teams', component: ProjectTeams },
    { label: 'Статистика', name: 'statistics', component: ProjectReports },
  ]
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
    ;(this as any).$insProgress.start()
    this.loadTab()
    const project = projectsStore.project || (await projectsStore.findOneById(this.id))
    this.projectItem = { ...project }
    projectsStore.setProject(project)
    ;(this as any).$insProgress.finish()
    this.loading = false
  }

  protected beforeDestroy(): void {
    projectsStore.setProject(null)
  }

  private loadTab() {
    const query = { ...this.$route.query }
    query.tab = query.tab?.toString() || this.currentTab
    this.currentTab = query.tab
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) this.$router.replace({ query })
  }

  private setTab() {
    const query = { tab: this.currentTab }
    if (JSON.stringify({ tab: '' }) !== JSON.stringify(this.$route.query))
      this.$router.replace({ query })
  }

  private async editEntity() {
    this.projectModalVisible = true
  }

  private async deleteEntity() {
    const allowDelete = await this.confirmDelete()
    if (!allowDelete) return
    await projectsStore.deleteOne(this.id)
    await this.$router.replace({ name: 'Project' })
  }

  private async updateEntity() {
    await projectsStore.updateOne(this.projectItem)
  }

  private async changeEntityDescription(value: string) {
    this.projectItem.description = value
    this.projectItem.teamIds = projectsStore.project?.teamIds
    await projectsStore.updateOne(this.projectItem)
  }

  private async createTask() {
    const goal = new Goal()
    goal.projectId = this.projectItem.id
    await goalsStore.openGoalWindow(goal)
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
