<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
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
            <el-checkbox v-model="form.canEditTasks">Изменение задач</el-checkbox>
          </el-form-item>
          <el-form-item>
            <el-checkbox v-model="form.canCloseTasks">Завершение задач</el-checkbox>
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
            <feather type="arrow-right"></feather>
          </el-button>
        </el-tooltip>
      </div>
    </template>
  </base-dialog>
</template>

<script lang="ts">
import { Component, Mixins, Prop } from 'vue-property-decorator'
import DialogMixin from '@/mixins/dialog.mixin'
import BaseDialog from '@/components/BaseDialog.vue'
import teamsModule from '@/store/modules/teams.module'
import User from '@/types/user.type'

@Component({
  components: { BaseDialog },
})
export default class ProjectEditUserRightsDialog extends Mixins(DialogMixin) {
  @Prop() readonly user!: User
  @Prop() readonly teamId!: number

  private form = {
    teamId: 0,
    userId: '',
    canEditUsers: false,
    canEditTasks: true,
    canCloseTasks: true,
  }

  protected mounted(): void {
    this.visible = true
    this.form.teamId = this.teamId
    this.form.userId = this.user.id || ''
    this.form.canEditUsers = !!this.user.canEditUsers
    this.form.canEditTasks = !!this.user.canEditGoals
    this.form.canCloseTasks = !!this.user.canCloseGoals
  }

  public async submit(): Promise<void> {
    this.loading = true
    await teamsModule.addUser({
      teamId: this.form.teamId,
      userId: this.form.userId,
      canEditUsers: this.form.canEditUsers,
      canEditTasks: this.form.canEditTasks,
      canCloseTasks: this.form.canCloseTasks,
    })
    this.$emit('submit')
    this.loading = false
    this.exit()
  }
}
</script>

<style scoped></style>
