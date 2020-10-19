<template>
  <div class="table-container">
    <el-table
      ref="table"
      height="100%"
      v-loading="loading"
      :data="data"
      :row-class-name="setIndex"
      @select="onRowSelect"
      @row-click="onRowSingleClick"
      @row-contextmenu="onRowRightClick"
      @row-dblclick="onRowDoubleClick"
      highlight-current-row="highlight-current-row"
      border="border"
    >
      <el-table-column type="selection" width="42"></el-table-column>
      <el-table-column prop="lastName" label="Фамилия" width="150"></el-table-column>
      <el-table-column prop="firstName" label="Имя" width="150"></el-table-column>
      <el-table-column prop="middleName" label="Отчество" width="150"></el-table-column>
      <el-table-column prop="userName" label="Логин" width="150"></el-table-column>
      <el-table-column prop="email" label="Почта"></el-table-column>
      <el-table-column prop="phone" label="Телефон" width="120"></el-table-column>
      <el-table-column prop="position" label="Должность" width="180"></el-table-column>
      <infinite-loading
        slot="append"
        ref="loader"
        spinner="waveDots"
        :distance="300"
        @infinite="loadData"
        force-use-infinite-wrapper=".el-table__body-wrapper"
      >
        <div slot="no-more"></div>
        <div slot="no-results"></div>
      </infinite-loading>
    </el-table>
    <vue-context ref="contextMenu">
      <template slot-scope="child">
        <li>
          <a
            v-if="!$route.params.teamId && !$route.params.projectId"
            @click.prevent="editEntity(child.data.row)"
            >{{ isRowEditable ? 'Изменить' : 'Информация' }}</a
          >
        </li>
        <el-divider
          v-if="isRowEditable && !$route.params.teamId && !$route.params.projectId"
        ></el-divider>
        <li>
          <a
            v-if="isRowEditable && !$route.params.teamId && !$route.params.projectId"
            @click.prevent="createEntity"
            >Новый пользователь</a
          >
        </li>
        <li>
          <a v-if="isRowEditable && $route.params.teamId" @click.prevent="addUser"
            >Добавить участника</a
          >
        </li>
        <li>
          <a
            v-if="isRowEditable && $route.params.projectId"
            @click.prevent="editProjectUserRights(child.data.row)"
            >Изменить права</a
          >
        </li>
        <el-divider
          v-if="
            isRowEditable &&
            ((!$route.params.teamId && !$route.params.projectId) || $route.params.teamId)
          "
        ></el-divider>
        <li>
          <a v-if="$route.params.teamId" @click.prevent="removeEntityFromTeam(child.data.row)"
            >Убрать из команды</a
          >
        </li>
        <li>
          <a
            v-if="isRowEditable && !$route.params.teamId && !$route.params.projectId"
            @click.prevent="deleteEntity"
          >
            Переместить в корзину
          </a>
        </li>
        <li>
          <a
            v-if="!isRowEditable"
            @click.prevent="restoreEntity(child.data.row, isMultipleSelected)"
            >Восстановить</a
          >
        </li>
      </template>
    </vue-context>
    <user-dialog
      v-if="dialogVisible"
      :id="dialogData"
      @close="dialogVisible = false"
      @submit="reloadData"
    ></user-dialog>
    <team-add-user-dialog
      v-if="dialogAddUserVisible"
      @close="dialogAddUserVisible = false"
      @submit="reloadData"
    ></team-add-user-dialog>
    <project-edit-user-rights-dialog
      v-if="dialogEditUserRightsVisible"
      :user="selectedRow"
      :team-id="parseInt($route.params.teamId) || this.teamId"
      @close="dialogEditUserRightsVisible = false"
    />
  </div>
</template>

<script lang="ts">
import { Component, Mixins, Prop } from 'vue-property-decorator'
import { StateChanger } from 'vue-infinite-loading'

import usersModule from '@/modules/users/store/users.store'
import teamsModule from '@/modules/teams/store/teams.store'
import projectsModule from '@/modules/projects/store/projects.store'
import TableMixin from '@/core/mixins/table.mixin'
import UserDialog from '@/modules/users/components/user-dialog.vue'
import TeamAddUserDialog from '@/modules/teams/components/team-add-user-dialog.vue'
import ProjectEditUserRightsDialog from '@/modules/projects/components/project-edit-user-rights-window.vue'
import User from '@/modules/users/models/user.type'
import Project from '@/modules/projects/models/project.type'
import Entity from '@/core/types/entity.type'

@Component({ components: { ProjectEditUserRightsDialog, UserDialog, TeamAddUserDialog } })
export default class UserTable extends Mixins(TableMixin) {
  @Prop() readonly teamId: number | undefined

  public data: User[] = []
  private loading = false
  private dialogAddUserVisible = false
  private dialogEditUserRightsVisible = false

  private async loadData($state: StateChanger): Promise<void> {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    let data: User[] = []
    try {
      if (this.$route.params.teamId || this.teamId) {
        this.query.teamId = parseInt(this.$route.params.teamId) || this.teamId
        data = await teamsModule.findUsers(this.query)
      } else data = await usersModule.findAll(this.query)
    } catch (e) {
      this.$message.error('Не удаётся загрузить пользователей')
      this.loading = false
    }
    if (this.query.pageNumber !== undefined) this.query.pageNumber++
    if (data.length) $state.loaded()
    else $state.complete()
    this.data = isFirstLoad ? data : this.data.concat(data)
    this.loading = false
  }

  public createEntity(): void {
    this.dialogData = undefined
    this.dialogVisible = true
  }

  public editEntity(entity: Project): void {
    this.dialogData = entity.id
    this.dialogVisible = true
  }

  private async deleteEntity(): Promise<void> {
    const allowDelete = await this.confirmDelete()
    if (!allowDelete) return
    const entity = this.selectedRow as User
    if (this.isMultipleSelected) await usersModule.deleteMany(this.selectionIds as string[])
    else await usersModule.deleteOne(entity.id as string)
    this.reloadData()
  }

  private async restoreEntity(entity: User, multiple = false): Promise<void> {
    if (multiple) await usersModule.restoreMany(this.selectionIds as string[])
    else await usersModule.restoreOne(entity.id as string)
    this.reloadData()
  }

  private async removeEntityFromTeam(entity: User): Promise<void> {
    const allowDelete = await this.confirmDelete()
    if (!allowDelete) return
    const teamId = parseInt(this.$route.params.teamId)
    const userId = entity.id?.toString() || ''
    if (this.isMultipleSelected) {
      const userIds = this.selectionIds as string[]
      await teamsModule.removeUsers({ teamId, userIds })
    } else await teamsModule.removeUser({ teamId, userId })
    this.reloadData()
  }

  private addUser(): void {
    this.dialogAddUserVisible = true
  }

  private editProjectUserRights(): void {
    this.dialogEditUserRightsVisible = true
  }

  public onRowDoubleClick(row: Entity): void {
    if (this.$route.params.teamId) return
    if (this.$route.params.projectId) this.editProjectUserRights()
    else {
      this.dialogData = row.id
      this.dialogVisible = true
    }
  }
}
</script>
