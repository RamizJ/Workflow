<template>
  <div class="sidebar-footer">
    <Popover>
      <PopoverButton icon="cube">Новая область</PopoverButton>
      <PopoverButton icon="user-arrows" @click="openTeamWindow">Новая команда</PopoverButton>
      <PopoverButton icon="users-alt" @click="openUserWindow">Новый пользователь</PopoverButton>
      <PopoverButton icon="layer-group" @click="openProjectWindow">Новый проект</PopoverButton>
      <PopoverButton icon="edit" @click="openGoalWindow">Новый раздел</PopoverButton>
      <PopoverButton icon="edit-alt" @click="openGoalWindow">Новая задача</PopoverButton>
      <IconButton slot="reference" icon="plus" />
    </Popover>
    <IconButton icon="sliders-v-alt" @click="openSettings" />

    <ProjectDialog v-if="isProjectWindowOpened" @close="closeProjectWindow" />
    <GoalWindow v-if="isGoalWindowOpened" @close="closeGoalWindow" />
    <TeamDialog v-if="isTeamWindowOpened" @close="closeTeamWindow" />
    <UserDialog v-if="isUserWindowOpened" @close="closeUserWindow" />
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

import Popover from '@/core/components/base-popover/base-popover.vue'
import PopoverButton from '@/core/components/base-popover/base-popover-button.vue'
import IconButton from '@/core/components/base-icon-button.vue'
import ProjectDialog from '@/modules/projects/components/project-window.vue'
import GoalWindow from '@/modules/goals/components/goal-window.vue'
import TeamDialog from '@/modules/teams/components/team-dialog.vue'
import UserDialog from '@/modules/users/components/user-dialog.vue'
import SettingsWindow from '@/modules/settings/components/settings-window.vue'

@Component({
  components: {
    GoalWindow,
    UserDialog,
    TeamDialog,
    ProjectDialog,
    SettingsWindow,
    IconButton,
    PopoverButton,
    Popover,
  },
})
export default class SidebarFooter extends Vue {
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

  private openProjectWindow(): void {
    projectsStore.openProjectWindow()
  }
  private closeProjectWindow(): void {
    projectsStore.closeProjectWindow()
  }

  private openGoalWindow(): void {
    goalsStore.openGoalWindow()
  }
  private closeGoalWindow(): void {
    goalsStore.closeGoalWindow()
  }

  private openTeamWindow(): void {
    teamsStore.openTeamWindow()
  }
  private closeTeamWindow(): void {
    teamsStore.closeTeamWindow()
  }

  private openUserWindow(): void {
    usersStore.openUserWindow()
  }
  private closeUserWindow(): void {
    usersStore.closeUserWindow()
  }

  private openSettings(): void {
    settingsStore.openSettings()
  }
  private closeSettings(): void {
    settingsStore.closeSettings()
  }
}
</script>

<style lang="scss" scoped>
.sidebar-footer {
  display: flex;
  justify-content: space-between;
}
</style>
