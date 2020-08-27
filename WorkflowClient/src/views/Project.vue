<template>
  <page v-loading="loading">
    <base-header v-if="projectItem.id">
      <template slot="title">
        <input
          class="title"
          v-model="projectItem.name"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="updateEntity"
        />
      </template>
      <template slot="action">
        <el-dropdown placement="bottom" :show-timeout="0">
          <el-button type="text" size="mini">
            <feather type="chevron-down" size="22"></feather>
          </el-button>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item>
              <el-button type="text" size="mini" @click="createTask">Создать задачу</el-button>
            </el-dropdown-item>
            <el-dropdown-item>
              <el-button type="text" size="mini" @click="addTeam">Добавить команду</el-button>
            </el-dropdown-item>
            <el-divider></el-divider>
            <el-dropdown-item>
              <el-button type="text" size="mini" @click="deleteEntity"
                >Переместить в корзину</el-button
              >
            </el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </template>
    </base-header>
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
      <el-tab-pane name="team" label="Команды">
        <project-teams ref="projectTeams" v-if="activeTab === 'team'"></project-teams>
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
  </page>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

import projectsModule from '@/store/modules/projects.module'
import Page from '@/components/Page.vue'
import BaseHeader from '@/components/BaseHeader.vue'
import ProjectOverview from '@/components/Project/ProjectOverview.vue'
import ProjectTasks from '@/components/Project/ProjectTasks.vue'
import ProjectTeams from '@/components/Project/ProjectTeams.vue'
import ProjectDialog from '@/components/Project/ProjectDialog.vue'
import ProjectAddTeamDialog from '@/components/Project/ProjectAddTeamDialog.vue'
import TaskDialog from '@/components/Task/TaskDialog.vue'
import TaskTable from '@/components/Task/TaskTable.vue'
import Project from '@/types/project.type'
import TeamTable from '@/components/Team/TeamTable.vue'

@Component({
  components: {
    Page,
    BaseHeader,
    ProjectOverview,
    ProjectTeams,
    ProjectTasks,
    ProjectDialog,
    ProjectAddTeamDialog,
    TaskDialog
  }
})
export default class ProjectPage extends Vue {
  private loading = true
  private searchQuery = ''
  private activeTab = 'overview'
  private tabs = [
    { value: 'overview', label: 'Общее' },
    { value: 'tasks', label: 'Задачи' },
    { value: 'team', label: 'Команда' },
    { value: 'history', label: 'История' }
  ]
  private projectItem: Project = {
    name: '',
    description: '',
    teamIds: []
  }
  private projectModalVisible = false
  private teamModalVisible = false
  private taskModalVisible = false

  private get id(): number {
    return parseInt(this.$route.params.projectId)
  }

  private async mounted() {
    this.loading = true
    this.loadTab()
    const project = await projectsModule.findOneById(this.id)
    this.projectItem = { ...project }
    this.loading = false
  }

  loadTab() {
    const query = { ...this.$route.query }
    query.tab = query.tab?.toString() || this.activeTab
    this.activeTab = query.tab
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) this.$router.push({ query })
  }

  setTab() {
    const query = { tab: this.activeTab }
    if (JSON.stringify({ tab: '' }) !== JSON.stringify(this.$route.query))
      this.$router.push({ query })
  }

  async editEntity() {
    this.projectModalVisible = true
  }

  async deleteEntity() {
    await projectsModule.deleteOne(this.id)
    await this.$router.push({ name: 'Project' })
  }

  async updateEntity() {
    await projectsModule.updateOne(this.projectItem)
  }

  async changeEntityDescription(value: string) {
    this.projectItem.description = value
    this.projectItem.teamIds = projectsModule.project?.teamIds
    await projectsModule.updateOne(this.projectItem)
  }

  async createTask() {
    const projectTasks = this.$refs.projectTasks as ProjectTasks
    if (projectTasks) (projectTasks.$refs.items as TaskTable).createEntity()
    else this.taskModalVisible = true
  }

  async addTeam() {
    this.teamModalVisible = true
  }

  onTeamAdd() {
    const projectTeams = this.$refs.projectTeams as ProjectTeams
    const projectTeamsTable = projectTeams.$refs.items as TeamTable
    projectTeamsTable.reloadData()
  }
}
</script>
