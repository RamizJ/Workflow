<template>
  <base-dialog class="project-team-users-dialog" v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Участники команды</h1>
    <project-team-users slot="body" :team-id="teamId" />
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
          <!--          <el-button type="text" @click="submit" circle="circle">-->
          <!--            <feather type="arrow-right"></feather>-->
          <!--          </el-button>-->
        </el-tooltip>
      </div>
    </template>
  </base-dialog>
</template>

<script lang="ts">
import { Component, Mixins, Prop } from 'vue-property-decorator'
import DialogMixin from '@/core/mixins/dialog.mixin'
import ProjectTeamUsers from '@/modules/projects/components/project-team-users.vue'
import projectsModule from '@/modules/projects/store/projects.store'
import BaseDialog from '@/core/components/BaseDialog.vue'

@Component({
  components: { ProjectTeamUsers, BaseDialog },
})
export default class ProjectTeamUsersDialog extends Mixins(DialogMixin) {
  @Prop() readonly teamId!: number

  protected async mounted(): Promise<void> {
    this.visible = true
    this.loading = true
    this.loading = false
  }
}
</script>

<style lang="scss">
.project-team-users-dialog {
  .el-dialog {
    margin-top: 10vh !important;
    width: 85%;
    //margin: 0 10px;
    height: 500px;
  }
  .el-dialog__body {
    height: 400px;
    .body {
      height: 100%;
    }
  }
}
</style>
