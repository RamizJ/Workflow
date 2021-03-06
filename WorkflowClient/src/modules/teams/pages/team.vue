<template>
  <BasePage>
    <BasePageHeader>
      <input
        v-if="!loading"
        v-model="teamItem.name"
        v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
        @change="onTeamUpdate"
      />
      <TeamActions
        slot="action"
        v-if="!loading"
        @add-user="onUserAdd"
        @delete-team="onTeamDelete"
      />
    </BasePageHeader>
    <BasePageSubheader>
      <input
        placeholder="Описание..."
        v-model="teamItem.description"
        v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
        @change="onTeamUpdate"
      />
    </BasePageSubheader>
    <BasePageSubheader :no-border="true">
      <BaseTabs v-model="currentTab" :tabs="tabs" @tab-click="setTab" :routing="true" />
    </BasePageSubheader>

    <RouterView />

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
  </BasePage>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { MessageBox } from 'element-ui'
import teamsModule from '@/modules/teams/store/teams.store'
import settingsModule from '@/modules/settings/store/settings.store'
import BasePage from '@/core/components/base-page/base-page.vue'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import BasePageSubheader from '@/core/components/base-page/base-page-subheader.vue'
import BaseTabs from '@/core/components/base-tabs/base-tabs.vue'
import TeamUsers from '@/modules/teams/components/team-users.vue'
import TeamProjects from '@/modules/teams/components/team-projects.vue'
import TeamDialog from '@/modules/teams/components/team-dialog.vue'
import TeamAddUserDialog from '@/modules/teams/components/team-add-user-dialog.vue'
import UserDialog from '@/modules/users/components/user-dialog.vue'
import UserTable from '@/modules/users/components/user-table-old.vue'
import TeamActions from '@/modules/teams/components/team-actions.vue'
import Team from '@/modules/teams/models/team.type'

@Component({
  components: {
    BaseTabs,
    BasePageSubheader,
    TeamActions,
    BasePageHeader,
    BasePage,
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
  private currentTab = 'users'
  private tabs: Array<{ label: string; name: string; component?: any }> = [
    { label: 'Участники', name: 'users' },
    { label: 'Проекты', name: 'projects' },
  ]
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
    const tab = this.$route.path.substring(this.$route.path.lastIndexOf('/') + 1)
    switch (tab) {
      case 'projects':
      case 'users':
        this.currentTab = tab
        break
      default:
        this.currentTab = 'users'
        if (this.$route.name !== 'team-users') this.$router.replace({ name: `team-users` })
        break
    }
  }

  private setTab() {
    const tab = this.$route.path.substring(this.$route.path.lastIndexOf('/') + 1)
    let path = this.$route.path

    switch (tab) {
      case 'projects':
      case 'users':
        path = path.replace(tab, this.currentTab)
        break
      default:
        path = this.currentTab === 'users' ? path : `${path}/${this.currentTab}`
        break
    }

    const targetRoute = `team-${this.currentTab}`
    if (this.$route.name !== targetRoute) this.$router.replace({ name: targetRoute })
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
