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
      :disabled="form.id && form.isRemoved"
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
                  v-for="item in teams"
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
            <unicon name="file-alt" />
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
            <unicon name="user-arrows" />
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
            <unicon name="save" />
          </el-button>
        </el-tooltip>
      </div>
    </template>
  </base-dialog>
</template>

<script lang="ts">
import { Component, Prop, Ref, Mixins } from 'vue-property-decorator'
import { ElForm } from 'element-ui/types/form'
import { Message } from 'element-ui'

import projectsModule from '@/modules/projects/store/projects.store'
import DialogMixin from '@/core/mixins/dialog.mixin'
import BaseDialog from '@/core/components/BaseDialog.vue'
import Project from '@/modules/projects/models/project.type'
import tableStore from '@/core/store/table.store'

@Component({ components: { BaseDialog } })
export default class ProjectDialog extends Mixins(DialogMixin) {
  @Prop() readonly id: number | undefined
  @Ref() readonly title?: HTMLInputElement

  public form: Project = {
    name: '',
    description: '',
    ownerId: '',
    ownerFio: '',
    creationDate: new Date(),
    teamIds: [],
    isRemoved: false,
  }
  private rules = {
    name: [{ required: true, message: '!', trigger: 'blur' }],
  }
  private descriptionVisible: boolean | null = null
  private teamsVisible: boolean | null = null

  protected async mounted(): Promise<void> {
    this.visible = true
    this.loading = true

    try {
      if (this.id) {
        const id: number = parseInt(this.id.toString())
        this.form = await projectsModule.findOneById(id)
        await projectsModule.findTeams({
          projectId: this.id,
          pageNumber: 0,
          pageSize: 10,
        })
        this.form.teamIds = []
        this.form.teamIds = projectsModule.projectTeams.map((team) => (team.id ? team.id : -1))
      } else if (projectsModule.project) {
        this.form = projectsModule.project
      }
    } catch (e) {
      this.$message.error('Не удаётся загрузить проект')
      this.loading = false
    }

    try {
      await this.searchTeams()
    } catch (e) {
      this.$message.error('Не удаётся подгрузить список команд')
      this.loading = false
    }

    this.loading = false
  }

  private async submit(): Promise<void> {
    const form = this.$refs.form as ElForm
    await form.validate(async (valid) => {
      if (valid) {
        await this.sendForm()
        tableStore.requireReload()
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
    const entity: Project = { ...this.form } as Project
    if (this.id) await projectsModule.updateOne(entity)
    else await projectsModule.createOne(entity)
    this.loading = false
  }

  public exit(): void {
    this.visible = false
    projectsModule.closeProjectWindow()
    this.$emit('close')
  }
}
</script>
