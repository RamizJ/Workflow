<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Команда</h1>
    <el-form
      slot="body"
      :model="form"
      :rules="rules"
      ref="form"
      v-loading="loading"
      @submit.native.prevent="submit"
    >
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item prop="name">
            <el-input ref="title" v-model="form.name" placeholder="Новая команда"></el-input>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col v-if="descriptionVisible || form.description" :span="24">
          <el-form-item prop="description">
            <el-input
              v-model="form.description"
              :autosize="{ minRows: 2 }"
              type="textarea"
              placeholder="Описание"
            ></el-input>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <transition name="fade">
          <el-col v-if="teamMembersVisible || (form.userIds && form.userIds.length)" :span="24">
            <el-form-item>
              <el-select
                v-model="form.userIds"
                placeholder="Участники"
                :remote-method="searchUsers"
                default-first-option
                filterable
                clearable
                multiple
                remote
              >
                <el-option
                  v-for="item in users"
                  :key="item.id"
                  :label="item.value"
                  :value="item.id"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </transition>
        <transition name="fade">
          <el-col
            v-if="
              !$route.params.projectId &&
                (projectsVisible || (form.projectIds && form.projectIds.length))
            "
            :span="24"
          >
            <el-form-item>
              <el-select
                v-model="form.projectIds"
                placeholder="Проекты"
                :remote-method="searchProjects"
                multiple="multiple"
                filterable="filterable"
                remote="remote"
                clearable="clearable"
                default-first-option="default-first-option"
              >
                <el-option
                  v-for="item in projects"
                  :key="item.id"
                  :label="item.value"
                  :value="item.id"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </transition>
      </el-row>
    </el-form>
    <template slot="footer">
      <div class="extra">
        <el-tooltip
          content="Описание"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            v-if="!form.description"
            type="text"
            @click="descriptionVisible = !descriptionVisible"
            circle="circle"
          >
            <feather type="align-left"></feather>
          </el-button>
        </el-tooltip>
        <el-tooltip
          content="Участники"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            size="small"
            v-if="!(form.userIds && form.userIds.length)"
            type="text"
            @click="teamMembersVisible = !teamMembersVisible"
            circle="circle"
          >
            <feather type="users"></feather>
          </el-button>
        </el-tooltip>
        <el-tooltip
          content="Проекты"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            size="small"
            v-if="!(form.projectIds && form.projectIds.length)"
            type="text"
            @click="projectsVisible = !projectsVisible"
            circle="circle"
          >
            <feather type="layers"></feather>
          </el-button>
        </el-tooltip>
      </div>
      <div class="send">
        <el-tooltip
          content="Сохранить"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button type="text" @click="submit" circle="circle">
            <feather type="arrow-right"></feather>
          </el-button>
        </el-tooltip>
      </div>
    </template>
  </base-dialog>
</template>

<script lang="ts">
import { Component, Prop, Ref } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'
import { ElForm } from 'element-ui/types/form'
import { Message } from 'element-ui'

import teamsModule from '@/store/modules/teams.module'
import DialogMixin from '@/mixins/dialog.mixin'
import BaseDialog from '@/components/BaseDialog.vue'
import Team from '@/types/team.type'
import Query from '@/types/query.type'
import usersModule from '@/store/modules/users.module'
import projectsModule from '@/store/modules/projects.module'

@Component({ components: { BaseDialog } })
export default class TeamDialog extends mixins(DialogMixin) {
  @Prop() readonly id: number | undefined
  @Ref() readonly title?: HTMLInputElement

  private isEdit = !!this.id

  private form: Team = {
    name: '',
    description: '',
    userIds: [],
    projectIds: []
  }
  private rules = {
    name: [{ required: true, message: '!', trigger: 'blur' }]
  }
  private descriptionVisible = null
  private teamMembersVisible = null
  private projectsVisible = null

  public get projects() {
    return teamsModule.teamProjects.map(project => {
      return {
        value: project.name,
        id: project.id
      }
    })
  }
  public get users() {
    return teamsModule.teamUsers.map(user => {
      return {
        value: `${user.lastName} ${user.firstName}`,
        id: user.id
      }
    })
  }

  async searchProjects(query = '') {
    if (!query && this.projects.length) return
    await teamsModule.findProjects({
      teamId: this.form.id,
      filter: query,
      pageNumber: 0,
      pageSize: 20
    } as Query)
  }

  async searchUsers(query = '') {
    if (!query && this.users.length) return
    await teamsModule.findUsers({
      teamId: this.form.id,
      filter: query,
      pageNumber: 0,
      pageSize: 20
    } as Query)
  }

  private async mounted() {
    this.visible = true

    if (this.$route.params.projectId) {
      if (this.form.projectIds?.length)
        this.form.projectIds.push(parseInt(this.$route.params.projectId))
      else this.form.projectIds = [parseInt(this.$route.params.projectId)]
    }

    this.loading = true
    if (this.id) {
      const id: number = parseInt(this.id.toString())
      this.form = await teamsModule.findOneById(id)

      await this.searchUsers()
      await this.searchProjects()

      this.form.userIds = []
      for (const user of this.users) {
        if (user.id) this.form.userIds.push(user.id)
      }
      this.form.projectIds = []
      for (const project of this.projects) {
        if (project.id) this.form.projectIds.push(project.id)
      }
    }
    this.loading = false
    ;(this.$refs.title as HTMLElement).focus()
  }

  async submit() {
    const form = this.$refs.form as ElForm
    await form.validate(async valid => {
      if (valid) {
        await this.sendForm()
        this.$emit('submit')
        this.exit()
      } else {
        Message({
          showClose: true,
          message: 'Форма заполнена некорректно',
          type: 'error'
        })
      }
    })
  }

  async sendForm() {
    this.loading = true
    const entity: Team = { ...this.form } as Team
    if (this.isEdit) await teamsModule.updateOne(entity)
    else await teamsModule.createOne(entity)
    this.loading = false
  }
}
</script>
