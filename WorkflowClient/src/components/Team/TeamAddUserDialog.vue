<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Добавить участника</h1>
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
              v-model="form.userId"
              placeholder="Найти участника..."
              :remote-method="searchUsers"
              filterable="filterable"
              remote="remote"
              clearable="clearable"
              default-first-option="default-first-option"
            >
              <el-option
                v-for="item in usersToAdd"
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
import teamsModule from '@/store/modules/teams.module'
import DialogMixin from '@/mixins/dialog.mixin'
import BaseDialog from '@/components/BaseDialog.vue'
import Query from '@/types/query.type'

@Component({ components: { BaseDialog } })
export default class TeamAddUserDialog extends Mixins(DialogMixin) {
  private form = {
    userId: '',
  }

  private get usersToAdd(): { value: string | undefined; id: string | undefined }[] {
    const allUsers = this.users
    const existingUsers = this.existingUsers
    return allUsers.filter((user) => {
      return !existingUsers.find(
        (existingUser: { value: string | undefined; id: string | undefined }) =>
          existingUser.id === user.id
      )
    })
  }
  private get existingUsers(): { value: string | undefined; id: string | undefined }[] {
    return teamsModule.teamUsers.map((user) => {
      return {
        value: `${user.lastName} ${user.firstName}`,
        id: user.id,
      }
    })
  }

  protected async mounted(): Promise<void> {
    this.visible = true
    await this.searchUsers()
    await teamsModule.findUsers({
      teamId: parseInt(this.$route.params.teamId),
      pageNumber: 0,
      pageSize: 100,
    } as Query)
    setTimeout(() => (this.$refs.input as Input).focus(), 150)
  }

  public async submit(): Promise<void> {
    const userId = this.form.userId.trim()
    if (userId) {
      this.loading = true
      await teamsModule.addUser({
        teamId: parseInt(this.$route.params.teamId),
        userId: userId,
      })
      this.$emit('submit')
      this.loading = false
    }
    this.exit()
  }
}
</script>
