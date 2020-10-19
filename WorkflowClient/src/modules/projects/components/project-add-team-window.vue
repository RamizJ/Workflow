<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Добавить команду</h1>
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
              v-model="form.teamId"
              placeholder="Найти команду..."
              :remote-method="searchTeams"
              filterable="filterable"
              remote="remote"
              clearable="clearable"
              default-first-option="default-first-option"
            >
              <el-option
                v-for="item in teamsToAdd"
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

import projectsModule from '@/modules/projects/store/projects.store'
import DialogMixin from '@/core/mixins/dialog.mixin'
import BaseDialog from '@/core/components/BaseDialog.vue'
import Team from '@/modules/teams/models/team.type'
import { Input } from 'element-ui'

@Component({
  components: {
    BaseDialog,
  },
})
export default class ProjectAddTeamDialog extends Mixins(DialogMixin) {
  private form = {
    teamId: '',
  }

  private get teamsToAdd(): { id: number | undefined; value: string | undefined }[] {
    const allTeams = this.teams
    const existingTeams = this.existingTeams
    return allTeams.filter((team) => {
      return !existingTeams.find((existingTeam) => existingTeam.id === team.id)
    })
  }
  private get existingTeams(): { id: number | undefined; value: string | undefined }[] {
    const teams: Team[] = projectsModule.project?.teams || []
    return teams.map((team: Team) => {
      return {
        id: team.id,
        value: team.name,
      }
    })
  }

  protected async mounted(): Promise<void> {
    this.visible = true
    await this.searchTeams()
    setTimeout(() => (this.$refs.input as Input).focus(), 150)
  }

  public async submit(): Promise<void> {
    const projectId = parseInt(this.$route.params.projectId)
    const teamId = parseInt(this.form.teamId)
    if (teamId) {
      this.loading = true
      await projectsModule.addTeam({ projectId, teamId })
      this.$emit('submit')
      this.loading = false
    }
    this.exit()
  }
}
</script>
