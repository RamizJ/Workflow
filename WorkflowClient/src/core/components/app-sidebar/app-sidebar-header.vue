<template>
  <div class="sidebar-header">
    <AppSidebarAdd />

    <TeamDialog v-if="isTeamWindowOpened" @close="closeTeamWindow" />
    <UserDialog v-if="isUserWindowOpened" @close="closeUserWindow" />
    <ProjectWindow v-if="isProjectWindowOpened" @close="closeProjectWindow" />
    <GoalWindowNew v-if="isGoalWindowOpened" @close="closeGoalWindow" />
    <SettingsWindow v-if="isSettingsOpened" @closed="closeSettings" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import projectsStore from '@/modules/projects/store/projects.store'
import goalsStore from '@/modules/goals/store/goals.store'
import teamsStore from '@/modules/teams/store/teams.store'
import usersStore from '@/modules/users/store/users.store'
import settingsStore from '@/modules/settings/store/settings.store'
import AppSidebarAdd from '@/core/components/app-sidebar/app-sidebar-add.vue'
import GoalWindowNew from '@/modules/goals/components/goal-window/goal-window-new.vue'
import TeamDialog from '@/modules/teams/components/team-dialog.vue'
import UserDialog from '@/modules/users/components/user-dialog.vue'
import ProjectWindow from '@/modules/projects/components/project-window.vue'
import SettingsWindow from '@/modules/settings/components/settings-window.vue'

@Component({
  components: {
    SettingsWindow,
    ProjectWindow,
    UserDialog,
    TeamDialog,
    GoalWindowNew,
    AppSidebarAdd,
  },
})
export default class AppSidebarHeader extends Vue {
  private get isProjectWindowOpened(): boolean {
    return projectsStore.isProjectWindowOpened
  }

  private get isGoalWindowOpened(): boolean {
    return goalsStore.isGoalWindowOpened
  }

  private get isTeamWindowOpened(): boolean {
    return teamsStore.isTeamWindowOpened
  }

  private get isUserWindowOpened(): boolean {
    return usersStore.isUserWindowOpened
  }

  private get isSettingsOpened(): boolean {
    return settingsStore.isSettingsOpened
  }

  private closeProjectWindow(): void {
    projectsStore.closeProjectWindow()
  }

  private closeGoalWindow(): void {
    goalsStore.closeGoalWindow()
  }

  private closeTeamWindow(): void {
    teamsStore.closeTeamWindow()
  }

  private closeUserWindow(): void {
    usersStore.closeUserWindow()
  }

  private closeSettings(): void {
    settingsStore.closeSettings()
  }
}
</script>

<style lang="scss" scoped>
.sidebar-header {
  padding-bottom: 20px;
}
</style>
