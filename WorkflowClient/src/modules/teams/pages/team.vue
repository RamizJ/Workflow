<template>
  <div class="page">
    <div class="header">
      <div class="header_title">
        <input
          class="title"
          v-model="teamItem.name"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="onTeamUpdate"
        />
        <div class="header__action">
          <el-dropdown placement="bottom" :show-timeout="0">
            <el-button type="text" size="mini">
              <unicon name="ellipsis-h" />
            </el-button>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item>
                <el-button type="text" size="mini" @click="onUserAdd">Добавить участника</el-button>
              </el-dropdown-item>
              <el-divider></el-divider>
              <el-dropdown-item>
                <el-button type="text" size="mini" @click="onTeamDelete"
                  >Переместить в корзину</el-button
                >
              </el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
        </div>
      </div>
      <div class="header__subtitle">
        <input
          class="subtitle"
          placeholder="Описание"
          v-model="teamItem.description"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="onTeamUpdate"
        />
      </div>
    </div>
    <el-tabs v-if="teamItem.id" ref="tabs" v-model="activeTab" @tab-click="setTab">
      <el-tab-pane name="members" label="Участники">
        <team-users v-if="activeTab === 'members'" ref="teamUsers"></team-users>
      </el-tab-pane>
      <el-tab-pane name="projects" label="Проекты">
        <team-projects v-if="activeTab === 'projects'" ref="teamProjects"></team-projects>
      </el-tab-pane>
    </el-tabs>

    <team-dialog
      v-if="dialogTeamVisible"
      :data="teamItem"
      @close="dialogTeamVisible = false"
    ></team-dialog>
    <team-add-user-dialog
      v-if="dialogAddUserVisible"
      @close="dialogAddUserVisible = false"
      @submit="onUserAdded"
    ></team-add-user-dialog>
    <user-dialog v-if="dialogUserVisible" @close="dialogUserVisible = false"></user-dialog>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { MessageBox } from 'element-ui'

import teamsModule from '@/modules/teams/store/teams.store'
import settingsModule from '@/modules/settings/store/settings.store'
import TeamUsers from '@/modules/teams/components/team-users.vue'
import TeamProjects from '@/modules/teams/components/team-projects.vue'
import TeamDialog from '@/modules/teams/components/team-dialog.vue'
import TeamAddUserDialog from '@/modules/teams/components/team-add-user-dialog.vue'
import UserDialog from '@/modules/users/components/user-dialog.vue'
import Team from '@/modules/teams/models/team.type'
import UserTable from '@/modules/users/components/user-table.vue'

@Component({
  components: {
    TeamUsers,
    TeamProjects,
    TeamDialog,
    TeamAddUserDialog,
    UserDialog,
  },
})
export default class TeamPage extends Vue {
  private loading = true
  private teamItem: Team = {
    name: '',
    description: '',
    userIds: [],
    projectIds: [],
  }
  private activeTab = 'members'
  private dialogTeamVisible = false
  private dialogUserVisible = false
  private dialogAddUserVisible = false

  private get id(): number {
    return parseInt(this.$route.params.teamId)
  }

  protected async mounted(): Promise<void> {
    this.loading = true
    this.loadTab()
    const team = await teamsModule.findOneById(this.id)
    this.teamItem = { ...team }
    this.loading = false
  }

  private loadTab() {
    const query = { ...this.$route.query }
    query.tab = query.tab?.toString() || this.activeTab
    this.activeTab = query.tab
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) this.$router.replace({ query })
  }

  private setTab() {
    const query = { tab: this.activeTab }
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) this.$router.replace({ query })
  }

  private async onUserAdd() {
    this.dialogAddUserVisible = true
  }

  private onUserAdded() {
    const teamUsers = this.$refs.teamUsers as TeamUsers
    const userTable = teamUsers.$refs.items as UserTable
    userTable.reloadData()
  }

  private async onUserCreate() {
    const teamUsers = this.$refs.teamUsers as TeamUsers
    if (teamUsers) (teamUsers.$refs.items as UserTable).createEntity()
    else this.dialogUserVisible = true
  }

  private async onTeamEdit() {
    this.dialogTeamVisible = true
  }

  private async onTeamUpdate() {
    await teamsModule.updateOne(this.teamItem)
  }

  private async onTeamDelete() {
    const allowDelete = await this.confirmDelete()
    if (!allowDelete) return
    await teamsModule.deleteOne(this.id)
    await this.$router.push({ name: 'Team' })
  }

  protected async confirmDelete(): Promise<boolean> {
    if (!settingsModule.confirmDelete) return true
    try {
      await MessageBox.confirm('Вы действительно хотите удалить элемент?', 'Предупреждение', {
        confirmButtonText: 'Удалить',
        cancelButtonText: 'Отменить',
        type: 'warning',
      })
      return true
    } catch (e) {
      return false
    }
  }
}
</script>
