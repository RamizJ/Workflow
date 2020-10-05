<template>
  <div class="sidebar-add-new">
    <el-popover
      placement="top"
      trigger="manual"
      transition="fade"
      width="225"
      popper-class="add-popover"
      v-model="isPopoverVisible"
      v-click-outside="() => (isPopoverVisible = false)"
    >
      <div class="list">
        <!--<el-button class="list__item">
          <unicon name="cube" />
          Новая область
        </el-button>
        <div class="divider"></div>-->
        <el-button class="list__item" @click="openProjectDialog">
          <unicon name="layer-group" />
          Новый проект
        </el-button>
        <div class="divider"></div>
        <el-button class="list__item" @click="openTeamDialog">
          <unicon name="user-arrows" />
          Новая команда
        </el-button>
        <div class="divider"></div>
        <el-button class="list__item" @click="openUserDialog">
          <unicon name="users-alt" />
          Новый пользователь
        </el-button>
      </div>
      <el-button
        slot="reference"
        size="small"
        class="sidebar-button"
        plain
        @click="isPopoverVisible = true"
      >
        <unicon name="plus" />
      </el-button>
    </el-popover>

    <project-dialog v-if="isProjectDialogOpened" />
    <team-dialog v-if="isTeamDialogOpened" />
    <user-dialog v-if="isUserDialogOpened" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import projectsModule from '@/store/modules/projects.module'
import teamsModule from '@/store/modules/teams.module'
import usersModule from '@/store/modules/users.module'
import ProjectDialog from '@/components/Project/ProjectDialog.vue'
import TeamDialog from '@/components/Team/TeamDialog.vue'
import UserDialog from '@/components/User/UserDialog.vue'

@Component({
  components: {
    UserDialog,
    TeamDialog,
    ProjectDialog,
  },
})
export default class SidebarAddNew extends Vue {
  private isPopoverVisible = false

  private get isProjectDialogOpened(): boolean {
    return projectsModule.isProjectDialogOpened
  }
  private get isTeamDialogOpened(): boolean {
    return teamsModule.isTeamDialogOpened
  }
  private get isUserDialogOpened(): boolean {
    return usersModule.isUserDialogOpened
  }

  private openProjectDialog(): void {
    this.isPopoverVisible = false
    projectsModule.openProjectDialog()
  }

  private openTeamDialog(): void {
    this.isPopoverVisible = false
    teamsModule.openTeamDialog()
  }

  private openUserDialog(): void {
    this.isPopoverVisible = false
    usersModule.openUserDialog()
  }
}
</script>

<style lang="scss">
.add-popover {
  padding: 6px !important;
  .popper__arrow {
    display: none !important;
  }
  .list {
    display: flex;
    flex-direction: column;
    &__item {
      width: 100%;
      margin-left: 0;
      padding: 12px 15px;
      text-align: left;
      background-color: transparent;
      &:hover,
      &:focus {
        svg {
          fill: var(--button-hover-color);
        }
      }
      svg {
        fill: var(--text);
        width: 18px;
        height: 18px;
        margin-right: 5px;
        vertical-align: sub;
      }
    }
    .divider {
      margin: 4px;
      border-bottom: 1px solid var(--text);
      opacity: 0.08;
    }
  }
  .el-button + .el-button {
    margin-left: 0;
  }
}
.sidebar-button.el-button.is-plain {
  padding: 7px 6px 6px;
  &:hover {
    border-color: var(--sidebar-item-hover-background);
    background-color: transparent;
  }
  &:focus {
    border-color: var(--sidebar-item-hover-background);
    background-color: var(--sidebar-item-hover-background);
  }
  svg {
    height: 20px;
  }
}
</style>
