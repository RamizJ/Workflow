<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Добавить проект</h1>
    <el-form
      slot="body"
      :model="form"
      ref="form"
      v-loading="loading"
      @submit.native.prevent="submit"
    >
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item>
            <el-select
              ref="input"
              v-model="form.projectId"
              placeholder="Найти проект..."
              :remote-method="searchProjects"
              filterable="filterable"
              remote="remote"
              clearable="clearable"
              default-first-option="default-first-option"
            >
              <el-option
                v-for="item in projectsToAdd"
                :key="item.id"
                :label="item.value"
                :value="item.id"
              ></el-option>
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template slot="footer">
      <div class="extra"></div>
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
import { Component, Mixins } from 'vue-property-decorator'
import { Input } from 'element-ui'
import DialogMixin from '@/core/mixins/dialog.mixin'
import BaseDialog from '@/core/components/BaseDialog.vue'
import tableStore from '@/core/store/table.store'
import groupsStore from '../store/groups.store'
import Group from '@/modules/groups/models/group.model'

@Component({ components: { BaseDialog } })
export default class GroupAddProjectDialog extends Mixins(DialogMixin) {
  private form: { projectId: number | null } = {
    projectId: null,
  }
  private group: Group | null = null

  private get groupId(): number {
    return parseInt(this.$route.params.groupId)
  }

  private get projectsToAdd(): { value: string | undefined; id: number | undefined }[] {
    const allProjects = this.projects
    const existingProjects = this.existingProjects
    return allProjects.filter((project) => {
      return !existingProjects.find(
        (existingProject: { value: string | undefined; id: number | undefined }) =>
          existingProject.id === project.id
      )
    })
  }

  private get existingProjects(): { value: string | undefined; id: number | undefined }[] {
    if (!this.group) return []
    return this.group.projects.map((project) => {
      return {
        value: project.name,
        id: project.id,
      }
    })
  }

  protected async mounted(): Promise<void> {
    this.visible = true
    await this.searchProjects()
    this.group = await groupsStore.get(this.groupId)
    setTimeout(() => (this.$refs.input as Input).focus(), 150)
  }

  public async submit(): Promise<void> {
    if (!this.form.projectId) return
    const projectId: number = this.form.projectId
    if (projectId) {
      this.loading = true
      await groupsStore.addProjects({
        groupId: this.groupId,
        projectIds: [projectId],
      })
      tableStore.requireReload()
      this.$emit('submit')
      this.loading = false
    }
    this.exit()
  }
}
</script>
