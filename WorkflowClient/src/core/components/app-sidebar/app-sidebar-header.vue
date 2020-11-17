<template>
  <div class="sidebar-header">
    <AppSidebarAdd />
    <GoalWindow v-if="isGoalWindowOpened" @close="closeGoalWindow" />
    <ProjectWindow v-if="isProjectWindowOpened" @close="closeProjectWindow" />
    <GroupWindow v-if="isGroupWindowOpened" @close="closeGroupWindow" />
    <TeamDialog v-if="isTeamWindowOpened" @close="closeTeamWindow" />
    <UserDialog v-if="isUserWindowOpened" @close="closeUserWindow" />
    <SettingsWindow v-if="isSettingsOpened" @closed="closeSettings" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import goalsStore from '@/modules/goals/store/goals.store'
import projectsStore from '@/modules/projects/store/projects.store'
import groupsStore from '@/modules/groups/store/groups.store'
import teamsStore from '@/modules/teams/store/teams.store'
import usersStore from '@/modules/users/store/users.store'
import settingsStore from '@/modules/settings/store/settings.store'
import AppSidebarAdd from '@/core/components/app-sidebar/app-sidebar-add.vue'
import ProjectWindow from '@/modules/projects/components/project-window.vue'
import GroupWindow from '@/modules/groups/components/group-window.vue'
import GoalWindow from '@/modules/goals/components/goal-window/goal-window-new.vue'
import TeamDialog from '@/modules/teams/components/team-dialog.vue'
import UserDialog from '@/modules/users/components/user-dialog.vue'
import SettingsWindow from '@/modules/settings/components/settings-window.vue'

@Component({
  components: {
    AppSidebarAdd,
    GroupWindow,
    ProjectWindow,
    GoalWindow,
    UserDialog,
    TeamDialog,
    SettingsWindow,
  },
})
export default class AppSidebarHeader extends Vue {
  private get isGoalWindowOpened(): boolean {
    return goalsStore.isGoalWindowOpened
  }

  private get isProjectWindowOpened(): boolean {
    return projectsStore.isProjectWindowOpened
  }

  private get isGroupWindowOpened(): boolean {
    return groupsStore.isGroupWindowOpened
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

  private closeGoalWindow(): void {
    goalsStore.closeGoalWindow()
  }

  private closeProjectWindow(): void {
    projectsStore.closeProjectWindow()
  }

  private closeGroupWindow(): void {
    groupsStore.closeGroupWindow()
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
  padding: 13px 15px 15px;
  border-bottom: var(--input-border);
  margin: -15px -15px 15px;
  min-height: var(--header-height);
  transition: border-color 0.2s;
}
</style>
