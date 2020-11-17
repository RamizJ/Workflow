<template>
  <BasePopover popover-class="add-popover" transition="fade-left-top" title="Создать">
    <BasePopoverButton icon="edit-alt" @click="openGoalWindow">Задачу</BasePopoverButton>
    <BasePopoverButton icon="layer-group" @click="openProjectWindow">Проект</BasePopoverButton>
    <!--<BasePopoverButton icon="cube" @click="openGroupWindow">Область</BasePopoverButton>-->
    <BasePopoverButton icon="user-arrows" @click="openTeamWindow">Команду</BasePopoverButton>
    <BasePopoverButton icon="users-alt" @click="openUserWindow">Пользователя</BasePopoverButton>
    <el-button class="sidebar-add-button" slot="reference">
      <unicon name="plus-circle" />
      Создать
    </el-button>
  </BasePopover>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import goalsStore from '@/modules/goals/store/goals.store'
import projectsStore from '@/modules/projects/store/projects.store'
import groupsStore from '@/modules/groups/store/groups.store'
import teamsStore from '@/modules/teams/store/teams.store'
import usersStore from '@/modules/users/store/users.store'

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
  private openGoalWindow(): void {
    goalsStore.openGoalWindow()
  }

  private openProjectWindow(): void {
    projectsStore.openProjectWindow()
  }

  private openGroupWindow(): void {
    groupsStore.openGroupWindow()
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
  top: 0px !important;
  left: 14px !important;
  background-color: var(--sidebar-background) !important;
}
.sidebar-add-button {
  width: 100%;
  padding: 12px 20px !important;
  background-color: var(--main-background) !important;
  font-size: 14px !important;
  font-weight: 500 !important;
  border: var(--button-border) !important;
  position: relative;
  svg {
    fill: var(--text);
    width: 18px;
    height: 18px;
    vertical-align: sub;
    position: absolute;
    top: 50%;
    left: 20px;
    transform: translate(-50%, -50%);
  }
  &:hover,
  &:focus {
    color: var(--text) !important;
    border: var(--button-hover-border) !important;
  }
}
</style>
