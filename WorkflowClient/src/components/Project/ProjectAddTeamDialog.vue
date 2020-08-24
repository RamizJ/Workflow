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
import BaseDialog from '@/components/BaseDialog.vue'
import Team from '@/types/team.type'

@Component({
  components: {
    BaseDialog
  }
})
export default class ProjectAddTeamDialog extends mixins(DialogMixin) {
  public form = {
    teamId: ''
  }

  private get teamsToAdd(): { id: number | undefined; value: string }[] {
    const allTeams = this.teamList
    const existingTeams = this.existingTeams
    return allTeams.filter(team => {
      return !existingTeams.find(existingTeam => existingTeam.id === team.id)
    })
  }
  private get existingTeams(): { id: number | undefined; value: string }[] {
    const teams: Team[] = projectsModule.projectTeams
    return teams.map((team: Team) => {
      return {
        id: team.id,
        value: team.name
      }
    })
  }

  public async submit() {
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
