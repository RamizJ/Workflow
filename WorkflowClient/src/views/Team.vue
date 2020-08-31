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
      </div>
      <div class="header__action">
        <el-dropdown placement="bottom" :show-timeout="0">
          <el-button type="text" size="mini">
            <feather type="chevron-down" size="22"></feather>
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

import teamsModule from '@/store/modules/teams.module'
import TeamUsers from '@/components/Team/TeamUsers.vue'
import TeamProjects from '@/components/Team/TeamProjects.vue'
import TeamDialog from '@/components/Team/TeamDialog.vue'
import TeamAddUserDialog from '@/components/Team/TeamAddUserDialog.vue'
import UserDialog from '@/components/User/UserDialog.vue'
import Team from '@/types/team.type'
import UserTable from '@/components/User/UserTable.vue'

@Component({
  components: {
    TeamUsers,
    TeamProjects,
    TeamDialog,
    TeamAddUserDialog,
    UserDialog
  }
})
export default class TeamPage extends Vue {
  private loading = true
  private searchQuery = ''
  private teamItem: Team = {
    name: '',
    description: '',
    userIds: [],
    projectIds: []
  }
  private activeTab = 'members'
  private tabs = [
    { value: 'members', label: 'Участники' },
    { value: 'projects', label: 'Проекты' }
  ]
  private dialogTeamVisible = false
  private dialogUserVisible = false
  private dialogAddUserVisible = false

  private get id(): number {
    return parseInt(this.$route.params.teamId)
  }

  private async mounted() {
    this.loading = true
    this.loadTab()
    const team = await teamsModule.findOneById(this.id)
    this.teamItem = { ...team }
    this.loading = false
  }

  loadTab() {
    const query = { ...this.$route.query }
    query.tab = query.tab?.toString() || this.activeTab
    this.activeTab = query.tab
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) this.$router.push({ query })
  }

  setTab() {
    const query = { tab: this.activeTab }
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) this.$router.push({ query })
  }

  async onUserAdd() {
    this.dialogAddUserVisible = true
  }

  onUserAdded() {
    const teamUsers = this.$refs.teamUsers as TeamUsers
    const userTable = teamUsers.$refs.items as UserTable
    userTable.reloadData()
  }

  async onUserCreate() {
    const teamUsers = this.$refs.teamUsers as TeamUsers
    if (teamUsers) (teamUsers.$refs.items as UserTable).createEntity()
    else this.dialogUserVisible = true
  }

  async onTeamEdit() {
    this.dialogTeamVisible = true
  }

  async onTeamUpdate() {
    await teamsModule.updateOne(this.teamItem)
  }

  async onTeamDelete() {
    await teamsModule.deleteOne(this.id)
    await this.$router.push({ name: 'Team' })
  }
}
</script>
