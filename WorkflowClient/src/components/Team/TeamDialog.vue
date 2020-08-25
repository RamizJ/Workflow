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
          <el-col
            v-if="teamMembersVisible || (form.userIds &amp;&amp; form.userIds.length)"
            :span="24"
          >
            <el-form-item>
              <el-select
                v-model="form.userIds"
                placeholder="Участники"
                :remote-method="searchUsers"
                multiple="multiple"
                filterable="filterable"
                remote="remote"
                clearable="clearable"
                default-first-option="default-first-option"
              >
                <el-option
                  v-for="item in userList"
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
            v-if="!$route.params.projectId &amp;&amp; (projectsVisible || (form.projectIds &amp;&amp; form.projectIds.length))"
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
                  v-for="item in projectList"
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
            v-if="!(form.userIds &amp;&amp; form.userIds.length)"
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
            v-if="!(form.projectIds &amp;&amp; form.projectIds.length)"
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
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import teamsModule from '@/store/modules/teams.module'
import DialogMixin from '@/mixins/dialog.mixin'
import BaseDialog from '@/components/BaseDialog.vue'
import Team from '@/types/team.type'
import Query from '@/types/query.type'

@Component({ components: { BaseDialog } })
export default class TeamDialog extends mixins(DialogMixin) {
  public form: Team = {
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

  private async mounted() {
    if (this.$route.params.projectId) {
      if (this.form.projectIds?.length)
        this.form.projectIds.push(parseInt(this.$route.params.projectId))
      else this.form.projectIds = [parseInt(this.$route.params.projectId)]
    }

    await this.searchUsers('')
    await this.searchProjects('')

    if (this.isEdit) {
      this.loading = true
      await teamsModule.findUsers({
        teamId: this.form.id,
        pageNumber: 0,
        pageSize: 10
      } as Query)
      await teamsModule.findProjects({
        teamId: this.form.id,
        pageNumber: 0,
        pageSize: 10
      } as Query)
      this.form.userIds = []
      for (const user of teamsModule.teamUsers) {
        if (user.id) this.form.userIds.push(user.id)
      }
      this.form.projectIds = []
      for (const project of teamsModule.teamProjects) {
        if (project.id) this.form.projectIds.push(project.id)
      }
      this.loading = false
    }
    ;(this.$refs.title as HTMLElement).focus()
  }
}
</script>
