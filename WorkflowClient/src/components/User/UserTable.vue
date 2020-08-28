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
          <a v-if="isRowEditable" @click.prevent="editEntity(child.data.row)">Изменить</a>
        </li>
        <el-divider v-if="isRowEditable"></el-divider>
        <li>
          <a v-if="isRowEditable && !$route.params.teamId" @click.prevent="createEntity"
            >Новый пользователь</a
          >
        </li>
        <li>
          <a v-if="isRowEditable && $route.params.teamId" @click.prevent="addUser"
            >Добавить участника</a
          >
        </li>
        <el-divider v-if="isRowEditable"></el-divider>
        <li>
          <a v-if="$route.params.teamId" @click.prevent="removeEntityFromTeam(child.data.row)"
            >Убрать из команды</a
          >
        </li>
        <li>
          <a
            v-if="isRowEditable && !$route.params.teamId"
            @click.prevent="deleteEntity(child.data.row, isMultipleSelected)"
            >Переместить в корзину</a
          >
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
      v-if="modalVisible"
      :id="modalData"
      @close="modalVisible = false"
      @submit="reloadData"
    ></user-dialog>
    <team-add-user-dialog
      v-if="modalAddUserVisible"
      @close="modalAddUserVisible = false"
      @submit="reloadData"
    ></team-add-user-dialog>
  </div>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'
import { StateChanger } from 'vue-infinite-loading'

import usersModule from '@/store/modules/users.module'
import teamsModule from '@/store/modules/teams.module'
import TableMixin from '@/mixins/table.mixin'
import UserDialog from '@/components/User/UserDialog.vue'
import TeamAddUserDialog from '@/components/Team/TeamAddUserDialog.vue'
import User from '@/types/user.type'
import Project from '@/types/project.type'

@Component({ components: { UserDialog, TeamAddUserDialog } })
export default class UserTable extends mixins(TableMixin) {
  private loading = false
  private modalAddUserVisible = false

  private async loadData($state: StateChanger) {
    const isFirstLoad = !this.data.length
    this.loading = isFirstLoad
    let data: User[]
    if (this.$route.params.teamId) data = await teamsModule.findUsers(this.query)
    else data = await usersModule.findAll(this.query)
    if (this.query.pageNumber !== undefined) this.query.pageNumber++
    if (data.length) $state.loaded()
    else $state.complete()
    this.data = isFirstLoad ? data : this.data.concat(data)
    this.loading = false
  }

  public createEntity() {
    this.modalData = undefined
    this.modalVisible = true
  }

  public editEntity(entity: Project) {
    this.modalData = entity.id
    this.modalVisible = true
  }

  private async deleteEntity(entity: User, multiple = false) {
    if (multiple) await usersModule.deleteMany(this.table.selection.map((item: User) => item.id))
    else await usersModule.deleteOne(entity.id as string)
    this.reloadData()
  }

  private async restoreEntity(entity: User, multiple = false) {
    if (multiple) await usersModule.restoreMany(this.table.selection.map((item: User) => item.id))
    else await usersModule.restoreOne(entity.id as string)
    this.reloadData()
  }

  private async removeEntityFromTeam(entity: User) {
    const teamId = parseInt(this.$route.params.teamId)
    const userId = entity.id!.toString()
    if (this.isMultipleSelected) {
      const userIds = this.table.selection.map((item: User) => item.id)
      await teamsModule.removeUsers({ teamId, userIds })
    } else await teamsModule.removeUser({ teamId, userId })
    this.reloadData()
  }

  private addUser() {
    this.modalAddUserVisible = true
  }
}
</script>
