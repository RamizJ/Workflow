<template lang="pug">
  page(v-loading="loading")
    base-header
      template(slot="title")
        input.title(
          v-model="teamItem.name"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="onTeamUpdate")
      template(slot="action")
        el-dropdown(placement="bottom" :show-timeout="0")
          el-button(type="text" size="mini")
            feather(type="chevron-down" size="22")
          el-dropdown-menu(slot="dropdown")
            el-dropdown-item
              el-button(type="text" size="mini" @click="onUserAdd") Добавить участника
            el-divider
            el-dropdown-item
              el-button(type="text" size="mini" @click="onTeamEdit") Редактировать
            el-dropdown-item
              el-button(type="text" size="mini" @click="onTeamDelete") Переместить в корзину

    el-tabs(v-if="teamItem.id" ref="tabs" v-model="activeTab" @tab-click="setTab")
      el-tab-pane(name="members" label="Участники")
        team-users(v-if="activeTab === 'members'" ref="teamUsers")
      el-tab-pane(name="projects" label="Проекты")
        team-projects(v-if="activeTab === 'projects'" ref="teamProjects")

    team-dialog(v-if="dialogTeamVisible" :data="teamItem" @close="dialogTeamVisible = false")
    team-add-user-dialog(v-if="dialogAddUserVisible" @close="dialogAddUserVisible = false" @submit="onUserAdded")
    user-dialog(v-if="dialogUserVisible" @close="dialogUserVisible = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '@/components/Page';
import BaseHeader from '@/components/BaseHeader';
import TeamUsers from '@/components/Team/TeamUsers';
import UserDialog from '@/components/User/UserDialog';
import TeamDialog from '@/components/Team/TeamDialog';
import TeamProjects from '@/components/Team/TeamProjects';
import TeamAddUserDialog from '@/components/Team/TeamAddUserDialog';

export default {
  name: 'Team',
  components: {
    TeamAddUserDialog,
    Page,
    BaseHeader,
    TeamUsers,
    TeamProjects,
    TeamDialog,
    UserDialog
  },
  data() {
    return {
      loading: true,
      searchQuery: '',
      teamItem: {
        name: '',
        description: '',
        userIds: [],
        projectIds: []
      },
      activeTab: 'members',
      tabs: [
        { value: 'members', label: 'Участники' },
        { value: 'projects', label: 'Проекты' }
      ],
      dialogTeamVisible: false,
      dialogUserVisible: false,
      dialogAddUserVisible: false
    };
  },
  computed: {
    ...mapGetters({
      team: 'teams/getTeam'
    })
  },
  async mounted() {
    this.loading = true;
    this.loadTab();
    await this.fetchTeam(this.$route.params.teamId);
    this.teamItem = { ...this.team };
    this.loading = false;
  },
  methods: {
    ...mapActions({
      fetchTeam: 'teams/fetchTeam',
      updateTeam: 'teams/updateTeam',
      deleteTeam: 'teams/deleteTeam'
    }),
    loadTab() {
      const query = { ...this.$route.query };
      query.tab = query.tab || this.activeTab;
      this.activeTab = query.tab;
      if (JSON.stringify(query) !== JSON.stringify(this.$route.query))
        this.$router.push({ query });
    },
    setTab() {
      const query = {};
      query.tab = this.activeTab;
      if (JSON.stringify(query) !== JSON.stringify(this.$route.query))
        this.$router.push({ query });
    },
    async onUserAdd(e) {
      this.dialogAddUserVisible = true;
    },
    onUserAdded() {
      this.$refs.teamUsers?.$refs.items.refresh();
    },
    async onUserCreate(e) {
      if (this.$refs.teamUsers) this.$refs.teamUsers.$refs.items.onItemCreate();
      else this.dialogUserVisible = true;
    },
    async onTeamEdit(e) {
      this.dialogTeamVisible = true;
    },
    async onTeamUpdate(e) {
      await this.updateTeam(this.teamItem);
    },
    async onTeamDelete() {
      await this.deleteTeam(this.teamItem.id);
      await this.$router.push({ name: 'Team' });
    }
  }
};
</script>

<style lang="scss" scoped></style>
