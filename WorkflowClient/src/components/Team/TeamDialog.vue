<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Команда</h1>
    <el-form
      slot="body"
      ref="form"
      v-loading="loading"
      :model="form"
      :rules="rules"
      :disabled="form.id && form.isRemoved"
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
    <template v-if="!loading && (!form.id || !form.isRemoved)" slot="footer">
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

@Component({ components: { BaseDialog } })
export default class TeamDialog extends mixins(DialogMixin) {
  @Prop() readonly id: number | undefined
  @Ref() readonly title?: HTMLInputElement

  private form: Team = {
    name: '',
    description: '',
    userIds: [],
    projectIds: [],
    isRemoved: false,
  }
  private rules = {
    name: [{ required: true, message: '!', trigger: 'blur' }],
  }
  private descriptionVisible = null
  private teamMembersVisible = null
  private projectsVisible = null

  public get projects(): { value: string | undefined; id: number | undefined }[] {
    return teamsModule.teamProjects.map((project) => {
      return {
        value: project.name,
        id: project.id,
      }
    })
  }
  public get users(): { value: string | undefined; id: string | undefined }[] {
    return teamsModule.teamUsers.map((user) => {
      return {
        value: `${user.lastName} ${user.firstName}`,
        id: user.id,
      }
    })
  }

  public async searchProjects(query = ''): Promise<void> {
    if (!query && this.projects.length) return
    await teamsModule.findProjects({
      teamId: this.form.id,
      filter: query,
      pageNumber: 0,
      pageSize: 20,
    } as Query)
  }

  public async searchUsers(query = ''): Promise<void> {
    if (!query && this.users.length) return
    await teamsModule.findUsers({
      teamId: this.form.id,
      filter: query,
      pageNumber: 0,
      pageSize: 20,
    } as Query)
  }

  protected async mounted(): Promise<void> {
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

  private async submit(): Promise<void> {
    const form = this.$refs.form as ElForm
    await form.validate(async (valid) => {
      if (valid) {
        await this.sendForm()
        this.$emit('submit')
        this.exit()
      } else {
        Message({
          showClose: true,
          message: 'Форма заполнена некорректно',
          type: 'error',
        })
      }
    })
  }

  private async sendForm(): Promise<void> {
    this.loading = true
    const entity: Team = { ...this.form } as Team
    if (this.id) await teamsModule.updateOne(entity)
    else await teamsModule.createOne(entity)
    this.loading = false
  }

  public exit(): void {
    this.visible = false
    teamsModule.closeTeamDialog()
    this.$emit('close')
  }
}
</script>
