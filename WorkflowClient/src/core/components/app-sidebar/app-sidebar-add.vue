<template>
  <BasePopover popover-class="add-popover" transition="fade-left-top" title="Создать">
    <BasePopoverButton icon="edit-alt" @click="openGoalWindow">Задачу</BasePopoverButton>
    <BasePopoverButton icon="layer-group" @click="openProjectWindow">Проект</BasePopoverButton>
    <BasePopoverButton icon="user-arrows" @click="openTeamWindow">Команду</BasePopoverButton>
    <BasePopoverButton icon="users-alt" @click="openUserWindow">Пользователя</BasePopoverButton>
    <!--    <BasePopoverButton icon="cube">Область</BasePopoverButton>-->
    <el-button class="sidebar-add-button" slot="reference">
      <unicon name="plus-circle" />
      Создать
    </el-button>
  </BasePopover>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import projectsStore from '@/modules/projects/store/projects.store'
import goalsStore from '@/modules/goals/store/goals.store'
import teamsStore from '@/modules/teams/store/teams.store'
import usersStore from '@/modules/users/store/users.store'
import settingsStore from '@/modules/settings/store/settings.store'

import BasePopover from '@/core/components/base-popover/base-popover.vue'
import BasePopoverButton from '@/core/components/base-popover/base-popover-button.vue'
import IconButton from '@/core/components/base-icon-button.vue'
import ProjectDialog from '@/modules/projects/components/project-window.vue'
import GoalWindow from '@/modules/goals/components/goal-window/goal-window.vue'
import TeamDialog from '@/modules/teams/components/team-dialog.vue'
import UserDialog from '@/modules/users/components/user-dialog.vue'
import SettingsWindow from '@/modules/settings/components/settings-window.vue'
import GoalWindowNew from '@/modules/goals/components/goal-window/goal-window-new.vue'

@Component({
  components: {
    GoalWindowNew,
    GoalWindow,
    UserDialog,
    TeamDialog,
    ProjectDialog,
    SettingsWindow,
    IconButton,
    BasePopoverButton,
    BasePopover,
  },
})
export default class AppSidebarAdd extends Vue {
  private openProjectWindow(): void {
    projectsStore.openProjectWindow()
  }

  private openGoalWindow(): void {
    goalsStore.openGoalWindow()
  }

  private openTeamWindow(): void {
    teamsStore.openTeamWindow()
  }

  private openUserWindow(): void {
    usersStore.openUserWindow()
  }
}
</script>

<style lang="scss">
.add-popover {
  transform-origin: left top !important;
  top: 10px !important;
  left: 23px !important;
  box-shadow: 0 10px 20px 0 rgba(0, 0, 0, 0.2), 0 0 0 1px rgba(0, 0, 0, 0.05) !important;
  border-color: rgba(0, 0, 0, 0.07) !important;
  background-color: var(--background) !important;
}
.sidebar-add-button {
  width: 100%;
  background-color: var(--page-background) !important;
  font-size: 15px !important;
  font-weight: 600 !important;
  box-shadow: 0 0 0 1px rgba(0, 0, 0, 0.05), 0 1px 0 1px rgba(0, 0, 0, 0.05),
    0 4px 6px 0 rgba(0, 0, 0, 0.05);
  transition: background-color 0.15s !important;
  position: relative;
  svg {
    fill: var(--text);
    width: 20px;
    height: 20px;
    vertical-align: sub;
    position: absolute;
    top: 50%;
    left: 22px;
    transform: translate(-50%, -50%);
  }
  &:hover,
  &:focus {
    color: var(--text) !important;
    background-color: #f5f5f5 !important;
  }
}
</style>
