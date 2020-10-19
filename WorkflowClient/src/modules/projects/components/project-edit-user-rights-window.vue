<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog" append-to-body>
    <h1 slot="title">Права пользователя</h1>
    <el-form
      slot="body"
      :model="form"
      ref="form"
      v-loading="loading"
      @submit.native.prevent="submit"
    >
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item v-if="form.userId">
            <el-checkbox v-model="form.canEditGoals">Изменение задач</el-checkbox>
          </el-form-item>
          <el-form-item>
            <el-checkbox v-model="form.canCloseGoals">Завершение задач</el-checkbox>
          </el-form-item>
          <el-form-item>
            <el-checkbox v-model="form.canEditUsers">Изменение пользователей</el-checkbox>
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
import { Component, Mixins, Prop } from 'vue-property-decorator'
import DialogMixin from '@/core/mixins/dialog.mixin'
import BaseDialog from '@/core/components/BaseDialog.vue'
import projectsModule from '@/modules/projects/store/projects.store'
import User from '@/modules/users/models/user.type'
import { UserRole } from '@/modules/users/models/user-role.type'

@Component({
  components: { BaseDialog },
})
export default class ProjectEditUserRightsDialog extends Mixins(DialogMixin) {
  @Prop() readonly user!: User
  @Prop() readonly teamId!: number

  private form: UserRole = {
    projectId: 0,
    userId: '',
    canEditUsers: false,
    canEditGoals: false,
    canCloseGoals: false,
  }

  private get userId(): string {
    return this.user.id || ''
  }

  private get projectId(): number {
    return parseInt(this.$route.params.projectId)
  }

  protected async mounted(): Promise<void> {
    this.visible = true
    this.loading = true
    if (this.user.id) {
      try {
        this.form = await projectsModule.getUserRole({
          projectId: this.projectId,
          userId: this.userId,
        })
      } catch (e) {
        this.$message.error(`Не удалось загрузить права пользователя`)
        this.loading = false
      }
    }
    this.loading = false
  }

  public async submit(): Promise<void> {
    this.loading = true
    this.form.projectId = this.projectId
    this.form.userId = this.userId
    await projectsModule.updateUserRole(this.form)
    this.$emit('submit')
    this.loading = false
    this.exit()
  }
}
</script>

<style scoped></style>
