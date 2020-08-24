<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Проект</h1>
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
            <el-input ref="title" v-model="form.name" placeholder="Новый проект"></el-input>
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
          <el-col v-if="teamsVisible || (form.teamIds && form.teamIds.length)" :span="24">
            <el-form-item prop="teamIds">
              <el-select
                v-model="form.teamIds"
                placeholder="Команды"
                :remote-method="searchTeams"
                multiple="multiple"
                filterable="filterable"
                remote="remote"
                clearable="clearable"
                default-first-option="default-first-option"
              >
                <el-option
                  v-for="item in teamList"
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
          content="Команды"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            v-if="!(form.teamIds && form.teamIds.length)"
            type="text"
            @click="teamsVisible = !teamsVisible"
            circle="circle"
          >
            <feather type="users"></feather>
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

import projectsModule from '@/store/modules/projects.module'
import DialogMixin from '@/mixins/dialog.mixin'
import Project from '@/types/project.type'

@Component
export default class ProjectDialog extends mixins(DialogMixin) {
  public form: Project = {
    name: '',
    description: '',
    ownerId: '',
    ownerFio: '',
    creationDate: new Date(),
    teamIds: []
  }
  private rules = {
    name: [{ required: true, message: '!', trigger: 'blur' }]
  }
  private descriptionVisible = null
  private teamsVisible = null

  private async mounted() {
    this.loading = true
    await this.searchTeams('')
    if (this.isEdit) {
      await projectsModule.findTeams({
        projectId: this.data.id,
        pageNumber: 0,
        pageSize: 10
      })
      this.form.teamIds = []
      this.form.teamIds = projectsModule.projectTeams.map(team => (team.id ? team.id : -1))
    }
    this.loading = false
  }
}
</script>
