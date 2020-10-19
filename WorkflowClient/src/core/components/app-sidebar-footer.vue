<template>
  <div class="sidebar-footer">
    <Popover>
      <PopoverButton icon="cube">Новая область</PopoverButton>
      <PopoverButton icon="layer-group" @click="openProjectDialog">Новый проект</PopoverButton>
      <PopoverButton icon="user-arrows" @click="openTeamDialog">Новая команда</PopoverButton>
      <PopoverButton icon="users-alt" @click="openUserDialog">Новый пользователь</PopoverButton>
      <IconButton slot="reference" icon="plus" />
    </Popover>
    <IconButton icon="sliders-v-alt" @click="openSettings" />

    <ProjectDialog v-if="isProjectDialogOpened" @close="closeProjectDialog" />
    <TeamDialog v-if="isTeamDialogOpened" @close="closeTeamDialog" />
    <UserDialog v-if="isUserDialogOpened" @close="closeUserDialog" />
    <SettingsWindow v-if="isSettingsOpened" @closed="closeSettings" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Popover from '@/core/components/base-popover.vue'
import PopoverButton from '@/core/components/base-popover-button.vue'
import IconButton from '@/core/components/base-icon-button.vue'
import ProjectDialog from '@/modules/projects/components/project-window.vue'
import TeamDialog from '@/modules/teams/components/team-dialog.vue'
import UserDialog from '@/modules/users/components/user-dialog.vue'
import SettingsWindow from '@/modules/settings/components/settings-window.vue'

import projectsModule from '@/modules/projects/store/projects.store'
import teamsModule from '@/modules/teams/store/teams.store'
import usersModule from '@/modules/users/store/users.store'
import settingsModule from '@/modules/settings/store/settings.store'

@Component({
  components: {
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
  private get isProjectDialogOpened(): boolean {
    return projectsModule.isProjectDialogOpened
  }

  private get isTeamDialogOpened(): boolean {
    return teamsModule.isTeamDialogOpened
  }

  private get isUserDialogOpened(): boolean {
    return usersModule.isUserDialogOpened
  }

  private get isSettingsOpened(): boolean {
    return settingsModule.isSettingsOpened
  }

  private openProjectDialog(): void {
    projectsModule.openProjectDialog()
  }

  private closeProjectDialog(): void {
    projectsModule.closeProjectDialog()
  }

  private openTeamDialog(): void {
    teamsModule.openTeamDialog()
  }

  private closeTeamDialog(): void {
    teamsModule.closeTeamDialog()
  }

  private openUserDialog(): void {
    usersModule.openUserDialog()
  }

  private closeUserDialog(): void {
    usersModule.closeUserDialog()
  }

  private openSettings(): void {
    settingsModule.openSettings()
  }

  private closeSettings(): void {
    settingsModule.closeSettings()
  }
}
</script>

<style lang="scss" scoped>
.sidebar-footer {
  display: flex;
  justify-content: space-between;
}
</style>
